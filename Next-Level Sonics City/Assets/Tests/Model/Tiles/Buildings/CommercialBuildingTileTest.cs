using Model.Persons;
using Model.RoadGrids;
using Model.Tiles.Buildings.BuildingCommands;
using NUnit.Framework;

namespace Model.Tiles.Buildings
{
	internal class CommercialBuildingTileTest
	{
		public class CommercialTests
		{
			private Commercial commercial;
			private IRoadGridElement roadGridElement;
			private MockResidentialBuildingTile mockResidential;

			[SetUp]
			public void SetUp()
			{
				City.Reset();

				roadGridElement = new RoadTile(0, 0);
				City.Instance.SetTile(roadGridElement.GetTile());

				commercial = new Commercial(0, 1, 123);
				City.Instance.SetTile(commercial);
				mockResidential = new MockResidentialBuildingTile(1, 0, Rotation.TwoSeventy);
				City.Instance.SetTile(mockResidential);
			}

			[Test]
			public void RegisterWorkplace_AddsWorkplaceToRoadGrid()
			{
				commercial.RegisterWorkplace(roadGridElement.RoadGrid);

				CollectionAssert.Contains(roadGridElement.RoadGrid.Workplaces, commercial);
			}

			[Test]
			public void UnregisterWorkplace_RemovesWorkplaceFromRoadGrid()
			{
				DestroyCommand destroy = new((int)commercial.Coordinates.x, (int)commercial.Coordinates.y);
				destroy.Execute();

				CollectionAssert.DoesNotContain(roadGridElement.RoadGrid.Workplaces, commercial);
			}

			[Test]
			public void GetZoneType_ReturnsCommercialZone()
			{
				var zoneType = commercial.GetZoneType();

				Assert.AreEqual(ZoneType.CommercialZone, zoneType);
			}

			[Test]
			public void LevelUp_IncreasesLevelAndWorkplaceLimit()
			{
				commercial.LevelUp();

				Assert.AreEqual(ZoneBuildingLevel.ZERO, commercial.Level);
				int previousWorkplaceLimit = commercial.WorkplaceLimit;

				_ = new Worker(mockResidential, commercial, 40, Qualification.LOW);

				Assert.AreEqual(ZoneBuildingLevel.ONE, commercial.Level);
				Assert.AreEqual(previousWorkplaceLimit, commercial.WorkplaceLimit);
				previousWorkplaceLimit = commercial.WorkplaceLimit;

				commercial.LevelUp();

				Assert.AreEqual(ZoneBuildingLevel.TWO, commercial.Level);
				Assert.Less(previousWorkplaceLimit, commercial.WorkplaceLimit);
				previousWorkplaceLimit = commercial.WorkplaceLimit;

				commercial.LevelUp();

				Assert.AreEqual(ZoneBuildingLevel.THREE, commercial.Level);
				Assert.Less(previousWorkplaceLimit, commercial.WorkplaceLimit);
				previousWorkplaceLimit = commercial.WorkplaceLimit;

				commercial.LevelUp();

				Assert.AreEqual(ZoneBuildingLevel.THREE, commercial.Level);
				Assert.AreEqual(previousWorkplaceLimit, commercial.WorkplaceLimit);
			}
		}
	}
}
