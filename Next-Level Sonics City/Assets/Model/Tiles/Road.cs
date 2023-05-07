using Model.Simulation;
using System;
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

			List<Building> buildings = RoadGridManager.GetBuildingsByRoadGridElement(this);
			foreach (Building building in buildings)
			{
				if (building is IWorkplace workplace)
				{
					Debug.Log("IWorkplace\t" + Coordinates + "\t" + building.Coordinates);
					workplace.UnregisterWorkplace(_roadGrid);
				}
				if (building is IResidential residential)
				{
					Debug.Log("IResidential\t" + Coordinates + "\t" + building.Coordinates);
					residential.UnregisterResidential(_roadGrid);
				}
			}

			if (roadGrid == null)
			{
				//TODO remove grids
				List<IRoadGridElement> nonProblematicSplits = RoadGridManager.GetRoadGridElementsByRoadGridElement(this).FindAll(x => x.GetParent() != this);
				List<IRoadGridElement> breakpoints = RoadGridManager.GetRoadGridElementsByRoadGridElement(this).FindAll(x => x.GetParent() == this);

				//NO MORE OPTIMIZED REQUEST ALLOWED BEYOND THIS POINT

				_roadGrid?.RemoveRoadGridElement(this);
				_roadGrid = roadGrid;
				SetParent(null, -1);

				Debug.Log("Breakpoints: " + breakpoints.Count + "\tNonProblematic: " + nonProblematicSplits.Count);

				//breadth first search from each element
				for (int i = 1000; i < breakpoints.Count; i++)
				{
					IRoadGridElement startElement = breakpoints[i];
					startElement.SetRoadGrid(new()); //TODO maybe one new grid touch multiple neighbour and if that happen the bottom algorithm will broke because it don't check the validity of parent chain
					SetParent(null, -1);

					List<IRoadGridElement> escapePoints = new();
					Queue<IRoadGridElement> queue = new();

					queue.Enqueue(startElement);

					while (queue.Count > 0)
					{
						IRoadGridElement element = queue.Dequeue();

						List<IRoadGridElement> neighbours = RoadGridManager.GetRoadGridElementsByRoadGridElement(element);
						foreach (IRoadGridElement neighbour in neighbours)
						{
							if (neighbour.GetParentUnoptimized() == element) //looking down
							{
								queue.Enqueue(neighbour);
								neighbour.SetRoadGrid(startElement.GetRoadGrid());
								neighbour.SetParent(null, -1);
							}
							else if (neighbour.GetParentUnoptimized() == null && neighbour.GetDepthUnoptimized() == -1) //looking back
							{

							}
							else if (
							/*
							else if (neighbour.GetParentUnoptimized() == null && neighbour.GetDepthUnoptimized() == 0) //it's a root -> but that's not mean it's an escape point
							{
								escapePoints.Add(neighbour);
								//neighbour.GetRoadGrid().Merge(startElement.GetRoadGrid());
							}
							else if (neighbour.GetParentUnoptimized() != null && neighbour.GetDepthUnoptimized() != -1) //it's a child of outside chain -> escape point
							{
								escapePoints.Add(neighbour);
								//neighbour.GetRoadGrid().Merge(startElement.GetRoadGrid());
							}
							*/
							else
							{
								//there is no logical way where an element's parent is null but the depth is bigger than zero or the parent is not null but the depth is -1
								throw new Exception();
							}
						}
					}

					/*
					if (startElement.GetParentUnoptimized() != null) { queue.Enqueue(startElement); }

					while (queue.Count > 0)
					{
						IRoadGridElement element = queue.Dequeue();
						if (element.GetParentUnoptimized().GetParentUnoptimized() == null && element.GetParentUnoptimized().GetDepthUnoptimized() == -1)
						{
							//looked into undefined part
							RoadGridManager.GetRoadGridElementsByRoadGridElement(element)
								.FindAll(x => !(x.GetParentUnoptimized() == null && x.GetDepthUnoptimized() == -1)) //we don't want to add an already processed element (like the parent and neighbour)
								.ForEach(x => { if (!queue.Contains(x)) { queue.Enqueue(x); } });                   //we don't want to add it twice

							element.SetRoadGrid(startElement.GetRoadGrid());
							element.SetParent(null, -1);

							//get roadGridElement's buildings
							
						}
						else if (element.GetParentUnoptimized().GetDepthUnoptimized() >= 0)
						{
							//looked outside to untouched part
							escapePoints.Add(element);
						}
						else
						{
							//there is no logical way where an element's parent is not null but the depth is -1
							throw new Exception();
						}
					}
					*/

					Debug.Log("-----------------------------------");
					foreach (IRoadGridElement escapePoint in escapePoints)
					{
						Debug.Log(escapePoint.GetTile().Coordinates);
					}

					Debug.Log("------");
					foreach (IRoadGridElement breakpoint in breakpoints)
					{
						Debug.Log(breakpoint.GetTile().Coordinates);
					}
					Debug.Log("-----------------------------------");

				}

				Debug.Log("........................................................................");

				for (int i = 0; i < nonProblematicSplits.Count; i++)
				{
					IRoadGridElement startElement = nonProblematicSplits[i];

					while (startElement.GetParentUnoptimized() != null)
					{
						startElement = startElement.GetParentUnoptimized();
					}
					Debug.Log("Start from " + startElement.GetTile().Coordinates);
					startElement.SetRoadGrid(new()); //TODO I don't think it through but at some conditions this may could break the algorithm (like if the parent is not null but the depth is -1)
					startElement.SetParent(null, -1);

					List<IRoadGridElement> escapePoints = new();
					Queue<IRoadGridElement> queue = new();
					queue.Enqueue(startElement);

					while (queue.Count > 0)
					{
						IRoadGridElement element = queue.Dequeue();

						List<IRoadGridElement> neighbours = RoadGridManager.GetRoadGridElementsByRoadGridElement(element);
						foreach (IRoadGridElement neighbour in neighbours)
						{
							if (neighbour.GetParentUnoptimized() == element) //looking down
							{
								queue.Enqueue(neighbour);
								neighbour.SetRoadGrid(startElement.GetRoadGrid());
								neighbour.SetParent(null, -1);
							}
							else if (neighbour.GetParentUnoptimized() == null && neighbour.GetDepthUnoptimized() == -1) //looking back
							{

							}
							else if (neighbour.GetParentUnoptimized() == null && neighbour.GetDepthUnoptimized() == 0) //it's a root -> escape point
							{
								escapePoints.Add(neighbour);
								neighbour.GetRoadGrid().Merge(startElement.GetRoadGrid());
							}
							else if (neighbour.GetParentUnoptimized() != null && neighbour.GetDepthUnoptimized() != -1) //it's a child of outside chain -> escape point
							{
								escapePoints.Add(neighbour);
								neighbour.GetRoadGrid().Merge(startElement.GetRoadGrid());
							}
							else
							{
								//there is no logical way where an element's parent is null but the depth is bigger than zero or the parent is not null but the depth is -1
								throw new Exception();
							}
						}
					}

					Debug.Log("???????????????????????????????????");
					foreach (IRoadGridElement escapePoint in escapePoints)
					{
						Debug.Log(escapePoint.GetTile().Coordinates);
					}

					Debug.Log("??????");
					foreach (IRoadGridElement nonProblematicSplit in nonProblematicSplits)
					{
						Debug.Log(nonProblematicSplit.GetTile().Coordinates);
					}
					Debug.Log("???????????????????????????????????");
				}
				Debug.Log("------------------------------------------------------------------------");



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

		private IRoadGridElement _parent = null;
		private int _depth = -1;
		public void SetParent(IRoadGridElement parent, int depth) { _parent = parent; _depth = depth; }
		public IRoadGridElement GetParent() { _roadGrid.OptimizePaths(); return _parent; }
		IRoadGridElement IRoadGridElement.GetParentUnoptimized() { return _parent; }
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
			SetRoadGrid(null);
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
