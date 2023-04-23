namespace Model.Tiles.Buildings
{
	public class BuildingManager
	{
		public BuildingManager() { }

		public void Build(Tile tile, TileType tileType)
		{
			if (tile is not EmptyTile)
			{
				return;
			}

			int x = (int)tile.Coordinates.x;
			int y = (int)tile.Coordinates.y;

			BuildCommand bc = new (x, y, tileType);
			bc.Execute();
		}

		public void Destroy(Tile tile)
		{
			if (tile is EmptyTile)
			{
				return;
			}

			int x = (int)tile.Coordinates.x;
			int y = (int)tile.Coordinates.y;

			DestroyCommand dc = new (x, y);
			dc.Execute();
		}
	}
}