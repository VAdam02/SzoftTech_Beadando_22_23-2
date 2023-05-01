using Model;
using Model.Simulation;
using Model.Tiles.Buildings;
using System;
using System.Collections.Generic;

public class RoadGrid
{
	public RoadGrid()
	{
		SimEngine.Instance.RoadGridManager.AddRoadGrid(this);
	}

	private readonly List<IRoadGridElement> _roadGridElements = new();
	public void AddRoadGridElement(IRoadGridElement roadGridElement)
	{
		lock (_roadGridElements)
		{
			_roadGridElements.Add(roadGridElement);
		}
	}
	public void RemoveRoadGridElement(IRoadGridElement roadGridElement)
	{
		lock (_roadGridElements)
		{
			_roadGridElements.Remove(roadGridElement);
		}
	}

	private readonly List<IWorkplace> _workplaces = new();
	public void AddWorkplace(IWorkplace workplace)
	{
		lock (_workplaces)
		{
			_workplaces.Add(workplace);
		}
	}
	public void RemoveWorkplace(IWorkplace workplace)
	{
		lock (_workplaces)
		{
			_workplaces.Remove(workplace);
		}
	}

	private readonly List<ResidentialBuildingTile> _homes = new();
	public void AddHome(ResidentialBuildingTile home)
	{
		lock (_homes)
		{
			_homes.Add(home);
		}
	}
	public void RemoveHome(ResidentialBuildingTile home)
	{
		lock (_homes)
		{
			_homes.Remove(home);
		}
	}

	public void Merge(RoadGrid roadGrid)
	{
		foreach (IRoadGridElement element in roadGrid._roadGridElements) { element.SetRoadGrid(this); }

		foreach (IWorkplace workplace in roadGrid._workplaces) { AddWorkplace(workplace); }

		foreach (ResidentialBuildingTile home in roadGrid._homes) { AddHome(home); }

		SimEngine.Instance.RoadGridManager.RemoveRoadGrid(roadGrid);
	}
}
