using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace View.GUI.VirtualMouse
{
	public class VirtualMouse : MonoBehaviour
	{
		private RectTransform _mouseRectTransform;
		private RawImage _mouseRawImage;

		private bool _visible = true;
		public bool Visible
		{
			get => _visible;
			set
			{
				_visible = value;
				_mouseRawImage.enabled = _visible;
			}
		}

		private Vector3 _mousePosition = Vector3.zero;
		private Vector3 MousePosition
		{
			get => _mousePosition;
			set
			{
				_mousePosition = value;
				_mousePosition.Set(
					Mathf.Clamp(_mousePosition.x, 0, Screen.width),
					Mathf.Clamp(_mousePosition.y, 0, Screen.height),
					0);
				_mouseRectTransform.transform.position = _mousePosition;

				_mouseRectTransform.transform.SetSiblingIndex(transform.childCount - 1);
			}
		}


		public float ClickMaxHoldTime = 0.3f;
		public float MouseSensitivity = 16.5f;
		public float MouseMoveToleranceDuringClick = 5;
		public float MouseScrollSensitivity = 1;

		private List<IClickable> _selectedElement = new();
		private List<IClickable> _hoveredElement = new();
		private List<IClickable> _oldSelectedElement = new();
		private bool _isMouseDown;
		private bool _isLeftMouse;
		private bool _isPossibleClick;
		private bool _isMouseHold;
		private float _mouseHoldTime;

		private Vector3 _deltaMove;
		private Vector3 _sumDeltaMove;
		private Vector3 _mouseAt;

		/// <summary>
		/// Call the IClickable OnScroll function
		/// </summary>
		/// <param name="deltaScroll"></param>
		private void ScrollCheck(float deltaScroll)
		{
			if (deltaScroll == 0) { return; }
			foreach (IClickable item in _hoveredElement) { item.OnScroll(deltaScroll); }
		}

		/// <summary>
		/// Call the IClickable OnHover related function
		/// </summary>
		/// <param name="move"></param>
		private void HoverCheck(Vector3 move)
		{
			List<IClickable> hovered = GetTarget(MousePosition, false);

			if (move.magnitude > 0)
			{
				foreach (IClickable item in hovered.Except(_hoveredElement)) { item.OnHoverStart(MousePosition); }
				foreach (IClickable item in _hoveredElement.Intersect(hovered)) { item.OnHover(MousePosition); }
				foreach (IClickable item in _hoveredElement.Except(hovered)) { item.OnHoverEnd(); }
			}

			_hoveredElement = hovered;
		}

		/// <summary>
		/// Manage mouse input and call the IClickable OnClick and OnDrag related functions
		/// </summary>
		/// <param name="move">Deltamove since last call</param>
		private void ClickCheck(Vector3 move)
		{
			//filter if action interrupted by other mouse button
			if (Input.GetMouseButtonDown(0) && Input.GetMouseButton(1) ||
				Input.GetMouseButton(0) && Input.GetMouseButtonDown(1))
			{ ClickRelease(); return; }

			//mouse down -> init
			if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
			{
				_isLeftMouse = Input.GetMouseButton(0);

				_isPossibleClick = true;
				_isMouseDown = true;
				_mouseHoldTime = 0;
				_isMouseHold = false;
				_deltaMove.Set(0, 0, 0);
				_sumDeltaMove.Set(0, 0, 0);

				_oldSelectedElement = _selectedElement;
				_selectedElement = GetTarget(MousePosition, true);
			}
			//holding -> wait/dragstart/drag
			else if (_isMouseDown && Input.GetMouseButton(_isLeftMouse ? 0 : 1))
			{
				_mouseHoldTime += Time.deltaTime;
				_deltaMove = move;
				_sumDeltaMove += _deltaMove;
				if (_isPossibleClick && (_mouseHoldTime > ClickMaxHoldTime || Mathf.Abs(_sumDeltaMove.magnitude) > MouseMoveToleranceDuringClick)) { _isPossibleClick = false; }
				if (!_isPossibleClick)
				{
					//hold start
					if (!_isMouseHold)
					{
						_isMouseHold = true;
						foreach (IClickable item in _selectedElement) item.OnDragStart(_isLeftMouse, _mouseAt);
					}

					bool visible = true;
					foreach (IClickable item in _selectedElement) visible &= item.OnDrag(_isLeftMouse, _deltaMove);
					Visible = visible;
					_deltaMove.Set(0, 0, 0);
				}
			}
			//mouse up -> click/dragend
			else if (Input.GetMouseButtonUp(_isLeftMouse ? 0 : 1)) { ClickRelease(); }
		}

		/// <summary>
		/// Call the OnClick or OnDragEnd function of all IClickable components those were selected at the start of the click
		/// </summary>
		private void ClickRelease()
		{
			foreach (IClickable item in _selectedElement)
			{
				if (_isPossibleClick) item.OnClick(_isLeftMouse, _mouseAt);
				else item.OnDragEnd(_isLeftMouse);
			}

			if (_isPossibleClick) { foreach (IClickable olditem in _oldSelectedElement) olditem.OnSecondClick(_selectedElement); }

			_isPossibleClick = false;
			Visible = true;
			_isMouseDown = false;
			_oldSelectedElement = new();
		}

		/// <summary>
		/// Get all IClickable components at the clicked location on the UI (if any including transparent) and on the world (if any)
		/// Please note that the function has sideeffects if makeSideEffect is true
		/// </summary>
		/// <param name="position">Clicked location</param>
		/// <returns>List of IClickable</returns>
		private List<IClickable> GetTarget(Vector3 position, bool makeSideEffect)
		{
			List<IClickable> output = new();

			PointerEventData pointerEventData = new(GetComponent<EventSystem>()) { position = position };
			List<RaycastResult> results = new();
			transform.parent.GetComponent<GraphicRaycaster>().Raycast(pointerEventData, results);

			GameObject hitObject;
			//get the first object that has an IClickable component
			if (results.Count > 0)
			{
				if (makeSideEffect) { _mouseAt = results[0].worldPosition; }
				hitObject = results[0].gameObject;
				while (!(hitObject == null || hitObject.transform.GetComponents(typeof(IClickable)).Length != 0))
				{
					hitObject = hitObject.transform.parent != null ? hitObject.transform.parent.gameObject : null;
				}
				if (hitObject != null) output.AddRange(hitObject.transform.GetComponents<IClickable>());
				if (hitObject == null || !hitObject.CompareTag("IClickableTransparent")) return output;
			}

			//get the first object that has an IClickable component in the world if no UI object was found or the UI object is transparent
			if (Physics.Raycast(Camera.main.ScreenPointToRay(position), out RaycastHit lookat))
			{
				hitObject = lookat.transform.gameObject;
				while (!(hitObject == null || hitObject.transform.GetComponents(typeof(IClickable)).Length != 0))
				{
					hitObject = hitObject.transform.parent != null ? hitObject.transform.parent.gameObject : null;
				}
				if (hitObject != null)
				{
					output.AddRange(hitObject.transform.GetComponents<IClickable>());
				}
			}

			return output;
		}

		// Start is called before the first frame update
		private void Start()
		{
			Cursor.lockState = CursorLockMode.Locked;

			_mouseRectTransform = GetComponent<RectTransform>();
			_mouseRawImage = GetComponent<RawImage>();
			_mousePosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
		}

		// Update is called once per frame
		private void Update()
		{
			float currentScrool = Input.GetAxis("Mouse ScrollWheel") * MouseScrollSensitivity;
			Vector3 currentMove = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0) * MouseSensitivity;

			HoverCheck(currentMove);

			ScrollCheck(currentScrool);

			ClickCheck(currentMove);

			if (Visible)
			{
				MousePosition += currentMove;
			}
		}
	}
}