using Model.Persons;
using Model.RoadGrids;
using Model.Statistics;
using System;
using System.Collections.Generic;

namespace Model.Tiles.Buildings
{
	public class Commercial : Building, IWorkplace, IZoneBuilding
	{
		public ZoneBuildingLevel Level { get; private set; } = 0;
		private readonly List<Worker> _workers = new();
		public int WorkplaceLimit { get; private set; }

		/// <summary>
		/// Construct a new commercial tile
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		public Commercial(int x, int y, uint designID) : base(x, y, designID, GetRandomRotationToLookAtRoadGridElement(x, y))
		{
			
		}

		private static Rotation GetRandomRotationToLookAtRoadGridElement(int x, int y)
		{
			List<(IRoadGridElement roadGridElement, Rotation rotation)> roadGridElements = RoadGridManager.GetRoadGridElementsAroundTile(City.Instance.GetTile(x, y));
			if (roadGridElements.Count == 0) { throw new InvalidOperationException("No road grid elements around tile"); }
			Random rnd = new();
			return roadGridElements[rnd.Next(roadGridElements.Count)].rotation;
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
			//TODO implement commercial workplace limit
			WorkplaceLimit = 10;
		}

		public override TileType GetTileType() { throw new InvalidOperationException(); }

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

		public ZoneType GetZoneType()
		{
			return ZoneType.CommercialZone;
		}

		public void LevelUp()
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			//TODO implement commercial level up workplace amount
			if (Level == ZoneBuildingLevel.ZERO) { return; }
			if (Level == ZoneBuildingLevel.THREE) { return; }
			StatEngine.Instance.RegisterCommercialLevelChange(WorkplaceLimit, WorkplaceLimit + 5);
			++Level;
			WorkplaceLimit += 5;
		}

		public int GetLevelUpCost()
		{
			//TODO implement commercial level up cost
			return 100000;
		}

		public void Employ(Worker worker)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			if (_workers.Count >= WorkplaceLimit) { throw new InvalidOperationException("The workplace is full"); }

			if (Level == ZoneBuildingLevel.ZERO) { Level = ZoneBuildingLevel.ONE; }

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
			//TODO implement commercial mark price
			return 100000;
		}

		public override int GetDestroyIncome()
		{
			//TODO implement commercial unmark income
			return 100000;
		}

		public override int GetMaintainanceCost() { return 0; }

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

			for (int i = minX; i < maxX; ++i)
			{
				for (int j = minY; j < maxY; ++j)
				{
					if (City.Instance.GetTile(i, j) is not EmptyTile)
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}