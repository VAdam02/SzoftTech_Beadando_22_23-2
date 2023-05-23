using System;

namespace Model
{

	internal class MockPerson : Person
	{
		public MockPerson(IResidential residential, int age) : base(residential, age)
		{

		}

		public override float PayTax(float taxRate)
		{
			throw new NotImplementedException();
		}
	}
}
