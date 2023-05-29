using Model.RoadGrids;
using Model.Tiles.Buildings;
using System;

namespace Model.Tiles
{
	public abstract class Building : Tile
	{
		public event EventHandler OnRotationChanged;

		private Rotation _rotation;
		public Rotation Rotation
		{
			get { return _rotation; }
			private set
			{
				if (_rotation != value) { OnRotationChanged?.Invoke(this, EventArgs.Empty); }
				_rotation = value;
			}
		}

		public Building(int x, int y, uint designID, Rotation rotation) : base(x, y, designID)
		{
			Rotation = rotation;
		}

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Finalizing()</code></para>
		/// <para>Do the actual finalization</para>
		/// </summary>
		protected new void Finalizing()
		{
			base.Finalizing();

			RoadGrid roadGrid = RoadGridManager.GetRoadGrigElementByBuilding(this)?.RoadGrid;
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
		}

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Deleting()</code></para>
		/// <para>Do the deletion administration</para>
		/// </summary>
		protected new void Deleting()
		{
			base.Deleting();

			if (this is IWorkplace workplace)
			{
				workplace.UnregisterWorkplace(RoadGridManager.GetRoadGrigElementByBuilding((Building)workplace)?.RoadGrid);
			}
			if (this is IResidential residential)
			{
				residential.UnregisterResidential(RoadGridManager.GetRoadGrigElementByBuilding((Building)residential)?.RoadGrid);
			}
		}

		/// <summary>
		/// Expand the building
		/// </summary>
		internal virtual void Expand()
		{

		}

		public Tile GetTile() => this;
	}
}