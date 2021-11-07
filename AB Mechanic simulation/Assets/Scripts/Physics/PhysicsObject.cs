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
    public Text MassMagnitude;
    public float externalForce;
    public Vector3 extForce_vector;
    public Vector3 pushAt;
    public Collider floor;
    public float floorStaticfriction = 0.001f;
    public float floorDynamicfriction = 0.001f;
    MainWorkSpace main;
    public ProblemGenerator probgen;
    public bool isPush;
    public bool isOnTop;


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
        if(VIsibilityController.showDetail <= 1)
        {
            if (rb.velocity.magnitude > 0.01)
            {
                velocityLine.transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
                if (!isCheck)
                {
                    isCheck = true;
                    StartCoroutine(CheckAcceleration());
                }
                velocityLine.SetActive(true);
            }
            else
            {
                velocityLine.SetActive(false);
            }
            centerofMass.SetActive(false);
        }
        else
        {
            centerofMass.SetActive(true);
            velocityLine.SetActive(false);
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
                    rb.AddForceAtPosition(extForce_vector * (externalForce),pushAt, ForceMode.Force);
                }
                else
                {
                    rb.AddForce(extForce_vector * (externalForce), ForceMode.Force);
                }
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
        velocityMagnitude.text = "loading..";
        yield return new WaitForSeconds(0.05f);
        while (isCheck)
        {
            if(VIsibilityController.showDetail == 0)
            {
                velocityMagnitude.text = rb.velocity.magnitude.ToString("F2") + " m/s";
            }
            else
            if (VIsibilityController.showDetail == 1)
            {
                float diff = rb.velocity.magnitude - speedz;
                speedz = rb.velocity.magnitude;
                if (diff > 0.001f)
                {
                    velocityMagnitude.text = (diff * 10).ToString("F2") + " m/s^2";
                    Debug.Log(gameObject.name + " has a = " + (diff * 10));
                }
            }
            yield return new WaitForSeconds(0.1f);
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
                float friction_static = properties[1] /(2*floorStaticfriction);
                if (myCollider.material == null)
                {
                    PhysicMaterial actorMaterial = new PhysicMaterial();
                    actorMaterial.staticFriction = friction_static;
                    actorMaterial.dynamicFriction = friction_static*1.2f; //change to propertie 2 in runtime
                  //  actorMaterial.dynamicFriction = properties[2];
                    actorMaterial.frictionCombine = PhysicMaterialCombine.Multiply;
                    myCollider.material = actorMaterial;
                }
                else
                {
                    myCollider.material.staticFriction = friction_static;
                    myCollider.material.dynamicFriction = friction_static*1.2f; //change to propertie 2 in runtime
                    //  myCollider.material.dynamicFriction = properties[2];
                    myCollider.material.frictionCombine = PhysicMaterialCombine.Multiply;
                }
                float allmass = 0;
                for (int i = 0; i < stacking_rb.Count; i++)
                {
                    allmass += stacking_rb[i].mass;
                }

                //stacking objects
                if (stacking_rb.Count > 0)
                {
                    PhysicsObject topBox = stacking_rb[0].transform.GetComponent<PhysicsObject>();
                    Debug.Log(gameObject.name + " assign friction to" + topBox.gameObject.name);
                    topBox.AssignFrictionFromFloor(myCollider.material.staticFriction*2, myCollider.material.dynamicFriction*2);
                }


                totalMass = rb.mass + allmass;
                UpdateMass();
                break;
            case ObjectType.staticObject:
            /*    if (myCollider.material == null)
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
                }*/
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


    public void UpdateMass()
    {
        if(MassMagnitude != null)
        MassMagnitude.text = "m = "+rb.mass+" kg";
    }

    public void AssignFrictionFromFloor(float floor_static, float floor_dynamic)
    {
        floorStaticfriction = floor_static;
        floorDynamicfriction = floor_dynamic;
        UpdateFriction(true);
    }

    public void UpdateFriction(bool isStatic)
    {
        //Debug.Log(properties[1] + "," + floorStaticfriction);
        float staticFriction = properties[1] / floorStaticfriction;
        float mul = 0.5f;
        if (isOnTop)
        {
            if (floorStaticfriction <= 1)
            {
                staticFriction = floorStaticfriction / properties[1];
            }
            mul = 1;
        }
       // myCollider.material.staticFriction = staticFriction * mul;
        Debug.Log("Static friction is "+staticFriction);
        if (isStatic)
        {
            myCollider.material.staticFriction = staticFriction * mul;
            myCollider.material.dynamicFriction = staticFriction * mul *1.2f;
            Debug.Log("(X) Dynamic friction is " + staticFriction *1.2f);
        }
        else
        {
            myCollider.material.staticFriction = staticFriction * mul;
            float dynamicFriction = properties[2] / floorDynamicfriction;
            if (isOnTop)
            {
                if (floorDynamicfriction <= 1)
                {
                    dynamicFriction = floorDynamicfriction / properties[2];
                }
                mul = 1;
            }
            myCollider.material.dynamicFriction = dynamicFriction * mul;
            Debug.Log("(O) Dynamic friction is " + dynamicFriction);
        }
    }

}

