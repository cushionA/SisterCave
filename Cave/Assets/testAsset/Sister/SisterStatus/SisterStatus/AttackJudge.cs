using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable] //�����������inspector�ɕ\�������B
public class AttackJudge:SisterConditionBase
{

    #region ��`


    public enum FirstCondition
    {
        �G�𐁂���΂�,
        �ђʂ���,
        �ݒu�U��,
        �͈͍U��,
        �ǔ�����,
        �T�[�`�U��,
        �w��Ȃ�

    }



    public enum AdditionalCondition
    {
        //  �G�̎�_,//��Ԉُ�܂�
        ���ː�,
        �r������,
        �U����,
        ���l,
        MP�g�p��,
        �w��Ȃ�
    }


    #endregion





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





    //��������̓^�[�Q�b�g�ݒ��̋���---------------------------------------------------


    public UseAction condition = UseAction.�Ȃɂ����Ȃ�;

    //[HideInInspector] public int percentage;
    //   [HideInInspector] public bool highOrLow;//���̍��ڂ��ȏォ�ȉ����B
    //  public byte WeakPointJudge;


    Element useElement;


    public FirstCondition firstCondition = FirstCondition.�w��Ȃ�;

    /// <summary>
    /// �^�Ȃ�܂܂Ȃ��̂�I��
    /// �������Ȃ��Ƃ�
    /// </summary>
    public bool notContain;

    public AdditionalCondition secondCondition = AdditionalCondition.�w��Ȃ�;

    [Tooltip("�Ⴂ�������������B����ق����łȂ������BFalse�łȂ��ق�")]
    public bool secondUpDown;//���邢�͒Ⴂ�������������BFalse�łȂ��ق�



}
