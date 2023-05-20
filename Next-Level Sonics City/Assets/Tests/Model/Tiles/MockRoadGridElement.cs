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

		public override float GetTransparency()
		{
			return 1;
		}

		public List<IRoadGridElement> ConnectsTo()
		{
			throw new NotImplementedException();
		}

		public RoadGrid GetRoadGrid()
		{
			return _roadGrid;
		}

		public Tile GetTile()
		{
			return this;
		}

		public void RegisterRoadGridElement()
		{
			RoadGridManager.Instance.AddRoadGridElement(this);
		}

		public void SetRoadGrid(RoadGrid roadGrid)
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

		public void UnregisterRoadGridElement()
		{
			_roadGrid.RemoveRoadGridElement(this);
		}
	}
}
