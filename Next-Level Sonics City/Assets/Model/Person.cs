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
			float happiness = 0.75f;
			Vector3 maincord = LiveAt.Coordinates;
			float cordx = LiveAt.Coordinates.x;
			float cordy = LiveAt.Coordinates.y;

			// TAX
			if(SimEngine.Instance.GetTax() <= 13){
				happiness += (SimEngine.Instance.GetTax() / 100);

			}
			else{
				happiness -= (SimEngine.Instance.GetTax() / 100);
			}

			//MONEY IN THE CITY
			if(SimEngine.Instance.GetMoney()<0){
				happiness += SimEngine.Instance.GetMoney()/100000;
			}
			//How many years is it negative TODO

			//In the specific area
			float d = 19;
			float r = d / 2;
			int policeStations = 0;
			for(int i = (int)cordx; i < (int)cordx + d; i++){
				for(int j = (int)cordy; j < (int)cordy + d; j++){

					double distance1 = Math.Sqrt(Math.Pow(r - j, 2) + Math.Pow(r - i, 2));
					if(distance1 <= r){
						//na ezen belül benne van a "körben" itt megy a vizsgálat
						
						if(SimEngine.Instance.GetTile(i,j) is Stadion){
							happiness += 0.05f;
						}
						if(SimEngine.Instance.GetTile(i,j) is PoliceDepartment ){
							IWorkplace temp = (IWorkplace)SimEngine.Instance.GetTile(i,j);
							if(temp.GetWorkersCount() > 0){
								policeStations++;
							}
						}

					}
					else{

					}
				}
			}
			happiness += policeStations*(happiness/100);
			//Workplace is near to Home
			float distance2 = float.MaxValue;
			foreach(IWorkplace workplace in SimEngine.Instance.RoadGridManager.RoadGrids){
				float current = Vector3.Distance(maincord,workplace.GetTile().Coordinates);
				if(current < distance2){
					distance2 = current;
				}
			}
			if(distance2 < 10){
			happiness += happiness*(distance2/100);
			}
			else{
				happiness -= happiness*(distance2/1000);
			}

			if(!SimEngine.Instance.isIndustrialNearby(LiveAt)){
				happiness += 0.1f;
			}
			else{
				float distance3 = float.MaxValue;
				foreach ( IWorkplace industrial in SimEngine.Instance.RoadGridManager.RoadGrids){
					float current = Vector3.Distance(industrial.GetTile().Coordinates,maincord);
					if(current < distance3){
						distance3 = current;
					}
				}
				happiness -= distance3/10000; 
			}
			
			if(happiness > 1){
				happiness = 1;
				return happiness;
			}
			else{
				return happiness;
			}
			
		}

		public void IncreaseAge()
		{
			++Age;
		}

		public abstract float PayTax(float taxRate);
	}
}