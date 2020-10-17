﻿using System.Collections;
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

            if (Input.GetButtonDown("Cancel"))
            {
                isDump = false;
                uw.isDump = false;
                uw.thirdButton.Select();
                this.gameObject.SetActive(false);
            }
        }
        else if (isSUse)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                isSUse = false;
                uw.isSUse = false;
                uw.secondButton.Select();
                this.gameObject.SetActive(false);
            }
        }

        if (Input.GetButtonDown("Menu"))
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
        ToolManager.instance.selectButton.GetComponent<Button>().Select();
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
 //       ToolManager.instance.selectButton = null;
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
