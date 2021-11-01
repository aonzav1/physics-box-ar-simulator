using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForceTmp : MonoBehaviour
{
    public byte calculateType;
    public float magnitude;
    public Vector3 direction;
    public string Name;
    public Transform arrow;
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(0.5f / transform.parent.localScale.x, 0.5f / transform.parent.localScale.y, 0.5f / transform.parent.localScale.z);
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    public void UpdateData(string namez, float mag, Vector3 dir)
    {
       // Debug.Log("namez update to "+mag+dir);
        UpdateName(namez);
        UpdateMagnitude(mag);
        UpdateDirection(dir);
    }

    public void UpdateMagnitude(float magnitudez)
    {
        if (magnitudez == 0)
        {
            text.text = Name;
            magnitude = magnitudez;
        }
        else
        {
            text.text = Name + " " + magnitudez.ToString("F2") + " N";
            magnitude = magnitudez;
        }
    }

    public void UpdateAll()
    {
        int mul = 1;
        if (magnitude == 0)
        {
            text.text = Name;
        }
        else
        {
            if (magnitude < 0)
            {
                mul = -1;
            }
            text.text = Name + " " + magnitude.ToString("F2") + " N";
        }
        transform.rotation = Quaternion.LookRotation(direction * mul, Vector3.up);
    }
    public void UpdateDirection(Vector3 dir)
    {
        direction = dir;
        int mul = 1;
        if(magnitude < 0)
        {
            mul = -1;
        }
        transform.rotation = Quaternion.LookRotation(direction*mul, Vector3.up);
    }

    public void UpdateName(string newName)
    {
        Name = newName;
    }
}
