using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model.Persons;
using Model.RoadGrids;
using Model.Simulation;
using Model.Tiles;
using Model.Tiles.Buildings;
using UnityEngine.Events;

namespace Model.Statistics
{
	public class StatEngine
	{
		private DateTime date; 
		public int Year { get { return date.Year; } }
		public int Quarter { get { return date.Month / 3; } }
		public string YearMonth { get { return date.ToString("yyyy MMM"); } }
		public string Day { get { return date.Day.ToString(); } }
		public float Budget { get; private set; }

		private int _buildPrice = 0;
		private int _destroyPrice = 0;
		private float _incomeTaxRate = 0;
		private float _residentialTaxRate = 0;
		private float _commercialCount = 0;
		private float _industrialCount = 0;

		private readonly List<StatReport> _statReports = new();
		public UnityEvent BudgetChanged = new();
		public UnityEvent DateChanged = new();


		public StatEngine(int startYear, float startBudget)
		{
			date = new DateTime(startYear, 1, 1);
			Budget = startBudget;

			StatReport zerothStatReport = new()
			{
				Quarter = Quarter,
				Year = Year,
				Budget = Budget,
				Happiness = 0,
				IncomeTax = 0,
				ResidentialTax = 0,
				DestroyIncomes = 0,
				BuildExpenses = 0,
				MaintainanceCosts = 0,
				Incomes = 0,
				Expenses = 0,
				Profit = 0,
				Population = 0,
				PopulationChange = 0,
				ElectricityProduced = 0,
				ElectricityConsumed = 0
			};
			_statReports.Add(zerothStatReport);
		}

		public void TimeElapsed()
		{
			int quarter = Quarter;
			string day = Day;

			date = date.AddMinutes(30	);

			if (day != Day)
			{
				MainThreadDispatcher.Instance.Enqueue(() =>
				{
					DateChanged.Invoke();
				});
			}
			if (quarter == Quarter)
			{
				//NextQuarter();
			}
		}
		
		public float CalculateResidentialTaxPerHouse(IResidential residential, float taxRate)
		{
			float houseTax = 0;

			List<Person> persons = residential.GetResidents();

			foreach (Person person in persons)
			{
				houseTax += person.PayTax(taxRate);
			}

			return houseTax;
		}

		public float CalculateResidentialTax(List<IResidential> residentials, float taxRate)
		{
			float totalTax = 0;
			object taxLock = new();

			Parallel.ForEach(residentials, residential =>
			{
				float tax = CalculateResidentialTaxPerHouse(residential, taxRate);

				lock (taxLock)
				{
					totalTax += tax;
				}
			});

			return totalTax;
		}

		public void SetResidentialTaxRate(float residentialTaxRate)
		{
			_residentialTaxRate = residentialTaxRate;
		}

		public float CalculateIncomeTaxPerWorkplace(IWorkplace workplace, float taxRate)
		{
			float workplaceTax = 0;

			List<Worker> persons = workplace.GetWorkers();

			foreach (Person person in persons)
			{
				workplaceTax += person.PayTax(taxRate);
			}

			return workplaceTax;
		}

		public float CalculateIncomeTax(List<IWorkplace> workplaces, float taxRate)
		{
			float totalTax = 0;
			object taxLock = new();

			Parallel.ForEach(workplaces, workplace =>
			{
				float tax = CalculateIncomeTaxPerWorkplace(workplace, taxRate);

				lock (taxLock)
				{
					totalTax += tax;
				}
			});

			return totalTax;
		}

		public void SetIncomeTaxRate(float incomeTaxRate)
		{
			_incomeTaxRate = incomeTaxRate;
		}

		public float CalculateWorkplaceHappiness(IWorkplace workplace)
		{
			float workplaceHappiness = 0;

			List<Worker> workers = workplace.GetWorkers();

			foreach (Worker worker in workers)
			{
				workplaceHappiness += worker.GetHappiness();
			}

			return workplaceHappiness / workers.Count;
		}

		public float CalculateResidentialHappinessPerHouse(IResidential residential)
		{
			float totalResidentialHappiness = 0;

			List<Person> persons = residential.GetResidents();

			foreach (Person person in persons)
			{
				totalResidentialHappiness += person.GetHappiness();
			}

			return totalResidentialHappiness / persons.Count;
		}

		public float CalculateHappiness(List<IResidential> residentials)
		{
			float totalCityHappiness = 0;
			float totalWeight = 0;
			object happinessLock = new();
			object weightLock = new();

			Parallel.ForEach(residentials, residential =>
			{
				float happiness = CalculateResidentialHappinessPerHouse(residential);
				float weight = residential.GetResidents().Count;

				lock (happinessLock)
				{
					totalCityHappiness += happiness * weight;
				}
				lock (weightLock)
				{
					totalWeight += weight;
				}
			});
			
			return totalCityHappiness / totalWeight;
		}

		public int CalculatePopulation(List<IResidential> residentials)
		{
			int totalPopulation = 0;

			foreach (var residential in residentials)
			{
				totalPopulation += residential.GetResidents().Count;
			}

			return totalPopulation;
		}

		public float SumMaintainance(List<Building> buildings)
		{
			float totalMaintainanceCost = 0;

			foreach (Building building in buildings)
			{
				totalMaintainanceCost += building.GetMaintainanceCost();
			}

			return totalMaintainanceCost;
		}

		public int GetElectricityProduced(List<Building> buildings)
		{
			//TODO
			throw new NotImplementedException();
		}

		public int GetElectricityConsumed(List<Building> buildings)
		{
			//TODO
			throw new NotImplementedException();
		}

