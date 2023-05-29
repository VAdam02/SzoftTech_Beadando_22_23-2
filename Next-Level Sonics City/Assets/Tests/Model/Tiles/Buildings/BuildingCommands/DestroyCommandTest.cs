using NUnit.Framework;

namespace Model.Tiles.Buildings.BuildingCommands
{
	public class DestroyCommandTests
	{
		[SetUp]
		public void Setup()
		{
			City.Reset();
		}

		[Test]
		public void Execute_DestroyTile_RemovesTileFromCity()
		{
			int x = 3;
			int y = 4;
			City.Instance.SetTile(new RoadTile(x, y));

			Assert.IsNotInstanceOf<EmptyTile>(City.Instance.GetTile(x, y));

			DestroyCommand command = new(x, y);
			command.Execute();

			Assert.IsInstanceOf<EmptyTile>(City.Instance.GetTile(x, y));
		}
	}
}
