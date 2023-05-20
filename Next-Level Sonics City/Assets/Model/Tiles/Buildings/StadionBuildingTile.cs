using Model.Persons;
using Model.RoadGrids;
using Model.Tiles.Buildings.BuildingCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model.Tiles.Buildings
{
	public class StadionBuildingTile : Building, IWorkplace, IHappyZone
	{
		private readonly List<Worker> _workers = new();
		public int WorkplaceLimit { get; private set; }

		public StadionBuildingTile(int x, int y, uint designID, Rotation rotation) : base(x, y, designID, rotation)
		{

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
			//TODO implement stadion workplace limit
			WorkplaceLimit = 10;

			for (int i = 0; i <= GetRegisterRadius(); i++)
				for (int j = 0; Mathf.Sqrt(i * i + j * j) <= GetRegisterRadius(); j++)
				{
					if (i == 0 && j == 0) { continue; }

					//register at the residentials
					if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y - j) is IResidential residentialTopRight) { residentialTopRight.RegisterHappinessChangerTile(this); }
					if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y + j) is IResidential residentialBottomRight && j != 0) { residentialBottomRight.RegisterHappinessChangerTile(this); }
					if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y + j) is IResidential residentialBottomLeft && j != 0) { residentialBottomLeft.RegisterHappinessChangerTile(this); }
					if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y - j) is IResidential residentialTopLeft) { residentialTopLeft.RegisterHappinessChangerTile(this); }

					//register at the workplaces
					if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y - j) is IWorkplace workplaceTopRight) { workplaceTopRight.RegisterHappinessChangerTile(this); }
					if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y + j) is IWorkplace workplaceBottomRight && j != 0) { workplaceBottomRight.RegisterHappinessChangerTile(this); }
					if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y + j) is IWorkplace workplaceBottomLeft && j != 0) { workplaceBottomLeft.RegisterHappinessChangerTile(this); }
					if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y - j) is IWorkplace workplaceTopLeft) { workplaceTopLeft.RegisterHappinessChangerTile(this); }

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
			if (newTile is IWorkplace workplace) { workplace.RegisterHappinessChangerTile(this); } //TODO
			newTile.OnTileDelete.AddListener(TileDestroyedInRadius);
		}

		public override TileType GetTileType() { return TileType.Stadion; }

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

		public void Employ(Worker worker)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			if (_workers.Count >= WorkplaceLimit) { throw new InvalidOperationException("The workplace is full"); }
			_workers.Add(worker);
		}

		public void Unemploy(Worker worker)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			_workers.Remove(worker);
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
			//TODO implement stadion build price
			return 100000;
		}

		public override int GetDestroyIncome()
		{
			//TODO implement stadion destroy income
			return 100000;
		}

		public override int GetMaintainanceCost()
		{
			//TODO implement stadion maintainance cost
			return 100000;
		}

		public override bool CanBuild()
		{
			int x1 = (int)Coordinates.x;
			int y1 = (int)Coordinates.y;
			int x2 = (int)Coordinates.x;
			int y2 = (int)Coordinates.y;


			switch (Rotation)
			{
				case Rotation.Zero:
					x1 += 1; y1 += 1;
					break;
				case Rotation.Ninety:
					x1 += -1; y1 += 1;
					break;
				case Rotation.OneEighty:
					x1 += -1; y1 += 1;
					break;
				case Rotation.TwoSeventy:
					x1 += 1; y1 += -1;
					break;
			}

			int minX = Math.Min(x1, x2);
			int maxX = Math.Max(x1, x2);
			int minY = Math.Min(y1, y2);
			int maxY = Math.Max(y1, y2);

			for (int i = minX; i <= maxX; ++i)
			{
				for (int j = minY; j <= maxY; ++j)
				{
					if (City.Instance.GetTile(i, j) is not EmptyTile)
					{
						return false;
					}
				}
			}

			return true;
		}

		internal override void Expand()
		{
			int x1 = (int)Coordinates.x;
			int y1 = (int)Coordinates.y;
			int x2 = (int)Coordinates.x;
			int y2 = (int)Coordinates.y;

			switch (Rotation)
			{
				case Rotation.Zero:
					x1 += 1; y1 += 1;
					break;
				case Rotation.Ninety:
					x1 += -1; y1 += 1;
					break;
				case Rotation.OneEighty:
					x1 += -1; y1 += 1;
					break;
				case Rotation.TwoSeventy:
					x1 += 1; y1 += -1;
					break;
			}

			int minX = Math.Min(x1, x2);
			int maxX = Math.Max(x1, x2);
			int minY = Math.Min(y1, y2);
			int maxY = Math.Max(y1, y2);

			for (int i = minX; i <= maxX; ++i)
			{
				for (int j = minY; j <= maxY; ++j)
				{
					if (i == (int)Coordinates.x && j == (int)Coordinates.y) { continue; }
					ExpandCommand ec = new(i, j, this);
					ec.Execute();
				}
			}
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
			return 0.75f;
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

			float weight = 1;

			//decrease weight by distance
			weight *= 1 - ((delta.magnitude - 1) / GetEffectiveRadius());

			return (0, weight);
		}
	}
}