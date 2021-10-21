using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForceTmp : MonoBehaviour
{
    public byte calculateType;
    public float magnitude;
    public Transform arrow;
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //    transform.rotation = Quaternion.LookRotation(LockVector, Vector3.up);
    }

    public void UpdateMagnitude(float magnitudez, bool isAnnonymus)
    {
        if (isAnnonymus)
            text.text = name;
        else
        {
            text.text = name + " " + magnitudez.ToString("F2") + " N";
            magnitude = magnitudez;
        }
    }
}
