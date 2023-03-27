using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tiles;
using Model;

namespace Buildings
{
    public class Commercial : Building
    {
        public BuildingLevel Level { get; private set; }
        public List<Person> Population { get; private set; }
        public int PopulationLimit { get; private set; }

        public Commercial()
        {
            Population = new List<Person>();
            Level = 0;
            PopulationLimit = 5;
        }

        public void LevelUp()
        {
            ++Level;
            PopulationLimit += 5; //?
        }

        public bool MoveIn(Person person)
        {
            if (Population.Count < PopulationLimit)
            {
                Population.Add(person);
                return true;
            }

            return false;
        }
    }
}