using UnityEngine.UI;
using UnityEngine;

public class MaterialWindow : MonoBehaviour
{

    public Button thirdButton;
    public Button changeButton;
    public GameObject numChange;
    [HideInInspector] public bool isNum;

    MaterialButton ub;
    bool isFirst;

    [HideInInspector] public bool isDump;



    // Start is called before the first frame update
    void Start()
    {
        ub = MainUICon.instance.selectButton.GetComponent<MaterialButton>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFirst)
        {
            thirdButton.Select();
            isFirst = true;
        }

        if (!isDump && GManager.instance.InputR.GetButtonDown(MainUICon.instance.rewiredAction17))
        {
            isNum = false;
            isDump = false;
            isFirst = false;
            MaterialManager.instance.isUseMenu = false;
            ub.CancelWindow();//選択ウィンドウを消す
        }

        if (GManager.instance.InputR.GetButtonDown(MainUICon.instance.rewiredAction14) && !isNum)
        {
            ResetWindow();
        }

    }

    /// <summary>
    /// ここで変更窓出してエンターの入力を待つ
    /// </summary>
    public void DumpSelect()
    {

        MaterialNumberChange ct;
        MaterialManager.instance.changeNum = 1;
        isDump = true;
        numChange.SetActive(true);
        numChange.GetComponent<Button>().Select();
        ct = numChange.GetComponent<MaterialNumberChange>();
        ct.isDump = true;
        ct.uw = this;
        isNum = true;
    }


    public void ResetWindow()
    {
        isNum = false;
        isDump = false;
        isFirst = false;
        MaterialManager.instance.isUseMenu = false;
        this.gameObject.SetActive(false);
        MainUICon.instance.MenuCancel();
        //  MainUICon.instance.selectButton = null;
        //    MaterialManager.instance.selectItem = null;
    }

}
