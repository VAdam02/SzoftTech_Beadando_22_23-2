using Model.Persons;
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
			City.Instance.SetTile(new RoadTile(0, 0));
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
			IResidential residential = new ResidentialBuildingTile(0, 1, 0, Rotation.Zero, ZoneBuildingLevel.ZERO);
			City.Instance.SetTile(residential.GetTile());
			_ = new Pensioner(residential, 70, 100);
			_ = new Pensioner(residential, 80, 100);

			int population = City.Instance.GetPopulation();

			Assert.AreEqual(2, population);
		}

	}
}
