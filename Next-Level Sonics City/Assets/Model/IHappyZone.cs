using System.Threading;
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
		/// Get the tile of the happy zone
		/// </summary>
		/// <returns>Tile of the happy zone</returns>
		public Tile GetTile();

		/// <summary>
		/// Register at and to the new tile
		/// </summary>
		/// <param name="oldTile">Old tile that was deletetd</param>
		protected void TileDestroyedInRadiusHandler(object sender, Tile oldTile);

		protected static void RegisterHappinessChangerTileToRegisterRadius(IHappyZone happyZone)
		{
			for (int i = 0; i <= happyZone.RegisterRadius; i++)
			for (int j = 0; new Vector2(i, j).magnitude <= happyZone.RegisterRadius; j++)
			{
				if (i == 0 && j == 0) { continue; }

				//register at the residentials
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x + i, happyZone.GetTile().Coordinates.y - j) is IResidential residentialTopRight)			  { residentialTopRight.RegisterHappinessChangerTile(happyZone);	}
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x + i, happyZone.GetTile().Coordinates.y + j) is IResidential residentialBottomRight && j != 0) { residentialBottomRight.RegisterHappinessChangerTile(happyZone); }
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x - i, happyZone.GetTile().Coordinates.y + j) is IResidential residentialBottomLeft  && j != 0) { residentialBottomLeft.RegisterHappinessChangerTile(happyZone);  }
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x - i, happyZone.GetTile().Coordinates.y - j) is IResidential residentialTopLeft)				  { residentialTopLeft.RegisterHappinessChangerTile(happyZone);		}

				//register at the workplaces
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x + i, happyZone.GetTile().Coordinates.y - j) is IWorkplace workplaceTopRight)			  { workplaceTopRight.RegisterHappinessChangerTile(happyZone);	  }
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x + i, happyZone.GetTile().Coordinates.y + j) is IWorkplace workplaceBottomRight && j != 0) { workplaceBottomRight.RegisterHappinessChangerTile(happyZone); }
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x - i, happyZone.GetTile().Coordinates.y + j) is IWorkplace workplaceBottomLeft && j != 0)  { workplaceBottomLeft.RegisterHappinessChangerTile(happyZone);  }
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x - i, happyZone.GetTile().Coordinates.y - j) is IWorkplace workplaceTopLeft)				  { workplaceTopLeft.RegisterHappinessChangerTile(happyZone);	  }

				//register to the destroy event to be notified about a new tile
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x + i, happyZone.GetTile().Coordinates.y - j) is Tile aboveTile)			  { aboveTile.OnTileDelete  += happyZone.TileDestroyedInRadiusHandler; }
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x + i, happyZone.GetTile().Coordinates.y + j) is Tile rightTile && j != 0)  { rightTile.OnTileDelete  += happyZone.TileDestroyedInRadiusHandler; }
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x - i, happyZone.GetTile().Coordinates.y + j) is Tile bottomTile && j != 0) { bottomTile.OnTileDelete += happyZone.TileDestroyedInRadiusHandler; }
				if (City.Instance.GetTile(happyZone.GetTile().Coordinates.x - i, happyZone.GetTile().Coordinates.y - j) is Tile leftTile)			  { leftTile.OnTileDelete	+= happyZone.TileDestroyedInRadiusHandler; }
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
		protected static float SightToHappyZone(IHappyZone happyZone, Tile tile)
		{
			if (happyZone == tile) { return 0; }

			float sight = 1;

			Vector3 delta = happyZone.GetTile().Coordinates - tile.Coordinates;

			//decrease sight by transparency
			if (delta.x > delta.y) //run on normal function
			{
				for (int i = 1; i < delta.x; i++)
				{
					Tile checkTile = City.Instance.GetTile((happyZone.GetTile().Coordinates.x < tile.Coordinates.x ? (Tile)happyZone : tile).Coordinates.y + i, (happyZone.GetTile().Coordinates.x < tile.Coordinates.x ? (Tile)happyZone : tile).Coordinates.y + Mathf.RoundToInt(i * delta.y / delta.x));
					Debug.Log(checkTile.Coordinates);
					sight *= checkTile.Transparency;
				}
			}
			else //run on inverted function
			{
				for (int i = (int)Mathf.Min(happyZone.GetTile().Coordinates.y, tile.Coordinates.y) + 1; i < Mathf.Max(happyZone.GetTile().Coordinates.y, happyZone.GetTile().Coordinates.y); i++)
				{
					Tile checkTile = City.Instance.GetTile((happyZone.GetTile().Coordinates.y < tile.Coordinates.y ? (Tile)happyZone : tile).Coordinates.x + Mathf.RoundToInt(i * delta.x / delta.y), (happyZone.GetTile().Coordinates.y < tile.Coordinates.y ? (Tile)happyZone : tile).Coordinates.x + i);
					Debug.Log(checkTile.Coordinates);
					sight *= checkTile.Transparency;
				}
			}

			return sight;
		}

		protected static float DistanceToHappyZone(IHappyZone happyZone, Tile tile)
		{
			Vector3 delta = happyZone.GetTile().Coordinates - tile.Coordinates;
			return (delta.magnitude - 1) / happyZone.EffectiveRadius;
		}
		#endregion
	}
}
