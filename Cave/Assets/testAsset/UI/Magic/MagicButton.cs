using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MagicButton : MonoBehaviour
{
    [HideInInspector]
    public Magic item;

    Vector2 position = new Vector2(70, -50);
    bool isFirst;


   // EventSystem eventSystem;
    //StandaloneInputModule stIn;


    // Start is called before the first frame update

    private void Start()
    {
        //GameObject ev = GameObject.Find("EventSystem");
      //  eventSystem = ev.GetComponent<EventSystem>();
       // stIn = ev.GetComponent<StandaloneInputModule>();
    }

    // Update is called once per frame
    void Update()
    {
        //  if(5 <= 0)
        //  {
        //      MainUICon.instance.UseReBuild();
        //   }

        if (!MagicManager.instance.isUseMenu)//選択ウィンドウ出てないなら
        {
            MainUICon.instance.selectButton = MainUICon.instance.eventSystem.currentSelectedGameObject;

            //  ////Debug.log($"青い稲妻{MainUICon.instance.selectButton == this.gameObject}");
            if (MainUICon.instance.selectButton == this.gameObject && !isFirst)
            {
                //////Debug.log("冥王星");
                MagicManager.instance.selectItem = item;
                //position = this.GetComponent<RectTransform>().anchoredPosition;
                isFirst = true;
            }
            else
            {
                isFirst = false;
            }
            if (MagicManager.instance.isSisterM || MagicManager.instance.isSisterM)
            {
                //  ////Debug.log("サイパン");
                if (GManager.instance.InputR.GetButtonDown(MainUICon.instance.rewiredAction17))
                {
                  //  MainUICon.instance.magicWindow.SetActive(false);
                  //  MainUICon.instance.eqWindow.SetActive(true);
                    if (MagicManager.instance.isKnightM && !MagicManager.instance.isSisterM)
                    {
                        // EquipmentManager.instance.EqWeapon[ToolManager.instance.setNumber].Select();
                        //魔法選択画面のインベントリ消した後選ぶボタンを入れる
                    }
                    else if (!MagicManager.instance.isKnightM && MagicManager.instance.isSisterM)
                    {
                        // EquipmentManager.instance.EqShield[ToolManager.instance.setNumber].Select();
                        //魔法選択画面のインベントリ消した後選ぶボタンを入れる
                    }
                    MagicManager.instance.isKnightM = false;
                    MagicManager.instance.isSisterM = false;
                    MainUICon.instance.ButtonOn();
                    //MainUICon.instance.eqButton.SetActive(true);
                }
                else if (!MainUICon.instance.menuButtonOff)
                {
                    MainUICon.instance.ButtonOff();
                    // MainUICon.instance.eqButton.SetActive(false);
                }
            }


            // MainUICon.instance.eqButton.SetActive(false);
        }


    }

    public void CallWindow()
    {
        //  ////Debug.log($"お名前教えて{MainUICon.instance.selectButton.name}");
        if (5 > 0 && item != null && MainUICon.instance.selectButton == this.gameObject)
        {

            if (!MagicManager.instance.isKnightM && !MagicManager.instance.isSisterM)
            {
                MagicManager.instance.selectWindow.SetActive(true);
                MagicManager.instance.selectWindow.transform.parent = MainUICon.instance.selectButton.transform;
                MagicManager.instance.selectWindow.GetComponent<RectTransform>().anchoredPosition = position;
            }
            else
            {
                MagicManager.instance.equipWindow.SetActive(true);
                //処理分けは装備窓の方で
                MagicManager.instance.equipWindow.transform.parent = MainUICon.instance.selectButton.transform;
                MagicManager.instance.equipWindow.GetComponent<RectTransform>().anchoredPosition = position;
            }

            MagicManager.instance.isUseMenu = true;
        }
        else
        {

            //スカの音
        }
    }

    public void CancelWindow()
    {
        if (!MagicManager.instance.isKnightM && !MagicManager.instance.isSisterM)
        {
            MagicManager.instance.selectWindow.SetActive(false);
        }
        else
        {
            MagicManager.instance.equipWindow.SetActive(false);
        }
        isFirst = false;
        MainUICon.instance.selectButton.GetComponent<Button>().Select();
        MagicManager.instance.isUseMenu = false;
    }


}
