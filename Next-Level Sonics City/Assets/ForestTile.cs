using System.Collections.Generic;
using UnityEngine;

public class ForestTile : MonoBehaviour
{
	public float resolution = 12f;
	public float noiseScale = 1.5f;
	public Material planeMaterial;
	public GameObject treePrefab;

	private void Start()
	{
		Vector3 to = new Vector3(10f, 10f, 10f);

		List<Vector3> localMaxima = FindLocalExtrema(-transform.localPosition, -transform.localPosition + to, resolution);
		PlaceTrees(localMaxima);
		VisualizePerlinNoise(-transform.localPosition, -transform.localPosition + to, resolution);
	}

	public float counter = 0f;
	private void Update()
	{
		counter += Time.deltaTime;

		if (counter < 0.3f)
		{
			return;
		}
		counter -= 0.3f;

		//destroy all child
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		Vector3 to = new Vector3(10f, 0f, 10f);

		List<Vector3> localMaxima = FindLocalExtrema(-transform.localPosition, -transform.localPosition + to, resolution);
		PlaceTrees(localMaxima);
		VisualizePerlinNoise(-transform.localPosition, -transform.localPosition + to, resolution);
	}

	List<Vector3> FindLocalExtrema(Vector3 from, Vector3 to, float resolution)
	{
		List<Vector3> localExtrema = new List<Vector3>();

		float stepSizeX = (to.x - from.x) / (resolution - 1);
		float stepSizeY = (to.z - from.z) / (resolution - 1);

		for (int x = 1; x < resolution - 1; x++)
		{
			for (int y = 1; y < resolution - 1; y++)
			{
				float centerX = from.x + x * stepSizeX;
				float centerY = from.z + y * stepSizeY;

				float currentHeight = GetPerlinNoiseValue(new Vector3(centerX, 0f, centerY));

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

						float neighborHeight = GetPerlinNoiseValue(new Vector3(neighborX, 0f, neighborY));

						if (neighborHeight < currentHeight)
						{
							if (potentially == -1)
							{
								isLocalExtrema = false;
								break;
							}
							else
							{
								potentially = 1;
							}
						}
						else if (neighborHeight > currentHeight)
						{
							if (potentially == 1)
							{
								isLocalExtrema = false;
								break;
							}
							else
							{
								potentially = -1;
							}
						}
						else
						{
							isLocalExtrema = false;
							break;
						}
					}

					if (!isLocalExtrema)
						break;
				}

				if (isLocalExtrema)
				{
					float xPos = centerX;
					float yPos = GetPerlinNoiseValue(new Vector3(centerX, 0f, centerY));
					float zPos = centerY;
					localExtrema.Add(new Vector3(xPos, yPos, zPos));
				}
			}
		}

		return localExtrema;
	}


	void PlaceTrees(List<Vector3> locations)
	{
		foreach (Vector3 location in locations)
		{
			GameObject tree = Instantiate(treePrefab);

			tree.transform.SetParent(transform);

			Vector3 loc = (location + transform.localPosition) * -1;
			loc.y = 0;

			tree.transform.localPosition = loc + new Vector3(5, 0, 5);
			tree.transform.localScale = new Vector3(20f, 20f, 20f);
			tree.transform.localRotation = Quaternion.Euler(-90f, 0, 0);
		}
	}

	float GetPerlinNoiseValue(Vector3 position)
	{
		return PerlinNoiseUtility.GetPerlinNoiseValue(position, noiseScale);
	}

	void VisualizePerlinNoise(Vector3 from, Vector3 to, float resolution)
	{
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		Material materialInstance = new Material(planeMaterial);
		meshRenderer.material = materialInstance;

		Texture2D texture = new Texture2D((int)resolution, (int)resolution);
		Color[] colors = new Color[(int)resolution * (int)resolution];

		float stepSizeX = (to.x - from.x) / (resolution - 1);
		float stepSizeY = (to.z - from.z) / (resolution - 1);

		for (int x = 0; x < resolution; x++)
		{
			for (int y = 0; y < resolution; y++)
			{
				float posX = from.x + x * stepSizeX;
				float posY = from.z + y * stepSizeY;
				float noiseValue = GetPerlinNoiseValue(new Vector3(posX, 0f, posY));

				colors[(int)(y * resolution + x)] = new Color(noiseValue, noiseValue, noiseValue);
			}
		}

		texture.SetPixels(colors);
		texture.Apply();
		materialInstance.mainTexture = texture;
	}

	public static class PerlinNoiseUtility
	{
		public static float GetPerlinNoiseValue(Vector3 position, float scale)
		{
			return Mathf.PerlinNoise(position.x * scale + 10, position.z * scale + 10);
		}
	}
}
