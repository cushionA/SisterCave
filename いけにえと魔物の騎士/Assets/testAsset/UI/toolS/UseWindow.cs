﻿using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// セレクトウインドウにつける
/// 
/// </summary>
public class UseWindow : MonoBehaviour
{
    public Button firstButton;
    public Button secondButton;
    public Button thirdButton;
    public Button changeButton;
    public GameObject numChange;
    [HideInInspector]public bool isNum;

    UseButtom ub;
    bool isFirst;

    //bool isUse;
    [HideInInspector]public bool isSUse;
    [HideInInspector]public bool isDump;
    bool isChange;
    private object eventSystem;


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
            firstButton.Select();
            isFirst = true;
        }

        if(!isDump && !isSUse && Input.GetButtonDown("Cancel"))
        {
            isNum = false;
            isSUse = false;
            isDump = false;
            isFirst = false;
            ToolManager.instance.isUseMenu = false;
            ub.CancelWindow();//選択ウィンドウを消す
        }
 /*       else if (isDump)
        {
            thirdButton.Select();
            if (Input.GetButtonDown("Submit"))
            {
                isDump = false;
                numChange.SetActive(false);
                ToolManager.instance.ReduceItem();
                ToolManager.instance.changeNum = 1;
                ToolManager.instance.selectButton.GetComponent<Button>().Select();
            }
            else if (Input.GetButtonDown("Cancel"))
            {
                isDump = false;
                numChange.SetActive(false);
                ToolManager.instance.selectButton.GetComponent<Button>().Select();
            }
        }
        else if (isSUse)
        {
            secondButton.Select();
            if (Input.GetButtonDown("Submit"))
            {
                isSUse = false;
                numChange.SetActive(false);
                ToolManager.instance.Use();
                mu.MenuCancel();
            }
            else if (Input.GetButtonDown("Cancel"))
            {
                isSUse = false;
                numChange.SetActive(false);
                ToolManager.instance.selectButton.GetComponent<Button>().Select();
            }
        }*/

        if (Input.GetButtonDown("Menu") && !isNum)
        {
            ResetWindow();
        }

    }
    public void UseSelect()
    {
            ToolManager.instance.changeNum = 1;
            ToolManager.instance.Use();
            ResetWindow();

    }
    /// <summary>
    /// ここで変更窓出してエンターの入力を待つ
    /// </summary>
    public void SUseSlect()
    {
        if (ToolManager.instance.selectItem.summarizeUse == true)
        {
            ChangeToolNum ct;
            ToolManager.instance.changeNum = 1;
            isSUse = true;
            numChange.SetActive(true);
            numChange.GetComponent<Button>().Select();
            ct = numChange.GetComponent<ChangeToolNum>();
            ct.isSUse = true;
            ct.uw = this;
            isNum = true;
        }
    }
    /// <summary>
    /// ここで変更窓出してエンターの入力を待つ
    /// </summary>
    public void DumpSelect()
    {

            ChangeToolNum ct;
            ToolManager.instance.changeNum = 1;
            isDump = true;
            numChange.SetActive(true);
            numChange.GetComponent<Button>().Select();
            ct = numChange.GetComponent<ChangeToolNum>();
            ct.isDump = true;
            ct.uw = this;
            isNum = true;
    }


    public void ResetWindow()
    {
        isNum = false;
        isSUse = false;
        isDump = false;
        isFirst = false;
        ToolManager.instance.isUseMenu = false;
        this.gameObject.SetActive(false);
        MainUI.instance.MenuCancel();
        //  ToolManager.instance.selectButton = null;
        //    ToolManager.instance.selectItem = null;
    }



}
