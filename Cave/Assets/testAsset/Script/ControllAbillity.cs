using MoreMountains.CorgiEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterStatus;
using static EnemyStatus;
using static Equip;

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


    /// <summary>
    /// �{����X�e�[�^�X�𑀍삷��Ƃ��Ɏg���񋓌^
    /// </summary>
    public enum StatusControlType
    {
        �X�e�[�^�X���Z,
        �X�e�[�^�X��Z,
        �{�����Z,
        �{����Z
    }






    #endregion

    //�ϐ�
    //���ꂢ��Ȃ��ˁH�@�������ڏ����ς��悤
    
    /// <summary>
    /// �h��֘A�̒l
    /// �h��v�Z�Ɏg��
    /// �����ӂ�ӂ�ς��Ƃ���Ƃ߂����ɕς��Ȃ��Ƃ��낪����
    /// �ς��Ȃ��̂̓X�e�[�g
    /// </summary>
    DefenseData myDefData;

    /// <summary>
    /// �U���͊֘A�̒l
    /// �߂����ɕς��Ȃ�
    /// </summary>
    AttackStatus myAtData;


    #region �L�����̐���֘A





    /// <summary>
    /// �L�����N�^�[�̏�Ԃ��}�l�[�W���[�ɃZ�b�g
    /// �����ƌĂё�����
    /// </summary>
    public abstract void TargetDataUpdate(int num);

    /// <summary>
    /// �L�����N�^�[�̏�Ԃ��}�l�[�W���[�ɒǉ�
    /// �ŏ�����
    /// </summary>
    public abstract void TargetDataAdd(int newID);


    /// <summary>
    /// �^�[�Q�b�g���X�g����폜���ꂽ�G�l�~�[����������
    /// �����ăw�C�g���X�g���𒲐�
    /// �v���C���[�͂Ȃ񂩕ʂ̏�������Ă�����������
    /// ���ƓG�̎���ʒm���郁�\�b�h�Ƃ��Ă��g����
    /// </summary>
    /// <param name="deletEnemy"></param>
    public abstract void TargetListChange(int deletEnemy,int deadID);


    /// <summary>
    /// ID���m�F
    /// ������g���Ė����ƘA�g����̂ŏd���m�F�厖
    /// </summary>
    /// <returns></returns>
    public abstract int ReturnID();

    /// <summary>
    /// �������_���^�[�Q�b�g�����肵����
    /// �w������C�x���g�������Ă����΂�
    /// �R���o�b�g�}�l�[�W���[��ʂ��Ē��ԂɒʒB���s��
    /// �ł��G���ł����������Ȃ�����
    /// </summary>
    public abstract void CommandEvent(TargetingEvent _event, int level, int targetNum, int commanderID);


    #endregion

    #region �_���[�W�v�Z�֘A


    /// 
    /// �ڎw������
    /// 
    /// �E�Ȃ�ׂ��L�����ƃw���X�̂��������Ȃ�
    /// �E�w���X���ɋ��ʐ��l�̑唼����������
    /// �E�A�[�}�[�֘A�̓L�������ł��B�V�X�^�[�����MP���A�[�}�[�������肷�邩��
    /// �E�X�e�[�^�X�ύX���ɃL�����N�^�[���w���X��_���[�W�@�\���X�V���銴��
    /// �E�h���Ԃ̃A�[�}�[�J�n�A�U�����Ȃǂ͍U���@�\�Ɏ������悤�B�G���͍U���A�r���e�B�A�v���C���[�͕���A�r���e�B
    /// �E�h���Ԃ̃X�^����Ԃ̓X�^���A�r���e�B���Ǘ��B�X�^���J�n�ƏI���Ō���Z
    /// �E�K�[�h���̓K�[�h�A�r���e�B�ƍU���A�r���e�B��
    /// �E�o�t�֘A�̓o�t�K�p���ɌĂԁi�o�t�Œ��ڃw���X�Ƃ��̃f�[�^������������悭�ˁH�j
    /// 
    /// �E���������Ɣ�e���ɉ����Ăяo�����肵�Ȃ��Ă悭�Ȃ�
    /// ///





    /// <summary>
    /// �e�ۂɃo�t�̐��l��^����
    /// �e�ۑ�����Ă�
    /// </summary>
    public abstract AttackData.AttackMultipler BulletBuffCalc();



    /// <summary>
    /// �_���[�W�@�\�̍U���o�t�i�f�o�t�j�̐��l���X�V
    /// �ύX���̂݌Ă�
    /// �{���ő����������ƑS�̔{���̕ύX
    /// ���o�[�X�̎��A�������݃o�t��f�o�t������Ȃ��Ȃ��Ă�Ȃ�S��1�A���邢�͏����l�ɖ߂��悤��
    /// </summary>
    /// <param name="type">����^�C�v</param>
    /// <param name="changeVlue">�ύX���l</param>
    /// <param name="isReverse">���ɖ߂��������ǂ���</param>
    public abstract void AttackStatusUpdate(StatusControlType type,AtEffectCon.Element changeElement,float changeVlue,bool isReverse);

    /// <summary>
    /// �_���[�W�@�\�̖h��o�t�i�f�o�t�j�A�X�e�[�^�X�̐��l���X�V
    /// �ύX���̂݌Ă�
    /// �{���ő����������ƑS�̔{���̕ύX
    /// ���o�[�X�̎��A�������݃o�t��f�o�t������Ȃ��Ȃ��Ă�Ȃ�S��1�A���邢�͏����l�ɖ߂��悤��
    /// </summary>
    /// <param name="type">����^�C�v</param>
    /// <param name="changeVlue">�ύX���l</param>
    /// <param name="isReverse">���ɖ߂��������ǂ���</param>
    public abstract void DefStatusUpdate(StatusControlType type, AtEffectCon.Element changeElement, float changeVlue);


    /// <summary>
    /// �p���B�������������̏���
    /// ���[�V�����J�n
    /// </summary>
    /// <param name="isBreake"></param>
    public abstract void ParryStart(bool isBreake);


    /// <summary>
    /// �K�[�h�̉��̏���
    /// �Q�ƌ����Ⴄ�̂ŏ������킯��
    /// </summary>
    public abstract void GuardSound();

    /// <summary>
    /// �X�^����w���X����A�[�}�[���񕜂�����̂Ɏg��
    /// </summary>
    public abstract void ArmorReset();

    /// <summary>
    /// �X�^�����ɃR���g���[���A�r���e�B��ʂ��ăX�^���J�n
    /// </summary>
    /// <param name="stunState"></param>
    public abstract void StartStun(MyWakeUp.StunnType stunState);

    /// <summary>
    /// �U���H������Ƃ�
    /// �w���X���瑗��ꂽ�A�[�}�[���ɉ����ăC�x���g�Ƃ΂�
    /// </summary>
    public abstract MyWakeUp.StunnType ArmorControll(float shock, bool isBlow, bool isBack);

    /// <summary>
    /// �X�^�����̉��Ƃ����o�����߂�
    /// ���݂̃X�^����������ʒm����@�\
    /// </summary>
    /// <returns></returns>
    public abstract int GetStunState();

    /// <summary>
    /// �������W���X�K���ꂽ���A�[�}�[�u���C�N���邩�ǂ�����`����
    /// �v���C���[�̓W���X�K�ꔭ�Ńp���B�ɂ���H
    /// </summary>
    /// <returns></returns>
    public abstract bool ParryArmorJudge();

    /// <summary>
    /// �󒆃_�E���Ɋւ��锻��
    /// </summary>
    /// <param name="stunnState"></param>
    /// <returns></returns>
    public abstract bool AirDownJudge(MyWakeUp.StunnType stunnState);


    /// <summary>
    /// �����̍U���X�e�[�^�X���_���[�W�@�\�ɓ`����
    /// </summary>
    public abstract void DamageCalc();



    /// <summary>
    /// �����̖h��X�e�[�^�X���_���[�W�@�\�ɓ`����
    /// </summary>
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
    /// �����炪�_���[�W�󂯂���ɌĂяo����鏈��
    /// �X�^�����Ă邩�A�_���[�W���ǂꂭ�炢�󂯂���
    /// �N�ɍU�����ꂽ���A�w�ォ��U�����󂯂����Ȃǂ��킩��
    /// </summary>
    public abstract void DamageEvent(bool isStunn,GameObject enemy,int damage,bool back);



    //���񂾂��f���Q�[�g��DamageOnTouch�ɂ���Ȃ�g���ׂ�
    //��������Ȃ��Ȃ�g����
    //����+=�ł�����ł��f���Q�[�g�ǉ��ł���
    //public OnHitDelegate OnHit;
    //public OnHitDelegate OnHitDamageable;
    //public OnHitDelegate OnHitNonDamageable;
    //public OnHitDelegate OnKill;

    /// <summary>
    /// �����̍U�����q�b�g�������ɂǂ̂悤�Ƀq�b�g���������܂߂ċ�����
    /// �����炪�U���𓖂Ă���
    /// </summary>
    /// <param name="isBack">���Ă����肪�w�������Ă���^/param>
    public abstract void HitReport(bool isBack);


    /// <summary>
    /// ���S����
    /// </summary>
    public abstract void Die();

}
