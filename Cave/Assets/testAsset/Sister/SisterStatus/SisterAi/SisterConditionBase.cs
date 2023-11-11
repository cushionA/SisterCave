using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable] //�����������inspector�ɕ\�������B
public class SisterConditionBase
{

    #region ��`


    /// <summary>
    /// �s�����N��������
    /// </summary>
    public enum ActCondition
    {
        �v���C���[��HP���䗦�̏����𖞂�����,//�����ŉ񕜂�����΋ً}�񕜂ɂȂ邵�AMP�����Ƃ��ɂ���ΐ؂�D���^�p�ł���
        �v���C���[��MP���䗦�̏����𖞂�����,
        ������HP���䗦�̏����𖞂�����,
        ������MP���䗦�̏����𖞂�����,
        �N����HP���䗦�̏����𖞂�����,
        �N����MP���䗦�̏����𖞂�����,
        �V�X�^�[����ƃv���C���[�������N����HP���䗦�̏����𖞂�����,
        �V�X�^�[����ƃv���C���[�������N����MP���䗦�̏����𖞂�����,
        �v���C���[�̓���̎x�����؂ꂽ��,
        �V�X�^�[����ƃv���C���[�������N���̓���̎x�����؂ꂽ��,
        �N���̓���̎x�����؂ꂽ��,
        �V�X�^�[����̓���̎x�����؂ꂽ��,
        �v���C���[������̏�Ԉُ�ɂ���������,//���ꖢ����
        �N��������̏�Ԉُ�ɂ���������,
        �V�X�^�[����ƃv���C���[�������N��������̏�Ԉُ�ɂ���������,


        �G��HP�䗦�̏����𖞂�����,
        �G���A�[�}�[�l�̔䗦�̏����𖞂�����,
        �G�^�C�v,
        ���G�̑���,
        ����̏�Ԉُ�ɂ������Ă�G�������,//��Ԉُ�̓X���[�ƍU���͖h��͒ቺ���t���邩�B���ꖢ����
        ����̎�_�̓G�������,
        �w��Ȃ�

    }

    /// <summary>
    /// ��������͈�
    /// �G�S�̂Ƃ������Ƃ�
    /// ���̕ϐ��ŏƉ�\�b�h�̑Ώەς��
    /// </summary>
    public enum CheckRange
    {
        Player,
        Sister,
        Ally,
        OtherAlly,
    }


    [HideInInspector]
    public enum Element
    {
        �a������,
        �h�ˑ���,
        �Ō�����,
        ������,
        �ő���,
        ������,
        ������,
        �ő���,
        �ړ����x�ቺ�U��,
        �U���͒ቺ�U��,
        �h��͒ቺ�U��,
        �w��Ȃ�
    }


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



    public enum UseAction
    {
        �U���s�����p��,
        �x���s���Ɉڍs,
        �񕜍s���Ɉڍs,
        �Ȃɂ����Ȃ�,
        ���݂̐ݒ�ōs��
    }




    /// <summary>
    /// �s�����N���������߂邽�߂̏���
    /// �S�ẴX�e�[�g�Ŏg��
    /// </summary>
    public class ActJudgeCondition
    {


        public ActCondition actCondition = ActCondition.�w��Ȃ�;


        //soldir���m1000000
        //Fly,//��Ԃ��0100000
        //Knight,//������0010000
        //Trap//�҂��\���Ă���00010000
        //UI�`�F�b�N�{�b�N�X����Đ����ς���

        [Header("�I������G�ƃp�[�Z���g�̐���")]
        [Tooltip("���m1,��Ԃ��2,Shooter4,Knight8,Trap,��킸0")]
        public int percentage;

        /// <summary>
        /// �����Actcondition�Ɋւ��
        /// </summary>
        [Tooltip("true�ŏ�AFalse�ŉ�")]
        public bool highOrLow;//���̍��ڂ��ȏォ�ȉ����B




        public AdditionalJudge nextCondition = AdditionalJudge.�w��Ȃ�;

        /// <summary>
        /// �����Additional�ł���
        /// </summary>
        public bool upDown;//���邢�͒Ⴂ������������
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




    /// <summary>
    /// ����͖��@���g�p������Ĕ�����s�킸�Ɏg�����߂̃L���b�V��
    /// ���̏����Ɩ��@�\���Ŕ��f�����͕̂ۑ����Ƃ�
    ///  �܌����������ɂ��Ă邩������̂͑�����AI�ݒ肵�Ȃ������Ƃ�����
    /// </summary>
    [HideInInspector] public SisMagic UseMagic;

    public bool AutoWait;//�����Ŏg�p���@�̎g�pMP���񕜂���N�[���^�C�������


}
