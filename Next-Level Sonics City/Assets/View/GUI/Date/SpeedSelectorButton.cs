using Model.Simulation;
using System.Collections.Generic;
using UnityEngine;

namespace View.GUI.Date
{
	public enum TimeSpeed
	{
		NORMAL = 1,
		FAST = 4,
		SONIC = 64
	}
	public class SpeedSelectorButton : MonoBehaviour, IClickable
	{
		public TimeSpeed speed;
		public void OnClick(bool isLeftMouseButton, Vector3 location)
		{
			SimEngine.Instance.SetTimeSpeed((int)speed);
			transform.parent.parent.GetComponent<Animator>().SetInteger("TimeSpeed", transform.GetSiblingIndex()-1);
		}

		public bool OnDrag(bool isLeftMouseButton, Vector3 direction) { return true; }

		public void OnDragEnd(bool isLeftMouseButton) { }

		public void OnDragStart(bool isLeftMouseButton, Vector3 location) { }

		public void OnHover(Vector3 location) { }

		public void OnHoverEnd() { }

		public void OnHoverStart(Vector3 location) { }

		public void OnScroll(float delta) { }

		public void OnSecondClick(List<IClickable> clicked) { }
	}
}
