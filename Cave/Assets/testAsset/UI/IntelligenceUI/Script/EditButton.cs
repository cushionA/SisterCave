using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ݒ葋�B��������ւ��ƃZ�b�e�B���O�i���o�[�ł����ύX
/// �G�f�B�b�g�i���o�[�����
/// </summary>
public class EditButton : MonoBehaviour
{
    /// <summary>
    /// ���Ԗڂ̔����ǂ���
    /// </summary>
    public int editNum;

    /// <summary>
    /// ����^�Ȃ�P�����A����
    /// </summary>
    [SerializeField]
    bool isAct;

    [SerializeReference]
    Text myTex;

    Selectable my;


    [SerializeReference]
    SettingWindowCon pare;

    // Start is called before the first frame update
    void Start()
    {
        my = GetComponent<Selectable>();
    }

    // Update is called once per frame
    void Update()
    {
      //  Debug.Log($"������΁[{MainUI.instance.edi}");
    }




    /// <summary>
    /// �{�^���̕����ύX
    /// �ǂ��Ŏg�����A����
    /// </summary>
    public void ChangeString()
    {
      
        int s = MainUI.instance.settingNumber;



        if (s == 1)
        {
            if (!isAct)
            {
                AttackJudge j = GetTarget();
                if (j.condition == AttackJudge.TargetJudge.�v���C���[��HP���K��l�ɒB������)
                {
                    myTex.text = "�v���C���[��HP���w�肵���l�̎�";
                }
                else if (j.condition == AttackJudge.TargetJudge.�v���C���[��MP���K��l�ɒB������)
                {
                    myTex.text = "�v���C���[��MP���w�肵���l�̎�";
                }
                else if (j.condition == AttackJudge.TargetJudge.������MP���K��l�ɒB������)
                {
                    myTex.text = "�V�X�^�[�����MP���w�肵���l�̎�";
                }
                else if (j.condition == AttackJudge.TargetJudge.��Ԉُ�ɂ������Ă�G)
                {
                    myTex.text = "��Ԉُ�̓G�ɔ���";
                }
                else if (j.condition == AttackJudge.TargetJudge.�G�^�C�v)
                {
                    myTex.text = "�w�肵���^�C�v�̓G�ɔ���";
                }
                else if (j.condition == AttackJudge.TargetJudge.�v���C���[����Ԉُ�ɂ���������)
                {
                    myTex.text = "�v���C���[����Ԉُ���󂯂���";
                }
                else if (j.condition == AttackJudge.TargetJudge.�������ĂȂ��x��������)
                {
                    myTex.text = "���ʐ؂�̎x�����@�����鎞";
                }
                else if (j.condition == AttackJudge.TargetJudge.���G�̑���)
                {
                    if (!j.highOrLow)
                    {
                        //���Ȃ��ق�
                        myTex.text = "���G�ȊO�ɔ���";
                    }
                    else
                    {
                        
                        myTex.text = "���G�ɔ���";
                    }
                }
                else
                {
                    myTex.text = "�w��Ȃ�";
                }
            }
            else
            {
                FireCondition a = GetAttack();
                if(a.condition == FireCondition.ActJudge.�a������)
                {
                    myTex.text = "�a�������U��";
                }
                else if (a.condition == FireCondition.ActJudge.�Ō�����)
                {
                    myTex.text = "�Ō������U��";
                }
                else if (a.condition == FireCondition.ActJudge.�h�ˑ���)
                {
                    myTex.text = "�h�ˑ����U��";
                }
                else if (a.condition == FireCondition.ActJudge.������)
                {
                    myTex.text = "�������U��";
                }
                else if (a.condition == FireCondition.ActJudge.�ő���)
                {
                    myTex.text = "�ő����U��";
                }
                else if (a.condition == FireCondition.ActJudge.������)
                {
                    myTex.text = "�������U��";
                }
                else if (a.condition == FireCondition.ActJudge.������)
                {
                    myTex.text = "�������U��";
                }
                else if (a.condition == FireCondition.ActJudge.�ő���)
                {
                    myTex.text = "�ő����U��";
                }
                else if (a.condition == FireCondition.ActJudge.�U���͒ቺ�U��)
                {
                    myTex.text = "�U���͒ቺ�U��";
                }
                else if (a.condition == FireCondition.ActJudge.�ړ����x�ቺ�U��)
                {
                    myTex.text = "�ړ����x�ቺ�U��";
                }
                else if (a.condition == FireCondition.ActJudge.�h��͒ቺ�U��)
                {
                    myTex.text = "�h��͒ቺ�U��";
                }
                else if (a.condition == FireCondition.ActJudge.�����w��Ȃ�)
                {
                    myTex.text = "�����w��Ȃ��ōU��";
                }
                else if (a.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
                {
                    myTex.text = "�x���s���Ɉڍs";
                }
                else if (a.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs)
                {
                    myTex.text = "�񕜍s���Ɉڍs";
                }
                else
                {
                    myTex.text = "�Ȃɂ����Ȃ�";
                }
            }
        }
        else if (s == 3)
        {
            SupportCondition e = GetSupport();
            if (!isAct)
            {
                if(e.sCondition == SupportCondition.SupportStatus.�v���C���[����Ԉُ�ɂ���������)
                {
                    myTex.text = "�v���C���[����Ԉُ���󂯂���";
                }
                else if (e.sCondition == SupportCondition.SupportStatus.�v���C���[��MP���K��l�ɒB������)
                {
                    myTex.text = "�v���C���[��MP���w�肵���l�̎�";
                }
                else if (e.sCondition == SupportCondition.SupportStatus.�v���C���[�̗̑͂��K��l�̎�)
                {
                    myTex.text = "�v���C���[��HP���w�肵���l�̎�";
                }
                else if (e.sCondition == SupportCondition.SupportStatus.�C�ӂ̎x�����؂�Ă���Ƃ�)
                {
                    myTex.text = "�w�肵���x���̌��ʂ��؂ꂽ��";
                }
                else if (e.sCondition == SupportCondition.SupportStatus.���G�����邩�ǂ���)
                {
                    if (!e.highOrLow)
                    {
                        myTex.text = "���G�����Ȃ���";
                    }
                    else
                    {
                        myTex.text = "���G�����鎞";
                    }
                    
                }
                else if (e.sCondition == SupportCondition.SupportStatus.�w��Ȃ�)
                {
                    myTex.text = "�w��Ȃ�";
                }
                else if (e.sCondition == SupportCondition.SupportStatus.�G�^�C�v)
                {
                    myTex.text = "�w�肵���^�C�v�̓G�����鎞";
                }
                else
                {
                    myTex.text = "�V�X�^�[�����MP���w�肵���l�̎�";
                }
            }
            else
            {
                if(e.ActBase == SupportCondition.MagicJudge.�e��x�����@)
                {
                    myTex.text = "�x�����@���g�p"; 
                }
                else if (e.ActBase == SupportCondition.MagicJudge.�񕜃X�e�[�g��)
                {
                    myTex.text = "�񕜍s���Ɉڍs";
                }
                else if (e.ActBase == SupportCondition.MagicJudge.�U���X�e�[�g��)
                {
                    myTex.text = "�U���s���Ɉڍs";
                }
                else
                {
                    myTex.text = "�Ȃɂ����Ȃ�";
                }
            }
        }
        else if (s == 5)
        {
            RecoverCondition e = GetRecover();
            if (!isAct)
            {
                if (e.condition == RecoverCondition.RecoverStatus.�v���C���[����Ԉُ�ɂ���������)
                {
                    myTex.text = "�v���C���[����Ԉُ���󂯂���";
                }
                else if (e.condition == RecoverCondition.RecoverStatus.�v���C���[��MP���K��l�ɒB������)
                {
                    myTex.text = "�v���C���[��MP���w�肵���l�̎�";
                }
                else if (e.condition == RecoverCondition.RecoverStatus.�v���C���[��HP���K��l�̎�)
                {
                    myTex.text = "�v���C���[��HP���w�肵���l�̎�";
                }
                else if (e.condition == RecoverCondition.RecoverStatus.�C�ӂ̎x�����؂�Ă���Ƃ�)
                {
                    myTex.text = "�w�肵���x���̌��ʂ��؂ꂽ��";
                }
                else if (e.condition == RecoverCondition.RecoverStatus.���G�����邩�ǂ���)
                {
                    if (!e.highOrLow)
                    {
                        myTex.text = "���G�����Ȃ���";
                    }
                    else
                    {
                        myTex.text = "���G�����鎞";
                    }

                }
                else if (e.condition == RecoverCondition.RecoverStatus.�w��Ȃ�)
                {
                    myTex.text = "�w��Ȃ�";
                }
                else if (e.condition == RecoverCondition.RecoverStatus.�G�^�C�v)
                {
                    myTex.text = "�w�肵���^�C�v�̓G�����鎞";
                }
                else
                {
                    myTex.text = "�V�X�^�[�����MP���w�肵���l�̎�";
                }
            }
            else
            {
                if (e.ActBase == RecoverCondition.MagicJudge.�������@)
                {
                    myTex.text = "�񕜖��@���g�p";
                }
                else if (e.ActBase == RecoverCondition.MagicJudge.�x���X�e�[�g��)
                {
                    myTex.text = "�x���s���Ɉڍs";
                }
                else if (e.ActBase == RecoverCondition.MagicJudge.�U���X�e�[�g��)
                {
                    myTex.text = "�U���s���Ɉڍs";
                }
                else
                {
                    myTex.text = "�Ȃɂ����Ȃ�";
                }
            }
        }
    }


    /// <summary>
    /// �{�^���������Ƃ�
    /// </summary>
    public void SelectButton()
    {
        MainUI.instance.editNumber = editNum;
        //�O�̑��𑀍�s�\��
        pare.enable = true;
        if (isAct)
        {
            MainUI.instance.settingNumber++;
        }
        pare.nextWindow.SetActive(true);

    }

    AttackJudge GetTarget()
    {
        if (editNum== 1)
        {
            return MainUI.instance.editParameter.firstTarget;
        }
        else if (editNum== 2)
        {
            return MainUI.instance.editParameter.secondTarget;
        }
        else if (editNum== 3)
        {
            return MainUI.instance.editParameter.thirdTarget;
        }
        else if (editNum== 4)
        {
            return MainUI.instance.editParameter.forthTarget;
        }
        else
        {
            return MainUI.instance.editParameter.fiveTarget;
        }
    }

    FireCondition GetAttack()
    {
        if (editNum== 1)
        {
            return MainUI.instance.editParameter.firstAttack;
        }
        else if (editNum== 2)
        {
            return MainUI.instance.editParameter.secondAttack;
        }
        else if (editNum== 3)
        {
            return MainUI.instance.editParameter.thirdAttack;
        }
        else if (editNum== 4)
        {
            return MainUI.instance.editParameter.fourthAttack;
        }
        else if (editNum== 5)
        {
            return MainUI.instance.editParameter.fiveAttack;
        }
        else
        {
            return MainUI.instance.editParameter.nonAttack;
        }
    }

    SupportCondition GetSupport()
    {
        if (editNum== 1)
        {
            return MainUI.instance.editParameter.firstPlan;
        }
        else if (editNum== 2)
        {
            return MainUI.instance.editParameter.secondPlan;
        }
        else if (editNum== 3)
        {
            return MainUI.instance.editParameter.thirdPlan;
        }
        else if (editNum== 4)
        {
            return MainUI.instance.editParameter.forthPlan;
        }
        else if (editNum== 5)
        {
            return MainUI.instance.editParameter.fivePlan;
        }
        else
        {
            return MainUI.instance.editParameter.sixPlan;
        }
    }
    RecoverCondition GetRecover()
    {
        if (editNum== 1)
        {
            return MainUI.instance.editParameter.firstRecover;
        }
        else if (editNum== 2)
        {
            return MainUI.instance.editParameter.secondRecover;
        }
        else if (editNum== 3)
        {
            return MainUI.instance.editParameter.thirdRecover;
        }
        else if (editNum== 4)
        {
            return MainUI.instance.editParameter.forthRecover;
        }
        else if (editNum== 5)
        {
            return MainUI.instance.editParameter.fiveRecover;
        }
        else
        {
            return MainUI.instance.editParameter.nonRecover;
        }
    }


}
