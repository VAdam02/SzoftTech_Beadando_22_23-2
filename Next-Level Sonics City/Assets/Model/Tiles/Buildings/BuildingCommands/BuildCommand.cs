using Model;
using Model.Tiles;


public class BuildCommand : IExecutionCommand
{
	private readonly Tile _tile;
	private readonly Building _building;

	public BuildCommand(Tile tile, Building building)
	{
		_tile = tile;
		_building = building;
	}

	public void Execute() //TODO what contains the building?
	{
		//if (_tile.Building is not null)
		//{
		//	return;
		//}

		//_tile.Building = _building;
	}
}
