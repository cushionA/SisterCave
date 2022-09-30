using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
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
    /// 0最初３まで攻撃支援回復。
    /// 4、5はなんかせーぶとかまえのまどにもどるまわり。5は未設定
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

    [SerializeField]
    Transform[] setObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
       // Debug.Log("sss");
        //最初にエディットに反映。
        OparationCopy(MainUICon.instance.editParameter, SManager.instance.Sister.MMGetComponentNoAlloc<FireAbility>().sister);
        myButton[0].Select();
        if (setObj[0].gameObject.activeSelf == false)
        {
            setObj[0].gameObject.SetActive(true);
            setObj[1].gameObject.SetActive(false);
        }
        MainUICon.instance.settingNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {

        //  Debug.Log($"ナンバー{MainUICon.instance.settingNumber}");
      //  Debug.Log($"選択ボタン{MainUICon.instance.selectButton.transform.root.gameObject.name}");
        /// 
        if (!next) {

            if (MainUICon.instance.selectButton == myButton[6].gameObject)
            {
                if (setObj[0].gameObject.activeSelf == false)
                {
                    setObj[0].gameObject.SetActive(true);
                    setObj[1].gameObject.SetActive(false);
                }
                MainUICon.instance.settingNumber = 0;
            }
            else
            {
             //   Debug.Log($"ｒｒ{MainUICon.instance.selectButton}{myButton[1].gameObject}");

                if (MainUICon.instance.selectButton == myButton[1].gameObject)
                {//Debug.Log("ほね");
                    if (setObj[1].gameObject.activeSelf == false)
                    {
                        setObj[1].gameObject.SetActive(true);
                        setObj[0].gameObject.SetActive(false);
                    }
                    // winName.text = "攻撃条件設定";
                    MainUICon.instance.settingNumber = 1;
                    
                }
                else if (MainUICon.instance.selectButton == myButton[2].gameObject)
                {
                    if (setObj[1].gameObject.activeSelf == false)
                    {
                        setObj[1].gameObject.SetActive(true);
                        setObj[0].gameObject.SetActive(false);
                    }
                    // winName.text = "支援条件設定";
                    MainUICon.instance.settingNumber = 3;
                }
                else if (MainUICon.instance.selectButton == myButton[3].gameObject)
                {
                    if (setObj[1].gameObject.activeSelf == false)
                    {
                        setObj[1].gameObject.SetActive(true);
                        setObj[0].gameObject.SetActive(false);
                    }
                    // winName.text = "回復条件設定";
                    MainUICon.instance.settingNumber = 5;
                }
                if (bNumber != MainUICon.instance.settingNumber)
                {
                    if (MainUICon.instance.settingNumber == 1)
                    {
                        winName.text = "攻撃条件設定";
                    }
                    else if (MainUICon.instance.settingNumber == 3)
                    {
                        winName.text = "支援条件設定";
                    }
                    else if (MainUICon.instance.settingNumber == 5)
                    {
                        winName.text = "回復条件設定";

                    }
                    setWin.ButtonSet();
                    bNumber = MainUICon.instance.settingNumber;
                }
            }

            if (MainUICon.instance._reInput.CancelButton.State.CurrentState == MoreMountains.Tools.MMInput.ButtonStates.ButtonDown)
            {
                if(MainUICon.instance.settingNumber == 0)
                {
                    MainUICon.instance.settingNumber = 100;
                }
                
                EditEnd();
            }
        }


       
    }

    /// <summary>
    /// 
    /// </summary>
    public void SelectReset()
    {
        int s = MainUICon.instance.settingNumber;

        if (!MainUICon.instance.isAH)
        {
            if (s == 1)
            {
                myButton[1].Select();
            }
            else if (s == 3)
            {
                myButton[2].Select();
               // Debug.Log("ssss1");
            }
            else if (s == 5)
            {
                myButton[3].Select();
            }
            else if(s == 0)
            {
                myButton[6].Select();
                next = false;
            }
        }
        else
        {
            myButton[5].Select();
            MainUICon.instance.isAH = false;
            next = true;
        }
    }

    public void EditEnd(bool d = false)
    {
        //要編集。セーブ窓から戻る方法がない
        if (MainUICon.instance.isSave)
        {
            MainUICon.instance.isSave = false;
            myButton[4].transform.root.gameObject.SetActive(true);
            myButton[4].Select();
            this.gameObject.SetActive(false);

         //    Debug.Log("sssssssss");
            if (d)
            {
                OparationCopy(MainUICon.instance.editParameter, SManager.instance.Sister.MMGetComponentNoAlloc<FireAbility>().sister);
            }
        }
        else
        {
            //本当に終了してもよろしいですか。
            MainUICon.instance.saveWin.gameObject.SetActive(true);
            MainUICon.instance.saveWin.WindowSet(0);
        }
    }



    public void OparationCopy(SisterParameter s, SisterParameter c,bool saveNow = false)
    {

        //ディープコピー

      //  Debug.Log($"まえ、もと{s.supportPlan[i].highOrLow}する{c.supportPlan[i].highOrLow}");
      //  Debug.Log($"てるみ{s.name}あいあ{c.name}");
        s.oparationName = c.oparationName;
        s.oparationDescription = c.oparationDescription;
        s.autoHeal = c.autoHeal;
        s.stateResetRes = c.stateResetRes;
        s.targetResetRes = c.targetResetRes;
        s.attackCT = new List<float>(c.attackCT);
        s.supportCT = new List<float>(c.supportCT);
        s.healCT = new List<float>(c.healCT);
        s.priority = c.priority;


        for (int i = 0; i < 6; i++)
        {
            //ここから攻撃条件
            #region
            s.AttackCondition[i].condition = c.AttackCondition[i].condition;
            s.AttackCondition[i].firstCondition = c.AttackCondition[i].firstCondition;
            s.AttackCondition[i].nextCondition = c.AttackCondition[i].nextCondition;
            s.AttackCondition[i].upDown = c.AttackCondition[i].upDown;
            s.attackCT[i] = c.attackCT[i];
            s.atSkipList[i] = c.atSkipList[i];
            #endregion

            //ここから支援
            #region
            s.supportPlan[i].sCondition = c.supportPlan[i].sCondition;
            s.supportPlan[i].percentage = c.supportPlan[i].percentage;
            s.supportPlan[i].highOrLow = c.supportPlan[i].highOrLow;
            s.supportPlan[i].useSupport = c.supportPlan[i].useSupport;
            s.supportPlan[i].upDown = c.supportPlan[i].upDown;
            s.supportPlan[i].nextCondition = c.supportPlan[i].nextCondition;
            s.supportPlan[i].needSupport = c.supportPlan[i].needSupport;
            s.supportPlan[i].ActBase = c.supportPlan[i].ActBase;
            s.supportCT[i] = c.supportCT[i];
            s.sSkipList[i] = c.sSkipList[i];
            #endregion

            //ここから回復
            #region
            s.recoverCondition[i].condition = c.recoverCondition[i].condition;
            s.recoverCondition[i].percentage = c.recoverCondition[i].percentage;
            s.recoverCondition[i].highOrLow = c.recoverCondition[i].highOrLow;
            s.recoverCondition[i].useSupport = c.recoverCondition[i].useSupport;
            s.recoverCondition[i].upDown = c.recoverCondition[i].upDown;
            s.recoverCondition[i].nextCondition = c.recoverCondition[i].nextCondition;
            s.recoverCondition[i].needSupport = c.recoverCondition[i].needSupport;
            s.recoverCondition[i].ActBase = c.recoverCondition[i].ActBase;
            s.healCT[i] = c.healCT[i];
            s.hSkipList[i] = c.hSkipList[i];
            #endregion

            if (i < 5)
            {
                //ここから攻撃対象の条件
                #region
                s.targetCondition[i].condition = c.targetCondition[i].condition;
                s.targetCondition[i].percentage = c.targetCondition[i].percentage;
                s.targetCondition[i].highOrLow = c.targetCondition[i].highOrLow;
                s.targetCondition[i].wp = c.targetCondition[i].wp;
                s.targetCondition[i].upDown = c.targetCondition[i].upDown;
                s.targetCondition[i].nextCondition = c.targetCondition[i].nextCondition;

                #endregion
                if (i < 3)
                {
                    //ここから道中回復
                    #region
                    s.nRecoverCondition[i].condition = c.nRecoverCondition[i].condition;
                    s.nRecoverCondition[i].percentage = c.nRecoverCondition[i].percentage;
                    s.nRecoverCondition[i].highOrLow = c.nRecoverCondition[i].highOrLow;
                    s.nRecoverCondition[i].useSupport = c.nRecoverCondition[i].useSupport;
                    s.nRecoverCondition[i].upDown = c.nRecoverCondition[i].upDown;
                    s.nRecoverCondition[i].nextCondition = c.nRecoverCondition[i].nextCondition;
                    s.nRecoverCondition[i].needSupport = c.nRecoverCondition[i].needSupport;
                    s.nRecoverCondition[i].ActBase = c.nRecoverCondition[i].ActBase;
                    s.autHealCT[i] = c.autHealCT[i];
                    s.ahSkipList[i] = c.ahSkipList[i];
                    #endregion
                }
            }
        }


        


        //セーブした時は魔法のキャッシュを捨てる
        if (saveNow)
        {


            for (int i = 0; i < 6 ;i++)
            {
                s.AttackCondition[i].UseMagic = null;
                s.supportPlan[i].UseMagic = null;
                s.recoverCondition[i].UseMagic = null;
                if (i < 3)
                {
                    s.nRecoverCondition[i].UseMagic = null;
                }
            }

        }
    }


    public void SaveData()
    {
        OparationCopy(SManager.instance.Sister.MMGetComponentNoAlloc<FireAbility>().sister, MainUICon.instance.editParameter,true);
        MainUICon.instance.isSave = true;
        next = false;
    }

    public void ReturnNext()
    {
        next = false;
    }



}
