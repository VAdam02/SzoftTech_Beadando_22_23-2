using Model;
using Model.Simulation;
using Model.Tiles;
using Model.Tiles.Buildings;
using System.Collections.Generic;
using UnityEngine;
using View.GUI;

namespace View
{
	public class Tile : MonoBehaviour, IClickable
	{
		internal const float MODELSCALE = 1;

		private Model.Tile _tileModel;
		public Model.Tile TileModel { get { return _tileModel; } private set { _tileModel = value; } }

		internal readonly List<Material> _materials = new();

		/// <summary>
		/// Initializes the view side tile with it's model side object.
		/// Must be called before the Start is executed!
		/// </summary>
		/// <param name="tileModel">The tile model.</param>
		internal void Init(Model.Tile tileModel)
		{
			TileModel = tileModel;
			TileModel.OnTileDelete.AddListener(Delete);
		}

		private void Delete()
		{
			TileManager.Instance.CloneTileFromModel(City.Instance.GetTile(TileModel.Coordinates));
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

		public void OnClick(bool isLeftMouseButton, Vector3 location)
		{
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
					City.Instance.GetTile(TileManager.Instance.GhostTile.TileModel.Coordinates),
					TileManager.Instance.GhostTile.TileModel.GetTileType(),
					TileManager.Instance.GhostTile.TileModel is Building building ? building.Rotation : Rotation.Zero);
				}
			}
		}

		public void OnDragStart(bool isLeftMouseButton, Vector3 location) { }

		public bool OnDrag(bool isLeftMouseButton, Vector3 direction) { return true; }

		public void OnDragEnd(bool isLeftMouseButton) { }

		public void OnSecondClick(List<IClickable> clicked)
		{
			if (TileManager.Instance.CurrentAction != Action.SELECTAREA) { return; }

			Tile tile = (Tile)clicked.Find(item => item is Tile);
			if (tile == null) { TileManager.Instance.SelectedTiles = new(); return; }

			TileManager.Instance.SelectedTiles = new List<Tile>() { this, tile };
		}

		public void OnHoverStart(Vector3 location)
		{
			//Debug.Log("HoverStart\t" + location);
		}

		public void OnHover(Vector3 location)
		{
			if (TileManager.Instance.CurrentAction == Action.BUILDGHOST)
			{
				TileManager.Instance.HoveredTile = this;
			}
		}

		public void OnHoverEnd()
		{
			//Debug.Log("HoverEnd" + "\t" + this);
		}

		public void OnScroll(float delta) { }
	}
}