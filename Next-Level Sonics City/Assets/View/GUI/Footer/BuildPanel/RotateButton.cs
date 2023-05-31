using Model.Tiles;
using System.Collections.Generic;
using UnityEngine;

namespace View.GUI.Footer.BuildPanel
{
	public class RotateButton : MonoBehaviour, IClickable
	{
		public void OnClick(bool isLeftMouseButton, Vector3 location)
		{
			if ((TileManager.Instance.GhostTile != null ? TileManager.Instance.GhostTile.TileModel : null) is Building building)
			{
				building.Rotate();
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
	}
}