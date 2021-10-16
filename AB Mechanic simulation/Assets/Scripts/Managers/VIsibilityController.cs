using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VIsibilityController : MonoBehaviour
{
    public SelectableObject selectingObject;
    public ObjectEditorController objEditor;
    public MenuController menu_controller;
    // Start is called before the first frame update
    void Start()
    {
        objEditor = GetComponent<ObjectEditorController>();
        menu_controller = GetComponent<MenuController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectingObject == null && Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
               // Debug.Log("Hit " + hitInfo.transform.gameObject.name);
                if (hitInfo.transform.gameObject.CompareTag("Object"))
                {
                    SelectThis(hitInfo.transform.gameObject);
                }
            }
        }
    }
    public bool SelectThis(GameObject targetGameObject)
    {
        SelectableObject target = targetGameObject.GetComponent<SelectableObject>();
        if (target == null)
            return false;
        if (selectingObject != null)
        {
            selectingObject.UpdateSelection(false);
        }
        if (selectingObject != target)
        {
            target.UpdateSelection(true);
            selectingObject = target;
            if (menu_controller.page_stack.Peek() != 1)
            {
                objEditor.startEditing(targetGameObject);
                menu_controller.OpenPage(1);
            }
        }
        return true;
    }

    public void DeselectAll()
    {
        selectingObject.UpdateSelection(false);
        selectingObject = null;
        menu_controller.OpenPage(-1);
    }
}
