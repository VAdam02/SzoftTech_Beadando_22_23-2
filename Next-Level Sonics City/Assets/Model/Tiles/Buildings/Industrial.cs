using System.Collections.Generic;
using Tiles;
using Model;
using System;

namespace Buildings
{
    public class Industrial : Building, IWorkplace, IZoneBuilding
    {
<<<<<<< Updated upstream
        public BuildingLevel Level { get; private set; }
        public int WorkerLimit { get; private set; }
        public List<Person> _workers;

        public Industrial()
        {
            Level = 0;
            WorkerLimit = 10;
=======
<<<<<<< Updated upstream

=======
        public BuildingLevel Level { get; private set; }
        private List<Person> _workers;
        private int _workersLimit;

        public Industrial()
        {
            Level = 0;
            _workersLimit = 10;
>>>>>>> Stashed changes
            _workers = new List<Person>();
        }

        public void LevelUp()
        {
            ++Level;
<<<<<<< Updated upstream
            WorkerLimit += 5;
=======
            _workersLimit += 5;
>>>>>>> Stashed changes
        }

        public bool Employ(Person person)
        {
<<<<<<< Updated upstream
            if (_workers.Count < WorkerLimit)
=======
            if (_workers.Count < _workersLimit)
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
        public List<Person> GetPeople()
        {
            return _workers;
        }
=======
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
>>>>>>> Stashed changes
>>>>>>> Stashed changes
    }
}