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

		void IWorkplace.Employ(Worker worker)
		{
			_workers.Add(worker);
		}

		Tile IWorkplace.GetTile()
		{
			throw new NotImplementedException();
		}

		List<Worker> IWorkplace.GetWorkers()
		{
			return _workers;
		}

		int IWorkplace.GetWorkersCount()
		{
			throw new NotImplementedException();
		}

		void IWorkplace.RegisterWorkplace(RoadGrid roadGrid)
		{
			roadGrid?.AddWorkplace(this);
		}

		void IWorkplace.Unemploy(Worker worker)
		{
			//Nevermind, we don't really want to send him to the black hole
		}

		void IWorkplace.UnregisterWorkplace(RoadGrid roadGrid)
		{
			throw new NotImplementedException();
		}
	}
}
