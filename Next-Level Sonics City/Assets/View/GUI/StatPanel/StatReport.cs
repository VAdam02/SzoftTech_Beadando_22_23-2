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
			Model.Statistics.StatReport statReport = StatEngine.Instance.GetLastNthStatisticsReport(NthStatReport);

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
						textBox.color = statReport.Happiness >= 0.5 ? _great : _bad;
						break;
					case "Population":
						textBox.text = statReport.Population.ToString("N0");
						break;
					case "MaintainanceCosts":
						textBox.text = "$" + (-statReport.MaintainanceCosts).ToString("N0");
						textBox.color = statReport.MaintainanceCosts < 0 ? _great : statReport.MaintainanceCosts == 0 ? Color.white : _bad;
						break;
					case "TaxIncome":
						textBox.text = "$" + ((statReport.WorkplaceTax) + (statReport.ResidentialTax)).ToString("N0");
						break;
					case "Pension":
						textBox.text ="$" + statReport.Pension.ToString("N0");
						textBox.color = statReport.Pension == 0 ? Color.white : _bad;
						break;
					case "PopulationChange":
						textBox.text = statReport.PopulationChange.ToString("N0");
						textBox.color = statReport.PopulationChange > 0 ? _great : statReport.PopulationChange == 0 ? Color.white : _bad ;
					break;
					case "DestroyIncomes":
						textBox.text = "$" + statReport.DestroyIncomes.ToString("N0");
						break;
					case "BuildExpenses":
						textBox.text = "$" + statReport.BuildExpenses.ToString("N0");
						textBox.color = statReport.BuildExpenses == 0 ? Color.white : _bad;
						break;
					case "Incomes":
						textBox.text = "$" + statReport.Incomes.ToString("N0");
						textBox.color = statReport.Incomes == 0 ? Color.white : _great;
						break;
					case "Expenses":
						textBox.text = "$" + statReport.Expenses.ToString("N0");
						textBox.color = statReport.Expenses == 0? Color.white : _bad;
						break;
					case "Profit":
						textBox.text = "$" + statReport.Profit.ToString("N0");
						textBox.color = statReport.Profit > 0 ? _great : statReport.Profit == 0 ? Color.white : _bad;
						break;
					default:
						throw new NotImplementedException("Unknown field found: " + child.name);
				}
			}
		}
	}
}