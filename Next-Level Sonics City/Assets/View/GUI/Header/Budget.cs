using Model;
using Model.Statistics;
using TMPro;
using UnityEngine;

namespace View.GUI.Header
{
	public class Budget : MonoBehaviour
	{
		private void Awake()
		{
			GetComponent<TextMeshProUGUI>().text = "$" + StatEngine.Instance.Budget.ToString("N0");

			StatEngine.Instance.BudgetChanged += (sender, args) =>
			{
				if (MainThreadDispatcher.Instance is MainThreadDispatcher mainThread)
				{
					mainThread.Enqueue(() =>
					{
						GetComponent<TextMeshProUGUI>().text = "$" + StatEngine.Instance.Budget.ToString("N0");
					});
				}
			};
		}
	}
}