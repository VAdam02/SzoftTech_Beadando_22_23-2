using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Tiles
{
	public class EmptyTile : Tile
	{
		public EmptyTile(int x, int y, uint designID) : base(x, y, designID)
		{

		}

		public override TileType GetTileType() { throw new InvalidOperationException(); }
	}
}