using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxProperties : MonoBehaviour
{
    public ObjectEditorController objEditor;
    public Text[] propertie_txt;
    public Slider mass_slider;
    public InputField[] inputFields;
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
        propertie_value[0] = mass_slider.value;
        propertie_txt[0].text = propertie_value[0] + backword[0];
    }

    public void LoadValue()
    {
        //load from save
        for(int i = 0; i < propertie_value.Length; i++)
        {
            propertie_value[i] = target_Object.properties[i];
            if(i==0)
                mass_slider.value = propertie_value[i];
            else
                inputFields[i-1].text = propertie_value[i].ToString();
        }
    }

    public void SetNewValue()
    {
        for (int i = 1; i < propertie_value.Length; i++)
        {
            if(inputFields[i - 1].text != "")
                propertie_value[i] = float.Parse(inputFields[i-1].text);
            if(propertie_value[i] < 0.001)
            {
                propertie_value[i] = 0.001f;
            }
        }
    //    Debug.Log("New value set!");
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
