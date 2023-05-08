using Model.Simulation;
using System.Collections.Generic;

namespace Model.RoadGrids
{
	public class RoadGrid
	{
		public RoadGrid()
		{
			SimEngine.Instance.RoadGridManager.AddRoadGrid(this);
		}

		private readonly List<IRoadGridElement> _roadGridElements = new();
		public List<IRoadGridElement> RoadGridElements { get { return _roadGridElements; } }
		public void AddRoadGridElement(IRoadGridElement roadGridElement)
		{
			_roadGridElements.Add(roadGridElement);
		}
		public void RemoveRoadGridElement(IRoadGridElement roadGridElement)
		{
			_roadGridElements.Remove(roadGridElement);
		}

		private readonly List<IWorkplace> _workplaces = new();
		public List<IWorkplace> Workplaces { get { return _workplaces; } }
		public void AddWorkplace(IWorkplace workplace)
		{
			_workplaces.Add(workplace);
		}
		public void RemoveWorkplace(IWorkplace workplace)
		{
			_workplaces.Remove(workplace);
		}

		private readonly List<IResidential> _residential = new();
		public List<IResidential> Residentials { get { return _residential; } }
		public void AddResidential(IResidential residential)
		{
			_residential.Add(residential);
		}
		public void RemoveResidential(IResidential residential)
		{
			_residential.Remove(residential);
		}

		public void Reinit()
		{
			Queue<IRoadGridElement> queue = new(_roadGridElements);

			while (queue.Count > 0)
			{
				IRoadGridElement roadGridElement = queue.Dequeue();

				List<IRoadGridElement> adjacentRoadGridElements = RoadGridManager.GetRoadGridElementsByRoadGridElement(roadGridElement);

				for (int i = 0; i < adjacentRoadGridElements.Count; i++)
				{
					if (adjacentRoadGridElements[i] == null) { continue; }

					if (adjacentRoadGridElements[i].GetRoadGrid() == this || adjacentRoadGridElements[i].GetRoadGrid() == null)
					{
						queue.Enqueue(adjacentRoadGridElements[i]);
						continue;
					}

					if (roadGridElement.GetRoadGrid() == this || roadGridElement.GetRoadGrid() == null)
					{
						roadGridElement.SetRoadGrid(adjacentRoadGridElements[i].GetRoadGrid());
					}
					else
					{
						adjacentRoadGridElements[i].GetRoadGrid().Merge(roadGridElement.GetRoadGrid());
					}
				}

				if (roadGridElement.GetRoadGrid() == this || roadGridElement.GetRoadGrid() == null)
				{
					roadGridElement.SetRoadGrid(new());
				}
			}

			SimEngine.Instance.RoadGridManager.RemoveRoadGrid(this);
		}

		public void Merge(RoadGrid roadGrid)
		{
			if (this == roadGrid) return;

			while (roadGrid._roadGridElements.Count > 0)
			{
				roadGrid._roadGridElements[0].SetRoadGrid(this);
			}

			SimEngine.Instance.RoadGridManager.RemoveRoadGrid(roadGrid);
		}
	}
}