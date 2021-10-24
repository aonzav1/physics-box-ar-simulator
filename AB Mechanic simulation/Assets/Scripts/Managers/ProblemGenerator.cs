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
    public Text questionTxt;
    public Text[] choices;
    private int difficulty; //1 easy, 2 normal, 3 hard
    public int timeleft;
    public ObjectDatacenter datacenter;
    private Problem targetProblem;
    private int question_num;
    public MenuController menu_con;
    public MainWorkSpace main;
    public int thisAnswer;
    bool isAnswered;
    public SceneCameraController cam_controller;

    private void Start()
    {
        highscoreTxt.text = highScore.ToString();
        cam_controller.enableRotate = true;
    }
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
        main.ClearObject();
        SaveScore();
        menu_con.OpenPage(0);
        Debug.Log("Lose");
    }

    public void forceExit()
    {
        timeleft = 0;
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
        UpdateScore();
        GenerateNewQuestion();
        menu_con.OpenPage(1);
        StartCoroutine(countdown(60*(4-difficulty)));
    }
    public void GenerateNewQuestion()
    {
        question_num += 1;
        int r = Random.Range(0,datacenter.questions.Length);
        targetProblem = datacenter.questions[r];
        main.SpawnObject(targetProblem.data,this);
        string Question = targetProblem.question;
        float ans = 0;
        switch (targetProblem.type)
        {
            case QuestionUnknownType.fall:
                float targetheight = Random.Range(targetProblem.unknown_vector.x, targetProblem.unknown_vector.y);
                main.tmp_spawned[targetProblem.targetobject].transform.localPosition = new Vector3(0,targetheight, 0);
                //fall
                ans = Mathf.Sqrt((targetheight-0.25f) * 2/(-Physics.gravity.y));
                Debug.Log("ans is "+ans);
                Question = Question.Replace("{h}", targetheight.ToString("F2"));
                break;
        }
        questionTxt.text = "Q" + question_num + ": " + Question;
            float mid = ans / 2;
            float smallmid = ans / 10;
            int R = Random.Range(0, 4);
            List<int> randList = new List<int>();
            for(int i = 0; i < 4; i++)
            {
                int L = 5;
                if (i != R) {
                    L = Random.Range(0, 10);
                    while (L == 5 || randList.Contains(L))
                    {
                    L = Random.Range(0, 10);
                    }
                }
                randList.Add(L); 
            }
            for (int i = 0; i < 4; i++)
            {
                if (i == R) {
                    choices[i].text = ans.ToString("F2") + " s";
                }
                else
                {
                    float rand = 0;
                    rand = mid + (smallmid * randList[i]);
                    choices[i].text = rand.ToString("F2") + " s";
                }
            }
            thisAnswer = R;
    }

    public void Answer(int choice)
    {
        if (isAnswered)
            return;
        isAnswered = true;
        main.StartSimulation();
        Debug.Log("Correct choise is " + thisAnswer+", your answer :"+choice);
        StartCoroutine(countForSimulation(choice == thisAnswer));
    }
    IEnumerator countForSimulation(bool isCorrect)
    {
        yield return new WaitForSeconds(0.25f);
        bool isContinue = true;
        while (isContinue)
        {
            isContinue = false;
            for(int i = 0; i < main.workSpace.childCount; i++)
            {
                if (main.workSpace.GetChild(i).GetComponent<Rigidbody>().velocity != Vector3.zero)
                {
                    isContinue = true;
                    break;
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.5f);
        if (isCorrect)
        {
            AddScoreAndTime();
            GenerateNewQuestion();
        }
        else
        {
            timeleft = 0;
        }
        isAnswered = false;
    }

    public void AddScoreAndTime()
    {
        curScore += difficulty;
        timeleft += 5+((3-difficulty)*5);
        UpdateScore();
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
        highscoreTxt.text = highScore.ToString();
    }

    public void objectHit()
    {
        if(targetProblem.type == QuestionUnknownType.fall)
        {
            main.StopTimeCount();
        }
    }
}

