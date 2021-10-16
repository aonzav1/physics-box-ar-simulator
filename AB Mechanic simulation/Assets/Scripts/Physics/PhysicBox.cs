using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicBox : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody box;
    void Start()
    {
        box = GetComponent<Rigidbody>();
    }

    // Update is called once per frame

    public void PullBox()
    {
        box.AddForce(new Vector3(0, 0, 250));
    }
}