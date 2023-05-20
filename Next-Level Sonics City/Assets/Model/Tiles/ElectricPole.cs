using Model.Service;

namespace Model.Tiles
{
	public class ElectricPole : Tile
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

		public override float GetTransparency()
		{
			return 1;
		}

		public override void DeleteTile() => Deleting();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Deleting()</code></para>
		/// <para>Do the deletion administration</para>
		/// </summary>
		protected new void Deleting()
		{
			base.Deleting();
		}
	}
}
