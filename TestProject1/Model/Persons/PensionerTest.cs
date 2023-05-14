using Model.Persons;
using Model.RoadGrids;

namespace Model
{
	[TestFixture]
	internal class PensionerTest
	{
		[Test]
		public void PensionerConstructorSetsProperties()
		{
			IResidential home = new MockResidential();
			int age = 67;
			float pension = 25f;

			Assert.Throws<ArgumentException>(() => new Pensioner(home, age, -1f));
			Assert.Throws<ArgumentException>(() => new Pensioner(home, Worker.PENSION_AGE - 1, pension));
			Assert.Throws<ArgumentException>(() => new Pensioner(null, age, pension));

			Pensioner pensioner = new(home, age, pension);
			Assert.Multiple(() =>
			{
				Assert.That(pensioner.LiveAt, Is.EqualTo(home));
				Assert.That(pensioner.Age, Is.EqualTo(age));
				Assert.That(pensioner.Pension, Is.EqualTo(pension));
			});
		}

		[Test]
		public void PensionerPayTaxReturnsZero()
		{
			IResidential home = new MockResidential();
			int age = 67;
			float pension = 1000f;
			Pensioner pensioner = new (home, age, pension);

			Assert.That(pensioner.PayTax(0.1f), Is.EqualTo(0f));
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
