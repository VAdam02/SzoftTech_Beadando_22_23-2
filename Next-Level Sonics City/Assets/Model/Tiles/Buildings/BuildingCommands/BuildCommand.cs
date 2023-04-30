using Model.Simulation;

namespace Model.Tiles.Buildings.BuildingCommands
{
	public class BuildCommand : IExecutionCommand
	{
		private readonly TileType _tileType;
		private readonly int _x;
		private readonly int _y;
		private readonly uint _designID;

		public BuildCommand(int x, int y, TileType tileType)
		{
			_tileType = tileType;
			_x = x;
			_y = y;
			_designID = 0;
		}

		public void Execute()
		{
			switch (_tileType)
			{
				case TileType.PoliceDepartment:
					SimEngine.Instance.SetTile(_x, _y, new PoliceDepartment(_x, _y, _designID));
					break;
				case TileType.FireDepartment:
					SimEngine.Instance.SetTile(_x, _y, new FireDepartment(_x, _y, _designID));
					break;
				case TileType.MiddleSchool:
					SimEngine.Instance.SetTile(_x, _y, new MiddleSchool(_x, _y, _designID));
					break;
				case TileType.HighSchool:
					SimEngine.Instance.SetTile(_x, _y, new HighSchool(_x, _y, _designID));
					break;
				case TileType.Stadion:
					SimEngine.Instance.SetTile(_x, _y, new Stadion(_x, _y, _designID));
					break;
				case TileType.PowerPlant:
					SimEngine.Instance.SetTile(_x, _y, new PowerPlant(_x, _y, _designID));
					break;
				case TileType.Forest:
					SimEngine.Instance.SetTile(_x, _y, new Forest(_x, _y, _designID));
					break;
				case TileType.Road:
					SimEngine.Instance.SetTile(_x, _y, new Road(_x, _y, _designID));
					break;
				case TileType.ElectricPole:
					SimEngine.Instance.SetTile(_x, _y, new ElectricPole(_x, _y, _designID));
					break;
				default:
					break;
			}
		}
	}
}