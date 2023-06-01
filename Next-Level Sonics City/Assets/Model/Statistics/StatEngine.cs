using Model.Persons;
using Model.RoadGrids;
using Model.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Model.Statistics
{
	public class StatEngine
	{
		private static StatEngine _instance;
		public static StatEngine Instance
		{
			get
			{
				_instance ??= new StatEngine(2020, 100000);
				return _instance;
			}
		}

		public static void Reset() { _instance = null; }

		private DateTime _date;
		public DateTime Date
		{
			get { return _date; }
			private set
			{
				bool changed = _date.Date != value.Date;
				int oldQuarter = Quarter;

				_date = value;

				if (changed)
				{
					DateChanged?.Invoke(this, new EventArgs());

					if (Quarter != oldQuarter)
					{
						NextQuarter();
					}
				}
			}
		}
		public int Year { get { return Date.Year; } }
		public int Quarter { get { return (Date.Month-1) / 3; } }

		private readonly object _budgetLock = new();
		private float _budget = 0;
		private int _negativeBudgetSince = 0;
		public float Budget
		{
			get { return _budget; }
			private set
			{
				_budget = value;
				if (_budget < 0 && _negativeBudgetSince == 0) { _negativeBudgetSince = Year; }
				if (_budget >= 0) { _negativeBudgetSince = 0; }
				BudgetChanged?.Invoke(this, new EventArgs());
			}
		}
		public int NegativeBudgetSince { get { return _negativeBudgetSince == 0 ? 0 : Year - _negativeBudgetSince; } }

		public event EventHandler<(float oldVal, float newVal)> WorkplaceTaxChanged;
		private float _workplaceTax = 0.2f;
		public float WorkplaceTaxRate
		{
			get => _workplaceTax;
			set
			{
				if (value < 0 || value > 1) { throw new ArgumentException("Tax rate must be between 0 and 1"); }
				if (_workplaceTax == value) { return; }
				WorkplaceTaxChanged?.Invoke(this, (_workplaceTax, value));
				_workplaceTax = value;
			}
		}

		public event EventHandler<(float oldVal, float newVal)> ResidentialTaxChanged;
		private float _residentialTax = 0.1f;
		public float ResidentialTaxRate
		{
			get => _residentialTax;
			set
			{
				if (value < 0 || value > 1) { throw new ArgumentException("Tax rate must be between 0 and 1"); }
				if (_residentialTax == value) { return; }
				ResidentialTaxChanged?.Invoke(this, (_residentialTax, value));
				_residentialTax = value;
			}
		}

		private readonly object _workerCountLock = new();
		private int _commercialWorkerCount = 0;
		private int _industrialWorkerCount = 0;

		private readonly List<StatReport> _statReports = new();
		public event EventHandler BudgetChanged;
		public event EventHandler DateChanged;

		private StatEngine(int startYear, float startBudget)
		{
			_date = new DateTime(startYear-1, 12, 31);

			_statReports.Add(new StatReport(Year, Quarter, Budget, 1, City.Instance.GetPopulation()));

			Budget = startBudget;

		}

		/// <summary>
		/// Calculate the tax payed by the residents in the residential building
		/// </summary>
		/// <param name="residential">Residential where the tax is summed</param>
		/// <returns>Tax sum payed by residentials</returns>
		public float CalculateResidentialTaxPerResidential(IResidential residential)
		{
			return CalculateResidentialTaxPerResidential(residential, ResidentialTaxRate);
		}

		/// <summary>
		/// Calculate the tax payed by the residents in the residential building
		/// </summary>
		/// <param name="residential">Residential where the tax is summed</param>
		/// <param name="taxRate">Taxrate payed by residentials</param>
		/// <returns>Tax sum payed by residentials</returns>
		public float CalculateResidentialTaxPerResidential(IResidential residential, float taxRate)
		{
			float residentialTax = 0;
			object taxLock = new();

			Parallel.ForEach(residential.GetResidents(), person =>
			{
				float tax = person.PayTax(taxRate);
				lock (taxLock)
				{
					residentialTax += tax;
				}
			});

			return residentialTax;
		}

		/// <summary>
		/// Calculate the tax payed by the residents in the residential buildings
		/// </summary>
		/// <param name="residentials">Residential list where the tax is summed</param>
		/// <returns>Tax sum payed by residentials</returns>
		public float CalculateResidentialTax(List<IResidential> residentials)
		{
			return CalculateResidentialTax(residentials, ResidentialTaxRate);
		}

		/// <summary>
		/// Calculate the tax payed by the residents in the residential buildings
		/// </summary>
		/// <param name="residentials">Residential list where the tax is summed</param>
		/// <param name="taxRate">Taxrate payed by the residentials</param>
		/// <returns>Tax sum payed by residentials</returns>
		public float CalculateResidentialTax(List<IResidential> residentials, float taxRate)
		{
			float totalTax = 0;
			object taxLock = new();

			Parallel.ForEach(residentials, residential =>
			{
				float tax = CalculateResidentialTaxPerResidential(residential, taxRate);

				lock (taxLock)
				{
					totalTax += tax;
				}
			});

			return totalTax;
		}

		/// <summary>
		/// Calculate the tax payed by the company in the workplace
		/// </summary>
		/// <param name="workplace">Workplace where the tax is summed</param>
		/// <returns>Tax sum payed by the company</returns>
		public float CalculateWorkplaceTaxPerWorkplace(IWorkplace workplace)
		{
			return CalculateWorkplaceTaxPerWorkplace(workplace, WorkplaceTaxRate);
		}

		/// <summary>
		/// Calculate the tax payed by the company in the workplace
		/// </summary>
		/// <param name="workplace">Workplace where the tax is summed</param>
		/// <param name="taxRate">Taxrate payed by the company</param>
		/// <returns>Tax sum payed by the company</returns>
		public float CalculateWorkplaceTaxPerWorkplace(IWorkplace workplace, float taxRate)
		{
			float workplaceTax = 0;
			object taxLock = new();

			List<Worker> workers = workplace.GetWorkers();

			Parallel.ForEach(workplace.GetWorkers(), worker =>
			{
				float tax = worker.PayTax(taxRate);

				lock (taxLock)
				{
					workplaceTax += tax;
				}
			});

			return workplaceTax;
		}

		/// <summary>
		/// Calculate the tax payed by the companies in the workplaces
		/// </summary>
		/// <param name="workplaces">Workplaces list where the tax is summed</param>
		/// <returns>Tax sum payed by companies</returns>
		public float CalculateWorkplaceTax(List<IWorkplace> workplaces)
		{
			return CalculateWorkplaceTax(workplaces, WorkplaceTaxRate);
		}

		/// <summary>
		/// Calculate the tax payed by the companies in the workplaces
		/// </summary>
		/// <param name="workplaces">Workplaces list where the tax is summed</param>
		/// <param name="taxRate">Taxrate payed by the company</param>
		/// <returns>Tax sum payed by companies</returns>
		public float CalculateWorkplaceTax(List<IWorkplace> workplaces, float taxRate)
		{
			float totalTax = 0;
			object taxLock = new();

			Parallel.ForEach(workplaces, workplace =>
			{
				float tax = CalculateWorkplaceTaxPerWorkplace(workplace, taxRate);

				lock (taxLock)
				{
					totalTax += tax;
				}
			});

			return totalTax;
		}

		/// <summary>
		/// Calculate the pension payed for the residents in the residential building
		/// </summary>
		/// <param name="residential">Residential where the pension summed</param>
		/// <returns>Pension sum payed for the pensioners</returns>
		public float CalculatePensionPerResidential(IResidential residential)
		{
			float totalPension = 0;
			object pensionLock = new();
			
			Parallel.ForEach(residential.GetResidents().OfType<Pensioner>().ToList(), pensioner =>
			{
				float pension = pensioner.Pension;

				lock (pensionLock)
				{
					totalPension += pension;
				}
			});
			return totalPension;
		}

		/// <summary>
		/// Calculate the pension payed for the residents in the residential buildings
		/// </summary>
		/// <param name="residentials">Residential list where the pension summed</param>
		/// <returns>Pension sum payed for the pensioners</returns>
		public float CalculatePension(List<IResidential> residentials)
		{
			float totalPension = 0;
			object pensionLock = new();

			Parallel.ForEach(residentials, residential =>
			{
				float pension = CalculatePensionPerResidential(residential);

				lock (pensionLock)
				{
					totalPension += pension;
				}
			});
			return totalPension;
		}

		/// <summary>
		/// Calculate the maintainance cost for the tiles
		/// </summary>
		/// <param name="tiles">Tile list where the maintance summed</param>
		/// <returns>Maintance sum</returns>
		public float SumMaintenance(List<Tile> tiles)
		{
			float totalMaintainanceCost = 0;
			object maintainanceLock = new();

			Parallel.ForEach(tiles, tile =>
			{
				float maintainanceCost = tile.MaintainanceCost;
				lock (maintainanceLock)
				{
					totalMaintainanceCost += maintainanceCost;
				}
			});

			return totalMaintainanceCost;
		}

		/// <summary>
		/// <para>MAKE SIDEEFFECTS</para>
		/// Add the income of the destroyed tiles to the budget
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void AddDestroyIncome(object sender, TileEventArgs e)
		{
			int destroyIncome = e.Tile.DestroyIncome;

			lock (_statReports)
			{
				_statReports[^1].DestroyIncomes += destroyIncome;
			}

			lock (_budgetLock)
			{
				Budget += destroyIncome;
			}
		}

		/// <summary>
		/// <para>MAKE SIDEEFFECTS</para>
		/// <para>Add the expense of the built tiles to the budget</para>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void AddBuildExpense(object sender, TileEventArgs e)
		{
			int buildExpense = City.Instance.GetTile(e.Tile).BuildPrice;

			lock (_statReports)
			{
				_statReports[^1].BuildExpenses += buildExpense;
			}

			lock (_budgetLock)
			{
				Budget -= buildExpense;
			}
		}

		/// <summary>
		/// Calculate the happiness of the workplace by the workers
		/// </summary>
		/// <param name="workplace">Workplace that should be calculated</param>
		/// <returns>Average happiness of workplace</returns>
		public float CalculateWorkplaceHappiness(IWorkplace workplace)
		{
			float workplaceHappiness = 0;
			object happinessLock = new();

			List<Worker> workers = workplace.GetWorkers();

			Parallel.ForEach(workers, worker =>
			{
				float happiness = worker.Happiness;
				lock (happinessLock)
				{
					workplaceHappiness += happiness;
				}
			});

			return workplaceHappiness / workers.Count;
		}

		/// <summary>
		/// Calculate the happiness of the residential by the residents
		/// </summary>
		/// <param name="residential">Residential that should be calculated</param>
		/// <returns></returns>
		public float CalculateResidentialHappiness(IResidential residential)
		{
			float totalResidentialHappiness = 0;
			object happinessLock = new();

			List<Person> persons = residential.GetResidents();

			Parallel.ForEach(persons, person =>
			{
				float happiness = person.Happiness;
				lock (happinessLock)
				{
					totalResidentialHappiness += happiness;
				}
			});

			return totalResidentialHappiness / (persons.Count == 0 ? 1 : persons.Count);
		}

		/// <summary>
		/// Calculate the happiness of persons
		/// </summary>
		/// <param name="persons">List of persons</param>
		/// <returns>AVG happiness</returns>
		public float CalculateHappiness(List<Person> persons)
		{
			float totalHappiness = 0;
			object happinessLock = new();

			Parallel.ForEach(persons, person =>
			{
				float happiness = person.Happiness;
				lock (happinessLock)
				{
					totalHappiness += happiness;
				}
			});

			return totalHappiness / persons.Count;
		}

		/// <summary>
		/// Return the last given statreports or the maximum amount
		/// </summary>
		/// <param name="count">Count of requested statreports</param>
		/// <returns>Requested count statreport if possible</returns>
		public StatReport GetLastNthStatisticsReport(int count)
		{
			UpdateCurrentStatReport(false);

			if (_statReports.Count - 1 - count < 0) { return _statReports[0]; }

			return _statReports[_statReports.Count-1-count];
		}

		public event EventHandler CommercialToIndustrialWorkerRateChanged;

		/// <summary>
		/// Register change in worker count for commercial buildings
		/// </summary>
		/// <param name="oldWorkersLimit">Old count for workers in workplace</param>
		/// <param name="newWorkersLimit">New count for workers in workplace</param>
		internal void RegisterCommercialWorkerChange(int oldWorkersCount, int newWorkersCount)
		{
			lock (_workerCountLock)
			{
				_commercialWorkerCount += newWorkersCount - oldWorkersCount;
			}
			CommercialToIndustrialWorkerRateChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Register change in worker count for industrial buildings
		/// </summary>
		/// <param name="oldWorkersLimit">Old count for workers in workplace</param>
		/// <param name="newWorkersLimit">New count for workers in workplace</param>
		internal void RegisterIndustrialWorkerChange(int oldWorkersCount, int newWorkersCount)
		{
			lock (_workerCountLock)
			{
				_industrialWorkerCount += newWorkersCount - oldWorkersCount;
			}
			CommercialToIndustrialWorkerRateChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Calculate the rate of commercial to industrial workers
		/// </summary>
		/// <returns>Rate of commercial to industial workers count</returns>
		public float GetCommercialWorkersPercentToCommercialAndIndustrialWorkers()
		{
			int allWorkers = (_commercialWorkerCount == 0 ? 1 :	_commercialWorkerCount) + (_industrialWorkerCount == 0 ? 1 : _industrialWorkerCount);
			
			return (float)(_commercialWorkerCount == 0 ? 1 : _commercialWorkerCount) / allWorkers;
		}

		/// <summary>
		/// Update the current statreport without side effects
		/// </summary>
		private void UpdateCurrentStatReport(bool withSideEffects)
		{
			List<IWorkplace> workplaces = ConcatenateWorkplaces();
			List<IResidential> residentials = ConcatenateResidentials();
			List<Tile> tiles = new();
			for (int i = 0; i < City.Instance.GetSize(); i++)
			for (int j = 0; j < City.Instance.GetSize(); j++)
			{
				tiles.Add(City.Instance.GetTile(i, j));
			}

			if (Quarter == 0 && withSideEffects)
			{
				float residentialTax = CalculateResidentialTax(residentials, ResidentialTaxRate);
				float workplaceTax = CalculateWorkplaceTax(workplaces, WorkplaceTaxRate);
				float pension = CalculatePension(residentials);
				float maintainanceCosts = SumMaintenance(tiles);

				lock (_statReports)
				{
					_statReports[^1].ResidentialTax = residentialTax;
					_statReports[^1].WorkplaceTax = workplaceTax;
					_statReports[^1].Pension = pension;
					_statReports[^1].MaintainanceCosts = maintainanceCosts;
				}
			}

			float newIncome = _statReports[^1].ResidentialTax + _statReports[^1].WorkplaceTax;
			float newExpenses = _statReports[^1].Pension + _statReports[^1].MaintainanceCosts;

			_statReports[^1].Budget = Budget;
			_statReports[^1].Incomes = newIncome;
			_statReports[^1].Expenses = newExpenses;
			_statReports[^1].Profit = newIncome - newExpenses;
			if (withSideEffects)
			{
				lock (_budgetLock)
				{
					Budget += newIncome - newExpenses;
				}
			}

			_statReports[^1].Population = City.Instance.GetPopulation();

			if (_statReports.Count >= 2)
			{
				_statReports[^1].BudgetChange = _statReports[^1].Budget - _statReports[^2].Budget;
				_statReports[^1].PopulationChange = _statReports[^1].Population - _statReports[^2].Population;
			}
			else
			{
				_statReports[^1].BudgetChange = _statReports[^1].Budget;
				_statReports[^1].PopulationChange = _statReports[^1].Population;
			}
		}

		/// <summary>
		/// Make a list from all of the residentials
		/// </summary>
		/// <returns>List contains all the residentials</returns>
		private List<IResidential> ConcatenateResidentials()
		{
			List<RoadGrid> roadGrids = RoadGridManager.Instance.RoadGrids;
			List<IResidential> residentials = new();

			foreach (RoadGrid roadGrid in roadGrids)
			{
				residentials.AddRange(roadGrid.Residentials);
			}

			return residentials;
		}

		/// <summary>
		/// Make a list from all of the workplaces
		/// </summary>
		/// <returns>List contains all the workplaces</returns>
		private List<IWorkplace> ConcatenateWorkplaces()
		{
			List<RoadGrid> roadGrids = RoadGridManager.Instance.RoadGrids;
			List<IWorkplace> workplaces = new();

			foreach (RoadGrid roadGrid in roadGrids)
			{
				workplaces.AddRange(roadGrid.Workplaces);
			}

			return workplaces;
		}

		public event EventHandler NextQuarterEvent;
		public event EventHandler NegativeBudgetYearElapsed;

		/// <summary>
		/// Close the current statreport and create a new one
		/// </summary>
		private void NextQuarter()
		{
			Debug.Log("Next quarter");

			if (Quarter == 0 && (NegativeBudgetSince != 0 || Budget < 0))
			{
				NegativeBudgetYearElapsed?.Invoke(this, new EventArgs());
			}

			NextQuarterEvent?.Invoke(this, EventArgs.Empty);

			City.Instance.LoseMiddleSchoolEducation();
			City.Instance.LoseHighSchoolEducation();
			City.Instance.MiddleSchoolEducatePersons();
			City.Instance.HighSchoolEducatePersons();

			UpdateCurrentStatReport(true);

			lock (_statReports)
			{
				_statReports.Add(new StatReport(Year, Quarter, Budget, City.Instance.CityHappiness, City.Instance.GetPopulation()));
			}

			UpdateCurrentStatReport(false);
		}

		/// <summary>
		/// <para>MUST BE CALLED BY SIMENGINE ONLY</para>
		/// <para>Make time pass</para>
		/// </summary>
		public void TimeElapsed()
		{
			Date = Date.AddHours(2);
		}
	}
}