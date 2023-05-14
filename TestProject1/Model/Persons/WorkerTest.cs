using Model.RoadGrids;

namespace Model.Persons
{
	[TestFixture]
	internal class WorkerTest
	{
		private IResidential _home;
		private IWorkplace _workplace;

		[SetUp]
		public void SetUp()
		{
			_home = new MockResidential();
			_workplace = new MockWorkplace();
		}

		[Test]
		public void TestTooOldAndYoungConstructor()
		{
			Assert.Throws<ArgumentException>(() => new Worker(_home, _workplace, 17, Qualification.LOW));
			Assert.Throws<ArgumentException>(() => new Worker(_home, _workplace, Worker.PENSION_AGE, Qualification.LOW));
			Assert.Throws<ArgumentException>(() => new Worker(_home, _workplace, Worker.PENSION_AGE + 1, Qualification.LOW));

			Assert.Throws<ArgumentNullException>(() => new Worker(null, _workplace, 18, Qualification.LOW));
			Assert.Throws<ArgumentNullException>(() => new Worker(_home, null, 18, Qualification.LOW));

			Assert.DoesNotThrow(() => new Worker(_home, _workplace, 18, Qualification.LOW));
		}

		[Test]
		public void RetireTest()
		{
			Worker worker = new(_home, _workplace, Worker.PENSION_AGE - 1, Qualification.LOW);
			worker.IncreaseAge();
			Pensioner pensioner = worker.Retire();
			Assert.Multiple(() =>
			{
				Assert.That(pensioner.Age, Is.EqualTo(worker.Age));
				Assert.That(pensioner.LiveAt, Is.EqualTo(worker.LiveAt));
			});
		}

		[Test]
		public void TestPensionAmount()
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
			Assert.Multiple(() =>
			{
				Assert.That(pensioner.Pension, Is.EqualTo(expectedPension / 2.0f));
				Assert.That(pensioner.Age, Is.EqualTo(worker.Age));
			});
		}

		[Test]
		public void TestIncreaseQualification()
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
		public void TestDecreaseQualification()
		{
			Worker worker = new(_home, _workplace, 30, Qualification.HIGH);

			worker.DecreaseQualificaiton();
			Assert.That(worker.PersonQualification, Is.EqualTo(Qualification.MID));

			worker.DecreaseQualificaiton();
			Assert.That(worker.PersonQualification, Is.EqualTo(Qualification.LOW));

			worker.DecreaseQualificaiton();
			Assert.That(worker.PersonQualification, Is.EqualTo(Qualification.LOW));
		}

		[Test]
		public void TestPayTax()
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

	public class MockWorkplace : IWorkplace
	{
		void IWorkplace.Employ(Worker worker)
		{
			//Throw person into a magic black hole
		}

		Tile IWorkplace.GetTile()
		{
			throw new NotImplementedException();
		}

		List<Worker> IWorkplace.GetWorkers()
		{
			throw new NotImplementedException();
		}

		int IWorkplace.GetWorkersCount()
		{
			throw new NotImplementedException();
		}

		int IWorkplace.GetWorkersLimit()
		{
			throw new NotImplementedException();
		}

		void IWorkplace.RegisterWorkplace(RoadGrid roadGrid)
		{
			throw new NotImplementedException();
		}

		void IWorkplace.Unemploy(Worker worker)
		{
			//Nevermind, we don't really want to send him to the black hole
		}

		void IWorkplace.UnregisterWorkplace(RoadGrid roadGrid)
		{
			throw new NotImplementedException();
		}
	}

	public class MockResidential : IResidential
	{
		List<Person> IResidential.GetResidents()
		{
			throw new NotImplementedException();
		}

		int IResidential.GetResidentsCount()
		{
			throw new NotImplementedException();
		}

		int IResidential.GetResidentsLimit()
		{
			throw new NotImplementedException();
		}

		Tile IResidential.GetTile()
		{
			throw new NotImplementedException();
		}

		void IResidential.MoveIn(Person person)
		{
			//We are going to live at an imaginary place where we don't need to move in
		}

		void IResidential.MoveOut(Person person)
		{
			//That imaginary place don't fit our expectations so we are going to move out
		}

		void IResidential.RegisterResidential(RoadGrid roadGrid)
		{
			throw new NotImplementedException();
		}

		void IResidential.UnregisterResidential(RoadGrid roadGrid)
		{
			throw new NotImplementedException();
		}
	}
}
