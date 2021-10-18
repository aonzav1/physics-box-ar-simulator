using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera mainCam;
    private void Start()
    {
        mainCam = Camera.main;
    }
    void Update()
    {
        transform.LookAt(mainCam.transform);
    }
}
