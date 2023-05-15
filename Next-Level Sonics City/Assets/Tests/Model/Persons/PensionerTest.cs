using Model.Tiles.Buildings;
using NUnit.Framework;
using System;

namespace Model.Persons
{
	internal class PensionerTest
	{
		[Test]
		public void PensionerConstructorSetsProperties()
		{
			IResidential home = new MockResidentialBuildingTile();
			int age = 67;
			float pension = 25f;

			Assert.Throws<ArgumentException>(() => new Pensioner(home, age, -1));
			Assert.Throws<ArgumentException>(() => new Pensioner(home, Worker.PENSION_AGE - 1, pension));
			Assert.Throws<ArgumentNullException>(() => new Pensioner(null, age, pension));

			Pensioner pensioner = new(home, age, pension);

			Assert.That(pensioner.LiveAt, Is.EqualTo(home));
			Assert.That(pensioner.Age, Is.EqualTo(age));
			Assert.That(pensioner.Pension, Is.EqualTo(pension));
		}

		[Test]
		public void PensionerPayTaxReturnsZero()
		{
			IResidential home = new MockResidentialBuildingTile();
			int age = 67;
			float pension = 1000f;
			Pensioner pensioner = new(home, age, pension);

			Assert.That(pensioner.PayTax(0.1f), Is.EqualTo(0f));
		}
	}
}