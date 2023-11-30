using Cysharp.Threading.Tasks;
using MoreMountains.Tools;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static CombatManager;
using static SisterMoveSetting;
using static SisterParameter;


namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// ���ʂɏ󋵔��f���ē������߂̃X�N���v�g
	/// ���[�v�A�U���A�R���r�l�[�V�����i�e����͕�����H�j�͕ʃX�N���v�g��
	/// ���邢�̓R���r�l�[�V�����͎�l���Ɏ�������H
	/// �ړ��@�\�̉��C
	/// �E�ړ��ƕ����]���͋��ʂ̃R���g���[���[�ɏW�߂�
	/// �E�e���\�b�h�ł͏�ԕω��ƈړ����������w��
    /// 
    /// 
    /// �@�\�Ē�`
    /// 
    /// �E�ړ��X�e�[�^�X�ɉ����Ĉړ����f������
    /// �E�Z���T�[�œG���x���͂���H
    /// �E�ړ��X�e�[�^�X�̏����ɉ����ă��\�b�h���Ăяo��
    /// �E���f������I�v�V�����ɉ����Ĉړ��n�_�����߂�
    /// �E���[�v�w��ŁA����Ƀ��[�v�ł���Ȃ烏�[�v����B���[�v��������������Œ�~
    /// �E�������f�͍U�������낤�����\���Ȃ��s���B�����Đ^�ɂȂ�����ړ�����
    /// �E���ړ���������m���ɍU������B�U���ȂǂōU�����f�����炳��Ɉړ�
    /// �E�ǂɓ����������̍s����
    /// 
    /// �E�U�����\�b�h�ł͍U���O�ɍ��ړ����������̃X�N���v�g�ɖ₢���킹�Ď~�܂�悤�ɂ���
    /// �E�U�����͉e�����Ȃ�
    /// 
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/EditBrain")]
    public class EditBrain : NPCControllerAbillity
    {

        #region ��`


        /// <summary>
        /// ���݂̏��
        /// ����ɂ���ē������ς��
        /// </summary>
        public enum SisterState
        {
            �̂�т�,
            �x��,
            �킢,
            �ŏ�
        }

        /// <summary>
        /// ���݂̈ړ�����
        /// �̂�т��x���A���������Ȃ�
        /// �󋵂ɉ��������f�ňړ����[�h�ƈړ��������������߂�
        /// �퓬���͑���ƒ�~����
        /// �����ċ��ʂ̈ړ����\�b�h�𗘗p���Ĉړ�����
        /// </summary>
        public enum MoveState
        {
            ��~,
            ����,
            ����,
            �ŏ�//�ŏ��̎��͈ړ��������f�ɍs��
        }





        #endregion

        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "TODO_HELPBOX_TEXT."; }



        [Header("�x���X�e�[�g�̎���")]
        public float patrolTime;


        public PlyerController pc;



        #region �����Ǘ����

        [Header("�V�X�^�[����̃X�e�[�^�X")]
        public SisterStatus status;

        [Header("�ړ��p�����[�^")]
        [SerializeField]
        SisterParameter parameter;


        /// <summary>
        /// ���݂̃��[�h�ňړ��Ɏg�����
        /// </summary>
        StateMoveStatus moveStatus;


        /// <summary>
        /// ���ړ��̊�ɂ��Ă鑊��
        /// </summary>
        TargetData nowTarget;

        //[HideInInspector]
        public SisterState nowState = SisterState.�̂�т�;
        
        [HideInInspector] public MoveState nowMove = MoveState.�ŏ�;


        [Header("���݂ǂ̃^�C�v�̖��@���g�����Ƃ��Ă�̂�")]
        [HideInInspector] public MoveType nowMode;

        #endregion


        // === �L���b�V�� ======================================
        // === �A�r���e�B ==========================================

        #region



        //���ړ��͌p�����Ƃɂ���

        public PlayerJump _jump;

        public PlayerRunning _characterRun;


        public MyWakeUp _wakeup;


        public MyDamageOntouch _damage;

        public WarpAbility _warp;

        public FireAbility _fire;


        public PlayerCrouch _squat;


        //�V�X�^�[����̌ŗL�A�N�V����

        public SensorAbility _sensor;


        // �A�j���[�V�����p�����[�^�[
        protected const string _stateParameterName = "_nowPlay";
        protected int _stateAnimationParameter;

        #endregion





        // === �����p�����[�^ ======================================

        #region

        /// <summary>
        /// �ړ���ɂǂꂭ�炢�_���[�W�󂯂���
        /// </summary>
        float totalDamage;

        /// <summary>
        /// �ړ���ɂǂꂾ�����Ԃ��o���������邽�߂̂���
        /// �o�ߎ��Ԃ������œ����Ƃ��Ɏg��
        /// �ړ��I����ɋL�^����
        /// </summary>
        float totalTime;

        /// <summary>
        /// ����
        /// ���ꂪ����ƃC�x���g�̈������ɂȂ�
        /// ���[�h���؂�ւ�����藣�ꂽ�肷��Ə�����
        /// ���|�[�g�Z���T�[�ŕ񍐂���Ȃ��Ȃ����������
        /// </summary>
        GameObject eventObject;

        /// <summary>
        /// �i�s�����Ɍ������邩���ׂ邽�߂̕ϐ�
        /// ���̈ʒu���烌�C���o��
        /// </summary>
        Vector2 holeCheckPoint;

        /// <summary>
        /// �n�ʂ����邩�A���邢�͌������邩���m���߂邽�߂Ɏg�����C�L���X�g�̒���
        /// </summary>
        float groundCheckLength = 30;

        #endregion



        // === �X�e�[�^�X�p�����[�^ ======================================

        #region

        /// <summary>
        /// �U���{��
        /// </summary>
        [HideInInspector]
        public float attackFactor = 1;
        [HideInInspector]
        public float fireATFactor = 1;
        [HideInInspector]
        public float thunderATFactor = 1;
        [HideInInspector]
        public float darkATFactor = 1;
        [HideInInspector]
        public float holyATFactor = 1;

        [HideInInspector]
        //�X�e�[�^�X
        //hp�̓w���X�ł���
        public float maxHp;
        [HideInInspector]
        public float maxMp;

        /// <summary>
        /// �����񕜕��܂߂�MP�̑���
        /// MP�̎����񕜂͂�����������Ă���
        /// </summary>
        float mpStorage;


        [HideInInspector]
        public float mp;

        #endregion

        /// <summary>
        /// �L�����Z���p�̃g�[�N��
        /// </summary>
        CancellationTokenSource moveJudgeCancel;

        // === �R�[�h�iAI�̓��e�j ================



        

        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            // randomBool = false;


            nowState = SisterState.�ŏ�;
            //�ŏ��̏���
            ParameterSet(status);
            StateChange(SisterState.�̂�т�);
        }



        /// <summary>
        /// 1�t���[�����ƂɁA���Ⴊ��ł��邩�ǂ����A�܂����Ⴊ��ł���ׂ������`�F�b�N���܂�
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            GManager.instance.sisMpSlider.value = mp / maxMp;
            Brain();
        }






        public void Brain()
        {

            JumpController();
        }



        #region ��ԊǗ��R�[�h







        /// <summary>
        /// �X�e�[�g�ω��O�̏���
        /// 
        /// �퓬:�_���[�W���󂯂邩�G��������ŋN���B�G�̑S�ł��퓬��Ԃ̓G�����Ȃ��Ȃ����猳�ɖ߂�B���邢�͈�ԋ߂��G���퓬�I�������ɗ�����H
        /// �x��:�퓬�ォ�댯�����������ɋN���B���Ԍo�߂ŉ���
        /// �̂�т�:���ɂ��Ȃ��Ƃ��͂���
        /// 
        /// �t�@�X�g�g���x����Ƃ��̂��߂ɑS�����Z�b�g����@�\���p�ӂ��Ƃ���
        /// </summary>
        /// <param name="nextState"></param>
        public void StateChange(SisterState nextState)
        {

            //���݂̏�Ԃ���ς��Ȃ��㏑���ł��邩
            bool notChange = nextState == nowState;

            if (nextState == SisterState.�킢)
            {
                //�ύX����Ă���Ȃ�
                if (!notChange)
                {
                    nowState = nextState;

                    //�C�x���g�I�u�W�F�N�g������
                    eventObject = null;

                    //�U�����W���[���ɐ퓬�J�n��`����
                    _fire.StateInitialize(true);

                    //�퓬�Z���T�[�N��
                    _sensor.BattleStart();


                    //�L�����Z�����ăg�[�N������ւ�
                    TokenReset();


                    //�s���X����D��̕��ɐݒ�
                    CombatModeChange(parameter.priority);


                    //�U���ړ��J�n
                    CombatMoveJudge(isFirst: true).Forget();

                    //�U����ԉ������f���\�b�h�N��
                    BattleEndJudge().Forget();

                }
            }
            //�퓬�ȊO�Ȃ�
            else
            {
                //�O���킢���ŏ��Ȃ�������̏�����
                if (nowState == SisterState.�킢 || nowState == SisterState.�ŏ�)
                {
                    //��퓬�Z���T�[�N��
                    _sensor.BattleEnd();
                    //�U�����W���[���ɐ퓬�I����`����
                    _fire.StateInitialize(false);

                    //��L����������
                    nowTarget = null;

                    //�L�����Z�����ăg�[�N������ւ�
                    TokenReset();

                    //�ʏ�ړ��J�n
                    MoveController().Forget();


                }

                //�ς���Ă�Ȃ�
                if (!notChange)
                {


                    //���x���Ȃ�x�������������\�b�h���N��
                    if(nextState == SisterState.�x��)
                    {
                        //�x����Ԃ̎�������
                        PatrolEndJudge(isFirst:true).Forget();
                    }

                    //��ԕύX
                    nowState = nextState;
                }
            }


        }


        /// <summary>
        /// �퓬���̍s�����[�h��ύX����
        /// �����Ɉړ��Ɏg�p�����������ւ���
        /// </summary>
        /// <param name="nextMode"></param>
        void CombatModeChange(MoveType nextMode)
        {
            nowMode = nextMode;

            if(nextMode == MoveType.�U��)
            {
                //�ړ��X�e�[�^�X��ݒ�
                moveStatus = parameter.sisterMoveSetting.AttackMoveSetting;
            }
            else if(nextMode == MoveType.�x��)
            {
                //�ړ��X�e�[�^�X��ݒ�
                moveStatus = parameter.sisterMoveSetting.SupportMoveSetting;
            }
            else
            {
                //�ړ��X�e�[�^�X��ݒ�
                moveStatus = parameter.sisterMoveSetting.HealMoveSetting;
            }

        }

        /// <summary>
        /// �L�����Z��������
        /// �L�����Z���g�[�N�����ēx���꒼��
        /// �����ɑS��ԂŎg�����\�b�h�̋O�����s��
        /// </summary>
        void TokenReset()
        {
            //�L�����Z�����ăg�[�N������ւ�
            moveJudgeCancel.Cancel();
            moveJudgeCancel = new CancellationTokenSource();

            //�L�����Z���g�[�N�������ւ����̂ŋ����ێ����[�v���Ăђ���
            DistanceKeepWarp().Forget();

            //MP��
            MPRecover().Forget();


        }

        /// <summary>
        /// �ړ��Ȃǂ̃f�[�^���Z�b�g
        /// �ŏ��ɌĂ�
        /// </summary>
        /// <param name="status"></param>
        void ParameterSet(SisterStatus status)
        {
            ///<summary>
            ///�@���X�g
            /// </summary>
            #region
            /*
		 CharacterJump _characterJump;
		 PlayerRoll _rolling;
		 CharacterRun _characterRun;
		 EnemyFly _flying;
		 GuardAbillity _guard;
		 MyWakeUp _wakeup;
		 EAttackCon _attack;

			*/
            #endregion

            GravitySet(status.firstGravity);

            _characterHorizontalMovement.WalkSpeed = status.walkSpeed;
            if (_characterRun != null)
            {
                _characterRun.RunSpeed = status.dashSpeed;
            }



            if (_jump != null)
            {
                _jump.CoyoteTime = status.jumpCool;
                _jump.JumpHeight = status.jumpRes;
                _jump.NumberOfJumps = 2;

            }

            maxMp = status.maxMp;
            mp = maxMp;

            //MP�̑��ʂ�ݒ�
            mpStorage = status.mpStorage;

            _health.MaximumHealth = status.maxHp;
            _health.CurrentHealth = (int)maxHp;
        }


        #region �퓬��ԉ������\�b�h



        /// <summary>
        /// �퓬�I���̔��f���s��
        /// �G����b�Ԃ��Ȃ���Όx���t�F�C�Y�Ɉڂ�
        /// ���̎��_�ōU���͒��~���悤�BFire�ҏW���ɍU�����~���\�b�h�����
        /// </summary>
        private async UniTaskVoid BattleEndJudge()
        {
            //��b�҂�
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken:moveJudgeCancel.Token) ;

            //�G���[���ɂȂ邩
            //�ŒZ�����ɂ���G�Ƃ̋������퓬�I�������ȉ��ɂȂ�����
            if (BattleEndCondition())
            {
                //����ɎO�b�҂���
                await UniTask.Delay(TimeSpan.FromSeconds(3f),cancellationToken: moveJudgeCancel.Token);

                //����ł������𖞂����Ă�����
                if (BattleEndCondition())
                {
                    //�x����Ԃ�
                    StateChange(SisterState.�x��);
                    return;
                }
            }

            //�����łȂ���΍ċA�Ăяo��
            BattleEndJudge().Forget();
        }

        /// <summary>
        /// �퓬�I���ɂȂ����
        /// �ł��߂������ɂ���G���퓬�ێ������̊O�ɂ��邩�A���邢�͓G���X�g����ɂȂ����Ȃ�
        /// </summary>
        /// <returns></returns>
        bool BattleEndCondition()
        {
            return !SManager.instance._targetList.Any() ||
                 Vector2.SqrMagnitude(SManager.instance.sisterPosition - SManager.instance.GetClosestEnemy()._condition.targetPosition) < Mathf.Pow(status.battleEndDistance, 2);
        }


        #endregion


        #region �x����ԉ������\�b�h



        /// <summary>
        /// �x����Ԃ̏I���҂����s��
        /// ����̎��ԑ҂��āA���̌�͌x�����ׂ��I�u�W�F�N�g���������10�b����
        /// ����Ƃ͊֌W�Ȃ����ǌx���I�u�W�F�N�g�̒��g�ŉ�b�̓��e�ς���Ă���������
        /// </summary>
        private async UniTaskVoid PatrolEndJudge(bool isFirst)
        {
            if (isFirst)
            {
                //�x����Ԃ̎������ԕ��҂�
                await UniTask.Delay(TimeSpan.FromSeconds(patrolTime), cancellationToken: moveJudgeCancel.Token);
            }



            //�x�����ׂ����I�u�W�F�N�g���Ȃ��Ȃ�
            if (eventObject == null)
            {

                StateChange(SisterState.�̂�т�);
                return;

            }
            //�܂�����Ȃ�
            else
            {
                //�����10�b�҂���
                await UniTask.Delay(TimeSpan.FromSeconds(3f), cancellationToken: moveJudgeCancel.Token);
            }

            //�ċA�Ăяo��
            PatrolEndJudge(false).Forget();
        }


        #endregion

        #endregion





        ///�ۗ��_
        ///
        ///�E
        ///�EfireAbillity�Ń^�[�Q�b�g�ɂ���͎̂��ӃZ���T�[�Ŋl�������G�����ɂ��邩
        ///�EfireAbillity�Ƃ̘A�g
        ///�E���s������܂ł͍Ĉړ����Ȃ��H
        ///�E�w���X�֘A�������P���邩���H


        ///�����v�f
        /// 
        /// �E�^�[�Q�b�g���f�@��
        /// �E�ړ����f�@�Z
        /// �E���[�v�ړ����f�@��
        /// �E�_���[�W�C�x���g�@�Z
        /// �E�ǃC�x���g�@�Z
        /// �E�ړ�������C�x���g�@�Z
        /// �E�ړ����f�C�x���g�@�Z
        /// 
        /// 
        /// �E�G�������̍s���@
        /// 

        ///�@����̂܂Ƃ�
        ///
        ///�܂��ŏ��Ɉړ����f
        ///�ړ����f����ړ��i���[�v���k���j
        ///�ړ����͒��f�����ƕǌ��˃C�x���g�𔻒f
        ///�ړ���͎��͂̏󋵂���Ĉړ����邩�𔻒f�ł���
        ///�ړ���͍U���Ȃǂ���B���̎��_���[�W���󂯂�ƃ_���[�W�C�x���g���󂯂�
        ///�Ĉړ������𖞂������_���[�W�C�x���g�ōĈړ�����

        ///�ړ����Ɏg���C�x���g
        ///�E���f
        ///�E�ǁi���j����
        ///�E�Ĉړ�����

        ///�ړ���Ɏg�����
        ///�E�Ĉړ����f
        ///�E�_���[�W�C�x���g

        ///�Y��ł邱��
        ///�E�ŏ��͂ǂ��������ɏ������X�^�[�g�����邩�B�����Ȃ�ړ����f�J�n�ŁA�ړ��ォ��U�����n�߂�H
        ///�E�|�W�V�����ɂ����̂��ǂ��\�����邩�Benum�̏�Ԃ��t���O��
        ///�E�ړ��I����̏���



        #region �퓬�ړ��X�e�[�^�X�֘A

        #region �ړ����f


        //�_���[�W���Ƃ̔��f�Ƃ̌��ˍ����̓_���[�W���f�J�n���ɔ񓯊��L�����Z�����ۂ�
        //

        /// <summary>
        /// �s�����邩�̔��f�����āA�����ʒu��ړ����@�����߂郁�\�b�h
        /// �ŏ��͖������Œʂ�
        /// �ړ����͌Ă΂�Ȃ��悤�ɂ���H
        /// </summary>
        /// <param name="isFirst"></param>
        async UniTaskVoid CombatMoveJudge(bool isFirst)
        {

            //�ŏ�����Ȃ��Ȃ�������f
            //�ŏ������͏����֌W�Ȃ��ʒu�����J�n����
            if (!isFirst)
            {
                //��Ԃ��ƂɌ��߂��b���҂�
                await UniTask.Delay(TimeSpan.FromSeconds(moveStatus.judgePace));


                //���f�ŋU�Ȃ�߂�
                if (!JudgeMoveStart(nowTarget, moveStatus.rejudgeCondition, moveStatus.orCondition))
                {
                    //�ċA�Ăяo��
                    CombatMoveJudge(false).Forget();
                    return;
                }
            }

            //�ړ����ނƃ_���[�W���Z�b�g
            totalDamage = 0;


            //�����N���A�����玟�̃^�[�Q�b�g��ݒ�
            nowTarget = TargetSelect(moveStatus.markTarget);


            //��������ړ���̏ꏊ�����߂�
            //�I�v�V�����Ƌ����Ō��߂āA�ړ��J�n������͈͂ő҂�
            //x���̈ʒu����
            float standPosition = StandPositionSet(nowTarget._condition.targetPosition.x,moveStatus.keepDistance,moveStatus.moveOption);


            //���[�v���邩�ǂ���
            //���ꂪ�^�Ȃ�ړ����\�b�h�ł͂Ȃ����[�v�ړ����Ăяo���H
            //���[�v�����͂܂����낢��l����B���m�ɂԂ������藎�����肵�Ȃ��悤�ɋC��t���Ȃ���
            bool useWarp = false;

            //���[�v���邩�ǂ����f�f
            //���[�v�g���Ȃ画�f
            if (moveStatus.warpCondition.condition != RejudgeCondition.���肵�Ȃ�)
            {
                //���[�v���邩�ǂ���
                useWarp = MoveStartJudgeExe(nowTarget,moveStatus.warpCondition);
            }

            //��~�����Ɩ��@���g�����Ⴄ�̂ŕʂ̂ɂ���
            //�����FireAbillity�����b�N
            nowMove = MoveState.�ŏ�;

            //���[�v�g���Ȃ烏�[�v���\�b�h
            if (useWarp)
            {
                WarpMoveAct(standPosition);
            }
            //�g��Ȃ��Ȃ�ʏ�ړ�
            else
            {
                NormalCombatMove(standPosition, moveStatus.MoveStopCondition,false,true).Forget();
            }

        }


        #region �^�[�Q�b�g��I��

        /// <summary>
        /// �ړ��̊�ɂ���^�[�Q�b�g��I��
        /// �e�����ɏ]���W�I�̃f�[�^���擾����
        /// </summary>
        /// <returns></returns>
        TargetData TargetSelect(MarkCharacterCondition condition)
        {
            if(condition == MarkCharacterCondition.�v���C���[)
            {
                return SManager.instance._targetList[0];
            }
            else if (condition == MarkCharacterCondition.��ԋ߂��G)
            {
                return SManager.instance.GetClosestEnemy();
            }
            else if (condition == MarkCharacterCondition.��ԉ����G)
            {
                //�������Ԃ��Ă��炤
                return SManager.instance.GetClosestEnemy(true);
            }
            else if (condition == MarkCharacterCondition.�����_���ȓG)
            {
                int count = SManager.instance._targetList.Count;

                //�����_���ȓG��
               return SManager.instance._targetList[_fire.RandomValue(0, count - 1)];

            }
            else if (condition == MarkCharacterCondition.���G)
            {
                return SManager.instance.GetStrongestEnemy();
            }
            else if (condition == MarkCharacterCondition.��ԍ����Ƃ���ɂ���G)
            {
                return SManager.instance.GetHighestEnemy();
            }

            return null;
        }


        #endregion


        #region �����ʒu�Ĕ��f�����̔��f

        /// <summary>
        /// �ړ��J�n�𔻒f����
        /// 
        /// �K�v�Ȃ̂�
        /// �E�G�f�[�^
        /// �E���f�����̔z��
        /// �EOr�������ǂ�����bool
        /// 
        /// �����ł͂����܂œ��������f���邾��
        /// ���ꂾ���Ȃ̂ňړ������Ƃ��͂���Ȃ�
        /// ���ƕW�I�̋��������ޏ������������炻�̓G���S������Ĕ��f����C�x���g���t���O��΂��Ȃ��Ƃ�
        /// </summary>
        bool JudgeMoveStart(TargetData target, RejudgeStruct[] condition,bool isOrJudge)
        {
            //�X�e�[�^�X��ݒ�
            moveStatus = parameter.sisterMoveSetting.AttackMoveSetting;

            //100����Ȃ��Ȃ�p�[�Z���g���f���Ă�
            if ( condition[0].percentage != 100)
            {
                //�m�������͊J���ŗ���
                if (condition[0].percentage > _fire.RandomValue(0,100))
                {
                    return false;
                }
            }

            //��ł��K���������������������Ă���
            bool isMatch = false;

            for(int i=0; i< 3; i++)
            {
                //�������f
                if (MoveStartJudgeExe(target, condition[i]))
                {
                    isMatch = true;
                }
                //����and�������U�Ȃ炻�̎��_�ŃT���i��
                else if (!isOrJudge)
                {
                    return false;
                }

            }

            //for�������������ƈ�ł����v��������������Ȃ�^
            return isMatch;

        }


        /// <summary>
        /// ���ۂ̔��f���s��
        /// </summary>
        bool MoveStartJudgeExe(TargetData target, RejudgeStruct condition)
        {
            if (condition.condition == RejudgeCondition.��̋�������O�ꂽ��)
            {

                //�^�[�Q�b�g���Ȃ��Ȃ�^��Ԃ��Ă�
                if(nowTarget == null)
                {
                    return true;
                }

                //�����Ńp�����[�^�[����ێ����������������Ă���
                float baseDistance = moveStatus.keepDistance * moveStatus.keepDistance;

                float distance = Vector2.SqrMagnitude(SManager.instance.sisterPosition - target._condition.targetPosition);

                //�p�[�Z���e�[�W���[���ȏ�Ȃ狗�����ȏ㗣�ꂽ��
                //�ȉ��Ȃ�ȓ��ɓ�������
                 return condition.value > 0 ? distance > baseDistance : baseDistance > distance ;
            }
            else if (condition.condition == RejudgeCondition.�G�ɑ_��ꂽ��)
            {
                //�V�X�^�[�����_���Ă�G������
                return EnemyManager.instance._targetList[1].targetAllCount > 0;
            }

            //����͓G���ϓ��C�x���g�����܂��g���Ăł��Ȃ���
            //�G�̐��ς�����t���O����b�Ԑ^�ɂȂ��Ă�Ƃ�
            //�퓬�}�l�[�W���[�ɂ�点�Ă���������
            else if (condition.condition == RejudgeCondition.�G�̐����ϓ�)
            {

            }
            else if (condition.condition == RejudgeCondition.���̋����ɓG�����鎞)
            {
                //�����Ńp�����[�^�[����ێ����������������Ă���
                float baseDistance = condition.value * condition.value;

                float distance = Vector2.SqrMagnitude(SManager.instance.sisterPosition - target._condition.targetPosition);

                //�p�[�Z���e�[�W���[���ȏ�Ȃ狗�����ȏ㗣�ꂽ��
                //�ȉ��Ȃ�ȓ��ɓ�������
                return condition.value > 0 ? distance > baseDistance : baseDistance > distance;
            }
            else if (condition.condition == RejudgeCondition.���Ԍo��)
            {

                //�Ĉړ���Ɏ��Ԃ��L�^���Ă�
                //0�ȏ�Ȃ�ȏ�A�ȉ��Ȃ�ȉ�
                return condition.value > 0 ? totalTime >= condition.value : totalTime <= Mathf.Abs(condition.value);
            }
            else if (condition.condition == RejudgeCondition.���ӂɓG�����̂��邩)
            {
                //�ȏォ�ȉ����͒l���[���ȏォ�Ō��܂�
                return condition.value > 0 ? _sensor.EnemyCount >= condition.value : _sensor.EnemyCount <= -condition.value;
            }
            else if (condition.condition == RejudgeCondition.������MP���w��̊����̎�)
            {
                //�ȏォ�ȉ����͒l���[���ȏォ�Ō��܂�
                return condition.value > 0 ? (mp / status.maxMp) > condition.value / 100 : (mp / status.maxMp) < Mathf.Abs(condition.value) / 100;
            }
            else if (condition.condition == RejudgeCondition.�ړ���Ɏw��̃_���[�W���󂯂���)
            {
                //�ړ���̃_���[�W���L�^���Ă�̂Ŕ��f
                return condition.value > 0 ?  totalDamage >= Mathf.Abs(condition.value) : totalDamage <= Mathf.Abs(condition.value);
            }

            return false;
        }


        #endregion


        #region �����ʒu����


        /// <summary>
        /// �����ɏ]���ė����ʒu�����߂�
        /// �G�̈ʒu���痣��鋗���𑫂����ʒu���������ʒu��
        /// �ǂ���ɍs������������
        /// </summary>
        /// <param name="xPosition"></param>
        /// <param name="setDistance"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        float StandPositionSet(float xPosition,float setDistance, PositionJudgeOption condition)
        {

            //�v���X
            float plusPosition = xPosition + setDistance;

            //�}�C�i�X
            float minusPosition = xPosition - setDistance;

            float usePosition = 0;

            //���ꂪ�^�Ȃ�}�C�i�X�̕���Ԃ�
            //���Ȃ킿��
            bool isLeft = false;

            //�߂����ɍs��
            if(condition == PositionJudgeOption.�I�v�V��������)
            {
                usePosition = SManager.instance.sisterPosition.x;
                //�}�C�i�X�̕����������߂��i���l���������j�Ȃ�}�C�i�X��n��
                isLeft = Mathf.Abs(plusPosition - usePosition) > Mathf.Abs(minusPosition - usePosition);

            }
            else if(condition == PositionJudgeOption.�v���C���[�̋߂��̈ʒu�ɍs��)
            {
                usePosition = GManager.instance.PlayerPosition.x;

                //�}�C�i�X�̕����������߂��i���l���������j�Ȃ�}�C�i�X��n��
                isLeft = Mathf.Abs(plusPosition - usePosition) > Mathf.Abs(minusPosition - usePosition);
            }
            else if (condition == PositionJudgeOption.�v���C���[�̉����̈ʒu�ɍs��)
            {
                usePosition = GManager.instance.PlayerPosition.x;

                //�v���X�̕����������߂��i���l���������j�Ȃ�}�C�i�X��n���B����������
                isLeft = Mathf.Abs(plusPosition - usePosition) < Mathf.Abs(minusPosition - usePosition);
            }
            else if (condition == PositionJudgeOption.��L�����̔w��ɉ��)
            {
                //�E�����Ă�Ȃ獶�i�}�C�i�X�j�ɍs��
                isLeft = nowTarget.targetObj.transform.localScale.x > 0;
            }
            else if (condition == PositionJudgeOption.��L�����̐��ʂɗ���)
            {
                //�������Ă�Ȃ獶�i�}�C�i�X�j�ɍs��
                isLeft = nowTarget.targetObj.transform.localScale.x < 0;
            }
            else if (condition == PositionJudgeOption.��L��������G���������ɍs��)
            {
                isLeft = SManager.instance.MoreEnemySide(xPosition);
            }
            else if (condition == PositionJudgeOption.��L��������G�����Ȃ����ɍs��)
            {
                isLeft = !SManager.instance.MoreEnemySide(xPosition);
            }
            else if (condition == PositionJudgeOption.�ǂ��߂����ɍs��)
            {
                //���̕����ǂ��߂���
                isLeft = WallDistanceCheck();
            }
            else if(condition == PositionJudgeOption.�ǂ��������ɍs��)
            {
                //���̕����ǂ�������
                isLeft = !WallDistanceCheck();
            }

            //���Ȃ�����������A�E�Ȃ瑫��������
            return isLeft ? minusPosition : plusPosition;

        }


        /// <summary>
        /// ���E�ǂ���Ɉړ�����ƕǂ��߂�����Ԃ�
        /// �^��Ԃ������̕����߂�
        /// ���C�L���X�g���g���č��E�̕ǂ�T��
        /// </summary>
        /// <returns></returns>
        bool WallDistanceCheck()
        {
            Vector2 basePosition = SManager.instance.sisterPosition;
            
            //�E�̕ǂ܂ł̋����A�ꉞ�ŏ��͈ꖜ����Ă���
            float distanceR = 10000f;


            //���̕ǂ܂ł̋����A�ꉞ�ŏ��͈ꖜ����Ă���
            float distanceL = 10000;

            //�E�̕ǂ����m
            RaycastHit2D result = Physics2D.Raycast(basePosition, Vector2.right, Mathf.Infinity, _controller.PlatformMask | _controller.MovingPlatformMask | _controller.OneWayPlatformMask | _controller.MovingOneWayPlatformMask);

            //���ɉE�ɕǂ��������炻�̋������i�[����
            if (result.collider != null)
            {
                distanceR = Mathf.Abs(result.point.x - basePosition.x);
            }

            //���̕ǂ����m
            result = Physics2D.Raycast(basePosition, Vector2.left, Mathf.Infinity, _controller.PlatformMask | _controller.MovingPlatformMask | _controller.OneWayPlatformMask | _controller.MovingOneWayPlatformMask);

            //���ɍ��ɕǂ��������炻�̋������i�[����
            if (result.collider != null)
            {
                distanceL = Mathf.Abs(result.point.x - basePosition.x);
            }


            //���̕����E���ǂւ̋������Z����ΐ^
            return distanceL < distanceR;

        }


        #endregion

        #endregion

        #region �ǉ������֘A



        #region �_���[�W�󂯂����̃C�x���g


        /// <summary>
        /// �_���[�W���󂯂����A�_���[�W�C�x���g�𔭓����邩�𔻒f����
        /// �_���[�W�C�x���g
        /// </summary>
        /// <param name="isBack"></param>
        void DamageEventController(bool isBack)
        {

            //�^���U�����̂܂ܐ키�Ȃ珈�����Ȃ�
            //���łɍ����ړ����Ȃ�Ă΂Ȃ��悤�ɂ����ׂ�
            if(moveStatus.damageFalseEvent == DamageMoveEvent.���̂܂ܐ키 && moveStatus.damageTrueEvent == DamageMoveEvent.���̂܂ܐ키)
            {
                return;
            }

            //�w��U���̏ꍇ����isBack�Ō���
            bool isTrue = (moveStatus.damageCondition.condition != RejudgeCondition.�w�ォ��̍U�����󂯂���) ? MoveStartJudgeExe(nowTarget,moveStatus.damageCondition) : isBack;

            //�p�[�Z���e�[�W��100�ȉ��ŏ����^�Ȃ�m���`�F�b�N��
            if (isTrue && moveStatus.damageCondition.percentage < 100)
            {
                isTrue = moveStatus.damageCondition.percentage < _fire.RandomValue(0, 100);
            }

            DamageMoveEvent useEvent = isTrue ? moveStatus.damageTrueEvent : moveStatus.damageFalseEvent;

            //�g�p����C�x���g�����̂܂ܐ키�Ȃ烊�^�[��
            if(useEvent == DamageMoveEvent.���̂܂ܐ키)
            {
                return;
            }

            //����ȊO�Ȃ獡�̈ړ����f���L�����Z������
            //�L�����Z��
            moveJudgeCancel.Cancel();
            moveJudgeCancel = new CancellationTokenSource();

            //��e�C�x���g�̎��s��
            DamageEventExe(useEvent);
        }


        /// <summary>
        /// �_���[�W�C�x���g�̎��s
        /// �ړ��n�͍s������߂ă��[�v���邩�����߂Ă���ړ����s���邾��
        /// </summary>
        /// <param name="useEvent"></param>
        void DamageEventExe(DamageMoveEvent useEvent)
        {

            var eventNum = (int)useEvent;

            //�����ňړ��n�_�ƃ��[�v���邩�����߂ē�����
            if (eventNum <= (int)DamageMoveEvent.�ً}�������[�v)
            {

                float escapePoint = EscapePointSerch(false);

                //���[�v�Ɠ����ԍ��Ȃ烏�[�v
                if(eventNum == (int)DamageMoveEvent.�ً}�������[�v)
                {
                    WarpMoveAct(escapePoint);
                }
                //���[�v����Ȃ��Ȃ�
                else
                {
                    NormalCombatMove(escapePoint,moveStatus.MoveStopCondition,true,true).Forget();
                }

            }
            if (eventNum <= (int)DamageMoveEvent.�v���C���[�̂��Ƃɋً}���[�v)
            {
                //�v���C���[�̈ʒu�ɓ�����
                float escapePoint = GManager.instance.PlayerPosition.x;

                //���[�v�Ɠ����ԍ��Ȃ烏�[�v
                if (eventNum == (int)DamageMoveEvent.�v���C���[�̂��Ƃɋً}���[�v)
                {
                    WarpMoveAct(escapePoint);
                }
                //���[�v����Ȃ��Ȃ�
                else
                {
                    NormalCombatMove(escapePoint, moveStatus.MoveStopCondition, true, true).Forget();
                }
            }
            else if (eventNum <= (int)DamageMoveEvent.�����_���Ƀ��[�h�`�F���W)
            {
                //�����_���Ƃ���ȊO�ŕ������
                if(eventNum == (int)DamageMoveEvent.�����_���Ƀ��[�h�`�F���W)
                {

                }
                else
                {

                }

            }
            else if (useEvent == DamageMoveEvent.�Ĉړ�)
            {
                //�^�[�Q�b�g���ߒ����Ĉړ�
                //�����������Ă邩�Ƃ��͊֌W�Ȃ������̂�first�͐^��
                CombatMoveJudge(true).Forget();
            }
        }


        /// <summary>
        /// ��������ꏊ��T��
        /// x���W��Ԃ����\�b�h
        /// 
        /// �ǂ���ɓ�����ƓG�����Ȃ����Ƃ��A�ǂ���ɓ�����ƕǂ��������Ƃ�����Ǝv���񂾂��
        /// �ǂ������Ȃ�������G�����f���邩�H
        /// ����ŁA���͂̓G�����ׂĂ܂�����Ȃ������x����������Ƃ�
        /// 
        /// ReverseEscape���^�Ȃ獡�����Ă�����̋t�ɓ����Ă���
        /// </summary>
        float EscapePointSerch(bool reverseEscape)
        {
                float basePosition = SManager.instance.sisterPosition.x;
            
            //�t�ɓ�����w��Ȃ��Ȃ�
            if (!reverseEscape)
            {
                bool isRight = SManager.instance.MoreEnemySide(basePosition);

                //�G�����ɑ����Ȃ猻�ݒn�_���E�ɍs��
                return basePosition + (isRight ? (moveStatus.keepDistance * 1.2f) : (-moveStatus.keepDistance * 1.2f));
            }
            //����Ȃ�
            else
            {
                //�E�����Ă�Ȃ獶�ցA�������Ă�Ȃ�E��
                return basePosition + (_character.IsFacingRight ? (-moveStatus.keepDistance * 1.2f) : (moveStatus.keepDistance * 1.2f));
            }
        }



        #endregion


        #region �ǂɂԂ��������̃C�x���g


        /// <summary>
        /// �ǂɂԂ��������̃C�x���g���󂯎��
        /// �ړ����̂ݔ��ʂ���
        /// </summary>
        /// <param name="isReverse">���łɋt���Ɉړ����ł��邩</param>
        void WallCollisionEventJudge(bool isEscape, bool isReverse)
        {
            //�����ɍ�����������
            bool isMatch;

            if (moveStatus.wallCondition.condition == RejudgeCondition.�������ł��邩)
            {
                isMatch = isEscape;
            }
            else
            {
                isMatch = MoveStartJudgeExe(nowTarget, moveStatus.wallCondition);
            }


            //�g���C�x���g������
            WallCollisionEvent useEvent = isMatch ? moveStatus.trueWallEvent : moveStatus.falseWallEvent;


            //�ǂ̂ڂ肷��Ȃ�
            if (isMatch ? moveStatus.trueWallClimb : moveStatus.falseWallClimb)
            {
                //�ǂ̂ڂ�ł��邩���ׂ�
                if (WallClimbCheck())
                {
                    //�ǂ̂ڂ�
                    //�ǂ̂ڂ�();

                    //�����Ė߂�
                    return;
                }
            }

            //�ǂ̂ڂ肵�Ȃ��Ȃ�ǃC�x���g
            WallCollisionEventExe(useEvent, isEscape, isReverse);
        }


        /// <summary>
        /// �ǏՓ˃C�x���g�̎��s
        /// 
        /// 
        ///  ��~,���Α��Ɉړ�,���Α��Ƀ��[�v,�o���Ȃ�ǂɂ悶�o��
        /// </summary>
        /// <param name="useEvent"></param>
        /// <param name="isReverse">���łɋt���Ɉړ����ł��邩�B���X�ƃ��[�v�Ƃ��ړ��J��Ԃ������Ȃ����</param>
        void WallCollisionEventExe(WallCollisionEvent useEvent,bool isEscape,bool isReverse)
        {
            if(useEvent == WallCollisionEvent.��~)
            {
                //�ړ��I������
                return;
            }
            else if((int)useEvent <= (int)WallCollisionEvent.���Α��Ƀ��[�v)
            {
                //���łɔ��Α��Ɉړ����Ȃ��~����
                if (isReverse)
                {
                    //�ړ���~����

                    return;
                }

                //���̎����Ɗ�G�̗����ʒu���x�[�X�ɋt���ɍs����悤�ɖړI�n�����߂�
                //�����Ĉړ���I������
                //�܂��A�������̓��o�[�X��^�ɂ��Ĉʒu��Ԃ��Ă��炤

                float standPosition;
                if (isEscape)
                {
                    standPosition = EscapePointSerch(true);
                }
                else
                {
                    standPosition = nowTarget._condition.targetPosition.x;

                    //��������L������荶�ɂ���Ȃ�E�Ɍ�����
                    standPosition += (SManager.instance.sisterPosition.x < standPosition) ? moveStatus.keepDistance : -moveStatus.keepDistance;
                }

                if(useEvent == WallCollisionEvent.���Α��Ƀ��[�v)
                {
                    WarpMoveAct(standPosition);
                }
                else
                {
                    //�߂��Ă����Ă邩��isReverse�͐^
                    NormalCombatMove(standPosition,moveStatus.MoveStopCondition,isEscape,isReverse = true).Forget();
                }

            }

        }


        /// <summary>
        /// �ǂ̂ڂ�ł��邩�̃`�F�b�N
        /// </summary>
        bool WallClimbCheck()
        {
            return false;
        }


        /// <summary>
        /// �����ǂ����邩�𒲂ׂ�
        /// ���ƕǂ𒲂ׂ���̍s���͓������ƕ��ʂ̈ړ����ŕς���H
        /// </summary>
        protected virtual bool CheckWallAndHole()
        {
            // �n�ʂɂ��Ă��鎞����
            //���ĂȂ��Ȃ�߂�
            if (!_controller.State.IsGrounded)
            {
                return false;
            }


            //�Ȃɂ��ǂɂԂ����Ă�Ȃ�
            if(_controller.CurrentWallCollider != null)
            {
                return true;
            }


            // ���C�L���X�g�𔭎˂���ʒu�����߂�
            //������菭���O���烌�C�L���X�g������
            if(_character.IsFacingRight)
            {
                //�E�����Ă�Ȃ班���E��
                holeCheckPoint.Set(SManager.instance.sisterPosition.x + (_controller.Bounds.x / 2 + 20f), (SManager.instance.sisterPosition.y));
            }
            else
            {
                holeCheckPoint.Set(SManager.instance.sisterPosition.x - (_controller.Bounds.x / 2 + 20f), (SManager.instance.sisterPosition.y));
            }

            //���C������������Ȃ���Ό�������
            return Physics2D.Raycast(holeCheckPoint, Vector2.down, groundCheckLength, _controller.PlatformMask | _controller.MovingPlatformMask | _controller.OneWayPlatformMask | _controller.MovingOneWayPlatformMask).collider == null;


        }

        #endregion








        #endregion

        #endregion




        ///�@
        /// 
        /// �E�R�Ƃ����̓W�����v�ł��Ȃ��Ƃ���͕ǂƓ��������ɂ���
        /// �E�_���[�W�C�x���g�̓����⃏�[�v�͈ړ��セ�̂܂ܐ킢�n�߂�
        /// 
        /// ///

        #region �퓬�ړ����s����


        #region �ʏ�ړ�


        ///<summary>
        ///���[�v����Ȃ��ӂ��̈ړ�
        ///��x�Ăяo�����珟��ɓ��삷��
        ///�퓬�ʒu�A���邢�͓����ȂǂŖڎw���ꏊ�ɑ���
        ///�����̏ꍇ�͒�~�����͂Ȃ�
        /// </summary>
        ///<param name="standPosition">�ڕW�̈ʒu</param>
        ///<param name="stopCondition">��~�������</param>
        ///<param name="isEscape">�������Ă��邩�ǂ����B�ǂɓ����������̏��������f����Ȃ�</param>
        ///<param name="isReverse"></param>
        ///<param name="isFirst"></param>
        async UniTaskVoid NormalCombatMove(float standPosition,RejudgeStruct stopCondition,bool isEscape,bool isReverse,bool isFirst = false)
        {
            //�ŏ�����Ȃ��Ȃ�
            if (!isFirst)
            {
                //0.3���Ƃ�
                await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: moveJudgeCancel.Token);
            }

            //�ړI�n�ւ̋���
            float distance = (standPosition - SManager.instance.sisterPosition.x);


            //��~�����������ĂȂ��Ȃ�_�b�V��
            if (!StopConditionJudge(distance, moveStatus.adjustRange,stopCondition))
            {  

                //����������ǂɂԂ������肵����
                if (CheckWallAndHole())
                {
                    //�ЂƂ܂���~
                    MoveAct(MoveState.��~,0);


                        WallCollisionEventJudge(isEscape,isReverse);

                    return;
                }


                //�ڎw���|�C���g���E�Ȃ�1�ňړ�
                MoveAct(MoveState.����, direction: distance >= 0 ? 1 : -1);

                //�ċA�Ăяo��
                NormalCombatMove(standPosition,stopCondition,isReverse,isEscape).Forget();
            }
            //���������Ȃ�
            else
            {

                //�ړ��I��
                CombatMoveEnd();
            }


        }

        #endregion


        #region ���[�v����

        /// <summary>
        /// �퓬�ʒu�A���邢�͖ڎw���ꏊ�Ƀ��[�v
        /// ���[�v�ړ������s����
        /// 
        /// ���[�v���ăG�t�F�N�g���o��
        /// �ǏՓˌ��m�͒ʏ�ړ����ɂ������f����Ȃ�����ǍۂɃ��[�v���Ă����S
        /// </summary>
        void WarpMoveAct(float standPosition)
        {

            //���[�v���s
            PointWarpExe(WarpPointSerch(standPosition));

            //�ړ��I��
            CombatMoveEnd();
        }

        /// <summary>
        /// ���[�v�̈ʒu�T�����\�b�h
        /// �r���ŕ�(Wall�^�O�Ƃ��H)�ɂԂ���������������瑤�͒T�����Ȃ�
        /// ���[�v�n�_�������Ȃ�������߂�H
        /// ���ƁA���Ƃ��Ǝ��������������i�E�Ƀ��[�v�Ȃ獶�����j�͌��̈ʒu�ɖ߂�܂ŒT������H
        /// 
        /// ���C�L���X�g�Z���T�[�g���ă��C�̒ʂ蓹�ɉ�������̂��𒲂ׂ�̂͂�����������
        /// �ǂɂԂ�������ǂ̈ʒu�Ŏ~�܂�΂悭�Ȃ��H
        /// �ǂ��������o���郌�C�Z���T�[��΂��āA�ǂɂԂ��邩�𒲂ׂ�
        /// �ǂɂԂ������炻���Ŏ~�܂�
        /// �ǂ����邩�𒲂ׂ�c���C�Z���T�[����Ȃ��Ȃ��H
        /// �L���X�g�ł������A�n�ʂ͂ǂ����悤
        /// ���n�n�_��adjust�������͈͂ɕǂ����邩�𒲂ׂ�
        /// �����ĕǂ��Ȃ��͈͂Œn�ʂ�����ꏊ��T��
        /// 
        /// �ǉ��ŃA�C�f�A�����ǁA�ǂɂԂ��������͂̈��͈͂Ƀ��[�v�|�C���g�����邩���������āA��������Ȃ炻���ɔ��?
        /// �Ƃ�����Ƃł��ڂ��̒i���ł������Ƃ��ɔ�ׂ���
        /// 
        /// ���₻��Ȃ��Ƃ��Ȃ��Ă����ݒn�_����\���Ƃ�y���W�Ɉ�{�����C�L���X�g�i�ړI�n�ɒ����������������j���΂�
        /// ���̎O�{���̃��C����ɕǂɂԂ���Ȃ��āA�Ȃ�����30�ȓ��ɒn�ʂ�����|�C���g�ň�ԖړI�n�ɋ߂����m��T��
        /// �O�{�Ƃ��ǂɂԂ���Ȃ�������ӂ��ɒT��
        /// 
        /// ���Ⴀ�܂��l�{�����āA��ԖړI�n�ɋ߂��|�C���g�ŕǂɂԂ��鍂�x��T��
        /// 
        /// </summary>
        Vector2 WarpPointSerch(float standPosition)
        {
            ///�b��̈�ԋ߂��|�C���g
            ///�ŏ��͍��̍��W������
            Vector2 startPoint = SManager.instance.sisterPosition;

            Vector2 nearestpoint = startPoint;

            //���ݒn���E�ɖړI�n������Ȃ�E����������
            bool isRight = (startPoint.x < standPosition);

            //���C�L���X�g�̒���
            float checkRange = Math.Abs(startPoint.x - standPosition);

            RaycastHit2D result;

            //�l�񃌃C�Ń`�F�b�N����
            //���C���ǂɂԂ���Ȃ��������_�ŏI���ł���
            for (int i = 0; i < 4; i++)
            {
                result = Physics2D.Raycast(startPoint, (isRight ? Vector2.right : Vector2.left), checkRange, 0);//�����ƒn�`�̃}�X�N�p�ӂ��āA0�̑����

                //����������Ȃ������ꍇ
                //����ŏI���
                if (result.collider == null)
                {
                    //���ݔ��蒆�̍��W��ݒ�
                    nearestpoint.Set(standPosition, startPoint.y);
                    break;
                }
                //��������̂��������ꍇ
                else
                {
                    //���ɏՓ˒n�_�̕����A�b��̒n�_���ړI�n�ւ̋������Z���Ȃ�
                    if (Math.Abs(standPosition - nearestpoint.x) > Math.Abs(standPosition - result.point.x))
                    {
                        //�V�����߂��|�C���g���X�V�����
                        nearestpoint.Set(result.point.x, startPoint.y);
                    }

                }

                //����̍Ō�ɂ͍��x�𑫂�
                //���͂������������|�C���g�Ō������邱�ƂŒn�`�̓ʉ��ɂ��Ή�
                //������Ƃ�10�����Ă���
                startPoint.MMSetY(startPoint.y + 10);

            }


            //���̌����̂��߂Ƀ��[�v�\��n�X�ւ̋�����10��������
            checkRange = Math.Abs(nearestpoint.x - startPoint.x) / 10;


            //�����ŏo��nearestPoint�����ɁA�n�ʂ��`�F�b�N���čœK�ȃ��[�v�n�_��T��
            //������Ȃ���΃v���C���[�̂��Ƃɔ��
            //���[�v�\��n���猻�ݒn�܂ł̋������\�������Č�������
            for (int i = 0; i < 10; i++)
            {
                //30���ȓ��ɒn�ʂ����邩�`�F�b�N����
                result = Physics2D.Raycast(nearestpoint, Vector2.down, 30, _controller.PlatformMask | _controller.MovingPlatformMask | _controller.OneWayPlatformMask | _controller.MovingOneWayPlatformMask);

                //�n�ʂ��Ȃ��Ȃ玟�̃|�C���g�Ń`�F�b�N����
                if (result.collider == null)
                {
                    //���̃|�C���g�͖ړI�n���E�Ȃ獶�ɋA���Ă��邵�A���Ȃ�E�ɖ߂�
                    nearestpoint.MMSetX(nearestpoint.x + (isRight ? -checkRange : checkRange));
                }
                //�n�`�ɓ��������Ȃ�
                else
                {
                    return nearestpoint;
                }



            }
            //�Ō�܂Œn�ʂ�������Ȃ�������v���C���[�̂��Ƃɍs��
            return GManager.instance.PlayerPosition;

        }


        /// <summary>
        /// ��̓I�ɍ��W���w�肵�ă��[�v���鏈��
        /// ���[�V�����Đ���ҋ@�Ȃǂ��܂�
        /// </summary>
        /// <param name="point"></param>
        void PointWarpExe(Vector2 point)
        {


            //���[�v�J�n
            //�ړ��Ƀ��b�N
            _movement.ChangeState(CharacterStates.MovementStates.Warp);
            _condition.ChangeState(CharacterStates.CharacterConditions.Moving);

            //�ړ�����~
            _characterHorizontalMovement.SetHorizontalMove(0);

            //��u��~
            _controller.SetForce(Vector2.zero);
            
            //�����ŃA�j���[�V�����Đ�
            //�����ɁA�G�t�F�N�g���ĂԃA�j���[�V�����C�x���g�ɂ��G�t�F�N�g���Ă΂��


            //�����Ĉړ�
            //����̓A�j���C�x���g�ɂ��邩�H
            transform.position = point;


            //�ŁA���[�v�A�j�����I����āA�n�ʂ̏�ɂ���Ȃ��Ԃ�Idle�ɖ߂��悤�ȑҋ@���\�b�h��
        }




        #endregion

        #region �ړ��I���㏈��


        /// <summary>
        /// �퓬���̈ړ��A���[�v�Ɠk�������̌�ɌĂ�
        /// </summary>
        void CombatMoveEnd()
        {
            //�ЂƂ܂���~
            _characterHorizontalMovement.SetHorizontalMove(0);
            _controller.SetHorizontalForce(0);

            //�Ĉړ������𖞂����Ȃ������x�ړ��J�n
            if (!MoveStartJudgeExe(nowTarget, moveStatus.RelocationCondition))
            {
                //�����֌W�Ȃ��ړ��J�n����̂Ő^�ɂ���
                CombatMoveJudge(true).Forget();
                return;
            }


            //���݂̎��Ԃ��L�^
            totalTime = GManager.instance.nowTime;
            //�ړ��I����ɑ��_���[�W�����Z�b�g
            totalDamage = 0;

            //�ړ��I�������Ȃ炱���Ŗ��@���g����悤�ɂ��Ă���
            
            //�ړ�������͌��݂̈ړ����~�ɂ���
            nowMove = MoveState.��~;

            //��̃L�������E�ɂ��邩���ɂ��邩���o��
            int direction = (int)Mathf.Sign(nowTarget._condition.targetPosition.x - SManager.instance.sisterPosition.x);

            //��̃L�����ւƐU�����
            SisFlip(direction);
        }


        #endregion


        #region ��~�������f


        /// <summary>
        /// ��~���������邩�Ȃ����Ŕ��f��ς���
        /// </summary>
        /// <param name="distance">���݂̖ړI�n�Ƃ̋���</param>
        /// <param name="adjust">���͈͈̔ȓ��Ȃ�ړI�n�ɒ������Ƃ��鐔�l</param>
        /// <param name="stopCondition">��~����</param>
        /// <returns></returns>
        bool StopConditionJudge(float distance, float adjust, RejudgeStruct stopCondition)
        {

            //��~�������Ȃ��Ȃ狗���𖞂���������Ԃ�
            if (stopCondition.condition != RejudgeCondition.���肵�Ȃ�)
            {
                return (Mathf.Abs(distance) <= adjust);
            }
            //����Ȃ狗������~������
            else
            {
                return (Mathf.Abs(distance) <= adjust) || MoveStartJudgeExe(nowTarget,stopCondition);
            }
        }


        #endregion


        #endregion



        #region ��퓬���ړ��X�e�[�^�X�ݒ胁�\�b�h


        /// <summary>
        /// ��퓬���͂����Ɖ���Ă�
        /// ��Ԃ��Ƃ̌��������W�ݒ�ƃ_�b�V�����~�Ȃǂ̈ړ����@������o��
        /// ���Ƃ͕����Ȃǂ�
        /// 
        /// 
        /// </summary>
        /// <returns></returns>
        async UniTaskVoid MoveController()
        {

            //0.3�b���Ƃ�
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: moveJudgeCancel.Token);

            //�ڎw���ڕW�̈ʒu
            float targetPosition;

            //�s������
            int direction;

            //�g�p����ړ����@
            MoveState useState;

            if (nowState == SisterState.�̂�т�)
            {
                //�̂�т�̎��̓^�[�Q�b�g�������ƃv���C���[�ɂ킩���
                //�̂ł����Ŕ��f
                targetPosition = TargetPointJudge();

                useState = PlayMoveJudge(targetPosition);

            }
            //�x��
            else
            {
                //�x�����͏�Ƀv���C���[�̈ʒu��
                targetPosition = GManager.instance.PlayerPosition.x;


                useState = PatrolMoveJudge(targetPosition);
            }

            //�^�[�Q�b�g�̕����E�ɂ���Ȃ�1���A�����łȂ��Ȃ�-1��Ԃ�
            direction = (targetPosition - SManager.instance.sisterPosition.x) >= 0 ? 1 : -1;

            //�ړ��̏��ƕ��������Ĉړ����s
            MoveAct(useState, direction);

            //�ċA�Ăяo��
            MoveController().Forget();

        }



        #region�@�x����Ԃ̈ړ��X�e�[�^�X�Z�o

        /// <summary>
        /// �^�[�Q�b�g�̈ʒu����x���ړ����̈ړ����@�Ȃǂ����߂�
        /// �_�b�V���Ȃ�
        /// </summary>
        /// <returns></returns>
        MoveState PatrolMoveJudge(float targetPosition)
        {

            //�^�[�Q�b�g�Ǝ����̋����̍����o��

            float distance = Mathf.Abs(SManager.instance.sisterPosition.x - targetPosition);

            if (distance > status.walkDistance)
            {
                return MoveState.����;
            }
            //�߂����鎞�͎~�܂�
            else if (distance < 15)
            {
                return MoveState.��~;
            }
            //�߂��ɂ��鎞
            //�x���͊�{�_�b�V����walkDistance�͎g��Ȃ�
            else// if (distance < status.patrolDistance)
            {

                float playerSpeed = pc.NowSpeed();



                    //���������ŁA�Ȃ��������Ă�Ȃ�
                    //�v���C���[�Ɛi�s������v�Ȃ�ǂ�������
                    if (((playerSpeed > 0 && _character.IsFacingRight) || (playerSpeed < 0 && !_character.IsFacingRight)) && Mathf.Abs(playerSpeed) >= 100)
                    {

                        //�����Ă�Ȃ瑖��Œǂ�������
                            return MoveState.����;

                    }
                    else  if (distance > status.patrolDistance)
                    {
                        return MoveState.����;
                    }
                    else
                    {
                        return MoveState.��~;
                    }


            }

        }

        #endregion

        #region �̂�т�̎��̈ړ��X�e�[�^�X�Z�o

        /// <summary>
        /// �̂�т�̎��̈ړ����f
        /// ��{�̓v���C���[�ɒǏ]
        /// ��������������ߊ��
        /// �ł��A�v���C���[�̈ʒu�����ȏ㗣�ꂽ��������痣��ăv���C���[�̂Ƃ��ɑ���
        /// 
        /// </summary>
        /// <returns></returns>
        MoveState PlayMoveJudge(float targetPosition)
        {

            float distance = Mathf.Abs(targetPosition - SManager.instance.sisterPosition.x);


            //��~��ԂȂ�܂�����o���ɂ͌��\����Ȃ��Ƃ����Ȃ��̂�
            //����������
            //�~�܂��Ă���Ă������Ƃ͏\���ɐڋ߂����Ƃ������Ƃ�����
            if(nowMove == MoveState.��~)
            {
                //��x�~�܂�����V��ł����͈͂��o��܂�
                //�~�܂����܂�
                if (distance < status.playDistance)
                {
                    return MoveState.��~;
                }
                //�V��ł��������ł��瑖��
                else
                {
                    return MoveState.����;

                }
            }

            else
            {
                //�߂��Ɋ������~�܂邩������
                if (distance <= status.patrolDistance + status.adjust)
                {
                        return MoveState.��~;

                }
                //�V��łĂ��������̒��ŁA�v���C���[�ɂ������ĂȂ��Ƃ�
                else if (distance <= status.walkDistance)
                {
                    return MoveState.����;

                }
                //�����ׂ��������痣�ꂽ��
                else
                {
                    return MoveState.����;

                }
            }


        }


        /// <summary>
        /// �^�[�Q�b�g�I�����Ĉʒu��Ԃ����\�b�h
        /// 
        /// ��������������ߊ��
        /// �ł��A�v���C���[�̈ʒu�����ȏ㗣�ꂽ��������痣��ăv���C���[�̂Ƃ��ɑ���
        /// </summary>
        /// <returns></returns>
        float TargetPointJudge()
        {

            //�v���C���[�Ƃ̋���
            float distance = Mathf.Abs(GManager.instance.PlayerPosition.x - SManager.instance.sisterPosition.x);


            //�C�x���g�I�u�W�F�N�g�����鎞
            //�����ăv���C���[���߂��ɂ��鎞�i������������_���j
            //�V��ł��������ȓ��ɂ���Ȃ�����̈ʒu��Ԃ�
            if (eventObject != null && distance <= status.playDistance)
            {
                return eventObject.transform.position.x;
            }
            else
            {
                return GManager.instance.PlayerPosition.x;
            }
        }


        #endregion

        #endregion


        #region ���ʈړ�����



        /// <summary>
        /// ���ʈړ�����
        /// ���Ԋu�ŌĂяo����Ĉړ���ύX����
        /// 
        /// ��Ԃ��ƂɈړ���Ԃƕ������Z�b�g����
        /// ���@�g�p����A�g�g�p���͒�~����@�\������
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="direction"></param>
        void MoveAct(MoveState condition,int direction)
        {

            //�ʏ펞�ȊO�A�����Ēn�ɑ����Ă��Ȃ���Β�~�ɂȂ��Ė߂�
            if(_condition.CurrentState != CharacterStates.CharacterConditions.Normal || !_controller.State.IsGrounded)
            {
                _characterHorizontalMovement.SetHorizontalMove(0);
                _controller.SetHorizontalForce(0);

                //�����ł͈ړ��f�[�^�͐G��Ȃ��B�U�����\�����邩��
               // nowMove = MoveState.��~;
                return;
            }
                //�U�����
                SisFlip(direction);

            //��~���͎~�܂���������]������
           if(condition == MoveState.��~)
            {
                _characterHorizontalMovement.SetHorizontalMove(0);
                _controller.SetHorizontalForce(0);
                //����
                _movement.ChangeState(CharacterStates.MovementStates.Idle);

                //��Ԃ��X�V
                nowMove = condition;

                return;
            }

            //�����ύX
            _characterHorizontalMovement.SetHorizontalMove(direction);

            //�O��ƃX�e�[�g�������Ȃ���������ς��Ė߂�
            if (nowMove == condition)
            {

                  return;
            }

            if (condition == MoveState.����)
            {
                _characterRun.RunStart();
            }
            else if (condition == MoveState.����)
            {
                if (nowMove == MoveState.����)
                {
                    _characterRun.RunStop();
                }
            }
            //��Ԃ��X�V
            nowMove = condition;

        }

        /// <summary>
        /// �U���������
        /// </summary>
        /// <param name="dire"></param>
        void SisFlip(float dire)
        {

            //�������E�ŉE�����Ă�Ȃ�U������Ȃ�
            //�������Ăĕ��������Ȃ��͂�U������Ȃ�
            //dire��0�Ȃ�߂�
            if ((dire > 0 && _character.IsFacingRight) || (dire < 0 && !_character.IsFacingRight)|| dire == 0)
            {
                return;
            }

            _character.Flip();
        }


        /// <summary>
        /// �v���C���[�Ɨ��ꂽ���A�v���C���[�̂��ƂɃ��[�v����
        /// ���u����h������A���c����Ȃ��悤�ɂ��邽�߃��\�b�h
        /// 
        /// ���[�h�ύX�Ńg�[�N���������Ă��܂�
        /// �퓬���[�h�J�n�Ɛ퓬���[�h�I���A������̃��[�h�ύX�ł��Ă΂��
        /// </summary>
        async UniTaskVoid DistanceKeepWarp()
        {
            await UniTask.WaitUntil(()=> (Vector2.SqrMagnitude(GManager.instance.PlayerPosition - SManager.instance.sisterPosition)) > Mathf.Pow(status.warpDistance,2),
                cancellationToken: moveJudgeCancel.Token);

            //�������ʏ��Ԃ���Ȃ��Ȃ�
            if(_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
            {
                //�ʏ��ԂɂȂ�܂ő҂���
                await UniTask.WaitUntil(() => _condition.CurrentState == CharacterStates.CharacterConditions.Normal,cancellationToken: moveJudgeCancel.Token);
            }

            Vector2 warpPoint = GManager.instance.PlayerPosition;

            //�v���C���[�̔w�����Ɍ����悤�ɓ]�ڈʒu�𒲐�
            warpPoint.MMSetX(pc.transform.localPosition.x > 0 ? warpPoint.x - 10 : warpPoint.x + 10);

            PointWarpExe(warpPoint);

        }

        #endregion











        public void JumpController()
        {

            //isVertical�͎����ŃI���I�t����
            if (!_controller.State.IsGrounded && _movement.CurrentState == CharacterStates.MovementStates.Jumping)
            {
                if (!isVertical)
                {
                    _horizontalInput = Mathf.Sign(transform.localScale.x);
                }
                if (!disEnable && _jump.JumpEnableJudge() == true)
                {
                    _jump.JumpStart();
                }
            }
            else
            {
                isVertical = false;
            }

        }

        public void JumpStart()
        {
            if (!disEnable && _controller.State.IsGrounded)
            {
                _jump.JumpStart();
            }
        }



        /// <summary>
        /// ���C���[�ύX�BAvoid�Ȃ񂩂Ǝg���Ă��������P�̂ł��g����
        /// </summary>
        /// <param name="layerNumber"></param>
        public void SetLayer(int layerNumber)
        {

            this.gameObject.layer = layerNumber;

        }

        /// <summary>
        /// �d�͐ݒ�
        /// </summary>
        /// <param name="gravity"></param>
        public void GravitySet(float gravity)
        {
            //rb.gravityScale = gravity;
            _controller.DefaultParameters.Gravity = gravity;
        }








        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_controller.State.IsGrounded)
            {

                //�g���l���̒������Ⴊ�݃g���K�[�Ŗ�����
                if (collision.tag == squatTag && _movement.CurrentState != CharacterStates.MovementStates.Crouching && _movement.CurrentState != CharacterStates.MovementStates.Crawling)
                {
                    _squat.Crouch();
                }
                //else�ł��ꂵ�Ⴊ�݉������邼
                if (collision.tag == jumpTag && _controller.State.IsGrounded)
                {
                    //GetComponent�͂Ȃ�ׂ����ʂ悤��
                    if (collision.gameObject.GetComponent<JumpTrigger>().jumpDirection == transform.localScale.x)
                    {
                        jumpTrigger = true;

                    }
                }

            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (_controller.State.IsGrounded)
            {
                //�g���l���̒������Ⴊ�݃g���K�[�Ŗ�����

                if (collision.tag == squatTag && _movement.CurrentState != CharacterStates.MovementStates.Crouching && _movement.CurrentState != CharacterStates.MovementStates.Crawling)
                {
                    _squat.Crouch();
                }
                if (collision.tag == jumpTag && _controller.State.IsGrounded)
                {
                    if (collision.gameObject.GetComponent<JumpTrigger>().jumpDirection == transform.localScale.x)
                    {
                        jumpTrigger = true;

                    }
                }

            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (_controller.State.IsGrounded)
            {
                //�g���l���̒������Ⴊ�݃g���K�[�Ŗ�����
                if (collision.tag == squatTag && (_movement.CurrentState != CharacterStates.MovementStates.Crouching || _movement.CurrentState == CharacterStates.MovementStates.Crawling))
                {
                    _squat.ExitCrouch();
                }
                if (collision.tag == jumpTag)
                {
                    jumpTrigger = false;

                }
            }
        }









        public void WarpEffect()
        {
            Vector3 posi = new Vector3(transform.position.x, transform.position.y - 11, 40);

            //Transform rotate = SManager.instance.useMagic.castEffect.LoadAssetAsync<Transform>().Result as Transform;

            Addressables.InstantiateAsync("WarpCircle", posi, Quaternion.Euler(-98, 0, 0));//.Result;//�����ʒu��Player
            GManager.instance.PlaySound("Warp", transform.position);
        }



        bool CheckEnd(string Name)
        {

            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(Name))// || _animator.GetCurrentAnimatorStateInfo(0).IsName("OStand"))
            {   // �����ɓ��B�����normalizedTime��"Default"�̌o�ߎ��Ԃ��E���Ă��܂��̂ŁAResult�ɑJ�ڊ�������܂ł�return����B
                return true;
            }
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {   // �ҋ@���Ԃ���肽���Ȃ�΁A�����̒l��傫������B
                return true;
            }
            //AnimatorClipInfo[] clipInfo = _animator.GetCurrentAnimatorClipInfo(0);

            ////Debug.Log($"�A�j���I��");

            return false;

            // return !(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
            //  (_currentStateName);
        }








        public void MPReset()
        {
            mp = maxMp;
        }




        /// <summary>
        /// ���͉�
        /// ���͂̑��ʂ�����Ԃ͉񕜂�������
        /// </summary>
        async UniTaskVoid MPRecover()
        {
            //MP�̑��ʂ��s����������񕜂��Ȃ�
            if (mpStorage <= 0)
            {
                //�����������瑍�ʂ��񕜂�����A�C�e���Ƃ���邩��
                //���̎��͂����̏����͕ς���
                return;
            }

            //���݂�MP���ő�MP�ȏ�̎�
            if (mp >= maxMp)
            {
                //mp���񕜉\�ɂȂ�܂ő҂�
                await UniTask.WaitUntil(() => mp < maxMp, cancellationToken: moveJudgeCancel.Token);
            }

            //2.5�b�Ɉ�񂾂���
            await UniTask.Delay(TimeSpan.FromSeconds(2.5f), cancellationToken: moveJudgeCancel.Token);

            //�s�����ĂȂ��Ƃ����͉�
            if (_condition.CurrentState != CharacterStates.CharacterConditions.Moving)
            {

                //�N�[���^�C�����͖��͉�1.2�{�H
                float recoverAmo = SManager.instance.sisStatus.mpRecover + SManager.instance.sisStatus.additionalRecover;

                mp += (recoverAmo);

                //�񕜂��������ʂ���͌��炷
                mpStorage -= recoverAmo;
            }

            //���E�ʂ͉����Ȃ��悤��
            if (mpStorage < 0)
            {
                mpStorage = 0;

            }

            //�ċA�Ăяo��
            MPRecover().Forget();
        }


        /// <summary>
        ///  �K�v�ȃA�j���[�^�[�p�����[�^�[������΁A�A�j���[�^�[�p�����[�^�[���X�g�ɒǉ����܂��B
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_stateParameterName, AnimatorControllerParameterType.Bool, out _stateAnimationParameter);
        }

        /// <summary>
        /// ������I�[�o�[���C�h����ƁA�L�����N�^�[�̃A�j���[�^�[�Ƀp�����[�^�𑗐M���邱�Ƃ��ł��܂��B
        /// ����́ACharacter�N���X�ɂ���āAEarly�Anormal�ALate process()�̌�ɁA1�T�C�N�����Ƃ�1��Ăяo�����B
        /// </summary>
        public override void UpdateAnimator()
        {
            //�̂�т�1�A�x��2�A�킢3
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _stateAnimationParameter, nowState != SisterState.�̂�т�, _character._animatorParameters);
        }


        #region �Z���T�[�Ɗ����֘A�̏���


        /// <summary>
        /// �G�����������ɌĂяo�����\�b�h
        /// </summary>
        public override void FindEnemy()
        {
            //�퓬���[�h��
            StateChange(SisterState.�킢);
        }


        /// <summary>
        /// ���������I�u�W�F�N�g��񍐂���
        /// </summary>
        /// <param name="isDanger">�댯�����ǂ���</param>
        /// <param name="obj">����������</param>
        public override void ReportObject(bool isDanger, GameObject obj)
        {

            //���݌x����ԂȂ�
            if(nowState == SisterState.�x��)
            {
                //�x���I�u�W�F�N�g�ȊO�̕񍐂͎󂯕t���Ȃ�
                if (!isDanger)
                {
                    eventObject = null;

                    return;
                }
            }

            //�댯�������������͌x�����[�h��
            //�x�����[�h�͈�莞�ԉ����Ȃ��Ǝ�������
            if (isDanger)
            {
                StateChange(SisterState.�x��);
            }


            //null���n���ꂽ�ꍇ�͋���ۂɂȂ�
            eventObject = obj;
        }

        #endregion



        #region �R���o�b�g�}�l�[�W���[�̘A�g

        /// <summary>
        /// �G�̃^�[�Q�b�g���X�g���̎����̃f�[�^���X�V����
        /// _condition�͏�ɍŐV�̃f�[�^��ۂ�
        /// �����ƌĂяo�����
        /// </summary>
        /// <param name="num"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void TargetDataUpdate(int num)
        {

            EnemyManager.instance._targetList[num]._condition.targetPosition = SManager.instance.sisterPosition;
            EnemyManager.instance._targetList[num]._condition.hpRatio = _health.CurrentHealth / status.maxHp;
            EnemyManager.instance._targetList[num]._condition.hpNum = _health.CurrentHealth;
            EnemyManager.instance._targetList[num]._condition.hpRatio = mp / status.maxMp;

            EnemyManager.instance._targetList[num]._condition.buf = false;
            EnemyManager.instance._targetList[num]._condition.de= false;
        }


        /// <summary>
        /// �����̃^�[�Q�b�g�f�[�^��G�̃^�[�Q�b�g���X�g�ɑ���
        /// ����NPC��ID�̈����͂ǂ����悤
        /// Id���p����悤�ȃC�x���g�̓v���C���[���ɂ͂Ȃ�
        /// </summary>
        public override void TargetDataAdd(int myId)
        {

            TargetData imfo = new TargetData();

            imfo._baseData = status._charaData;

            imfo._condition.targetPosition = SManager.instance.sisterPosition;
            imfo._condition.hpRatio = _health.CurrentHealth / status.maxHp;
            imfo._condition.hpNum = _health.CurrentHealth;
            imfo._condition.hpRatio = mp / status.maxMp;

            imfo._condition.buffImfo = false;
            imfo._condition.debuffImfo = false;

            imfo.targetObj = this.gameObject;

            imfo.targetID = myId;

            EnemyManager.instance._targetList.Add(imfo);
        }




        /// <summary>
        /// �^�[�Q�b�g���X�g����폜���ꂽ�G�l�~�[����������
        /// �����ăw�C�g���X�g���𒲐�
        /// �v���C���[�͂Ȃ񂩕ʂ̏�������Ă�����������
        /// ���ƓG�̎���ʒm���郁�\�b�h�Ƃ��Ă��g����
        /// �G���ϓ��C�x���g�͂����ł��
        /// 
        /// </summary>
        /// <param name="deletEnemy"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void TargetListChange(int deletEnemy,int deadID)
        {
            //�^�[�Q�b�g�ƈ�v������Ĕ��f���s��
            if (nowTarget.targetID == deadID)
            {
                //�L�����Z��
                moveJudgeCancel.Cancel();
                moveJudgeCancel = new CancellationTokenSource();
                
                //�����I�ɍĔ��f
                CombatMoveJudge(true).Forget();

                //�g�[�N���������̂ōČĂяo��
                DistanceKeepWarp().Forget();
            }

            //����ɍU���A�r���e�B�ɂ����ł����G��ID�������Ă�����

        }


        /// <summary>
        /// ����������ID��Ԃ�
        /// �����ԂŃC�x���g������肷���ő厖
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override int ReturnID()
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// �������_���^�[�Q�b�g�����肵����
        /// �w������C�x���g�������Ă����΂�
        /// �R���o�b�g�}�l�[�W���[��ʂ��Ē��ԂɒʒB���s��
        /// �ł��G���ł����������Ȃ�����
        /// </summary>
        public override void CommandEvent(EnemyStatus.TargetingEvent _event, int level, int targetNum, int commanderID)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// ���͂̓G����FireAbillity�ɋ����Ă��
        /// �����炩��Ăяo���Ďg��
        /// </summary>
        /// <returns></returns>
        public int EnemyCountReport()
        {
            return _sensor.EnemyCount;
        }


        #endregion


        #region ��e�֘A�̏���

        /// <summary>
        /// ���̃L�������K�[�h���Ă邩�ǂ�����`����
        /// ��ɃK�[�h���Ă�
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool GuardReport()
        {
            return true;
        }



        public override void BuffCalc(FireBullet _fire)
        {
            throw new System.NotImplementedException();
        }





        /// <summary>
        /// �p���B�͏o���Ȃ��L�����N�^�[
        /// ��x�����W���X�g�K�[�h������܂Ƃ����@�Ƃ�����Ȃ�
        /// �p���B����͖��@�ɂ���ďo���\����������
        /// ���������̂���������Ƃ��ɂ܂����߂�
        /// ����܂ł͋󔒂ł���
        /// </summary>
        /// <param name="isBreake"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void ParryStart(bool isBreake)
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// ��ɃK�[�h���Ȃ̂ł��ꂪ�_���[�W���ɂȂ�
        /// �ŗL�̃o���A�I�ȃK�[�h����炷�悤�ɂ���
        /// </summary>
        public override void GuardSound()
        {

        }


        /// <summary>
        /// ����̓X�^���񕜎��ɃA�[�}�[���}�b�N�X�ɂ��邽�߂̃��\�b�h
        /// �Ȃ̂ŃX�^�����Ȃ��L����������Ă΂�邱�Ƃ͂Ȃ�
        /// </summary>
        public override void ArmorReset()
        {
            //��ł���
        }


        /// <summary>
        /// ���̃L�����͊�{�I�ɃX�^���͂��Ȃ��̂ŋ���ۂŗǂ�
        /// </summary>
        /// <param name="stunState"></param>
        public override void StartStun(MyWakeUp.StunnType stunState)
        {

        }

        /// <summary>
        /// �U�����󂯂����̖��͍��
        /// �V���b�N�ɉ����Ė��͂�����
        /// </summary>
        /// <param name="shock"></param>
        /// <param name="isBlow"></param>
        /// <param name="isBack"></param>
        /// <returns></returns>
        public override MyWakeUp.StunnType ArmorControll(float shock, bool isBlow, bool isBack)
        {

            //������΂����w��U���Ȃ�1.1�{�̍����󂯂�
            shock = isBlow || isBack ? shock * 1.1f : shock;

            //mp���甼���J�b�g�����V���b�N������
            //�����̃J�b�g���̓X�e�[�^�X�ŏ㉺���Ă���������
            mp -= (shock/2);

            //0�ȉ��Ȃ�0��
            mp = mp < 0 ? 0 : mp;


            //�X�^���͂��Ȃ�
            return MyWakeUp.StunnType.notStunned;
        }


        /// <summary>
        /// ���ȊO�͂Ȃ�
        /// HP�����Ċm�F���Ă�
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override int GetStunState()
        {
            //�X�^���͂Ȃ�
            return 0;
        }

        /// <summary>
        /// �V�X�^�[����̃A�[�}�[�͖���
        /// 
        /// ���@�����g��Ȃ����p���B����邱�Ƃ͂Ȃ��̂ŌĂ΂�邱�Ƃ͂Ȃ�
        /// </summary>
        /// <returns></returns>
        public override bool ParryArmorJudge()
        {
            return false;
        }



        /// <summary>
        /// �󒆃_�E���𔻒�
        /// ���false
        /// </summary>
        /// <param name="stunnState"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool AirDownJudge(MyWakeUp.StunnType stunnState)
        {
            return false;
        }



        /// <summary>
        /// �ŏI�I�ȃ_���[�W���v�Z
        /// ��ɃK�[�h���
        /// </summary>

        public override void DamageCalc()
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// �h��͂��v�Z
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void DefCalc()
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// ���S���[�V�����J�n
        /// </summary>
        /// <param name="stunnState"></param>
        public override void DeadMotionStart(MyWakeUp.StunnType stunnState)
        {

        }


        /// <summary>
        /// �_���[�W�󂯂����̃C�x���g
        /// �ړ���̍��v�_���[�W�����Z�A�����č��̔�e�C�x���g���N��
        /// </summary>
        /// <param name="isStunn"></param>
        /// <param name="enemy"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void DamageEvent(bool isStunn, GameObject enemy,int damage,bool back)
        {
            //����|�W�V�����ɂ����퓬�������ɂ��Ă�
            if (nowState == SisterState.�킢 && nowMove == MoveState.��~)
            {
                
                //�g�[�^���_���[�W��ǉ�
                totalDamage += damage;

            }
            //�퓬���łȂ���ΐ퓬���[�h��
            else if (nowState != SisterState.�킢)
            {
                StateChange(SisterState.�킢);
            }

            //������������_���[�W���n������
            //�̗͂̉��p�[�Z���g�Ƃ�
            DamageEventController(back);
        }

        /// <summary>
        /// �_���[�W�󂯂����Ƃ�ʒm�H
        /// </summary>
        /// <param name="isBack"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void HitReport(bool isBack)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// �o���A�������Ĉ�莞�ԏ�����
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void Die()
        {
            throw new System.NotImplementedException();
        }

#endregion

    }
}
