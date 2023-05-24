using Model.Persons;
using Model.RoadGrids;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Tiles.Buildings
{
	public class MockWorkplaceBuildingTile : Building, IWorkplace
	{
		private readonly List<Worker> _workers = new();
		public int WorkplaceLimit { get; private set; }

		public MockWorkplaceBuildingTile(int x, int y, Rotation rotation) : base(x, y, 0, rotation)
		{

		}

		public override int BuildPrice => 100000;

		public override int DestroyIncome => 100000;

		public override void FinalizeTile()
		{
			Finalizing();
		}

		protected new void Finalizing()
		{
			base.Finalizing();
			WorkplaceLimit = 10;
		}

		public override TileType GetTileType()
		{
			throw new NotImplementedException();
		}

		public List<Worker> GetWorkers()
		{
			return _workers;
		}

		public int GetWorkersCount()
		{
			throw new NotImplementedException();
		}

		public void RegisterWorkplace(RoadGrid roadGrid)
		{
			roadGrid?.AddWorkplace(this);
		}

		public void UnregisterWorkplace(RoadGrid roadGrid)
		{
			throw new NotImplementedException();
		}

		public void Employ(Worker worker)
		{
			_workers.Add(worker);
		}

		public void Unemploy(Worker worker)
		{
			_workers.Remove(worker);
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

		public override void DeleteTile() => Deleting();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Deleting()</code></para>
		/// <para>Do the deletion administration</para>
		/// </summary>
		protected new void Deleting() => base.Deleting();
	}
}
