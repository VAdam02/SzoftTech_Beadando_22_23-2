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

		/// <summary>
		/// <para>MUST BE CALLED BY UNITY ONLY</para>
		/// </summary>
		private void Start()
		{
			StartSimulation();
		}

		/// <summary>
		/// Called once when the simulation should do a cycle
		/// </summary>
		private static void Tick()
		{
			StatEngine.Instance.TimeElapsed();
		}

		/// <summary>
		/// Set the time speed
		/// </summary>
		/// <param name="speed"></param>
		public void SetTimeSpeed(int speed)
		{
			_timeSpeed = speed;
		}

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
				long sleepTime = (long)(1000 / (TPS * _timeSpeed) - timeNeeded);
				if (sleepTime > 0) { Thread.Sleep((int)sleepTime); }
				else { Debug.LogWarning("Last tick took " + timeNeeded + " thats " + (-sleepTime) + "ms longer than the maximum allowed tick process time"); }
				startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

				if (!_isDebugPrinted)
				{
					_isDebugPrinted = true;
					Debug.Log((sumTickTime / tickCount) + "ms average tick time");
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
