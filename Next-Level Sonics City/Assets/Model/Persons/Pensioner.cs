using System;

namespace Model.Persons
{
	public class Pensioner : Person
	{
		public float Pension { get; private set; }

		protected override (float happiness, float weight) HappinessByPersonInheritance
		{
			get => (0.0f, 0.0f);
		}

		/// <summary>
		/// Creates a new pensioner and move in to the given residential
		/// </summary>
		/// <param name="residential">Residential where will live</param>
		/// <param name="age">Age of the person</param>
		/// <param name="pension">Amount of pension</param>
		public Pensioner(IResidential residential, int age, float pension) : base(residential, age)
		{
			Pension = pension;
			if (Pension < 0) throw new ArgumentException("Pension cannot be negative");
			if (Age < Worker.PENSION_AGE) throw new ArgumentException("Pensioner cannot be younger than " + Worker.PENSION_AGE + " years old");

			UpdateHappiness();
		}

		public override float PayTax(float taxRate)
		{
			return 0f;
		}

		public override void ForcedMoveOut()
		{
			Die();
		}

		public override void Die() => Dying();
		protected new void Dying() => base.Dying();

		public override void ForcedLockedRoadDestroy()
		{
			Die();
		}
	}
}