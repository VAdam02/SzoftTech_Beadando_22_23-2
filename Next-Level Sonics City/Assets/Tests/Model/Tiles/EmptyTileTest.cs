using NUnit.Framework;
using System;

namespace Model.Tiles.Buildings
{
	[TestFixture]
	public class EmptyTileTest
	{
		private EmptyTile emptyTile;

		[SetUp]
		public void SetUp()
		{
			City.Reset();
			emptyTile = new EmptyTile(0, 0, 123);
			City.Instance.SetTile(emptyTile);
		}

		[Test]
		public void CanBuild_ReturnsFalseIfAdjacentTileIsNotEmptyTile()
		{
			var adjacentTile = new EmptyTile(0, 1, 456);
			City.Instance.SetTile(adjacentTile);

			var canBuild = emptyTile.CanBuild();

			Assert.IsFalse(canBuild);
		}

		[Test]
		public void GetZoneType_ReturnsNoZone()
		{
			var zoneType = ((IZoneBuilding)emptyTile).GetZoneType();

			Assert.AreEqual(ZoneType.NoZone, zoneType);
		}

		[Test]
		public void GetTile_ReturnsSelf()
		{
			var tile = ((IZoneBuilding)emptyTile).GetTile();

			Assert.AreEqual(emptyTile, tile);
		}

		[Test]
		public void LevelUp_ThrowsInvalidOperationException()
		{
			Assert.Throws<InvalidOperationException>(() => ((IZoneBuilding)emptyTile).LevelUp());
		}

		[Test]
		public void GetLevelUpCost_ThrowsInvalidOperationException()
		{
			Assert.Throws<InvalidOperationException>(() => ((IZoneBuilding)emptyTile).GetLevelUpCost());
		}
	}
}
