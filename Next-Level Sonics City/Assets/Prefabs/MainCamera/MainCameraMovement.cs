using UnityEngine;

public class MainCameraMovement : MonoBehaviour
{
	//LIMIT
	public float MinZoom = 0;
	public float MaxZoom = 10;
	public float MinDeg = 0;
	public float MaxDeg = 90;

	//MOUSE
	public float MouseTransMultiplier    = 5f;
	public float MouseZoomMultiplier     = 500f;
	public float MouseRotationMultiplier = 50f;

	//KEYBOARD
	public float TransMultiplier         = 10f;
	public float ZoomMultiplier          = 20f;
	public float RotationMultiplier      = 40f;

	public GameObject VirtualMouse;
	private VirtualMouse _virtualMouse;

	private Transform _rotate;
	private Transform _cameraobj;

	// Start is called before the first frame update
	void Start()
	{
		_virtualMouse = VirtualMouse.GetComponent<VirtualMouse>();
		_rotate = transform.GetChild(0);
		_cameraobj = _rotate.GetChild(0); 
	}

	// Update is called once per frame
	void Update()
	{
		/*
		//Manage input
		Vector3 translation = GetTranslationByInput().trans;
		Vector3 rotation = GetTranslationByInput().rot;
		
		transform.Translate(new Vector3(translation.x, 0, translation.z) * Time.deltaTime); //Translation
		_cameraobj.Translate(new Vector3(0, 0, translation.y * -1)       * Time.deltaTime); //Zoom
		_rotate.Rotate(new Vector3(rotation.x, 0, 0)                     * Time.deltaTime); //Vertical
		transform.Rotate(new Vector3(0, rotation.y, 0)                   * Time.deltaTime); //Horizontal

		//Limit zoom
		_cameraobj.transform.GetLocalPositionAndRotation(out Vector3 pos, out Quaternion rot);
		pos.y = Mathf.Clamp(pos.y, MinZoom, MaxZoom);
		_cameraobj.transform.SetLocalPositionAndRotation(pos, rot);

		//Limit rotation
		_rotate.transform.GetLocalPositionAndRotation(out pos, out rot);
		rot.x = Mathf.Clamp(rot.x, MinDeg, MaxDeg);
		_rotate.transform.SetLocalPositionAndRotation(pos, rot);
		*/
	}

	private (Vector3 trans, Vector3 rot) GetTranslationByInput()
	{
		Vector3 translation = Vector3.zero;
		Vector3 rotation = Vector3.zero;
	
		/*
		//HIDE/SHOW CURSOR
		if (_virtualMouse.Visible && (_virtualMouse.IsLeftHold() || _virtualMouse.IsRightHold())) { _virtualMouse.Visible = false; }        //HIDE CURSOR (if panning)
		else if (!_virtualMouse.Visible && !(_virtualMouse.IsLeftHold() || _virtualMouse.IsRightHold())) { _virtualMouse.Visible = true; }  //SHOW CURSOR (if not panning)

		//TRANSLATION
		if (_virtualMouse.IsLeftHold()) { translation += new Vector3(Input.GetAxis("Mouse X") * -1, 0, Input.GetAxis("Mouse Y") * -1) * MouseTransMultiplier; }    //PAN TRANSLATION
		else    //WASD
		{
			//X
			if (Input.GetKey(KeyCode.W)) { translation += new Vector3(0, 0, 1) * TransMultiplier; }
			if (Input.GetKey(KeyCode.S)) { translation += new Vector3(0, 0, -1) * TransMultiplier; }

			//Z
			if (Input.GetKey(KeyCode.A)) { translation += new Vector3(-1, 0, 0) * TransMultiplier; }
			if (Input.GetKey(KeyCode.D)) { translation += new Vector3(1, 0, 0) * TransMultiplier; }
		}

		//Zoom
		if (Input.GetAxis("Mouse ScrollWheel") != 0) { translation += new Vector3(0, Input.GetAxis("Mouse ScrollWheel") * -1 * MouseZoomMultiplier, 0); }   //WHEEL
		else    //NUMPAD +-
		{
			if (Input.GetKey(KeyCode.KeypadPlus)) { translation += new Vector3(0, -1, 0) * ZoomMultiplier; }
			if (Input.GetKey(KeyCode.KeypadMinus)) { translation += new Vector3(0, 1, 0) * ZoomMultiplier; }
		}

		//ROTATION
		if (_virtualMouse.IsRightHold()) { rotation += new Vector3(Input.GetAxis("Mouse Y") * -1, Input.GetAxis("Mouse X"), 0) * MouseRotationMultiplier; } //PAN ROTATION
		else    //ARROWS
		{
			//Vertical
			if (Input.GetKey(KeyCode.UpArrow)) { rotation += new Vector3(1, 0, 0) * RotationMultiplier; }
			if (Input.GetKey(KeyCode.DownArrow)) { rotation += new Vector3(-1, 0, 0) * RotationMultiplier; }

			//Horizontal
			if (Input.GetKey(KeyCode.LeftArrow)) { rotation += new Vector3(0, -1, 0) * RotationMultiplier; }
			if (Input.GetKey(KeyCode.RightArrow)) { rotation += new Vector3(0, 1, 0) * RotationMultiplier; }

		}
		*/

		return (translation, rotation);
	}
}
