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
		public List<IRoadGridElement> ConnectsTo { get; }

		/// <summary>
		/// Returns the IRoadGridElement from above
		/// </summary>
		public IRoadGridElement ConnectsFromAbove { get; }
		
		/// <summary>
		/// Returns the IRoadGridElement from right
		/// </summary>
		public IRoadGridElement ConnectsFromRight { get; }

		/// <summary>
		/// Returns the IRoadGridElement from below
		/// </summary>
		public IRoadGridElement ConnectsFromBelow { get; }

		/// <summary>
		/// Returns the IRoadGridElement from left
		/// </summary>
		public IRoadGridElement ConnectsFromLeft { get; }

		/// <summary>
		/// Returns the road grid this belongs to
		/// </summary>
		public RoadGrid RoadGrid { get; set; }

		public bool IsLocked { get; }
		public void LockBy(Person person);
		public void UnlockBy(Person person);

		/// <summary>
		/// Returns the tile this element is on
		/// </summary>
		/// <returns>Tile for the element</returns>
		public Tile GetTile();
	}
}