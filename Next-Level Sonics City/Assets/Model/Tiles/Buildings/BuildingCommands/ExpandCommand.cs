using Model.Simulation;

namespace Model.Tiles.Buildings.BuildingCommands
{
	public class ExpandCommand : IExecutionCommand
	{
		private int _x;
		private int _y;
		private IWorkplace _workplace;

		public ExpandCommand(int x, int y, IWorkplace workplace)
		{
			_x = x;
			_y = y;
			_workplace = workplace;
		}

		public void Execute()
		{
			SimEngine.Instance.SetTile(_x, _y, new WorkplaceSubTile(_x, _y, 0, _workplace));
			SimEngine.Instance.BuildingManager.BuildingDestroyed += ((WorkplaceSubTile)SimEngine.Instance.GetTile(_x, _y)).ParentDestroyedEventHandler;
		}
	}
}
