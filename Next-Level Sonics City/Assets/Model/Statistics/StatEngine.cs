using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Model.Tiles;
using Model.Tiles.Buildings;

namespace Model.Statistics
{
	public class StatEngine
	{
		private List<StatReport> _statReports;
		public int Year { get; private set; }
		public int Quarter { get; private set; }

		private const int STARTYEAR = 2020;

		public StatEngine()
		{
			_statReports = new List<StatReport>();
			Year = STARTYEAR;
			Quarter = 0;
		}

		public async Task<float> CalculateResidenceTaxPerHouse(Residential residential, float taxRate)
		{
			float houseTax = 0;
			List<Person> persons = residential.GetResidents();

			foreach (Person person in persons)
			{
				houseTax += person.PayTax(taxRate);
			}

			return houseTax;
		}

		public async Task<float> CalculateResidenceTax(List<Residential> residentials, float taxRate)
		{
			float totalTax = 0;

			foreach (Residential residential in residentials)
			{
				totalTax += await CalculateResidenceTaxPerHouse(residential, taxRate);
			}

			return totalTax;
		}

		public async Task<float> CalculateIncomeTaxPerWorkplace(IWorkplace workplace, float taxRate)
		{
			float workplaceTax = 0;
			List<Person> persons = workplace.GetWorkers();

			foreach (Person person in persons)
			{
				workplaceTax += person.PayTax(taxRate);
			}

			return workplaceTax;
		}

		public async Task<float> CalculateIncomeTax(List<IWorkplace> workplaces, float taxRate)
		{
			float totalTax = 0;

			foreach (IWorkplace workplace in workplaces)
			{
				totalTax += await CalculateIncomeTaxPerWorkplace(workplace, taxRate);
			}

			return totalTax;
		}

		public (float avg, int weight) CalculateHappinessPerResident(Residential residential)
		{
			float totalResidentialHappiness = 0;
			List<Person> persons = residential.GetResidents();

			foreach (Person person in persons)
			{
				totalResidentialHappiness += person.GetHappiness();
			}

			return (totalResidentialHappiness / persons.Count, persons.Count);
		}

		public (float avg, int weight) CalculateHappiness(List<Residential> residentials)
		{
			float totalCityHappiness = 0;
			int count = 0;

			foreach (Residential residential in residentials)
			{
				(float avg, int weight) = CalculateHappinessPerResident(residential);
				count += weight;
				totalCityHappiness += avg * weight;
			}
			
			return (totalCityHappiness / count, count);
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
			List<StatReport> reports = new List<StatReport>(index);

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

		public void SumBuildingPrice(int price)
		{
			//TODO
			throw new NotImplementedException();
		}

		/// <summary>
		/// Records the income of destruction
		/// </summary>
		/// <param name="price">positive if income and negative if expense</param>
		/// <exception cref="NotImplementedException"></exception>
		public void SumDestroyPrice(int price)
		{
			//TODO
			throw new NotImplementedException();
		}

		public void NextQuarter()
		{
			//TODO
			throw new NotImplementedException();
		}
	}
}