using Model.Tiles.Buildings;
using Model.Simulation;

namespace Model.Tiles.ZoneCommands
{
	public class UnMarkZoneCommand : IExecutionCommand
	{
		private readonly int _x;
		private readonly int _y;
		private readonly uint _designID;

		public UnMarkZoneCommand(int x, int y)
		{
			_x = x;
			_y = y;
			_designID = ResidentialBuildingTile.GenerateResidential(0);
		}

		public void Execute()
		{
			if (SimEngine.Instance.GetTile(_x, _y) is not IZoneBuilding)
			{
				return;
			}

			DestroyCommand dc = new (_x, _y);
			dc.Execute();

			SimEngine.Instance.SetTile(_x, _y, new EmptyTile(_x, _y, _designID));
		}
	}
}