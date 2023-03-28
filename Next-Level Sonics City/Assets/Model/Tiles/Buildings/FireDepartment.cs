using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tiles;
using Model;
using System;

namespace Buildings
{
    public class FireDepartment : Building, IWorkplace
    {
        private List<Person> _workers;
        private int _workersLimit;

        public FireDepartment()
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