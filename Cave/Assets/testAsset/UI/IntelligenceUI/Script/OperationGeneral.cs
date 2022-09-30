using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;
/// <summary>
/// ����͍�푋���o�鎞��Edit�p�����[�^�����݂̂ɏ㏑��������AEdit��V�����ۑ��p�p�����[�^�[�ɕۑ�������t�̂��Ƃ�����
/// ���ƍŏ��ɑI��������
/// ���͑I������
/// ��͉E�̑��̕���������������H
/// �e�{�^���������Ƃ��̏���������Ƃ���
/// </summary>
public class OperationGeneral : MonoBehaviour
{


    /// <summary>
    /// �߂��̂ɂ��g��
    /// 4���O�̂ŁA5���Z�[�u�m�F���
    /// 0�ŏ��R�܂ōU���x���񕜁B
    /// 4�A5�͂Ȃ񂩂��[�ԂƂ��܂��̂܂ǂɂ��ǂ�܂��B5�͖��ݒ�
    /// </summary>
    public Selectable[] myButton;
    public Text winName;

    public SettingWindowCon setWin;

    /// <summary>
    /// ���̑��Ŏg�����߂̃t���O�A
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
        //�ŏ��ɃG�f�B�b�g�ɔ��f�B
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

        //  Debug.Log($"�i���o�[{MainUICon.instance.settingNumber}");
      //  Debug.Log($"�I���{�^��{MainUICon.instance.selectButton.transform.root.gameObject.name}");
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
             //   Debug.Log($"����{MainUICon.instance.selectButton}{myButton[1].gameObject}");

                if (MainUICon.instance.selectButton == myButton[1].gameObject)
                {//Debug.Log("�ق�");
                    if (setObj[1].gameObject.activeSelf == false)
                    {
                        setObj[1].gameObject.SetActive(true);
                        setObj[0].gameObject.SetActive(false);
                    }
                    // winName.text = "�U�������ݒ�";
                    MainUICon.instance.settingNumber = 1;
                    
                }
                else if (MainUICon.instance.selectButton == myButton[2].gameObject)
                {
                    if (setObj[1].gameObject.activeSelf == false)
                    {
                        setObj[1].gameObject.SetActive(true);
                        setObj[0].gameObject.SetActive(false);
                    }
                    // winName.text = "�x�������ݒ�";
                    MainUICon.instance.settingNumber = 3;
                }
                else if (MainUICon.instance.selectButton == myButton[3].gameObject)
                {
                    if (setObj[1].gameObject.activeSelf == false)
                    {
                        setObj[1].gameObject.SetActive(true);
                        setObj[0].gameObject.SetActive(false);
                    }
                    // winName.text = "�񕜏����ݒ�";
                    MainUICon.instance.settingNumber = 5;
                }
                if (bNumber != MainUICon.instance.settingNumber)
                {
                    if (MainUICon.instance.settingNumber == 1)
                    {
                        winName.text = "�U�������ݒ�";
                    }
                    else if (MainUICon.instance.settingNumber == 3)
                    {
                        winName.text = "�x�������ݒ�";
                    }
                    else if (MainUICon.instance.settingNumber == 5)
                    {
                        winName.text = "�񕜏����ݒ�";

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
        //�v�ҏW�B�Z�[�u������߂���@���Ȃ�
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
            //�{���ɏI�����Ă���낵���ł����B
            MainUICon.instance.saveWin.gameObject.SetActive(true);
            MainUICon.instance.saveWin.WindowSet(0);
        }
    }



    public void OparationCopy(SisterParameter s, SisterParameter c,bool saveNow = false)
    {

        //�f�B�[�v�R�s�[

      //  Debug.Log($"�܂��A����{s.supportPlan[i].highOrLow}����{c.supportPlan[i].highOrLow}");
      //  Debug.Log($"�Ă��{s.name}������{c.name}");
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
            //��������U������
            #region
            s.AttackCondition[i].condition = c.AttackCondition[i].condition;
            s.AttackCondition[i].firstCondition = c.AttackCondition[i].firstCondition;
            s.AttackCondition[i].nextCondition = c.AttackCondition[i].nextCondition;
            s.AttackCondition[i].upDown = c.AttackCondition[i].upDown;
            s.attackCT[i] = c.attackCT[i];
            s.atSkipList[i] = c.atSkipList[i];
            #endregion

            //��������x��
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

            //���������
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
                //��������U���Ώۂ̏���
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
                    //�������瓹����
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


        


        //�Z�[�u�������͖��@�̃L���b�V�����̂Ă�
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
