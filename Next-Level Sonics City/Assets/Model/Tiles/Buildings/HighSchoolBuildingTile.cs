using Model.Persons;
using Model.RoadGrids;
using Model.Tiles.Buildings.BuildingCommands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Tiles.Buildings
{
	public class HighSchoolBuildingTile : Building, IWorkplace
	{
		#region Tile implementation
		public override TileType GetTileType() => TileType.HighSchool;

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

		public override void FinalizeTile() => Finalizing();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Finalizing()</code></para>
		/// <para>Do the actual finalization</para>
		/// </summary>
		protected new void Finalizing()
		{
			base.Finalizing();
			//TODO implement stadion workplace limit
			WorkplaceLimit = 10;
		}

		public override void DeleteTile() => Deleting();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Deleting()</code></para>
		/// <para>Do the deletion administration</para>
		/// </summary>
		protected new void Deleting()
		{
			base.Deleting();
			while (_workers.Count > 0)
			{
				_workers[0].ForcedUnemploy();
			}
		}

		public override int BuildPrice => 30000;

		#endregion

		#region Building implementation
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
		#endregion

		#region IWorkplace implementation
		private readonly List<Worker> _workers = new();
		public int WorkplaceLimit { get; private set; }

		void IWorkplace.Employ(Worker worker)
		{
			if (!_isFinalized) { throw new InvalidOperationException("Not allowed to employ before tile is set"); }
			if (_workers.Count >= WorkplaceLimit) { throw new InvalidOperationException("The workplace is full"); }
			_workers.Add(worker);
		}

		void IWorkplace.Unemploy(Worker worker)
		{
			if (!_isFinalized) { throw new InvalidOperationException("Not allowed to unemploy before tile is set"); }
			_workers.Remove(worker);
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

		private readonly List<(IHappyZone happyZone, float happiness, float weight)> _happinessChangers = new();

		(float happiness, float weight) IWorkplace.HappinessByBuilding
		{
			get
			{
				float happinessSum = _happinessChangers.Aggregate(0.0f, (acc, item) => acc + item.happiness * item.weight);
				float happinessWeight = _happinessChangers.Aggregate(0.0f, (acc, item) => acc + item.weight);
				return (happinessSum / (happinessWeight == 0 ? 1 : happinessWeight), happinessWeight);
			}
		}

		void IWorkplace.RegisterHappinessChangerTile(IHappyZone happyZone)
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

		/// <summary>
		/// Construct a new road tile
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		/// <param name="rotation">Rotation of the tile</param>
		public HighSchoolBuildingTile(int x, int y, uint designID, Rotation rotation) : base(x, y, designID, rotation)
		{

		}
	}
}