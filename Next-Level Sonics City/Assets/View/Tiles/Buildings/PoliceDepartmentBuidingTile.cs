using Model.Tiles;
using UnityEngine;

namespace View.Tiles.Buildings
{
	public class PoliceDepartmentBuidingTile : Tile
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

		private static Material _towerBlueMaterial;
		public static Material TowerBlueMaterial { get { if (_towerBlueMaterial == null) _towerBlueMaterial = LoadMaterialByName("TowerBlueMaterial"); return _towerBlueMaterial; } }
		private Material _sharedTowerBlueMaterial;
		private Material SharedTowerBlueMaterial
		{
			get
			{
				if (_sharedTowerBlueMaterial == null)
				{
					_sharedTowerBlueMaterial = new Material(TowerBlueMaterial);
					_materials.Add(_sharedTowerBlueMaterial);
				}
				return _sharedTowerBlueMaterial;
			}
		}

		private static Material _helipadMaterial;
		public static Material HelipadMaterial { get { if (_helipadMaterial == null) _helipadMaterial = LoadMaterialByName("HelipadMaterial"); return _helipadMaterial; } }
		private Material _sharedHelipadMaterial;
		private Material SharedHelipadMaterial
		{
			get
			{
				if (_sharedHelipadMaterial == null)
				{
					_sharedHelipadMaterial = new Material(HelipadMaterial);
					_materials.Add(_sharedHelipadMaterial);
				}
				return _sharedHelipadMaterial;
			}
		}

		private static Material _helipadHMaterial;
		public static Material HelipadHMaterial { get { if (_helipadHMaterial == null) _helipadHMaterial = LoadMaterialByName("HelipadHMaterial"); return _helipadHMaterial; } }
		private Material _sharedHelipadHMaterial;
		private Material SharedHelipadHMaterial
		{
			get
			{
				if (_sharedHelipadHMaterial == null)
				{
					_sharedHelipadHMaterial = new Material(HelipadHMaterial);
					_materials.Add(_sharedHelipadHMaterial);
				}
				return _sharedHelipadHMaterial;
			}
		}

		private static Material LoadMaterialByName(string name)
		{
			return Resources.Load<Material>("Tiles/PoliceDepartmentBuildingTile/Material/" + name);
		}

		private void SetSharedMaterials(Renderer renderer)
		{
			if (renderer == null) return;

			Material[] materials = renderer.sharedMaterials;

			for (int i = 0; i < materials.Length; i++)
			{
				if		(materials[i].name.Split(' ')[0] == "GrassMaterial") { materials[i] = SharedGrassMaterial; }
				else if	(materials[i].name.Split(' ')[0] == "WindowMaterial") { materials[i] = SharedWindowMaterial; }
				else if (materials[i].name.Split(' ')[0] == "BaseMaterial") { materials[i] = SharedBaseMaterial; }
				else if (materials[i].name.Split(' ')[0] == "TowerBlueMaterial") { materials[i] = SharedTowerBlueMaterial; }
				else if (materials[i].name.Split(' ')[0] == "HelipadMaterial") { materials[i] = SharedHelipadMaterial; }
				else if (materials[i].name.Split(' ')[0] == "HelipadHMaterial") { materials[i] = SharedHelipadHMaterial; }
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
			transform.localRotation = Quaternion.Euler(0, ((int)((Building)TileModel).Rotation) * 90, 0);

			SetSharedMaterials(gameObject.GetComponent<Renderer>());
			foreach (Renderer renderer in gameObject.transform.GetComponentsInChildren<Renderer>())
			{
				SetSharedMaterials(renderer);
			}
			
		}
	}
}