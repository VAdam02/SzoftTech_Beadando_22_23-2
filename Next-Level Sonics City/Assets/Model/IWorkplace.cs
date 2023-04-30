using Model.Tiles.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Model
{
	public interface IWorkplace
	{
		public bool Employ(Person person);
		public bool Unemploy(Person person);
		public List<Person> GetWorkers();
		public int GetWorkersCount();
		public int GetWorkersLimit();
	}
}