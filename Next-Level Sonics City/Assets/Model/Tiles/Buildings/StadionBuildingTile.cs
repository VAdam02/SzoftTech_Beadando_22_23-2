using Model.Persons;
using Model.RoadGrids;
using Model.Tiles.Buildings.BuildingCommands;
using System;
using System.Collections.Generic;

namespace Model.Tiles.Buildings
{
	public class StadionBuildingTile : Building, IWorkplace
	{
		private readonly List<Worker> _workers = new();
		public int WorkplaceLimit { get; private set; }

		public StadionBuildingTile(int x, int y, uint designID, Rotation rotation) : base(x, y, designID, rotation)
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
			//TODO implement stadion workplace limit
			WorkplaceLimit = 10;
		}

		public override TileType GetTileType() { return TileType.Stadion; }

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
			//TODO implement stadion build price
			return 100000;
		}

		public override int GetDestroyIncome()
		{
			//TODO implement stadion destroy income
			return 100000;
		}

		public override int GetMaintainanceCost()
		{
			//TODO implement stadion maintainance cost
			return 100000;
		}

		public override bool CanBuild()
		{
			int x1 = (int)Coordinates.x;
			int y1 = (int)Coordinates.y;
			int x2 = (int)Coordinates.x;
			int y2 = (int)Coordinates.y;


			switch (Rotation)
			{
				case Rotation.Zero:
					x1 += 1; y1 += 1;
					break;
				case Rotation.Ninety:
					x1 += -1; y1 += 1;
					break;
				case Rotation.OneEighty:
					x1 += -1; y1 += 1;
					break;
				case Rotation.TwoSeventy:
					x1 += 1; y1 += -1;
					break;
			}

			int minX = Math.Min(x1, x2);
			int maxX = Math.Max(x1, x2);
			int minY = Math.Min(y1, y2);
			int maxY = Math.Max(y1, y2);

			for (int i = minX; i <= maxX; ++i)
			{
				for (int j = minY; j <= maxY; ++j)
				{
					if (City.Instance.GetTile(i, j) is not EmptyTile)
					{
						return false;
					}
				}
			}

			return true;
		}

		internal override void Expand()
		{
			int x1 = (int)Coordinates.x;
			int y1 = (int)Coordinates.y;
			int x2 = (int)Coordinates.x;
			int y2 = (int)Coordinates.y;

			switch (Rotation)
			{
				case Rotation.Zero:
					x1 += 1; y1 += 1;
					break;
				case Rotation.Ninety:
					x1 += -1; y1 += 1;
					break;
				case Rotation.OneEighty:
					x1 += -1; y1 += 1;
					break;
				case Rotation.TwoSeventy:
					x1 += 1; y1 += -1;
					break;
			}

			int minX = Math.Min(x1, x2);
			int maxX = Math.Max(x1, x2);
			int minY = Math.Min(y1, y2);
			int maxY = Math.Max(y1, y2);

			for (int i = minX; i <= maxX; ++i)
			{
				for (int j = minY; j <= maxY; ++j)
				{
					if (i == (int)Coordinates.x && j == (int)Coordinates.y) { continue; }
					ExpandCommand ec = new(i, j, this);
					ec.Execute();
				}
			}
		}

		public override float GetTransparency()
		{
			return 0.75f;
		}
	}
}