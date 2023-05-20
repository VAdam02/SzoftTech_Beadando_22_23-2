using Model.RoadGrids;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model.Tiles.Buildings
{
	public class ResidentialBuildingTile : Building, IResidential, IZoneBuilding
	{
		#region ResidentialElementsStructs
		public struct ResidentialDesignBase
		{
			public bool ForeBigBase;
			public bool BackBigBase;
			public bool OnlyFloor;
			public bool OnlyStorey;

			public GameObject Prefab;

			public Vector3 ForeTerraceAttachPoint;
			public Vector3 BackTerraceAttachPoint;
		}

		public struct ResidentialDesignTerrace
		{
			public bool NeedSmallNextLevel;
			public bool NeedBigNextLevel;
			public bool OnlyFloor;
			public bool OnlyStorey;

			public GameObject Prefab;

			public List<Vector3> WindowAttachPoints;
			public Vector3 DoorAttachPoint;
		}

		public struct ResidentialDesignJustWindowTerrace
		{
			public bool OnlyFloor;
			public bool OnlyStorey;
			public bool HasDoor;

			public GameObject Prefab;

			public List<Vector3> WindowAttachPoints;
			public Vector3 DoorAttachPoint;
		}

		public struct ResidentialDesignRoof
		{
			public bool ForeBigBase;
			public bool BackBigBase;

			public GameObject Prefab;

			public Vector3 ForeTerraceAttachPoint;
			public Vector3 BackTerraceAttachPoint;
		}

		public struct ResidentialDesignTerraceRoof
		{
			public GameObject Prefab;

			public Vector3 WindowAttachPoint;
		}
		#endregion

		#region ResidentialDesignElements
		private static JObject _residentalGenerationRules;
		public static JObject ResidentialDesignRules { get { _residentalGenerationRules ??= JObject.Parse(Resources.Load<TextAsset>("Tiles/ResidentialBuildingTile/ResidentialBuildingTileGenerationRule").text); return _residentalGenerationRules; } }

		private static List<ResidentialDesignBase> _residentialDesignBases;
		public static List<ResidentialDesignBase> ResidentialDesignBases { get { if (_residentialDesignBases == null) LoadResidentialDesignBases(); return _residentialDesignBases; } }
		private static List<ResidentialDesignTerrace> _residentialDesignTerraces;
		public static List<ResidentialDesignTerrace> ResidentialDesignTerraces { get { if (_residentialDesignTerraces == null) LoadResidentialDesignTerraces(); return _residentialDesignTerraces; } }
		private static List<ResidentialDesignJustWindowTerrace> _residentialDesignJustWindowTerraces;
		public static List<ResidentialDesignJustWindowTerrace> ResidentialDesignJustWindowTerraces { get { if (_residentialDesignJustWindowTerraces == null) LoadResidentialDesignJustWindowTerraces(); return _residentialDesignJustWindowTerraces; } }
		private static List<ResidentialDesignRoof> _residentialDesignRoofs;
		public static List<ResidentialDesignRoof> ResidentialDesignRoofs { get { if (_residentialDesignRoofs == null) LoadResidentialDesignRoofs(); return _residentialDesignRoofs; } }
		private static List<GameObject> _residentialDesignDoors;
		public static List<GameObject> ResidentialDesignDoors { get { if (_residentialDesignDoors == null) LoadResidentialDesignDoors(); return _residentialDesignDoors; } }
		private static List<GameObject> _residentialDesignWindows;
		public static List<GameObject> ResidentialDesignWindows { get { if (_residentialDesignWindows == null) LoadResidentialDesignWindows(); return _residentialDesignWindows; } }
		private static List<ResidentialDesignTerraceRoof> _residentialDesignTerraceRoofs;
		public static List<ResidentialDesignTerraceRoof> ResidentialDesignTerraceRoofs { get { if (_residentialDesignTerraceRoofs == null) LoadResidentialDesignTerraceRoofs(); return _residentialDesignTerraceRoofs; } }
		private static List<GameObject> _residentialDesignRoofFrontWindows;
		public static List<GameObject> ResidentialDesignRoofFrontWindows { get { if (_residentialDesignRoofFrontWindows == null) LoadResidentialDesignRoofFrontWindows(); return _residentialDesignRoofFrontWindows; } }
		#endregion

		public const uint RESIDENTIAL_LEVEL_COUNT_MASK = 0x00000007; // 3 bits

		public static uint GenerateResidential(uint levelCount)
		{
			System.Random rnd = new();
			uint thirtyBits = (uint)rnd.Next(1 << 30);
			uint twoBits = (uint)rnd.Next(1 << 2);
			uint value = (thirtyBits << 2) | twoBits;
			value &= ~RESIDENTIAL_LEVEL_COUNT_MASK;
			value |= (levelCount & RESIDENTIAL_LEVEL_COUNT_MASK);
			return value;
		}

		#region Load and reload ResidentialDesignElements
		private static void ReloadResidentialDesignRules()
		{
			_residentalGenerationRules = null;
			_residentialDesignBases = null;
			_residentialDesignTerraces = null;
			_residentialDesignJustWindowTerraces = null;
			_residentialDesignRoofs = null;
			_residentialDesignDoors = null;
			_residentialDesignWindows = null;
			_residentialDesignTerraceRoofs = null;
			_residentialDesignRoofFrontWindows = null;
		}

		private static void LoadResidentialDesignBases()
		{
			_residentialDesignBases = new List<ResidentialDesignBase>();
			foreach (var item in ResidentialDesignRules["Models"]["Base"])
			{
				_residentialDesignBases.Add(new()
				{
					ForeBigBase = Boolean.Parse(item["ForeBigBase"].ToString()),
					BackBigBase = Boolean.Parse(item["BackBigBase"].ToString()),
					OnlyFloor = Boolean.Parse(item["OnlyFloor"].ToString()),
					OnlyStorey = Boolean.Parse(item["OnlyStorey"].ToString()),

					Prefab = Resources.Load<GameObject>(ResidentialDesignRules["Models"][item["PrefabFolder"].ToString()].ToString() + item["Prefab"].ToString()),

					ForeTerraceAttachPoint = new Vector3(
						Single.Parse(item["ForeTerraceAttachPoint"][0].ToString()),
						Single.Parse(item["ForeTerraceAttachPoint"][1].ToString()),
						Single.Parse(item["ForeTerraceAttachPoint"][2].ToString())),
					BackTerraceAttachPoint = new Vector3(
						Single.Parse(item["BackTerraceAttachPoint"][0].ToString()),
						Single.Parse(item["BackTerraceAttachPoint"][1].ToString()),
						Single.Parse(item["BackTerraceAttachPoint"][2].ToString()))
				});
			}
		}

		private static void LoadResidentialDesignTerraces()
		{
			_residentialDesignTerraces = new List<ResidentialDesignTerrace>();
			foreach (var item in ResidentialDesignRules["Models"]["Terrace"])
			{
				ResidentialDesignTerrace designTerrace = new()
				{
					NeedSmallNextLevel = Boolean.Parse(item["NeedSmallNextLevel"].ToString()),
					NeedBigNextLevel = Boolean.Parse(item["NeedBigNextLevel"].ToString()),
					OnlyFloor = Boolean.Parse(item["OnlyFloor"].ToString()),
					OnlyStorey = Boolean.Parse(item["OnlyStorey"].ToString()),

					Prefab = Resources.Load<GameObject>(ResidentialDesignRules["Models"][item["PrefabFolder"].ToString()].ToString() + item["Prefab"].ToString()),

					WindowAttachPoints = new List<Vector3>(),
					DoorAttachPoint = new Vector3(
						Single.Parse(item["DoorAttachPoint"][0].ToString()),
						Single.Parse(item["DoorAttachPoint"][1].ToString()),
						Single.Parse(item["DoorAttachPoint"][2].ToString()))
				};

				foreach (var windowAttachPoint in item["WindowAttachPoints"])
				{
					designTerrace.WindowAttachPoints.Add(new Vector3(
						Single.Parse(windowAttachPoint[0].ToString()),
						Single.Parse(windowAttachPoint[1].ToString()),
						Single.Parse(windowAttachPoint[2].ToString())));
				}

				_residentialDesignTerraces.Add(designTerrace);
			}
		}

		private static void LoadResidentialDesignJustWindowTerraces()
		{
			_residentialDesignJustWindowTerraces = new List<ResidentialDesignJustWindowTerrace>();
			foreach (var item in ResidentialDesignRules["Models"]["JustWindowTerrace"])
			{
				ResidentialDesignJustWindowTerrace designJustWindowTerrace = new()
				{
					OnlyFloor = Boolean.Parse(item["OnlyFloor"].ToString()),
					OnlyStorey = Boolean.Parse(item["OnlyStorey"].ToString()),
					HasDoor = Boolean.Parse(item["HasDoor"].ToString()),

					Prefab = Resources.Load<GameObject>(ResidentialDesignRules["Models"][item["PrefabFolder"].ToString()].ToString() + item["Prefab"].ToString()),

					WindowAttachPoints = new List<Vector3>()
				};
				foreach (var windowAttachPoint in item["WindowAttachPoints"])
				{
					designJustWindowTerrace.WindowAttachPoints.Add(new Vector3(
						Single.Parse(windowAttachPoint[0].ToString()),
						Single.Parse(windowAttachPoint[1].ToString()),
						Single.Parse(windowAttachPoint[2].ToString())));
				}
				if (designJustWindowTerrace.HasDoor)
				{
					designJustWindowTerrace.DoorAttachPoint = new Vector3(
						Single.Parse(item["DoorAttachPoint"][0].ToString()),
						Single.Parse(item["DoorAttachPoint"][1].ToString()),
						Single.Parse(item["DoorAttachPoint"][2].ToString()));
				}

				_residentialDesignJustWindowTerraces.Add(designJustWindowTerrace);
			}
		}

		private static void LoadResidentialDesignRoofs()
		{
			_residentialDesignRoofs = new List<ResidentialDesignRoof>();
			foreach (var item in ResidentialDesignRules["Models"]["Roof"])
			{
				_residentialDesignRoofs.Add(new()
				{
					ForeBigBase = Boolean.Parse(item["ForeBigBase"].ToString()),
					BackBigBase = Boolean.Parse(item["BackBigBase"].ToString()),

					Prefab = Resources.Load<GameObject>(ResidentialDesignRules["Models"][item["PrefabFolder"].ToString()].ToString() + item["Prefab"].ToString()),

					ForeTerraceAttachPoint = new Vector3(
						Single.Parse(item["ForeTerraceAttachPoint"][0].ToString()),
						Single.Parse(item["ForeTerraceAttachPoint"][1].ToString()),
						Single.Parse(item["ForeTerraceAttachPoint"][2].ToString())),
					BackTerraceAttachPoint = new Vector3(
						Single.Parse(item["BackTerraceAttachPoint"][0].ToString()),
						Single.Parse(item["BackTerraceAttachPoint"][1].ToString()),
						Single.Parse(item["BackTerraceAttachPoint"][2].ToString()))
				});
			}
		}

		private static void LoadResidentialDesignDoors()
		{
			_residentialDesignDoors = new List<GameObject>();
			foreach (var item in ResidentialDesignRules["Models"]["Door"])
			{
				_residentialDesignDoors.Add(Resources.Load<GameObject>(ResidentialDesignRules["Models"][item["PrefabFolder"].ToString()].ToString() + item["Prefab"].ToString()));
			}
		}

		private static void LoadResidentialDesignWindows()
		{
			_residentialDesignWindows = new List<GameObject>();
			foreach (var item in ResidentialDesignRules["Models"]["Window"])
			{
				_residentialDesignWindows.Add(Resources.Load<GameObject>(ResidentialDesignRules["Models"][item["PrefabFolder"].ToString()].ToString() + item["Prefab"].ToString()));
			}
		}

		private static void LoadResidentialDesignTerraceRoofs()
		{
			_residentialDesignTerraceRoofs = new List<ResidentialDesignTerraceRoof>();
			foreach (var item in ResidentialDesignRules["Models"]["TerraceRoof"])
			{
				_residentialDesignTerraceRoofs.Add(new()
				{
					Prefab = Resources.Load<GameObject>(ResidentialDesignRules["Models"][item["PrefabFolder"].ToString()].ToString() + item["Prefab"].ToString()),

					WindowAttachPoint = new Vector3(
						Single.Parse(item["WindowAttachPoint"][0].ToString()),
						Single.Parse(item["WindowAttachPoint"][1].ToString()),
						Single.Parse(item["WindowAttachPoint"][2].ToString()))
				});
			}
		}

		private static void LoadResidentialDesignRoofFrontWindows()
		{
			_residentialDesignRoofFrontWindows = new List<GameObject>();
			foreach (var item in ResidentialDesignRules["Models"]["RoofFrontWindow"])
			{
				_residentialDesignRoofFrontWindows.Add(Resources.Load<GameObject>(ResidentialDesignRules["Models"][item["PrefabFolder"].ToString()].ToString() + item["Prefab"].ToString()));
			}
		}
		#endregion

		public ZoneBuildingLevel Level { get; private set; } = 0;
		public int ResidentLimit { get; private set; }
		private readonly List<Person> _residents = new();

		/// <summary>
		/// Construct a new residential tile
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		/// <param name="rotation">Rotation of the tile</param>
		public ResidentialBuildingTile(int x, int y, uint designID, Rotation rotation) : base(x, y, designID, rotation)
		{

		}

		/// <summary>
		/// Construct a new residential tile
		/// </summary>
		/// <param name="x">X coordinate of the tile</param>
		/// <param name="y">Y coordinate of the tile</param>
		/// <param name="designID">DesignID for the tile</param>
		public ResidentialBuildingTile(int x, int y, uint designID) : base(x, y, designID, GetRandomRotationToLookAtRoadGridElement(x, y))
		{
			
		}

		private static Rotation GetRandomRotationToLookAtRoadGridElement(int x, int y)
		{
			List<(IRoadGridElement roadGridElement, Rotation rotation)> roadGridElements = RoadGridManager.GetRoadGridElementsAroundTile(City.Instance.GetTile(x, y));
			if (roadGridElements.Count == 0) { throw new InvalidOperationException("No road grid elements around tile"); }
			System.Random rnd = new();
			return roadGridElements[rnd.Next(roadGridElements.Count)].rotation;
		}

		public override void FinalizeTile()
		{
			Finalizing();
		}

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Finalizing()</code></para>
		/// <para>Do the actual finalization</para>
		/// </summary>
		protected new void Finalizing()
		{
			base.Finalizing();
			//TODO implement residential residential limit
			ResidentLimit = 10;
		}

		public override TileType GetTileType() { throw new InvalidOperationException(); }

		public void RegisterResidential(RoadGrid roadGrid)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			roadGrid?.AddResidential(this);
		}

		public void UnregisterResidential(RoadGrid roadGrid)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			roadGrid?.RemoveResidential(this);
		}

		public ZoneType GetZoneType()
		{
			return ZoneType.ResidentialZone;
		}

		public void LevelUp()
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			if (Level == ZoneBuildingLevel.ZERO) { return; }
			if (Level == ZoneBuildingLevel.THREE) { return; }
			++Level;
			ResidentLimit += 5;
		}

		public int GetLevelUpCost()
		{
			//TODO implement commercial level up cost
			return 100000;
		}

		public void MoveIn(Person person)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			if (_residents.Count >= ResidentLimit) { throw new InvalidOperationException("The residential is full"); }

			if (Level == ZoneBuildingLevel.ZERO) { Level = ZoneBuildingLevel.ONE; }

			_residents.Add(person);
		}

		//TODO implement electric pole build price
		public override int BuildPrice => 100000;

		//TODO implement electric pole destroy price
		public override int DestroyIncome => 100000;

		public override float Transparency => 1 - (float)(int)Level / 12;

		public void MoveOut(Person person)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			_residents.Remove(person);
		}

		public List<Person> GetResidents()
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			return _residents;
		}

		public int GetResidentsCount()
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			return _residents.Count;
		}

		public (float happiness, float weight) HappinessByBuilding
		{
			get
			{
				float happinessSum = _happinessChangers.Aggregate(0.0f, (acc, item) => acc + item.happiness * item.weight);
				float happinessWeight = _happinessChangers.Aggregate(0.0f, (acc, item) => acc + item.weight);
				return (happinessSum / (happinessWeight == 0 ? 1 : happinessWeight), happinessWeight);
			}
		}

		private readonly List<(IHappyZone happyZone, float happiness, float weight)> _happinessChangers = new();
		public void RegisterHappinessChangerTile(IHappyZone happyZone)
		{
			happyZone.GetTile().OnTileDelete += UnregisterHappinessChangerTile;
			happyZone.GetTile().OnTileChange += UpdateHappiness;

			(float happiness, float weight) = happyZone.GetHappinessModifierAtTile(this);
			_happinessChangers.Add((happyZone, happiness, weight));
		}

		private void UnregisterHappinessChangerTile(object sender, Tile deletedTile)
		{
			IHappyZone happyZone = (IHappyZone)deletedTile;
			_happinessChangers.RemoveAll((values) => values.happyZone == happyZone);
		}

		private void UpdateHappiness(object sender, Tile changedTile)
		{
			IHappyZone happyZone = (IHappyZone)changedTile;
			_happinessChangers.RemoveAll((values) => values.happyZone == happyZone);

			(float happiness, float weight) = happyZone.GetHappinessModifierAtTile(this);
			_happinessChangers.Add((happyZone, happiness, weight));
		}
	}
}