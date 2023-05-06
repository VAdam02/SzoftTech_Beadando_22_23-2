using System.Collections.Generic;

namespace Model
{
	public interface IWorkplace
	{
		public void RegisterWorkplace();
		public void UnregisterWorkplace();

		public bool Employ(Person person);
		public bool Unemploy(Person person);
		public List<Person> GetWorkers();
		public int GetWorkersCount();
		public int GetWorkersLimit();

		public Tile GetTile();
	}
}