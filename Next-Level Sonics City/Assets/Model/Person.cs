using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buildings;
using Persons;
using Model.Tiles.Buildings;

namespace Model
{
    public abstract class Person : MonoBehaviour
    {
        private static ulong s_id;
        public int Born { get; protected set; }
        public Residential LiveAt { get; protected set; }
        public Qualification Qualification { get; protected set; }

        public float GetHappiness()
        {
            //TODO
            throw new NotImplementedException();
        }

        public float GetTax()
        {
            //TODO
            throw new NotImplementedException();
        }

        public int GetAge()
        {
            //TODO
            throw new NotImplementedException();
        }

        public void MakeItSilly()
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}