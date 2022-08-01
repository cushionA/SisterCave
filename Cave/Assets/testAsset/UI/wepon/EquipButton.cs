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
        //  if(5 <= 0)
        //  {
        //      MainUICon.instance.UseReBuild();
        //   }

        if (!EquipManager.instance.isUseMenu)//選択ウィンドウ出てないなら
        {
            MainUICon.instance.selectButton = MainUICon.instance.eventSystem.currentSelectedGameObject;

          //  ////Debug.log($"青い稲妻{MainUICon.instance.selectButton == this.gameObject}");
            if (MainUICon.instance.selectButton == this.gameObject && !isFirst)
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
                if (GManager.instance.InputR.GetButtonDown(MainUICon.instance.rewiredAction17))
                {
                 //   MainUICon.instance.weaponWindow.SetActive(false);
                //    MainUICon.instance.eqWindow.SetActive(true);
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
            EquipManager.instance.selectItem = item;
            // position = this.GetComponent<RectTransform>().anchoredPosition;
            if (!EquipManager.instance.isWeaponM && !EquipManager.instance.isShieldM)
            {
                EquipManager.instance.selectWindow.SetActive(true);
                EquipManager.instance.selectWindow.transform.parent = MainUICon.instance.selectButton.transform;
                EquipManager.instance.selectWindow.GetComponent<RectTransform>().anchoredPosition = position;

            }
            else
            {
                EquipManager.instance.equipWindow.SetActive(true);
                //処理分けは装備窓の方で
                EquipManager.instance.equipWindow.transform.parent = MainUICon.instance.selectButton.transform;
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
        
        MainUICon.instance.selectButton.GetComponent<Button>().Select();
        EquipManager.instance.isUseMenu = false;
    }


}
