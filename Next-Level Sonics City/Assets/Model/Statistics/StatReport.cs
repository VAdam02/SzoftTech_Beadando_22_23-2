namespace Model.Statistics
{
	public class StatReport
	{
		public int Year { get; internal set; }
		public int Quarter { get; internal set; }
		public float Budget { get; internal set; }
		public float Happiness { get; internal set; }

		public float IncomeTax { get; internal set; }
		public float ResidentialTax { get; internal set; }
		public float BuildExpenses { get; internal set; }
		public float DestroyIncomes { get; internal set; }
		public float MaintainanceCosts { get; internal set; }
		public float Incomes { get; internal set; }
		public float Expenses { get; internal set; }
		public float Profit { get; internal set; }

		public int Population { get; internal set; }
		public int PopulationChange { get; internal set; }

		public int ElectricityProduced { get; internal set; }
		public int ElectricityConsumed { get; internal set; }

		public StatReport()
		{

		}
	}
}