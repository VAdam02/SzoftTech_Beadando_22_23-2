using Model.Persons;
using Model.RoadGrids;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Tiles
{
	public class RoadTile : Tile, IRoadGridElement
	{
		#region Tile implementation
		public override TileType GetTileType() => TileType.Road;

		public override void FinalizeTile() => Finalizing();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Finalizing()</code></para>
		/// <para>Do the actual finalization</para>
		/// </summary>
		protected new void Finalizing()
		{
			RegisterNeighbourTileDeleteListeners();

			base.Finalizing();
		}

		public override void DeleteTile() => Deleting();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Deleting()</code></para>
		/// <para>Do the deletion administration</para>
		/// </summary>
		protected new void Deleting()
		{
			base.Deleting();

			while (_lockedBy.Count > 0)
			{
				_lockedBy[0].ForcedLockedRoadDestroy();
			}
		}

		public override int BuildPrice => 500;

		public override float Transparency => 1;

		#endregion

		#region IRoadGridElement implementation
		void IRoadGridElement.RegisterRoadGridElement()
		{
			if (!_isFinalized) { throw new InvalidOperationException("Tile is not set in the city"); }

			RoadGridManager.Instance.AddRoadGridElement(this);
		}

		void IRoadGridElement.UnregisterRoadGridElement()
		{
			if (!_isFinalized) { throw new InvalidOperationException("Tile is not set in the city"); }

			SetRoadGrid(null);
		}

		private readonly IRoadGridElement[] _roads = new IRoadGridElement[4];
		List<IRoadGridElement> IRoadGridElement.ConnectsTo { get => _roads.Where(x => x != null).ToList(); }

		private void RegisterNeighbourTileDeleteListeners()
		{
			if (City.Instance.GetTile(Coordinates.x, Coordinates.y - 1) is RoadTile aboveRoad) { ConnectsFromAbove = aboveRoad; }
			if (City.Instance.GetTile(Coordinates.x + 1, Coordinates.y) is RoadTile rightRoad) { ConnectsFromRight = rightRoad; }
			if (City.Instance.GetTile(Coordinates.x, Coordinates.y + 1) is RoadTile belowRoad) { ConnectsFromBelow = belowRoad; }
			if (City.Instance.GetTile(Coordinates.x - 1, Coordinates.y) is RoadTile leftRoad)  { ConnectsFromLeft  = leftRoad;  }

			if (City.Instance.GetTile(Coordinates.x, Coordinates.y - 1) is Tile aboveTile) { aboveTile.OnTileDelete += TileDeleteHandler; }
			if (City.Instance.GetTile(Coordinates.x + 1, Coordinates.y) is Tile rightTile) { rightTile.OnTileDelete += TileDeleteHandler; }
			if (City.Instance.GetTile(Coordinates.x, Coordinates.y + 1) is Tile belowTile) { belowTile.OnTileDelete += TileDeleteHandler; }
			if (City.Instance.GetTile(Coordinates.x - 1, Coordinates.y) is Tile leftTile)  { leftTile.OnTileDelete  += TileDeleteHandler; }
		}

		private void TileDeleteHandler(object sender, Tile oldTile)
		{
			Tile newTile = City.Instance.GetTile(oldTile);
			newTile.OnPreTileDelete += TileDeleteHandler;

			if (newTile is RoadTile road)
			{
				if (oldTile.Coordinates.x < Coordinates.x)		{ ConnectsFromLeft  = road; }
				else if (oldTile.Coordinates.x > Coordinates.x) { ConnectsFromRight = road; }
				else if (oldTile.Coordinates.y > Coordinates.y) { ConnectsFromBelow = road; }
				else if (oldTile.Coordinates.y < Coordinates.y) { ConnectsFromAbove = road; }
			}
			else
			{
				if (oldTile.Coordinates.x < Coordinates.x)		{ ConnectsFromLeft  = null; }
				else if (oldTile.Coordinates.x > Coordinates.x) { ConnectsFromRight = null; }
				else if (oldTile.Coordinates.y > Coordinates.y) { ConnectsFromBelow = null; }
				else if (oldTile.Coordinates.y < Coordinates.y) { ConnectsFromAbove = null; }
			}
		}

		public IRoadGridElement ConnectsFromAbove
		{
			get { return _roads[0]; }
			private set
			{
				_roads[0] = value;
				if (value == null) { DesignID &= ~ABOVEROADMASK; }
				else { DesignID |= ABOVEROADMASK; }
			}
		}
		public IRoadGridElement ConnectsFromRight
		{
			get { return _roads[1]; }
			private set
			{
				_roads[1] = value;
				if (value == null) { DesignID &= ~RIGHTROADMASK; }
				else { DesignID |= RIGHTROADMASK; }
			}
		}
		public IRoadGridElement ConnectsFromBelow
		{
			get { return _roads[2]; }
			private set
			{
				_roads[2] = value;
				if (value == null) { DesignID &= ~BELOWROADMASK; }
				else { DesignID |= BELOWROADMASK; }
			}
		}
		public IRoadGridElement ConnectsFromLeft
		{
			get { return _roads[3]; }
			private set
			{
				_roads[3] = value;
				if (value == null) { DesignID &= ~LEFTROADMASK; }
				else { DesignID |= LEFTROADMASK; }
			}
		}

		private RoadGrid _roadGrid = null;

		RoadGrid IRoadGridElement.RoadGrid
		{
			get => _roadGrid;
			set
			{
				SetRoadGrid(value);
			}
		}

		/// <summary>
		/// Sets the new roadgrid
		/// </summary>
		/// <param name="roadGrid">New roadgrid</param>
		private void SetRoadGrid(RoadGrid roadGrid)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			if (_roadGrid == roadGrid) { return; }

			List<Building> buildings = RoadGridManager.GetBuildingsByRoadGridElement(this);
			foreach (Building building in buildings)
			{
				if (building is IWorkplace workplace)
				{
					workplace.UnregisterWorkplace(_roadGrid);
				}
				if (building is IResidential residential)
				{
					residential.UnregisterResidential(_roadGrid);
				}
			}

			if (roadGrid == null)
			{
				_roadGrid?.RemoveRoadGridElement(this);
				_roadGrid?.Reinit();
			}
			else
			{
				_roadGrid?.RemoveRoadGridElement(this);
				_roadGrid = roadGrid;
				_roadGrid?.AddRoadGridElement(this);
			}

			if (_roadGrid != null)
			{
				foreach (Building building in buildings)
				{
					if (building is IWorkplace workplace)
					{
						workplace.RegisterWorkplace(_roadGrid);
					}
					if (building is IResidential residential)
					{
						residential.RegisterResidential(_roadGrid);
					}
				}
			}
		}

		private readonly List<Person> _lockedBy = new();
		public bool IsLocked { get => _lockedBy.Count != 0; }
		public void LockBy(Person person) { _lockedBy.Add(person); }
		public void UnlockBy(Person person) { _lockedBy.Remove(person); }
		#endregion

		#region Common implementation
		public Tile GetTile() => this;
		#endregion

		public const uint ABOVEROADMASK = 1;
		public const uint RIGHTROADMASK = 2;
		public const uint BELOWROADMASK = 4;
		public const uint LEFTROADMASK  = 8;

		/// <summary>
		/// Construct a new road tile
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		public RoadTile(int x, int y) : base(x, y, 0)
		{

		}
	}
}
