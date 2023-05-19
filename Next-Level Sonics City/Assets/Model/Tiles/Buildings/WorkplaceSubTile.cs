using Model.Persons;
using Model.RoadGrids;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Tiles.Buildings
{
	public class WorkplaceSubTile : Building, IWorkplace
	{
		private readonly IWorkplace _baseWorkplace;

		public int WorkplaceLimit { get { return _baseWorkplace.WorkplaceLimit; } }

		/// <summary>
		/// Construct a new workplace sub tile
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		/// <param name="baseBuilding">Workplace that is the base for this and contains the logic</param>
		public WorkplaceSubTile(int x, int y, uint designID, IWorkplace baseBuilding) : base(x, y, designID, ((Building)baseBuilding.GetTile()).Rotation)
		{
			_baseWorkplace = baseBuilding;
		}

		public override TileType GetTileType() { return ((Tile)_baseWorkplace).GetTileType(); }

		public void RegisterWorkplace(RoadGrid roadGrid) { if (!_isFinalized) { throw new InvalidOperationException(); } }

		public void UnregisterWorkplace(RoadGrid roadGrid) { if (!_isFinalized) { throw new InvalidOperationException(); } }

		/// <summary>
		/// This method is called when the parent building is destroyed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void ParentDestroyedEventHandler(object sender, EventArgs e)
		{
			BuildingManager.Instance.Destroy(this);
		}

		/// <summary>
		/// Return the real coordinates of the subtile
		/// </summary>
		/// <returns>Coordinates of subtile</returns>
		public Vector2 GetRealCoordinates()
		{
			return Coordinates;
		}

		/// <summary>
		/// Return the coordinates of the base building
		/// </summary>
		/// <returns>Coordinates of base building</returns>
		public Vector2 GetBaseCoordinates()
		{
			return ((Building)_baseWorkplace).Coordinates;
		}

		public void Employ(Worker worker)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }
			_baseWorkplace.Employ(worker);
		}

		public void Unemploy(Worker worker)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			_baseWorkplace.Unemploy(worker);
		}

		public List<Worker> GetWorkers()
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			return _baseWorkplace.GetWorkers();
		}

		public int GetWorkersCount()
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			return _baseWorkplace.GetWorkersCount();
		}

		public override bool CanBuild()
		{
			throw new InvalidOperationException();
		}

		internal override void Expand()
		{
			throw new InvalidOperationException();
		}

		public override int GetBuildPrice()
		{
			return 0;
		}

		public override int GetDestroyIncome()
		{
			return 0;
		}
	}
}
