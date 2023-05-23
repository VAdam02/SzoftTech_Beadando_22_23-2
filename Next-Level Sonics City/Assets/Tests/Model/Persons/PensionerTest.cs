using Model.RoadGrids;
using Model.Tiles.Buildings;
using NUnit.Framework;
using System;

namespace Model.Persons
{
	internal class PensionerTest
	{
		private MockResidentialBuildingTile _home;

		[SetUp]
		public void SetUp()
		{
			RoadGridManager.Reset();
			City.Reset();

			_home = new(0, 0, Rotation.Zero);
			City.Instance.SetTile(_home);
		}

		[Test]
		public void PensionerConstructorSetsProperties()
		{
			int age = 67;
			float pension = 25f;

			Assert.Throws<ArgumentException>(() => new Pensioner(_home, age, -1));
			Assert.Throws<ArgumentException>(() => new Pensioner(_home, Worker.PENSION_AGE - 1, pension));
			Assert.Throws<ArgumentNullException>(() => new Pensioner(null, age, pension));

			Pensioner pensioner = new(_home, age, pension);

			Assert.That(pensioner.Residential, Is.EqualTo(_home));
			Assert.That(pensioner.Age, Is.EqualTo(age));
			Assert.That(pensioner.Pension, Is.EqualTo(pension));
		}

		[Test]
		public void PensionerPayTaxReturnsZero()
		{
			int age = 67;
			float pension = 1000f;
			Pensioner pensioner = new(_home, age, pension);

			Assert.That(pensioner.PayTax(0.1f), Is.EqualTo(0f));
		}
	}
}