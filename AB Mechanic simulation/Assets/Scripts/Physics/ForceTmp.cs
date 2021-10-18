using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForceTmp : MonoBehaviour
{
    public Transform arrow;
    public bool isLockVector;
    public Vector3 LockVector;
    public bool isNormal;
    public PhysicsObject target;
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isLockVector)
        {
            transform.rotation = Quaternion.LookRotation(LockVector, Vector3.up);
        }
        else if(isNormal)
        {
            transform.rotation = Quaternion.LookRotation(target.normalVector, Vector3.up);
        }
    }

    public void UpdateMagnitude(float magnitude)
    {
        text.text = name + " " + magnitude.ToString("F2") + " m/s^2";
    }
}
