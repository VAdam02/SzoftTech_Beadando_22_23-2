using Model.Tiles.Buildings;
using NUnit.Framework;

namespace Model.Tiles
{
	public class ZoneManagerTests
	{
		[SetUp]
		public void Setup()
		{
			ZoneManager.Reset();
			City.Reset();
		}

		[Test]
		public void MarkZone_ValidZoneType_MarksZoneAndRaisesZoneMarkedEvent()
		{
			for (int i = 0; i < 10; i++)
			{
				City.Instance.SetTile(new RoadTile(i, 5));
			}

			ZoneType zoneType = ZoneType.ResidentialZone;

			bool eventRaised = false;
			ZoneManager.Instance.ZoneMarked += (sender, e) => eventRaised = true;

			ZoneManager.Instance.MarkZone(City.Instance.GetTile(0, 4), City.Instance.GetTile(9, 6), zoneType);

			for (int i = 0; i < 10; i++)
			{
				Assert.IsInstanceOf<ResidentialBuildingTile>(City.Instance.GetTile(i, 4));
				Assert.IsInstanceOf<ResidentialBuildingTile>(City.Instance.GetTile(i, 6));
			}

			Assert.True(eventRaised);
		}
	}
}
