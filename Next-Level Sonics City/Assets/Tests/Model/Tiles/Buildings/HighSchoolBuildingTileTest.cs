using Model.Persons;
using Model.RoadGrids;
using NUnit.Framework;
using System;

namespace Model.Tiles.Buildings
{
	public class HighSchoolBuildingTileTest
	{
		private HighSchool highSchool;
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
			highSchool = new HighSchool(0, 1, 123, Rotation.Zero);
			City.Instance.SetTile(highSchool.GetTile());
			residential = new ResidentialBuildingTile(1, 0, 456);
			City.Instance.SetTile(residential.GetTile());
		}

		[Test]
		public void GetTileType_ReturnsHighSchool()
		{
			var tileType = highSchool.GetTileType();

			Assert.AreEqual(TileType.HighSchool, tileType);
		}

		[Test]
		public void Employ_AddsWorkerToHighSchool()
		{
			var worker = new Worker(residential, highSchool, 25, Qualification.HIGH);

			Assert.Contains(worker, ((IWorkplace)highSchool).GetWorkers());
		}

		[Test]
		public void Unemploy_RemovesWorkerFromHighSchool()
		{
			var worker = new Worker(residential, highSchool, 25, Qualification.HIGH);
			worker.Die();

			CollectionAssert.DoesNotContain(((IWorkplace)highSchool).GetWorkers(), worker);
		}

		[Test]
		public void GetWorkers_ReturnsListOfWorkersInHighSchool()
		{
			var worker1 = new Worker(residential, highSchool, 25, Qualification.HIGH);
			var worker2 = new Worker(residential, highSchool, 25, Qualification.HIGH);

			var workers = ((IWorkplace)highSchool).GetWorkers();

			Assert.Contains(worker1, workers);
			Assert.Contains(worker2, workers);
		}

		[Test]
		public void GetWorkersCount_ReturnsNumberOfWorkersInHighSchool()
		{
			_ = new Worker(residential, highSchool, 25, Qualification.HIGH);
			_ = new Worker(residential, highSchool, 25, Qualification.HIGH);

			var count = ((IWorkplace)highSchool).GetWorkersCount();

			Assert.AreEqual(2, count);
		}

		[Test]
		public void GetTile_ReturnsSelf()
		{
			var tile = ((IWorkplace)highSchool).GetTile();

			Assert.AreEqual(highSchool, tile);
		}
	}
}