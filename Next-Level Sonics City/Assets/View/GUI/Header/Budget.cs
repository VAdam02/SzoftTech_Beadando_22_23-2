using Model.Statistics;
using TMPro;
using UnityEngine;

public class Budget : MonoBehaviour
{
	private void Awake()
	{
		StatEngine.Instance.BudgetChanged.AddListener(RefreshMoney);
	}

	private void RefreshMoney()
	{
		GetComponent<TextMeshProUGUI>().text = "$" + StatEngine.Instance.Budget.ToString("N0");
	}
}
