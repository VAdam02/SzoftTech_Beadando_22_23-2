using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.ElectricGrids
{
	public class ElectricGrid
	{
		public event EventHandler PowerStateChanged;

		public bool GetPowerState() => PowerProduction - PowerConsumption >= 0;

		/// <summary>
		/// It is also register itself in ElectricGridManager
		/// </summary>
		public ElectricGrid()
		{
			ElectricGridManager.Instance.AddElectricGrid(this);
		}

		private readonly List<IElectricGridElement> _electricGridElements = new();
		public List<IElectricGridElement> ElectricGridElements { get { return _electricGridElements; } }

		/// <summary>
		/// Add electricGridElement to this
		/// </summary>
		/// <param name="electricGridElement">Element that should be added</param>
		public void AddElectricGridElement(IElectricGridElement electricGridElement)
		{
			lock (_electricGridElements)
				_electricGridElements.Add(electricGridElement);
		}

		/// <summary>
		/// Remove electricGridElement from this
		/// </summary>
		/// <param name="electricGridElement">Element that should be removed</param>
		public void RemoveElectricGridElement(IElectricGridElement electricGridElement)
		{
			lock (_electricGridElements)
				_electricGridElements.Remove(electricGridElement);
		}

		public int PowerConsumption { get => _consumers.Aggregate(0, (sum, current) => sum + current.GetPowerConsumption()); }
		public int PowerProduction { get => _producers.Aggregate(0, (sum, current) => sum + current.GetPowerProduction()); }
		private readonly List<IPowerConsumer> _consumers = new();
		private readonly List<IPowerProducer> _producers = new();
		public List<IPowerConsumer> Consumers { get { return _consumers; } }
		public List<IPowerProducer> Producers { get { return _producers; } }

		/// <summary>
		/// Add producer to this
		/// </summary>
		/// <param name="producer">Producer that should be added</param>
		public void AddProducer(IPowerProducer producer)
		{
			bool prev = GetPowerState();
			lock (_producers) _producers.Add(producer);
			if (prev != GetPowerState()) PowerStateChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Remove producer from this
		/// </summary>
		/// <param name="producer">Producer that should be removed</param>
		public void RemoveProducer(IPowerProducer producer)
		{
			bool prev = GetPowerState();
			lock (_producers) _producers.Remove(producer);
			if (prev != GetPowerState()) PowerStateChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Add consumer to this
		/// </summary>
		/// <param name="consumer">Consumer that should be added</param>
		public void AddConsumer(IPowerConsumer consumer)
		{
			bool prev = GetPowerState();
			lock (_consumers) _consumers.Add(consumer);
			if (prev != GetPowerState()) PowerStateChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Remove consumer from this
		/// </summary>
		/// <param name="consumer">Consumer that should be removed</param>
		public void RemoveConsumer(IPowerConsumer consumer)
		{
			bool prev = GetPowerState();
			lock (_consumers) _consumers.Remove(consumer);
			if (prev != GetPowerState()) PowerStateChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Reinitialize the electric grid and fix errors which may caused by removing electric grid elements and the graph splitted
		/// </summary>
		public void Reinit()
		{
			Queue<IElectricGridElement> queue = new(_electricGridElements);

			while (queue.Count > 0)
			{
				IElectricGridElement electricGridElement = queue.Dequeue();

				List<IElectricGridElement> adjacentElectricGridElements = electricGridElement.ConnectsTo;

				for (int i = 0; i < adjacentElectricGridElements.Count; i++)
				{
					if (adjacentElectricGridElements[i] == null) { continue; }

					if (adjacentElectricGridElements[i].ElectricGrid == this || adjacentElectricGridElements[i].ElectricGrid == null)
					{
						queue.Enqueue(adjacentElectricGridElements[i]);
						continue;
					}

					if (electricGridElement.ElectricGrid == this || electricGridElement.ElectricGrid == null)
					{
						electricGridElement.ElectricGrid = adjacentElectricGridElements[i].ElectricGrid;
					}
					else
					{
						adjacentElectricGridElements[i].ElectricGrid.Merge(electricGridElement.ElectricGrid);
					}
				}

				if (electricGridElement.ElectricGrid == this || electricGridElement.ElectricGrid == null)
				{
					electricGridElement.ElectricGrid = new();
				}
			}

			ElectricGridManager.Instance.RemoveElectricGrid(this);
		}

		/// <summary>
		/// Merge electricGrid into this
		/// </summary>
		/// <param name="electricGrid">Electric grid that will be merged into this</param>
		public void Merge(ElectricGrid electricGrid)
		{
			if (this == electricGrid) return;

			while (electricGrid._electricGridElements.Count > 0)
			{
				electricGrid._electricGridElements[0].ElectricGrid = this;
			}

			ElectricGridManager.Instance.RemoveElectricGrid(electricGrid);
		}
	}
}
