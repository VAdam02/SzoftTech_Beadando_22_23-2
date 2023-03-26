using Model;

namespace Persons
{
    public class Pensioner : Person
    {
        public int Pension { get; private set; }

        public Pensioner(int pension)
        {
            Pension = pension;
        }
    }
}