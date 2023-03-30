using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;

namespace Model.Tiles
{
    public class EmptyTile : Tile
    {
        public EmptyTile(int x, int y)
        {
            Coordinates = new Vector3(x, y, 0);
        }
    }
}