using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Tiles.Buildings
{
	public class ResidentialBuildingTile : Building
	{
		public ResidentialBuildingTile(int x, int y, uint designID) : base(x, y, designID)
		{

		}

		internal List<Person> GetResidents()
		{
			throw new NotImplementedException();
		}
	}
}