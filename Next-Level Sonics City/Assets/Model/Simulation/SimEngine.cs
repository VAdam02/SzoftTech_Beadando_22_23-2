using UnityEngine;
using Model.Tiles;
using Model.Tiles.Buildings;
using Model.Statistics;
using Model.RoadGrids;
using System.Collections.Generic;
using System;
using System.Threading;

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

		private float _tax;
		private List<Person> _people;

		private void Init()
		{
			//TODO FIX IT
			//ZoneManager.ZoneMarked += StatEngine.SumMarkZonePrice;
			//ZoneManager.ZoneUnMarked += StatEngine.SumUnMarkZonePrice;
			//BuildingManager.BuildingBuilt += StatEngine.SumBuildPrice;
			//BuildingManager.BuildingDestroyed += StatEngine.SumDestroyPrice;
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
			tile.FinalizeTile();
			GetTile(x - 1, y)?.NeighborTileChanged(old, tile);
			GetTile(x + 1, y)?.NeighborTileChanged(old, tile);
			GetTile(x, y - 1)?.NeighborTileChanged(old, tile);
			GetTile(x, y + 1)?.NeighborTileChanged(old, tile);
			old?.Delete();
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
			RoadGridManager = new();

			//DEMO CODE
			int n = 7;
			_tiles = new Tile[n, n];

			long startTime = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;

			for (int i = 0; i < n; i++)
			for (int j = 0; j < (n-1); j++)
			{
				if (i % 3 == 0)
				{
					SetTile(i, j, new RoadTile(i, j, 0));
				}
				/*
				else if (i % 3 == 1 && j % 2 == 0)
				{
					Building stadion = new StadionBuildingTile(i, j, 0, Rotation.Zero);
					SetTile(i, j, stadion);
					stadion.Expand();
				}
				else if (i % 3 == 2 || i % 3 == 1 && j % 2 == 0)
				{

				}
				*/
				else
				{
					//_tiles[i, j] = new Industrial(i, j, 0);
					//_tiles[i, j] = new ResidentialBuildingTile(i, j, ResidentialBuildingTile.GenerateResidential((uint)new System.Random().Next(1, 6)));
					//_tiles[i, j] = new Commercial(i, j, 0);
					//_tiles[i, j] = new PoliceDepartmentBuildingTile(i, j, 0, Rotation.TwoSeventy);
					SetTile(i, j, new EmptyTile(i, j, 0));
				}
			}

			Debug.Log("SimEngine tile generation takes up " + ((System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond) - startTime) + " ms");

			for (int k = 0; k < n; k++)
			{
				SetTile(k, n-1, new RoadTile(k, n-1, 0));
			}

			foreach (RoadGrid grid in RoadGridManager.RoadGrids)
			{
				Debug.Log(grid.Workplaces.Count + " IWorkplace\t" + grid.Residentials.Count + " IResidential\t" + grid.RoadGridElements.Count + " IRoadGridElement");
			}

			Debug.Log("DESTROY START");
			startTime = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
			BuildingManager.Destroy(GetTile(2, n-1));
			Debug.Log("Destroy takes up " + ((System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond) - startTime) + " ms");
			Debug.Log("DESTROY FINISH");

			foreach (RoadGrid grid in RoadGridManager.RoadGrids)
			{
				Debug.Log(grid.Workplaces.Count + " IWorkplace\t" + grid.Residentials.Count + " IResidential\t" + grid.RoadGridElements.Count + " IRoadGridElement");
			}
			
			Debug.Log("BUILD START");
			startTime = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
			BuildingManager.Build(GetTile(2, n-1), TileType.Road, 0);
			Debug.Log("BUILD takes up " + ((System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond) - startTime) + " ms");
			Debug.Log("BUILD FINISH");
			
			foreach (RoadGrid grid in RoadGridManager.RoadGrids)
			{
				Debug.Log(grid.Workplaces.Count + " IWorkplace\t" + grid.Residentials.Count + " IResidential\t" + grid.RoadGridElements.Count + " IRoadGridElement");
			}
			//DEMO CODE

			Init();

			StatEngine = new(2020, 100000);
			StartSimulation();
		}

		// Update is called once per frame
		void Update()
		{

		}

		/// <summary>
		/// Called once when the simulation should do a cycle
		/// </summary>
		private static void Tick()
		{
			//Do the things that should done during a tick
			Instance.StatEngine.TimeElapsed();
		}

		private bool BuildByPeople(Tile t, ZoneBuilding z)
		{
			throw new NotImplementedException();
			//TODO
		}
		
		private bool LevelUpZone(Building b)
		{
			throw new NotImplementedException();
			//TODO
		}

		public int GetPriceMarkZone(List<Tile> tile, ZoneBuilding z)
		{
			throw new NotImplementedException();
			//TODO
		}
		public int GetPriceRemoveZone(List<Tile> tiles)
		{
			throw new NotImplementedException();
			//TODO
		}
		public int GetPriceBuildService(Tile tile, ServiceBuilding sb)
		{
			throw new NotImplementedException();
			//TODO
		}
		public int GetPriceDestroy(Tile tile)
		{
			
			throw new NotImplementedException();
			//TODO
		}
		
		public int GetPriceLevelUpZone(Building building)
		{
			throw new NotImplementedException();
			//TODO
		}

		public void SetTax(float f)
		{
			_tax = f;
			
			//TODO
		}
		private bool MoveIn(int i)
		{/*
			bool move_in = true;
			
			foreach(Person p in _people)
			{
				if(p.GetHappiness() < 0.5)
				{
					p.MoveIn()
				}
			}*/
			throw new NotImplementedException();
			//TODO
		}
		private bool MoveOut(int i)
		{
			throw new NotImplementedException();
			//TODO
		}
		private void Die(Person person)
		{
			_people.Remove(person);
			//TODO
		}

		public List<Building> GetBuildingsOnFire()
		{
			throw new NotImplementedException();
			//TODO
		}
		
		public int GetTimeSpeed()
		{
			return _timeSpeed;
		}
		public void SetTimeSpeed(int speed)
		{
			_timeSpeed = speed;
		}

		#region Thread
		private static int _timeSpeed = 1;
		private static readonly int _tps = 10;
		private static Thread _t;

		private static readonly object _lock = new();		//lock for _isSimulating and _isRunning
		private static bool _isSimulating = false;			//dont modify
		private static bool _isRunning = false;				//dont modify

		private static readonly object _pauseLock = new();	//lock for _isPaused
		private static bool _isPaused = false;				//dont modify

		/// <summary>
		/// Run the ticking loop
		/// </summary>
		private static void ThreadProc()
		{
			lock (_lock)
			{
				if (_isRunning) { return; }
				_isRunning = true;
				_isSimulating = true;
			}

			long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			while (_isSimulating)
			{
				//TICK
				if (!_isPaused) { Tick(); }

				//TICKING DELAY
				long sleepTime = 1000 / (_tps * _timeSpeed) - (DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime);
				if (sleepTime > 0) { Thread.Sleep((int)sleepTime); }
				else { Debug.LogWarning("Last tick took " + (-sleepTime) + "ms longer than the maximum time"); }
				startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			}

			lock (_lock)
			{
				_isRunning = false;
			}
		}

		/// <summary>
		/// Start the simulation
		/// </summary>
		private static void StartSimulation()
		{
			_t ??= new Thread(new ThreadStart(ThreadProc));
			_t.Start();
		}

		/// <summary>
		/// Stop the simulation
		/// </summary>
		private static void StopSimulation()
		{
			lock (_lock)
			{
				_isSimulating = false;
			}
		}

		private void OnApplicationQuit()
		{
			StopSimulation();
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			lock (_pauseLock)
			{
				_isPaused = !hasFocus;
			}
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			lock (_pauseLock)
			{
				_isPaused = pauseStatus;
			}
		}
		#endregion
	}
}
