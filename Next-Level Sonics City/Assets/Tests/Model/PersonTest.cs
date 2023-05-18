using Model.RoadGrids;
using Model.Tiles.Buildings;
using NUnit.Framework;
using System;

namespace Model
{
	public class PersonTest
	{
		private MockResidentialBuildingTile _residential;

		[SetUp]
		public void SetUp()
		{
			RoadGridManager.Reset();
			City.Reset();

			_residential = new(0, 0, Rotation.Zero);
			City.Instance.SetTile(_residential);

		}

		[Test]
		public void Constructor_WithValidArguments_SetsProperties()
		{
			int age = 30;

			Person person = new MockPerson(_residential, age);

			Assert.AreEqual(_residential, person.Residential);
			Assert.AreEqual(age, person.Age);
		}

		[Test]
		public void Constructor_WithNullResidential_ThrowsArgumentNullException()
		{
			IResidential residential = null;
			int age = 30;

			Assert.Throws<ArgumentNullException>(() => new MockPerson(residential, age));
		}

		[Test]
		public void Constructor_WithInvalidAge_ThrowsArgumentException()
		{
			int invalidAge = 10;

			Assert.Throws<ArgumentException>(() => new MockPerson(_residential, invalidAge));
		}

		[Test]
		public void IncreaseAge_IncrementsAgeByOne()
		{
			int initialAge = 25;
			Person person = new MockPerson(_residential, initialAge);

			person.IncreaseAge();

			Assert.AreEqual(initialAge + 1, person.Age);
		}
	}
}