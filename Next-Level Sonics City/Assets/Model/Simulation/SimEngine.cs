using Model.Persons;
using Model.Statistics;
using Model.Tiles;
using Model.Tiles.Buildings;
using System;
using System.Threading;
using UnityEngine;

namespace Model.Simulation
{
	public class SimEngine : MonoBehaviour
	{
		private static SimEngine _instance;
		public static SimEngine Instance { get { return _instance; } }

		/// <summary>
		/// MUST BE CALLED BY UNITY ONLY
		/// </summary>
		private void Awake()
		{
			_instance = this;

			ZoneManager.Instance.ZoneMarked += (tile, zone) => { StatEngine.Instance.AddBuildExpense(tile, zone); };
			ZoneManager.Instance.ZoneUnMarked += (tile, zone) => { StatEngine.Instance.AddDestroyIncome(tile, zone); };
			BuildingManager.Instance.BuildingBuilt += (tile, building) => { StatEngine.Instance.AddBuildExpense(tile, building); };
			BuildingManager.Instance.BuildingDestroyed += (tile, building) => { StatEngine.Instance.AddDestroyIncome(tile, building); };
		}

		public IResidential residential;

		/// <summary>
		/// <para>MUST BE CALLED BY UNITY ONLY</para>
		/// </summary>
		private void Start()
		{
			StartSimulation();

			for (int i = 0; i < City.Instance.GetSize() / 2; i++)
			{
				for (int j = 0; j < City.Instance.GetSize() / 2; j++)
				{
					City.Instance.SetTile(new ForestTile(i, j, 0));
				}
			}
		}

