using Model;

namespace Persons
{
    public class Worker : Person
    {
        public IWorkplace WorkPlace { get; private set; }
        public Qualification Qualification { get; private set; }
        public int BaseSalary { get; private set; }

        private float _taxSum = 0f;
        private int _taxCount = 0;

        private const int TAXED_YEARS_FOR_PENSION = 20;

        public Worker(IWorkplace workPlace, int baseSalary)
        {
            WorkPlace = workPlace;
            BaseSalary = baseSalary;
            Qualification = Qualification.LOW;
        }

        public Pensioner Retire()
        {
            float pension = (TAXED_YEARS_FOR_PENSION * _taxSum / _taxCount) / 2.0f;
            return new Pensioner(pension);
        }

        public void IncreaseQualification()
        {
            ++Qualification;
        }

        public void DecreaseQualificaiton()
        {
            --Qualification;
        }

        public float PayTax(float taxRate)
        {
            float currentTax = CalculateSalary() * taxRate;
            
            if (Age >= 45)
            {
                RecordTax(currentTax);
            }

            return currentTax;
        }

        private void RecordTax(float paidTax)
        {
            ++_taxCount;
            _taxSum += paidTax;
        }

        private float CalculateSalary()
        {
            float salary = 0;

            switch (Qualification)
            {
                case Qualification.MID:
                    salary = BaseSalary * 1.2f;
                    break;

                case Qualification.HIGH:
                    salary = BaseSalary * 1.5f;
                    break;

                default:
                    salary = BaseSalary;
                    break;
            }

            return salary;
        }
    }
}