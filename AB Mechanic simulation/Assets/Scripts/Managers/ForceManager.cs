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
                    CreateObjectForce("fk", dynamicFriction_1, -objectList[0].extForce_vector, ForceColor.blue, 0, unknown == 3);
                    objectList[0].UpdateFriction(false);
                }
                else
                {
                    CreateObjectForce("fs", objectList[0].externalForce, -objectList[0].extForce_vector, ForceColor.blue, 0, unknown == 3);
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
                CreateObjectForce("N1", mg_2, new Vector3(0, 1, 0), ForceColor.blue, 0, unknown == 2);

                //bottom cube
                float mg_3 = objectList[1].totalMass * -Physics.gravity.y;
                CreateObjectForce("(m1+m2)g", mg_3, new Vector3(0, -1, 0), ForceColor.pink, 1, unknown == 0);
                CreateObjectForce("N12", mg_3, new Vector3(0, 1, 0), ForceColor.blue, 1, unknown == 2);
                if (objectList[1].externalForce > 0) {
                    CreateObjectForce("F", objectList[1].externalForce, objectList[1].extForce_vector, ForceColor.red, 1, unknown == 1);
                    externalF_3 = objectList[1].externalForce;
                    externalTo = 2;
                }
                else
                {
                    CreateObjectForce("F", objectList[0].externalForce, objectList[0].extForce_vector, ForceColor.red, 0, unknown == 1);
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
                    CreateObjectForce("f(1)", friction_3, -objectList[0].extForce_vector, ForceColor.blue, 0, unknown == 3);
                    CreateObjectForce("f(1)", friction_3, objectList[0].extForce_vector, ForceColor.blue, 1, unknown == 3);
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
                    CreateObjectForce("f(2)", groundForce, -objectList[0].extForce_vector, ForceColor.red, 1, unknown == 3);
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
                    CreateObjectForce("f(1)", frictionFrom2to3, objectList[1].extForce_vector, ForceColor.blue, 0, unknown == 3);
                    CreateObjectForce("f(1)", frictionFrom2to3, -objectList[1].extForce_vector, ForceColor.blue, 1, unknown == 3);
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
                    CreateObjectForce("f(2)", groundForce, -objectList[1].extForce_vector, ForceColor.red, 1, unknown == 3);
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
            }
        }
    }
    public float CalculateNewForcesWithUnknown(int unknown)
    {
        CleanTempForces();
        float res = 0;
      /*  for (int i = 0; i < curForceList.Length; i++)
        {
            if (i != unknown)
            {
                CreateObjectForce(curForceList[i], false);
            }
            else
            {
                res = CreateObjectForce(curForceList[i], true).magnitude;
            }
        }*/
        return res;
    }

    public void CleanTempForces()
    {
        AllForce = new List<ForceTmp>();
    }

    public void TurnOffForces()
    {
        for (int i = 0; i < AllForce.Count; i++)
        {
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

    public float CalculateForceMagnitude(int num)
    {
        return 0;
     /*   switch (num)
        {
            case 0: //mg
                return rb.mass * -Physics.gravity.y;
            case 1: //N
                return rb.mass * -Physics.gravity.y;
            case 2: //N
                return totalMass * -Physics.gravity.y;
            case 3: //friction
                float normal = totalMass * -Physics.gravity.y;
                PhysicMaterial floorMatt_1 = floor.material;
                PhysicMaterial myMatt_1 = myCollider.material;
                float maxstaticF = (floorMatt_1.staticFriction + myMatt_1.staticFriction) * normal;
                Rigidbody floorRb = floor.GetComponent<Rigidbody>();
                float f_totalForce = externalForce + ext_relative;
                if (floor.GetComponent<Rigidbody>() != null)
                {
                    float acceleration = externalForce / (floorRb.mass + rb.mass);
                    f_totalForce = acceleration * rb.mass;
                }
                Debug.Log("Max static force is " + maxstaticF);
                if (f_totalForce > maxstaticF)
                {
                    Debug.Log(gameObject.name + " will move");
                    return (floorMatt_1.dynamicFriction + myMatt_1.dynamicFriction) * normal;
                }
                else
                {
                    Debug.Log(gameObject.name + " won't move");
                    return f_totalForce;
                }
            case 4: //relative friction from above
                float r_normal = (totalMass - rb.mass) * -Physics.gravity.y;
                PhysicMaterial upperMatt_2 = stacking_rb[0].GetComponent<Collider>().material;
                PhysicMaterial myMatt_2 = myCollider.material;
                float r_maxstaticF = (upperMatt_2.staticFriction + myMatt_2.staticFriction) * r_normal;
                float external = stacking_rb[0].GetComponent<PhysicsObject>().externalForce; //top box has external force
                if (external > r_maxstaticF)
                {
                    float totalforce_2 = (upperMatt_2.dynamicFriction + myMatt_2.dynamicFriction) * r_normal;
                    ext_relative = totalforce_2;
                    return totalforce_2;
                }
                else
                {
                    ext_relative = external;
                    return external;
                }
            case 5:
                return externalForce;
            case 6: //floor friction
                float r_normal2 = rb.mass * -Physics.gravity.y;
                PhysicMaterial floorMatt_3 = floor.GetComponent<Collider>().material;
                PhysicMaterial myMatt_3 = myCollider.material;
                float r_maxstaticF2 = (floorMatt_3.staticFriction + myMatt_3.staticFriction) * r_normal2;
                float external2 = floor.GetComponent<PhysicsObject>().externalForce;
                if (external2 > r_maxstaticF2)
                {
                    float totalforce = (floorMatt_3.dynamicFriction + myMatt_3.dynamicFriction) * r_normal2;
                    // ext_relative = totalforce;
                    return totalforce;
                }
                else
                {
                    // ext_relative = external2;
                    return external2;
                }
            default:
                return 0;
        }*/
    }

    public void CreateObjectForce(string namez, float mag, Vector3 dir,ForceColor color,int target_num,bool isUnkown)
    {
        GameObject force = Instantiate(datacenter.forceArrow[(byte)color],objectList[target_num].transform);
        force.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        ForceTmp forceScript = force.GetComponent<ForceTmp>();
        if (isUnkown)
            mag = 0;
        forceScript.UpdateData(namez, mag, dir);
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
