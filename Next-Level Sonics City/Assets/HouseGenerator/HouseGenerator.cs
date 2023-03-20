using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HouseBase
{
    public GameObject Prefab;

    public Vector3 FrontTerraceAttachPoint;
    public Vector3 BackTerraceAttachPoint;

    public bool ForeBigBase;
    public bool BackBigBase;

    public bool OnlyFloor;
    public bool OnlyStorey;
}

[System.Serializable]
public struct HouseTerrace
{
    public GameObject Prefab;

    public bool NeedSmallNextLevel;
    public bool NeedBigNextLevel;

    public bool OnlyFloor;
    public bool OnlyStorey;

    public List<Vector3> WindowAttachPoints;
    public Vector3 DoorAttachPoint;
}

[System.Serializable]
public struct HouseRoof
{
    public GameObject Prefab;

    public Vector3 ForeTerraceAttachPoint;
    public Vector3 BackTerraceAttachPoint;

    public bool ForeBigBase;
    public bool BackBigBase;
}

[System.Serializable]
public struct HouseTerraceRoof
{
    public GameObject Prefab;

    public Vector3 WindowAttachPoint;
}

public class HouseGenerator : MonoBehaviour
{
    public List<HouseBase> Base;
    public HouseTerrace TerraceNone;
    public List<HouseTerrace> Terrace;
    public List<HouseRoof> Roof;
    public List<GameObject> Door;
    public List<GameObject> Window;
    public List<HouseTerraceRoof> TerraceRoof;
    public List<GameObject> RoofFrontWindow;

    public Gradient HouseBottom;
    public Material HouseBottomMaterial;
    public Gradient HouseColor;
    public Material HouseColorMaterial;
    public Material WoodPillarMaterial;
    public Gradient RoofColor;
    public Material RoofMaterial;
    public Gradient DoorColor;
    public Material DoorMaterial;
    public Gradient WindowColor;
    public Material WindowMaterial;
    public Material WindowFrameMaterial;

    private Material _houseBottomMaterial;
    private Material _houseColorMaterial;
    private Material _woodPillarMaterial;
    private Material _roofMaterial;
    private Material _doorMaterial;
    private Material _windowMaterial;
    private Material _windowFrameMaterial;


