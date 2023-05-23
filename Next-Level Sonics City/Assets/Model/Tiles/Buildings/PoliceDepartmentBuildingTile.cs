using Model.Persons;
using Model.RoadGrids;
using System;
using System.Collections.Generic;
using System.Linq;
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

			IHappyZone.RegisterHappinessChangerTileToRegisterRadius(this);
		}

		/// <summary>
		/// Register at and to the new tile
		/// </summary>
		/// <param name="oldTile">Old tile that was deletetd</param>
		private void TileDestroyedInRadius(object sender, Tile oldTile)
		{
			Tile newTile = City.Instance.GetTile(oldTile);

			if (newTile is IResidential residential)	{ residential.RegisterHappinessChangerTile(this); }
			if (newTile is IWorkplace workplace)		{ workplace.RegisterHappinessChangerTile(this); } //TODO
			newTile.OnTileDelete += TileDestroyedInRadius;
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
				TileChangeInvoke();
			}

			_workers.Add(person);
		}

		public void Unemploy(Worker person)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			_workers.Remove(person);

			if (GetWorkersCount() == 0)
			{
				TileChangeInvoke();
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

		//TODO implement electric pole build price
		public override int BuildPrice => 100000;

		//TODO implement electric pole destroy price
		public override int DestroyIncome => 100000;

		//TODO implement electric pole maintainance cost
		public override int MaintainanceCost => 100000;

		public (float happiness, float weight) GetHappinessModifierAtTile(Building building)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			if (building == null) { throw new ArgumentNullException(); }
			if (building == this) { throw new ArgumentException("Target can't be same as this"); }

			//it is not even in the same grid
			if (RoadGridManager.GetRoadGrigElementByBuilding(this) == null) { return (0, 0); }
			if (RoadGridManager.GetRoadGrigElementByBuilding(building) == null) { return (0, 0); }
			if (RoadGridManager.GetRoadGrigElementByBuilding(building).RoadGrid != RoadGridManager.GetRoadGrigElementByBuilding(this).RoadGrid) { return (0, 0); }

			//it is not reachable
			int distance = GetDistanceOnRoad(RoadGridManager.GetRoadGrigElementByBuilding(building), ((IHappyZone)this).EffectiveRadius - 1);
			if (distance == -1)
			{
				return (0, 0);
			}

			return (1, Mathf.Cos(distance * Mathf.PI / 2 / ((IHappyZone)this).EffectiveRadius));
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
					foreach (IRoadGridElement element in roadGridElement.ConnectsTo)
					{
						queue.Enqueue((element, distance + 1));
					}
				}
			}

			return -1;
		}

		public (float happiness, float weight) HappinessByBuilding
		{
			get
			{
				float happinessSum = _happinessChangers.Aggregate(0.0f, (acc, item) => acc + item.happiness * item.weight);
				float happinessWeight = _happinessChangers.Aggregate(0.0f, (acc, item) => acc + item.weight);
				return (happinessSum / (happinessWeight == 0 ? 1 : happinessWeight), happinessWeight);
			}
		}

		int IHappyZone.RegisterRadius => 5;

		int IHappyZone.EffectiveRadius => GetWorkersCount() > 0 ? ((IHappyZone)this).RegisterRadius : 0;

		private readonly List<(IHappyZone happyZone, float happiness, float weight)> _happinessChangers = new();
		public void RegisterHappinessChangerTile(IHappyZone happyZone)
		{
			happyZone.GetTile().OnTileDelete += UnregisterHappinessChangerTile;
			happyZone.GetTile().OnTileChange += UpdateHappiness;

			(float happiness, float weight) = happyZone.GetHappinessModifierAtTile(this);
			_happinessChangers.Add((happyZone, happiness, weight));
		}

		private void UnregisterHappinessChangerTile(object sender, Tile deletedTile)
		{
			IHappyZone happyZone = (IHappyZone)deletedTile;
			_happinessChangers.RemoveAll((values) => values.happyZone == happyZone);
		}

		private void UpdateHappiness(object sender, Tile changedTile)
		{
			IHappyZone happyZone = (IHappyZone)changedTile;
			_happinessChangers.RemoveAll((values) => values.happyZone == happyZone);

			(float happiness, float weight) = happyZone.GetHappinessModifierAtTile(this);
			_happinessChangers.Add((happyZone, happiness, weight));
		}

		(float happiness, float weight) IHappyZone.GetHappinessModifierAtTile(Building building)
		{
			return (0, 0); //TODO
		}

		Tile IHappyZone.GetTile()
		{
			return this;
		}

		void IHappyZone.TileDestroyedInRadiusHandler(object sender, Tile oldTile)
		{
			throw new NotImplementedException();
		}
	}
}