using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraMovement : MonoBehaviour
{
    public float TransSpeed = 10.0f;
    public float HorizontalSpeed = 40.0f;
    public float VerticalSpeed = 40.0f;
    public float ZoomSpeed = 20.0f;

    private Transform _rotate;
    private Transform _cameraobj;

    // Start is called before the first frame update
    void Start()
    {
        _rotate = transform.GetChild(0);
        _cameraobj = _rotate.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 translation = GetTranslationByInput().trans;
        Vector3 rotation = GetTranslationByInput().rot;
        
        transform.Translate(new Vector3(translation.x, 0, translation.z) * Time.deltaTime * TransSpeed);    //XZ
        transform.Rotate(new Vector3(0, rotation.y, 0) * Time.deltaTime * HorizontalSpeed);                 //Horizontal
        _rotate.Rotate(new Vector3(rotation.x, 0, 0) * Time.deltaTime * VerticalSpeed);                      //Vertical
        _cameraobj.Translate(new Vector3(0, 0, translation.y*-1) * Time.deltaTime * ZoomSpeed);              //Zoom
    }

    private (Vector3 trans, Vector3 rot) GetTranslationByInput()
    {
        Vector3 translation = Vector3.zero;
        Vector3 rotation = Vector3.zero;
        
        //X
        if (Input.GetKey(KeyCode.W)) { translation += new Vector3(0, 0, 1); }
        if (Input.GetKey(KeyCode.S)) { translation += new Vector3(0, 0, -1); }
        
        //Z
        if (Input.GetKey(KeyCode.A)) { translation += new Vector3(-1, 0, 0); }
        if (Input.GetKey(KeyCode.D)) { translation += new Vector3(1, 0, 0); }

        //Zoom
        if (Input.GetKey(KeyCode.KeypadPlus)) { translation += new Vector3(0, -1, 0); }
        if (Input.GetKey(KeyCode.KeypadMinus)) { translation += new Vector3(0, 1, 0); }

        //Vertical
        if (Input.GetKey(KeyCode.UpArrow)) { rotation += new Vector3(1, 0, 0); }
        if (Input.GetKey(KeyCode.DownArrow)) { rotation += new Vector3(-1, 0, 0); }

        //Horizontal
        if (Input.GetKey(KeyCode.LeftArrow)) { rotation += new Vector3(0, 1, 0); }
        if (Input.GetKey(KeyCode.RightArrow)) { rotation += new Vector3(0, -1, 0); }

        return (translation, rotation);
    }
}
