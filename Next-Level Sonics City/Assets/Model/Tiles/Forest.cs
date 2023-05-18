using Model.Statistics;

namespace Model.Tiles
{
	public class Forest : Tile
	{
		private int _plantedYear;

		private const int MAINTANCENEEDEDFORYEAR = 10;

		/// <summary>
		/// Construct a new forest
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		public Forest(int x, int y, uint designID) : base(x, y, designID)
		{
			
		}

		public override TileType GetTileType() { return TileType.Forest; }
		
		public override void FinalizeTile()
		{
			Finalizing();
		}

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Finalizing()</code></para>
		/// <para>Do the actual finalization</para>
		/// </summary>
		protected new void Finalizing()
		{
			base.Finalizing();
			_plantedYear = StatEngine.Instance.Year;
		}

		public override int GetBuildPrice()
		{
			//TODO implement forest build price
			return 100000;
		}

		public override int GetDestroyIncome()
		{
			//TODO implement forest destroy price
			return 100000;
		}

		public override int GetMaintainanceCost()
		{
			//TODO implement forest maintainance cost
			if (_plantedYear + MAINTANCENEEDEDFORYEAR < StatEngine.Instance.Year) { return 0; }
			return 100000;
		}
	}
}