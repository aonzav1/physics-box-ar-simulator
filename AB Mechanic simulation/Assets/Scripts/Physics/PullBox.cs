using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PullBox : MonoBehaviour
{
    public PhysicsObject[] Targets;
    public MainWorkSpace main;
    public InputField value;
    public Text valueText;
    public GameObject curRope;
    public GameObject ropePref;
    public int pull_object;
    public float pullmagnitude;
    public Vector3 pull_vector;
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
        Pull(0);
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

    public void Pull(int num)
    {
        if(Targets.Length != main.workSpace.childCount)
        {
            FindTarget();
        }
        if(curRope != null)
        {
            Destroy(curRope);
        }
        curRope = Instantiate(ropePref,Targets[num].transform);
        pull_object = num;
        CalculateMagnitude();
        for(int i = 0; i < Targets.Length; i++)
        {
            Targets[i].externalForce = 0;
        }
        Targets[num].externalForce = pullmagnitude;
        Targets[num].extForce_vector = pull_vector;
        main.forcemanager.UpdateForces();
        MainWorkSpace.isRecalculateRequire = false;
        Debug.Log("Pull no."+num);
        Debug.Log("Recalculated");
    }

    /* public void CalculateForce()
     {
         for(int i = 0; i < Targets.Length; i++)
         {
             if (i == 0)
             {
                // Targets.
             }
             else
             {

             }
         }
     }*/
    public void CheckMagnitude()
    {
        MainWorkSpace.isRecalculateRequire = true;
    }
    public void CalculateMagnitude()
    {
        float.TryParse(value.text, out pullmagnitude);
        Debug.Log("Parsed value, result : "+pullmagnitude);
        value.text = pullmagnitude.ToString();
    }
  /*  public void PullSimulate()
    {
        isPulled = true;
        if(Targets[pull_object].rb != null)
            Targets[pull_object].rb.AddForce(pullmagnitude, 0, 0);
    }*/
}
