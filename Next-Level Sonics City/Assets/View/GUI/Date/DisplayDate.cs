using Model;
using Model.Statistics;
using UnityEngine;

namespace View.GUI.Date
{
	public class DisplayDate : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
			transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = StatEngine.Instance.Date.Day.ToString();
			transform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = StatEngine.Instance.Date.ToString("yyyy MMM");

			StatEngine.Instance.DateChanged += (sender, args) =>
			{
				if (MainThreadDispatcher.Instance is MainThreadDispatcher mainThread)
				{
					mainThread.Enqueue(() =>
					{
						transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = StatEngine.Instance.Date.Day.ToString();
						transform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = StatEngine.Instance.Date.ToString("yyyy MMM");
					});
				}
			};
        }
    }
}