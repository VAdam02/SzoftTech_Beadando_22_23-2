using Model.Simulation;

namespace Model.Tiles.Buildings.BuildingCommands
{
	public class ExpandCommand : IExecutionCommand
	{
		private readonly int _x;
		private readonly int _y;
		private readonly IWorkplace _workplace;

		/// <summary>
		/// Create a new ExpandCommand
		/// </summary>
		/// <param name="x">X coordinate of core tile</param>
		/// <param name="y">Y coordinate of core tile</param>
		/// <param name="workplace">Workplace instace of core</param>
		public ExpandCommand(int x, int y, IWorkplace workplace)
		{
			_x = x;
			_y = y;
			_workplace = workplace;
		}

		/// <summary>
		/// Expand the workplace
		/// </summary>
		public void Execute()
		{
			City.Instance.SetTile(new WorkplaceSubTile(_x, _y, 0, _workplace));
			BuildingManager.Instance.BuildingDestroyed += ((WorkplaceSubTile)City.Instance.GetTile(_x, _y)).ParentDestroyedEventHandler;
		}
	}
}
