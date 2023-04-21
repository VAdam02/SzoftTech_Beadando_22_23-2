using System.Collections.Generic;
using UnityEngine;

namespace View
{
	public class Tile : MonoBehaviour, IClickable
	{
		internal const float MODELSCALE = 1.0f/1.0f;

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

		public bool OnDrag(bool isLeftMouseButton, Vector3 direction)
		{
			return true;
		}

		public void OnDragEnd(bool isLeftMouseButton)
		{
			
		}

		public void OnDragStart(bool isLeftMouseButton, Vector3 location)
		{

		}

		public void OnSecondClick(List<IClickable> clicked)
		{
			Debug.Log("Tile second clicked! " + clicked);
		}
	}
}