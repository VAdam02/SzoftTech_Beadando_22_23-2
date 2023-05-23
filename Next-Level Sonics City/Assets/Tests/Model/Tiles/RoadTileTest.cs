using Model.RoadGrids;
using NUnit.Framework;
using System.Collections.Generic;

namespace Model.Tiles
{
	public class RoadTileTest
	{
		private RoadTile roadTile;
		private RoadTile aboveRoad;
		private RoadTile rightRoad;
		private RoadTile belowRoad;
		private RoadTile leftRoad;

		[SetUp]
		public void Setup()
		{
			City.Reset();

			roadTile = new RoadTile(1, 1, 0);
			City.Instance.SetTile(roadTile);
			aboveRoad = new RoadTile(1, 0, 0);
			City.Instance.SetTile(aboveRoad);
			rightRoad = new RoadTile(2, 1, 0);
			City.Instance.SetTile(rightRoad);
			belowRoad = new RoadTile(1, 2, 0);
			City.Instance.SetTile(belowRoad);
			leftRoad = new RoadTile(0, 1, 0);
			City.Instance.SetTile(leftRoad);
		}

		[Test]
		public void ConnectsTo_ReturnsCorrectConnectedRoads()
		{
			List<IRoadGridElement> connectedRoads = roadTile.ConnectsTo();

			Assert.Contains(aboveRoad, connectedRoads);
			Assert.Contains(rightRoad, connectedRoads);
			Assert.Contains(belowRoad, connectedRoads);
			Assert.Contains(leftRoad, connectedRoads);
		}

		[Test]
		public void GetTile_ReturnsItself()
		{
			Tile tile = roadTile.GetTile();

			Assert.AreEqual(roadTile, tile);
		}

		[Test]
		public void GetRoadGrid_ReturnsNullByDefault()
		{
			RoadGrid roadGrid = roadTile.GetRoadGrid();

			Assert.IsNotNull(roadGrid);
		}

		[Test]
		public void FinalizeTile_CallsConnectToSurroundingRoads()
		{
			roadTile.FinalizeTile();

			Assert.IsNotNull(roadTile.FromAbove);
			Assert.IsNotNull(roadTile.FromRight);
			Assert.IsNotNull(roadTile.FromBelow);
			Assert.IsNotNull(roadTile.FromLeft);
		}

		[Test]
		public void NeighborTileReplaced_RemovesOldRoadConnection()
		{
			roadTile.FromAbove = aboveRoad;

			roadTile.NeighborTileReplaced(aboveRoad, null);

			Assert.IsNull(roadTile.FromAbove);
		}

		[Test]
		public void NeighborTileReplaced_AddsNewRoadConnection()
		{
			roadTile.NeighborTileReplaced(null, aboveRoad);

			Assert.AreEqual(aboveRoad, roadTile.FromAbove);
		}
	}
}
