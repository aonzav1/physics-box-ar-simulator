using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationScript : MonoBehaviour
{
    public Text[] allCustomText;
    public int maxObjectText;
    public MainWorkSpace main;
    // Start is called before the first frame update
    void Start()
    {
        main = FindObjectOfType<MainWorkSpace>();
        SetupCustomTexts();
    }
    public void CloseTab()
    {
        main.visiblecontroler.CloseInformation();
    }

    void SetupCustomTexts()
    {
        int k = 0;
        for(int i = 0; i < maxObjectText; i++)
        {
            if(main.tmp_spawned[i].type == ObjectType.Box)
            {
                if (allCustomText[k] == null)
                    return;
                allCustomText[k].text = main.tmp_spawned[i].properties[1].ToString("F2");
                k += 1;
                allCustomText[k].text = main.tmp_spawned[i].properties[2].ToString("F2");
                k += 1;
            }
        }
    }
}
