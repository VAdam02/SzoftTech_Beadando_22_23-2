using Model.Simulation;

namespace Model.Tiles.Buildings.BuildingCommands
{
	public class DestroyCommand : IExecutionCommand
	{
		private readonly int _x;
		private readonly int _y;

		public DestroyCommand(int x, int y)
		{
			_x = x;
			_y = y;
		}

		public void Execute()
		{
			SimEngine.Instance.SetTile(_x, _y, new EmptyTile(_x, _y, 0));
		}
	}
}