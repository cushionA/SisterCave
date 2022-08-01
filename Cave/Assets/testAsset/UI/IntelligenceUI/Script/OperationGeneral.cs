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
        OparationCopy(MainUICon.instance.editParameter, SManager.instance.Sister.GetComponent<SisterFire>().sister);
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
      //  Debug.Log($"選択ボタン{MainUICon.instance.eventSystem.currentSelectedGameObject.transform.root.gameObject.name}");
        /// 
        if (!next) {

            if (MainUICon.instance.eventSystem.currentSelectedGameObject == myButton[6].gameObject)
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


                if (MainUICon.instance.eventSystem.currentSelectedGameObject == myButton[1].gameObject)
                {
                    if (setObj[1].gameObject.activeSelf == false)
                    {
                        setObj[1].gameObject.SetActive(true);
                        setObj[0].gameObject.SetActive(false);
                    }
                    // winName.text = "攻撃条件設定";
                    MainUICon.instance.settingNumber = 1;

                }
                else if (MainUICon.instance.eventSystem.currentSelectedGameObject == myButton[2].gameObject)
                {
                    if (setObj[1].gameObject.activeSelf == false)
                    {
                        setObj[1].gameObject.SetActive(true);
                        setObj[0].gameObject.SetActive(false);
                    }
                    // winName.text = "支援条件設定";
                    MainUICon.instance.settingNumber = 3;
                }
                else if (MainUICon.instance.eventSystem.currentSelectedGameObject == myButton[3].gameObject)
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

            if (GManager.instance.InputR.GetButtonDown(MainUICon.instance.rewiredAction17))
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
                OparationCopy(MainUICon.instance.editParameter, SManager.instance.Sister.GetComponent<SisterFire>().sister);
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

      //  Debug.Log($"まえ、もと{s.firstPlan.highOrLow}する{c.firstPlan.highOrLow}");
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
        #region
        s.firstTarget.condition = c.firstTarget.condition;
        s.firstTarget.percentage = c.firstTarget.percentage;
        s.firstTarget.highOrLow = c.firstTarget.highOrLow;
        s.firstTarget.wp = c.firstTarget.wp;
        s.firstTarget.upDown = c.firstTarget.upDown;
        s.firstTarget.nextCondition = c.firstTarget.nextCondition;
        #endregion
        #region
        s.secondTarget.condition = c.secondTarget.condition;
        s.secondTarget.percentage = c.secondTarget.percentage;
        s.secondTarget.highOrLow = c.secondTarget.highOrLow;
        s.secondTarget.wp = c.secondTarget.wp;
        s.secondTarget.upDown = c.secondTarget.upDown;
        s.secondTarget.nextCondition = c.secondTarget.nextCondition;
        #endregion

        #region
        s.thirdTarget.condition = c.thirdTarget.condition;
        s.thirdTarget.percentage = c.thirdTarget.percentage;
        s.thirdTarget.highOrLow = c.thirdTarget.highOrLow;
        s.thirdTarget.wp = c.thirdTarget.wp;
        s.thirdTarget.upDown = c.thirdTarget.upDown;
        s.thirdTarget.nextCondition = c.thirdTarget.nextCondition;
        #endregion

        #region
        s.forthTarget.condition = c.forthTarget.condition;
        s.forthTarget.percentage = c.forthTarget.percentage;
        s.forthTarget.highOrLow = c.forthTarget.highOrLow;
        s.forthTarget.wp = c.forthTarget.wp;
        s.forthTarget.upDown = c.forthTarget.upDown;
        s.forthTarget.nextCondition = c.forthTarget.nextCondition;
        #endregion
        #region
        s.fiveTarget.condition = c.fiveTarget.condition;
        s.fiveTarget.percentage = c.fiveTarget.percentage;
        s.fiveTarget.highOrLow = c.fiveTarget.highOrLow;
        s.fiveTarget.wp = c.fiveTarget.wp;
        s.fiveTarget.upDown = c.fiveTarget.upDown;
        s.fiveTarget.nextCondition = c.fiveTarget.nextCondition;
        #endregion

        //ここから攻撃
        #region
        s.firstAttack.condition = c.firstAttack.condition;
        s.firstAttack.firstCondition = c.firstAttack.firstCondition;
        s.firstAttack.nextCondition = c.firstAttack.nextCondition;
        s.firstAttack.upDown = c.firstAttack.upDown;
        #endregion
        #region
        s.secondAttack.condition = c.secondAttack.condition;
        s.secondAttack.firstCondition = c.secondAttack.firstCondition;
        s.secondAttack.nextCondition = c.secondAttack.nextCondition;
        s.secondAttack.upDown = c.secondAttack.upDown;
        #endregion
        #region
        s.thirdAttack.condition = c.thirdAttack.condition;
        s.thirdAttack.firstCondition = c.thirdAttack.firstCondition;
        s.thirdAttack.nextCondition = c.thirdAttack.nextCondition;
        s.thirdAttack.upDown = c.thirdAttack.upDown;
        #endregion
        #region
        s.fourthAttack.condition = c.fourthAttack.condition;
        s.fourthAttack.firstCondition = c.fourthAttack.firstCondition;
        s.fourthAttack.nextCondition = c.fourthAttack.nextCondition;
        s.fourthAttack.upDown = c.fourthAttack.upDown;
        #endregion

        #region
        s.fiveAttack.condition = c.fiveAttack.condition;
        s.fiveAttack.firstCondition = c.fiveAttack.firstCondition;
        s.fiveAttack.nextCondition = c.fiveAttack.nextCondition;
        s.fiveAttack.upDown = c.fiveAttack.upDown;
        #endregion
        #region
        s.nonAttack.condition = c.nonAttack.condition;
        s.nonAttack.firstCondition = c.nonAttack.firstCondition;
        s.nonAttack.nextCondition = c.nonAttack.nextCondition;
        s.nonAttack.upDown = c.nonAttack.upDown;
        #endregion

        //ここから支援
        #region
        s.firstPlan.sCondition = c.firstPlan.sCondition;
        s.firstPlan.percentage = c.firstPlan.percentage;
        s.firstPlan.highOrLow = c.firstPlan.highOrLow;
        s.firstPlan.useSupport = c.firstPlan.useSupport;
        s.firstPlan.upDown = c.firstPlan.upDown;
        s.firstPlan.nextCondition = c.firstPlan.nextCondition;
        s.firstPlan.needSupport = c.firstPlan.needSupport;
        s.firstPlan.ActBase = c.firstPlan.ActBase;
        #endregion
        #region
        s.secondPlan.sCondition = c.secondPlan.sCondition;
        s.secondPlan.percentage = c.secondPlan.percentage;
        s.secondPlan.highOrLow = c.secondPlan.highOrLow;
        s.secondPlan.useSupport = c.secondPlan.useSupport;
        s.secondPlan.upDown = c.secondPlan.upDown;
        s.secondPlan.nextCondition = c.secondPlan.nextCondition;
        s.secondPlan.needSupport = c.secondPlan.needSupport;
        s.secondPlan.ActBase = c.secondPlan.ActBase;
        #endregion
        #region
        s.thirdPlan.sCondition = c.thirdPlan.sCondition;
        s.thirdPlan.percentage = c.thirdPlan.percentage;
        s.thirdPlan.highOrLow = c.thirdPlan.highOrLow;
        s.thirdPlan.useSupport = c.thirdPlan.useSupport;
        s.thirdPlan.upDown = c.thirdPlan.upDown;
        s.thirdPlan.nextCondition = c.thirdPlan.nextCondition;
        s.thirdPlan.needSupport = c.thirdPlan.needSupport;
        s.thirdPlan.ActBase = c.thirdPlan.ActBase;
        #endregion
        #region
        s.forthPlan.sCondition = c.forthPlan.sCondition;
        s.forthPlan.percentage = c.forthPlan.percentage;
        s.forthPlan.highOrLow = c.forthPlan.highOrLow;
        s.forthPlan.useSupport = c.forthPlan.useSupport;
        s.forthPlan.upDown = c.forthPlan.upDown;
        s.forthPlan.nextCondition = c.forthPlan.nextCondition;
        s.forthPlan.needSupport = c.forthPlan.needSupport;
        s.forthPlan.ActBase = c.forthPlan.ActBase;
        #endregion
        #region
        s.fivePlan.sCondition = c.fivePlan.sCondition;
        s.fivePlan.percentage = c.fivePlan.percentage;
        s.fivePlan.highOrLow = c.fivePlan.highOrLow;
        s.fivePlan.useSupport = c.fivePlan.useSupport;
        s.fivePlan.upDown = c.fivePlan.upDown;
        s.fivePlan.nextCondition = c.fivePlan.nextCondition;
        s.fivePlan.needSupport = c.fivePlan.needSupport;
        s.fivePlan.ActBase = c.fivePlan.ActBase;
        #endregion
        #region
        //      s.sixPlan.sCondition = c.sixPlan.sCondition;
        //   s.sixPlan.percentage = c.sixPlan.percentage;
        //    s.sixPlan.highOrLow = c.sixPlan.highOrLow;
        s.sixPlan.useSupport = c.sixPlan.useSupport;
        s.sixPlan.upDown = c.sixPlan.upDown;
        s.sixPlan.nextCondition = c.sixPlan.nextCondition;
        //      s.sixPlan.needSupport = c.sixPlan.needSupport;
        s.sixPlan.ActBase = c.sixPlan.ActBase;
        #endregion

        //ここから回復
        #region
        s.firstRecover.condition = c.firstRecover.condition;
        s.firstRecover.percentage = c.firstRecover.percentage;
        s.firstRecover.highOrLow = c.firstRecover.highOrLow;
        s.firstRecover.useSupport = c.firstRecover.useSupport;
        s.firstRecover.upDown = c.firstRecover.upDown;
        s.firstRecover.nextCondition = c.firstRecover.nextCondition;
        s.firstRecover.needSupport = c.firstRecover.needSupport;
        s.firstRecover.ActBase = c.firstRecover.ActBase;
        #endregion


        #region
        s.secondRecover.condition = c.secondRecover.condition;
        s.secondRecover.percentage = c.secondRecover.percentage;
        s.secondRecover.highOrLow = c.secondRecover.highOrLow;
        s.secondRecover.useSupport = c.secondRecover.useSupport;
        s.secondRecover.upDown = c.secondRecover.upDown;
        s.secondRecover.nextCondition = c.secondRecover.nextCondition;
        s.secondRecover.needSupport = c.secondRecover.needSupport;
        s.secondRecover.ActBase = c.secondRecover.ActBase;
        #endregion
        #region

        s.thirdRecover.condition = c.thirdRecover.condition;
        s.thirdRecover.percentage = c.thirdRecover.percentage;
        s.thirdRecover.highOrLow = c.thirdRecover.highOrLow;
        s.thirdRecover.useSupport = c.thirdRecover.useSupport;
        s.thirdRecover.upDown = c.thirdRecover.upDown;
        s.thirdRecover.nextCondition = c.thirdRecover.nextCondition;
        s.thirdRecover.needSupport = c.thirdRecover.needSupport;
        s.thirdRecover.ActBase = c.thirdRecover.ActBase;
        #endregion
        #region

        s.forthRecover.condition = c.forthRecover.condition;
        s.forthRecover.percentage = c.forthRecover.percentage;
        s.forthRecover.highOrLow = c.forthRecover.highOrLow;
        s.forthRecover.useSupport = c.forthRecover.useSupport;
        s.forthRecover.upDown = c.forthRecover.upDown;
        s.forthRecover.nextCondition = c.forthRecover.nextCondition;
        s.forthRecover.needSupport = c.forthRecover.needSupport;
        s.forthRecover.ActBase = c.forthRecover.ActBase;
        #endregion
        #region

        s.fiveRecover.condition = c.fiveRecover.condition;
        s.fiveRecover.percentage = c.fiveRecover.percentage;
        s.fiveRecover.highOrLow = c.fiveRecover.highOrLow;
        s.fiveRecover.useSupport = c.fiveRecover.useSupport;
        s.fiveRecover.upDown = c.fiveRecover.upDown;
        s.fiveRecover.nextCondition = c.fiveRecover.nextCondition;
        s.fiveRecover.needSupport = c.fiveRecover.needSupport;
        s.fiveRecover.ActBase = c.fiveRecover.ActBase;
        #endregion
        #region
        //      s.nonRecover.condition = c.nonRecover.condition;
        //   s.nonRecover.percentage = c.nonRecover.percentage;
        //    s.nonRecover.highOrLow = c.nonRecover.highOrLow;

            s.nonRecover.useSupport = c.nonRecover.useSupport;
        s.nonRecover.upDown = c.nonRecover.upDown;
        s.nonRecover.nextCondition = c.nonRecover.nextCondition;
  //      s.nonRecover.needSupport = c.nonRecover.needSupport;
        s.nonRecover.ActBase = c.nonRecover.ActBase;
        #endregion
        #region

        s.nFirstRecover.condition = c.nFirstRecover.condition;
        s.nFirstRecover.percentage = c.nFirstRecover.percentage;
        s.nFirstRecover.highOrLow = c.nFirstRecover.highOrLow;
        s.nFirstRecover.useSupport = c.nFirstRecover.useSupport;
        s.nFirstRecover.upDown = c.nFirstRecover.upDown;
        s.nFirstRecover.nextCondition = c.nFirstRecover.nextCondition;
        s.nFirstRecover.needSupport = c.nFirstRecover.needSupport;
        s.nFirstRecover.ActBase = c.nFirstRecover.ActBase;
        #endregion
        #region

        s.nSecondRecover.condition = c.nSecondRecover.condition;
        s.nSecondRecover.percentage = c.nSecondRecover.percentage;
        s.nSecondRecover.highOrLow = c.nSecondRecover.highOrLow;
        s.nSecondRecover.useSupport = c.nSecondRecover.useSupport;
        s.nSecondRecover.upDown = c.nSecondRecover.upDown;
        s.nSecondRecover.nextCondition = c.nSecondRecover.nextCondition;
        s.nSecondRecover.needSupport = c.nSecondRecover.needSupport;
        s.nSecondRecover.ActBase = c.nSecondRecover.ActBase;
        #endregion
        #region

        s.nThirdRecover.condition = c.nThirdRecover.condition;
        s.nThirdRecover.percentage = c.nThirdRecover.percentage;
        s.nThirdRecover.highOrLow = c.nThirdRecover.highOrLow;
        s.nThirdRecover.useSupport = c.nThirdRecover.useSupport;
        s.nThirdRecover.upDown = c.nThirdRecover.upDown;
        s.nThirdRecover.nextCondition = c.nThirdRecover.nextCondition;
        s.nThirdRecover.needSupport = c.nThirdRecover.needSupport;
        s.nThirdRecover.ActBase = c.nThirdRecover.ActBase;
        #endregion
        //  Debug.Log($"あと、もと{s.firstPlan.highOrLow}され{c.firstPlan.highOrLow}");


        if (saveNow)
        {
            s.firstAttack.UseMagic = null;
            s.secondAttack.UseMagic = null;
            s.thirdAttack.UseMagic = null;
            s.fourthAttack.UseMagic = null;
            s.fiveAttack.UseMagic = null;
            s.nonAttack.UseMagic = null;

            s.firstPlan.UseMagic = null;
            s.secondPlan.UseMagic = null;
            s.thirdPlan.UseMagic = null;
            s.forthPlan.UseMagic = null;
            s.fivePlan.UseMagic = null;
            s.sixPlan.UseMagic = null;

            s.firstRecover.UseMagic = null;
            s.secondRecover.UseMagic = null;
            s.thirdRecover.UseMagic = null;
            s.forthRecover.UseMagic = null;
            s.fiveRecover.UseMagic = null;
            s.nonRecover.UseMagic = null;
            s.nFirstRecover.UseMagic = null;
            s.nSecondRecover.UseMagic = null;
            s.nThirdRecover.UseMagic = null;
        }
    }


    public void SaveData()
    {
        OparationCopy(SManager.instance.Sister.GetComponent<SisterFire>().sister, MainUICon.instance.editParameter,true);
        MainUICon.instance.isSave = true;
        next = false;
    }

    public void ReturnNext()
    {
        next = false;
    }



}
