using Model.Persons;
using Model.RoadGrids;
using Model.Tiles;
using System.Collections.Generic;

namespace Model
{
	public interface IWorkplace
	{
		public int WorkplaceLimit { get; }

		/// <summary>
		/// Employ a worker
		/// </summary>
		/// <param name="worker">Worker who will be employed</param>
		public void Employ(Worker worker);

		/// <summary>
		/// Unemploy a worker
		/// </summary>
		/// <param name="worker">Worker who will be unemployed</param>
		public void Unemploy(Worker worker);

		/// <summary>
		/// Get all workers
		/// </summary>
		/// <returns>List of workers</returns>
		public List<Worker> GetWorkers();

		/// <summary>
		/// Register workplace to road grid
		/// </summary>
		/// <param name="roadGrid">Roadgrid where should be registered</param>
		public void RegisterWorkplace(RoadGrid roadGrid);

		/// <summary>
		/// Unregister workplace from road grid
		/// </summary>
		/// <param name="roadGrid">Roadgrid where should be unregistered</param>
		public void UnregisterWorkplace(RoadGrid roadGrid);

		/// <summary>
		/// Get count of workers
		/// </summary>
		/// <returns>Count of workers</returns>
		public int GetWorkersCount();

		/// <summary>
		/// Get the tile of the workplace
		/// </summary>
		/// <returns>Tile of the workplace</returns>
		public Tile GetTile();

		public (float happiness, float weight) HappinessByBuilding { get; }

		/// <summary>
		/// Register happy zone as a possible happiness changer
		/// </summary>
		/// <param name="zone">Zone that should be calculated into happiness as a changer</param>
		public void RegisterHappinessChangerTile(IHappyZone zone);
	}
}