using Model.Tiles.Buildings.BuildingCommands;

namespace Model.Tiles.Buildings
{
	public class BuildingManager
	{
		private static BuildingManager _instance;
		public static BuildingManager Instance
		{
			get
			{
				_instance ??= new BuildingManager();
				return _instance;
			}
		}
		private BuildingManager() { }

		public delegate void BuildingBuiltOrDestroyedEventHandler(object sender, TileEventArgs e);
		public event BuildingBuiltOrDestroyedEventHandler BuildingBuilt;
		public event BuildingBuiltOrDestroyedEventHandler BuildingDestroyed;

		/// <summary>
		/// Builds a building on the given tile.
		/// </summary>
		/// <param name="tile">Tile where the tile will created</param>
		/// <param name="tileType">Type of the tile</param>
		/// <param name="rotation">Rotation of tile</param>
		public void Build(Tile tile, TileType tileType, Rotation rotation)
		{
			bool isRoad = false;
			if (tile is RoadTile && tileType is TileType.ElectricRoad)
			{
				isRoad = true;
			}

			if (tile is not EmptyTile && !isRoad)
			{
				return;
			}

			int x = (int)tile.Coordinates.x;
			int y = (int)tile.Coordinates.y;

			BuildCommand bc = new (x, y, tileType, rotation);
			bc.Execute();

			OnBuildingBuilt(tile);
		}
		
		/// <summary>
		/// Destroys the given tile.
		/// </summary>
		/// <param name="tile">Tile that will be destroyed</param>
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

			if (City.Instance.GetTile(x, y) is EmptyTile)
			{
				OnBuildingDestroyed(tile);
			}
		}

		/// <summary>
		/// Force destroys the given tile.
		/// </summary>
		/// <param name="tile">Tile that will be destroyed</param>
		public void ForcedDestroy(Tile tile)
		{
			if (tile is EmptyTile)
			{
				return;
			}

			int x = (int)tile.Coordinates.x;
			int y = (int)tile.Coordinates.y;

			ForcedDestroyCommand fdc = new(x, y);
			fdc.Execute();

			OnBuildingDestroyed(tile);
		}

		/// <summary>
		/// Invokes the BuildingBuilt event.
		/// </summary>
		/// <param name="tile">Tile of the old building</param>
		protected virtual void OnBuildingBuilt(Tile tile)
		{
			BuildingBuilt?.Invoke(this, new TileEventArgs(tile));
		}

		/// <summary>
		/// Invokes the BuildingDestroyed event.
		/// </summary>
		/// <param name="tile">Tile of the old building</param>
		protected virtual void OnBuildingDestroyed(Tile tile)
		{
			BuildingDestroyed?.Invoke(this, new TileEventArgs(tile));
		}
	}
}