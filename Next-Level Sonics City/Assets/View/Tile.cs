using Model.Simulation;
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
			TileManager.Instance.CloneTileFromModel(SimEngine.Instance.GetTile((int)TileModel.Coordinates.x, (int)TileModel.Coordinates.y));
			Destroy(gameObject);
		}

		public void Highlight()
		{
			foreach (Material material in _materials)
			{
				material.EnableKeyword("_EMISSION");
				material.SetVector("_EmissionColor", new Vector4(1, 1, 1, 1) * 0.75f);
			}
		}

		public void Unhighlight()
		{
			foreach (Material material in _materials)
			{
				material.DisableKeyword("_EMISSION");
			}
		}

		public void OnClick(bool isLeftMouseButton, Vector3 location)
		{
			if (TileManager.Instance.CurrentAction == Action.SELECTAREA)
			{
				if (TileManager.Instance.SelectedTiles.Count == 0)
				{
					TileManager.Instance.SelectedTiles = new List<Tile>() { this, this };
				}
			}

			if (TileManager.Instance.CurrentAction == Action.BUILDROAD)
			{
				TileManager.Instance.SelectedTile = this;
				SimEngine.Instance.BuildingManager.Build(TileManager.Instance.SelectedTile.TileModel, TileType.Road, Model.Tiles.Buildings.Rotation.Zero);
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
			//Debug.Log("HoverStart " + location + "\t" + this);
		}

		public void OnHover(Vector3 location)
		{
			//Debug.Log("Hover " + location + "\t" + this);
		}

		public void OnHoverEnd()
		{
			//Debug.Log("HoverEnd" + "\t" + this);
		}

		public void OnScroll(float delta) { }
	}
}