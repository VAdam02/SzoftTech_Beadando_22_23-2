using Model.Tiles;
using Model.Tiles.Buildings;
using NUnit.Framework;

namespace Model.RoadGrids
{
	internal class RoadGridTest
	{
		[SetUp]
		public void SetUp()
		{
			RoadGridManager.Reset();

			City.Reset();
			for (int i = 0; i < City.Instance.GetSize(); i++)
			{
				for (int j = 0; j < City.Instance.GetSize(); j++)
				{
					City.Instance.SetTile(new EmptyTile(i, j));
				}
			}
		}

		[Test]
		public void SplitRoadGrid()
		{
			City.Instance.SetTile(new RoadTile(0, 0));
			City.Instance.SetTile(new RoadTile(1, 0));
			City.Instance.SetTile(new RoadTile(2, 0));

			Assert.AreEqual(1, RoadGridManager.Instance.RoadGrids.Count);

			ForcedDestroyCommand destroyCommand = new(1, 0);
			destroyCommand.Execute();

			Assert.AreEqual(2, RoadGridManager.Instance.RoadGrids.Count);
		}

		[Test]
		public void AddRoadGridElement_AddsElementToList()
		{
			var element = new RoadTile(0, 0);
			City.Instance.SetTile(element);

			Assert.That(((IRoadGridElement)element).RoadGrid.RoadGridElements, Has.Count.EqualTo(1));
			Assert.That(((IRoadGridElement)element).RoadGrid.RoadGridElements, Does.Contain(element));
		}

		[Test]
		public void RemoveRoadGridElement_RemovesElementFromList()
		{
			var element = new RoadTile(0, 0);
			City.Instance.SetTile(element);

			City.Instance.SetTile(new EmptyTile(0, 0));

			Assert.That(((IRoadGridElement)element).RoadGrid.RoadGridElements, Is.Empty);
		}

		[Test]
		public void AddWorkplace_AddsWorkplaceToList()
		{
			var element = new RoadTile(0, 0);
			City.Instance.SetTile(element);
			var workplace = new CommercialBuildingTIle(0, 1, 0, Rotation.Zero, ZoneBuildingLevel.ZERO);
			City.Instance.SetTile(workplace);

			Assert.That(((IRoadGridElement)element).RoadGrid.Workplaces, Has.Count.EqualTo(1));
			Assert.That(((IRoadGridElement)element).RoadGrid.Workplaces, Does.Contain(workplace));
		}

		[Test]
		public void RemoveWorkplace_RemovesWorkplaceFromList()
		{
			var element = new RoadTile(0, 0);
			City.Instance.SetTile(element);
			var workplace = new CommercialBuildingTIle(0, 1, 0, Rotation.Zero, ZoneBuildingLevel.ZERO);
			City.Instance.SetTile(workplace);

			City.Instance.SetTile(new EmptyTile(0, 1));

			Assert.AreEqual(((IRoadGridElement)element).RoadGrid.Workplaces.Count, 0);
			Assert.That(!((IRoadGridElement)element).RoadGrid.Workplaces.Contains(workplace));
		}

		[Test]
		public void AddResidential_AddsResidentialToList()
		{
			var element = new RoadTile(0, 0);
			City.Instance.SetTile(element);
			var residential = new ResidentialBuildingTile(0, 1, 0, Rotation.Zero, ZoneBuildingLevel.ZERO);
			City.Instance.SetTile(residential);

			Assert.That(((IRoadGridElement)element).RoadGrid.Residentials, Has.Count.EqualTo(1));
			Assert.That(((IRoadGridElement)element).RoadGrid.Residentials, Does.Contain(residential));
		}

		[Test]
		public void RemoveResidential_RemovesResidentialFromList()
		{
			Tile element = new RoadTile(0, 0);
			City.Instance.SetTile(element);
			var residential = new ResidentialBuildingTile(0, 1, 0, Rotation.Zero, ZoneBuildingLevel.ZERO);
			City.Instance.SetTile(residential);

			ForcedDestroyCommand destroyCommand = new(0, 1);
			destroyCommand.Execute();

			Assert.That(((IRoadGridElement)element).RoadGrid.Residentials, Is.Empty);
			Assert.That(!((IRoadGridElement)element).RoadGrid.Residentials.Contains(residential));
		}

		[Test]
		public void Merge_TwoNonEmptyRoadGridsWithNoCommonElements_RoadGridElementsMerged()
		{
			RoadTile element1 = new(0, 0);
			City.Instance.SetTile(element1);
			RoadTile element2 = new(0, 2);
			City.Instance.SetTile(element2);

			City.Instance.SetTile(new RoadTile(0, 1));

			var roadGrid1 = ((IRoadGridElement)element1).RoadGrid;
			var roadGrid2 = ((IRoadGridElement)element2).RoadGrid;

			Assert.That(roadGrid1.RoadGridElements, Has.Count.EqualTo(3));
			Assert.That(roadGrid1.RoadGridElements, Does.Contain(element1));
			Assert.That(roadGrid1.RoadGridElements, Does.Contain(element2));
			Assert.AreEqual(roadGrid2, roadGrid1);
		}
	}
}
