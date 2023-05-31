using Model;
using TMPro;
using UnityEngine;

namespace View.GUI.Header
{
	public class Population : MonoBehaviour
	{
		private void Awake()
		{
			GetComponent<TextMeshProUGUI>().text = City.Instance.GetPopulation().ToString("N0");
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
}