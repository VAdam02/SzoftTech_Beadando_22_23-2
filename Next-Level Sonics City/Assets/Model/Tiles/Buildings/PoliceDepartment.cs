using System.Collections;
using System.Collections.Generic;

namespace Model.Tiles.Buildings
{
	public class PoliceDepartment : Building, IWorkplace
	{
		private List<Person> _workers;
		private int _workersLimit;

		public PoliceDepartment()
		{

		}

		public bool Employ(Person person)
		{
			if (_workers.Count < _workersLimit)
			{
				_workers.Add(person);
				return true;
			}

			return false;
		}

		public bool Unemploy(Person person)
		{
			if (_workers.Count > 0)
			{
				_workers.Remove(person);
				return true;
			}

			return false;
		}

		public List<Person> GetWorkers()
		{
			return _workers;
		}

		public int GetWorkersCount()
		{
			return _workers.Count;
		}

		public int GetWorkersLimit()
		{
			return _workersLimit;
		}
	}
}