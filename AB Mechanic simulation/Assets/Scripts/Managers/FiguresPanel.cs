using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FiguresPanel : MonoBehaviour
{
    public int menupage;
    public Transform figureContents;
    public GameObject figurePad;
    public Text pagenum;
    public ObjectDatacenter datacenter;
    public MainWorkSpace main;
    public MenuController menu;
    int maxPage;
    // Start is called before the first frame update
    void Start()
    {
        maxPage = datacenter.objects.Length / 6;
        SetPage(0);
        GenerateFigurePanel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangePage(int change)
    {
        if(change == 1)
        {
            if(menupage < maxPage-1)
            {
                menupage += change;
                GenerateFigurePanel();
            }
        }else if(change == -1)
        {
            if (menupage >0)
            {
                menupage += change;
                GenerateFigurePanel();
            }
        }
        pagenum.text = (menupage + 1).ToString();
    }

    public void SetPage(int val)
    {
        menupage = val;
        pagenum.text = (menupage+1).ToString();
    }

    public void GenerateFigurePanel()
    {
        int offset = menupage * 6;
        int max = datacenter.objects.Length;
        for (int i = 0; i < 6; i++)
        {
            int num = offset + i;
            if (num >= max)
                break;
            GameObject a = Instantiate(figurePad, figureContents);
            ObjectData data = datacenter.objects[num];
            FigurePad pad = a.GetComponent<FigurePad>();
            pad.figure_pic.sprite = data.icon;
            pad.figure_name.text = data.Name;
            pad.figure_num = num;
            pad.main = this;
            pad.SetStart();
        }
    }

    public void ChooseFigure(int num)
    {
        main.SpawnObject(datacenter.objects[num]);
        menu.OpenPage(0);
    }
}
