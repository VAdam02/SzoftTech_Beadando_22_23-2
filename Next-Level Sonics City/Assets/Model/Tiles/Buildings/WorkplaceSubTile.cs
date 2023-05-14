using Model.Persons;
using Model.Simulation;
using System;
using System.Collections.Generic;
using UnityEngine;
using Model.RoadGrids;

namespace Model.Tiles.Buildings
{
	public class WorkplaceSubTile : Building, IWorkplace
	{
		private IWorkplace _baseWorkplace;

		public WorkplaceSubTile(int x, int y, uint designID, IWorkplace baseBuilding) : base(x, y, designID, ((Building)baseBuilding.GetTile()).Rotation)
		{
			_baseWorkplace = baseBuilding;
		}

		public override TileType GetTileType() { return ((Tile)_baseWorkplace).GetTileType(); }

		public void RegisterWorkplace(RoadGrid roadGrid) { }

		public void UnregisterWorkplace(RoadGrid roadGrid) { }

		public void ParentDestroyedEventHandler(object sender, EventArgs e)
		{
			SimEngine.Instance.BuildingManager.Destroy(this);
		}

		public Vector2 GetRealCoordinates()
		{
			return this.Coordinates;
		}

		public Vector2 GetBaseCoordinates()
		{
			return ((Building)_baseWorkplace).Coordinates;
		}

		public bool Employ(Worker worker)
		{
			return _baseWorkplace.Employ(worker);
		}

		public bool Unemploy(Worker worker)
		{
			return _baseWorkplace.Unemploy(worker);
		}

		public List<Worker> GetWorkers()
		{
			return _baseWorkplace.GetWorkers();
		}

		public int GetWorkersCount()
		{
			return _baseWorkplace.GetWorkersCount();
		}

		public int GetWorkersLimit()
		{
			return _baseWorkplace.GetWorkersLimit();
		}

		public Tile GetTile() { return _baseWorkplace.GetTile(); }

		internal override bool CanBuild()
		{
			throw new InvalidOperationException();
		}

		internal override void Expand()
		{
			throw new InvalidOperationException();
		}
	}
}
