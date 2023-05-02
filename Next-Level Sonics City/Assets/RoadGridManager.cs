using Model;
using Model.Simulation;
using Model.Tiles;
using Model.Tiles.Buildings;
using System.Collections.Generic;
using UnityEngine;

public class RoadGridManager
{
	private readonly List<RoadGrid> _roadGrids = new();
	public List<RoadGrid> RoadGrids { get { return _roadGrids; } }
	public void AddRoadGrid(RoadGrid roadGrid)
	{
		lock (_roadGrids)
		{
			_roadGrids.Add(roadGrid);
		}
	}
	public void RemoveRoadGrid(RoadGrid roadGrid)
	{
		lock (_roadGrids)
		{
			_roadGrids.Remove(roadGrid);
		}
	}

	public void AddRoadGridElement(IRoadGridElement roadGridElement)
	{
		Vector3 coords = roadGridElement.GetTile().Coordinates;
		IRoadGridElement[] adjacentRoadGridElements = new IRoadGridElement[4];

		adjacentRoadGridElements[0] = SimEngine.Instance.GetTile((int)coords.x - 1, (int)coords.y) as IRoadGridElement;
		adjacentRoadGridElements[1] = SimEngine.Instance.GetTile((int)coords.x + 1, (int)coords.y) as IRoadGridElement;
		adjacentRoadGridElements[2] = SimEngine.Instance.GetTile((int)coords.x, (int)coords.y - 1) as IRoadGridElement;
		adjacentRoadGridElements[3] = SimEngine.Instance.GetTile((int)coords.x, (int)coords.y + 1) as IRoadGridElement;

		foreach (IRoadGridElement adjacentRoadGridElement in adjacentRoadGridElements)
		{
			if (adjacentRoadGridElement == null) { continue; }

			adjacentRoadGridElement.GetRoadGrid().Merge(roadGridElement.GetRoadGrid());
		}
	}

	public RoadGridManager()
	{
		long startTime = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
		Init();
		Debug.Log("Takes up : " + ((System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond) - startTime) + " ms");
	}

	private void Init()
	{
		Queue<IRoadGridElement> workplaceRoadGridElements = new();

		foreach (IWorkplace workplace in SimEngine.Instance.City.Workplaces)
		{
			IRoadGridElement roadGridElement = GetWorkplaceRoadGrig(workplace);
			if (roadGridElement is null) { continue; }
			workplaceRoadGridElements.Enqueue(roadGridElement);
		}

		BreadthFirstSearch(workplaceRoadGridElements);
	}

	private IRoadGridElement GetWorkplaceRoadGrig(IWorkplace workplace)
	{
		Tile tile = workplace.GetTile();
		Vector3 coords = tile.Coordinates;

		if (((Building)tile).Rotation == Rotation.Zero)			{ return (IRoadGridElement)SimEngine.Instance.GetTile((int)coords.x, (int)coords.y - 1); }
		if (((Building)tile).Rotation == Rotation.Ninety)		{ return (IRoadGridElement)SimEngine.Instance.GetTile((int)coords.x + 1, (int)coords.y); }
		if (((Building)tile).Rotation == Rotation.OneEighty)	{ return (IRoadGridElement)SimEngine.Instance.GetTile((int)coords.x, (int)coords.y + 1); }
		if (((Building)tile).Rotation == Rotation.TwoSeventy)	{ return (IRoadGridElement)SimEngine.Instance.GetTile((int)coords.x - 1, (int)coords.y); }
		return null;
	}

	private void BreadthFirstSearch(Queue<IRoadGridElement> workplaceRoadGridElements)
	{
		while (workplaceRoadGridElements.Count > 0)
		{
			IRoadGridElement roadGridElement;

			lock (workplaceRoadGridElements)
			{
				roadGridElement = workplaceRoadGridElements.Dequeue();
			}

			Vector3 coords = roadGridElement.GetTile().Coordinates;
			Tile[] adjacentTiles = new Tile[4];
			adjacentTiles[0] = SimEngine.Instance.GetTile((int)coords.x - 1, (int)coords.y);
			adjacentTiles[1] = SimEngine.Instance.GetTile((int)coords.x + 1, (int)coords.y);
			adjacentTiles[2] = SimEngine.Instance.GetTile((int)coords.x, (int)coords.y - 1);
			adjacentTiles[3] = SimEngine.Instance.GetTile((int)coords.x, (int)coords.y + 1);

			foreach (Tile adjacentTile in adjacentTiles)
			{
				if (adjacentTile is IRoadGridElement adjacentRoadGridElement)
				{
					if (adjacentRoadGridElement.GetRoadGrid() == null)
					{
						adjacentRoadGridElement.SetRoadGrid(roadGridElement.GetRoadGrid());
						adjacentRoadGridElement.SetParent(roadGridElement);
						lock (workplaceRoadGridElements)
						{
							workplaceRoadGridElements.Enqueue(adjacentRoadGridElement);
						}
					}
					else if (adjacentRoadGridElement.GetRoadGrid() != roadGridElement.GetRoadGrid())
					{
						adjacentRoadGridElement.GetRoadGrid().Merge(roadGridElement.GetRoadGrid());
					}
				}
				else if (adjacentTile is IWorkplace workplace)
				{
					roadGridElement.GetRoadGrid().AddWorkplace(workplace);
				}
				else if (adjacentTile is ResidentialBuildingTile home)
				{
					roadGridElement.GetRoadGrid().AddHome(home);
				}
			}
		}
	}
}
