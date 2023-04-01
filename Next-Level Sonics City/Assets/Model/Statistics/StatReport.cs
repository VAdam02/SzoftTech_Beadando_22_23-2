namespace Model.Statistics
{
    public class StatReport
    {
        public int Year { get; internal set; }
        public int Quarter { get; internal set; }
        public float Happiness { get; internal set; }

        public int Tax { get; internal set; }
        public int DestroyIncomes { get; internal set; }
        public int BuildExpenses { get; internal set; }
        public int MaintainanceCosts { get; internal set; }

        public int Incomes { get; internal set; }
        public int Expenses { get; internal set; }
        public int Total { get; internal set; }

        public int Population { get; internal set; }
        public int PopulationChange { get; internal set; }

        public int ElectricityProduced { get; internal set; }
        public int ElectricityConsumed { get; internal set; }

		public StatReport()
		{

		}
	}
}