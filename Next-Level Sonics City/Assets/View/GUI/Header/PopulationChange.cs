using Model;
using Model.Statistics;
using TMPro;
using UnityEngine;

namespace View.GUI.Header
{
	public class PopulationChange : MonoBehaviour
	{
		private Color _great = new Color32(128, 255, 0, 255);
		private Color _bad = new Color32(255, 32, 0, 255);

		private void Awake()
		{
			Display();
			City.Instance.PopulationChanged += (sender, person) =>
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
					GetComponent<TextMeshProUGUI>().text = (City.Instance.GetPopulation() - StatEngine.Instance.GetLastNthStatisticsReport(4).Population).ToString("N0");
					GetComponent<TextMeshProUGUI>().color = (City.Instance.GetPopulation() - StatEngine.Instance.GetLastNthStatisticsReport(4).Population) >= 0 ? _great : _bad;
				});
			}
		}
	}
}