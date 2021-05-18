using UnityEngine;
using UnityEngine.UI;

public class EquipWindow : MonoBehaviour
{

    public Button thirdButton;
    public Button changeButton;
    public GameObject numChange;
    [HideInInspector] public bool isNum;

    EquipButton ub;
    bool isFirst;

    [HideInInspector] public bool isDump;



    // Start is called before the first frame update
    void Start()
    {
        ub = MainUI.instance.selectButton.GetComponent<EquipButton>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFirst)
        {
            thirdButton.Select();
            isFirst = true;
        }

        if (!isDump && GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction17))
        {
            isNum = false;
            isDump = false;
            isFirst = false;
            EquipManager.instance.isUseMenu = false;
            ub.CancelWindow();//選択ウィンドウを消す
        }

        if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction14) && !isNum)
        {
            ResetWindow();
        }

    }

    /// <summary>
    /// ここで変更窓出してエンターの入力を待つ
    /// </summary>
    public void DumpSelect()
    {

        ChangeEquipNum ct;
        EquipManager.instance.changeNum = 1;
        isDump = true;
        numChange.SetActive(true);
        numChange.GetComponent<Button>().Select();
        ct = numChange.GetComponent<ChangeEquipNum>();
        ct.isDump = true;
        ct.uw = this;
        isNum = true;
    }


    public void ResetWindow()
    {
        isNum = false;
        isDump = false;
        isFirst = false;
        EquipManager.instance.isUseMenu = false;
        this.gameObject.SetActive(false);
        MainUI.instance.MenuCancel();
        //  MainUI.instance.selectButton = null;
        //    EquipManager.instance.selectItem = null;
    }

}
