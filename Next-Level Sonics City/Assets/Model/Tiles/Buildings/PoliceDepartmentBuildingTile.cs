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
		#region Tile implementation
		public override TileType GetTileType() => TileType.PoliceDepartment;

		public override void FinalizeTile() => Finalizing();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Finalizing()</code></para>
		/// <para>Do the actual finalization</para>
		/// </summary>
		protected new void Finalizing()
		{
			base.Finalizing();
			//TODO implement stadion workplace limit
			WorkplaceLimit = 10;

			IHappyZone.RegisterHappinessChangerTileToRegisterRadius(this);
		}

		public override void DeleteTile() => Deleting();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Deleting()</code></para>
		/// <para>Do the deletion administration</para>
		/// </summary>
		protected new void Deleting()
		{
			base.Deleting();
			while (_workers.Count > 0)
			{
				_workers[0].ForcedUnemploy();
			}
		}

		public override int BuildPrice => 10000;

		#endregion

		#region Building implementation
		
		#endregion

		#region IWorkplace implementation
		private readonly List<Worker> _workers = new();
		public int WorkplaceLimit { get; private set; }

		void IWorkplace.Employ(Worker worker)
		{
			if (!_isFinalized) { throw new InvalidOperationException("Not allowed to employ before tile is set"); }
			if (_workers.Count >= WorkplaceLimit) { throw new InvalidOperationException("The workplace is full"); }
			_workers.Add(worker);
		}

		void IWorkplace.Unemploy(Worker worker)
		{
			if (!_isFinalized) { throw new InvalidOperationException("Not allowed to unemploy before tile is set"); }
			_workers.Remove(worker);
		}

		List<Worker> IWorkplace.GetWorkers()
		{
			if (!_isFinalized) { throw new InvalidOperationException("Not allowed to get the employers before tile is set"); }
			return _workers;
		}

		int IWorkplace.GetWorkersCount()
		{
			if (!_isFinalized) { throw new InvalidOperationException("Not allowed to get the employers count before tile is set"); }
			return _workers.Count;
		}

		void IWorkplace.RegisterWorkplace(RoadGrid roadGrid)
		{
			if (!_isFinalized) { throw new InvalidOperationException("Not allowed to register workplace at roadgrid before tile is set"); }
			roadGrid?.AddWorkplace(this);
		}

		void IWorkplace.UnregisterWorkplace(RoadGrid roadGrid)
		{
			if (!_isFinalized) { throw new InvalidOperationException("Not allowed to unregister workplace at roadgrid before tile is set"); }

			roadGrid?.RemoveWorkplace(this);
		}

		private readonly List<(IHappyZone happyZone, float happiness, float weight)> _happinessChangers = new();

		(float happiness, float weight) IWorkplace.HappinessByBuilding
		{
			get
			{
				float happinessSum = _happinessChangers.Aggregate(0.0f, (acc, item) => acc + item.happiness * item.weight);
				float happinessWeight = _happinessChangers.Aggregate(0.0f, (acc, item) => acc + item.weight);
				return (happinessSum / (happinessWeight == 0 ? 1 : happinessWeight), happinessWeight);
			}
		}

		void IWorkplace.RegisterHappinessChangerTile(IHappyZone happyZone)
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
		#endregion

		#region IHappyZone implementation
		int IHappyZone.RegisterRadius => 5;

		int IHappyZone.EffectiveRadius => ((IWorkplace)this).GetWorkersCount() > 0 ? ((IHappyZone)this).RegisterRadius : 0;

		(float happiness, float weight) IHappyZone.GetHappinessModifierAtTile(Building building)
		{
			if (!_isFinalized) { throw new InvalidOperationException("Tile is not set in the city"); }
			if (building == null) { throw new ArgumentNullException(nameof(building) + " can't be null"); }
			if (RoadGridManager.GetRoadGrigElementByBuilding(this)?.RoadGrid != RoadGridManager.GetRoadGrigElementByBuilding(building)?.RoadGrid) { return (0, 0); }

			float weight = 1;

			//decrease weight by distance on road
			try { weight *= 1 - RoadGridManager.GetPathOnRoad(RoadGridManager.GetRoadGrigElementByBuilding(this), RoadGridManager.GetRoadGrigElementByBuilding(building), ((IHappyZone)this).EffectiveRadius).Count / Mathf.Max(((IHappyZone)this).EffectiveRadius, 1); }
			catch { weight *= 0; }

			//increase weight by population
			weight *= Mathf.Pow(City.Instance.GetPopulation(), 2f) / 1000.0f;
			weight *= (float)((IWorkplace)this).GetWorkersCount() / WorkplaceLimit + 0.5f;

			return (1, weight);
		}

		void IHappyZone.TileDestroyedInRadiusHandler(object sender, Tile oldTile)
		{
			IHappyZone.TileDestroyedInRadius(this, oldTile);

			if (oldTile is IRoadGridElement) { TileChangeInvoke(); }
		}
		#endregion

		#region IPowerConsumer implementation
		public override int GetPowerConsumption() => 10;
		#endregion

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
	}
}