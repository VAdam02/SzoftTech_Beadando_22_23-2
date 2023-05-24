using Model.Persons;
using Model.RoadGrids;
using NUnit.Framework;
using System;

namespace Model.Tiles.Buildings
{
	public class WorkplaceSubTileTest
	{
		private WorkplaceSubTile workplaceSubTile;
		private IRoadGridElement roadGridElement;
		private IWorkplace baseWorkplace;
		private ResidentialBuildingTile residential;

		[SetUp]
		public void SetUp()
		{
			City.Reset();

			roadGridElement = new RoadTile(0, 0);
			City.Instance.SetTile(roadGridElement.GetTile());

			baseWorkplace = new StadionBuildingTile(0, 1, 123, Rotation.Zero);
			City.Instance.SetTile(baseWorkplace.GetTile());

			workplaceSubTile = new WorkplaceSubTile(1, 1, 456, baseWorkplace);
			City.Instance.SetTile(workplaceSubTile);
			workplaceSubTile = new WorkplaceSubTile(0, 2, 456, baseWorkplace);
			City.Instance.SetTile(workplaceSubTile);
			workplaceSubTile = new WorkplaceSubTile(1, 2, 456, baseWorkplace);
			City.Instance.SetTile(workplaceSubTile);

			residential = new ResidentialBuildingTile(1, 0, 789);
			City.Instance.SetTile(residential.GetTile());
		}

		[Test]
		public void GetTileType_ReturnsBaseWorkplaceTileType()
		{
			var tileType = workplaceSubTile.GetTileType();

			Assert.AreEqual(baseWorkplace.GetTile().GetTileType(), tileType);
		}

		[Test]
		public void Employ_AddsWorkerToBaseWorkplace()
		{
			var worker = new Worker(residential, workplaceSubTile, 25, Qualification.HIGH);

			Assert.Contains(worker, baseWorkplace.GetWorkers());
		}

		[Test]
		public void Unemploy_RemovesWorkerFromBaseWorkplace()
		{
			var worker = new Worker(residential, workplaceSubTile, 25, Qualification.HIGH);
			((IWorkplace)workplaceSubTile).Unemploy(worker); //TODO illegal way to move out

			CollectionAssert.DoesNotContain(baseWorkplace.GetWorkers(), worker);
		}

		[Test]
		public void GetWorkers_ReturnsWorkersFromBaseWorkplace()
		{
			var worker1 = new Worker(residential, workplaceSubTile, 25, Qualification.HIGH);
			var worker2 = new Worker(residential, workplaceSubTile, 25, Qualification.HIGH);

			var workers = ((IWorkplace)workplaceSubTile).GetWorkers();

			Assert.Contains(worker1, workers);
			Assert.Contains(worker2, workers);
		}

		[Test]
		public void GetWorkersCount_ReturnsWorkersCountFromBaseWorkplace()
		{
			_ = new Worker(residential, workplaceSubTile, 25, Qualification.HIGH);
			_ = new Worker(residential, workplaceSubTile, 25, Qualification.HIGH);

			var count = ((IWorkplace)workplaceSubTile).GetWorkersCount();

			Assert.AreEqual(2, count);
		}

		[Test]
		public void CanBuild_ThrowsException()
		{
			Assert.Throws<InvalidOperationException>(() => workplaceSubTile.CanBuild());
		}

		[Test]
		public void Expand_ThrowsException()
		{
			Assert.Throws<InvalidOperationException>(() => workplaceSubTile.Expand());
		}
	}
}
