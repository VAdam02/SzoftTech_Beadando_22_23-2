using Model.Tiles;
using Model.Tiles.Buildings;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Tests")]

namespace Model.RoadGrids
{
	public class RoadGridManager
	{
		private static RoadGridManager _instance;
		public static RoadGridManager Instance
		{
			get
			{
				_instance ??= new RoadGridManager();
				return _instance;
			}
		}

		public static void Reset() { _instance = null; }

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

		private RoadGridManager()
		{

		}

		internal static IRoadGridElement GetRoadGrigElementByBuilding(Building building)
		{
			Vector3 coords = building.Coordinates;

			if (building.Rotation == Rotation.Zero && City.Instance.GetTile(coords.x, coords.y - 1) is IRoadGridElement aboveRoadGridElement) { return aboveRoadGridElement; }
			if (building.Rotation == Rotation.Ninety && City.Instance.GetTile(coords.x + 1, coords.y) is IRoadGridElement rightRoadGridElement) { return rightRoadGridElement; }
			if (building.Rotation == Rotation.OneEighty && City.Instance.GetTile(coords.x, coords.y + 1) is IRoadGridElement belowRoadGridElement) { return belowRoadGridElement; }
			if (building.Rotation == Rotation.TwoSeventy && City.Instance.GetTile(coords.x - 1, coords.y) is IRoadGridElement leftRoadGridElement) { return leftRoadGridElement; }
			return null;
		}

		internal static List<Building> GetBuildingsByRoadGridElement(IRoadGridElement roadGridElement)
		{
			Vector3 coords = roadGridElement.GetTile().Coordinates;
			List<Building> buildings = new();

			if (City.Instance.GetTile(coords.x, coords.y + 1) is Building belowBuilding && belowBuilding.Rotation == Rotation.Zero) { buildings.Add(belowBuilding); }
			if (City.Instance.GetTile(coords.x - 1, coords.y) is Building leftBuilding && leftBuilding.Rotation == Rotation.Ninety) { buildings.Add(leftBuilding); }
			if (City.Instance.GetTile(coords.x, coords.y - 1) is Building aboveBuilding && aboveBuilding.Rotation == Rotation.OneEighty) { buildings.Add(aboveBuilding); }
			if (City.Instance.GetTile(coords.x + 1, coords.y) is Building rightBuilding && rightBuilding.Rotation == Rotation.TwoSeventy) { buildings.Add(rightBuilding); }

			return buildings;
		}

		internal static List<IRoadGridElement> GetRoadGridElementsByRoadGridElement(IRoadGridElement roadGridElement)
		{
			Vector3 coords = roadGridElement.GetTile().Coordinates;
			List<IRoadGridElement> roadGridElements = new();
			if (City.Instance.GetTile(coords.x, coords.y - 1) is IRoadGridElement aboveRoadGridElement) { roadGridElements.Add(aboveRoadGridElement); }
			if (City.Instance.GetTile(coords.x + 1, coords.y) is IRoadGridElement rightRoadGridElement) { roadGridElements.Add(rightRoadGridElement); }
			if (City.Instance.GetTile(coords.x, coords.y + 1) is IRoadGridElement belowRoadGridElement) { roadGridElements.Add(belowRoadGridElement); }
			if (City.Instance.GetTile(coords.x - 1, coords.y) is IRoadGridElement leftRoadGridElement) { roadGridElements.Add(leftRoadGridElement); }
			return roadGridElements;
		}
	}
}