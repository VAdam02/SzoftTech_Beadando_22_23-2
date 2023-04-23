using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Model.Tiles;
using Model.Tiles.Buildings;

namespace Model.Statistics
{
	public class StatEngine
	{
		public int Year { get; private set; }
		public int Quarter { get; private set; }

		private int _buildPrice;
		private int _destoryPrice;

		private readonly object _lock;
		private readonly List<StatReport> _statReports = new();
		private const int STARTYEAR = 2020;

		public StatEngine()
		{
			Year = STARTYEAR;
			Quarter = 0;
		}

		public float CalculateResidenceTaxPerHouse(ResidentialBuildingTile residential, float taxRate)
		{
			float houseTax = 0;
			List<Person> persons = residential.GetResidents();

			foreach (Person person in persons)
			{
				houseTax += person.PayTax(taxRate);
			}

			return houseTax;
		}

		public float CalculateResidenceTax(List<ResidentialBuildingTile> residentials, float taxRate)
		{
			float totalTax = 0;

			Parallel.ForEach(residentials, residential =>
			{
				float tax = CalculateResidenceTaxPerHouse(residential, taxRate);

				lock (_lock)
				{
					totalTax += tax;
				}
			});

			return totalTax;
		}

		public float CalculateIncomeTaxPerWorkplace(IWorkplace workplace, float taxRate)
		{
			float workplaceTax = 0;
			List<Person> persons = workplace.GetWorkers();

			foreach (Person person in persons)
			{
				workplaceTax += person.PayTax(taxRate);
			}

			return workplaceTax;
		}

		public float CalculateIncomeTax(List<IWorkplace> workplaces, float taxRate)
		{
			float totalTax = 0;

			Parallel.ForEach(workplaces, workplace =>
			{
				float tax = CalculateIncomeTaxPerWorkplace(workplace, taxRate);

				lock (_lock)
				{
					totalTax += tax;
				}
			});

			return totalTax;
		}

		public float CalculateHappinessPerResident(ResidentialBuildingTile residential)
		{
			float totalResidentialHappiness = 0;
			List<Person> persons = residential.GetResidents();

			foreach (Person person in persons)
			{
				totalResidentialHappiness += person.GetHappiness();
			}

			return totalResidentialHappiness / persons.Count;
		}

		public float CalculateHappiness(List<ResidentialBuildingTile> residentials)
		{
			float totalCityHappiness = 0;
			float totalWeight = 0;

			Parallel.ForEach(residentials, residential =>
			{
				float happiness = CalculateHappinessPerResident(residential);
				float weight = residential.GetResidents().Count;

				lock (_lock)
				{
					totalCityHappiness += happiness * weight;
					totalWeight += weight;
				}
			});
			
			return totalCityHappiness / totalWeight;
		}

		public int SumMaintainance(List<Building> buildings)
		{
			int totalMaintainanceCost = 0;

			foreach (Building building in buildings)
			{
				totalMaintainanceCost += building.GetMaintainanceCost();
			}

			return totalMaintainanceCost;
		}

		public int GetElectricityProduced()
		{
			//TODO
			throw new NotImplementedException();
		}

		public int GetElectricityConsumed()
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

		public float GetCommercialToIndustrialRate(List<IZoneBuilding> zoneBuildings)
		{
			float commercialCount = 0;
			float IndustrialCount = 0;

			foreach (IZoneBuilding zoneBuilding in zoneBuildings)
			{
				if (zoneBuilding is Industrial)
				{
					++IndustrialCount;
				}
				else if (zoneBuilding is Commercial)
				{
					++commercialCount;
				}
				else
				{
					continue;
				}
			}

			return commercialCount / IndustrialCount;
		}

		/// <summary>
		/// Records the expense of the building
		/// </summary>
		/// <param name="price">positive if expense and negative if income</param>
		/// <exception cref="NotImplementedException"></exception>
		public void SumBuildingPrice(object sender, EventArgs e)
		{
			//TODO
			throw new NotImplementedException();
		}

		/// <summary>
		/// Records the income of destruction
		/// </summary>
		/// <param name="price">positive if income and negative if expense</param>
		/// <exception cref="NotImplementedException"></exception>
		public void SumDestroyPrice(object sender, EventArgs e)
		{
			//TODO
			throw new NotImplementedException();
		}

		public void NextQuarter()
		{
			//TODO
			throw new NotImplementedException();

			_buildPrice = 0;
			_destoryPrice = 0;
		}
	}
}