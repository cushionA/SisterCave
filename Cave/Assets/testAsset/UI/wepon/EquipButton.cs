using UnityEngine;

using UnityEngine.UI;

public class EquipButton : MonoBehaviour
{
    [HideInInspector]
    public Equip item;

    Vector2 position = new Vector2(70, -50);
    bool isFirst;





    // Start is called before the first frame update

    private void Start()
    {
        //GameObject ev = GameObject.Find("EventSystem");
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
            MainUI.instance.selectButton = MainUI.instance.eventSystem.currentSelectedGameObject;

          //  ////Debug.log($"青い稲妻{MainUI.instance.selectButton == this.gameObject}");
            if (MainUI.instance.selectButton == this.gameObject && !isFirst)
            {
                //////Debug.log("冥王星");
                EquipManager.instance.selectItem = item;
                //position = this.GetComponent<RectTransform>().anchoredPosition;
                isFirst = true;
            }
            else
            {
                isFirst = false;
            }


            if (EquipManager.instance.isWeaponM || EquipManager.instance.isShieldM)
            {
              //  ////Debug.log("サイパン");
                if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction17))
                {
                    MainUI.instance.weaponWindow.SetActive(false);
                    MainUI.instance.eqWindow.SetActive(true);
                    if (EquipManager.instance.isWeaponM && !EquipManager.instance.isShieldM)
                    {
                        EquipmentManager.instance.EqWeapon[ToolManager.instance.setNumber].Select();
                    }
                    else if (!EquipManager.instance.isWeaponM && EquipManager.instance.isShieldM)
                    {
                        EquipmentManager.instance.EqShield[ToolManager.instance.setNumber].Select();
                    }
                    EquipManager.instance.isWeaponM = false;
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
      //  ////Debug.log($"お名前教えて{MainUI.instance.selectButton.name}");
        if (EquipManager.instance.selectItem.inventoryNum > 0 && item != null && MainUI.instance.selectButton == this.gameObject)
        {
            EquipManager.instance.selectItem = item;
            // position = this.GetComponent<RectTransform>().anchoredPosition;
            if (!EquipManager.instance.isWeaponM && !EquipManager.instance.isShieldM)
            {
                EquipManager.instance.selectWindow.SetActive(true);
                EquipManager.instance.selectWindow.transform.parent = MainUI.instance.selectButton.transform;
                EquipManager.instance.selectWindow.GetComponent<RectTransform>().anchoredPosition = position;

            }
            else
            {
                EquipManager.instance.equipWindow.SetActive(true);
                //処理分けは装備窓の方で
                EquipManager.instance.equipWindow.transform.parent = MainUI.instance.selectButton.transform;
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
        if (!EquipManager.instance.isWeaponM && !EquipManager.instance.isShieldM)
        {
            EquipManager.instance.selectWindow.SetActive(false);
        }
        else
        {
            EquipManager.instance.equipWindow.SetActive(false);
        }

        isFirst = false;
        
        MainUI.instance.selectButton.GetComponent<Button>().Select();
        EquipManager.instance.isUseMenu = false;
    }


}
