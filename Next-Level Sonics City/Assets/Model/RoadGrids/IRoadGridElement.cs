using System.Collections.Generic;

namespace Model.RoadGrids
{
	public interface IRoadGridElement
	{
		public void RegisterRoadGridElement();
		public void UnregisterRoadGridElement();

		public List<IRoadGridElement> ConnectsTo();

		public RoadGrid GetRoadGrid();
		public void SetRoadGrid(RoadGrid roadGrid);

		public Tile GetTile();
	}
}