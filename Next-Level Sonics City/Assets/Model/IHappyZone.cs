using Model.RoadGrids;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Tiles
{
	public interface IHappyZone
	{
		/// <summary>
		/// Max radius of the happy zone
		/// </summary>
		public int RegisterRadius { get; }

		/// <summary>
		/// Radius of the happy zone
		/// </summary>
		public int EffectiveRadius { get; }

		/// <summary>
		/// Get the happiness at the given building
		/// </summary>
		/// <param name="building">Building for reference where to calculate the effect</param>
		/// <returns>Value of the happiness modifier made by this</returns>
		public (float happiness, float weight) GetHappinessModifierAtTile(Building building);

		/// <summary>
		/// Register at and to the new tile
		/// </summary>
		/// <param name="oldTile">Old tile that was deletetd</param>
		protected void TileDestroyedInRadiusHandler(object sender, Tile oldTile);

		/// <summary>
		/// Get the tile of the happy zone
		/// </summary>
		/// <returns>Tile of the happy zone</returns>
		public Tile GetTile();

		protected static void RegisterHappinessChangerTileToRegisterRadius(IHappyZone happyZone)
		{
			for (int i = 0; i <= happyZone.RegisterRadius; i++)
			for (int j = 0; new Vector2(i, j).magnitude <= happyZone.RegisterRadius; j++)
			{
				if (i == 0 && j == 0) { continue; }

				//register at the residentials
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x + i, happyZone.GetTile().Coordinates.y - j) is IResidential residentialTopRight    && i != 0)			{ residentialTopRight.RegisterHappinessChangerTile(happyZone);	}
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x + i, happyZone.GetTile().Coordinates.y + j) is IResidential residentialBottomRight && i != 0 && j != 0)	{ residentialBottomRight.RegisterHappinessChangerTile(happyZone); }
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x - i, happyZone.GetTile().Coordinates.y + j) is IResidential residentialBottomLeft  && j != 0)			{ residentialBottomLeft.RegisterHappinessChangerTile(happyZone);  }
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x - i, happyZone.GetTile().Coordinates.y - j) is IResidential residentialTopLeft)							{ residentialTopLeft.RegisterHappinessChangerTile(happyZone);		}

				//register at the workplaces
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x + i, happyZone.GetTile().Coordinates.y - j) is IWorkplace workplaceTopRight		&& i != 0)				{ workplaceTopRight.RegisterHappinessChangerTile(happyZone);	  }
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x + i, happyZone.GetTile().Coordinates.y + j) is IWorkplace workplaceBottomRight	&& i != 0 && j != 0)	{ workplaceBottomRight.RegisterHappinessChangerTile(happyZone); }
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x - i, happyZone.GetTile().Coordinates.y + j) is IWorkplace workplaceBottomLeft	&& j != 0)				{ workplaceBottomLeft.RegisterHappinessChangerTile(happyZone);  }
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x - i, happyZone.GetTile().Coordinates.y - j) is IWorkplace workplaceTopLeft)								{ workplaceTopLeft.RegisterHappinessChangerTile(happyZone);	  }

				//register to the destroy event to be notified about a new tile
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x + i, happyZone.GetTile().Coordinates.y - j) is Tile aboveTile	&& i != 0)				{ aboveTile.OnTileDelete  += happyZone.TileDestroyedInRadiusHandler; }
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x + i, happyZone.GetTile().Coordinates.y + j) is Tile rightTile	&& i != 0 && j != 0)	{ rightTile.OnTileDelete  += happyZone.TileDestroyedInRadiusHandler; }
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x - i, happyZone.GetTile().Coordinates.y + j) is Tile bottomTile	&& j != 0)				{ bottomTile.OnTileDelete += happyZone.TileDestroyedInRadiusHandler; }
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x - i, happyZone.GetTile().Coordinates.y - j) is Tile leftTile)							{ leftTile.OnTileDelete   += happyZone.TileDestroyedInRadiusHandler; }
			}
		}

		protected static void TileDestroyedInRadius(IHappyZone happyZone, Tile oldTile)
		{
			Tile newTile = City.Instance.GetTile(oldTile);

			if (newTile is IResidential residential) { residential.RegisterHappinessChangerTile(happyZone); }
			if (newTile is IWorkplace workplace)	 { workplace.RegisterHappinessChangerTile(happyZone);   }
			newTile.OnTileDelete += happyZone.TileDestroyedInRadiusHandler;
		}

		#region HappinessModifier modules
		/// <summary>
		/// Get the happiness modifier for the given tile based on sight
		/// </summary>
		/// <param name="happyZone">HappyZone where the ray finish</param>
		/// <param name="tile">Tile where from the ray start</param>
		/// <returns>Value of visibility</returns>
		protected static float SightToHappyZone(IHappyZone happyZone, Tile tile)
		{
			float sight = 1;

			Vector3 delta = happyZone.GetTile().Coordinates - tile.Coordinates;
			Vector3 absDelta = new(Mathf.Abs(delta.x), Mathf.Abs(delta.y), 0);

			if (absDelta.x > absDelta.y)
			{
				for (int i = 1; i < absDelta.x; i++)
				{
					float t = i / absDelta.x;
					Vector3 position = Vector3.Lerp(happyZone.GetTile().Coordinates, tile.Coordinates, t);
					sight *= City.Instance.GetTile(position).Transparency;
				}
			}
			else //run on inverted
			{
				for (int i = 1; i < absDelta.y; i++)
				{
					float t = i / absDelta.y;
					Vector3 position = Vector3.Lerp(happyZone.GetTile().Coordinates, tile.Coordinates, t);
					sight *= City.Instance.GetTile(position).Transparency;
				}
			}

			return sight;
		}

		/// <summary>
		/// Get the happiness modifier for the given tile based on distance in air
		/// </summary>
		/// <param name="happyZone">HappyZone where the ray finish</param>
		/// <param name="tile">Tile where from the ray start</param>
		/// <returns>Value of distance</returns>
		protected static float DistanceToHappyZone(IHappyZone happyZone, Tile tile)
		{
			Vector3 delta = happyZone.GetTile().Coordinates - tile.Coordinates;
			return (delta.magnitude - 1) / happyZone.EffectiveRadius;
		}
		#endregion
	}
}
