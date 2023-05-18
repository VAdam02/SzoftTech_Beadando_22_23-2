namespace Model.Tiles
{
	public interface IHappyZone
	{
		/// <summary>
		/// Get the radius of the happy zone
		/// </summary>
		/// <returns>Get max distance where it makes sense</returns>
		public int GetEffectiveRadius();

		/// <summary>
		/// Get the happiness at the given tile
		/// </summary>
		/// <param name="tile">Tile for reference where to calculate the effect</param>
		/// <returns>Value of the happiness modifier made by this</returns>
		public (float happiness, float weight) GetHappinessModifierAtTile(Tile tile);

		public Tile GetTile();
	}
}
