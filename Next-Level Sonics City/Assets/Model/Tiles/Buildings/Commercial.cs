using System;
using System.Collections.Generic;
using Tiles;
using Model;

namespace Buildings
{
    public class Commercial : Building, IWorkplace, IZoneBuilding
    {
        public ZoneBuildingLevel Level { get; private set; }
        public int WorkerLimit { get; private set; }
        public List<Person> _workers;

        public Commercial()
        {
            Level = 0;
            WorkerLimit = 10;
            _workers= new List<Person>();
        }

        public void LevelUp()
        {
            ++Level;
            WorkerLimit += 5;
        }

        public bool Employ(Person person)
        {
            if (_workers.Count < WorkerLimit)
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

        public List<Person> GetPeople()
        {
            return _workers;
        }
    }
}