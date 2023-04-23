using System.Collections;
using System.Collections.Generic;

namespace Model.Tiles
{
    public class Road : Tile
    {
        public Road(int x, int y, uint designID) : base(x, y, designID)
		{

		}

		public override int GetBuildPrice()
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
