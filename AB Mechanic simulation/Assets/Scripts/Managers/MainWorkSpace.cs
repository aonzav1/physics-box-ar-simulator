using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ObjectType:byte
{
    Box,Force,Rope,staticObject
}

public class MainWorkSpace : MonoBehaviour
{
    public Transform workSpace;
    public GameObject startButton;
    public GameObject pauseButton;
    public static bool isSimulate;
    public ObjectDatacenter datacenter;
    public Text simulationTime_txt;
    public float simulationTime;
    public GameObject object_interaction;
    public Transform mainUI;
    public Rigidbody[] tmp_spawned;
    public VIsibilityController visiblecontroler;
    public ForceManager forcemanager;
    public bool isGenInteraction;
    bool stopCount;

    public List<ObjectSaveData> tmpSave = new List<ObjectSaveData>();
    
    // Start is called before the first frame update
    void Start()
    {
        forcemanager = GetComponent<ForceManager>();
        StopSimulation();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSimulate && !stopCount)
        {
            simulationTime += Time.deltaTime;
            simulationTime_txt.text = "t = "+simulationTime.ToString("F2")+" s";
        }
    }
    public void StartSimulation()
    {
        stopCount = false;
        if (!isSimulate)
        {
            SaveTmpData();
            isSimulate = true;
        }
        Time.timeScale = 1;
        isSimulate = true;
        if (startButton != null)
        {
            startButton.SetActive(false);
            pauseButton.SetActive(true);
        }
        for (int i = 0; i < tmp_spawned.Length; i++)
        {
            if(tmp_spawned[i]!= null)
                tmp_spawned[i].isKinematic = false;
        }
    }
    public void PauseSimulation()
    {
        Time.timeScale = 0;
        if (startButton != null)
        {
            startButton.SetActive(true);
            pauseButton.SetActive(false);
        }
       /* for (int i = 0; i < workSpace.childCount; i++)
        {
            workSpace.GetChild(i).GetComponent<Rigidbody>().isKinematic = true;
        }*/
    }
    public void StopSimulation()
    {
        Time.timeScale = 1;
        if (startButton != null)
        {
            startButton.SetActive(true);
            pauseButton.SetActive(false);
        }
        for (int i = 0; i < tmp_spawned.Length; i++)
        {
            if (tmp_spawned[i] != null)
                tmp_spawned[i].isKinematic = true;
        }
        simulationTime = 0;
        simulationTime_txt.text = "t = 0.00 s";
        if (isSimulate)
        {
            LoadTmpData();
            isSimulate = false;
        }
    }

    public void SaveTmpData()
    {
        tmpSave = new List<ObjectSaveData>();
        for (int i = 0; i < workSpace.childCount; i++)
        {
            ObjectSaveData newsave = new ObjectSaveData();
            newsave.obj = workSpace.GetChild(i).gameObject;
            newsave.position = newsave.obj.transform.position;
            newsave.rotation = newsave.obj.transform.rotation.eulerAngles;
            tmpSave.Add(newsave);
        }
        Debug.Log("Save temp data, total of "+tmpSave.Count);
    }

    public void LoadTmpData()
    {
        Debug.Log("Load temp data, total of " + tmpSave.Count);
        for (int i = 0; i < tmpSave.Count; i++)
        {
            if (tmpSave[i].obj != null)
            {
                tmpSave[i].obj.transform.position = tmpSave[i].position;
                Vector3 rot = tmpSave[i].rotation;
                tmpSave[i].obj.transform.eulerAngles = rot;
            }
          //  tmpSave[i].obj.transform.rotation.eulerAngles.Set(rot.x, rot.y, rot.z);
        }
        tmpSave.Clear();
    }
    public void StopTimeCount()
    {
        stopCount = true;
    }

    public void SpawnObject(ObjectData data, ProblemGenerator probgen=null)
    {
        ClearObject();
        Rigidbody[] tmp_obj = new Rigidbody[data.prefab.Length]; 
        for (int i = 0; i < data.prefab.Length; i++)
        {
            PhysicsObject a= Instantiate(data.prefab[i], workSpace).GetComponent<PhysicsObject>();
            if(a.stacking_object != null)
            {
                a.stacking_rb = new List<Rigidbody>();
                for(int j=0; j < a.stacking_object.Length; j++)
                {
                    a.stacking_rb.Add(tmp_obj[a.stacking_object[j]]);
                }
            }
            a.probgen = probgen;
            tmp_obj[i] = a.GetComponent<Rigidbody>();
        }
        tmp_spawned = tmp_obj;

        if(data.control_panel != null && isGenInteraction)
        {
            object_interaction = Instantiate(data.control_panel, mainUI);
        }
        isSimulate = false;
        StopSimulation();
        /*  if (data.requireSurroundData)
              StartCoroutine(GetObjectSurroundingData());
          else
              StopSimulation();*/

        if (visiblecontroler != null)
            visiblecontroler.ShowAsDetail();

        if(probgen == null)
            StartCoroutine(GetObjectSurroundingData(data));
    }
    IEnumerator GetObjectSurroundingData(ObjectData data)
    {
        Debug.Log("Get object data");
        yield return new WaitForSeconds(0.2f);
        forcemanager.ReceivedAllObjectData();
        if (forcemanager != null)
        {
            forcemanager.CalculateNewForces(data.forceCalculationNum, -1);
        }
    }

    public void ClearObject()
    {
        for(int i = 0; i < workSpace.childCount; i++)
        {
            Destroy(workSpace.GetChild(i).gameObject);
        }
        if(object_interaction != null)
        {
            Destroy(object_interaction);
        }
    }

}

[System.Serializable]
public class ObjectSaveData
{
    public GameObject obj;
    public Vector3 position;
    public Vector3 rotation;
}
