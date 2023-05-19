using Model;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace View.Tiles.Buildings
{
	public class ResidentialBuildingTile : Tile
	{
		// Start is called before the first frame update
		void Start()
		{
			transform.localPosition = new Vector3(TileModel.Coordinates.x, 0, -TileModel.Coordinates.y) * 10;
			transform.localScale = Vector3.one;

			TileModel.DesignIDChangeEvent.AddListener(Display);
			gradientAndMaterialReloadEvent.AddListener(Display);

			Display();
		}

		// Update is called once per frame
		void Update()
		{
			
		}

		private static readonly UnityEvent gradientAndMaterialReloadEvent = new();

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

		public uint Seed
		{
			get { return (TileModel.DesignID & ~(Model.Tiles.Buildings.ResidentialBuildingTile.RESIDENTIAL_LEVEL_COUNT_MASK)) / (Model.Tiles.Buildings.ResidentialBuildingTile.RESIDENTIAL_LEVEL_COUNT_MASK + 1); }
		}
		public uint LevelCount
		{
			get { return TileModel.DesignID & Model.Tiles.Buildings.ResidentialBuildingTile.RESIDENTIAL_LEVEL_COUNT_MASK; }
		}

		#region Gradients and materials
		private static Material _grassMaterial;
		public static Material GrassMaterial { get { if (_grassMaterial == null) _grassMaterial = LoadMaterialByName("GrassMaterial"); return _grassMaterial; } }
		private Material _sharedGrassMaterial;
		private Material SharedGrassMaterial
		{
			get
			{
				if (_sharedGrassMaterial == null)
				{
					_sharedGrassMaterial = new Material(GrassMaterial);
					_materials.Add(_sharedGrassMaterial);
				}
				return _sharedGrassMaterial;
			}
		}

		private static Gradient _houseBottom;
		public static Gradient HouseBottom { get { _houseBottom ??= LoadGradientByName("HouseBottom"); return _houseBottom; } }
		private static Material _houseBottomMaterial;
		public static Material HouseBottomMaterial { get { if (_houseBottomMaterial == null) _houseBottomMaterial = LoadMaterialByName("HouseBottomMaterial"); return _houseBottomMaterial; } }
		private Material _sharedHouseBottomMaterial;
		private Material SharedHouseBottomMaterial
		{
			get
			{
				if (_sharedHouseBottomMaterial == null)
				{
					_sharedHouseBottomMaterial = new Material(HouseBottomMaterial)
					{
						color = HouseBottom.Evaluate(GetColor(0))
					};
					_materials.Add(_sharedHouseBottomMaterial);
				}
				return _sharedHouseBottomMaterial;
			}
		}

		private static Gradient _houseColor;
		public static Gradient HouseColor { get { _houseColor ??= LoadGradientByName("HouseColor"); return _houseColor; } }
		private static Material _houseColorMaterial;
		public static Material HouseColorMaterial { get { if (_houseColorMaterial == null) _houseColorMaterial = LoadMaterialByName("HouseColorMaterial"); return _houseColorMaterial; } }
		private Material _sharedHouseColorMaterial;
		private Material SharedHouseColorMaterial
		{
			get
			{
				if (_sharedHouseColorMaterial == null)
				{
					_sharedHouseColorMaterial = new Material(HouseColorMaterial)
					{
						color = HouseColor.Evaluate(GetColor(1))
					};
					_materials.Add(_sharedHouseColorMaterial);
				}
				return _sharedHouseColorMaterial;
			}
		}

		private static Material _woodPillarMaterial;
		public static Material WoodPillarMaterial { get { if (_woodPillarMaterial == null) _woodPillarMaterial = LoadMaterialByName("WoodPillarMaterial"); return _woodPillarMaterial; } }
		private Material _sharedWoodPillarMaterial;
		private Material SharedWoodPillarMaterial
		{
			get
			{
				if (_sharedWoodPillarMaterial == null)
				{
					_sharedWoodPillarMaterial = new Material(WoodPillarMaterial);
					_materials.Add(_sharedWoodPillarMaterial);
				}
				return _sharedWoodPillarMaterial;
			}
		}

		private static Gradient _roof;
		public static Gradient Roof { get { _roof ??= LoadGradientByName("Roof"); return _roof; } }
		private static Material _roofMaterial;
		public static Material RoofMaterial { get { if (_roofMaterial == null) _roofMaterial = LoadMaterialByName("RoofMaterial"); return _roofMaterial; } }
		private Material _sharedRoofMaterial;
		private Material SharedRoofMaterial
		{
			get
			{
				if (_sharedRoofMaterial == null)
				{
					_sharedRoofMaterial = new Material(RoofMaterial)
					{
						color = Roof.Evaluate(GetColor(2))
					};
					_materials.Add(_sharedRoofMaterial);
				}
				return _sharedRoofMaterial;
			}
		}

		private static Gradient _door;
		public static Gradient Door { get { _door ??= LoadGradientByName("Door"); return _door; } }
		private static Material _doorMaterial;
		public static Material DoorMaterial { get { if (_doorMaterial == null) _doorMaterial = LoadMaterialByName("DoorMaterial"); return _doorMaterial; } }
		private Material _sharedDoorMaterial;
		private Material SharedDoorMaterial
		{
			get
			{
				if (_sharedDoorMaterial == null)
				{
					_sharedDoorMaterial = new Material(DoorMaterial)
					{
						color = Door.Evaluate(GetColor(3))
					};
					_materials.Add(_sharedDoorMaterial);
				}
				return _sharedDoorMaterial;
			}
		}

		private static Gradient _window;
		public static Gradient Window { get { _window ??= LoadGradientByName("Window"); return _window; } }
		private static Material _windowMaterial;
		public static Material WindowMaterial { get { if (_windowMaterial == null) _windowMaterial = LoadMaterialByName("WindowMaterial"); return _windowMaterial; } }
		private Material _sharedWindowMaterial;
		private Material SharedWindowMaterial
		{
			get
			{
				if (_sharedWindowMaterial == null)
				{
					_sharedWindowMaterial = new Material(WindowMaterial)
					{
						color = Window.Evaluate(GetColor(4))
					};
					_materials.Add(_sharedWindowMaterial);
				}
				return _sharedWindowMaterial;
			}
		}

		private static Material _windowFrameMaterial;
		public static Material WindowFrameMaterial { get { if (_windowFrameMaterial == null) _windowFrameMaterial = LoadMaterialByName("WindowFrameMaterial"); return _windowFrameMaterial; } }
		private Material _sharedWindowFrameMaterial;
		private Material SharedWindowFrameMaterial
		{
			get
			{
				if (_sharedWindowFrameMaterial == null)
				{
					_sharedWindowFrameMaterial = new Material(WindowFrameMaterial);
					_materials.Add(_sharedWindowFrameMaterial);
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
					 if (materials[i].name.Split(' ')[0] == "GrassMaterial") 	{ materials[i] = SharedGrassMaterial; }
				else if (materials[i].name.Split(' ')[0] == "HouseBottom") 		{ materials[i] = SharedHouseBottomMaterial; }
				else if (materials[i].name.Split(' ')[0] == "HouseColor") 		{ materials[i] = SharedHouseColorMaterial; }
				else if (materials[i].name.Split(' ')[0] == "WoodPillar") 		{ materials[i] = SharedWoodPillarMaterial; }
				else if (materials[i].name.Split(' ')[0] == "Roof") 			{ materials[i] = SharedRoofMaterial; }
				else if (materials[i].name.Split(' ')[0] == "Door") 			{ materials[i] = SharedDoorMaterial; }
				else if (materials[i].name.Split(' ')[0] == "Window") 			{ materials[i] = SharedWindowMaterial; }
				else if (materials[i].name.Split(' ')[0] == "WindowFrame") 		{ materials[i] = SharedWindowFrameMaterial; }
				else
				{
					Debug.LogWarning(renderer);
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

			Vector3 attachPoint = new(0, 0, 0);
			SetSharedMaterials(gameObject.GetComponent<Renderer>());
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
			List<Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignBase> availableBase = Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignBases.FindAll(x => (!x.OnlyFloor || levelIndex == 0) && (!x.OnlyStorey || levelIndex > 0) &&
																																												  (!bottomNeedForeSmallNextLevel || !x.ForeBigBase) &&
																																												  (!bottomNeedBackSmallNextLevel || !x.BackBigBase));
			Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignBase selectedBase = availableBase[GetElement(BASEELEMENTID, levelIndex, availableBase.Count)];

			int divider = 5;
			GameObject baseElement = Instantiate(selectedBase.Prefab);
			baseElement.transform.parent = core.transform;
			baseElement.transform.localScale = Vector3.one / divider;
			baseElement.transform.SetLocalPositionAndRotation(attachPoint/divider, Quaternion.Euler(0, 0, 0));


			needForeBigNextLevel = needForeBigNextLevel || selectedBase.ForeBigBase;
			needBackBigNextLevel = needBackBigNextLevel || selectedBase.BackBigBase;

			if (baseElement.GetComponent<Renderer>() != null) SetSharedMaterials(baseElement.GetComponent<Renderer>());
			if (baseElement.GetComponent<LODGroup>() != null)
			{
				foreach (var lodGroup in baseElement.GetComponent<LODGroup>().GetLODs())
				foreach (var renderer in lodGroup.renderers)
				{
					SetSharedMaterials(renderer);
				}
			}

			DisplayTerraceElement(selectedBase.ForeBigBase, bottomNeedForeSmallNextLevel, levelIndex, selectedBase.ForeTerraceAttachPoint, true, baseElement, ref needForeSmallNextLevel, ref needForeBigNextLevel);
			DisplayTerraceElement(selectedBase.BackBigBase, bottomNeedBackSmallNextLevel, levelIndex, selectedBase.BackTerraceAttachPoint, false, baseElement, ref needBackSmallNextLevel, ref needBackBigNextLevel);
		}

		private void DisplayTerraceElement(bool bigBase, bool bottomNeedSmallNextLevel, uint levelIndex, Vector3 attachPoint, bool fore, GameObject parentElement, ref bool needSmallNextLevel, ref bool needBigNextLevel)
		{
			if (bigBase || bottomNeedSmallNextLevel)
			{
				DisplayJustWindowTerrace(levelIndex, attachPoint, fore, parentElement);
				return;
			}

			List<Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignTerrace> availableTerrace = Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignTerraces.FindAll(x => (!x.OnlyFloor || levelIndex == 0) && (!x.OnlyStorey || levelIndex > 0));
			Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignTerrace selectedTerrace = availableTerrace[GetElement(TERRACEELEMENTID, levelIndex, availableTerrace.Count)];

			GameObject terrace = Instantiate(selectedTerrace.Prefab);
			terrace.transform.parent = parentElement.transform;
			terrace.transform.localScale = Vector3.one;
			terrace.transform.SetLocalPositionAndRotation(attachPoint, Quaternion.Euler(0, 0, 0));
			terrace.transform.Rotate(new Vector3(0, fore ? 180 : 0, 0));

			needSmallNextLevel = needSmallNextLevel || selectedTerrace.NeedSmallNextLevel;
			needBigNextLevel = needBigNextLevel || selectedTerrace.NeedBigNextLevel;

			if (terrace.GetComponent<Renderer>() != null) SetSharedMaterials(terrace.GetComponent<Renderer>());
			if (terrace.GetComponent<LODGroup>() != null)
			{
				foreach (var lodGroup in terrace.GetComponent<LODGroup>().GetLODs())
				foreach (var renderer in lodGroup.renderers)
				{
					SetSharedMaterials(renderer);
				}
			}

			DisplayDoor(levelIndex, selectedTerrace.DoorAttachPoint, terrace);

			GameObject usedWindow = null;
			for (int i = 0; i < selectedTerrace.WindowAttachPoints.Count; i++)
			{
				usedWindow = DisplayWindow(levelIndex, selectedTerrace.WindowAttachPoints[i], terrace, usedWindow);
			}
		}

		private void DisplayJustWindowTerrace(uint levelIndex, Vector3 attachPoint, bool fore, GameObject parentElement)
		{
			List<Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignJustWindowTerrace> availableTerrace = Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignJustWindowTerraces.FindAll(x => (!x.OnlyFloor || levelIndex == 0) && (!x.OnlyStorey || levelIndex > 0));
			Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignJustWindowTerrace selectedTerrace = availableTerrace[GetElement(JUSTWINDOWTERRACEELEMENTID, levelIndex, availableTerrace.Count)];

			GameObject justWindowTerrace = Instantiate(selectedTerrace.Prefab, parentElement.transform.position + attachPoint, parentElement.transform.rotation);
			justWindowTerrace.transform.parent = parentElement.transform;
			justWindowTerrace.transform.localScale = Vector3.one;
			justWindowTerrace.transform.SetLocalPositionAndRotation(attachPoint, Quaternion.Euler(0, 0, 0));
			justWindowTerrace.transform.Rotate(new Vector3(0, fore ? 180 : 0, 0));

			if (justWindowTerrace.GetComponent<Renderer>() != null) SetSharedMaterials(justWindowTerrace.GetComponent<Renderer>());
			if (justWindowTerrace.GetComponent<LODGroup>() != null)
			{
				foreach (var lodGroup in justWindowTerrace.GetComponent<LODGroup>().GetLODs())
				foreach (var renderer in lodGroup.renderers)
				{
					SetSharedMaterials(renderer);
				}
			}

			if (selectedTerrace.HasDoor) DisplayDoor(levelIndex, selectedTerrace.DoorAttachPoint, justWindowTerrace);

			GameObject usedWindow = null;
			for (int i = 0; i < selectedTerrace.WindowAttachPoints.Count; i++)
			{
				usedWindow = DisplayWindow(levelIndex, selectedTerrace.WindowAttachPoints[i], justWindowTerrace, usedWindow);
			}
		}

		private void DisplayDoor(uint levelIndex, Vector3 attachPoint, GameObject parentElement)
		{
			GameObject selectedDoor = Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignDoors[GetElement(DOORELEMENTID, levelIndex, Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignDoors.Count)];

			GameObject door = Instantiate(selectedDoor);
			door.transform.parent = parentElement.transform;
			door.transform.localScale = Vector3.one;
			door.transform.SetLocalPositionAndRotation(attachPoint, Quaternion.Euler(0, 0, 0));

			if (door.GetComponent<Renderer>() != null) SetSharedMaterials(door.GetComponent<Renderer>());
			if (door.GetComponent<LODGroup>() != null)
			{
				foreach (var lodGroup in door.GetComponent<LODGroup>().GetLODs())
				foreach (var renderer in lodGroup.renderers)
				{
					SetSharedMaterials(renderer);
				}
			}
		}

		private GameObject DisplayWindow(uint levelIndex, Vector3 attachPoint, GameObject parentElement, GameObject preSelectedWindow = null)
		{
			GameObject selectedWindow = (preSelectedWindow != null ? preSelectedWindow : Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignWindows[GetElement(WINDOWELEMENTID, levelIndex, Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignWindows.Count)]);

			GameObject window = Instantiate(selectedWindow);
			window.transform.parent = parentElement.transform;
			window.transform.localScale = Vector3.one;
			window.transform.SetLocalPositionAndRotation(attachPoint, Quaternion.Euler(0, 0, 0));

			if (window.GetComponent<Renderer>() != null) SetSharedMaterials(window.GetComponent<Renderer>());
			if (window.GetComponent<LODGroup>() != null)
			{
				foreach (var lodGroup in window.GetComponent<LODGroup>().GetLODs())
				foreach (var renderer in lodGroup.renderers)
				{
					SetSharedMaterials(renderer);
				}
			}

			return selectedWindow;
		}

		private void DisplayRoof(uint levelIndex, Vector3 attachPoint, GameObject parentElement, bool bottomNeedForeSmallNextLevel, bool bottomNeedBackSmallNextLevel, bool bottomNeedForeBigNextLevel, bool bottomNeedBackBigNextLevel)
		{
			List<Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignRoof> availableRoof = Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignRoofs.FindAll(x => (!bottomNeedForeSmallNextLevel || !x.ForeBigBase) && (!bottomNeedBackSmallNextLevel || !x.BackBigBase) &&
																																(!bottomNeedForeBigNextLevel || x.ForeBigBase) && (!bottomNeedBackBigNextLevel || x.BackBigBase));
			Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignRoof selectedRoof = availableRoof[GetElement(ROOFELEMENTID, levelIndex, availableRoof.Count)];

			int divider = 5;
			GameObject roof = Instantiate(selectedRoof.Prefab);
			roof.transform.parent = parentElement.transform;
			roof.transform.localScale = Vector3.one * 100 / divider;
			roof.transform.SetLocalPositionAndRotation(attachPoint / divider, Quaternion.Euler(-90, 0, 0));

			if (roof.GetComponent<Renderer>() != null) SetSharedMaterials(roof.GetComponent<Renderer>());
			if (roof.GetComponent<LODGroup>() != null)
			{
				foreach (var lodGroup in roof.GetComponent<LODGroup>().GetLODs())
				foreach (var renderer in lodGroup.renderers)
				{
					SetSharedMaterials(renderer);
				}
			}

			DisplayTerraceRoof(levelIndex, selectedRoof.ForeTerraceAttachPoint, true, roof);
			DisplayTerraceRoof(levelIndex, selectedRoof.BackTerraceAttachPoint, false, roof);
		}

		private void DisplayTerraceRoof(uint levelIndex, Vector3 attachPoint, bool fore, GameObject parentElement)
		{
			Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignTerraceRoof selectedTerraceRoof = Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignTerraceRoofs[GetElement(TERRACEROOFELEMENTID, levelIndex, Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignTerraceRoofs.Count)];

			GameObject terraceRoof = Instantiate(selectedTerraceRoof.Prefab);
			terraceRoof.transform.parent = parentElement.transform;
			terraceRoof.transform.localScale = Vector3.one;
			terraceRoof.transform.SetLocalPositionAndRotation(attachPoint / -100, Quaternion.Euler(90, 0, 0));
			terraceRoof.transform.Rotate(new Vector3(0, fore ? 0 : 180, 0));

			if (terraceRoof.GetComponent<Renderer>() != null) SetSharedMaterials(terraceRoof.GetComponent<Renderer>());
			if (terraceRoof.GetComponent<LODGroup>() != null)
			{
				foreach (var lodGroup in terraceRoof.GetComponent<LODGroup>().GetLODs())
				foreach (var renderer in lodGroup.renderers)
				{
					SetSharedMaterials(renderer);
				}
			}

			DisplayRoofFrontWindow(levelIndex, selectedTerraceRoof.WindowAttachPoint, terraceRoof);
		}

		private void DisplayRoofFrontWindow(uint levelIndex, Vector3 attachPoint, GameObject parentElement)
		{
			GameObject selectedWindow = Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignRoofFrontWindows[GetElement(ROOFFRONTWINDOWELEMENTID, levelIndex, Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignRoofFrontWindows.Count)];

			GameObject window = Instantiate(selectedWindow);
			window.transform.parent = parentElement.transform;
			window.transform.localScale = Vector3.one / 100;
			window.transform.SetLocalPositionAndRotation(attachPoint, Quaternion.Euler(180, 0, 180));

			if (window.GetComponent<Renderer>() != null) SetSharedMaterials(window.GetComponent<Renderer>());
			if (window.GetComponent<LODGroup>() != null)
			{
				foreach (var lodGroup in window.GetComponent<LODGroup>().GetLODs())
				foreach (var renderer in lodGroup.renderers)
				{
					SetSharedMaterials(renderer);
				}
			}
		}
		#endregion

		#region GetValuesBySeed
		private float GetColor(int colorID)
		{
			return (Mathf.Clamp(Mathf.PerlinNoise((float)((float)Seed / 10), (float)((float)(colorID + 1) * COLORMULTIPLIER / 10)), 0, 1) * 1000) % 1;
		}

		private int GetElement(uint elementID, uint levelID, int elementCount)
		{
			return (int)(Mathf.Clamp(Mathf.PerlinNoise((float)((float)Seed / 10), (float)((float)(elementID + 1) * (levelID + 1) * ELEMENTMULTIPLIER / 10)), 0, 1) * 1000) % elementCount;
		}
		#endregion

		#region Load and reload materials and gradients
		public void ReloadMaterialsAndGradients()
		{
			_grassMaterial = null;
			_sharedGrassMaterial = null;

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

			_materials.Clear();

			gradientAndMaterialReloadEvent.Invoke();
		}

		private static Material LoadMaterialByName(string name)
		{
			return Resources.Load<Material>("Tiles/ResidentialBuildingTile/Material/" + name);
		}

		private static Gradient LoadGradientByName(string name)
		{
			Gradient gradient = new();
			GradientColorKey[] colorKeys = new GradientColorKey[((JArray)Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignRules["Colors"][name]).Count];
			for (int i = 0; i < colorKeys.Length; i++)
			{
				colorKeys[i].time = (float)Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignRules["Colors"][name][i][0];
				colorKeys[i].color = new Color(
					(float)Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignRules["Colors"][name][i][1] / 256,
					(float)Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignRules["Colors"][name][i][2] / 256,
					(float)Model.Tiles.Buildings.ResidentialBuildingTile.ResidentialDesignRules["Colors"][name][i][3] / 256
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
}
