using Model.Persons;
using Model.RoadGrids;
using Model.Statistics;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Tiles.Buildings
{
	public class PoliceDepartmentBuildingTile : Building, IWorkplace, IHappyZone
	{
		private readonly List<Worker> _workers = new();
		public int WorkplaceLimit { get; private set; }

		/// <summary>
		/// Construct a new police department tile
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		/// <param name="rotation">Rotation of the tile</param>
		public PoliceDepartmentBuildingTile(int x, int y, uint designID, Rotation rotation) : base(x, y, designID, rotation)
		{

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
			//TODO implement  workplace limit
			WorkplaceLimit = 10;

			for (int i = 0; i <= GetRegisterRadius(); i++)
			for (int j = 0; Mathf.Sqrt(i * i + j * j) <= GetRegisterRadius(); j++)
			{
				if (i == 0 && j == 0) { continue; }

				//register at the residentials
				if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y - j) is IResidential residentialTopRight) { residentialTopRight.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y + j) is IResidential residentialBottomRight && j != 0) { residentialBottomRight.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y + j) is IResidential residentialBottomLeft && j != 0) { residentialBottomLeft.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y - j) is IResidential residentialTopLeft) { residentialTopLeft.RegisterHappinessChangerTile(this); }

				//register at the workplaces //TODO
				/*
				if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y - j) is IWorkplace workplaceTopRight)	{ workplaceTopRight.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y + j) is IWorkplace workplaceBottomRight)	{ workplaceBottomRight.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y + j) is IWorkplace workplaceBottomLeft)	{ workplaceBottomLeft.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y - j) is IWorkplace workplaceTopLeft)		{ workplaceTopLeft.RegisterHappinessChangerTile(this); }
				*/

				//register to the destroy event to be notified about a new tile
				City.Instance.GetTile(Coordinates.x + i, Coordinates.y - j)?.OnTileDelete.AddListener(TileDestroyedInRadius);
				if (j != 0) City.Instance.GetTile(Coordinates.x + i, Coordinates.y + j)?.OnTileDelete.AddListener(TileDestroyedInRadius);
				if (j != 0) City.Instance.GetTile(Coordinates.x - i, Coordinates.y + j)?.OnTileDelete.AddListener(TileDestroyedInRadius);
				City.Instance.GetTile(Coordinates.x - i, Coordinates.y - j)?.OnTileDelete.AddListener(TileDestroyedInRadius);
			}
		}

		/// <summary>
		/// Register at and to the new tile
		/// </summary>
		/// <param name="oldTile">Old tile that was deletetd</param>
		private void TileDestroyedInRadius(Tile oldTile)
		{
			Tile newTile = City.Instance.GetTile(oldTile.Coordinates);

			if (newTile is IResidential residential) { residential.RegisterHappinessChangerTile(this); }
			//if (newTile is IWorkplace workplace)		{ workplace.RegisterHappinessChangerTile(this);	} //TODO
			newTile.OnTileDelete.AddListener(TileDestroyedInRadius);
		}

		public override TileType GetTileType() { return TileType.PoliceDepartment; }

		public void RegisterWorkplace(RoadGrid roadGrid)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			roadGrid?.AddWorkplace(this);
		}

		public void UnregisterWorkplace(RoadGrid roadGrid)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			roadGrid?.RemoveWorkplace(this);
		}

		public void Employ(Worker person)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			if (GetWorkersCount() >= WorkplaceLimit) { throw new InvalidOperationException("The workplace is full"); }

			if (GetWorkersCount() == 0)
			{
				if (MainThreadDispatcher.Instance is MainThreadDispatcher mainThread)
				{
					mainThread.Enqueue(() =>
					{
						OnTileChange.Invoke(this);
					});
				}
			}

			_workers.Add(person);
		}

		public void Unemploy(Worker person)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			_workers.Remove(person);

			if (GetWorkersCount() == 0)
			{
				if (MainThreadDispatcher.Instance is MainThreadDispatcher mainThread)
				{
					mainThread.Enqueue(() =>
					{
						OnTileChange.Invoke(this);
					});
				}
			}
		}

		public List<Worker> GetWorkers()
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			return _workers;
		}

		public int GetWorkersCount()
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			return _workers.Count;
		}

		public override int GetBuildPrice()
		{
			//TODO implement police department build price
			return 100000;
		}

		public override int GetDestroyIncome()
		{
			//TODO implement police department destroy income
			return 100000;
		}

		public override int GetMaintainanceCost()
		{
			//TODO implement police department maintainance cost
			return 100000;
		}

		private int GetRegisterRadius()
		{
			return 5;
		}

		public int GetEffectiveRadius()
		{
			return GetWorkersCount() > 0 ? GetRegisterRadius() : 0;
		}

		public (float happiness, float weight) GetHappinessModifierAtTile(Building building)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			if (building == null) { throw new ArgumentNullException(); }
			if (building == this) { throw new ArgumentException("Target can't be same as this"); }
			
			//it is not even in the same grid
			if (RoadGridManager.GetRoadGrigElementByBuilding(building).GetRoadGrid() != RoadGridManager.GetRoadGrigElementByBuilding(this).GetRoadGrid()) { return (0, 0); }

			//it is not reachable
			int distance = GetDistanceOnRoad(RoadGridManager.GetRoadGrigElementByBuilding(building), GetEffectiveRadius() - 1);
			if (distance == -1)
			{
				return (0, 0);
			}

			Debug.Log("distance\t" + distance + "\t" + building.Coordinates);

			return (1, Mathf.Cos(distance * Mathf.PI / 2 / GetEffectiveRadius()));
		}

		public int GetDistanceOnRoad(IRoadGridElement target, int maxDistance)
		{
			Queue<(IRoadGridElement, int)> queue = new();
			queue.Enqueue((RoadGridManager.GetRoadGrigElementByBuilding(this), 0));
			while (queue.Count > 0)
			{
				(IRoadGridElement roadGridElement, int distance) = queue.Dequeue();

				if (roadGridElement == target) return distance;

				if (distance < maxDistance)
				{
					foreach (IRoadGridElement element in RoadGridManager.GetRoadGridElementsByRoadGridElement(roadGridElement))
					{
						queue.Enqueue((element, distance + 1));
					}
				}
			}

			return -1;
		}
	}
}