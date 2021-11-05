using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PushAtPoint : MonoBehaviour
{
    public PhysicsObject[] Targets;
    public float targetHeight;
    public MainWorkSpace main;
    public InputField value;
    public Text valueText;
    public GameObject curRope;
    public GameObject ropePref;
    public Slider positon_selector;
    public Text position_txt;
    public int push_object;
    public float pushmagnitude;
    public Vector3 push_vector;
    public Vector3 push_at;
    // Start is called before the first frame update
    void Start()
    {
        main = FindObjectOfType<MainWorkSpace>();
        StartCoroutine(DelayPull());
    }
    IEnumerator DelayPull()
    {
        yield return new WaitForSeconds(0.2f);
        FindTarget();
        positon_selector.maxValue = targetHeight;
        positon_selector.minValue = 0;
        Push(0);
        CalculatePosition();
    }
    void FindTarget()
    {
        Targets = new PhysicsObject[main.workSpace.childCount];
        for (int i = 0; i < main.workSpace.childCount; i++)
        {
            Targets[i] = main.workSpace.GetChild(i).GetComponent<PhysicsObject>();
        }
    }

    private void Update()
    {
        //        valueText.text = val_slider.value.ToString("F2") + "N";

        /* if (MainWorkSpace.isSimulate)
         {
             if (!isPulled)
             {
                 PullSimulate();
             }
         }
         else
         {
             isPulled = false;
         }*/
    }

    public void Push(int num)
    {
        if (Targets.Length != main.workSpace.childCount)
        {
            FindTarget();
        }
        if (curRope != null)
        {
            Destroy(curRope);
        }
        curRope = Instantiate(ropePref, Targets[num].transform);
        push_object = num;
        CalculateMagnitude();
        for (int i = 0; i < Targets.Length; i++)
        {
            Targets[i].externalForce = 0;
            Targets[i].pushAt = Vector3.zero;
        }
        if (push_at != Vector3.zero)
        {
            curRope.transform.position = push_at + new Vector3(-0.5f, 0, 0);
        }
        Targets[num].externalForce = pushmagnitude;
        Targets[num].extForce_vector = push_vector;
        Targets[num].pushAt = push_at;
        MainWorkSpace.isRecalculateRequire = false;
        Debug.Log("Recalculated");
    }

    public void CheckMagnitude()
    {
        MainWorkSpace.isRecalculateRequire = true;
    }

    public void CalculateMagnitude()
    {
        float.TryParse(value.text, out pushmagnitude);
        Debug.Log("Parsed value, result : " + pushmagnitude);
        value.text = pushmagnitude.ToString();
    }

    public void CalculatePosition()
    {
        MainWorkSpace.isRecalculateRequire = true;
        Vector3 localPush = new Vector3(-0.5f,positon_selector.value,0);
        push_at = Targets[push_object].transform.TransformPoint(localPush);
        position_txt.text = "At height: " + positon_selector.value.ToString("F2");
        curRope.transform.position = push_at + new Vector3(-0.5f, 0, 0);
    }
    /*  public void PullSimulate()
      {
          isPulled = true;
          if(Targets[pull_object].rb != null)
              Targets[pull_object].rb.AddForce(pullmagnitude, 0, 0);
      }*/
}
