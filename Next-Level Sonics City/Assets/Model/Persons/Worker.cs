using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace Model.Persons
{
	public class Worker : Person
	{
		public IWorkplace WorkPlace { get; private set; }
		public Qualification PersonQualification { get; private set; }

		private float _taxSum = 0f;
		private int _taxCount = 0;

		private const int BASE_SALARY = 500; //dollar
		public const int TAXED_YEARS_FOR_PENSION = 20;
		public const int PENSION_AGE = 65;

		/// <summary>
		/// Creates a new worker and move in to the residential and employ to the workplace
		/// </summary>
		/// <param name="residential">Residential where the worker will live</param>
		/// <param name="workPlace">Workplace where the worker will work</param>
		/// <param name="age">Age of the person</param>
		/// <param name="qualification">Qualification of worker</param>
		public Worker(IResidential residential, IWorkplace workPlace, int age, Qualification qualification) : base(residential, age)
		{
			if (age < 18 || PENSION_AGE <= age) throw new ArgumentException("Worker cannot be younger than 18 and older than " + PENSION_AGE + " years old");
			WorkPlace = workPlace ?? throw new ArgumentNullException("Worker must have a workplace");
			PersonQualification = qualification;

			WorkPlace.Employ(this);
		}
		 public override IWorkplace GetWorkplace() {
        	return WorkPlace;
   		 }

		/// <summary>
		/// Retires the worker and recreate as a pensioner
		/// </summary>
		/// <returns>Pensioner that created based on the worker datas</returns>
		public Pensioner Retire()
		{
			if (Age < PENSION_AGE) throw new ArgumentException("Worker cannot retire before " + PENSION_AGE + " years old");

			WorkPlace.Unemploy(this);

			float pension = _taxSum / _taxCount / 2.0f;
			return new Pensioner(LiveAt, Age, pension);
		}

		/// <summary>
		/// Increase the qualification of the worker
		/// </summary>
		public void IncreaseQualification()
		{
			if (PersonQualification == Qualification.HIGH) return;
			++PersonQualification;
		}

		/// <summary>
		/// Decrease the qualification of the worker
		/// </summary>
		public void DecreaseQualification()
		{
			if (PersonQualification == Qualification.LOW) return;
			--PersonQualification;
		}
		
		/// <summary>
		/// Pay tax based on the tax rate
		/// </summary>
		/// <param name="taxRate">Taxrate that should be include in calculations</param>
		/// <returns>Amount of tax payed</returns>
		public override float PayTax(float taxRate)
		{
			float currentTax = CalculateSalary() * taxRate;

			if (Age >= PENSION_AGE) { return 0; }

			if (Age <= (PENSION_AGE - TAXED_YEARS_FOR_PENSION)) { RecordTax(currentTax); }

			return currentTax;
		}

		/// <summary>
		/// Log the tax that the worker payed for the pension
		/// </summary>
		/// <param name="paidTax"></param>
		private void RecordTax(float paidTax)
		{
			++_taxCount;
			_taxSum += paidTax;
		}

		/// <summary>
		/// Calculate the salary of the worker based on the qualification and other parameters
		/// </summary>
		/// <returns>Amount of salary</returns>
		internal float CalculateSalary()
		{
			float multiplier = 1.0f;
			switch (PersonQualification)
			{
				case Qualification.HIGH:
					multiplier *= 1.5f;
					break;
				case Qualification.MID:
					multiplier *= 1.2f;
					break;
			}

			//TODO add more parameters to calculate salary

			return BASE_SALARY * multiplier;
		}
	}
}