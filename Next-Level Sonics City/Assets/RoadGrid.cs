using Model;
using Model.Simulation;
using Model.Tiles;
using System.Collections.Generic;
using UnityEngine;

public class RoadGrid
{
	public RoadGrid()
	{
		SimEngine.Instance.RoadGridManager.AddRoadGrid(this);
	}

	private readonly List<IRoadGridElement> _roadGridElements = new();
	public List<IRoadGridElement> RoadGridElements { get { OptimizePaths(); return _roadGridElements; } }
	public void AddRoadGridElement(IRoadGridElement roadGridElement)
	{
		lock (_roadGridElements)
		{
			_roadGridElements.Add(roadGridElement);
		}
		_isOptimized = false;
	}
	public void RemoveRoadGridElement(IRoadGridElement roadGridElement)
	{
		lock (_roadGridElements)
		{
			_roadGridElements.Remove(roadGridElement);
		}
	}

	private readonly List<IWorkplace> _workplaces = new();
	public List<IWorkplace> Workplaces { get { OptimizePaths(); return _workplaces; } }
	public void AddWorkplace(IWorkplace workplace)
	{
		lock (_workplaces)
		{
			_workplaces.Add(workplace);
		}
	}
	public void RemoveWorkplace(IWorkplace workplace)
	{
		lock (_workplaces)
		{
			_workplaces.Remove(workplace);
		}
	}

	private readonly List<IResidential> _homes = new();
	public List<IResidential> Homes { get { OptimizePaths(); return _homes; } }
	public void AddHome(IResidential home)
	{
		lock (_homes)
		{
			_homes.Add(home);
		}
	}
	public void RemoveHome(IResidential home)
	{
		lock (_homes)
		{
			_homes.Remove(home);
		}
	}

	public void Merge(RoadGrid roadGrid)
	{
		if (this == roadGrid) return;

		while (roadGrid._roadGridElements.Count > 0)
		{
			roadGrid._roadGridElements[0].SetRoadGrid(this);
		}
		
		foreach (IWorkplace workplace in roadGrid._workplaces) { _workplaces.Add(workplace); }

		foreach (IResidential home in roadGrid._homes) { _homes.Add(home); }

		SimEngine.Instance.RoadGridManager.RemoveRoadGrid(roadGrid);

		_isOptimized = false;
	}

	private bool _isOptimized = true;
	public void OptimizePaths()
	{
		if (_isOptimized) { return; }

		Debug.Log("Optimizing");
		long startTime = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;

		Queue<(IRoadGridElement, int)> queue = new();

		foreach (IRoadGridElement roadGridElement in _roadGridElements)
		{
			roadGridElement.SetParent(null, -1);
		}

		foreach (IWorkplace workplace in _workplaces)
		{
			IRoadGridElement roadGridElement = RoadGridManager.GetBuildingRoadGrig((Building)workplace);
			if (roadGridElement is null) { continue; }
			roadGridElement.SetParent(null, 0);
			queue.Enqueue((roadGridElement, 0));
		}
		
		while (queue.Count > 0)
		{
			(IRoadGridElement element, int depth) = queue.Dequeue();

			Vector3 coords = element.GetTile().Coordinates;
			IRoadGridElement[] adjacentRoadGridElements = new IRoadGridElement[4];
			adjacentRoadGridElements[0] = SimEngine.Instance.GetTile((int)coords.x, (int)coords.y - 1) is IRoadGridElement aboveRoadGridElement ? aboveRoadGridElement : null;
			adjacentRoadGridElements[1] = SimEngine.Instance.GetTile((int)coords.x + 1, (int)coords.y) is IRoadGridElement rightRoadGridElement ? rightRoadGridElement : null;
			adjacentRoadGridElements[2] = SimEngine.Instance.GetTile((int)coords.x, (int)coords.y + 1) is IRoadGridElement belowRoadGridElement ? belowRoadGridElement : null;
			adjacentRoadGridElements[3] = SimEngine.Instance.GetTile((int)coords.x - 1, (int)coords.y) is IRoadGridElement leftRoadGridElement ? leftRoadGridElement : null;


			foreach (IRoadGridElement adjacentRoadGridElement in adjacentRoadGridElements)
			{
				if (adjacentRoadGridElement is not null && adjacentRoadGridElement.GetDepthUnoptimized() == -1)
				{
					adjacentRoadGridElement.SetParent(element, depth + 1);
					queue.Enqueue((adjacentRoadGridElement, depth+1));
				}
			}
		}

		_isOptimized = true;

		Debug.Log("Optimizing takes up " + ((System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond) - startTime) + " ms");
	}
}
