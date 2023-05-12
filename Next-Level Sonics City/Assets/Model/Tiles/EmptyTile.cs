using Model.Simulation;
using System;

namespace Model.Tiles
{
	public class EmptyTile : Tile
	{
		public EmptyTile(int x, int y, uint designID) : base(x, y, designID)
		{

		}

		public override TileType GetTileType() { throw new InvalidOperationException(); }

		internal override bool CanBuild()
		{
			return SimEngine.Instance.GetTile((int)Coordinates.x, (int)Coordinates.y) is not EmptyTile;
		}
	}
}