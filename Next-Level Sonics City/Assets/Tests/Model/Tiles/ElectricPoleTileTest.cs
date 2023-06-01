using NUnit.Framework;

namespace Model.Tiles
{
	public class ElectricPoleTileTest
	{
		private ElectricPoleTile electricPole;

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

			electricPole = new ElectricPoleTile(0, 0, 123);
		}

		[Test]
		public void GetTileType_ReturnsElectricPole()
		{
			var tileType = electricPole.GetTileType();

			Assert.AreEqual(TileType.ElectricPole, tileType);
		}
	}
}