		/// <summary>
		/// Called once when the simulation should do a cycle
		/// </summary>
		private static void Tick()
		{
			StatEngine.Instance.TimeElapsed();

			//kill who needed



			/*
			//mandatory continuous move-in
			int startindex = City.Instance.GetPersons().Keys.Last();
			if(SimEngine._instance.freeResidentals.Count !=0 && SimEngine._instance.freeWorkplaces.Count != 0){
				if(_instance.freeResidentals.Count>=2 && _instance.freeWorkplaces.Count>=2){
					for(int i = startindex+1;i < startindex+3;i++){	

						if(SimEngine._instance.FindHomeWithoutIndustrial(SimEngine._instance.freeResidentals) is null){			
							int age = rand.Next(18,65);
							Qualification randomq = (Qualification)new System.Random().Next(0,Enum.GetValues(typeof(Qualification)).Length);
							int randResidental = rand.Next(0,_instance.freeResidentals.Count);
							ResidentialBuildingTile home = SimEngine._instance.FindClosestHomeToWorkplace(SimEngine._instance.RoadGridManager.RoadGrids);
							int randWorkplace = rand.Next(0,_instance.freeWorkplaces.Count);
							IWorkplace workPlace = SimEngine._instance.FindClosestWorkplaceToResidential(SimEngine._instance.RoadGridManager.RoadGrids,home);
							Worker w = new Worker(home,workPlace,age,randomq);
							_instance.Personslist.Add(i,w);
						}
						else{
							
							int age = rand.Next(18,65);
							Qualification randomq = (Qualification)new System.Random().Next(0,Enum.GetValues(typeof(Qualification)).Length);
							int randResidental = rand.Next(0,_instance.freeResidentals.Count);
							ResidentialBuildingTile home = SimEngine._instance.FindHomeWithoutIndustrial(SimEngine._instance.freeResidentals);
							int randWorkplace = rand.Next(0,_instance.freeWorkplaces.Count);
							IWorkplace workPlace = SimEngine._instance.FindClosestWorkplaceToResidential(SimEngine._instance.RoadGridManager.RoadGrids,home);
							Worker w = new Worker(home,workPlace,age,randomq);
							_instance.Personslist.Add(i,w);
						}
					}
				}
			}
					  
			// temporary solution
			//happiness move-in or move-out
			foreach(RoadGrid roadGrid in SimEngine.Instance.RoadGridManager.RoadGrids){
				SimEngine._instance.freeResidentals = (List<ResidentialBuildingTile>)roadGrid.Residentials.Where(current => current.GetResidentsCount() < current.GetResidentsLimit());
				SimEngine._instance.freeWorkplaces = (List<IWorkplace>)roadGrid.Workplaces.Where(current => current.GetWorkersCount() < current.GetWorkersLimit());
				SimEngine._instance.ResidentialsList = roadGrid.Residentials;
			}
			for(int i = 1; i < SimEngine._instance.Personslist.Count+1;i++){
				Person person = SimEngine._instance.Personslist[i];
				if(SimEngine._instance.MoveOut(person)){
					SimEngine._instance.MoveOutList.Add(person);
					//remove the person from the Personlist
					SimEngine._instance.Personslist.Remove(i);
				}
			}
			startindex = _instance.Personslist.Keys.Last();
			if(SimEngine._instance.StatEngine.CalculateHappiness(SimEngine._instance.ResidentialsList) > 0.65){
			int be = (int)(SimEngine._instance.Personslist.Count * SimEngine._instance.StatEngine.CalculateHappiness(SimEngine._instance.ResidentialsList)/10);

				if(SimEngine._instance.freeResidentals.Count >=be && SimEngine._instance.freeWorkplaces.Count >=be){
					for(int i = startindex+1;i< startindex+be+1;i++){
						if(SimEngine._instance.FindHomeWithoutIndustrial(SimEngine._instance.freeResidentals) is null){			
							int age = rand.Next(18,65);
							Qualification randomq = (Qualification)new System.Random().Next(0,Enum.GetValues(typeof(Qualification)).Length);
							int randResidental = rand.Next(0,_instance.freeResidentals.Count);
							ResidentialBuildingTile home = SimEngine._instance.FindClosestHomeToWorkplace(SimEngine._instance.RoadGridManager.RoadGrids);
							int randWorkplace = rand.Next(0,_instance.freeWorkplaces.Count);
							IWorkplace workPlace = SimEngine._instance.FindClosestWorkplaceToResidential(SimEngine._instance.RoadGridManager.RoadGrids,home);
							Worker w = new Worker(home,workPlace,age,randomq);
							_instance.Personslist.Add(i,w);
						}
						else{
							
							int age = rand.Next(18,65);
							Qualification randomq = (Qualification)new System.Random().Next(0,Enum.GetValues(typeof(Qualification)).Length);
							int randResidental = rand.Next(0,_instance.freeResidentals.Count);
							ResidentialBuildingTile home = SimEngine._instance.FindHomeWithoutIndustrial(SimEngine._instance.freeResidentals);
							int randWorkplace = rand.Next(0,_instance.freeWorkplaces.Count);
							IWorkplace workPlace = SimEngine._instance.FindClosestWorkplaceToResidential(SimEngine._instance.RoadGridManager.RoadGrids,home);
							Worker w = new Worker(home,workPlace,age,randomq);
							_instance.Personslist.Add(i,w);
						}
					}
				}				
			}
			//moving the persons out
			foreach (Person residentToRemove in SimEngine._instance.MoveOutList)
			{
				residentToRemove.Residential.MoveOut(residentToRemove);

				if (residentToRemove is Worker worker)
				{
					worker.WorkPlace.Unemploy(worker);
				}
			}
			*/
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
		private bool MoveOut(Person p)
		{
			if(p.GetHappiness() <= 0.5){
				return(new System.Random().NextDouble()<0.9);//90% chance of moving out
			}
			else if(p.GetHappiness()>0.5 && p.GetHappiness() < 0.65){
				return(new System.Random().NextDouble()<0.55);//55% chance of moving out

			}
			else if(p.GetHappiness()>= 0.65 && p.GetHappiness()<0.75) {
				return(new System.Random().NextDouble()<0.25);//25% chance of moving out

			}
			else{
				return(new System.Random().NextDouble()<0.05);//5% chance of moving out

			}
			//TODO
		}

		private void Die(Person person)
		{
			/*
			Person toKill = person;
			
			foreach(KeyValuePair<int,Person> kvp in Personslist){
				if(kvp.Value == toKill){
					Personslist.Remove(kvp.Key);
					break;
				}
			}
			//TODO
			*/
		}

		/// <summary>
		/// Set the time speed
		/// </summary>
		/// <param name="speed"></param>
		public void SetTimeSpeed(int speed)
		{
			_timeSpeed = speed;
		}

		/*
		public ResidentialBuildingTile FindHomeWithoutIndustrial(List<ResidentialBuildingTile> freeResidential){
			foreach(ResidentialBuildingTile residential in freeResidential){
				if(!IsIndustrialNearby(residential)){
					return residential;
				}
			}
			return null;
		}
		public IResidential FindClosestHomeToWorkplace(List<RoadGrid> roadGrids)
		{
			IResidential closestHome = null;
			float distance = float.MaxValue;
			foreach (RoadGrid roadGrid in roadGrids)
			foreach (IResidential residential in roadGrid.Residentials.FindAll((IResidential residential) => residential.ResidentLimit > residential.GetResidentsCount()))
			foreach (IWorkplace workplace in roadGrid.Workplaces.FindAll((IWorkplace workplace) => workplace.WorkplaceLimit > workplace.GetWorkersCount()))
			{
				float currentDistance = Vector3.Distance(residential.GetTile().Coordinates, workplace.GetTile().Coordinates);
				if (currentDistance < distance)
				{
					distance = currentDistance;
					closestHome = residential;
				}
			}

			return closestHome;
		}

		public bool IsIndustrialNearby(ResidentialBuildingTile residentialBuilding){
			foreach(IWorkplace industrial  in SimEngine._instance.RoadGridManager.RoadGrids){
				if(industrial is Industrial){
					float distance = Vector3.Distance(industrial.GetTile().Coordinates, residentialBuilding.Coordinates);
					
					if(distance < 10){
						return true;
					}
				}
			}
			return false;
		}
		public IWorkplace FindClosestWorkplaceToResidential(List<RoadGrid> roadGrids, ResidentialBuildingTile residentialBuilding){
			
			IWorkplace closestWorkplace = null;
			float distance = float.MaxValue;
			
				foreach (RoadGrid roadGrid in roadGrids)
				{
					foreach (IWorkplace workplace in freeWorkplaces)
					{
						float current = Vector3.Distance(workplace.GetTile().Coordinates, residentialBuilding.Coordinates);
						
						if (current < distance)
						{
							distance = current;
							closestWorkplace = workplace;
						}
					}
				}

			return closestWorkplace;
		}
		*/

		#region Thread
		private static float _timeSpeed = 1;							//multiplier for tps
		private const int TPS = 10;
		private static Thread _t;										//don't modify

		private static readonly object _simulationStateLock = new();	//lock for _isSimulating and _isRunning
		private static bool _isSimulating = false;						//don't modify
		private static bool _isRunning = false;							//don't modify

		private static bool _isPaused = false;							//don't modify
		private static bool _isDebugPrinted = true;                     //don't modify

		/// <summary>
		/// <para>MUST BE CALLED BY THREAD</para>
		/// <para>Run the ticking loop</para>
		/// </summary>
		private static void ThreadProc()
		{
			lock (_simulationStateLock)
			{
				if (_isRunning) { return; }
				_isRunning = true;
				_isSimulating = true;
			}

			long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			
			long sumTickTime = 0;
			long tickCount = 0;

			while (_isSimulating)
			{
				//TICK
				if (!_isPaused) { Tick(); }

				//TICKING DELAY
				long timeNeeded = DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
				if (!_isPaused ) { sumTickTime += timeNeeded; tickCount++; }

				long timeAllowed = (long)(1000 / (TPS * _timeSpeed));
				long sleepTime = timeAllowed - timeNeeded;
				if (sleepTime > 0)
				{
					Thread.Sleep((int)sleepTime);
					startTime += timeAllowed;
				}
				else
				{
					startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
					Debug.LogWarning("Last tick took " + timeNeeded + "ms thats " + (-sleepTime) + "ms longer than the maximum allowed tick process time");
				}

				if (!_isDebugPrinted)
				{
					_isDebugPrinted = true;
					Debug.Log((sumTickTime / (tickCount == 0 ? 1 : tickCount)) + "ms average tick time");
				}
			}

			_isRunning = false;
		}

		/// <summary>
		/// <para>PREFERABLY CALLED BY UNITY ONLY</para>
		/// <para>Start the simulation</para>
		/// </summary>
		private static void StartSimulation()
		{
			_t ??= new Thread(new ThreadStart(ThreadProc));
			_t.Start();
		}

		/// <summary>
		/// <para>PREFERABLY CALLED BY UNITY ONLY</para>
		/// <para>Stop the simulation</para>
		/// </summary>
		private static void StopSimulation()
		{
			if (!_isPaused) { _isDebugPrinted = false; }
			_isSimulating = false;
		}

		/// <summary>
		/// <para>MUST BE CALLED BY UNITY ONLY</para>
		/// <para>Called when the application is closed</para>
		/// </summary>
		private void OnApplicationQuit()
		{
			StopSimulation();
		}

		/// <summary>
		/// <para>MUST BE CALLED BY UNITY ONLY</para>
		/// <para>Called when the application loses focus</para>
		/// </summary>
		/// <param name="hasFocus">True if has focus else false</param>
		private void OnApplicationFocus(bool hasFocus)
		{
			if (!_isPaused && !hasFocus) { _isDebugPrinted = false; }
			_isPaused = !hasFocus;
		}

		/// <summary>
		/// <para>MUST BE CALLED BY UNITY ONLY</para>
		/// <para>Called when the application is paused</para>
		/// </summary>
		/// <param name="pauseStatus">True if paused and false if resumed</param>
		private void OnApplicationPause(bool pauseStatus)
		{
			if (!_isPaused && pauseStatus) { _isDebugPrinted = false; }
			_isPaused = pauseStatus;
		}
		#endregion
	}
}
