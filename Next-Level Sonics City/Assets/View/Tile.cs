using Model;
using Model.Tiles;
using Model.Tiles.Buildings;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using View.GUI;

namespace View
{
	public class Tile : MonoBehaviour, IClickable
	{
		internal const float MODELSCALE = 1;

		private Model.Tile _tileModel;
		public Model.Tile TileModel { get { return _tileModel; } private set { _tileModel = value; } }

		internal readonly List<Material> _materials = new();

		public readonly UnityEvent<Model.Tile> OnTileDelete = new();
		public readonly UnityEvent OnDesignIDChange = new();

		/// <summary>
		/// Initializes the view side tile with it's model side object.
		/// Must be called before the Start is executed!
		/// </summary>
		/// <param name="tileModel">The tile model.</param>
		internal void Init(Model.Tile tileModel)
		{
			TileModel = tileModel;

			TileModel.OnTileDelete += (sender, tile) =>
			{
				if (MainThreadDispatcher.Instance is MainThreadDispatcher mainThread)
				{
					mainThread.Enqueue(() =>
					{
						OnTileDelete.Invoke(tile);
					});
				}
			};

			TileModel.OnTileChange += (sender, args) =>
			{
				if (MainThreadDispatcher.Instance is MainThreadDispatcher mainThread)
				{
					mainThread.Enqueue(() =>
					{
						OnDesignIDChange.Invoke();
					});
				}
			};

			OnTileDelete.AddListener(Delete);
			if (TileModel is Building building)
			{
				transform.localRotation = Quaternion.Euler(0, (int)building.Rotation * 90, 0);
				building.OnRotationChanged += (object sender, EventArgs e) => transform.localRotation = Quaternion.Euler(0, (int)building.Rotation * 90, 0);
			}
		}

		private void Delete(Model.Tile deletedTile)
		{
			TileManager.Instance.CloneTileFromModel(City.Instance.GetTile(TileModel));
			Destroy(gameObject);
		}

		public void Highlight(Color color)
		{
			foreach (Material material in _materials)
			{
				material.EnableKeyword("_EMISSION");
				material.SetVector("_EmissionColor", new Vector4(color.r, color.g, color.b, 1) * 0.75f);
			}
		}

		public void Unhighlight()
		{
			foreach (Material material in _materials)
			{
				material.DisableKeyword("_EMISSION");
			}
		}

		public virtual Vector3 GetPivot() { return Vector3.zero; }

		private GameObject _popUp;
		public virtual GameObject DisplayPopUp()
		{
			return null;
		}

		public void OnClick(bool isLeftMouseButton, Vector3 location)
		{
			if (TileManager.Instance.CurrentAction == Action.NONE)
			{
				if (_popUp != null)
				{
					Destroy(_popUp);
					_popUp = null;
				}
				_popUp = DisplayPopUp();
			}
			if (TileManager.Instance.CurrentAction == Action.SELECTAREA)
			{
				if (TileManager.Instance.SelectedTiles.Count == 0)
				{
					TileManager.Instance.SelectedTiles = new List<Tile>() { this, this };
				}
			}
			if (TileManager.Instance.CurrentAction == Action.BUILDGHOST)
			{
				if (TileManager.Instance.GhostTile != null)
				{
					BuildingManager.Instance.Build(
					City.Instance.GetTile(TileManager.Instance.GhostTile.TileModel),
					TileManager.Instance.GhostTile.TileModel.GetTileType(),
					TileManager.Instance.GhostTile.TileModel is Building building ? building.Rotation : Rotation.Zero);
				}
			}
			if (TileManager.Instance.CurrentAction == Action.SOFTDESTROY)
			{
				BuildingManager.Instance.Destroy(City.Instance.GetTile(TileModel));
			}
			if (TileManager.Instance.CurrentAction == Action.FORCEDESTROY)
			{
				BuildingManager.Instance.ForcedDestroy(City.Instance.GetTile(TileModel));
			}
		}

		public void OnDragStart(bool isLeftMouseButton, Vector3 location) { }

		public bool OnDrag(bool isLeftMouseButton, Vector3 direction) { return true; }

		public void OnDragEnd(bool isLeftMouseButton) { }

		public void OnSecondClick(List<IClickable> clicked)
		{
			if (_popUp != null)
			{
				Destroy(_popUp);
				_popUp = null;
			}

			if (TileManager.Instance.CurrentAction != Action.SELECTAREA) { return; }

			Tile tile = (Tile)clicked.Find(item => item is Tile);
			if (tile == null) { TileManager.Instance.SelectedTiles = new(); return; }

			TileManager.Instance.SelectedTiles = new List<Tile>() { this, tile };
		}

		public void OnHoverStart(Vector3 location)
		{
			if (TileManager.Instance.CurrentAction == Action.BUILDGHOST
			 || TileManager.Instance.CurrentAction == Action.SOFTDESTROY
			 || TileManager.Instance.CurrentAction == Action.FORCEDESTROY)
			{
				TileManager.Instance.HoveredTile = this;
			}
		}

		public void OnHover(Vector3 location)
		{
			
		}

		public void OnHoverEnd()
		{
			
		}

		public void OnScroll(float delta) { }
	}
}