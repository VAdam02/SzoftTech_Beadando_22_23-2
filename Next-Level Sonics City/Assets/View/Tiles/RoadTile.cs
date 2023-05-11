using UnityEngine;

namespace View.Tiles
{
	public class RoadTile : Tile
	{
		private static Material _asphaltMaterial;
		public static Material AsphaltMaterial { get { if (_asphaltMaterial == null) _asphaltMaterial = LoadMaterialByName("AsphaltMaterial"); return _asphaltMaterial; } }
		private Material _sharedAsphaltMaterial;
		private Material SharedAsphaltMaterial
		{
			get
			{
				if (_sharedAsphaltMaterial == null)
				{
					_sharedAsphaltMaterial = new Material(AsphaltMaterial);
					_materials.Add(_sharedAsphaltMaterial);
				}
				return _sharedAsphaltMaterial;
			}
		}

		private static Material _sidewalkMaterial;
		public static Material SidewalkMaterial { get { if (_sidewalkMaterial == null) _sidewalkMaterial = LoadMaterialByName("SidewalkMaterial"); return _sidewalkMaterial; } }
		private Material _sharedSidewalkMaterial;
		private Material SharedSidewalkMaterial
		{
			get
			{
				if (_sharedSidewalkMaterial == null)
				{
					_sharedSidewalkMaterial = new Material(SidewalkMaterial);
					_materials.Add(_sharedSidewalkMaterial);
				}
				return _sharedSidewalkMaterial;
			}
		}

		private static Material _whitelineMaterial;
		public static Material WhitelineMaterial { get { if (_whitelineMaterial == null) _whitelineMaterial = LoadMaterialByName("WhitelineMaterial"); return _whitelineMaterial; } }
		private Material _sharedWhitelineMaterial;
		private Material SharedWhitelineMaterial
		{
			get
			{
				if (_sharedWhitelineMaterial == null)
				{
					_sharedWhitelineMaterial = new Material(WhitelineMaterial);
					_materials.Add(_sharedWhitelineMaterial);
				}
				return _sharedWhitelineMaterial;
			}
		}

		private static Material LoadMaterialByName(string name)
		{
			return Resources.Load<Material>("Tiles/RoadTile/Material/" + name);
		}

		private void SetSharedMaterials(Renderer renderer)
		{
			if (renderer == null) return;

			Material[] materials = renderer.sharedMaterials;

			for (int i = 0; i < materials.Length; i++)
			{
				if		(materials[i].name.Split(' ')[0] == "AsphaltMaterial")		{ materials[i] = SharedAsphaltMaterial;		}
				else if (materials[i].name.Split(' ')[0] == "SidewalkMaterial")		{ materials[i] = SharedSidewalkMaterial;	}
				else if (materials[i].name.Split(' ')[0] == "WhitelineMaterial")	{ materials[i] = SharedWhitelineMaterial;	}
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

			TileModel.DesignIDChangeEvent.AddListener(OnDesignIDChange);

			SetSharedMaterials(gameObject.GetComponent<Renderer>());
		}

		// Update is called once per frame
		void Update()
		{

		}

		private void OnDesignIDChange()
		{
			
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}

			int dirCount = 0;
			if ((TileModel.DesignID & Model.Tiles.RoadTile.ABOVEROADMASK) != 0) { dirCount++; }
			if ((TileModel.DesignID & Model.Tiles.RoadTile.RIGHTROADMASK) != 0) { dirCount++; }
			if ((TileModel.DesignID & Model.Tiles.RoadTile.BELOWROADMASK) != 0) { dirCount++; }
			if ((TileModel.DesignID & Model.Tiles.RoadTile.LEFTROADMASK)  != 0) { dirCount++; }

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
				if ((TileModel.DesignID & (Model.Tiles.RoadTile.ABOVEROADMASK | Model.Tiles.RoadTile.BELOWROADMASK | Model.Tiles.RoadTile.RIGHTROADMASK | Model.Tiles.RoadTile.LEFTROADMASK) & 0b1001) == 0b1011)
				{
					rotation = new(0, 0, 180);
				}
				else if ((TileModel.DesignID & (Model.Tiles.RoadTile.ABOVEROADMASK | Model.Tiles.RoadTile.BELOWROADMASK | Model.Tiles.RoadTile.RIGHTROADMASK | Model.Tiles.RoadTile.LEFTROADMASK) & 0b1001) == 0b1101)
				{
					rotation = new(0, 0, 270);
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