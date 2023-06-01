using Model.Persons;
using Model.RoadGrids;
using Model.Tiles.Buildings.BuildingCommands;
using NUnit.Framework;

namespace Model.Tiles.Buildings
{
	public class ResidentialBuildingTileTest
	{
		private IResidential residential;
		private IRoadGridElement mockRoadGridElement;

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

			mockRoadGridElement = new RoadTile(0, 0);
			City.Instance.SetTile(mockRoadGridElement.GetTile());

			residential = new ResidentialBuildingTile(0, 1, 0);
			City.Instance.SetTile(residential.GetTile());
		}

		[Test]
		public void SetTile_SetsWorkplaceLimit()
		{
			IRoadGridElement roadGridElement = new RoadTile(0, 0);
			City.Instance.SetTile(roadGridElement.GetTile());

			ResidentialBuildingTile residential = new(0, 1, 123);
			City.Instance.SetTile(residential);

			Assert.AreEqual(1, residential.ResidentLimit);
		}

		[Test]
		public void RegisterResidential_AddsResidentialToRoadGrid()
		{
			CollectionAssert.Contains(mockRoadGridElement.RoadGrid.Residentials, residential);
		}

		[Test]
		public void UnregisterResidential_RemovesResidentialFromRoadGrid()
		{
			DestroyCommand destroy = new((int)residential.GetTile().Coordinates.x, (int)residential.GetTile().Coordinates.y);
			destroy.Execute();

			CollectionAssert.DoesNotContain(mockRoadGridElement.RoadGrid.Residentials, residential);
		}

		[Test]
		public void GetZoneType_ReturnsResidentialZone()
		{
			var zoneType = ((IZoneBuilding)residential).GetZoneType();

			Assert.AreEqual(ZoneType.ResidentialZone, zoneType);
		}

		[Test]
		public void LevelUp_IncreasesLevelAndResidentLimit()
		{
			((IZoneBuilding)residential).GetZoneType();

			Assert.AreEqual(ZoneBuildingLevel.ZERO, ((IZoneBuilding)residential).Level);
			int previousWorkplaceLimit = residential.ResidentLimit;

			_ = new Pensioner(residential, 70, 50);

			Assert.AreEqual(ZoneBuildingLevel.ONE, ((IZoneBuilding)residential).Level);
			Assert.Less(previousWorkplaceLimit, residential.ResidentLimit);
			previousWorkplaceLimit = residential.ResidentLimit;

			((IZoneBuilding)residential).LevelUp();

			Assert.AreEqual(ZoneBuildingLevel.TWO, ((IZoneBuilding)residential).Level);
			Assert.Less(previousWorkplaceLimit, residential.ResidentLimit);
			previousWorkplaceLimit = residential.ResidentLimit;

			((IZoneBuilding)residential).LevelUp();

			Assert.AreEqual(ZoneBuildingLevel.THREE, ((IZoneBuilding)residential).Level);
			Assert.Less(previousWorkplaceLimit, residential.ResidentLimit);
			previousWorkplaceLimit = residential.ResidentLimit;

			((IZoneBuilding)residential).LevelUp();

			Assert.AreEqual(ZoneBuildingLevel.THREE, ((IZoneBuilding)residential).Level);
			Assert.AreEqual(previousWorkplaceLimit, residential.ResidentLimit);
		}

		[Test]
		public void MoveIn_AddsPersonToResidents()
		{
			var person = new Pensioner(residential, 80, 30);

			CollectionAssert.Contains(residential.GetResidents(), person);
		}

		[Test]
		public void MoveOut_RemovesPersonFromResidents()
		{
			var person = new Pensioner(residential, 80, 30);
			person.Die();

			CollectionAssert.DoesNotContain(residential.GetResidents(), person);
		}
	}
}
