namespace Model
{
	public interface ITransparent
	{
		/// <summary>
		/// Returns the tile this element is on
		/// </summary>
		/// <returns>Tile which is transparent</returns>
		public Tile GetTile();

		/// <summary>
		/// Returns the tile transparency for the effects
		/// </summary>
		/// <returns></returns>
		public float GetTransparency();
	}
}
