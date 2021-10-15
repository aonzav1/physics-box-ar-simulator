using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    private Vector3 staticPosition;
    private Quaternion staticRotation;
    // Start is called before the first frame update
    void Start()
    {
        SmoothlyEnter();
    }

    private void SmoothlyEnter()
    {
        staticPosition = transform.position;
        transform.position = transform.position + new Vector3(0, 1, -2);
        staticRotation = transform.rotation;
        transform.rotation = transform.rotation * Quaternion.Euler(10, 0, 0);
        LeanTween.move(this.gameObject, staticPosition, 2).setEaseOutBack();
        LeanTween.rotateX(this.gameObject, 0, 2).setEaseOutBack();
    }

}
