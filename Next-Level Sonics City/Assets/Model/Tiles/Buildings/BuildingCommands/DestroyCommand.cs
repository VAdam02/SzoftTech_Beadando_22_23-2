using Model;
using Model.Tiles;


public class DestroyCommand : IExecutionCommand
{
	private readonly Tile _tile;
	private readonly Building _building;

	public DestroyCommand(Tile tile)
	{
		_tile = tile;
	}

	public void Execute() //TODO what contains the building? Deallocation logic
	{
		//if (_tile.Building is not null)
		//{
		//	return;
		//}

		//_tile.Building = null;
	}
}
