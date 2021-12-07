using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// これは作戦窓を出る時にEditパラメータを現在のに上書きしたり、Editを新しい保存用パラメーターに保存したり逆のことしたり
/// あと最初に選択したり
/// 今は選択だけ
/// 後は右の窓の文字も書き換える？
/// 各ボタン押したときの処理も入れとくか
/// </summary>
public class OperationGeneral : MonoBehaviour
{


    /// <summary>
    /// 戻すのにも使う
    /// 4が前ので、5がセーブ確認画面
    /// </summary>
    public Selectable[] myButton;
    public Text winName;

    public SettingWindowCon setWin;

    /// <summary>
    /// 次の窓で使うためのフラグ、
    /// </summary>
    [HideInInspector]
    public bool next;

    int bNumber;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        //最初にエディットに反映。
        //MainUI.instance.editParameter = SManager.instance.Sister.GetComponent<SisterFire>().sister;
        myButton[0].Select();
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log($"ナンバー{MainUI.instance.settingNumber}");
        /// Debug.Log($"選択ボタン{MainUI.instance.eventSystem.currentSelectedGameObject}");
        if (MainUI.instance.eventSystem.currentSelectedGameObject == myButton[1].gameObject)
        {
           // winName.text = "攻撃条件設定";
            MainUI.instance.settingNumber = 1;
            
        }
        else if (MainUI.instance.eventSystem.currentSelectedGameObject == myButton[2].gameObject)
        {
           // winName.text = "支援条件設定";
            MainUI.instance.settingNumber = 3;
        }
        else if (MainUI.instance.eventSystem.currentSelectedGameObject == myButton[3].gameObject)
        {
           // winName.text = "回復条件設定";
            MainUI.instance.settingNumber = 5;
        }
        if(bNumber != MainUI.instance.settingNumber)
        {
            if (MainUI.instance.settingNumber == 1)
            {
                 winName.text = "攻撃条件設定";
            }
            else if (MainUI.instance.settingNumber == 3)
            {
                 winName.text = "支援条件設定";
            }
            else if (MainUI.instance.settingNumber == 5)
            {
                 winName.text = "回復条件設定";
                
            }
                 setWin.ButtonSet();
            bNumber = MainUI.instance.settingNumber;
        }

       
    }

    /// <summary>
    /// 
    /// </summary>
    public void SelectReset()
    {
        int s = MainUI.instance.settingNumber;

        if(s == 1)
        {
            myButton[1].Select();
        }
        else if (s == 3)
        {
            myButton[2].Select();
        }
        else if (s == 5)
        {
            myButton[3].Select();
        }
    }

    public void EditEnd()
    {
        //要編集。セーブ窓から戻る方法がない
        if (MainUI.instance.isSave)
        {
            MainUI.instance.isSave = false;
            myButton[4].transform.root.gameObject.SetActive(true);
            myButton[4].Select();
        }
        else
        {
            myButton[5].transform.root.gameObject.SetActive(true);
            myButton[5].Select();
        }
    }



}
