using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;
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

            if (!MainUICon.instance.editNow && MainUICon.instance._reInput.CancelButton.State.CurrentState == MoreMountains.Tools.MMInput.ButtonStates.ButtonDown)
            {
            int i = MainUICon.instance.isAH ? 3 : 2;

         //   Debug.Log($"����{MainUICon.instance.editNow}");
                UIEnd();
                value[i].gameObject.MMGetComponentNoAlloc<SettingWindowCon>().CancelWindow();
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

        if (MainUICon.instance.valueWindow != null)
        {
            MainUICon.instance.valueWindow.SetActive(false);
        }
        if(MainUICon.instance.secondDrop != null)
        MainUICon.instance.secondDrop.SetActive(false);

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
      //  MainUICon.instance.beforeSet = this.gameObject;
    }


    /// <summary>
    /// �ŏ��̃h���b�v�_�E�����I��������B
    /// �o�b�N�X�y�[�X�ōs���B�ԍ������Ƃɖ߂��B
    /// </summary>
    public void UIEnd()
    {
        //   MainUICon.instance.selectList = new List<Selectable>();

        MainUICon.instance.beforeSet = null;
        this.gameObject.SetActive(false);
        if (MainUICon.instance.valueWindow != null)
        {
            MainUICon.instance.valueWindow.SetActive(false);
        }
            MainUICon.instance.secondDrop.SetActive(false);

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

        SisterParameter sis = MainUICon.instance.editParameter;
        int s = MainUICon.instance.settingNumber;
        int e = MainUICon.instance.editNumber;
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
        sl = this.gameObject.MMGetComponentNoAlloc<ChildWindow>().objList[num].MMGetComponentNoAlloc<Selectable>();
        sl2 = MainUICon.instance.secondDrop.MMGetComponentNoAlloc<ChildWindow>().objList[num].MMGetComponentNoAlloc<Selectable>();

        //�㉺�̑I���������A���s���ĂȂ�������
        UnderSet(sl, null);
        UpSet(sl2, null);
        //�܂��Z���N�g������
        MainUICon.instance.beforeSet = null;
        this.gameObject.MMGetComponentNoAlloc<UIPlaceSet>().PlaceSet();
        if (s == 1)
        {
            AttackJudge editJudge = new AttackJudge();
            //�ݒ�
            #region
            editJudge = sis.targetCondition[e];
            #endregion

            if (editJudge.condition == AttackJudge.TargetJudge.�v���C���[����Ԉُ�ɂ��������� ||
               editJudge.condition == AttackJudge.TargetJudge.�������ĂȂ��x�������� ||
               editJudge.condition == AttackJudge.TargetJudge.���G�̑��� ||
               editJudge.condition == AttackJudge.TargetJudge.�w��Ȃ� ||
               editJudge.condition == AttackJudge.TargetJudge.��Ԉُ�ɂ������Ă�G)
            {
                if(MainUICon.instance.valueWindow != null)
                {
                    MainUICon.instance.valueWindow.SetActive(false);
                }
            }
            else
            {
                if(editJudge.condition == AttackJudge.TargetJudge.�G�^�C�v)
                {
                    MainUICon.instance.valueWindow = value[0].gameObject;
                }
                else
                {
                    MainUICon.instance.valueWindow = value[1].gameObject;
                  //  MainUICon.instance.valueWindow
                }
                MainUICon.instance.isChange = 1;
                MainUICon.instance.valueWindow.SetActive(true);
                valueCheck = true;
                MainUICon.instance.valueWindow.MMGetComponentNoAlloc<UIPlaceSet>().PlaceSet();
            }
                MainUICon.instance.secondDrop.SetActive(true);
                MainUICon.instance.secondDrop.MMGetComponentNoAlloc<UIPlaceSet>().PlaceSet();
        }
        else if(s == 2)
        {
            //MainUICon.instance.valueWindow.SetActive(false);
            FireCondition editAT;
            //���e���蓖��


            editAT = sis.AttackCondition[e];

            if (editAT.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ� || editAT.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs ||
                editAT.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
            {
                MainUICon.instance.secondDrop.SetActive(false);
               // thirdDrop.SetActive(false);
            }
            else
            {
                MainUICon.instance.secondDrop.SetActive(true);
                MainUICon.instance.secondDrop.MMGetComponentNoAlloc<UIPlaceSet>().PlaceSet();
            }
        }
        else if (s == 3 || s == 4)
        {
            SupportCondition editSP;
            //�ݒ�

            editSP = sis.supportPlan[e];

            if (s == 3)
            {
                if(editSP.sCondition == SupportCondition.SupportStatus.���G�����邩�ǂ��� || editSP.sCondition == SupportCondition.SupportStatus.�w��Ȃ� || editSP.sCondition == SupportCondition.SupportStatus.�v���C���[����Ԉُ�ɂ���������
                    || editSP.sCondition == SupportCondition.SupportStatus.�C�ӂ̎x�����؂�Ă���Ƃ�)
                {
                    if (MainUICon.instance.valueWindow != null)
                    {
                        MainUICon.instance.valueWindow.SetActive(false);
                    }
                    if (editSP.sCondition == SupportCondition.SupportStatus.�C�ӂ̎x�����؂�Ă���Ƃ�)
                    {
                        MainUICon.instance.secondDrop.SetActive(true);
                        MainUICon.instance.secondDrop.MMGetComponentNoAlloc<UIPlaceSet>().PlaceSet();
                    }
                }
                else
                {
                    if (editSP.sCondition == SupportCondition.SupportStatus.�G�^�C�v)
                    {
                        MainUICon.instance.valueWindow = value[0].gameObject;
                    }
                    else
                    {
                        MainUICon.instance.valueWindow = value[1].gameObject;

                    }

                    MainUICon.instance.valueWindow.SetActive(true);
                    MainUICon.instance.isChange = 1;
                    valueCheck = true;
                    MainUICon.instance.valueWindow.MMGetComponentNoAlloc<UIPlaceSet>().PlaceSet();
                }
            }
            else
            {
                if (editSP.ActBase != SupportCondition.MagicJudge.�e��x�����@)
                {
                    MainUICon.instance.secondDrop.SetActive(false);
                }
                else
                {
                    MainUICon.instance.secondDrop.SetActive(true);
                    MainUICon.instance.secondDrop.MMGetComponentNoAlloc<UIPlaceSet>().PlaceSet();
            }
            }

        }
        else if (s == 5 || s == 6)
        {
            RecoverCondition editRC;
            //�ݒ�
            if (MainUICon.instance.isAH)
            {
                editRC = sis.nRecoverCondition[e];
            }
            else
            {
                editRC = sis.recoverCondition[e];
            }
            if (s == 5)
            {
                if(editRC.condition == RecoverCondition.RecoverStatus.���G�����邩�ǂ��� || editRC.condition == RecoverCondition.RecoverStatus.�w��Ȃ� ||
                    editRC.condition == RecoverCondition.RecoverStatus.�v���C���[����Ԉُ�ɂ��������� || editRC.condition == RecoverCondition.RecoverStatus.�C�ӂ̎x�����؂�Ă���Ƃ�)
                {
                    if (MainUICon.instance.valueWindow != null)
                    {
                        MainUICon.instance.valueWindow.SetActive(false);
                    }
                    if (editRC.condition == RecoverCondition.RecoverStatus.�C�ӂ̎x�����؂�Ă���Ƃ�)
                    {
                        MainUICon.instance.secondDrop.SetActive(true);
                        MainUICon.instance.secondDrop.MMGetComponentNoAlloc<UIPlaceSet>().PlaceSet();

                    }
                }
                else
                {
                    if(editRC.condition == RecoverCondition.RecoverStatus.�G�^�C�v)
                    {
                        MainUICon.instance.valueWindow = value[0].gameObject;

                    }
                    else
                    {
                        MainUICon.instance.valueWindow = value[1].gameObject;

                    }
                        MainUICon.instance.valueWindow.SetActive(true);
                        MainUICon.instance.isChange = 1;
                        valueCheck = true;
                        MainUICon.instance.valueWindow.MMGetComponentNoAlloc<UIPlaceSet>().PlaceSet();
            }
            }
            else
            {
                if(editRC.ActBase != RecoverCondition.MagicJudge.�������@)
                {
                    MainUICon.instance.secondDrop.SetActive(false);
                }
                else
                {
                    MainUICon.instance.secondDrop.SetActive(true);
                    MainUICon.instance.secondDrop.MMGetComponentNoAlloc<UIPlaceSet>().PlaceSet();
                }
            }
        }

        if (!valueCheck && MainUICon.instance.secondDrop.activeSelf)
        {
            sl = this.gameObject.MMGetComponentNoAlloc<ChildWindow>().objList[num].MMGetComponentNoAlloc<Selectable>();
            sl2 = MainUICon.instance.secondDrop.MMGetComponentNoAlloc<ChildWindow>().objList[num].MMGetComponentNoAlloc<Selectable>();

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
