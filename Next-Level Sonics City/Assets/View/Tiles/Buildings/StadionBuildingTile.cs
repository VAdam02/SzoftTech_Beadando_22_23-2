using Model.Tiles;
using Model.Tiles.Buildings;
using UnityEngine;

namespace View.Tiles.Buildings
{
	public class StadionBuildingTile : Tile
	{
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

		private static Material _planeMaterial;
		public static Material PlaneMaterial { get { if (_planeMaterial == null) _planeMaterial = LoadMaterialByName("PlaneMaterial"); return _planeMaterial; } }
		private Material _sharedPlaneMaterial;
		private Material SharedPlaneMaterial
		{
			get
			{
				if (_sharedPlaneMaterial == null)
				{
					_sharedPlaneMaterial = new Material(PlaneMaterial);
					_materials.Add(_sharedPlaneMaterial);
				}
				return _sharedPlaneMaterial;
			}
		}

		private static Material _topMaterial;
		public static Material TopMaterial { get { if (_topMaterial == null) _topMaterial = LoadMaterialByName("TopMaterial"); return _topMaterial; } }
		private Material _sharedTopMaterial;
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

		// Start is called before the first frame update
		void Start()
		{
			transform.localPosition = new Vector3(TileModel.Coordinates.x, 0, -TileModel.Coordinates.y) * 10 + GetPivot();
			transform.localScale = Vector3.one * 2;

			SetSharedMaterials(gameObject.GetComponent<Renderer>());
			foreach (Renderer renderer in gameObject.transform.GetComponentsInChildren<Renderer>())
			{
				SetSharedMaterials(renderer);
			}
		}

		// Update is called once per frame
		void Update()
		{

		}

		private static Material LoadMaterialByName(string name)
		{
			return Resources.Load<Material>("Tiles/StadionBuildingTile/Material/" + name);
		}

		private void SetSharedMaterials(Renderer renderer)
		{
			if (renderer == null) return;

			Material[] materials = renderer.sharedMaterials;

			for (int i = 0; i < materials.Length; i++)
			{
				if		(materials[i].name.Split(' ')[0] == "BaseMaterial")		{ materials[i] = SharedBaseMaterial;  }
				else if (materials[i].name.Split(' ')[0] == "GrassMaterial")	{ materials[i] = SharedGrassMaterial; }
				else if (materials[i].name.Split(' ')[0] == "PlaneMaterial")	{ materials[i] = SharedPlaneMaterial; }
				else if (materials[i].name.Split(' ')[0] == "TopMaterial")		{ materials[i] = SharedTopMaterial;   }
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
