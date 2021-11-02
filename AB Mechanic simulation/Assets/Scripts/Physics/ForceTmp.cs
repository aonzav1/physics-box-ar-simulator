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
    public bool isUnknown;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(0.5f / transform.parent.localScale.x, 0.5f / transform.parent.localScale.y, 0.5f / transform.parent.localScale.z);
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    public void UpdateData(string namez, float mag, Vector3 dir, bool isUnknownz)
    {
        // Debug.Log("namez update to "+mag+dir);
        isUnknown = isUnknownz;
        UpdateName(namez);
        UpdateMagnitude(mag);
        UpdateDirection(dir);
    }

    public void UpdateMagnitude(float magnitudez)
    {
        if (isUnknown)
        {
            text.text = Name;
            magnitude = magnitudez;
            ChangeVisibility(true);
        }
        else if(magnitudez == 0)
        {
            text.text = "";
            ChangeVisibility(false);
        }
        else
        {
            ChangeVisibility(true);
            if (magnitudez < 0)
                magnitudez = -magnitudez;
            text.text = Name + " " + magnitudez.ToString("F2") + " N";
            magnitude = magnitudez;
        }
    }
    public void ChangeVisibility(bool isOn)
    {
        arrow.gameObject.SetActive(isOn);
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
