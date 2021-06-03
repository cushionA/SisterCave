﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LibraryButton : MonoBehaviour
{
    [HideInInspector]
    public EnemyLibrary item;

    Vector2 position = new Vector2(70, -50);
    bool isFirst;
    

  //  EventSystem eventSystem;
  //  StandaloneInputModule stIn;


    // Start is called before the first frame update

    private void Start()
    {
        //GameObject ev = GameObject.Find("EventSystem");
      //  eventSystem = ev.GetComponent<EventSystem>();
    //    stIn = ev.GetComponent<StandaloneInputModule>();
    }

    // Update is called once per frame
    void Update()
    {
        //  if(item.inventoryNum <= 0)
        //  {
        //      MainUI.instance.UseReBuild();
        //   }

        if (!EnemyDataManager.instance.isUseMenu)//選択ウィンドウ出てないなら
        {
            MainUI.instance.selectButton = MainUI.instance.eventSystem.currentSelectedGameObject;

            //  ////Debug.log($"青い稲妻{MainUI.instance.selectButton == this.gameObject}");
            if (MainUI.instance.selectButton == this.gameObject && !isFirst)
            {
                //////Debug.log("冥王星");
                EnemyDataManager.instance.selectItem = item;
                //position = this.GetComponent<RectTransform>().anchoredPosition;
                isFirst = true;
            }
            else
            {
                isFirst = false;
            }
        }


    }

    public void CallWindow()
    {
        //  ////Debug.log($"お名前教えて{MainUI.instance.selectButton.name}");
        if (EnemyDataManager.instance.selectItem.inventoryNum > 0 && item != null && MainUI.instance.selectButton == this.gameObject)
        {
            EnemyDataManager.instance.selectItem = item;
            // position = this.GetComponent<RectTransform>().anchoredPosition;
            EnemyDataManager.instance.selectWindow.SetActive(true);
            //EnemyDataManager.instance.selectWindow.GetComponent<RectTransform>().anchoredPosition = position;
            //EnemyDataManager.instance.selectWindow.GetComponent<RectTransform>().parent = MainUI.instance.selectButton.GetComponent<RectTransform>();
            EnemyDataManager.instance.selectWindow.transform.parent = MainUI.instance.selectButton.transform;
            EnemyDataManager.instance.selectWindow.GetComponent<RectTransform>().anchoredPosition = position;

            EnemyDataManager.instance.isUseMenu = true;
        }
        else
        {

            //スカの音
        }
    }

    public void CancelWindow()
    {
        EnemyDataManager.instance.selectWindow.SetActive(false);
        isFirst = false;
        MainUI.instance.selectButton.GetComponent<Button>().Select();
        EnemyDataManager.instance.isUseMenu = false;
    }

}