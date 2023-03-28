using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;
using Persons;

namespace Model
{
    public abstract class Person : MonoBehaviour
    {
        private static ulong s_id;
        public int Age { get; protected set; }
        public Residential LiveAt { get; protected set; }

        public float GetHappiness()
        {
            //TODO
            throw new NotImplementedException();
        }

        public void IncreaseAge()
        {
            ++Age;
        }

        public abstract float PayTax(float taxRate);
    }
}