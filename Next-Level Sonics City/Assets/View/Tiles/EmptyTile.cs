using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model;

namespace View.Tiles
{
    public class EmptyTile : MonoBehaviour
    {
        private Model.Tile _tileModel;
        public Model.Tile TileModel { get { return _tileModel; } private set { _tileModel = value; } }

        // Start is called before the first frame update
        void Start()
        {
            transform.position = new Vector3(TileModel.Coordinates.x, 0, TileModel.Coordinates.y) * 50;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        /// <summary>
        /// Initializes the view side tile with it's model side object.
        /// </summary>
        /// <param name="tileModel">The tile model.</param>
        internal void Init(Model.Tile tileModel)
        {
            TileModel = tileModel;
        }
    }
}