using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public int MenuIndex = 0;
    public Stack<int> page_stack;
    public GameObject[] Pages;
    public GameObject SubMenu;
    public  SceneCameraController CamController;
    // Start is called before the first frame update
    void Start()
    {
        page_stack = new Stack<int>();
        OpenPage(0);
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
                if(num == 2)
                {
                    SceneManager.LoadScene("Creative", LoadSceneMode.Single);
                }
                //num 2 for open creative mode
                //num 3 for open survival mode
                //num 4 for open setting page
                break;
            case 2:
                for (int i = 0; i < Pages.Length; i++)
                {
                    Pages[i].SetActive(num == i);
                }
                SubMenu.SetActive(false);
                CamController.enableRotate = (num != 2);
                if (num == 3)
                {
                    SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                }
                break;
        }
    }

    public void OpenSubMenu()
    {
        SubMenu.SetActive(!SubMenu.activeSelf);
    }
}
