using System.Collections;
using System.Collections.Generic;
using Model.Tiles;
using UnityEngine;
using Model.Tiles.Buildings;


namespace View.Tiles.Buildings
{
    public class IndustrialBuildingTile : Tile
    {
        private static Material _grassMaterial;
        public static Material GrassMaterial { get { if (_grassMaterial == null) _grassMaterial = LoadMaterialByName("GrassMaterial"); return _grassMaterial; } }
        private static Material _sharedGrassMaterial;
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
        private static Material _sharedWindowMaterial;
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
        private static Material _doorMaterial;
		public static Material DoorMaterial { get { if (_doorMaterial == null) _doorMaterial = LoadMaterialByName("DoorMaterial"); return _doorMaterial; } }
        private static Material _sharedDoorMaterial;
        private Material SharedDoorMaterial
		{
			get
			{
				if (_sharedDoorMaterial == null)
				{
					_sharedDoorMaterial = new Material(DoorMaterial);
					_materials.Add(_sharedDoorMaterial);
				}
				return _sharedDoorMaterial;
			}
		}
        private static Material _baseMaterial;
		public static Material BaseMaterial { get { if (_baseMaterial == null) _baseMaterial = LoadMaterialByName("BaseMaterial"); return _baseMaterial; } }
        private static Material _sharedBaseMaterial;
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
        private static Material _floorMaterial;
		public static Material FloorMaterial { get { if (_floorMaterial == null) _floorMaterial = LoadMaterialByName("FloorMaterial"); return _floorMaterial; } }
        private static Material _sharedFloorMaterial;
        private Material SharedFloorMaterial
		{
			get
			{
				if (_sharedFloorMaterial == null)
				{
					_sharedFloorMaterial = new Material(FloorMaterial);
					_materials.Add(_sharedFloorMaterial);
				}
				return _sharedFloorMaterial;
			}
		}
        private static Material _garageBaseMaterial;
		public static Material GarageBaseMaterial { get { if (_garageBaseMaterial == null) _garageBaseMaterial = LoadMaterialByName("GarageBaseMaterial"); return _garageBaseMaterial; } }
        private static Material _sharedGarageBaseMaterial;
        private Material SharedGarageBaseMaterial
		{
			get
			{
				if (_sharedGarageBaseMaterial == null)
				{
					_sharedGarageBaseMaterial = new Material(GarageBaseMaterial);
					_materials.Add(_sharedGarageBaseMaterial);
				}
				return _sharedGarageBaseMaterial;
			}
		}
        
        private static Material _groundMaterial;
		public static Material GroundMaterial { get { if (_groundMaterial == null) _groundMaterial = LoadMaterialByName("GroundMaterial"); return _groundMaterial; } }
        private static Material _sharedGroundMaterial;
        private Material SharedGroundMaterial
		{
			get
			{
				if (_sharedGroundMaterial == null)
				{
					_sharedGroundMaterial = new Material(GroundMaterial);
					_materials.Add(_sharedGroundMaterial);
				}
				return _sharedGroundMaterial;
			}
		}
        private static Material _roofMaterial;
		public static Material RoofMaterial { get { if (_roofMaterial == null) _roofMaterial = LoadMaterialByName("RoofMaterial"); return _roofMaterial; } }
        private static Material _sharedRoofMaterial;
        private Material SharedRoofMaterial
		{
			get
			{
				if (_sharedRoofMaterial == null)
				{
					_sharedRoofMaterial = new Material(RoofMaterial);
					_materials.Add(_sharedRoofMaterial);
				}
				return _sharedRoofMaterial;
			}
		}
        private static Material _topMaterial;
		public static Material TopMaterial { get { if (_topMaterial == null) _topMaterial = LoadMaterialByName("TopMaterial"); return _topMaterial; } }
        private static Material _sharedTopMaterial;
        private Material SharedTopMaterial
		{
			get
			{
				if (_sharedTopMaterial == null)
				{
					_sharedTopMaterial = new Material(TopMaterial);
					_materials.Add(_sharedTopMaterial);
				}
				return _sharedTopMaterial;
			}
		}
        private static Material _wallMaterial;
		public static Material WallMaterial { get { if (_wallMaterial == null) _wallMaterial = LoadMaterialByName("WallMaterial"); return _wallMaterial; } }
        private static Material _sharedWallMaterial;
        private Material SharedWallMaterial
		{
			get
			{
				if (_sharedWallMaterial == null)
				{
					_sharedWallMaterial = new Material(WallMaterial);
					_materials.Add(_sharedWallMaterial);
				}
				return _sharedWallMaterial;
			}
		}

        private static Material LoadMaterialByName(string name)
		{
			return Resources.Load<Material>("Tiles/Industrial/Material/" + name);
		}
        private void SetSharedMaterials(Renderer renderer)
		{
			if (renderer == null) return;

			Material[] materials = renderer.sharedMaterials;

			for (int i = 0; i < materials.Length; i++)
			{
				if		(materials[i].name.Split(' ')[0] == "GrassMaterial") { materials[i] = SharedGrassMaterial; }
				else if	(materials[i].name.Split(' ')[0] == "WindowMaterial") { materials[i] = SharedWindowMaterial; }
				else if (materials[i].name.Split(' ')[0] == "TopMaterial") { materials[i] = SharedTopMaterial; }
				else if (materials[i].name.Split(' ')[0] == "BaseMaterial") { materials[i] = SharedBaseMaterial; }
				else if (materials[i].name.Split(' ')[0] == "WallMaterial") { materials[i] = SharedWallMaterial; }
				else if (materials[i].name.Split(' ')[0] == "RoofMaterial") { materials[i] = SharedRoofMaterial; }
                else if (materials[i].name.Split(' ')[0] == "GroundMaterial") { materials[i] = SharedGroundMaterial; }
                else if (materials[i].name.Split(' ')[0] == "GarageBaseMaterial") { materials[i] = SharedGarageBaseMaterial; }
                else if (materials[i].name.Split(' ')[0] == "FloorMaterial") { materials[i] = SharedFloorMaterial; }
                else if (materials[i].name.Split(' ')[0] == "DoorMaterial") { materials[i] = SharedDoorMaterial; }
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
        private void Display()
        {
            foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}

            GameObject industrial;
            if((TileModel.DesignID & (Commercial.COMMERCIAL_LEVEL_MASK )) == 0)
            {
                industrial = Instantiate(LoadModelByName("EmptyObject"));
            }
            else if((TileModel.DesignID & (Commercial.COMMERCIAL_LEVEL_MASK )) == 1)
            {
                industrial = Instantiate(LoadModelByName("Industrial1"));
            }
            else if((TileModel.DesignID & (Commercial.COMMERCIAL_LEVEL_MASK )) == 2)
            {
                industrial = Instantiate(LoadModelByName("Industrial2"));
            }
            else
            {
                industrial = Instantiate(LoadModelByName("Industrial3"));
            }
            industrial.transform.parent = transform;
			industrial.transform.localScale = Vector3.one * 20;
			industrial.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(-90, 0, 0));
			

			SetSharedMaterials(industrial.GetComponent<Renderer>());
        }
        private static GameObject LoadModelByName(string name)
		{
			return Resources.Load<GameObject>("Tiles/Industrial/Model/" + name);
		}
    }
}
