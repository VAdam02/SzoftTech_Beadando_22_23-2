using NUnit.Framework;

namespace Model.Tiles.Buildings.BuildingCommands
{
	public class ExpandCommandTests
	{
		[SetUp]
		public void Setup()
		{
			City.Reset();
		}

		[Test]
		public void Execute_ExpandWorkplace_CreatesWorkplaceSubTile()
		{
			int x = 3;
			int y = 4;

			Assert.IsNotInstanceOf<WorkplaceSubTile>(City.Instance.GetTile(x + 1, y));
			Assert.IsNotInstanceOf<WorkplaceSubTile>(City.Instance.GetTile(x, y + 1));
			Assert.IsNotInstanceOf<WorkplaceSubTile>(City.Instance.GetTile(x + 1, y + 1));

			IWorkplace workplace = new StadionBuildingTile(x, y, 0, Rotation.Zero);

			ExpandCommand command = new(x + 1, y, workplace);
			command.Execute();
			Assert.IsInstanceOf<WorkplaceSubTile>(City.Instance.GetTile(x + 1, y));

			command = new(x, y + 1, workplace);
			command.Execute();
			Assert.IsInstanceOf<WorkplaceSubTile>(City.Instance.GetTile(x, y + 1));

			command = new(x + 1, y + 1, workplace);
			command.Execute();
			Assert.IsInstanceOf<WorkplaceSubTile>(City.Instance.GetTile(x + 1, y + 1));
		}
	}
}
