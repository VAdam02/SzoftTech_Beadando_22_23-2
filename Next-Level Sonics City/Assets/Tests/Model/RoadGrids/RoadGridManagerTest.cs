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
			RoadGridManager.Reset();
			City.Reset();
		}

		[Test]
		public void AddRoadGridElement_NoAdjacentRoadGridElements_NewRoadGridCreated()
		{
			MockRoadGridElement roadGridElement = new(0, 0);
			City.Instance.SetTile(roadGridElement);

			Assert.That(RoadGridManager.Instance.RoadGrids, Has.Count.EqualTo(1));
			Assert.That(((IRoadGridElement)roadGridElement).GetRoadGrid(), Is.InstanceOf<RoadGrid>());
		}

		[Test]
		public void AddRoadGridElement_AdjacentRoadGridElementExists_RoadGridsMerged()
		{
			MockRoadGridElement roadGridElement1 = new(0, 0);
			City.Instance.SetTile(roadGridElement1);
			MockRoadGridElement roadGridElement2 = new(0, 1);
			City.Instance.SetTile(roadGridElement2);

			Assert.That(RoadGridManager.Instance.RoadGrids, Has.Count.EqualTo(1));
			Assert.That(((IRoadGridElement)roadGridElement1).GetRoadGrid(), Is.InstanceOf<RoadGrid>());
			Assert.That(((IRoadGridElement)roadGridElement2).GetRoadGrid(), Is.SameAs(((IRoadGridElement)roadGridElement1).GetRoadGrid()));
		}

		[Test]
		public void AddRoadGridElement_AdjacentRoadGridElementsHaveDifferentRoadGrids_RoadGridsMerged()
		{
			MockRoadGridElement roadGridElement1 = new(0, 0);
			City.Instance.SetTile(roadGridElement1);
			MockRoadGridElement roadGridElement2 = new(0, 1);
			City.Instance.SetTile(roadGridElement2);

			var roadGrid1 = ((IRoadGridElement)roadGridElement1).GetRoadGrid();
			var roadGrid2 = ((IRoadGridElement)roadGridElement2).GetRoadGrid();

			Assert.That(roadGrid2, Is.SameAs(roadGrid1));
			Assert.That(RoadGridManager.Instance.RoadGrids, Has.Count.EqualTo(1));
		}

		[Test]
		public void AddRoadGridElement_RoadGridElementHasExistingRoadGrid_NoChange()
		{
			MockRoadGridElement roadGridElement1 = new(0, 0);
			City.Instance.SetTile(roadGridElement1);
			MockRoadGridElement roadGridElement2 = new(0, 1);
			City.Instance.SetTile(roadGridElement2);

			var roadGrid = new RoadGrid();
			roadGrid.AddRoadGridElement(roadGridElement1);

			Assert.That(((IRoadGridElement)roadGridElement2).GetRoadGrid(), Is.Not.SameAs(roadGrid));
			Assert.That(RoadGridManager.Instance.RoadGrids, Has.Count.EqualTo(2));
		}

		[Test]
		public void GetRoadGrigElementByBuilding_BuildingHasAdjacentRoadGridElement_ReturnsCorrectRoadGridElement()
		{
			var roadGridElement = new MockRoadGridElement(0, 0);
			City.Instance.SetTile(roadGridElement);
			var building = new MockResidentialBuildingTile(0, 1, Rotation.Zero);
			City.Instance.SetTile(building);

			var result = RoadGridManager.GetRoadGrigElementByBuilding(building);

			Assert.That(result, Is.SameAs(roadGridElement));
		}

		[Test]
		public void GetRoadGrigElementByBuilding_BuildingHasNoAdjacentRoadGridElement_ReturnsNull()
		{
			var building = new MockResidentialBuildingTile(0, 0, Rotation.Zero);
			City.Instance.SetTile(building);

			var result = RoadGridManager.GetRoadGrigElementByBuilding(building);

			Assert.That(result, Is.Null);
		}


		[Test]
		public void GetBuildingsByRoadGridElement_RoadGridElementHasAdjacentBuildings_ReturnsListOfBuildings()
		{
			var roadGridElement = new MockRoadGridElement(0, 0);
			City.Instance.SetTile(roadGridElement);
			var residential1 = new MockResidentialBuildingTile(0, 1, Rotation.Zero);
			City.Instance.SetTile(residential1);
			var residential2 = new MockResidentialBuildingTile(1, 0, Rotation.TwoSeventy);
			City.Instance.SetTile(residential2);

			var result = RoadGridManager.GetBuildingsByRoadGridElement(roadGridElement);

			Assert.That(result, Does.Contain(residential1));
			Assert.That(result, Does.Contain(residential2));
		}

		[Test]
		public void GetBuildingsByRoadGridElement_RoadGridElementHasNoAdjacentBuildings_ReturnsEmptyList()
		{
			var roadGridElement = new MockRoadGridElement(0, 0);
			City.Instance.SetTile(roadGridElement);

			var result = RoadGridManager.GetBuildingsByRoadGridElement(roadGridElement);

			Assert.That(result, Is.Empty);
		}

		[Test]
		public void GetRoadGridElementsByRoadGridElement_RoadGridElementHasAdjacentRoadGridElements_ReturnsListOfRoadGridElements()
		{
			var roadGridElement = new MockRoadGridElement(0, 0);
			City.Instance.SetTile(roadGridElement);
			var adjacentRoadGridElement1 = new MockRoadGridElement(0, 1);
			City.Instance.SetTile(adjacentRoadGridElement1);
			var adjacentRoadGridElement2 = new MockRoadGridElement(1, 0);
			City.Instance.SetTile(adjacentRoadGridElement2);

			var result = RoadGridManager.GetRoadGridElementsByRoadGridElement(roadGridElement);

			Assert.That(result, Does.Contain(adjacentRoadGridElement1));
			Assert.That(result, Does.Contain(adjacentRoadGridElement2));
		}

		[Test]
		public void GetRoadGridElementsByRoadGridElement_RoadGridElementHasNoAdjacentRoadGridElements_ReturnsEmptyList()
		{
			var roadGridElement = new MockRoadGridElement(0, 0);
			City.Instance.SetTile(roadGridElement);

			var result = RoadGridManager.GetRoadGridElementsByRoadGridElement(roadGridElement);

			Assert.That(result, Is.Empty);
		}
	}
}
