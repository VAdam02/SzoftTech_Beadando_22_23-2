using System;

namespace Model.Tiles
{
	public class TileEventArgs : EventArgs
	{
		public Tile Tile { get; set; }
		public TileEventArgs(Tile tile)
		{
			Tile = tile;
		}
	}
}