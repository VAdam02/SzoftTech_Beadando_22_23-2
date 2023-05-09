using Model.Simulation;
using System.Collections.Generic;
using System.Linq;
using Model.RoadGrids;
using UnityEngine;

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
				_roadGrid.Reinit();
			}
			else
			{
				_roadGrid?.RemoveRoadGridElement(this);
				_roadGrid = roadGrid;
				_roadGrid?.AddRoadGridElement(this);
			}

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

		public RoadTile(int x, int y, uint designID) : base(x, y, designID)
		{
			ConnectToSurroundingRoads();
		}

		public void RegisterRoadGridElement()
		{
			SimEngine.Instance.RoadGridManager.AddRoadGridElement(this);
		}

		public void UnregisterRoadGridElement()
		{
			SetRoadGrid(null);
		}

		private void ConnectToSurroundingRoads()
		{
			if (SimEngine.Instance.GetTile((int)Coordinates.x, (int)Coordinates.y - 1) is RoadTile aboveRoad) { FromAbove = aboveRoad; }
			if (SimEngine.Instance.GetTile((int)Coordinates.x + 1, (int)Coordinates.y) is RoadTile rightRoad) { FromRight = rightRoad; }
			if (SimEngine.Instance.GetTile((int)Coordinates.x, (int)Coordinates.y + 1) is RoadTile belowRoad) { FromBelow = belowRoad; }
			if (SimEngine.Instance.GetTile((int)Coordinates.x - 1, (int)Coordinates.y) is RoadTile leftRoad)  { FromLeft  = leftRoad;  }
		}

		public override void NeighborTileChanged(Tile oldTile, Tile newTile)
		{
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

		public override int GetBuildPrice() //TODO implementing logic
		{
			return BUILD_PRICE;
		}

		public override int GetDestroyPrice()
		{
			return DESTROY_PRICE;
		}

		public override int GetMaintainanceCost()
		{
			return GetBuildPrice() / 10;
		}
	}
}
