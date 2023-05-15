using Model.Tiles.Buildings;
using NUnit.Framework;
using System;

namespace Model
{
	public class PersonTest
	{
		[Test]
		public void Constructor_WithValidArguments_SetsProperties()
		{
			// Arrange
			IResidential residential = new MockResidentialBuildingTile();
			int age = 30;

			// Act
			Person person = new MockPerson(residential, age);

			// Assert
			Assert.AreEqual(residential, person.LiveAt);
			Assert.AreEqual(age, person.Age);
		}

		[Test]
		public void Constructor_WithNullResidential_ThrowsArgumentNullException()
		{
			// Arrange
			IResidential residential = null;
			int age = 30;

			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => new MockPerson(residential, age));
		}

		[Test]
		public void Constructor_WithInvalidAge_ThrowsArgumentException()
		{
			// Arrange
			IResidential residential = new MockResidentialBuildingTile();
			int invalidAge = 10;

			// Act & Assert
			Assert.Throws<ArgumentException>(() => new MockPerson(residential, invalidAge));
		}

		[Test]
		public void IncreaseAge_IncrementsAgeByOne()
		{
			// Arrange
			IResidential residential = new MockResidentialBuildingTile();
			int initialAge = 25;
			Person person = new MockPerson(residential, initialAge);

			// Act
			person.IncreaseAge();

			// Assert
			Assert.AreEqual(initialAge + 1, person.Age);
		}
	}
}