using Model.Persons;
using Model.RoadGrids;
using System;
using System.Collections.Generic;

namespace Model.Tiles.Buildings
{
	public class MockWorkplaceBuildingTile : Building, IWorkplace
	{
		private readonly List<Worker> _workers = new();
		public int WorkplaceLimit { get; private set; }

		public MockWorkplaceBuildingTile(int x, int y, Rotation rotation) : base(x, y, 0, rotation)
		{

		}

		public override int GetBuildPrice()
		{
			return 1000;
		}

		public override int GetDestroyIncome()
		{
			return 100;
		}

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

		public void mploy(Worker worker)
		{
			_workers.Add(worker);
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

		public override float GetTransparency()
		{
			return 0.75f;
		}
	}
}
