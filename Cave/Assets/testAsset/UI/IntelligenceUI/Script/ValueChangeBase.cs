using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��{�I�Ƀx�[�X�X�N���v�g�^�p�B
/// ������p�������X�N���v�g�̂��ꂼ��G�^�C�v���Ƃ��Ȃ�Ƃ��Łi�_���ςƂ��́j�l�ݒ�͂���āA�Ō��ApplyValue���g��
/// fase�͊e�{�^���Ō��߂āAset��edit�͂ڂ��ڂ����߂Ă�B���ƌp����̃X�N���v�g�ŕ\�����镶���ς�����B
/// ���ƌ���ɂɎ��̑����Ăяo���@�\��
/// </summary>
public  class ValueChangeBase : MonoBehaviour
{
    ///<summary>
    /// �ݒ蒆�̃E�B���h�E�͂Ȃɂ�
    /// ��{�Ƃ��U���Ƃ�
    /// ����ɍU��0~2�A��3~5�Ƃ��ɂ��ĂP�Ȃ����2�Ȃ�s���Ƃ������ɕ�����
    ///</summary>
    //   [HideInInspector]
    //  public int settingNumber;

    ///<summary>
    /// ���Ԗڂ̏�����ҏW�����B�Q�Ɛ�l��
    /// �������ҏW���Ƃ�
    /// �Z�b�e�B���O�i���o�[�ƕ��p�œ���ł���
    ///</summary>
    //[HideInInspector]
    // public int editNumber;

    ///<summary>
    ///�����i�K�ڂ̐ݒ肵�Ă邩�B�C���X�y�N�^����s���B
    ///�Ⴆ�΃^�[�Q�b�g�Ȃ�WP��NextCondition���B�X�N���v�g���Ō��߂�B�ς���B
    ///</summary>
    public int fase;

    /// <summary>
    /// �e�X�N���v�g�Œl��ۑ��������
    /// </summary>
    protected int numberSave;

    /// <summary>
    /// �e�X�N���v�g�Œl��ۑ��������
    /// </summary>
    protected bool boolSave;


    // Start is called before the first frame update
    virtual protected void Start()
    {
        
    }

    // Update is called once per frame

    /// <summary>
    /// ���l��K�p���邽�߂̃��\�b�h
    /// </summary>
    public void ApplyValue()
    {
        SisterParameter sis = MainUI.instance.editParameter;
        int s = MainUI.instance.settingNumber;
        int e = MainUI.instance.editNumber;
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

            ///<summary>
            ///�^�[�Q�b�g�ݒ�
            /// </summary>
            #region
            if (fase == 0)
            {
                if (numberSave == 0)
                {
                    editJudge.condition = AttackJudge.TargetJudge.�G�^�C�v;
                }
                else if (numberSave == 1)
                {
                    editJudge.condition = AttackJudge.TargetJudge.�v���C���[��HP���K��l�ɒB������;
                }
                else if (numberSave == 2)
                {
                    editJudge.condition = AttackJudge.TargetJudge.�v���C���[��MP���K��l�ɒB������;
                }
                else if (numberSave == 3)
                {
                    editJudge.condition = AttackJudge.TargetJudge.������MP���K��l�ɒB������;
                }
                else if (numberSave == 4)
                {
                    editJudge.condition = AttackJudge.TargetJudge.�v���C���[����Ԉُ�ɂ���������;
                }
                else if (numberSave == 5)
                {
                    editJudge.condition = AttackJudge.TargetJudge.���G�̑���;
                    editJudge.highOrLow = true;
                }
                else if (numberSave == 6)
                {
                    editJudge.condition = AttackJudge.TargetJudge.���G�̑���;
                    editJudge.highOrLow = false;
                }
                else if (numberSave == 7)
                {
                    editJudge.condition = AttackJudge.TargetJudge.��Ԉُ�ɂ������Ă�G;
                }
                else if (numberSave == 8)
                {
                    editJudge.condition = AttackJudge.TargetJudge.�������ĂȂ��x��������;
                }
                else if (numberSave == 9)
                {
                    editJudge.condition = AttackJudge.TargetJudge.�w��Ȃ�;
                }

            }
            else if (fase == 1)
            {
                editJudge.percentage = numberSave;
                editJudge.highOrLow = boolSave;
            }
            else if (fase == 2)
            {
                //  editJudge.wp = saveWeak;

                if (numberSave == 0)
                {
                    editJudge.wp = AttackJudge.WeakPoint.�a������;
                }
                else if (numberSave == 1)
                {
                    editJudge.wp = AttackJudge.WeakPoint.�h�ˑ���;
                }
                else if (numberSave == 2)
                {
                    editJudge.wp = AttackJudge.WeakPoint.�Ō�����;
                }
                else if (numberSave == 3)
                {
                    editJudge.wp = AttackJudge.WeakPoint.������;
                }
                else if (numberSave == 4)
                {
                    editJudge.wp = AttackJudge.WeakPoint.�ő���;
                }
                else if (numberSave == 5)
                {
                    editJudge.wp = AttackJudge.WeakPoint.������;
                }
                else if (numberSave == 6)
                {
                    editJudge.wp = AttackJudge.WeakPoint.������;
                }
                else if (numberSave == 7)
                {
                    editJudge.wp = AttackJudge.WeakPoint.�ő���;
                }
                else if (numberSave == 8)
                {
                    editJudge.wp = AttackJudge.WeakPoint.�w��Ȃ�;
                }
            }
            else if (fase == 3)
            {
                editJudge.upDown = false;
                if (numberSave == 0)
                {
                    editJudge.upDown = true;
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.�G��HP;
                }
                else if (numberSave == 1)
                {
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.�G��HP;
                }
                else if (numberSave == 2)
                {
                    editJudge.upDown = true;
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.�G�̋���;
                }
                else if (numberSave == 3)
                {
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.�G�̋���;
                }
                else if (numberSave == 4)
                {
                    editJudge.upDown = true;
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.�G�̍��x;
                }
                else if (numberSave == 5)
                {
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.�G�̍��x;
                }
                else if (numberSave == 6)
                {
                    editJudge.upDown = true;
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.�G�̍U����;
                }
                else if (numberSave == 7)
                {
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.�G�̍U����;
                }
                else if (numberSave == 8)
                {
                    editJudge.upDown = true;
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.�G�̖h���;
                }
                else if (numberSave == 9)
                {
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.�G�̖h���;
                }
                else if (numberSave == 10)
                {
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.�w��Ȃ�;
                }
                
            }
            #endregion

            //�ݒ�
            #region
            if (e == 1)
            {
                sis.firstTarget = editJudge;
            }
            else if (e == 2)
            {
                sis.secondTarget = editJudge;
            }
            else if (e == 3)
            {
                sis.thirdTarget = editJudge;
            }
            else if (e == 4)
            {
                sis.forthTarget = editJudge;
            }
            else if (e == 5)
            {
                sis.fiveTarget = editJudge;
            }
            #endregion
        }
        //�U���I���̎�
        else if (s == 2)
        {
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
            //�s��
            //����
            //����2�{�^�U�l
            ///<summary>
            ///�U���I��
            /// </summary>
            #region
            if (fase == 0)
            {
                if (numberSave == 0)
                {
                    editAT.condition = FireCondition.ActJudge.�a������;
                }
                else if (numberSave == 1)
                {
                    editAT.condition = FireCondition.ActJudge.�h�ˑ���;
                }
                else if (numberSave == 2)
                {
                    editAT.condition = FireCondition.ActJudge.�Ō�����;
                }
                else if (numberSave == 3)
                {
                    editAT.condition = FireCondition.ActJudge.������;
                }
                else if (numberSave == 4)
                {
                    editAT.condition = FireCondition.ActJudge.�ő���;
                }
                else if (numberSave == 5)
                {
                    editAT.condition = FireCondition.ActJudge.������;
                }
                else if (numberSave == 6)
                {
                    editAT.condition = FireCondition.ActJudge.������;
                }
                else if (numberSave == 7)
                {
                    editAT.condition = FireCondition.ActJudge.�ő���;
                }
                else if (numberSave == 8)
                {
                    editAT.condition = FireCondition.ActJudge.�ړ����x�ቺ�U��;
                }
                else if (numberSave == 9)
                {
                    editAT.condition = FireCondition.ActJudge.�U���͒ቺ�U��;
                }
                else if (numberSave == 10)
                {
                    editAT.condition = FireCondition.ActJudge.�h��͒ቺ�U��;
                }
                else if (numberSave == 11)
                {
                    editAT.condition = FireCondition.ActJudge.�����w��Ȃ�;
                }
                else if (numberSave == 12)
                {
                    editAT.condition = FireCondition.ActJudge.�x���s���Ɉڍs;
                }
                else if (numberSave == 13)
                {
                    editAT.condition = FireCondition.ActJudge.�񕜍s���Ɉڍs;
                }
                else if (numberSave == 14)
                {
                    editAT.condition = FireCondition.ActJudge.�Ȃɂ����Ȃ�;
                }
            }
            else if (fase == 1)
            {
                //  sis.firstTarget.wp = saveWeak;

                if (numberSave == 0)
                {
                    editAT.firstCondition = FireCondition.FirstCondition.�G�𐁂���΂�;
                }
                else if (numberSave == 1)
                {
                    editAT.firstCondition = FireCondition.FirstCondition.�ђʂ���;
                }
                else if (numberSave == 2)
                {
                    editAT.firstCondition = FireCondition.FirstCondition.�ݒu�U��;
                }
                else if (numberSave == 3)
                {
                    editAT.firstCondition = FireCondition.FirstCondition.�͈͍U��;
                }
                else if (numberSave == 4)
                {
                    editAT.firstCondition = FireCondition.FirstCondition.�ǔ�����;
                }
                else if (numberSave == 5)
                {
                    editAT.firstCondition = FireCondition.FirstCondition.�T�[�`�U��;
                }
                else if (numberSave == 6)
                {
                    editAT.firstCondition = FireCondition.FirstCondition.�w��Ȃ�;
                }

            }
            else if (fase == 2)
            {
                editAT.upDown = false;
                if (numberSave == 0)
                {
                    editAT.upDown = true;
                    editAT.nextCondition = FireCondition.AdditionalCondition.���ː�;
                }
                else if (numberSave == 1)
                {
                    editAT.nextCondition = FireCondition.AdditionalCondition.���ː�;
                }
                else if (numberSave == 2)
                {
                    editAT.upDown = true;
                    editAT.nextCondition = FireCondition.AdditionalCondition.�r������;
                }
                else if (numberSave == 3)
                {
                    editAT.nextCondition = FireCondition.AdditionalCondition.�r������;
                }
                else if (numberSave == 4)
                {
                    editAT.upDown = true;
                    editAT.nextCondition = FireCondition.AdditionalCondition.�U����;
                }
                else if (numberSave == 5)
                {
                    editAT.nextCondition = FireCondition.AdditionalCondition.�U����;
                }
                else if (numberSave == 6)
                {
                    editAT.upDown = true;
                    editAT.nextCondition = FireCondition.AdditionalCondition.���l;
                }
                else if (numberSave == 7)
                {
                    editAT.nextCondition = FireCondition.AdditionalCondition.���l;
                }
                else if (numberSave == 8)
                {
                    editAT.upDown = true;
                    editAT.nextCondition = FireCondition.AdditionalCondition.MP�g�p��;
                }
                else if (numberSave == 9)
                {
                    editAT.nextCondition = FireCondition.AdditionalCondition.MP�g�p��;
                }
                else if (numberSave == 10)
                {
                    editAT.nextCondition = FireCondition.AdditionalCondition.�w��Ȃ�;
                }  
            }
            #endregion
            #region
            if (e == 1)
            {
                sis.firstAttack = editAT;
            }
            else if (e == 2)
            {
                sis.secondAttack = editAT;
            }
            else if (e == 3)
            {
                sis.thirdAttack = editAT;
            }
            else if (e == 4)
            {
                sis.fourthAttack = editAT;
            }
            else if (e == 5)
            {
                sis.fiveAttack = editAT;
            }
            else if (e == 6)
            {
                sis.nonAttack = editAT;
            }
            #endregion
        }
        //�x�������̎�
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


                ///<summary>
                ///�@�x������
                /// </summary>
                #region
                if (fase == 0)
                {
                    if (numberSave == 0)
                    {
                        editSP.sCondition = SupportCondition.SupportStatus.�G�^�C�v;
                    }
                    else if (numberSave == 1)
                    {
                        editSP.sCondition = SupportCondition.SupportStatus.�v���C���[�̗̑͂��K��l�̎�;
                    }
                    else if (numberSave == 2)
                    {
                        editSP.sCondition = SupportCondition.SupportStatus.�v���C���[��MP���K��l�ɒB������;
                    }
                    else if (numberSave == 3)
                    {
                        editSP.sCondition = SupportCondition.SupportStatus.������MP���K��l�ɒB������;
                    }
                    else if (numberSave == 4)
                    {
                        editSP.sCondition = SupportCondition.SupportStatus.�v���C���[����Ԉُ�ɂ���������;
                    }
                    else if (numberSave == 5)
                    {
                        editSP.sCondition = SupportCondition.SupportStatus.���G�����邩�ǂ���;
                    }
                    else if (numberSave == 6)
                    {
                        editSP.sCondition = SupportCondition.SupportStatus.�C�ӂ̎x�����؂�Ă���Ƃ�;
                    }

                    else if (numberSave == 7)
                    {
                        editSP.sCondition = SupportCondition.SupportStatus.�w��Ȃ�;
                    }
                }
                else if (fase == 1)
                {
                    if (editSP.sCondition != SupportCondition.SupportStatus.�C�ӂ̎x�����؂�Ă���Ƃ�)
                    {
                        editSP.percentage = numberSave;
                        editSP.highOrLow = boolSave;
                    }
                    else
                    {
                        if (numberSave == 0)
                        {
                            editSP.needSupport = SisMagic.SupportType.�U������;
                        }
                        else if (numberSave == 1)
                        {
                            editSP.needSupport = SisMagic.SupportType.�h�䋭��;
                        }
                        else if (numberSave == 2)
                        {
                            editSP.needSupport = SisMagic.SupportType.�G���`�����g;
                        }
                        else if (numberSave == 3)
                        {
                            editSP.needSupport = SisMagic.SupportType.�A�N�V��������;
                        }
                        else if (numberSave == 4)
                        {
                            editSP.needSupport = SisMagic.SupportType.�o���A;
                        }
                        else if (numberSave == 5)
                        {
                            editSP.needSupport = SisMagic.SupportType.���W�F�l;
                        }
                        else if (numberSave == 6)
                        {
                            editSP.needSupport = SisMagic.SupportType.�Ȃ�;
                        }
                    }
                }
                #endregion

            }
            else if (s == 4)
            {
                ///<summary>
                /// �x���s���ݒ�
                /// </summary>
                #region
                if (fase == 0)
                {
                    if (numberSave == 0)
                    {
                        editSP.ActBase = SupportCondition.MagicJudge.�e��x�����@;
                    }
                    else if (numberSave == 1)
                    {
                        editSP.ActBase = SupportCondition.MagicJudge.�U���X�e�[�g��;
                    }
                    else if (numberSave == 2)
                    {
                        editSP.ActBase = SupportCondition.MagicJudge.�񕜃X�e�[�g��;
                    }
                    else if (numberSave == 3)
                    {
                        editSP.ActBase = SupportCondition.MagicJudge.�Ȃɂ����Ȃ�;
                    }
                }
                else if (fase == 1)
                {
                    if (numberSave == 0)
                    {
                        editSP.useSupport = SisMagic.SupportType.�U������;
                    }
                    else if (numberSave == 1)
                    {
                        editSP.useSupport = SisMagic.SupportType.�h�䋭��;
                    }
                    else if (numberSave == 2)
                    {
                        editSP.useSupport = SisMagic.SupportType.�G���`�����g;
                    }
                    else if (numberSave == 3)
                    {
                        editSP.useSupport = SisMagic.SupportType.�A�N�V��������;
                    }
                    else if (numberSave == 4)
                    {
                        editSP.useSupport = SisMagic.SupportType.�o���A;
                    }
                    else if (numberSave == 5)
                    {
                        editSP.useSupport = SisMagic.SupportType.���W�F�l;
                    }
                    else if (numberSave == 6)
                    {
                        editSP.useSupport = SisMagic.SupportType.�Ȃ�;
                    }
                }
                else if (fase == 2)
                {
                    editSP.upDown = false;
                    if (numberSave == 0)
                    {
                        editSP.upDown = true;
                        editSP.nextCondition = SupportCondition.AdditionalJudge.�r������;
                    }
                    else if (numberSave == 1)
                    {
                        editSP.nextCondition = SupportCondition.AdditionalJudge.�r������;
                    }
                    else if (numberSave == 2)
                    {
                        editSP.upDown = true;
                        editSP.nextCondition = SupportCondition.AdditionalJudge.�������ʎ���;
                    }
                    else if (numberSave == 3)
                    {
                        editSP.nextCondition = SupportCondition.AdditionalJudge.�������ʎ���;
                    }
                    else if (numberSave == 4)
                    {
                        editSP.upDown = true;
                        editSP.nextCondition = SupportCondition.AdditionalJudge.MP�g�p��;
                    }
                    else if (numberSave == 5)
                    {
                        editSP.nextCondition = SupportCondition.AdditionalJudge.MP�g�p��;
                    }
                    else if (numberSave == 6)
                    {
                        editSP.upDown = true;
                        editSP.nextCondition = SupportCondition.AdditionalJudge.�����{��;
                    }
                    else if (numberSave == 7)
                    {
                        editSP.nextCondition = SupportCondition.AdditionalJudge.�����{��;
                    }
                    else if (numberSave == 8)
                    {
                        editSP.nextCondition = SupportCondition.AdditionalJudge.�w��Ȃ�;
                    }
                }
                #endregion

            }
            //�ݒ�
            #region
            if (e == 1)
            {
                sis.firstPlan = editSP;
            }
            else if (e == 2)
            {
                sis.secondPlan = editSP;
            }
            else if (e == 3)
            {
                sis.thirdPlan = editSP;
            }
            else if (e == 4)
            {
                sis.forthPlan = editSP;
            }
            else if (e == 5)
            {
                sis.fivePlan = editSP;
            }
            else
            {
                sis.sixPlan = editSP;
            }
            #endregion
        }
        //�x���s���I��

        //�񕜑I��
        else if (s == 5 || s == 6)
        {
            RecoverCondition editRC;
            //�ݒ�
            #region
            if (e == 1)
            {
                editRC = sis.firstRecover;
            }
            else if (e == 2)
            {
                editRC = sis.secondRecover;
            }
            else if (e == 3)
            {
                editRC = sis.thirdRecover;
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
            if (s == 5)
            {


                ///<summary>
                ///�@�x������
                /// </summary>
                #region
                if (fase == 0)
                {
                    if (numberSave == 0)
                    {
                        editRC.condition = RecoverCondition.RecoverStatus.�G�^�C�v;
                    }
                    else if (numberSave == 1)
                    {
                        editRC.condition = RecoverCondition.RecoverStatus.�v���C���[��HP���K��l�̎�;
                    }
                    else if (numberSave == 2)
                    {
                        editRC.condition = RecoverCondition.RecoverStatus.�v���C���[��MP���K��l�ɒB������;
                    }
                    else if (numberSave == 3)
                    {
                        editRC.condition = RecoverCondition.RecoverStatus.������MP���K��l�ɒB������;
                    }
                    else if (numberSave == 4)
                    {
                        editRC.condition = RecoverCondition.RecoverStatus.�v���C���[����Ԉُ�ɂ���������;
                    }
                    else if (numberSave == 5)
                    {
                        editRC.condition = RecoverCondition.RecoverStatus.���G�����邩�ǂ���;
                    }
                    else if (numberSave == 6)
                    {
                        editRC.condition = RecoverCondition.RecoverStatus.�C�ӂ̎x�����؂�Ă���Ƃ�;
                    }

                    else if (numberSave == 7)
                    {
                        editRC.condition = RecoverCondition.RecoverStatus.�w��Ȃ�;
                    }


                }
                else if (fase == 1)
                {
                    if (editRC.condition != RecoverCondition.RecoverStatus.�C�ӂ̎x�����؂�Ă���Ƃ�)
                    {
                        editRC.percentage = numberSave;
                        editRC.highOrLow = boolSave;
                    }
                    else
                    {
                        if (numberSave == 0)
                        {
                            editRC.needSupport = SisMagic.SupportType.�U������;
                        }
                        else if (numberSave == 1)
                        {
                            editRC.needSupport = SisMagic.SupportType.�h�䋭��;
                        }
                        else if (numberSave == 2)
                        {
                            editRC.needSupport = SisMagic.SupportType.�G���`�����g;
                        }
                        else if (numberSave == 3)
                        {
                            editRC.needSupport = SisMagic.SupportType.�A�N�V��������;
                        }
                        else if (numberSave == 4)
                        {
                            editRC.needSupport = SisMagic.SupportType.�o���A;
                        }
                        else if (numberSave == 5)
                        {
                            editRC.needSupport = SisMagic.SupportType.���W�F�l;
                        }
                        else if (numberSave == 6)
                        {
                            editRC.needSupport = SisMagic.SupportType.�Ȃ�;
                        }
                    }
                }
                #endregion

            }
            else if (s == 6)
            {
                ///<summary>
                /// �x���s���ݒ�
                /// </summary>
                #region
                if (fase == 0)
                {
                    if (numberSave == 0)
                    {
                        editRC.ActBase = RecoverCondition.MagicJudge.�������@;
                    }
                    else if (numberSave == 1)
                    {
                        editRC.ActBase = RecoverCondition.MagicJudge.�U���X�e�[�g��;
                    }
                    else if (numberSave == 2)
                    {
                        editRC.ActBase = RecoverCondition.MagicJudge.�x���X�e�[�g��;
                    }
                    else if (numberSave == 3)
                    {
                        editRC.ActBase = RecoverCondition.MagicJudge.�Ȃɂ����Ȃ�;
                    }
                }
                else if (fase == 1)
                {
                    if (numberSave == 0)
                    {
                        editRC.useSupport = SisMagic.SupportType.�U������;
                    }
                    else if (numberSave == 1)
                    {
                        editRC.useSupport = SisMagic.SupportType.�h�䋭��;
                    }
                    else if (numberSave == 2)
                    {
                        editRC.useSupport = SisMagic.SupportType.�G���`�����g;
                    }
                    else if (numberSave == 3)
                    {
                        editRC.useSupport = SisMagic.SupportType.�A�N�V��������;
                    }
                    else if (numberSave == 4)
                    {
                        editRC.useSupport = SisMagic.SupportType.�o���A;
                    }
                    else if (numberSave == 5)
                    {
                        editRC.useSupport = SisMagic.SupportType.���W�F�l;
                    }
                    else if (numberSave == 6)
                    {
                        editRC.useSupport = SisMagic.SupportType.�Ȃ�;
                    }
                }
                else if (fase == 2)
                {
                    editRC.upDown = false;
                    if (numberSave == 0)
                    {
                        editRC.upDown = true;
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.�r������;
                    }
                    else if (numberSave == 1)
                    {
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.�r������;
                    }
                    else if (numberSave == 2)
                    {
                        editRC.upDown = true;
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.�������ʎ���;
                    }
                    else if (numberSave == 3)
                    {
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.�������ʎ���;
                    }
                    else if (numberSave == 4)
                    {
                        editRC.upDown = true;
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.MP�g�p��;
                    }
                    else if (numberSave == 5)
                    {
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.MP�g�p��;
                    }
                    else if (numberSave == 6)
                    {
                        editRC.upDown = true;
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.�񕜗�;
                    }
                    else if (numberSave == 7)
                    {
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.�񕜗�;
                    }
                    else if (numberSave == 8)
                    {
                        editRC.upDown = true;
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.���W�F�l�񕜗�;
                    }
                    else if (numberSave == 9)
                    {
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.���W�F�l�񕜗�;
                    }
                    else if (numberSave == 10)
                    {
                        editRC.upDown = true;
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.���W�F�l���񕜗�;
                    }
                    else if (numberSave == 11)
                    {
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.���W�F�l���񕜗�;
                    }
                    else if (numberSave == 12)
                    {
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.��Ԉُ��;
                    }
                    else if (numberSave == 13)
                    {
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.�w��Ȃ�;
                    }
                   
                }
                #endregion

            }
            //�ݒ�
            #region
            if (e == 1)
            {
                sis.firstRecover = editRC;
            }
            else if (e == 2)
            {
                sis.secondRecover = editRC;
            }
            else if (e == 3)
            {
                sis.thirdRecover = editRC;
            }
            else if (e == 4)
            {
                sis.forthRecover = editRC;
            }
            else if (e == 5)
            {
                sis.fiveRecover = editRC;
            }
            else
            {
                sis.nonRecover = editRC;
            } 
            #endregion
        }



        MainUI.instance.editParameter = sis;
    }

   //abstract public void ValueSet(ref int valueInt,ref bool valueBool) ;




}