		public StatReport GetLastStatisticsReport()
		{
			return _statReports[^1];
		}

		public List<StatReport> GetEveryStatisticsReport()
		{
			return _statReports;
		}

		public List<StatReport> GetLastGivenStatisticsReports(int index)
		{
			List<StatReport> reports = new(index);

			int length = _statReports.Count - 1;

			for (int i = 0; i < index; ++i)
			{
				reports[i] = _statReports[length - i];
			}

			return reports;
		}

		public float GetCommercialToIndustrialRate()
		{
			return _commercialCount / _industrialCount;
		}

		private readonly object _lock = new();
		private readonly object _eventLock = new();
		private readonly object _budgetLock = new();
		private readonly object _incrementLock = new();

		public void SumBuildPrice(object sender, TileEventArgs e)
		{
			lock (_lock)
			{
				_buildPrice += e.Tile.GetBuildPrice();
			}
			lock (_budgetLock)
			{
				Budget -= e.Tile.GetBuildPrice();
			}
			lock (_eventLock)
			{
				MainThreadDispatcher.Instance.Enqueue(() =>
				{
					BudgetChanged.Invoke();
				});
			}
		}

		public void SumDestroyPrice(object sender, TileEventArgs e)
		{
			lock (_lock)
			{
				_destroyPrice += e.Tile.GetDestroyPrice();
			}
			lock (_budgetLock)
			{
				Budget += e.Tile.GetDestroyPrice();
			}
			lock (_eventLock)
			{
				MainThreadDispatcher.Instance.Enqueue(() =>
				{
					BudgetChanged.Invoke();
				});
			}
		}

		public void SumMarkZonePrice(object sender, TileEventArgs e)
		{
			lock (_lock)
			{
				_buildPrice += e.Tile.GetBuildPrice();
			}
			lock (_budgetLock)
			{
				Budget -= e.Tile.GetBuildPrice();
			}
			lock (_incrementLock)
			{
				if (e.Tile is Industrial) { ++_industrialCount; return; }
				if (e.Tile is Commercial) { ++_commercialCount; }
			}
			lock (_eventLock)
			{
				MainThreadDispatcher.Instance.Enqueue(() =>
				{
					BudgetChanged.Invoke();
				});
			}
		}

		public void SumUnMarkZonePrice(object sender, TileEventArgs e)
		{
			lock (_lock)
			{
				_destroyPrice += e.Tile.GetDestroyPrice();
			}
			lock (_budgetLock)
			{
				Budget += e.Tile.GetDestroyPrice();
			}
			lock (_incrementLock)
			{
				if (e.Tile is Industrial) { --_industrialCount; return; }
				if (e.Tile is Commercial) { --_commercialCount; }
			}
			lock (_eventLock)
			{
				MainThreadDispatcher.Instance.Enqueue(() =>
				{
					BudgetChanged.Invoke();
				});
			}
		}

		public void NextQuarter()
		{
			StatReport statReport = new();

			List<IWorkplace> workplaces = ConcatenateWorkplaces();
			List<IResidential> residentials = ConcatenateResidentials();
			List<Building> buildings = (List<Building>)Enumerable.Concat((IEnumerable<Building>)residentials, (IEnumerable<Building>)workplaces);

			statReport.Quarter = Quarter;
			statReport.Year = Year;

			if (Quarter == 0)
			{
				statReport.IncomeTax = CalculateIncomeTax(workplaces, _incomeTaxRate);
				statReport.ResidentialTax = CalculateResidentialTax(residentials, _residentialTaxRate);
				statReport.MaintainanceCosts = SumMaintainance(buildings);
			}
			else
			{
				statReport.IncomeTax = 0;
				statReport.ResidentialTax = 0;
				statReport.MaintainanceCosts = 0;
			}

			Budget += statReport.IncomeTax + statReport.ResidentialTax - statReport.MaintainanceCosts;
			statReport.Budget = Budget;

			MainThreadDispatcher.Instance.Enqueue(() =>
			{
				BudgetChanged.Invoke();
			});

			statReport.Happiness = CalculateHappiness(residentials);

			statReport.BuildExpenses = _buildPrice;
			statReport.DestroyIncomes = _destroyPrice;

			statReport.Incomes = statReport.IncomeTax + statReport.ResidentialTax + _destroyPrice;
			statReport.Expenses = statReport.MaintainanceCosts + _buildPrice;
			statReport.Profit = statReport.Incomes - statReport.Expenses;

			statReport.Population = CalculatePopulation(residentials);
			statReport.PopulationChange = statReport.Population - _statReports[^1].Population;

			statReport.ElectricityProduced = GetElectricityProduced(buildings);
			statReport.ElectricityConsumed = GetElectricityConsumed(buildings);

			_statReports.Add(statReport);

			_buildPrice = 0;
			_destroyPrice = 0;
		}

		private List<IResidential> ConcatenateResidentials()
		{
			List<RoadGrid> roadGrids = SimEngine.Instance.RoadGridManager.RoadGrids;
			List<IResidential> residentials = new();		

			foreach (RoadGrid roadGrid in roadGrids)
			{
				residentials = Enumerable.Concat(residentials, roadGrid.Residentials).ToList();
			}

			return residentials;
		}

		private List<IWorkplace> ConcatenateWorkplaces()
		{
			List<RoadGrid> roadGrids = SimEngine.Instance.RoadGridManager.RoadGrids;
			List<IWorkplace> workplaces = new();

			foreach (RoadGrid roadGrid in roadGrids)
			{
				workplaces = Enumerable.Concat(workplaces, roadGrid.Workplaces).ToList();
			}

			return workplaces;
		}
	}
}