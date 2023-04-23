using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Model.Tiles.ZoneCommands;

namespace Model.Tiles
{
	public class ZoneManager : MonoBehaviour
	{
		private readonly object _lockObject = new object();
		public ZoneManager() { }

		public void MarkZone(Tile[,] matrix, Tile topLeft, Tile bottomRight, ZoneType zoneType)
		{
			int rowStart = (int)topLeft.Coordinates[0];
			int rowEnd = (int)bottomRight.Coordinates[0] + 1;
			int columnStart = (int)topLeft.Coordinates[1];
			int columnEnd = (int)bottomRight.Coordinates[1] + 1;

			Parallel.For(rowStart, rowEnd, x =>
			{
				for (int y = columnStart; y < columnEnd; ++y)
				{
					lock (_lockObject)
					{
						MarkZoneCommand markZoneCommand = new MarkZoneCommand(matrix, x, y, zoneType);
						markZoneCommand.Execute();
					}
				}
			});
		}

		public void UnMarkZone(Tile[,] matrix, Tile topLeft, Tile bottomRight)
		{
			int rowStart = (int)topLeft.Coordinates[0];
			int rowEnd = (int)bottomRight.Coordinates[0] + 1;
			int columnStart = (int)topLeft.Coordinates[1];
			int columnEnd = (int)bottomRight.Coordinates[1] + 1;

			Parallel.For(rowStart, rowEnd, x =>
			{
				for (int y = columnStart; y < columnEnd; ++y)
				{
					lock (_lockObject)
					{
						UnMarkZoneCommand unMarkZoneCommand = new UnMarkZoneCommand(matrix, x, y);
						unMarkZoneCommand.Execute();
					}
				}
			});
		}
	}
}