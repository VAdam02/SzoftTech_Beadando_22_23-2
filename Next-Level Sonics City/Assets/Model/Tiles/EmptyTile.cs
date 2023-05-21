using Model.Tiles.Buildings;
using System;

namespace Model.Tiles
{
	public class EmptyTile : Tile, IZoneBuilding
	{
		#region Tile implementation
		public override TileType GetTileType() { throw new InvalidOperationException("This is not a valid tile type"); }

		public override bool CanBuild() => City.Instance.GetTile(Coordinates) is not EmptyTile;

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

		public override int BuildPrice => 0;

		public override int DestroyIncome => 0;

		public override float Transparency => 1;
		#endregion

		#region IZoneBuilding implementation
		ZoneType IZoneBuilding.GetZoneType() => ZoneType.NoZone;

		void IZoneBuilding.LevelUp() => throw new InvalidOperationException("Empty tile can't be level upped");

		int IZoneBuilding.GetLevelUpCost() => throw new InvalidOperationException("Empty tile can't be level upped");

		ZoneBuildingLevel IZoneBuilding.Level => throw new InvalidOperationException("Empty tile do not have level");
		#endregion

		#region Common implementation
		public Tile GetTile()
		{
			return this;
		}
		#endregion

		/// <summary>
		/// Construct a new empty tile
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		public EmptyTile(int x, int y) : base(x, y, 0)
		{

		}
	}
}