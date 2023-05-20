using Model.RoadGrids;
using Model.Tiles.Buildings.BuildingCommands;
using NUnit.Framework;

namespace Model.Tiles.Buildings
{
	public class FireDepartmentBuildingTileTest
	{
		private FireDepartment fireDepartment;
		private IRoadGridElement roadGridElement;

		[SetUp]
		public void SetUp()
		{
			City.Reset();

			roadGridElement = new RoadTile(0, 0);
			City.Instance.SetTile(roadGridElement.GetTile());

			fireDepartment = new FireDepartment(0, 1, 123, Rotation.Zero);
			City.Instance.SetTile(fireDepartment);
		}

		[Test]
		public void GetTileType_ReturnsFireDepartment()
		{
			var tileType = fireDepartment.GetTileType();

			Assert.AreEqual(TileType.FireDepartment, tileType);
		}

		[Test]
		public void RegisterWorkplace_AddsFireDepartmentToRoadGrid()
		{
			Assert.IsTrue(roadGridElement.RoadGrid.Workplaces.Contains(fireDepartment));
		}

		[Test]
		public void UnregisterWorkplace_RemovesFireDepartmentFromRoadGrid()
		{
			DestroyCommand destroy = new((int)fireDepartment.Coordinates.x, (int)fireDepartment.Coordinates.y);
			destroy.Execute();

			Assert.IsFalse(roadGridElement.RoadGrid.Workplaces.Contains(fireDepartment));
		}

		[Test]
		public void GetTile_ReturnsSelf()
		{
			var tile = fireDepartment.GetTile();

			Assert.AreEqual(fireDepartment, tile);
		}
	}
}
