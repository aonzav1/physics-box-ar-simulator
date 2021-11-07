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
    public float[] answers;
    private int difficulty; //1 easy, 2 normal, 3 hard
    public int timeleft;
    public ObjectDatacenter datacenter;
    private Problem targetProblem;
    private int question_num;
    private int correct;
    private float totalTime;
    bool countTime;
    public MenuController menu_con;
    public MainWorkSpace main;
    public ForceManager force_manager;
    public int thisAnswer;
    bool isAnswered;
    public SceneCameraController cam_controller;
    public GameObject correct_pref;
    public GameObject wrong_pref;
    public GameObject end_1_pref;
  //  public GameObject end_2_pref;

    private void Start()
    {
        highscoreTxt.text = highScore.ToString();
        cam_controller.enableRotate = true;
        VIsibilityController.showDetail = 1;
    }
    // Update is called once per frame
    void Update()
    {
        if (countTime)
        {
            totalTime += Time.deltaTime;
        }
    }

    IEnumerator countdown(int maxtime)
    {
        timeleft = maxtime;
        while (timeleft > 0)
        {
            timeTxt.text = timeleft + " s";
            yield return new WaitForSeconds(1);
            timeleft -= 1;
        }
        countTime = false;
        timeTxt.text = timeleft + " s";
        ShowEnd();
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
        scoreTxt.text = "Score:" + curScore.ToString();
    }
    public void PlayMode(int diff)
    {
        difficulty = diff;
        question_num = 0;
        curScore = 0;
        correct = 0;
        UpdateScore();
        GenerateNewQuestion();
        menu_con.OpenPage(1);
        StartCoroutine(countdown(30 + 30 * (3 - difficulty)));
    }
    public void ReTry()
    {
        PlayMode(difficulty);
    }
    public void GenerateNewQuestion()
    {
        countTime = true;
        answers = new float[4];
        question_num += 1;
        int r = Random.Range(0, datacenter.questions.Length);
        targetProblem = datacenter.questions[r];
        main.SpawnObject(targetProblem.data, this);
        string Question = targetProblem.question;
        float ans = 0;
        switch (targetProblem.type)
        {
            case 0: //Time fall
                float targetheight = Random.Range(targetProblem.unknown_vector.x, targetProblem.unknown_vector.y);
                main.tmp_spawned[targetProblem.targetobject].transform.localPosition = new Vector3(0, targetheight, 0);
                //fall
                ans = Mathf.Sqrt((targetheight - 0.25f) * 2 / (-Physics.gravity.y));
                Debug.Log("ans is " + ans);
                Question = Question.Replace("{h}", targetheight.ToString("F2"));
                GenerateAnswer(0, " s", ans);
                break;
            case 1: //Weight fall
                float targetmass = Random.Range(targetProblem.unknown_vector.x, targetProblem.unknown_vector.y);
                PhysicsObject weightobject = main.tmp_spawned[targetProblem.targetobject].GetComponent<PhysicsObject>();
                weightobject.properties[0] = targetmass;
                // weightobject.CalculateNewForcesWithUnknown(targetProblem.unknown_force);
                float upwardforce = targetmass * Random.Range(1, 6);
                ans = targetmass * (-Physics.gravity.y) - upwardforce;
                Debug.Log("ans is " + ans);
                weightobject.externalForce = upwardforce;
                weightobject.extForce_vector = targetProblem.externalVector;
                StartCoroutine(DelayCalculation_1(weightobject.gameObject, 2, 0, true));
                // weightobject.CreateObjectForce(targetProblem.externalVector, false) ;
                Question = Question.Replace("{m}", targetmass.ToString("F2"));
                GenerateAnswer(0, " N", ans);
                break;
            case 2: //Find acceleration
                int mu_multiplier_1 = Random.Range(1, 8);
                float mu_multiplier_2 = Random.Range(0, 2);
                float targetmass_2 = Random.Range(0.5f, 3);
                PhysicsObject boxObject_1 = main.tmp_spawned[targetProblem.targetobject].GetComponent<PhysicsObject>();
                boxObject_1.properties[0] = targetmass_2;
                boxObject_1.properties[1] = (targetProblem.unknown_vector.y) * mu_multiplier_1 + 0.05f * mu_multiplier_2;
                boxObject_1.properties[2] = (targetProblem.unknown_vector.x) * mu_multiplier_1;
                // weightobject.CalculateNewForcesWithUnknown(targetProblem.unknown_force);
                float pullingForce = Random.Range(1, 15);
                ans = 0;
                float fakeans = 0;
                float maxStaticFriction_acc = boxObject_1.properties[1] * targetmass_2 * -Physics.gravity.y;
                float DynamicFriction_acc = boxObject_1.properties[2] * targetmass_2 * -Physics.gravity.y;
                if (pullingForce > maxStaticFriction_acc)
                {
                    ans = (pullingForce - DynamicFriction_acc) / targetmass_2;
                }
                else
                {
                    fakeans = (pullingForce - DynamicFriction_acc) / targetmass_2;
                    if (fakeans < 0)
                    {
                        fakeans = pullingForce / targetmass_2;
                        if (fakeans < 0)
                        {
                            fakeans = pullingForce;
                        }
                    }
                }
                Debug.Log("ans is " + ans);
                int R_2 = Random.Range(0, 2);
                int mul_2 = 1;
                if (R_2 == 0)
                {
                    mul_2 = -1;
                }
                boxObject_1.externalForce = pullingForce;
                boxObject_1.extForce_vector = mul_2 * targetProblem.externalVector;
                StartCoroutine(DelayCalculation_1(boxObject_1.gameObject, 1, -1, false, 1)); ;
                // weightobject.CreateObjectForce(targetProblem.externalVector, false) ;
                Question = Question.Replace("{m}", targetmass_2.ToString("F2"));
                Question = Question.Replace("{a}", boxObject_1.properties[1].ToString("F2"));
                Question = Question.Replace("{b}", boxObject_1.properties[2].ToString("F2"));
                GenerateAnswer(1, " m/s^2", ans, fakeans);
                break;
            case 3: //Find minimum force
                int mu_multiplier_3 = Random.Range(1, 8);
                float mu_multiplier_3_2 = Random.Range(0, 2);
                float targetmass_3 = Random.Range(0.5f, 3);
                PhysicsObject boxObject_3 = main.tmp_spawned[targetProblem.targetobject].GetComponent<PhysicsObject>();
                boxObject_3.properties[0] = targetmass_3;
                boxObject_3.properties[1] = (targetProblem.unknown_vector.y) * mu_multiplier_3 + 0.05f * mu_multiplier_3_2;
                boxObject_3.properties[2] = (targetProblem.unknown_vector.x) * mu_multiplier_3;
                // weightobject.CalculateNewForcesWithUnknown(targetProblem.unknown_force);
                ans = boxObject_3.properties[1] * targetmass_3 * -Physics.gravity.y + 0.04f;
                Debug.Log("ans is " + ans);
                int R_3 = Random.Range(0, 2);
                int mul_3 = 1;
                if (R_3 == 0)
                {
                    mul_3 = -1;
                }
                boxObject_3.extForce_vector = mul_3 * targetProblem.externalVector;
                StartCoroutine(DelayCalculation_1(boxObject_3.gameObject, 1, 1, false, 1));
                // weightobject.CreateObjectForce(targetProblem.externalVector, false) ;
                Question = Question.Replace("{m}", targetmass_3.ToString("F2"));
                Question = Question.Replace("{a}", boxObject_3.properties[1].ToString("F2"));
                Question = Question.Replace("{b}", boxObject_3.properties[2].ToString("F2"));
                GenerateAnswer(0, " N", ans);
                break;
            case 4: //Find mass
                int mu_multiplier_4 = Random.Range(1, 7);
                float targetmass_4 = Random.Range(0.5f, 2);
                PhysicsObject boxObject_4 = main.tmp_spawned[targetProblem.targetobject].GetComponent<PhysicsObject>();
                boxObject_4.properties[0] = targetmass_4;
                boxObject_4.properties[1] = 0;
                boxObject_4.properties[2] = (targetProblem.unknown_vector.x) * mu_multiplier_4;
                // weightobject.CalculateNewForcesWithUnknown(targetProblem.unknown_force);
                float pullingForce4 = Random.Range(6, 10);
                ans = targetmass_4;
                float DynamicFriction_acc4 = boxObject_4.properties[2] * targetmass_4 * -Physics.gravity.y;
                float acc_4 = (pullingForce4 - DynamicFriction_acc4) / targetmass_4;
                Debug.Log("ans is " + ans);
                int R_4 = Random.Range(0, 2);
                int mul_4 = 1;
                if (R_4 == 0)
                {
                    mul_4 = -1;
                }
                boxObject_4.externalForce = pullingForce4;
                boxObject_4.extForce_vector = mul_4 * targetProblem.externalVector;
                StartCoroutine(DelayCalculation_1(boxObject_4.gameObject, 1, -1, false, 1)); ;
                // weightobject.CreateObjectForce(targetProblem.externalVector, false) ;
                Question = Question.Replace("{a}", acc_4.ToString("F2"));
                Question = Question.Replace("{b}", boxObject_4.properties[2].ToString("F2"));
                GenerateAnswer(0, " kg", ans);
                break;
        }
        questionTxt.text = "Q" + question_num + ": " + Question;
    }
    public void GenerateAnswer(int num, string unit, float ans, float ans_2 = 0)
    {
        switch (num)
        {
            case 0:
                float mid = ans / 2;
                float smallmid = ans / 10;
                int R = Random.Range(0, 4);
                List<int> randList = new List<int>();
                for (int i = 0; i < 4; i++)
                {
                    int L = 5;
                    if (i != R)
                    {
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
                    if (i == R)
                    {
                        choices[i].text = ans.ToString("F2") + unit;
                        answers[i] = ans;
                    }
                    else
                    {
                        float rand = 0;
                        rand = mid + (smallmid * randList[i]);
                        answers[i] = rand;
                        choices[i].text = rand.ToString("F2") + unit;
                    }
                }
                thisAnswer = R;
                break;
            case 1:
                float mid_1;
                float smallmid_1;
                if (ans > 0)
                {
                    mid_1 = ans / 2;
                    smallmid_1 = ans / 10;
                }
                else
                {
                    mid_1 = ans_2 / 2;
                    smallmid_1 = ans_2 / 10;
                }
                int R_1 = Random.Range(0, 4);
                List<int> randList_1 = new List<int>();
                for (int i = 0; i < 4; i++)
                {
                    int L = 5;
                    if (i != R_1)
                    {
                        L = Random.Range(0, 10);
                        while (L == 5 || randList_1.Contains(L))
                        {
                            L = Random.Range(0, 10);
                        }
                    }
                    randList_1.Add(L);
                }
                for (int i = 0; i < 4; i++)
                {
                    if (i == R_1)
                    {
                        choices[i].text = ans.ToString("F2") + unit;
                        answers[i] = ans;
                    }
                    else
                    {
                        float rand = 0;
                        rand = mid_1 + (smallmid_1 * randList_1[i]);
                        answers[i] = rand;
                        choices[i].text = rand.ToString("F2") + unit;
                    }
                }
                thisAnswer = R_1;
                break;
        }
    }
    IEnumerator DelayCalculation_1(GameObject target, int cal_type, int unknown, bool isTurnOn, int turnonNum = -1)
    {
        yield return new WaitForSeconds(0.2f);
        force_manager.ReceivedAllObjectData();
        force_manager.CalculateNewForces(cal_type, unknown);
        force_manager.selected = target;
        if (isTurnOn)
            force_manager.TurnOnForces();
        if (turnonNum >= 0)
        {
            force_manager.TurnOnForceNum(turnonNum);
        }
    }

    public void Answer(int choice)
    {
        if (isAnswered)
            return;
        countTime = false;
        isAnswered = true;
        if (targetProblem.isReceiveInput)
        {
            PhysicsObject weightobject = main.tmp_spawned[targetProblem.targetobject].GetComponent<PhysicsObject>();
            weightobject.externalForce = answers[choice];
            Debug.Log("pull with " + answers[choice] + " N");
            force_manager.UpdateForces();
        }
        main.StartSimulation();
        force_manager.TurnOffForces();
        Debug.Log("Correct choise is " + thisAnswer + ", your answer :" + choice);
        bool isCorrect = choice == thisAnswer;
        if (isCorrect)
        {
            correct += 1;
        }
        StartCoroutine(countForSimulation(isCorrect));
    }
    IEnumerator countForSimulation(bool isCorrect)
    {
        timeleft += 4;
        yield return new WaitForSeconds(0.25f);
        bool isContinue = true;
        int maxcount = 4;
        while (isContinue && maxcount > 0)
        {
            isContinue = false;
            for (int i = 0; i < main.workSpace.childCount; i++)
            {
                if (main.workSpace.GetChild(i).GetComponent<Rigidbody>().velocity != Vector3.zero)
                {
                    isContinue = true;
                    break;
                }
            }
            maxcount -= 1;
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.5f);
        main.StopSimulation();
        if (isCorrect)
        {
            AddScoreAndTime();
            GenerateNewQuestion();
            ShowResult(true);
        }
        else
        {
            if (difficulty == 1) //easy
            {
                ShowResult(false);
                timeleft -= 10;
                UpdateScore();
                if (timeleft > 0)
                    GenerateNewQuestion();
            }
            else if (difficulty == 2) //normal
            {
                ShowResult(false);
                timeleft -= 10;
                curScore -= 2;
                UpdateScore();
                if (timeleft > 15)
                    GenerateNewQuestion();
            }
            else //hard
            {
                timeleft = 0;
            }
        }
        isAnswered = false;
    }
    public void ShowResult(bool isCorrect)
    {
        if (isCorrect)
        {
            GameObject a = Instantiate(correct_pref, menu_con.canvas.transform);
            Destroy(a,1f);
        }
        else
        {
            GameObject b = Instantiate(wrong_pref, menu_con.canvas.transform);
            Destroy(b, 1f);
        }
    }
    public void ShowEnd()
    {
        GameObject b = Instantiate(end_1_pref, menu_con.canvas.transform);
        b.GetComponent<EndPad>().LoadData(curScore,totalTime/question_num,question_num,correct);
    }

    public void AddScoreAndTime()
    {
        curScore += difficulty;
        timeleft += 15;
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
        if(targetProblem.type == 0)
        {
            main.StopTimeCount();
        }
    }

}

