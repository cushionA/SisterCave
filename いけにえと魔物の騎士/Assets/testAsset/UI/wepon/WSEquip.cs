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
        ub = EquipManager.instance.selectButton.GetComponent<EquipButton>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFirst)
        {
            eqButton.Select();
            isFirst = true;
        }

        if (Input.GetButtonDown("Cancel"))
        {

            EquipManager.instance.isUseMenu = false;
            ub.CancelWindow();//選択ウィンドウを消す
        }


        if (Input.GetButtonDown("Menu"))
        {
            ResetWindow();
        }

    }

    public void Equip()
    {
        EquipManager.instance.isUseMenu = false;
        this.gameObject.SetActive(false);
        MainUI.instance.MenuCancel();
        MainUI.instance.weponWindow.SetActive(false);
        MainUI.instance.eqWindow.SetActive(true);
        if (EquipManager.instance.isWeponM)
        {
            GManager.instance.setWepon[EquipManager.instance.setNumber] = EquipManager.instance.selectItem as Wepon;
            EquipmentManager.instance.EqWepon[EquipManager.instance.setNumber].Select();
        }
        else
        {
            GManager.instance.setShield[EquipManager.instance.setNumber] = EquipManager.instance.selectItem as Shield;
            EquipmentManager.instance.EqShield[EquipManager.instance.setNumber].Select();
        }
        EquipManager.instance.isWeponM = false;
        EquipManager.instance.isShieldM = false;
        isFirst = false;
        MainUI.instance.ButtonOn();
        //  MainUI.instance.eqButton.SetActive(true);
    }
    public void ResetWindow()
    {
        isFirst = false;
        EquipManager.instance.isUseMenu = false;
        this.gameObject.SetActive(false);
        MainUI.instance.MenuCancel();
        MainUI.instance.ButtonOn();
        //  EquipManager.instance.selectButton = null;
        //    EquipManager.instance.selectItem = null;
    }
}
