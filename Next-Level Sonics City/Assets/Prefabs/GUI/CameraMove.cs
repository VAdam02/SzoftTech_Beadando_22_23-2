using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour, IClickable
{
	public float MouseTransMultiplier =		0.25f;
	public float MouseRotateMultiplier =	0.25f;
	public float MouseZoomMultiplier =		10;
	public float KeyboardTransMultiplier =	100;
	public float KeyboardRotateMultiplier =	100;
	public float KeyboardZoomMultiplier =	10;


	public GameObject Camera;

	public	Vector3 LookAtMinLimit =	new(0,		0,		0);
	public	Vector3 LookAtMaxLimit =	new(1000,	10,		1000);
	private Vector3 _lookAt = Vector3.zero;
	public	Vector3 LookAt
	{
		get => _lookAt;
		set
		{
			_lookAt.Set(Mathf.Clamp(value.x, LookAtMinLimit.x, LookAtMaxLimit.x),
						Mathf.Clamp(value.y, LookAtMinLimit.y, LookAtMaxLimit.y),
						Mathf.Clamp(value.z, LookAtMinLimit.z, LookAtMaxLimit.z));
			Camera.transform.localPosition = _lookAt;
		}
	}

	public	Vector3 LookFromMinLimit =	new(0,		30,	2);
	public	Vector3 LookFromMaxLimit =	new(360,	90,	150);
	private Vector3 _lookFrom = Vector3.zero;
	public	Vector3 LookFrom
	{
		get => _lookFrom;
		set
		{
			_lookFrom.Set(Mathf.Clamp((value.x + 360) % 360, LookFromMinLimit.x, LookFromMaxLimit.x),
						  Mathf.Clamp(value.y, LookFromMinLimit.y, LookFromMaxLimit.y),
						  Mathf.Clamp(value.z, LookFromMinLimit.z, LookFromMaxLimit.z));
			Camera.transform.localRotation = Quaternion.Euler(0, _lookFrom.x, 0);
			Camera.transform.GetChild(0).localRotation = Quaternion.Euler(_lookFrom.y, 0, 0);
			Camera.transform.GetChild(0).transform.GetChild(0).localPosition = new(0, 0, -_lookFrom.z);
		}
	}

	public void OnClick(bool isLeftMouseButton, Vector3 location) { }
	 
	public void OnDragStart(bool isLeftMouseButton, Vector3 location) { }

	public bool OnDrag(bool isLeftMouseButton, Vector3 direction)
	{
		if (isLeftMouseButton) { LookAt += (Camera.transform.right * direction.x + Camera.transform.forward * direction.y) * MouseTransMultiplier; }
		else { LookFrom += new Vector3(direction.x, -direction.y, direction.z) * MouseRotateMultiplier; }

		return false;
	}

	public void OnDragEnd(bool isLeftMouseButton) { }

	public void OnSecondClick(List<IClickable> clicked) { }

	public void OnHoverStart(Vector3 location) { }

	public void OnHover(Vector3 location) { }

	public void OnHoverEnd() { }

	public void OnScroll(float delta)
	{
		LookFrom += -delta * MouseZoomMultiplier * new Vector3(0, 0, 1);
	}

	// Start is called before the first frame update
	void Start()
	{
		LookAt =	new(0, 5, 0);
		LookFrom =	new(0, 0, 10);
	}

	// Update is called once per frame
	void Update()
	{
		//Forward/backwards
		if (Input.GetKey(KeyCode.W) ^ Input.GetKey(KeyCode.S))
		{
			LookAt += (Input.GetKey(KeyCode.W) ? 1 : -1) * KeyboardTransMultiplier * Time.deltaTime * Camera.transform.forward;
		}
		//Left/right
		if (Input.GetKey(KeyCode.A) ^ Input.GetKey(KeyCode.D))
		{
			LookAt += (Input.GetKey(KeyCode.D) ? 1 : -1) * KeyboardTransMultiplier * Time.deltaTime * Camera.transform.right;
		}
		//Zoom
		if (Input.GetKey(KeyCode.KeypadPlus) ^ Input.GetKey(KeyCode.KeypadMinus))
		{
			LookFrom += (Input.GetKey(KeyCode.KeypadMinus) ? 1 : -1) * KeyboardZoomMultiplier * Time.deltaTime * new Vector3(0, 0, 1);
		}
		//Pitch
		if (Input.GetKey(KeyCode.UpArrow) ^ Input.GetKey(KeyCode.DownArrow))
		{
			LookFrom += (Input.GetKey(KeyCode.UpArrow) ? 1 : -1) * KeyboardRotateMultiplier * Time.deltaTime * new Vector3(0, 1, 0);
		}
		//Yaw
		if (Input.GetKey(KeyCode.LeftArrow) ^ Input.GetKey(KeyCode.RightArrow))
		{
			LookFrom += (Input.GetKey(KeyCode.RightArrow) ? 1 : -1) * KeyboardRotateMultiplier * Time.deltaTime * new Vector3(1, 0, 0);
		}
	}
}
