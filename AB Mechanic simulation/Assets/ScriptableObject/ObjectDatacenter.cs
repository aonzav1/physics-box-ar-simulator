using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Datacenter", menuName = "ScriptableObjects/ObjectDatacenter", order = 1)]
public class ObjectDatacenter : ScriptableObject
{
    public ObjectData[] objects;
    public GameObject[] forceArrow;
    public Problem[] questions;
    public Material velocity_arrow;
    public Material acceleration_arrow;
}

public enum QuestionUnknownType:byte {none,fall,force,frictionCo,velocity,weight}

[System.Serializable]
public class Problem
{
    public string question;
    public ObjectData data;
    public int targetobject;
    public int unknown_force;
    public Vector3 unknown_vector;
 //   public float external_force;
    public Vector3 externalVector;
    public QuestionUnknownType type;
    public bool isReceiveInput;
}
