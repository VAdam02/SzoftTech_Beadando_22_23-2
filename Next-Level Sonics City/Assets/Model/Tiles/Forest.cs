using Model.Statistics;
using System;
using UnityEngine;

namespace Model.Tiles
{
	public class Forest : Tile, IHappyZone
	{
		private int _plantedYear;

		private const int MAINTANCENEEDEDFORYEAR = 10;

		/// <summary>
		/// Construct a new forest
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		public Forest(int x, int y, uint designID) : base(x, y, designID)
		{
			
		}

		public override TileType GetTileType() { return TileType.Forest; }
		
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
				TileChangeInvoke();
			};

			_plantedYear = StatEngine.Instance.Year;

			for (int i = 0; i <= GetEffectiveRadius(); i++)
			for (int j = 0; Mathf.Sqrt(i * i + j * j) <= GetEffectiveRadius(); j++)
			{
				if (i == 0 && j == 0) { continue; }

				//register at the residentials
				if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y - j) is IResidential residentialTopRight)				{ residentialTopRight.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y + j) is IResidential residentialBottomRight && j != 0)	{ residentialBottomRight.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y + j) is IResidential residentialBottomLeft  && j != 0)	{ residentialBottomLeft.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y - j) is IResidential residentialTopLeft)					{ residentialTopLeft.RegisterHappinessChangerTile(this); }

				//register at the workplaces
				if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y - j) is IWorkplace workplaceTopRight)				{ workplaceTopRight.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y + j) is IWorkplace workplaceBottomRight && j != 0)	{ workplaceBottomRight.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y + j) is IWorkplace workplaceBottomLeft && j != 0)	{ workplaceBottomLeft.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y - j) is IWorkplace workplaceTopLeft)					{ workplaceTopLeft.RegisterHappinessChangerTile(this); }

				//register to the destroy event to be notified about a new tile
				if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y - j) is Tile aboveTile) { aboveTile.OnTileDelete += TileDestroyedInRadius; }
				if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y + j) is Tile rightTile && j != 0) { rightTile.OnTileDelete += TileDestroyedInRadius; }
				if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y + j) is Tile bottomTile && j != 0) { bottomTile.OnTileDelete += TileDestroyedInRadius; }
				if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y - j) is Tile leftTile) { leftTile.OnTileDelete += TileDestroyedInRadius; }
			}
		}

		/// <summary>
		/// Register at and to the new tile
		/// </summary>
		/// <param name="oldTile">Old tile that was deletetd</param>
		private void TileDestroyedInRadius(object sender, Tile oldTile)
		{
			Tile newTile = City.Instance.GetTile(oldTile);

			if (newTile is IResidential residential)	{ residential.RegisterHappinessChangerTile(this); }
			if (newTile is IWorkplace workplace)		{ workplace.RegisterHappinessChangerTile(this);	} //TODO
			newTile.OnTileDelete += TileDestroyedInRadius;
		}

		public override int GetBuildPrice()
		{
			//TODO implement forest build price
			return 100000;
		}

		public override int GetDestroyIncome()
		{
			//TODO implement forest destroy price
			return 100000;
		}

		public override int GetMaintainanceCost()
		{
			//TODO implement forest maintainance cost
			if (_plantedYear + MAINTANCENEEDEDFORYEAR < StatEngine.Instance.Year) { return 0; }
			return 100000;
		}

		public Tile GetTile()
		{
			return this;
		}

		public override float GetTransparency()
		{
			return 1 - Mathf.Sin(Mathf.Clamp(StatEngine.Instance.Year - _plantedYear, 0, 10) * Mathf.PI / 2 / MAINTANCENEEDEDFORYEAR) / 4;
		}

		public int GetEffectiveRadius()
		{
			return 3;
		}

		public (float happiness, float weight) GetHappinessModifierAtTile(Building building)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			if (building == null) { throw new ArgumentNullException(); }

			Vector3 delta = building.Coordinates - Coordinates;

			float weight = Mathf.Sin(Mathf.Clamp(StatEngine.Instance.Year - _plantedYear, 0, 10) * Mathf.PI / 2 / MAINTANCENEEDEDFORYEAR);

			//decrease weight by transparency of the sight
			if (delta.x > delta.y) //run on normal function
			{
				for (int i = (int)Mathf.Min(Coordinates.x, building.Coordinates.x) + 1; i < Mathf.Max(Coordinates.x, building.Coordinates.x); i++)
				{
					Tile checkTile = City.Instance.GetTile(i, (building.Coordinates.x < Coordinates.x ? (Tile)building : this).Coordinates.y + Mathf.RoundToInt(i * delta.y / delta.x));
					weight *= checkTile.GetTransparency();
				}
			}
			else //run on inverted function
			{
				for (int i = (int)Mathf.Min(Coordinates.y, building.Coordinates.y) + 1; i < Mathf.Max(Coordinates.y, building.Coordinates.y); i++)
				{
					Tile checkTile = City.Instance.GetTile((building.Coordinates.y < Coordinates.y ? (Tile)building : this).Coordinates.x + Mathf.RoundToInt(i * delta.x / delta.y), i);
					weight *= checkTile.GetTransparency();
				}
			}

			//decrease weight by distance
			weight *= 1 - ((delta.magnitude - 1) / GetEffectiveRadius());

			return (1, weight);
		}

		public override void DeleteTile() => Deleting();

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Deleting()</code></para>
		/// <para>Do the deletion administration</para>
		/// </summary>
		protected new void Deleting()
		{
			base.Deleting();
		}
	}
}