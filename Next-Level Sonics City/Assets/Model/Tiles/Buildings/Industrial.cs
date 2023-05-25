using Model.Persons;
using Model.RoadGrids;
using Model.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model.Tiles.Buildings
{
	public class Industrial : Building, IWorkplace, IZoneBuilding, IHappyZone
	{
		#region Tile implementation
		public override TileType GetTileType() { throw new InvalidOperationException("IZoneBuilding is not a valid tile type"); }

		public override void FinalizeTile() => Finalizing();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Finalizing()</code></para>
		/// <para>Do the actual finalization</para>
		/// </summary>
		protected new void Finalizing() => base.Finalizing();

		public override void DeleteTile() => Deleting();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Deleting()</code></para>
		/// <para>Do the deletion administration</para>
		/// </summary>
		protected new void Deleting() => base.Deleting();

		//TODO implement electric pole build price
		public override int BuildPrice => 100000;

		//TODO implement electric pole destroy price
		public override int DestroyIncome => 100000;

		public override float Transparency => 1 - (float)(int)Level / 12;
		#endregion

		#region Building implementation

		#endregion

		#region IZoneBuilding implementation
		ZoneType IZoneBuilding.GetZoneType() => ZoneType.IndustrialZone;

		void IZoneBuilding.LevelUp()
		{
			if (!_isFinalized) { throw new InvalidOperationException("Tile is not set in city"); }

			if (Level == ZoneBuildingLevel.ZERO) { return; }
			if (Level == ZoneBuildingLevel.THREE) { return; }
			++Level;
		}

		//TODO implement residential level up cost
		int IZoneBuilding.LevelUpCost => 100000;

		private ZoneBuildingLevel _level = ZoneBuildingLevel.ZERO;
		public ZoneBuildingLevel Level
		{
			get => _level;
			private set
			{
				_level = value;
				WorkplaceLimit = (int)Mathf.Clamp(5 * Mathf.Pow((int)Level, 2), 1, int.MaxValue);
				DesignID = (~INDUSTRIAL_LEVEL_MASK & DesignID) | (INDUSTRIAL_LEVEL_MASK & (uint)_level);
			}
		}
		#endregion

		#region IWorkplace implementation
		private readonly List<Worker> _workers = new();
		private int _workplaceLimit = 0;
		public int WorkplaceLimit
		{
			get => _workplaceLimit;
			private set
			{
				StatEngine.Instance.RegisterIndustrialLevelChange(_workplaceLimit, value);
				_workplaceLimit = value;
			}
		}

		void IWorkplace.Employ(Worker worker)
		{
			if (!_isFinalized) { throw new InvalidOperationException("Not allowed to employ before tile is set"); }
			if (((IWorkplace)this).GetWorkersCount() >= WorkplaceLimit) { throw new InvalidOperationException("The workplace is full"); }

			if (Level == ZoneBuildingLevel.ZERO) { Level = ZoneBuildingLevel.ONE; }

			_workers.Add(worker);
		}

		void IWorkplace.Unemploy(Worker worker)
		{
			if (!_isFinalized) { throw new InvalidOperationException("Not allowed to unemploy before tile is set"); }
			_workers.Remove(worker);

			if (((IWorkplace)this).GetWorkersCount() == 0) { Level = ZoneBuildingLevel.ZERO; }
		}

		List<Worker> IWorkplace.GetWorkers()
		{
			if (!_isFinalized) { throw new InvalidOperationException("Not allowed to get the employers before tile is set"); }
			return _workers;
		}

		int IWorkplace.GetWorkersCount()
		{
			if (!_isFinalized) { throw new InvalidOperationException("Not allowed to get the employers count before tile is set"); }
			return _workers.Count;
		}

		void IWorkplace.RegisterWorkplace(RoadGrid roadGrid)
		{
			if (!_isFinalized) { throw new InvalidOperationException("Not allowed to register workplace at roadgrid before tile is set"); }
			roadGrid?.AddWorkplace(this);
		}

		void IWorkplace.UnregisterWorkplace(RoadGrid roadGrid)
		{
			if (!_isFinalized) { throw new InvalidOperationException("Not allowed to unregister workplace at roadgrid before tile is set"); }
			roadGrid?.RemoveWorkplace(this);
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
			happyZone.GetTile().OnTileDelete += UnregisterHappinessChangerTile;
			happyZone.GetTile().OnTileChange += UpdateHappiness;

			(float happiness, float weight) = happyZone.GetHappinessModifierAtTile(this);
			_happinessChangers.Add((happyZone, happiness, weight));
		}

		private void UnregisterHappinessChangerTile(object sender, Tile deletedTile)
		{
			IHappyZone happyZone = (IHappyZone)deletedTile;
			_happinessChangers.RemoveAll((values) => values.happyZone == happyZone);
		}

		private void UpdateHappiness(object sender, Tile changedTile)
		{
			IHappyZone happyZone = (IHappyZone)changedTile;
			_happinessChangers.RemoveAll((values) => values.happyZone == happyZone);

			(float happiness, float weight) = happyZone.GetHappinessModifierAtTile(this);
			_happinessChangers.Add((happyZone, happiness, weight));
		}
		#endregion

		#region IHappyZone implementation
		int IHappyZone.RegisterRadius => 5;

		int IHappyZone.EffectiveRadius => ((IHappyZone)this).RegisterRadius;

		public (float happiness, float weight) GetHappinessModifierAtTile(Building building)
		{
			if (!_isFinalized) { throw new InvalidOperationException("Tile is not set in the city"); }
			if (building == null) { throw new ArgumentNullException(nameof(building) + " can't be null"); }

			float weight = 1;

			//decrease weight by transparency of the sight
			weight *= IHappyZone.SightToHappyZone(this, building);

			//decrease weight by distance
			weight *= 1 - IHappyZone.DistanceToHappyZone(this, building);

			return (0, weight);
		}

		void IHappyZone.TileDestroyedInRadiusHandler(object sender, Tile oldTile) => IHappyZone.TileDestroyedInRadius(this, oldTile);
		#endregion

		public const uint INDUSTRIAL_LEVEL_MASK = 0x00000003; // 2 bits

		/// <summary>
		/// Construct a new industrial tile
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		/// <param name="rotation">Rotation of the tile</param>
		public Industrial(int x, int y, uint designID, Rotation rotation, ZoneBuildingLevel level) : base(x, y, designID, rotation)
		{
			Level = level;
		}

		/// <summary>
		/// Construct a new industrial tile
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		public Industrial(int x, int y, uint designID) : base(x, y, designID, RoadGridManager.GetRandomRotationToLookAtRoadGridElement(x, y))
		{
			Level = ZoneBuildingLevel.ZERO;
		}
	}
}