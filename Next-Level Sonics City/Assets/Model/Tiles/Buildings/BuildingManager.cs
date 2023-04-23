using Model.Simulation;
using System;

namespace Model.Tiles.Buildings
{
	public class BuildingManager
	{
		public delegate void BuildingBuiltOrDestroyedEventHandler(object sender, TileEventArgs e);
		public event BuildingBuiltOrDestroyedEventHandler BuildingBuilt;
		public event BuildingBuiltOrDestroyedEventHandler BuildingDestroyed;

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

			SimEngine.Instance.Tiles[x, y].OnTileChange.Invoke();
			OnBuildingBuilt(tile);
		}

		public void Destroy(Tile tile)
		{
			if (tile is EmptyTile)
			{
				return;
			}

			int x = (int)tile.Coordinates.x;
			int y = (int)tile.Coordinates.y;

			OnBuildingDestroyed(tile);

			DestroyCommand dc = new (x, y);
			dc.Execute();

			SimEngine.Instance.Tiles[x, y].OnTileChange.Invoke();
		}

		protected virtual void OnBuildingBuilt(Tile tile)
		{
			if (BuildingBuilt is not null)
			{
				BuildingBuilt.Invoke(this, new TileEventArgs(tile));
			}
		}

		protected virtual void OnBuildingDestroyed(Tile tile)
		{
			if (BuildingDestroyed is not null)
			{
				BuildingDestroyed.Invoke(this, new TileEventArgs(tile));
			}
		}
	}
}