using Model.Simulation;
using UnityEngine;

namespace View.GUI.Date
{
    public class DisplayDate : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SimEngine.Instance.StatEngine.DateChanged.AddListener(UpdateDate);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateDate()
        {
            transform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = SimEngine.Instance.StatEngine.Day;
            transform.GetChild(1).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = SimEngine.Instance.StatEngine.YearMonth;
        }
    }
}