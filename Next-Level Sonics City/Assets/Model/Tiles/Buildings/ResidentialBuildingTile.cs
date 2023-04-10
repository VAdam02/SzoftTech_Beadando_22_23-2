using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Tiles.Buildings
{
	public class ResidentialBuildingTile : Building, IZoneBuilding
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

		public ZoneBuildingLevel Level { get; private set; }
		public int ResidentLimit { get; private set; }
		private List<Person> _residents;

		public ResidentialBuildingTile(int x, int y, uint designID) : base(x, y, designID)
		{
			Level = 0;
			ResidentLimit = 5;
			_residents = new List<Person>();
		}

		public void LevelUp()
		{
			if (Level == ZoneBuildingLevel.THREE) { return; }
			//TODO level up design ID too
			++Level;
			ResidentLimit += 5;
		}

		public bool MoveIn(Person person)
		{
			if (_residents.Count < ResidentLimit)
			{
				_residents.Add(person);
				return true;
			}

			return false;
		}

		public bool MoveOut(Person person)
		{
			return _residents.Remove(person);
		}

		public List<Person> GetResidents()
		{
			return _residents;
		}
	}
}