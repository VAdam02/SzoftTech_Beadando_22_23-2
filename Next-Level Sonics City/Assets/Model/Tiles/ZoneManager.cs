using System.Threading.Tasks;
using Model.Tiles.ZoneCommands;

namespace Model.Tiles
{
	public class ZoneManager
	{
		private readonly object _lock = new ();
		public ZoneManager() { }

		public void MarkZone(Tile topLeft, Tile bottomRight, ZoneType zoneType)
		{
			int rowStart = (int)topLeft.Coordinates.x;
			int rowEnd = (int)bottomRight.Coordinates.x + 1;
			int columnStart = (int)topLeft.Coordinates.y;
			int columnEnd = (int)bottomRight.Coordinates.y + 1;

			Parallel.For(rowStart, rowEnd, x =>
			{
				for (int y = columnStart; y < columnEnd; ++y)
				{
					lock (_lock)
					{
						MarkZoneCommand markZoneCommand = new (x, y, zoneType);
						markZoneCommand.Execute();
					}
				}
			});
		}

		public void UnMarkZone(Tile topLeft, Tile bottomRight)
		{
			int rowStart = (int)topLeft.Coordinates.x;
			int rowEnd = (int)bottomRight.Coordinates.x + 1;
			int columnStart = (int)topLeft.Coordinates.y;
			int columnEnd = (int)bottomRight.Coordinates.y + 1;

			Parallel.For(rowStart, rowEnd, x =>
			{
				for (int y = columnStart; y < columnEnd; ++y)
				{
					lock (_lock)
					{
						UnMarkZoneCommand unMarkZoneCommand = new (x, y);
						unMarkZoneCommand.Execute();
					}
				}
			});
		}
	}
}