using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject mainMenuPage;
    public GameObject modeSelectionPage;
    // Start is called before the first frame update
    void Start()
    {
        OpenPage(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenPage(int num)
    {
        mainMenuPage.SetActive(num == 0);
        modeSelectionPage.SetActive(num == 1);
    }
}
