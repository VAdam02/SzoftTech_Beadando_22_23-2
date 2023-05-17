using Model.RoadGrids;
using System;
using System.Collections.Generic;

namespace Model.Tiles
{
	internal class MockRoadGridElement : Tile, IRoadGridElement
	{
		private RoadGrid _roadGrid = null;

		public MockRoadGridElement(int x, int y) : base(x, y, 0)
		{

		}

		public override int GetBuildPrice()
		{
			return 100;
		}

		public override int GetDestroyIncome()
		{
			return 100;
		}

		public override TileType GetTileType()
		{
			throw new NotImplementedException();
		}

		List<IRoadGridElement> IRoadGridElement.ConnectsTo()
		{
			throw new NotImplementedException();
		}

		RoadGrid IRoadGridElement.GetRoadGrid()
		{
			return _roadGrid;
		}

		Tile IRoadGridElement.GetTile()
		{
			return this;
		}

		void IRoadGridElement.RegisterRoadGridElement()
		{
			RoadGridManager.Instance.AddRoadGridElement(this);
		}

		void IRoadGridElement.SetRoadGrid(RoadGrid roadGrid)
		{
			if (_roadGrid == roadGrid) { return; }

			List<Building> buildings = RoadGridManager.GetBuildingsByRoadGridElement(this);
			foreach (Building building in buildings)
			{
				if (building is IWorkplace workplace)
				{
					workplace.UnregisterWorkplace(_roadGrid);
				}
				if (building is IResidential residential)
				{
					residential.UnregisterResidential(_roadGrid);
				}
			}

			if (roadGrid == null)
			{
				_roadGrid?.RemoveRoadGridElement(this);
				_roadGrid?.Reinit();
			}
			else
			{
				_roadGrid?.RemoveRoadGridElement(this);
				_roadGrid = roadGrid;
				_roadGrid?.AddRoadGridElement(this);
			}

			if (_roadGrid != null)
			{
				foreach (Building building in buildings)
				{
					if (building is IWorkplace workplace)
					{
						workplace.RegisterWorkplace(_roadGrid);
					}
					if (building is IResidential residential)
					{
						residential.RegisterResidential(_roadGrid);
					}
				}
			}
		}

		void IRoadGridElement.UnregisterRoadGridElement()
		{
			_roadGrid.RemoveRoadGridElement(this);
		}
	}
}
