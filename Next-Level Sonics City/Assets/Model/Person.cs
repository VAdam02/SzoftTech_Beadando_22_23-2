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

		/// <summary>
		/// Creates a new person and moves him into the given residential
		/// </summary>
		/// <param name="residential">Residential where will live</param>
		/// <param name="age">Age of person</param>
		public Person(IResidential residential, int age)
		{
			_id = s_id++;
			LiveAt = residential ?? throw new ArgumentNullException("Person must have a home");
			Age = age;
			if (Age < 18) throw new ArgumentException("Person cannot be younger than 18 years old");

			LiveAt.MoveIn(this);
		}

		/// <summary>
		/// Get the happiness of the person
		/// </summary>
		/// <returns>Happiness of person</returns>
		public float GetHappiness()
		{
			//TODO
			return 0.5f;
		}

		/// <summary>
		/// <para>MUST BE CALLED ONLY BE MAIN THREAD</para>
		/// <para>Increase the age of the person by 1</para>
		/// </summary>
		public void IncreaseAge()
		{
			++Age;
		}

		public abstract float PayTax(float taxRate);
	}
}