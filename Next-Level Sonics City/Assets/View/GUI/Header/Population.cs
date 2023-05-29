using Model;
using Model.Statistics;
using TMPro;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class Population : MonoBehaviour
{
	private void Awake()
	{
		City.Instance.PopulationChanged += (sender, person) =>
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
