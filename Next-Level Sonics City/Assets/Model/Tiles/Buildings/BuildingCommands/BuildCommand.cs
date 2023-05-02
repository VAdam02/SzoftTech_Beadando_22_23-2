using Model.Simulation;
using System;

namespace Model.Tiles.Buildings.BuildingCommands
{
	public class BuildCommand : IExecutionCommand
	{
		private readonly TileType _tileType;
		private readonly Rotation _rotation;
		private readonly int _x;
		private readonly int _y;
		private readonly uint _designID;

		public BuildCommand(int x, int y, TileType tileType, Rotation rotation)
		{
			_tileType = tileType;
			_rotation = rotation;
			_x = x;
			_y = y;
			_designID = 0;
		}

		public void Execute()
		{
			Tile tile;
			switch (_tileType)
			{
				case TileType.PoliceDepartment:
					tile = new PoliceDepartment(_x, _y, _designID, _rotation);
					if (((Building)tile).IsExpandable() && !((Building)tile).CanExpand())
					{ throw new System.Exception("Not ennough space to build"); }
					break;
				case TileType.FireDepartment:
					tile = new FireDepartment(_x, _y, _designID, _rotation);
					if (((Building)tile).IsExpandable() && !((Building)tile).CanExpand())
					{ throw new System.Exception("Not ennough space to build"); }
					break;
				case TileType.MiddleSchool:
					tile = new MiddleSchool(_x, _y, _designID, _rotation);
					if (((Building)tile).IsExpandable() && !((Building)tile).CanExpand())
					{ throw new System.Exception("Not ennough space to build"); }
					break;
				case TileType.HighSchool:
					tile = new HighSchool(_x, _y, _designID, _rotation);
					if (((Building)tile).IsExpandable() && !((Building)tile).CanExpand())
					{ throw new System.Exception("Not ennough space to build"); }
					break;
				case TileType.Stadion:
					tile = new Stadion(_x, _y, _designID, _rotation);
					if (((Building)tile).IsExpandable() && !((Building)tile).CanExpand())
					{ throw new System.Exception("Not ennough space to build"); }
					break;
				case TileType.PowerPlant:
					tile = new PowerPlant(_x, _y, _designID, _rotation);
					if (((Building)tile).IsExpandable() && !((Building)tile).CanExpand())
					{ throw new System.Exception("Not ennough space to build"); }
					break;
				case TileType.Forest:
					tile = new Forest(_x, _y, _designID);
					break;
				case TileType.Road:
					tile = new Road(_x, _y, _designID);
					break;
				case TileType.ElectricPole:
					tile = new ElectricPole(_x, _y, _designID);
					break;
				default:
					throw new NotImplementedException("TileType \'" + _tileType + "\' not implemented");
			}

			SimEngine.Instance.SetTile(_x, _y, tile);

			if (tile is Building building && building.IsExpandable())
			{
				building.Expand();
			}
		}
	}
}