using Model.RoadGrids;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Tiles
{
	public class RoadTile : Tile, IRoadGridElement
	{
		public const uint ABOVEROADMASK = 1;
		public const uint RIGHTROADMASK = 2;
		public const uint BELOWROADMASK = 4;
		public const uint LEFTROADMASK = 8;

		private readonly RoadTile[] _roads = new RoadTile[4];
		public RoadTile FromAbove
		{
			get { return _roads[0]; }
			set
			{
				_roads[0] = value;
				if (value == null)	{ DesignID &= ~ABOVEROADMASK; }
				else				{ DesignID |= ABOVEROADMASK;  }
			}
		}
		public RoadTile FromRight
		{
			get { return _roads[1]; }
			set
			{
				_roads[1] = value;
				if (value == null)	{ DesignID &= ~RIGHTROADMASK; }
				else				{ DesignID |= RIGHTROADMASK;  }
			}
		}
		public RoadTile FromBelow
		{
			get { return _roads[2]; }
			set
			{
				_roads[2] = value;
				if (value == null)	{ DesignID &= ~BELOWROADMASK; }
				else				{ DesignID |= BELOWROADMASK;  }
			}
		}
		public RoadTile FromLeft
		{
			get { return _roads[3]; }
			set
			{
				_roads[3] = value;
				if (value == null) { DesignID &= ~LEFTROADMASK; }
				else { DesignID |= LEFTROADMASK; }
			}
		}

		public List<IRoadGridElement> ConnectsTo()
		{
			return new List<IRoadGridElement>(_roads).Where(x => x != null).ToList();
		}

		public Tile GetTile() { return this; }

		private RoadGrid _roadGrid = null;
		public RoadGrid GetRoadGrid() { return _roadGrid; }

		public void SetRoadGrid(RoadGrid roadGrid)
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

		/// <summary>
		/// Construct a new road tile
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		public RoadTile(int x, int y, uint designID) : base(x, y, designID)
		{
			
		}

		public override TileType GetTileType() { return TileType.Road; }

		public override void FinalizeTile()
		{
			Finalizing();
		}

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Finalizing()</code></para>
		/// <para>Do the actual finalization</para>
		/// </summary>
		protected new void Finalizing()
		{
			base.Finalizing();
			ConnectToSurroundingRoads();
		}

		public void RegisterRoadGridElement()
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			RoadGridManager.Instance.AddRoadGridElement(this);
		}

		public void UnregisterRoadGridElement()
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			SetRoadGrid(null);
		}

		/// <summary>
		/// Connects this road to the surrounding roads.
		/// </summary>
		private void ConnectToSurroundingRoads()
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			if (City.Instance.GetTile(Coordinates.x, Coordinates.y - 1) is RoadTile aboveRoad) { FromAbove = aboveRoad; }
			if (City.Instance.GetTile(Coordinates.x + 1, Coordinates.y) is RoadTile rightRoad) { FromRight = rightRoad; }
			if (City.Instance.GetTile(Coordinates.x, Coordinates.y + 1) is RoadTile belowRoad) { FromBelow = belowRoad; }
			if (City.Instance.GetTile(Coordinates.x - 1, Coordinates.y) is RoadTile leftRoad)  { FromLeft  = leftRoad;  }
		}

		public override void NeighborTileReplaced(Tile oldTile, Tile newTile)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			if (oldTile is RoadTile)
			{
				if      (oldTile.Coordinates.x < Coordinates.x)	{ FromLeft  = null; }
				else if (oldTile.Coordinates.x > Coordinates.x)	{ FromRight = null; }
				else if (oldTile.Coordinates.y > Coordinates.y)	{ FromBelow = null; }
				else if (oldTile.Coordinates.y < Coordinates.y)	{ FromAbove = null; }
			}

			if (newTile is RoadTile road)
			{
				if      (newTile.Coordinates.x < Coordinates.x)	{ FromLeft  = road; }
				else if (newTile.Coordinates.x > Coordinates.x)	{ FromRight = road; }
				else if (newTile.Coordinates.y > Coordinates.y)	{ FromBelow = road; }
				else if (newTile.Coordinates.y < Coordinates.y)	{ FromAbove = road; }
			}
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
	}
}
