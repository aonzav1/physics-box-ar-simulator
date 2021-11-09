using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ARmanager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(!VuforiaApplication.Instance.IsInitialized)
            VuforiaApplication.Instance.Initialize();
    }
}
