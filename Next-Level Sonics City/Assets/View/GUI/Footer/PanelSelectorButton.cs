using System.Collections.Generic;
using UnityEngine;

namespace View.GUI.Footer
{
	public class PanelSelectorButton : MonoBehaviour, IClickable
	{
		public int PanelID;
		public Action Action;

		public void OnClick(bool isLeftMouseButton, Vector3 location)
		{
			Animator anim = transform.parent.parent.GetComponent<Animator>();

			TileManager.Instance.CurrentAction = Action.NONE;

			if (PanelID == anim.GetInteger("DisplayedPanel"))
			{
				anim.SetInteger("DisplayedPanel", 0);
			}
			else
			{
				anim.SetInteger("DisplayedPanel", PanelID);
				TileManager.Instance.CurrentAction = Action;
			}
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

		// Start is called before the first frame update
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}
	}

}
