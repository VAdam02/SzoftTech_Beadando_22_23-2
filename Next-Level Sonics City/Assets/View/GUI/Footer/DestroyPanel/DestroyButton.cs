using System.Collections.Generic;
using UnityEngine;

namespace View.GUI.Footer.DestroyPanel
{
	public class DestroyButton : MonoBehaviour, IClickable
	{
		public bool ForceDestroy = false;
		public void OnClick(bool isLeftMouseButton, Vector3 location)
		{
			TileManager.Instance.CurrentAction = ForceDestroy ? Action.FORCEDESTROY : Action.SOFTDESTROY;
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
