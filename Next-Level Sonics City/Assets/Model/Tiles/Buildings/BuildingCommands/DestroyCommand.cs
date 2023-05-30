using Model.RoadGrids;
using System;

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
			if (IsForcedRequired(City.Instance.GetTile(_x, _y)))
			{
				throw new InvalidOperationException("Force destroy required");
			}
			City.Instance.SetTile(new EmptyTile(_x, _y));
		}

		private bool IsForcedRequired(Tile tile)
		{
			if (tile is IRoadGridElement roadGridElement)
			{
				return roadGridElement.IsLocked;
			}
			
			if (tile is IWorkplace workplace)
			{
				return workplace.GetWorkersCount() > 0;
			}

			if (tile is IResidential residential)
			{
				return residential.GetResidentsCount() > 0;
			}

			return false;
		}
	}
}