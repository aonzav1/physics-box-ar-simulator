using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PullBox : MonoBehaviour
{
    public GameObject[] Targets;
    public MainWorkSpace main;
    public Slider val_slider;
    public Text valueText;
    // Start is called before the first frame update
    void Start()
    {
        main = FindObjectOfType<MainWorkSpace>();
        Targets = new GameObject[main.workSpace.childCount];
        for(int i = 0; i < main.workSpace.childCount; i++)
        {
            Targets[i] = main.workSpace.GetChild(i).gameObject;
        }
    }
    private void Update()
    {
        valueText.text = val_slider.value.ToString("F2") + "N";
    }

    public void Pull(int num)
    {
        Targets[num].GetComponent<Rigidbody>().AddForce(val_slider.value, 0, 0);
    }
}
