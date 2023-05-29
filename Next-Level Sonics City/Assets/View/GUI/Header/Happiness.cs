using Model;
using UnityEngine;
using UnityEngine.UI;

namespace View.GUI.Header
{
	public class Happiness : MonoBehaviour
	{
		public Sprite[] happinessSprites;
		public float[] happinessThresholds;

		private void Awake()
		{
			int i = 0;
			while (i < happinessThresholds.Length && City.Instance.CityHappiness < happinessThresholds[i]) { i++; }
			GetComponent<Image>().sprite = happinessSprites[i];

			City.Instance.CityHappinessChanged += (sender, args) =>
			{
				if (MainThreadDispatcher.Instance is MainThreadDispatcher mainThread)
				{
					mainThread.Enqueue(() =>
					{
						int i = 0;
						while (i < happinessThresholds.Length && City.Instance.CityHappiness > happinessThresholds[i]) { i++; }
						GetComponent<Image>().sprite = happinessSprites[i];
					});
				}
			};
		}
	}
}