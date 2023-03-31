using Model;
using Service;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tiles
{
	public class ElectricPole : Tile, IPowerGridElement
	{
		public ElectricPole(int x, int y, uint designID) : base(x, y, designID)
		{

		}
	}
}
