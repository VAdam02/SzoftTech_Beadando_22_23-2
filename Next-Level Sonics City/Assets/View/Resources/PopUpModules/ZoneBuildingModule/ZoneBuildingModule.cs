using Model;
using Model.Tiles.Buildings;
using TMPro;
using UnityEngine;

public class ZoneBuildingModule : MonoBehaviour
{
	private void Awake()
	{
		transform.parent.GetComponent<PopUpWindow>().TileModelChanged.AddListener(OnTileModelChanged);
	}

	private void OnTileModelChanged(Tile tilemodel)
	{
		transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = ((IZoneBuilding)tilemodel).Level.ToString();
		transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = ((IZoneBuilding)tilemodel).Level == ZoneBuildingLevel.THREE || ((IZoneBuilding)tilemodel).Level == ZoneBuildingLevel.ZERO ? "----------" : ((IZoneBuilding)tilemodel).LevelUpCost.ToString("N0");
	}
}
