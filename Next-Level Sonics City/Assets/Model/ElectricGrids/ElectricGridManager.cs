using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

			electricGridElement.ElectricGrid = new();
		}
	}
}