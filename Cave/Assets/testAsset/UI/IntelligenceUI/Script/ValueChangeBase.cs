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
    public void NumberSet()
    {
        int s = MainUICon.instance.settingNumber;
        int e = MainUICon.instance.editNumber;
        //�h���b�v�_�E���Ȃ�
        //  0�ȏ�3�ȉ�
        if (fase >= 0 && fase < 3)
        {
            //�h���b�v�_�E���������Q�Ƃ��Ă邩��UI�̃h���b�v�_�E���̂�����߂�
            //objList����h���b�v�_�E�����������肾��
            //���ƃI�u�W�F�N�g���X�g��S��e�ł��܂�����SetActive�𑀂�B
            if (s == 1)
            {
                AttackJudge editor = GetTarget(e);
             //   Dropdown dd = objList[0].GetComponent<Dropdown>();
                //�W�I
                //������ƕ��G������
                if (fase == 0)
                {
                    //�^�C�v�[���͌܌���


                    if (editor.condition == AttackJudge.TargetJudge.�G�^�C�v)
                    {
                        numberSave = 0;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.�v���C���[��HP���K��l�ɒB������)
                    {
                        numberSave = 1;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.�v���C���[��MP���K��l�ɒB������)
                    {
                        numberSave = 2;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.������MP���K��l�ɒB������)
                    {
                        numberSave = 3;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.�v���C���[����Ԉُ�ɂ���������)
                    {
                        numberSave = 4;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.���G�̑���)
                    {
                        if (editor.highOrLow)
                        {
                            numberSave = 5;
                        }
                        else
                        {
                            numberSave = 6;
                        }
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.��Ԉُ�ɂ������Ă�G)
                    {
                        numberSave = 7;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.�������ĂȂ��x��������)
                    {
                        
                        numberSave = 8;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.�w��Ȃ�)
                    {
                        numberSave = 9;
                    }

                }
                else if (fase == 1)
                {
                    if (editor.wp == AttackJudge.WeakPoint.�a������)
                    {
                        numberSave = 0;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.�h�ˑ���)
                    {
                        numberSave = 1;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.�Ō�����)
                    {
                        numberSave = 2;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.������)
                    {
                        numberSave = 3;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.�ő���)
                    {
                        numberSave = 4;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.������)
                    {
                        numberSave = 5;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.������)
                    {
                        numberSave = 6;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.�ő���)
                    {
                        numberSave = 7;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.�w��Ȃ�)
                    {
                        numberSave = 8;
                    }
                }
                else if (fase == 2)
                {
                    if (editor.nextCondition == AttackJudge.AdditionalJudge.�G��HP)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 0;
                        }
                        else
                        {
                            numberSave = 1;
                        }
                    }
                    else if (editor.nextCondition == AttackJudge.AdditionalJudge.�G�̋���)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 2;
                        }
                        else
                        {
                            numberSave = 3;
                        }
                    }
                    else if (editor.nextCondition == AttackJudge.AdditionalJudge.�G�̍��x)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 4;
                        }
                        else
                        {
                            numberSave = 5;
                        }
                    }
                    else if (editor.nextCondition == AttackJudge.AdditionalJudge.�G�̍U����)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 6;
                        }
                        else
                        {
                            numberSave = 7;
                        }
                    }
                    else if (editor.nextCondition == AttackJudge.AdditionalJudge.�G�̖h���)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 8;
                        }
                        else
                        {
                            numberSave = 9;
                        }
                    }
                    else if (editor.nextCondition == AttackJudge.AdditionalJudge.�w��Ȃ�)
                    {
                        numberSave = 10;
                    }

                }
            }
            //�U��
            else if (s == 2)
            {
                FireCondition editor = GetAttack(e);
           //     Dropdown dd = objList[1].GetComponent<Dropdown>();
                //������ƕ��G������
                if (fase == 0)
                {
                    if (editor.condition == FireCondition.ActJudge.�a������)
                    {
                        numberSave = 0;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�h�ˑ���)
                    {
                        numberSave = 1;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�Ō�����)
                    {
                        numberSave = 2;
                    }
                    else if (editor.condition == FireCondition.ActJudge.������)
                    {
                        numberSave = 3;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�ő���)
                    {
                        numberSave = 4;
                    }
                    else if (editor.condition == FireCondition.ActJudge.������)
                    {
                        numberSave = 5;
                    }
                    else if (editor.condition == FireCondition.ActJudge.������)
                    {
                        numberSave = 6;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�ő���)
                    {
                        numberSave = 7;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�ړ����x�ቺ�U��)
                    {
                        numberSave = 8;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�U���͒ቺ�U��)
                    {
                        numberSave = 9;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�h��͒ቺ�U��)
                    {
                        numberSave = 10;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�����w��Ȃ�)
                    {
                        numberSave = 11;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
                    {
                        numberSave = 12;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs)
                    {
                        numberSave = 13;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
                    {
                        numberSave = 14;
                    }

                }
                else if (fase == 1)
                {
                    if (editor.firstCondition == FireCondition.FirstCondition.�G�𐁂���΂�)
                    {
                        numberSave = 0;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.�ђʂ���)
                    {
                        numberSave = 1;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.�ݒu�U��)
                    {
                        numberSave = 2;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.�͈͍U��)
                    {
                        numberSave = 3;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.�ǔ�����)
                    {
                        numberSave = 4;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.�T�[�`�U��)
                    {
                        numberSave = 5;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.�w��Ȃ�)
                    {
                        numberSave = 6;
                    }
                }
                else if (fase == 2)
                {
                    if (editor.nextCondition == FireCondition.AdditionalCondition.���ː�)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 0;
                        }
                        else
                        {
                            numberSave = 1;
                        }
                    }
                    else if (editor.nextCondition == FireCondition.AdditionalCondition.�r������)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 2;
                        }
                        else
                        {
                            numberSave = 3;
                        }
                    }
                    else if (editor.nextCondition == FireCondition.AdditionalCondition.�U����)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 4;
                        }
                        else
                        {
                            numberSave = 5;
                        }
                    }
                    else if (editor.nextCondition == FireCondition.AdditionalCondition.���l)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 6;
                        }
                        else
                        {
                            numberSave = 7;
                        }
                    }
                    else if (editor.nextCondition == FireCondition.AdditionalCondition.MP�g�p��)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 8;
                        }
                        else
                        {
                            numberSave = 9;
                        }
                    }
                    else if (editor.nextCondition == FireCondition.AdditionalCondition.�w��Ȃ�)
                    {
                        numberSave = 10;
                    }


                }
            }
            else if (s == 3)
            {
                SupportCondition editor = GetSupport(e);
                //����͍ŏ��̃h���b�v�_�E������
               // Dropdown dd = objList[4].GetComponent<Dropdown>();
                if (fase == 0)
                {
                    if (editor.sCondition == SupportCondition.SupportStatus.�G�^�C�v)
                    {
                        numberSave = 0;
                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.�v���C���[�̗̑͂��K��l�̎�)
                    {
                        numberSave = 1;
                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.�v���C���[��MP���K��l�ɒB������)
                    {
                        numberSave = 2;
                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.������MP���K��l�ɒB������)
                    {
                        numberSave = 3;
                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.�v���C���[����Ԉُ�ɂ���������)
                    {
                        numberSave = 4;
                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.���G�����邩�ǂ���)
                    {
                        if (editor.highOrLow)
                        {
                            numberSave = 5;
                        }
                        else
                        {
                            numberSave = 6;
                        }

                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.�C�ӂ̎x�����؂�Ă���Ƃ�)
                    {
                        numberSave = 7;
                    }

                    else if (editor.sCondition == SupportCondition.SupportStatus.�w��Ȃ�)
                    {
                        numberSave = 8;
                    }

                }
                else if (fase == 1)
                {
                    if (editor.needSupport == SisMagic.SupportType.�U������)
                    {
                        numberSave = 0;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.�h�䋭��)
                    {
                        numberSave = 1;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.�G���`�����g)
                    {
                        numberSave = 2;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.�A�N�V��������)
                    {
                        numberSave = 3;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.�o���A)
                    {
                        numberSave = 4;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.���W�F�l)
                    {
                        numberSave = 5;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.�Ȃ�)
                    {
                        numberSave = 6;
                    }
                }

            }
            else if (s == 4)
            {
                SupportCondition editor = GetSupport(e);
              //  Dropdown dd = objList[2].GetComponent<Dropdown>();
                //������ƕ��G������
                if (fase == 0)
                {
                    if (editor.ActBase == SupportCondition.MagicJudge.�e��x�����@)
                    {
                        numberSave = 0;
                    }
                    else if (editor.ActBase == SupportCondition.MagicJudge.�U���X�e�[�g��)
                    {
                        numberSave = 1;
                    }
                    else if (editor.ActBase == SupportCondition.MagicJudge.�񕜃X�e�[�g��)
                    {
                        numberSave = 2;
                    }
                    else if (editor.ActBase == SupportCondition.MagicJudge.�Ȃɂ����Ȃ�)
                    {
                        numberSave = 3;
                    }
                }
                else if (fase == 1)
                {
                    if (editor.useSupport == SisMagic.SupportType.�U������)
                    {
                        numberSave = 0;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.�h�䋭��)
                    {
                        numberSave = 1;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.�G���`�����g)
                    {
                        numberSave = 2;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.�A�N�V��������)
                    {
                        numberSave = 3;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.�o���A)
                    {
                        numberSave = 4;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.���W�F�l)
                    {
                        numberSave = 5;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.�Ȃ�)
                    {
                        numberSave = 6;
                    }
                }
                else if (fase == 2)
                {
                    if (editor.nextCondition == SupportCondition.AdditionalJudge.�r������)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 0;
                        }
                        else
                        {
                            numberSave = 1;
                        }
                    }
                    else if (editor.nextCondition == SupportCondition.AdditionalJudge.�������ʎ���)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 2;
                        }
                        else
                        {
                            numberSave = 3;
                        }
                    }
                    else if (editor.nextCondition == SupportCondition.AdditionalJudge.MP�g�p��)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 4;
                        }
                        else
                        {
                            numberSave = 5;
                        }
                    }
                    else if (editor.nextCondition == SupportCondition.AdditionalJudge.�����{��)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 6;
                        }
                        else
                        {
                            numberSave = 7;
                        }
                    }
                    else if (editor.nextCondition == SupportCondition.AdditionalJudge.�w��Ȃ�)
                    {
                        numberSave = 8;
                    }

                }
            }
            else if (s == 5)
            {
                RecoverCondition editor = GetRecover(e);
                //����͍ŏ��̃h���b�v�_�E������
            //    Dropdown dd = objList[4].GetComponent<Dropdown>();
                if (fase == 0)
                {
                    if (editor.condition == RecoverCondition.RecoverStatus.�G�^�C�v)
                    {
                        numberSave = 0;
                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.�v���C���[��HP���K��l�̎�)
                    {
                        numberSave = 1;
                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.�v���C���[��MP���K��l�ɒB������)
                    {
                        numberSave = 2;
                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.������MP���K��l�ɒB������)
                    {
                        numberSave = 3;
                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.�v���C���[����Ԉُ�ɂ���������)
                    {
                        numberSave = 4;
                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.���G�����邩�ǂ���)
                    {
                        if (editor.highOrLow)
                        {
                            numberSave = 5;
                        }
                        else
                        {
                            numberSave = 6;
                        }

                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.�C�ӂ̎x�����؂�Ă���Ƃ�)
                    {
                        numberSave = 7;
                    }

                    else if (editor.condition == RecoverCondition.RecoverStatus.�w��Ȃ�)
                    {
                        numberSave = 8;
                    }

                }
                else if (fase == 1)
                {
                    if (editor.needSupport == SisMagic.SupportType.�U������)
                    {
                        numberSave = 0;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.�h�䋭��)
                    {
                        numberSave = 1;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.�G���`�����g)
                    {
                        numberSave = 2;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.�A�N�V��������)
                    {
                        numberSave = 3;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.�o���A)
                    {
                        numberSave = 4;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.���W�F�l)
                    {
                        numberSave = 5;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.�Ȃ�)
                    {
                        numberSave = 6;
                    }
                }
            }
            else
            {
                RecoverCondition editor = GetRecover(e);
               // Dropdown dd = objList[3].GetComponent<Dropdown>();
                //������ƕ��G������
                if (fase == 0)
                {
                    if (editor.ActBase == RecoverCondition.MagicJudge.�������@)
                    {
                        numberSave = 0;
                    }
                    else if (editor.ActBase == RecoverCondition.MagicJudge.�U���X�e�[�g��)
                    {
                        numberSave = 1;
                    }
                    else if (editor.ActBase == RecoverCondition.MagicJudge.�x���X�e�[�g��)
                    {
                        numberSave = 2;
                    }
                    else if (editor.ActBase == RecoverCondition.MagicJudge.�Ȃɂ����Ȃ�)
                    {
                        numberSave = 3;
                    }
                }
                else if (fase == 1)
                {
                    if (editor.useSupport == SisMagic.SupportType.�U������)
                    {
                        numberSave = 0;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.�h�䋭��)
                    {
                        numberSave = 1;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.�G���`�����g)
                    {
                        numberSave = 2;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.�A�N�V��������)
                    {
                        numberSave = 3;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.�o���A)
                    {
                        numberSave = 4;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.���W�F�l)
                    {
                        numberSave = 5;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.�Ȃ�)
                    {
                        numberSave = 6;
                    }
                }
                else if (fase == 2)
                {
                    if (editor.nextCondition == RecoverCondition.AdditionalJudge.�r������)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 0;
                        }
                        else
                        {
                            numberSave = 1;
                        }
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.�������ʎ���)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 2;
                        }
                        else
                        {
                            numberSave = 3;
                        }
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.MP�g�p��)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 4;
                        }
                        else
                        {
                            numberSave = 5;
                        }
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.�񕜗�)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 6;
                        }
                        else
                        {
                            numberSave = 7;
                        }
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.���W�F�l�񕜗�)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 8;
                        }
                        else
                        {
                            numberSave = 9;
                        }
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.��Ԉُ��)
                    {
                        numberSave = 10;
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.�w��Ȃ�)
                    {
                        numberSave = 11;
                    }
                }
            }

        }
    }



    // Update is called once per frame

    /// <summary>
    /// ���l��K�p���邽�߂̃��\�b�h
    /// </summary>
    public void ApplyValue()
    {

        int s = MainUICon.instance.settingNumber;
        int e = MainUICon.instance.editNumber;
        if (s == 1)
        {
            AttackJudge editJudge = MainUICon.instance.editParameter.targetCondition[e];

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
                //    Debug.Log($"ebihurai{gameObject.name}");
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
            MainUICon.instance.editParameter.targetCondition[e] = editJudge;

        }
        //�U���I���̎�
        else if (s == 2)
        {
            FireCondition editAT;
            //���e���蓖��

            editAT = MainUICon.instance.editParameter.AttackCondition[e];

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
                //  MainUICon.instance.editParameter.firstTarget.wp = saveWeak;

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
            //UseMagic���Ȃ���
            editAT.UseMagic = null;

            MainUICon.instance.editParameter.AttackCondition[e] = editAT;

        }
        //�x�������̎�
        else if (s == 3 || s == 4)
        {
            SupportCondition editSP;
            //�ݒ�

            editSP = MainUICon.instance.editParameter.supportPlan[e];
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
                        editSP.highOrLow = true;
                    }
                    else if (numberSave == 6)
                    {
                        editSP.sCondition = SupportCondition.SupportStatus.���G�����邩�ǂ���;
                        editSP.highOrLow = false;
                    }
                    else if (numberSave == 7)
                    {
                        editSP.sCondition = SupportCondition.SupportStatus.�C�ӂ̎x�����؂�Ă���Ƃ�;
                    }
                    else if (numberSave == 8)
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
            //UseMagic���Ȃ���
            editSP.UseMagic = null;
            //�ݒ�
            MainUICon.instance.editParameter.supportPlan[e] = editSP;
        }
        //�x���s���I��

        //�񕜑I��
        else if (s == 5 || s == 6)
        {
            RecoverCondition editRC;
            //�ݒ�
            #region
            if (!MainUICon.instance.isAH)
            {
                editRC = MainUICon.instance.editParameter.recoverCondition[e];
            }
            else
            {
                editRC = MainUICon.instance.editParameter.nRecoverCondition[e];
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
                        editRC.highOrLow = true;
                    }
                    else if (numberSave == 6)
                    {
                        editRC.condition = RecoverCondition.RecoverStatus.���G�����邩�ǂ���;
                        editRC.highOrLow = false;
                    }
                    else if (numberSave == 7)
                    {
                        editRC.condition = RecoverCondition.RecoverStatus.�C�ӂ̎x�����؂�Ă���Ƃ�;
                    }
                    else if (numberSave == 8)
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

            //UseMagic���Ȃ���
            editRC.UseMagic = null;

            #region
            if (!MainUICon.instance.isAH)
            {
                MainUICon.instance.editParameter.recoverCondition[e] = editRC;
            }
            else
            {
                MainUICon.instance.editParameter.nRecoverCondition[e] = editRC;
            }
            #endregion
        }


        MainUICon.instance.isSave = false;
        //MainUICon.instance.editParameter = sis;
    }

    //abstract public void ValueSet(ref int valueInt,ref bool valueBool) ;


    public void EditStart()
    {
        MainUICon.instance.editNow = true;
    }

    public void EditEnd()
    {
        MainUICon.instance.editNow = false;
    }


    AttackJudge GetTarget(int e)
    {
        return MainUICon.instance.editParameter.targetCondition[e];
    }
    FireCondition GetAttack(int e)
    {
        return MainUICon.instance.editParameter.AttackCondition[e];
    }

    SupportCondition GetSupport(int e)
    {
        return MainUICon.instance.editParameter.supportPlan[e];
    }
    RecoverCondition GetRecover(int e)
    {
        if (!MainUICon.instance.isAH)
        {
            return MainUICon.instance.editParameter.recoverCondition[e];
        }
        else
        {
            return MainUICon.instance.editParameter.nRecoverCondition[e];
        }
    }
}
