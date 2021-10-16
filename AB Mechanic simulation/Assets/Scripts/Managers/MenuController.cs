using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public int nowPage = 0;
    public GameObject mainMenuPage;
    public GameObject modeSelectionPage;
    // Start is called before the first frame update
    void Start()
    {
        OpenPage(nowPage);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenPage(int num)
    {
        switch (nowPage) {
            case 0:
                mainMenuPage.SetActive(num == 0);
                modeSelectionPage.SetActive(num == 1);
                //num 2 for open creative mode
                //num 3 for open survival mode
                //num 4 for open setting page
                break;
            case 2:
                break;
        }
    }
}
