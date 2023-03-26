using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Tiles;

namespace Model{
    public class Navigator : MonoBehaviour
    {
        private Road _from;
        private Road _to;
        private List<Road> _path;

        public List<Road> GetPath()
        {
            throw new NotImplementedException();
        }
    }
}

