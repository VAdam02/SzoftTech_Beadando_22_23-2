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
			private CommercialBuildingTIle commercial;
			private IRoadGridElement roadGridElement;
			private ResidentialBuildingTile mockResidential;

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

				commercial = new CommercialBuildingTIle(0, 1, 123);
				City.Instance.SetTile(commercial);
				mockResidential = new ResidentialBuildingTile(1, 0, 0, Rotation.TwoSeventy, ZoneBuildingLevel.ZERO);
				City.Instance.SetTile(mockResidential);
			}

			[Test]
			public void SetTile_SetsWorkplaceLimit()
			{
				IRoadGridElement roadGridElement = new RoadTile(0, 0);
				City.Instance.SetTile(roadGridElement.GetTile());

				IndustrialBuildingTile industrial = new(0, 1, 123);
				City.Instance.SetTile(industrial);

				Assert.AreEqual(1, industrial.WorkplaceLimit);
			}

			[Test]
			public void RegisterWorkplace_AddsWorkplaceToRoadGrid()
			{
				((IWorkplace)commercial).RegisterWorkplace(roadGridElement.RoadGrid);

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
				var zoneType = ((IZoneBuilding)commercial).GetZoneType();

				Assert.AreEqual(ZoneType.CommercialZone, zoneType);
			}

			[Test]
			public void LevelUp_IncreasesLevelAndWorkplaceLimit()
			{
				((IZoneBuilding)commercial).LevelUp();

				Assert.AreEqual(ZoneBuildingLevel.ZERO, commercial.Level);
				int previousWorkplaceLimit = commercial.WorkplaceLimit;

				_ = new Worker(mockResidential, commercial, 40, Qualification.LOW);

				Assert.AreEqual(ZoneBuildingLevel.ONE, commercial.Level);
				Assert.Less(previousWorkplaceLimit, commercial.WorkplaceLimit);
				previousWorkplaceLimit = commercial.WorkplaceLimit;

				((IZoneBuilding)commercial).LevelUp();

				Assert.AreEqual(ZoneBuildingLevel.TWO, commercial.Level);
				Assert.Less(previousWorkplaceLimit, commercial.WorkplaceLimit);
				previousWorkplaceLimit = commercial.WorkplaceLimit;

				((IZoneBuilding)commercial).LevelUp();

				Assert.AreEqual(ZoneBuildingLevel.THREE, commercial.Level);
				Assert.Less(previousWorkplaceLimit, commercial.WorkplaceLimit);
				previousWorkplaceLimit = commercial.WorkplaceLimit;

				((IZoneBuilding)commercial).LevelUp();

				Assert.AreEqual(ZoneBuildingLevel.THREE, commercial.Level);
				Assert.AreEqual(previousWorkplaceLimit, commercial.WorkplaceLimit);
			}
		}
	}
}
