using Model.Tiles;
using UnityEngine;

namespace View.Tiles.Buildings
{
	public class WorkplaceSubTile : Tile
	{
		private void SetSharedMaterials(Renderer renderer)
		{
			if (renderer == null) return;

			Material[] materials = renderer.sharedMaterials;

			for (int i = 0; i < materials.Length; i++)
			{
				Debug.LogWarning(renderer);
				Debug.LogError("Unknown material found: " + renderer.materials[i].name);
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

		// Update is called once per frame
		void Update()
		{

		}
	}
}