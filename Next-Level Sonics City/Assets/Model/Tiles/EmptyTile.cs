using Model.Tiles.Buildings;
using System;

namespace Model.Tiles
{
	public class EmptyTile : Tile, IZoneBuilding, ITransparent
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

		public override int GetBuildPrice()
		{
			return 0;
		}

		public override int GetDestroyIncome()
		{
			return 0;
		}

		ZoneType IZoneBuilding.GetZoneType()
		{
			return ZoneType.NoZone;
		}

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

		public float GetTransparency()
		{
			return 1;
		}
	}
}