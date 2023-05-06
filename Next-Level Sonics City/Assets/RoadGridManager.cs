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

		adjacentRoadGridElements[0] = SimEngine.Instance.GetTile((int)coords.x, (int)coords.y - 1) as IRoadGridElement;
		adjacentRoadGridElements[1] = SimEngine.Instance.GetTile((int)coords.x + 1, (int)coords.y) as IRoadGridElement;
		adjacentRoadGridElements[2] = SimEngine.Instance.GetTile((int)coords.x, (int)coords.y + 1) as IRoadGridElement;
		adjacentRoadGridElements[3] = SimEngine.Instance.GetTile((int)coords.x - 1, (int)coords.y) as IRoadGridElement;
		
		for (int i = 0; i < adjacentRoadGridElements.Length; i++)
		{
			if (adjacentRoadGridElements[i] == null) { continue; }

			if (roadGridElement.GetRoadGrid() == null)
			{
				roadGridElement.SetRoadGrid(adjacentRoadGridElements[i].GetRoadGrid());
			}
			else
			{
				adjacentRoadGridElements[i].GetRoadGrid().Merge(roadGridElement.GetRoadGrid());
			}
		}

		if (roadGridElement.GetRoadGrid() == null)
		{
			roadGridElement.SetRoadGrid(new());
		}
	}

	public RoadGridManager()
	{

	}

	internal static IRoadGridElement GetBuildingRoadGrig(Building building)
	{
		Vector3 coords = building.Coordinates;

		if (building.Rotation == Rotation.Zero			&& SimEngine.Instance.GetTile((int)coords.x, (int)coords.y - 1) is IRoadGridElement aboveRoadGridElement)	{ return aboveRoadGridElement; }
		if (building.Rotation == Rotation.Ninety		&& SimEngine.Instance.GetTile((int)coords.x + 1, (int)coords.y) is IRoadGridElement rightRoadGridElement)	{ return rightRoadGridElement; }
		if (building.Rotation == Rotation.OneEighty		&& SimEngine.Instance.GetTile((int)coords.x, (int)coords.y + 1) is IRoadGridElement belowRoadGridElement)	{ return belowRoadGridElement; }
		if (building.Rotation == Rotation.TwoSeventy	&& SimEngine.Instance.GetTile((int)coords.x - 1, (int)coords.y) is IRoadGridElement leftRoadGridElement)	{ return leftRoadGridElement; }
		return null;
	}

	private void BreadthFirstSearch(Queue<(IRoadGridElement, int)> workplaceRoadGridElements) //bad
	{
		while (workplaceRoadGridElements.Count > 0)
		{
			(IRoadGridElement element, int depth) roadGridElement;

			lock (workplaceRoadGridElements)
			{
				roadGridElement = workplaceRoadGridElements.Dequeue();
			}

			Vector3 coords = roadGridElement.element.GetTile().Coordinates;
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
						adjacentRoadGridElement.SetRoadGrid(roadGridElement.element.GetRoadGrid());
						adjacentRoadGridElement.SetParent(roadGridElement.element, roadGridElement.depth + 1);
						lock (workplaceRoadGridElements)
						{
							workplaceRoadGridElements.Enqueue((adjacentRoadGridElement, roadGridElement.depth + 1));
						}
					}
					else if (adjacentRoadGridElement.GetRoadGrid() != roadGridElement.element.GetRoadGrid())
					{
						adjacentRoadGridElement.GetRoadGrid().Merge(roadGridElement.element.GetRoadGrid());
					}
				}
				else if (adjacentTile is IWorkplace workplace)
				{
					roadGridElement.element.GetRoadGrid().AddWorkplace(workplace);
				}
				else if (adjacentTile is ResidentialBuildingTile home)
				{
					roadGridElement.element.GetRoadGrid().AddHome(home);
				}
			}
		}
	}
}
