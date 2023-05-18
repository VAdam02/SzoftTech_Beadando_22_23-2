using NUnit.Framework;
using System;

namespace Model.Tiles.Buildings.BuildingCommands
{
	public class BuildCommandTests
	{
		[SetUp]
		public void Setup()
		{
			City.Reset();
		}

		[Test]
		public void Execute_ValidTileType_CreatesTileAndAddsToCity()
		{
			int x = 3;
			int y = 4;
			TileType tileType = TileType.PoliceDepartment;
			Rotation rotation = Rotation.Zero;
			uint designID = 123;

			BuildCommand command = new(x, y, tileType, rotation, designID);
			command.Execute();

			Tile createdTile = City.Instance.GetTile(x, y);
			Assert.NotNull(createdTile);
			Assert.AreEqual(tileType, createdTile.GetTileType());
			if (createdTile is Building building)
			{
				Assert.AreEqual(rotation, building.Rotation);
			}
			Assert.AreEqual(designID, createdTile.DesignID);
		}

		[Test]
		public void Execute_TileCannotBuild_ThrowsException()
		{
			int x = 3;
			int y = 4;
			TileType tileType = TileType.PoliceDepartment;
			Rotation rotation = Rotation.TwoSeventy;
			uint designID = 123;

			BuildCommand command = new (x, y, tileType, rotation, designID);
			command.Execute();

			Assert.Throws<Exception>(() => command.Execute());

		}
	}
}