using Model.RoadGrids;
using Model.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Tests")]

namespace Model.Persons
{
	public class Worker : Person
	{
		public IWorkplace WorkPlace { get; private set; }
		public Qualification PersonQualification { get; private set; }
		public List<IRoadGridElement> PathToWork { get; private set; }

		protected override (float happiness, float weight) HappinessByPersonInheritance
		{
			get
			{
				if (WorkPlace == null || Residential == null) return (0.0f, 0.0f);
				 
				List<(float happiness, float weight)> happinessChangers = new()
				{
					(1, 5f - Mathf.Atan(Mathf.Sqrt(Mathf.Pow(WorkPlace.GetTile().Coordinates.x - Residential.GetTile().Coordinates.x, 2) + Mathf.Pow(WorkPlace.GetTile().Coordinates.y - Residential.GetTile().Coordinates.y, 2))) * Mathf.PI),
					WorkPlace.HappinessByBuilding
				};

				float happinessSum = happinessChangers.Aggregate(0.0f, (acc, item) => acc + item.happiness * item.weight);
				float happinessWeight = happinessChangers.Aggregate(0.0f, (acc, item) => acc + item.weight);
				return (happinessSum / happinessWeight, happinessWeight);
			}
		}

		private float _taxSum = 0f;
		private int _taxCount = 0;

		private const int BASE_SALARY = 500; //dollar
		public const int TAXED_YEARS_FOR_PENSION = 20;
		public const int PENSION_AGE = 65;

		/// <summary>
		/// Creates a new worker and move in to the residential and employ to the workplace
		/// </summary>
		/// <param name="residential">Residential where the worker will live</param>
		/// <param name="workPlace">Workplace where the worker will work</param>
		/// <param name="age">Age of the person</param>
		/// <param name="qualification">Qualification of worker</param>
		public Worker(IResidential residential, IWorkplace workPlace, int age, Qualification qualification) : base(residential, age)
		{
			if (age < 18 || PENSION_AGE <= age) throw new ArgumentException("Worker cannot be younger than 18 and older than " + PENSION_AGE + " years old");
			WorkPlace = workPlace ?? throw new ArgumentNullException("Worker must have a workplace");

			PathToWork = RoadGridManager.GetPathOnRoad(RoadGridManager.GetRoadGrigElementByBuilding((Building)residential.GetTile()), RoadGridManager.GetRoadGrigElementByBuilding((Building)workPlace.GetTile()), int.MaxValue);
			foreach (IRoadGridElement element in PathToWork)
			{
				element.LockBy(this);
			}

			PersonQualification = qualification;
			WorkPlace.Employ(this);

			UpdateHappiness();
		}

		/// <summary>
		/// Creates a new worker and move in to the residential and employ to the workplace
		/// </summary>
		/// <param name="residential">Residential where the worker will live</param>
		/// <param name="workPlace">Workplace where the worker will work</param>
		/// <param name="age">Age of the person</param>
		/// <param name="qualification">Qualification of worker</param>
		/// <param name="pathToWork">Prefered path to work</param>
		public Worker(IResidential residential, IWorkplace workPlace, int age, Qualification qualification, List<IRoadGridElement> pathToWork) : base(residential, age)
		{
			if (age < 18 || PENSION_AGE <= age) throw new ArgumentException("Worker cannot be younger than 18 and older than " + PENSION_AGE + " years old");
			WorkPlace = workPlace ?? throw new ArgumentNullException("Worker must have a workplace");

			if (pathToWork == null || pathToWork[0] != RoadGridManager.GetRoadGrigElementByBuilding((Building)residential) || pathToWork[^1] != RoadGridManager.GetRoadGrigElementByBuilding((Building)workPlace))
			{
				throw new ArgumentException("Path to work must be started with residential and ended with workplace");
			}
			PathToWork = pathToWork;
			foreach (IRoadGridElement element in PathToWork)
			{
				element.LockBy(this);
			}

			PersonQualification = qualification;
			
			WorkPlace.Employ(this);

			UpdateHappiness();
		}

