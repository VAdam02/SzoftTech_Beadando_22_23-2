using Model.Tiles.Buildings;
using Model.Tiles.ZoneCommands;
using System;
using System.Threading.Tasks;

namespace Model.Tiles
{
	public class ZoneManager
	{
		private static ZoneManager _instance;
		public static ZoneManager Instance
		{
			get
			{
				_instance ??= new ZoneManager();
				return _instance;
			}
		}

		public delegate void ZoneMarkedOrUnmarkedEventHandler(object sender, TileEventArgs e);
		public event ZoneMarkedOrUnmarkedEventHandler ZoneMarked;
		public event ZoneMarkedOrUnmarkedEventHandler ZoneUnMarked;

		public static void Reset()
		{
			_instance = null;
		}

		private ZoneManager() { }
		
		/// <summary>
		/// Mark a zone area
		/// </summary>
		/// <param name="limit1">One of the corners</param>
		/// <param name="limit2">Other corner</param>
		/// <param name="zoneType">Type of the created zone</param>
		public void MarkZone(Tile limit1, Tile limit2, ZoneType zoneType)
		{
			CalculateSubMatrix(limit1, limit2, out int rowStart, out int rowEnd, out int columnStart, out int columnEnd);

			if (zoneType is not ZoneType.NoZone && zoneType is not ZoneType.VoidZone)
			{
				int row =  rowEnd + 1 - rowStart;
				int column = columnEnd + 1 - columnStart;

				if (row * column < 16) { throw new ArgumentOutOfRangeException("At least 16 tile must be selected"); }
			}
			
			Parallel.For(rowStart, rowEnd + 1, x =>
			{
				for (int y = columnStart; y < columnEnd + 1; ++y)
				{
					MarkZoneCommand markZoneCommand = new (x, y, zoneType);
					markZoneCommand.Execute();
				}
			});
		}

		/// <summary>
		/// Calculate the submatrix of the selected tiles
		/// </summary>
		/// <param name="limit1">One of the corners</param>
		/// <param name="limit2">Other corner</param>
		/// <param name="rowStart">First row index</param>
		/// <param name="rowEnd">Last row index</param>
		/// <param name="columnStart">First column index</param>
		/// <param name="columnEnd">Last column index</param>
		private void CalculateSubMatrix(Tile limit1, Tile limit2, out int rowStart, out int rowEnd, out int columnStart, out int columnEnd)
		{
			rowStart =		(int)Math.Min(limit1.Coordinates.x, limit2.Coordinates.x);
			rowEnd =		(int)Math.Max(limit1.Coordinates.x, limit2.Coordinates.x);
			columnStart =	(int)Math.Min(limit1.Coordinates.y, limit2.Coordinates.y);
			columnEnd =		(int)Math.Max(limit1.Coordinates.y, limit2.Coordinates.y);
		}

		/// <summary>
		/// Invoke the ZoneMarked event
		/// </summary>
		/// <param name="tile">Tile which markerd</param>
		public void OnZoneMarked(Tile tile)
		{
			ZoneMarked?.Invoke(this, new TileEventArgs(tile));
		}

		/// <summary>
		/// Invoke the ZoneUnMarked event
		/// </summary>
		/// <param name="tile">Tile which unmarked</param>
		public void OnZoneUnMarked(Tile tile)
		{
			ZoneUnMarked?.Invoke(this, new TileEventArgs(tile));
		}
	}
}