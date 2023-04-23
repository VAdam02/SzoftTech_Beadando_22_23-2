using Model.Tiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.Tiles;
using Model.Tiles.Buildings;

namespace Model.Tiles.ZoneCommands
{
	public class UnMarkZoneCommand : IExecutionCommand
	{
		private Tile[,] _matrix;
		private int _x;
		private int _y;
		private uint _designID;

		public UnMarkZoneCommand(Tile[,] matrix, int x, int y)
		{
			_matrix = matrix;
			_x = x;
			_y = y;
			_designID = _matrix[_x, _y].DesignID;
		}

		public void Execute()
		{
			_matrix[_x, _y] = new EmptyTile(_x, _y, _designID);
		}
	}
}