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
    public int[] items_count;
    public Text[] items_count_txt;
    public GameObject cur_info;
    public GameObject info_butt;
    public GameObject rope_pref;
    public GameObject problemSetting;
    public bool isCustomGame;
    public byte[] targetProblems;
    public bool is2x;
  //  public GameObject end_2_pref;

    private void Start()
    {
        highScore = SaveLoad.cur_setting.highscore;
        items_count = new int[3];
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
    public void OpenPlayPage(int diff)
    {
        isCustomGame = false;
        menu_con.OpenPage(1);
        difficulty = diff;
    }
    public void OpenPlayPageCustom(byte[] data)
    {
        targetProblems = data;
        isCustomGame = true;
        menu_con.OpenPage(1);
        difficulty = 0;
    }
    public void StartGame()
    {
        menu_con.Pages[2].SetActive(false);
        PlayMode();
    }
    public void PlayMode()
    {
        question_num = 0;
        curScore = 0;
        correct = 0;
        for(int i = 0; i < 3; i++)
        {
            items_count[i] = Random.Range(0, 5-difficulty);
            items_count_txt[i].text = items_count[i].ToString();
        }
        UpdateScore();
        GenerateNewQuestion();
        is2x = false;
        scoreTxt.color = Color.white;
        StartCoroutine(countdown(60 + 60 * (3 - difficulty)));
    }
    public void UpdateItemCount(int num)
    {
        items_count_txt[num].text = items_count[num].ToString();
    }
    public void ReTry()
    {
        menu_con.OpenPage(1);
    }
    public void GenerateNewQuestion()
    {
        countTime = true;
        answers = new float[4];
        question_num += 1;
        //tmp
        if (cur_info != null)
            Destroy(cur_info);
        int r = 0;
        if (isCustomGame)
        {
            int R2 = Random.Range(0, targetProblems.Length);
            r = targetProblems[R2];
        }
        else
        {
            r = Random.Range(0, datacenter.questions.Length);
        }
        targetProblem = datacenter.questions[r];
        if (targetProblem.info_pref != null)
            info_butt.SetActive(true);
        else
            info_butt.SetActive(false);
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
                Debug.Log("======= ans is " + ans);
                Question = Question.Replace("{h}", targetheight.ToString("F2"));
                GenerateAnswer(0, " s", ans);
                break;
            case 1: //Weight fall
                float targetmass = Random.Range(targetProblem.unknown_vector.x, targetProblem.unknown_vector.y);
                PhysicsObject weightobject = main.tmp_spawned[targetProblem.targetobject];
                weightobject.properties[0] = targetmass;
                // weightobject.CalculateNewForcesWithUnknown(targetProblem.unknown_force);
                float upwardforce = targetmass * Random.Range(1, 6);
                ans = targetmass * (-Physics.gravity.y) - upwardforce;
                Debug.Log("======= ans is " + ans);
                weightobject.externalForce = upwardforce;
                weightobject.extForce_vector = targetProblem.externalVector;
                StartCoroutine(DelayCalculation_1(weightobject.gameObject, 2, 0, true));
                // weightobject.CreateObjectForce(targetProblem.externalVector, false) ;
                Question = Question.Replace("{m}", targetmass.ToString("F2"));
                GenerateAnswer(0, " N", ans);
                break;
            case 2: //Find acceleration
                int mu_multiplier_1 = Random.Range(2, 8);
                float mu_multiplier_2 = Random.Range(0, 2);
                float targetmass_2 = Random.Range(0.5f, 3);
                PhysicsObject boxObject_1 = main.tmp_spawned[targetProblem.targetobject];
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
                Debug.Log("======= ans is " + ans);
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
                int mu_multiplier_3 = Random.Range(2, 8);
                float mu_multiplier_3_2 = Random.Range(0, 2);
                float targetmass_3 = Random.Range(0.5f, 3);
                PhysicsObject boxObject_3 = main.tmp_spawned[targetProblem.targetobject];
                boxObject_3.properties[0] = targetmass_3;
                boxObject_3.properties[1] = (targetProblem.unknown_vector.y) * mu_multiplier_3 + 0.05f * mu_multiplier_3_2;
                boxObject_3.properties[2] = (targetProblem.unknown_vector.x) * mu_multiplier_3;
                // weightobject.CalculateNewForcesWithUnknown(targetProblem.unknown_force);
                ans = boxObject_3.properties[1] * targetmass_3 * -Physics.gravity.y + 0.04f;
                Debug.Log("======= ans is " + ans);
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
                int mu_multiplier_4 = Random.Range(2, 7);
                float targetmass_4 = Random.Range(0.5f, 2);
                PhysicsObject boxObject_4 = main.tmp_spawned[targetProblem.targetobject];
                boxObject_4.properties[0] = targetmass_4;
                boxObject_4.properties[1] = 0;
                boxObject_4.properties[2] = (targetProblem.unknown_vector.x) * mu_multiplier_4;
                // weightobject.CalculateNewForcesWithUnknown(targetProblem.unknown_force);
                float pullingForce4 = Random.Range(6, 10);
                ans = targetmass_4;
                float DynamicFriction_acc4 = boxObject_4.properties[2] * targetmass_4 * -Physics.gravity.y;
                float acc_4 = (pullingForce4 - DynamicFriction_acc4) / targetmass_4;
                Debug.Log("======= ans is " + ans);
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
            case 5: //Find acceleration
                int mu_multiplier_5 = Random.Range(2, 8);
                float mu_multiplier_5_2 = Random.Range(0, 2);
                float targetmass_5 = Random.Range(0.5f, 3);
                PhysicsObject boxObject_5 = main.tmp_spawned[targetProblem.targetobject];
                boxObject_5.properties[0] = targetmass_5;
                boxObject_5.properties[1] = (targetProblem.unknown_vector.y) * mu_multiplier_5 + 0.05f * mu_multiplier_5_2;
                boxObject_5.properties[2] = (targetProblem.unknown_vector.x) * mu_multiplier_5;
                // weightobject.CalculateNewForcesWithUnknown(targetProblem.unknown_force);
                float pullingForce5 = Random.Range(1, 15);
                ans = 0;
                float fakeans5 = 0;
                float normal5 = targetmass_5 * -Physics.gravity.y * Mathf.Cos((targetProblem.unknown_vector.z * Mathf.PI) / 180);
                float downward5 = targetmass_5 * -Physics.gravity.y * Mathf.Sin((targetProblem.unknown_vector.z * Mathf.PI) / 180);
                float maxStaticFriction_acc5 = boxObject_5.properties[1] * normal5;
                float DynamicFriction_acc5 = boxObject_5.properties[2] * normal5;
                if (pullingForce5 - (downward5 + maxStaticFriction_acc5) > 0) //the box move up
                {
                    ans = (pullingForce5 - (downward5 + DynamicFriction_acc5)) / targetmass_5;
                }
                else
                {
                    if (downward5 - (pullingForce5 + maxStaticFriction_acc5) > 0) //the box move down
                    {
                        ans = (downward5 - (pullingForce5 + DynamicFriction_acc5)) / targetmass_5;
                    }
                    else //the box is not move
                    {
                        fakeans5 = Mathf.Abs((pullingForce5 - DynamicFriction_acc5) / targetmass_5);
                    }
                }
                Debug.Log("======= ans is " + ans);
                boxObject_5.externalForce = pullingForce5;
                boxObject_5.extForce_vector = targetProblem.externalVector;
                StartCoroutine(DelayCalculation_1(boxObject_5.gameObject, 4, -1, false, 1)); ;
                // weightobject.CreateObjectForce(targetProblem.externalVector, false) ;
                Question = Question.Replace("{m}", targetmass_5.ToString("F2"));
                Question = Question.Replace("{a}", boxObject_5.properties[1].ToString("F2"));
                Question = Question.Replace("{b}", boxObject_5.properties[2].ToString("F2"));
                GenerateAnswer(1, " m/s^2", ans, fakeans5);
                break;
            case 6: //Find minimum force
                int mu_multiplier_6 = Random.Range(2, 8);
                float mu_multiplier_6_2 = Random.Range(0, 2);
                float targetmass_6 = Random.Range(0.5f, 3);
                PhysicsObject boxObject_6 = main.tmp_spawned[targetProblem.targetobject];
                boxObject_6.properties[0] = targetmass_6;
                boxObject_6.properties[1] = (targetProblem.unknown_vector.y) * mu_multiplier_6 + 0.05f * mu_multiplier_6_2;
                boxObject_6.properties[2] = (targetProblem.unknown_vector.x) * mu_multiplier_6;
                // weightobject.CalculateNewForcesWithUnknown(targetProblem.unknown_force);
                float normal6 = targetmass_6 * -Physics.gravity.y * Mathf.Cos((targetProblem.unknown_vector.z * Mathf.PI) / 180);
                float downward6 = targetmass_6 * -Physics.gravity.y * Mathf.Sin((targetProblem.unknown_vector.z * Mathf.PI) / 180);
                float maxStaticFriction_acc6 = boxObject_6.properties[1] * normal6;
                ans = downward6 - maxStaticFriction_acc6 + 0.02f;
                Debug.Log("======= ans is " + ans);
              //  boxObject_6.externalForce = pullingForce6;
                boxObject_6.extForce_vector = targetProblem.externalVector;
                StartCoroutine(DelayCalculation_1(boxObject_6.gameObject, 4, 1, false, 1)); ;
                // weightobject.CreateObjectForce(targetProblem.externalVector, false) ;
                Question = Question.Replace("{m}", targetmass_6.ToString("F2"));
                Question = Question.Replace("{a}", boxObject_6.properties[1].ToString("F2"));
                Question = Question.Replace("{b}", boxObject_6.properties[2].ToString("F2"));
                GenerateAnswer(0, " N", ans);
                break;
            case 7: //Find maximum force to pull bottom
                int mu_multiplier_7 = Random.Range(4, 8);
                float mu_multiplier_7_2 = Random.Range(0, 2);
                float targetmass_7 = Random.Range(0.5f, 3);
                PhysicsObject boxObject_7 = main.tmp_spawned[0];
                boxObject_7.properties[0] = targetmass_7;
                boxObject_7.properties[1] = (targetProblem.unknown_vector.y) * mu_multiplier_7 + 0.05f * mu_multiplier_7_2;
                boxObject_7.properties[2] = (targetProblem.unknown_vector.x) * mu_multiplier_7;
                int mu_multiplier_8 = Random.Range(3, 8);
                float mu_multiplier_8_2 = Random.Range(0, 2);
                float targetmass_8 = targetmass_7+Random.Range(0.5f, 2);
                PhysicsObject boxObject_8 = main.tmp_spawned[1];
                boxObject_8.properties[0] = targetmass_8;
                boxObject_8.properties[1] = (targetProblem.unknown_vector.y) * mu_multiplier_8 + 0.05f * mu_multiplier_8_2;
                boxObject_8.properties[2] = (targetProblem.unknown_vector.x) * mu_multiplier_8;
                // weightobject.CalculateNewForcesWithUnknown(targetProblem.unknown_force);
                float totalWeight = (targetmass_7 + targetmass_8) *-Physics.gravity.y;
                ans = totalWeight*(boxObject_8.properties[2] + boxObject_7.properties[1]) - 0.04f;
                Debug.Log("======= ans is " + ans);
                boxObject_8.externalForce = ans;
                boxObject_8.extForce_vector = targetProblem.externalVector;
                StartCoroutine(DelayCalculation_1(boxObject_8.gameObject, 3, 4, false, 4));
                // weightobject.CreateObjectForce(targetProblem.externalVector, false) ;
                Question = Question.Replace("{m1}", targetmass_7.ToString("F2"));
                Question = Question.Replace("{m2}", targetmass_8.ToString("F2"));
                GenerateAnswer(0, " N", ans);
                break;
            case 9: //Find acceleration of topbox
                int mu_multiplier_9 = Random.Range(4, 6);
                float mu_multiplier_9_2 = Random.Range(0, 2);
                float targetmass_9 = Random.Range(0.5f, 3);
                PhysicsObject boxObject_9 = main.tmp_spawned[0];
                boxObject_9.properties[0] = targetmass_9;
                boxObject_9.properties[1] = (targetProblem.unknown_vector.y) * mu_multiplier_9 + 0.05f * mu_multiplier_9_2;
                boxObject_9.properties[2] = (targetProblem.unknown_vector.x) * mu_multiplier_9;
                int mu_multiplier_10 = Random.Range(1, 4);
                float mu_multiplier_10_2 = Random.Range(0, 2);
                float targetmass_10 = targetmass_9 + Random.Range(0.5f, 1.5f);
                PhysicsObject boxObject_10 = main.tmp_spawned[1];
                boxObject_10.properties[0] = targetmass_10;
                boxObject_10.properties[1] = (targetProblem.unknown_vector.y) * mu_multiplier_10 + 0.05f * mu_multiplier_10_2;
                boxObject_10.properties[2] = (targetProblem.unknown_vector.x) * mu_multiplier_10;
                // weightobject.CalculateNewForcesWithUnknown(targetProblem.unknown_force);
                float pullingForce10 = Random.Range(5, 10);
                float totalMass = targetmass_9 + targetmass_10;
                float maxStatic_9 = targetmass_9 * boxObject_9.properties[1] * -Physics.gravity.y;
                float maxStatic_10 = totalMass * boxObject_10.properties[1] * -Physics.gravity.y;
                float groundDynamicFriction_10 = totalMass * boxObject_10.properties[2] * -Physics.gravity.y;
                float a_9 = (pullingForce10 - maxStatic_10) / totalMass;
                if (a_9 > 0)
                {
                    a_9 = (pullingForce10 - groundDynamicFriction_10) / totalMass;
                }
                else
                {
                    a_9 = 0;
                }
                Debug.Log("A Expected a = " + a_9);
                //uppercube
                float friction_req_2 = pullingForce10 - targetmass_9 * a_9;
                if (friction_req_2 > maxStatic_9) //top cube slide
                {
                    float dynamicFriction_2 = boxObject_9.properties[2] * targetmass_9 * -Physics.gravity.y;
                    ans = (pullingForce10 - dynamicFriction_2)/targetmass_9;
                }
                else //top cube with a
                {
                    ans = a_9;
                }
                Debug.Log("======= ans is " + ans);
                boxObject_9.externalForce = pullingForce10;
                boxObject_9.extForce_vector = targetProblem.externalVector;
                StartCoroutine(DelayCalculation_1(boxObject_9.gameObject, 3, -1, false, 4));
                // weightobject.CreateObjectForce(targetProblem.externalVector, false) ;
                Question = Question.Replace("{m1}", targetmass_9.ToString("F2"));
                Question = Question.Replace("{m2}", targetmass_10.ToString("F2"));
                GenerateAnswer(0, " m/s^2", ans);
                break;
            case 11: //Find minimum to move
                int mu_multiplier_11 = Random.Range(4, 8);
                float mu_multiplier_11_2 = Random.Range(0, 2);
                float targetmass_11 = Random.Range(0.5f, 3);
                PhysicsObject boxObject_11 = main.tmp_spawned[0];
                boxObject_11.properties[0] = targetmass_11;
                boxObject_11.properties[1] = (targetProblem.unknown_vector.y) * mu_multiplier_11 + 0.05f * mu_multiplier_11_2;
                boxObject_11.properties[2] = (targetProblem.unknown_vector.x) * mu_multiplier_11;
                int mu_multiplier_12 = Random.Range(3, 8);
                float mu_multiplier_12_2 = Random.Range(0, 2);
                float targetmass_12 = targetmass_11 + Random.Range(0.5f, 2);
                PhysicsObject boxObject_12 = main.tmp_spawned[1];
                boxObject_12.properties[0] = targetmass_12;
                boxObject_12.properties[1] = (targetProblem.unknown_vector.y) * mu_multiplier_12 + 0.05f * mu_multiplier_12_2;
                boxObject_12.properties[2] = (targetProblem.unknown_vector.x) * mu_multiplier_12;
                // weightobject.CalculateNewForcesWithUnknown(targetProblem.unknown_force);
                float totalWeight12 = (targetmass_11 + targetmass_12) * -Physics.gravity.y;
                ans = totalWeight12 * (boxObject_12.properties[1]) +0.005f;
                Debug.Log("======= ans is " + ans);
                boxObject_12.externalForce = ans;
                boxObject_12.extForce_vector = targetProblem.externalVector;
                StartCoroutine(DelayCalculation_1(boxObject_12.gameObject, 3, 4, false, 4));
                // weightobject.CreateObjectForce(targetProblem.externalVector, false) ;
                Question = Question.Replace("{m1}", targetmass_11.ToString("F2"));
                Question = Question.Replace("{m2}", targetmass_12.ToString("F2"));
                GenerateAnswer(0, " N", ans);
                break;
            case 13: //Find maximum height
                float targetmass_13= Random.Range(2, 10);
                PhysicsObject boxObject_13 = main.tmp_spawned[0];
                boxObject_13.properties[0] = targetmass_13;
                if (targetProblem.unknown_vector.z == 0)
                {
                    boxObject_13.properties[1] = 0;
                    boxObject_13.properties[2] = 0;
                }
                else
                {
                    float dynamic = 0.05f * Random.Range(5, 7);
                    boxObject_13.properties[1] = dynamic + 0.05f * Random.Range(1,3);
                    boxObject_13.properties[2] = dynamic;
                }
                float halfwidth13 = (targetProblem.unknown_vector.x) / 2;
                float halfheight13 = (targetProblem.unknown_vector.y) / 2;
                float PushForce = Random.Range(10, 100);
                // weightobject.CalculateNewForcesWithUnknown(targetProblem.unknown_force);
                float weight13 = targetmass_13 * -Physics.gravity.y;
                float theA = (PushForce - weight13* boxObject_13.properties[2]) / targetmass_13;
                ans = (targetmass_13*theA* halfheight13 + weight13* halfwidth13) /PushForce -0.04f;
                Debug.Log("======= ans is " + ans);
                if (ans > (targetProblem.unknown_vector.y))
                    ans = (targetProblem.unknown_vector.y);
                Vector3 localPush = new Vector3(-0.5f, ans, 0);
                Vector3  push_at = boxObject_13.transform.TransformPoint(localPush);
                boxObject_13.externalForce = PushForce;
                boxObject_13.pushAt = push_at;
                boxObject_13.extForce_vector = targetProblem.externalVector;
                StartCoroutine(DelayCalculation_1(boxObject_13.gameObject, 5, -1, false, 1)); ;
                // weightobject.CreateObjectForce(targetProblem.externalVector, false) ;
                Question = Question.Replace("{m}", targetmass_13.ToString("F2"));
                Question = Question.Replace("{F}", PushForce.ToString("F2"));
                GenerateAnswer(0, " m", ans);
                break;
            case 14: //Find maximum force
                float targetmass_14 = Random.Range(2f, 7f);
                PhysicsObject boxObject_14 = main.tmp_spawned[0];
                boxObject_14.properties[0] = targetmass_14;
                if (targetProblem.unknown_vector.z == 0)
                {
                    boxObject_14.properties[1] = 0;
                    boxObject_14.properties[2] = 0;
                }
                else
                {
                    float dynamic = 0.05f * Random.Range(5, 7);
                    boxObject_14.properties[1] = dynamic + 0.05f * Random.Range(1, 3);
                    boxObject_14.properties[2] = dynamic;
                }
                float halfwidth14 = (targetProblem.unknown_vector.x) / 2;
                float halfheight14 = (targetProblem.unknown_vector.y) / 2;
                float height = Random.Range(halfheight14 + 0.02f, targetProblem.unknown_vector.y);
                // weightobject.CalculateNewForcesWithUnknown(targetProblem.unknown_force);
                float weight14 = targetmass_14 * -Physics.gravity.y;
                float maxStatic = weight14 * boxObject_14.properties[1];
                Debug.Log("max friction is "+ maxStatic);
                float force14 = (weight14 * halfwidth14) / height;
                if(force14 > maxStatic)
                {
                    Debug.Log("cabinet moves");
                    force14 = weight14 * (halfwidth14 - boxObject_14.properties[2] * halfheight14) / (height- halfheight14);
                }
                else
                {
                    Debug.Log("cabinet wont move");
                }
                ans = force14+0.05f;
                Debug.Log("======= ans is " + ans);
                Vector3 localPush2 = new Vector3(-0.5f, height, 0);
                Vector3 push_at2 = boxObject_14.transform.TransformPoint(localPush2);
                boxObject_14.externalForce = ans;
                boxObject_14.pushAt = push_at2;
                boxObject_14.extForce_vector = targetProblem.externalVector;
                GameObject rope = Instantiate(rope_pref, boxObject_14.transform);
                rope.transform.position = push_at2 + new Vector3(-0.5f, 0, 0);
                StartCoroutine(DelayCalculation_1(boxObject_14.gameObject, targetProblem.data.forceCalculationNum, -1, false)); ;
                // weightobject.CreateObjectForce(targetProblem.externalVector, false) ;
                Question = Question.Replace("{m}", targetmass_14.ToString("F2"));
                Question = Question.Replace("{h}", height.ToString("F2"));
                GenerateAnswer(0, " N", ans);
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
                if(ans_2 == 0)
                {
                    int RR = R_1;
                    while (RR == R_1)
                    {
                        RR = Random.Range(0, 4);
                    }
                    answers[RR] = 0;
                    choices[RR].text = "0.00" + unit;
                    Debug.Log("Generate fake ans at "+RR);
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
            PhysicsObject weightobject = main.tmp_spawned[targetProblem.targetobject];
            weightobject.externalForce = answers[choice];
            Debug.Log("pull with " + answers[choice] + " N");
            force_manager.UpdateForces();
        }
        if (targetProblem.isReceiveInput2)
        {
            PhysicsObject weightobject = main.tmp_spawned[targetProblem.targetobject];
            Vector3 localPush = new Vector3(-0.5f, answers[choice], 0);
            weightobject.pushAt = weightobject.transform.TransformPoint(localPush);
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
                if (main.tmp_spawned[i].type != ObjectType.staticObject)
                {
                    if (main.tmp_spawned[i].rb.velocity != Vector3.zero)
                    {
                        isContinue = true;
                        break;
                    }
                }
            }
            maxcount -= 1;
            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(0.5f);
        if (isCorrect)
        {
            AddScoreAndTime();
            GenerateNewQuestion();
            ShowResult(true);
        }
        else
        {
            if (is2x)
            {
                is2x = false;
                scoreTxt.color = Color.white;
            }
            main.StopSimulation();
            if (difficulty == 0) //custom
            {
                ShowResult(false);
                timeleft -= 10;
                UpdateScore();
                if (timeleft > 0)
                    GenerateNewQuestion();
            }else
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
                curScore -= 1;
                UpdateScore();
                if (timeleft > 0)
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
        if (is2x)
        {
            is2x = false;
            scoreTxt.color = Color.white;
            curScore += difficulty*2;
            if (difficulty == 0)
                curScore += 1;
        }
        else
        {
            curScore += difficulty;
        }
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
            SaveLoad.cur_setting.highscore = highScore;
            SaveLoad.Save(); 
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

    public void UseItem(int num)
    {
        if(items_count[num] > 0)
        {
            items_count[num] -= 1;
            UpdateItemCount(num);
            switch (num)
            {
                case 0:
                    force_manager.TurnOnForces();
                    break;
                case 1:
                    main.StopSimulation();
                    question_num -= 1;
                    GenerateNewQuestion();
                    break;
                case 2:
                    is2x = true;
                    scoreTxt.color = Color.yellow;
                    break;
            }
        }
    }
    public void ShowInformation()
    {
        if (cur_info != null)
            Destroy(cur_info);
        if(targetProblem.info_pref != null)
            cur_info = Instantiate(targetProblem.info_pref,menu_con.Pages[1].transform);
    }

    public void OpenProblemSetting()
    {
        problemSetting.SetActive(true);
    }
}

