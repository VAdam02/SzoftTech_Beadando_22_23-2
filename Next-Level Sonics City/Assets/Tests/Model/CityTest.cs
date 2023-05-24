using Model.Tiles;
using Model.Tiles.Buildings;
using NUnit.Framework;

namespace Model
{
	internal class CityTest
	{
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
		}

		[Test]
		public void GetTile_ByVectorCoordinates_ReturnsCorrectTile()
		{
			City city = City.Instance;
			Tile tile = new RoadTile(2, 3);
			city.SetTile(tile);

			Tile retrievedTile = city.GetTile(2, 3);

			Assert.AreEqual(tile, retrievedTile);
		}

		[Test]
		public void GetTile_ByFloatCoordinates_ReturnsCorrectTile()
		{
			City city = City.Instance;
			Tile tile = new RoadTile(2, 3);
			city.SetTile(tile);

			Tile retrievedTile = city.GetTile(2.0f, 3.0f);

			Assert.AreEqual(tile, retrievedTile);
		}

		[Test]
		public void GetTile_ByIntegerCoordinates_ReturnsCorrectTile()
		{
			City city = City.Instance;
			Tile tile = new RoadTile(2, 3);
			city.SetTile(tile);

			Tile retrievedTile = city.GetTile(2, 3);

			Assert.AreEqual(tile, retrievedTile);
		}

		[Test]
		public void SetTile_SetsTileAtCoordinates()
		{
			City city = City.Instance;
			Tile tile = new RoadTile(2, 3);

			city.SetTile(tile);
			Tile retrievedTile = city.GetTile(2, 3);

			Assert.AreEqual(tile, retrievedTile);
		}

		[Test]
		public void GetPopulation_ReturnsCorrectPopulation()
		{
			IResidential residential = new MockResidentialBuildingTile(0, 0, Rotation.Zero);
			City.Instance.SetTile(residential.GetTile());
			_ = new MockPerson(residential, 50);
			_ = new MockPerson(residential, 60);

			int population = City.Instance.GetPopulation();

			Assert.AreEqual(2, population);
		}

	}
}
