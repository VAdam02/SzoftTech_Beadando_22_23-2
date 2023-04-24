using UnityEngine;
using Model.Simulation;
using Model.Tiles.Buildings;
using Model.Tiles;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;

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
				if (_selectedTiles.Count >= 2)
				foreach (Tile tile in GetTilesInArea(_selectedTiles[0], _selectedTiles[1])) { tile.Unhighlight(); }
				_selectedTiles = value;
				if (_selectedTiles.Count >= 2)
				foreach (Tile tile in GetTilesInArea(_selectedTiles[0], _selectedTiles[1])) { tile.Highlight(); }
			}
		}

		private List<Tile> GetTilesInArea(Tile corner1, Tile corner2)
		{
			List<Tile> tiles = new();

			Vector2 bottomleft =	new(Mathf.Min(corner1.TileModel.Coordinates.x, corner2.TileModel.Coordinates.x),
										Mathf.Min(corner1.TileModel.Coordinates.y, corner2.TileModel.Coordinates.y));
			Vector2 topright =		new(Mathf.Max(corner1.TileModel.Coordinates.x, corner2.TileModel.Coordinates.x),
										Mathf.Max(corner1.TileModel.Coordinates.y, corner2.TileModel.Coordinates.y));

			for (int i = (int)bottomleft.x; i <= topright.x; i++)
			for (int j = (int)bottomleft.y; j <= topright.y; j++)
			{
				tiles.Add(_tiles[i, j]);
			}

			return tiles;
		}

		public void MarkZone(TileType tileType)
		{
			if (_selectedTiles.Count >= 2)
			foreach (Tile tile in GetTilesInArea(_selectedTiles[0], _selectedTiles[1]))
			{
				SimEngine.Instance.BuildingManager.Build(tile.TileModel, tileType);
			}
		}

		public void CloneTileFromModel(Model.Tile tileModel)
		{
			GameObject tileView = Instantiate(Resources.Load<GameObject>("Tiles/" + tileModel.GetType().Name + "/" + tileModel.GetType().Name), Vector3.zero, Quaternion.identity);
			tileView.transform.localScale *= Tile.MODELSCALE;
			tileView.GetComponent<Tile>().Init(tileModel);
			tileView.transform.SetParent(transform);
			_tiles[(int)tileModel.Coordinates.x, (int)tileModel.Coordinates.y] = tileView.GetComponent<Tile>();
			//_tiles[(int)tileModel.Coordinates.x, (int)tileModel.Coordinates.y] = (Tile)tileView.GetComponents<Component>().ToList().Find(item => item.GetType().BaseType == typeof(Tile));
		}	

		private Tile[,] _tiles;

		// Start is called before the first frame update
		void Start()
		{
			_instance = this;

			_tiles = new Tile[SimEngine.Instance.GetSize(), SimEngine.Instance.GetSize()];

			for (int i = 0; i < SimEngine.Instance.GetSize(); i++)
			for (int j = 0; j < SimEngine.Instance.GetSize(); j++)
			{
				CloneTileFromModel(SimEngine.Instance.GetTile(i, j));
				/*
				_tiles[i, j] = (Tile)tileView.GetComponents<Component>().ToList().Find(item => item.GetType().BaseType == typeof(Tile));
				*/
			}
		}

		// Update is called once per frame
		void Update()
		{
			
		}
	}
}
