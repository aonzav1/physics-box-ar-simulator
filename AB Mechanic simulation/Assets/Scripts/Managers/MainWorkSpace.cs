using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainWorkSpace : MonoBehaviour
{
    public Transform workSpace;
    public GameObject startButton;
    public GameObject pauseButton;
    public static bool isSimulate;
    // Start is called before the first frame update
    void Start()
    {
        StopSimulation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartSimulation()
    {
        isSimulate = true;
        startButton.SetActive(false);
        pauseButton.SetActive(true);
        for (int i = 0; i < workSpace.childCount; i++)
        {
            workSpace.GetChild(i).GetComponent<Rigidbody>().isKinematic = false;
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
    }
}
