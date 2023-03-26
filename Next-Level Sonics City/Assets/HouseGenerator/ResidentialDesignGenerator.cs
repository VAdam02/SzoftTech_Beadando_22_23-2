using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

#region HouseElements
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

public class ResidentialDesignGenerator
{
    private static ResidentialDesignGenerator _instance;
    public  static ResidentialDesignGenerator Instance { get { if (_instance == null) _instance = new ResidentialDesignGenerator(); return _instance; } }

    private JObject _residentalGenerationRules;
    public  JObject ResidentialDesignRules { get { if (_residentalGenerationRules == null) _residentalGenerationRules = JObject.Parse(Resources.Load<TextAsset>("HouseGenerator/ResidentialDesignRules").text); return _residentalGenerationRules; } }

    private List<ResidentialDesignBase> _residentialDesignBases;
    public  List<ResidentialDesignBase> ResidentialDesignBases { get { if (_residentialDesignBases == null) LoadResidentialDesignBases(); return _residentialDesignBases; } }
    private List<ResidentialDesignTerrace> _residentialDesignTerraces;
    public  List<ResidentialDesignTerrace> ResidentialDesignTerraces { get { if (_residentialDesignTerraces == null) LoadResidentialDesignTerraces(); return _residentialDesignTerraces; } }
    private List<ResidentialDesignJustWindowTerrace> _residentialDesignJustWindowTerraces;
    public  List<ResidentialDesignJustWindowTerrace> ResidentialDesignJustWindowTerraces { get { if (_residentialDesignJustWindowTerraces == null) LoadResidentialDesignJustWindowTerraces(); return _residentialDesignJustWindowTerraces; } }
    private List<ResidentialDesignRoof> _residentialDesignRoofs;
    public  List<ResidentialDesignRoof> ResidentialDesignRoofs { get { if (_residentialDesignRoofs == null) LoadResidentialDesignRoofs(); return _residentialDesignRoofs; } }
    private List<GameObject> _residentialDesignDoors;
    public  List<GameObject> ResidentialDesignDoors { get { if (_residentialDesignDoors == null) LoadResidentialDesignDoors(); return _residentialDesignDoors; } }
    private List<GameObject> _residentialDesignWindows;
    public  List<GameObject> ResidentialDesignWindows { get { if (_residentialDesignWindows == null) LoadResidentialDesignWindows(); return _residentialDesignWindows; } }
    private List<ResidentialDesignTerraceRoof> _residentialDesignTerraceRoofs;
    public  List<ResidentialDesignTerraceRoof> ResidentialDesignTerraceRoofs { get { if (_residentialDesignTerraceRoofs == null) LoadResidentialDesignTerraceRoofs(); return _residentialDesignTerraceRoofs; } }
    private List<GameObject> _residentialDesignRoofFrontWindows;
    public  List<GameObject> ResidentialDesignRoofFrontWindows { get { if (_residentialDesignRoofFrontWindows == null) LoadResidentialDesignRoofFrontWindows(); return _residentialDesignRoofFrontWindows; } }

    private ResidentialDesignGenerator() { }

    public const uint RESIDENTIAL_LEVEL_COUNT_MASK = 0x00000007; // 3 bits

    public uint GenerateResidential(uint levelCount)
    {
        System.Random rnd = new System.Random();
        uint thirtyBits = (uint) rnd.Next(1 << 30);
        uint twoBits = (uint) rnd.Next(1 << 2);
        uint value = (thirtyBits << 2) | twoBits;
        value = value & ~RESIDENTIAL_LEVEL_COUNT_MASK;
        value = value | (levelCount & RESIDENTIAL_LEVEL_COUNT_MASK);
        return value;
    }

    #region Load and reload ResidentialDesignRules
    private void ReloadResidentialDesignRules()
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

