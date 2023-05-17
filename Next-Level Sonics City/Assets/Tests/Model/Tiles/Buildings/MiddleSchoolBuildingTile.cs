using Model.Persons;
using Model.RoadGrids;
using NUnit.Framework;
using System;

namespace Model.Tiles.Buildings
{
	public class MiddleSchoolBuildingTileTest
	{
		private MiddleSchool middleSchool;
		private ResidentialBuildingTile residential;
		private IRoadGridElement roadGridElement;

		[SetUp]
		public void SetUp()
		{
			City.Reset();

			roadGridElement = new MockRoadGridElement(0, 0);
			City.Instance.SetTile(roadGridElement.GetTile());
			middleSchool = new MiddleSchool(0, 1, 123, Rotation.Zero);
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

			Assert.Contains(worker, middleSchool.GetWorkers());
		}

		[Test]
		public void Unemploy_RemovesWorkerFromMiddleSchool()
		{
			var worker = new Worker(residential, middleSchool, 25, Qualification.HIGH);
			middleSchool.Unemploy(worker); //TODO illegal way to move out

			CollectionAssert.DoesNotContain(middleSchool.GetWorkers(), worker);
		}

		[Test]
		public void GetWorkers_ReturnsListOfWorkersInMiddleSchool()
		{
			var worker1 = new Worker(residential, middleSchool, 25, Qualification.HIGH);
			var worker2 = new Worker(residential, middleSchool, 25, Qualification.HIGH);
			middleSchool.Employ(worker1);
			middleSchool.Employ(worker2);

			var workers = middleSchool.GetWorkers();

			Assert.Contains(worker1, workers);
			Assert.Contains(worker2, workers);
		}

		[Test]
		public void GetWorkersCount_ReturnsNumberOfWorkersInMiddleSchool()
		{
			_ = new Worker(residential, middleSchool, 25, Qualification.HIGH);
			_ = new Worker(residential, middleSchool, 25, Qualification.HIGH);

			var count = middleSchool.GetWorkersCount();

			Assert.AreEqual(2, count);
		}

		[Test]
		public void GetTile_ReturnsSelf()
		{
			var tile = middleSchool.GetTile();

			Assert.AreEqual(middleSchool, tile);
		}
	}
}
