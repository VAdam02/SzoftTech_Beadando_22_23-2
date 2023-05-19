using Model.Persons;
using Model.RoadGrids;
using Model.Statistics;
using System;
using System.Collections.Generic;

namespace Model.Tiles.Buildings
{
	public class Industrial : Building, IWorkplace, IZoneBuilding
	{
		public ZoneBuildingLevel Level { get; private set; } = 0;
		private readonly List<Worker> _workers = new();
		public int WorkplaceLimit { get; private set; }

		/// <summary>
		/// Construct a new industrial tile
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		public Industrial(int x, int y, uint designID) : base(x, y, designID, GetRandomRotationToLookAtRoadGridElement(x, y))
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
			//TODO implement industrial workplace limit
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
			return ZoneType.IndustrialZone;
		}

		public void LevelUp()
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			//TODO implement industrial level up workplace amount
			if (Level == ZoneBuildingLevel.ZERO) { return; }
			if (Level == ZoneBuildingLevel.THREE) { return; }
			StatEngine.Instance.RegisterIndustrialLevelChange(WorkplaceLimit, WorkplaceLimit + 5);
			++Level;
			WorkplaceLimit += 5;
		}

		public int GetLevelUpCost()
		{
			//TODO implement industrial level up cost
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
			//TODO implement industrial building price
			return 100000;
		}

		public override int GetDestroyIncome()
		{
			//TODO implement industrial destroy income
			return 100000;
		}
	}
}