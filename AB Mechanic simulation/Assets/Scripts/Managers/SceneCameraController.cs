using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCameraController : MonoBehaviour
{
    public Camera targetCamera;
    private Vector3 originalPositon;
    private Vector3 originalRotation;
    public bool enableRotate;
    public float speed = 3.5f;
    private float X;
    private float Y;

    // Start is called before the first frame update
    void Start()
    {
        targetCamera = transform.GetChild(0).GetComponent<Camera>();
        originalPositon = targetCamera.transform.position;
        originalRotation = transform.rotation.eulerAngles;
    }

    public void ResetCam()
    {
        targetCamera.transform.position = originalPositon;
        transform.rotation.eulerAngles.Set(originalRotation.x,originalRotation.y, originalRotation.z);
        Debug.Log("Reset camera");
    }

    void Update()
    {
        if (enableRotate && Input.GetMouseButton(0))
        {
            transform.Rotate(-new Vector3(Input.GetAxis("Mouse Y") * speed, -Input.GetAxis("Mouse X") * speed, 0));
            X = transform.rotation.eulerAngles.x;
            Y = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(X, Y, 0);
        }
    }

    public void Zoom(float num)
    {
        LeanTween.moveLocal(targetCamera.gameObject, targetCamera.transform.localPosition + new Vector3(0, 0, num),0.2f);
    }
}
