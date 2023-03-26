using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ResidentialDesignDisplay : MonoBehaviour
{
    private bool _needRedisplay = true;

    void Update()
    {
        if (_needRedisplay)
        {
            Display();
            _needRedisplay = false;
        }
    }

    //2^32-1        / MAXELEMENTID / MAXLEVELCOUNT = MAXELEMENTCOUNT
    //4 294 967 295 / 211           / 8            = 2 544 411
    private const uint COLORMULTIPLIER = 101;
    private const uint ELEMENTMULTIPLIER = 211;

    private const uint BASEELEMENTID = 0;
    private const uint TERRACEELEMENTID = 1;
    private const uint JUSTWINDOWTERRACEELEMENTID = 2;
    private const uint ROOFELEMENTID = 3;
    private const uint DOORELEMENTID = 4;
    private const uint WINDOWELEMENTID = 5;
    private const uint TERRACEROOFELEMENTID = 6;
    private const uint ROOFFRONTWINDOWELEMENTID = 7;

    private uint _seed = 0;
    public uint DesignID {
        get { return _seed; }
        set { _seed = value; _needRedisplay = true; }
    }
    public uint Seed {
        get { return (_seed & ~(ResidentialDesignGenerator.RESIDENTIAL_LEVEL_COUNT_MASK)) / (ResidentialDesignGenerator.RESIDENTIAL_LEVEL_COUNT_MASK + 1); }
    }
    public uint LevelCount {
        get { return _seed & ResidentialDesignGenerator.RESIDENTIAL_LEVEL_COUNT_MASK; }
    }

    #region Gradients and materials
    private static Gradient _houseBottom;
    public static Gradient HouseBottom { get { if (_houseBottom == null) _houseBottom = LoadGradientByName("HouseBottom"); return _houseBottom; } }
    private static Material _houseBottomMaterial;
    public static Material HouseBottomMaterial { get { if (_houseBottomMaterial == null) _houseBottomMaterial = LoadMaterialByName("HouseBottomMaterial"); return _houseBottomMaterial; } }
    private Material _sharedHouseBottomMaterial;
    private Material SharedHouseBottomMaterial {
        get {
            if (_sharedHouseBottomMaterial == null) {
                _sharedHouseBottomMaterial = new Material(HouseBottomMaterial);
                _sharedHouseBottomMaterial.color = HouseBottom.Evaluate(GetColor(0));
            }
            return _sharedHouseBottomMaterial;
        }
    }

    private static Gradient _houseColor;
    public static Gradient HouseColor { get { if (_houseColor == null) _houseColor = LoadGradientByName("HouseColor"); return _houseColor; } }
    private static Material _houseColorMaterial;
    public static Material HouseColorMaterial { get { if (_houseColorMaterial == null) _houseColorMaterial = LoadMaterialByName("HouseColorMaterial"); return _houseColorMaterial; } }
    private Material _sharedHouseColorMaterial;
    private Material SharedHouseColorMaterial {
        get {
            if (_sharedHouseColorMaterial == null) {
                _sharedHouseColorMaterial = new Material(HouseColorMaterial);
                _sharedHouseColorMaterial.color = HouseColor.Evaluate(GetColor(1));
            }
            return _sharedHouseColorMaterial;
        }
    }

    private static Material _woodPillarMaterial;
    public static Material WoodPillarMaterial { get { if (_woodPillarMaterial == null) _woodPillarMaterial = LoadMaterialByName("WoodPillarMaterial"); return _woodPillarMaterial; } }
    private Material _sharedWoodPillarMaterial;
    private Material SharedWoodPillarMaterial {
        get {
            if (_sharedWoodPillarMaterial == null) {
                _sharedWoodPillarMaterial = new Material(WoodPillarMaterial);
            }
            return _sharedWoodPillarMaterial;
        }
    }

    private static Gradient _roof;
    public static Gradient Roof { get { if (_roof == null) _roof = LoadGradientByName("Roof"); return _roof; } }
    private static Material _roofMaterial;
    public static Material RoofMaterial { get { if (_roofMaterial == null) _roofMaterial = LoadMaterialByName("RoofMaterial"); return _roofMaterial; } }
    private Material _sharedRoofMaterial;
    private Material SharedRoofMaterial {
        get {
            if (_sharedRoofMaterial == null) {
                _sharedRoofMaterial = new Material(RoofMaterial);
                _sharedRoofMaterial.color = Roof.Evaluate(GetColor(2));
            }
            return _sharedRoofMaterial;
        }
    }

    private static Gradient _door;
    public static Gradient Door { get { if (_door == null) _door = LoadGradientByName("Door"); return _door; } }
    private static Material _doorMaterial;
    public static Material DoorMaterial { get { if (_doorMaterial == null) _doorMaterial = LoadMaterialByName("DoorMaterial"); return _doorMaterial; } }
    private Material _sharedDoorMaterial;
    private Material SharedDoorMaterial {
        get {
            if (_sharedDoorMaterial == null) {
                _sharedDoorMaterial = new Material(DoorMaterial);
                _sharedDoorMaterial.color = Door.Evaluate(GetColor(3));
            }
            return _sharedDoorMaterial;
        }
    }

    private static Gradient _window;
    public static Gradient Window { get { if (_window == null) _window = LoadGradientByName("Window"); return _window; } }
    private static Material _windowMaterial;
    public static Material WindowMaterial { get { if (_windowMaterial == null) _windowMaterial = LoadMaterialByName("WindowMaterial"); return _windowMaterial; } }
    private Material _sharedWindowMaterial;
    private Material SharedWindowMaterial {
        get {
            if (_sharedWindowMaterial == null) {
                _sharedWindowMaterial = new Material(WindowMaterial);
                _sharedWindowMaterial.color = Window.Evaluate(GetColor(4));
            }
            return _sharedWindowMaterial;
        }
    }

    private static Material _windowFrameMaterial;
    public static Material WindowFrameMaterial { get { if (_windowFrameMaterial == null) _windowFrameMaterial = LoadMaterialByName("WindowFrameMaterial"); return _windowFrameMaterial; } }
    private Material _sharedWindowFrameMaterial;
    private Material SharedWindowFrameMaterial {
        get {
            if (_sharedWindowFrameMaterial == null) {
                _sharedWindowFrameMaterial = new Material(WindowFrameMaterial);
            }
            return _sharedWindowFrameMaterial;
        }
    }
    #endregion

    #region Display residential
    private void SetSharedMaterials(Renderer renderer)
    {
        if (renderer == null) return;

        Material[] materials = renderer.sharedMaterials;

        for (int i = 0; i < materials.Length; i++)
        {
            if      (materials[i].name.Split(' ')[0] == "HouseBottom") { materials[i] = SharedHouseBottomMaterial; }
            else if (materials[i].name.Split(' ')[0] == "HouseColor")  { materials[i] = SharedHouseColorMaterial; }
            else if (materials[i].name.Split(' ')[0] == "WoodPillar")  { materials[i] = SharedWoodPillarMaterial; }
            else if (materials[i].name.Split(' ')[0] == "Roof")        { materials[i] = SharedRoofMaterial; }
            else if (materials[i].name.Split(' ')[0] == "Door")        { materials[i] = SharedDoorMaterial; }
            else if (materials[i].name.Split(' ')[0] == "Window")      { materials[i] = SharedWindowMaterial; }
            else if (materials[i].name.Split(' ')[0] == "WindowFrame") { materials[i] = SharedWindowFrameMaterial; }
            else
            {
                Debug.LogError("Unknown material found: " + renderer.materials[i].name);
            }
        }

        renderer.sharedMaterials = materials;
    }

    private void Display()
    {
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }

        bool needForeSmallNextLevel = false;
        bool needForeBigNextLevel = false;
        bool needBackSmallNextLevel = false;
        bool needBackBigNextLevel = false;

        bool bottomNeedForeSmallNextLevel = false;
        bool bottomNeedForeBigNextLevel = false;
        bool bottomNeedBackSmallNextLevel = false;
        bool bottomNeedBackBigNextLevel = false;

        Vector3 attachPoint = new Vector3(0, 0, 0);

        for (uint i = 0; i < LevelCount; i++)
        {
            DisplayLevelElement(bottomNeedForeSmallNextLevel, bottomNeedBackSmallNextLevel, i, attachPoint, gameObject, ref needForeSmallNextLevel, ref needForeBigNextLevel, ref needBackSmallNextLevel, ref needBackBigNextLevel);

            bottomNeedForeSmallNextLevel = needForeSmallNextLevel;
            bottomNeedForeBigNextLevel = needForeBigNextLevel;
            bottomNeedBackSmallNextLevel = needBackSmallNextLevel;
            bottomNeedBackBigNextLevel = needBackBigNextLevel;

            needForeBigNextLevel = false;
            needBackBigNextLevel = false;

            attachPoint += new Vector3(0, 7, 0);
        }
        
        DisplayRoof(LevelCount, attachPoint, gameObject, bottomNeedForeSmallNextLevel, bottomNeedBackSmallNextLevel, bottomNeedForeBigNextLevel, bottomNeedBackBigNextLevel);
    }

    private void DisplayLevelElement(bool bottomNeedForeSmallNextLevel, bool bottomNeedBackSmallNextLevel, uint levelIndex, Vector3 attachPoint, GameObject core, ref bool needForeSmallNextLevel, ref bool needForeBigNextLevel, ref bool needBackSmallNextLevel, ref bool needBackBigNextLevel)
    {
        List<ResidentialDesignBase> availableBase = ResidentialDesignGenerator.Instance.ResidentialDesignBases.FindAll(x => (x.OnlyFloor ? levelIndex == 0 : true) && (x.OnlyStorey ? levelIndex > 0 : true) &&
                                                                                                                            (bottomNeedForeSmallNextLevel ? !x.ForeBigBase : true) && 
                                                                                                                            (bottomNeedBackSmallNextLevel ? !x.BackBigBase : true));
        ResidentialDesignBase selectedBase = availableBase[GetElement(BASEELEMENTID, levelIndex, availableBase.Count)];

        GameObject baseElement = Instantiate(selectedBase.Prefab, attachPoint, core.transform.rotation);
        baseElement.transform.parent = core.transform;
        baseElement.transform.localRotation = Quaternion.Euler(-90, 0, 0);

        needForeBigNextLevel = (needForeBigNextLevel ? true : selectedBase.ForeBigBase);
        needBackBigNextLevel = (needBackBigNextLevel ? true : selectedBase.BackBigBase);

        SetSharedMaterials(baseElement.GetComponent<Renderer>());

        DisplayTerraceElement(selectedBase.ForeBigBase, bottomNeedForeSmallNextLevel, levelIndex, attachPoint + selectedBase.ForeTerraceAttachPoint, true, baseElement, ref needForeSmallNextLevel, ref needForeBigNextLevel);
        DisplayTerraceElement(selectedBase.BackBigBase, bottomNeedBackSmallNextLevel, levelIndex, attachPoint + selectedBase.BackTerraceAttachPoint, false, baseElement, ref needBackSmallNextLevel, ref needBackBigNextLevel);
    }

    private void DisplayTerraceElement(bool bigBase, bool bottomNeedSmallNextLevel, uint levelIndex, Vector3 attachPoint, bool fore, GameObject parentElement, ref bool needSmallNextLevel, ref bool needBigNextLevel)
    {
        if (bigBase || bottomNeedSmallNextLevel)
        {
            DisplayJustWindowTerrace(levelIndex, attachPoint, fore, parentElement);
            return;
        }

        List<ResidentialDesignTerrace> availableTerrace = ResidentialDesignGenerator.Instance.ResidentialDesignTerraces.FindAll(x => (x.OnlyFloor ? levelIndex == 0 : true) && (x.OnlyStorey ? levelIndex > 0 : true));
        ResidentialDesignTerrace selectedTerrace = availableTerrace[GetElement(TERRACEELEMENTID, levelIndex, availableTerrace.Count)];

        GameObject terrace = Instantiate(selectedTerrace.Prefab, attachPoint, parentElement.transform.rotation);
        terrace.transform.parent = parentElement.transform;
        terrace.transform.localRotation = Quaternion.Euler(0, 0, (fore ? 180 : 0));

        needSmallNextLevel = (needSmallNextLevel ? true : selectedTerrace.NeedSmallNextLevel);
        needBigNextLevel = (needBigNextLevel ? true : selectedTerrace.NeedBigNextLevel);

        SetSharedMaterials(terrace.GetComponent<Renderer>());

        DisplayDoor(levelIndex, attachPoint + selectedTerrace.DoorAttachPoint, terrace);

        GameObject usedWindow = null;
        for (int i = 0; i < selectedTerrace.WindowAttachPoints.Count; i++)
        {
            usedWindow = DisplayWindow(levelIndex, attachPoint + selectedTerrace.WindowAttachPoints[i], terrace, usedWindow);
        }
    }

    private void DisplayJustWindowTerrace(uint levelIndex, Vector3 attachPoint, bool fore, GameObject parentElement)
    {
        List<ResidentialDesignJustWindowTerrace> availableTerrace = ResidentialDesignGenerator.Instance.ResidentialDesignJustWindowTerraces.FindAll(x => (x.OnlyFloor ? levelIndex == 0 : true) && (x.OnlyStorey ? levelIndex > 0 : true));
        ResidentialDesignJustWindowTerrace selectedTerrace = availableTerrace[GetElement(JUSTWINDOWTERRACEELEMENTID, levelIndex, availableTerrace.Count)];

        GameObject justWindowTerrace = Instantiate(selectedTerrace.Prefab, attachPoint, parentElement.transform.rotation);
        justWindowTerrace.transform.parent = parentElement.transform;
        justWindowTerrace.transform.localRotation = Quaternion.Euler(0, 0, (fore ? 180 : 0));

        SetSharedMaterials(justWindowTerrace.GetComponent<Renderer>());

        if (selectedTerrace.HasDoor) DisplayDoor(levelIndex, attachPoint + selectedTerrace.DoorAttachPoint, justWindowTerrace);


        GameObject usedWindow = null;
        for (int i = 0; i < selectedTerrace.WindowAttachPoints.Count; i++)
        {
            usedWindow = DisplayWindow(levelIndex, attachPoint + selectedTerrace.WindowAttachPoints[i], justWindowTerrace, usedWindow);
        }
    }

    private void DisplayDoor(uint levelIndex, Vector3 attachPoint, GameObject parentElement)
    {
        GameObject selectedDoor = ResidentialDesignGenerator.Instance.ResidentialDesignDoors[GetElement(DOORELEMENTID, levelIndex, ResidentialDesignGenerator.Instance.ResidentialDesignDoors.Count)];

        GameObject door = Instantiate(selectedDoor, attachPoint, parentElement.transform.rotation);
        door.transform.parent = parentElement.transform;

        SetSharedMaterials(door.GetComponent<Renderer>());
    }

    private GameObject DisplayWindow(uint levelIndex, Vector3 attachPoint, GameObject parentElement, GameObject preSelectedWindow = null)
    {
        GameObject selectedWindow = (preSelectedWindow != null ? preSelectedWindow : ResidentialDesignGenerator.Instance.ResidentialDesignWindows[GetElement(WINDOWELEMENTID, levelIndex, ResidentialDesignGenerator.Instance.ResidentialDesignWindows.Count)]);

        GameObject window = Instantiate(selectedWindow, attachPoint, parentElement.transform.rotation);
        window.transform.parent = parentElement.transform;

        SetSharedMaterials(window.GetComponent<Renderer>());

        return selectedWindow;
    }

    private void DisplayRoof(uint levelIndex, Vector3 attachPoint, GameObject parentElement, bool bottomNeedForeSmallNextLevel, bool bottomNeedBackSmallNextLevel, bool bottomNeedForeBigNextLevel, bool bottomNeedBackBigNextLevel)
    {
        List<ResidentialDesignRoof> availableRoof = ResidentialDesignGenerator.Instance.ResidentialDesignRoofs.FindAll(x => (bottomNeedForeSmallNextLevel ? !x.ForeBigBase : true) && (bottomNeedBackSmallNextLevel ? !x.BackBigBase : true) && 
                                                                                                                            (bottomNeedForeBigNextLevel ? x.ForeBigBase : true) && (bottomNeedBackBigNextLevel ? x.BackBigBase : true));
        ResidentialDesignRoof selectedRoof = availableRoof[GetElement(ROOFELEMENTID, levelIndex, availableRoof.Count)];

        GameObject roof = Instantiate(selectedRoof.Prefab, attachPoint, parentElement.transform.rotation);
        roof.transform.parent = parentElement.transform;
        roof.transform.localRotation = Quaternion.Euler(-90, 0, 0);

        SetSharedMaterials(roof.GetComponent<Renderer>());

        DisplayTerraceRoof(levelIndex, attachPoint + selectedRoof.ForeTerraceAttachPoint, true, roof);
        DisplayTerraceRoof(levelIndex, attachPoint + selectedRoof.BackTerraceAttachPoint, false, roof);
    }

    private void DisplayTerraceRoof(uint levelIndex, Vector3 attachPoint, bool fore, GameObject parentElement)
    {
        ResidentialDesignTerraceRoof selectedTerraceRoof = ResidentialDesignGenerator.Instance.ResidentialDesignTerraceRoofs[GetElement(TERRACEROOFELEMENTID, levelIndex, ResidentialDesignGenerator.Instance.ResidentialDesignTerraceRoofs.Count)];

        GameObject terraceRoof = Instantiate(selectedTerraceRoof.Prefab, attachPoint, parentElement.transform.rotation);
        terraceRoof.transform.parent = parentElement.transform;
        terraceRoof.transform.localRotation = Quaternion.Euler(0, 0, (fore ? 180 : 0));

        SetSharedMaterials(terraceRoof.GetComponent<Renderer>());

        DisplayRoofFrontWindow(levelIndex, attachPoint + selectedTerraceRoof.WindowAttachPoint, terraceRoof);        
    }

    private void DisplayRoofFrontWindow(uint levelIndex, Vector3 attachPoint, GameObject parentElement)
    {
        GameObject selectedWindow = ResidentialDesignGenerator.Instance.ResidentialDesignRoofFrontWindows[GetElement(ROOFFRONTWINDOWELEMENTID, levelIndex, ResidentialDesignGenerator.Instance.ResidentialDesignRoofFrontWindows.Count)];

        GameObject window = Instantiate(selectedWindow, attachPoint, parentElement.transform.rotation);
        window.transform.parent = parentElement.transform;

        SetSharedMaterials(window.GetComponent<Renderer>());
    }
    #endregion

    #region GetValuesBySeed
    private float GetColor(int colorID)
    {
        return (Mathf.Clamp(Mathf.PerlinNoise((float)((float)Seed/10), (float)((float)(colorID + 1) * COLORMULTIPLIER / 10)), 0, 1) * 1000) % 1;
    }

    private int GetElement(uint elementID, uint levelID, int elementCount)
    {
        return (int)(Mathf.Clamp(Mathf.PerlinNoise((float)((float)Seed/10), (float)((float)(elementID + 1) * (levelID + 1) * ELEMENTMULTIPLIER / 10)), 0 , 1) * 1000) % elementCount;
    }
    #endregion

    #region Load and reload materials and gradients
    public void ReloadMaterialsAndGradients()
    {
        _houseBottom = null;
        _houseBottomMaterial = null;
        _sharedHouseBottomMaterial = null;

        _houseColor = null;
        _houseColorMaterial = null;
        _sharedHouseColorMaterial = null;

        _woodPillarMaterial = null;
        _sharedWoodPillarMaterial = null;

        _roof = null;
        _roofMaterial = null;
        _sharedRoofMaterial = null;

        _door = null;
        _doorMaterial = null;
        _sharedDoorMaterial = null;

        _window = null;
        _windowMaterial = null;
        _sharedWindowMaterial = null;

        _windowFrameMaterial = null;
        _sharedWindowFrameMaterial = null;

        _needRedisplay = true;
    }

    private static Material LoadMaterialByName(string name)
    {
        return Resources.Load<Material>("HouseGenerator/Materials/" + name);
    }

    private static Gradient LoadGradientByName(string name)
    {
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[((JArray)ResidentialDesignGenerator.Instance.ResidentialDesignRules["Colors"][name]).Count];
        for (int i = 0; i < colorKeys.Length; i ++)
        {
            colorKeys[i].time = (float)ResidentialDesignGenerator.Instance.ResidentialDesignRules["Colors"][name][i][0];
            colorKeys[i].color = new Color(
                (float)ResidentialDesignGenerator.Instance.ResidentialDesignRules["Colors"][name][i][1]/256,
                (float)ResidentialDesignGenerator.Instance.ResidentialDesignRules["Colors"][name][i][2]/256,
                (float)ResidentialDesignGenerator.Instance.ResidentialDesignRules["Colors"][name][i][3]/256
            );
        }

        GradientAlphaKey[] generalAlphaKeys = new GradientAlphaKey[2];
        generalAlphaKeys[0].time = 0.0f;
        generalAlphaKeys[0].alpha = 1.0f;
        generalAlphaKeys[1].time = 1.0f;
        generalAlphaKeys[1].alpha = 1.0f;

        gradient.SetKeys(colorKeys, generalAlphaKeys);

        return gradient;
    }
    #endregion
}
