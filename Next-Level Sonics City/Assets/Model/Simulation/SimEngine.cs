using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Model.Tiles;
using System.Threading;
using Model.Tiles.Buildings;

namespace Model.Simulation
{
	public class SimEngine : MonoBehaviour
	{
		private static SimEngine _instance;
		public static SimEngine Instance { get { return _instance; } }
		
		private Tile[,] _tiles;
		public Tile[,] Tiles { get { return _tiles; } private set { _tiles = value; } }

		// Start is called before the first frame update
		void Start()
		{
			_instance = this;

			//DEMO CODE
			int n = 100;
			System.Random rnd = new ();
			Instance.Tiles = new Tile[n, n];

			for (int i = 0; i < n; i++)
			for (int j = 0; j < n; j++)
			{
				if (rnd.Next(0, 2) == 0)
					Instance.Tiles[i, j] = new EmptyTile(i, j, ResidentialBuildingTile.GenerateResidential(0));
				else
					Instance.Tiles[i, j] = new ResidentialBuildingTile(i, j, ResidentialBuildingTile.GenerateResidential((uint)rnd.Next(1, 6)));

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
