using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using Tiles;
using UnityEngine;

namespace Buildings
{
    public class PowerPlant : Building, IWorkplace
    {
        private List<Person> _workers;
        private int _workersLimit;

        public PowerPlant()
        {

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