using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipButton : MonoBehaviour
{
    [HideInInspector]
    public Equip item;

    Vector2 position = new Vector2(70, -50);
    bool isFirst;


    EventSystem eventSystem;
    StandaloneInputModule stIn;


    // Start is called before the first frame update

    private void Start()
    {
        GameObject ev = GameObject.Find("EventSystem");
        eventSystem = ev.GetComponent<EventSystem>();
        stIn = ev.GetComponent<StandaloneInputModule>();
    }

    // Update is called once per frame
    void Update()
    {
        //  if(item.inventoryNum <= 0)
        //  {
        //      MainUI.instance.UseReBuild();
        //   }

        if (!EquipManager.instance.isUseMenu)//選択ウィンドウ出てないなら
        {
            EquipManager.instance.selectButton = eventSystem.currentSelectedGameObject;

          //  Debug.Log($"青い稲妻{EquipManager.instance.selectButton == this.gameObject}");
            if (EquipManager.instance.selectButton == this.gameObject && !isFirst)
            {
                //Debug.Log("冥王星");
                EquipManager.instance.selectItem = item;
                //position = this.GetComponent<RectTransform>().anchoredPosition;
                isFirst = true;
            }
            else
            {
                isFirst = false;
            }


            if (EquipManager.instance.isWeponM || EquipManager.instance.isShieldM)
            {
              //  Debug.Log("サイパン");
                if (Input.GetButtonDown("Cancel"))
                {
                    MainUI.instance.weponWindow.SetActive(false);
                    MainUI.instance.eqWindow.SetActive(true);
                    if (EquipManager.instance.isWeponM && !EquipManager.instance.isShieldM)
                    {
                        EquipmentManager.instance.EqWepon[ToolManager.instance.setNumber].Select();
                    }
                    else if (!EquipManager.instance.isWeponM && EquipManager.instance.isShieldM)
                    {
                        EquipmentManager.instance.EqShield[ToolManager.instance.setNumber].Select();
                    }
                    EquipManager.instance.isWeponM = false;
                    EquipManager.instance.isShieldM = false;
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
      //  Debug.Log($"お名前教えて{EquipManager.instance.selectButton.name}");
        if (EquipManager.instance.selectItem.inventoryNum > 0 && item != null && EquipManager.instance.selectButton == this.gameObject)
        {
            EquipManager.instance.selectItem = item;
            // position = this.GetComponent<RectTransform>().anchoredPosition;
            if (!EquipManager.instance.isWeponM && !EquipManager.instance.isShieldM)
            {
                EquipManager.instance.selectWindow.SetActive(true);
                EquipManager.instance.selectWindow.transform.parent = EquipManager.instance.selectButton.transform;
                EquipManager.instance.selectWindow.GetComponent<RectTransform>().anchoredPosition = position;

            }
            else
            {
                EquipManager.instance.equipWindow.SetActive(true);
                //処理分けは装備窓の方で
                EquipManager.instance.equipWindow.transform.parent = EquipManager.instance.selectButton.transform;
                EquipManager.instance.equipWindow.GetComponent<RectTransform>().anchoredPosition = position;
            }

            EquipManager.instance.isUseMenu = true;
        }
        else
        {

            //スカの音
        }
    }

    public void CancelWindow()
    {
        if (!EquipManager.instance.isWeponM && !EquipManager.instance.isShieldM)
        {
            EquipManager.instance.selectWindow.SetActive(false);
        }
        else
        {
            EquipManager.instance.equipWindow.SetActive(false);
        }

        isFirst = false;
        EquipManager.instance.selectButton.GetComponent<Button>().Select();
        EquipManager.instance.isUseMenu = false;
    }


}
