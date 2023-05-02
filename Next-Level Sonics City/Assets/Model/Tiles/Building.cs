using Model.Tiles.Buildings;
using System;
using System.Collections.Generic;

namespace Model.Tiles
{
	public abstract class Building : Tile
	{
		public int ID { get; private set; }
		public int Helath { get; private set; }
		public bool IsOnFire { get; private set; }
		public List<Road> ConnectedTo { get; private set; }

		public Rotation Rotation { get; private set; }

		public Building(int x, int y, uint designID, Rotation rotation) : base(x, y, designID)
		{
			Rotation = rotation;
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