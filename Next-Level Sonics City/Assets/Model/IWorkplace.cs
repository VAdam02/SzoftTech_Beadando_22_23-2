using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public interface IWorkplace
    {
<<<<<<< Updated upstream

=======
        public bool Employ(Person person);
        public void Fire(Person person);
        public List<Person> GetWorkers();
        public int GetWorkersCount();
        public int GetWorkersLimit();
>>>>>>> Stashed changes
    }
}