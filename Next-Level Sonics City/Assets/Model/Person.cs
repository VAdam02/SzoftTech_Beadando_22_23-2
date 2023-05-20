using Model.Persons;
using Model.Statistics;
using System;
using UnityEngine;

namespace Model
{
	public abstract class Person
	{
		private static ulong s_id;

		public readonly ulong ID;
		public IResidential Residential { get; protected set; }
		public int Age { get; protected set; }
		public Qualification Qualification { get; protected set; }
		public abstract (float happiness, float weight) HappinessByPersonInheritance { get; }

		/// <summary>
		/// Creates a new person and moves him into the given residential
		/// </summary>
		/// <param name="residential">Residential where will live</param>
		/// <param name="age">Age of person</param>
		public Person(IResidential residential, int age)
		{
			ID = s_id++;
			Residential = residential ?? throw new ArgumentNullException("Person must have a home");
			Age = age;
			if (Age < 18) throw new ArgumentException("Person cannot be younger than 18 years old");

			Residential.MoveIn(this);

			City.Instance.AddPerson(this);
		}

		/// <summary>
		/// Get the happiness of the person
		/// </summary>
		/// <returns>Happiness of person</returns>
		public virtual float GetHappiness()
		{
			float happiness = 0;
			float happinessWeight = 0;

			//happiness by tax //TODO make it as a city thing
			happiness += Mathf.Clamp(Mathf.Cos(StatEngine.Instance.ResidentialTaxRate * Mathf.PI * 1.5f), 0, 1) * 10;
			happinessWeight += 10;

			//happiness weight by negative budget //TODO make it as a city thing
			happiness += 0;
			happinessWeight += StatEngine.Instance.NegativeBudgetSince;

			//happiness and weight by residential
			(float happiness, float weight) residentialHappiness = Residential.HappinessByBuilding;
			happiness += residentialHappiness.happiness * residentialHappiness.weight;
			happinessWeight += residentialHappiness.weight;

			//happiness and weight by inheritance
			(float happiness, float weight) inheritanceHappiness = HappinessByPersonInheritance;
			happiness += inheritanceHappiness.happiness * inheritanceHappiness.weight;
			happinessWeight += inheritanceHappiness.weight;

			return happiness / (happinessWeight == 0 ? 1 : happinessWeight); 
		}

		/// <summary>
		/// <para>MUST BE CALLED ONLY BE MAIN THREAD</para>
		/// <para>Increase the age of the person by 1</para>
		/// </summary>
		public void IncreaseAge()
		{
			++Age;
		}

		/// <summary>
		/// Calculate the tax for the person
		/// </summary>
		/// <param name="taxRate">Tax rate which should be included in calculations</param>
		/// <returns></returns>
		public abstract float PayTax(float taxRate);
	}
}