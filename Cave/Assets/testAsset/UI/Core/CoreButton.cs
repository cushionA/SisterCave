﻿using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.EventSystems;

public class CoreButton : MonoBehaviour
{
    [HideInInspector]
    public CoreItem item;

    Vector2 position = new Vector2(70, -50);
    bool isFirst;


 //   EventSystem eventSystem;
    //StandaloneInputModule stIn;


    // Start is called before the first frame update

    private void Start()
    {
        //GameObject ev = GameObject.Find("EventSystem");
       // eventSystem = ev.GetComponent<EventSystem>();
      //  stIn = ev.GetComponent<StandaloneInputModule>();
    }

    // Update is called once per frame
    void Update()
    {
        //  if(item.inventoryNum <= 0)
        //  {
        //      MainUI.instance.UseReBuild();
        //   }

        if (!CoreManager.instance.isUseMenu)//選択ウィンドウ出てないなら
        {
            MainUI.instance.selectButton = MainUI.instance.eventSystem.currentSelectedGameObject;

            //  ////Debug.log($"青い稲妻{MainUI.instance.selectButton == this.gameObject}");
            if (MainUI.instance.selectButton == this.gameObject && !isFirst)
            {
                //////Debug.log("冥王星");
                CoreManager.instance.selectItem = item;
                //position = this.GetComponent<RectTransform>().anchoredPosition;
                isFirst = true;
            }
            else
            {
                isFirst = false;
            }
            if (CoreManager.instance.isEquipMenu)
            {
                if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction17))
                {
                    ////Debug.log("サイパン");
                    MainUI.instance.useWindow.SetActive(false);
                    MainUI.instance.eqWindow.SetActive(true);
                    EquipmentManager.instance.EqCore.Select();
                    MainUI.instance.ButtonOn();
                    //MainUI.instance.eqButton.SetActive(true);
                }
                else if(!MainUI.instance.menuButtonOff)
                {
                    MainUI.instance.ButtonOff();
                    // MainUI.instance.eqButton.SetActive(false);
                }
            }
        }


    }

    public void CallWindow()
    {
        //  ////Debug.log($"お名前教えて{MainUI.instance.selectButton.name}");
        if (CoreManager.instance.selectItem.inventoryNum > 0 && item != null && MainUI.instance.selectButton == this.gameObject && item != null && !ToolManager.instance.isEquipMenu)
        {
            CoreManager.instance.selectItem = item;
            // position = this.GetComponent<RectTransform>().anchoredPosition;
            CoreManager.instance.selectWindow.SetActive(true);
            //CoreManager.instance.selectWindow.GetComponent<RectTransform>().anchoredPosition = position;
            //CoreManager.instance.selectWindow.GetComponent<RectTransform>().parent = MainUI.instance.selectButton.GetComponent<RectTransform>();
            CoreManager.instance.selectWindow.transform.parent = MainUI.instance.selectButton.transform;
            CoreManager.instance.selectWindow.GetComponent<RectTransform>().anchoredPosition = position;

            CoreManager.instance.isUseMenu = true;
        }
        else if (CoreManager.instance.selectItem.inventoryNum > 0 && item != null && MainUI.instance.selectButton == this.gameObject && CoreManager.instance.isEquipMenu)
        {
            CoreManager.instance.selectItem = item;
            // position = this.GetComponent<RectTransform>().anchoredPosition;
            CoreManager.instance.equipWindow.SetActive(true);
            //CoreManager.instance.selectWindow.GetComponent<RectTransform>().anchoredPosition = position;
            //CoreManager.instance.selectWindow.GetComponent<RectTransform>().parent = MainUI.instance.selectButton.GetComponent<RectTransform>();
            CoreManager.instance.selectWindow.transform.parent = MainUI.instance.selectButton.transform;
            CoreManager.instance.selectWindow.GetComponent<RectTransform>().anchoredPosition = position;

            CoreManager.instance.isUseMenu = true;
        }
        else
        {

            //スカの音
        }
    }

    public void CancelWindow()
    {
        if (!CoreManager.instance.isEquipMenu)
        {
            CoreManager.instance.selectWindow.SetActive(false);
        }
        else
        {
            CoreManager.instance.equipWindow.SetActive(false);
        }
        isFirst = false;
        MainUI.instance.selectButton.GetComponent<Button>().Select();
        CoreManager.instance.isUseMenu = false;
    }


}