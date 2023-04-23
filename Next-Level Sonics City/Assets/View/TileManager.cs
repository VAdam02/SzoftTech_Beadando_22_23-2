using UnityEngine;
using Model.Simulation;
using Model.Tiles.Buildings;
using Model.Tiles;
using System.Collections.Generic;

namespace View
{
	public enum Action
	{
		NONE,
		SELECTAREA
	}

	public class TileManager : MonoBehaviour
	{
		private static TileManager _instance;
		public static TileManager Instance { get { return _instance; } }

		private Action _currentAction = Action.SELECTAREA;
		public Action CurrentAction { get { return _currentAction; } set { _currentAction = value; } }

		private List<Tile> _selectedTiles = new();
		public List<Tile> SelectedTiles
		{
			get { return _selectedTiles; }
			set
			{
				foreach (Tile tile in _selectedTiles)
				{
					tile.Unhighlight();
				}
				_selectedTiles = value;
				foreach (Tile tile in _selectedTiles)
				{
					tile.Highlight();
				}
			}
		}

		// Start is called before the first frame update
		void Start()
		{
			_instance = this;

			for (int i = 0; i < SimEngine.Instance.GetSize(); i++)
			for (int j = 0; j < SimEngine.Instance.GetSize(); j++)
			{
				Model.Tile tileModel = SimEngine.Instance.GetTile(i, j);

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
