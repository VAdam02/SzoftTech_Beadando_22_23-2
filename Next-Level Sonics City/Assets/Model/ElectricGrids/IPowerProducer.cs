namespace Model.ElectricGrids
{
	public interface IPowerProducer
	{
		/// <summary>
		/// Register a producer zone to the electricc grid
		/// </summary>
		/// <param name="electricGrid">Electricgrid where to register</param>
		public void RegisterPowerProducer(ElectricGrid elctricGrid);

		/// <summary>
		/// Unregister a producer zone from the elctric grid
		/// </summary>
		/// <param name="electricGrid">Electricgrid where from unregister</param>
		public void UnregisterPowerProducer(ElectricGrid electricGrid);

		public int GetPowerProduction();

		/// <summary>
		/// Get the tile of the power producer
		/// </summary>
		/// <returns>Tile of the power producer</returns>
		public Tile GetTile();
	}
}