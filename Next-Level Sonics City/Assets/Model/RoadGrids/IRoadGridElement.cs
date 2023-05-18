using System.Collections.Generic;

namespace Model.RoadGrids
{
	public interface IRoadGridElement
	{
		/// <summary>
		/// Register this element in the road grid
		/// </summary>
		public void RegisterRoadGridElement();

		/// <summary>
		/// Unregister this element from the road grid
		/// </summary>
		public void UnregisterRoadGridElement();

		/// <summary>
		/// Returns a list of elements this element connects to
		/// </summary>
		/// <returns>List of elements this connects to</returns>
		public List<IRoadGridElement> ConnectsTo();

		/// <summary>
		/// Returns the road grid this belongs to
		/// </summary>
		/// <returns>Current road grid</returns>
		public RoadGrid GetRoadGrid();

		/// <summary>
		/// Sets the road grid this belongs to
		/// This also contains the logic to register and unregister this element and other types depending on this
		/// </summary>
		/// <param name="roadGrid">New road grid</param>
		public void SetRoadGrid(RoadGrid roadGrid);

		/// <summary>
		/// Returns the tile this element is on
		/// </summary>
		/// <returns>Tile for the element</returns>
		public Tile GetTile();
	}
}