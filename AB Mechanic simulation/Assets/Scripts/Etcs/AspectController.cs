using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AspectController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float ratio = Screen.height/ (float)Screen.width;
        Debug.Log(ratio+" VS "+ 16 / 9f);
        if (ratio < 1.8f)
        {
            GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        }else if(ratio == 2)
        {
            GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1215);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
