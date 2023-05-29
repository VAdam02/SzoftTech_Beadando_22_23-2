using Model;
using Model.Tiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcedDestroyCommand : IExecutionCommand
{
	private readonly int _x;
	private readonly int _y;

	/// <summary>
	/// Create a new DestroyCommand
	/// </summary>
	/// <param name="x">X coordinate of the destroyed tile</param>
	/// <param name="y">Y coordinate of the destroyed tile</param>
	public ForcedDestroyCommand(int x, int y)
	{
		_x = x;
		_y = y;
	}

	/// <summary>
	/// Destroy the tile
	/// </summary>
	public void Execute()
	{
		City.Instance.SetTile(new EmptyTile(_x, _y));
	}
}
