using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace View.Tiles
{
	public class ForestTile : Tile
	{
		private const float RESOLUTION = 50;
		private const float NOISESCALE = 0.3f;

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

		private static Material _leavesMaterial;
		public static Material LeavesMaterial { get { if (_leavesMaterial == null) _leavesMaterial = LoadMaterialByName("LeavesMaterial"); return _leavesMaterial; } }
		private Material _sharedLeavesMaterial;
		private Material SharedLeavesMaterial
		{
			get
			{
				if (_sharedLeavesMaterial == null)
				{
					_sharedLeavesMaterial = new Material(LeavesMaterial);
					_materials.Add(_sharedLeavesMaterial);
				}
				return _sharedLeavesMaterial;
			}
		}

		private static Material _trunkMaterial;
		public static Material TrunkMaterial { get { if (_trunkMaterial == null) _trunkMaterial = LoadMaterialByName("TrunkMaterial"); return _trunkMaterial; } }
		private Material _sharedTrunkMaterial;
		private Material SharedTrunkMaterial
		{
			get
			{
				if (_sharedTrunkMaterial == null)
				{
					_sharedTrunkMaterial = new Material(TrunkMaterial);
					_materials.Add(_sharedTrunkMaterial);
				}
				return _sharedTrunkMaterial;
			}
		}

		private static Material LoadMaterialByName(string name)
		{
			return Resources.Load<Material>("Tiles/ForestTile/Material/" + name);
		}

		private static GameObject LoadModelByName(string name)
		{
			return Resources.Load<GameObject>("Tiles/ForestTile/Model/" + name);
		}

		private void SetSharedMaterials(Renderer renderer)
		{
			if (renderer == null) return;

			Material[] materials = renderer.sharedMaterials;

			for (int i = 0; i < materials.Length; i++)
			{
				if		(materials[i].name.Split(' ')[0] == "GrassMaterial")	{ materials[i] = SharedGrassMaterial;  }
				else if (materials[i].name.Split(' ')[0] == "LeavesMaterial")	{ materials[i] = SharedLeavesMaterial; }
				else if (materials[i].name.Split(' ')[0] == "TrunkMaterial")	{ materials[i] = SharedTrunkMaterial;  }
				else
				{
					Debug.LogWarning(renderer);
					Debug.LogError("Unknown material found: " + renderer.materials[i].name);
				}
			}

			renderer.sharedMaterials = materials;
		}

		// Start is called before the first frame update
		private void Start()
		{
			transform.localPosition = new Vector3(TileModel.Coordinates.x, 0, -TileModel.Coordinates.y) * 10;
			transform.localScale = Vector3.one;

			OnDesignIDChange.AddListener(Display);
			Display();

			SetSharedMaterials(gameObject.GetComponent<Renderer>());
		}


		private void Display()
		{
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}

			List<Vector3> locations = FindLocalExtremas(-transform.localPosition - new Vector3(10f, 0f, 10f), -transform.localPosition, RESOLUTION);
			foreach (Vector3 location in locations)
			{
				GameObject tree = Instantiate(LoadModelByName("Spruce"));

				tree.transform.SetParent(transform);

				Vector3 loc = (location + transform.localPosition) * -1;
				loc.y = 0;

				tree.transform.localPosition = loc - new Vector3(5, 0, 5);
				tree.transform.localScale = new Vector3(40, 40, 40f) * Mathf.Sin(Mathf.Clamp(((Model.Tiles.ForestTile)TileModel).Age, 0.5f, 10) * Mathf.PI / 2 / Model.Tiles.ForestTile.MAINTANCENEEDEDFORYEAR);
				tree.transform.localRotation = Quaternion.Euler(-90f, 0, 0);

				SetSharedMaterials(tree.GetComponent<Renderer>());
			}
		}

		List<Vector3> FindLocalExtremas(Vector3 from, Vector3 to, float resolution)
		{
			List<Vector3> localExtrema = new();

			float stepSizeX = (to.x - from.x) / (resolution - 1);
			float stepSizeY = (to.z - from.z) / (resolution - 1);

			Parallel.For(1, (int)resolution - 1, x =>
			{
				for (int y = 1; y < resolution - 1; y++)
				{
					float centerX = from.x + x * stepSizeX;
					float centerY = from.z + y * stepSizeY;

					float currentHeight = GetPerlinNoiseValue(new Vector3(centerX, 0f, centerY), NOISESCALE);

					bool isLocalExtrema = true;
					int potentially = 0;

					for (int i = -1; i <= 1; i++)
					{
						for (int j = -1; j <= 1; j++)
						{
							if (i == 0 && j == 0)
								continue;

							float neighborX = centerX + i * stepSizeX;
							float neighborY = centerY + j * stepSizeY;

							float neighborHeight = GetPerlinNoiseValue(new Vector3(neighborX, 0f, neighborY), NOISESCALE);

							if (neighborHeight < currentHeight)
							{
								if (potentially == -1) { isLocalExtrema = false; break; }
								else { potentially = 1; }
							}
							else if (neighborHeight > currentHeight)
							{
								if (potentially == 1) { isLocalExtrema = false; break; }
								else { potentially = -1; }
							}
							else { isLocalExtrema = false; break; }
						}

						if (!isLocalExtrema) break;
					}

					if (isLocalExtrema)
					{
						float xPos = centerX;
						float yPos = GetPerlinNoiseValue(new Vector3(centerX, 0f, centerY), NOISESCALE);
						float zPos = centerY;
						lock (localExtrema)
						{
							localExtrema.Add(new Vector3(xPos, yPos, zPos));
						}
					}
				}
			});

			return localExtrema;
		}

		public override GameObject DisplayPopUp()
		{
			GameObject popup = Instantiate(Resources.Load<GameObject>("Tiles/ForestTile/ForestTilePopUp"));
			popup.transform.SetParent(GameObject.Find("Canvas").transform);
			popup.GetComponent<PopUpWindow>().TileModel = TileModel;
			return popup;
		}

		float GetPerlinNoiseValue(Vector3 position, float scale)
		{
			return Mathf.PerlinNoise(position.x * scale, position.z * scale);
		}
	}
}