using Model.Statistics;
using NUnit.Framework;

namespace Model.Tiles
{
	public class ForestTileTest
	{
		private ForestTile forest;

		[SetUp]
		public void SetUp()
		{
			City.Reset();

			forest = new ForestTile(0, 0, 123);
			City.Instance.SetTile(forest);
		}

		[Test]
		public void GetTileType_ReturnsForest()
		{
			var tileType = forest.GetTileType();

			Assert.AreEqual(TileType.Forest, tileType);
		}

		[Test]
		public void FinalizeTile_SetsPlantedYear()
		{
			var plantedYear = GetPlantedYear(forest);

			Assert.AreEqual(StatEngine.Instance.Year, plantedYear);
		}

		[Test]
		public void GetMaintainanceCost_ReturnsZero_WhenMaintenanceNotNeeded()
		{
			int plantedYear = GetPlantedYear(forest);
			while (plantedYear + 10 >= StatEngine.Instance.Year)
			{
				StatEngine.Instance.TimeElapsed();
			}

			var maintainanceCost = forest.MaintainanceCost;
			Assert.AreEqual(0, maintainanceCost);
		}

		// Helper method to access the private field _plantedYear
		private int GetPlantedYear(ForestTile forest)
		{
			return (int)typeof(ForestTile)
				.GetField("_plantedYear", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
				.GetValue(forest);
		}
	}
}
