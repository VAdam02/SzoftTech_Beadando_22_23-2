using Model.RoadGrids;
using Model.Tiles.Buildings;
using Model.Tiles.Buildings.BuildingCommands;
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
			for (int i = 0; i < City.Instance.GetSize(); i++)
			{
				for (int j = 0; j < City.Instance.GetSize(); j++)
				{
					City.Instance.SetTile(new EmptyTile(i, j));
				}
			}

			roadTile = new RoadTile(1, 1);
			City.Instance.SetTile(roadTile);
			aboveRoad = new RoadTile(1, 0);
			City.Instance.SetTile(aboveRoad);
			rightRoad = new RoadTile(2, 1);
			City.Instance.SetTile(rightRoad);
			belowRoad = new RoadTile(1, 2);
			City.Instance.SetTile(belowRoad);
			leftRoad = new RoadTile(0, 1);
			City.Instance.SetTile(leftRoad);
		}

		[Test]
		public void ConnectsTo_ReturnsCorrectConnectedRoads()
		{
			List<IRoadGridElement> connectedRoads = ((IRoadGridElement)roadTile).ConnectsTo;

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
			RoadGrid roadGrid = ((IRoadGridElement)roadTile).RoadGrid;

			Assert.IsNotNull(roadGrid);
		}

		[Test]
		public void FinalizeTile_CallsConnectToSurroundingRoads()
		{
			roadTile.FinalizeTile();

			Assert.IsNotNull(roadTile.ConnectsFromAbove);
			Assert.IsNotNull(roadTile.ConnectsFromRight);
			Assert.IsNotNull(roadTile.ConnectsFromBelow);
			Assert.IsNotNull(roadTile.ConnectsFromLeft);
		}

		[Test]
		public void NeighborTileReplaced_DestroyAndAddsNewRoadConnection()
		{
			int x = (int)aboveRoad.Coordinates.x;
			int y = (int)aboveRoad.Coordinates.y;
			DestroyCommand destroy = new(x, y);
			destroy.Execute();

			Assert.IsNull(roadTile.ConnectsFromAbove);

			BuildCommand build = new(x, y, TileType.Road, Rotation.Zero);
			build.Execute();

			Assert.AreEqual(City.Instance.GetTile(x, y), roadTile.ConnectsFromAbove);
		}
	}
}
