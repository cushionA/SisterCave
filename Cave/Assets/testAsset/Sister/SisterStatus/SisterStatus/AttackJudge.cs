using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable] //�����������inspector�ɕ\�������B
public class AttackJudge
{

    [HideInInspector]
    public enum TargetJudge
    {
        �G�^�C�v,
        �v���C���[��HP���K��l�ɒB������,//�����ŉ񕜂�����΋ً}�񕜂ɂȂ邵�AMP�����Ƃ��ɂ���ΐ؂�D���^�p�ł���
                          //�v���C���[�̗̑͊֘A�͑O��target�����̂܂܎g���B�O�̂����ł���������Ȃ����̏����ő_��
        �v���C���[��MP���K��l�ɒB������,
        ������MP���K��l�ɒB������,
        �v���C���[����Ԉُ�ɂ���������,//���ꖢ����
        ���G�̑���,
        ��Ԉُ�ɂ������Ă�G,//��Ԉُ�̓X���[�ƍU���͖h��͒ቺ���t���邩�B���ꖢ����
        �������ĂȂ��x��������,
        �w��Ȃ�

    }

    public TargetJudge condition = TargetJudge.�w��Ȃ�;
   
      //soldir���m1000000
      //Fly,//��Ԃ��0100000
      //Knight,//������0010000
      //Trap//�҂��\���Ă���00010000
      //UI�`�F�b�N�{�b�N�X����Đ����ς���
      [Header("�I������G�ƃp�[�Z���g�̐���")]
      [Tooltip("���m1,��Ԃ��2,Shooter4,Knight8,Trap,��킸0")]
    public int percentage;
    [Tooltip("true�ŏ�AFalse�ŉ�")]
     public bool highOrLow;//���̍��ڂ��ȏォ�ȉ����B
    //public char SelectType;
    //  public byte WeakPointJudge;

    [HideInInspector]public enum WeakPoint
    {
        �a������,
        �h�ˑ���,
        �Ō�����,
        ������,
        �ő���,
        ������,
        ������,
        �ő���,
        �w��Ȃ�
    }

    [Tooltip("�_��������_�̎w��")]
    public WeakPoint wp = WeakPoint.�w��Ȃ�;

    [HideInInspector]
    public enum AdditionalJudge
    {
        //  �G�̎�_,//��Ԉُ�܂�
        �G��HP,
        �G�̋���,
        �G�̍��x,
        //  �G�̈ړ����x,
        �G�̍U����,
        �G�̖h���,
        �w��Ȃ�
    }

    public AdditionalJudge nextCondition = AdditionalJudge.�w��Ȃ�;

     public bool upDown;//���邢�͒Ⴂ������������



  /*  public byte SlashSet()
    {
        WeakPointJudge |= 0x80;
        return WeakPointJudge;
    }
    public byte StabSet()
    {
        WeakPointJudge |= 0x40;
        return WeakPointJudge;
    }
    public byte StrikeSet()
    {
        WeakPointJudge |= 0x20;
        return WeakPointJudge;
    }
    public byte HolySet()
    {
        WeakPointJudge |= 0x10;
        return WeakPointJudge;
    }
    public byte DarkSet()
    {
        WeakPointJudge |= 0x08;
        return WeakPointJudge;
    }
    public byte FireSet()
    {
        WeakPointJudge |= 0x04;
        return WeakPointJudge;
    }
    public byte ThunderSet()
    {
        WeakPointJudge |= 0x02;
        return WeakPointJudge;
    }
    public byte PoisonSet()
    {
        WeakPointJudge |= 0x01;
        return WeakPointJudge;
    }*/


}
