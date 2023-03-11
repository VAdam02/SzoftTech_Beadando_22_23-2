using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualMouse : MonoBehaviour
{
    private RectTransform _mouseRectTransform;
    private RawImage _mouseRawImage;

    private bool _visible = true;
    public bool Visible
    {
        get => _visible;
        set { _visible = value; }
    }

    private Vector3 _mousePosition = Vector3.zero;
    public Vector3 MousePosition
    {
        get => _mousePosition;
        set
        {
            _mousePosition = value;
            VerifyMousePos();
        }
    }

    public float _clickMaxHoldTime = 0.2f;
    public int _mouseSensitivity = 650;

    /*
     * true && 0 => just clicked
     * true && >0 => hold
     * false && >0 => just released
     * false && 0 => not clicked
    */
    private bool _leftClick = false;
    private float _leftClickTime = 0;
    private bool _rightClick = false;
    private float _rightClickTime = 0;
    
    public bool IsLeftClick() { return (!_leftClick && 0 < _leftClickTime && _leftClickTime <= _clickMaxHoldTime ? true : false); }
    public bool IsLeftHold() { return (_leftClick && _leftClickTime > _clickMaxHoldTime ? true : false); }

    public bool IsRightClick() { return (!_rightClick && 0 < _rightClickTime && _rightClickTime <= _clickMaxHoldTime ? true : false); }
    public bool IsRightHold() { return (_rightClick && _rightClickTime > _clickMaxHoldTime ? true : false); }

    void VerifyMousePos()
    {
        _mousePosition = new Vector3((_mousePosition.x < 0 ? 0 : _mousePosition.x), (_mousePosition.y < 0 ? 0 : _mousePosition.y), 0); //Bottom left wall
        _mousePosition = new Vector3((_mousePosition.x > Screen.width ? Screen.width : _mousePosition.x), (_mousePosition.y > Screen.height ? Screen.height : _mousePosition.y), 0); //Top right wall
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _mouseRectTransform = GetComponent<RectTransform>();
        _mouseRawImage = GetComponent<RawImage>();
        _mousePosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //LEFT CLICK
        if (Input.GetMouseButtonDown(0) && !Input.GetMouseButton(1)) { _leftClick = true; }
        else if (Input.GetMouseButton(0) && !Input.GetMouseButton(1)) { _leftClickTime += Time.deltaTime; }
        else if (Input.GetMouseButtonUp(0) || Input.GetMouseButton(1)) { _leftClick = false; }
        else if (!Input.GetMouseButton(0)) { _leftClickTime = 0; }

        //RIGHT CLICK
        if (Input.GetMouseButtonDown(1) && !Input.GetMouseButton(0)) { _rightClick = true; }
        else if (Input.GetMouseButton(1) && !Input.GetMouseButton(0)) { _rightClickTime += Time.deltaTime; }
        else if (Input.GetMouseButtonUp(1) || Input.GetMouseButton(0)) { _rightClick = false; }
        else if (!Input.GetMouseButton(1)) { _rightClickTime = 0; }

        //MOUSE POSITION
        if (_visible)
        {
            _mousePosition += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0) * Time.deltaTime * _mouseSensitivity;
            VerifyMousePos();
            _mouseRectTransform.transform.position = _mousePosition;
            _mouseRawImage.enabled = true;
        }
        else
        {
            _mouseRawImage.enabled = false;
        }
    }
}
