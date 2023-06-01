using Model.ElectricGrids;
using Model.RoadGrids;
using Model.Tiles.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Tiles
{
	public abstract class Building : Tile, IElectricGridElement, IPowerConsumer
	{
		#region IElectricGridElement implementation
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

		private readonly IElectricGridElement[] _electrics = new IElectricGridElement[4];
		List<IElectricGridElement> IElectricGridElement.ConnectsTo { get => _electrics.Where(x => x != null).ToList(); }

		private void RegisterNeighbourTileDeleteListeners()
		{
			if (City.Instance.GetTile(Coordinates.x, Coordinates.y - 1) is IElectricGridElement aboveElectric) { ConnectsFromAbove = aboveElectric; }
			if (City.Instance.GetTile(Coordinates.x + 1, Coordinates.y) is IElectricGridElement rightElectric) { ConnectsFromRight = rightElectric; }
			if (City.Instance.GetTile(Coordinates.x, Coordinates.y + 1) is IElectricGridElement belowElectric) { ConnectsFromBelow = belowElectric; }
			if (City.Instance.GetTile(Coordinates.x - 1, Coordinates.y) is IElectricGridElement leftElectric) { ConnectsFromLeft = leftElectric; }

			if (City.Instance.GetTile(Coordinates.x, Coordinates.y - 1) is Tile aboveTile) { aboveTile.OnTileDelete += TileDeleteHandler; }
			if (City.Instance.GetTile(Coordinates.x + 1, Coordinates.y) is Tile rightTile) { rightTile.OnTileDelete += TileDeleteHandler; }
			if (City.Instance.GetTile(Coordinates.x, Coordinates.y + 1) is Tile belowTile) { belowTile.OnTileDelete += TileDeleteHandler; }
			if (City.Instance.GetTile(Coordinates.x - 1, Coordinates.y) is Tile leftTile) { leftTile.OnTileDelete += TileDeleteHandler; }
		}

		private void TileDeleteHandler(object sender, Tile oldTile)
		{
			Tile newTile = City.Instance.GetTile(oldTile);
			newTile.OnPreTileDelete += TileDeleteHandler;

			if (newTile is IElectricGridElement electric)
			{
				if (oldTile.Coordinates.x < Coordinates.x) { ConnectsFromLeft = electric; }
				else if (oldTile.Coordinates.x > Coordinates.x) { ConnectsFromRight = electric; }
				else if (oldTile.Coordinates.y > Coordinates.y) { ConnectsFromBelow = electric; }
				else if (oldTile.Coordinates.y < Coordinates.y) { ConnectsFromAbove = electric; }
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
			get { return _electrics[0]; }
			private set
			{
				_electrics[0] = value;
			}
		}
		public IElectricGridElement ConnectsFromRight
		{
			get { return _electrics[1]; }
			private set
			{
				_electrics[1] = value;
			}
		}
		public IElectricGridElement ConnectsFromBelow
		{
			get { return _electrics[2]; }
			private set
			{
				_electrics[2] = value;
			}
		}
		public IElectricGridElement ConnectsFromLeft
		{
			get { return _electrics[3]; }
			private set
			{
				_electrics[3] = value;
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

			{
				if (this is IPowerProducer producer)
				{
					producer.UnregisterPowerProducer(_electricGrid);
				}
				if (this is IPowerConsumer consumer)
				{
					consumer.UnregisterPowerConsumer(_electricGrid);
				}
			}

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

			if (_electricGrid != null)
			{
				if (this is IPowerProducer producer)
				{
					producer.RegisterPowerProducer(_electricGrid);
				}
				if (this is IPowerConsumer consumer)
				{
					consumer.RegisterPowerConsumer(_electricGrid);
				}
			}
		}
		#endregion

		#region IPowerConsumer
		void IPowerConsumer.RegisterPowerConsumer(ElectricGrid elctricGrid)
		{
			if (!_isFinalized) { throw new InvalidOperationException("Tile is not set in the city"); }

			ElectricGridManager.Instance.AddElectricGridElement(this);
		}

		void IPowerConsumer.UnregisterPowerConsumer(ElectricGrid electricGrid)
		{
			if (!_isFinalized) { throw new InvalidOperationException("Tile is not set in the city"); }

			SetElectricGrid(null);
		}

		public abstract int GetPowerConsumption();
		#endregion

		public event EventHandler OnRotationChanged;

		private Rotation _rotation;
		public Rotation Rotation
		{
			get { return _rotation; }
			private set
			{
				if (_rotation != value) { OnRotationChanged?.Invoke(this, EventArgs.Empty); }
				_rotation = value;
			}
		}

		public Building(int x, int y, uint designID, Rotation rotation) : base(x, y, designID)
		{
			Rotation = rotation;
		}

		public void Rotate()
		{
			if (_isFinalized) { throw new InvalidOperationException("Not allowed to rotate the tile after finalized"); }

			Rotation = (Rotation)(((int)Rotation + 1) % Enum.GetValues(typeof(Rotation)).Length);
		}

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Finalizing()</code></para>
		/// <para>Do the actual finalization</para>
		/// </summary>
		protected new void Finalizing()
		{
			base.Finalizing();

			RoadGrid roadGrid = RoadGridManager.GetRoadGrigElementByBuilding(this)?.RoadGrid;
			if (roadGrid != null)
			{
				if (this is IWorkplace workplace)
				{
					workplace.RegisterWorkplace(roadGrid);
				}
				if (this is IResidential residential)
				{
					residential.RegisterResidential(roadGrid);
				}
			}

			if (this is IPowerProducer producer)
			{
				producer.RegisterPowerProducer(((IElectricGridElement)producer).ElectricGrid);
			}
			if (this is IPowerConsumer consumer)
			{
				consumer.RegisterPowerConsumer(((IElectricGridElement)consumer).ElectricGrid);
			}
		}

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Deleting()</code></para>
		/// <para>Do the deletion administration</para>
		/// </summary>
		protected new void Deleting()
		{
			base.Deleting();

			if (this is IWorkplace workplace)
			{
				workplace.UnregisterWorkplace(RoadGridManager.GetRoadGrigElementByBuilding((Building)workplace)?.RoadGrid);
			}	
			if (this is IResidential residential)
			{
				residential.UnregisterResidential(RoadGridManager.GetRoadGrigElementByBuilding((Building)residential)?.RoadGrid);
			}

			if (this is IPowerProducer producer)
			{
				producer.UnregisterPowerProducer(((IElectricGridElement)producer).ElectricGrid);
			}
			if (this is IPowerConsumer consumer)
			{
				consumer.UnregisterPowerConsumer(((IElectricGridElement)consumer).ElectricGrid);
			}
		}

		/// <summary>
		/// Expand the building
		/// </summary>
		internal virtual void Expand()
		{

		}

		public Tile GetTile() => this;
	}
}