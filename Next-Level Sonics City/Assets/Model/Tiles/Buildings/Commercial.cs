using System;
using System.Collections.Generic;

namespace Model.Tiles.Buildings
{
	public class Commercial : Building, IWorkplace, IZoneBuilding
	{
		public ZoneBuildingLevel Level { get; private set; }
		private List<Person> _workers;
		private int _workersLimit;

		public Commercial(int x, int y, uint designID) : base(x, y, designID)
		{
			Level = 0;
			_workersLimit = 10;
			_workers= new List<Person>();
		}

		public void LevelUp()
		{
			if (Level == ZoneBuildingLevel.THREE) { return; }
			++Level;
			_workersLimit += 5;
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
			return _workers.Remove(person);
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