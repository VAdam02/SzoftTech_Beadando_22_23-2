using System.Collections.Generic;
using Tiles;
using Model;
using System;

namespace Buildings
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

        public void LevelUp()
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

        public void Fire(Person person)
        {
            throw new NotImplementedException();
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