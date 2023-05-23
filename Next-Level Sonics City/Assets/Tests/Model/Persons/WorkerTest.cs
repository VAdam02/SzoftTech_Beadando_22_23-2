using Model.RoadGrids;
using Model.Tiles.Buildings;
using NUnit.Framework;
using System;

namespace Model.Persons
{
	internal class WorkerTest
	{
		private MockResidentialBuildingTile _home;
		private MockWorkplaceBuildingTile _workplace;

		[SetUp]
		public void SetUp()
		{
			RoadGridManager.Reset();
			City.Reset();

			_home = new(0, 0, Rotation.Zero);
			City.Instance.SetTile(_home);
			_workplace = new(0, 1, Rotation.Zero);
			City.Instance.SetTile(_workplace);
		}

		[Test]
		public void Constructor_ThrowsExceptionWhenAgeTooYoungOrTooOld()
		{
			Assert.Throws<ArgumentException>(() => new Worker(_home, _workplace, 17, Qualification.LOW));
			Assert.Throws<ArgumentException>(() => new Worker(_home, _workplace, Worker.PENSION_AGE, Qualification.LOW));
			Assert.Throws<ArgumentException>(() => new Worker(_home, _workplace, Worker.PENSION_AGE + 1, Qualification.LOW));
		}

		[Test]
		public void Constructor_ThrowsExceptionWhenHomeOrWorkplaceIsNull()
		{
			Assert.Throws<ArgumentNullException>(() => new Worker(null, _workplace, 18, Qualification.LOW));
			Assert.Throws<ArgumentNullException>(() => new Worker(_home, null, 18, Qualification.LOW));
		}

		[Test]
		public void Constructor_SucceedsWhenValidArgumentsProvided()
		{
			Assert.DoesNotThrow(() => new Worker(_home, _workplace, 18, Qualification.LOW));
		}

		[Test]
		public void IncreaseAge_IncreasesAgeByOne()
		{
			Worker worker = new(_home, _workplace, 30, Qualification.LOW);

			worker.IncreaseAge();

			Assert.That(worker.Age, Is.EqualTo(31));
		}

		[Test]
		public void Retire_ReturnsPensionerWithMatchingProperties()
		{
			Worker worker = new(_home, _workplace, Worker.PENSION_AGE - 1, Qualification.LOW);
			worker.IncreaseAge();

			Pensioner pensioner = worker.Retire();

			Assert.That(pensioner.Age, Is.EqualTo(worker.Age));
			Assert.That(pensioner.Residential, Is.EqualTo(worker.Residential));
		}

		[Test]
		public void CalculatePension_ReturnsExpectedPensionAmount()
		{
			Worker worker = new(_home, _workplace, Worker.PENSION_AGE - Worker.TAXED_YEARS_FOR_PENSION, Qualification.LOW);

			float taxRate = 0.1f;
			float expectedPension = 0f;

			for (int i = 0; i < Worker.TAXED_YEARS_FOR_PENSION; i++)
			{
				worker.PayTax(taxRate);
				expectedPension += worker.CalculateSalary() * taxRate;
				worker.IncreaseAge();
			}
			expectedPension /= Worker.TAXED_YEARS_FOR_PENSION;

			Pensioner pensioner = worker.Retire();

			Assert.That(pensioner.Pension, Is.EqualTo(expectedPension / 2.0f));
			Assert.That(pensioner.Age, Is.EqualTo(worker.Age));
		}

		[Test]
		public void IncreaseQualification_IncreasesQualificationByOneLevel()
		{
			Worker worker = new(_home, _workplace, 30, Qualification.LOW);

			worker.IncreaseQualification();
			Assert.That(worker.PersonQualification, Is.EqualTo(Qualification.MID));

			worker.IncreaseQualification();
			Assert.That(worker.PersonQualification, Is.EqualTo(Qualification.HIGH));

			worker.IncreaseQualification();
			Assert.That(worker.PersonQualification, Is.EqualTo(Qualification.HIGH));
		}

		[Test]
		public void DecreaseQualification_DecreasesQualificationByOneLevel()
		{
			Worker worker = new(_home, _workplace, 30, Qualification.HIGH);

			worker.DecreaseQualification();
			Assert.That(worker.PersonQualification, Is.EqualTo(Qualification.MID));

			worker.DecreaseQualification();
			Assert.That(worker.PersonQualification, Is.EqualTo(Qualification.LOW));

			worker.DecreaseQualification();
			Assert.That(worker.PersonQualification, Is.EqualTo(Qualification.LOW));
		}

		[Test]
		public void PayTax_ReturnsExpectedTaxAmount()
		{
			Worker worker = new(_home, _workplace, Worker.PENSION_AGE - 1, Qualification.LOW);
			float taxRate = 0.2f;

			Assert.That(worker.PayTax(taxRate), Is.EqualTo(100));

			worker.IncreaseQualification();
			Assert.That(worker.PayTax(taxRate), Is.EqualTo(120));

			worker.IncreaseQualification();
			Assert.That(worker.PayTax(taxRate), Is.EqualTo(150));

			Assert.That(worker.PayTax(taxRate), Is.EqualTo(150));
			worker.IncreaseAge();
			Assert.That(worker.PayTax(taxRate), Is.EqualTo(0));
		}
	}
}
