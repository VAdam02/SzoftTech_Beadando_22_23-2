using Model.Persons;
using Model.RoadGrids;
using Model.Simulation;
using Model.Statistics;
using Model.Tiles;
using Model.Tiles.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Model
{
	public class City
	{
		public readonly Vector2 PERLINNOISEDELTA;

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

		public event EventHandler<Person> PopulationChanged;

		/// <summary>
		/// Create a city filled with empty tiles
		/// </summary>
		private City()
		{
			_instance = this;

			int n = 50;
			_tiles = new Tile[n, n];
			for (int i = 0; i < n; i++)
			for (int j = 0; j < n; j++)
			{
				SetTile(new EmptyTile(i, j));
			}

            System.Random rnd = new();
			PERLINNOISEDELTA = new Vector2(rnd.Next(0, (int)((float)int.MaxValue / 1000 / n)), rnd.Next(0, (int)((float)int.MaxValue / 1000 / n)));

			float scale = 0.25f;
			for (int i = 0; i < n; i++)
			for (int j = 0; j < n; j++)
			{
				if (Mathf.PerlinNoise(PERLINNOISEDELTA.x + i * scale, PERLINNOISEDELTA.y + j * scale) > 0.6f)
				{
					SetTile(new ForestTile(i, j, ForestTile.MAINTANCENEEDEDFORYEAR + 1));
				}
			}

			ResidentialTaxChangeHandler(this, (StatEngine.Instance.ResidentialTaxRate, StatEngine.Instance.ResidentialTaxRate));
			StatEngine.Instance.ResidentialTaxChanged += ResidentialTaxChangeHandler;
			StatEngine.Instance.NextQuarterEvent += NextQuarter;
			NegativeBudgetYearElapsedHandler(this, new EventArgs());
			StatEngine.Instance.NegativeBudgetYearElapsed += NegativeBudgetYearElapsedHandler;
			CommercialToIndustrialWorkersRatioHandler(this, new EventArgs());
			StatEngine.Instance.CommercialToIndustrialWorkerRateChanged += CommercialToIndustrialWorkersRatioHandler;

			Tile t1 = GetTile(4, 4);
			Tile t2 = GetTile(8, 8);

			BuildingManager.Instance.Build(t1, TileType.MiddleSchool, Rotation.Zero);
			BuildingManager.Instance.Build(t2, TileType.HighSchool, Rotation.Zero);
		}


		private void NextQuarter(object sender, EventArgs e)
		{
			float newPersonMultiplier = Mathf.Pow(Mathf.Sin((CityHappiness-1) * (float)Math.PI / 2) + 1, 8) / 2;
			int minNewPersonCount = 1;

			object newPersonShouldMoveInLock = new();	
			int newPersonShouldMoveIn = Mathf.RoundToInt(GetPopulation() * Mathf.Min(newPersonMultiplier, 0.1f) + minNewPersonCount);

			List<RoadGrid> roadGrids = RoadGridManager.Instance.RoadGrids.FindAll((item) =>
			{
				return item.FreeWorkplaces.Count > 0 && item.FreeResidentials.Count > 0;
			}) ?? new();

			while (newPersonShouldMoveIn > 0 && roadGrids.Count > 0)
			{
				Parallel.For(0, Mathf.Min(roadGrids.Count, newPersonShouldMoveIn), (index) =>
				{
					RoadGrid target = roadGrids[index];

					IWorkplace workplace = Person.LookForWorkplaceInRoadGrid(target);
					IResidential residential = Person.LookForResidentialByWorkplace(workplace, out List<IRoadGridElement> shortestPath);

					if (workplace == null || residential == null) { return; }

					System.Random rnd = new();
					Worker worker = new(residential, workplace, rnd.Next(18, 60), Qualification.LOW, shortestPath);
					lock (newPersonShouldMoveInLock)
					{
						newPersonShouldMoveIn--;
					}
				});

				roadGrids = RoadGridManager.Instance.RoadGrids.FindAll((item) =>
				{
					return item.FreeWorkplaces.Count > 0 && item.FreeResidentials.Count > 0;
				}) ?? new();
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

		private void NegativeBudgetYearElapsedHandler(object sender, EventArgs e)
		{
			_happinessChangers.RemoveAll(item => item.type == "NegativeBudgetTime");
			_happinessChangers.RemoveAll(item => item.type == "NegativeBudgetVolume");
			if (StatEngine.Instance.NegativeBudgetSince != 0)
			{
				_happinessChangers.Add(("NegativeBudgetTime", 0, Mathf.Tan(Mathf.Clamp(StatEngine.Instance.NegativeBudgetSince, 0, 9.99f) * MathF.PI / 20) * 10));
				_happinessChangers.Add(("NegativeBudgetVolume", 0, MathF.Tan(Mathf.Clamp(-StatEngine.Instance.Budget, 0, 99999) * MathF.PI / 200000)));
				HappinessByCityChanged?.Invoke(this, new EventArgs());
			}
		}

		private void CommercialToIndustrialWorkersRatioHandler(object sender, EventArgs e)
		{
			_happinessChangers.RemoveAll(item => item.type == "CommercialToIndustrialWorkersRatio");
			_happinessChangers.Add(("CommercialToIndustrialWorkersRatio", 0, Mathf.Pow(Mathf.Sin((StatEngine.Instance.GetCommercialWorkersPercentToCommercialAndIndustrialWorkers() - 1) * Mathf.PI) + 2, 3)));
			HappinessByCityChanged?.Invoke(this, new EventArgs());
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
		/// <param name="person">Person to add to the city</param>
		internal void AddPerson(Person person)
		{
			_persons.Add(person.ID, person);
			lock (_cityHappinessLock)
			{
				_cityHappiness.sumHappiness += person.Happiness;
				_cityHappiness.count++;
			}
			person.HappinessByPersonChanged += (sender, e) => PersonHappinessChangedHandler((Person)sender, e);
			PopulationChanged?.Invoke(this, person);
		}

		/// <summary>
		/// Remove person to the city
		/// </summary>
		/// <param name="person">Person to remove from the city</param>
		internal void RemovePerson(Person person)
		{
			_persons.Remove(person.ID);
			lock (_cityHappinessLock)
			{
				_cityHappiness.sumHappiness -= person.Happiness;
				_cityHappiness.count--;
			}
			person.HappinessByPersonChanged += (sender, e) => PersonHappinessChangedHandler((Person)sender, e);
			PopulationChanged?.Invoke(this, person);
		}

		private int _middleSchoolEducatedCount = 0;
		private int _highSchoolEducatedCount = 0;
		private float _maxMiddleSchoolEducatedPercentage = 0.4f;
		private float _maxHighSchoolEducatedPercentage = 0.2f;

		internal void MiddleSchoolEducatePersons()
		{
			int populationCount = GetPopulation();
			float currentEducatedPercentage = _middleSchoolEducatedCount / ((float)populationCount);
			
			if (currentEducatedPercentage >= _maxMiddleSchoolEducatedPercentage)
			{
				return;
			}

			List<RoadGrid> roadGrids = RoadGridManager.Instance.RoadGrids;
			int schoolCount = 0;

			foreach (RoadGrid roadGrid in roadGrids)
			{
				schoolCount += roadGrid.MiddleSchools.Count;
			}

			int canBeEducatedCount = (int)(populationCount * _maxMiddleSchoolEducatedPercentage) - (int)(populationCount * currentEducatedPercentage);
			int toBeEducatedCount = Math.Min(canBeEducatedCount, schoolCount * 10);

			foreach (var person in GetPersons())
			{
				if (person.Value is Worker worker && worker.Qualification == Qualification.LOW && toBeEducatedCount == 0)
				{
					worker.IncreaseQualification();
					++_middleSchoolEducatedCount;
					--toBeEducatedCount;
				}
			}
		}

		internal void HighSchoolEducatePersons()
		{
			int populationCount = GetPopulation();
			float currentEducatedPercentage = _highSchoolEducatedCount / ((float)populationCount);

			if (currentEducatedPercentage >= _maxHighSchoolEducatedPercentage)
			{
				return;
			}

			List<RoadGrid> roadGrids = RoadGridManager.Instance.RoadGrids;
			int schoolCount = 0;

			foreach (RoadGrid roadGrid in roadGrids)
			{
				schoolCount += roadGrid.HighSchools.Count;
			}

			int canBeEducatedCount = (int)(populationCount * _maxHighSchoolEducatedPercentage) - (int)(populationCount * currentEducatedPercentage);
			int toBeEducatedCount = Math.Min(canBeEducatedCount, schoolCount * 5);

			foreach (var person in GetPersons())
			{
				if (person.Value is Worker worker && worker.Qualification == Qualification.MID && toBeEducatedCount == 0)
				{
					worker.IncreaseQualification();
					++_highSchoolEducatedCount;
					--toBeEducatedCount;
				}
			}
		}

		internal void LoseMiddleSchoolEducation()
		{
			int toBeDecreasedCount = _middleSchoolEducatedCount / 10;

			foreach (var person in GetPersons())
			{
				if (person.Value is Worker worker && worker.Qualification == Qualification.MID && toBeDecreasedCount == 0)
				{
					worker.DecreaseQualification();
					--_middleSchoolEducatedCount;
					--toBeDecreasedCount;
				}
			}
		}

		internal void LoseHighSchoolEducation()
		{
			int toBeDecreasedCount = _highSchoolEducatedCount / 20;

			foreach (var person in GetPersons())
			{
				if (person.Value is Worker worker && worker.Qualification == Qualification.HIGH && toBeDecreasedCount == 0)
				{
					worker.DecreaseQualification();
					--_highSchoolEducatedCount;
					--toBeDecreasedCount;
				}
			}
		}
	}
}