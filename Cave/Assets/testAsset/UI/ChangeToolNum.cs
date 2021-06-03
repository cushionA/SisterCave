using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeToolNum : MonoBehaviour
{

    [HideInInspector]public UseWindow uw;
    [HideInInspector] public bool isDump;
    [HideInInspector] public bool isSUse;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDump)
        {

            if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction17))
            {
                isDump = false;
                uw.isDump = false;
                uw.thirdButton.Select();
                this.gameObject.SetActive(false);
            }
        }
        else if (isSUse)
        {
            if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction17))
            {
                isSUse = false;
                uw.isSUse = false;
                uw.secondButton.Select();
                this.gameObject.SetActive(false);
            }
        }

        if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction14))
        {
            ResetFlag();
            uw.ResetWindow();
        }
    }

    public void DumpAct()
    {
        isDump = false;
        uw.isDump = false;
        ToolManager.instance.DumpTool();
        ToolManager.instance.changeNum = 1;
        MainUI.instance.selectButton.GetComponent<Button>().Select();
        uw.ResetWindow();
        this.gameObject.SetActive(false);
        MainUI.instance.MenuCancel();
    }
    public void SUseAct()
    {
        isSUse = false;
        ToolManager.instance.Use();
        ToolManager.instance.changeNum = 1;
        uw.ResetWindow();
        this.gameObject.SetActive(false);
        MainUI.instance.MenuCancel();
    }

    public void ResetFlag()
    {
 //       MainUI.instance.selectButton = null;
   //     ToolManager.instance.selectItem = null;
        isDump = false;
        isSUse = false;
        this.gameObject.SetActive(false);
    }

    public void ActChange()
    {
        if (isSUse)
        {
            SUseAct();
        }
        else if (isDump)
        {
            DumpAct();
        }

    }

}
