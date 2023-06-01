using Model.ElectricGrids;
using Model.RoadGrids;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Tiles
{
	public class ElectricRoadTile : Tile, IRoadGridElement, IElectricGridElement
	{
		#region Tile implementation
		public override TileType GetTileType() => TileType.ElectricRoad;

		public override void FinalizeTile() => Finalizing();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Finalizing()</code></para>
		/// <para>Do the actual finalization</para>
		/// </summary>
		protected new void Finalizing()
		{
			base.Finalizing();

			RegisterNeighbourTileDeleteListeners();
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

		IRoadGridElement IRoadGridElement.ConnectsFromAbove
		{
			get { return _roads[0]; }
		}
		private IRoadGridElement ConnectsFromAboveRoad
		{
			set
			{
				_roads[0] = value;
				if (value == null) { DesignID &= ~ABOVEROADMASK; }
				else { DesignID |= ABOVEROADMASK; }
			}
		}
		IRoadGridElement IRoadGridElement.ConnectsFromRight
		{
			get { return _roads[1]; }
		}
		private IRoadGridElement ConnectsFromRightRoad
		{
			set
			{
				_roads[1] = value;
				if (value == null) { DesignID &= ~RIGHTROADMASK; }
				else { DesignID |= RIGHTROADMASK; }
			}
		}
		IRoadGridElement IRoadGridElement.ConnectsFromBelow
		{
			get { return _roads[2]; }
		}
		private IRoadGridElement ConnectsFromBelowRoad
		{
			set
			{
				_roads[2] = value;
				if (value == null) { DesignID &= ~BELOWROADMASK; }
				else { DesignID |= BELOWROADMASK; }
			}
		}
		IRoadGridElement IRoadGridElement.ConnectsFromLeft
		{
			get { return _roads[3]; }
		}
		private IRoadGridElement ConnectsFromLeftRoad
		{
			set
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

		#region IElectricGridElement implementation
		void IElectricGridElement.RegisterElectricGridElement()
		{
			if (!_isFinalized) { throw new InvalidOperationException("Tile is not set in the city"); }

			ElectricGridManager.Instance.AddElectricGridElement(this);
		}

		void IElectricGridElement.UnregisterElectricGridElement()
		{
			if (!_isFinalized) { throw new InvalidOperationException("Tile is not set in the city"); }

			SetElectricGrid(null);
		}

		private readonly IElectricGridElement[] _electrics = new IElectricGridElement[4];
		List<IElectricGridElement> IElectricGridElement.ConnectsTo { get => _electrics.Where(x => x != null).ToList(); }

		IElectricGridElement IElectricGridElement.ConnectsFromAbove
		{
			get { return _electrics[0]; }
		}
		private IElectricGridElement ConnectsFromAboveElectric { set => _electrics[0] = value; }
		IElectricGridElement IElectricGridElement.ConnectsFromRight
		{
			get { return _electrics[1]; }
		}
		private IElectricGridElement ConnectsFromRightElectric { set => _electrics[1] = value; }
		IElectricGridElement IElectricGridElement.ConnectsFromBelow
		{
			get { return _electrics[2]; }
		}
		private IElectricGridElement ConnectsFromBelowElectric { set => _electrics[2] = value; }
		IElectricGridElement IElectricGridElement.ConnectsFromLeft
		{
			get { return _electrics[3]; }
		}
		private IElectricGridElement ConnectsFromLeftElectric { set => _electrics[3] = value; }

		private ElectricGrid _electricGrid = null;

		ElectricGrid IElectricGridElement.ElectricGrid
		{
			get => _electricGrid;
			set
			{
				SetElectricGrid(value);
			}
		}

		/// <summary>
		/// Sets the new electricgrid
		/// </summary>
		/// <param name="electricGrid">New electricgrid</param>
		private void SetElectricGrid(ElectricGrid electricGrid)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			if (_electricGrid == electricGrid) { return; }

			{
				if (this is IPowerProducer producer)
				{
					producer.UnregisterPowerProducer(_electricGrid);
				}
				if (this is IPowerConsumer consumer)
				{
					consumer.UnregisterPowerConsumer(_electricGrid);
				}
			}

			if (electricGrid == null)
			{
				_electricGrid?.RemoveElectricGridElement(this);
				_electricGrid?.Reinit();
			}
			else
			{
				_electricGrid?.RemoveElectricGridElement(this);
				_electricGrid = electricGrid;
				_electricGrid?.AddElectricGridElement(this);
			}

			if (_electricGrid != null)
			{
				if (this is IPowerProducer producer)
				{
					producer.RegisterPowerProducer(_electricGrid);
				}
				if (this is IPowerConsumer consumer)
				{
					consumer.RegisterPowerConsumer(_electricGrid);
				}
			}
		}
		#endregion

		#region Common implementation
		public Tile GetTile() => this;

		private void RegisterNeighbourTileDeleteListeners()
		{
			if (City.Instance.GetTile(Coordinates.x, Coordinates.y - 1) is IRoadGridElement aboveRoad) { ConnectsFromAboveRoad = aboveRoad; }
			if (City.Instance.GetTile(Coordinates.x + 1, Coordinates.y) is IRoadGridElement rightRoad) { ConnectsFromRightRoad = rightRoad; }
			if (City.Instance.GetTile(Coordinates.x, Coordinates.y + 1) is IRoadGridElement belowRoad) { ConnectsFromBelowRoad = belowRoad; }
			if (City.Instance.GetTile(Coordinates.x - 1, Coordinates.y) is IRoadGridElement leftRoad) { ConnectsFromLeftRoad = leftRoad; }

			if (City.Instance.GetTile(Coordinates.x, Coordinates.y - 1) is IElectricGridElement aboveElectric) { ConnectsFromAboveElectric = aboveElectric; }
			if (City.Instance.GetTile(Coordinates.x + 1, Coordinates.y) is IElectricGridElement rightElectric) { ConnectsFromRightElectric = rightElectric; }
			if (City.Instance.GetTile(Coordinates.x, Coordinates.y + 1) is IElectricGridElement belowElectric) { ConnectsFromBelowElectric = belowElectric; }
			if (City.Instance.GetTile(Coordinates.x - 1, Coordinates.y) is IElectricGridElement leftElectric) { ConnectsFromLeftElectric = leftElectric; }

			if (City.Instance.GetTile(Coordinates.x, Coordinates.y - 1) is Tile aboveTile) { aboveTile.OnTileDelete += TileDeleteHandler; }
			if (City.Instance.GetTile(Coordinates.x + 1, Coordinates.y) is Tile rightTile) { rightTile.OnTileDelete += TileDeleteHandler; }
			if (City.Instance.GetTile(Coordinates.x, Coordinates.y + 1) is Tile belowTile) { belowTile.OnTileDelete += TileDeleteHandler; }
			if (City.Instance.GetTile(Coordinates.x - 1, Coordinates.y) is Tile leftTile) { leftTile.OnTileDelete += TileDeleteHandler; }
		}

		private void TileDeleteHandler(object sender, Tile oldTile)
		{
			Tile newTile = City.Instance.GetTile(oldTile);
			newTile.OnPreTileDelete += TileDeleteHandler;

			if (newTile is IElectricGridElement electric)
			{
				if (oldTile.Coordinates.x < Coordinates.x) { ConnectsFromLeftElectric = electric; }
				else if (oldTile.Coordinates.x > Coordinates.x) { ConnectsFromRightElectric = electric; }
				else if (oldTile.Coordinates.y > Coordinates.y) { ConnectsFromBelowElectric = electric; }
				else if (oldTile.Coordinates.y < Coordinates.y) { ConnectsFromAboveElectric = electric; }
			}
			else
			{
				if (oldTile.Coordinates.x < Coordinates.x) { ConnectsFromLeftElectric = null; }
				else if (oldTile.Coordinates.x > Coordinates.x) { ConnectsFromRightElectric = null; }
				else if (oldTile.Coordinates.y > Coordinates.y) { ConnectsFromBelowElectric = null; }
				else if (oldTile.Coordinates.y < Coordinates.y) { ConnectsFromAboveElectric = null; }
			}

			if (newTile is IRoadGridElement road)
			{
				if (oldTile.Coordinates.x < Coordinates.x) { ConnectsFromLeftRoad = road; }
				else if (oldTile.Coordinates.x > Coordinates.x) { ConnectsFromRightRoad = road; }
				else if (oldTile.Coordinates.y > Coordinates.y) { ConnectsFromBelowRoad = road; }
				else if (oldTile.Coordinates.y < Coordinates.y) { ConnectsFromAboveRoad = road; }
			}
			else
			{
				if (oldTile.Coordinates.x < Coordinates.x) { ConnectsFromLeftRoad = null; }
				else if (oldTile.Coordinates.x > Coordinates.x) { ConnectsFromRightRoad = null; }
				else if (oldTile.Coordinates.y > Coordinates.y) { ConnectsFromBelowRoad = null; }
				else if (oldTile.Coordinates.y < Coordinates.y) { ConnectsFromAboveRoad = null; }
			}
		}
		#endregion

		public const uint ABOVEROADMASK = 1;
		public const uint RIGHTROADMASK = 2;
		public const uint BELOWROADMASK = 4;
		public const uint LEFTROADMASK = 8;

		/// <summary>
		/// Construct a new road tile
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		public ElectricRoadTile(int x, int y) : base(x, y, 0)
		{

		}
	}
}