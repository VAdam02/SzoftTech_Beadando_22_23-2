using Model.Persons;
using Model.RoadGrids;
using System;
using System.Collections.Generic;

namespace Model.Tiles.Buildings
{
	public class PoliceDepartmentBuildingTile : Building, IWorkplace
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

			if (_workers.Count >= WorkplaceLimit) { throw new InvalidOperationException("The workplace is full"); }
			_workers.Add(person);
		}

		public void Unemploy(Worker person)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			_workers.Remove(person);
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

		public Tile GetTile() { return this; }

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
	}
}