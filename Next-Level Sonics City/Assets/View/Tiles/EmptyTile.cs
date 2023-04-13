using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View.Tiles
{
    public class EmptyTile : Tile
    {
        // Start is called before the first frame update
        void Start()
        {
            transform.localPosition = new Vector3(TileModel.Coordinates.x, 0, TileModel.Coordinates.y) * 10;
			transform.localScale = Vector3.one;
		}

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}