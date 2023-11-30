
using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;
using Unity.Mathematics;
using MonKey.Extensions;
using static EnemyStatus;
using static CombatManager;
using static RenownedGames.ApexEditor.SerializedMember;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{


    /// <summary>
    /// �����ƃC���X�y�N�^�����œ�������悤�ɉ���
    /// ���Ƃ������A�߂Â��ĉ��邾���Ƃ��͔ėpAI�Ƃ��ėp�ӂ��Ƃ���
    /// �����ɃX�e�[�^�X�ňړ����x�Ƃ��ԍ����Ƃ��Ǐ]���͑�����������̕ύX�őΉ��ł���
    /// 
    /// 
    /// �ۑ�
    /// ���x���}�l�[�W���[�ɂ���ăZ�O�����g���Ə����ꂽ��ǂ����邩
    /// ���̃Z�O�����g�ɂ͒ǂ������Ă��Ȃ��̂�
    /// �Z�O�����g�Ƃ̐e�q�֌W���������邩
    /// �v���C���[���Z�O�����g���ꂽ��g�[�N����~���ăX���[�v�H
    /// 
    /// </summary>
    public class EnemyAIBase : NPCControllerAbillity
    {

        #region ����


        /// <summary>
        /// �^�[�Q�b�g�̏��
        /// �Œ��
        /// �s�����߂��肷��̂ɂ��g���H
        /// ���Ⴀ�ԍ��ʒu�o�t�t���f�o�t�t��
        /// ��_�A�����A�^�C�v���炢����
        /// </summary>
        public struct TargetImfomation
        {
            /// <summary>
            /// �^�[�Q�b�g�̈ʒu
            /// </summary>
            public Vector2 targetPosition;

            /// <summary>
            /// �^�[�Q�b�g�̔ԍ�
            /// </summary>
            public int targetNum;

            public int targetID;

        }


        #endregion



        //�A�r���e�B�Z�b�g�����[�g���ɂ����Ă��

        //�u//sAni.�v���usAni.�v�Œu��������B�A�j���������

        //�A�j���̎��
        //�K�{
        //�ړ��iMove�j�A�����iWalk�j�A����iDash�j�A���݁iFalter�j�A�_�E���iDown�j�A�N���オ��iWakeup�j
        //������сiBlow�j�A�e����iBounce�j�A�������iBackMove�j�A�퓬�ҋ@�iPose�j�A���iStand�j�A���S�iNDie,DDie�j
        //
        //�K�v�ɉ�����
        //�����iFall�j�A�K�[�h�iGuard�j�A�K�[�h�ړ��iGuardMove�j�A���K�[�h�ړ��iBackGuard�j�A�K�[�h�u���C�N�iGuardBreak�j
        //����iAvoid�j�A�W�����v�iJump�j
        //
        //���p�����[�^
        #region
        [SerializeField] GameObject effectController;





        [Header("�ˏo�n�_")]
        ///<summary>
        ///�e�ۏo���ꏊ
        ///</summary>
        public Transform firePosition;

        /// <summary>
        /// �W���X�K�s�G�t�F�N�g���o���Ƃ�
        /// </summary>
        [SerializeField]
        Transform dispary;

        /// <summary>
        /// �J�����Ɏʂ��Ă邩
        /// </summary>
        [HideInInspector] public bool cameraRendered;



        [Header("�G�l�~�[�̃X�e�[�^�X")]
        public EnemyStatus status;



        // === �O���p�����[�^ ======================================

        /// <summary>
        /// �U�����[�h�ł��邩�ǂ���
        /// </summary>
        /*[HideInInspector]*/
        public bool isAggressive;//�U�����[�h�̓G

        /// <summary>
        /// �퓬�Ƃ��ł�����𗣂ꂽ�Ƃ��߂邽�߂̃t���O
        /// </summary>
        protected bool posiReset;



        protected float escapeTime;//�����邾��



        /// <summary>
        /// �ڕW�̊ԍ����ɂ���
        /// </summary>
        protected bool isReach;


        protected int initialLayer;


        /// <summary>
        /// �K�[�h���肷�邩�ǂ���
        /// ���ꂪ�^�̊Ԃ͖���ړ��J�n���ɃK�[�h���������
        /// </summary>
        protected bool guardJudge;


        //protected bool isDamage;

        /// <summary>
        /// �������邩�ǂ���
        /// </summary>
        protected bool isMovable;
        [HideInInspector] public EnemyStatus.MoveState ground = EnemyStatus.MoveState.wakeup;
        [HideInInspector] public EnemyStatus.MoveState air = EnemyStatus.MoveState.wakeup;

        List<Transform> mattTrans = new List<Transform>();


        protected int attackNumber;//���Ԃ̍U�������Ă���̂�

        /// <summary>
        /// �U���̃N�[���^�C��������������
        /// </summary>
        protected bool isAtEnable = true;
        /// <summary>
        /// �U���̒l
        /// </summary>
        [HideInInspector] public EnemyValue atV;


        /// <summary>
        /// �U���Ԋu���Ԃ��͂���̂Ɏg��Float
        /// </summary>
        float attackChanceTime;

        /// <summary>
        /// �Z���Ƃ̃N�[���^�C��
        /// </summary>
        List<float> skillCoolTime;

        /// <summary>
        /// �Z�̐�
        /// </summary>
        int skillCount;

        /// <summary>
        /// ���߂Ŏg�����X�L���̔ԍ�
        /// </summary>
        int useSkillNum;

        /// <summary>
        /// �⑫�s��������
        /// 
        /// </summary>
        int suppleNumber;

        /// <summary>
        /// �G�ɓ���������
        /// �U���I������False����
        /// </summary>
        bool isHit;

        /// <summary>
        /// �̎���ID 
        /// �퓬�Q�����Ɋ��蓖�ĂĂ��炤
        /// </summary>
        int myID;
  

        // === �L���b�V�� ==========================================

        [Header("�h�b�O�p�C��")]
        ///<summary>
        ///�ړ��̒��S�ƂȂ�I�u�W�F�N�g�B�G�������A�ꂽ��
        ///</summary>
        public GameObject dogPile;

        protected Rigidbody2D dRb;//�h�b�O�p�C���̃��W�b�h�{�f�B
       protected Vector2 startPosition;//�J�n�ʒu�Ƒҋ@���e���g���[�̋N�_
        protected Vector2 basePosition;

        #region �^�[�Q�b�g�֘A

        /// <summary>
        /// �^�[�Q�b�g�Ƃ̋���
        /// </summary>
        protected Vector2 distance;//

        //�����̈ʒu
        protected Vector2 myPosition;

        /// <summary>
        /// �ŏ��Ɍ����Ă�����
        /// </summary>
        protected int firstDirection;

        protected int direction;
        protected int directionY;

        /// <summary>
        /// ���݃^�[�Q�b�g�ɂ��Ă鑊��̏��
        /// </summary>
        protected TargetImfomation targetImfo;

        /// <summary>
        /// �w�C�g���X�g
        /// ���l������
        /// �������͍ŏ��ƁA�G���ϓ��C�x���g�ł��
        /// �������G�̔ԍ���ʒm���Ă���邩��폜����
        /// </summary>
        public List<float> _hateList;

        /// <summary>
        /// �U�����T�����[�h�ɂȂ�
        /// �t���O
        /// </summary>
        public bool attackStop;

        #endregion

 

        /// <summary>
        /// ���̈ʒu�ɖ߂������ǂ���������Ă��邩
        /// </summary>
        protected int baseDirection;

        // === �����p�����[�^ ======================================






        protected float waitCast;

        [HideInInspector] public bool guardHit;

        /// <summary>
        /// �����̈ړ����f�œ��������Ԃ��ǂ���
        /// </summary>
        bool isEscape;


        protected int lastArmor = 0;

        /// <summary>
        /// �A�[�}�[�@�\�����ǂ���
        /// �U���̓r���̃A�j���ŗL���ɂ��Ĉ��\�}�����̃^�C�~���O��I��
        /// </summary>
        bool isArmor;


        /// <summary>
        /// ���̈ړ�����ŃK�[�h����m��
        /// </summary>
        int guardProballity = 100;


        /// <summary>
        /// ���ꂪ�I���̊ԃK�[�h�g�p��
        /// ���Ƒ���Ȃ�
        /// �K�[�h���~�ⓦ���ŉ���
        /// </summary>
        protected bool useGuard;
        //-----------------------------------------------------

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

        protected float attackBuff = 1;//�U���{��
                                       //�X�e�[�^�X
                                       //HP�̓w���X����
        public float maxHp = 100;

        //���̕ӂ���Ȃ�����
        //�ʂɎ��������{���ő��삷��΂悭�ˁH

        //�@�������h��́B�̗͂ŏオ��
        public float Def = 70;
        //�h�˖h��B�ؗ͂ŏオ��
        public float pierDef = 70;
        //�Ō��h��A�Z�ʂŏオ��
        public float strDef = 70;
        //�_���h��A�؂ƌ����ŏオ��B
        public float holyDef = 70;
        //�Ŗh��B�����ŏオ��
        public float darkDef = 70;
        //���h��B�����Ɛ����ŏオ��
        public float fireDef = 70;
        //���h��B�����Ǝ��v�ŏオ��B
        public float thunderDef = 70;


        [HideInInspector]
        public float nowArmor;


        //-------------------------------------------
        [Header("�퓬���̈ړ����x")]
        public Vector2 combatSpeed;
        [Header("�Q�Ɨp�̍U����")]
        /// <summary>
        /// �Q�Ɨp�̍U����
        /// </summary>
        public int atkDisplay;
        [Header("�Q�Ɨp�̖h���")]
        /// <summary>
        /// �Q�Ɨp�̖h���
        /// </summary>
        public int defDisplay;
        //---------------------------------------------



        [SerializeField]
        protected SpriteRenderer td;




        /// <summary>
        /// �G�t�F�N�g����p�̃}�e���A��
        /// </summary>
        [SerializeField]
        protected Renderer parentMatt;
        /// <summary>
        /// �}�e���A������n���t���O
        /// </summary>
        protected int materialSet;
        /// <summary>
        /// ����Ώۂ̃}�e���A���Ǘ��i���o�[�B0����
        /// </summary>
        int mattControllNum;
        /// <summary>
        /// ����Ώۂ̃X�v���C�g�ꗗ
        /// </summary>
        public Transform[] spriteList;
        /*
        /// <summary>
        /// �}�e���A�����Z�b�g���ꂽ��
        /// </summary>
        protected bool spriteSet;*/
        protected List<Renderer> controllTarget = new List<Renderer>();
        /// <summary>
        /// �}�e���A������̂��߂ɕK�v�Ȏ��Ԍv��
        /// </summary>
        float materialConTime;


        #endregion

        //�V�p�����[�^
        #region



        public PlayerJump _jump;
        //�@protected int _numberOfJumps = 0;

        public PlayerRoll _rolling;

        public PlayerRunning _characterRun;

        public EnemyFly _flying;

        public GuardAbillity _guard;

        public MyWakeUp _wakeup;

        public AtEffectCon _atEf;

        public EAttackCon _attack;

        [SerializeField]
        public MyDamageOntouch _damage;

        public ESensorAbillity _sensor;
        private bool isVertical;

        public ParryAbility _parry;

        public MyAttackMove _rush;


        #endregion



        protected const string _suppleParameterName = "suppleAct";
        protected int _suppleAnimationParameter;

        protected const string _combatParameterName = "isCombat";
        protected int _combatAnimationParameter;


        /// <summary>
        /// ���V�X�^�[����̃Z���T�[���ɂ��邩�ǂ���
        /// </summary>
        bool _seenNow;



        /// <summary>
        /// �V�X�^�[���񂪂��̓G���������Ă鎞��
        /// </summary>
        float loseSightTime;



        /// <summary>
        /// �L�����N�^�[�̕ω��f�[�^
        /// </summary>
        ConditionData nowCondition;


        public CancellationTokenSource patrolToken = new CancellationTokenSource();
        public CancellationTokenSource agrToken = new CancellationTokenSource();

        #region ���[�h�֘A�̕ϐ�

        /// <summary>
        /// ���݂̃��[�h
        /// ModeBehaivior�ŕς��
        /// 0����
        /// </summary>
        protected int nowMode = 0;

        /// <summary>
        /// �`�F���W�\�ȃ��[�h�̐�
        /// </summary>
        int modeCount;

        /// <summary>
        /// ���̃��[�h�ɂȂ�������
        /// </summary>
        float modeTime;

        #endregion



        // === �R�[�h�iMonobehaviour��{�@�\�̎����j ================
        protected override void Initialization()
        {
            base.Initialization();

            //�X�e�[�^�X�ݒ�

            ParameterSet(status);
            ArmorReset();
            HPReset();


            //���ۑ�
            startPosition = transform.position;
            firstDirection = Math.Sign(transform.localScale.x);
            myPosition.Set(startPosition.x, startPosition.y);
            initialLayer = this.gameObject.layer;


            if (dogPile != null)
            {
                basePosition = dogPile.transform.position;
                baseDirection = Math.Sign(dogPile.transform.localScale.x);
                dRb = dogPile.GetComponent<Rigidbody2D>();
            }
            else
            {
                basePosition = startPosition;
                baseDirection = firstDirection;

            }

            //���[�h�̐����m�F
            //���[�h�ύX����������Ȃ��Ȃ烂�[�h�ύX���Ȃ�
            modeCount = status.modeChangeCondition.Length;

            //���[�h�ɂȂ������Ԃ�������
            modeTime = GManager.instance.nowTime;

            //
            skillCount = status.AttackCondition.Length;
            skillCoolTime = new List<float>(skillCount);

            for (int i = 0; i < skillCount; i++)
            {

                if (status.AttackCondition[i].skilLevel == 0)
                {
                    skillCoolTime[i] = -100;
                }
                else
                {
                    skillCoolTime[i] = GManager.instance.nowTime;
                }
            }

            //�ŏ��ɂǂ��炩�̃��[�h���N��
            if (!isAggressive)
            {
                PatrolAction();
            }
            else
            {
                CombatAction();
            }

        }


        /// <summary>
        /// �ŏ��̃}�e���A���̖����L���ݒ�
        /// </summary>
        protected void MaterialSet()
        {
            //�e�}�e���A���̏�������
            //	Debug.Log($"{parentMatt.material}");
            //�S���̃X�v���C�g���W�߂Đݒ肷��
            for (int i = 0; i < spriteList.Length; i++)
            {

                GetAllChildren(spriteList[i]);
                //	await UniTask.WaitForFixedUpdate();
            }

            Material coppy = controllTarget[0].material;

            //���炩�̏����ł�������ς���
            if (true)
            {
                coppy.EnableKeyword("Fade_ON");
                coppy.DisableKeyword("BLUR_ON");
                coppy.DisableKeyword("MOTIONBLUR_ON");
            }

            for (int i = 0; i <= controllTarget.Count - 1; i++)
            {
                controllTarget[i].material.CopyPropertiesFromMaterial(coppy);

            }

            controllTarget.Clear();
        }

        public override void ProcessAbility()
        {
            base.ProcessAbility();
            Brain();

            LoseSightWait();
            //	Debug.Log($"���ǂ���{_animator.name}");


        }





        #region �Ǘ��R�[�h

        /// <summary>
        /// ���ʂŖ��t���[���J��Ԃ�����
        /// </summary>
        public void Brain()
        {
            //Debug.Log($"dsfwe");
            //�J�����Ɏʂ��ĂȂ��Ƃ��͎~�܂�܂��B
            if (!cameraRendered && !status.unBaind)
            {


                //�����Ȃ��Ȃ���
                return;
            }

            //�ʒu���X�V
            myPosition.Set(transform.position.x, transform.position.y);



            //�ړ��\���ǂ����m�F
            //�^�[�Q�b�g�i���o�[99�͓G�s�݂̈Ӗ��Ȃ̂Œ�~
            if (guardHit || _condition.CurrentState != CharacterStates.CharacterConditions.Normal || targetImfo.targetNum == 99)
            {

                isMovable = false;

            }
            //�󒆉���Ƃ��󒆍U�����邩��
            /*
            else if (!_controller.State.IsGrounded && status._charaData._kind != EnemyStatus.KindofEnemy.Fly)
            {
                isMovable = false;
        }*/
            else
            {

                isMovable = true;
            }

            //�h�b�O�p�C������܂���

            Debug.Log($"�`�F�b�N{myPosition.x >= startPosition.x + status.waitDistance.x}");

            //�������鎞����
            if (isMovable)
            {


                ////////Debug.log($"�W�����v��{nowJump}");
                ////////Debug.log($"���{_movement.CurrentState != CharacterStates.MovementStates.Rolling}");
                if (isAggressive)
                {
                    //�����X�V
                    distance = targetImfo.targetPosition - myPosition;
                    direction = distance.x >= 0 ? 1 : -1;

                    //�^�[�Q�b�g�������Ă邩�`�F�b�N
                    EscapeCheck();






                    //�퓬���J��Ԃ�����
                    CombatLoop();

                }

                else if (!isAggressive)
                {

                    if (posiReset)
                    {
                        PositionReset();
                    }

                    //�x�����J��Ԃ�����
                    PatrolLoop();
                }
            }



            //�g���K�[�ŌĂт܂��傤
            JumpController();


            MaterialControll();



        }

        ///<summary
        ///�@�^�[�Q�b�g��ݒ肵�Ă��̏����l������
        ///�@�l�����ׂ����͓G�ւ̃w�C�g�A�����ē����G��_���Ă閡�������l���邩
        ///�@�����ċ߂����������H
        ///�@
        ///</summary>
       �@async UniTaskVoid  TargetSet(bool isFirst = false)
        {

            //�ŏ��͍U�����Ă�������^�[�Q�b�g�Ȃ̂�

            //�ŏ��ȊO���߂����ԑ҂��ă`�F�b�N
            //�܂��̓i���o�[��99�ŕW�I���S�A�����`�F�b�N��
            await UniTask.WhenAny(UniTask.Delay(TimeSpan.FromSeconds(status.hateBehaivior.TargetChangeTime), cancellationToken: agrToken.Token),
                UniTask.WaitUntil(()=> targetImfo.targetNum == 99, cancellationToken: agrToken.Token));

            int count = EnemyManager.instance._targetList.Count;

            //�J�E���g���[���̎��퓬�I��
            if(count== 0)
            {
                CombatEnd();
            }


            int utillityCount = status.hateBehaivior.priorityTarget.Length;

            //�D��^�C�v����Ȃ�w�C�g���Z����
            if (utillityCount > 0)
            {
                
                for (int i = 0; i < utillityCount; i++)
                {

                    //��l��l�����Ă���������
                    for (int s = 0; s < count; s++)
                    {

                        //�܂��̓^�[�Q�b�g�␳


                        //�L�����̑�����v���邩�w��Ȃ��Ȃ�
                        if (status.hateBehaivior.priorityTarget[s].targetType != CharacterStatus.CharaType.none &&
                            status.hateBehaivior.priorityTarget[s].targetType != EnemyManager.instance._targetList[i]._baseData._type)
                        {
                            return;
                        }
                        //�L�����̎�ނ���v���邩�w��Ȃ��Ȃ���
                        if (status.hateBehaivior.priorityTarget[s].targetKind != CharacterStatus.KindofEnemy.none &&
                            status.hateBehaivior.priorityTarget[s].targetKind != EnemyManager.instance._targetList[i]._baseData._kind)
                        {
                            return;
                        }

                        /*
                        //�w�C�g���Z����
                        float addHate;

                        //�����̍Œ�ۏ�
                        float insurance = status.hateBehaivior.priorityTarget[s].initialHate * (status.hateBehaivior.priorityTarget[s].addRatio - 0.5f);

                        //�w�C�g��0�ȉ��̎��͔{�������Ă�����܈Ӗ��Ȃ�
                        if (_hateList[i] >= 0)
                        {

                            //���Z�Ȃ�ŏ��̃w�C�g�ɔ{���������̂𑫂�
                            //���Z�Ȃ�����������Ȃ�
                            if (status.hateBehaivior.priorityTarget[s].addRatio > 1)
                            {
                            addHate =  insurance;
                            }
                            else
                            {
                                continue;
                            }

                        }
                        else
                        {
                            //���Z���镪���v�Z
                            addHate = _hateList[i] * (status.hateBehaivior.priorityTarget[s].addRatio - 1);

                            //���������Œ�ۏ؂�菬�����Ȃ�
                            addHate = insurance > addHate ? addHate : insurance;
                        }

                        //���Z����
                        _hateList[i] += addHate;
                        */


                        float addHate = status.hateBehaivior.priorityTarget[s].initialHate / 2;
                        //���Z����
                        _hateList[i] = !status.hateBehaivior.priorityTarget[s].isDecrease ? _hateList[i] + addHate : _hateList[i] - addHate;



                    }
                }
            }



            //���������������
            utillityCount = status.hateBehaivior.hateEffect.Length;

            if (utillityCount > 0)
            {
                Vector2[] container;
                    Vector2 set = new Vector2(99,0);
                for (int i = 0; i < utillityCount; i++)
                {
                    //��r���Ȃ��A�����ȏ���
                    bool isFair = false;

                    container = new Vector2[3] {set,set,set};
                    bool half = (int)status.hateBehaivior.hateEffect[i]._influence > 3;

                    for (int s = 0; s < count; s++)
                    {



                        if (!half)
                        {
                            if (status.hateBehaivior.hateEffect[i]._influence == EnemyStatus.HateInfluence.��틗��)
                            {


                                //��틗���ƓG�Ƃ̋����̍����o��
                                float jDistance = Vector2.SqrMagnitude(myPosition - EnemyManager.instance._targetList[s]._condition.targetPosition);

                                //��틗������
                                //���ۂ̋����̕����傫���Ȃ�ȉ��ɂȂ�
                                if (status.hateBehaivior.hateEffect[i].isHigh && Vector2.SqrMagnitude(Vector2.zero - status.agrDistance[nowMode]) - jDistance < 0)
                                {
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //�������߂����D��
                                        if (container[t].x == 99 || (container[t].y > jDistance))
                                        {
                                            container[t].Set(s, jDistance);
                                            break;
                                        }
                                    }

                                }

                                //�ȓ�
                                else if (!status.hateBehaivior.hateEffect[i].isHigh && Vector2.SqrMagnitude(Vector2.zero - status.agrDistance[nowMode]) - jDistance >= 0)
                                {
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //�������߂����D��
                                        if (container[t].x == 99 || (container[t].y > jDistance))
                                        {
                                            container[t].Set(s, jDistance);
                                            break;
                                        }
                                    }
                                }




                            }
                            else if (status.hateBehaivior.hateEffect[i]._influence == EnemyStatus.HateInfluence.������)
                            {

                                //��틗���ƓG�Ƃ̋����̍����o��
                                float jDistance = Math.Abs(Vector2.SqrMagnitude(myPosition - EnemyManager.instance._targetList[s]._condition.targetPosition));

                                //������
                                if (status.hateBehaivior.hateEffect[i].isHigh)
                                {
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //�������߂����D��
                                        if (container[t].x == 99 || (container[t].y > jDistance))
                                        {
                                            container[t].Set(s, jDistance);
                                            break;
                                        }
                                    }

                                }

                                //�߂���
                                else
                                {
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //�������������D��
                                        if (container[t].x == 99 || (container[t].y < jDistance))
                                        {
                                            container[t].Set(s, jDistance);
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (status.hateBehaivior.hateEffect[i]._influence == EnemyStatus.HateInfluence.HP����)
                            {
                                    float ratio = EnemyManager.instance._targetList[s]._condition.hpRatio;

                                //������
                                if (status.hateBehaivior.hateEffect[i].isHigh)
                                {
                                    for (int t = 0; t < 3; t++)
                                    {


                                        //�������������D��
                                        if (container[t].x == 99 || (container[t].y < ratio))
                                        {
                                            container[t].Set(s, ratio);
                                            break;
                                        }
                                    }
                                }

                                //�Ⴂ��
                                else
                                {
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //�������Ⴂ���D��
                                        if (container[t].x == 99 || (container[t].y > ratio))
                                        {
                                            container[t].Set(s, ratio);
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (status.hateBehaivior.hateEffect[i]._influence == EnemyStatus.HateInfluence.MP����)
                            {

                                float ratio = EnemyManager.instance._targetList[s]._condition.mpRatio;

                                //������
                                if (status.hateBehaivior.hateEffect[i].isHigh)
                                {
                                    for (int t = 0; t < 3; t++)
                                    {


                                        //�������������D��
                                        if (container[t].x == 99 || (container[t].y < ratio))
                                        {
                                            container[t].Set(s, ratio);
                                            break;
                                        }
                                    }
                                }

                                //�Ⴂ��
                                else
                                {
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //�������Ⴂ���D��
                                        if (container[t].x == 99 || (container[t].y > ratio))
                                        {
                                            container[t].Set(s, ratio);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (status.hateBehaivior.hateEffect[i]._influence == EnemyStatus.HateInfluence.�������)
                            {
                                isFair = true;
                                //�������B�o�t����
                                if (status.hateBehaivior.hateEffect[i].isHigh && EnemyManager.instance._targetList[s]._condition.buffImfo != 0)
                                {
                                    bool isEnd = true;
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //�Ƃ肠�����O�l�W�߂�
                                        if (container[t].x == 99)
                                        {
                                            isEnd = false;
                                            container[t].Set(s, 0);
                                            break;
                                        }
                                    }
                                    //�����O�l������Ȃ�s���[�v���I������
                                    if (isEnd)
                                    {
                                        break;
                                    }
                                }

                                //�Ⴂ���B�o�t�Ȃ�
                                else if(!status.hateBehaivior.hateEffect[i].isHigh && EnemyManager.instance._targetList[s]._condition.buffImfo == 0)
                                {
                                    bool isEnd = true;
                                    for (int t = 0; t < 3; t++)
                                    {
                                        if (container[t].x == 99)
                                        {
                                            isEnd = false;
                                            container[t].Set(s, 0);
                                            break;
                                        }
                                    }
                                    //�����O�l������Ȃ�s���[�v���I������
                                    if (isEnd)
                                    {
                                        break;
                                    }
                                }
                            }
                            
                            else if (status.hateBehaivior.hateEffect[i]._influence == EnemyStatus.HateInfluence.�f�o�t���)
                            {
                                isFair = true;

                                //������
                                if (status.hateBehaivior.hateEffect[i].isHigh && EnemyManager.instance._targetList[s]._condition.debuffImfo != 0)
                                {
                                    bool isEnd = true;
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //�Ƃ肠�����O�l�W�߂�
                                        if (container[t].x == 99)
                                        {
                                            isEnd = false;
                                            container[t].Set(s, 0);
                                            break;
                                        }
                                    }
                                    //�����O�l������Ȃ�s���[�v���I������
                                    if (isEnd)
                                    {
                                        break;
                                    }
                                }

                                //�Ⴂ���B�f�o�t�Ȃ�
                                else if (!status.hateBehaivior.hateEffect[i].isHigh && EnemyManager.instance._targetList[s]._condition.debuffImfo == 0)
                                {
                                    bool isEnd = true;
                                    for (int t = 0; t < 3; t++)
                                    {
                                        if (container[t].x == 99)
                                        {
                                            isEnd = false;
                                            container[t].Set(s, 0);
                                            break;
                                        }
                                    }
                                    //�����O�l������Ȃ�s���[�v���I������
                                    if (isEnd)
                                    {
                                        break;
                                    }
                                }
                            }

                            else if (status.hateBehaivior.hateEffect[i]._influence == EnemyStatus.HateInfluence.�U����)
                            {

                                //�U���͂��݂�
                                float value = EnemyManager.instance._targetList[s]._baseData.displayAtk;

                                //������
                                if (status.hateBehaivior.hateEffect[i].isHigh)
                                {
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //�������D��
                                        if (container[t].x == 99 || (container[t].y < value))
                                        {
                                            container[t].Set(s, value);
                                            break;
                                        }
                                    }

                                }

                                //�߂���
                                else
                                {
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //�Ⴂ���D��
                                        if (container[t].x == 99 || (container[t].y > value))
                                        {
                                            container[t].Set(s, value);
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (status.hateBehaivior.hateEffect[i]._influence == EnemyStatus.HateInfluence.�h���)
                            {
                                //�h��͂�����
                                float value = EnemyManager.instance._targetList[s]._baseData.displayAtk;

                                //������
                                if (status.hateBehaivior.hateEffect[i].isHigh)
                                {
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //�������D��
                                        if (container[t].x == 99 || (container[t].y < value))
                                        {
                                            container[t].Set(s, value);
                                            break;
                                        }
                                    }

                                }

                                //�߂���
                                else
                                {
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //�Ⴂ���D��
                                        if (container[t].x == 99 || (container[t].y > value))
                                        {
                                            container[t].Set(s, value);
                                            break;
                                        }
                                    }
                                }
                            }

                        }


                    }

                    //�Ō�Ƀw�C�g����
                    for (int s = 1; s < 4; s++)
                    {
                        //x��99�Ȃ痬����
                        if (container[s].x == 99)
                        {
                            continue;
                        }

                        //�D�򂪂��Ă�Ȃ�
                        if (!isFair)
                        {
                            _hateList[(int)container[s].x] += 30 / s;
                        }
                        else
                        {
                            _hateList[(int)container[s].x] += 20;
                        }
                    }
                }

            }



            //�w�C�g�l���H��������^�[�Q�b�g����
            float nowHate = 0;

            for (int i = 0; i < count; i++)
            {
                //�ŏ����w�C�g����Ȃ�
                if(i == 0 || _hateList[i] >= nowHate)
                {
                    nowHate = _hateList[i];
                    utillityCount = i;
                }

            }


            //�^�[�Q�b�g���}�l�[�W���[�ɕ�
            //�����ă^�[�Q�b�g��ID���Q�b�g
            targetImfo.targetID = EnemyManager.instance.TargetSet(utillityCount,targetImfo.targetNum,status.hateBehaivior._event,(int)status.hateBehaivior.level,myID,isFirst);

            //�^�[�Q�b�g�����̏���
            //�^�[�Q�b�g���胁�\�b�h���}�l�[�W���[����Ăяo������
            //�^�[�Q�b�g���胁�\�b�h�ɂ̓^�[�Q�b�g���莞�C�x���g����������
            //���Ƃ͍U�����b�N���[�h�ɂ�����
            targetImfo.targetNum = utillityCount;


            //�U�����b�N�����ݒ肳��ĂĂȂ����������ӂ�Ă��
            //�^�[�Q�b�g�Z�b�g�̎��������ꔻ�f����
            //�U���䖝����l�ɑI�΂ꂽ��͋����Ƃ��ɋC�������ē����Ȃ��悤��

            int needCount = status.hateBehaivior.attackLockCount;

            if (needCount != 0 &&
                needCount <= EnemyManager.instance.TargettingCount((int)status.hateBehaivior.level, targetImfo.targetNum))
            {

                //���������āA�����������Ȃ�U�����T�����[�h�Ƀ`�F���W
                //���T���t���O���ĂĎ��̃��[�h�ύX�ŗ}��
                attackStop = (EnemyManager.instance.AttackStopCheck(targetImfo.targetNum, (int)status.hateBehaivior.level, Vector2.SqrMagnitude(Vector2.zero - distance), needCount,myID));


            }
            //�ČĂяo��
            TargetSet().Forget();
        }


        /// <summary>
        /// �U���Ԋu���o�߂�����J��Ԃ���鏈��
        /// �퓬����AI����
        /// ����̓I�[�o�[���C�h���Ȃ�
        /// </summary>
        async UniTaskVoid AttackBehaivior()
        {

            //�U���p�x�����҂�
            await UniTask.Delay(TimeSpan.FromSeconds(status.attackFrquency),cancellationToken:agrToken.Token);

            //��Ԃ�����ɂȂ�܂ő҂�
            await UniTask.WaitUntil(()=>(isMovable && isAtEnable),cancellationToken:agrToken.Token);

            //�U��������Ȃ��Ȃ�

                float time = GManager.instance.nowTime;
                int count = status.AttackCondition.Length;

                //�g�p����U���̔ԍ�
                int useNum = 100;

                //�g�p����X�L���̃��x��
                //���������D��
                int useLevel = -1;
                //�������Ă���
                for (int i = 0; i < count; i++)
                {



                    //�S���̃��[�h�ŏo���Ȃ��āA�����������̃��[�h�łȂ��Ȃ�
                    if (status.AttackCondition[i]._attackMode != EnemyStatus.Mode.AllMode && (int)status.AttackCondition[i]._attackMode != nowMode)
                    {
                        continue;
                    }

                    //�����ƕK�v�Ȃ����҂��ĂȂ��Ȃ�
                    if (status.AttackCondition[i].coolTime > time - skillCoolTime[i])
                    {
                        continue;
                    }

                    float dist = Math.Abs(distance.x);

                    //�����֌W�����āA�Ȃ�����x��y�̋����̊Ԃɂ��Ȃ��Ȃ�
                    if ((status.AttackCondition[i].useDistance != Vector2.zero) && !(status.AttackCondition[i].useDistance.x <= dist && status.AttackCondition[i].useDistance.y >= dist))
                    {
                        dist = Math.Abs(distance.y);

                        //���x���֌W�����ĂȂ�����������Ȃ�
                        if ((status.AttackCondition[i].useHeight != 0) && (status.AttackCondition[i].useHeight < dist))
                        {
                            continue;
                        }

                    }
                    //�m�����O�����Ȃ�
                    if (status.AttackCondition[i].probability < RandomValue(0, 100))
                    {
                        //�N�[���^�C�������Z�b�g
                        skillCoolTime[i] = time;
                        continue;
                    }
                    //�X�L�����x�����Ⴂ�Ȃ�D�悵�Ȃ�
                    if (status.AttackCondition[i].skilLevel < useLevel)
                    {
                        continue;
                    }

                    useLevel = status.AttackCondition[i].skilLevel;

                    useNum = i;
                }

                //�ԍ���100����ς���Ă�Ȃ�
                if (useNum != 100)
                {
                    Attack(useNum);
                }

                //�I�ׂȂ������Ȃ�܂��Ă�
                //�I�ׂ��ꍇ�̓��[�v���Ȃ�
                else
                {
                    AttackBehaivior().Forget();
                }
        }



        /// <summary>
        /// �퓬���[�h�𔻒f����
        /// �퓬����AI����
        /// ����̓I�[�o�[���C�h���Ȃ�
        /// �̗͂̊����͎����̓G��񂩂�Ƃ�
        /// </summary>
        async UniTaskVoid ModeBehaivior()
        {

                //1�b�Ɉ��`�F�b�N
                await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: agrToken.Token);
                
                //���݂̎���
                float time = GManager.instance.nowTime;

                float testratio = 1;

                //�ς����̃��[�h
                int changeMode = 100;

                //��������
                float realDistance = Vector2.SqrMagnitude(myPosition - targetImfo.targetPosition);

                for (int i =0;i< modeCount;i++)
                {

                    //�U���}���t���O�������Ă�Ȃ�U���}�����[�h�n��
                    //�����ă��[�v�I��
                    if (status.modeChangeCondition[i].isAttackStop && attackStop)
                    {
                        changeMode = i;
                        break;
                    }

                    //�J�ڌ��̃��[�h���֌W�����ĂȂ����قȂ��Ă���
                    if(status.modeChangeCondition[i]._nowMode!= EnemyStatus.Mode.AllMode && (int)status.modeChangeCondition[i]._nowMode != nowMode)
                    {
                        continue;
                    }
                    //���[�h�J�ڂɎ��Ԃ��֌W����A�Ȃ������Ԃ��o�߂��ĂȂ�
                    else if (status.modeChangeCondition[i].changeTime != 0 && status.modeChangeCondition[i].changeTime > time - modeTime)
                    {
                        continue;
                    }
                    //HP�������֌W�����āA�w��ȉ��̊����łȂ��Ȃ�
                    else if (status.modeChangeCondition[i].healthRatio != 0 && status.modeChangeCondition[i].healthRatio < testratio)
                    {
                        continue;
                    }
                    //�����������֌W�����ĂȂ����������Ȃ��Ȃ�
                    //x���߂���y��藣��Ă���
                    else if (status.modeChangeCondition[i].changeDistance != Vector2.zero && (status.modeChangeCondition[i].changeDistance.x > realDistance || status.modeChangeCondition[i].changeDistance.y < realDistance) )
                    {
                        continue;
                    }
                    //���ύX�\��̃��[�h��背�x�����Ⴂ�Ȃ�R���e�B�j���[
                    if (changeMode != 100 && status.modeChangeCondition[i].modeLevel < status.modeChangeCondition[changeMode].modeLevel)
                    {
                        continue;
                    }

                    //�ȏ�̏����S�Ăɓ��Ă͂܂�Ȃ�������b�����
                    changeMode = i;
                }

                if(changeMode != 100)
                {
                    nowMode = changeMode;

                    //���[�h�ύX���Ԃ��m��
                    modeTime = time;

                    //���[�h���x���ő�Ȃ�����J�ڂ��Ȃ�
                    if (status.modeChangeCondition[nowMode].modeLevel == 5)
                    {
                        modeCount = 0;
                    }

                    //���[�h�ύX�C�x���g���Ă�
                    ModeChangeEvent(changeMode);
                }

                //���[�h�ύX���Ă�
                ModeBehaivior().Forget();
            
        }


        /// <summary>
        /// �U����~��Ԃɂ��邽�߂̃`�F�b�N�Ɏg��
        /// </summary>
        /// <param name="target"></param>
        /// <param name="level"></param>
        /// <param name="cDistance"></param>
        /// <returns></returns>
        public bool ATBlockDistanceCheck(int target,int level, float cDistance,int id)
        {
            //�^�[�Q�b�g���Ⴄ�����x�����Ⴄ�Ȃ�߂�
            if(target != targetImfo.targetNum || level > (int)status.hateBehaivior.level || myID == id)
            {
                return false;
            }

            //�`�F�b�N�̎�̂̕��������������Ȃ�
            //�����͑�������������m�[�J����
            return cDistance > Vector2.SqrMagnitude(distance);

        }



        #endregion

        //�I�[�o�[���C�h���Ďg���Ηl�X�ȃ^�C�~���O�ɏ������������߂�
        #region �����̃C�x���g�Q





        /// <summary>
        /// �U����ɉ�������Ȃ炱���ɓ����
        /// �C�x���g
        /// </summary>
        protected void AttackEvent()
        {
            //�R���{�Ȃ�ŏI�i�܂ŉ������Ȃ�
            if (atV.isCombo)
            {
                return;
            }

            //��s�G�͔�e��m���ɓ�����
            if (status._charaData._kind == EnemyStatus.KindofEnemy.Fly)
            {
                EscapeJudge(100);
            }
            //�����łȂ��Ȃ�m����
            else
            {
                EscapeJudge(atV.escapePercentage);

            }

            //�U���Ԋu���Z�b�g
            attackChanceTime = GManager.instance.nowTime;

            //�X�L���̃N�[���^�C�����Z�b�g
            skillCoolTime[useSkillNum] = GManager.instance.nowTime;
        }


        /// <summary>
        /// �U�����q�b�g�������ɂǂ̂悤�Ƀq�b�g���������܂߂ċ�����
        /// </summary>
        /// <param name="isBack">���Ă����肪�����̌��ɂ��鎞�͐^</param>
        public override void HitReport(bool isBack)
        {
            isHit = true;

        }

        /// <summary>
        /// �U���퓬�J�n���ɋN�����鏈��
		/// AgrMove�Ȃ�
        /// </summary>
        protected void CombatAction()
        {
            if(skillCount > 0)
            {
                AttackBehaivior().Forget();
            }
            if(modeCount > 0)
            {
                ModeBehaivior().Forget();
            }
            TargetSet(true).Forget();

            //�o�g���ɎQ��
            EnemyManager.instance.JoinBattle(this);

        }


        /// <summary>
        /// �퓬���ɌJ��Ԃ�����
		/// Update����
        /// �x�[�X�Ăяo���K�v�Ȃ�
        /// </summary>
        protected void CombatLoop()
        {


        }

        /// <summary>
        /// ���[�h�ύX���ɉ������鏈��
        /// ��������ύX�������[�h��m���
        /// </summary>
        /// <param name="mode"></param>
        protected void ModeChangeEvent(int mode)
        {

        }

        /// <summary>
        /// �U���󂯂���̃C�x���g
        /// Enemy�͓G�̃Q�[���I�u�W�F�N�g
        /// </summary>
        public override void DamageEvent(bool isStun, GameObject enemy, int damage, bool back)
        {

            if (!isAggressive)
            {
                StartCombat(enemy);
            }

 

            //�A�[�}�[�񕜂����Z�b�g
            lastArmor = 0;

            //�q�b�g�X�g�b�v
            if (isStun)
            {
                _controller.SetForce(Vector2.zero);
            }

            //��s�G�͔�e��m���ɓ�����
            if (status._charaData._kind == EnemyStatus.KindofEnemy.Fly)
            {
                EscapeJudge();
            }


        }


        /// <summary>
        /// �ҋ@��Ԃłǂ̂悤�ȍs�����s��������������
        /// �����ł̓��[�v���Ȃ�������
        /// </summary>
        protected void PatrolAction()
        {
            //��
            if(status._charaData._kind != EnemyStatus.KindofEnemy.Fly)
            {
                //�X�s�[�h����Ȃ�
                if (status.patrolSpeed.x != 0)
                {
                    PatrolMove().Forget();
                }
                //�҂����Ԃ���Ȃ炭�邭������ς���
                else if (status.waitRes != 0)
                {
                    Wait().Forget();
                }
            }
            else
            {
                //�X�s�[�h����Ȃ�
                if (status.patrolSpeed != Vector2.zero)
                {
                    PatrolFly().Forget();
                }
                //�҂����Ԃ���Ȃ炭�邭������ς���
                else if (status.waitRes != 0)
                {
                    Wait().Forget();
                }
            }


        }

        /// <summary>
        /// �x���s�����Ƀ��[�v������������������
        /// �A�b�v�f�[�g����
        /// �x�[�X�Ăяo���K�v�Ȃ�
        /// </summary>
        protected void PatrolLoop()
        {

        }



        /// <summary>
        /// �^�[�Q�b�g���X�g����폜���ꂽ�G�l�~�[����������
        /// �����ăw�C�g���X�g���𒲐�
        /// �v���C���[�͂Ȃ񂩕ʂ̏�������Ă�����������
        /// ���ƓG�̎���ʒm���郁�\�b�h�Ƃ��Ă��g����
        /// </summary>
        /// <param name="deletEnemy"></param>
        public override void TargetListChange(int deleteEnemy,int deadID)
        {
            //�w�C�g���X�g����폜
            _hateList.RemoveAt(deleteEnemy);

            //�^�[�Q�b�g�������Ȃ�^�[�Q�b�g�ύX
            if(targetImfo.targetID == deadID)
            {
                targetImfo.targetNum = 99;
            }
            //�^�[�Q�b�g���폜�v�f����ɂ���Ȃ�
            //��O�ւ��炷��
            else if (targetImfo.targetNum > deleteEnemy)
            {
                targetImfo.targetNum--;
            }
        }


        #endregion


        #region �����Ƃ̘A�g�C�x���g�Q


        /// <summary>
        /// ID���m�F
        /// ������g���Ė����ƘA�g����̂ŏd���m�F�厖
        /// </summary>
        /// <returns></returns>
        public override int ReturnID()
        {
            return myID;
        }


        /// <summary>
        /// �������^�[�Q�b�g�����肵����
        /// �C�x���g�������Ă����΂�
        /// </summary>
        public override void CommandEvent(TargetingEvent _event,int level, int targetNum,int commanderID)
        {
            //�R�}���_�[���������A�܂��̓��x�����Ⴂ�Ȃ疳��
            if(commanderID == myID || (int)status.hateBehaivior.level > level)
            {
                return;
            }
        }


        /// <summary>
        /// �����Ƃ̘A�g�p
        /// �G�l�~�[�}�l�[�W���[����Ăяo��
        /// �����ɃC�x���g��ޓn����
        /// ���Ƃ̗͑͂������܂Ō�������C�x���g��΂�
        /// �_���[�W�C�x���g�Ƃ��������Ă�����
        /// </summary>
        public void AllyDeathEvent()
        {

        }


        #endregion


        #region �ėp�c�[��

        /// <summary>
        /// X��Y�̊ԂŃ����_���Ȓl��Ԃ�
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public int RandomValue(int X, int Y)
        {
            return UnityEngine.Random.Range(X, Y + 1);
        }


        /// <summary>
        /// �A�j���[�V�����̏I�������m
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        bool CheckEnd(string Name = null)
        {
            if (Name != null)
            {

                if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(Name))// || sAni.GetCurrentAnimatorStateInfo(0).IsName("OStand"))
                {   // �����ɓ��B�����normalizedTime��"Default"�̌o�ߎ��Ԃ��E���Ă��܂��̂ŁAResult�ɑJ�ڊ�������܂ł�return����B

                    return true;
                }
                if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                {   // �ҋ@���Ԃ���肽���Ȃ�΁A�����̒l��傫������B
                    //	Debug.Log("����2");
                    return true;
                }
                //AnimatorClipInfo[] clipInfo = sAni.GetCurrentAnimatorClipInfo(0);

                ////Debug.Log($"�A�j���I��");

                return false;
            }
            else
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                {   // �ҋ@���Ԃ���肽���Ȃ�΁A�����̒l��傫������B
                    //	Debug.Log("����2");
                    return true;
                }
                return false;
            }
        }


        /// <summary>
        /// n�b�҂��ď����Ƃ����悤�Ȃ̂�����
        /// </summary>
        /// <returns></returns>
        protected async UniTaskVoid WaitAction(float waitTime, Action _action)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: destroyCancellationToken);

            _action();
        }

        /// <summary>
        /// �U����Ԃ������łȂ����ŋ������ς��
		/// n�b�҂��ď����Ƃ����悤�Ȃ̂�����
        /// </summary>
        /// <returns></returns>
        protected async UniTaskVoid StateWaitAction(float waitTime, Action _action, bool agrAct)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: destroyCancellationToken);
            if (agrAct && !isAggressive)
            {
                return;
            }
            else if (!agrAct && isAggressive)
            {
                return;
            }
            _action();
        }





        #endregion


        #region�@�����ύX
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
            _controller.DefaultParameters.Gravity = -gravity;
        }

        #endregion


        #region �A�[�}�[�֘A


        /// <summary>
        /// �A�[�}�[�����Z�b�g
        /// ���ݒ��Ȃ�U��Ԃ�
        /// </summary>
        public override void ArmorReset()
        {
            nowArmor = status.Armor;

        }


        /// <summary>
        /// ��e��Ɏ��Ԍo�߂ŃA�[�}�[���񕜂���
        /// ����͑������ʂ̃��[�v�����ł�������������H
        /// </summary>
        async UniTaskVoid ArmorRecover()
        {

            //�O�b�҂�
            await UniTask.Delay(TimeSpan.FromSeconds(3f), cancellationToken: agrToken.Token);

            //�U����ԂłȂ��X�^�����Ă����Ȃ��Ȃ�
            if (_condition.CurrentState != CharacterStates.CharacterConditions.Stunned && _movement.CurrentState != CharacterStates.MovementStates.Attack)
            {
                //�܉�񕜂�����t����
                if (lastArmor >= 4)
                {
                    ArmorReset();
                    lastArmor = 0;
                }
                else
                {
                    //�܉�܂ł͂U���̈ꂸ��
                    nowArmor = nowArmor + status.Armor / 6 > status.Armor ? status.Armor : nowArmor + status.Armor / 6;
                    lastArmor++;
                }
            }

            ArmorRecover().Forget();


        }


        /// <summary>
        /// �U���H������Ƃ�
        /// �A�[�}�[�l�ɉ����ăC�x���g�Ƃ΂�
        /// </summary>
        public override MyWakeUp.StunnType ArmorControll(float shock, bool isDown, bool isBack)
        {

            MyWakeUp.StunnType result = 0;

            if (_movement.CurrentState != CharacterStates.MovementStates.Attack)
            {
                atV.aditionalArmor = 0;
            }
            if ((_movement.CurrentState == CharacterStates.MovementStates.GuardMove || _movement.CurrentState == CharacterStates.MovementStates.Guard) && isBack)
            {
                _guard.GuardEnd();
            }
            if (!isBack && (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove))
            {
                _guard.GuardHit();
                if (isDown)
                {
                    nowArmor -= ((shock * 2) * status.guardPower / 100) * 1.2f;

                }
                else
                {
                    nowArmor -= (shock * 2) * ((100 - status.guardPower) / 100);
                }
                if (nowArmor <= 0)
                {
                    _guard.GuardEnd();
                }
            }

            else
            {
                if (!isArmor || atV.aditionalArmor == 0)
                {
                    nowArmor -= shock;

                }
                else
                {
                    nowArmor -= (shock - atV.aditionalArmor) < 0 ? 0 : (shock - atV.aditionalArmor);
                    atV.aditionalArmor = (atV.aditionalArmor - shock) < 0 ? 0 : atV.aditionalArmor - shock;

                }
            }

            if (nowArmor <= 0)
            {


                if (isDown)
                {
                    result = (MyWakeUp.StunnType.Down);
                }
                else
                {

                    if (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove)
                    {
                        result = MyWakeUp.StunnType.GuardBreake;
                    }
                    //�p���B�͕ʔ���
                    else
                    {
                        result = MyWakeUp.StunnType.Falter;
                    }

                }
                isAtEnable = false;
            }
            else
            {
                result = MyWakeUp.StunnType.notStunned;
            }


            return result;
        }


        /// <summary>
        /// �W���X�K���ꂽ���A�[�}�[�l�ɉ����ăC�x���g�Ƃ΂�
        /// </summary>
        public override bool ParryArmorJudge()
        {

            nowArmor -= Mathf.Ceil(status.Armor * ((100 - atV.parryResist) / 100));

            if (nowArmor <= 0)
            {

                //	_wakeup.StartStunn(MyWakeUp.StunnType.Parried);
                return true;
            }
            else
            {
                Debug.Log($"����{nowArmor}");
                return false;
            }
        }


        /// <summary>
        /// �X�^���J�n
        /// �����ŐF�X�~�߂�邩��
        /// </summary>
        /// <param name="stunState"></param>
        public override void StartStun(MyWakeUp.StunnType stunState)
        {
            if (_movement.CurrentState == CharacterStates.MovementStates.Attack)
            {
                AttackEnd(true);
            }
            _wakeup.StartStunn(stunState);
        }



        /// <summary>
        /// ���݂̃X�^���󋵂�Ԃ�
        /// </summary>
        /// <returns></returns>
        public override int GetStunState()
        {
            return _wakeup.GetStunState();
        }


        /// <summary>
        /// �󒆂ōU�����󂯂����_�E�����邩�ǂ����̔��f
        /// </summary>
        /// <param name="stunnState"></param>
        /// <returns></returns>
        public override bool AirDownJudge(MyWakeUp.StunnType stunnState)
        {
            if (status._charaData._kind != EnemyStatus.KindofEnemy.Fly)
            {
                if (stunnState == MyWakeUp.StunnType.Falter)
                {
                    return true;
                    //������΂�����
                }
                else if (stunnState == MyWakeUp.StunnType.notStunned)
                {
                    //�U�����łȂ��Ȃ����_�E��
                    if (!AttackCheck())
                    {
                        return true;
                        //������΂�����
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// ���S�A�j���[�V�������Đ�����
        /// �������Ŏ��ʂ̂����ʂɎ��ʂ̂�
		/// �G�l�~�[�Ɋւ��Ă͂��̎��_�Ŏ��S�����Ƃ݂Ȃ��ēG��񂩂疕�������肷��
        /// </summary>
        /// <param name="stunState"></param>
        public override void DeadMotionStart(MyWakeUp.StunnType stunState)
        {
            //�̗̓[���̎��_�Ŋ��m����Ȃ����C���[��
            this.gameObject.layer = 15;

            EnemyManager.instance.Die(myID,this);

            //�^�[�Q�b�g�ł͂Ȃ��Ȃ�
            if (SManager.instance.target == this.gameObject)
            {
                SManager.instance.target = null;
            }

            //�^�[�Q�b�g���X�g���玩�����폜
            SManager.instance.RemoveEnemy(this.gameObject);

            //�G�t�F�N�g��������
            TargetEffectCon(1);



            //���ԓG�̓_�E�����Ď���
            if ((status._charaData._kind == EnemyStatus.KindofEnemy.Fly))
            {
                _controller.SetHorizontalForce(0);
                stunState = MyWakeUp.StunnType.BlowDie;
            }

            _wakeup.StartStunn(stunState);

        }




        #endregion

        #region �W�����v�g���K�[
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {



            if (isAggressive && collision.tag == EnemyManager.instance.JumpTag && _controller.State.IsGrounded)
            {
                //�W�����v���������Ă�Ȃ�
                if (collision.gameObject.MMGetComponentNoAlloc<JumpTrigger>().jumpDirection == transform.localScale.x)
                {
                    JumpAct();
                    Debug.Log($"sss");
                }
            }

        }
        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            if (_condition.CurrentState != CharacterStates.CharacterConditions.Dead)
            {

                if (isAggressive && collision.tag == EnemyManager.instance.JumpTag && _controller.State.IsGrounded)
                {
                    if (collision.gameObject.MMGetComponentNoAlloc<JumpTrigger>().jumpDirection == transform.localScale.x)
                    {
                        JumpAct();
                        Debug.Log($"dfggrferfer");
                    }
                }
            }
        }

        #endregion


        #region �f�[�^�Z�b�g

        /// <summary>
        /// �W�����v�͂Ȃǂ�ϐ��ɔ��f���Ă���
        /// </summary>
        /// <param name="status"></param>
        protected void ParameterSet(EnemyStatus status)
        {
            ///<summary>
            ///�@���X�g
            /// </summary>
            /// 
            _characterHorizontalMovement.FlipCharacterToFaceDirection = false;

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
            if (status._charaData._kind != EnemyStatus.KindofEnemy.Fly)
            {
                GravitySet(status.firstGravity);
                #region
                /*
				 		/// the speed of the character when it's walking
		[Tooltip("the speed of the character when it's walking")]
		public float WalkSpeed = 6f;
		/// the multiplier to apply to the horizontal movement
		//�@�ǂݎ���p�B�R�[�h����ς��Ă�
		[MMReadOnly]
		[Tooltip("���������̈ړ��ɓK�p����{��")]
		public float MovementSpeedMultiplier = 1f;
		/// the multiplier to apply to the horizontal movement, dedicated to abilities
		[MMReadOnly]
		[Tooltip("���������̈ړ��ɓK�p����{���ŁA�A�r���e�B�ɓ������Ă��܂��B")]
		public float AbilityMovementSpeedMultiplier = 1f;
        /// the multiplier to apply when pushing
        [MMReadOnly]
		[Tooltip("the multiplier to apply when pushing")]
		public float PushSpeedMultiplier = 1f;
        /// the multiplier that gets set and applied by CharacterSpeed
        [MMReadOnly]
        [Tooltip("the multiplier that gets set and applied by CharacterSpeed")]
        public float StateSpeedMultiplier = 1f;
        /// if this is true, the character will automatically flip to face its movement direction
        [Tooltip("if this is true, the character will automatically flip to face its movement direction")]
        public bool FlipCharacterToFaceDirection = true;


        /// the current horizontal movement force
		public float HorizontalMovementForce { get { return _horizontalMovementForce; }}
        /// if this is true, movement will be forbidden (as well as flip)
        public bool MovementForbidden { get; set; }

        [Header("Input")]

		/// if this is true, will get input from an input source, otherwise you'll have to set it via SetHorizontalMove()
		//  ���ꂪ�^�̏ꍇ�A���̓\�[�X����̓��͂��擾���܂��B�����łȂ��ꍇ�́ASetHorizontalMove() �Őݒ肷��K�v������܂��B
		[Tooltip("if this is true, will get input from an input source, otherwise you'll have to set it via SetHorizontalMove()")]
		public bool ReadInput = true;
		/// if this is true, no acceleration will be applied to the movement, which will instantly be full speed (think Megaman movement). Attention : a character with instant acceleration won't be able to get knockbacked on the x axis as a regular character would, it's a tradeoff
		/// ���ꂪ�^�̏ꍇ�A�����x�͓K�p���ꂸ�A�u���ɑS���͂ƂȂ�܂��i���b�N�}���̓�����z�����Ă��������j�B
		/// ���ӁF�u�ԓI�ȉ��������L�����N�^�[�́A�ʏ�̃L�����N�^�[�̂悤��X���Ńm�b�N�o�b�N���󂯂邱�Ƃ͂ł��܂���B
		[Tooltip("if this is true, no acceleration will be applied to the movement, which will instantly be full speed (think Megaman movement). Attention : a character with instant acceleration won't be able to get knockbacked on the x axis as a regular character would, it's a tradeoff")]
		public bool InstantAcceleration = false;
		/// the threshold after which input is considered (usually 0.1f to eliminate small joystick noise)
		/// ���͂��l������臒l�i�����ȃW���C�X�e�B�b�N�m�C�Y���������邽�ߒʏ�0.1f�j(���m���Ȃ��l���m�F)
		[Tooltip("the threshold after which input is considered (usually 0.1f to eliminate small joystick noise)")]
		public float InputThreshold = 0.1f;
        /// how much air control the player has
        [Range(0f, 1f)]
		[Tooltip("how much air control the player has")]
		public float AirControl = 1f;
		/// whether or not the player can flip in the air
		[Tooltip("whether or not the player can flip in the air")]
		public bool AllowFlipInTheAir = true;
		/// whether or not this ability should keep taking care of horizontal movement after death
		[Tooltip("whether or not this ability should keep taking care of horizontal movement after death")]
		public bool ActiveAfterDeath = false;

        [Header("Touching the Ground")]
		/// the MMFeedbacks to play when the character hits the ground
		/// �L�����N�^�[���n�ʂɏՓ˂����Ƃ��ɍĐ������MMFeedbacks
		[Tooltip("the MMFeedbacks to play when the character hits the ground")]
		public MMFeedbacks TouchTheGroundFeedback;
		/// the duration (in seconds) during which the character has to be airborne before a feedback can be played when touching the ground
		/// �L�����N�^���󒆂ɂ���ԁA�n�ʂɐG��Ă��t�B�[�h�o�b�N���Đ������܂ł̎��ԁi�b�j�ł��B
		[Tooltip("the duration (in seconds) during which the character has to be airborne before a feedback can be played when touching the ground")]
		public float MinimumAirTimeBeforeFeedback = 0.2f;

        [Header("Walls")]
		/// Whether or not the state should be reset to Idle when colliding laterally with a wall
		/// �ǂɉ�����Փ˂����Ƃ��ɏ�Ԃ�Idle�ɖ߂����ǂ���
		[Tooltip("Whether or not the state should be reset to Idle when colliding laterally with a wall")]
		public bool StopWalkingWhenCollidingWithAWall = false;
				 */
                #endregion
                _characterHorizontalMovement.WalkSpeed = status.patrolSpeed.x;
                if (_characterRun != null)
                {
                    _characterRun.RunSpeed = status.combatSpeed.x;
                }

            }
            else
            {
                GravitySet(status.firstGravity);
                if (_flying != null)
                {
                    //Debug.Log($"����������{_flying.nFlySpeed == null}{status.patrolSpeed.x}");
                    _flying.SpeedSet(status.patrolSpeed.x, status.patrolSpeed.y, false);
                    _flying.SpeedSet(status.combatSpeed.x, status.combatSpeed.y, true);
                    //	_flying.FastFly(false, false);
                    //Debug.Log($"hhhhd{_flying.FlySpeed.x}");
                }
                else
                {
                    Debug.Log("����������");
                }
            }

            if (_rolling != null)
            {
                #region
                /*
				         /// ���b�]����
		[Tooltip("���[�����O����")]
        public float RollDuration = 0.5f;
        /// the speed of the roll (a multiplier of the regular walk speed)
        [Tooltip("�]���鑬�����ʏ�̕��������̉��{��")]
        public float RollSpeed = 3f;
        /// if this is true, horizontal input won't be read, and the character won't be able to change direction during a roll
        [Tooltip("true�̏ꍇ�A���������̓��͓͂ǂݍ��܂ꂸ�A���[�����ɕ�����ς��邱�Ƃ͂ł��܂���B")]
        public bool BlockHorizontalInput = false;
        /// if this is true, no damage will be applied during the roll, and the character will be able to go through enemies
        /// //���̃p�����[�^�[��������鏈��������Ζ��G�������킩��
        [Tooltip("true�̏ꍇ�A���[�����Ƀ_���[�W���^����ꂸ�A�G���X���[�ł���悤�ɂȂ�܂��B")]
        public bool PreventDamageCollisionsDuringRoll = false;

        //����
        [Header("Direction")]

        /// the roll's aim properties
        [Tooltip("the roll's aim properties")]
        public MMAim Aim;
        /// the minimum amount of input required to apply a direction to the roll
        [Tooltip(" ���[���ɕ�����^���邽�߂ɕK�v�ȍŏ����̓��͗�")]
        public float MinimumInputThreshold = 0.1f;
        /// if this is true, the character will flip when rolling and facing the roll's opposite direction
        [Tooltip("���ꂪ�^�Ȃ�A�L�����N�^�[�̓��[�����ɔ��]���A���[���̔��Α��������܂��B")]
        public bool FlipCharacterIfNeeded = true;

        //����R���[�`���Ȃ�
        public enum SuccessiveRollsResetMethods { Grounded, Time }

        [Header("Cooldown")]
        /// the duration of the cooldown between 2 rolls (in seconds)
        [Tooltip("���̃��[�����O�܂łɕK�v�Ȏ���")]
        public float RollCooldown = 1f;

        [Header("Uses")]
        /// whether or not rolls can be performed infinitely
        [Tooltip("�����Ƀ��[�����O�ł��邩")]
        public bool LimitedRolls = false;
        /// the amount of successive rolls a character can perform, only if rolls are not infinite
        [Tooltip("the amount of successive rolls a character can perform, only if rolls are not infinite")]
        [MMCondition("LimitedRolls", true)]
        public int SuccessiveRollsAmount = 1;
        /// the amount of rollss left (runtime value only), only if rolls are not infinite
        [Tooltip("���[�����O�̎c���")]
        [MMCondition("LimitedRolls", true)]
        [MMReadOnly]
        public int SuccessiveRollsLeft = 1;
        /// when in time reset mode, the duration, in seconds, after which the amount of rolls left gets reset, only if rolls are not infinite
        [Tooltip("when in time reset mode, the duration, in seconds, after which the amount of rolls left gets reset, only if rolls are not infinite")]
        [MMCondition("LimitedRolls", true)]
        public float SuccessiveRollsResetDuration = 2f;

				*/
                #endregion

                _rolling.RollDuration = status.avoidRes;
                _rolling.RollSpeed = status.avoidSpeed;
                _rolling.BlockHorizontalInput = true;
                _rolling.PreventDamageCollisionsDuringRoll = true;
                _rolling.RollCooldown = status.avoidCool;


            }

            if (_jump != null)
            {
                #region
                /*
                  		/// the maximum number of jumps allowed (0 : no jump, 1 : normal jump, 2 : double jump, etc...)
		[Tooltip("the maximum number of jumps allowed (0 : no jump, 1 : normal jump, 2 : double jump, etc...)")]
		public int NumberOfJumps = 2;
		/// defines how high the character can jump
		[Tooltip("defines how high the character can jump")]
		public float JumpHeight = 3.025f;
		/// basic rules for jumps : where can the player jump ?
		[Tooltip("basic rules for jumps : where can the player jump ?")]
		public JumpBehavior JumpRestrictions = JumpBehavior.CanJumpAnywhere;
		/// if this is true, camera offset will be reset on jump
		[Tooltip("if this is true, camera offset will be reset on jump")]
		public bool ResetCameraOffsetOnJump = false;
		/// if this is true, this character can jump down one way platforms by doing down + jump
		[Tooltip("if this is true, this character can jump down one way platforms by doing down + jump")]
		public bool CanJumpDownOneWayPlatforms = true;

		[Header("Proportional jumps")]

		/// if true, the jump duration/height will be proportional to the duration of the button's press
		[Tooltip("if true, the jump duration/height will be proportional to the duration of the button's press")]
		public bool JumpIsProportionalToThePressTime = true;
		/// the minimum time in the air allowed when jumping - this is used for pressure controlled jumps
		[Tooltip("the minimum time in the air allowed when jumping - this is used for pressure controlled jumps")]
		public float JumpMinimumAirTime = 0.1f;
		/// the amount by which we'll modify the current speed when the jump button gets released
		[Tooltip("the amount by which we'll modify the current speed when the jump button gets released")]
		public float JumpReleaseForceFactor = 2f;

		[Header("Quality of Life")]

		/// a timeframe during which, after leaving the ground, the character can still trigger a jump
		[Tooltip("a timeframe during which, after leaving the ground, the character can still trigger a jump")]
		public float CoyoteTime = 0f;

		/// if the character lands, and the jump button's been pressed during that InputBufferDuration, a new jump will be triggered 
		[Tooltip("�L�����N�^�[�����n���A����InputBufferDuration�̊ԂɃW�����v�{�^���������ꂽ�ꍇ�A�V�����W�����v���J�n����܂��B")]
		public float InputBufferDuration = 0f;

		[Header("Collisions")]

		/// duration (in seconds) we need to disable collisions when jumping down a 1 way platform
		[Tooltip("duration (in seconds) we need to disable collisions when jumping down a 1 way platform")]
		public float OneWayPlatformsJumpCollisionOffDuration = 0.3f;
		/// duration (in seconds) we need to disable collisions when jumping off a moving platform
		[Tooltip("duration (in seconds) we need to disable collisions when jumping off a moving platform")]
		public float MovingPlatformsJumpCollisionOffDuration = 0.05f;

		[Header("Air Jump")]

		/// the MMFeedbacks to play when jumping in the air
		[Tooltip("the MMFeedbacks to play when jumping in the air")]
		public MMFeedbacks AirJumpFeedbacks;

		/// the number of jumps left to the character
		[MMReadOnly]
		[Tooltip("the number of jumps left to the character")]
		public int NumberOfJumpsLeft;

		/// whether or not the jump happened this frame
		public bool JumpHappenedThisFrame { get; set; }
		/// whether or not the jump can be stopped
		public bool CanJumpStop { get; set; }
                 
				 */
                #endregion
                _jump.CoyoteTime = status.jumpCool;
                _jump.JumpHeight = status.jumpRes;
                _jump.NumberOfJumps = status.jumpLimit;

            }
            maxHp = status.maxHp;

            //���̕ӂ���Ȃ�����
            //�ʂɎ��������{���ő��삷��΂悭�ˁH

            //�@�������h��́B�̗͂ŏオ��
            Def = status.Def;
            //�h�˖h��B�ؗ͂ŏオ��
            pierDef = status.pierDef;
            //�Ō��h��A�Z�ʂŏオ��
            strDef = status.strDef;
            //�_���h��A�؂ƌ����ŏオ��B
            holyDef = status.holyDef;
            //�Ŗh��B�����ŏオ��
            darkDef = status.darkDef;
            //���h��B�����Ɛ����ŏオ��
            fireDef = 70;
            //���h��B�����Ǝ��v�ŏオ��B
            thunderDef = 70;

            _health.InitialHealth = (int)maxHp;
            _health.MaximumHealth = (int)maxHp;
            _health.CurrentHealth = _health.InitialHealth;
            //	Debug.Log($"tanomu{_health.CurrentHealth}");
            nowArmor = status.Armor;
        }


        /// <summary>
        /// ���t���[���g�p
        /// �����̃f�[�^�𑗐M
        /// </summary>
        /// <param name="num"></param>
        public override void TargetDataUpdate(int num)
        {
            SManager.instance._targetList[num]._condition.hpRatio =_health.CurrentHealth / status.maxHp;

            SManager.instance._targetList[num]._condition.targetPosition = myPosition;
            SManager.instance._targetList[num]._condition.hpRatio = _health.CurrentHealth / status.maxHp;
            SManager.instance._targetList[num]._condition.hpNum = _health.CurrentHealth;
            SManager.instance._targetList[num]._condition.isBuffOn = false;
            SManager.instance._targetList[num]._condition.isDebuffOn = false;
        }

        /// <summary>
        /// ����͍ŏ��̈�񂫂�
        /// �^�[�Q�b�g�f�[�^�̒ǉ�
        /// </summary>
        /// <param name="data"></param>
        public override void  TargetDataAdd(int newID)
        {

            TargetData imfo = new TargetData();

            imfo._baseData = status._charaData;

            imfo._condition.targetPosition = myPosition;
            imfo._condition.hpRatio = _health.CurrentHealth / status.maxHp;
            imfo._condition.hpNum = _health.CurrentHealth;
            imfo._condition.isBuffOn = false;
            imfo._condition.isDebuffOn = false;

            imfo.targetObj = this.gameObject;

            imfo.targetID = newID;

            //ID���蓖��
            myID = newID;

            SManager.instance._targetList.Add(imfo);
        }




        #endregion



        #region�@�X�e�[�^�X�֘A

        public void HPReset()
        {
            _health.SetHealth((int)status.maxHp, this.gameObject);
        }



        /// <summary>
        /// �_���[�W�v�Z
        /// ����AtV�ʂ��K�v����H
        /// </summary>
        /// <param name="isFriend">�^�Ȃ疡��</param>
        public override void DamageCalc()
        {
            //GManager.instance.isDamage = true;
            //status.hitLimmit--;
            //mValue�̓��[�V�����l


            _damage._attackData._attackType = atV.mainElement;
            _damage._attackData.phyType = atV.phyElement;

            if (status.phyAtk > 0)
            {
                _damage._attackData.phyAtk = status.phyAtk * attackFactor;

                if (atV.shock >= 40)
                {
                    _damage._attackData.isHeavy = true;
                }
                else
                {
                    _damage._attackData.isHeavy = false;
                }

            }
            //�_��
            if (status.holyAtk > 0)
            {
                _damage._attackData.holyAtk = status.holyAtk * holyATFactor;

            }
            //��
            if (status.darkAtk > 0)
            {
                _damage._attackData.darkAtk = status.darkAtk * darkATFactor;

            }
            //��
            if (status.fireAtk > 0)
            {
                _damage._attackData.fireAtk = status.fireAtk * fireATFactor;

            }
            //��
            if (status.thunderAtk > 0)
            {
                _damage._attackData.thunderAtk = status.thunderAtk * thunderATFactor;

            }
            _damage._attackData.shock = atV.shock;
            _damage._attackData.mValue = atV.mValue;
            _damage._attackData.disParry = atV.disParry;
            _damage._attackData.attackBuff = attackBuff;
            //damage = Mathf.Floor(damage * attackBuff);
            _damage._attackData._parryResist = atV.parryResist;
            _damage._attackData.isBlow = atV.isBlow;

            _damage._attackData.isLight = atV.isLight;
            _damage._attackData.blowPower.Set(atV.blowPower.x, atV.blowPower.y);
        }

        /// <summary>
        /// �����̃_���[�W���t���O���ĂĂ�����̖h��͂������Ă������
        /// </summary>
        public override void DefCalc()
        {


            _health.InitialHealth = (int)maxHp;
            _health._defData.Def = Def;
            _health._defData.pierDef = pierDef;
            _health._defData.strDef = strDef;
            _health._defData.fireDef = fireDef;
            _health._defData.holyDef = holyDef;
            _health._defData.darkDef = darkDef;

            _health._defData.phyCut = status.phyCut;
            _health._defData.fireCut = status.fireCut;
            _health._defData.holyCut = status.holyCut;
            _health._defData.darkCut = status.darkCut;

            _health._defData.guardPower = status.guardPower;


            if (_condition.CurrentState == CharacterStates.CharacterConditions.Stunned)
            {
                _health._defData.isDangerous = true;

            }
            _health._defData.attackNow = _movement.CurrentState == CharacterStates.MovementStates.Attack ? true : false;

        }

        /// <summary>
        /// ���݃K�[�h�����ǂ�����m�点��
        /// </summary>
        public override bool GuardReport()
        {
            return _movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove;
        }

        /// <summary>
        /// �o�t�̐��l��^����
        /// �e�ۂ���Ă�
        /// </summary>
        public override void BuffCalc(FireBullet _fire)
        {
            _fire.attackFactor = attackFactor;
            _fire.fireATFactor = fireATFactor;
            _fire.thunderATFactor = thunderATFactor;
            _fire.darkATFactor = darkATFactor;
            _fire.holyATFactor = holyATFactor;
        }

        public override void ParryStart(bool isBreake)
        {
            _parry.ParryStart(isBreake);
            _guard.GuardEnd();

            //�A�[�}�[��
            nowArmor = nowArmor + (status.Armor / 3) > status.Armor ? status.Armor : nowArmor + (status.Armor / 3);
        }

        /// <summary>
        /// ���ʂƂ��ɉ������鏈��
        /// �������^�[�Q�b�g���X�g����폜������c�͎��S���[�V�����J�n�ł���Ă邩
        /// </summary>
        public override void Die()
        {


            if (SManager.instance.target == this.gameObject)
            {
                SManager.instance.target = null;

            }
        }


        #endregion



        ///<sammary>
        /// �@�U���֘A�̏���
        /// �@�U���ɕK�v�Ȃ���
        /// �@�܂��U���̃p�����[�^��ݒ�B�ԍ����w�肵�U���J�n
        /// �@�����U���Ȃǂɂ͈����t���̍U���I�������ݒ�A�j���C�x���g������B����ŃA�[�}�[���L���ɂ���
        /// �@�A�j���C�x���g�ɂ͂ق��ɃG�t�F�N�g���o����Ɖ����o���������B
        /// �@�����čU���I�������𖞂����Ă邩�A���邢�̓A�j���I��������ōU���I���m�F
        /// �@�����ĕ⑫�s������Ȃ��邪�A�⑫�s���͌����ɂ͍U�������Ɛ؂藣���čs���B
        /// �@�⑫�s���t���O�͉��p����Β����Ƃ��G�ɂ������
        /// �@�U�����Ƃɂ���N�[���^�C�����I��������܂��U������悤��
        /// �@���p�n�̍U���̉r���͍U������Ȃ��U���ɂ��ăR���{�Ŗ��p������
        /// �@�U���l��isShoot�Ƃ�����邩
        ///</sammary>
        #region �U���֘A�̏���

        




        /// <summary>
        /// �U���O�̏�����
        /// </summary>
        public void AttackPrepare()
        {
            //_movement.CurrentState != CharacterStates.MovementStates.Attack = true;
            atV.coolTime = status.atValue[attackNumber].coolTime;
            atV.isBlow = status.atValue[attackNumber].isBlow;
            atV.mValue = status.atValue[attackNumber].mValue;
            atV.aditionalArmor = status.atValue[attackNumber].aditionalArmor;
            atV.isLight = status.atValue[attackNumber].isLight;
            atV.disParry = status.atValue[attackNumber].disParry;
            atV.blowPower = status.atValue[attackNumber].blowPower;
            atV.shock = status.atValue[attackNumber].shock;
            atV.type = status.atValue[attackNumber].type;
            atV.isCombo = status.atValue[attackNumber].isCombo;
            atV.escapePercentage = status.atValue[attackNumber].escapePercentage;
            atV.parryResist = status.atValue[attackNumber].parryResist;
            //	atV.attackEffect = status.atValue[attackNumber].attackEffect;
            atV.suppleNumber = status.atValue[attackNumber].suppleNumber;

            //�ːi�p�̏�����
            atV._moveDuration = status.atValue[attackNumber]._moveDuration;
            atV._moveDistance = status.atValue[attackNumber]._moveDistance;
            atV._contactType = status.atValue[attackNumber]._contactType;
            atV.fallAttack = status.atValue[attackNumber].fallAttack;
            atV.startMoveTime = status.atValue[attackNumber].startMoveTime;
            atV.lockAttack = status.atValue[attackNumber].lockAttack;
            atV.backAttack = status.atValue[attackNumber].backAttack;

            atV.mainElement = status.atValue[attackNumber].mainElement;
            atV.phyElement = status.atValue[attackNumber].phyElement;
            atV.motionType = status.atValue[attackNumber].motionType;
            atV.EffectLevel = status.atValue[attackNumber].EffectLevel;
            atV.endCondition = status.atValue[attackNumber].endCondition;

            //�q�b�g������֘A
            _damage._attackData._hitLimit = status.atValue[attackNumber]._hitLimit;
            _damage.CollidRestoreResset();

            _health._superArumor = status.atValue[attackNumber].superArmor;
            _health._guardAttack = status.atValue[attackNumber].guardAttack;



            int adType = 0;

            if (atV.disParry)
            {
                adType = 1;
            }

            _atEf.EffectPrepare(status.atValue[attackNumber].EffectLevel, adType, status.atValue[attackNumber].mainElement, status.atValue[attackNumber].motionType);
        }


        /// <summary>
        /// �e�ۂ𔭎˂���
        /// �r���A�j����ɂ���ł悭�ˁH
        /// </summary>
        /// <param name="i"></param>
        /// <param name="random"></param>
        public void ActionFire(int i, float random = 0.0f)
        {
            //�����_���ɓ���Ă��������Ǖ��ʂɓ���Ă�����
            i = i < 0 ? 0 : i;
            i = i > status.enemyFire.Count - 1 ? status.enemyFire.Count : i;


            waitCast += _controller.DeltaTime;
            if (waitCast >= status.enemyFire[i].castTime)
                if (random != 0)
                {
                    firePosition.position.Set
                        (firePosition.position.x + random, firePosition.position.y + random, firePosition.position.z);//�e������
                }
            Transform goFire = firePosition;

            for (int x = 0; x >= status.enemyFire[i].bulletNumber; x++)
            {
                Addressables.InstantiateAsync(status.enemyFire[i].effects, goFire.position, Quaternion.identity);//.Result;//�����ʒu��Player
            }
            //go.GetComponent<EnemyFireBullet>().ownwer = transform;

        }


        /// <summary>
        /// �U��
        /// ���C�v��
        /// isShoot = true�̎��̏�������H�B����Ȃ��H�܂肽�܂������Ă�������
        /// �����@�[�X�ŋt������
        /// </summary>
        /// <param name="select">�U���ԍ����w�肷�邩�ǂ���</param>
        /// <param name="number">�w�肷��U��</param>
        /// <param name="reverse">�t�ɐU��Ԃ��čU��</param>
        public void Attack( int number = 0,bool isUseSkill = true)
        {

            //�N�[���^�C���������Ăē�����Ȃ�
            if (!isAtEnable || !isMovable || number > status.atValue.Count || number <= 0)
            {//Debug.Log($"��񂫂�{isAtEnable}{isMovable}{ number > status.atValue.Count}{number <= 0}");
                return;
            }

            if (isUseSkill)
            {
                useSkillNum = number;
            }


            //���b�N������
            isMovable = false;
            isAtEnable = false;

            if (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove)
            {
                //guardJudge = false;
                _guard.GuardEnd();
            }



            //�G�̕�����
            if (!atV.backAttack)
            {
                NormalFlip(direction);

            }
            else
            {

                NormalFlip(-direction);

            }


            AttackPrepare();

            //�A�j���[�V�����̗p��
            //�U�������̓A�j���[�V�����ɔC���Ă�̂ł����ōU���J�n
            _attack.AttackTrigger(number);



            if (status._charaData._kind == EnemyStatus.KindofEnemy.Fly)
            {
                _flying.SetHorizontalMove(0);
                _flying.SetVerticalMove(0);
            }
            else
            {
                _characterHorizontalMovement.SetHorizontalMove(0);
            }

            //��~
            _controller.SetForce(Vector2.zero);

            //�������ړ��͈͓��ŁA���b�N�I������Ȃ狗����ς���
            float moveDistance = (atV.lockAttack && Mathf.Abs(distance.x) < atV._moveDistance) ? distance.x : atV._moveDistance;

            //�U���̓��ݍ��݈ړ��J�n
            _rush.RushStart(atV._moveDuration, moveDistance * direction, atV._contactType, atV.fallAttack, atV.startMoveTime, atV.backAttack);


            //�I�������̊Ď����n�߂�
            WaitAttack(atV.endCondition).Forget();

            //    ExecuteAttack().Forget();



        }


        //�e�ۏ����̗�
        //���[�h����
        #region
        /*
		 		/// <summary>
		/// for���ł͂Ȃ���	bcount�𒴂���܂�useMagic���^�Ȃ̂Ŕ�����������
		/// �e�ۂ���郁�\�b�h
		/// </summary>
		/// <param name="hRandom"></param>
		/// <param name="vRandom"></param>
		 void MagicUse(int hRandom, int vRandom)
		{

			if (!fireStart || delayNow)
			{
				return;
			}
			bCount += 1;
			Debug.Log("���Ă�");
			//	Debug.Log($"�n�U�[�h{SManager.instance.useMagic.name}�W�I{SManager.instance.target}����{sister.nowMove}");
			//���@�g�p��MagicUse�ł��e�ې������łȂ����

			//�e�̔��˂Ƃ����������ʒu
			Vector3 goFire = sb.firePosition.position;
			//�e���ꔭ�ڂȂ�
			if (bCount == 1)
			{
				//   MyInstantiate(SManager.instance.useMagic.fireEffect, goFire, Quaternion.identity).Forget();
				//Addressables.InstantiateAsync(SManager.instance.useMagic.fireEffect, goFire, Quaternion.identity);
				if (SManager.instance.useMagic.fireType == SisMagic.FIREBULLET.RAIN)
				{
					//�R�Ȃ�̒e���őł������Ƃ��Ƃ��ˏo�p�x���߂ꂽ�炢������
					//�ʒu�������_���ɂ���Ίp�x�͂ǂ��ł�������������
					SManager.instance.useMagic.angle = GetAim(sb.firePosition.position, SManager.instance.target.transform.position);

				}
				sb.mp -= SManager.instance.useMagic.useMP;
			}

			//�G�̈ʒu�ɃT�[�`�U������Ƃ�
			if (SManager.instance.useMagic.isChaice)
			{
				goFire.Set(SManager.instance.target.myPosition.x, SManager.instance.target.myPosition.y, SManager.instance.target.myPosition.y);

			}
			//�����_���Ȉʒu�ɔ�������Ƃ�
			else if (hRandom != 0 || vRandom != 0)
			{
				//Transform goFire = firePosition;


				float xRandom = 0;
				float yRandom = 0;
				if (hRandom != 0)
				{

					xRandom = RandomValue(-hRandom, hRandom);

				}
				if (vRandom != 0)
				{
					yRandom = RandomValue(-vRandom, vRandom);
				}
				//	xRandom = RandomValue(RandomValue(-random,0),RandomValue(0, random));
				//	yRandom = RandomValue(RandomValue(-random, 0), RandomValue(0, random));

				goFire = new Vector3(sb.firePosition.position.x + xRandom, sb.firePosition.position.y + yRandom, 0);//�e������

			}
			//Debug.Log($"���@�̖��O5{SManager.instance.useMagic.hiraganaName}");
			//    MyInstantiate(SManager.instance.useMagic.effects, goFire, Quaternion.identity).Forget();//.Result;//�����ʒu��Player
			//�����ɔ�������e�ۂ̈ꔭ�ڂȂ�
			if (SManager.instance.useMagic.delayTime == 0 || bCount == 1)
			{
				Debug.Log("aaa");
                UnityEngine.Object h = Addressables.LoadAssetAsync<UnityEngine.Object>(SManager.instance.useMagic.effects).Result;
				 GameObject t =  Instantiate(h, goFire, Quaternion.Euler(SManager.instance.useMagic.startRotation)) as GameObject;//.MMGetComponentNoAlloc<FireBullet>().InitializedBullet(this.gameObject, SManager.instance.target);
				t.MMGetComponentNoAlloc<FireBullet>().InitializedBullet(this.gameObject,SManager.instance.target);
			}
			//2���ڈȍ~�̒e�Ő���������Ȃ��Ȃ�
			else if (bCount > 1 && !delayNow)
			{
				DelayInstantiate(SManager.instance.useMagic.effects, goFire, Quaternion.Euler(SManager.instance.useMagic.startRotation)).Forget();
			}
			//�e�ۂ𐶐����I�������
			if (bCount >= SManager.instance.useMagic.bulletNumber)
			{
				//Debug.Log($"�e���y�X�g{SManager.instance.useMagic.name}�W�I{SManager.instance.target}����{sister.nowMove}");
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				//disEnable = true;
				coolTime = SManager.instance.useMagic.coolTime;
				bCount = 0;
				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);

				actionNum = 0;

				SManager.instance.useMagic = null;
				fireStart = false;
				SManager.instance.target.GetComponent<EnemyAIBase>().TargetEffectCon(3);
			}
				
		}


		 */
        #endregion








        /// <summary>
        /// �U���I���̏��������A�[�}�[��ݒ�
        /// �A�j���[�V�����C�x���g
        /// ��������A�[�}�[���肪�J�n
        /// </summary>
        public void ConditionAttack()
        {
            //���̃i���o�[�ɂ���ďd�͂�������Ƃ����H

            isArmor = true;
        }

        /// <summary>
        /// �⑫�s���̋N�����s�����\�b�h
        /// </summary>
        /// <param name="useNumber"></param>
        public void SActTrigger(int useNumber)
        {
            suppleNumber = atV.suppleNumber;
            _condition.ChangeState(CharacterStates.CharacterConditions.Moving);
        }

        /// <summary>
        /// �ǂ̃��[�V��������邩�ǂ���
        /// </summary>
        /// <param name="number"></param>
        public void SetAttackNumber(int number)
        {
            number = number > status.serectableNumber.Count - 1 ? status.serectableNumber.Count - 1 : number;
            number = number < 0 ? 0 : number;
            attackNumber = status.serectableNumber[number];
        }

        /// <summary>
        /// �N�[���^�C���̂�U���I��������҂Ԏ��̍U����҂�
        /// </summary>
        public async UniTaskVoid WaitAttack(EnemyValue.AttackEndCondition endCon)
        {
            int result = 0;

            if (endCon == EnemyValue.AttackEndCondition.���[�V�����I��)
            {

                string name = $"Attack{attackNumber}";
                
                // ���[�V�����I�����X�^���ŋ���������ҋ@
                result = await UniTask.WhenAny(UniTask.WaitUntil(() => CheckEnd(name), cancellationToken: destroyCancellationToken),
                 UniTask.WaitUntil(() => _movement.CurrentState != CharacterStates.MovementStates.Attack, cancellationToken: destroyCancellationToken));

                //����ɃA�j���[�V�������I�����ďI������Ȃ瑱����
                if (result != 0)
                {
                    return;
                }
            }

            else if (endCon == EnemyValue.AttackEndCondition.���n�����Ԍo��)
            {
                result = await UniTask.WhenAny(UniTask.WaitUntil(() => _controller.State.IsGrounded, cancellationToken: destroyCancellationToken),
                    UniTask.Delay(TimeSpan.FromSeconds(4f), cancellationToken: destroyCancellationToken),
                    UniTask.WaitUntil(() => _movement.CurrentState != CharacterStates.MovementStates.Attack, cancellationToken: destroyCancellationToken));

                //����ɏ����𖞂������Ȃ���
                if (result == 2)
                {
                    return;
                }
            }
            else if (endCon == EnemyValue.AttackEndCondition.�ړ������Ԍo��)
            {
                float stPosi = myPosition.x;
                float moveDis = atV._moveDistance;

                result = await UniTask.WhenAny(UniTask.WaitUntil(() => Math.Abs(myPosition.x - stPosi) >= moveDis, cancellationToken: destroyCancellationToken),
                UniTask.Delay(TimeSpan.FromSeconds(4f), cancellationToken: destroyCancellationToken),
                UniTask.WaitUntil(() => _movement.CurrentState != CharacterStates.MovementStates.Attack, cancellationToken: destroyCancellationToken));

                //����ɏ����𖞂������Ȃ���
                if (result == 2)
                {
                    return;
                }
            }

            else if (endCon == EnemyValue.AttackEndCondition.�q�b�g�����[�V�����I��)
            {
                string name = $"Attack{attackNumber}";

                result = await UniTask.WhenAny(UniTask.WaitUntil(() => isHit, cancellationToken: destroyCancellationToken),
                UniTask.WaitUntil(() => CheckEnd(name), cancellationToken: destroyCancellationToken),
                UniTask.WaitUntil(() => _movement.CurrentState != CharacterStates.MovementStates.Attack, cancellationToken: destroyCancellationToken));

                //����ɏ����𖞂������Ȃ���
                if (result == 2)
                {
                    return;
                }
                //�q�b�g���ĂȂ��Ȃ�R���{���f
                else if (result == 1)
                {
                    atV.isCombo = false;
                }
            }
            else if (endCon == EnemyValue.AttackEndCondition.�q�b�g�����Ԍo��)
            {

                result = await UniTask.WhenAny(UniTask.WaitUntil(() => isHit, cancellationToken: destroyCancellationToken),
                UniTask.Delay(TimeSpan.FromSeconds(4f), cancellationToken: destroyCancellationToken),
                UniTask.WaitUntil(() => _movement.CurrentState != CharacterStates.MovementStates.Attack, cancellationToken: destroyCancellationToken));

                //����ɏ����𖞂������Ȃ���
                if (result == 2)
                {
                    return;
                }
                //�q�b�g���ĂȂ��Ȃ�R���{���f
                else if (result == 1)
                {
                    atV.isCombo = false;
                }
            }
            else if (endCon == EnemyValue.AttackEndCondition.�⑫�s���̏I��)
            {
                string name = $"Attack{attackNumber}";
                // ���[�V�����I�����X�^���ŋ���������ҋ@
                result = await UniTask.WhenAny(UniTask.WaitUntil(() => CheckEnd(name), cancellationToken: destroyCancellationToken),
                    UniTask.WaitUntil(() => _movement.CurrentState != CharacterStates.MovementStates.Attack, cancellationToken: destroyCancellationToken));

                //����ɃA�j���[�V�������I�����ďI������Ȃ瑱����
                if (result != 0)
                {
                    return;
                }

                //��������⑫�A�j���Đ�
                //�⑫�s���ɂ�
                //�㌄�A�j���A�W���X�g�i����j�K�[�h�A����A�e���|�[�g�Ƃ��H
                //�e���|�[�g�Ƃ��͍U����C�x���g�ł�������
                //���ꂩ�e���|�[�g����ŃR���{�p���Ƃ�����Ȃ炱�ꂶ��Ȃ��ƃ_����
                SActTrigger(atV.suppleNumber);


            }

            //      if (!CheckEnd($"SuppleAction{suppleNumber}"))


            _attack.AttackEnd();
            NormalFlip(direction);
            isHit = false;

            _movement.ChangeState(CharacterStates.MovementStates.Idle);

            if (atV.isCombo)
            {
                //�}�C�i�X�ȉ��̃A�[�}�[�Ȃ狭�����Ă��
                atV.aditionalArmor = atV.aditionalArmor < 0 ? 30 : atV.aditionalArmor;

                isAtEnable = true;
                isMovable = true;
                attackNumber++;
                Attack(attackNumber,false);
            }
            //�R���{�����I��
            else
            {
                atV.aditionalArmor = 0;
                isArmor = false;

                attackNumber = 0;

                //�N�[���^�C�����K�v�Ȃ�
                if (atV.coolTime > 0)
                {
                    //�U���I����̓N�[���^�C������
                    WaitAction(atV.coolTime, (() => isAtEnable = true)).Forget();

                }
                //����Ȃ��Ȃ�U���\��
                else
                {
                    isAtEnable = true;
                }

                //�U�����f���ĊJ
                AttackBehaivior().Forget();

                //�U���I�����̏���
                AttackEvent();

            }
        }

        /// <summary>
        /// �A�j�����f
        /// </summary>
        public void AttackEnd(bool stun = false)
        {
            if (!stun)
            {
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
                _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
            }

            atV.aditionalArmor = 0;
            isAtEnable = true;

            attackNumber = 0;

            _attack.AttackEnd();
            GravitySet(status.firstGravity);
            isArmor = false;

            //�U�����f���ĊJ
            AttackBehaivior().Forget();
        }

        /// <summary>
        /// �p���B�s�̍s��
        /// �G�t�F�N�g�o�����Ԃ��w��ł���悤�ɂ�������
        /// LifTime�����邩
        /// </summary>
        public void DisParriableAct()
        {
            GManager.instance.PlaySound("DisenableParry", transform.position);
            Vector3 Scale = Vector3.zero;
            if (transform.localScale.x > 0)
            {
                Scale.Set(status.disparriableScale.x, status.disparriableScale.y, status.disparriableScale.z);
            }
            else
            {
                Scale.Set(status.disparriableScale.x * -1f, status.disparriableScale.y, status.disparriableScale.z);
            }

            dispary.transform.localScale = Scale;
            dispary.transform.rotation = Quaternion.Euler(-10, 0, 0);
            //	Addressables.InstantiateAsync("DisParriableEffect", dispary.transform);
        }

        //�w���X�̂��߂ɍU����Ԃ��ۂ���Ԃ�
        public bool AttackCheck()
        {
            return _movement.CurrentState == CharacterStates.MovementStates.Attack;
        }

        #endregion









        #region ���֘A




        public override void GuardSound()
        {
            MyCode.SoundManager.instance.GuardSound(status.isMetal, status.shieldType, transform.position);
        }





        #endregion



        #region �ړ��֘A����

        public void NormalFlip(float direction)
        {
            if (direction == 0 || (_condition.CurrentState == CharacterStates.CharacterConditions.Dead || _condition.CurrentState == CharacterStates.CharacterConditions.Stunned))
            {
                return;
            }

            //���̌����ƌ��������������Ⴄ�Ȃ�
            if (direction != MathF.Sign(transform.localScale.x))
            {

                _character.Flip();
            }

        }

        /// <summary>
        /// �A�j���̓s���㔽�Α����������Ƃ��̃��\�b�h
        /// </summary>
        public void AnimeFlip()
        {
            //_characterFly.SetHorizontalMove(1f);
            //���̂ւ�g���ĐU������s����

            NormalFlip(Mathf.Sign(-transform.localScale.x));
        }


        #region �x����ԂŌĂԃ��\�b�h

        /// <summary>
        /// �ŏ��̈ʒu�ɖ߂�B
        /// </summary>
        public void PositionReset()
        {
            //�����āA�Ȃ����|�W�V�������Z�b�g���Ȃ�
            if (isMovable && posiReset)
            {


                if (status._charaData._kind == EnemyStatus.KindofEnemy.Fly)
                {
                    int returneDireX = 0;

                    int returneDireY = 0;
                    //�߂����������o��
                    if (Math.Abs(myPosition.x - basePosition.x) >= 3)
                    {
                        returneDireX = myPosition.x <= basePosition.x ? 1 : -1;
                    }

                    if (Math.Abs(myPosition.y - basePosition.y) >= 3)
                    {
                        returneDireY = myPosition.y <= basePosition.y ? 1 : -1;
                    }


                    //�߂�ꏊ�Ƃ̋������o��
                    //����
                    float returnDis = Vector2.SqrMagnitude(myPosition - basePosition);

                    if (returnDis <= 25)
                    {
                        _flying.SetHorizontalMove(0);
                        _flying.SetVerticalMove(0);
                        posiReset = false;

                    }
                    else
                    {
                        NormalFlip(returneDireX);
                        _flying.SetHorizontalMove(returneDireX);
                        _flying.SetVerticalMove(returneDireY);

                    }


                    //Direction��Distance�ł��Ȃ��H

                }
                else
                {
                    //�߂����������o��
                    int returneDire = myPosition.x <= basePosition.x ? 1 : -1;

                    //�߂�ꏊ�Ƃ̋������o��
                    float returnDis = Math.Abs(myPosition.x - basePosition.x);

                    if (returnDis <= 5)
                    {
                        _characterHorizontalMovement.SetHorizontalMove(0);
                        posiReset = false;
                    }
                    else
                    {
                        NormalFlip(returneDire);
                        _characterHorizontalMovement.SetHorizontalMove(returneDire);
                    }
                }



            }
        }


        /// <summary>
        /// �ҋ@���̏����s��
        /// �񓯊��ōŏ��Ɉ�x�����Ăяo��
        /// �_���[�W�H������璆�f���ă_���[�W�����I����ɂ�����x�Ăяo��
        /// �p�g���[���g�[�N�����K�v
        /// </summary>
        public async UniTaskVoid PatrolMove()
        {
            if (posiReset)
            {
                //���̈ʒu�ɖ߂�܂ł͑҂�
                await UniTask.WaitUntil(() => !posiReset, cancellationToken: patrolToken.Token);
            }

            //����͍������Ŕz�u����Ă鎞�̂��߂ɕK�v
            int myDire = Math.Sign(transform.localScale.x);



            //�E�����ĂĖړI�n��荶�Ȃ�
            if (myDire > 0 && myPosition.x < startPosition.x + status.waitDistance.x)
            {           

                _characterHorizontalMovement.SetHorizontalMove(1);

                //�����͈͂���ɍs���̂�҂�
                await UniTask.WaitUntil(() => (myPosition.x >= startPosition.x + status.waitDistance.x), cancellationToken: patrolToken.Token);
                
                _characterHorizontalMovement.SetHorizontalMove(0);
                //�b���҂�
                await UniTask.Delay(TimeSpan.FromSeconds(status.waitRes), cancellationToken: patrolToken.Token);

             NormalFlip(-1);
                PatrolMove().Forget();

            }
            else if(myPosition.x > startPosition.x - status.waitDistance.x)
            {
               
                _characterHorizontalMovement.SetHorizontalMove(-1);

                //�����͈͂���ɍs���̂�҂�
                await UniTask.WaitUntil(() => (myPosition.x <= startPosition.x - status.waitDistance.x), cancellationToken: patrolToken.Token);


                _characterHorizontalMovement.SetHorizontalMove(0);
                //�b���҂�
                await UniTask.Delay(TimeSpan.FromSeconds(status.waitRes), cancellationToken: patrolToken.Token);

                NormalFlip(1);
                PatrolMove().Forget();
            }

        }

        /// <summary>
        /// �ҋ@�������
        /// patrolMove�͉��ړ��B����͏c�ړ��B����c���ǂ��������
        /// �ҋ@�ƑI���Őς�
        /// �ŏ������Ă�
        /// </summary>
        public async UniTaskVoid PatrolFly()
        {
            if (posiReset)
            {
                //���̈ʒu�ɖ߂�܂ł͑҂�
                await UniTask.WaitUntil(() => !posiReset, cancellationToken: patrolToken.Token);
            }

            //����͍������Ŕz�u����Ă鎞�̂��߂ɕK�v
            int faceDire = Math.Sign(transform.localScale.x);


            int pDireX = 0;
            int pDireY = 0;

            if (status.waitDistance.x != 0)
            {
                //�E�����ĂČ����͈͂̉E�[�ȓ��Ȃ������1
                pDireX =  faceDire > 0 && myPosition.x < startPosition.x + status.waitDistance.x  ? 1 : -1;
            }

            if (status.waitDistance.y != 0)
            {
                pDireY = myPosition.y < startPosition.y + status.waitDistance.y ? 1 : -1;
            }

            NormalFlip(pDireX);

            _flying.SetHorizontalMove(pDireX);
            _flying.SetVerticalMove(pDireY);


            //X�ɂ��������Ƃ�
            if (pDireY == 0 && pDireX != 0)
            {

                //�����͈͂���ɍs���̂�҂�
                await UniTask.WaitUntil(() => (Math.Abs(myPosition.x - startPosition.x) >= status.waitDistance.x), cancellationToken: patrolToken.Token);

                //�~�܂�
                _flying.SetHorizontalMove(0);

                //��������������b���҂�
                await UniTask.Delay(TimeSpan.FromSeconds(status.waitRes), cancellationToken: patrolToken.Token);

            }
            //y�ɂ��������Ƃ�
            else if (pDireX == 0)
            {
                //�����͈͂���ɍs���̂�҂�
                await UniTask.WaitUntil(() => (Math.Abs(myPosition.y - startPosition.y) >= status.waitDistance.y), cancellationToken: patrolToken.Token);

                //�~�܂�
                _flying.SetVerticalMove(0);

                //��������������b���҂�
                await UniTask.Delay(TimeSpan.FromSeconds(status.waitRes), cancellationToken: patrolToken.Token);


                //�t����
                NormalFlip(-faceDire);
            }
            //�����ɓ���
            else
            {
                //�����͈͂���ɍs���̂�҂�
                await UniTask.WaitUntil(() => (Math.Abs(myPosition.x - startPosition.x) >= status.waitDistance.x) || (Math.Abs(myPosition.y - startPosition.y) >= status.waitDistance.y), cancellationToken: patrolToken.Token);

                if ((Math.Abs(myPosition.x - startPosition.x) >= status.waitDistance.x))
                {
                    //�~�܂�
                    _flying.SetHorizontalMove(0);
                    _flying.SetVerticalMove(0);

                    //x�̏�������������b���҂�
                    await UniTask.Delay(TimeSpan.FromSeconds(status.waitRes), cancellationToken: patrolToken.Token);

                    //�t����
                    NormalFlip(-faceDire);
                }

                //y�̏����𖞂������Ȃ���������ς���

            }


            //������x�Ă�
            PatrolMove().Forget();


        }


        /// <summary>
        /// �ҋ@����~���Ă���낫���U����������B�����̓p�g���[�����Ȃ�����waitTime�͎g���܂킵�ł���
        /// �p�g���[���ƑI���łǂ������ς�
        /// �ŏ������Ă�
        /// </summary>
        public async UniTaskVoid Wait()
        {
            if (posiReset)
            {
                //���̈ʒu�ɖ߂�܂ł͑҂�
                await UniTask.WaitUntil(() => !posiReset, cancellationToken: patrolToken.Token);
            }

            //�b���҂�
            await UniTask.Delay(TimeSpan.FromSeconds(status.waitRes), cancellationToken: patrolToken.Token);

            NormalFlip(-transform.localScale.x);

            Wait().Forget();
        }

        #endregion


        /// <summary>
        /// �v���C���[�����������m�F
		/// ��莞�ԓ��������邩�S���͈͂��痣�ꂽ��ǂ��̂�߂�
		/// ����͕��ʂɂ�邩
        /// ������ꂽ�㑼�Ƀ^�[�Q�b�g���Ȃ���ΐ퓬�I���H
        /// </summary>
        public void EscapeCheck()
        {

            if (Mathf.Abs(distance.x) >= status.escapeDistance.x || Mathf.Abs(distance.y) >= status.escapeDistance.y)
            {
                //���肪�����J�n�����Ȃ玞�ԋL�^
                if (escapeTime == 0)
                {
                    escapeTime = GManager.instance.nowTime;
                }

                if (GManager.instance.nowTime - escapeTime >= status.chaseRes)
                {
                    CombatEnd();
                    escapeTime = 0.0f;
                }
            }
            else
            {
                escapeTime = 0.0f;
            }
        }


        //�������m�F�B�������Ă܂��g��
        void fallCheck()
        {
            if (myPosition.y < -30.0f)
            {

            }
        }

        /// <summary>
        /// ���̓G���������邩�ǂ��������߂�
        /// </summary>
        protected void EscapeJudge(int probably = 100)
        {
            if (RandomValue(0, 100) <= probably)
            {
                isEscape = true;

                //�K�[�h�g�p���Ȃ��߂�
                if (guardJudge)
                {
                    guardJudge = false;
                    guardProballity = 0;
                    useGuard = false;
                    _guard.GuardEnd();
                }
            }

        }


        //��{�I�ɐ퓬�n�̈ړ���CombatStart�i�j�݂����Ȃ̂�Move�n������
        //�����Ĕ�e����U�����Judge�n����ĊJ
        //�퓬�ړ��𗘗p���Ȃ��n���̓G��status�̈ړ����x���琄������H
        //���ꂩbool�Ŏ����Ƃ��Ă��������ǂ�


        #region �n��̓G�̐퓬�ړ��֘A


        /// <summary>
        /// �퓬�ړ����J�n���郁�\�b�h
		/// �ŏ��ɌĂяo��
        /// </summary>
        protected void CombatMoveStart()
        {
            AgrMove(true).Forget();
            BattleFlip().Forget();
        }

        /// <summary>
        /// �퓬���̋����̎���
        /// nowMode�ŋ����̎����̃p�^�[����I���ł����
        ///�@�퓬�J�n���ɍŏ������Ă�
        ///�@��e�ȂǂŒ�~������g�[�N���ōĔ��f�����Ă�����
        ///�@�A�[�}�[�����Ƀg�[�N�������������ނ�
        ///�@��e�Ȃǂŏ�����~��AgrJudge�i�j���Ă��
        ///�@��������AgrMove���܂��o�Ă���
        /// </summary>
        protected async UniTaskVoid AgrMove(bool wakeUp = false)
        {

            if (!isMovable)
            {
                //�X�^�������Ȃ炷���U�������悤��
                wakeUp = _condition.CurrentState == CharacterStates.CharacterConditions.Stunned;
                //������悤�ɂȂ�܂ő҂�
                await UniTask.WaitUntil(() => isMovable, cancellationToken: agrToken.Token);
            }
            //�ۗ�
            GuardJudge();

            if (!wakeUp)
            {

                //���Ԍo�߂����画��
                await UniTask.WaitForSeconds(status.judgePace, cancellationToken: agrToken.Token);
            }
            AgrJudge();

            //�܂�AgrMove���Ăяo��
            AgrMove().Forget();

        }


        /// <summary>
        /// �����ɋ�������ړ������Ȃǂ��Z�o���邾���ł���
        /// isEscape�͍U�����e���ɍČĂяo������Ȃ玩���Œl�����đ����
        /// battleFlip�͑��x�Ƃ����������̂����삷��悤��
        /// </summary>
        public void AgrJudge()
        {
            //�_�b�V���\�ȃL�������ǂ���
            bool isDashable = status.combatSpeed.x > 0;

            //���f�Ɏg������
            float useDis = Mathf.Abs(distance.x);



            if (!isEscape)
            {
                //�K�[�h�g�p��Ԃ���Ȃ��Ȃ�
                if (!useGuard)
                {
                    //��틗���ȓ��ň��̊m���Ŏ~�܂�H
                    //��~�m���݂����Ȃ̎������邩
                    if (((useDis < status.agrDistance[nowMode].x + status.adjust) && RandomValue(0, 100) >= 40))
                    {

                        ground = EnemyStatus.MoveState.stay;
                        //	flipWaitTime = 10;
                    }

                    //��틗����藣��Ă�
                    else if (useDis > status.agrDistance[nowMode].x)//�߂Â�������ˁH
                    {
                        //�����������߂�������Ȃ��Ȃ����
                        if (useDis <= status.walkDistance.x || !isDashable)
                        {
                            ground = EnemyStatus.MoveState.accessWalk;
                        }
                        //����������艓���Ȃ瑖��
                        else
                        {
                            ground = EnemyStatus.MoveState.accessDash;
                        }
                    }
                    //��틗�����߂��Ȃ�
                    else if (useDis < status.agrDistance[nowMode].x)//��������
                    {
                        //���������Ȃ�G�������܂܌���
                        //�����Ȃ��|���Ƃ��͈ړ����x�[����
                        if (Mathf.Abs((useDis - status.agrDistance[nowMode].x)) <= status.walkDistance.x / 2 || !isDashable)
                        {
                            ground = EnemyStatus.MoveState.leaveWalk;
                        }
                        else
                        {
                            ground = EnemyStatus.MoveState.leaveDash;

                        }
                    }
                }
                else
                {
                    //��틗���ȓ��ň��̊m���Ŏ~�܂�H
                    //��~�m���݂����Ȃ̎������邩
                    if (((useDis < status.agrDistance[nowMode].x + status.adjust) && RandomValue(0, 100) >= 40) || guardHit)
                    {
                        ground = EnemyStatus.MoveState.stay;
                        //	flipWaitTime = 10;
                    }

                    //��틗����藣��Ă�
                    else if (useDis > status.agrDistance[nowMode].x)//�߂Â�������ˁH
                    {

                        ground = EnemyStatus.MoveState.accessWalk;


                    }
                    //��틗�����߂��Ȃ�
                    else if (useDis < status.agrDistance[nowMode].x)//��������
                    {

                        ground = EnemyStatus.MoveState.leaveWalk;

                    }
                }
            }
            else
            {

                if (status.agrDistance[nowMode].x <= 30 || !isDashable)
                {
                    ground = EnemyStatus.MoveState.leaveWalk;
                }
                else if (Mathf.Abs((useDis / status.agrDistance[nowMode].x)) >= 0.6)
                {
                    //�����鎞�̓K�[�h�I��点��
                    ground = EnemyStatus.MoveState.leaveDash;

                }

                isEscape = false;
            }
            AgrMoveSet();



        }


        /// <summary>
        /// ���݂̈ړ����[�h�ɍ��킹�ĕ�����
        /// �ړ����x���Z�b�g����
        /// BattleFrip��AgrJudge����Ă΂��
        /// </summary>
        void AgrMoveSet()
        {


            if (ground == EnemyStatus.MoveState.stay)
            {
                //�o�g���t���b�v�̓X�e�C�������ɂ���

                _characterHorizontalMovement.SetHorizontalMove(0f);
                NormalFlip(direction);

            }


            else if (ground == EnemyStatus.MoveState.leaveDash || ground == EnemyStatus.MoveState.escape)
            {

                _characterRun.RunStart();
                _characterHorizontalMovement.SetHorizontalMove(-direction);
                NormalFlip(-direction);
            }
            else if (ground == EnemyStatus.MoveState.accessDash)

            {
                _characterRun.RunStart();
                _characterHorizontalMovement.SetHorizontalMove(direction);
                NormalFlip(direction);
            }
            else if (ground == EnemyStatus.MoveState.accessWalk)

            {
                _characterRun.RunStop();
                _characterHorizontalMovement.SetHorizontalMove(direction);
                NormalFlip(direction);
            }
            else if (ground == EnemyStatus.MoveState.leaveWalk)

            {
                _characterRun.RunStop();
                _characterHorizontalMovement.SetHorizontalMove(-direction);
                NormalFlip(direction);
            }



        }



        /// <summary>
        /// �퓬���̋������i�郁�\�b�h
        /// </summary>
        async UniTaskVoid BattleFlip()
        {
            if (!isMovable)
            {

                //������悤�ɂȂ�܂ő҂�
                await UniTask.WaitUntil(() => isMovable, cancellationToken: agrToken.Token);
            }

            //0.5�b���Ƃ�
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: agrToken.Token);
            AgrMoveSet();

            //�����ɓ��B�������ǂ����𒲂ׂ�
            isReach = Math.Abs(distance.x) <= status.agrDistance[nowMode].x;

            //�ċA
            BattleFlip().Forget();


        }

        #endregion


        #region �󒆂̓G�̐퓬�Ŏg��


        /// <summary>
        /// �퓬��s���J�n���郁�\�b�h
        /// </summary>
        protected void CombatFlyStart()
        {
            AgrFly(true).Forget();
            BattleFlyFlip().Forget();
        }

        /// <summary>
        /// �퓬���̋����̎���
        /// nowMode�ŋ����̎����̃p�^�[����I���ł����
        ///�@�퓬�J�n���ɍŏ������Ă�
        ///�@��e�ȂǂŒ�~������g�[�N���ōĔ��f�����Ă�����
        ///�@�A�[�}�[�����Ƀg�[�N�������������ނ�
        ///�@��e�Ȃǂŏ�����~��AgrJudge�i�j���Ă��
        ///�@��������AgrMove���܂��o�Ă���
        /// </summary>
        protected async UniTaskVoid AgrFly(bool wakeUp = false)
        {

            if (!isMovable)
            {
                //�X�^�������Ȃ炷���U�������悤��
                wakeUp = _condition.CurrentState == CharacterStates.CharacterConditions.Stunned;
                //������悤�ɂȂ�܂ő҂�
                await UniTask.WaitUntil(() => isMovable, cancellationToken: agrToken.Token);
            }
            AirGuardJudge();
            //	Debug.Log($"�m�肽��{ground}");


            if (!wakeUp)
            {
                //���Ԍo�߂����画��
                await UniTask.WaitForSeconds(status.judgePace, cancellationToken: agrToken.Token);
            }
            AgrFlyJudge();

            //�܂�AgrFly���Ăяo��
            AgrFly().Forget();
        }



        /// <summary>
        /// �����ɋ�������ړ������Ȃǂ��Z�o���邾���ł���
        /// isEscape�͍U�����e���ɍČĂяo������Ȃ玩���Œl�����đ����
        /// battleFlip�͑��x�Ƃ����������̂����삷��悤��
        /// </summary>
        public void AgrFlyJudge()
        {


            //Escape�͌�ł܂Ƃ߂�
            if (!isEscape)
            {
                //���f�Ɏg������
                float useDis = Mathf.Abs(distance.x);

                //�K�[�h�g�p��Ԃ���Ȃ��Ȃ�
                if (!useGuard)
                {
                    //�_�b�V���\�ȃL�������ǂ���
                    bool isDashable = status.combatSpeed.x > 0;

                    #region ������

                    //��틗���ȓ��ň��̊m���Ŏ~�܂�H
                    //��~�m���݂����Ȃ̎������邩
                    if (((useDis < status.agrDistance[nowMode].x + status.adjust) && RandomValue(0, 100) >= 40))
                    {

                        ground = EnemyStatus.MoveState.stay;
                        //	flipWaitTime = 10;
                    }

                    //��틗����藣��Ă�
                    else if (useDis > status.agrDistance[nowMode].x)//�߂Â�������ˁH
                    {
                        //�����������߂�������Ȃ��Ȃ����
                        if (useDis <= status.walkDistance.x || !isDashable)
                        {
                            ground = EnemyStatus.MoveState.accessWalk;
                        }
                        //����������艓���Ȃ瑖��
                        else
                        {
                            ground = EnemyStatus.MoveState.accessDash;
                        }
                    }
                    //��틗�����߂��Ȃ�
                    else if (useDis < status.agrDistance[nowMode].x)//��������
                    {
                        //���������Ȃ�G�������܂܌���
                        //�����Ȃ��|���Ƃ��͈ړ����x�[����
                        if (Mathf.Abs((useDis - status.agrDistance[nowMode].x)) <= status.walkDistance.x / 2 || !isDashable)
                        {
                            ground = EnemyStatus.MoveState.leaveWalk;
                        }
                        else
                        {
                            ground = EnemyStatus.MoveState.leaveDash;

                        }
                    }
                    #endregion

                    #region �c����

                    //�����ɍU�������𑫂����ƂŃv���C���[�����̈ʒu�ւ̋����ɂȂ�
                    useDis = distance.y + status.agrDistance[nowMode].y;

                    isDashable = status.combatSpeed.y > 0;

                    //��틗���ȓ��ň��̊m���Ŏ~�܂�H
                    //��~�m���݂����Ȃ̎������邩
                    if (useDis <= status.adjust)
                    {
                        air = EnemyStatus.MoveState.stay;

                    }

                    //��ɂ��鎞���ɍs��
                    else if (useDis < 0)
                    {
                        //�����������߂�������Ȃ��Ȃ����
                        if (Mathf.Abs(useDis) <= status.walkDistance.y || !isDashable)
                        {
                            air = EnemyStatus.MoveState.accessWalk;
                        }
                        //����������艓���Ȃ瑖��
                        else
                        {
                            air = EnemyStatus.MoveState.accessDash;
                        }
                    }
                    //���ɂ��鎞��ɍs�� 
                    else
                    {
                        //���������Ȃ�G�������܂܌���
                        //�����Ȃ��|���Ƃ��͈ړ����x�[����
                        if (useDis <= status.walkDistance.y || !isDashable)
                        {
                            air = EnemyStatus.MoveState.leaveWalk;
                        }
                        else
                        {
                            air = EnemyStatus.MoveState.leaveDash;

                        }
                    }

                    #endregion
                }
                else
                {

                    #region ������
                    //��틗���ȓ��ň��̊m���Ŏ~�܂�H
                    //��~�m���݂����Ȃ̎������邩
                    if (((useDis < status.agrDistance[nowMode].x + status.adjust) && RandomValue(0, 100) >= 40) || guardHit)
                    {
                        ground = EnemyStatus.MoveState.stay;
                        //	flipWaitTime = 10;
                    }

                    //��틗����藣��Ă�
                    else if (useDis > status.agrDistance[nowMode].x)//�߂Â�������ˁH
                    {

                        ground = EnemyStatus.MoveState.accessWalk;


                    }
                    //��틗�����߂��Ȃ�
                    else if (useDis < status.agrDistance[nowMode].x)//��������
                    {

                        ground = EnemyStatus.MoveState.leaveWalk;

                    }
                    #endregion

                    #region �c����
                    //�����ɍU�������𑫂����ƂŃv���C���[�����̈ʒu�ւ̋����ɂȂ�
                    useDis = distance.y + status.agrDistance[nowMode].y;

                    //��틗���ȓ��ň��̊m���Ŏ~�܂�H
                    //��~�m���݂����Ȃ̎������邩
                    if (useDis <= status.adjust || guardHit)
                    {
                        air = EnemyStatus.MoveState.stay;
                        //	flipWaitTime = 10;
                    }
                    //��ɂ��鎞���ɍs��
                    else if (useDis < 0)
                    {

                        air = EnemyStatus.MoveState.accessWalk;
                    }

                    else
                    {

                        air = EnemyStatus.MoveState.leaveWalk;

                    }

                    #endregion

                }
            }
            //������
            else
            {
                //����͓������Ƃ��ɕς��������悭�Ȃ��H
                if (atV.escapePercentage > 0)
                {
                    if (atV.escapePercentage == 1)
                    {
                        ground = EnemyStatus.MoveState.leaveDash;
                        air = EnemyStatus.MoveState.leaveDash;
                    }
                    else if (atV.escapePercentage == 2)
                    {
                        ground = EnemyStatus.MoveState.leaveWalk;
                        air = EnemyStatus.MoveState.leaveWalk;
                    }
                    else if (atV.escapePercentage == 3)
                    {
                        ground = EnemyStatus.MoveState.leaveDash;
                        air = EnemyStatus.MoveState.stay;
                    }
                    else if (atV.escapePercentage == 4)
                    {
                        ground = EnemyStatus.MoveState.leaveWalk;
                        air = EnemyStatus.MoveState.stay;
                    }

                }
                isEscape = false;
            }


            AgrFlySet();






        }



        /// <summary>
        /// ���݂̈ړ����[�h�ɍ��킹�ĕ�����
        /// �ړ����x���Z�b�g����
        /// BattleFrip��AgrJudge����Ă΂��
        /// </summary>
        void AgrFlySet()
        {


            if (ground == EnemyStatus.MoveState.stay)
            {
                //�o�g���t���b�v�̓X�e�C�������ɂ���
                _flying.FastFly(false, true);
                _characterHorizontalMovement.SetHorizontalMove(0f);
                NormalFlip(direction);

            }


            else if (ground == EnemyStatus.MoveState.leaveDash)
            {

                _flying.FastFly();
                _flying.SetHorizontalMove(-direction);
                NormalFlip(-direction);
            }
            else if (ground == EnemyStatus.MoveState.accessDash)

            {
                _flying.FastFly();
                _flying.SetHorizontalMove(direction);
                NormalFlip(direction);
            }
            else if (ground == EnemyStatus.MoveState.accessWalk)

            {
                _flying.FastFly(false, true);
                _flying.SetHorizontalMove(direction);
                NormalFlip(direction);
            }
            else if (ground == EnemyStatus.MoveState.leaveWalk)

            {
                _flying.FastFly(false, true);
                _flying.SetHorizontalMove(-direction);
                NormalFlip(direction);
            }



            if (air == EnemyStatus.MoveState.stay)
            {
                //�o�g���t���b�v�̓X�e�C�������ɂ���
                _flying.FastFly(true, true);
                _flying.SetVerticalMove(0f);

            }


            else if (air == EnemyStatus.MoveState.leaveDash)
            {

                _flying.FastFly(true);
                _flying.SetVerticalMove(1);
            }
            else if (air == EnemyStatus.MoveState.accessDash)

            {
                _flying.FastFly(true);
                _flying.SetVerticalMove(-1);
            }
            else if (air == EnemyStatus.MoveState.accessWalk)

            {
                _flying.FastFly(true, true);
                _flying.SetVerticalMove(-1);
            }
            else if (air == EnemyStatus.MoveState.leaveWalk)

            {
                _flying.FastFly(true, true);
                _flying.SetVerticalMove(1);
            }


        }



        /// <summary>
        /// �퓬���̋������i�郁�\�b�h
        /// </summary>
        async UniTaskVoid BattleFlyFlip()
        {
            if (!isMovable)
            {

                //������悤�ɂȂ�܂ő҂�
                await UniTask.WaitUntil(() => isMovable, cancellationToken: agrToken.Token);
            }

            //0.5�b���Ƃ�
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: agrToken.Token);
            AgrFlySet();

            //�퓬�����ɓ��B���Ă��邩
            isReach = Math.Abs(distance.x) <= status.agrDistance[nowMode].x && Math.Abs(distance.y) <= status.agrDistance[nowMode].y;




            //�ċA
            BattleFlyFlip().Forget();

        }

        #endregion

        /// <summary>
        /// ����B�����w����\�����퓬���Ɍ���direction�őO���B-direction�Ō���
        /// </summary>
        /// <param name="direction"></param>
        public void Avoid(float direction)
        {
            if (!isMovable)
            {
                _rolling.StartRoll();
            }

        }





        /// <summary>
        ///	�n��L�����p�̃W�����v�B�W�����v��Ԃ͎��������B�W�����v�L�����Z���ƃZ�b�g
        ///	�o�b�N�W�����v���^�Ȃ���ɐU������Ĕ��ł���
        /// </summary>	
        public void JumpAct(bool verticalJump = false, bool backJump = false)
        {

            if (isMovable)
            {
                if (!verticalJump)
                {
                    isVertical = verticalJump;
                    if (backJump)
                    {
                        float dire = -1;
                        if (transform.localScale.x > 0)
                        {
                            dire = 1;
                        }
                        NormalFlip(dire);
                    }
                }
                if (status._charaData._kind != EnemyStatus.KindofEnemy.Fly)
                {
                    if (_controller.State.IsGrounded)
                    {
                        _jump.JumpStart();
                    }
                }
                else
                {
                    _jump.JumpStart();
                }


            }
        }

        /// <summary>
        /// �g���K�[�ŌĂԂ悤��
        /// </summary>
        /// <param name="isVertical"></param>
        public void JumpController()
        {
            if (_movement.CurrentState == CharacterStates.MovementStates.Jumping || _movement.CurrentState == CharacterStates.MovementStates.DoubleJumping)
            {
                if (!isVertical)
                {
                    _characterHorizontalMovement.SetHorizontalMove(Mathf.Sign(transform.localScale.x));
                }

            }

            //�_�u���W�����v���邩�ǂ����͔C�ӂŌ��߂܂�
            /*else if (!_controller.State.IsGrounded)
			{
				if (_jump.JumpEnableJudge() == true)
				{
					Debug.Log($"fffffffffff");
					_jump.JumpStart();
				}
			}*/
        }





        #endregion
        #region�@���S���̃}�e���A���֘A

        protected void MaterialControll()
        {
            if (materialSet > 0)
            {


                //�e�}�e���A���̏�������
                //	Debug.Log($"{parentMatt.material}");
                if (materialSet == 1)
                {

                    GetAllChildren(spriteList[mattControllNum]);
                    //	await UniTask.WaitForFixedUpdate();

                    materialSet++;
                }
                if (materialSet > 1)
                {
                    //Debug.Log($"Hello{controllTarget[2].material.name}");
                    materialConTime += _controller.DeltaTime;
                    float test = Mathf.Lerp(0f, 1, materialConTime / 2);
                    for (int i = 0; i <= controllTarget.Count - 1; i++)
                    {


                        controllTarget[i].material.SetFloat("_FadeAmount", test);

                    }


                }

            }

        }
        /// <summary>
        /// ���ʂƂ��̃}�e���A������n���p���\�b�h�B���X�g�̃[���͂悭�g����ɂ���̂�����
        /// ��b�ŏ�����
        /// </summary>
        /// <param name="controllNumber"></param>
        protected void MattControllStart(int controllNumber = 0)
        {
            materialSet = 1;
            MyCode.SoundManager.instance.DeathEffect(effectController.transform.position);
            if (controllNumber != mattControllNum)
            {
                materialConTime = 0;
                controllTarget = null;
                controllTarget = new List<Renderer>();
                mattControllNum = controllNumber;
                mattTrans = null;
            }
        }



        /// <summary>
        /// �}�e���A�����擾������
        /// Weapon�͕����T���t���O
        /// �X�v���C�g���X�g�̊O���畐��������Ă���
        /// </summary>
        /// <param name="parent">�}�e���A�������W����I�u�W�F�N�g�̐e</param>
        /// <param name="transforms">�g�p���郊�X�g</param>
        /// <param name="Weapon">��ԏ�̃I�u�W�F�N�g�ł��邩�ǂ����B�I�������X�v���C�g���X�g����Ȃ��Ƃ��ɕ���̃}�e���A��������</param>
        private void GetAllChildren(Transform parent, bool Weapon = false)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                //�܂��q�I�u�W�F�N�g���g�����X�t�H�[���̃��X�g�ɒǉ�
                //�q�I�u�W�F�N�g�̎q�I�u�W�F�N�g�܂ŒT��

                Transform child = parent.GetChild(i);

                GetAllChildren(child, true);
                //�����_���[�����o���i���Ƃł����𑀍삷��j
                Renderer sr = child.gameObject.MMGetComponentNoAlloc<Renderer>();
                if (sr != null)
                {
                    //���X�g�ɒǉ�
                    //Debug.Log(sr.name);
                    controllTarget.Add(sr);
                }
            }

            //��ԏ�̃I�u�W�F�N�g�̎�����Ə���T��
            if (!Weapon)
            {
                Transform die = transform.MMFindDeepChildBreadthFirst("Attack");
                if (die != null)
                {
                    GetAllChildren(die, true);
                }
                die = transform.Find("Guard");
                if (die != null)
                {
                    GetAllChildren(die, true);
                }

            }

        }


        #endregion


        #region �K�[�h�֘A

        /// <summary>
        /// �����̓K�[�h����m��
        /// �K�[�h���ړ����Ă���
        /// �K�[�h�t���O�����Ă�Ԃ̓W���b�W�̂��тɃK�[�h�m���`�F�b�N���ăK�[�h�������邩�����߂�H
        /// </summary>
        protected void GuardFlagOn(int Proballity = 100)
        {

            //�������K�[�h�ǂ�����H
            guardProballity = Proballity;
            guardJudge = true;

        }

        protected void GuardJudge()
        {
            if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal && guardJudge && _controller.State.IsGrounded)
            {
                useGuard = false;
                if (RandomValue(0, 100) <= guardProballity)
                {
                    //����̂�߂ăK�[�h
                    if (_movement.CurrentState == CharacterStates.MovementStates.Running)
                    {

                        _characterRun.RunStop();
                    }
                    _guard.ActGuard();
                    useGuard = true;
                }



            }
        }

        protected void AirGuardJudge()
        {
            if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal && guardJudge)
            {
                useGuard = false;
                if (RandomValue(0, 100) <= guardProballity)
                {
                    //����̂�߂ăK�[�h
                    if (_movement.CurrentState == CharacterStates.MovementStates.FastFlying)
                    {
                        _flying.FastFly(false, true);
                        _flying.FastFly(true, true);
                    }
                    _guard.ActGuard();
                    useGuard = true;
                }



            }
        }


        /// <summary>
        /// �����̓K�[�h����m��
        /// �K�[�h���ړ����Ă���
        /// �K�[�h�t���O�����Ă�Ԃ̓W���b�W�̂��тɃK�[�h�m���`�F�b�N���ăK�[�h�������邩�����߂�H
        /// </summary>
        protected void GuardFlagOff()
        {

            //�������K�[�h�ǂ�����H
            guardProballity = 0;
            guardJudge = false;

        }

        #endregion

        #region �V�X�^�[����̍��G�֘A�̃��\�b�h

        ///<summary>
        /// ���b�N�J�[�\���̐F��ς���
        /// 0�Ŏn���i���F�j�A1�ŏ����A2�ŕW�I�\���i�ԁj����ȊO�ŉ��F�ɖ߂�
        /// </summary>
        public void TargetEffectCon(int state = 0)
        {
            if (state == 0)
            {
                //�Ԃ����F�ɖ߂��Ȃ��悤�ɂ��łɃJ�[�\�����Ă�Ȃ�߂�Ȃ���
                if (td.gameObject.activeSelf)
                {
                    return;
                }
                td.gameObject.SetActive(true);
                td.color = EnemyManager.instance.stateClor[0];
            }
            else if (state == 1)
            {
                td.gameObject.SetActive(false);
            }
            else if (state == 2)
            {
                td.gameObject.SetActive(true);
                td.color = EnemyManager.instance.stateClor[1];
            }
            else
            {
                // td.enabled = true;
                td.color = EnemyManager.instance.stateClor[0];
            }
        }

        /// <summary>
        /// �Z���T�[���痣��Ă��鎞�Ԃ��v������
        /// </summary>
        void LoseSightWait()
        {
            if (!_seenNow)
            {
                loseSightTime += _controller.DeltaTime;
            }
        }


        /// <summary>
        /// �V�X�^�[����̃Z���T�[���ɓ����Ă��邩�ǂ���
        /// seen��False�Ȃ�Z���T�[����o�Ă���
        /// </summary>
        /// <param name="seen"></param>
        public void SisterRecognition(bool seen)
        {
            if (seen)
            {
                _seenNow = true;
                loseSightTime = 0;
            }
            else
            {
                _seenNow = false;
            }
        }


        /// <summary>
        /// ���̓G���V�X�^�[����Ɍ��ݔF������Ă��邩�ǂ���
        /// ��莞�ԃZ���T�[�����痣�ꂽ�G�⎀�񂾓G�͔F�����Ȃ�
        /// ���Ԃ̓Z���T�[���痣�ꂽ���Ƀt���O���Ă��đ���n�߂�H
        /// ���ꂩ���b�N������R�[�h�ł��낢�낵�Ă�������
        /// �^�Ȃ猩�����Ă�
        /// </summary>
        /// <returns></returns>
        public bool SisterCheck()
        {
            return (loseSightTime > 8 || _condition.CurrentState == CharacterStates.CharacterConditions.Dead);

        }


        #endregion



        #region�@�퓬��Ԑ؂�ւ�

        /// <summary>
        /// �U����ԊJ�n
        /// �G�̏����W�߂ăZ���T�[�͈̔͂��ύX
        /// </summary>
        public void StartCombat(GameObject TriggerEnemy)
        {

            //�G���Ȃ��Ȃ�
            //�Ⴆ�Δ�ѓ���Ƃ��Ȃ�N�����Ȃ�
            //���̑���G�����ł��Ȃ��U���Ƃ��āA���������ȓ��Ȃ�U�����Ă�������̈ʒu�܂ōs���悤��
            //�����Ō�������퓬�J�n�ł��������
            if(TriggerEnemy == null)
            {
                return;
            }

            //�g�[�N�����~���ĐV�i��
            patrolToken.Cancel();
            patrolToken = new CancellationTokenSource();


            //��Ŗ߂��悤�ɃI���ɂ��Ȃ��Ƃ�
            posiReset = true;



            if (dogPile != null)
            {
                basePosition = dogPile.transform.position;
                baseDirection = Math.Sign(dogPile.transform.localScale.x);
            }

            //�퓬�J�n����direction�͂O�ł�

            //���̏ƍ���String�A�I�u�W�F�N�g���ł��Ă��������H
            targetImfo.targetNum = EnemyManager.instance.GetTargetNumber(TriggerEnemy);

            int num = targetImfo.targetNum;

            //�G�̈ʒu��ۑ�����������������߂�
            targetImfo.targetPosition = EnemyManager.instance._targetList[num]._condition.targetPosition;
            distance = targetImfo.targetPosition - myPosition;
            direction = distance.x >= 0 ? 1 : -1;

            //�w�C�g�����₷�H
           // _hateList[num] 

            isAggressive = true;
            NormalFlip(direction);


            //�����Z���T�[�����Ȃ��H
            //�����ł���
            _sensor.RangeChange();


            //�A�[�}�[�񕜊J�n
            ArmorRecover().Forget();


            //�퓬�J�n���ɌĂяo��������
            CombatAction();
        }

        /// <summary>
        /// �퓬�I��
        /// </summary>
        public void CombatEnd()
        {

            isAggressive = false;

            //�g�[�N�����~�߂ĐV�i��
            agrToken.Cancel();
            agrToken = new CancellationTokenSource();

            if (status._charaData._kind == EnemyStatus.KindofEnemy.Fly)
            {
                _flying.FastFly(false, true);
                _flying.FastFly(true, true);
            }
            else
            {
                _characterRun.RunStop();
                _guard.GuardEnd();
            }

            ground = EnemyStatus.MoveState.wakeup;
            air = EnemyStatus.MoveState.wakeup;
            _sensor.RangeChange();

            //�x����ԂŌĂяo���A�N�V����
            PatrolAction();
        }

        #endregion


        /// <summary>
        ///  �K�v�ȃA�j���[�^�[�p�����[�^�[������΁A�A�j���[�^�[�p�����[�^�[���X�g�ɒǉ����܂��B
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            //    RegisterAnimatorParameter(_attackParameterName, AnimatorControllerParameterType.Bool, out _attackAnimationParameter);
            RegisterAnimatorParameter(_suppleParameterName, AnimatorControllerParameterType.Int, out _suppleAnimationParameter);
            RegisterAnimatorParameter(_combatParameterName, AnimatorControllerParameterType.Bool, out _combatAnimationParameter);
        }

        /// <summary>
        /// �A�r���e�B�̃T�C�N�����I���������_�B
        /// ���݂̂��Ⴊ�ށA�����̏�Ԃ��A�j���[�^�[�ɑ���B
        /// </summary>
        public override void UpdateAnimator()
        {
            //���̃X�e�[�g��Attack�ł��邩�ǂ�����Bool����ւ��Ă�

            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _suppleAnimationParameter, suppleNumber, _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _combatAnimationParameter, isAggressive, _character._animatorParameters);
        }





    }
}

