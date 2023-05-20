using Model.Persons;
using Model.RoadGrids;
using Model.Statistics;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Tiles.Buildings
{
	public class Industrial : Building, IWorkplace, IZoneBuilding, IHappyZone
	{
		public ZoneBuildingLevel Level { get; private set; } = 0;
		private readonly List<Worker> _workers = new();
		public int WorkplaceLimit { get; private set; }

		/// <summary>
		/// Construct a new industrial tile
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		/// <param name="rotation">Rotation of the tile</param>
		public Industrial(int x, int y, uint designID, Rotation rotation) : base(x, y, designID, rotation)
		{

		}

		/// <summary>
		/// Construct a new industrial tile
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		public Industrial(int x, int y, uint designID) : base(x, y, designID, GetRandomRotationToLookAtRoadGridElement(x, y))
		{

		}

		private static Rotation GetRandomRotationToLookAtRoadGridElement(int x, int y)
		{
			List<(IRoadGridElement roadGridElement, Rotation rotation)> roadGridElements = RoadGridManager.GetRoadGridElementsAroundTile(City.Instance.GetTile(x, y));
			if (roadGridElements.Count == 0) { throw new InvalidOperationException("No road grid elements around tile"); }
			System.Random rnd = new();
			return roadGridElements[rnd.Next(roadGridElements.Count)].rotation;
		}

		public override void FinalizeTile()
		{
			Finalizing();
		}

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Finalizing()</code></para>
		/// <para>Do the actual finalization</para>
		/// </summary>
		protected new void Finalizing()
		{
			base.Finalizing();
			//TODO implement industrial workplace limit
			WorkplaceLimit = 10;

			for (int i = 0; i <= GetRegisterRadius(); i++)
			for (int j = 0; Mathf.Sqrt(i * i + j * j) <= GetRegisterRadius(); j++)
			{
				if (i == 0 && j == 0) { continue; }

				//register at the residentials
				if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y - j) is IResidential residentialTopRight)				{ residentialTopRight.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y + j) is IResidential residentialBottomRight && j != 0)	{ residentialBottomRight.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y + j) is IResidential residentialBottomLeft && j != 0)	{ residentialBottomLeft.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y - j) is IResidential residentialTopLeft)					{ residentialTopLeft.RegisterHappinessChangerTile(this); }

				//register at the workplaces //TODO
				/*
				if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y - j) is IWorkplace workplaceTopRight)	{ workplaceTopRight.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y + j) is IWorkplace workplaceBottomRight)	{ workplaceBottomRight.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y + j) is IWorkplace workplaceBottomLeft)	{ workplaceBottomLeft.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y - j) is IWorkplace workplaceTopLeft)		{ workplaceTopLeft.RegisterHappinessChangerTile(this); }
				*/

				//register to the destroy event to be notified about a new tile
				City.Instance.GetTile(Coordinates.x + i, Coordinates.y - j)?.OnTileDelete.AddListener(TileDestroyedInRadius);
				if (j != 0) City.Instance.GetTile(Coordinates.x + i, Coordinates.y + j)?.OnTileDelete.AddListener(TileDestroyedInRadius);
				if (j != 0) City.Instance.GetTile(Coordinates.x - i, Coordinates.y + j)?.OnTileDelete.AddListener(TileDestroyedInRadius);
				City.Instance.GetTile(Coordinates.x - i, Coordinates.y - j)?.OnTileDelete.AddListener(TileDestroyedInRadius);
			}
		}

		/// <summary>
		/// Register at and to the new tile
		/// </summary>
		/// <param name="oldTile">Old tile that was deletetd</param>
		private void TileDestroyedInRadius(Tile oldTile)
		{
			Tile newTile = City.Instance.GetTile(oldTile.Coordinates);

			if (newTile is IResidential residential) { residential.RegisterHappinessChangerTile(this); }
			//if (newTile is IWorkplace workplace)		{ workplace.RegisterHappinessChangerTile(this);	} //TODO
			newTile.OnTileDelete.AddListener(TileDestroyedInRadius);
		}

		public override TileType GetTileType() { throw new InvalidOperationException(); }

		public void RegisterWorkplace(RoadGrid roadGrid)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			roadGrid?.AddWorkplace(this);
		}

		public void UnregisterWorkplace(RoadGrid roadGrid)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			roadGrid?.RemoveWorkplace(this);
		}

		public ZoneType GetZoneType()
		{
			return ZoneType.IndustrialZone;
		}

		public void LevelUp()
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			//TODO implement industrial level up workplace amount
			if (Level == ZoneBuildingLevel.ZERO) { return; }
			if (Level == ZoneBuildingLevel.THREE) { return; }
			StatEngine.Instance.RegisterIndustrialLevelChange(WorkplaceLimit, WorkplaceLimit + 5);
			++Level;
			WorkplaceLimit += 5;
		}

		public int GetLevelUpCost()
		{
			//TODO implement industrial level up cost
			return 100000;
		}

		public void Employ(Worker worker)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			if (_workers.Count >= WorkplaceLimit) { throw new InvalidOperationException("The workplace is full"); }

			if (Level == ZoneBuildingLevel.ZERO) { Level = ZoneBuildingLevel.ONE; }

			if (GetWorkersCount() == 0)
			{
				if (MainThreadDispatcher.Instance is MainThreadDispatcher mainThread)
				{
					mainThread.Enqueue(() =>
					{
						OnTileChange.Invoke(this);
					});
				}
			}

			_workers.Add(worker);
		}

		public void Unemploy(Worker worker)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			_workers.Remove(worker);

			if (GetWorkersCount() == 0)
			{
				if (MainThreadDispatcher.Instance is MainThreadDispatcher mainThread)
				{
					mainThread.Enqueue(() =>
					{
						OnTileChange.Invoke(this);
					});
				}
			}
		}

		public List<Worker> GetWorkers()
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			return _workers;
		}

		public int GetWorkersCount()
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			return _workers.Count;
		}

		public override int GetBuildPrice()
		{
			//TODO implement industrial building price
			return 100000;
		}

		public override int GetDestroyIncome()
		{
			//TODO implement industrial destroy income
			return 100000;
		}

		private int GetRegisterRadius()
		{
			return 5;
		}

		public int GetEffectiveRadius()
		{
			return GetWorkersCount() > 0 ? GetRegisterRadius() : 0;
		}

		public (float happiness, float weight) GetHappinessModifierAtTile(Building building)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			if (building == null) { throw new ArgumentNullException(); }

			Vector3 delta = building.Coordinates - Coordinates;

			float weight = (int)Level; //saddness weight is made by the level of the building

			//decrease weight by transparency of the sight
			if (delta.x > delta.y) //run on normal function
			{
				for (int i = (int)Mathf.Min(Coordinates.x, building.Coordinates.x) + 1; i <= Mathf.Max(Coordinates.x, building.Coordinates.x) - 1; i++)
				{
					Tile checkTile = City.Instance.GetTile(i, i + Mathf.RoundToInt(i * delta.y / delta.x));

					weight *= checkTile.GetTransparency();
				}
			}
			else //run on inverted function
			{
				for (int i = (int)Mathf.Min(Coordinates.y, building.Coordinates.y) + 1; i <= Mathf.Max(Coordinates.y, building.Coordinates.y) - 1; i++)
				{
					Tile checkTile = City.Instance.GetTile(i + Mathf.RoundToInt(i * delta.x / delta.y), i);

					weight *= checkTile.GetTransparency();
				}
			}

			//decrease weight by distance
			weight *= 1 - ((Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y) - 1) / GetEffectiveRadius());

			return (0, weight);
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
			return 1 - (float)(int)Level / 12;
		}
	}
}