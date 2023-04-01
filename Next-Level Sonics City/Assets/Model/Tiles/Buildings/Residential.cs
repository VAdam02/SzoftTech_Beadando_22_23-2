using System.Collections.Generic;

namespace Model.Tiles.Buildings
{
	public class Residential : Building, IZoneBuilding
	{
		public BuildingLevel Level{ get; private set; }
		public int ResidentLimit { get; private set; }
		private List<Person> _residents;

		public Residential()
		{
			Level = 0;
			ResidentLimit = 5;
			_residents= new List<Person>();
		}

		public void LevelUp()
		{
			if (Level == BuildingLevel.THREE) { return; }
			++Level;
			ResidentLimit += 5;
		}

		public bool MoveIn(Person person)
		{
			if (_residents.Count < ResidentLimit)
			{
				_residents.Add(person);
				return true;
			}
			
			return false;
		}

		public bool MoveOut(Person person)
		{
			return _residents.Remove(person);
		}

		public List<Person> GetResidents()
		{
			return _residents;
		}
	}
}