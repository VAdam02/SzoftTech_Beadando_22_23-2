using Model;
using Model.Tiles;
using TMPro;
using UnityEngine;

public class ForestTilePopUp : MonoBehaviour
{
	private void Awake()
	{
		transform.parent.GetComponent<PopUpWindow>().TileModelChanged.AddListener(OnTileModelChanged);
	}

	private void OnTileModelChanged(Tile tilemodel)
	{
		transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = ((ForestTile)tilemodel).Age.ToString("N0");
	}
}
