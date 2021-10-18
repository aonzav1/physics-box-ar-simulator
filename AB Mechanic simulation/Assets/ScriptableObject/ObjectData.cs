using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ObjectData", order = 1)]
public class ObjectData : ScriptableObject
{
    public string Name;
    public GameObject prefab;
    public ObjectType type;
}
