using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public int MenuIndex = 0;
    public Stack<int> page_stack;
    public GameObject[] Pages;
    public GameObject SubMenu;
    public SceneCameraController CamController;
    public GameObject Error_pref;
    public Canvas canvas;
    public MainWorkSpace mainworlspace;
    public GameObject soundOn;
    public GameObject soundOff;
    // Start is called before the first frame update
    void Start()
    {
        if(SaveLoad.issetting == false)
        {
            LoadSetting();
        }
        mainworlspace = GetComponent<MainWorkSpace>();
        page_stack = new Stack<int>();
        OpenPage(0);
        SoundEditor se = FindObjectOfType<SoundEditor>();
        if (se != null)
        {
            se.UpdateSetting();
        }
    }
    void LoadSetting()
    {
        SaveLoad.Load();
        if(SaveLoad.issetting == false)
        {
            NewSetting();
        }

    }
    void NewSetting()
    {
        SettingSaveFile s = new SettingSaveFile();
        s.highscore = 0;
        s.volume = 0.8f;
        SaveLoad.cur_setting = s;
        SaveLoad.Save();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenPage(int num)
    {
        if(num == -1)
        {
            //back to previous page
            page_stack.Pop(); 
            int previousPage = page_stack.Peek();
            OpenPage(previousPage);
            return;
        }
        else
        {
            page_stack.Push(num);
        }
        Debug.Log("Open page "+num);
        switch (MenuIndex) {
            case 0:
                for(int i = 0; i < Pages.Length; i++)
                {
                    Pages[i].SetActive(num == i);
                }
                if(num == 3)
                {
                    SceneManager.LoadScene("Creative", LoadSceneMode.Single);
                }
                if(num == 4)
                {
                    SceneManager.LoadScene("ProblemMode", LoadSceneMode.Single);
                }
                if(num == 5)
                {
                    SceneManager.LoadScene("Creative_AR", LoadSceneMode.Single);
                }
                if (num == 6)
                {
                    SceneManager.LoadScene("ProblemMode_AR", LoadSceneMode.Single);
                }
                //num 2 for open creative mode
                //num 3 for open survival mode
                //num 4 for open setting page
                break;
            case 1:
                for (int i = 0; i < Pages.Length; i++)
                {
                    Pages[i].SetActive(num == i);
                }
                if(num == 1)
                {
                    Pages[2].SetActive(true);
                }
                if (num == 4)
                {
                    SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                }
                break;
            case 2:
                if (num == 2 && MainWorkSpace.isSimulate)
                {
                    DisplayError("Stop simulation first!");
                    return;
                }
                if (MainWorkSpace.isRecalculateRequire)
                {
                    DisplayError("Tap pull/push to recalculate");
                    return;
                }
                for (int i = 0; i < Pages.Length; i++)
                {
                    Pages[i].SetActive(num == i);
                }
                SubMenu.SetActive(false);
                CamController.enableRotate = (num != 2);
                if (num == 4)
                {
                    SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                }
                break;
        }
    }

    public void DisplayError(string mes)
    {
        GameObject a = Instantiate(Error_pref, canvas.transform);
        a.GetComponent<Text>().text = mes;
    }

    public void OpenSubMenu()
    {
        SubMenu.SetActive(!SubMenu.activeSelf);
    }

}
