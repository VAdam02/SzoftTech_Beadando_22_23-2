using System;

namespace Model.Persons
{
	public class Pensioner : Person
	{
		public float Pension { get; private set; }

		public Pensioner(IResidential home, int age, float pension) : base(home, age)
		{
			Pension = pension;
			if (Pension < 0) throw new ArgumentException("Pension cannot be negative");
			if (Age < Worker.PENSION_AGE) throw new ArgumentException("Pensioner cannot be younger than " + Worker.PENSION_AGE + " years old");
		}

		public override float PayTax(float taxRate)
		{
			return 0f;
		}
	}
}