using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View
{
	public class Tile : MonoBehaviour
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
	}
}