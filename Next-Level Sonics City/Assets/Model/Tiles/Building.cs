using Model.Tiles.Buildings;
using System;
using Model.RoadGrids;
using Model.Simulation;

namespace Model.Tiles
{
	public abstract class Building : Tile
	{
		public int Helath { get; private set; }
		public bool IsOnFire { get; private set; }
		public Rotation Rotation { get; private set; }

		public Building(int x, int y, uint designID, Rotation rotation) : base(x, y, designID)
		{
			Rotation = rotation;
		}

		public override void FinalizeTile()
		{
			Finalizing();
		}

		protected new void Finalizing()
		{
			base.Finalizing();

			RoadGrid roadGrid = RoadGridManager.GetRoadGrigElementByBuilding(this)?.GetRoadGrid();
			if (roadGrid != null)
			{
				if (this is IWorkplace workplace)
				{
					workplace.RegisterWorkplace(roadGrid);
				}
				if (this is IResidential residential)
				{
					residential.RegisterResidential(roadGrid);
				}
			}

			OnTileDelete.AddListener(Destroy);
		}

		private void Destroy()
		{
			if (this is IWorkplace workplace)
			{
				workplace.UnregisterWorkplace(RoadGridManager.GetRoadGrigElementByBuilding((Building)workplace).GetRoadGrid());
			}
			if (this is IResidential residential)
			{
				residential.UnregisterResidential(RoadGridManager.GetRoadGrigElementByBuilding((Building)residential).GetRoadGrid());
			}
		}

		public bool StartFire()
		{
			throw new NotImplementedException();
		}

		public void GetFirePercentage()
		{
			throw new NotImplementedException();
		}

		internal virtual bool CanBuild()
		{
			return SimEngine.Instance.GetTile((int)Coordinates.x, (int)Coordinates.y) is EmptyTile;
		}

		internal virtual void Expand()
		{

		}
	}
}