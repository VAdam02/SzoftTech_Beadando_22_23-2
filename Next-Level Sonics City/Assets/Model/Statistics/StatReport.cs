namespace Model.Statistics
{
	public class StatReport
	{
		public int Year { get; internal set; }
		public int Quarter { get; internal set; }
		public float Budget { get; internal set; }
		public float BudgetChange { get; internal set; } = 0;
		public float Happiness { get; internal set; }

		public float ResidentialTax { get; internal set; } = 0;
		public float WorkplaceTax { get; internal set; } = 0;
		public float Pension { get; internal set; } = 0;
		public float MaintainanceCosts { get; internal set; } = 0;

		public float DestroyIncomes { get; internal set; } = 0;
		public float BuildExpenses { get; internal set; } = 0;
		public float Incomes { get; internal set; } = 0;
		public float Expenses { get; internal set; } = 0;
		public float Total { get; internal set; } = 0;

		public int Population { get; internal set; }
		public int PopulationChange { get; internal set; } = 0;

		public StatReport(int year, int quarter, float budget, float happiness, int population)
		{
			Year = year;
			Quarter = quarter;
			Budget = budget;
			Happiness = happiness;
			Population = population;
		}
	}
}