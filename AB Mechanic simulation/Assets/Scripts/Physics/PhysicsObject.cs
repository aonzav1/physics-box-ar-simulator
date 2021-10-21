using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhysicsObject : MonoBehaviour
{
    public ObjectType type;
    public float[] properties;
    Rigidbody rb;
    BoxCollider boxCollider;
    public GameObject velocityLine;
    public GameObject centerofMass;
    public Text velocityMagnitude;
    public ForceVector[] forces;

    // Start is called before the first frame update
    void Start()
    {
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
                break;
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
            case 1:
                return "N";
        }
        return "";
    }
}
