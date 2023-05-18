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

			mockRoadGridElement = new MockRoadGridElement(0, 0);
			City.Instance.SetTile(mockRoadGridElement.GetTile());

			residential = new ResidentialBuildingTile(0, 1, 0);
			City.Instance.SetTile(residential.GetTile());
		}

		[Test]
		public void RegisterResidential_AddsResidentialToRoadGrid()
		{
			CollectionAssert.Contains(mockRoadGridElement.GetRoadGrid().Residentials, residential);
		}

		[Test]
		public void UnregisterResidential_RemovesResidentialFromRoadGrid()
		{
			DestroyCommand destroy = new((int)residential.GetTile().Coordinates.x, (int)residential.GetTile().Coordinates.y);
			destroy.Execute();

			CollectionAssert.DoesNotContain(mockRoadGridElement.GetRoadGrid().Residentials, residential);
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
			Assert.AreEqual(previousWorkplaceLimit, residential.ResidentLimit);
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
			
			residential.MoveOut(person); //TODO illegal way to move out

			CollectionAssert.DoesNotContain(residential.GetResidents(), person);
		}
	}
}
