using Model.Tiles.Buildings;

namespace Model.Persons
{
	public class Pensioner : Person
	{
		public float Pension { get; private set; }

		public Pensioner(Residential home, int age, float pension) : base(home, age)
		{
			Pension = pension;
		}

		public override float PayTax(float taxRate)
		{
			return 0f;
		}
	}
}