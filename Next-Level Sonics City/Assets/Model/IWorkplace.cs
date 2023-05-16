using Model.Persons;
using System.Collections.Generic;
using Model.RoadGrids;

namespace Model
{
	public interface IWorkplace
	{
		public bool Employ(Worker worker);
		public bool Unemploy(Worker worker);
		public List<Worker> GetWorkers();
		public void RegisterWorkplace(RoadGrid roadGrid);
		public void UnregisterWorkplace(RoadGrid roadGrid);
		public int GetWorkersCount();
		public int GetWorkersLimit();
		public Tile GetTile();
	}
}