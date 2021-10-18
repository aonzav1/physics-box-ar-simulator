using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public int objectID;
    public int refID;
    public ObjectType type;
    public float[] properties;
    Rigidbody rb;
    BoxCollider boxCollider;
    public GameObject velocityLine;

    // Start is called before the first frame update
    void Start()
    {
        switch (type)
        {
            case ObjectType.Box:
                rb = GetComponent<Rigidbody>();
                boxCollider = GetComponent<BoxCollider>();
              /*  MeshRenderer renderer = GetComponent<MeshRenderer>();
                renderer.sortingLayerName = "Default";
                renderer.sortingOrder = 0;*/
                //should be set when instantiate
                properties = new float[3];
                properties[0] = 1; //mass
                properties[1] = 1; //static friction coefficient
                properties[2] = 1; //dynamic friction coefficient
                break;
        }
        LoadProperties();
    }
    private void Update()
    {
        if (VIsibilityController.showVelocity)
        {
            if (rb.velocity.magnitude > 0.01)
            {
                velocityLine.transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
                velocityLine.SetActive(true);
            }
            else
            {
                velocityLine.SetActive(false);
            }
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
                break;
        }
    }
}
