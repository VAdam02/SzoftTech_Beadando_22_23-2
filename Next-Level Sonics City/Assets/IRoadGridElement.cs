using Model;
using System.Collections.Generic;

public interface IRoadGridElement
{
	public List<IRoadGridElement> ConnectsTo();

	public RoadGrid GetRoadGrid();
	public void SetRoadGrid(RoadGrid roadGrid);

	public Tile GetTile();

	public void SetParent(IRoadGridElement parent);
	public IRoadGridElement GetParent();
}
