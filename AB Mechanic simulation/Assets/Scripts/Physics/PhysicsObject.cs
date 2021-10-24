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
    public BoxCollider boxCollider;
    public GameObject velocityLine;
    public GameObject centerofMass;
    public Text velocityMagnitude;
    public ForceVector[] forces;
    public float externalForce;
    public Collider floor;
    MainWorkSpace main;
    public ProblemGenerator probgen;

    [Header("Memory")]
    public List<ForceTmp> AllForce = new List<ForceTmp>();

    // Start is called before the first frame update
    void Start()
    {
        main = FindObjectOfType<MainWorkSpace>();
        switch (type)
        {
            case ObjectType.Box:
                rb = GetComponent<Rigidbody>();
                boxCollider = GetComponent<BoxCollider>();

                centerofMass.transform.localPosition = rb.centerOfMass;
                break;
        }
        LoadProperties();
    }
    private void Update()
    {
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
                if (boxCollider.material == null)
                {
                    PhysicMaterial actorMaterial = new PhysicMaterial();
                    actorMaterial.staticFriction = properties[1];
                    actorMaterial.dynamicFriction = properties[2];
                    boxCollider.material = actorMaterial;
                }
                else
                {
                    boxCollider.material.staticFriction = properties[1];
                    boxCollider.material.dynamicFriction = properties[2];
                }
                float allmass = 0;
                for (int i = 0; i < stacking_rb.Count; i++)
                {
                    allmass += stacking_rb[i].mass;
                }
                totalMass = rb.mass + allmass;
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
        for (int i = 0; i < forces.Length; i++)
        {
            CreateObjectForce(forces[i], false);
        }
    }
    public float CalculateNewForcesWithUnknown(int unknown)
    {
        CleanTempForces();

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
                PhysicMaterial myMatt_1 = boxCollider.material;
                float maxstaticF = (floorMatt_1.staticFriction + myMatt_1.staticFriction) / 2 * normal;
                if (externalForce > maxstaticF)
                {
                    return (floorMatt_1.dynamicFriction + myMatt_1.dynamicFriction) / 2 * normal;
                }
                else
                {
                    return externalForce;
                }
            case 4: //relative friction
                float r_normal = (totalMass - rb.mass) * -Physics.gravity.y;
                PhysicMaterial upperMatt_2 = stacking_rb[0].GetComponent<Collider>().material;
                PhysicMaterial myMatt_2 = boxCollider.material;
                float r_maxstaticF = (upperMatt_2.staticFriction + myMatt_2.staticFriction) / 2 * r_normal;
                if (externalForce > r_maxstaticF)
                {

                    return -(upperMatt_2.dynamicFriction + myMatt_2.dynamicFriction) / 2 * r_normal;
                }
                else
                {
                    return -externalForce;
                }
            case 5:
                return externalForce;
            default:
                return 0;
        }
    }

    public ForceTmp CreateObjectForce(ForceVector force_vector, bool isUnkown)
    {
        GameObject force = Instantiate(main.datacenter.forceArrow[(byte)force_vector.color], transform);
        force.name = ForceVector.GetName(force_vector.force_num);
        force.transform.rotation = Quaternion.LookRotation(force_vector.direction, Vector3.up);
        ForceTmp forceScript = force.GetComponent<ForceTmp>();
        float magnitude = CalculateForceMagnitude(force_vector.force_num);
        if(magnitude == 0)
        {
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
        }
        return "";
    }
}
