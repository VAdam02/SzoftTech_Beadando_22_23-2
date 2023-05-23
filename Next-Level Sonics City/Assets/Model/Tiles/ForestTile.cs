using Model.Statistics;
using System;
using UnityEngine;

namespace Model.Tiles
{
	public class ForestTile : Tile, IHappyZone
	{
		#region Tile implementation
		public override TileType GetTileType() => TileType.Forest;

		public override void FinalizeTile() => Finalizing();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Finalizing()</code></para>
		/// <para>Do the actual finalization</para>
		/// </summary>
		protected new void Finalizing()
		{
			base.Finalizing();

			StatEngine.Instance.NextQuarterEvent += (object sender, EventArgs e) =>
			{
				if (StatEngine.Instance.Quarter == 0)
				{
					DesignID = (DesignID & ~DESIGNID_AGE_MASK) | (uint)Mathf.Clamp(Age, 0, MAINTANCENEEDEDFORYEAR);
					TileChangeInvoke();
				}
			};

			_plantedYear = StatEngine.Instance.Year;

			IHappyZone.RegisterHappinessChangerTileToRegisterRadius(this);
		}

		public override void DeleteTile() => Deleting();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Deleting()</code></para>
		/// <para>Do the deletion administration</para>
		/// </summary>
		protected new void Deleting() => base.Deleting();

		//TODO implement forest build price
		public override int BuildPrice => 100000;

		//TODO implement forest destroy price
		public override int DestroyIncome => 100000;

		//TODO implement forest maintainance cost
		public override int MaintainanceCost => (_plantedYear + MAINTANCENEEDEDFORYEAR < StatEngine.Instance.Year) ? 0 : 100000;

		public override float Transparency => 1 - Mathf.Sin(Mathf.Clamp(StatEngine.Instance.Year - _plantedYear, 0, 10) * Mathf.PI / 2 / MAINTANCENEEDEDFORYEAR) / 4;
		#endregion

		#region IHappyZone implementation
		int IHappyZone.RegisterRadius => 3;

		int IHappyZone.EffectiveRadius => ((IHappyZone)this).RegisterRadius;

		public (float happiness, float weight) GetHappinessModifierAtTile(Building building)
		{
			if (!_isFinalized) { throw new InvalidOperationException("Tile is not set in the city"); }
			if (building == null) { throw new ArgumentNullException(nameof(building) + " can't be null"); }

			Vector3 delta = building.Coordinates - Coordinates;

			float weight = Mathf.Sin(Mathf.Clamp(StatEngine.Instance.Year - _plantedYear, 0, 10) * Mathf.PI / 2 / MAINTANCENEEDEDFORYEAR);

			//decrease weight by transparency of the sight
			weight *= IHappyZone.SightToHappyZone(this, building);

			//decrease weight by distance
			weight *= 1 - IHappyZone.DistanceToHappyZone(this, building);

			return (1, weight);
		}

		void IHappyZone.TileDestroyedInRadiusHandler(object sender, Tile oldTile) => IHappyZone.TileDestroyedInRadius(this, oldTile);
		#endregion

		#region Common implementation
		public Tile GetTile() => this;
		#endregion

		private int _plantedYear;

		public const int MAINTANCENEEDEDFORYEAR = 10;
		public const uint DESIGNID_AGE_MASK = 0x00000015; // 4 bits

		public int Age => StatEngine.Instance.Year - _plantedYear;

		/// <summary>
		/// Construct a new forest
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		public ForestTile(int x, int y, uint designID) : base(x, y, designID)
		{
			
		}
	}
}