using UnityEngine;
using Model.Simulation;
using Model.Tiles;
using System.Collections.Generic;
using Model.Tiles.Buildings;
using Model;

namespace View
{
	public enum Action
	{
		NONE,
		SELECTAREA,
		BUILDGHOST
	}

	public class TileManager : MonoBehaviour
	{
		private static TileManager _instance;
		public static TileManager Instance { get { return _instance; } }

		private Action _currentAction;
		public Action CurrentAction
		{
			get { return _currentAction; }
			set
			{
				_currentAction = value;
				if (_currentAction != Action.SELECTAREA)
				{
					SelectedTiles = new List<Tile>();
				}
				if (_currentAction != Action.BUILDGHOST)
				{
					HoveredTile = null;
					GhostTile = null;
					Rotation = Rotation.Zero;
				}
			}
		}

		private List<Tile> _selectedTiles = new();
		public List<Tile> SelectedTiles
		{
			get { return _selectedTiles; }
			set
			{
				lock (_selectedTiles)
				{
					if (_selectedTiles.Count >= 2)
						foreach (Tile tile in GetTilesInArea(_selectedTiles[0], _selectedTiles[1])) { tile.Unhighlight(); }
					_selectedTiles = value;
					if (_selectedTiles.Count >= 2)
						foreach (Tile tile in GetTilesInArea(_selectedTiles[0], _selectedTiles[1])) { tile.Highlight(Color.white); }
				}
			}
		}

		private readonly object _ghostTileLock = new();
		private Tile _hoveredTile = null;
		private Tile _ghostTile = null;
		private Rotation _rotation = Rotation.Zero;
		public Tile HoveredTile
		{
			get { return _hoveredTile; }
			set
			{
				lock (_ghostTileLock)
				{
					_hoveredTile = value;
					if (_ghostTile != null && _hoveredTile != null)
					{
						_ghostTile.transform.SetPositionAndRotation(_hoveredTile.transform.position + _ghostTile.GetPivot() + new Vector3(0, 0.001f, 0), Quaternion.Euler(0, ((int)_rotation) * 90, 0));
						_ghostTile.TileModel.UpdateCoordinates((int)_hoveredTile.TileModel.Coordinates.x, (int)_hoveredTile.TileModel.Coordinates.y);
						_ghostTile.Highlight(_ghostTile.TileModel.CanBuild() ? Color.green : Color.red);
					}
				}
			}
		}
		public Tile GhostTile
		{
			get { return _ghostTile; }
			set
			{
				lock (_ghostTileLock)
				{
					Tile oldGhostTile = _ghostTile;
					_ghostTile = value;

					if (MainThreadDispatcher.Instance is MainThreadDispatcher mainThread)
					{
						mainThread.Enqueue(() =>
						{
							if (oldGhostTile != null) { Destroy(oldGhostTile.gameObject); }
							if (_ghostTile != null) { _ghostTile.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); }
							if (!(_ghostTile == null || _hoveredTile == null)) { _ghostTile.transform.SetPositionAndRotation(_hoveredTile.transform.position + new Vector3(0, 0.001f, 0), Quaternion.Euler(0, ((int)_rotation) * 90, 0)); }
						});
					}
				}
			}
		}
		public Rotation Rotation
		{
			get { return _rotation; }
			set
			{
				lock (_ghostTileLock)
				{
					_rotation = value;
					if (!(_ghostTile == null || _hoveredTile == null)) { _ghostTile.transform.SetPositionAndRotation(_hoveredTile.transform.position + new Vector3(0, 0.001f, 0), Quaternion.Euler(0, ((int)_rotation) * 90, 0)); }
				}
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

		public void MarkZone(ZoneType zoneType)
		{
			if (_selectedTiles.Count >= 2)
				SimEngine.Instance.ZoneManager.MarkZone(_selectedTiles[0].TileModel, _selectedTiles[1].TileModel, zoneType);
		}

		public Tile GenerateFromModel(Model.Tile tileModel)
		{
			Tile tileView = Instantiate(Resources.Load<GameObject>("Tiles/" + tileModel.GetType().Name + "/" + tileModel.GetType().Name), Vector3.zero, Quaternion.identity).GetComponent<Tile>();
			tileView.transform.localScale *= Tile.MODELSCALE;
			tileView.GetComponent<Tile>().Init(tileModel);
			tileView.transform.SetParent(transform);
			return tileView;
		}

		public void CloneTileFromModel(Model.Tile tileModel)
		{
			_tiles[(int)tileModel.Coordinates.x, (int)tileModel.Coordinates.y] = GenerateFromModel(tileModel).GetComponent<Tile>();
		}

		private Tile[,] _tiles;

		// Start is called before the first frame update
		void Start()
		{
			_instance = this;

			_tiles = new Tile[City.Instance.GetSize(), City.Instance.GetSize()];

			for (int i = 0; i < City.Instance.GetSize(); i++)
			for (int j = 0; j < City.Instance.GetSize(); j++)
			{
				CloneTileFromModel(City.Instance.GetTile(i, j));
			}
		}

		// Update is called once per frame
		void Update()
		{
			
		}
	}
}
