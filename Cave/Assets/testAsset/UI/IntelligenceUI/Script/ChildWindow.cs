using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChildWindow : MonoBehaviour
{
    /// <summary>
    /// �؂�ւ���I�u�W�F�N�g
    /// �l�ݒ�̏ꍇ�v�f�̐���ɂ��g����
    /// �h���b�v�_�E���n�͑ΏہA�U���I���A�x���I���A�񕜑I���A�x�������A�񕜏����̏�
    /// </summary>
    public List<GameObject> objList;

    /// <summary>
    /// �}�X�^�[���ł��邩
    /// </summary>
    [SerializeField]
    bool Master;

    /// <summary>
    /// ���ꂪ�h���b�v�_�E��������
    /// </summary>
    [SerializeField]
    int type;

    /// <summary>
    /// �����p�̕����z��
    /// </summary>
    [SerializeField]
    string[] introduction;

    //�����p�̕���
    [SerializeField]
    Text tx;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnEnable()
    {
        //a�܂��o�O����
        

        if (type >= 0 && type < 3)
        {
            ResetWindow();
            int i;
            //�h���b�v�_�E���������Q�Ƃ��Ă邩��UI�̃h���b�v�_�E���̂�����߂�
            //objList����h���b�v�_�E�����������肾��
            //���ƃI�u�W�F�N�g���X�g��S��e�ł��܂�����SetActive�𑀂�B
            int s = MainUICon.instance.settingNumber;
           // Debug.Log($"���l���H{s}");
            if (s == 1)
            {

                objList[0].SetActive(true);
                i = 0;
            }
            //�U��
            else if (s == 2)
            {
                objList[1].SetActive(true);
                if(type == 1)
                {
                    objList[5].SetActive(true);
                }
                i = 1;
            }
            else if (s == 3)
            {
                i = 4;
                objList[4].SetActive(true);
            }
            else if (s == 4)
            {
                objList[2].SetActive(true);
                if (type == 1)
                {
                    objList[5].SetActive(true);
                }
                i = 2;
            }
            else if (s == 5)
            {
                //����͍ŏ��̃h���b�v�_�E������
                objList[4].SetActive(true);
                i = 4;
            }
            else
            {
                objList[3].SetActive(true);
                if (type == 1)
                {
                    objList[5].SetActive(true);
                }
                i = 3;
            }
             objList[i].MMGetComponentNoAlloc<ValueChangeBase>().NumberSet();
            tx.text = introduction[i];
            if (Master)
            {
                objList[i].MMGetComponentNoAlloc<Selectable>().Select();
                
            }
        }
        //�A�v���C�o�����[�̑O�ɂǂ̃I�u�W�F�N�g��\�����邩�����߂�
        ApplyValue();
    }
    private void OnDisable()
    {
        if (type >= 0 && type < 3)
        {
            ResetWindow();
            tx.text = null;
        }
    }
    /// <summary>
    /// �ݒ肵���菉�������ꂽ�肵���l��UI�ɔ��f
    /// </summary>
    public void ApplyValue()
    {
        int s = MainUICon.instance.settingNumber;
        int e = MainUICon.instance.editNumber;
        //�h���b�v�_�E���Ȃ�
        //  0�ȏ�3�ȉ�
        if (type >= 0 && type < 3)
        {
            //�h���b�v�_�E���������Q�Ƃ��Ă邩��UI�̃h���b�v�_�E���̂�����߂�
            //objList����h���b�v�_�E�����������肾��
            //���ƃI�u�W�F�N�g���X�g��S��e�ł��܂�����SetActive�𑀂�B
            if (s == 1)
            {
                AttackJudge editor = GetTarget(e);
                Dropdown dd = objList[0].MMGetComponentNoAlloc<Dropdown>();
                //�W�I
                //������ƕ��G������
                if (type == 0)
                {
                    //�^�C�v�[���͌܌���


                    if (editor.condition == AttackJudge.TargetJudge.�G�^�C�v)
                    {
                        dd.value = 0;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.�v���C���[��HP���K��l�ɒB������)
                    {
                        dd.value = 1;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.�v���C���[��MP���K��l�ɒB������)
                    {
                        dd.value = 2;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.������MP���K��l�ɒB������)
                    {
                        dd.value = 3;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.�v���C���[����Ԉُ�ɂ���������)
                    {
                        dd.value = 4;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.���G�̑���)
                    {
                        if (editor.highOrLow)
                        {
                            dd.value = 5;
                        }
                        else
                        {
                            dd.value = 6;
                        }
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.��Ԉُ�ɂ������Ă�G)
                    {
                        dd.value = 7;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.�������ĂȂ��x��������)
                    {
                        dd.value = 8;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.�w��Ȃ�)
                    {
                        dd.value = 9;
                    }

                }
                else if (type == 1)
                {
                    if (editor.wp == AttackJudge.WeakPoint.�a������)
                    {
                        dd.value = 0;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.�h�ˑ���)
                    {
                        dd.value = 1;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.�Ō�����)
                    {
                        dd.value = 2;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.������)
                    {
                        dd.value = 3;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.�ő���)
                    {
                        dd.value = 4;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.������)
                    {
                        dd.value = 5;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.������)
                    {
                        dd.value = 6;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.�ő���)
                    {
                        dd.value = 7;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.�w��Ȃ�)
                    {
                        dd.value = 8;
                    }
                }
                else if (type == 2)
                {
                    if (editor.nextCondition == AttackJudge.AdditionalJudge.�G��HP)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 0;
                        }
                        else
                        {
                            dd.value = 1;
                        }
                    }
                    else if (editor.nextCondition == AttackJudge.AdditionalJudge.�G�̋���)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 2;
                        }
                        else
                        {
                            dd.value = 3;
                        }
                    }
                    else if (editor.nextCondition == AttackJudge.AdditionalJudge.�G�̍��x)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 4;
                        }
                        else
                        {
                            dd.value = 5;
                        }
                    }
                    else if (editor.nextCondition == AttackJudge.AdditionalJudge.�G�̍U����)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 6;
                        }
                        else
                        {
                            dd.value = 7;
                        }
                    }
                    else if (editor.nextCondition == AttackJudge.AdditionalJudge.�G�̖h���)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 8;
                        }
                        else
                        {
                            dd.value = 9;
                        }
                    }
                    else if (editor.nextCondition == AttackJudge.AdditionalJudge.�w��Ȃ�)
                    {
                        dd.value = 10;
                    }

                }
            }
            //�U��
            else if (s == 2)
            {
                FireCondition editor = GetAttack(e);
                Dropdown dd = objList[1].MMGetComponentNoAlloc<Dropdown>();
                //������ƕ��G������
                if (type == 0)
                {
                    if (editor.condition == FireCondition.ActJudge.�a������)
                    {
                        dd.value = 0;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�h�ˑ���)
                    {
                        dd.value = 1;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�Ō�����)
                    {
                        dd.value = 2;
                    }
                    else if (editor.condition == FireCondition.ActJudge.������)
                    {
                        dd.value = 3;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�ő���)
                    {
                        dd.value = 4;
                    }
                    else if (editor.condition == FireCondition.ActJudge.������)
                    {
                        dd.value = 5;
                    }
                    else if (editor.condition == FireCondition.ActJudge.������)
                    {
                        dd.value = 6;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�ő���)
                    {
                        dd.value = 7;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�ړ����x�ቺ�U��)
                    {
                        dd.value = 8;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�U���͒ቺ�U��)
                    {
                        dd.value = 9;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�h��͒ቺ�U��)
                    {
                        dd.value = 10;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�����w��Ȃ�)
                    {
                        dd.value = 11;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
                    {
                        dd.value = 12;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs)
                    {
                        dd.value = 13;
                    }
                    else if (editor.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
                    {
                        dd.value = 14;
                    }

                }
                else if(type == 1)
                {
                    if (editor.firstCondition == FireCondition.FirstCondition.�G�𐁂���΂�)
                    {
                        dd.value = 0;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.�ђʂ���)
                    {
                        dd.value = 1;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.�ݒu�U��)
                    {
                        dd.value = 2;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.�͈͍U��)
                    {
                        dd.value = 3;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.�ǔ�����)
                    {
                        dd.value = 4;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.�T�[�`�U��)
                    {
                        dd.value = 5;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.�w��Ȃ�)
                    {
                        dd.value = 6;
                    }
                }
                else if (type == 2)
                {
                    if (editor.nextCondition == FireCondition.AdditionalCondition.���ː�)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 0;
                        }
                        else
                        {
                            dd.value = 1;
                        }
                    }
                    else if (editor.nextCondition == FireCondition.AdditionalCondition.�r������)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 2;
                        }
                        else
                        {
                            dd.value = 3;
                        }
                    }
                    else if (editor.nextCondition == FireCondition.AdditionalCondition.�U����)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 4;
                        }
                        else
                        {
                            dd.value = 5;
                        }
                    }
                    else if (editor.nextCondition == FireCondition.AdditionalCondition.���l)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 6;
                        }
                        else
                        {
                            dd.value = 7;
                        }
                    }
                    else if (editor.nextCondition == FireCondition.AdditionalCondition.MP�g�p��)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 8;
                        }
                        else
                        {
                            dd.value = 9;
                        }
                    }
                    else if (editor.nextCondition == FireCondition.AdditionalCondition.�w��Ȃ�)
                    {
                        dd.value = 10;
                    }


                }
            }
            else if (s == 3)
            {
                SupportCondition editor = GetSupport(e);
                //����͍ŏ��̃h���b�v�_�E������
                Dropdown dd = objList[4].MMGetComponentNoAlloc<Dropdown>();
                if (type == 0)
                {
                    if (editor.sCondition == SupportCondition.SupportStatus.�G�^�C�v)
                    {
                        dd.value = 0;
                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.�v���C���[�̗̑͂��K��l�̎�)
                    {
                        dd.value = 1;
                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.�v���C���[��MP���K��l�ɒB������)
                    {
                        dd.value = 2;
                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.������MP���K��l�ɒB������)
                    {
                        dd.value = 3;
                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.�v���C���[����Ԉُ�ɂ���������)
                    {
                        dd.value = 4;
                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.���G�����邩�ǂ���)
                    {
                        if (editor.highOrLow)
                        {
                            dd.value = 5;
                        }
                        else
                        {
                            dd.value = 6;
                        }

                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.�C�ӂ̎x�����؂�Ă���Ƃ�)
                    {
                        dd.value = 7;
                    }

                    else if (editor.sCondition == SupportCondition.SupportStatus.�w��Ȃ�)
                    {
                        dd.value = 8;
                    }

                }
                else if (type == 1)
                {
                    if (editor.needSupport == SisMagic.SupportType.�U������)
                    {
                        dd.value = 0;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.�h�䋭��)
                    {
                        dd.value = 1;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.�G���`�����g)
                    {
                        dd.value = 2;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.�A�N�V��������)
                    {
                        dd.value = 3;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.�o���A)
                    {
                        dd.value = 4;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.���W�F�l)
                    {
                        dd.value = 5;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.�Ȃ�)
                    {
                        dd.value = 6;
                    }
                }

            }
            else if (s == 4)
            {
                SupportCondition editor = GetSupport(e);
                Dropdown dd = objList[2].MMGetComponentNoAlloc<Dropdown>();
                //������ƕ��G������
                if (type == 0)
                {
                    if (editor.ActBase == SupportCondition.MagicJudge.�e��x�����@)
                    {
                        dd.value = 0;
                    }
                    else if (editor.ActBase == SupportCondition.MagicJudge.�U���X�e�[�g��)
                    {
                        dd.value = 1;
                    }
                    else if (editor.ActBase == SupportCondition.MagicJudge.�񕜃X�e�[�g��)
                    {
                        dd.value = 2;
                    }
                    else if (editor.ActBase == SupportCondition.MagicJudge.�Ȃɂ����Ȃ�)
                    {
                        dd.value = 3;
                    }
                }
                else if (type == 1)
                {
                    if (editor.useSupport == SisMagic.SupportType.�U������)
                    {
                        dd.value = 0;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.�h�䋭��)
                    {
                        dd.value = 1;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.�G���`�����g)
                    {
                        dd.value = 2;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.�A�N�V��������)
                    {
                        dd.value = 3;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.�o���A)
                    {
                        dd.value = 4;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.���W�F�l)
                    {
                        dd.value = 5;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.�Ȃ�)
                    {
                        dd.value = 6;
                    }
                }
                else if (type == 2)
                {
                    if (editor.nextCondition == SupportCondition.AdditionalJudge.�r������)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 0;
                        }
                        else
                        {
                            dd.value = 1;
                        }
                    }
                    else if (editor.nextCondition == SupportCondition.AdditionalJudge.�������ʎ���)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 2;
                        }
                        else
                        {
                            dd.value = 3;
                        }
                    }
                    else if (editor.nextCondition == SupportCondition.AdditionalJudge.MP�g�p��)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 4;
                        }
                        else
                        {
                            dd.value = 5;
                        }
                    }
                    else if (editor.nextCondition == SupportCondition.AdditionalJudge.�����{��)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 6;
                        }
                        else
                        {
                            dd.value = 7;
                        }
                    }
                    else if (editor.nextCondition == SupportCondition.AdditionalJudge.�w��Ȃ�)
                    {
                        dd.value = 8;
                    }
                    
                }
            }
            else if (s == 5)
            {
                RecoverCondition editor = GetRecover(e);
                //����͍ŏ��̃h���b�v�_�E������
                Dropdown dd = objList[4].MMGetComponentNoAlloc<Dropdown>();
                if (type == 0)
                {
                    if (editor.condition == RecoverCondition.RecoverStatus.�G�^�C�v)
                    {
                        dd.value = 0;
                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.�v���C���[��HP���K��l�̎�)
                    {
                        dd.value = 1;
                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.�v���C���[��MP���K��l�ɒB������)
                    {
                        dd.value = 2;
                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.������MP���K��l�ɒB������)
                    {
                        dd.value = 3;
                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.�v���C���[����Ԉُ�ɂ���������)
                    {
                        dd.value = 4;
                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.���G�����邩�ǂ���)
                    {
                        if (editor.highOrLow)
                        {
                            dd.value = 5;
                        }
                        else
                        {
                            dd.value = 6;
                        }

                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.�C�ӂ̎x�����؂�Ă���Ƃ�)
                    {
                        dd.value = 7;
                    }

                    else if (editor.condition == RecoverCondition.RecoverStatus.�w��Ȃ�)
                    {
                        dd.value = 8;
                    }

                }
                else if (type == 1)
                {
                    if (editor.needSupport == SisMagic.SupportType.�U������)
                    {
                        dd.value = 0;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.�h�䋭��)
                    {
                        dd.value = 1;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.�G���`�����g)
                    {
                        dd.value = 2;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.�A�N�V��������)
                    {
                        dd.value = 3;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.�o���A)
                    {
                        dd.value = 4;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.���W�F�l)
                    {
                        dd.value = 5;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.�Ȃ�)
                    {
                        dd.value = 6;
                    }
                }
            }
            else
            {
                RecoverCondition editor = GetRecover(e);
                Dropdown dd = objList[3].MMGetComponentNoAlloc<Dropdown>();
                //������ƕ��G������
                if (type == 0)
                {
                    if (editor.ActBase == RecoverCondition.MagicJudge.�������@)
                    {
                        dd.value = 0;
                    }
                    else if (editor.ActBase == RecoverCondition.MagicJudge.�U���X�e�[�g��)
                    {
                        dd.value = 1;
                    }
                    else if (editor.ActBase == RecoverCondition.MagicJudge.�x���X�e�[�g��)
                    {
                        dd.value = 2;
                    }
                    else if (editor.ActBase == RecoverCondition.MagicJudge.�Ȃɂ����Ȃ�)
                    {
                        dd.value = 3;
                    }
                }
                else if (type == 1)
                {
                    if (editor.useSupport == SisMagic.SupportType.�U������)
                    {
                        dd.value = 0;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.�h�䋭��)
                    {
                        dd.value = 1;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.�G���`�����g)
                    {
                        dd.value = 2;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.�A�N�V��������)
                    {
                        dd.value = 3;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.�o���A)
                    {
                        dd.value = 4;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.���W�F�l)
                    {
                        dd.value = 5;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.�Ȃ�)
                    {
                        dd.value = 6;
                    }
                }
                else if (type == 2)
                {
                    if (editor.nextCondition == RecoverCondition.AdditionalJudge.�r������)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 0;
                        }
                        else
                        {
                            dd.value = 1;
                        }
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.�������ʎ���)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 2;
                        }
                        else
                        {
                            dd.value = 3;
                        }
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.MP�g�p��)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 4;
                        }
                        else
                        {
                            dd.value = 5;
                        }
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.�񕜗�)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 6;
                        }
                        else
                        {
                            dd.value = 7;
                        }
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.���W�F�l�񕜗�)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 8;
                        }
                        else
                        {
                            dd.value = 9;
                        }
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.��Ԉُ��)
                    {
                        dd.value = 10;
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.�w��Ȃ�)
                    {
                        dd.value = 11;
                    }
                }
            }
        }


        //�G�^�C�v
        else if (type == 3)
        {
            Toggle target;
            //���m1,��Ԃ��2,Shooter,Knight4,Trap8,��킸0

            int p = GetType(s, e);
            if (p != 0)
            {
             //   Debug.Log("wee");
                for (int i = 0; i <= objList.Count - 1; i++)
                {
                    target = objList[i].MMGetComponentNoAlloc<Toggle>();
                    if (i == 0)
                    {
                     //   Debug.Log("ssss����");
                        target.isOn = ((p & 0b00000001) == 0b00000001);
                    }
                    else if (i == 1)
                    {
                        target.isOn = ((p & 0b00000010) == 0b00000010);
                    }
                    else if (i == 2)
                    {
                        target.isOn = ((p & 0b00000100) == 0b00000100);
                    }
                    else if (i == 3)
                    {
                        target.isOn = ((p & 0b00001000) == 0b00001000);
                    }
                    else
                    {
                        target.isOn = ((p & 0b00010000) == 0b00010000);
                    }
                }
            }
            else
            {
                Debug.Log("aee");
                for (int i = 0; i <= objList.Count- 1; i++)
                {
                    target = objList[i].MMGetComponentNoAlloc<Toggle>();
                    target.isOn = false;
                }
            }
        }
        //�X���C�h�l�ݒ�
        else
        {
            int p = GetType(s, e);
            bool h = GetBool(s, e);
            objList[0].MMGetComponentNoAlloc<Slider>().value = p;
            objList[1].MMGetComponentNoAlloc<Toggle>().isOn = h;
        }

    }



    void ResetWindow()
    {
        for (int i = 0;i <= objList.Count - 1; i++)
        {
            if (objList[i].activeSelf == true)
                objList[i].SetActive(false);
        }

    }




    /// <summary>
    /// �Q�Ƃ��Ă�^�C�v���m�F
    /// </summary>
    /// <param name="s"></param>
    /// <param name="e"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    int GetType(int s,int e)
    {
        if(s == 1)
        {
            return MainUICon.instance.editParameter.targetCondition[e].percentage;
        }
        else if (s == 3)
        {
            return MainUICon.instance.editParameter.supportPlan[e].percentage;
        }
        else
        {
            return MainUICon.instance.editParameter.recoverCondition[e].percentage;
        }
    }

    /// <summary>
    /// �Q�Ƃ��Ă�^�C�v���m�F
    /// </summary>
    /// <param name="s"></param>
    /// <param name="e"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    bool GetBool(int s, int e)
    {
        if (s == 1)
        {
            return MainUICon.instance.editParameter.targetCondition[e].highOrLow;
        }
        else if (s == 3)
        {
            return MainUICon.instance.editParameter.supportPlan[e].highOrLow;
        }
        else
        {
            return MainUICon.instance.editParameter.recoverCondition[e].highOrLow;
        }
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
    RecoverCondition GetRecover( int e)
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
