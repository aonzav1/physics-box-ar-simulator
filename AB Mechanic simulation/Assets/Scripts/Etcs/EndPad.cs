using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndPad : MonoBehaviour
{
    public Text scoretxt;
    public Text time_average;
    public Text did_correct;
    public ProblemGenerator probgen;
    // Start is called before the first frame update
    public void Start()
    {
        probgen = FindObjectOfType<ProblemGenerator>();
        //animation
    }
    public void LoadData(int score,float time_avg,int count,int correct)
    {
        scoretxt.text = score.ToString();
        time_average.text = "Average time :" + time_avg.ToString("F1") + "s";
        did_correct.text = "Correct answer :"+ correct + "/" + count;
    }
    public void Back()
    {
        Destroy(this.gameObject);
    }
    public void ReTry()
    {
        probgen.ReTry();
        Destroy(this.gameObject);
    }
}
