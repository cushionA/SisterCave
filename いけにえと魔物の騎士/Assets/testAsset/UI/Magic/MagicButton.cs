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
        GameObject ev = GameObject.Find("EventSystem");
      //  eventSystem = ev.GetComponent<EventSystem>();
       // stIn = ev.GetComponent<StandaloneInputModule>();
    }

    // Update is called once per frame
    void Update()
    {
        //  if(item.inventoryNum <= 0)
        //  {
        //      MainUI.instance.UseReBuild();
        //   }

        if (!MagicManager.instance.isUseMenu)//選択ウィンドウ出てないなら
        {
            MagicManager.instance.selectButton = MainUI.instance.eventSystem.currentSelectedGameObject;

            //  Debug.Log($"青い稲妻{MagicManager.instance.selectButton == this.gameObject}");
            if (MagicManager.instance.selectButton == this.gameObject && !isFirst)
            {
                //Debug.Log("冥王星");
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
                //  Debug.Log("サイパン");
                if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction17))
                {
                    MainUI.instance.magicWindow.SetActive(false);
                    MainUI.instance.eqWindow.SetActive(true);
                    if (MagicManager.instance.isKnightM && !MagicManager.instance.isSisterM)
                    {
                        // EquipmentManager.instance.EqWepon[ToolManager.instance.setNumber].Select();
                        //魔法選択画面のインベントリ消した後選ぶボタンを入れる
                    }
                    else if (!MagicManager.instance.isKnightM && MagicManager.instance.isSisterM)
                    {
                        // EquipmentManager.instance.EqShield[ToolManager.instance.setNumber].Select();
                        //魔法選択画面のインベントリ消した後選ぶボタンを入れる
                    }
                    MagicManager.instance.isKnightM = false;
                    MagicManager.instance.isSisterM = false;
                    MainUI.instance.ButtonOn();
                    //MainUI.instance.eqButton.SetActive(true);
                }
                else if (!MainUI.instance.menuButtonOff)
                {
                    MainUI.instance.ButtonOff();
                    // MainUI.instance.eqButton.SetActive(false);
                }
            }


            // MainUI.instance.eqButton.SetActive(false);
        }


    }

    public void CallWindow()
    {
        //  Debug.Log($"お名前教えて{MagicManager.instance.selectButton.name}");
        if (MagicManager.instance.selectItem.inventoryNum > 0 && item != null && MagicManager.instance.selectButton == this.gameObject)
        {

            if (!MagicManager.instance.isKnightM && !MagicManager.instance.isSisterM)
            {
                MagicManager.instance.selectWindow.SetActive(true);
                MagicManager.instance.selectWindow.transform.parent = MagicManager.instance.selectButton.transform;
                MagicManager.instance.selectWindow.GetComponent<RectTransform>().anchoredPosition = position;
            }
            else
            {
                MagicManager.instance.equipWindow.SetActive(true);
                //処理分けは装備窓の方で
                MagicManager.instance.equipWindow.transform.parent = MagicManager.instance.selectButton.transform;
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
        MagicManager.instance.selectButton.GetComponent<Button>().Select();
        MagicManager.instance.isUseMenu = false;
    }


}
