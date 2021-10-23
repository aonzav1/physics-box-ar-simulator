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

    public List<ObjectSaveData> tmpSave = new List<ObjectSaveData>();
    
    // Start is called before the first frame update
    void Start()
    {
        StopSimulation();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSimulate)
        {
            simulationTime += Time.deltaTime;
            simulationTime_txt.text = "t = "+simulationTime.ToString("F2")+" s";
        }
    }
    public void StartSimulation()
    {
        if (!isSimulate)
        {
            SaveTmpData();
            isSimulate = true;
        }
        Time.timeScale = 1;
        isSimulate = true;
        startButton.SetActive(false);
        pauseButton.SetActive(true);
        for (int i = 0; i < workSpace.childCount; i++)
        {
            workSpace.GetChild(i).GetComponent<Rigidbody>().isKinematic = false;
        }
    }
    public void PauseSimulation()
    {
        Time.timeScale = 0;
        startButton.SetActive(true);
        pauseButton.SetActive(false);
       /* for (int i = 0; i < workSpace.childCount; i++)
        {
            workSpace.GetChild(i).GetComponent<Rigidbody>().isKinematic = true;
        }*/
    }
    public void StopSimulation()
    {
        Time.timeScale = 1;
        startButton.SetActive(true);
        pauseButton.SetActive(false);
        for (int i = 0; i < workSpace.childCount; i++)
        {
            workSpace.GetChild(i).GetComponent<Rigidbody>().isKinematic = true;
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
        Debug.Log("Save temp data");
        tmpSave = new List<ObjectSaveData>();
        for (int i = 0; i < workSpace.childCount; i++)
        {
            ObjectSaveData newsave = new ObjectSaveData();
            newsave.obj = workSpace.GetChild(i).gameObject;
            newsave.position = newsave.obj.transform.position;
            newsave.rotation = newsave.obj.transform.rotation.eulerAngles;
            tmpSave.Add(newsave);
        }
    }

    public void LoadTmpData()
    {
        Debug.Log("Load temp data");
        for (int i = 0; i < tmpSave.Count; i++)
        {
            tmpSave[i].obj.transform.position = tmpSave[i].position;
            Vector3 rot = tmpSave[i].rotation;
            tmpSave[i].obj.transform.eulerAngles = rot;
          //  tmpSave[i].obj.transform.rotation.eulerAngles.Set(rot.x, rot.y, rot.z);
        }
        tmpSave.Clear();
    }

    public void SpawnObject(ObjectData data)
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
            tmp_obj[i] = a.GetComponent<Rigidbody>();
        }
        if(data.control_panel != null)
        {
            object_interaction = Instantiate(data.control_panel, mainUI);
        }
        isSimulate = false;
        StopSimulation();
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
