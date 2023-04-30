using Model.Simulation;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Model.Tiles.Buildings
{
	public class PowerPlant : Building, IWorkplace
	{
		private readonly List<Person> _workers = new();
		private int _workersLimit = 10;

		public PowerPlant(int x, int y, uint designID) : base(x, y, designID)
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

		public override int GetBuildPrice() //TODO implementik logic
		{
			return BUILD_PRICE;
		}

		public override int GetDestroyPrice()
		{
			return DESTROY_PRICE;
		}

		public override int GetMaintainanceCost()
		{
			return GetBuildPrice() / 10;
		}

		public override bool IsExpandable()
		{
			return true;
		}

		public override bool CanExpand(Rotation rotation)
		{
			int x1 = (int)Coordinates.x;
			int y1 = (int)Coordinates.y;
			int x2 = (int)Coordinates.x;
			int y2 = (int)Coordinates.y;


			switch (rotation)
			{
				case Rotation.Zero:
					x1 += 1; y1 += 1;
					break;
				case Rotation.Ninety:
					x1 += -1; y1 += 1;
					break;
				case Rotation.OneEighty:
					x1 += -1; y1 += 1;
					break;
				case Rotation.TwoSeventy:
					x1 += 1; y1 += -1;
					break;
			}

			int minX = Math.Min(x1, x2);
			int maxX = Math.Max(x1, x2);
			int minY = Math.Min(y1, y2);
			int maxY = Math.Max(y1, y2);

			int lowerLimit = 0;
			int upperLimit = SimEngine.Instance.GetSize();

			if (minX < lowerLimit || minY < lowerLimit || maxX > upperLimit || maxY > upperLimit)
			{
				return false;
			}

			for (int i = minX; minX < maxX; ++i)
			{
				for (int j = minY; minY < maxY; ++j)
				{
					if (SimEngine.Instance.GetTile(i, j) is not EmptyTile)
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}