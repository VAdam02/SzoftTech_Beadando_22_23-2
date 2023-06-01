using UnityEngine;

namespace View.Tiles
{
	public class ElectricPoleTile : Tile
	{
		private static Material _cabel;
		public static Material Cabel { get { if (_cabel == null) _cabel = LoadMaterialByName("Cabel"); return _cabel; } }
		private Material _sharedCabel;
		private Material SharedCabel
		{
			get
			{
				if (_sharedCabel == null)
				{
					_sharedCabel = new Material(Cabel);
					_materials.Add(_sharedCabel);
				}
				return _sharedCabel;
			}
		}

		private static Material _concrete;
		public static Material Concrete { get { if (_concrete == null) _concrete = LoadMaterialByName("Concrete"); return _concrete; } }
		private Material _sharedConcrete;
		private Material SharedConcrete
		{
			get
			{
				if (_sharedConcrete == null)
				{
					_sharedConcrete = new Material(Concrete);
					_materials.Add(_sharedConcrete);
				}
				return _sharedConcrete;
			}
		}
        private static Material _grassMaterial;
		public static Material GrassMaterial { get { if (_grassMaterial == null) _grassMaterial = LoadMaterialByName("GrassMaterial"); return _grassMaterial; } }
		private Material _sharedGrassMaterial;
		private Material SharedGrassMaterial
		{
			get
			{
				if (_sharedGrassMaterial == null)
				{
					_sharedGrassMaterial = new Material(GrassMaterial);
					_materials.Add(_sharedGrassMaterial);
				}
				return _sharedGrassMaterial;
			}
		}

		private static Material LoadMaterialByName(string name)
		{
			return Resources.Load<Material>("Tiles/ElectricPoleTile/Material/" + name);
		}

		private void SetSharedMaterials(Renderer renderer)
		{
			if (renderer == null) return;

			Material[] materials = renderer.sharedMaterials;

			for (int i = 0; i < materials.Length; i++)
			{
				if		(materials[i].name.Split(' ')[0] == "Cabel")		{ materials[i] = SharedCabel;		}
				else if (materials[i].name.Split(' ')[0] == "Concrete")		{ materials[i] = SharedConcrete;	}
				else if (materials[i].name.Split(' ')[0] == "GrassMaterial")	{ materials[i] = SharedGrassMaterial;	}
				else
				{
					Debug.LogWarning(renderer);
					Debug.LogError("Unknown material found: " + renderer.materials[i].name);
				}
			}

			renderer.sharedMaterials = materials;
		}

		// Start is called before the first frame update
		void Start()
		{
			transform.localPosition = new Vector3(TileModel.Coordinates.x, 0, -TileModel.Coordinates.y) * 10;
			transform.localScale = Vector3.one;

			OnDesignIDChange.AddListener(Display);
			Display();

			SetSharedMaterials(gameObject.GetComponent<Renderer>());
		}

		public override GameObject DisplayPopUp()
        {
            GameObject popup = Instantiate(Resources.Load<GameObject>("Tiles/ElectricPoleTile/ElectricPoleTilePopUp"), GameObject.Find("Canvas").transform);
            popup.GetComponent<PopUpWindow>().TileModel = TileModel;
            return popup;
        }
		private void Display()
		{
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}

			int dirCount = 0;
			if ((TileModel.DesignID & Model.Tiles.ElectricPoleTile.ABOVEPOLEMASK) != 0) { dirCount++; }
			if ((TileModel.DesignID & Model.Tiles.ElectricPoleTile.RIGHTPOLEMASK) != 0) { dirCount++; }
			if ((TileModel.DesignID & Model.Tiles.ElectricPoleTile.BELOWPOLEMASK) != 0) { dirCount++; }
			if ((TileModel.DesignID & Model.Tiles.ElectricPoleTile.LEFTPOLEMASK)  != 0) { dirCount++; }

			GameObject pole;
			Vector3 rotation = new(0, 0, 0);

