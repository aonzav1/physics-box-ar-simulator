using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    public bool isSelected;
    Outline thisOutline;
    // Start is called before the first frame update
    void Start()
    {
        thisOutline = GetComponent<Outline>();
        UpdateSelection(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   public void UpdateSelection(bool isSelect)
    {
        isSelected = isSelect;
        if (isSelected)
        {
            thisOutline.enabled = true;
        }
        else
        {
            thisOutline.enabled = false;
        }
    }
}
