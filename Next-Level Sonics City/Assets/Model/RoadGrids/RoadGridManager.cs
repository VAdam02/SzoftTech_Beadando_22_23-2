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

		private RoadGridManager()
		{

		}

		/// <summary>
		/// Adds a new road grid to the list of road grids
		/// </summary>
		/// <param name="roadGrid">Newly generated road grid</param>
		public void AddRoadGrid(RoadGrid roadGrid)
		{
			_roadGrids.Add(roadGrid);
		}

		/// <summary>
		/// Removes a road grid from the list of road grids
		/// </summary>
		/// <param name="roadGrid">Destroyed road grid</param>
		public void RemoveRoadGrid(RoadGrid roadGrid)
		{
			_roadGrids.Remove(roadGrid);
		}

		/// <summary>
		/// Detect the new road grid element is individual, or it is connected to one or more existing road grids and merge if needed
		/// </summary>
		/// <param name="roadGridElement">Newly created road grid element</param>
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

		/// <summary>
		/// Return the road grid which belongs to the building
		/// </summary>
		/// <param name="building">Building that should be checked</param>
		/// <returns>Road grid element which belongs to the building or null if there's no any</returns>
		internal static IRoadGridElement GetRoadGrigElementByBuilding(Building building)
		{
			Vector3 coords = building.Coordinates;

			if (building.Rotation == Rotation.Zero && City.Instance.GetTile(coords.x, coords.y - 1) is IRoadGridElement aboveRoadGridElement) { return aboveRoadGridElement; }
			if (building.Rotation == Rotation.Ninety && City.Instance.GetTile(coords.x + 1, coords.y) is IRoadGridElement rightRoadGridElement) { return rightRoadGridElement; }
			if (building.Rotation == Rotation.OneEighty && City.Instance.GetTile(coords.x, coords.y + 1) is IRoadGridElement belowRoadGridElement) { return belowRoadGridElement; }
			if (building.Rotation == Rotation.TwoSeventy && City.Instance.GetTile(coords.x - 1, coords.y) is IRoadGridElement leftRoadGridElement) { return leftRoadGridElement; }
			return null;
		}

		/// <summary>
		/// Return the list of buildings which are connected to the road grid element
		/// </summary>
		/// <param name="roadGridElement">Road grid element which should be checked</param>
		/// <returns>List of building that connected to road grid element</returns>
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

		/// <summary>
		/// Return the list of road grid elements which are connected to the road grid element
		/// </summary>
		/// <param name="roadGridElement">Road grid element which should be checked</param>
		/// <returns>List of connected road grid elements</returns>
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