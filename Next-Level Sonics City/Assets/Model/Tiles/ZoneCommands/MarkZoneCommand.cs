using Model.Simulation;
using Model.Tiles.Buildings;

namespace Model.Tiles.ZoneCommands
{
	public class MarkZoneCommand : IExecutionCommand
	{
		private readonly int _x;
		private readonly int _y;
		private readonly ZoneType _zoneType;
		private readonly uint _designID;

		public MarkZoneCommand(int x, int y, ZoneType zoneType)
		{;
			_x = x;
			_y = y;
			_zoneType = zoneType;
			_designID = ResidentialBuildingTile.GenerateResidential(0);
		}

		public void Execute()
		{
			if (SimEngine.Instance.Tiles[_x, _y] is not EmptyTile)
			{
				return;
			}

			switch (_zoneType)
			{
				case ZoneType.IndustrialZone:
					SimEngine.Instance.Tiles[_x, _y] = new Industrial(_x, _y, _designID); // TODO update to IndustrialBuildingTIle
					break;
				case ZoneType.CommercialZone:
					SimEngine.Instance.Tiles[_x, _y] = new Commercial(_x, _y, _designID); // TODO update to CommercialBuildingTile
					break;
				case ZoneType.ResidentialZone:
					SimEngine.Instance.Tiles[_x, _y] = new ResidentialBuildingTile(_x, _y, _designID);
					break;
				default:
					break;
			}

			SimEngine.Instance.Tiles[_x, _y].OnTileChange.Invoke();
		}
	}
}