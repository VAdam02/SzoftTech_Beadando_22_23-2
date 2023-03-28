using System;
using System.Collections.Generic;
using UnityEngine;
using Tiles;
using Model;
using Buildings;
using System.Threading.Tasks;
using Persons;

namespace Statistics
{
    public class StatEngine : MonoBehaviour
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

        public float CalculateResidenceTaxPerHouse(Residential residential, float taxRate)
        {
            float houseTax = 0;
            List<Person> persons = residential.GetResidents();

            foreach (Person person in persons)
            {
                if (person is Worker)
                {
                    houseTax += person.PayTax(taxRate);
                }
            }

            houseTax /= persons.Count;

            return houseTax;
        }

        public float CalculateResidenceTax(List<Residential> residentials, float taxRate)
        {
            float totalTax = 0;

            foreach (Residential residential in residentials)
            {
                totalTax += CalculateResidenceTaxPerHouse(residential, taxRate);
            }

            totalTax /= residentials.Count;

            return totalTax;
        }     

        public async Task<float> CalculateBuildingHappiness(IZoneBuilding zoneBuilding)// Izone -> Building
        {
            float totalBuildingHappiness = 0;
            List<Person> persons = zoneBuilding.GetPeople();

            foreach (Person person in persons)
            {
                totalBuildingHappiness += person.GetHappiness();
            }

            totalBuildingHappiness /= persons.Count;

            return totalBuildingHappiness;
        }

        public async Task<float> CalculateCityHappiness(List<IZoneBuilding> buildings)// Izone -> Building
        {
            float totalCityHappiness = 0;

            foreach (IZoneBuilding building in buildings)
            {
                totalCityHappiness += await CalculateBuildingHappiness(building);
            }

            totalCityHappiness /= buildings.Count;

            return totalCityHappiness;
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
            StatReport statReport = new StatReport();

            Quarter = ++Quarter % 4;

            statReport.Quarter = Quarter;

            if (Quarter == 0) { ++Year; }

            statReport.Year = Year;
            
            
            
            _statReports.Add(statReport);
        }
    }
}