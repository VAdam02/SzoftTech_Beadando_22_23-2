using System;
using System.Collections;
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

        
        public float CalculateTax(List<Residential> residentials)
        {
            int totalTax = 0;

            foreach (Residential residential in residentials)
            {
                
            }


            return 0;
        }

        public float CalculatePersonHappiness(Person person)
        {
            //TODO
            return 0;
        }
        

        public float CalculateBuildingHappiness(Building building)
        {
            //TODO
            return 0;
        }

        public float CalculateCityHappiness(List<Building> buildings)
        {
            //TODO
            return 0;
        }

        public int sumMaintainance(List<Building> buildings)
        {
            int totalMaintainance = 0;

            foreach (Building building in buildings)
            {
                
            }


            return 0;
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

        public StatReport GetStatisticsReport()
        {
            return _statReports[_statReports.Count - 1];
        }

        public List<StatReport> GetLastGivenStatisticsReport(int index)
        {
            List<StatReport> reports = new List<StatReport>(index);

            int length = _statReports.Count - 1;

            for (int i = 0; i < index; ++i)
            {
                reports[i] = _statReports[length - i];
            }

            return reports;
        }

        public float GetCommercialToIndustrialRate()
        {
            //TODO
            throw new NotImplementedException();
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