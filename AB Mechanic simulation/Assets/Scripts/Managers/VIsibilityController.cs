using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VIsibilityController : MonoBehaviour
{
    public SelectableObject selectingObject;
    public ObjectEditorController objEditor;
    public MenuController menu_controller;
    public SceneCameraController CamController;
    public static bool showVelocity;
    public GameObject vectorVisibility_hover;
    public Transform workSpace;
    // Start is called before the first frame update
    void Start()
    {
        objEditor = GetComponent<ObjectEditorController>();
        menu_controller = GetComponent<MenuController>();
        CamController = menu_controller.CamController;
        ChangeVelocitVisibility();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectingObject == null && !CamController.enableRotate && Input.GetMouseButtonDown(0))
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
        if (objEditor.viewmode == ViewMode.focus)
            SeeAllObjects();
        selectingObject.UpdateSelection(false);
        selectingObject = null;
        Debug.Log("say Opne page 0");
        menu_controller.OpenPage(0);
    }

    public void DisableOtherObjects()
    {
        bool isFound = false;
        for (int i = 0; i < workSpace.childCount; i++)
        {
            if (isFound || workSpace.GetChild(i).gameObject != objEditor.selecting)
            {
                workSpace.GetChild(i).gameObject.SetActive(false);
            }
            else
            {
                isFound = true;
            }
        }
        CamController.LockTarget(selectingObject.transform);
    }
    public void SeeAllObjects()
    {
        CamController.LockOriginal();
        for (int i = 0; i < workSpace.childCount; i++)
        {
            workSpace.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void ChangeVelocitVisibility()
    {
        if (showVelocity)
        {
            showVelocity = false;
            vectorVisibility_hover.SetActive(true);
            for (int i = 0; i < workSpace.childCount; i++)
            {
                workSpace.GetChild(i).GetComponent<PhysicsObject>().velocityLine.SetActive(false);
            }
        }
        else
        {
            showVelocity = true;
            vectorVisibility_hover.SetActive(false);
        }
    }
}
