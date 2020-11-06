using UnityEngine;
using UnityEngine.UI;

public class ToolEquip : MonoBehaviour
{

    UseButtom ub;
    bool isFirst;
    public Button eqButton;

    // Start is called before the first frame update
    void Start()
    {
        ub = ToolManager.instance.selectButton.GetComponent<UseButtom>();
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

            ToolManager.instance.isUseMenu = false;
            ub.CancelWindow();//選択ウィンドウを消す
        }


        if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction14))
        {
            ResetWindow();
        }

    }

    public void Equip()
    {
        GManager.instance.useList[ToolManager.instance.setNumber] = ToolManager.instance.selectItem;
        ToolManager.instance.isUseMenu = false;
        this.gameObject.SetActive(false);
        MainUI.instance.MenuCancel();
        MainUI.instance.useWindow.SetActive(false);
        MainUI.instance.eqWindow.SetActive(true);
        EquipmentManager.instance.EqItem[ToolManager.instance.setNumber].Select();
        isFirst = false;
        MainUI.instance.ButtonOn();
        //  MainUI.instance.eqButton.SetActive(true);
    }
    public void ResetWindow()
    {
        isFirst = false;
        ToolManager.instance.isUseMenu = false;
        this.gameObject.SetActive(false);
        MainUI.instance.MenuCancel();
        MainUI.instance.ButtonOn();
        //  ToolManager.instance.selectButton = null;
        //    ToolManager.instance.selectItem = null;
    }
}
