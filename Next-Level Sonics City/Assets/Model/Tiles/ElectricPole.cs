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

		//TODO implement electric pole build price
		public override int BuildPrice => 100000;

		//TODO implement electric pole destroy price
		public override int DestroyIncome => 100000;

		//TODO implement electric pole maintainance cost
		public override int MaintainanceCost => 100000;

		public override float Transparency => 1;

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
