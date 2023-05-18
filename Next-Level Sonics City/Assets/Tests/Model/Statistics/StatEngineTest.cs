using Model.Persons;
using Model.Tiles;
using Model.Tiles.Buildings;
using NUnit.Framework;
using System.Collections.Generic;

namespace Model.Statistics
{
	public class StatEngineTests
	{
		MockRoadGridElement _road;
		MockResidentialBuildingTile _residential1;
		MockResidentialBuildingTile _residential2;
		MockWorkplaceBuildingTile _workplace1;
		MockWorkplaceBuildingTile _workplace2;

		List<Worker> _workers;
		List<Pensioner> _pensioners;

		[SetUp]
		public void SetUp()
		{
			StatEngine.Reset();
			City.Reset();

			_road = new(1, 1);
			City.Instance.SetTile(_road);
			_residential1 = new(1, 0, Rotation.OneEighty);
			City.Instance.SetTile(_residential1);
			_residential2 = new(2, 1, Rotation.TwoSeventy);
			City.Instance.SetTile(_residential2);

			_workplace1 = new(1, 2, Rotation.Zero);
			City.Instance.SetTile(_workplace1);
			_workplace2 = new(0, 1, Rotation.Ninety);
			City.Instance.SetTile(_workplace2);

			_workers = new()
			{
				new Worker(_residential1, _workplace1, 30, Qualification.LOW),
				new Worker(_residential1, _workplace2, 40, Qualification.LOW),
				new Worker(_residential2, _workplace1, 50, Qualification.LOW),
				new Worker(_residential2, _workplace2, 60, Qualification.LOW)
			};

			_pensioners = new()
			{
				new Pensioner(_residential1, 70, 25),
				new Pensioner(_residential1, 80, 50),
				new Pensioner(_residential2, 90, 75),
				new Pensioner(_residential2, 100, 100)
			};
		}

		[Test]
		public void CalculateResidentialTaxPerResidential_EqualsToIndivideualWorkersTaxSum()
		{
			float taxRate = 0.1f;
			float residentialTax = _workers[0].PayTax(taxRate) + _workers[1].PayTax(taxRate);
			
			Assert.AreEqual(residentialTax, StatEngine.Instance.CalculateResidentialTaxPerResidential(_residential1, taxRate));
		}

		[Test]
		public void CalculateResidentialTax_EqualsToIndividualWorkersTaxSum()
		{
			float taxRate = 0.1f;
			float residentialTax = 0;
			foreach (Worker worker in _workers)
			{
				residentialTax += worker.PayTax(taxRate);
			}

			Assert.AreEqual(residentialTax, StatEngine.Instance.CalculateResidentialTax(new List<IResidential>() { _residential1, _residential2 }, taxRate));
		}

		[Test]
		public void CalculateWorkplaceTaxPerWorkplace_EqualsToIndivideualWorkersTaxSum()
		{
			float taxRate = 0.1f;
			float workplaceTax = _workers[0].PayTax(taxRate) + _workers[2].PayTax(taxRate);
			Assert.AreEqual(workplaceTax, StatEngine.Instance.CalculateWorkplaceTaxPerWorkplace(_workplace1, taxRate));
		}

		[Test]
		public void CalculateWorkplaceTax_EqualsToIndividualWorkersTaxSum()
		{
			float taxRate = 0.1f;
			float workplaceTax = 0;
			foreach (Worker worker in _workers)
			{
				workplaceTax += worker.PayTax(taxRate);
			}
			Assert.AreEqual(workplaceTax, StatEngine.Instance.CalculateWorkplaceTax(new List<IWorkplace>() { _workplace1, _workplace2 }, taxRate));
		}

		[Test]
		public void CalculatePensionPerResidential_EqualsToIndividualPensionersPensionSum()
		{
			float pension = _pensioners[0].Pension + _pensioners[1].Pension;
			Assert.AreEqual(pension, StatEngine.Instance.CalculatePensionPerResidential(_residential1));
		}

		[Test]
		public void CalculatePension_EqualsToIndividualPensionersPensionSum()
		{
			float pension = 0;
			foreach (Pensioner pensioner in _pensioners)
			{
				pension += pensioner.Pension;
			}
			Assert.AreEqual(pension, StatEngine.Instance.CalculatePension(new List<IResidential>() { _residential1, _residential2 }));
		}

		[Test]
		public void SumMaintenance_EqualsToIndividualTilesMaintenanceSum()
		{
			float maintenance = _residential1.GetMaintainanceCost() + _residential2.GetMaintainanceCost() + _workplace1.GetMaintainanceCost() + _workplace2.GetMaintainanceCost();
			Assert.AreEqual(maintenance, StatEngine.Instance.SumMaintenance(new List<Tile>() { _residential1, _residential2, _workplace1, _workplace2 }));
		}

		[Test]
		public void CalculateWorkplaceHappiness_EqualsToIndividualWorkersHappinessAVG()
		{
			float happiness = (_workers[0].GetHappiness() + _workers[1].GetHappiness()) / 2;
			Assert.AreEqual(happiness, StatEngine.Instance.CalculateWorkplaceHappiness(_workplace1));
		}

		[Test]
		public void CalculateResidentialHappiness_EqualsToIndividualPersonsHappinessAVG()
		{
			float happiness = (_workers[0].GetHappiness() + _workers[1].GetHappiness() + _pensioners[0].GetHappiness() + _pensioners[1].GetHappiness()) / 4;
			Assert.AreEqual(happiness, StatEngine.Instance.CalculateResidentialHappiness(_residential1));
		}

		[Test]
		public void CalculateHappiness_EqualsToIndividualPersonsHappinessAVG()
		{
			float happiness = 0;
			foreach (Worker worker in _workers)
			{
				happiness += worker.GetHappiness();
			}
			foreach (Pensioner pensioner in _pensioners)
			{
				happiness += pensioner.GetHappiness();
			}
			happiness /= _workers.Count + _pensioners.Count;

			List<Person> persons = new();
			persons.AddRange(_workers);
			persons.AddRange(_pensioners);

			Assert.AreEqual(happiness, StatEngine.Instance.CalculateHappiness(persons));
		}
	}
}