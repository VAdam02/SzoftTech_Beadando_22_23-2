using System.Collections.Generic;
using UnityEngine;

namespace View.GUI.Date
{
	public class TimeSpeedPanelOpenButton : MonoBehaviour, IClickable
	{
		public void OnClick(bool isLeftMouseButton, Vector3 location)
		{
			Animator anim = transform.GetComponent<Animator>();
			anim.SetBool("Display", !anim.GetBool("Display"));
		}

		public bool OnDrag(bool isLeftMouseButton, Vector3 direction)
		{
			return true;
		}

		public void OnDragEnd(bool isLeftMouseButton)
		{

		}

		public void OnDragStart(bool isLeftMouseButton, Vector3 location)
		{

		}

		public void OnHover(Vector3 location)
		{

		}

		public void OnHoverEnd()
		{

		}

		public void OnHoverStart(Vector3 location)
		{

		}

		public void OnScroll(float delta)
		{

		}

		public void OnSecondClick(List<IClickable> clicked)
		{

		}
	}
}
