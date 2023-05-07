using Model.Tiles.Buildings;
using System;

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

			if (this is IWorkplace workplace)
			{
				workplace.RegisterWorkplace(RoadGridManager.GetRoadGrigElementByBuilding((Building)workplace).GetRoadGrid());
			}
			if (this is IResidential residential)
			{
				residential.RegisterResidential(RoadGridManager.GetRoadGrigElementByBuilding((Building)residential).GetRoadGrid());
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

		internal abstract bool IsExpandable();
		internal abstract bool CanExpand();
		internal abstract void Expand();
	}
}