using Model.Tiles.Buildings;

namespace Model.Persons
{
	public class Worker : Person
	{
		public IWorkplace WorkPlace { get; private set; }
		public Qualification PersonQualification { get; private set; }

		private float _taxSum = 0f;
		private int _taxCount = 0;

		private const int BASE_SALARY = 500; //dollar
		private const int TAXED_YEARS_FOR_PENSION = 20;
		private const int PENSION_AGE = 65;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="home"></param>
		/// <param name="workPlace"></param>
		/// <param name="age"></param>
		/// <param name="qualification"></param>
		public Worker(ResidentialBuildingTile home, IWorkplace workPlace, int age, Qualification qualification) : base(home, age)
		{
			WorkPlace = workPlace;
			PersonQualification = qualification;
		}

		public Pensioner Retire()
		{
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
			
			if (Age <= (PENSION_AGE - TAXED_YEARS_FOR_PENSION)) { RecordTax(currentTax); }

			return currentTax;
		}

		private void RecordTax(float paidTax)
		{
			++_taxCount;
			_taxSum += paidTax;
		}

		private float CalculateSalary()
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