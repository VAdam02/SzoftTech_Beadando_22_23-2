using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGridManager
{
	private readonly List<RoadGrid> _roadGrids = new();
	public List<RoadGrid> RoadGrids { get { return _roadGrids; } }
	public void AddRoadGrid(RoadGrid roadGrid)
	{
		lock (_roadGrids)
		{
			_roadGrids.Add(roadGrid);
		}
	}
	public void RemoveRoadGrid(RoadGrid roadGrid)
	{
		lock (_roadGrids)
		{
			_roadGrids.Remove(roadGrid);
		}
	}
}
