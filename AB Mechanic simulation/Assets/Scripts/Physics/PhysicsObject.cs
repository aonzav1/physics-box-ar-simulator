using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhysicsObject : MonoBehaviour
{
    public ObjectType type;
    public float[] properties;
    public int[] stacking_object;
    public List<Rigidbody> stacking_rb;
    public float totalMass = 0;
    public Rigidbody rb;
    public Collider myCollider;
    public GameObject velocityLine;
    public GameObject centerofMass;
    public Text velocityMagnitude;
    public ForceVector[] forces;
    public float externalForce;
    public float ext_relative;
    public float totalForce;
    public Vector3 extForce_vector;
    public Vector3 pushAt;
    public Collider floor;
    MainWorkSpace main;
    public ProblemGenerator probgen;
    public bool isPush;

    [Header("Memory")]
    public List<ForceTmp> AllForce = new List<ForceTmp>();

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    void Setup()
    {
        main = FindObjectOfType<MainWorkSpace>();
        switch (type)
        {
            case ObjectType.Box:
                rb = GetComponent<Rigidbody>();
                myCollider = GetComponent<BoxCollider>();
                if(myCollider == null)
                    myCollider = transform.GetChild(0).GetComponent<BoxCollider>();

                centerofMass.transform.localPosition = rb.centerOfMass;
                break;
            case ObjectType.staticObject:
                myCollider = GetComponent<Collider>();
                break;
        }
        LoadProperties();
    }
    private void Update()
    {
        if (type == ObjectType.staticObject)
            return;
        if (VIsibilityController.showVelocity)
        {
            centerofMass.SetActive(true);
           // if (Time.timeScale > 0)
        //    {
                if (rb.velocity.magnitude > 0.01)
                {
                    velocityLine.transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
                    velocityMagnitude.text = rb.velocity.magnitude.ToString("F2") + " m/s";
                    velocityLine.SetActive(true);
                }
                else
                {
                    velocityLine.SetActive(false);
                }
          //  }
        }
        else
        {
            velocityLine.SetActive(false);
            centerofMass.SetActive(false);
        }
    }
    float speedz;
    bool isCheck;
    private void FixedUpdate()
    {
        if (type == ObjectType.staticObject)
            return;
        if (MainWorkSpace.isSimulate)
        {
            if (externalForce > 0 )
            {
                if (pushAt != Vector3.zero)
                {
                   // Debug.Log("push at "+pushAt);
                    rb.AddForceAtPosition(extForce_vector * (externalForce),pushAt, ForceMode.Force);
                }
                else
                {
                    rb.AddForce(extForce_vector * (externalForce), ForceMode.Force);
                }
            }
            if (!isCheck)
            {
                isCheck = true;
                StartCoroutine(CheckAcceleration());
            }
        }
        else
        {
            isCheck = false;
      //      isPush = false;
        }

    }
    IEnumerator CheckAcceleration()
    {
        speedz = 0;
        while (isCheck)
        {
            yield return new WaitForSeconds(0.1f);
            float diff = rb.velocity.magnitude - speedz;
            speedz = rb.velocity.magnitude;
            if(diff > 0.01f)
                Debug.Log(gameObject.name + " has a = "+(diff*10));
        }
    }
    public void NewProperties(float[] newdata)
    {
        properties = newdata;
        LoadProperties();
    }
    public void LoadProperties()
    {
        switch (type)
        {
            case ObjectType.Box:
                rb.mass = properties[0];
                if (myCollider.material == null)
                {
                    PhysicMaterial actorMaterial = new PhysicMaterial();
                    actorMaterial.staticFriction = properties[1]/2;
                    actorMaterial.dynamicFriction = properties[2]/2;
                    actorMaterial.frictionCombine = 0;
                    myCollider.material = actorMaterial;
                }
                else
                {
                    myCollider.material.staticFriction = properties[1]/2;
                    myCollider.material.dynamicFriction = properties[2]/2;
                    myCollider.material.frictionCombine = 0;
                }
                float allmass = 0;
                for (int i = 0; i < stacking_rb.Count; i++)
                {
                    allmass += stacking_rb[i].mass;
                }
                totalMass = rb.mass + allmass;
                break;
            case ObjectType.staticObject:
                if (myCollider.material == null)
                {
                    PhysicMaterial actorMaterial = new PhysicMaterial();
                    actorMaterial.staticFriction = properties[0] / 2;
                    actorMaterial.dynamicFriction = properties[1] / 2;
                    actorMaterial.frictionCombine = 0;
                    myCollider.material = actorMaterial;
                }
                else
                {
                    myCollider.material.staticFriction = properties[0] / 2;
                    myCollider.material.dynamicFriction = properties[1] / 2;
                    myCollider.material.frictionCombine = 0;
                }
                break;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name); 
        if(floor == null)
        {
            if(collision.gameObject.transform.position.y < transform.position.y)
            {
                floor = collision.gameObject.GetComponent<Collider>();
            }
            if (probgen != null)
            {
                probgen.objectHit();
            }
        }
    }

    public void CalculateNewForces()
    {
        CleanTempForces();
        ext_relative = 0;
        totalForce = 0;
        for (int i = 0; i < forces.Length; i++)
        {
            CreateObjectForce(forces[i], false);
        }
    }
    public float CalculateNewForcesWithUnknown(int unknown)
    {
        CleanTempForces();
        ext_relative = 0;
        totalForce = 0;
        float res = 0;
        for (int i = 0; i < forces.Length; i++)
        {
            if (i != unknown)
            {
                CreateObjectForce(forces[i], false);
            }
            else
            {
                res = CreateObjectForce(forces[i], true).magnitude;
            }
        }
        return res;
    }

    public void CleanTempForces()
    {
        for (int i = 0; i < AllForce.Count; i++)
        {
            Destroy(AllForce[i].gameObject);
        }
        AllForce = new List<ForceTmp>();
    }

    public void ChangeForce(int num)
    {
        float magnitude = CalculateForceMagnitude(AllForce[num].calculateType);
        AllForce[num].UpdateMagnitude(magnitude, false);
    }

    public float CalculateForceMagnitude(int num)
    {
        switch (num)
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
                Debug.Log("Max static force is "+maxstaticF);
                if (f_totalForce > maxstaticF)
                {
                    Debug.Log(gameObject.name+" will move");
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
                float r_maxstaticF = (upperMatt_2.staticFriction + myMatt_2.staticFriction)  * r_normal;
                float external = stacking_rb[0].GetComponent<PhysicsObject>().externalForce; //top box has external force
                if (external > r_maxstaticF)
                {
                    float totalforce_2 = (upperMatt_2.dynamicFriction + myMatt_2.dynamicFriction) * r_normal;
                    ext_relative = totalforce_2;
                    return totalforce_2;
                }
                else
                {
                    ext_relative = external ;
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
        }
    }

    public ForceTmp CreateObjectForce(ForceVector force_vector, bool isUnkown)
    {
        if(main == null)
        {
            Setup();
        }
        GameObject force = Instantiate(main.datacenter.forceArrow[(byte)force_vector.color], transform);
        force.name = ForceVector.GetName(force_vector.force_num);
        force.transform.rotation = Quaternion.LookRotation(force_vector.direction, Vector3.up);
        ForceTmp forceScript = force.GetComponent<ForceTmp>();
        float magnitude = CalculateForceMagnitude(force_vector.force_num);
        if(magnitude == 0)
        {
            Debug.Log("Vector 0");
            Destroy(force);
            return null;
        }
        forceScript.calculateType = force_vector.force_num;
        if (magnitude < 0)
        {
            magnitude = -magnitude;
            force.transform.rotation = Quaternion.LookRotation(-force_vector.direction, Vector3.up);
            //forceScript.arrow.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        forceScript.UpdateMagnitude(magnitude, isUnkown);
        AllForce.Add(forceScript);
        return forceScript;
    }

    public void UpdateForces()
    {
        ext_relative = 0;
        totalForce = 0;
        for (int i = 0; i < AllForce.Count; i++)
        {
            ChangeForce(i);
        }
    }

}

[System.Serializable]
public class ForceVector
{
    public Vector3 direction;
    public byte force_num;
    public ForceColor color;

    public static string GetName(int num)
    {
        switch (num)
        {
            case 0:
                return "mg";
            case 1: //one object
                return "N";
            case 2:  //group
                return "N";
            case 3:  //friction
                return "friction";
            case 4:  //relative friction
                return "friction(r)";
            case 5:
                return "P";
            case 6:  //relative friction
                return "friction(r)";
        }
        return "";
    }
}
