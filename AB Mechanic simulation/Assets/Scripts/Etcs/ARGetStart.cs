using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ARGetStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseThis()
    {
        gameObject.SetActive(false);
    }

    public void BackToMain()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
