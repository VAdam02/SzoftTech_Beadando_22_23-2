using System.Collections.Generic;
using Tiles;
using Model;

namespace Buildings
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

        public void LevelUp()//TODO limit upgrading logic
        {
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
            if (_residents.Count > 0)
            {
                _residents.Remove(person);
                return true;
            }

            return false;
        }

        public List<Person> GetResidents()
        {
            return _residents;
        }
    }
}