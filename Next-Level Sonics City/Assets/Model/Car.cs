using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Tiles;
using Model;



namespace Model
{
    public class Car : MonoBehaviour
    {
        private int _design;
        private bool _isDuty;
        private Building _from;
        private Building _to;
        private Road _isAt;
        private Navigator navigator;


        public int GetDesign()
        {
            //TODO
            throw new NotImplementedException();
        }
        public bool IsOnDuty()
        {
            throw new NotImplementedException();
            //TODO
        }
        public Road GetLocation()
        {
            throw new NotImplementedException();
            //TODO
        }
        public Navigator GetNavigator()
        {
            throw new NotImplementedException();
            //TODO
        }
    }
}
