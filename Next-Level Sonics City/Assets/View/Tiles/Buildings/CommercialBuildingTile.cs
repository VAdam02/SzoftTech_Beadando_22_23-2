using Model.Tiles;
using Model.Tiles.Buildings;
using UnityEngine;

namespace View.Tiles.Buildings
{
	public class CommercialBuildingTile : Tile
    {
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
        private static Material _windowMaterial;
		public static Material WindowMaterial { get { if (_windowMaterial == null) _windowMaterial = LoadMaterialByName("WindowMaterial"); return _windowMaterial; } }
        private Material _sharedWindowMaterial;
        private Material SharedWindowMaterial
		{
			get
			{
				if (_sharedWindowMaterial == null)
				{
					_sharedWindowMaterial = new Material(WindowMaterial);
					_materials.Add(_sharedWindowMaterial);
				}
				return _sharedWindowMaterial;
			}
		}
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
        private static Material _baseMaterial;
		public static Material BaseMaterial { get { if (_baseMaterial == null) _baseMaterial = LoadMaterialByName("BaseMaterial"); return _baseMaterial; } }
        private Material _sharedBaseMaterial;
        		private Material SharedBaseMaterial
		{
			get
			{
				if (_sharedBaseMaterial == null)
				{
					_sharedBaseMaterial = new Material(BaseMaterial);
					_materials.Add(_sharedBaseMaterial);
				}
				return _sharedBaseMaterial;
			}
		}
        private static Material _blackMaterial;
		public static Material BlackMaterial { get { if (_blackMaterial == null) _blackMaterial = LoadMaterialByName("BlackMaterial"); return _blackMaterial; } }
        private Material _sharedBlackMaterial;
        private Material SharedBlackMaterial
		{
			get
			{
				if (_sharedBlackMaterial == null)
				{
					_sharedBlackMaterial = new Material(BlackMaterial);
					_materials.Add(_sharedBlackMaterial);
				}
				return _sharedBlackMaterial;
			}
		}
        private static Material _whiteMaterial;
		public static Material WhiteMaterial { get { if (_whiteMaterial == null) _whiteMaterial = LoadMaterialByName("WhiteMaterial"); return _whiteMaterial; } }
        private Material _sharedWhiteMaterial;
        private Material SharedWhiteMaterial
		{
			get
			{
				if (_sharedWhiteMaterial == null)
				{
					_sharedWhiteMaterial = new Material(WhiteMaterial);
					_materials.Add(_sharedWhiteMaterial);
				}
				return _sharedWhiteMaterial;
			}
		}
        
        private static Material _houseColor;
		public static Material HouseColor { get { if (_houseColor == null) _houseColor = LoadMaterialByName("HouseColor"); return _houseColor; } }
        private Material _sharedHouseColor;
        private Material SharedHouseColor
		{
			get
			{
				if (_sharedHouseColor == null)
				{
					_sharedHouseColor = new Material(HouseColor);
					_materials.Add(_sharedHouseColor);
				}
				return _sharedHouseColor;
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

        private static Material LoadMaterialByName(string name)
		{
			return Resources.Load<Material>("Tiles/CommercialBuildingTile/Material/" + name);
		}
        private void SetSharedMaterials(Renderer renderer)
		{
			if (renderer == null) return;

			Material[] materials = renderer.sharedMaterials;

			for (int i = 0; i < materials.Length; i++)
			{
				if		(materials[i].name.Split(' ')[0] == "GrassMaterial") { materials[i] = SharedGrassMaterial; }
				else if	(materials[i].name.Split(' ')[0] == "WindowMaterial") { materials[i] = SharedWindowMaterial; }
				else if (materials[i].name.Split(' ')[0] == "AsphaltMaterial") { materials[i] = SharedAsphaltMaterial; }
				else if (materials[i].name.Split(' ')[0] == "BaseMaterial") { materials[i] = SharedBaseMaterial; }
				else if (materials[i].name.Split(' ')[0] == "BlackMaterial") { materials[i] = SharedBlackMaterial; }
				else if (materials[i].name.Split(' ')[0] == "WhiteMaterial") { materials[i] = SharedWhiteMaterial; }
                else if (materials[i].name.Split(' ')[0] == "HouseColor") { materials[i] = SharedHouseColor; }
                else if (materials[i].name.Split(' ')[0] == "SidewalkMaterial") { materials[i] = SharedSidewalkMaterial; }
				else
				{
					Debug.LogWarning(renderer);
					Debug.LogError("Unknown material found: " + renderer.materials[i].name);
				}
			}

			renderer.sharedMaterials = materials;
		}
        
        void Start()
        {
			transform.localPosition = new Vector3(TileModel.Coordinates.x, 0, -TileModel.Coordinates.y) * 10;
			transform.localScale = Vector3.one;
			transform.localRotation = Quaternion.Euler(0, ((int)((Building)TileModel).Rotation) * 90, 0);

            Display();
            OnDesignIDChange.AddListener(Display);
			SetSharedMaterials(gameObject.GetComponent<Renderer>());
			foreach (Renderer renderer in gameObject.transform.GetComponentsInChildren<Renderer>())
			{
				SetSharedMaterials(renderer);
			}
        }
		public override GameObject DisplayPopUp()
        {
            GameObject popup = Instantiate(Resources.Load<GameObject>("Tiles/CommercialBuildingTile/CommercialBuildingTilePopUp"), GameObject.Find("Canvas").transform);
            popup.GetComponent<PopUpWindow>().TileModel = TileModel;
            return popup;
        }
        private void Display()
        {
            foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}

            GameObject commercial;
            if((TileModel.DesignID & (CommercialBuildingTIle.COMMERCIAL_LEVEL_MASK )) == 0)
            {
                commercial = Instantiate(LoadModelByName("EmptyObject"));
            }
            else if((TileModel.DesignID & (CommercialBuildingTIle.COMMERCIAL_LEVEL_MASK )) == 1)
            {
                commercial = Instantiate(LoadModelByName("Commercial1"));
            }
            else if((TileModel.DesignID & (CommercialBuildingTIle.COMMERCIAL_LEVEL_MASK )) == 2)
            {
                commercial = Instantiate(LoadModelByName("Commercial2"));
            }
            else
            {
                commercial = Instantiate(LoadModelByName("Commercial3"));
            }
            commercial.transform.parent = transform;
			commercial.transform.localScale = Vector3.one * 20;
			commercial.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(-90, 0, 0));
			

			SetSharedMaterials(commercial.GetComponent<Renderer>());
        }
        private static GameObject LoadModelByName(string name)
		{
			return Resources.Load<GameObject>("Tiles/CommercialBuildingTile/Model/" + name);
		}
    }
}
