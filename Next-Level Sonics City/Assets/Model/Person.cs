using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.Tiles.Buildings;

namespace Model
{
	public abstract class Person : MonoBehaviour
	{
		private static ulong s_id = 0;

		private ulong _id;
		public int Age { get; protected set; }
		public Residential LiveAt { get; protected set; }

		public Person(Residential home, int age)
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