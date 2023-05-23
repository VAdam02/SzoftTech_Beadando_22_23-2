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

		List<Person> IResidential.GetResidents()
		{
			return _residents;
		}

		int IResidential.GetResidentsCount()
		{
			throw new NotImplementedException();
		}

		Tile IResidential.GetTile()
		{
			return this;
		}

		void IResidential.MoveIn(Person person)
		{
			_residents.Add(person);
		}

		void IResidential.MoveOut(Person person)
		{
			//That imaginary place don't fit our expectations so we are going to move out
		}

		void IResidential.RegisterResidential(RoadGrid roadGrid)
		{
			roadGrid?.AddResidential(this);
		}

		void IResidential.UnregisterResidential(RoadGrid roadGrid)
		{
			throw new NotImplementedException();
		}

		ZoneType IZoneBuilding.GetZoneType()
		{
			throw new NotImplementedException();
		}

		void IZoneBuilding.LevelUp()
		{
			throw new NotImplementedException();
		}

		int IZoneBuilding.GetLevelUpCost()
		{
			throw new NotImplementedException();
		}

		Tile IZoneBuilding.GetTile()
		{
			throw new NotImplementedException();
		}
	}
}
