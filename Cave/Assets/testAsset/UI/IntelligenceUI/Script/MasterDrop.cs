using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterDrop : MonoBehaviour
{


    public Transform[] value;


    //�����炩��q�I�u�W�F�N�g��`���Đݒ肷��B

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

            if (!MainUI.instance.editNow && (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction17)))
            {
            int i = MainUI.instance.isAH ? 3 : 2;

         //   Debug.Log($"����{MainUI.instance.editNow}");
                UIEnd();
                value[i].GetComponent<SettingWindowCon>().CancelWindow();
            }


    }

    private void OnEnable()
    {
        ChangeWindow();
    }

    /// <summary>
    /// ����ς��������ׂȂ�������
    /// �ŏ����Ă�
    /// </summary>
    public void ChangeWindow()
    {
        //�����{�^���������тɌĂڂ�
        //S���Ƃ��ŐF�X���߂悤

        //������value���̐؂�ւ���UIPlaceSet�̌Ăяo����SetActive����

        if (MainUI.instance.valueWindow != null)
        {
            MainUI.instance.valueWindow.SetActive(false);
        }
        if(MainUI.instance.secondDrop != null)
        MainUI.instance.secondDrop.SetActive(false);

        //���̐؂�ւ�
        WindowJudge();

        //Setting�i���o�[�ŏ����ς���B
        //Setting�i���o�[1�Ȃ�l�ݒ�̑����X�L�b�v���Ⴄ���̂ɕς��邩�B�Z�J���h�h���b�v�ȍ~�𓮂����̂ƁA�l�ݒ葋�̐؂�ւ��Ɣz�u�B
        //Selectable�̑I�ђ������B�i���G�A��Ԉُ�A�x���A�Ȃ��̎��͂Ȃ��j
        //Setting�i���o�[3�A5�Ȃ�l�ݒ�̑����X�L�b�v���Ⴄ���̂ɕς��邩�B���͈�����Ȃ̂ő���؂�ւ���̂�Selectable�̑I�тȂ���
        
        //pre�ɂ�next�ɂ��ŏ��̃h���b�v�_�E��������
        //Setting�i���o�[2�A4�A6�Ȃ�Z�J���h�h���b�v�ȍ~�����������B�l�ݒ葋�̐؂�ւ��Ɣz�u
        //Selectable�̍Đݒ�͂Ȃ�






        //���ёւ��̂��߂ɑO�̃I�u�W�F�N�g������
      //  MainUI.instance.beforeSet = this.gameObject;
    }


    /// <summary>
    /// �ŏ��̃h���b�v�_�E�����I��������B
    /// �o�b�N�X�y�[�X�ōs���B�ԍ������Ƃɖ߂��B
    /// </summary>
    public void UIEnd()
    {
        //   MainUI.instance.selectList = new List<Selectable>();

        MainUI.instance.beforeSet = null;
        this.gameObject.SetActive(false);
        if (MainUI.instance.valueWindow != null)
        {
            MainUI.instance.valueWindow.SetActive(false);
        }
            MainUI.instance.secondDrop.SetActive(false);

    }


    void WindowJudge()
    {


        //�U������
        //�t�@�[�X�g�h���b�v�i��b�����j���l�ݒ�i���G�A��Ԉُ�A�x���A�Ȃ��̎��͂Ȃ��j���Z�J���h�h���b�v�i��_�j���T�[�h�h���b�v�i�W�I�����j
        //�U���I���p�^�[��
        //�t�@�[�X�g�h���b�v�i�I���s���A�����j�i�Ȃɂ����Ȃ��ƈڍs�Ō�̑���������j���Z�J���h�h���b�v�i�g�p����U���̏����ݒ�j���T�[�h�h���b�v�i����ɏ����j
        //�x�������p�^�[��
        //�t�@�[�X�g�h���b�v�i��b�����j���l�ݒ�i���G�A�Ȃ��̎��͂Ȃ��j
        //�x���I���p�^�[��
        //�t�@�[�X�g�h���b�v�i�s���I���j�i�Ȃɂ����Ȃ��ƈڍs�Ō�̑���������j���Z�J���h�h���b�v�i�ǂ�ȃT�|�[�g���������̂�I�Ԃ��j���T�[�h�h���b�v�i�ǂ�ȏ����Ŗ��@�I�Ԃ��j

        SisterParameter sis = MainUI.instance.editParameter;
        int s = MainUI.instance.settingNumber;
        int e = MainUI.instance.editNumber;
        Selectable sl;
        Selectable sl2;
        int num;
        bool valueCheck = false;

        #region
        if (s == 1)
        {
            num = 0;
        }
        else if (s == 2)
        {
            num = 1;
        }
        else if (s == 3 || s == 5)
        {
            num = 4;
        }
        else if (s == 4)
        {
            num = 2;
        }
        else
        {
            num = 3;
        }
        #endregion
        sl = this.gameObject.GetComponent<ChildWindow>().objList[num].GetComponent<Selectable>();
        sl2 = MainUI.instance.secondDrop.GetComponent<ChildWindow>().objList[num].GetComponent<Selectable>();

        //�㉺�̑I���������A���s���ĂȂ�������
        UnderSet(sl, null);
        UpSet(sl2, null);
        //�܂��Z���N�g������
        MainUI.instance.beforeSet = null;
        this.gameObject.GetComponent<UIPlaceSet>().PlaceSet();
        if (s == 1)
        {
            AttackJudge editJudge = new AttackJudge();
            //�ݒ�
            #region
            if (e == 1)
            {
                editJudge = sis.firstTarget;
            }
            else if (e == 2)
            {
                editJudge = sis.secondTarget;
            }
            else if (e == 3)
            {
                editJudge = sis.thirdTarget;
            }
            else if (e == 4)
            {
                editJudge = sis.forthTarget;
            }
            else
            {
                editJudge = sis.fiveTarget;
            }
            #endregion

            if(editJudge.condition == AttackJudge.TargetJudge.�v���C���[����Ԉُ�ɂ��������� ||
               editJudge.condition == AttackJudge.TargetJudge.�������ĂȂ��x�������� ||
               editJudge.condition == AttackJudge.TargetJudge.���G�̑��� ||
               editJudge.condition == AttackJudge.TargetJudge.�w��Ȃ� ||
               editJudge.condition == AttackJudge.TargetJudge.��Ԉُ�ɂ������Ă�G)
            {
                if(MainUI.instance.valueWindow != null)
                {
                    MainUI.instance.valueWindow.SetActive(false);
                }
            }
            else
            {
                if(editJudge.condition == AttackJudge.TargetJudge.�G�^�C�v)
                {
                    MainUI.instance.valueWindow = value[0].gameObject;
                }
                else
                {
                    MainUI.instance.valueWindow = value[1].gameObject;
                  //  MainUI.instance.valueWindow
                }
                MainUI.instance.isChange = 1;
                MainUI.instance.valueWindow.SetActive(true);
                valueCheck = true;
                MainUI.instance.valueWindow.GetComponent<UIPlaceSet>().PlaceSet();
            }
                MainUI.instance.secondDrop.SetActive(true);
                MainUI.instance.secondDrop.GetComponent<UIPlaceSet>().PlaceSet();
        }
        else if(s == 2)
        {
            //MainUI.instance.valueWindow.SetActive(false);
            FireCondition editAT;
            //���e���蓖��
            #region
            if (e == 1)
            {
                editAT = sis.firstAttack;
            }
            else if (e == 2)
            {
                editAT = sis.secondAttack;
            }
            else if (e == 3)
            {
                editAT = sis.thirdAttack;
            }
            else if (e == 4)
            {
                editAT = sis.fourthAttack;
            }
            else if (e == 5)
            {
                editAT = sis.fiveAttack;
            }
            else
            {
                editAT = sis.nonAttack;
            }
            #endregion
            if (editAT.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ� || editAT.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs ||
                editAT.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
            {
                MainUI.instance.secondDrop.SetActive(false);
               // thirdDrop.SetActive(false);
            }
            else
            {
                MainUI.instance.secondDrop.SetActive(true);
                MainUI.instance.secondDrop.GetComponent<UIPlaceSet>().PlaceSet();
            }
        }
        else if (s == 3 || s == 4)
        {
            SupportCondition editSP;
            //�ݒ�
            #region
            if (e == 1)
            {
                editSP = sis.firstPlan;
            }
            else if (e == 2)
            {
                editSP = sis.secondPlan;
            }
            else if (e == 3)
            {
                editSP = sis.thirdPlan;
            }
            else if (e == 4)
            {
                editSP = sis.forthPlan;
            }
            else if (e == 5)
            {
                editSP = sis.fivePlan;
            }
            else
            {
                editSP = sis.sixPlan;
            }
            #endregion
            if (s == 3)
            {
                if(editSP.sCondition == SupportCondition.SupportStatus.���G�����邩�ǂ��� || editSP.sCondition == SupportCondition.SupportStatus.�w��Ȃ� || editSP.sCondition == SupportCondition.SupportStatus.�v���C���[����Ԉُ�ɂ���������
                    || editSP.sCondition == SupportCondition.SupportStatus.�C�ӂ̎x�����؂�Ă���Ƃ�)
                {
                    if (MainUI.instance.valueWindow != null)
                    {
                        MainUI.instance.valueWindow.SetActive(false);
                    }
                    if (editSP.sCondition == SupportCondition.SupportStatus.�C�ӂ̎x�����؂�Ă���Ƃ�)
                    {
                        MainUI.instance.secondDrop.SetActive(true);
                        MainUI.instance.secondDrop.GetComponent<UIPlaceSet>().PlaceSet();
                    }
                }
                else
                {
                    if (editSP.sCondition == SupportCondition.SupportStatus.�G�^�C�v)
                    {
                        MainUI.instance.valueWindow = value[0].gameObject;
                    }
                    else
                    {
                        MainUI.instance.valueWindow = value[1].gameObject;

                    }

                    MainUI.instance.valueWindow.SetActive(true);
                    MainUI.instance.isChange = 1;
                    valueCheck = true;
                    MainUI.instance.valueWindow.GetComponent<UIPlaceSet>().PlaceSet();
                }
            }
            else
            {
                if (editSP.ActBase != SupportCondition.MagicJudge.�e��x�����@)
                {
                    MainUI.instance.secondDrop.SetActive(false);
                }
                else
                {
                    MainUI.instance.secondDrop.SetActive(true);
                    MainUI.instance.secondDrop.GetComponent<UIPlaceSet>().PlaceSet();
            }
            }

        }
        else if (s == 5 || s == 6)
        {
            RecoverCondition editRC;
            //�ݒ�
            #region
            if (e == 1)
            {
                if (MainUI.instance.isAH)
                {
                    editRC = sis.nFirstRecover;
                }
                else
                {
                    editRC = sis.firstRecover;
                }
            }
            else if (e == 2)
            {
                if (MainUI.instance.isAH)
                {
                    editRC = sis.nSecondRecover;
                }
                else
                {
                    editRC = sis.secondRecover;
                }
            }
            else if (e == 3)
            {
                if (MainUI.instance.isAH)
                {
                    editRC = sis.nThirdRecover;
                }
                else
                {
                    editRC = sis.thirdRecover;
                }
            }
            else if (e == 4)
            {
                editRC = sis.forthRecover;
            }
            else if (e == 5)
            {

                editRC = sis.fiveRecover;
            }
            else
            {
                editRC = sis.nonRecover;
            }
            #endregion
            if(s == 5)
            {
                if(editRC.condition == RecoverCondition.RecoverStatus.���G�����邩�ǂ��� || editRC.condition == RecoverCondition.RecoverStatus.�w��Ȃ� ||
                    editRC.condition == RecoverCondition.RecoverStatus.�v���C���[����Ԉُ�ɂ��������� || editRC.condition == RecoverCondition.RecoverStatus.�C�ӂ̎x�����؂�Ă���Ƃ�)
                {
                    if (MainUI.instance.valueWindow != null)
                    {
                        MainUI.instance.valueWindow.SetActive(false);
                    }
                    if (editRC.condition == RecoverCondition.RecoverStatus.�C�ӂ̎x�����؂�Ă���Ƃ�)
                    {
                        MainUI.instance.secondDrop.SetActive(true);
                        MainUI.instance.secondDrop.GetComponent<UIPlaceSet>().PlaceSet();

                    }
                }
                else
                {
                    if(editRC.condition == RecoverCondition.RecoverStatus.�G�^�C�v)
                    {
                        MainUI.instance.valueWindow = value[0].gameObject;

                    }
                    else
                    {
                        MainUI.instance.valueWindow = value[1].gameObject;

                    }
                        MainUI.instance.valueWindow.SetActive(true);
                        MainUI.instance.isChange = 1;
                        valueCheck = true;
                        MainUI.instance.valueWindow.GetComponent<UIPlaceSet>().PlaceSet();
            }
            }
            else
            {
                if(editRC.ActBase != RecoverCondition.MagicJudge.�������@)
                {
                    MainUI.instance.secondDrop.SetActive(false);
                }
                else
                {
                    MainUI.instance.secondDrop.SetActive(true);
                    MainUI.instance.secondDrop.GetComponent<UIPlaceSet>().PlaceSet();
                }
            }
        }

        if (!valueCheck && MainUI.instance.secondDrop.activeSelf)
        {
            sl = this.gameObject.GetComponent<ChildWindow>().objList[num].GetComponent<Selectable>();
            sl2 = MainUI.instance.secondDrop.GetComponent<ChildWindow>().objList[num].GetComponent<Selectable>();

            //�l���Ȃ��ē�Ԗڂ��鎞��Ԗڂƈ�Ԗڂ̏㉺���Ȃ���B
            UnderSet(sl, sl2);
            UpSet(sl2, sl);
        }
    }


    public void UnderSet(Selectable me ,Selectable nextObject)
    {
        if (nextObject != null)
        {
            Navigation navi = me.navigation;
            navi.selectOnDown = nextObject;
            me.navigation = navi;
            navi = nextObject.navigation;
            navi.selectOnUp = me;
            nextObject.navigation = navi;
        }
        else
        {
            Navigation navi = me.navigation;
            navi.selectOnDown = null;
            me.navigation = navi;
        }
    }
    public void UpSet(Selectable me ,Selectable preObject)
    {
        if (preObject != null)
        {
            Navigation navi = me.navigation;
            navi.selectOnUp = preObject;
            me.navigation = navi;
            navi = preObject.navigation;
            navi.selectOnDown = me;
            preObject.navigation = navi;
        }
        else
        {
            Navigation navi = me.navigation;
            navi.selectOnUp = null;
            me.navigation = navi;
        }
    }





}
