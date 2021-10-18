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
    public static bool tmp_workspace;
    public ObjectDatacenter datacenter;
    public Text simulationTime_txt;
    public float simulationTime;

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
        if (!tmp_workspace)
        {
            SaveTmpData();
            tmp_workspace = true;
        }
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
        isSimulate = false;
        startButton.SetActive(true);
        pauseButton.SetActive(false);
        for (int i = 0; i < workSpace.childCount; i++)
        {
            workSpace.GetChild(i).GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    public void StopSimulation()
    {
        isSimulate = false;
        startButton.SetActive(true);
        pauseButton.SetActive(false);
        for (int i = 0; i < workSpace.childCount; i++)
        {
            workSpace.GetChild(i).GetComponent<Rigidbody>().isKinematic = true;
        }
        simulationTime = 0;
        simulationTime_txt.text = "t = 0.00 s";
        if (tmp_workspace)
        {
            LoadTmpData();
            tmp_workspace = false;
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
            tmpSave[i].obj.transform.rotation.eulerAngles.Set(rot.x, rot.y, rot.z);
        }
        tmpSave.Clear();
    }

}

[System.Serializable]
public class ObjectSaveData
{
    public GameObject obj;
    public Vector3 position;
    public Vector3 rotation;
}
