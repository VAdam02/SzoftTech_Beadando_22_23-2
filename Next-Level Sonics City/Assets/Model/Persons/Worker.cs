using System;

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

		public Worker(IResidential home, IWorkplace workPlace, int age, Qualification qualification) : base(home, age)
		{
			if (age < 18 || PENSION_AGE <= age) throw new ArgumentException("Worker cannot be younger than 18 and older than " + PENSION_AGE + " years old");
			WorkPlace = workPlace ?? throw new ArgumentNullException("Worker must have a workplace");
			PersonQualification = qualification;

			WorkPlace.Employ(this);
		}

		public Pensioner Retire()
		{
			if (Age < PENSION_AGE) throw new ArgumentException("Worker cannot retire before " + PENSION_AGE + " years old");

			WorkPlace.Unemploy(this);

			float pension = _taxSum / _taxCount / 2.0f;
			return new Pensioner(LiveAt, Age, pension);
		}

		public void IncreaseQualification()
		{
			if (PersonQualification == Qualification.HIGH) return;
			++PersonQualification;
		}

		public void DecreaseQualificaiton()
		{
			if (PersonQualification == Qualification.LOW) return;
			--PersonQualification;
		}
		
		public override float PayTax(float taxRate)
		{
			float currentTax = CalculateSalary() * taxRate;

			if (Age >= PENSION_AGE) { return 0; }

			if (Age <= (PENSION_AGE - TAXED_YEARS_FOR_PENSION)) { RecordTax(currentTax); }

			return currentTax;
		}

		private void RecordTax(float paidTax)
		{
			++_taxCount;
			_taxSum += paidTax;
		}

		public float CalculateSalary()
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