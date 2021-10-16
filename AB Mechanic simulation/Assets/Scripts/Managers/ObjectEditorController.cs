using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEditorController : MonoBehaviour
{
    public GameObject selecting;
    VIsibilityController visible_controller;
    public Vector3 tmp_position;
    public Vector3 tmp_rotation;
    public Vector3 tmp_scale;

    private void Start()
    {
        visible_controller = GetComponent<VIsibilityController>();
    }
    public void startEditing(GameObject target)
    {
        selecting = target;
        tmp_position = target.transform.position;
        tmp_rotation = target.transform.rotation.eulerAngles;
        tmp_scale = target.transform.localScale;
    }
    public void SaveEdit()
    {
        Debug.Log("Save");
        visible_controller.DeselectAll();
    }
    public void CancelEdit()
    {
        Debug.Log("Cancel");
        selecting.transform.position = tmp_position;
        selecting.transform.rotation.eulerAngles.Set(tmp_rotation.x, tmp_rotation.y, tmp_rotation.z);
        selecting.transform.localScale = tmp_scale;
        visible_controller.DeselectAll();
    }
}
