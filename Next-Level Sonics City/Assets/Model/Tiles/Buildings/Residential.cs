using System.Collections.Generic;
using Tiles;
using Model;

namespace Buildings
{
    public class Residential : Building, IZoneBuilding
    {
        public BuildingLevel Level{ get; private set; }
        public int ResidentLimit { get; private set; }
        public List<Person> Residents { get; private set; }

        public Residential()
        {
            Level = 0;
            ResidentLimit = 5;
            Residents= new List<Person>();
        }

        public void LevelUp()
        {
            ++Level;
            ResidentLimit += 5;
        }

        public bool MoveIn(Person person)
        {
            if (Residents.Count < ResidentLimit)
            {
                Residents.Add(person);
                return true;
            }
            
            return false;
        }
    }
}