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
			transform.localPosition = new Vector3(TileModel.Coordinates.x, 0, -TileModel.Coordinates.y) * 10 + GetPivot();
			transform.localScale = Vector3.one;
			transform.localRotation = Quaternion.Euler(0, ((int)((Building)TileModel).Rotation) * 90, 0);


			SetSharedMaterials(gameObject.GetComponent<Renderer>());
			foreach (Renderer renderer in gameObject.transform.GetComponentsInChildren<Renderer>())
			{
				SetSharedMaterials(renderer);
			}
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
