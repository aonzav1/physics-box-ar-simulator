using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProblemGenerator : MonoBehaviour
{
    private int highScore;
    private int curScore;
    public Text highscoreTxt;
    public Text scoreTxt;
    public Text timeTxt;
    private int difficulty; //1 easy, 2 normal, 3 hard
    public int timeleft;
    private int question_num;
    public MenuController menu_con;

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator countdown(int maxtime)
    {
        timeleft = maxtime;
        while(timeleft > 0)
        {
            timeTxt.text = timeleft + " s";
            yield return new WaitForSeconds(1);
            timeleft -= 1;
        }
        timeTxt.text = timeleft + " s";
        SaveScore();
        menu_con.OpenPage(0);
        Debug.Log("Lose");
    }

    public void UpdateHighScore()
    {
        highscoreTxt.text = highScore.ToString();
    }
    public void UpdateScore()
    {
        scoreTxt.text = "Score:"+curScore.ToString();
    }
    public void PlayMode(int diff)
    {
        difficulty = diff;
        question_num = 0;
        curScore = 0;
        GenerateNewQuestion();
        menu_con.OpenPage(1);
        StartCoroutine(countdown(60*(4-difficulty)));
    }
    public void GenerateNewQuestion()
    {
        question_num += 1;
    }

    public void Answer(int choice)
    {
        //simulate
        bool isCorrect = false;
        if (isCorrect)
        {
            AddScoreAndTime();
            GenerateNewQuestion();
        }
        else
        {
            timeleft = 0;
        }
    }

    public void AddScoreAndTime()
    {
        curScore += difficulty;
        timeleft += 5+((3-difficulty)*5);
        //show add score and time
    }
    public void SaveScore()
    {
        //show result
        if (curScore > highScore)
        {
            Debug.Log("new high score : "+curScore);
            highScore = curScore;
            //show award
        }
    }
}

[System.Serializable]
public class Problem
{
    public string question;
    public ObjectData data;
}
