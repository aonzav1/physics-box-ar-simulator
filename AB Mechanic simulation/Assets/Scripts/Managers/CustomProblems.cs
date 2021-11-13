using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomProblems : MonoBehaviour
{
    ProblemGenerator probgen;
    public bool[] targetData = new bool[5];
    public GameObject[] blocks;
    // Start is called before the first frame update
    void Start()
    {
        probgen = FindObjectOfType<ProblemGenerator>();
        targetData = new bool[5];
        for (int i = 0; i < 5; i++)
        {
            targetData[i] = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Choose(int num)
    {
        targetData[num] = !targetData[num];
        blocks[num].SetActive(!targetData[num]);
    }
    public void CloseProblemSetting()
    {
        gameObject.SetActive(false);
    }
    public void StartCustomProblem()
    {
        List<byte> rawData = new List<byte>();
        if (targetData[0]) //fallbox
        {
            rawData.Add(0);
            rawData.Add(1);
        }
        if (targetData[1])  //drag cube
        {
            rawData.Add(2);
            rawData.Add(3);
            rawData.Add(4);
        }
        if (targetData[2])  //two box
        {
            for (int i = 10; i < 13; i++)
            {
                rawData.Add((byte)i);
            }
        }
        if (targetData[3])  //slant
        {
            for (int i = 5; i < 10; i++)
            {
                rawData.Add((byte)i);
            }
        }
        if (targetData[4])  //cabinet
        {
            for (int i = 13; i < 21; i++)
            {
                rawData.Add((byte)i);
            }
        }
        if (rawData.Count == 0)
            return;
        byte[] data = new byte[rawData.Count];
        for(int i = 0; i < rawData.Count; i++)
        {
            data[i] = rawData[i];
        }
        probgen.OpenPlayPageCustom(data);
        CloseProblemSetting();
    }
}