		/// <summary>
		/// Retires the worker and recreate as a pensioner
		/// </summary>
		/// <returns>Pensioner that created based on the worker datas</returns>
		public Pensioner Retire()
		{
			if (Age < PENSION_AGE) throw new ArgumentException("Worker cannot retire before " + PENSION_AGE + " years old");

			WorkPlace.Unemploy(this);

			float pension = _taxSum / _taxCount / 2.0f;
			return new Pensioner(Residential, Age, pension);
		}

		/// <summary>
		/// Increase the qualification of the worker
		/// </summary>
		public void IncreaseQualification()
		{
			if (PersonQualification == Qualification.HIGH) return;
			++PersonQualification;
		}

		/// <summary>
		/// Decrease the qualification of the worker
		/// </summary>
		public void DecreaseQualification()
		{
			if (PersonQualification == Qualification.LOW) return;
			--PersonQualification;
		}
		
		/// <summary>
		/// Pay tax based on the tax rate
		/// </summary>
		/// <param name="taxRate">Taxrate that should be include in calculations</param>
		/// <returns>Amount of tax payed</returns>
		public override float PayTax(float taxRate)
		{
			float currentTax = CalculateSalary() * taxRate;

			if (Age >= PENSION_AGE) { return 0; }

			if (Age <= (PENSION_AGE - TAXED_YEARS_FOR_PENSION)) { RecordTax(currentTax); }

			return currentTax;
		}

		/// <summary>
		/// Log the tax that the worker payed for the pension
		/// </summary>
		/// <param name="paidTax"></param>
		private void RecordTax(float paidTax)
		{
			++_taxCount;
			_taxSum += paidTax;
		}

		/// <summary>
		/// Calculate the salary of the worker based on the qualification and other parameters
		/// </summary>
		/// <returns>Amount of salary</returns>
		internal float CalculateSalary()
		{
			float multiplier = 1.0f;
			switch (PersonQualification)
			{
				case Qualification.HIGH:
					multiplier *= 1.5f;
					break;
				case Qualification.MID:
					multiplier *= 1.2f;
					break;
			}

			return BASE_SALARY * multiplier;
		}

		public void ForcedUnemploy()
		{
			IWorkplace _old = WorkPlace;
			WorkPlace?.Unemploy(this);
			lock (PathToWork)
			{
				while (PathToWork.Count > 0)
				{
					PathToWork[0].UnlockBy(this);
					PathToWork.RemoveAt(0);
				}
			}

			WorkPlace = LookForWorkplaceByResidential(Residential, out List<IRoadGridElement> shortestPath);
			if (WorkPlace == null) { Die(); return; }

			PathToWork = shortestPath;
			foreach (IRoadGridElement element in PathToWork)
			{
				element.LockBy(this);
			}
			WorkPlace.Employ(this);

			UpdateHappiness();
		}

		public override void ForcedMoveOut()
		{
			IResidential _old = Residential;

			Residential?.MoveOut(this);
			lock (PathToWork)
			{
				while (PathToWork.Count > 0)
				{
					PathToWork[0].UnlockBy(this);
					PathToWork.RemoveAt(0);
				}
			}

			Residential = LookForResidentialByWorkplace(WorkPlace, out List<IRoadGridElement> shortestPath);
			if (Residential == null) { Die(); return; }

			PathToWork = shortestPath;
			foreach (IRoadGridElement element in PathToWork)
			{
				element.LockBy(this);
			}
			Residential.MoveIn(this);
			
			UpdateHappiness();
		}

		public override void ForcedLockedRoadDestroy()
		{
			if (RoadGridManager.GetRoadGrigElementByBuilding((Building)Residential)?.RoadGrid.FreeWorkplaces.Count > 0)
			{
				ForcedUnemploy();
			}
			else if (RoadGridManager.GetRoadGrigElementByBuilding((Building)WorkPlace)?.RoadGrid.FreeResidentials.Count > 0)
			{
				ForcedMoveOut();
			}
			else
			{
				Die();
			}
		}

		public override void Die() => Dying();
		protected new void Dying()
		{
			base.Dying();

			WorkPlace?.Unemploy(this);

			lock (PathToWork)
			{
				while (PathToWork.Count > 0)
				{
					PathToWork[0].UnlockBy(this);
					PathToWork.RemoveAt(0);
				}
			}
		}
	}
}