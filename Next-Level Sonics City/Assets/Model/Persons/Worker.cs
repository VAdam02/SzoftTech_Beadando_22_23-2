using Model;

namespace Persons
{
    public class Worker : Person
    {
        public IWorkplace WorkPlace { get; private set; }
        public Qualification Qualification { get; private set; }

        private int _baseSalary;
        private float _taxSum = 0f;
        private int _taxCount = 0;

        private const int TAXED_YEARS_FOR_PENSION = 20;

        public Worker(IWorkplace workPlace, Qualification qualification, int baseSalary)
        {
            WorkPlace = workPlace;
            _baseSalary = baseSalary;
            Qualification = qualification;
        }

        public Pensioner Retire()
        {
            _taxCount /= 2;
            float pension = (TAXED_YEARS_FOR_PENSION * _taxSum / _taxCount) / 2.0f;
            return new Pensioner(pension);
        }

        public void IncreaseQualification()//if
        {
            ++Qualification;
        }

        public void DecreaseQualificaiton()//if
        {
            --Qualification;
        }
        
        public override float PayTax(float taxRate)
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
            float multiplier = 1.0f;
            if (Qualification is Qualification.HIGH)
            {
                return _baseSalary * 1.5f;
            }
            else if (Qualification is Qualification.MID)
            {
                return _baseSalary * 1.2f;
            }
            else { return _baseSalary; }
            //TODO
        }
    }
}