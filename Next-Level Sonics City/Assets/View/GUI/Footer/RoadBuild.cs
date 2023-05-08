using System.Collections.Generic;
using UnityEngine;

namespace View.GUI.Footer
{
	public class RoadBuild : MonoBehaviour, IClickable
	{
		public void OnClick(bool isLeftMouseButton, Vector3 location)
		{
			Animator anim = transform.parent.parent.GetComponent<Animator>();
			anim.SetInteger("DisplayedPanel", anim.GetInteger("DisplayedPanel") == 4 ? 0 : 4);
			
			//TileManager.Instance.CurrentAction = anim.GetInteger("DisplayedPanel") == 4 ? Action.SELECTAREA : Action.NONE;
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
