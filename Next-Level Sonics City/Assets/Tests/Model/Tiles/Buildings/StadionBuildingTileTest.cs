using Model.Persons;
using Model.RoadGrids;
using Model.Tiles.Buildings.BuildingCommands;
using NUnit.Framework;

namespace Model.Tiles.Buildings
{
	public class StadionBuildingTilTest
	{
		private StadionBuildingTile stadion;
		private ResidentialBuildingTile residential;
		private IRoadGridElement roadGridElement;

		[SetUp]
		public void SetUp()
		{
			City.Reset();

			roadGridElement = new RoadTile(0, 0);
			City.Instance.SetTile(roadGridElement.GetTile());
			stadion = new StadionBuildingTile(0, 1, 123, Rotation.Zero);
			City.Instance.SetTile(stadion.GetTile());
			residential = new ResidentialBuildingTile(1, 0, 456);
			City.Instance.SetTile(residential.GetTile());
		}

		[Test]
		public void GetTileType_ReturnsStadionBuildingTile()
		{
			var tileType = stadion.GetTileType();

			Assert.AreEqual(TileType.Stadion, tileType);
		}

		[Test]
		public void RegisterWorkplace_AddsStadionBuildingTileToRoadGrid()
		{
			Assert.Contains(stadion, roadGridElement.RoadGrid.Workplaces);
		}

		[Test]
		public void UnregisterWorkplace_RemovesStadionBuildingTileFromRoadGrid()
		{
			DestroyCommand destroy = new((int)stadion.Coordinates.x, (int)stadion.Coordinates.y);
			destroy.Execute();

			CollectionAssert.DoesNotContain(roadGridElement.RoadGrid.Workplaces, stadion);
		}

		[Test]
		public void Employ_AddsWorkerToStadionBuildingTile()
		{
			var worker = new Worker(residential, stadion, 25, Qualification.HIGH);

			Assert.Contains(worker, stadion.GetWorkers());
		}

		[Test]
		public void Unemploy_RemovesWorkerFromStadionBuildingTile()
		{
			var worker = new Worker(residential, stadion, 25, Qualification.HIGH);
			stadion.Unemploy(worker); //TODO illegal way to move out

			CollectionAssert.DoesNotContain(stadion.GetWorkers(), worker);
		}

		[Test]
		public void GetWorkers_ReturnsListOfWorkersInStadionBuildingTile()
		{
			var worker1 = new Worker(residential, stadion, 25, Qualification.HIGH);
			var worker2 = new Worker(residential, stadion, 25, Qualification.HIGH);

			var workers = stadion.GetWorkers();

			Assert.Contains(worker1, workers);
			Assert.Contains(worker2, workers);
		}

		[Test]
		public void GetWorkersCount_ReturnsNumberOfWorkersInStadionBuildingTile()
		{
			_ = new Worker(residential, stadion, 25, Qualification.HIGH);
			_ = new Worker(residential, stadion, 25, Qualification.HIGH);

			var count = stadion.GetWorkersCount();

			Assert.AreEqual(2, count);
		}

		[Test]
		public void GetTile_ReturnsSelf()
		{
			var tile = stadion.GetTile();

			Assert.AreEqual(stadion, tile);
		}
	}
}
