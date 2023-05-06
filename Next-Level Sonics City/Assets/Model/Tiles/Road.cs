using Model.Simulation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model.Tiles
{
	public class Road : Tile, IRoadGridElement
	{
		private const uint ABOVEROADMASK = 1;
		private const uint RIGHTROADMASK = 2;
		private const uint BELOWROADMASK = 4;
		private const uint LEFTROADMASK = 8;

		private readonly Road[] _roads = new Road[4];
		public Road FromAbove
		{
			get { return _roads[0]; }
			set
			{
				_roads[1] = value;
				if (value == null)	{ DesignID &= ~ABOVEROADMASK; }
				else				{ DesignID |= ABOVEROADMASK;  }
			}
		}
		public Road FromRight
		{
			get { return _roads[1]; }
			set
			{
				_roads[2] = value;
				if (value == null)	{ DesignID &= ~RIGHTROADMASK; }
				else				{ DesignID |= RIGHTROADMASK;  }
			}
		}
		public Road FromBelow
		{
			get { return _roads[2]; }
			set
			{
				_roads[3] = value;
				if (value == null)	{ DesignID &= ~BELOWROADMASK; }
				else				{ DesignID |= BELOWROADMASK;  }
			}
		}
		public Road FromLeft
		{
			get { return _roads[3]; }
			set
			{
				_roads[0] = value;
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

			List<Building> buildings = RoadGridManager.GetBuildingsByRoadGrid(this);
			foreach (Building building in buildings)
			{
				if (building is IWorkplace workplace)
				{
					workplace.UnregisterWorkplace();
				}
				if (building is IResidential residential)
				{
					residential.UnregisterResidential();
				}
			}

			_roadGrid?.RemoveRoadGridElement(this);
			_roadGrid = roadGrid;
			_roadGrid.AddRoadGridElement(this);

			foreach (Building building in buildings)
			{
				if (building is IWorkplace workplace)
				{
					workplace.RegisterWorkplace();
				}
				if (building is IResidential residential)
				{
					residential.RegisterResidential();
				}
			}
		}

		private IRoadGridElement _parent = null;
		private int _depth = -1;
		public void SetParent(IRoadGridElement parent, int depth) { _parent = parent; _depth = depth; }
		public IRoadGridElement GetParent() { _roadGrid.OptimizePaths(); return _parent; }
		public int GetDepth() { _roadGrid.OptimizePaths(); return _depth; }

		int IRoadGridElement.GetDepthUnoptimized() { return _depth; }

		public Road(int x, int y, uint designID) : base(x, y, designID)
		{
			ConnectToSurroundingRoads();
		}

		public void RegisterRoadGridElement()
		{
			SimEngine.Instance.RoadGridManager.AddRoadGridElement(this);
		}

		public void UnregisterRoadGridElement()
		{
			_roadGrid.RemoveRoadGridElement(this);
		}

		private void ConnectToSurroundingRoads()
		{
			if (SimEngine.Instance.GetTile((int)Coordinates.x - 1, (int)Coordinates.y) is Road leftRoad)  { FromLeft = leftRoad;   }
			if (SimEngine.Instance.GetTile((int)Coordinates.x  +1, (int)Coordinates.y) is Road rightRoad) { FromRight = rightRoad; }
			if (SimEngine.Instance.GetTile((int)Coordinates.x, (int)Coordinates.y - 1) is Road aboveRoad) { FromAbove = aboveRoad; }
			if (SimEngine.Instance.GetTile((int)Coordinates.x, (int)Coordinates.y + 1) is Road belowRoad) { FromBelow = belowRoad; }
		}

		public override void NeighborTileChanged(Tile oldTile, Tile newTile)
		{
			if (oldTile is Road)
			{
				if (oldTile.Coordinates.x < Coordinates.x)		{ FromLeft = null;  }
				else if (oldTile.Coordinates.x > Coordinates.x)	{ FromRight = null; }
				else if (oldTile.Coordinates.y < Coordinates.y)	{ FromBelow = null; }
				else if (oldTile.Coordinates.y > Coordinates.y)	{ FromAbove = null; }
			}

			if (newTile is Road road)
			{
				if (newTile.Coordinates.x < Coordinates.x)		{ FromLeft = road;  }
				else if (newTile.Coordinates.x > Coordinates.x)	{ FromRight = road; }
				else if (newTile.Coordinates.y < Coordinates.y)	{ FromBelow = road; }
				else if (newTile.Coordinates.y > Coordinates.y)	{ FromAbove = road; }
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
