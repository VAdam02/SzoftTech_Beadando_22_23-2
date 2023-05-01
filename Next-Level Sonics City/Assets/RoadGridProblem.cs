using Model.Simulation;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Model.Tiles.Buildings;

public class RoadGridProblem
{
	public static void Generate()
	{
		Queue<IRoadGridElement> workplaceRoadGridElements = new();

		foreach (IWorkplace workplace in SimEngine.Instance.City.Workplaces)
		{
			Vector3 coords = workplace.GetTile().Coordinates;
			IRoadGridElement[] adjacentRoadGridElements = new IRoadGridElement[4];
			
			adjacentRoadGridElements[0] = SimEngine.Instance.GetTile((int)coords.x - 1, (int)coords.y) as IRoadGridElement;
			adjacentRoadGridElements[1] = SimEngine.Instance.GetTile((int)coords.x + 1, (int)coords.y) as IRoadGridElement;
			adjacentRoadGridElements[2] = SimEngine.Instance.GetTile((int)coords.x, (int)coords.y - 1) as IRoadGridElement;
			adjacentRoadGridElements[3] = SimEngine.Instance.GetTile((int)coords.x, (int)coords.y + 1) as IRoadGridElement;

			foreach (IRoadGridElement adjacentRoadGridElement in adjacentRoadGridElements)
			{
				if (adjacentRoadGridElement == null) { continue; }

				if (adjacentRoadGridElement.GetRoadGrid() == null)
				{
					RoadGrid roadGrid = new();
					adjacentRoadGridElement.SetRoadGrid(roadGrid);
				}

				adjacentRoadGridElement.GetRoadGrid().AddWorkplace(workplace);

				lock (workplaceRoadGridElements)
				{
					workplaceRoadGridElements.Enqueue(adjacentRoadGridElement);
				}
			}
		}

		BreadthFirstSearch(workplaceRoadGridElements);
	}

	private static void BreadthFirstSearch(Queue<IRoadGridElement> workplaceRoadGridElements)
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
