namespace Model.Tiles.Buildings.BuildingCommands
{
	public class DestroyCommand : IExecutionCommand
	{
		private readonly int _x;
		private readonly int _y;

		/// <summary>
		/// Create a new DestroyCommand
		/// </summary>
		/// <param name="x">X coordinate of the destroyed tile</param>
		/// <param name="y">Y coordinate of the destroyed tile</param>
		public DestroyCommand(int x, int y)
		{
			_x = x;
			_y = y;
		}

		/// <summary>
		/// Destroy the tile
		/// </summary>
		public void Execute()
		{
			City.Instance.SetTile(new EmptyTile(_x, _y, 0));
		}
	}
}