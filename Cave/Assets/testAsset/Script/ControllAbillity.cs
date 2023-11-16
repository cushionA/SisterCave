using MoreMountains.CorgiEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterStatus;
using static EnemyStatus;

/// <summary>
/// �w���X�ȂǂŎg���L�����N�^�[�R���g���[���ɕK�v�ȃ��\�b�h��ςݍ���
/// </summary>
public abstract class ControllAbillity : MyAbillityBase
{





    #region ��`



    /// <summary>
    /// �̗͂̊����Ƃ��ω�����L�����f�[�^
    /// </summary>
    public struct ConditionData
    {
        /// <summary>
        /// �G�̈ʒu
        /// </summary>
        public Vector2 targetPosition;

        /// <summary>
        /// �̗͂̊���
        /// </summary>
        public float hpRatio;

        /// <summary>
        /// �̗͂̐��l
        /// </summary>
        public float hpNum;

        /// <summary>
        /// MP�̊���
        /// </summary>
        public float mpRatio;

        /// <summary>
        /// ���Ă�o�t���L�^
        /// </summary>
        public PositiveCondition buffImfo;

        /// <summary>
        /// ���Ă���f�o�t���L�^
        /// </summary>
        public NegativeCondition debuffImfo;


        public TargettigData target;
    }

    /// <summary>
    /// �^�[�Q�b�g���Ă鑊�肪�N�Ȃ̂��������f�[�^
    /// </summary>
    public struct TargettigData
    {
        /// <summary>
        /// �W�I�̏���
        /// </summary>
        public Side targetSide;

        /// <summary>
        /// �W�I�̔ԍ�
        /// </summary>
        public int targetNum;
    }

    /// <summary>
    /// �ǂ̐w�c��\����
    /// </summary>
    public enum Side
    {
        Enemy = 1,
        Player = 2,
        Other = 3,
        �Ȃ� = 0//�G��F�����ĂȂ�
    }


    /// <summary>
    /// �������ʁA������o�t
    /// </summary>
    [Flags]

