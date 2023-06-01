using Model.Tiles;
using Model.Tiles.Buildings;
using UnityEngine;

namespace View.Tiles.Buildings
{
	public class MiddleSchoolBuildingTile : Tile
	{
		private static Material _base;
		public static Material Base { get { if (_base == null) _base = LoadMaterialByName("Base"); return _base; } }
		private Material _sharedBase;
		private Material SharedBase
		{
			get
			{
				if (_sharedBase == null)
				{
					_sharedBase = new Material(Base);
					_materials.Add(_sharedBase);
				}
				return _sharedBase;
			}
		}

		private static Material _door;
		public static Material Door { get { if (_door == null) _door = LoadMaterialByName("Door"); return _door; } }
		private Material _sharedDoor;
		private Material SharedDoor
		{
			get
			{
				if (_sharedDoor == null)
				{
					_sharedDoor = new Material(Door);
					_materials.Add(_sharedDoor);
				}
				return _sharedDoor;
			}
		}

		private static Material _roof;
		public static Material Roof { get { if (_roof == null) _roof = LoadMaterialByName("Roof"); return _roof; } }
		private Material _sharedroof;
		private Material SharedRoof
		{
			get
			{
				if (_sharedroof == null)
				{
					_sharedroof = new Material(Roof);
					_materials.Add(_sharedroof);
				}
				return _sharedroof;
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
			transform.localScale = new Vector3(2,1,1);

			SetSharedMaterials(gameObject.GetComponent<Renderer>());
			foreach (Renderer renderer in gameObject.transform.GetComponentsInChildren<Renderer>())
			{
				SetSharedMaterials(renderer);
			}
		}
		public override GameObject DisplayPopUp()
        {
            GameObject popup = Instantiate(Resources.Load<GameObject>("Tiles/MiddleSchoolBuildingTile/MiddleSchoolBuildingTilePopUp"), GameObject.Find("Canvas").transform);
            popup.GetComponent<PopUpWindow>().TileModel = TileModel;
            return popup;
        }

		// Update is called once per frame
		void Update()
		{

		}

		private static Material LoadMaterialByName(string name)
		{
			return Resources.Load<Material>("Tiles/MiddleSchoolBuildingTile/Material/" + name);
		}

		private void SetSharedMaterials(Renderer renderer)
		{
			if (renderer == null) return;

			Material[] materials = renderer.sharedMaterials;

			for (int i = 0; i < materials.Length; i++)
			{
				if		(materials[i].name.Split(' ')[0] == "Base")		{ materials[i] = SharedBase;  }
				else if (materials[i].name.Split(' ')[0] == "Door")	{ materials[i] = SharedDoor; }
				else if (materials[i].name.Split(' ')[0] == "Roof")	{ materials[i] = SharedRoof; }
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
