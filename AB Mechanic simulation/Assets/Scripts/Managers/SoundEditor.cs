using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEditor : MonoBehaviour
{
    public GameObject off;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SettingSound()
    {
        if (SaveLoad.cur_setting.volume > 0)
        {
            SaveLoad.cur_setting.volume = 0;
        }
        else
        {
            SaveLoad.cur_setting.volume = 0.8f;
        }
        UpdateSetting();
    }
    public void UpdateSetting()
    {
        if(SaveLoad.cur_setting.volume > 0)
        {
            off.SetActive(false);
        }
        else
        {
            off.SetActive(true);
        }
    }
}
