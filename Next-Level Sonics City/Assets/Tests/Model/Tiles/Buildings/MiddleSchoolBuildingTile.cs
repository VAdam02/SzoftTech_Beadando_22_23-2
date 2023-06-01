using Model.Persons;
using Model.RoadGrids;
using NUnit.Framework;
using System;

namespace Model.Tiles.Buildings
{
	public class MiddleSchoolBuildingTileTest
	{
		private MiddleSchoolBuildingTile middleSchool;
		private ResidentialBuildingTile residential;
		private IRoadGridElement roadGridElement;

		[SetUp]
		public void SetUp()
		{
			City.Reset();
			for (int i = 0; i < City.Instance.GetSize(); i++)
			{
				for (int j = 0; j < City.Instance.GetSize(); j++)
				{
					City.Instance.SetTile(new EmptyTile(i, j));
				}
			}

			roadGridElement = new RoadTile(0, 0);
			City.Instance.SetTile(roadGridElement.GetTile());
			middleSchool = new MiddleSchoolBuildingTile(0, 1, 123, Rotation.Zero);
			City.Instance.SetTile(middleSchool.GetTile());
			residential = new ResidentialBuildingTile(1, 0, 456);
			City.Instance.SetTile(residential.GetTile());
		}

		[Test]
		public void GetTileType_ReturnsMiddleSchool()
		{
			var tileType = middleSchool.GetTileType();

			Assert.AreEqual(TileType.MiddleSchool, tileType);
		}

		[Test]
		public void Employ_AddsWorkerToMiddleSchool()
		{
			var worker = new Worker(residential, middleSchool, 25, Qualification.HIGH);

			Assert.Contains(worker, ((IWorkplace)middleSchool).GetWorkers());
		}

		[Test]
		public void Unemploy_RemovesWorkerFromMiddleSchool()
		{
			var worker = new Worker(residential, middleSchool, 25, Qualification.HIGH);
			((IWorkplace)middleSchool).Unemploy(worker); //TODO illegal way to move out

			CollectionAssert.DoesNotContain(((IWorkplace)middleSchool).GetWorkers(), worker);
		}

		[Test]
		public void GetWorkers_ReturnsListOfWorkersInMiddleSchool()
		{
			var worker1 = new Worker(residential, middleSchool, 25, Qualification.HIGH);
			var worker2 = new Worker(residential, middleSchool, 25, Qualification.HIGH);
			((IWorkplace)middleSchool).Employ(worker1);
			((IWorkplace)middleSchool).Employ(worker2);

			var workers = ((IWorkplace)middleSchool).GetWorkers();

			Assert.Contains(worker1, workers);
			Assert.Contains(worker2, workers);
		}

		[Test]
		public void GetWorkersCount_ReturnsNumberOfWorkersInMiddleSchool()
		{
			_ = new Worker(residential, middleSchool, 25, Qualification.HIGH);
			_ = new Worker(residential, middleSchool, 25, Qualification.HIGH);

			var count = ((IWorkplace)middleSchool)			.GetWorkersCount();

			Assert.AreEqual(2, count);
		}

		[Test]
		public void GetTile_ReturnsSelf()
		{
			var tile = ((IWorkplace)middleSchool).GetTile();

			Assert.AreEqual(middleSchool, tile);
		}
	}
}
