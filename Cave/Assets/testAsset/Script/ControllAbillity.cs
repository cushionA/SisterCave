using MoreMountains.CorgiEngine;
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
        /// MP�̊���
        /// </summary>
        public float mpRatio;

        /// <summary>
        /// �o�t���Ă��邩
        /// </summary>
        public bool isBuffOn;

        /// <summary>
        /// �f�o�t���Ă��邩
        /// </summary>
        public bool isDebuffOn;
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
