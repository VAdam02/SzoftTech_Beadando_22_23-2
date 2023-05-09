using System;
using System.Collections;
using System.Collections.Generic;
using Model.Persons;
using Model.Tiles.Buildings;
using Model;
using UnityEngine;
using Model.Simulation;

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
			float happiness = 1.0f;
			Vector3 maincord = LiveAt.Coordinates;
			float cordx = LiveAt.Coordinates.x;
			float cordy = LiveAt.Coordinates.y;

			// TAX
			if( SimEngine.Instance.GetTax() < 10){
				happiness += 0.1f;
			}
			else if (SimEngine.Instance.GetTax() < 7.5 ){
				happiness += 0.2f;
			}
			else if (SimEngine.Instance.GetTax() == 0){
				happiness += 1.0f;
			}
			else if (SimEngine.Instance.GetTax() > 10){
				happiness -= 0.1f;
			}
			else if (SimEngine.Instance.GetTax() > 15){
				happiness -= 0.2f;
			}
			else if (SimEngine.Instance.GetTax() >= 20){
				happiness -= 0.3f;
			}

			//MONEY IN THE CITY

			if(SimEngine.Instance.GetMoney() < 0){
				happiness -= 0.05f;

			}
			else if(SimEngine.Instance.GetMoney() < -2000){
				happiness -= 0.15f;
			}

			//How many years is it negative TODO

			//In the specific area
			float d = 9;
			float r = d / 2;
			for(int i = (int)cordx; i < (int)cordx + d; i++){
				for(int j = (int)cordy; j < (int)cordy + d; j++){

					double distance = Math.Sqrt(Math.Pow(r - j, 2) + Math.Pow(r - i, 2));
					if(distance <= r){
						//na ezen belül benne van a "körben" itt megy a vizsgálat


					}
					else{

					}
				}
			}
			//Workplace is near to Home
			d = 19;
			r = d / 2;
			for(int i = 0; i < d; i++){
				for(int j = 0; j < d; j++){

					double distance = Math.Sqrt(Math.Pow(r - j, 2) + Math.Pow(r - i, 2));
					if(distance <= r){
						//na ezen belül benne van a "körben" itt megy a vizsgálat
						
						if(SimEngine.Instance.Tiles[i,j] is Industrial){
							
								if(SimEngine.Instance.Tiles[i,j].GetWorkers())
						}

					}
					else{

					}
				}
			}
			
			return happiness;
		}

		public void IncreaseAge()
		{
			++Age;
		}

		public abstract float PayTax(float taxRate);
	}
}