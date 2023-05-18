using Model.Service;

namespace Model.Tiles
{
	public class ElectricPole : Tile, IPowerGridElement
	{
		/// <summary>
		/// Construct a new electric pole
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		public ElectricPole(int x, int y, uint designID) : base(x, y, designID)
		{

		}

		public override TileType GetTileType() { return TileType.ElectricPole; }

		public override int GetBuildPrice()
		{
			//TODO implement electric pole build price
			return 100000;
		}

		public override int GetDestroyIncome()
		{
			//TODO implement electric pole destroy price
			return 100000;
		}

		public override int GetMaintainanceCost()
		{
			//TODO implement electric pole maintainance cost
			return 100000;
		}
	}
}
