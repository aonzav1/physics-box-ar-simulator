using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ViewMode:byte
{
    all,focus
}

public enum EditMode:byte
{
   none,move,rotate,scale
}

public enum ForceColor : byte
{
    blue,red,pink
}

public class ObjectEditorController : MonoBehaviour
{
    [Header("Selected object properties")]
    public GameObject selecting;
    PhysicsObject selecting_physicsObject;
    Vector3 tmp_position;
    Vector3 tmp_rotation;
    public GameObject OpeningProperties;
    public bool isViewForce;

    Vector3 backup_position;
    Vector3 backup_rotation;

    Vector3 now_position;
    Vector3 now_rotation;

    [Header("Mode selection UI")]
    public EditMode editmode;
    public ViewMode viewmode;
    public Transform viewSelectionRing;
    public Transform modeSelectionRing;
    public Transform[] viewList;
    public Transform[] modeList;
    public Text selecting_txt;
    public Canvas canvas;
    public GameObject forcevisible_hover;

    [Header("Edit paremeter")]
  //  public float moveFactor;
    public float rotateFactor;

    [Header("Prefabs")]
    public GameObject BoxProperties;
    public GameObject[] ForceArrow;

    [Header("Memory")]
    public List<ForceTmp> AllForce = new List<ForceTmp>();

    // [Header("Ref")]
    bool enableEdit;
    bool isEditing;
    Rigidbody sel_rb;
    VIsibilityController visible_controller;

    private void Start()
    {
        visible_controller = GetComponent<VIsibilityController>();
       // ToggleViewForce();
    }
    private void Update()
    {
        if(selecting != null && enableEdit)
        {
            if (Input.GetMouseButton(0))
            {
                RaycastHit hitInfo = new RaycastHit();
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
                if (hit)
                {
                    if (hitInfo.transform.gameObject == selecting)
                    {
                        if(!isEditing)
                         SaveHistory();
                        isEditing = true;
                        FunctionEditMode();
                    }
                }
            }
            if (isEditing && Input.GetMouseButtonUp(0))
            {
                SaveChange();
                isEditing = false;
                if(MainWorkSpace.isSimulate)
                    sel_rb.isKinematic = false;
                else
                    sel_rb.isKinematic = true;
                sel_rb.useGravity = true;
                visible_controller.CamController.enableRotate = true;
            }
        } 
    }
    private void SaveHistory()
    {
       backup_position = selecting.transform.position;
       backup_rotation = selecting.transform.rotation.eulerAngles;
    }
    private void SaveChange()
    {
        //CalculateForce();
        now_position = selecting.transform.position;
        now_rotation = selecting.transform.rotation.eulerAngles;
    }

    private void FunctionEditMode()
    {
        visible_controller.CamController.enableRotate = false;
        switch (editmode)
        {
            case EditMode.move:
                sel_rb.isKinematic = true;
                float distance_to_screen = Camera.main.WorldToScreenPoint(selecting.transform.position).z;
                selecting.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));
                break;
            case EditMode.rotate:
                sel_rb.isKinematic = false;
                sel_rb.useGravity = false;

                float X = Input.GetAxis("Mouse X") * rotateFactor*sel_rb.mass;
                float Y = Input.GetAxis("Mouse Y") * rotateFactor* sel_rb.mass;

