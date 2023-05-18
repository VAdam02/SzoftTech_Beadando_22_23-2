using Model.Statistics;
using System;
using UnityEngine;

namespace Model.Tiles
{
	public class Forest : Tile, IHappyZone, ITransparent
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
			_plantedYear = StatEngine.Instance.Year;

			for (int i = 0; i <= GetEffectiveRadius(); i++)
			for (int j = 0; Mathf.Sqrt(i * i + j * j) <= GetEffectiveRadius(); j++)
			{
				if (i == 0 && j == 0) { continue; }

				//register at the residentials
				if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y - j) is IResidential residentialTopRight)	{ residentialTopRight.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y + j) is IResidential residentialBottomRight)	{ residentialBottomRight.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y + j) is IResidential residentialBottomLeft)	{ residentialBottomLeft.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y - j) is IResidential residentialTopLeft)		{ residentialTopLeft.RegisterHappinessChangerTile(this); }

				//register at the workplaces
				if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y - j) is IWorkplace workplaceTopRight)	{ workplaceTopRight.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x + i, Coordinates.y + j) is IWorkplace workplaceBottomRight)	{ workplaceBottomRight.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y + j) is IWorkplace workplaceBottomLeft)	{ workplaceBottomLeft.RegisterHappinessChangerTile(this); }
				if (City.Instance.GetTile(Coordinates.x - i, Coordinates.y - j) is IWorkplace workplaceTopLeft)		{ workplaceTopLeft.RegisterHappinessChangerTile(this); }

				//register to the destroy event to be notified about a new tile
				City.Instance.GetTile(Coordinates.x + i, Coordinates.y - j).OnTileDelete.AddListener(TileDestroyedInRadius);
				City.Instance.GetTile(Coordinates.x + i, Coordinates.y + j).OnTileDelete.AddListener(TileDestroyedInRadius);
				City.Instance.GetTile(Coordinates.x - i, Coordinates.y + j).OnTileDelete.AddListener(TileDestroyedInRadius);
				City.Instance.GetTile(Coordinates.x - i, Coordinates.y - j).OnTileDelete.AddListener(TileDestroyedInRadius);
			}
		}

		/// <summary>
		/// Register at and to the new tile
		/// </summary>
		/// <param name="oldTile">Old tile that was deletetd</param>
		private void TileDestroyedInRadius(Tile oldTile)
		{
			Tile newTile = City.Instance.GetTile(oldTile.Coordinates);

			if (newTile is IResidential residential)	{ residential.RegisterHappinessChangerTile(this); }
			if (newTile is IWorkplace workplace)		{ workplace.RegisterHappinessChangerTile(this);	}
			newTile.OnTileDelete.AddListener(TileDestroyedInRadius);
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

		public float GetTransparency()
		{
			return Mathf.Sin((StatEngine.Instance.Year - _plantedYear) * Mathf.PI / 2 / MAINTANCENEEDEDFORYEAR);
		}

		public int GetEffectiveRadius()
		{
			return 3;
		}

		public (float happiness, float weight) GetHappinessModifierAtTile(Tile tile)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			if (tile == null) { throw new ArgumentNullException(); }
			if (tile == this) { throw new ArgumentException("Target can't be same as this"); }

			Vector3 delta = tile.Coordinates - Coordinates;

			float weight = 1;

			//decrease weight by transparency
			if (delta.x > delta.y) //run on normal function
			{
				for (int i = (int)Mathf.Min(Coordinates.x, tile.Coordinates.x) + 1; i <= Mathf.Max(Coordinates.x, tile.Coordinates.x) - 1; i++)
				{
					Tile checkTile = City.Instance.GetTile(i, Mathf.RoundToInt(i * delta.y / delta.x));

					if (checkTile is ITransparent transparent) { weight *= transparent.GetTransparency(); }
					else { return (0, 0); }
				}
			}
			else //run on inverted function
			{
				for (int i = (int)Mathf.Min(Coordinates.y, tile.Coordinates.y) + 1; i <= Mathf.Max(Coordinates.y, tile.Coordinates.y) - 1; i++)
				{
					Tile checkTile = City.Instance.GetTile(Mathf.RoundToInt(i * delta.x / delta.y), i);

					if (checkTile is ITransparent transparent) { weight *= transparent.GetTransparency(); }
					else { return (0, 0); }
				}
			}

			//decrease weight by distance
			weight *= 1 - (Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y) / GetEffectiveRadius());

			return (1, weight);
		}
	}
}