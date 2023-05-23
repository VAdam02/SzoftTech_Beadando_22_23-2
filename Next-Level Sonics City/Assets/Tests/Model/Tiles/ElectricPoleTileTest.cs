using NUnit.Framework;

namespace Model.Tiles
{
	public class ElectricPoleTileTest
	{
		private ElectricPole electricPole;

		[SetUp]
		public void SetUp()
		{
			electricPole = new ElectricPole(0, 0, 123);
		}

		[Test]
		public void GetTileType_ReturnsElectricPole()
		{
			var tileType = electricPole.GetTileType();

			Assert.AreEqual(TileType.ElectricPole, tileType);
		}
	}
}
