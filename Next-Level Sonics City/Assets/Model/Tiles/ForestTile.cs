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

			_plantedYear = (int)(StatEngine.Instance.Year - (DesignID & DESIGNID_AGE_MASK));

			StatEngine.Instance.NextQuarterEvent += (object sender, EventArgs e) =>
			{
				if (StatEngine.Instance.Quarter == 3 && Age <= MAINTANCENEEDEDFORYEAR)
				{
					DesignID = (DesignID & ~DESIGNID_AGE_MASK) | (uint)Mathf.Clamp(Age, 0, MAINTANCENEEDEDFORYEAR);
					TileChangeInvoke();
				}
			};

			IHappyZone.RegisterHappinessChangerTileToRegisterRadius(this);
		}

		public override void DeleteTile() => Deleting();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Deleting()</code></para>
		/// <para>Do the deletion administration</para>
		/// </summary>
		protected new void Deleting() => base.Deleting();

		public override int BuildPrice => 3000;

		public override int MaintainanceCost => (_plantedYear + MAINTANCENEEDEDFORYEAR < StatEngine.Instance.Year) ? 0 : BuildPrice / 5;

		public override float Transparency => 1 - Mathf.Sin(Mathf.Clamp(StatEngine.Instance.Year - _plantedYear, 0, 10) * Mathf.PI / 2 / MAINTANCENEEDEDFORYEAR) / 4;
		#endregion

		#region IHappyZone implementation
		int IHappyZone.RegisterRadius => 3;

		int IHappyZone.EffectiveRadius => ((IHappyZone)this).RegisterRadius;

		public (float happiness, float weight) GetHappinessModifierAtTile(Building building)
		{
			if (!_isFinalized) { throw new InvalidOperationException("Tile is not set in the city"); }
			if (building == null) { throw new ArgumentNullException(nameof(building) + " can't be null"); }

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
		public const uint DESIGNID_AGE_MASK = 0x000000F; // 4 bits

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