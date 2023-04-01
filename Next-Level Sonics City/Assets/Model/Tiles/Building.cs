using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Tiles
{
	public abstract class Building : Tile
	{
		public int ID { get; private set; }
		public int Helath { get; private set; }
		public bool IsOnFire { get; private set; }
		public List<Road> ConnectedTo { get; private set; }

		public bool StartFire()
		{
			throw new NotImplementedException();
		}

		public void GetFirePercentage()
		{
			throw new NotImplementedException();
		}

	}
}