using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Model;
using Model.Tiles;
using System.Threading;
using Model.Tiles.Buildings;
using Model.Statistics;
using System.Linq;
using Model.Persons;

namespace Model.Simulation
{
	public class SimEngine : MonoBehaviour
	{
		StatReport report = new StatReport();
		private static SimEngine _instance;

		public static SimEngine Instance { get { return _instance; } }
		
		public Tile[,] Tiles { get; private set; }
		
		public SortedDictionary<int,Person> Personslist = new SortedDictionary<int, Person>();
		public List<ResidentialBuildingTile>freeResidentals = new List<ResidentialBuildingTile>();
		
		public List<IWorkplace>freeWorkplaces = new List<IWorkplace>();

		public Worker worker;

		private float _money;
		private float _tax;
		private DateTime _date;
		private City _city;
		private List<Car> _carsOnRoad;
		private int _timeSpeed;
		//private List<Person> _people;
		private StatEngine _statEngine;

		// Start is called before the first frame update
		void Start()
		{
			_instance = this;
			Init();
			StartSimulation();
		}

		// Update is called once per frame
		void Update()
		{

		}

		/// <summary>
		/// Initialize things before the starting of the simulation
		/// </summary>
		private void Init()
		{
			int size = 100;
			System.Random rnd = new();
			Tiles = new Tile[size, size];
			for (int i = 0; i < Tiles.GetLength(0); i++)
			for (int j = 0; j < Tiles.GetLength(1); j++)
			{
				
				if (rnd.Next(0, 2) < 1)
				{
					Tiles[i, j] = new EmptyTile(i, j, 0);
				}
				else
				{
					Tiles[i, j] = new ResidentialBuildingTile(i, j, (uint)rnd.Next(int.MinValue, int.MaxValue) + int.MaxValue);
				}
			}
		}

		/// <summary>
		/// Called once when the simulation should do a cycle
		/// </summary>
		private static void Tick()
		{
            //Do the things that should done during a tick
            // temporary solution
            System.Random rand = new();
            int be = rand.Next(1,6);
			int ki = rand.Next(1,6);
			
			_instance.report.PopulationChange = be - ki;
			int startindex = _instance.Personslist.Keys.Last();
			if(_instance.report.PopulationChange > 0){
				for(int i = startindex+1;i< _instance.report.PopulationChange+1;i++){
					//mág a worker paraméterei nincsenek meg
					int age = rand.Next(18,65);
					Qualification randomq = (Qualification)new System.Random().Next(0,Enum.GetValues(typeof(Qualification)).Length);
					int randResidental = rand.Next(0,_instance.freeResidentals.Count);
					ResidentialBuildingTile home = _instance.freeResidentals[randResidental];
					int randWorkplace = rand.Next(0,_instance.freeWorkplaces.Count);
					IWorkplace workPlace = _instance.freeWorkplaces[randWorkplace];
					Worker w = new Worker(home,workPlace,age,randomq);
					_instance.Personslist.Add(i,w);
				}
			}
			else if(_instance.report.PopulationChange < 0){
				for(int i = startindex;i > _instance.report.PopulationChange-1;i--){
					_instance.Personslist.Remove(i);
				}				
			}

		}

		public bool MarkZone(List<Tile> tiles, ZoneBuilding z)
		{
			return true;
			//TODO
		}
		public bool RemoveZone(List<Tile> tiles)
		{
			throw new NotImplementedException();
			//TODO
		}
		public bool BuildService(Tile tile, ServiceBuilding s)
		{
			throw new NotImplementedException();
			//TODO
		}
		public bool Destroy(Tile tile)
		{
			throw new NotImplementedException();
			//TODO
		}
		public bool DestoryForce(Tile tile)
		{

			throw new NotImplementedException();
			//TODO
		}
		private bool BuildByPeople(Tile t, ZoneBuilding z)
		{
			throw new NotImplementedException();
			//TODO
		}
		/*
		private bool LevelUpZone(IBuilding b)
		{
			
		}*/
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
		/*
		public int GetPriceLevelUpZone(IBuilding)
		{
			throw new NotImplementedException();
			//TODO
		}*/

		public void SetTax(float f)
		{
			_tax = f;
			
			//TODO
		}
		public float GetTax(){
			return _tax;
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
			Person toKill = person;
			
			foreach(KeyValuePair<int,Person> kvp in Personslist){
				if(kvp.Value == toKill){
					Personslist.Remove(kvp.Key);
					break;
				}
			}
			//TODO
		}
		public float GetMoney()
		{
			return _money;
			//TODO
		}
		public DateTime Getdate()
		{
			throw new NotImplementedException();
			//TODO
		}
		/*
		public List<IBuilding> GetBuildingsOnFire()
		{
			throw new NotImplementedException();
			//TODO
		}*/
		public List<Car> GetCarsOnRoad()
		{
			throw new NotImplementedException();
			//TODO
		}
		public int GetTimeSpeed()
		{
			

			return _timeSpeed;
			//TODO
		}
		public int SetTimeSpeed(int speed)
		{

			_timeSpeed = speed;
			return _timeSpeed;
			
			//TODO
		}

		#region Thread
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
				long sleepTime = 1000 / _tps - (DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime);
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
