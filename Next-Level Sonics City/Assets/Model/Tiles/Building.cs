using Model.RoadGrids;
using Model.Tiles.Buildings;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Model.Tiles
{
	public abstract class Building : Tile
	{
		public int Helath { get; private set; }
		public bool IsOnFire { get; private set; }

		public readonly UnityEvent OnRotationChanged = new();

		private Rotation _rotation;
		public Rotation Rotation
		{
			get { return _rotation; }
			private set
			{
				_rotation = value;
				if (MainThreadDispatcher.Instance is MainThreadDispatcher mainThread)
				{
					mainThread.Enqueue(() =>
					{
						OnRotationChanged.Invoke();
					});
				}
			}
		}

		public Building(int x, int y, uint designID, Rotation rotation) : base(x, y, designID)
		{
			Rotation = rotation;
		}

		public override void FinalizeTile()
		{
			Finalizing();
		}

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Finalizing()</code></para>
		/// <para>Do the actual finalization</para>
		/// </summary>
		protected new void Finalizing()
		{
			base.Finalizing();

			RoadGrid roadGrid = RoadGridManager.GetRoadGrigElementByBuilding(this)?.GetRoadGrid();
			if (roadGrid != null)
			{
				if (this is IWorkplace workplace)
				{
					workplace.RegisterWorkplace(roadGrid);
				}
				if (this is IResidential residential)
				{
					residential.RegisterResidential(roadGrid);
				}
			}
		}

		public override void DeleteTile()
		{
			Deleting();
		}

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Deleting()</code></para>
		/// <para>Do the deletion administration</para>
		/// </summary>
		protected new void Deleting()
		{
			base.Deleting();

			if (this is IWorkplace workplace)
			{
				workplace.UnregisterWorkplace(RoadGridManager.GetRoadGrigElementByBuilding((Building)workplace)?.GetRoadGrid());
			}
			if (this is IResidential residential)
			{
				residential.UnregisterResidential(RoadGridManager.GetRoadGrigElementByBuilding((Building)residential)?.GetRoadGrid());
			}
		}

		/// <summary>
		/// Starts the fire
		/// </summary>
		/// <returns></returns>
		public bool StartFire()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Calculate the chance of fire
		/// </summary>
		public void GetFirePercentage()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Expand the building
		/// </summary>
		internal virtual void Expand()
		{

		}
	}
}