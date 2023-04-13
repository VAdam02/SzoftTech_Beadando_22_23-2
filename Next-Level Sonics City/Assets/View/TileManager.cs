using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.Simulation;
using Model;
using Model.Tiles.Buildings;
using Model.Tiles;

namespace View
{
	public class TileManager : MonoBehaviour
	{
		private static TileManager _instance;
		public static TileManager Instance { get { return _instance; } }

		// Start is called before the first frame update
		void Start()
		{
			_instance = this;

			for (int i = 0; i < SimEngine.Instance.Tiles.GetLength(0); i++)
			for (int j = 0; j < SimEngine.Instance.Tiles.GetLength(1); j++)
			{
				Model.Tile tileModel = SimEngine.Instance.Tiles[i, j];

				GameObject tileView = Instantiate(Resources.Load<GameObject>("Tiles/" + tileModel.GetType().Name + "/" + tileModel.GetType().Name), Vector3.zero, Quaternion.identity);
				tileView.transform.localScale *= Tile.MODELSCALE;

				if (tileModel.GetType() == typeof(EmptyTile)) { tileView.GetComponent<View.Tiles.EmptyTile>().Init(tileModel); }
				else if (tileModel.GetType() == typeof(ResidentialBuildingTile)) { tileView.GetComponent<View.Tiles.Buildings.ResidentialBuildingTile>().Init(tileModel); }
				else
				{
					Debug.LogError("Unknown tile type found: " + tileModel.GetType());
				}
				tileView.transform.SetParent(transform);
			}
		}

		// Update is called once per frame
		void Update()
		{
			
		}
	}
}
