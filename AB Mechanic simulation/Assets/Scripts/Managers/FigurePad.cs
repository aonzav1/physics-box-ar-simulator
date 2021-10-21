using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FigurePad : MonoBehaviour
{
    public Text figure_name;
    public Image figure_pic;
    public int figure_num;
    public Button butt;
    public FiguresPanel main;
    // Start is called before the first frame update
    public void SetStart()
    {
        butt.onClick.AddListener(delegate { ChooseThis(); });
    }
    public void ChooseThis()
    {
        main.ChooseFigure(figure_num);
    }

}
