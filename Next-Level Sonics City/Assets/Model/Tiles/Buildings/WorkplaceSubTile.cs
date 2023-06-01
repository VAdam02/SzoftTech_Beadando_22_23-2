using Model.Persons;
using Model.RoadGrids;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Tiles.Buildings
{
	public class WorkplaceSubTile : Building, IWorkplace
	{
		#region Tile implementation
		public override TileType GetTileType() => _baseWorkplace.GetTile().GetTileType();

		public override bool CanBuild() => throw new InvalidOperationException("Can't check because it's the base task");

		public override void FinalizeTile() => Finalizing();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Finalizing()</code></para>
		/// <para>Do the actual finalization</para>
		/// </summary>
		protected new void Finalizing() => base.Finalizing();

		public override void DeleteTile() => Deleting();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Deleting()</code></para>
		/// <para>Do the deletion administration</para>
		/// </summary>
		protected new void Deleting() => base.Deleting();

		public override int BuildPrice => 0;

		public override int DestroyIncome => 0;
		#endregion

		#region Building implementation
		internal override void Expand() => throw new InvalidOperationException("Not valid to expand the subtile");
		#endregion

		#region IWorkplace implementation
		int IWorkplace.WorkplaceLimit { get { return _baseWorkplace.WorkplaceLimit; } }

		void IWorkplace.Employ(Worker worker)
		{
			if (!_isFinalized) { throw new InvalidOperationException("Not allowed to employ before tile is set"); }
			_baseWorkplace.Employ(worker);
		}

		void IWorkplace.Unemploy(Worker worker)
		{
			if (!_isFinalized) { throw new InvalidOperationException("Not allowed to unemploy before tile is set"); }
			_baseWorkplace.Unemploy(worker);
		}

		List<Worker> IWorkplace.GetWorkers()
		{
			if (!_isFinalized) { throw new InvalidOperationException("Not allowed to get the employers before tile is set"); }
			return _baseWorkplace.GetWorkers();
		}

		int IWorkplace.GetWorkersCount()
		{
			if (!_isFinalized) { throw new InvalidOperationException("Not allowed to get the employers count before tile is set"); }
			return _baseWorkplace.GetWorkersCount();
		}

		void IWorkplace.RegisterWorkplace(RoadGrid roadGrid)
		{
			if (!_isFinalized) { throw new InvalidOperationException("Not allowed to register workplace at roadgrid before tile is set"); }
		}

		void IWorkplace.UnregisterWorkplace(RoadGrid roadGrid)
		{
			if (!_isFinalized) { throw new InvalidOperationException("Not allowed to unregister workplace at roadgrid before tile is set"); }
		}

		(float happiness, float weight) IWorkplace.HappinessByBuilding { get => (0, 0); }

		void IWorkplace.RegisterHappinessChangerTile(IHappyZone happyZone) { }
		#endregion

		#region Common implementation
		public new Tile GetTile() => _baseWorkplace.GetTile();
		#endregion

		#region IPowerConsumer implementation
		public override int GetPowerConsumption() => 0;
		#endregion

		private readonly IWorkplace _baseWorkplace;

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
		/// Return the coordinates of the base building
		/// </summary>
		/// <returns>Coordinates of base building</returns>
		public Vector2 GetBaseCoordinates()
		{
			return ((Building)_baseWorkplace).Coordinates;
		}
	}
}
