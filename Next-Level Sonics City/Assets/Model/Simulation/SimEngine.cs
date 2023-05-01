using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Model.Tiles;
using System.Threading;
using Model.Tiles.Buildings;
using Model.Statistics;

namespace Model.Simulation
{
	public class SimEngine : MonoBehaviour
	{
		private static SimEngine _instance;
		public static SimEngine Instance { get { return _instance; } }
		
		private Tile[,] _tiles;

		public readonly StatEngine StatEngine = new();
		public readonly ZoneManager ZoneManager = new();
		public readonly BuildingManager BuildingManager = new();
		
		private void Init()
		{
			ZoneManager.ZoneMarked += StatEngine.SumMarkZonePrice;
			ZoneManager.ZoneUnMarked += StatEngine.SumUnMarkZonePrice;
			BuildingManager.BuildingBuilt += StatEngine.SumBuildPrice;
			BuildingManager.BuildingDestroyed += StatEngine.SumDestroyPrice;
		}

		public Tile GetTile(int x, int y)
		{
			return _tiles[x, y];
		}

		public void SetTile(int x, int y, Tile tile)
		{
			Tile old = _tiles[x, y];
			_tiles[x, y] = tile;
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
			Init();

			//DEMO CODE
			int n = 3;
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
		}

		// Update is called once per frame
		void Update()
		{
			
		}
	}
}
