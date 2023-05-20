using Model.RoadGrids;
using System;
using System.Collections.Generic;

namespace Model.Tiles.Buildings
{
	public class MockResidentialBuildingTile : Building, IResidential, IZoneBuilding
	{
		private readonly List<Person> _residents = new();
		public int ResidentLimit { get; private set; }

		ZoneBuildingLevel IZoneBuilding.Level => throw new NotImplementedException();

		(float happiness, float weight) IResidential.HappinessByBuilding => throw new NotImplementedException();

		public MockResidentialBuildingTile(int x, int y, Rotation rotation) : base(x, y, 0, rotation)
		{

		}
		public override void FinalizeTile()
		{
			Finalizing();
		}

		protected new void Finalizing()
		{
			base.Finalizing();
			ResidentLimit = 10;
		}

		public override int GetBuildPrice()
		{
			return 100;
		}

		public override int GetDestroyIncome()
		{
			return 100;
		}

		public override TileType GetTileType()
		{
			throw new NotImplementedException();
		}

		public List<Person> GetResidents()
		{
			return _residents;
		}

		public int GetResidentsCount()
		{
			throw new NotImplementedException();
		}
		public void MoveIn(Person person)
		{
			_residents.Add(person);
		}

		public void MoveOut(Person person)
		{
			_residents.Remove(person);
		}

		public void RegisterResidential(RoadGrid roadGrid)
		{
			roadGrid?.AddResidential(this);
		}

		public void UnregisterResidential(RoadGrid roadGrid)
		{
			throw new NotImplementedException();
		}

		public ZoneType GetZoneType()
		{
			throw new NotImplementedException();
		}

		public void LevelUp()
		{
			throw new NotImplementedException();
		}

		public int GetLevelUpCost()
		{
			throw new NotImplementedException();
		}

		public void RegisterHappinessChangerTile(IHappyZone happyZone)
		{
			throw new NotImplementedException();
		}

		public override float GetTransparency()
		{
			throw new NotImplementedException();
		}
	}
}