    private void LoadResidentialDesignBases()
    {
        _residentialDesignBases = new List<ResidentialDesignBase>();
        foreach (var item in ResidentialDesignRules["Models"]["Base"])
        {
            ResidentialDesignBase designBase = new ResidentialDesignBase();

            designBase.ForeBigBase = Boolean.Parse(item["ForeBigBase"].ToString());
            designBase.BackBigBase = Boolean.Parse(item["BackBigBase"].ToString());
            designBase.OnlyFloor   = Boolean.Parse(item["OnlyFloor"].ToString());
            designBase.OnlyStorey  = Boolean.Parse(item["OnlyStorey"].ToString());

            designBase.Prefab = Resources.Load<GameObject>(ResidentialDesignRules["Models"][item["PrefabFolder"].ToString()].ToString() + item["Prefab"].ToString());

            designBase.ForeTerraceAttachPoint = new Vector3(
                Single.Parse(item["ForeTerraceAttachPoint"][0].ToString()), 
                Single.Parse(item["ForeTerraceAttachPoint"][1].ToString()), 
                Single.Parse(item["ForeTerraceAttachPoint"][2].ToString()));
            designBase.BackTerraceAttachPoint = new Vector3(
                Single.Parse(item["BackTerraceAttachPoint"][0].ToString()), 
                Single.Parse(item["BackTerraceAttachPoint"][1].ToString()), 
                Single.Parse(item["BackTerraceAttachPoint"][2].ToString()));

            _residentialDesignBases.Add(designBase);
        }
    }

    private void LoadResidentialDesignTerraces()
    {
        _residentialDesignTerraces = new List<ResidentialDesignTerrace>();
        foreach (var item in ResidentialDesignRules["Models"]["Terrace"])
        {
            ResidentialDesignTerrace designTerrace = new ResidentialDesignTerrace();

            designTerrace.NeedSmallNextLevel = Boolean.Parse(item["NeedSmallNextLevel"].ToString());
            designTerrace.NeedBigNextLevel = Boolean.Parse(item["NeedBigNextLevel"].ToString());
            designTerrace.OnlyFloor = Boolean.Parse(item["OnlyFloor"].ToString());
            designTerrace.OnlyStorey = Boolean.Parse(item["OnlyStorey"].ToString());

            designTerrace.Prefab = Resources.Load<GameObject>(ResidentialDesignRules["Models"][item["PrefabFolder"].ToString()].ToString() + item["Prefab"].ToString());

            designTerrace.WindowAttachPoints = new List<Vector3>();
            foreach (var windowAttachPoint in item["WindowAttachPoints"])
            {
                designTerrace.WindowAttachPoints.Add(new Vector3(
                    Single.Parse(windowAttachPoint[0].ToString()), 
                    Single.Parse(windowAttachPoint[1].ToString()), 
                    Single.Parse(windowAttachPoint[2].ToString())));
            }
            designTerrace.DoorAttachPoint = new Vector3(
                Single.Parse(item["DoorAttachPoint"][0].ToString()), 
                Single.Parse(item["DoorAttachPoint"][1].ToString()), 
                Single.Parse(item["DoorAttachPoint"][2].ToString()));

            _residentialDesignTerraces.Add(designTerrace);
        }
    }

