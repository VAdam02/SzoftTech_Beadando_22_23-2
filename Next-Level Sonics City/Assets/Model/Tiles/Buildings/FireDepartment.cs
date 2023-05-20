using Model.Persons;
using Model.RoadGrids;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Tiles.Buildings
{
	public class FireDepartment : Building, IWorkplace
	{
		private readonly List<Worker> _workers = new();
		public int WorkplaceLimit { get; private set; }

		/// <summary>
		/// Construct a new fire department tile
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		/// <param name="rotation">Rotation of the tile</param>
		public FireDepartment(int x, int y, uint designID, Rotation rotation) : base(x, y, designID, rotation)
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
			//TODO implement fire department workplace limit
			WorkplaceLimit = 10;
		}

		public override TileType GetTileType() { return TileType.FireDepartment; }

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

		public void Employ(Worker worker)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			if (_workers.Count >= WorkplaceLimit) { throw new InvalidOperationException("The workplace is full"); }
			_workers.Add(worker);
		}

		public void Unemploy(Worker worker)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			_workers.Remove(worker);
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
			//TODO implement fire department build price
			return 1000000;
		}

		public override int GetDestroyIncome()
		{
			//TODO implement fire department destroy income
			return 100000;
		}

		public override int GetMaintainanceCost()
		{
			//TODO implement fire department maintainance cost
			return 100000;
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

		private readonly List<(IHappyZone happyZone, float happiness, float weight)> _happinessChangers = new();
		public void RegisterHappinessChangerTile(IHappyZone happyZone)
		{
			happyZone.GetTile().OnTileDelete.AddListener(UnregisterHappinessChangerTile);
			happyZone.GetTile().OnTileChange.AddListener(UpdateHappiness);

			(float happiness, float weight) = happyZone.GetHappinessModifierAtTile(this);
			_happinessChangers.Add((happyZone, happiness, weight));
		}

		private void UnregisterHappinessChangerTile(Tile deletedTile)
		{
			IHappyZone happyZone = (IHappyZone)deletedTile;
			_happinessChangers.RemoveAll((values) => values.happyZone == happyZone);
		}

		private void UpdateHappiness(Tile changedTile)
		{
			IHappyZone happyZone = (IHappyZone)changedTile;
			_happinessChangers.RemoveAll((values) => values.happyZone == happyZone);

			(float happiness, float weight) = happyZone.GetHappinessModifierAtTile(this);
			_happinessChangers.Add((happyZone, happiness, weight));
		}

		public override float GetTransparency()
		{
			return 0.75f;
		}
	}
}