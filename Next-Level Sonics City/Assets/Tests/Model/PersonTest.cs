using Model.Persons;
using Model.RoadGrids;
using Model.Tiles;
using Model.Tiles.Buildings;
using NUnit.Framework;
using System;

namespace Model
{
	public class PersonTest
	{
		private ResidentialBuildingTile _residential;

		[SetUp]
		public void SetUp()
		{
			RoadGridManager.Reset();
			City.Reset();
			for (int i = 0; i < City.Instance.GetSize(); i++)
			{
				for (int j = 0; j < City.Instance.GetSize(); j++)
				{
					City.Instance.SetTile(new EmptyTile(i, j));
				}
			}

			_residential = new(0, 0, 0, Rotation.Zero, ZoneBuildingLevel.ZERO);
			City.Instance.SetTile(_residential);

		}

		[Test]
		public void Constructor_WithValidArguments_SetsProperties()
		{
			int age = 30;

			Person person = new Pensioner(_residential, age, 100);

			Assert.AreEqual(_residential, person.Residential);
			Assert.AreEqual(age, person.Age);
		}

		[Test]
		public void Constructor_WithNullResidential_ThrowsArgumentNullException()
		{
			IResidential residential = null;
			int age = 30;

			Assert.Throws<ArgumentNullException>(() => new Pensioner(residential, age, 100));
		}

		[Test]
		public void Constructor_WithInvalidAge_ThrowsArgumentException()
		{
			int invalidAge = 10;

			Assert.Throws<ArgumentException>(() => new Pensioner(_residential, invalidAge, 100));
		}

		[Test]
		public void IncreaseAge_IncrementsAgeByOne()
		{
			int initialAge = 25;
			Person person = new Pensioner(_residential, initialAge, 100);

			person.IncreaseAge();

			Assert.AreEqual(initialAge + 1, person.Age);
		}
	}
}