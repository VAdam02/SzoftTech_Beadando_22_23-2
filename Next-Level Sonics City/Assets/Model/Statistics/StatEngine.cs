using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model.Persons;
using Model.RoadGrids;
using Model.Simulation;
using Model.Tiles;
using Model.Tiles.Buildings;

namespace Model.Statistics
{
	public class StatEngine
	{
		public int Year { get; private set; }
		public int Quarter { get; private set; }

		private int _buildPrice = 0;
		private int _destroyPrice = 0;
		private float _commercialCount = 0;
		private float _industrialCount = 0;
		private float _incomeTaxRate = 0;
		private float _residenceTaxRate = 0;

		private readonly List<StatReport> _statReports = new();
		private const int STARTYEAR = 2020;

		public StatEngine()
		{
			Year = STARTYEAR;
			Quarter = 0;

			StatReport zerothStatReport = new StatReport();
			zerothStatReport.Quarter = Quarter;
			zerothStatReport.Year = Year;
			zerothStatReport.Happiness = 0;
			zerothStatReport.IncomeTax = 0;
			zerothStatReport.ResidenceTax = 0;
			zerothStatReport.DestroyIncomes = 0;
			zerothStatReport.BuildExpenses = 0;
			zerothStatReport.MaintainanceCosts = 0;
			zerothStatReport.Incomes = 0;
			zerothStatReport.Expenses = 0;
			zerothStatReport.Profit = 0;
			zerothStatReport.Population = 0;
			zerothStatReport.PopulationChange = 0;
			zerothStatReport.ElectricityProduced = 0;
			zerothStatReport.ElectricityConsumed = 0;
			_statReports.Add(zerothStatReport);
		}

		public float CalculateResidenceTaxPerHouse(IResidential residential, float taxRate)
		{
			float houseTax = 0;

			List<Person> persons = residential.GetResidents();

			foreach (Person person in persons)
			{
				houseTax += person.PayTax(taxRate);
			}

			return houseTax;
		}

		public float CalculateResidenceTax(List<IResidential> residentials, float taxRate)
		{
			_residenceTaxRate = taxRate;
			float totalTax = 0;
			object taxLock = new();

			Parallel.ForEach(residentials, residential =>
			{
				float tax = CalculateResidenceTaxPerHouse(residential, taxRate);

				lock (taxLock)
				{
					totalTax += tax;
				}
			});

			return totalTax;
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
			_incomeTaxRate = taxRate;
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

		public float CalculateHappinessPerResident(IResidential residential)
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
				float happiness = CalculateHappinessPerResident(residential);
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
		private readonly object _incrementLock = new();

		public void SumBuildPrice(object sender, TileEventArgs e)
		{
			lock (_lock)
			{
				_buildPrice += e.Tile.GetBuildPrice();
			}
		}

		public void SumDestroyPrice(object sender, TileEventArgs e)
		{
			lock (_lock)
			{
				_destroyPrice += e.Tile.GetDestroyPrice();
			}
		}

		public void SumMarkZonePrice(object sender, TileEventArgs e)
		{
			lock (_lock)
			{
				_buildPrice += e.Tile.GetBuildPrice();
			}
			lock (_incrementLock)
			{
				if (e.Tile is Industrial) { ++_industrialCount; return; }
				if (e.Tile is Commercial) { ++_commercialCount; }
			}
		}

		public void SumUnMarkZonePrice(object sender, TileEventArgs e)
		{
			lock (_lock)
			{
				_destroyPrice += e.Tile.GetDestroyPrice();
			}
			lock (_incrementLock)
			{
				if (e.Tile is Industrial) { --_industrialCount; return; }
				if (e.Tile is Commercial) { --_commercialCount; }
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

			statReport.Happiness = CalculateHappiness(residentials);

			statReport.BuildExpenses = _buildPrice;
			statReport.DestroyIncomes = _destroyPrice;
			statReport.MaintainanceCosts = SumMaintainance(buildings);

			statReport.IncomeTax = CalculateIncomeTax(workplaces, _incomeTaxRate);
			statReport.ResidenceTax = CalculateResidenceTax(residentials, _residenceTaxRate);
			statReport.Incomes = statReport.IncomeTax + statReport.ResidenceTax + _destroyPrice;
			statReport.Expenses = statReport.MaintainanceCosts + _buildPrice;
			statReport.Profit = statReport.Incomes - statReport.Expenses;

			statReport.Population = CalculatePopulation(residentials);
			statReport.PopulationChange = statReport.Population - _statReports[^1].Population;

			statReport.ElectricityProduced = GetElectricityProduced(buildings);
			statReport.ElectricityConsumed = GetElectricityConsumed(buildings);

			_statReports.Add(statReport);

			++Quarter;
			Quarter %= 4;

			if (Quarter == 0)
			{
				++Year;
			}

			_buildPrice = 0;
			_destroyPrice = 0;
		}

		private List<IResidential> ConcatenateResidentials()
		{
			List<RoadGrid> roadGrids = SimEngine.Instance.RoadGridManager.RoadGrids;
			List<IResidential> residentials = new List<IResidential>();		

			foreach (RoadGrid roadGrid in roadGrids)
			{
				residentials = Enumerable.Concat(residentials, roadGrid.Residentials).ToList();
			}

			return residentials;
		}

		private List<IWorkplace> ConcatenateWorkplaces()
		{
			List<RoadGrid> roadGrids = SimEngine.Instance.RoadGridManager.RoadGrids;
			List<IWorkplace> workplaces = new List<IWorkplace>();

			foreach (RoadGrid roadGrid in roadGrids)
			{
				workplaces = Enumerable.Concat(workplaces, roadGrid.Workplaces).ToList();
			}

			return workplaces;
		}
	}
}