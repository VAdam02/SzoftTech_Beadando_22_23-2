using Model.Tiles;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
	public class City
	{
		private static City _instance;
		public static City Instance
		{
			get
			{
				_instance ??= new City();
				return _instance;
			}
		}

		public static void Reset() { _instance = null; }

		/// <summary>
		/// Create a city filled with empty tiles
		/// </summary>
		private City()
		{
			_instance = this;
			
			int n = 10;
			_tiles = new Tile[n, n];
			for (int i = 0; i < n; i++)
			for (int j = 0; j < n; j++)
			{
				SetTile(new EmptyTile(i, j, 0));
			}
		}

		private readonly Tile[,] _tiles;

		/// <summary>
		/// Get a tile at the given coordinates
		/// </summary>
		/// <param name="tile">tile of the requested tile</param>
		/// <returns>Tile at the location</returns>
		public Tile GetTile(Tile tile) { return GetTile(tile.Coordinates); }

		/// <summary>
		/// Get a tile at the given coordinates
		/// </summary>
		/// <param name="coordinates">Coordinates of the requested tile</param>
		/// <returns>Tile at the location</returns>
		public Tile GetTile(Vector3 coordinates) { return GetTile(coordinates.x, coordinates.y); }

		/// <summary>
		/// Get a tile at the given coordinates
		/// </summary>
		/// <param name="x">X coordinate of the requested tile</param>
		/// <param name="y">Y coordinate of the requested tile</param>
		/// <returns>Tile at the location</returns>
		public Tile GetTile(float x, float y) { return GetTile((int)x, (int)y); }

		/// <summary>
		/// Get a tile at the given coordinates
		/// </summary>
		/// <param name="x">X coordinate of the requested tile</param>
		/// <param name="y">Y coordinate of the requested tile</param>
		/// <returns>Tile at the location</returns>
		public Tile GetTile(int x, int y)
		{
			if (!(0 <= x && x < _tiles.GetLength(0) && 0 <= y && y < _tiles.GetLength(1))) return null;

			return _tiles[x, y];
		}

		/// <summary>
		/// Set a tile to the tile's coordinates
		/// </summary>
		/// <param name="tile">Tile which should be set</param>
		public void SetTile(Tile tile)
		{
			Tile old = _tiles[(int)tile.Coordinates.x, (int)tile.Coordinates.y];
			_tiles[(int)tile.Coordinates.x, (int)tile.Coordinates.y] = tile;
			tile.FinalizeTile();
			old?.DeleteTile();
		}

		/// <summary>
		/// Get the size of the city
		/// </summary>
		/// <returns>Size of the city</returns>
		public int GetSize()
		{
			return _tiles.GetLength(0);
		}

		private readonly SortedDictionary<ulong, Person> _persons = new();

		/// <summary>
		/// Get the population of the city
		/// </summary>
		/// <returns></returns>
		public int GetPopulation()
		{
			return _persons.Count;
		}

		/// <summary>
		/// Get person list of the city
		/// </summary>
		/// <returns>List of persons in the city</returns>
		public SortedDictionary<ulong, Person> GetPersons()
		{
			return _persons;
		}

		/// <summary>
		/// Add person to the city
		/// </summary>
		/// <param name="id"></param>
		/// <param name="person"></param>
		internal void AddPerson(Person person)
		{
			_persons.Add(person.ID, person);
		}
	}
}