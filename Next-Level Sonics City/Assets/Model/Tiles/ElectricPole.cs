using System.Collections;
using System.Collections.Generic;
using Model.Service;

namespace Model.Tiles
{
	public class ElectricPole : Tile, IPowerGridElement
	{
		public ElectricPole(int x, int y, uint designID) : base(x, y, designID)
		{

		}

		public override TileType GetTileType() { return TileType.ElectricPole; }
	}
}
