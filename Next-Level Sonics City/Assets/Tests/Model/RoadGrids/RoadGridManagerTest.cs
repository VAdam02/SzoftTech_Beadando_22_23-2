using Model.Tiles;
using Model.Tiles.Buildings;
using NUnit.Framework;

namespace Model.RoadGrids
{
	internal class RoadGridManagerTest
	{
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

			RoadGridManager.Reset();
		}

		[Test]
		public void AddRoadGridElement_NoAdjacentRoadGridElements_NewRoadGridCreated()
		{
			RoadTile roadGridElement = new(0, 0);
			City.Instance.SetTile(roadGridElement);

			Assert.That(RoadGridManager.Instance.RoadGrids, Has.Count.EqualTo(1));
			Assert.That(((IRoadGridElement)roadGridElement).RoadGrid, Is.InstanceOf<RoadGrid>());
		}

		[Test]
		public void AddRoadGridElement_AdjacentRoadGridElementExists_RoadGridsMerged()
		{
			RoadTile roadGridElement1 = new(0, 0);
			City.Instance.SetTile(roadGridElement1);
			RoadTile roadGridElement2 = new(0, 1);
			City.Instance.SetTile(roadGridElement2);

			Assert.That(RoadGridManager.Instance.RoadGrids, Has.Count.EqualTo(1));
			Assert.That(((IRoadGridElement)roadGridElement1).RoadGrid, Is.InstanceOf<RoadGrid>());
			Assert.That(((IRoadGridElement)roadGridElement2).RoadGrid, Is.SameAs(((IRoadGridElement)roadGridElement1).RoadGrid));
		}

		[Test]
		public void AddRoadGridElement_AdjacentRoadGridElementsHaveDifferentRoadGrids_RoadGridsMerged()
		{
			RoadTile roadGridElement1 = new(0, 0);
			City.Instance.SetTile(roadGridElement1);
			RoadTile roadGridElement2 = new(0, 1);
			City.Instance.SetTile(roadGridElement2);

			var roadGrid1 = ((IRoadGridElement)roadGridElement1).RoadGrid;
			var roadGrid2 = ((IRoadGridElement)roadGridElement2).RoadGrid;

			Assert.That(roadGrid2, Is.SameAs(roadGrid1));
			Assert.That(RoadGridManager.Instance.RoadGrids, Has.Count.EqualTo(1));
		}

		[Test]
		public void AddRoadGridElement_RoadGridElementHasExistingRoadGrid_NoChange()
		{
			RoadTile roadGridElement1 = new(0, 0);
			City.Instance.SetTile(roadGridElement1);
			RoadTile roadGridElement2 = new(0, 1);
			City.Instance.SetTile(roadGridElement2);

			var roadGrid = new RoadGrid();
			roadGrid.AddRoadGridElement(roadGridElement1);

			Assert.That(((IRoadGridElement)roadGridElement2).RoadGrid, Is.Not.SameAs(roadGrid));
			Assert.That(RoadGridManager.Instance.RoadGrids, Has.Count.EqualTo(2));
		}

		[Test]
		public void GetRoadGrigElementByBuilding_BuildingHasAdjacentRoadGridElement_ReturnsCorrectRoadGridElement()
		{
			var roadGridElement = new RoadTile(0, 0);
			City.Instance.SetTile(roadGridElement);
			var building = new ResidentialBuildingTile(0, 1, 0, Rotation.Zero, ZoneBuildingLevel.ZERO);
			City.Instance.SetTile(building);

			var result = RoadGridManager.GetRoadGrigElementByBuilding(building);

			Assert.That(result, Is.SameAs(roadGridElement));
		}

		[Test]
		public void GetRoadGrigElementByBuilding_BuildingHasNoAdjacentRoadGridElement_ReturnsNull()
		{
			var building = new ResidentialBuildingTile(0, 0, 0, Rotation.Zero, ZoneBuildingLevel.ZERO);
			City.Instance.SetTile(building);

			var result = RoadGridManager.GetRoadGrigElementByBuilding(building);

			Assert.That(result, Is.Null);
		}


		[Test]
		public void GetBuildingsByRoadGridElement_RoadGridElementHasAdjacentBuildings_ReturnsListOfBuildings()
		{
			var roadGridElement = new RoadTile(0, 0);
			City.Instance.SetTile(roadGridElement);
			var residential1 = new ResidentialBuildingTile(0, 1, 0, Rotation.Zero, ZoneBuildingLevel.ZERO);
			City.Instance.SetTile(residential1);
			var residential2 = new ResidentialBuildingTile(1, 0, 0, Rotation.TwoSeventy, ZoneBuildingLevel.ZERO);
			City.Instance.SetTile(residential2);

			var result = RoadGridManager.GetBuildingsByRoadGridElement(roadGridElement);

			Assert.That(result, Does.Contain(residential1));
			Assert.That(result, Does.Contain(residential2));
		}

		[Test]
		public void GetBuildingsByRoadGridElement_RoadGridElementHasNoAdjacentBuildings_ReturnsEmptyList()
		{
			var roadGridElement = new RoadTile(0, 0);
			City.Instance.SetTile(roadGridElement);

			var result = RoadGridManager.GetBuildingsByRoadGridElement(roadGridElement);

			Assert.That(result, Is.Empty);
		}

		[Test]
		public void GetRoadGridElementsByRoadGridElement_RoadGridElementHasAdjacentRoadGridElements_ReturnsListOfRoadGridElements()
		{
			var roadGridElement = new RoadTile(0, 0);
			City.Instance.SetTile(roadGridElement);
			var adjacentRoadGridElement1 = new RoadTile(0, 1);
			City.Instance.SetTile(adjacentRoadGridElement1);
			var adjacentRoadGridElement2 = new RoadTile(1, 0);
			City.Instance.SetTile(adjacentRoadGridElement2);

			var result = ((IRoadGridElement)roadGridElement).ConnectsTo;

			Assert.That(result, Does.Contain(adjacentRoadGridElement1));
			Assert.That(result, Does.Contain(adjacentRoadGridElement2));
		}

		[Test]
		public void GetRoadGridElementsByRoadGridElement_RoadGridElementHasNoAdjacentRoadGridElements_ReturnsEmptyList()
		{
			var roadGridElement = new RoadTile(0, 0);
			City.Instance.SetTile(roadGridElement);

			var result = ((IRoadGridElement)roadGridElement).ConnectsTo;

			Assert.That(result, Is.Empty);
		}
	}
}
