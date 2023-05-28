using Model.Persons;
using Model.RoadGrids;
using Model.Statistics;
using Model.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

			int n = 20;
			_tiles = new Tile[n, n];
			for (int i = 0; i < n; i++)
			for (int j = 0; j < n; j++)
			{
				SetTile(new EmptyTile(i, j));
			}

			float scale = 0.25f;
			for (int i = 0; i < n; i++)
			for (int j = 0; j < n; j++)
			{
				if (Mathf.PerlinNoise(i * scale, j * scale) > 0.5f)
				{
					SetTile(new ForestTile(i, j, ForestTile.MAINTANCENEEDEDFORYEAR + 1));
				}
			}

			StatEngine.Instance.ResidentialTaxChanged += ResidentialTaxChangeHandler;
			StatEngine.Instance.NextQuarterEvent += NextQuarter;
		}


		private void NextQuarter(object sender, EventArgs e)
		{
			float newPersonMultiplier = Mathf.Pow(Mathf.Sin((CityHappiness-1) * (float)Math.PI / 2) + 1, 8) / 2;
			int minNewPersonCount = 1;

			object newPersonShouldMoveInLock = new();	
			int newPersonShouldMoveIn = Mathf.RoundToInt(GetPopulation() * Mathf.Min(newPersonMultiplier, 0.1f) + minNewPersonCount);
			System.Random rnd = new();

			for (int i = 0; i < newPersonShouldMoveIn; i++)
			{
				List<RoadGrid> roadGrids = RoadGridManager.Instance.RoadGrids.FindAll((item) =>
				{
					return item.FreeWorkplaces.Count > 0 && item.FreeResidentials.Count > 0;
				}) ?? new();

				while (newPersonShouldMoveIn > 0 && roadGrids.Count > 0)
				{
					Parallel.For(0, Mathf.Min(roadGrids.Count, newPersonShouldMoveIn), (index) =>
					{
						RoadGrid target = roadGrids[index];

						IWorkplace workplace;
						if (target.FreeOtherWorkplaces.Count > 0 && rnd.Next(0, 3) == 0)
						{
							workplace = target.FreeOtherWorkplaces[rnd.Next(0, target.FreeOtherWorkplaces.Count)];
						}

						IWorkplace canBeOther = target.FreeOtherWorkplaces.Count > 0 ? target.FreeOtherWorkplaces[rnd.Next(0, target.FreeOtherWorkplaces.Count)] : null;
						IWorkplace canBeComOrInd = rnd.NextDouble() < StatEngine.Instance.GetCommercialToIndustrialRate() ? 
												(target.FreeIndustrialWorkplaces.Count > 0 ? target.FreeIndustrialWorkplaces[rnd.Next(0, target.FreeIndustrialWorkplaces.Count)] : null) :
												(target.FreeCommercialWorkplaces.Count > 0 ? target.FreeCommercialWorkplaces[rnd.Next(0, target.FreeCommercialWorkplaces.Count)] : null);
						if (canBeOther != null && canBeComOrInd != null)
						{
							workplace = rnd.Next(0, 2) == 0 ? canBeOther : canBeComOrInd;
						}
						else
						{
							workplace = canBeOther ?? canBeComOrInd;
						}

						IRoadGridElement workplaceroad = RoadGridManager.GetRoadGrigElementByBuilding((Building)workplace);

						IResidential residential = null;
						List<IRoadGridElement> shortestPath = null;
						foreach (IResidential curResidential in target.FreeResidentials)
						{
							try
							{
								List<IRoadGridElement> curPath = RoadGridManager.GetPathOnRoad(RoadGridManager.GetRoadGrigElementByBuilding((Building)curResidential), workplaceroad, shortestPath == null ? int.MaxValue : shortestPath.Count);
								if (curPath.Count < (shortestPath == null ? int.MaxValue : shortestPath.Count))
								{
									shortestPath = curPath;
									residential = curResidential;
								}
							}
							catch
							{
								
							}
						}

						if (workplace == null || residential == null) { return; }

						Worker worker = new(residential, workplace, rnd.Next(18, 60), Qualification.LOW, shortestPath);
						lock (newPersonShouldMoveInLock)
						{
							newPersonShouldMoveIn--;
						}
						Debug.Log(worker.ID + " moved in to " + residential.GetTile().Coordinates + " and works at " + workplace.GetTile().Coordinates + " and path is " + shortestPath.Count);
					});

					roadGrids = RoadGridManager.Instance.RoadGrids.FindAll((item) =>
					{
						return item.FreeWorkplaces.Count > 0 && item.FreeResidentials.Count > 0;
					}) ?? new();
				}
			}
		}

		public event EventHandler CityHappinessChanged;
		private readonly object _cityHappinessLock = new();
		private (float sumHappiness, int count) _cityHappiness = (0, 0);
		public float CityHappiness { get => _cityHappiness.sumHappiness / (_cityHappiness.count == 0 ? 1 : _cityHappiness.count); }
		private void PersonHappinessChangedHandler(Person sender, float oldHappiness)
		{
			lock (_cityHappinessLock)
			{
				_cityHappiness.sumHappiness += sender.Happiness - oldHappiness;
			}

			CityHappinessChanged?.Invoke(this, EventArgs.Empty);
		}

		public event EventHandler HappinessByCityChanged;
		public (float happiness, float weight) HappinessByCity
		{
			get
			{
				float happinessSum = _happinessChangers.Aggregate(0.0f, (acc, item) => acc + item.happiness * item.weight);
				float happinessWeight = _happinessChangers.Aggregate(0.0f, (acc, item) => acc + item.weight);
				return (happinessSum / (happinessWeight == 0 ? 1 : happinessWeight), happinessWeight);
			}
		}

		private readonly List<(string type, float happiness, float weight)> _happinessChangers = new();
		private void ResidentialTaxChangeHandler(object sender, (float oldVal, float newVal) e)
		{
			_happinessChangers.RemoveAll((item) => item.type == "ResidentialTax");
			_happinessChangers.Add(("ResidentialTax", 0, Mathf.Pow(Mathf.Tan(e.newVal * MathF.PI / 2), 2) * 20));
			HappinessByCityChanged?.Invoke(this, new EventArgs());
		}

		private void NegativeBudgetYearEllapsedHandler(object sender, EventArgs e)
		{
			if (StatEngine.Instance.Quarter == 3)
			{
				_happinessChangers.RemoveAll(item => item.type == "NegativeBudget");
				if (StatEngine.Instance.NegativeBudgetSince != 0)
				{
					_happinessChangers.Add(("NegativeBudget", 0, Mathf.Tan(StatEngine.Instance.NegativeBudgetSince * MathF.PI / 20) * 10));
					HappinessByCityChanged?.Invoke(this, new EventArgs());
				}
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
			lock (_cityHappinessLock)
			{
				_cityHappiness.sumHappiness += person.Happiness;
				_cityHappiness.count++;
			}
			person.HappinessByPersonChanged += (sender, e) => PersonHappinessChangedHandler((Person)sender, e);
		}
	}
}