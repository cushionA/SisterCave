using UnityEngine;
using UnityEngine.UI;

public class WSEquip : MonoBehaviour
{

    EquipButton ub;
    bool isFirst;
    public Button eqButton;

    // Start is called before the first frame update
    void Start()
    {
        ub = MainUICon.instance.selectButton.GetComponent<EquipButton>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFirst)
        {
            eqButton.Select();
            isFirst = true;
        }

        if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction17))
        {

            EquipManager.instance.isUseMenu = false;
            ub.CancelWindow();//選択ウィンドウを消す
        }


        if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction14))
        {
            ResetWindow();
        }

    }

    public void Equip()
    {
        EquipManager.instance.isUseMenu = false;
        this.gameObject.SetActive(false);
        MainUICon.instance.MenuCancel();
     //   MainUICon.instance.weaponWindow.SetActive(false);
     //   MainUICon.instance.eqWindow.SetActive(true);
        if (EquipManager.instance.isWeaponM)
        {
            GManager.instance.setWeapon[EquipManager.instance.setNumber] = EquipManager.instance.selectItem as MyCode.Weapon;
            EquipmentManager.instance.EqWeapon[EquipManager.instance.setNumber].Select();
        }
        else
        {
            GManager.instance.setShield[EquipManager.instance.setNumber] = EquipManager.instance.selectItem as Shield;
            EquipmentManager.instance.EqShield[EquipManager.instance.setNumber].Select();
        }
        EquipManager.instance.isWeaponM = false;
        EquipManager.instance.isShieldM = false;
        isFirst = false;
        MainUICon.instance.ButtonOn();
        //  MainUICon.instance.eqButton.SetActive(true);
    }
    public void ResetWindow()
    {
        isFirst = false;
        EquipManager.instance.isUseMenu = false;
        this.gameObject.SetActive(false);
        MainUICon.instance.MenuCancel();
        MainUICon.instance.ButtonOn();
        //  MainUICon.instance.selectButton = null;
        //    EquipManager.instance.selectItem = null;
    }
}
