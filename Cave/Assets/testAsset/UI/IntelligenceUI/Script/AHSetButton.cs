using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AHSetButton : MonoBehaviour
{

    [SerializeField]
    bool isAct;

    [SerializeField]
    int editNum;

    [SerializeField]
    Text myTex;

    [SerializeField]
    SettingWindowCon pare;



 

    public void ValueApply()
    {
        RecoverCondition e;
        if(editNum == 1)
        {
            e = MainUI.instance.editParameter.nFirstRecover;
        }
        else if(editNum == 2)
        {
            e = MainUI.instance.editParameter.nSecondRecover;
        }
        else
        {
            e = MainUI.instance.editParameter.nThirdRecover;
        }
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
            MainUI.instance.settingNumber = 6;
        }
        else
        {
            MainUI.instance.settingNumber = 5;
        }
        pare.nextWindow.SetActive(true);

    }
}
