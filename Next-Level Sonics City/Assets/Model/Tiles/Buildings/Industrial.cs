using System.Collections.Generic;
using Tiles;
using Model;
using System;

namespace Buildings
{
    public class Industrial : Building
    {
        public BuildingLevel Level { get; private set; }
        public int WorkerLimit { get; private set; }
        public List<Person> _workers;

        public Industrial()
        {
            Level = 0;
            WorkerLimit = 10;
            _workers = new List<Person>();
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