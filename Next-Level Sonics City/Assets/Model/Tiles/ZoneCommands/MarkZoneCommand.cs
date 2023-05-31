using Model.RoadGrids;
using Model.Tiles.Buildings;
using System;

namespace Model.Tiles.ZoneCommands
{
	public class MarkZoneCommand : IExecutionCommand
	{
		private readonly int _x;
		private readonly int _y;
		private readonly ZoneType _zoneType;
		private readonly uint _designID;

		/// <summary>
		/// Create a new MarkZoneCommand
		/// </summary>
		/// <param name="x">X coordinate of the created tile</param>
		/// <param name="y">Y coordinate of the created tile</param>
		/// <param name="zoneType">Type of the tile</param>
		public MarkZoneCommand(int x, int y, ZoneType zoneType)
		{
			_x = x;
			_y = y;
			_zoneType = zoneType;
			_designID = ResidentialBuildingTile.GenerateResidential(0);
		}

		/// <summary>
		/// Build the tile
		/// </summary>
		public void Execute()
		{
			if (!(_zoneType == ZoneType.NoZone || _zoneType == ZoneType.VoidZone
			  || City.Instance.GetTile(_x - 1, _y) is IRoadGridElement || City.Instance.GetTile(_x + 1, _y) is IRoadGridElement
			  || City.Instance.GetTile(_x, _y - 1) is IRoadGridElement || City.Instance.GetTile(_x, _y + 1) is IRoadGridElement))
			{ return; }

			Tile tile = City.Instance.GetTile(_x, _y);
			switch (_zoneType)
			{
				case ZoneType.IndustrialZone:
					if (tile is not EmptyTile) { break; }
					City.Instance.SetTile(new IndustrialBuildingTile(_x, _y, _designID));
					ZoneManager.Instance.OnZoneMarked(tile);
					break;
				case ZoneType.CommercialZone:
					if (tile is not EmptyTile) { break; }
					City.Instance.SetTile(new CommercialBuildingTIle(_x, _y, _designID));
					ZoneManager.Instance.OnZoneMarked(tile);
					break;
				case ZoneType.ResidentialZone:
					if (tile is not EmptyTile) { break; }
					City.Instance.SetTile(new ResidentialBuildingTile(_x, _y, _designID));
					ZoneManager.Instance.OnZoneMarked(tile);
					break;
				case ZoneType.NoZone:
					if (tile is not IndustrialBuildingTile && tile is not CommercialBuildingTIle && tile is not ResidentialBuildingTile) { break; }
					if (tile is IZoneBuilding zoneBuilding && zoneBuilding.Level != ZoneBuildingLevel.ZERO) { break; }
					City.Instance.SetTile(new EmptyTile(_x, _y));
					ZoneManager.Instance.OnZoneUnMarked(tile);
					break;
				case ZoneType.VoidZone:
					if (tile is not IndustrialBuildingTile && tile is not CommercialBuildingTIle && tile is not ResidentialBuildingTile) { break; }
					City.Instance.SetTile(new EmptyTile(_x, _y));
					ZoneManager.Instance.OnZoneUnMarked(tile);
					break;
				default:
					throw new NotImplementedException("Not implemented zone type");
			}
		}
	}
}