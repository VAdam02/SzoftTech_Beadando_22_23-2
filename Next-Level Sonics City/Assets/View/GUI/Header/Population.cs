using Model;
using TMPro;
using UnityEngine;

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
					GetComponent<TextMeshProUGUI>().text = City.Instance.GetPopulation().ToString("N0");
				});
			}
		};
	}
}