			if ((TileModel.DesignID & Model.Tiles.ElectricPoleTile.ABOVEPOLEMASK) != 0) { rotation = new(0, 0, 0); }
			else if ((TileModel.DesignID & Model.Tiles.ElectricPoleTile.RIGHTPOLEMASK) != 0) { rotation = new(0, 0, 90); }
			else if ((TileModel.DesignID & Model.Tiles.ElectricPoleTile.BELOWPOLEMASK) != 0) { rotation = new(0, 0, 180); }
			else if ((TileModel.DesignID & Model.Tiles.ElectricPoleTile.LEFTPOLEMASK) != 0) { rotation = new(0, 0, 270); }

			if (dirCount == 0)
			{
				pole = Instantiate(LoadModelByName("0direction"));
			}
			else if (dirCount == 1)
			{
				pole = Instantiate(LoadModelByName("1direction"));
			}
			else if (dirCount == 2)
			{
				if (((TileModel.DesignID & Model.Tiles.ElectricPoleTile.ABOVEPOLEMASK) != 0 &&
					(TileModel.DesignID & Model.Tiles.ElectricPoleTile.BELOWPOLEMASK) != 0) ||
					((TileModel.DesignID & Model.Tiles.ElectricPoleTile.RIGHTPOLEMASK) != 0 &&
					(TileModel.DesignID & Model.Tiles.ElectricPoleTile.LEFTPOLEMASK) != 0))
				{
					pole = Instantiate(LoadModelByName("2direction"));
				}
				else
				{
					pole = Instantiate(LoadModelByName("2directionTurn"));
					if ((TileModel.DesignID & (Model.Tiles.ElectricPoleTile.ABOVEPOLEMASK | Model.Tiles.ElectricPoleTile.BELOWPOLEMASK | Model.Tiles.ElectricPoleTile.RIGHTPOLEMASK | Model.Tiles.ElectricPoleTile.LEFTPOLEMASK) & 0b1001) == 0b1001)
					{
						rotation = new(0, 0, 270);
					}
				}
			}
			else if (dirCount == 3)
			{
				pole = Instantiate(LoadModelByName("3direction"));
				if (TileModel.DesignID == (Model.Tiles.ElectricPoleTile.ABOVEPOLEMASK | Model.Tiles.ElectricPoleTile.RIGHTPOLEMASK | Model.Tiles.ElectricPoleTile.BELOWPOLEMASK))
				{
					rotation = new(0, 0, 90);
				}
				else if (TileModel.DesignID == (Model.Tiles.ElectricPoleTile.RIGHTPOLEMASK | Model.Tiles.ElectricPoleTile.BELOWPOLEMASK | Model.Tiles.ElectricPoleTile.LEFTPOLEMASK))
				{
					rotation = new(0, 0, 180);
				}
				else if (TileModel.DesignID == (Model.Tiles.ElectricPoleTile.BELOWPOLEMASK | Model.Tiles.ElectricPoleTile.LEFTPOLEMASK | Model.Tiles.ElectricPoleTile.ABOVEPOLEMASK))
				{
					rotation = new(0, 0, 270);
				}
				else if (TileModel.DesignID == (Model.Tiles.ElectricPoleTile.LEFTPOLEMASK | Model.Tiles.ElectricPoleTile.ABOVEPOLEMASK | Model.Tiles.ElectricPoleTile.RIGHTPOLEMASK))
				{
					rotation = new(0, 0, 0);
				}
			}
			else
			{
				pole = Instantiate(LoadModelByName("4direction"));
			}

			pole.transform.parent = transform;
			pole.transform.localScale = Vector3.one * 20;
			pole.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(-90, 0, 0));
			pole.transform.Rotate(rotation);

			SetSharedMaterials(pole.GetComponent<Renderer>());
		}

		private static GameObject LoadModelByName(string name)
		{
			return Resources.Load<GameObject>("Tiles/ElectricPoleTile/Model/" + name);
		}
	}
}