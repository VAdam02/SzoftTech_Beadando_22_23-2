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
        public List<Person> Workers { get; private set; }

        public Industrial()
        {
            Level = 0;
            WorkerLimit = 10;
            Workers = new List<Person>();
        }

        public void LevelUp()
        {
            ++Level;
            WorkerLimit += 5;
        }

        public bool Employ(Person person)
        {
            if (Workers.Count < WorkerLimit)
            {
                Workers.Add(person);
                return true;
            }

            return false;
        }

        public void Fire(Person person)
        {
            throw new NotImplementedException();
        }
    }
}