    // Start is called before the first frame update
    void Start()
    {
        _houseBottomMaterial = new Material(HouseBottomMaterial);
        _houseBottomMaterial.color = HouseBottom.Evaluate(Random.Range(0f, 1f));
        _houseColorMaterial = new Material(HouseColorMaterial);
        _houseColorMaterial.color = HouseColor.Evaluate(Random.Range(0f, 1f));
        _woodPillarMaterial = new Material(WoodPillarMaterial);
        _roofMaterial = new Material(RoofMaterial);
        _roofMaterial.color = RoofColor.Evaluate(Random.Range(0f, 1f));
        _doorMaterial = new Material(DoorMaterial);
        _doorMaterial.color = DoorColor.Evaluate(Random.Range(0f, 1f));
        _windowMaterial = new Material(WindowMaterial);
        _windowMaterial.color = WindowColor.Evaluate(Random.Range(0f, 1f));
        _windowFrameMaterial = new Material(WindowFrameMaterial);



        GenerateHouse(7, gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetMaterials(Renderer renderer)
    {
        for (int i = 0; i < renderer.materials.Length; i++)
        {
            if (renderer.materials[i].name.Split(' ')[0] == "HouseBottom") { renderer.materials[i].CopyPropertiesFromMaterial(_houseBottomMaterial); }
            else if (renderer.materials[i].name.Split(' ')[0] == "HouseColor") { renderer.materials[i].CopyPropertiesFromMaterial(_houseColorMaterial); }
            else if (renderer.materials[i].name.Split(' ')[0] == "WoodPillar") { renderer.materials[i].CopyPropertiesFromMaterial(_woodPillarMaterial); }
            else if (renderer.materials[i].name.Split(' ')[0] == "Roof") { renderer.materials[i].CopyPropertiesFromMaterial(_roofMaterial); }
            else if (renderer.materials[i].name.Split(' ')[0] == "Door") { renderer.materials[i].CopyPropertiesFromMaterial(_doorMaterial); }
            else if (renderer.materials[i].name.Split(' ')[0] == "Window") { renderer.materials[i].CopyPropertiesFromMaterial(_windowMaterial); }
            else if (renderer.materials[i].name.Split(' ')[0] == "WindowFrame") { renderer.materials[i].CopyPropertiesFromMaterial(_windowFrameMaterial); }
            else
            {
                Debug.LogError("Unknown material found: " + renderer.materials[i].name);
            }
        }
    }

    private void GenerateHouse(int levelCount, GameObject core)
    {
        Vector3 attachPoint = core.transform.position;

        bool needForeSmallNextLevel = false;
        bool needForeBigNextLevel = false;
        bool needBackSmallNextLevel = false;
        bool needBackBigNextLevel = false;

        bool bottomNeedForeSmallNextLevel = false;
        bool bottomNeedForeBigNextLevel = false;
        bool bottomNeedBackSmallNextLevel = false;
        bool bottomNeedBackBigNextLevel = false;

        for (int i = 0; i < levelCount; i++)
        {
            GenerateLevel(bottomNeedForeSmallNextLevel, bottomNeedBackSmallNextLevel, i, attachPoint, core, ref needForeSmallNextLevel, ref needForeBigNextLevel, ref needBackSmallNextLevel, ref needBackBigNextLevel);

            bottomNeedForeSmallNextLevel = needForeSmallNextLevel;
            bottomNeedForeBigNextLevel = needForeBigNextLevel;
            bottomNeedBackSmallNextLevel = needBackSmallNextLevel;
            bottomNeedBackBigNextLevel = needBackBigNextLevel;

            needForeBigNextLevel = false;
            needBackBigNextLevel = false;

            attachPoint += new Vector3(0, 7, 0);
        }

        GenerateRoof(bottomNeedForeSmallNextLevel, bottomNeedBackSmallNextLevel, bottomNeedForeBigNextLevel, bottomNeedBackBigNextLevel, attachPoint, core);
    }

    private void GenerateLevel(bool bottomNeedForeSmallNextLevel, bool bottomNeedBackSmallNextLevel, int levelIndex, Vector3 attachPoint, GameObject core, ref bool needForeSmallNextLevel, ref bool needForeBigNextLevel, ref bool needBackSmallNextLevel, ref bool needBackBigNextLevel)
    {
        List<HouseBase> availableBase = Base.FindAll(x => (x.OnlyFloor ? levelIndex == 0 : true) && (x.OnlyStorey ? levelIndex > 0 : true) &&
                                                              (bottomNeedForeSmallNextLevel ? !x.ForeBigBase : true) && 
                                                              (bottomNeedBackSmallNextLevel ? !x.BackBigBase : true));
        HouseBase selectedBase = availableBase[Random.Range(0, availableBase.Count)];

        GameObject baseElement = Instantiate(selectedBase.Prefab, attachPoint, core.transform.rotation);
        baseElement.transform.parent = core.transform;
        baseElement.transform.localRotation = Quaternion.Euler(-90, 0, 0);

        needForeBigNextLevel = (needForeBigNextLevel ? true : selectedBase.ForeBigBase);
        needBackBigNextLevel = (needBackBigNextLevel ? true : selectedBase.BackBigBase);

        SetMaterials(baseElement.GetComponent<Renderer>());

        GenerateTerrace(selectedBase.ForeBigBase, bottomNeedForeSmallNextLevel, levelIndex, attachPoint + selectedBase.FrontTerraceAttachPoint, true, baseElement, ref needForeSmallNextLevel, ref needForeBigNextLevel);
        GenerateTerrace(selectedBase.BackBigBase, bottomNeedBackSmallNextLevel, levelIndex, attachPoint + selectedBase.BackTerraceAttachPoint, false, baseElement, ref needBackSmallNextLevel, ref needBackBigNextLevel);
    }

    private void GenerateTerrace(bool bigBase, bool bottomNeedSmallNextLevel, int levelIndex, Vector3 attachPoint, bool fore, GameObject parentElement, ref bool needSmallNextLevel, ref bool needBigNextLevel)
    {
        HouseTerrace selectedTerrace = TerraceNone;

        if (!(bigBase || bottomNeedSmallNextLevel))
        {
            List<HouseTerrace> availableTerrace = Terrace.FindAll(x => (x.OnlyFloor ? levelIndex == 0 : true) && (x.OnlyStorey ? levelIndex > 0 : true));
            selectedTerrace = availableTerrace[Random.Range(0, availableTerrace.Count)];
        }

        if (selectedTerrace.Equals(TerraceNone) && levelIndex > 0) { return; }

        GameObject terrace = Instantiate(selectedTerrace.Prefab, attachPoint, parentElement.transform.rotation);
        terrace.transform.parent = parentElement.transform;
        terrace.transform.localRotation = Quaternion.Euler(0, 0, (fore ? 180 : 0));

        needSmallNextLevel = (needSmallNextLevel ? true : selectedTerrace.NeedSmallNextLevel);
        needBigNextLevel = (needBigNextLevel ? true : selectedTerrace.NeedBigNextLevel);

        if (!selectedTerrace.Equals(TerraceNone))
        {
            SetMaterials(terrace.GetComponent<Renderer>());
        }

        GenerateDoor(attachPoint + selectedTerrace.DoorAttachPoint, terrace);

        for (int i = 0; i < selectedTerrace.WindowAttachPoints.Count; i++)
        {
            GenerateWindow(attachPoint + selectedTerrace.WindowAttachPoints[i], false, terrace);
        }
    }

    private void GenerateDoor(Vector3 attachPoint, GameObject parentElement)
    {
        GameObject selectedDoor = Door[Random.Range(0, Door.Count)];

        GameObject door = Instantiate(selectedDoor, attachPoint, parentElement.transform.rotation);
        door.transform.parent = parentElement.transform;

        SetMaterials(door.GetComponent<Renderer>());
    }

    private void GenerateWindow(Vector3 attachPoint, bool rotate, GameObject parentElement)
    {
        GameObject selectedWindow = Window[Random.Range(0, Window.Count)];

        GameObject window = Instantiate(selectedWindow, attachPoint, parentElement.transform.rotation);
        window.transform.parent = parentElement.transform;

        SetMaterials(window.GetComponent<Renderer>());
    }

    private void GenerateRoof(bool bottomNeedForeSmallNextLevel, bool bottomNeedBackSmallNextLevel, bool bottomNeedForeBigNextLevel, bool bottomNeedBackBigNextLevel, Vector3 attachPoint, GameObject parentElement)
    {
        List<HouseRoof> availableRoof = Roof.FindAll(x => (bottomNeedForeSmallNextLevel ? !x.ForeBigBase : true) && (bottomNeedBackSmallNextLevel ? !x.BackBigBase : true) && 
                                                          (bottomNeedForeBigNextLevel ? x.ForeBigBase : true) && (bottomNeedBackBigNextLevel ? x.BackBigBase : true));
        HouseRoof selectedRoof = availableRoof[Random.Range(0, availableRoof.Count)];

        GameObject roof = Instantiate(selectedRoof.Prefab, attachPoint, parentElement.transform.rotation);
        roof.transform.parent = parentElement.transform;
        roof.transform.localRotation = Quaternion.Euler(-90, 0, 0);

        SetMaterials(roof.GetComponent<Renderer>());

        GenerateTerraceRoof(attachPoint + selectedRoof.ForeTerraceAttachPoint, true, roof);
        GenerateTerraceRoof(attachPoint + selectedRoof.BackTerraceAttachPoint, false, roof);
    }

    private void GenerateTerraceRoof(Vector3 attachPoint, bool fore, GameObject parentElement)
    {
        HouseTerraceRoof selectedTerraceRoof = TerraceRoof[Random.Range(0, TerraceRoof.Count)];

        GameObject terraceRoof = Instantiate(selectedTerraceRoof.Prefab, attachPoint, parentElement.transform.rotation);
        terraceRoof.transform.parent = parentElement.transform;
        terraceRoof.transform.localRotation = Quaternion.Euler(0, 0, (fore ? 180 : 0));

        GenerateRoofFrontWindow(attachPoint + selectedTerraceRoof.WindowAttachPoint, terraceRoof);
    }

    private void GenerateRoofFrontWindow(Vector3 attachPoint, GameObject parentElement)
    {
        GameObject selectedWindow = RoofFrontWindow[Random.Range(0, RoofFrontWindow.Count)];

        GameObject window = Instantiate(selectedWindow, attachPoint, parentElement.transform.rotation);
        window.transform.parent = parentElement.transform;

        SetMaterials(window.GetComponent<Renderer>());
    }
}