                sel_rb.AddTorque(Vector3.down* X);
                sel_rb.AddTorque(Vector3.right * Y);
                break;
        }
    }

    public void startEditing(GameObject target)
    {
        selecting = target;
        selecting_txt.text = "Selecting: "+target.name;
        sel_rb = target.GetComponent<Rigidbody>();
        selecting_physicsObject = target.GetComponent<PhysicsObject>();
        tmp_position = target.transform.position;
        tmp_rotation = target.transform.rotation.eulerAngles;
        SelectEditmode(0);
        SelectViewmode(0);
       // StartCoroutine(DelayedEnableEdit());
    }
 /*   IEnumerator DelayedEnableEdit() //deprecated
    {
        yield return new WaitForSeconds(0.2f);
        enableEdit = true;
    }*/
    public void SelectEditmode(int mode)
    {
        editmode = (EditMode)mode;
      //  modeSelectionRing.position = modeList[(byte)mode].position;
    }
    public void SelectViewmode(int mode)
    {
        viewmode = (ViewMode)mode;
        viewSelectionRing.position = viewList[(byte)mode].position;
        UpdateViewMode();
    }
    public void UpdateViewMode()
    {
        switch (viewmode)
        {
            case ViewMode.all:
                visible_controller.SeeAllObjects();
                break;
            case ViewMode.focus:
                visible_controller.DisableOtherObjects();
                break;
        }
    }


    public void SaveEdit()
    {/*
        if (!enableEdit)
            return;*/
        Debug.Log("Save");
        CloseEditGracefully();
    }
    public void CancelEdit()
    {
    /*    if (!enableEdit)
            return; */
        Debug.Log("Cancel");
        selecting.transform.position = tmp_position;
        selecting.transform.eulerAngles = tmp_rotation;
        CloseEditGracefully();
    }
    void CloseEditGracefully()
    {
        Debug.Log("Close");
        selecting = null;
        enableEdit = false;
        CleanTempForces();
        visible_controller.DeselectAll();
    }

    public void Undo()
    {
        selecting.transform.position = backup_position;
        selecting.transform.eulerAngles = backup_rotation;
    }

    public void Redo()
    {
        selecting.transform.position = now_position;
        selecting.transform.eulerAngles = now_rotation;
    }

    public void EditProperties()
    {
        visible_controller.CamController.enableRotate = false;
        switch (selecting.tag)
        {
            case "Object":
                GameObject a = Instantiate(BoxProperties, canvas.transform);
                BoxProperties boxProperties = a.GetComponent<BoxProperties>();
                boxProperties.objEditor = this;
                boxProperties.target_Object = selecting_physicsObject;
                OpeningProperties = a;
                enableEdit = false;
                break;
        }
    }

    public void CloseEditProperties()
    {
        UpdateForces();
        visible_controller.CamController.enableRotate = true;
        Destroy(OpeningProperties);
        enableEdit = true;
    }

    public void ToggleViewForce()
    {
        if (isViewForce)
        {
            CleanTempForces();
            forcevisible_hover.SetActive(true);
            isViewForce = false;
        }
        else
        {
            CalculateNewForces();
            forcevisible_hover.SetActive(false);
            isViewForce = true;
        }
    }

    public void CalculateNewForces()
    {
        CleanTempForces();
        for(int i = 0; i < selecting_physicsObject.forces.Length; i++)
        {
            CreateObjectForce(selecting_physicsObject.forces[i],false);
        }
    }
    public void UpdateForces()
    {
        for (int i = 0; i <AllForce.Count; i++)
        {
            ChangeForce(i);
        }
    }

    public ForceTmp CreateObjectForce(ForceVector force_vector,bool isUnkown)
    {
        GameObject force = Instantiate(ForceArrow[(byte)force_vector.color], selecting.transform);
        force.name = ForceVector.GetName(force_vector.force_num);
        force.transform.rotation = Quaternion.LookRotation(force_vector.direction, Vector3.up);
        ForceTmp forceScript = force.GetComponent<ForceTmp>();
        float magnitude = CalculateForceMagnitude(force_vector.force_num);
        forceScript.calculateType = force_vector.force_num;
        /*if (magnitude < 0)
        {
            magnitude = -magnitude;
            forceScript.arrow.transform.eulerAngles = new Vector3(0, 180, 0);
        }*/
        forceScript.UpdateMagnitude(magnitude,false);
        AllForce.Add(forceScript);
        return forceScript;
    }
    public float CalculateForceMagnitude(int num)
    {
        switch (num)
        {
            case 0: //mg
                return sel_rb.mass * -Physics.gravity.y;
            case 1: //N
                return sel_rb.mass * -Physics.gravity.y;
            case 2: //N
                return selecting_physicsObject.totalMass * -Physics.gravity.y;
            case 3: //friction
                float normal = selecting_physicsObject.totalMass * -Physics.gravity.y;
                PhysicMaterial floorMatt_1 = selecting_physicsObject.floor.material;
                PhysicMaterial myMatt_1 = selecting_physicsObject.boxCollider.material;
                float maxstaticF = (floorMatt_1.staticFriction+ myMatt_1.staticFriction)/2 * normal;
                if(selecting_physicsObject.externalForce > maxstaticF)
                {
                    return (floorMatt_1.dynamicFriction + myMatt_1.dynamicFriction) / 2 * normal; 
                }
                else
                {
                    return selecting_physicsObject.externalForce;
                }
            case 4: //relative friction
                float r_normal = (selecting_physicsObject.totalMass-sel_rb.mass) * -Physics.gravity.y;
                PhysicMaterial upperMatt_2 = selecting_physicsObject.stacking_rb[0].GetComponent<Collider>().material;
                PhysicMaterial myMatt_2 = selecting_physicsObject.boxCollider.material;
                float r_maxstaticF = (upperMatt_2.staticFriction + myMatt_2.staticFriction) / 2 * r_normal;
                if (selecting_physicsObject.externalForce > r_maxstaticF)
                {
                 
                    return -(upperMatt_2.dynamicFriction + myMatt_2.dynamicFriction) / 2 * r_normal;
                }
                else
                {
                    return -selecting_physicsObject.externalForce;
                }
            case 5:
                return selecting_physicsObject.externalForce;
            default:
                return 0;
        }
    }

    public void ChangeForce(int num)
    {
        float magnitude = CalculateForceMagnitude(AllForce[num].calculateType);
        AllForce[num].UpdateMagnitude(magnitude,false);
    }

    public void CleanTempForces()
    {
        for(int i = 0; i < AllForce.Count; i++)
        {
            Destroy(AllForce[i].gameObject);
        }
        AllForce = new List<ForceTmp>();
    }
}
