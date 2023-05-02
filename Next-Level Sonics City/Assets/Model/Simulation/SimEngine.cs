using UnityEngine;
using Model.Tiles;
using Model.Tiles.Buildings;
using Model.Statistics;

namespace Model.Simulation
{
	public class SimEngine : MonoBehaviour
	{
		private static SimEngine _instance;
		public static SimEngine Instance { get { return _instance; } }
		
		private Tile[,] _tiles;
		public City City;

		public ZoneManager ZoneManager;
		public BuildingManager BuildingManager;

		public RoadGridManager RoadGridManager;
		public StatEngine StatEngine;

		private void Init()
		{
			ZoneManager.ZoneMarked += StatEngine.SumMarkZonePrice;
			ZoneManager.ZoneUnMarked += StatEngine.SumUnMarkZonePrice;
			BuildingManager.BuildingBuilt += StatEngine.SumBuildPrice;
			BuildingManager.BuildingDestroyed += StatEngine.SumDestroyPrice;
		}

		public Tile GetTile(int x, int y)
		{
			if (!(0 <= x && x < _tiles.GetLength(0) && 0 <= y && y < _tiles.GetLength(1))) return null;

			return _tiles[x, y];
		}

		public void SetTile(int x, int y, Tile tile)
		{
			Tile old = _tiles[x, y];
			_tiles[x, y] = tile;
			GetTile(x - 1, y)?.NeighborTileChanged(old, tile);
			GetTile(x + 1, y)?.NeighborTileChanged(old, tile);
			GetTile(x, y - 1)?.NeighborTileChanged(old, tile);
			GetTile(x, y + 1)?.NeighborTileChanged(old, tile);
			old.Delete();
		}

		public int GetSize()
		{
			return _tiles.GetLength(0);
		}


		// Start is called before the first frame update
		void Start()
		{
			_instance = this;
			City = new();
			ZoneManager = new();
			BuildingManager = new();

			//DEMO CODE
			int n = 100;
			System.Random rnd = new ();
			_tiles = new Tile[n, n];

			for (int i = 0; i < n; i++)
			for (int j = 0; j < n; j++)
			{
				if (rnd.Next(0, 2) == 0)
					_tiles[i, j] = new EmptyTile(i, j, ResidentialBuildingTile.GenerateResidential(0));
				else
					_tiles[i, j] = new ResidentialBuildingTile(i, j, ResidentialBuildingTile.GenerateResidential((uint)rnd.Next(1, 6)));

				//Instance.Tiles[i, j] = new EmptyTile(i, j, ResidentialBuildingTile.GenerateResidential((uint)rnd.Next(1,6)));
			}
			//DEMO CODE

			Init();

			RoadGridManager = new();
			StatEngine = new();
		}

		// Update is called once per frame
		void Update()
		{
			
		}
	}
}
