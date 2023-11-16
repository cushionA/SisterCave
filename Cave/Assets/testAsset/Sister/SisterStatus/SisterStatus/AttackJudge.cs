using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Net.Sockets;
using MoreMountains.CorgiEngine;

[System.Serializable] //�����������inspector�ɕ\�������B
public class AttackJudge:SisterConditionBase
{

    #region ��`


    /// <summary>
    /// �U�����@�g�p�̋����ݒ�
    /// </summary>
    public enum AttackWaitCondition
    {
        ��������,
        �C�ӂ̕b��������҂��Ĕ���,
        �C�ӂ̕b��������҂��ăL�����Z��
    }

    /// <summary>
    /// �U�����@�g�p�ő҂���
    /// </summary>
    public enum WaitCondition
    {
        stun,//�^�[�Q�b�g�̃X�^����҂�
        attack,//�^�[�Q�b�g�ւ̃v���C���[�̍U����҂�
        range,//�˒������ɓ���̂�҂�
        hit,//�ː��ʂ�̂�҂�
        enemyAttack,//�G���U�����悤�Ƃ���̂�҂�
    }


    /// <summary>
    /// �U���O�ɑҋ@���邩�̏���
    /// </summary>
    public struct AttackWaitSetting
    {
        [Header("������҂��Ă���U�����邩")]
        public AttackWaitCondition waitCondition;


        [Header("�ҋ@�������")]
        public WaitCondition waitEvent;

        [Header("�҂b��")]
        public float waitTime;

        /// <summary>
        /// �w�C�g�������Ă����G��������^�[�Q�b�g�؂�ւ��Č����ǂ���
        /// </summary>
        [Header("�ҋ@���U�����Ă���G�ɕW�I��ς��邩")]
        public bool targetHateChange;

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






    //[HideInInspector] public int percentage;
    //   [HideInInspector] public bool highOrLow;//���̍��ڂ��ȏォ�ȉ����B
    //  public byte WeakPointJudge;


    /// <summary>
    /// �g�����@�̑���
    /// ���ꂪ�U���ɌŗL�̍s���I������
    /// </summary>
    public AtEffectCon.Element useElement;


    [Header("�U���O�̑ҋ@���������߂�")]
    public AttackWaitSetting waitSetting;


}
