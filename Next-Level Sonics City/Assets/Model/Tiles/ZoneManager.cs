using System;
using System.Threading.Tasks;
using Model.Simulation;
using Model.Tiles.ZoneCommands;

namespace Model.Tiles
{
	public class ZoneManager
	{
		public delegate void ZoneMarkedOrUnmarkedEventHandler(object sender, TileEventArgs e);
		public event ZoneMarkedOrUnmarkedEventHandler ZoneMarked;
		public event ZoneMarkedOrUnmarkedEventHandler ZoneUnMarked;
		
		private readonly object _lock = new ();
		
		public ZoneManager() { }

		//FIXME it's run in paraller but it's wait for each other because of lock
		public void MarkZone(Tile limit1, Tile limit2, ZoneType zoneType)
		{
			CalculateSubMatrix(limit1, limit2);

			Parallel.For(_rowStart, _rowEnd + 1, x =>
			{
				for (int y = _columnStart; y < _columnEnd + 1; ++y)
				{
					lock (_lock)
					{
						Tile oldTile = SimEngine.Instance.GetTile(x, y);
						MarkZoneCommand markZoneCommand = new (x, y, zoneType);
						markZoneCommand.Execute();

						OnZoneMarked(SimEngine.Instance.GetTile(x, y));
					}
				}
			});
		}

		private int _rowStart;
		private int _rowEnd;
		private int _columnStart;
		private int _columnEnd;

		private void CalculateSubMatrix(Tile limit1, Tile limit2)
		{
			_rowStart =    (int)Math.Min(limit1.Coordinates.x, limit2.Coordinates.x);
			_rowEnd =      (int)Math.Max(limit1.Coordinates.x, limit2.Coordinates.x);
			_columnStart = (int)Math.Min(limit1.Coordinates.y, limit2.Coordinates.y);
			_columnEnd =   (int)Math.Max(limit1.Coordinates.y, limit2.Coordinates.y);
		}

		protected void OnZoneMarked(Tile tile)
		{
			ZoneMarked?.Invoke(this, new TileEventArgs(tile));
		}

		protected void OnZoneUnMarked(Tile tile)
		{
			ZoneUnMarked?.Invoke(this, new TileEventArgs(tile));
		}
	}
}