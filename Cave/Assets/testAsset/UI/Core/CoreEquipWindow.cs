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
        ub = MainUICon.instance.selectButton.GetComponent<UseButtom>();
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
        MainUICon.instance.MenuCancel();
      //  MainUICon.instance.coreWindow.SetActive(false);
      //  MainUICon.instance.eqWindow.SetActive(true);
        EquipmentManager.instance.EqCore.Select();
        isFirst = false;
        MainUICon.instance.ButtonOn();
        //  MainUICon.instance.eqButton.SetActive(true);
    }
    public void ResetWindow()
    {
        isFirst = false;
        CoreManager.instance.isUseMenu = false;
        this.gameObject.SetActive(false);
        MainUICon.instance.MenuCancel();
        MainUICon.instance.ButtonOn();
        //  MainUICon.instance.selectButton = null;
        //    CoreManager.instance.selectItem = null;
    }
}
