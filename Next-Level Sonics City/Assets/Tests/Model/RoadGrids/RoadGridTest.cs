using Model.Tiles;
using Model.Tiles.Buildings;
using NUnit.Framework;

namespace Model.RoadGrids
{
	internal class RoadGridTest
	{
		private RoadGrid _roadGrid;

		[SetUp]
		public void SetUp()
		{
			RoadGridManager.Reset();
			_roadGrid = new RoadGrid();

			City.Reset();
		}

		[Test]
		public void AddRoadGridElement_AddsElementToList()
		{
			var element = new MockRoadGridElement(0, 0);
			City.Instance.SetTile(element);

			_roadGrid.AddRoadGridElement(element);

			Assert.That(_roadGrid.RoadGridElements, Has.Count.EqualTo(1));
			Assert.That(_roadGrid.RoadGridElements, Does.Contain(element));
		}

		[Test]
		public void RemoveRoadGridElement_RemovesElementFromList()
		{
			var element = new MockRoadGridElement(0, 0);
			City.Instance.SetTile(element);

			_roadGrid.AddRoadGridElement(element);
			_roadGrid.RemoveRoadGridElement(element);

			Assert.That(_roadGrid.RoadGridElements, Is.Empty);
			Assert.That(!_roadGrid.RoadGridElements.Contains(element));
		}

		[Test]
		public void AddWorkplace_AddsWorkplaceToList()
		{
			var workplace = new MockWorkplaceBuildingTile(0, 0, Rotation.Zero);
			City.Instance.SetTile(workplace);

			_roadGrid.AddWorkplace(workplace);

			Assert.That(_roadGrid.Workplaces, Has.Count.EqualTo(1));
			Assert.That(_roadGrid.Workplaces, Does.Contain(workplace));
		}

		[Test]
		public void RemoveWorkplace_RemovesWorkplaceFromList()
		{
			var workplace = new MockWorkplaceBuildingTile(0, 0, Rotation.Zero);
			City.Instance.SetTile(workplace);

			_roadGrid.AddWorkplace(workplace);
			_roadGrid.RemoveWorkplace(workplace);

			Assert.That(_roadGrid.Workplaces, Is.Empty);
			Assert.That(!_roadGrid.Workplaces.Contains(workplace));
		}

		[Test]
		public void AddResidential_AddsResidentialToList()
		{
			var residential = new MockResidentialBuildingTile(0, 0, Rotation.Zero);
			City.Instance.SetTile(residential);

			_roadGrid.AddResidential(residential);

			Assert.That(_roadGrid.Residentials, Has.Count.EqualTo(1));
			Assert.That(_roadGrid.Residentials, Does.Contain(residential));
		}

		[Test]
		public void RemoveResidential_RemovesResidentialFromList()
		{
			var residential = new MockResidentialBuildingTile(0, 0, Rotation.Zero);
			City.Instance.SetTile(residential);

			_roadGrid.AddResidential(residential);
			_roadGrid.RemoveResidential(residential);

			Assert.That(_roadGrid.Residentials, Is.Empty);
			Assert.That(!_roadGrid.Residentials.Contains(residential));
		}

		[Test]
		public void Merge_TwoEmptyRoadGrids_NoChanges()
		{
			var roadGrid1 = new RoadGrid();
			var roadGrid2 = new RoadGrid();

			roadGrid1.Merge(roadGrid2);

			Assert.That(roadGrid1.RoadGridElements, Is.Empty);
			Assert.That(roadGrid2.RoadGridElements, Is.Empty);
		}

		[Test]
		public void Merge_EmptyRoadGridIntoNonEmptyRoadGrid_RoadGridElementsMerged()
		{
			MockRoadGridElement element = new(0, 0);
			City.Instance.SetTile(element);

			var roadGrid1 = new RoadGrid();
			var roadGrid2 = ((IRoadGridElement)element).GetRoadGrid();

			roadGrid1.Merge(roadGrid2);

			Assert.That(roadGrid1.RoadGridElements, Has.Count.EqualTo(1));
			Assert.That(roadGrid1.RoadGridElements, Does.Contain(element));
			Assert.That(roadGrid2.RoadGridElements, Is.Empty);
		}

		[Test]
		public void Merge_NonEmptyRoadGridIntoEmptyRoadGrid_RoadGridElementsMerged()
		{
			MockRoadGridElement element = new(0, 0);
			City.Instance.SetTile(element);

			var roadGrid1 = ((IRoadGridElement)element).GetRoadGrid();
			var roadGrid2 = new RoadGrid();

			roadGrid1.Merge(roadGrid2);

			Assert.That(roadGrid1.RoadGridElements, Has.Count.EqualTo(1));
			Assert.That(roadGrid1.RoadGridElements, Does.Contain(element));
			Assert.That(roadGrid2.RoadGridElements, Is.Empty);
		}

		[Test]
		public void Merge_TwoNonEmptyRoadGridsWithNoCommonElements_RoadGridElementsMerged()
		{
			MockRoadGridElement element1 = new(0, 0);
			City.Instance.SetTile(element1);
			MockRoadGridElement element2 = new(0, 2);
			City.Instance.SetTile(element2);

			var roadGrid1 = ((IRoadGridElement)element1).GetRoadGrid();
			var roadGrid2 = ((IRoadGridElement)element2).GetRoadGrid();

			roadGrid1.Merge(roadGrid2);

			Assert.That(roadGrid1.RoadGridElements, Has.Count.EqualTo(2));
			Assert.That(roadGrid1.RoadGridElements, Does.Contain(element1));
			Assert.That(roadGrid1.RoadGridElements, Does.Contain(element2));
			Assert.That(roadGrid2.RoadGridElements, Is.Empty);
		}
	}
}
