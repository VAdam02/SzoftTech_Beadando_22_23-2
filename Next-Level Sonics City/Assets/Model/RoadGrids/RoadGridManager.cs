using Model.Simulation;
using Model.Tiles;
using Model.Tiles.Buildings;
using System.Collections.Generic;
using UnityEngine;

namespace Model.RoadGrids
{
	public class RoadGridManager
	{
		private readonly List<RoadGrid> _roadGrids = new();
		public List<RoadGrid> RoadGrids { get { return _roadGrids; } }
		public void AddRoadGrid(RoadGrid roadGrid)
		{
			_roadGrids.Add(roadGrid);
		}
		public void RemoveRoadGrid(RoadGrid roadGrid)
		{
			_roadGrids.Remove(roadGrid);
		}

		public void AddRoadGridElement(IRoadGridElement roadGridElement)
		{
			List<IRoadGridElement> adjacentRoadGridElements = GetRoadGridElementsByRoadGridElement(roadGridElement);

			for (int i = 0; i < adjacentRoadGridElements.Count; i++)
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

		internal static IRoadGridElement GetRoadGrigElementByBuilding(Building building)
		{
			Vector3 coords = building.Coordinates;

			if (building.Rotation == Rotation.Zero && SimEngine.Instance.GetTile((int)coords.x, (int)coords.y - 1) is IRoadGridElement aboveRoadGridElement) { return aboveRoadGridElement; }
			if (building.Rotation == Rotation.Ninety && SimEngine.Instance.GetTile((int)coords.x + 1, (int)coords.y) is IRoadGridElement rightRoadGridElement) { return rightRoadGridElement; }
			if (building.Rotation == Rotation.OneEighty && SimEngine.Instance.GetTile((int)coords.x, (int)coords.y + 1) is IRoadGridElement belowRoadGridElement) { return belowRoadGridElement; }
			if (building.Rotation == Rotation.TwoSeventy && SimEngine.Instance.GetTile((int)coords.x - 1, (int)coords.y) is IRoadGridElement leftRoadGridElement) { return leftRoadGridElement; }
			return null;
		}

		internal static List<Building> GetBuildingsByRoadGridElement(IRoadGridElement roadGridElement)
		{
			Vector3 coords = roadGridElement.GetTile().Coordinates;
			List<Building> buildings = new();

			if (SimEngine.Instance.GetTile((int)coords.x, (int)coords.y + 1) is Building belowBuilding && belowBuilding.Rotation == Rotation.Zero) { buildings.Add(belowBuilding); }
			if (SimEngine.Instance.GetTile((int)coords.x - 1, (int)coords.y) is Building leftBuilding && leftBuilding.Rotation == Rotation.Ninety) { buildings.Add(leftBuilding); }
			if (SimEngine.Instance.GetTile((int)coords.x, (int)coords.y - 1) is Building aboveBuilding && aboveBuilding.Rotation == Rotation.OneEighty) { buildings.Add(aboveBuilding); }
			if (SimEngine.Instance.GetTile((int)coords.x + 1, (int)coords.y) is Building rightBuilding && rightBuilding.Rotation == Rotation.TwoSeventy) { buildings.Add(rightBuilding); }

			return buildings;
		}

		internal static List<IRoadGridElement> GetRoadGridElementsByRoadGridElement(IRoadGridElement roadGridElement)
		{
			Vector3 coords = roadGridElement.GetTile().Coordinates;
			List<IRoadGridElement> roadGridElements = new();
			if (SimEngine.Instance.GetTile((int)coords.x, (int)coords.y - 1) is IRoadGridElement aboveRoadGridElement) { roadGridElements.Add(aboveRoadGridElement); }
			if (SimEngine.Instance.GetTile((int)coords.x + 1, (int)coords.y) is IRoadGridElement rightRoadGridElement) { roadGridElements.Add(rightRoadGridElement); }
			if (SimEngine.Instance.GetTile((int)coords.x, (int)coords.y + 1) is IRoadGridElement belowRoadGridElement) { roadGridElements.Add(belowRoadGridElement); }
			if (SimEngine.Instance.GetTile((int)coords.x - 1, (int)coords.y) is IRoadGridElement leftRoadGridElement) { roadGridElements.Add(leftRoadGridElement); }
			return roadGridElements;
		}
	}
}