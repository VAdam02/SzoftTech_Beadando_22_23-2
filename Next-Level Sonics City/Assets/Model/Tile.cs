using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Model
{
	public abstract class Tile
	{
		private uint _designID;
		public uint DesignID { get { return _designID; } protected set { _designID = value; DesignIDChangeEvent.Invoke(); } }
		public UnityEvent DesignIDChangeEvent = new UnityEvent();
		public Vector3 Coordinates { get; protected set; }

		public Tile(int x, int y, uint designID)
		{
			DesignID = designID;
			Coordinates = new Vector3(x, y, 0);
		}

		public void Build()
		{
			//TODO
			throw new NotImplementedException();
		}

		public void Destroy()
		{
			//TODO
			throw new NotImplementedException();
		}

		public int GetMaintainanceCost()
		{
			//TODO
			throw new NotImplementedException();
		}
	}
}