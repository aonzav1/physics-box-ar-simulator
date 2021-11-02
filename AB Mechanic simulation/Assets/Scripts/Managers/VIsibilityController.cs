using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VIsibilityController : MonoBehaviour
{
    public SelectableObject selectingObject;
    public ObjectEditorController objEditor;
    public MenuController menu_controller;
    public SceneCameraController CamController;
    public static byte showDetail;
    public GameObject Visibility_hover;
    public Transform[] visibleSelector;
    public ObjectDatacenter datacenter;
    public Transform workSpace;
    // Start is called before the first frame update
    void Start()
    {
        objEditor = GetComponent<ObjectEditorController>();
        menu_controller = GetComponent<MenuController>();
        CamController = menu_controller.CamController;
       // ChangeVisibility(0);
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

    public void ChangeVisibility(int num)
    {
        Visibility_hover.transform.position = visibleSelector[num].transform.position;
        showDetail = (byte)num;
        ShowAsDetail();
    }
    public void ShowAsDetail()
    {
        switch (showDetail)
        {
            case 0:
                changeMotionLineColor(1);
                break;
            case 1:
                changeMotionLineColor(2);
                break;
            case 2:
                TurnMotionLineOff();
                break;
        }
    }

    void TurnMotionLineOff()
    {
        for (int i = 0; i < workSpace.childCount; i++)
        {
            PhysicsObject ps = workSpace.GetChild(i).GetComponent<PhysicsObject>();
            if(ps.type != ObjectType.staticObject)
                ps.velocityLine.SetActive(false);
        }
    }

    void changeMotionLineColor(byte linenum)
    {
        if(linenum == 1)
        {
            for (int i = 0; i < workSpace.childCount; i++)
            {
                PhysicsObject ps = workSpace.GetChild(i).GetComponent<PhysicsObject>();
                if (ps.type != ObjectType.staticObject) { 
                    for (int j = 0; j < 4; j++)
                        {
                            ps.velocityLine.transform.GetChild(j).GetComponent<Renderer>().material = datacenter.velocity_arrow;
                        }
                }
            }
        }
        else if(linenum == 2)
        {
            for (int i = 0; i < workSpace.childCount; i++)
            {
                PhysicsObject ps = workSpace.GetChild(i).GetComponent<PhysicsObject>();
                if (ps.velocityLine != null)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        ps.velocityLine.transform.GetChild(j).GetComponent<Renderer>().material = datacenter.acceleration_arrow;
                    }
                }
            }
        }
    }
}
