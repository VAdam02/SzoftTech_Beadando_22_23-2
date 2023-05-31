using Model;
using Model.Statistics;
using TMPro;
using UnityEngine;

public class IWorkplaceModule : MonoBehaviour
{
	private void Awake()
	{
		transform.parent.GetComponent<PopUpWindow>().TileModelChanged.AddListener(OnTileModelChanged);
	}

	private void OnTileModelChanged(Tile tilemodel)
	{
		transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = ((IWorkplace)tilemodel).GetWorkersCount().ToString("N0") + " / " + ((IWorkplace)tilemodel).WorkplaceLimit.ToString("N0");
		transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(StatEngine.Instance.CalculateWorkplaceHappiness((IWorkplace)tilemodel) * 100) + "%";
	}
}
