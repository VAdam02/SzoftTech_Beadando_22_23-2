using Model;
using TMPro;
using UnityEngine;

public class TileModule : MonoBehaviour
{
	private void Awake()
	{
		transform.parent.GetComponent<PopUpWindow>().TileModelChanged.AddListener(OnTileModelChanged);
	}

	private void OnTileModelChanged(Tile tilemodel)
	{
		transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "$" + tilemodel.DestroyIncome.ToString("N0");
		transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = "$" + tilemodel.MaintainanceCost.ToString("N0");
	}
}
