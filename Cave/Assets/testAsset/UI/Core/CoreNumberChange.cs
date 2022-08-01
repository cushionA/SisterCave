using UnityEngine.UI;
using UnityEngine;

public class CoreNumberChange : MonoBehaviour
{

    [HideInInspector] public CoreWindow uw;
    [HideInInspector] public bool isDump;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isDump)
        {

            if (GManager.instance.InputR.GetButtonDown(MainUICon.instance.rewiredAction17))
            {
                isDump = false;
                uw.isDump = false;
                uw.thirdButton.Select();
                this.gameObject.SetActive(false);
            }
        }


        if (GManager.instance.InputR.GetButtonDown(MainUICon.instance.rewiredAction14))
        {
            ResetFlag();
            uw.ResetWindow();
        }
    }

    public void DumpAct()
    {
        isDump = false;
        uw.isDump = false;
        CoreManager.instance.DumpCore();
        CoreManager.instance.changeNum = 1;
        MainUICon.instance.selectButton.GetComponent<Button>().Select();
        uw.ResetWindow();
        this.gameObject.SetActive(false);
        MainUICon.instance.MenuCancel();
    }

    public void ResetFlag()
    {
        //       MainUICon.instance.selectButton = null;
        //     CoreManager.instance.selectItem = null;
        isDump = false;
        this.gameObject.SetActive(false);
    }


}

