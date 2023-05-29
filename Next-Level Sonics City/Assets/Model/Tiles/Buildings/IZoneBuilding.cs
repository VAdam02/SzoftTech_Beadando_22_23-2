using System;

namespace Model.Tiles.Buildings
{
	public interface IZoneBuilding
	{
		/// <summary>
		/// Returns the type of zone this ZoneBuilding
		/// </summary>
		/// <returns></returns>
		public ZoneType GetZoneType();

		/// <summary>
		/// Level up the building
		/// </summary>
		public void LevelUp();

		/// <summary>
		/// Returns the cost of leveling up the building
		/// </summary>
		public int LevelUpCost { get; }

		/// <summary>
		/// Returns the current level of the building
		/// </summary>
		public ZoneBuildingLevel Level { get; }

		/// <summary>
		/// Returns the tile of the zone building
		/// </summary>
		/// <returns>Tile of the zone building</returns>
		public Tile GetTile();
	}
}