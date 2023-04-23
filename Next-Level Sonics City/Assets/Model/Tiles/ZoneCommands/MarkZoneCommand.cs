using Model.Tiles.Buildings;

namespace Model.Tiles.ZoneCommands
{
	public class MarkZoneCommand : IExecutionCommand
	{
		private Tile[,] _matrix;
		private int _x;
		private int _y;
		private ZoneType _zoneType;
		private uint _designID;

		public MarkZoneCommand(Tile[,] matrix, int x, int y, ZoneType zoneType)
		{
			_matrix = matrix;
			_x = x;
			_y = y;
			_zoneType = zoneType;
			_designID = _matrix[_x, _y].DesignID;
		}

		public void Execute()
		{
			switch (_zoneType)
			{
				case ZoneType.IndustrialZone:
					_matrix[_x, _y] = new Industrial(_x, _y, _designID);
					break;
				case ZoneType.CommercialZone:
					_matrix[_x, _y] = new Commercial(_x, _y, _designID);
					break;
				case ZoneType.ResidentialZone:
					_matrix[_x, _y] = new ResidentialBuildingTile(_x, _y, _designID);
					break;
				default:
					break;
			}
		}
	}
}