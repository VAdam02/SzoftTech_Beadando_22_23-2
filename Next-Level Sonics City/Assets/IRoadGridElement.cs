using Model;
using System.Collections.Generic;

public interface IRoadGridElement
{
	public void RegisterRoadGridElement();
	public void UnregisterRoadGridElement();

	public List<IRoadGridElement> ConnectsTo();

	public RoadGrid GetRoadGrid();
	public void SetRoadGrid(RoadGrid roadGrid);

	public void SetParent(IRoadGridElement parent, int depth);
	public IRoadGridElement GetParent();

	public int GetDepth();
	internal int GetDepthUnoptimized();

	public Tile GetTile();
}
