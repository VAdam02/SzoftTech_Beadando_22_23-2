using System;
using System.Collections.Generic;
using UnityEngine;
using Tiles;
using Model;
using Buildings;

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

        public float CalculateHouseTax(Residential residential)
        {
            float houseTax = 0;
            List<Person> persons = residential.GetPeople();

            foreach (Person person in persons)
            {
                houseTax += person.GetTax();
            }

            houseTax /= persons.Count;

            return houseTax;
        }

        public float CalculateTax(List<Residential> residentials)
        {
            float totalTax = 0;

            foreach (Residential residential in residentials)
            {
                totalTax += CalculateHouseTax(residential);
            }

            totalTax /= residentials.Count;

            return totalTax;
        }     

        public float CalculateBuildingHappiness(IZoneBuilding zoneBuilding)
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

        public float CalculateCityHappiness(List<IZoneBuilding> buildings)
        {
            float totalCityHappiness = 0;

            foreach (IZoneBuilding building in buildings)
            {
                totalCityHappiness += CalculateBuildingHappiness(building);
            }

            totalCityHappiness /= buildings.Count;

            return totalCityHappiness;
        }

        public int sumMaintainance(List<Building> buildings)
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

        public void nextQuarter()
        {            
            StatReport statReport = new StatReport();

            statReport.Quarter = Quarter;

            Quarter = ++Quarter % 4;

            if (Quarter == 0) { ++Year; }

            statReport.Year = Year;
            
            
            
            _statReports.Add(statReport);
            
        }
    }
}