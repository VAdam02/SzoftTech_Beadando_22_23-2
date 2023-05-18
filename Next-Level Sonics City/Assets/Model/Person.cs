using Model.Persons;
using Model.Statistics;
using System;
using UnityEngine;

namespace Model
{
	public abstract class Person
	{
		private static ulong s_id;

		private readonly ulong _id;
		public IResidential Residential { get; protected set; }
		public int Age { get; protected set; }
		public Qualification Qualification { get; protected set; }

		/// <summary>
		/// Creates a new person and moves him into the given residential
		/// </summary>
		/// <param name="residential">Residential where will live</param>
		/// <param name="age">Age of person</param>
		public Person(IResidential residential, int age)
		{
			_id = s_id++;
			Residential = residential ?? throw new ArgumentNullException("Person must have a home");
			Age = age;
			if (Age < 18) throw new ArgumentException("Person cannot be younger than 18 years old");

			Residential.MoveIn(this);

			City.Instance.AddPerson(_id, this);
		}

		/// <summary>
		/// Get the happiness of the person
		/// </summary>
		/// <returns>Happiness of person</returns>
		public virtual float GetHappiness()
		{
			float happiness = 0;
			float happinessWeight = 0;

			//happiness by tax
			happiness += Mathf.Clamp(Mathf.Cos(StatEngine.Instance.ResidentialTaxRate * Mathf.PI * 1.5f), 0, 1);
			happinessWeight += 1;

			//happiness by negative budget
			happiness += (float)1 / (StatEngine.Instance.NegativeBudgetSince + 1);
			happinessWeight += 1;

			//check the residential happiness
			(float happiness, float weight) residentialHappiness = Residential.HappinessByBuilding;
			happiness += residentialHappiness.happiness * residentialHappiness.weight;
			happinessWeight += residentialHappiness.weight;

			return happiness / happinessWeight;
			//(int)Residential.GetTile().Coordinates.x
			
			//TODO furthermore happiness parameter

			//In the specific area

			/*
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

			if(!SimEngine.Instance.IsIndustrialNearby(Residential)){
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
			*/
			
		}

		/// <summary>
		/// <para>MUST BE CALLED ONLY BE MAIN THREAD</para>
		/// <para>Increase the age of the person by 1</para>
		/// </summary>
		public void IncreaseAge()
		{
			++Age;
		}

		/// <summary>
		/// Calculate the tax for the person
		/// </summary>
		/// <param name="taxRate">Tax rate which should be included in calculations</param>
		/// <returns></returns>
		public abstract float PayTax(float taxRate);
	}
}