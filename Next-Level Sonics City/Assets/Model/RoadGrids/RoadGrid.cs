using System.Collections.Generic;

namespace Model.RoadGrids
{
	public class RoadGrid
	{
		/// <summary>
		/// It is also register itself in RoadGridManager
		/// </summary>
		public RoadGrid()
		{
			RoadGridManager.Instance.AddRoadGrid(this);
		}

		private readonly List<IRoadGridElement> _roadGridElements = new();
		public List<IRoadGridElement> RoadGridElements { get { return _roadGridElements; } }

		/// <summary>
		/// Add roadGridElement to this
		/// </summary>
		/// <param name="roadGridElement">Element that should be added</param>
		public void AddRoadGridElement(IRoadGridElement roadGridElement)
		{
			_roadGridElements.Add(roadGridElement);
		}

		/// <summary>
		/// Remove roadGridElement from this
		/// </summary>
		/// <param name="roadGridElement">Element that should be removed</param>
		public void RemoveRoadGridElement(IRoadGridElement roadGridElement)
		{
			_roadGridElements.Remove(roadGridElement);
		}

		private readonly List<IWorkplace> _workplaces = new();
		public List<IWorkplace> Workplaces { get { return _workplaces; } }
		public List<IWorkplace> FreeWorkplaces { get { return _workplaces.FindAll(workplace => workplace.WorkplaceLimit > workplace.GetWorkersCount()); } }

		/// <summary>
		/// Add workplace to this
		/// </summary>
		/// <param name="workplace">Workplace that should be added</param>
		public void AddWorkplace(IWorkplace workplace)
		{
			_workplaces.Add(workplace);
		}

		/// <summary>
		/// Remove workplace from this
		/// </summary>
		/// <param name="workplace">Workplace that should be removed</param>
		public void RemoveWorkplace(IWorkplace workplace)
		{
			_workplaces.Remove(workplace);
		}

		private readonly List<IResidential> _residential = new();
		public List<IResidential> Residentials { get { return _residential; } }
		public List<IResidential> FreeResidentials { get { return _residential.FindAll(residential => residential.ResidentLimit > residential.GetResidentsCount()); } }

		/// <summary>
		/// Add residential to this
		/// </summary>
		/// <param name="residential">Residential that should be added</param>
		public void AddResidential(IResidential residential)
		{
			_residential.Add(residential);
		}

		/// <summary>
		/// Remove residential from this
		/// </summary>
		/// <param name="residential">Residential that should be removed</param>
		public void RemoveResidential(IResidential residential)
		{
			_residential.Remove(residential);
		}

		/// <summary>
		/// Reinitialize the road grid and fix errors which may caused by removing road grid elements and the graph splitted
		/// </summary>
		public void Reinit()
		{
			Queue<IRoadGridElement> queue = new(_roadGridElements);

			while (queue.Count > 0)
			{
				IRoadGridElement roadGridElement = queue.Dequeue();

				List<IRoadGridElement> adjacentRoadGridElements = roadGridElement.ConnectsTo;

				for (int i = 0; i < adjacentRoadGridElements.Count; i++)
				{
					if (adjacentRoadGridElements[i] == null) { continue; }

					if (adjacentRoadGridElements[i].RoadGrid == this || adjacentRoadGridElements[i].RoadGrid == null)
					{
						queue.Enqueue(adjacentRoadGridElements[i]);
						continue;
					}

					if (roadGridElement.RoadGrid == this || roadGridElement.RoadGrid == null)
					{
						roadGridElement.RoadGrid = adjacentRoadGridElements[i].RoadGrid;
					}
					else
					{
						adjacentRoadGridElements[i].RoadGrid.Merge(roadGridElement.RoadGrid);
					}
				}

				if (roadGridElement.RoadGrid == this || roadGridElement.RoadGrid == null)
				{
					roadGridElement.RoadGrid = new();
				}
			}

			RoadGridManager.Instance.RemoveRoadGrid(this);
		}

		/// <summary>
		/// Merge roadGrid into this
		/// </summary>
		/// <param name="roadGrid">Road grid that will be merged into this</param>
		public void Merge(RoadGrid roadGrid)
		{
			if (this == roadGrid) return;

			while (roadGrid._roadGridElements.Count > 0)
			{
				roadGrid._roadGridElements[0].RoadGrid = this;
			}

			RoadGridManager.Instance.RemoveRoadGrid(roadGrid);
		}
	}
}