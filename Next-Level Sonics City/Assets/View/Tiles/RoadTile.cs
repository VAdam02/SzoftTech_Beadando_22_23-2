using UnityEngine;

namespace View.Tiles
{
	public class RoadTile : Tile
	{
		// Start is called before the first frame update
		void Start()
		{
			transform.localPosition = new Vector3(TileModel.Coordinates.x, 0, -TileModel.Coordinates.y) * 10;
			transform.localScale = Vector3.one;

			TileModel.DesignIDChangeEvent.AddListener(OnDesignIDChange);
		}

		// Update is called once per frame
		void Update()
		{

		}

		private void OnDesignIDChange()
		{
			
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}

			int dirCount = 0;
			if ((TileModel.DesignID & Model.Tiles.RoadTile.ABOVEROADMASK) != 0) { dirCount++; }
			if ((TileModel.DesignID & Model.Tiles.RoadTile.RIGHTROADMASK) != 0) { dirCount++; }
			if ((TileModel.DesignID & Model.Tiles.RoadTile.BELOWROADMASK) != 0) { dirCount++; }
			if ((TileModel.DesignID & Model.Tiles.RoadTile.LEFTROADMASK)  != 0) { dirCount++; }

			GameObject road;
			Vector3 rotation = new(0, 0, 0);

			if ((TileModel.DesignID & Model.Tiles.RoadTile.ABOVEROADMASK) != 0) { rotation = new(0, 0, 0); }
			else if ((TileModel.DesignID & Model.Tiles.RoadTile.RIGHTROADMASK) != 0) { rotation = new(0, 0, 90); }
			else if ((TileModel.DesignID & Model.Tiles.RoadTile.BELOWROADMASK) != 0) { rotation = new(0, 0, 180); }
			else if ((TileModel.DesignID & Model.Tiles.RoadTile.LEFTROADMASK) != 0) { rotation = new(0, 0, 270); }

			if (dirCount == 0)
			{
				road = Instantiate(LoadModelByName("0direction"));
			}
			else if (dirCount == 1)
			{
				road = Instantiate(LoadModelByName("1direction"));
			}
			else if (dirCount == 2)
			{
				if (((TileModel.DesignID & Model.Tiles.RoadTile.ABOVEROADMASK) != 0 &&
					(TileModel.DesignID & Model.Tiles.RoadTile.BELOWROADMASK) != 0) ||
					((TileModel.DesignID & Model.Tiles.RoadTile.RIGHTROADMASK) != 0 &&
					(TileModel.DesignID & Model.Tiles.RoadTile.LEFTROADMASK) != 0))
				{
					road = Instantiate(LoadModelByName("2direction"));
				}
				else
				{
					road = Instantiate(LoadModelByName("2directionTurn"));
					if ((TileModel.DesignID & (Model.Tiles.RoadTile.ABOVEROADMASK | Model.Tiles.RoadTile.BELOWROADMASK | Model.Tiles.RoadTile.RIGHTROADMASK | Model.Tiles.RoadTile.LEFTROADMASK) & 0b1001) == 0b1001)
					{
						rotation = new(0, 0, 270);
					}
				}
			}
			else if (dirCount == 3)
			{
				road = Instantiate(LoadModelByName("3direction"));
				if ((TileModel.DesignID & (Model.Tiles.RoadTile.ABOVEROADMASK | Model.Tiles.RoadTile.BELOWROADMASK | Model.Tiles.RoadTile.RIGHTROADMASK | Model.Tiles.RoadTile.LEFTROADMASK) & 0b1001) == 0b1011)
				{
					rotation = new(0, 0, 180);
				}
				else if ((TileModel.DesignID & (Model.Tiles.RoadTile.ABOVEROADMASK | Model.Tiles.RoadTile.BELOWROADMASK | Model.Tiles.RoadTile.RIGHTROADMASK | Model.Tiles.RoadTile.LEFTROADMASK) & 0b1001) == 0b1101)
				{
					rotation = new(0, 0, 270);
				}
			}
			else
			{
				road = Instantiate(LoadModelByName("4direction"));
			}

			road.transform.parent = transform;
			road.transform.localScale = Vector3.one * 20;
			road.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(-90, 0, 0));
			road.transform.Rotate(rotation);

			/*
			if (dirCount == 0)
			{
				GameObject road = Instantiate(LoadModelByName("0direction.fbx"));
			}
			*/
		}

		private static GameObject LoadModelByName(string name)
		{
			return Resources.Load<GameObject>("Tiles/RoadTile/Model/" + name);
		}
	}
}