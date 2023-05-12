using System.Collections.Generic;
using Model.RoadGrids;

namespace Model
{
	public interface IWorkplace
	{
		public void RegisterWorkplace(RoadGrid roadGrid);
		public void UnregisterWorkplace(RoadGrid roadGrid);

		public bool Employ(Person person);
		public bool Unemploy(Person person);
		public List<Person> GetWorkers();
		public int GetWorkersCount();
		public int GetWorkersLimit();

		public Tile GetTile();
	}
}