using Model.Statistics;
using System;
using TMPro;
using UnityEngine;

namespace View.GUI.StatPanel
{
	public class StatReport : MonoBehaviour
	{
		public int NthStatReport = 0;
		private Color _great = new Color32(128, 255, 0, 255);
		private Color _bad = new Color32(255, 32, 0, 255);

		void OnEnable()
		{
			Model.Statistics.StatReport statReport = StatEngine.Instance.GetLastNthStatisticsReports(NthStatReport);
			if (statReport == null) { return; }

			foreach (Transform child in gameObject.transform)
			{
				TextMeshProUGUI textBox = child.GetChild(0).GetComponent<TextMeshProUGUI>();
				switch (child.name)
				{
					case "YearQuarter":
						textBox.text = statReport.Year + "/" + statReport.Quarter;
						break;
					case "Happiness":
						textBox.text = Mathf.Round(statReport.Happiness * 100) + "%";
						textBox.color = statReport.Happiness >= 0.75 ? _great : _bad;
						break;
					case "Population":
						textBox.text = statReport.Population.ToString("N0");
						break;
					case "MaintainanceCosts":
						textBox.text = "$" + (-statReport.MaintainanceCosts).ToString("N0");
						textBox.color = statReport.MaintainanceCosts < 0 ? _great : _bad;
						break;
					default:
						throw new NotImplementedException("Unknown field found: " + child.name);
				}
			}
		}
	}
}