    public enum PositiveCondition
    {
        HP�p���� = 1,
        MP�p���� = 1 << 1,
        �h��͏㏸ = 1 << 2,
        �U���͏㏸ = 1 << 3,
        ��_���[�W�J�b�g = 1 << 4,
        �^�_���[�W���� = 1 <<5 ,
        �ړ����x�㏸ = 1 << 6,

        �X�^�~�i�񕜉��� = 1 << 7,
        �K�[�h���\�㏸ = 1 << 8,
        �A�[�}�[�㏸ = 1 << 9,
        �U��HP�� = 1 << 10,
        ���jHP�� = 1 << 11,
        �U��MP�� = 1 << 12,
        ���jMP�� = 1 << 13,
        ��ѓ���o���A = 1 <<14,
        ��Ԉُ�ϐ��㏸ = 1 << 15,//�łƂ��ʂɂ��邩��
    }


    /// <summary>
    /// �������ʁA������f�o�t
    /// </summary>
    [Flags]

    public enum NegativeCondition
    {
        �� = 1 << 0,//�����Ȃǂ̏�Ԉُ�Ƃ̋�ʂǂ�����H�@�~�ς����������̃f�[�^������Ƃ��H
        �Z�H = 1 << 1,//MP��
        ���� = 1 << 2,//�ړ���~
        ���� = 1<<3,//���p�֎~
        �h��͌��� = 1 << 4,
        �U���͌��� = 1 << 5,
        ��_���[�W���� = 1 << 6,
        �^�_���[�W���� = 1 << 7,
        �ړ����x�ቺ = 1 << 8,
        �X�^�~�i�񕜌��� = 1 << 9,
        �K�[�h���\�ቺ = 1 << 10,
        �A�[�}�[�ቺ = 1 << 11,
        �w�C�g�㏸ = 1 << 12,
        ��Ԉُ�ϐ����� = 1 << 13,//�łƂ��ʂɂ��邩��
    }


    #endregion

    #region �L�����̐���֘A





    /// <summary>
    /// �L�����N�^�[�̏�Ԃ��}�l�[�W���[�ɃZ�b�g
    /// ������
    /// </summary>
    public abstract void TargetDataSet(int num);

    /// <summary>
    /// �L�����N�^�[�̏�Ԃ��}�l�[�W���[�ɒǉ�
    /// �ŏ�����
    /// </summary>
    public abstract void TargetDataAdd();

    /// <summary>
    /// �^�[�Q�b�g���X�g����폜���ꂽ�G�l�~�[����������
    /// �����ăw�C�g���X�g���𒲐�
    /// �v���C���[�͂Ȃ񂩕ʂ̏�������Ă�����������
    /// ���ƓG�̎���ʒm���郁�\�b�h�Ƃ��Ă��g����
    /// </summary>
    /// <param name="deletEnemy"></param>
    public abstract void TargetListChange(int deletEnemy);


    /// <summary>
    /// ID���m�F
    /// ������g���Ė����ƘA�g����̂ŏd���m�F�厖
    /// </summary>
    /// <returns></returns>
    public abstract int ReturnID();


    /// <summary>
    /// �������^�[�Q�b�g�����肵����
    /// �C�x���g�������Ă����΂�
    /// </summary>
    public abstract void CommandEvent(TargetingEvent _event, int level, int targetNum, int commanderID);

    #endregion

    #region �_���[�W�v�Z�֘A

    public abstract void GuardReport();

    /// <summary>
    /// �o�t�̐��l��^����
    /// �e�ۂ���Ă�
    /// </summary>
    public abstract void BuffCalc(FireBullet _fire);

    public abstract void ParryStart(bool isBreake);

    public abstract void GuardSound();

    public abstract void ArmorReset();

    public abstract void StartStun(MyWakeUp.StunnType stunState);
    public abstract MyWakeUp.StunnType ArmorControll(float shock, bool isBlow, bool isBack);

    public abstract int GetStunState();

    /// <summary>
    /// �p���B�ł������`�F�b�N
    /// </summary>
    /// <returns></returns>
    public abstract bool ParryArmorJudge();

    /// <summary>
    /// �󒆃_�E���Ɋւ��锻��
    /// </summary>
    /// <param name="stunnState"></param>
    /// <returns></returns>
    public abstract bool AirDownJudge(MyWakeUp.StunnType stunnState);

    public abstract void DamageCalc();
    public abstract void DefCalc();

#endregion







    /// <summary>
    /// ���S���[�V�����̊J�n
    /// </summary>
    public abstract void DeadMotionStart(MyWakeUp.StunnType stunnState);




    //���񂾂��f���Q�[�g��Health�ɂ���Ȃ�g���ׂ�
    //��������Ȃ��Ȃ�g����
    //����+=�ł�����ł��f���Q�[�g�ǉ��ł���
    //����σ_���f���Q�[�g��Invoke�ŌĂяo�����тɃN���X�̃C���X�^���X������Ă�
    //���׏d�����Ȃ񂾂��

    //public OnDeathDelegate OnDeath;
    //public OnHitDelegate OnHit;
    //public OnHitZeroDelegate OnHitZero;
    //public OnReviveDelegate OnRevive;

    /// <summary>
    /// �_���[�W�󂯂���ɌĂяo����鏈��
    /// </summary>
    public abstract void DamageEvent(bool isStunn,GameObject enemy);



    //���񂾂��f���Q�[�g��DamageOnTouch�ɂ���Ȃ�g���ׂ�
    //��������Ȃ��Ȃ�g����
    //����+=�ł�����ł��f���Q�[�g�ǉ��ł���
    //public OnHitDelegate OnHit;
    //public OnHitDelegate OnHitDamageable;
    //public OnHitDelegate OnHitNonDamageable;
    //public OnHitDelegate OnKill;

    /// <summary>
    /// �U�����q�b�g�������ɂǂ̂悤�Ƀq�b�g���������܂߂ċ�����
    /// </summary>
    /// <param name="isBack">���Ă����肪�����̌��ɂ��鎞�͐^</param>
    public abstract void HitReport(bool isBack);


    /// <summary>
    /// ���S����
    /// </summary>
    public abstract void Die();

}
