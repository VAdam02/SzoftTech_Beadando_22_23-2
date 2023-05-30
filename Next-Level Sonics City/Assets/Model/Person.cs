using Model.Persons;
using Model.RoadGrids;
using Model.Statistics;
using Model.Tiles;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
	public abstract class Person
	{
		private static ulong s_id;

		public readonly ulong ID;
		public IResidential Residential { get; protected set; }
		public int Age { get; protected set; }
		public Qualification Qualification { get; protected set; }
		protected abstract (float happiness, float weight) HappinessByPersonInheritance { get; }

		/// <summary>
		/// Creates a new person and moves him into the given residential
		/// </summary>
		/// <param name="residential">Residential where will live</param>
		/// <param name="age">Age of person</param>
		public Person(IResidential residential, int age)
		{
			ID = s_id++;
			Residential = residential ?? throw new ArgumentNullException("Person must have a home");
			Age = age;
			if (Age < 18) throw new ArgumentException("Person cannot be younger than 18 years old");

			Residential.MoveIn(this);
			Residential.HappinessByBuildingChanged += (sender, e) => UpdateHappiness();

			City.Instance.AddPerson(this);
			City.Instance.HappinessByCityChanged += (sender, e) => UpdateHappiness();
		}

		public event EventHandler<float> HappinessByPersonChanged;

		public float Happiness { get; private set; }

		protected void UpdateHappiness()
		{
			float oldHappiness = Happiness;

			float happiness = 0;
			float happinessWeight = 0;

			//happiness by city
			(float happiness, float weight) cityHappiness = City.Instance.HappinessByCity;
			happiness += cityHappiness.happiness * cityHappiness.weight;
			happinessWeight += cityHappiness.weight;

			//happiness and weight by residential
			if (Residential != null)
			{
				(float happiness, float weight) residentialHappiness = Residential.HappinessByBuilding;
				happiness += residentialHappiness.happiness * residentialHappiness.weight;
				happinessWeight += residentialHappiness.weight;
			}

			//happiness and weight by inheritance
			(float happiness, float weight) inheritanceHappiness = HappinessByPersonInheritance;
			happiness += inheritanceHappiness.happiness * inheritanceHappiness.weight;
			happinessWeight += inheritanceHappiness.weight;

			Happiness = happiness / (happinessWeight == 0 ? 1 : happinessWeight);

			HappinessByPersonChanged?.Invoke(this, oldHappiness);
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

		public abstract void ForcedMoveOut();

		public abstract void ForcedLockedRoadDestroy();




		public abstract void Die();
		protected void Dying()
		{
			Debug.Log(ID + " died");
			UpdateHappiness();
			City.Instance.RemovePerson(this);
			Residential?.MoveOut(this);
		}

		public static IWorkplace LookForWorkplaceInRoadGrid(RoadGrid target)
		{
            System.Random rnd = new();
			List<IWorkplace> workplaces = FilteredWorkplacesInRoadGrid(target);
			return workplaces.Count > 0 ? workplaces[rnd.Next(0, workplaces.Count)] : null;
		}

		public static List<IWorkplace> FilteredWorkplacesInRoadGrid(RoadGrid target)
		{
			System.Random rnd = new();
			List<IWorkplace> workplaces;

			List<IWorkplace> canBeOther = target.FreeOtherWorkplaces;
			List<IWorkplace> canBeComOrInd;
			if (target.FreeIndustrialWorkplaces.Count > 0 && target.FreeCommercialWorkplaces.Count > 0)
			{
				canBeComOrInd = rnd.NextDouble() < StatEngine.Instance.GetCommercialWorkersPercentToCommercialAndIndustrialWorkers() ?
									target.FreeIndustrialWorkplaces :
									target.FreeCommercialWorkplaces;
			}
			else
			{
				if (target.FreeIndustrialWorkplaces.Count > 0) { canBeComOrInd = target.FreeIndustrialWorkplaces; }
				else if (target.FreeCommercialWorkplaces.Count > 0) { canBeComOrInd = target.FreeCommercialWorkplaces; }
				else { canBeComOrInd = new(); }
			}

			if (canBeOther.Count != 0 && canBeComOrInd.Count != 0) { workplaces = rnd.Next(0, 2) == 0 ? canBeOther : canBeComOrInd; }
			else { workplaces = canBeOther ?? canBeComOrInd; }

			return workplaces;
		}

		public static IResidential LookForResidentialByWorkplace(IWorkplace workplace, out List<IRoadGridElement> shortestPath)
		{
			IRoadGridElement workplaceroad = RoadGridManager.GetRoadGrigElementByBuilding((Building)workplace);
			IResidential residential = null;
			shortestPath = new();
			float interestValue = -1;
			foreach (IResidential curResidential in workplaceroad.RoadGrid.FreeResidentials)
			{
				try
				{
					List<IRoadGridElement> curPath = RoadGridManager.GetPathOnRoad(RoadGridManager.GetRoadGrigElementByBuilding((Building)curResidential), workplaceroad);
					float curInterestValue = CalculateInterestValue(curResidential, curPath);
					if (curInterestValue > interestValue)
					{
						shortestPath = curPath;
						residential = curResidential;
						interestValue = curInterestValue;
					}
				}
				catch { }
			}

			return residential;
		}

		public static float CalculateInterestValue(IResidential residential, List<IRoadGridElement> path)
		{
			float multiplierByHappiness = residential.HappinessByBuilding.weight == 0 ? 1 : Mathf.Tan(residential.HappinessByBuilding.happiness * 0.9f * Mathf.PI / 2);
			float multiplierByPath = 5 / (float)path.Count;
			float multiplierByResidents = (residential.ResidentLimit == 1 ? 1 : 0.25f / Mathf.Tan((residential.GetResidentsCount() / (float)residential.ResidentLimit * 0.9f + 0.1f) * Mathf.PI / 2));
			return multiplierByHappiness * multiplierByPath * multiplierByResidents;
		}

		public static IWorkplace LookForWorkplaceByResidential(IResidential residential, out List<IRoadGridElement> shortestPath)
		{
			IRoadGridElement residentialRoad = RoadGridManager.GetRoadGrigElementByBuilding((Building)residential);
			IWorkplace workplace = null;
			shortestPath = new();
			float interestValue = -1;
			foreach (IWorkplace curWorkplace in FilteredWorkplacesInRoadGrid(residentialRoad.RoadGrid))
			{
				try
				{
					List<IRoadGridElement> curPath = RoadGridManager.GetPathOnRoad(RoadGridManager.GetRoadGrigElementByBuilding((Building)curWorkplace), residentialRoad);
					float curInterestValue = CalculateInterestValue(curWorkplace, curPath);
					if (curInterestValue > interestValue)
					{
						shortestPath = curPath;
						workplace = curWorkplace;
						interestValue = curInterestValue;
					}
				}
				catch { }
			}
			return workplace;
		}

		public static float CalculateInterestValue(IWorkplace workplace, List<IRoadGridElement> path)
		{
			float multiplierByHappiness = workplace.HappinessByBuilding.weight == 0 ? 1 : Mathf.Tan(workplace.HappinessByBuilding.happiness * 0.9f * Mathf.PI / 2);
			float multiplierByPath = 5 / (float)path.Count;
			float multiplierByResidents = (workplace.WorkplaceLimit == 1 ? 1 : 0.25f / Mathf.Tan((workplace.GetWorkersCount() / (float)workplace.WorkplaceLimit * 0.9f + 0.1f) * Mathf.PI / 2));
			return multiplierByHappiness * multiplierByPath * multiplierByResidents;
		}
	}
}