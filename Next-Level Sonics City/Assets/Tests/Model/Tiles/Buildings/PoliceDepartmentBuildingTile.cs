using Model.RoadGrids;
using Model.Tiles.Buildings.BuildingCommands;
using NUnit.Framework;

namespace Model.Tiles.Buildings
{
	[TestFixture]
	public class PoliceDepartmentBuildingTileTest
	{
		private PoliceDepartmentBuildingTile policeDepartment;
		private IRoadGridElement roadGridElement;

		[SetUp]
		public void SetUp()
		{
			City.Reset();

			roadGridElement = new RoadTile(0, 0);
			City.Instance.SetTile(roadGridElement.GetTile());

			policeDepartment = new PoliceDepartmentBuildingTile(0, 1, 123, Rotation.Zero);
			City.Instance.SetTile(policeDepartment);
		}

		[Test]
		public void GetTileType_ReturnsPoliceDepartment()
		{
			var tileType = policeDepartment.GetTileType();

			Assert.AreEqual(TileType.PoliceDepartment, tileType);
		}

		[Test]
		public void RegisterWorkplace_AddsPoliceDepartmentToRoadGrid()
		{
			var roadGrid = roadGridElement.RoadGrid;
			policeDepartment.RegisterWorkplace(roadGrid);

			Assert.IsTrue(roadGrid.Workplaces.Contains(policeDepartment));
		}

		[Test]
		public void UnregisterWorkplace_RemovesPoliceDepartmentFromRoadGrid()
		{
			DestroyCommand destroy = new((int)policeDepartment.Coordinates.x, (int)policeDepartment.Coordinates.y);
			destroy.Execute();

			Assert.IsFalse(roadGridElement.RoadGrid.Workplaces.Contains(policeDepartment));
		}

		[Test]
		public void GetTile_ReturnsSelf()
		{
			var tile = policeDepartment.GetTile();

			Assert.AreEqual(policeDepartment, tile);
		}
	}
}
