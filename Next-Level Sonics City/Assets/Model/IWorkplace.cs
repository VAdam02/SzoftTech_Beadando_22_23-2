using Model.Persons;
using System.Collections.Generic;

namespace Model
{
	public interface IWorkplace
	{
		public bool Employ(Worker worker);
		public bool Unemploy(Worker worker);
		public List<Worker> GetWorkers();
		public int GetWorkersCount();
		public int GetWorkersLimit();

		public Tile GetTile();
	}
}