using Model.Persons;
using Model.RoadGrids;
using Model.Tiles.Buildings.BuildingCommands;
using NUnit.Framework;
using System.Collections.Generic;

namespace Model.Tiles.Buildings
{
	internal class IndustrialTest
	{
		[SetUp]
		public void SetUp()
		{
			City.Reset();
		}

		[Test]
		public void SetTile_SetsWorkplaceLimit()
		{
			IRoadGridElement roadGridElement = new MockRoadGridElement(0, 0);
			City.Instance.SetTile(roadGridElement.GetTile());

			Industrial industrial = new(0, 1, 123);
			City.Instance.SetTile(industrial);

			Assert.AreEqual(10, industrial.WorkplaceLimit);
		}

		[Test]
		public void RegisterWorkplace_AddsWorkplaceToRoadGrid()
		{
			IRoadGridElement roadGridElement = new MockRoadGridElement(0, 0);
			City.Instance.SetTile(roadGridElement.GetTile());

			Industrial industrial = new(0, 1, 123);
			City.Instance.SetTile(industrial);

			industrial.RegisterWorkplace(roadGridElement.GetRoadGrid());

			CollectionAssert.Contains(roadGridElement.GetRoadGrid().Workplaces, industrial);
		}

		[Test]
		public void UnregisterWorkplace_RemovesWorkplaceFromRoadGrid()
		{
			IRoadGridElement roadGridElement = new MockRoadGridElement(0, 0);
			City.Instance.SetTile(roadGridElement.GetTile());

			Industrial industrial = new(0, 1, 123);
			City.Instance.SetTile(industrial);

			DestroyCommand destroy = new((int)industrial.Coordinates.x, (int)industrial.Coordinates.y);
			destroy.Execute();

			CollectionAssert.DoesNotContain(roadGridElement.GetRoadGrid().Workplaces, industrial);
		}

		[Test]
		public void GetZoneType_ReturnsIndustrialZone()
		{
			IRoadGridElement roadGridElement = new MockRoadGridElement(0, 0);
			City.Instance.SetTile(roadGridElement.GetTile());

			Industrial industrial = new(0, 1, 123);
			City.Instance.SetTile(industrial);

			ZoneType zoneType = industrial.GetZoneType();

			Assert.AreEqual(ZoneType.IndustrialZone, zoneType);
		}

		[Test]
		public void LevelUp_IncreasesLevelAndWorkplaceLimit()
		{
			IRoadGridElement roadGridElement = new MockRoadGridElement(0, 0);
			City.Instance.SetTile(roadGridElement.GetTile());

			Industrial industrial = new(1, 0, 2);
			City.Instance.SetTile(industrial);

			MockResidentialBuildingTile residential = new(0, 1, Rotation.Zero);
			City.Instance.SetTile(residential);

			Assert.AreEqual(ZoneBuildingLevel.ZERO, industrial.Level);
			int previousWorkplaceLimit = industrial.WorkplaceLimit;

			_ = new Worker(residential, industrial, 40, Qualification.LOW);

			Assert.AreEqual(ZoneBuildingLevel.ONE, industrial.Level);
			Assert.AreEqual(previousWorkplaceLimit, industrial.WorkplaceLimit);
			previousWorkplaceLimit = industrial.WorkplaceLimit;

			industrial.LevelUp();

			Assert.AreEqual(ZoneBuildingLevel.TWO, industrial.Level);
			Assert.Less(previousWorkplaceLimit, industrial.WorkplaceLimit);
			previousWorkplaceLimit = industrial.WorkplaceLimit;

			industrial.LevelUp();

			Assert.AreEqual(ZoneBuildingLevel.THREE, industrial.Level);
			Assert.Less(previousWorkplaceLimit, industrial.WorkplaceLimit);
			previousWorkplaceLimit = industrial.WorkplaceLimit;

			industrial.LevelUp();

			Assert.AreEqual(ZoneBuildingLevel.THREE, industrial.Level);
			Assert.AreEqual(previousWorkplaceLimit, industrial.WorkplaceLimit);
		}

		[Test]
		public void Employ_AddsWorkerToWorkersList()
		{
			IRoadGridElement roadGridElement = new MockRoadGridElement(0, 0);
			City.Instance.SetTile(roadGridElement.GetTile());

			Industrial industrial = new(0, 1, 123);
			City.Instance.SetTile(industrial);

			MockResidentialBuildingTile residential = new(1, 0, Rotation.Zero);
			City.Instance.SetTile(residential);

			Worker worker = new(residential, industrial, 40, Qualification.LOW);

			List<Worker> workers = industrial.GetWorkers();

			Assert.AreEqual(1, workers.Count);
			CollectionAssert.Contains(workers, worker);
		}

		[Test]
		public void Unemploy_RemovesWorkerFromWorkersList()
		{
			IRoadGridElement roadGridElement = new MockRoadGridElement(0, 0);
			City.Instance.SetTile(roadGridElement.GetTile());

			MockResidentialBuildingTile residential = new(1, 0, Rotation.Zero);
			City.Instance.SetTile(residential);

			Industrial industrial = new(0, 1, 123);
			City.Instance.SetTile(industrial);

			Worker worker = new(residential, industrial, 40, Qualification.LOW);

			industrial.Unemploy(worker);

			List<Worker> workers = industrial.GetWorkers();

			Assert.AreEqual(0, workers.Count);
			CollectionAssert.DoesNotContain(workers, worker);
		}

		[Test]
		public void GetWorkers_ReturnsWorkersList()
		{
			IRoadGridElement roadGridElement = new MockRoadGridElement(0, 0);
			City.Instance.SetTile(roadGridElement.GetTile());

			MockResidentialBuildingTile residential = new(1, 0, Rotation.Zero);
			City.Instance.SetTile(residential);

			Industrial industrial = new(0, 1, 123);
			City.Instance.SetTile(industrial);

			Worker worker1 = new(residential, industrial, 40, Qualification.LOW);
			Worker worker2 = new(residential, industrial, 50, Qualification.HIGH);

			List<Worker> workers = industrial.GetWorkers();

			Assert.AreEqual(2, workers.Count);
			CollectionAssert.Contains(workers, worker1);
			CollectionAssert.Contains(workers, worker2);
		}

		[Test]
		public void GetWorkersCount_ReturnsWorkersCount()
		{
			IRoadGridElement roadGridElement = new MockRoadGridElement(0, 0);
			City.Instance.SetTile(roadGridElement.GetTile());

			MockResidentialBuildingTile residential = new(1, 0, Rotation.Zero);
			City.Instance.SetTile(residential);

			Industrial industrial = new(0, 1, 123);
			City.Instance.SetTile(industrial);

			_ = new Worker(residential, industrial, 40, Qualification.LOW);
			_ = new Worker(residential, industrial, 50, Qualification.HIGH);

			int workersCount = industrial.GetWorkersCount();

			Assert.AreEqual(2, workersCount);
		}

		[Test]
		public void GetTile_ReturnsItself()
		{
			IRoadGridElement roadGridElement = new MockRoadGridElement(0, 0);
			City.Instance.SetTile(roadGridElement.GetTile());

			Industrial industrial = new(0, 1, 123);

			Tile tile = industrial.GetTile();

			Assert.AreSame(industrial, tile);
		}
	}
}
