using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static Magic;

[System.Serializable] //�����������inspector�ɕ\�������B
public class SisterConditionBase
{

    #region ��`


    /// <summary>
    /// �s�����N��������
    /// ����͐ݒ�Ɏg�������ɂ���H
    /// ����̐ݒ�Ɋ�Â��Ē����X�e�[�^�X�����肷��
    /// 
    /// �����X�e�[�^�XcheckRange��checkContent������
    /// </summary>
    public enum ActCondition
    {
        �v���C���[��HP���䗦�̏����𖞂�����,//�����ŉ񕜂�����΋ً}�񕜂ɂȂ邵�AMP�����Ƃ��ɂ���ΐ؂�D���^�p�ł���
        �v���C���[��MP���䗦�̏����𖞂�����,
        ������HP���䗦�̏����𖞂�����,
        ������MP���䗦�̏����𖞂�����,
        ������HP���䗦�̏����𖞂�����,
        ������MP���䗦�̏����𖞂�����,
        �V�X�^�[����ƃv���C���[������������HP���䗦�̏����𖞂�����,
        �V�X�^�[����ƃv���C���[������������MP���䗦�̏����𖞂�����,
        �v���C���[�̓���̎x�����؂ꂽ��,
        �V�X�^�[����ƃv���C���[�����������̓���̎x�����؂ꂽ��,
        �����̓���̎x�����؂ꂽ��,
        �V�X�^�[����̓���̎x�����؂ꂽ��,
        �v���C���[������̏�Ԉُ�ɂ���������,//�����I�񂾎��A�p�[�Z���e�[�W�����ꂼ��̏�Ԉُ�Ɠ����r�b�g�ɂȂ�悤�ɑ��삷��
        ����������̏�Ԉُ�ɂ���������,
        �V�X�^�[����ƃv���C���[����������������̏�Ԉُ�ɂ���������,
        �w�苗���Ƀv���C���[�����鎞,
        �w�苗���ɖ��������鎞,


        �G��HP�䗦�̏����𖞂�����,
        �G���A�[�}�[�l�̔䗦�̏����𖞂�����,
        �G�^�C�v,
        ���G�̑���,
        ����̏�Ԉُ�ɂ������Ă�G�����鎞,//��Ԉُ�̓X���[�ƍU���͖h��͒ቺ���t���邩�B���ꖢ����
        ����̍U�����������G�����鎞,
        ����̎�_�̓G�����鎞,
        �A�g���̓G�����鎞,
        �w�苗���ɓG�����鎞,
        �v���C���[��_���Ă�G,
        �V�X�^�[�����_���Ă�G,
        �V�X�^�[����ƃv���C���[�ȊO��_���G,
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
        Enemy
    }

    /// <summary>
    /// �Ȃɂɂ��Ē��ׂ�̂�
    /// �̗͂Ƃ�
    /// </summary>
    public enum CheckContent
    {
        HP,
        MP,
        type,
        strength,//�������ǂ���
        posiCondition,//�o�t
        negaCondition,//��Ԉُ�
        weakPoint,
        distance,
        armor,
        playerHate,//�N��_���Ă邩
        sisterHate,
        otherHate
    }








    /// <summary>
    /// �^�[�Q�b�g������ɍi�荞�ނ��߂̏���
    /// ��r�\�Ȃ��̂�������g��
    /// </summary>
    [HideInInspector]
    public enum AdditionalJudge
    {
        �^�[�Q�b�g��HP����,
        �^�[�Q�b�g��HP���l,
        �^�[�Q�b�g��MP����,//�G��MP�̈����ǂ������B�U�����@�̎���UI�ɕ\�����Ȃ���΂���
        �^�[�Q�b�g�̋���,
        �^�[�Q�b�g�̍��x,
        �^�[�Q�b�g�̍U����,
        �^�[�Q�b�g�̖h���,
        �^�[�Q�b�g�̃A�[�}�[�l,
        �^�[�Q�b�g�̃o�t��,
        �^�[�Q�b�g�̃f�o�t��,

        �w��Ȃ�
    }




    /// <summary>
    /// �����̔��f���@
    /// �A���h��
    /// </summary>
    public enum JudgeRuLe
    {
        and����,
        or����,
        //   ����and����,//���̌���and�����ƌ��т��Ĉ�̏����ɂȂ�
        //   ����or����
    }



    /// <summary>
    /// �s�����N���������߂邽�߂̏���
    /// �S�ẴX�e�[�g�Ŏg��
    /// </summary>
    public struct ActJudgeCondition
    {


        public ActCondition actCondition;

        /// <summary>
        /// �����Ώۂ͈̔�
        /// </summary>
        [HideInInspector]
        public CheckRange range;

        /// <summary>
        /// �����Ώۂ͈̔�
        /// </summary>
        [HideInInspector]
        public CheckContent content;


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


        /// <summary>
        /// ���̏�����and��or���𔻒f����
        /// �A���h�͐�΂ɖ������ĂȂ��ƃ_��
        /// or�͑��𖞂����Ă�΂����B�S���������ĂȂ��Ƃ��͗��΂�?
        /// </summary>
        [Header("�����Ԃ̌�������")]
        public JudgeRuLe rule;

    }

    /// <summary>
    /// �^�[�Q�b�g��₩��^�[�Q�b�g�����߂邽�߂̏���
    /// </summary>
    public struct TargetSelectCondition
    {
        /// <summary>
        /// ����Ń^�[�Q�b�g�̗D��x�����߂�
        /// </summary>
        [Header("��ڂ̃^�[�Q�b�g�i�荞�ݏ���")]
        public AdditionalJudge SecondCondition;

        [Header("�������~����")]
        /// <summary>
        /// �^�Ȃ珸��
        /// </summary>
        public bool targetASOrder;


        //���������Ԃ������̂��߂̑�O��������邩�H

        /// <summary>
        /// �������ŕ��Ԃ���������̂��߂̏���
        /// </summary>
        [Header("�Ăт̃^�[�Q�b�g�i�荞�ݏ���")]
        public AdditionalJudge spareCondition;

        [Header("�������~����")]
        /// <summary>
        /// �^�Ȃ珸��
        /// </summary>
        public bool spareASOrder;
    }



    #region �������疂�@�������



    public enum UseAction
    {
        �U���s�����p��,
        �x���s���Ɉڍs,
        �񕜍s���Ɉڍs,
        �Ȃɂ����Ȃ�,
        ���݂̐ݒ�ōs��
    }


    /// <summary>
    /// ���ʂ��������
    /// ����͈����
    /// UI�ł͍�������H
    /// ���Ƃ��Γ����u���ʂ̑傫���v�ł�
    /// �x���ł͔{���ɂ�����U���ł͍U���͂ɂ���H�\����ς���
    /// </summary>
    public enum MagicSortCondition
    {
        ���@���x��,
        ���ː�,
        �r������,
        ���ʎ���,
        �ǔ����\,
        ���ʂ̑傫��,
        ���l,
        MP�g�p��,
        ���W�F�l�񕜑��x,//���W�F�l�Ƃ���UI�ŕ\�����Ȃ���΂����B
        ���W�F�l���񕜗�,//�Ƃ肠�����܂�ǂ��ΑS�ẴX�e�[�g�Ŏg����
        �e��,
        �e�ۂ̑傫��,//�����n�ł���Ȃ甚�����a�̑傫���ł�����
        �w��Ȃ�
    }





    #endregion




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

    #region �^�[�Q�b�g����


    [Header("�s�����N���������f�������")]
    public ActJudgeCondition[] judgeCondition = new ActJudgeCondition[3];


    [Header("�i�荞�񂾓G����^�[�Q�b�g��I�ԏ���")]
    public TargetSelectCondition selectCondition;



    #endregion


    #region ���@�I��


    /// <summary>
    /// �I������s��
    /// </summary>
    [Header("�ǂ������s�������邩")]
    public UseAction selectAction = UseAction.�Ȃɂ����Ȃ�;


    /// <summary>
    /// �g�p����s���̎˒��������`�F�b�N���邩
    /// �g�p���鋗���͌��肵���^�[�Q�b�g
    /// �ݒu�n�͎˒������͖���
    /// </summary>
    [Header("�˒������`�F�b�N")]
    public bool rangeCheck;

    /// <summary>
    /// �g�p����s����MP���`�F�b�N���邩
    /// ���ꂠ��ƈ��MP��������p�Ƃ��ł��������o�Ă��邩�疈��g�����p�ς���ăL���b�V���Ӗ��Ȃ��Ȃ��ˁH
    /// </summary>
    [Header("�˒������`�F�b�N")]
    public bool mpCheck;




    /// <summary>
    /// �g�p����e�ۂ̏���
    /// �񕜖��@�ł��ݒu������~�蒍�����肷�邩��
    /// �����ł��I�ׂ�
    /// �g�p���@������ōi���
    /// �r�b�g�������邩
    /// </summary>
    [Header("�g�p���閂�@�̒e�ۓ���")]
    [EnumFlags]
    public BulletType bulletCondition = BulletType.�w��Ȃ�;

  
    /// �e�ۂ̐��������Ă͂܂���̂�I��
    /// �U�Ȃ瓖�Ă͂܂�Ȃ����̂�I��
    /// ����͂���Ȃ��A���Ă͂܂�Ȃ��̑I�т����Ȃ��ȊO�S���I�ԂƂ�����΂���
 //   public bool matchCheck = true;



    /// <summary>
    /// �g�p����e�ۂ̏���
    /// �񕜖��@�ł��ݒu������~�蒍�����肷�邩��
    /// �����ł��I�ׂ�
    /// �g�p���@������ōi���
    /// </summary>
    [Header("�e�ۂ̎g�p�D�挈��̏���")]
    public MagicSortCondition magicSort = MagicSortCondition.�w��Ȃ�;


    [Header("�������~����")]
    /// <summary>
    /// �^�Ȃ珸��
    /// </summary>
    public bool bulletASOder;


    #endregion

    //��������͊e���@�̓Ǝ����


    /// <summary>
    /// ����͖��@���g�p������Ĕ�����s�킸�Ɏg�����߂̃L���b�V��
    /// ���̏����Ɩ��@�\���Ŕ��f�����͕̂ۑ����Ƃ�
    ///  �܌����������ɂ��Ă邩������̂͑�����AI�ݒ肵�Ȃ������Ƃ�����
    /// </summary>
    [HideInInspector] public SisMagic UseMagic;

    public bool AutoWait;//�����Ŏg�p���@�̎g�pMP���񕜂���N�[���^�C�������

    /// <summary>
    /// ���̍s���̃N�[���^�C��
    /// �N�[���^�C���[���ŉ������Ȃ��Ȃ画�f�͂��Ȃ�
    /// </summary>
    [Header("�N�[���^�C��")]
    public float coolTime;


    //����Ȃ��������Ԃ�
    [Header("�s���ケ�̏����Ŕ��f���J��Ԃ���")]
    public int reJudgeCount = 0;
}
