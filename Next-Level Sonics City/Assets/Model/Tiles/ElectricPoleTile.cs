namespace Model.Tiles
{
	public class ElectricPoleTile : Tile
	{
		#region Tile implementation
		public override TileType GetTileType() { return TileType.ElectricPole; }

		public override void FinalizeTile() => Finalizing();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Finalizing()</code></para>
		/// <para>Do the actual finalization</para>
		/// </summary>
		protected new void Finalizing() => base.Finalizing();

		public override void DeleteTile() => Deleting();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Deleting()</code></para>
		/// <para>Do the deletion administration</para>
		/// </summary>
		protected new void Deleting() => base.Deleting();

		public override int BuildPrice => 500;

		public override float Transparency => 1;
		#endregion

		#region Common implementation
		public Tile GetTile() => this;
		#endregion

		/// <summary>
		/// Construct a new electric pole
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		public ElectricPoleTile(int x, int y, uint designID) : base(x, y, designID)
		{

		}
	}
}
