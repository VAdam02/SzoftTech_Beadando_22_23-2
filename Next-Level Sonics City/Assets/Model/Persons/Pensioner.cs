using Model;

namespace Persons
{
    public class Pensioner : Person
    {
        public float Pension { get; private set; }

        public Pensioner(float pension)
        {
            Pension = pension;
        }
    }
}