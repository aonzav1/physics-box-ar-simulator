using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad
{
    public static bool issetting = false;
    public static SettingSaveFile cur_setting = new SettingSaveFile();
    //it's static so we can call it from anywhere
    public static void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/PhysSimSave"); 
        bf.Serialize(file, cur_setting);
        file.Close();
        issetting = true;
        Debug.Log("Setting saved!");
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/PhysSimSave"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/PhysSimSave", FileMode.Open);
            cur_setting = (SettingSaveFile)bf.Deserialize(file);
            file.Close();
            issetting = true;
            Debug.Log("Setting loaded!");
        }
    }
}