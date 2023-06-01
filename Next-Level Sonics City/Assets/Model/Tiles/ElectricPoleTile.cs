using System.Collections.Generic;
using System.Linq;
using System;
using Model.ElectricGrids;

namespace Model.Tiles
{
	public class ElectricPoleTile : Tile, IElectricGridElement
	{
		#region Tile implementation
		public override TileType GetTileType() { return TileType.ElectricPole; }

		public override void FinalizeTile() => Finalizing();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Finalizing()</code></para>
		/// <para>Do the actual finalization</para>
		/// </summary>
		protected new void Finalizing()
		{
			RegisterNeighbourTileDeleteListeners();
			base.Finalizing();
		}

		public override void DeleteTile() => Deleting();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Deleting()</code></para>
		/// <para>Do the deletion administration</para>
		/// </summary>
		protected new void Deleting() => base.Deleting();

		public override int BuildPrice => 500;

		public override float Transparency => 1;
		#endregion

		#region Common implementation
		public Tile GetTile() => this;
		#endregion

		#region IPoleGridElement implementation
		void IElectricGridElement.RegisterElectricGridElement()
		{
			if (!_isFinalized) { throw new InvalidOperationException("Tile is not set in the city"); }

			ElectricGridManager.Instance.AddElectricGridElement(this);
		}

		void IElectricGridElement.UnregisterElectricGridElement()
		{
			if (!_isFinalized) { throw new InvalidOperationException("Tile is not set in the city"); }

			SetElectricGrid(null);
		}

		private readonly IElectricGridElement[] _poles = new IElectricGridElement[4];
		List<IElectricGridElement> IElectricGridElement.ConnectsTo { get => _poles.Where(x => x != null).ToList(); }

		private void RegisterNeighbourTileDeleteListeners()
		{
			if (City.Instance.GetTile(Coordinates.x, Coordinates.y - 1) is IElectricGridElement aboveGrid) { ConnectsFromAbove = aboveGrid; }
			if (City.Instance.GetTile(Coordinates.x + 1, Coordinates.y) is IElectricGridElement rightGrid) { ConnectsFromRight = rightGrid; }
			if (City.Instance.GetTile(Coordinates.x, Coordinates.y + 1) is IElectricGridElement belowGrid) { ConnectsFromBelow = belowGrid; }
			if (City.Instance.GetTile(Coordinates.x - 1, Coordinates.y) is IElectricGridElement leftGrid) { ConnectsFromLeft = leftGrid; }

			if (City.Instance.GetTile(Coordinates.x, Coordinates.y - 1) is Tile aboveTile) { aboveTile.OnTileDelete += TileDeleteHandler; }
			if (City.Instance.GetTile(Coordinates.x + 1, Coordinates.y) is Tile rightTile) { rightTile.OnTileDelete += TileDeleteHandler; }
			if (City.Instance.GetTile(Coordinates.x, Coordinates.y + 1) is Tile belowTile) { belowTile.OnTileDelete += TileDeleteHandler; }
			if (City.Instance.GetTile(Coordinates.x - 1, Coordinates.y) is Tile leftTile) { leftTile.OnTileDelete += TileDeleteHandler; }
		}

		private void TileDeleteHandler(object sender, Tile oldTile)
		{
			Tile newTile = City.Instance.GetTile(oldTile);
			newTile.OnPreTileDelete += TileDeleteHandler;

			if (newTile is IElectricGridElement pole)
			{
				if (oldTile.Coordinates.x < Coordinates.x) { ConnectsFromLeft = pole; }
				else if (oldTile.Coordinates.x > Coordinates.x) { ConnectsFromRight = pole; }
				else if (oldTile.Coordinates.y > Coordinates.y) { ConnectsFromBelow = pole; }
				else if (oldTile.Coordinates.y < Coordinates.y) { ConnectsFromAbove = pole; }
			}
			else
			{
				if (oldTile.Coordinates.x < Coordinates.x) { ConnectsFromLeft = null; }
				else if (oldTile.Coordinates.x > Coordinates.x) { ConnectsFromRight = null; }
				else if (oldTile.Coordinates.y > Coordinates.y) { ConnectsFromBelow = null; }
				else if (oldTile.Coordinates.y < Coordinates.y) { ConnectsFromAbove = null; }
			}
		}

		public IElectricGridElement ConnectsFromAbove
		{
			get { return _poles[0]; }
			private set
			{
				_poles[0] = value;
				if (value == null) { DesignID &= ~ABOVEPOLEMASK; }
				else { DesignID |= ABOVEPOLEMASK; }
			}
		}
		public IElectricGridElement ConnectsFromRight
		{
			get { return _poles[1]; }
			private set
			{
				_poles[1] = value;
				if (value == null) { DesignID &= ~RIGHTPOLEMASK; }
				else { DesignID |= RIGHTPOLEMASK; }
			}
		}
		public IElectricGridElement ConnectsFromBelow
		{
			get { return _poles[2]; }
			private set
			{
				_poles[2] = value;
				if (value == null) { DesignID &= ~BELOWPOLEMASK; }
				else { DesignID |= BELOWPOLEMASK; }
			}
		}
		public IElectricGridElement ConnectsFromLeft
		{
			get { return _poles[3]; }
			private set
			{
				_poles[3] = value;
				if (value == null) { DesignID &= ~LEFTPOLEMASK; }
				else { DesignID |= LEFTPOLEMASK; }
			}
		}

		private ElectricGrid _electricGrid = null;

		ElectricGrid IElectricGridElement.ElectricGrid
		{
			get => _electricGrid;
			set
			{
				SetElectricGrid(value);
			}
		}

		/// <summary>
		/// Sets the new electricgrid
		/// </summary>
		/// <param name="electricGrid">New electricgrid</param>
		private void SetElectricGrid(ElectricGrid electricGrid)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			if (_electricGrid == electricGrid) { return; }

			if (electricGrid == null)
			{
				_electricGrid?.RemoveElectricGridElement(this);
				_electricGrid?.Reinit();
			}
			else
			{
				_electricGrid?.RemoveElectricGridElement(this);
				_electricGrid = electricGrid;
				_electricGrid?.AddElectricGridElement(this);
			}
		}
		#endregion

		public const uint ABOVEPOLEMASK = 1;
		public const uint RIGHTPOLEMASK = 2;
		public const uint BELOWPOLEMASK = 4;
		public const uint LEFTPOLEMASK = 8;

		/// <summary>
		/// Construct a new electric pole
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		public ElectricPoleTile(int x, int y, uint designID) : base(x, y, designID)
		{

		}
	}
}
