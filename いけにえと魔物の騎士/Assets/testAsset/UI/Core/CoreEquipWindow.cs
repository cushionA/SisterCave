using UnityEngine.UI;
using UnityEngine;

public class CoreEquipWindow : MonoBehaviour
{

    UseButtom ub;
    bool isFirst;
    public Button eqButton;

    // Start is called before the first frame update
    void Start()
    {
        ub = CoreManager.instance.selectButton.GetComponent<UseButtom>();
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

            CoreManager.instance.isUseMenu = false;
            ub.CancelWindow();//選択ウィンドウを消す
        }


        if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction14))
        {
            ResetWindow();
        }

    }

    public void Equip()
    {
        GManager.instance.pStatus.equipCore = CoreManager.instance.selectItem;
        CoreManager.instance.isUseMenu = false;
        this.gameObject.SetActive(false);
        MainUI.instance.MenuCancel();
        MainUI.instance.coreWindow.SetActive(false);
        MainUI.instance.eqWindow.SetActive(true);
        EquipmentManager.instance.EqCore.Select();
        isFirst = false;
        MainUI.instance.ButtonOn();
        //  MainUI.instance.eqButton.SetActive(true);
    }
    public void ResetWindow()
    {
        isFirst = false;
        CoreManager.instance.isUseMenu = false;
        this.gameObject.SetActive(false);
        MainUI.instance.MenuCancel();
        MainUI.instance.ButtonOn();
        //  CoreManager.instance.selectButton = null;
        //    CoreManager.instance.selectItem = null;
    }
}
