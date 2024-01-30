using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Equip;

/// <summary>
/// �\���̂ɂ���
/// �\���̂Ȃ�l�����ł��邩��ǐ��オ��
/// </summary>
public struct AttackValueBase
{
    #region ��`

    /// <summary>
    /// �s�����Ƃɓ���ւ���K�v�̂���f�[�^
    /// �Ƃ�킯�_���[�W�v�Z�Ɏg�����̂̍\����
    /// ��Ԉُ���ǂ��ɂ����Ȃ��Ƃ�
    /// </summary>
    public struct ActionData
    {
        /// <summary>
        /// ���[�V�����l
        /// </summary>
        [Header("���[�V�����l")]
        public float mValue;

        /// <summary>
        /// �A�[�}�[���
        /// </summary>
        [Header("�A�[�}�[���")]
        public float shock;//


        /// <summary>
        /// �A�[�}�[
        /// </summary>
        [Header("�U�����̃A�[�}�[")]
        public float additionalArmor;


        /// <summary>
        /// ������΂���
        /// ���ꂪ0�ȏ�Ȃ琁����΂��U�����s��
        /// </summary>
        [Header("������΂���")]
        public Vector2 blowPower;


        /// <summary>
        /// �p���B�̃A�[�}�[���ɑ΂����R
        /// </summary>
        [Header("�p���B�̃A�[�}�[���ɑ΂����R")]
        public float _parryResist;

        /// <summary>
        /// �q�b�g�񐔐���
        /// </summary>
        [Header("�q�b�g�񐔐���")]
        public int _hitLimit;


        [Header("�U���̃��C������")]
        public MoreMountains.CorgiEngine.AtEffectCon.Element mainElement;


        /// <summary>
        /// �_���[�W�v�Z�Ŏg��
        /// </summary>
        [Header("�U���̑S����")]
        [EnumFlags]
        public MoreMountains.CorgiEngine.AtEffectCon.Element useElement;




        /// <summary>
        /// �p���B����邩�A�y�����d�����Ȃ�
        /// </summary>
        [Header("�U���̐���")]
        public AttackFeature feature;

    }


    /// <summary>
    /// �U���ړ��Ɋւ���f�[�^�̍\����
    /// ���@�ł����ݍ��݂��������肵����g��
    /// </summary>
    public struct AttackMoveData
    {
        /// <summary>
        /// �ړ����鎞��
        /// </summary>
        public float _moveDuration;

        /// <summary>
        /// �U���̈ړ�����
        /// ���b�N�I������ꍇ�͂��͈͓̔��œG�Ƃ̋���������
        /// </summary>
        public float _moveDistance;

        /// <summary>
        /// �U���ړ����ɓG�ƐڐG���̋���
        /// </summary>
        public MoreMountains.CorgiEngine.MyAttackMove.AttackContactType _contactType;



        /// <summary>
        /// �U�����Ɉړ��J�n����܂ł̎���
        /// </summary>
        public float startMoveTime;

        /// <summary>
        /// �G�����b�N�I�����Ĉړ�����U�����ǂ���
        /// ���b�N�I��������ړ��������L�т��肵�ĉ��Ƃ��Ăł��G�̑O�ɍs��
        /// </summary>
        public bool lockAttack;



        /// <summary>
        /// �w��������U�����ǂ���
        /// </summary>
        public bool backAttack;

    }

    /// <summary>
    /// �U���̐���
    /// </summary>
    public enum AttackFeature
    {
        light = 1 << 0,
        heavy = 1 << 1,
        disPariable = 1 << 2,
        selfRecover = 1 << 3,//��������������
        hitRecover = 1 << 4,//���������������񕜁B�񕜖��@
        superArmor = 1 << 5,//���[�V�������X�[�p�[�A�[�}�[
        guardAttack = 1 << 6,
        fallAttack = 1 << 7,
        positiveEffect = 1 << 8,
        badEffect = 1 << 9,
        nothing = 0
    }


    /// <summary>
    /// �A�N�V�����̋��x
    /// �G�t�F�N�g�̔��f�Ɏg��
    /// </summary>
    public enum AttackLevel
    {
        Weak,
        Normal,
        Strong,
        Fatal,
        Special,
        SEOnly
    }

    /// <summary>
    /// ���[�V�����̃^�C�v
    /// ���̔���Ɏg��
    /// </summary>
    public enum MotionType
    {
        slash,
        stab,
        strike,
        shoot
    }






    #endregion


    [Header("�A�N�V�����̏��")]
    public ActionData actionImfo;


    #region ���[�V�����Đ��֘A

    /// <summary>
    /// �ǂ�ȃG�t�F�N�g�≹�����炤��
    /// </summary>
    public AttackLevel EffectLevel;

    /// <summary>
    /// �ǂ�ȃ��[�V�����ł��邩
    /// </summary>
    public MotionType motionType;



    [Header("�R���{�U�����ǂ���")]
    public bool isCombo;




    #endregion



    #region�@�U���ړ��֘A

    /// <summary>
    /// �U���ړ��Ɋւ���f�[�^
    /// </summary>
    public AttackMoveData moveData;


    #endregion

}
