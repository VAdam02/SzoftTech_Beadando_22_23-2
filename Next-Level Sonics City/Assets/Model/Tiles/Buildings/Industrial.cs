using System.Collections.Generic;

namespace Model.Tiles.Buildings
{
    public class Industrial : Building, IWorkplace, IZoneBuilding
    {
        public BuildingLevel Level { get; private set; }
        private List<Person> _workers;
        private int _workersLimit;

        public Industrial()
        {
            Level = 0;
            _workersLimit = 10;
            _workers = new List<Person>();
        }

        public void LevelUp()//TODO limit upgrading logic
        {
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