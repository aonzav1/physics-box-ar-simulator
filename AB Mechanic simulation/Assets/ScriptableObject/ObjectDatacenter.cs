using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Datacenter", menuName = "ScriptableObjects/ObjectDatacenter", order = 1)]
public class ObjectDatacenter : ScriptableObject
{
    public ObjectData[] objects;
}
