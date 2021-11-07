using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceManager : MonoBehaviour
{
    public ObjectDatacenter datacenter;
    public Collider floor;
    public GameObject selected;
    public float forceCalculationNum;
    public Transform workSpace;
    public List<PhysicsObject> objectList = new List<PhysicsObject>();

    [Header("Memory")]
    public List<ForceTmp> AllForce = new List<ForceTmp>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ReceivedAllObjectData()
    {
        objectList = new List<PhysicsObject>();
        for(int i = 0; i < workSpace.childCount; i++)
        {
            objectList.Add(workSpace.GetChild(i).GetComponent<PhysicsObject>());
        }
    }

    public void CalculateNewForces(int cal_num, int unknown)
    {
        forceCalculationNum = cal_num;
        CleanTempForces();
        switch (forceCalculationNum)
        {
            case 0:
                //falling cube with pull
                CreateObjectForce("mg", objectList[0].rb.mass * -Physics.gravity.y, new Vector3(0, -1, 0), ForceColor.pink,0, unknown == 0);
                CreateObjectForce("F", objectList[0].externalForce, objectList[0].extForce_vector, ForceColor.red,0, unknown == 1);
                break;
            case 1:
                //cube drag
                float mg_1 = objectList[0].rb.mass * -Physics.gravity.y;
                CreateObjectForce("mg", mg_1, new Vector3(0, -1, 0), ForceColor.pink, 0, unknown == 0);
                CreateObjectForce("F", objectList[0].externalForce, objectList[0].extForce_vector, ForceColor.red, 0, unknown == 1);
                CreateObjectForce("N", mg_1, new Vector3(0,1,0), ForceColor.blue, 0, unknown == 2);
                float friction_req_1 = objectList[0].externalForce;
                float maxStatic_1 = objectList[0].properties[1] * mg_1;
                if(friction_req_1 > maxStatic_1)
                {
                    float dynamicFriction_1 = objectList[0].properties[2] * mg_1;
                    CreateObjectForce("f", dynamicFriction_1, -objectList[0].extForce_vector, ForceColor.yellow, 0, unknown == 3);
                    objectList[0].UpdateFriction(false);
                }
                else
                {
                    CreateObjectForce("f", objectList[0].externalForce, -objectList[0].extForce_vector, ForceColor.yellow, 0, unknown == 3);
                    objectList[0].UpdateFriction(true);
                }
                break;
            case 2:
                //falling cube with pull (problem)
                CreateObjectForce("Total F", 0, new Vector3(0, -1, 0), ForceColor.pink, 0, unknown == 0);
                CreateObjectForce("P", objectList[0].externalForce, objectList[0].extForce_vector, ForceColor.blue, 0, unknown == 1);
                break;
            case 3:
                //2blocks pull

                float externalF_3 = 0;
                byte externalTo = 0;

                //top cube
                float mg_2 = objectList[0].rb.mass * -Physics.gravity.y;
                CreateObjectForce("m1g", mg_2, new Vector3(0, -1, 0), ForceColor.pink, 0, unknown == 0);
                CreateObjectForce("N1", mg_2, new Vector3(0, 1, 0), ForceColor.blue, 0, unknown == 1);

                //bottom cube
                float mg_3 = objectList[1].totalMass * -Physics.gravity.y;
                CreateObjectForce("(m1+m2)g", mg_3, new Vector3(0, -1, 0), ForceColor.pink, 1, unknown == 2);
                CreateObjectForce("N12", mg_3, new Vector3(0, 1, 0), ForceColor.blue, 1, unknown == 3);
                if (objectList[1].externalForce > 0) {
                    CreateObjectForce("F", objectList[1].externalForce, objectList[1].extForce_vector, ForceColor.red, 1, unknown == 4);
                    externalF_3 = objectList[1].externalForce;
                    externalTo = 2;
                }
                else
                {
                    CreateObjectForce("F", objectList[0].externalForce, objectList[0].extForce_vector, ForceColor.red, 0, unknown == 4);
                    externalF_3 = objectList[0].externalForce;
                    externalTo = 1;
                }
                //calculate max friction each
                float maxStatic_2 = objectList[0].properties[1] * mg_2;
                float maxStatic_3 = objectList[1].properties[1] * mg_3;

                float groundDynamicFriction_3 = objectList[1].properties[2] * mg_3;
                float friction_req_2 = 0;

                float a_3 = (externalF_3 - maxStatic_3) / (objectList[1].totalMass);
                if (a_3 > 0)
                {
                    a_3 = (externalF_3 - groundDynamicFriction_3) / (objectList[1].totalMass);
                    Debug.Log("Expected a = " + a_3);
                }
                else
                {
                    a_3 = 0;
                    Debug.Log("Expected a = 0");
                }
                if (externalTo == 1) //pull upper block
                {
                    //TOP CUBE
                    friction_req_2 = externalF_3 - objectList[0].rb.mass * a_3;
                    float friction_3 = 0;
                    if (friction_req_2 > maxStatic_2) //top cube slide
                    {
                        float dynamicFriction_2 = objectList[0].properties[2] * mg_2;
                        friction_3 = dynamicFriction_2;
                        objectList[0].UpdateFriction(false);
                    }
                    else
                    {
                        friction_3 = friction_req_2;
                        objectList[0].UpdateFriction(true);
                    }
                    CreateObjectForce("f(1)", friction_3, -objectList[0].extForce_vector, ForceColor.blue, 0, unknown == 5);
                    CreateObjectForce("f(1)", friction_3, objectList[0].extForce_vector, ForceColor.blue, 1, unknown == 6);
                    //BOTTOM CUBE
                    float groundForce = friction_3;
                    if (groundForce > maxStatic_3)
                    {
                        groundForce = groundDynamicFriction_3;
                        objectList[1].UpdateFriction(false);
                    }
                    else
                    {
                        objectList[1].UpdateFriction(true);
                    }
                    CreateObjectForce("f(2)", groundForce, -objectList[0].extForce_vector, ForceColor.red, 1, unknown == 7);
                }
                else if (externalTo == 2) //pull lower block
                {
                    //TOP CUBE
                    friction_req_2 = objectList[0].rb.mass * a_3;
                    float frictionFrom2to3 = 0;
                    if (friction_req_2 > maxStatic_2) //top cube slide
                    {
                        float dynamicFriction_2 = objectList[0].properties[2] * mg_2;
                        //relative friction to bottom cube
                        frictionFrom2to3 = dynamicFriction_2;
                        objectList[0].UpdateFriction(false);
                    }
                    else //no slide
                    {
                        //relative friction to bottom cube
                        frictionFrom2to3 = friction_req_2;
                        objectList[0].UpdateFriction(true);
                    }
                    CreateObjectForce("f(1)", frictionFrom2to3, objectList[1].extForce_vector, ForceColor.blue, 0, unknown == 5);
                    CreateObjectForce("f(1)", frictionFrom2to3, -objectList[1].extForce_vector, ForceColor.blue, 1, unknown == 6);
                    float groundForce = objectList[1].externalForce + frictionFrom2to3 - objectList[1].totalMass * a_3;
                    if (groundForce > maxStatic_3)
                    {
                        groundForce = groundDynamicFriction_3;
                        objectList[1].UpdateFriction(false);
                    }
                    else
                    {
                        objectList[1].UpdateFriction(true);
                    }
                    //BOTTOM CUBE
                    CreateObjectForce("f(2)", groundForce, -objectList[1].extForce_vector, ForceColor.red, 1, unknown == 7);
                }
                break;
            case 4:
                //cube drag slant
                float mg_4 = objectList[0].rb.mass * -Physics.gravity.y;
                float angle = Mathf.Atan(objectList[0].extForce_vector.y / objectList[0].extForce_vector.x); ;
                float normal_4 = mg_4*Mathf.Cos(angle);
                CreateObjectForce("mg", mg_4, new Vector3(0, -1, 0), ForceColor.pink, 0, unknown == 0);
                CreateObjectForce("F", objectList[0].externalForce, objectList[0].extForce_vector, ForceColor.red, 0, unknown == 1);
                CreateObjectForce("N", normal_4, new Vector3(-objectList[0].extForce_vector.y, objectList[0].extForce_vector.x, 0), ForceColor.blue, 0, unknown == 2);
                float pulling_mg = mg_4 * Mathf.Sin(angle);
                float friction_req_4 = objectList[0].externalForce - pulling_mg;
                float maxStatic_4 = objectList[0].properties[1] * normal_4;
                float dynamicFriction_4 = objectList[0].properties[2] * normal_4;
                if (friction_req_4 > maxStatic_4)
                {
                    CreateObjectForce("f", dynamicFriction_4, -objectList[0].extForce_vector, ForceColor.yellow, 0, unknown == 3);
                    objectList[0].UpdateFriction(false);
                }
                else if (friction_req_4 < -maxStatic_4)
                {
                    CreateObjectForce("f", dynamicFriction_4, objectList[0].extForce_vector, ForceColor.yellow, 0, unknown == 3);
                    objectList[0].UpdateFriction(false);
                }
                else
                {
                    CreateObjectForce("f", friction_req_4, -objectList[0].extForce_vector, ForceColor.yellow, 0, unknown == 3);
                    objectList[0].UpdateFriction(true);
                }
                break;
            case 5:
                //cabinet flip
                float mg_5 = objectList[0].rb.mass * -Physics.gravity.y;
                CreateObjectForce("mg", mg_5, new Vector3(0, -1, 0), ForceColor.pink, 0, unknown == 0, new Vector3(0,1,0));
                CreateObjectForce("F", objectList[0].externalForce, objectList[0].extForce_vector, ForceColor.red, 0, unknown == 1, new Vector3(0,1,0));
                float normal_5 = mg_5 / 4;
                CreateObjectForce("N1", normal_5, new Vector3(0, 1, 0), ForceColor.blue, 0, unknown == 2, new Vector3(0.297f,0.03f, 0.32f));
                CreateObjectForce("N2", normal_5, new Vector3(0, 1, 0), ForceColor.blue, 0, unknown == 3, new Vector3(0.297f, 0.03f, -0.32f));
                CreateObjectForce("N3", normal_5, new Vector3(0, 1, 0), ForceColor.blue, 0, unknown == 4, new Vector3(-0.297f, 0.03f, 0.32f));
                CreateObjectForce("N4", normal_5, new Vector3(0, 1, 0), ForceColor.blue, 0, unknown == 5, new Vector3(-0.297f, 0.03f, -0.32f));
                float friction_req_5 = objectList[0].externalForce;
                float maxStatic_5 = objectList[0].properties[1] * mg_5;
                if (friction_req_5 > maxStatic_5)
                {
                    float dynamicFriction_5 = objectList[0].properties[2] * normal_5;
                    CreateObjectForce("f", dynamicFriction_5, -objectList[0].extForce_vector, ForceColor.yellow, 0, unknown == 6, new Vector3(0.297f, 0.03f, 0.32f));
                    CreateObjectForce("f", dynamicFriction_5, -objectList[0].extForce_vector, ForceColor.yellow, 0, unknown == 7, new Vector3(0.297f, 0.03f, -0.32f));
                    CreateObjectForce("f", dynamicFriction_5, -objectList[0].extForce_vector, ForceColor.yellow, 0, unknown == 8, new Vector3(-0.297f, 0.03f, 0.32f));
                    CreateObjectForce("f", dynamicFriction_5, -objectList[0].extForce_vector, ForceColor.yellow, 0, unknown == 9, new Vector3(-0.297f, 0.03f, -0.32f));
                    objectList[0].UpdateFriction(false);
                }
                else
                {
                    float distributedForce = objectList[0].externalForce / 4;
                    CreateObjectForce("f", distributedForce, -objectList[0].extForce_vector, ForceColor.yellow, 0, unknown == 6, new Vector3(0.297f, 0.03f, 0.32f));
                    CreateObjectForce("f", distributedForce, -objectList[0].extForce_vector, ForceColor.yellow, 0, unknown == 7, new Vector3(0.297f, 0.03f, -0.32f));
                    CreateObjectForce("f", distributedForce, -objectList[0].extForce_vector, ForceColor.yellow, 0, unknown == 8, new Vector3(-0.297f, 0.03f, 0.32f));
                    CreateObjectForce("f", distributedForce, -objectList[0].extForce_vector, ForceColor.yellow, 0, unknown == 9, new Vector3(-0.297f, 0.03f, -0.32f));
                    objectList[0].UpdateFriction(true);
                }
                break;
        }
      /*  for (int i = 0; i < curForceList.Length; i++)
        {
            CreateObjectForce(curForceList[i], false);
        }*/
    }
    public void UpdateForces()
    {
        if (AllForce != null && AllForce.Count > 0)
        {
            switch (forceCalculationNum)
            {
                case 0:
                    //only falling cube
                    UpdateObjectForce(AllForce[0], objectList[0].rb.mass * -Physics.gravity.y, new Vector3(0, -1, 0));
                    UpdateObjectForce(AllForce[1], objectList[0].externalForce, objectList[0].extForce_vector);
                    break;
                case 1:
                    //cube drag
                    float mg_1 = objectList[0].rb.mass * -Physics.gravity.y;
                    UpdateObjectForce(AllForce[0], mg_1, new Vector3(0, -1, 0));
                    UpdateObjectForce(AllForce[1], objectList[0].externalForce, objectList[0].extForce_vector);
                    UpdateObjectForce(AllForce[2], mg_1, new Vector3(0, 1, 0));
                    float friction_req_1 = objectList[0].externalForce;
                    float maxStatic_1 = objectList[0].properties[1] * mg_1;
                    if (friction_req_1 > maxStatic_1)
                    {
                        float dynamicFriction_1 = objectList[0].properties[2] * mg_1;
                        UpdateObjectForce(AllForce[3], dynamicFriction_1, -objectList[0].extForce_vector);
                        objectList[0].UpdateFriction(false);
                    }
                    else
                    {
                        UpdateObjectForce(AllForce[3], objectList[0].externalForce, -objectList[0].extForce_vector);
                        objectList[0].UpdateFriction(true);
                    }
                    break;

                case 3:
                    //2blocks pull

                    float externalF_3 = 0;
                    byte externalTo = 0;

                    //top cube
                    float mg_2 = objectList[0].rb.mass * -Physics.gravity.y;
                    UpdateObjectForce(AllForce[0], mg_2, new Vector3(0, -1, 0));
                    UpdateObjectForce(AllForce[1], mg_2, new Vector3(0, 1, 0));

                    //bottom cube
                    float mg_3 = objectList[1].totalMass * -Physics.gravity.y;
                    UpdateObjectForce(AllForce[2], mg_3, new Vector3(0, -1, 0));
                    UpdateObjectForce(AllForce[3], mg_3, new Vector3(0, 1, 0));
                    if (objectList[0].externalForce > 0)
                    {
                        AllForce[4].transform.parent = objectList[0].transform;
                        AllForce[4].transform.position = new Vector3(AllForce[4].transform.position.x, objectList[0].transform.position.y, AllForce[4].transform.position.z);
                        UpdateObjectForce(AllForce[4], objectList[0].externalForce, objectList[0].extForce_vector);
                        externalF_3 = objectList[0].externalForce;
                        externalTo = 1;
                    }
                    if (objectList[1].externalForce > 0)
                    {
                        AllForce[4].transform.parent = objectList[1].transform;
                        AllForce[4].transform.position = new Vector3(AllForce[4].transform.position.x, objectList[1].transform.position.y, AllForce[4].transform.position.z);
                        UpdateObjectForce(AllForce[4], objectList[1].externalForce, objectList[1].extForce_vector);
                        externalF_3 = objectList[1].externalForce;
                        externalTo = 2;
                    }
                    //calculate max friction each
                    float maxStatic_2 = objectList[0].properties[1] * mg_2;
                    float maxStatic_3 = objectList[1].properties[1] * mg_3;

                    float groundDynamicFriction_3 = objectList[1].properties[2] * mg_3;
                    float friction_req_2 = 0;

                    float a_3 = (externalF_3 - maxStatic_3) / (objectList[1].totalMass);
                    if (a_3 > 0)
                    {
                        a_3 = (externalF_3 - groundDynamicFriction_3) / (objectList[1].totalMass);
                    }
                    else
                    {
                        a_3 = 0;
                    }
                    Debug.Log("Expected a = " + a_3);
                    if (externalTo == 1) //pull upper block
                    {
                        //TOP CUBE
                        friction_req_2 = externalF_3 - objectList[0].rb.mass * a_3;
                        float friction_3 = 0;
                        if (friction_req_2 > maxStatic_2) //top cube slide
                        {
                            float dynamicFriction_2 = objectList[0].properties[2] * mg_2;
                            friction_3 = dynamicFriction_2;
                            objectList[0].UpdateFriction(false);
                        }
                        else
                        {
                            friction_3 = friction_req_2;
                            objectList[0].UpdateFriction(true);
                        }
                        UpdateObjectForce(AllForce[5], friction_3, -objectList[0].extForce_vector);
                        UpdateObjectForce(AllForce[6], friction_3, objectList[0].extForce_vector);
                        //BOTTOM CUBE
                        float groundForce = friction_3;
                        if (groundForce > maxStatic_3)
                        {
                            groundForce = groundDynamicFriction_3;
                            objectList[1].UpdateFriction(false);
                        }
                        else
                        {
                            objectList[1].UpdateFriction(true);
                        }
                        UpdateObjectForce(AllForce[7], groundForce, -objectList[0].extForce_vector);
                    }
                    else if (externalTo == 2) //pull lower block
                    {
                        //TOP CUBE
                        friction_req_2 = objectList[0].rb.mass * a_3;
                        float frictionFrom2to3 = 0;
                        if (friction_req_2 > maxStatic_2) //top cube slide
                        {
                            float dynamicFriction_2 = objectList[0].properties[2] * mg_2;
                            //relative friction to bottom cube
                            frictionFrom2to3 = dynamicFriction_2;
                            objectList[0].UpdateFriction(false);
                        }
                        else //no slide
                        {
                            //relative friction to bottom cube
                            frictionFrom2to3 = friction_req_2;
                            objectList[0].UpdateFriction(true);
                        }
                        UpdateObjectForce(AllForce[5], frictionFrom2to3, objectList[1].extForce_vector);
                        UpdateObjectForce(AllForce[6], frictionFrom2to3, -objectList[1].extForce_vector);
                        //BOTTOM CUBE
                        float groundForce = objectList[1].externalForce;
                        
                        if (groundForce > maxStatic_3)
                        {
                            groundForce = groundDynamicFriction_3;
                            objectList[1].UpdateFriction(false);
                        }
                        else
                        {
                            objectList[1].UpdateFriction(true);
                        }
                        UpdateObjectForce(AllForce[7], groundForce, -objectList[1].extForce_vector);
                    }
                    break;
                case 4:
                    //cube drag slant
                    float mg_4 = objectList[0].rb.mass * -Physics.gravity.y;
                    float angle = Mathf.Atan(objectList[0].extForce_vector.y / objectList[0].extForce_vector.x); ;
                    float normal_4 = mg_4 * Mathf.Cos(angle);
                    UpdateObjectForce(AllForce[0], mg_4, new Vector3(0, -1, 0));
                    UpdateObjectForce(AllForce[1], objectList[0].externalForce, objectList[0].extForce_vector);
                    UpdateObjectForce(AllForce[2], normal_4, new Vector3(-objectList[0].extForce_vector.y, objectList[0].extForce_vector.x, 0));
                    float pulling_mg = mg_4 * Mathf.Sin(angle);
                    float friction_req_4 = objectList[0].externalForce - pulling_mg;
                    float maxStatic_4 = objectList[0].properties[1] * normal_4;
                    float dynamicFriction_4 = objectList[0].properties[2] * normal_4;
                    if (friction_req_4 > maxStatic_4)
                    {
                        UpdateObjectForce(AllForce[3], dynamicFriction_4, -objectList[0].extForce_vector);
                        objectList[0].UpdateFriction(false);
                    }else if(friction_req_4 < -maxStatic_4)
                    {
                        UpdateObjectForce(AllForce[3], dynamicFriction_4, objectList[0].extForce_vector);
                        objectList[0].UpdateFriction(false);
                    }
                    else
                    {
                        UpdateObjectForce(AllForce[3], friction_req_4, -objectList[0].extForce_vector);
                        objectList[0].UpdateFriction(true);
                    }
                    break;
                case 5:
                    //cabinet flip
                    float mg_5 = objectList[0].rb.mass * -Physics.gravity.y;
                    UpdateObjectForce(AllForce[0], mg_5, new Vector3(0, -1, 0));
                    UpdateObjectForce(AllForce[1], objectList[0].externalForce, objectList[0].extForce_vector);
                    float normal_5 = mg_5 / 4;
                    UpdateObjectForce(AllForce[2], normal_5, new Vector3(0, 1, 0));
                    UpdateObjectForce(AllForce[3], normal_5, new Vector3(0, 1, 0));
                    UpdateObjectForce(AllForce[4], normal_5, new Vector3(0, 1, 0));
                    UpdateObjectForce(AllForce[5], normal_5, new Vector3(0, 1, 0));
                    float friction_req_5 = objectList[0].externalForce;
                    float maxStatic_5 = objectList[0].properties[1] * mg_5;
                    if (friction_req_5 > maxStatic_5)
                    {
                        float dynamicFriction_5 = objectList[0].properties[2] * normal_5;
                        UpdateObjectForce(AllForce[6], dynamicFriction_5, -objectList[0].extForce_vector);
                        UpdateObjectForce(AllForce[7], dynamicFriction_5, -objectList[0].extForce_vector);
                        UpdateObjectForce(AllForce[8], dynamicFriction_5, -objectList[0].extForce_vector);
                        UpdateObjectForce(AllForce[9], dynamicFriction_5, -objectList[0].extForce_vector);
                        objectList[0].UpdateFriction(false);
                    }
                    else
                    {
                        float distributedForce = objectList[0].externalForce / 4;
                        UpdateObjectForce(AllForce[6], distributedForce, -objectList[0].extForce_vector);
                        UpdateObjectForce(AllForce[7], distributedForce, -objectList[0].extForce_vector);
                        UpdateObjectForce(AllForce[8], distributedForce, -objectList[0].extForce_vector);
                        UpdateObjectForce(AllForce[9], distributedForce, -objectList[0].extForce_vector);
                        objectList[0].UpdateFriction(true);
                    }
                    break;
            }
            Debug.Log("Update forces with "+ forceCalculationNum+" successfully");
        }
    }
    public void CleanTempForces()
    {
        AllForce = new List<ForceTmp>();
    }

    public void TurnOffForces()
    {
        for (int i = 0; i < AllForce.Count; i++)
        {
            if(AllForce[i] != null)
                AllForce[i].gameObject.SetActive(false);
        }
    }
    public void TurnOnForces()
    {
        if (selected != null)
        {
            for (int i = 0; i < AllForce.Count; i++)
            {
                if (AllForce[i].transform.parent.gameObject == selected)
                    AllForce[i].gameObject.SetActive(true);
            }
        }
    }
    public void TurnOnForceNum(int num)
    {
        AllForce[num].gameObject.SetActive(true);
    }

    public void CreateObjectForce(string namez, float mag, Vector3 dir,ForceColor color,int target_num,bool isUnkown,Vector3 offset=default(Vector3))
    {
        GameObject force = Instantiate(datacenter.forceArrow[(byte)color],objectList[target_num].transform);
        force.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        force.transform.localPosition = offset;
        ForceTmp forceScript = force.GetComponent<ForceTmp>();
        forceScript.UpdateData(namez, mag, dir, isUnkown);
        AllForce.Add(forceScript);
        force.gameObject.SetActive(false);
        //return forceScript;
    }
    public void UpdateObjectForce(ForceTmp target,float mag, Vector3 dir)
    {
        target.UpdateMagnitude(mag);
        target.UpdateDirection(dir);
    }

}
