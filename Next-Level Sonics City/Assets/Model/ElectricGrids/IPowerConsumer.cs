namespace Model.ElectricGrids
{
	public interface IPowerConsumer
	{
		/// <summary>
		/// Register a consumer zone to the electricc grid
		/// </summary>
		/// <param name="electricGrid">Electricgrid where to register</param>
		public void RegisterPowerConsumer(ElectricGrid elctricGrid);

		/// <summary>
		/// Unregister a consumer zone from the elctric grid
		/// </summary>
		/// <param name="electricGrid">Electricgrid where from unregister</param>
		public void UnregisterPowerConsumer(ElectricGrid electricGrid);

		/// <summary>
		/// Get the power consumption of the building
		/// </summary>
		/// <returns></returns>
		public int GetPowerConsumption();

		/// <summary>
		/// Get the tile of the power consumer
		/// </summary>
		/// <returns>Tile of the power consumer</returns>
		public Tile GetTile();
	}
}