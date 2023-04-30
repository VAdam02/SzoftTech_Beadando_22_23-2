using Model.Simulation;
using Model.Tiles.Buildings.BuildingCommands;
using System;

namespace Model.Tiles.Buildings
{
	public class BuildingManager
	{
		public delegate void BuildingBuiltOrDestroyedEventHandler(object sender, TileEventArgs e);
		public event BuildingBuiltOrDestroyedEventHandler BuildingBuilt;
		public event BuildingBuiltOrDestroyedEventHandler BuildingDestroyed;

		public BuildingManager() { }

		public void Build(Tile tile, TileType tileType, Rotation rotation)
		{
			if (tile is not EmptyTile)
			{
				return;
			}

			bool isExpandable = false;

			if (tile is Building)
			{
				isExpandable = ((Building)tile).IsExpandable() ? true : false;
			}

			if (isExpandable)
			{
				if (!((Building)tile).CanExpand(rotation))
				{
					return;
				}
			}

			int x = (int)tile.Coordinates.x;
			int y = (int)tile.Coordinates.y;
			Tile oldTile = SimEngine.Instance.GetTile(x, y);

			BuildCommand bc = new (x, y, tileType);
			bc.Execute();

			oldTile.OnTileDelete.Invoke();
			OnBuildingBuilt(tile);

			if (isExpandable)
			{
				Expand(tile, rotation);
			}
		}

		private void Expand(Tile baseTile, Rotation rotation)
		{
			int x1 = (int)baseTile.Coordinates.x;
			int y1 = (int)baseTile.Coordinates.y;
			int x2 = (int)baseTile.Coordinates.x;
			int y2 = (int)baseTile.Coordinates.y;

			if (baseTile is MiddleSchool)
			{
				switch (rotation)
				{
					case Rotation.Zero:
						x1 += 1;
						break;
					case Rotation.Ninety:
						y1 += 1;
						break;
					case Rotation.OneEighty:
						x1 += -1;
						break;
					case Rotation.TwoSeventy:
						y1 += -1;
						break;
				}
			}
			else
			{
				switch (rotation)
				{
					case Rotation.Zero:
						x1 += 1; y1 += 1;
						break;
					case Rotation.Ninety:
						x1 += -1; y1 += 1;
						break;
					case Rotation.OneEighty:
						x1 += -1; y1 += 1;
						break;
					case Rotation.TwoSeventy:
						x1 += 1; y1 += -1;
						break;
				}
			}

			int minX = Math.Min(x1, x2);
			int maxX = Math.Max(x1, x2);
			int minY = Math.Min(y1, y2);
			int maxY = Math.Max(y1, y2);

			for (int i = minX; minX < maxX; ++i)
			{
				for (int j = minY; minY < maxY; ++j)
				{
					Tile oldTile = SimEngine.Instance.GetTile(i, j);
					ExpandCommand ec = new ExpandCommand(i, j, (IWorkplace)baseTile);
					ec.Execute();
					oldTile.OnTileDelete.Invoke();
				}
			}
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

			SimEngine.Instance.GetTile(x, y).OnTileDelete.Invoke();
		}



		protected virtual void OnBuildingBuilt(Tile tile)
		{
			BuildingBuilt?.Invoke(this, new TileEventArgs(tile));
		}

		protected virtual void OnBuildingDestroyed(Tile tile)
		{
			BuildingDestroyed?.Invoke(this, new TileEventArgs(tile));
		}
	}
}