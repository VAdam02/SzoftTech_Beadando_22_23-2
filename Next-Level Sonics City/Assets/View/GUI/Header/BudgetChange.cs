using Model;
using Model.Statistics;
using TMPro;
using UnityEngine;

namespace View.GUI.Header
{
	public class BudgetChange : MonoBehaviour
	{
		private Color _great = new Color32(128, 255, 0, 255);
		private Color _bad = new Color32(255, 32, 0, 255);

		private void Awake()
		{
			Display();
			StatEngine.Instance.BudgetChanged += (sender, args) =>
			{
				Display();
			};
			StatEngine.Instance.NextQuarterEvent += (sender, e) =>
			{
				Display();
			};
		}

		private void Display()
		{
			if (MainThreadDispatcher.Instance is MainThreadDispatcher mainThread)
			{
				mainThread.Enqueue(() =>
				{
					GetComponent<TextMeshProUGUI>().text = "$" + (StatEngine.Instance.Budget - StatEngine.Instance.GetLastNthStatisticsReport(4).Budget).ToString("N0");
					GetComponent<TextMeshProUGUI>().color = (StatEngine.Instance.Budget - StatEngine.Instance.GetLastNthStatisticsReport(4).Budget) >= 0 ? _great : _bad;
				});
			}
		}
	}
}