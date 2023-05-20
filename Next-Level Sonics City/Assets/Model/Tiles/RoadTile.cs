using Model.RoadGrids;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Tiles
{
	public class RoadTile : Tile, IRoadGridElement
	{
		#region Tile implementation
		public override TileType GetTileType() { return TileType.Road; }

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
		}

		public override int GetBuildPrice()
		{
			//TODO implement road build price
			return 100000;
		}

		public override int GetDestroyIncome()
		{
			//TODO implement road destroy income
			return 100000;
		}

		public override int GetMaintainanceCost()
		{
			//TODO implement road maintainance cost
			return 100000;
		}

		public override float GetTransparency()
		{
			return 1;
		}
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
			newTile.OnTileDelete += TileDeleteHandler;

			if (newTile is RoadTile road)
			{
				if (oldTile.Coordinates.x < Coordinates.x)		{ ConnectsFromLeft = road; }
				else if (oldTile.Coordinates.x > Coordinates.x) { ConnectsFromRight = road; }
				else if (oldTile.Coordinates.y > Coordinates.y) { ConnectsFromBelow = road; }
				else if (oldTile.Coordinates.y < Coordinates.y) { ConnectsFromAbove = road; }
			}
			else
			{
				if (oldTile.Coordinates.x < Coordinates.x)		{ ConnectsFromLeft = null; }
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
		#endregion

		#region Common implementation
		public Tile GetTile() { return this; }
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
