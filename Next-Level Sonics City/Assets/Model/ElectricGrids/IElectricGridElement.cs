using System.Collections.Generic;

namespace Model.ElectricGrids
{
	public interface IElectricGridElement
	{
		/// <summary>
		/// Register this element in the electric grid
		/// </summary>
		public void RegisterElectricGridElement();

		/// <summary>
		/// Unregister this element from the electric grid
		/// </summary>
		public void UnregisterElectricGridElement();

		/// <summary>
		/// Returns a list of elements this element connects to
		/// </summary>
		public List<IElectricGridElement> ConnectsTo { get; }

		/// <summary>
		/// Returns the IElectricGridElement from above
		/// </summary>
		public IElectricGridElement ConnectsFromAbove { get; }

		/// <summary>
		/// Returns the IElectricGridElement from right
		/// </summary>
		public IElectricGridElement ConnectsFromRight { get; }

		/// <summary>
		/// Returns the IElectricGridElement from below
		/// </summary>
		public IElectricGridElement ConnectsFromBelow { get; }

		/// <summary>
		/// Returns the IElectricGridElement from left
		/// </summary>
		public IElectricGridElement ConnectsFromLeft { get; }

		/// <summary>
		/// Returns the electric grid this belongs to
		/// </summary>
		public ElectricGrid ElectricGrid { get; set; }

		/// <summary>
		/// Returns the tile this element is on
		/// </summary>
		/// <returns>Tile for the element</returns>
		public Tile GetTile();
	}
}