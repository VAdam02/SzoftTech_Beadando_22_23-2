using System;
using Model.Persons;

namespace Model
{
	public abstract class Person
	{
		private static ulong s_id;

		private readonly ulong _id;
        public IResidential LiveAt { get; protected set; }
		public int Age { get; protected set; }
        public Qualification Qualification { get; protected set; }

		public Person(IResidential residential, int age)
		{
			_id = s_id++;
			LiveAt = residential ?? throw new ArgumentNullException("Person must have a home");
			Age = age;
			if (Age < 18) throw new ArgumentException("Person cannot be younger than 18 years old");

			LiveAt.MoveIn(this);
		}

		public float GetHappiness()
		{
			//TODO
			throw new NotImplementedException();
		}

		public void IncreaseAge()
		{
			++Age;
		}

		public abstract float PayTax(float taxRate);
	}
}