    private void LoadResidentialDesignJustWindowTerraces()
    {
        _residentialDesignJustWindowTerraces = new List<ResidentialDesignJustWindowTerrace>();
        foreach (var item in ResidentialDesignRules["Models"]["JustWindowTerrace"])
        {
            ResidentialDesignJustWindowTerrace designJustWindowTerrace = new ResidentialDesignJustWindowTerrace();

            designJustWindowTerrace.OnlyFloor = Boolean.Parse(item["OnlyFloor"].ToString());
            designJustWindowTerrace.OnlyStorey = Boolean.Parse(item["OnlyStorey"].ToString());
            designJustWindowTerrace.HasDoor = Boolean.Parse(item["HasDoor"].ToString());

            designJustWindowTerrace.Prefab = Resources.Load<GameObject>(ResidentialDesignRules["Models"][item["PrefabFolder"].ToString()].ToString() + item["Prefab"].ToString());

            designJustWindowTerrace.WindowAttachPoints = new List<Vector3>();
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

    private void LoadResidentialDesignRoofs()
    {
        _residentialDesignRoofs = new List<ResidentialDesignRoof>();
        foreach (var item in ResidentialDesignRules["Models"]["Roof"])
        {
            ResidentialDesignRoof designRoof = new ResidentialDesignRoof();

            designRoof.ForeBigBase = Boolean.Parse(item["ForeBigBase"].ToString());
            designRoof.BackBigBase = Boolean.Parse(item["BackBigBase"].ToString());

            designRoof.Prefab = Resources.Load<GameObject>(ResidentialDesignRules["Models"][item["PrefabFolder"].ToString()].ToString() + item["Prefab"].ToString());

            designRoof.ForeTerraceAttachPoint = new Vector3(
                Single.Parse(item["ForeTerraceAttachPoint"][0].ToString()), 
                Single.Parse(item["ForeTerraceAttachPoint"][1].ToString()), 
                Single.Parse(item["ForeTerraceAttachPoint"][2].ToString()));
            designRoof.BackTerraceAttachPoint = new Vector3(
                Single.Parse(item["BackTerraceAttachPoint"][0].ToString()), 
                Single.Parse(item["BackTerraceAttachPoint"][1].ToString()), 
                Single.Parse(item["BackTerraceAttachPoint"][2].ToString()));

            _residentialDesignRoofs.Add(designRoof);
        }
    }

    private void LoadResidentialDesignDoors()
    {
        _residentialDesignDoors = new List<GameObject>();
        foreach (var item in ResidentialDesignRules["Models"]["Door"])
        {
            GameObject designDoor = Resources.Load<GameObject>(ResidentialDesignRules["Models"][item["PrefabFolder"].ToString()].ToString() + item["Prefab"].ToString());

            _residentialDesignDoors.Add(designDoor);
        }
    }

    private void LoadResidentialDesignWindows()
    {
        _residentialDesignWindows = new List<GameObject>();
        foreach (var item in ResidentialDesignRules["Models"]["Window"])
        {
            GameObject designWindow = Resources.Load<GameObject>(ResidentialDesignRules["Models"][item["PrefabFolder"].ToString()].ToString() + item["Prefab"].ToString());

            designWindow = Resources.Load<GameObject>(ResidentialDesignRules["Models"][item["PrefabFolder"].ToString()].ToString() + item["Prefab"].ToString());

            _residentialDesignWindows.Add(designWindow);
        }
    }

    private void LoadResidentialDesignTerraceRoofs()
    {
        _residentialDesignTerraceRoofs = new List<ResidentialDesignTerraceRoof>();
        foreach (var item in ResidentialDesignRules["Models"]["TerraceRoof"])
        {
            ResidentialDesignTerraceRoof designTerraceRoof = new ResidentialDesignTerraceRoof();

            designTerraceRoof.Prefab = Resources.Load<GameObject>(ResidentialDesignRules["Models"][item["PrefabFolder"].ToString()].ToString() + item["Prefab"].ToString());

            designTerraceRoof.WindowAttachPoint = new Vector3(
                Single.Parse(item["WindowAttachPoint"][0].ToString()), 
                Single.Parse(item["WindowAttachPoint"][1].ToString()), 
                Single.Parse(item["WindowAttachPoint"][2].ToString()));

            _residentialDesignTerraceRoofs.Add(designTerraceRoof);
        }
    }

    private void LoadResidentialDesignRoofFrontWindows()
    {
        _residentialDesignRoofFrontWindows = new List<GameObject>();
        foreach (var item in ResidentialDesignRules["Models"]["RoofFrontWindow"])
        {
            GameObject designTerraceRoofWindow = Resources.Load<GameObject>(ResidentialDesignRules["Models"][item["PrefabFolder"].ToString()].ToString() + item["Prefab"].ToString());

            designTerraceRoofWindow = Resources.Load<GameObject>(ResidentialDesignRules["Models"][item["PrefabFolder"].ToString()].ToString() + item["Prefab"].ToString());

            _residentialDesignRoofFrontWindows.Add(designTerraceRoofWindow);
        }
    }
    #endregion
}
