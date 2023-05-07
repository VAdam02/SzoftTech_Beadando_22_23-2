using System.Collections;
using System.Collections.Generic;

namespace Model.Tiles
{
	public class Forest : Tile
	{
		public Forest(int x, int y, uint designID) : base(x, y, designID)
		{

		}

		public override int GetBuildPrice() //TODO implementik logic
		{
			return BUILD_PRICE;
		}

		public override int GetDestroyPrice()
		{
			return DESTROY_PRICE;
		}

		public override int GetMaintainanceCost()
		{
			return GetBuildPrice() / 10;
		}
	}
}