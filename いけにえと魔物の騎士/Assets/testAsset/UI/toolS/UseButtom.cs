using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// カーソルが特定ボタンに合ってる時の処理
/// 押したらその座標にウィンドウが現れたりする
/// </summary>
public class UseButtom : MonoBehaviour
{
    [HideInInspector]
    public ToolItem item;

    Vector2 position = new Vector2(70, -50);
    bool isFirst;


   // EventSystem eventSystem;
   // StandaloneInputModule stIn;


    // Start is called before the first frame update

    private void Start()
    {
        GameObject ev = GameObject.Find("EventSystem");
    //    eventSystem = ev.GetComponent<EventSystem>();
    //    stIn = ev.GetComponent<StandaloneInputModule>();
    }

    // Update is called once per frame
    void Update()
    {
      //  if(item.inventoryNum <= 0)
      //  {
      //      MainUI.instance.UseReBuild();
     //   }


        if (!ToolManager.instance.isUseMenu)
        {
            ToolManager.instance.selectButton = MainUI.instance.eventSystem.currentSelectedGameObject;

            //Debug.Log($"青い稲妻{ToolManager.instance.selectButton == this.gameObject}");
            if (ToolManager.instance.selectButton == this.gameObject && !isFirst)
            {
                //Debug.Log("冥王星");
                ToolManager.instance.selectItem = item;
                //position = this.GetComponent<RectTransform>().anchoredPosition;
                isFirst = true;
            }
            else
            {
                isFirst = false;
            }
            if (ToolManager.instance.isEquipMenu)
            {
                if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction17))
                {
                    Debug.Log("サイパン");
                    MainUI.instance.useWindow.SetActive(false);
                    MainUI.instance.eqWindow.SetActive(true);
                    EquipmentManager.instance.EqItem[ToolManager.instance.setNumber].Select();
                    MainUI.instance.ButtonOn();
                    //MainUI.instance.eqButton.SetActive(true);
                }
                else if (!MainUI.instance.menuButtonOff)
                {
                    MainUI.instance.ButtonOff();
                    // MainUI.instance.eqButton.SetActive(false);
                }
            }
        }


    }

    public void CallWindow()
    {
        if (ToolManager.instance.selectItem.inventoryNum > 0 && item != null && ToolManager.instance.selectButton == this.gameObject && !ToolManager.instance.isEquipMenu)
        {

                ToolManager.instance.selectItem = item;
               // position = this.GetComponent<RectTransform>().anchoredPosition;
                ToolManager.instance.selectWindow.SetActive(true);
             //ToolManager.instance.selectWindow.GetComponent<RectTransform>().anchoredPosition = position;
            //ToolManager.instance.selectWindow.GetComponent<RectTransform>().parent = ToolManager.instance.selectButton.GetComponent<RectTransform>();
            ToolManager.instance.selectWindow.transform.parent = ToolManager.instance.selectButton.transform;
            ToolManager.instance.selectWindow.GetComponent<RectTransform>().anchoredPosition = position;

            ToolManager.instance.isUseMenu = true;
        }
        else if(ToolManager.instance.selectItem.inventoryNum > 0 && item != null && ToolManager.instance.selectButton == this.gameObject && ToolManager.instance.isEquipMenu)
        {
            ToolManager.instance.selectItem = item;
            // position = this.GetComponent<RectTransform>().anchoredPosition;
            ToolManager.instance.equipWindow.SetActive(true);
            //ToolManager.instance.selectWindow.GetComponent<RectTransform>().anchoredPosition = position;
            //ToolManager.instance.selectWindow.GetComponent<RectTransform>().parent = ToolManager.instance.selectButton.GetComponent<RectTransform>();
            ToolManager.instance.equipWindow.transform.parent = ToolManager.instance.selectButton.transform;
            ToolManager.instance.equipWindow.GetComponent<RectTransform>().anchoredPosition = position;

            ToolManager.instance.isUseMenu = true;
        }
        else
        {
            //スカの音
        }
    }

    public void CancelWindow()
    {
        if (!ToolManager.instance.isEquipMenu)
        {
            ToolManager.instance.selectWindow.SetActive(false);
        }
        else if(ToolManager.instance.isEquipMenu)
        {
            ToolManager.instance.equipWindow.SetActive(false);
        }
        isFirst = false;
        ToolManager.instance.selectButton.GetComponent<Button>().Select();
        ToolManager.instance.isUseMenu = false;
    }


}
