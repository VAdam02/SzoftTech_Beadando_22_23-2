using Model.Tiles;
using System.Collections.Generic;
using UnityEngine;

namespace View.GUI.Footer.BuildPanel
{
	public class BuildButton : MonoBehaviour, IClickable
	{
		public ZoneType zone;
		public void OnClick(bool isLeftMouseButton, Vector3 location)
		{
			TileManager.Instance.MarkZone(zone);
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
