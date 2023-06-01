using Model.Tiles;
using Model.Tiles.Buildings;
using UnityEngine;

namespace View.Tiles.Buildings
{
	public class UpgradedRoadTile : Tile
	{
		private static Material _cabelmaterial;
		public static Material CabelMaterial { get { if (_cabelmaterial == null) _cabelmaterial = LoadMaterialByName("CabelMaterial"); return _cabelmaterial; } }
		private Material _sharedCabelMaterial;
		private Material SharedCabelMaterial
		{
			get
			{
				if (_sharedCabelMaterial == null)
				{
					_sharedCabelMaterial = new Material(CabelMaterial);
					_materials.Add(_sharedCabelMaterial);
				}
				return _sharedCabelMaterial;
			}
		}

		private static Material _polematerial;
		public static Material PoleMaterial { get { if (_polematerial == null) _polematerial = LoadMaterialByName("PoleMaterial"); return _polematerial; } }
		private Material _sharedPoleMaterial;
		private Material SharedPoleMaterial
		{
			get
			{
				if (_sharedPoleMaterial == null)
				{
					_sharedPoleMaterial = new Material(PoleMaterial);
					_materials.Add(_sharedPoleMaterial);
				}
				return _sharedPoleMaterial;
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
            GameObject popup = Instantiate(Resources.Load<GameObject>("Tiles/UpgradedRoadTile/UpgradedRoadTilePopUp"), GameObject.Find("Canvas").transform);
            popup.GetComponent<PopUpWindow>().TileModel = TileModel;
            return popup;
        }

		// Update is called once per frame
		void Update()
		{

		}

		private static Material LoadMaterialByName(string name)
		{
			return Resources.Load<Material>("Tiles/UpgradedRoadTile/Material/" + name);
		}

		private void SetSharedMaterials(Renderer renderer)
		{
			if (renderer == null) return;

			Material[] materials = renderer.sharedMaterials;

			for (int i = 0; i < materials.Length; i++)
			{
				if		(materials[i].name.Split(' ')[0] == "CabelMaterial")		{ materials[i] = SharedCabelMaterial;  }
				else if (materials[i].name.Split(' ')[0] == "PoleMaterial")	{ materials[i] = SharedPoleMaterial; }
                else if (materials[i].name.Split(' ')[0] == "GrassMaterial")		{ materials[i] = SharedGrassMaterial;   }
				else
				{
					Debug.LogWarning(renderer);
					Debug.LogError("Unknown material found: " + renderer.materials[i].name);
				}
			}

			renderer.sharedMaterials = materials;
		}

		private void Display()
		{
			foreach (Transform child in transform)
			{
				if (child.name == "RoadUpgrade") continue;
				Destroy(child.gameObject);
			}

			int dirCount = 0;
			if ((TileModel.DesignID & Model.Tiles.RoadTile.ABOVEROADMASK) != 0) { dirCount++; }
			if ((TileModel.DesignID & Model.Tiles.RoadTile.RIGHTROADMASK) != 0) { dirCount++; }
			if ((TileModel.DesignID & Model.Tiles.RoadTile.BELOWROADMASK) != 0) { dirCount++; }
			if ((TileModel.DesignID & Model.Tiles.RoadTile.LEFTROADMASK) != 0) { dirCount++; }

			GameObject road;
			Vector3 rotation = new(0, 0, 0);

			if ((TileModel.DesignID & Model.Tiles.RoadTile.ABOVEROADMASK) != 0) { rotation = new(0, 0, 0); }
			else if ((TileModel.DesignID & Model.Tiles.RoadTile.RIGHTROADMASK) != 0) { rotation = new(0, 0, 90); }
			else if ((TileModel.DesignID & Model.Tiles.RoadTile.BELOWROADMASK) != 0) { rotation = new(0, 0, 180); }
			else if ((TileModel.DesignID & Model.Tiles.RoadTile.LEFTROADMASK) != 0) { rotation = new(0, 0, 270); }

			if (dirCount == 0)
			{
				road = Instantiate(LoadModelByName("0direction"));
			}
			else if (dirCount == 1)
			{
				road = Instantiate(LoadModelByName("1direction"));
			}
			else if (dirCount == 2)
			{
				if (((TileModel.DesignID & Model.Tiles.RoadTile.ABOVEROADMASK) != 0 &&
					(TileModel.DesignID & Model.Tiles.RoadTile.BELOWROADMASK) != 0) ||
					((TileModel.DesignID & Model.Tiles.RoadTile.RIGHTROADMASK) != 0 &&
					(TileModel.DesignID & Model.Tiles.RoadTile.LEFTROADMASK) != 0))
				{
					road = Instantiate(LoadModelByName("2direction"));
				}
				else
				{
					road = Instantiate(LoadModelByName("2directionTurn"));
					if ((TileModel.DesignID & (Model.Tiles.RoadTile.ABOVEROADMASK | Model.Tiles.RoadTile.BELOWROADMASK | Model.Tiles.RoadTile.RIGHTROADMASK | Model.Tiles.RoadTile.LEFTROADMASK) & 0b1001) == 0b1001)
					{
						rotation = new(0, 0, 270);
					}
				}
			}
			else if (dirCount == 3)
			{
				road = Instantiate(LoadModelByName("3direction"));
				if (TileModel.DesignID == (Model.Tiles.RoadTile.ABOVEROADMASK | Model.Tiles.RoadTile.RIGHTROADMASK | Model.Tiles.RoadTile.BELOWROADMASK))
				{
					rotation = new(0, 0, 90);
				}
				else if (TileModel.DesignID == (Model.Tiles.RoadTile.RIGHTROADMASK | Model.Tiles.RoadTile.BELOWROADMASK | Model.Tiles.RoadTile.LEFTROADMASK))
				{
					rotation = new(0, 0, 180);
				}
				else if (TileModel.DesignID == (Model.Tiles.RoadTile.BELOWROADMASK | Model.Tiles.RoadTile.LEFTROADMASK | Model.Tiles.RoadTile.ABOVEROADMASK))
				{
					rotation = new(0, 0, 270);
				}
				else if (TileModel.DesignID == (Model.Tiles.RoadTile.LEFTROADMASK | Model.Tiles.RoadTile.ABOVEROADMASK | Model.Tiles.RoadTile.RIGHTROADMASK))
				{
					rotation = new(0, 0, 0);
				}
			}
			else
			{
				road = Instantiate(LoadModelByName("4direction"));
			}

			road.transform.parent = transform;
			road.transform.localScale = Vector3.one * 20;
			road.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(-90, 0, 0));
			road.transform.Rotate(rotation);

			SetSharedMaterials(road.GetComponent<Renderer>());
		}

		private static GameObject LoadModelByName(string name)
		{
			return Resources.Load<GameObject>("Tiles/RoadTile/Model/" + name);
		}
	}
}
