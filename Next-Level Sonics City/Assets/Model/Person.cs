using System;
using System.Collections;
using System.Collections.Generic;
using Model.Persons;
using Model.Tiles.Buildings;

namespace Model
{
	public abstract class Person
	{
		private static ulong s_id;

		private ulong _id;
        public ResidentialBuildingTile LiveAt { get; protected set; }
		public int Age { get; protected set; }
        public Qualification Qualification { get; protected set; }

		public Person(ResidentialBuildingTile home, int age)
		{
			_id = s_id++;
			LiveAt = home;
			Age = age;
		}

		public float GetHappiness()
		{
			//TODO
			throw new NotImplementedException();
		}

		public void IncreaseAge()
		{
			++Age;
		}

		public abstract float PayTax(float taxRate);
	}
}