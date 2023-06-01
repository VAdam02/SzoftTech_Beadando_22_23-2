using Model.Tiles;
using Model.Tiles.Buildings;
using UnityEngine;

namespace View.Tiles.Buildings
{
	public class PowerPlantBuildingTile : Tile
	{
		private static Material _buildingbase;
		public static Material BuildingBase { get { if (_buildingbase == null) _buildingbase = LoadMaterialByName("BuildingBase"); return _buildingbase; } }
		private Material _sharedBuildingBase;
		private Material SharedBuildingBase
		{
			get
			{
				if (_sharedBuildingBase == null)
				{
					_sharedBuildingBase = new Material(BuildingBase);
					_materials.Add(_sharedBuildingBase);
				}
				return _sharedBuildingBase;
			}
		}

		private static Material _pipe;
		public static Material Pipe { get { if (_pipe == null) _pipe = LoadMaterialByName("Pipe"); return _pipe; } }
		private Material _sharedPipe;
		private Material SharedPipe
		{
			get
			{
				if (_sharedPipe == null)
				{
					_sharedPipe = new Material(Pipe);
					_materials.Add(_sharedPipe);
				}
				return _sharedPipe;
			}
		}

		private static Material _chimneyhole;
		public static Material ChimneyHole { get { if (_chimneyhole == null) _chimneyhole = LoadMaterialByName("ChimneyHole"); return _chimneyhole; } }
		private Material _sharedChimneyhole;
		private Material SharedChimneyhole
		{
			get
			{
				if (_sharedChimneyhole == null)
				{
					_sharedChimneyhole = new Material(ChimneyHole);
					_materials.Add(_sharedChimneyhole);
				}
				return _sharedChimneyhole;
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

		private static Material _window;
		public static Material Window { get { if (_window == null) _window = LoadMaterialByName("Window"); return _window; } }
		private Material _sharedWindow;
		private Material SharedWindow
		{
			get
			{
				if (_sharedWindow == null)
				{
					_sharedWindow = new Material(Window);
					_materials.Add(_sharedWindow);
				}
				return _sharedWindow;
			}
		}

		// Start is called before the first frame update
		void Start()
		{
			transform.localPosition = new Vector3(TileModel.Coordinates.x, 0, -TileModel.Coordinates.y) * 10 + GetPivot();
			transform.localScale = Vector3.one * 2;
			transform.localRotation = Quaternion.Euler(0, ((int)((Building)TileModel).Rotation) * 90, 0);


			SetSharedMaterials(gameObject.GetComponent<Renderer>());
			foreach (Renderer renderer in gameObject.transform.GetComponentsInChildren<Renderer>())
			{
				SetSharedMaterials(renderer);
			}
		}
		public override GameObject DisplayPopUp()
        {
            GameObject popup = Instantiate(Resources.Load<GameObject>("Tiles/PowerPlantBuildingTile/PowerPlantBuildingTilePopUp"), GameObject.Find("Canvas").transform);
            popup.GetComponent<PopUpWindow>().TileModel = TileModel;
            return popup;
        }

		// Update is called once per frame
		void Update()
		{

		}

		private static Material LoadMaterialByName(string name)
		{
			return Resources.Load<Material>("Tiles/PowerPlantBuildingTile/Material/" + name);
		}

		private void SetSharedMaterials(Renderer renderer)
		{
			if (renderer == null) return;

			Material[] materials = renderer.sharedMaterials;

			for (int i = 0; i < materials.Length; i++)
			{
				if		(materials[i].name.Split(' ')[0] == "BuildingBase")		{ materials[i] = SharedBuildingBase;  }
				else if (materials[i].name.Split(' ')[0] == "Pipe")	{ materials[i] = SharedPipe; }
				else if (materials[i].name.Split(' ')[0] == "ChimneyHole")	{ materials[i] = SharedChimneyhole; }
				else if (materials[i].name.Split(' ')[0] == "Window")		{ materials[i] = SharedWindow;   }
                else if (materials[i].name.Split(' ')[0] == "GrassMaterial")		{ materials[i] = SharedGrassMaterial;   }
				else
				{
					Debug.LogWarning(renderer);
					Debug.LogError("Unknown material found: " + renderer.materials[i].name);
				}
			}

			renderer.sharedMaterials = materials;
		}

		public override Vector3 GetPivot()
		{
			Vector3 pivot = new(5f, 0, -5f);
			if (((Building)TileModel).Rotation == Rotation.Zero) return pivot;
			else if (((Building)TileModel).Rotation == Rotation.Ninety) return new(pivot.z, 0, -pivot.x);
			else if (((Building)TileModel).Rotation == Rotation.OneEighty) return new(-pivot.x, 0, pivot.z);
			else return new(-pivot.z, 0, pivot.x);
		}
	}
}
