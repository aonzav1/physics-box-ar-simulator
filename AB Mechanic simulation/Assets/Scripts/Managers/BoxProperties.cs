using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxProperties : MonoBehaviour
{
    public ObjectEditorController objEditor;
    public Text[] propertie_txt;
    public Slider[] propertie_slider;
    public float[] propertie_value;
    public string[] backword;
    public PhysicsObject target_Object; //target physics box
    // Start is called before the first frame update
    void Start()
    {
        LoadValue();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < propertie_txt.Length; i++)
        {
            propertie_value[i] = propertie_slider[i].value;
            propertie_txt[i].text = propertie_value[i] + backword[i];
        }
    }

    public void LoadValue()
    {
        //load from save
        for(int i = 0; i < propertie_txt.Length; i++)
        {
            propertie_value[i] = target_Object.properties[i];
            propertie_slider[i].value = propertie_value[i];
        }
    }
    public void Cancel()
    {
        objEditor.CloseEditProperties();
    }
    public void Save()
    {
        target_Object.NewProperties(propertie_value);
        objEditor.CloseEditProperties();
    }
}