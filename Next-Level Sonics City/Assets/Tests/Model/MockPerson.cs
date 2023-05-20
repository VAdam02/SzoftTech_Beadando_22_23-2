using System;

namespace Model
{

	internal class MockPerson : Person
	{
		public MockPerson(IResidential residential, int age) : base(residential, age)
		{

		}

		public override (float happiness, float weight) HappinessByPersonInheritance => throw new NotImplementedException();

		public override float PayTax(float taxRate)
		{
			throw new NotImplementedException();
		}
	}
}
