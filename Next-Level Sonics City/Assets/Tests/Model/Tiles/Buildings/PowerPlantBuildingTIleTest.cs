using Model.Persons;
using Model.RoadGrids;
using Model.Tiles.Buildings.BuildingCommands;
using NUnit.Framework;

namespace Model.Tiles.Buildings
{
	public class PowerPlantBuildingTileTest
	{
		private PowerPlant powerPlant;
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
			powerPlant = new PowerPlant(0, 1, 123, Rotation.Zero);
			City.Instance.SetTile(powerPlant.GetTile());
			residential = new ResidentialBuildingTile(1, 0, 456);
			City.Instance.SetTile(residential.GetTile());
		}

		[Test]
		public void GetTileType_ReturnsPowerPlant()
		{
			var tileType = powerPlant.GetTileType();

			Assert.AreEqual(TileType.PowerPlant, tileType);
		}

		[Test]
		public void RegisterWorkplace_AddsPowerPlantToRoadGrid()
		{
			Assert.Contains(powerPlant, roadGridElement.RoadGrid.Workplaces);
		}

		[Test]
		public void UnregisterWorkplace_RemovesPowerPlantFromRoadGrid()
		{
			DestroyCommand destroy = new((int)powerPlant.Coordinates.x, (int)powerPlant.Coordinates.y);
			destroy.Execute();

			CollectionAssert.DoesNotContain(roadGridElement.RoadGrid.Workplaces, powerPlant);
		}

		[Test]
		public void Employ_AddsWorkerToPowerPlant()
		{
			var worker = new Worker(residential, powerPlant, 25, Qualification.HIGH);

			Assert.Contains(worker, ((IWorkplace)powerPlant).GetWorkers());
		}

		[Test]
		public void Unemploy_RemovesWorkerFromPowerPlant()
		{
			var worker = new Worker(residential, powerPlant, 25, Qualification.HIGH);
			worker.Die();

			CollectionAssert.DoesNotContain(((IWorkplace)powerPlant).GetWorkers(), worker);
		}

		[Test]
		public void GetWorkers_ReturnsListOfWorkersInPowerPlant()
		{
			var worker1 = new Worker(residential, powerPlant, 25, Qualification.HIGH);
			var worker2 = new Worker(residential, powerPlant, 25, Qualification.HIGH);

			var workers = ((IWorkplace)powerPlant).GetWorkers();

			Assert.Contains(worker1, workers);
			Assert.Contains(worker2, workers);
		}

		[Test]
		public void GetWorkersCount_ReturnsNumberOfWorkersInPowerPlant()
		{
			_ = new Worker(residential, powerPlant, 25, Qualification.HIGH);
			_ = new Worker(residential, powerPlant, 25, Qualification.HIGH);

			var count = ((IWorkplace)powerPlant).GetWorkersCount();

			Assert.AreEqual(2, count);
		}

		[Test]
		public void GetTile_ReturnsSelf()
		{
			var tile = powerPlant.GetTile();

			Assert.AreEqual(powerPlant, tile);
		}
	}
}
