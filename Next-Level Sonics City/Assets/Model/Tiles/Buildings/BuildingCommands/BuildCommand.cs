using System;

namespace Model.Tiles.Buildings.BuildingCommands
{
	public class BuildCommand : IExecutionCommand
	{
		private readonly int _x;
		private readonly int _y;
		private readonly TileType _tileType;
		private readonly Rotation _rotation;
		private readonly uint _designID;

		/// <summary>
		/// Creates a new BuildCommand
		/// </summary>
		/// <param name="x">X coordinate of the created tile</param>
		/// <param name="y">Y coordinate of the created tile</param>
		/// <param name="tileType">Type of the tile</param>
		/// <param name="rotation">Rotation of the tile</param>
		public BuildCommand(int x, int y, TileType tileType, Rotation rotation) : this(x, y, tileType, rotation, 0) { }

		/// <summary>
		/// Creates a new BuildCommand
		/// </summary>
		/// <param name="x">X location of the created tile</param>
		/// <param name="y">Y location of the created tile</param>
		/// <param name="tileType">Type of the tile</param>
		/// <param name="rotation">Rotation of the tile</param>
		/// <param name="designID">DesignID of the tile</param>
		public BuildCommand(int x, int y, TileType tileType, Rotation rotation, uint designID)
		{
			if (x < 0 || y < 0) { throw new ArgumentException("X and Y must be positive"); }
			if (City.Instance.GetSize() <= x || City.Instance.GetSize() <= y) { throw new ArgumentException("X and Y must be smaller than the city size"); }

			_x = x;
			_y = y;
			_tileType = tileType;
			_rotation = rotation;
			_designID = designID;
		}

		/// <summary>
		/// Build the tile
		/// </summary>
		public void Execute()
		{
			Tile tile = _tileType switch
			{
				TileType.PoliceDepartment => new PoliceDepartmentBuildingTile(_x, _y, _designID, _rotation),
				TileType.FireDepartment => new FireDepartment(_x, _y, _designID, _rotation),
				TileType.MiddleSchool => new MiddleSchool(_x, _y, _designID, _rotation),
				TileType.HighSchool => new HighSchool(_x, _y, _designID, _rotation),
				TileType.Stadion => new StadionBuildingTile(_x, _y, _designID, _rotation),
				TileType.PowerPlant => new PowerPlant(_x, _y, _designID, _rotation),
				TileType.Forest => new Forest(_x, _y, _designID),
				TileType.Road => new RoadTile(_x, _y, _designID),
				TileType.ElectricPole => new ElectricPole(_x, _y, _designID),
				_ => throw new NotImplementedException("TileType \'" + _tileType + "\' not implemented"),
			};

			if (!tile.CanBuild()) { throw new System.Exception("Not ennough space to build"); }

			City.Instance.SetTile(tile);

			if (tile is Building building)
			{
				building.Expand();
			}
		}
	}
}