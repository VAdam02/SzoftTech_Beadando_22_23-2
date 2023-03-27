using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    public interface IZoneBuilding
    {
        public abstract void LevelUp();
        public abstract List<Person> GetPeople(); 
    }
}