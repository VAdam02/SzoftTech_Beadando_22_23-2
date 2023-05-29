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
			if (!(_zoneType == ZoneType.NoZone
			  || City.Instance.GetTile(_x - 1, _y) is IRoadGridElement || City.Instance.GetTile(_x + 1, _y) is IRoadGridElement
			  || City.Instance.GetTile(_x, _y - 1) is IRoadGridElement || City.Instance.GetTile(_x, _y + 1) is IRoadGridElement))
			{ return; }

			switch (_zoneType)
			{
				case ZoneType.IndustrialZone:
					if (City.Instance.GetTile(_x, _y) is not EmptyTile) { break; }
					City.Instance.SetTile(new Industrial(_x, _y, _designID));
					ZoneManager.Instance.OnZoneMarked(City.Instance.GetTile(_x, _y));
					break;
				case ZoneType.CommercialZone:
					if (City.Instance.GetTile(_x, _y) is not EmptyTile) { break; }
					City.Instance.SetTile(new Commercial(_x, _y, _designID));
					ZoneManager.Instance.OnZoneMarked(City.Instance.GetTile(_x, _y));
					break;
				case ZoneType.ResidentialZone:
					if (City.Instance.GetTile(_x, _y) is not EmptyTile) { break; }
					City.Instance.SetTile(new ResidentialBuildingTile(_x, _y, _designID));
					ZoneManager.Instance.OnZoneMarked(City.Instance.GetTile(_x, _y));
					break;
				case ZoneType.NoZone:
					if (City.Instance.GetTile(_x, _y) is not Industrial && City.Instance.GetTile(_x, _y) is not Commercial && City.Instance.GetTile(_x, _y) is not ResidentialBuildingTile) { break; }
					ZoneManager.Instance.OnZoneUnMarked(City.Instance.GetTile(_x, _y));
					City.Instance.SetTile(new EmptyTile(_x, _y));
					break;
				default:
					throw new NotImplementedException("Not implemented zone type");
			}
		}
	}
}