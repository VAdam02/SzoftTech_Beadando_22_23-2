using Model.Tiles.Buildings;
using System;

namespace Model.Tiles
{
	public class EmptyTile : Tile, IZoneBuilding
	{
		/// <summary>
		/// Construct a new empty tile
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		public EmptyTile(int x, int y, uint designID) : base(x, y, designID)
		{

		}

		public override TileType GetTileType() { throw new InvalidOperationException(); }

		public override bool CanBuild()
		{
			return City.Instance.GetTile(Coordinates) is not EmptyTile;
		}

		public override int BuildPrice => 0;

		public override int DestroyIncome => 0;

		public override float Transparency => 1;

		ZoneBuildingLevel IZoneBuilding.Level => throw new InvalidOperationException();

		void IZoneBuilding.LevelUp()
		{
			throw new InvalidOperationException();
		}

		int IZoneBuilding.GetLevelUpCost()
		{
			throw new InvalidOperationException();
		}

		public Tile GetTile()
		{
			return this;
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

		ZoneType IZoneBuilding.GetZoneType() => ZoneType.NoZone;
	}
}