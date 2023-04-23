using System.Collections.Generic;
using UnityEngine;

namespace View
{
	public class Tile : MonoBehaviour, IClickable
	{
		internal const float MODELSCALE = 1;

		private Model.Tile _tileModel;
		public Model.Tile TileModel { get { return _tileModel; } private set { _tileModel = value; } }

		/// <summary>
		/// Initializes the view side tile with it's model side object.
		/// Must be called before the Start is executed!
		/// </summary>
		/// <param name="tileModel">The tile model.</param>
		internal void Init(Model.Tile tileModel)
		{
			TileModel = tileModel;
		}

		public void OnClick(bool isLeftMouseButton, Vector3 location)
		{
			Debug.Log("Tile clicked! " + isLeftMouseButton + " " + location);
		}

		public void OnDragStart(bool isLeftMouseButton, Vector3 location) { }

		public bool OnDrag(bool isLeftMouseButton, Vector3 direction) { return true; }

		public void OnDragEnd(bool isLeftMouseButton) { }

		public void OnSecondClick(List<IClickable> clicked)
		{
			Debug.Log("Tile second clicked! " + clicked + "\t" + this);
		}

		public void OnHoverStart(Vector3 location)
		{
			Debug.Log("HoverStart " + location + "\t" + this);
		}

		public void OnHover(Vector3 location)
		{
			//Debug.Log("Hover " + location + "\t" + this);
		}

		public void OnHoverEnd()
		{
			Debug.Log("HoverEnd" + "\t" + this);
		}

		public void OnScroll(float delta) { }
	}
}