using Model;
using Model.Tiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighSchool : Building, IWorkplace
{
	private readonly List<Person> _workers = new();
	private int _workersLimit = 10;

	public HighSchool(int x, int y, uint designID) : base(x, y, designID)
	{

	}

	public bool Employ(Person person)
	{
		if (_workers.Count < _workersLimit)
		{
			_workers.Add(person);
			return true;
		}

		return false;
	}

	public bool Unemploy(Person person)
	{
		if (_workers.Count > 0)
		{
			_workers.Remove(person);
			return true;
		}

		return false;
	}

	public List<Person> GetWorkers()
	{
		return _workers;
	}

	public int GetWorkersCount()
	{
		return _workers.Count;
	}

	public int GetWorkersLimit()
	{
		return _workersLimit;
	}
}