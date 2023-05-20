using Model.RoadGrids;
using System;
using System.Collections.Generic;
using System.Linq;

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

		public (float happiness, float weight) HappinessByBuilding
		{
			get
			{
				float happinessSum = _happinessChangers.Aggregate(0.0f, (acc, item) => acc + item.happiness * item.weight);
				float happinessWeight = _happinessChangers.Aggregate(0.0f, (acc, item) => acc + item.weight);
				return (happinessSum / (happinessWeight == 0 ? 1 : happinessWeight), happinessWeight);
			}
		}

		private readonly List<(IHappyZone happyZone, float happiness, float weight)> _happinessChangers = new();
		public void RegisterHappinessChangerTile(IHappyZone happyZone)
		{
			happyZone.GetTile().OnTileDelete.AddListener(UnregisterHappinessChangerTile);
			happyZone.GetTile().OnTileChange.AddListener(UpdateHappiness);

			(float happiness, float weight) = happyZone.GetHappinessModifierAtTile(this);
			_happinessChangers.Add((happyZone, happiness, weight));
		}

		private void UnregisterHappinessChangerTile(Tile deletedTile)
		{
			IHappyZone happyZone = (IHappyZone)deletedTile;
			_happinessChangers.RemoveAll((values) => values.happyZone == happyZone);
		}

		private void UpdateHappiness(Tile changedTile)
		{
			IHappyZone happyZone = (IHappyZone)changedTile;
			_happinessChangers.RemoveAll((values) => values.happyZone == happyZone);

			(float happiness, float weight) = happyZone.GetHappinessModifierAtTile(this);
			_happinessChangers.Add((happyZone, happiness, weight));
		}

		public override float GetTransparency()
		{
			throw new NotImplementedException();
		}
	}
}
