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
		 public virtual IWorkplace GetWorkplace() {
        	return null; // A nem dolgozóknak nincs munkahelye
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
			if(SimEngine.Instance.StatEngine.Budget<0){
				happiness += SimEngine.Instance.StatEngine.Budget/100000;
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
						
						if(SimEngine.Instance.GetTile(i,j) is StadionBuildingTile) {
							happiness += 0.05f;
						}
						if(SimEngine.Instance.GetTile(i,j) is PoliceDepartmentBuildingTile) {
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
			if(GetWorkplace() is not null){
				//check if person is worker the pensioners dont have workplaces
				float current = Vector3.Distance(maincord,GetWorkplace().GetTile().Coordinates);
				if(current < 10){
					happiness += happiness*(current/100);
				}
				else{
					happiness -= happiness*(current/1000);
				}
			}

			if(!SimEngine.Instance.IsIndustrialNearby(LiveAt)){
				happiness += 0.1f;
			}
			else{
				float distance3 = float.MaxValue;
				foreach ( IWorkplace industrial in SimEngine.Instance.RoadGridManager.RoadGrids){
					float current1 = Vector3.Distance(industrial.GetTile().Coordinates,maincord);
					if(current1 < distance3){
						distance3 = current1;
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