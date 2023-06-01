using Model.Tiles;
using Model.Tiles.Buildings;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Tests")]

namespace Model.ElectricGrids
{
	public class ElectricGridManager
	{
		private static ElectricGridManager _instance;
		public static ElectricGridManager Instance
		{
			get
			{
				_instance ??= new ElectricGridManager();
				return _instance;
			}
		}

		public static void Reset() { _instance = null; }

		private readonly List<ElectricGrid> _electricGrids = new();
		public List<ElectricGrid> ElectricGrids { get { return _electricGrids; } }

		private ElectricGridManager()
		{

		}

		/// <summary>
		/// Adds a new electric grid to the list of electric grids
		/// </summary>
		/// <param name="electricGrid">Newly generated electric grid</param>
		public void AddElectricGrid(ElectricGrid electricGrid)
		{
			lock (_electricGrids) _electricGrids.Add(electricGrid);
		}

		/// <summary>
		/// Removes a electric grid from the list of electric grids
		/// </summary>
		/// <param name="electricGrid">Destroyed electric grid</param>
		public void RemoveElectricGrid(ElectricGrid electricGrid)
		{
			lock (_electricGrids) _electricGrids.Remove(electricGrid);
		}

		/// <summary>
		/// Detect the new electric grid element is individual, or it is connected to one or more existing electric grids and merge if needed
		/// </summary>
		/// <param name="electricGridElement">Newly created electric grid element</param>
		public void AddElectricGridElement(IElectricGridElement electricGridElement)
		{
			List<IElectricGridElement> adjacentElectricGridElements = electricGridElement.ConnectsTo;

			for (int i = 0; i < adjacentElectricGridElements.Count; i++)
			{
				if (adjacentElectricGridElements[i] == null) { continue; }

				if (electricGridElement.ElectricGrid == null)
				{
					electricGridElement.ElectricGrid = adjacentElectricGridElements[i].ElectricGrid;
				}
				else
				{
					adjacentElectricGridElements[i].ElectricGrid.Merge(electricGridElement.ElectricGrid);
				}
			}

			electricGridElement.ElectricGrid ??= new();
		}

		/// <summary>
		/// Return the electric grid which belongs to the building
		/// </summary>
		/// <param name="building">Building that should be checked</param>
		/// <returns>Electric grid element which belongs to the building or null if there's no any</returns>
		internal static IElectricGridElement GetElectricGrigElementByBuilding(Building building)
		{
			Vector3 coords = building.Coordinates;

			if (building.Rotation == Rotation.Zero && City.Instance.GetTile(coords.x, coords.y - 1) is IElectricGridElement aboveElectricGridElement) { return aboveElectricGridElement; }
			if (building.Rotation == Rotation.Ninety && City.Instance.GetTile(coords.x + 1, coords.y) is IElectricGridElement rightElectricGridElement) { return rightElectricGridElement; }
			if (building.Rotation == Rotation.OneEighty && City.Instance.GetTile(coords.x, coords.y + 1) is IElectricGridElement belowElectricGridElement) { return belowElectricGridElement; }
			if (building.Rotation == Rotation.TwoSeventy && City.Instance.GetTile(coords.x - 1, coords.y) is IElectricGridElement leftElectricGridElement) { return leftElectricGridElement; }
			return null;
		}

		/// <summary>
		/// Return the list of buildings which are connected to the electric grid element
		/// </summary>
		/// <param name="electricGridElement">Electric grid element which should be checked</param>
		/// <returns>List of building that connected to electric grid element</returns>
		internal static List<Building> GetBuildingsByElectricGridElement(IElectricGridElement electricGridElement)
		{
			Vector3 coords = electricGridElement.GetTile().Coordinates;
			List<Building> buildings = new();

			if (City.Instance.GetTile(coords.x, coords.y + 1) is Building belowBuilding && belowBuilding.Rotation == Rotation.Zero) { lock (buildings) buildings.Add(belowBuilding); }
			if (City.Instance.GetTile(coords.x - 1, coords.y) is Building leftBuilding && leftBuilding.Rotation == Rotation.Ninety) { lock (buildings) buildings.Add(leftBuilding); }
			if (City.Instance.GetTile(coords.x, coords.y - 1) is Building aboveBuilding && aboveBuilding.Rotation == Rotation.OneEighty) { lock (buildings) buildings.Add(aboveBuilding); }
			if (City.Instance.GetTile(coords.x + 1, coords.y) is Building rightBuilding && rightBuilding.Rotation == Rotation.TwoSeventy) { lock (buildings) buildings.Add(rightBuilding); }

			return buildings;
		}

		internal static List<IElectricGridElement> GetPathOnElectric(IElectricGridElement from, IElectricGridElement to, int maxStep)
		{
			if (from == null || to == null) throw new ArgumentNullException(from + " or " + to + " is null");
			if (from.ElectricGrid != to.ElectricGrid) throw new ArgumentException("Not in the same electricgrid");

			maxStep = Mathf.Min(maxStep, from.ElectricGrid.ElectricGridElements.Count);

			Queue<(IElectricGridElement, int, List<IElectricGridElement>)> queue = new();
			queue.Enqueue((from, 0, new() { from }));
			while (queue.Count > 0)
			{
				(IElectricGridElement electricGridElement, int distance, List<IElectricGridElement> path) = queue.Dequeue();

				if (electricGridElement == to) return path;

				if (maxStep > distance)
				{
					foreach (IElectricGridElement element in electricGridElement.ConnectsTo)
					{
						if (path.Contains(element)) { continue; }

						List<IElectricGridElement> list = new(path) { element };
						queue.Enqueue((element, distance + 1, list));
					}
				}
			}

			throw new Exception("It is in the same electricgrid but not found");
		}
	}
}