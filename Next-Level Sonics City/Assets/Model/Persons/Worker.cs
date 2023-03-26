using Model;

namespace Persons
{
    public class Worker : Person
    {
        public IWorkplace WorkPlace { get; private set; }
        private float _taxSum = 0;
        private int _taxCount = 0;

        private const int TAXED_YEARS_FOR_PENSION = 20;

        public Worker(IWorkplace workPlace)
        {
            WorkPlace= workPlace;
        }

        public float GetPaidTax()
        {
            return TAXED_YEARS_FOR_PENSION * _taxSum / _taxCount;
        }
    }
}