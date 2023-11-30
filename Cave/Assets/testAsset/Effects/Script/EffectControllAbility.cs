using Cysharp.Threading.Tasks;
using MoreMountains.Tools;
using PathologicalGames;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{

    //�����܂��Ȑ݌v
    //�@���̋@�\�͉�����G�t�F�N�g���󋵂ɉ����čĐ����邽�߂̋@�\�ł�
    //�@
    //�@�����g�p���Ă���A�Z�b�g�́A�L�����N�^�[�̍s���i�_�b�V���A�W�����v�Ȃǁj���Ԉُ�i���A�X�^���j���X�e�[�g�}�V���ŊǗ�����@�\���g���Ď������Ă��܂��B
    //�@(��:�X�e�[�g��nowState == �_�b�V���̂悤�Ȍ`�ŏƉ���肷�邱�ƂŃL�����N�^�[�̏�Ԃ�c���ł��܂��j
    //�@���̃R�[�h�ł͂��̃X�e�[�g���Ƃɋ�؂��āu�G�t�F�N�g�≹���v�i�ȉ��ł͗v�f�ƌĂт܂��j���Đ����Ă��܂��B
    //�@�X�e�[�g�̐؂�ւ��̓C�x���g�V�X�e���Œʒm����܂��B
    //�@�܂�Unitask�ɂ��񓯊����������p���āA���t���[���J��Ԃ��������ւ炵�Ă��܂�
    //�@�����ăG�t�F�N�g�̍Đ��ɂ̓I�u�W�F�N�g�v�[���𗘗p���A�������ׂ��y�����Ă��܂��B
    //�@
    //�@�ȉ��ɋ�̓I�ȏ����̗����������܂�
    //�@
    //�@�܂��X�e�[�g���؂�ւ�������i�_�b�V���J�n���Ȃǁj�ɌĂ΂��C�x���g��ʂ��ď������J�n���A�؂�ւ������̃X�e�[�g�A���Ƃ��΃_�b�V�����ɍĐ�����v�f�����邩���m�F���܂��B
    //�@�v�f�͎���\���̂̃��X�g���ŊǗ�����Ă���A���̍\���̂ɂ͗v�f�̃f�[�^���̂��̂ɉ����āA�ǂ̃X�e�[�g�łǂ̂悤�ȕ����ōĐ����邩�Ȃǂ̏�񂪊܂܂�Ă��܂��B
    //�@�\���̂̒��ōĐ�����X�e�[�g������_useState�����o�[���A���݂̃X�e�[�g�ɍ��v����ꍇ�͍Đ�����܂��B
    //�@�����čĐ�����ꍇ�͗v�f���J��Ԃ��Đ������̂��A���̃X�e�[�g�ɂƂǂ܂�Ԃ̓��[�v����̂��Ȃǂ̍\���̓��̏��𗘗p���ď�����U�蕪���܂��B
    //�@
    //�@�Đ������ɂ��Ă͑����Đ��Ȃ�X�e�[�g���؂�ւ�����u�ԂɍĐ��A�Z�b�҂��čĐ��Ƃ����悤�ȑҋ@�Đ��̏ꍇ�̓A�j���[�V�����C�x���g�ŌĂяo���Ă��܂��B
    //�@���s�[�g�̏ꍇ�̓A�j���[�V�����C�x���g�ŌĂяo�����ۂ̕b�����i�[���A���̕b�����g���Ď������܂����B
    //�@�܂�X�e�[�g���ς��Ȃ�����A�񓯊������ł��Ȃ��b���҂��Ă܂��v�f���Đ��Ƃ����悤�ȃR�[�h���J��Ԃ��Ăяo�������Ă��܂��B
    //�@
    //�@�����āA�Đ����̍\���̂̒��Łu���ʃG�t�F�N�g/���ʉ����𗘗p����v�Ƃ����Ӗ���������bool�t���O���I���ɂȂ��Ă���ꍇ�́A���̃N���X���ꊇ�Ǘ����Ă��鋤�ʗv�f���Đ����Ă���܂��B
    //�@�Ⴆ�΃_�b�V���Ȃǂ̉���G�t�F�N�g�͗l�X�ȃL�����ŋ��ʂ��Ă��Ă����������͂Ȃ��̂ŁA���ʐݒ�Ƃ��邱�ƂŐݒ�̎�Ԃ�����܂��B
    //�@����ɑf�ނ����L���邱�Ƃł����ȃL�����N�^�[���悭�g���G�t�F�N�g���e�L�������]���Ƀv�[�����Ȃ��悤�ɂ��Ă���܂��B
    //�@
    //�@�@
    //�@�Ō�ɁA���ɃX�e�[�g���؂�ւ�����ۂɃ��[�v�Đ����Ă���v�f���~������A�X�e�[�g�̏I���Ɏg�p����ݒ�̉������Đ����Ă��̃X�e�[�g�̏����͏I���ƂȂ�܂��B
    //�@���͐؂�ւ������̃X�e�[�g�ŁA�����悤�ɏ������J��Ԃ��Ă����܂�
    //�@
    //�@�܂��A�\���̂̒��g��ς��邱�ƂŊȒP��GUI�ォ��Đ�����v�f��ύX�ł��܂�
    //
    //--------------------------------------------------------------
    //
    // �s�R�[�h���ŗp�����Ă���ق��̃N���X�ɂ��āt
    //
    //�@�EMyCode.SoundManager�N���X
    //�@���ʃG�t�F�N�g�Ƌ��ʃT�E���h���Ǘ����Ă���N���X�ł��B
    //�@���݂̃X�e�[�g��n���΂���ɍ����v�f���Đ����Ă����B
    //�@�������Ⴆ�΃_�b�V�����ȂǂŁA�����ԉ��x�������X�e�[�g�̗v�f�𐶐����Ă��炤�悤�ȏꍇ����������
    //  �O��Đ����Ă�������v�f�̖��O���L���b�V�����āA�����󋵂Ȃ�������f�����ł��̖��O�̗v�f���Đ�����悤�ɂ��Ă��܂��B
    //
    //  �EEffectCondition�N���X
    //�@���̃N���X�̓X�e�[�g���Ƃ̗v�f���Ǘ����邽�߂ɕK�v�ȃN���X�ŁA�傫�������ĎO�̃f�[�^�������Ă��܂��B
    //  �܂���̓G�t�F�N�g�̍Đ������f�[�^�i���ʑf�ނ��g���̂��A�����G�t�F�N�g�������Ăǂ��Đ�����̂��j�B
    //�@��ڂ��T�E���h�̍Đ������f�[�^�B
    //  �����čŌ�ɗv�f���ǂ̃X�e�[�g�ōĐ�����̂��Ƃ����f�[�^�ł��B
    //�@�X�e�[�g���؂�ւ�����ۂɂ́A���̃R�[�h�̓����ɂ���Effectcondition�N���X�̃��X�g���猻�݂̃X�e�[�g�ɍ��v����Effectcondition�����o�[��T��
    //�@�����݂������̂Ȃ�΂��̍Đ������Q�Ƃ��ėv�f�𗘗p���܂��B�@
    //      
    //  �ESpawnPool / PrefabPool�N���X�@
    //�@���̃N���X�̓A�Z�b�g�ŁA�I�u�W�F�N�g�v�[���V�X�e���̋@�\��񋟂���N���X�ł��B
    //  �r���Ńv�[���̓��e��ύX�ł���悤�ɉ������A�����ύX�Ȃǂɂ��Ή������Ă��܂��B
    //  SpawnPool���I�u�W�F�N�g���X�|�[���A�f�X�|�[������N���X�ŁAPrefabPool�N���X��SpawnPool�Ɋi�[����v�[���I�u�W�F�N�g�̃N���X�ɂȂ�܂��B
    //  �����▂�@��ύX�����ۂɂ͑������Ɏ�������PrefabPool�N���X���X�g����V���ȃv�[���𐶐����Ďg�p���܂��B
    [AddComponentMenu("Corgi Engine/Character/Abilities/EffectControllAbility")]
    public class EffectControllAbility : MyAbillityBase, MMEventListener<MMStateChangeEvent<CharacterStates.CharacterConditions>>,
        MMEventListener<MMStateChangeEvent<CharacterStates.MovementStates>>
    {

        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        public override string HelpBoxText() { return "���̋@�\�͉�����G�t�F�N�g���Đ����邽�߂̋@�\�ł��B"; }


        //�t�B�[���h
        //�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\-�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\
#region

        #region ��`

        /// <summary>
        /// ���݃v���C���[���ǂ̂悤�ȏ�Ԃɂ���̂������邽�߂̗񋓌^
        /// </summary>
        public enum SelectState
        {
            Idle,
            moving,//�ʏ�ړ�
            Falling,
            Running,
            Crouching,
            Crawling,

            Jumping,
            DoubleJumping,
            Flying,
            FastFlying,
            Rolling,
            Attack,
            Guard,
            GuardMove,
            Warp,
            Parry,
            justGuard,
            Cast,
            Combination,
            Frozen,
            BlowDead,
            Dead,
            Faltter,
            Blow,
            Wakeup,
            GBreake,//�K�[�h�u���C�N
            Null
        }

        #endregion



        #region �C���X�y�N�^�Őݒ�

        /// <summary>
        /// �G�t�F�N�g�𐶐�����N���X
        /// �I�u�W�F�N�g�v�[���@�\
        /// Spawn�Ő����B�G�t�F�N�g�̍Đ����I������Ǝ����ŏ���
        /// </summary>
        [SerializeField]
        SpawnPool particlesPool;


        /// <summary>
        /// �O����͕ҏW�ł����A�ݒ�̓��\�b�h���g���čs���悤�ɂ���
        /// �G�t�F�N�g�≹���g���X�e�[�g�ƍĐ����@�̃��X�g
        /// </summary>
        [Header("���ʂ��g���X�e�[�g�Ǝg�����̐ݒ�")]
        [SerializeField]
        private List<EffectCondition> _stateList;

        [Header("�Z���Ă邩�ǂ���")]
        /// <summary>
        /// �Z���Ă邩�ǂ���
        /// ���Ă���Ƌ��ʃT�E���h�������n�̉��ɂȂ�
        /// </summary>
        public bool isMetal;//�Z���Ă邩�ǂ���

        [Header("�̂̃T�C�Y")]
        /// <summary>
        /// �̂̑傫��
        /// �傫���Ƌ��ʃT�E���h��G�t�F�N�g���ω�����
        /// </summary>
        public MyCode.SoundManager.SizeTag bodySize;

        [Header("�G�t�F�N�g�̃T�C�Y�{��")]
        /// <summary>
        /// �G�t�F�N�g�̃T�C�Y�{��
        /// </summary>
        public float sizeMultipler = 1;


        [Header("�G�t�F�N�g�����|�C���g")]
        /// <summary>
        ///�ǂ��ŃG�t�F�N�g�𐶐����邩
        /// </summary>
        public Transform[] effecter;


        #endregion



        #region�@�����X�e�[�^�X

        /// <summary>
        /// ���ݍĐ��ɗ��p���Ă���X�e�[�g
        /// </summary>
        SelectState _useState = SelectState.Null;

        /// <summary>
        /// �A�j���̍Đ����x���i�[������
        /// �Đ����x����e�����󂯂�Ȃ�
        /// </summary>
        float speedMultipler = 1;

        /// <summary>
        /// �����ݗ��p���̑f�ނ��Ǘ����邽�߂̃��X�g
        /// </summary>
        [SerializeField]
        List<EffectCondition.StateEffect> _waitEffect = new List<EffectCondition.StateEffect>();

        /// <summary>
        /// ���[�v�G�t�F�N�g
        /// �X�e�[�g�̕ς��ڂŔj�����邽�߂ɕێ�����
        /// </summary>
        List<ParticleSystem> _loopEffect = new List<ParticleSystem>();

        /// <summary>
        /// �����ݗ��p���̉����Ǘ����邽�߂̃��X�g
        /// </summary>
        List<EffectCondition.StateSound> _waitSound = new List<EffectCondition.StateSound>();

        /// <summary>
        /// ���ʐݒ�̍Đ����s���ۂɂǂ��Đ����邩����
        /// </summary>
        [SerializeField]
        EffectCondition.EmitType generalEType = EffectCondition.EmitType.None;

        /// <summary>
        /// ���ʐݒ�̍Đ����s���ۂɂǂ��Đ����邩����
        /// </summary>
        EffectCondition.EmitType generalSType = EffectCondition.EmitType.None;


        //�O��̍Đ�����ۑ����邱�ƂŖ���������f���J��Ԃ��Ȃ��悤�ɂ���L���b�V��
        MyCode.SoundManager.PreviousEffect prevE = new MyCode.SoundManager.PreviousEffect();
        MyCode.SoundManager.PreviousSound prevS = new MyCode.SoundManager.PreviousSound();


        /// <summary>
        /// �҂����Ԍv���p
        /// </summary>
        float waitTimer;

        /// <summary>
        /// ���ʃT�E���h�����łɌ��݂̃X�e�[�g�ŗ��p�������ǂ���
        /// </summary>
        bool pubSUsed;

        /// <summary>
        /// ���ʃG�t�F�N�g�����łɗ��p�������ǂ���
        /// </summary>
        bool pubEUsed;


        /// <summary>
        /// ��x�����g����L�[
        /// ��Ԃ��ω�����Ƃ��ɕς��
        /// </summary>
        int oneTimeKey = 0;


        #endregion



#endregion


        //���\�b�h
        //�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\-�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\
        #region

        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();

            //�T�E���h�}�l�[�W���[�ɓn���X�e�[�g��񂪏��񂩂獇�v���Ȃ��悤��
            //���肦�Ȃ����������Ă���
            prevE.state = 10000;
            prevS.state = 10000;
            
        }
        
         public override void ProcessAbility()
        {
            base.ProcessAbility();




            //���n�����g���K�[
            //������΂���Ă���̂Ȃ�n�ʂɂ������_�ŋN���オ��ɏ�ԕύX
            if (_controller.State.JustGotGrounded)
            {
                //������΂����ɐڒn�����炿���ƋN���オ���
                if (_useState == SelectState.Blow)
                {
                    _useState = SelectState.Wakeup;

                    //�O�̃X�e�[�g�̃G�t�F�N�g�Ȃǂ�����
                    EffectCheck();
                    
                    SoundCheck();
                    UseSelect(_useState);
                }
                //���񂾏�Ԃł̐�����΂�
                else if (_useState == SelectState.BlowDead)
                {
                    _useState = SelectState.Dead;


                    //�O�̃X�e�[�g�̃G�t�F�N�g�Ȃǂ�����
                    EffectCheck();
                    SoundCheck();
                    UseSelect(_useState);
                }
                //���n���ƃG�t�F�N�g���g�p����
                //�����U�����␁����΂�����̒��n�̏ꍇ����G�t�F�N�g���ς��
                MyCode.SoundManager.instance.GotGround(effecter[0],_useState,isMetal,bodySize,_character.nowGround);
            }
        }



        ///<summary>
        /// �C�x���g�V�X�e���֘A�̏���
        /// �X�e�[�g�̐؂�ւ�莞�ɂ��̃X�e�[�g�ōĐ����鉹����G�t�F�N�g���Ăяo��
        /// </summary>
        #region
        protected override void OnEnable()
        {
            base.OnEnable();
            this.MMEventStartListening<MMStateChangeEvent<CharacterStates.CharacterConditions>>();
            this.MMEventStartListening<MMStateChangeEvent<CharacterStates.MovementStates>>();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            this.MMEventStopListening<MMStateChangeEvent<CharacterStates.CharacterConditions>>();
            this.MMEventStopListening<MMStateChangeEvent<CharacterStates.MovementStates>>();
        }


        /// <summary>
        /// ����X�^���Ȃǂ̏�ԕω���ǂݎ��C�x���g
        /// </summary>
        /// <param name="eventType"></param>
        public void OnMMEvent(MMStateChangeEvent<CharacterStates.CharacterConditions> eventType)
        {
            //��Ԃ��ω������̂Ȃ�V������ԂōĐ��v�f��T���B
            if (eventType.Target == this.gameObject)
            {
                StateSelect(CharacterStates.MovementStates.Nostate, eventType.NewState);
            }
        }

        /// <summary>
        /// �_�b�V����W�����v�Ȃǂ̍s���̏�ԕω���ǂݎ��C�x���g
        /// </summary>
        /// <param name="eventType"></param>
        public void OnMMEvent(MMStateChangeEvent<CharacterStates.MovementStates> eventType)
        {
            if (eventType.Target == this.gameObject) 
            {
                //��Ԃ��ω������̂Ȃ�V������ԂōĐ��v�f��T���B
                StateSelect(eventType.NewState);

            }
        }

        #endregion

        ///<summary>
        /// �X�e�[�g�̐؂�ւ�莞�ɁA���̃X�e�[�g�ōĐ�����G�t�F�N�g�≹��I�Ԃ��߂̏���
        /// </summary>
        #region


        /// <summary>
        /// �܂��̓R�[�M�[�G���W���̃X�e�[�g�����̃R�[�h�Ŏg������̂ɕϊ�����
        /// </summary>
        /// <param name="_mState"></param>
        /// <param name="_cState"></param>
        void StateSelect(CharacterStates.MovementStates _mState = CharacterStates.MovementStates.Nostate,
            CharacterStates.CharacterConditions _cState = CharacterStates.CharacterConditions.Normal)
        {

            SelectState newState = SelectState.Null;

            if (_mState != CharacterStates.MovementStates.Nostate)
            {
                newState = (SelectState)_mState;
            }

           // Debug.Log($"�ނ���{_movement.PreviousState}����{_movement.CurrentState}");
            //��Ԉُ�̃X�e�[�g�͓���̃X�e�[�g�ɗD�悷��̂�
            //��Ԉُ킪����̂Ȃ�useState���㏑��

            //�X�^������Health����X�^���̎�ނ��擾
            if (_cState == CharacterStates.CharacterConditions.Stunned)
            {
                SelectState container;
                container = _health.GetStunState();

                if(container == SelectState.Null)
                {
                    return;
                }
                newState = container;
                MyCode.SoundManager.instance.StanEffect(effecter[0],newState,_condition.PreviousState == CharacterStates.CharacterConditions.Stunned,sizeMultipler);
                
            }
            else if (_cState == CharacterStates.CharacterConditions.Dead)
            {
               newState = _health.GetStunState() == SelectState.BlowDead ? SelectState.BlowDead : SelectState.Dead;
            }

            if(newState == SelectState.Null || _useState == newState)
            {
                return;
                
            }
            //�O�̃X�e�[�g�̃G�t�F�N�g�Ƃ�������
            EffectCheck();
            SoundCheck();

            _useState = newState;

            //���݂̎��Ԃ��L�^�B���s�[�g�����Ɏg��
            waitTimer = Time.time;

            //�������L�^���Č��݂̏�Ԃ��ŗL�̂��̂�
            int keyNum = RandomValue(0,100);
            if (keyNum == oneTimeKey)
            {
                oneTimeKey *= 2;
            }
            else
            {
                oneTimeKey = keyNum;
            }

 
           
            //���݂̃X�e�[�g�Ɋ�Â��g�p�ł���v�f�����邩�T��
            UseSelect(_useState);
        }

        /// <summary>
        /// ���݂̃X�e�[�g�ŁA�g�p�ł���G�t�F�N�g�≹������Ȃ珈�����J�n����
        /// </summary>
        protected void UseSelect(SelectState state)
        {
            for (int i = 0; i < _stateList.Count;i++)
            {

 

                //���X�g����Effectcondition�̎g�p����X�e�[�g�����݂̃X�e�[�g�ɍ��v����Ȃ�
                if (state == _stateList[i]._useState)
                {

                    //���݂̃A�j���̍Đ����x����Ă���
                    //����G�t�F�N�g�̍Đ����x���A�j���̍Đ����x�ɍ��킹��悤�ɂȂ��Ă���̂Ȃ�g�p����
                    speedMultipler =�@_character._animator.GetCurrentAnimatorStateInfo(0).speed;

                    //���ʐݒ�g���Ȃ�
                    if (_stateList[i].generalEffect)
                    {
                        GeneralEPlay(state);
                    }

                    //�ŗL�G�t�F�N�g����ȏ�o�^����Ă�Ȃ�
                    else if (_stateList[i]._stateEffects.Any())
                    {
                        float count = _stateList[i]._stateEffects.Count;
                        if (count == 1)
                        {
                            EffectStart(_stateList[i]._stateEffects[0]);
                        }
                        else
                        {
                            for (int s = 0; s < count; s++)
                            {
                                EffectStart(_stateList[i]._stateEffects[s]);
                            }

                        }

                    }
                    

                    //���ʐݒ�g���Ȃ�
                    if (_stateList[i].generalSound)
                    {
                        GeneralSPlay(state);
                    }
                    //�ŗL�̉�����ȏ�o�^����Ă�Ȃ�
                    else if (_stateList[i]._stateSounds.Any())
                    {
                        float count = _stateList[i]._stateSounds.Count;
                        if (count == 1)
                        {
                            SoundStart(_stateList[i]._stateSounds[0]);
                        }
                        else
                        {
                            for (int s = 0; s < count; s++)
                            {
                                SoundStart(_stateList[i]._stateSounds[s]);
                            }

                        }
                    }

                    //��ł����v�����Ԃ�����Έȍ~�̃��[�v�͉񂳂Ȃ�
                    break;
                }
            }
        }

        #endregion

        ///<summary>
        /// �g�p���鉹��G�t�F�N�g�ɁA�����Đ���ҋ@�Đ��Ȃǂ̐����ɏ]���ď�����U�蕪���鏈��
        /// </summary>
        #region�@�X�e�[�g�Ŏg�p����v�f���Đ������ɏ]���čĐ����鏈��

        /// <summary>
        /// �G�t�F�N�g���Đ����鏈��
        /// </summary>
        protected void EffectStart(EffectCondition.StateEffect _condition)
        {

            if (_condition._emitType == EffectCondition.EmitType.Soon)
            {
                //�����ɍĐ��Ȃ炱���ŃG�t�F�N�g���o��
                EffectSpawn(_condition);
            }
            else
            {
                // �Ǘ����X�g�ɓ����
                _waitEffect.Add(_condition);

                //���s�[�g�̏ꍇ�܂������Ŗ炵�āA���ڂ̃^�C�~���O���A�j�����Ŏw�肷��
                //���ڂ����͎���waitRepeat�Ɠ��������ɂȂ�
                if (_condition._emitType == EffectCondition.EmitType.Repeat)
                {
                    EffectSpawn(_condition);
                }
                //���[�v�Ȃ�ǉ����Ă���
                if (_condition._emitType == EffectCondition.EmitType.Loop)
                {
                    _loopEffect.Add(EffectSpawn(_condition));
                }
            }
        }

        /// <summary>
        /// ���ʃG�t�F�N�g���Đ����鏈��
        /// </summary>
        protected void GeneralEPlay(SelectState _state)
        {
            generalEType = EffectCondition.EmitType.Soon;

            //���ʑf�ނł͍Đ��������X�e�[�g���ƂɎw�肷��
�@�@�@�@�@�@if (_state == SelectState.moving || _state == SelectState.Running || _state == SelectState.FastFlying ||
                _state == SelectState.Flying || _state == SelectState.Crawling || _state == SelectState.GuardMove)
            {
                generalEType = EffectCondition.EmitType.Repeat;
            }

            if (generalEType != EffectCondition.EmitType.Soon || generalEType != EffectCondition.EmitType.Repeat)
            {
                //�Đ�����Ɠ����Ɏg�p�����f�ނ̏���ێ����Ď���̍Đ��Ɏg��
                prevE = MyCode.SoundManager.instance.GeneralEffectPlay(effecter[0],_state,bodySize,_character.nowGround,prevE,sizeMultipler,transform.localScale.x > 0);
            }

        }

        /// <summary>
        /// �����Đ�������Ǘ��J�n������
        /// </summary>
        protected void SoundStart(EffectCondition.StateSound _condition)
        {
            if (_condition._playType == EffectCondition.EmitType.Soon)
            {
                //�����ŉ��炷
                SoundPlay(_condition);
            }
            else
            {
                if(_waitSound == null)
                {
                    _waitSound = new List<EffectCondition.StateSound>();
                }

                // �Ǘ����X�g�ɓ����
                _waitSound.Add(_condition);

                //���s�[�g�̏ꍇ�܂������Ŗ炵�āA���ڂ̃^�C�~���O���A�j�����Ŏw�肷��
                //���ڂ����͎���waitRepeat�Ɠ��������ɂȂ�
                if (_condition._playType == EffectCondition.EmitType.Repeat || _condition._playType == EffectCondition.EmitType.Loop)
                {
                    //�A�j���̍Đ����x�ɍ��킹�Ȃ��Ȃ�
                    SoundPlay(_condition);
                }
            }
        }

        /// <summary>
        /// �����Đ�������Ǘ��J�n������
        /// ���[�v�͂Ȃ���
        /// </summary>
        protected void GeneralSPlay(SelectState _state)
        {
            generalSType = EffectCondition.EmitType.Soon;
            if (_state == SelectState.moving || _state == SelectState.Running || _state == SelectState.FastFlying ||
                _state == SelectState.Flying || _state == SelectState.Crawling || _state == SelectState.GuardMove)
            {
                generalSType = EffectCondition.EmitType.Repeat;
            }

            if (generalSType != EffectCondition.EmitType.Soon || generalSType != EffectCondition.EmitType.Repeat)
            {
                prevS =  MyCode.SoundManager.instance.GeneralSoundPlay(effecter[0],_state, speedMultipler, isMetal, bodySize, _character.nowGround,prevS);
            }
        }

        #endregion


        ///<summary>
        /// �X�e�[�g�̐؂�ւ�莞�ɁA�O�̃X�e�[�g�Ŏg�p���Ă�������G�t�F�N�g���~�������肷�邽�߂̏���
        /// </summary>
        #region�@�X�e�[�g�I�����̂��Ə���


        /// <summary>
        /// �X�e�[�g���ς�������̃C�x���g�ŌĂ�
        /// ���[�v�G�t�F�N�g�̍Đ��̏I���ƍŌ�ɖ炷�G�t�F�N�g�̍Đ�
        /// </summary>
        void EffectCheck()
        {
            //���ʑf�ނ��g�p�����ꍇ�̌㏈��
            if (generalEType != EffectCondition.EmitType.None)
            {
                
                Debug.Log($"������������{generalEType}{_movement.PreviousState}");
                //�X�e�[�g�̏I���Ɏg�p����f�ނł���̂Ȃ�
                if (generalEType == EffectCondition.EmitType.End)
                {
                    prevE = MyCode.SoundManager.instance.GeneralEffectPlay(effecter[0],_useState,  bodySize, _character.nowGround,prevE,sizeMultipler, transform.localScale.x > 0);

                }
                generalEType = EffectCondition.EmitType.None;

                //���ʑf�ޗ��p�t���O�𗘗p�O�ɖ߂�
                pubEUsed = false;
            }
           // �ŗL�f�ނ��g�p�����ꍇ�̌㏈��
                //�Đ����A�ҋ@���̃G�t�F�N�g����ł�����Ȃ�
                if (_waitEffect.Any())
                {
                    for (int i = 0; i < _waitEffect.Count; i++)
                    {
                        //���g�p��Ԃɖ߂�
                        _waitEffect[i].isUsed = false;

                        //�Ō�ɏo���f�ނ͍��o��
                        if (_waitEffect[i]._emitType == EffectCondition.EmitType.End)
                        {
                            EffectSpawn(_waitEffect[i]);
                        }
                    }
                    //�G�t�F�N�g�̏������I������烊�X�g�̂��|��
                    _waitEffect.Clear();
                }

                //���[�v���̃G�t�F�N�g����ł�����Ȃ�
                if (_loopEffect.Any())
                {
                    for (int i = 0; i < _loopEffect.Count; i++)
                    {
                        particlesPool.Despawn(_loopEffect[i].transform);
                    }
                    //�G�t�F�N�g�̏������I������烊�X�g�̂��|��
                    _loopEffect.Clear();
                }


        }

        /// <summary>
        /// ������X�e�[�g���ς�������̃C�x���g�ŌĂ�
        /// ���[�v���̍Đ��̏I���ƍŌ�ɖ炷���̍Đ�
        /// </summary>
        void SoundCheck()
        {
            //���ʑf�ނ��g�p�����ꍇ�̌㏈��
            if (generalSType != EffectCondition.EmitType.None)
            {
                if (generalSType == EffectCondition.EmitType.End)
                {
                    prevS = MyCode.SoundManager.instance.GeneralSoundPlay(effecter[0], _useState, speedMultipler, isMetal, bodySize, _character.nowGround, prevS);
                }

                generalSType = EffectCondition.EmitType.None;

                //���ʑf�ޗ��p�t���O�𗘗p�O�ɖ߂�
                pubSUsed = false;
            }
            else
            {
                //�Đ����A�ҋ@���̉�����ł�����Ȃ�
                if (_waitSound.Any())
                {
                    for (int i = 0; i < _waitSound.Count; i++)
                    {
                        //���g�p��Ԃɖ߂�
                        _waitSound[i].isUsed = false;

                        //���s�[�g�̉����t�F�C�h������
                        if (_waitSound[i]._playType == EffectCondition.EmitType.Loop || _waitSound[i]._playType == EffectCondition.EmitType.WaitLoop)
                        {
                            GManager.instance.StopSound(_waitSound[i]._useSound);
                        }
                        //�Ō�ɖ炷��Ȃ獡�炷
                        else if (_waitSound[i]._playType == EffectCondition.EmitType.End)
                        {
                            SoundPlay(_waitSound[i]);
                        }
                    }
                    //���̏������I������烊�X�g�̑|��
                    _waitSound.Clear();

                }
            }
        }
        #endregion

        ///<summary>
        /// ����G�t�F�N�g�̍Đ����ĂԃA�j���[�V�����C�x���g�Ƃ��̊֘A����
        /// ���s�[�g�̏ꍇ�����Ԋu�ňȌ�Đ����J��Ԃ�
        /// </summary>
        #region

        /// <summary>
        /// �A�j���C�x���g
        /// ���̃i���o�[�̓��X�g�ɂ����đΏۂ����Ԗڂ̗v�f�ł��邩�Ƃ�������
        /// </summary>
        /// <param name="number"></param>
        public void EffectStartEvent(int number)
        {

      

            //���ꂪ�m������Ȃ��Ȃ狤�ʂōĐ�
            if (generalEType != EffectCondition.EmitType.None)
            {
                //���łɋ��ʑf�ނ𗘗p�����Ȃ�߂�
                //�A�j���C�x���g����΂Ȃ��悤�����ݒ肵�Ă����x�������G�t�F�N�g�͌ĂׂȂ�
                if (pubEUsed)
                {
                    return;
                }

                pubEUsed = true;

                prevE = MyCode.SoundManager.instance.GeneralEffectPlay(effecter[0],_useState,   bodySize, _character.nowGround,prevE,sizeMultipler, transform.localScale.x > 0);

                //���s�[�g�������J�n
                if (generalEType == EffectCondition.EmitType.Repeat || generalEType == EffectCondition.EmitType.WaitRepeat)
                {
                    GeneralERepeat(effecter[0], _useState, speedMultipler, isMetal, bodySize, _character.nowGround, prevS, Time.time - waitTimer, oneTimeKey).Forget();
                }
            }
            else
            {
                //�g�p�ς݃G�t�F�N�g�Ȃ�߂�
                //�A�j���C�x���g����΂Ȃ��悤�����ݒ肵�Ă����x�������G�t�F�N�g�͌ĂׂȂ�
                if (_waitEffect == null || !_waitEffect.Any() || _waitEffect[number].isUsed)
                {
                    return;
                }


                _waitEffect[number].isUsed = true;

                //���[�v�Ȃ�ǉ����Ă���
                if (_waitEffect[number]._emitType == EffectCondition.EmitType.WaitLoop)
                {
                    //�A�j���̍Đ����x�ɍ��킹�Ȃ��Ȃ�
                    _loopEffect.Add(EffectSpawn(_waitEffect[number]));
                }
                else
                {
                    //�G�t�F�N�g���Đ�
                    EffectSpawn(_waitEffect[number]);
                }

                //���s�[�g����G�t�F�N�g�Ȃ�
                if (_waitEffect[number]._emitType == EffectCondition.EmitType.Repeat || _waitEffect[number]._emitType == EffectCondition.EmitType.WaitRepeat)
                {
                    //�J��Ԃ��f�ނ̍Đ��n��
                    EffectRepeat(_waitEffect[number], Time.time - waitTimer, _useState, oneTimeKey).Forget();
                }
            }
        }

        /// <summary>
        /// �A�j���C�x���g
        /// ���̃i���o�[�̓��X�g�ɂ����đΏۂ����Ԗڂ̗v�f�ł��邩�Ƃ�������
        /// </summary>
        /// <param name="number"></param>
        public void SoundStartEvent(int number)
        {
            EffectCondition.EmitType type;

            //���ꂪ�m������Ȃ��Ȃ狤�ʂōĐ�
            if (generalSType != EffectCondition.EmitType.None)
            {
                //���łɋ��ʑf�ނ𗘗p�����Ȃ�߂�
                if (pubSUsed)
                {
                    return;
                }

                pubSUsed = true;

                prevS = MyCode.SoundManager.instance.GeneralSoundPlay(effecter[0],_useState, speedMultipler, isMetal, bodySize, _character.nowGround,prevS);


                if (generalSType == EffectCondition.EmitType.Repeat || generalSType == EffectCondition.EmitType.WaitRepeat)
                {

                    GeneralSRepeat(effecter[0], _useState, speedMultipler, isMetal, bodySize, _character.nowGround, prevS, Time.time - waitTimer,oneTimeKey).Forget();
                }
            }
            else
            {
                //�g�p�ς݃T�E���h�Ȃ�߂�
                if (_waitSound ==null || !_waitSound.Any() || _waitSound[number].isUsed)
                {
                    return;
                }

                _waitSound[number].isUsed = true;


                //����炷
                SoundPlay(_waitSound[number]);
                type = _waitSound[number]._playType;

                //���s�[�g���鉹�Ȃ�
                if (type == EffectCondition.EmitType.Repeat || type == EffectCondition.EmitType.WaitRepeat)
                {
                    //�J��Ԃ��f�ނ̍Đ��n��
                    SoundRepeat(_waitSound[number], Time.time - waitTimer, _useState, oneTimeKey).Forget();
                }

            }


        }




        #endregion


        #region�@�Đ��ƃ��s�[�g�Đ�����


        /// <summary>
        /// X��Y�̊Ԃŗ������o��
        /// �����̓��s�[�g�����Ŋm�F�̂��߂Ɏg��
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public int RandomValue(int X, int Y)
        {

            return UnityEngine.Random.Range(X, Y + 1);

        }


        /// <summary>
        /// �G�t�F�N�g�̃X�e�[�^�X�ɍ��킹�ēK�؂ɐ�������
        /// </summary>
        ParticleSystem EffectSpawn(EffectCondition.StateEffect effect)
        {
            //�G�t�F�N�g���o�������ꏊ���w�肵�Ȃ��Ȃ�W���̃G�t�F�N�g�����n�_���g��
            Transform posi = effecter[effect._emitPosition];

                  ParticleSystem ef;
            //�A�j���̍Đ����x�ɍ��킹�Ȃ��Ȃ�
            if (!effect._matchAnime)
            {
                
                
                if (effect._isFollow)
                {
                    ef = particlesPool.ControlSpawn(effect._useEffect,posi.position, posi.rotation, posi);
                }
                else
                {
                    ef = particlesPool.Spawn(effect._useEffect, posi.position, posi.rotation);
                }

                var main = ef.main;
                main.simulationSpeed = speedMultipler;
            }
            else
            {
                if (effect._isFollow)
                {
                    ef = particlesPool.ControlSpawn(effect._useEffect, posi.position, posi.rotation, posi);
                }
                else
                {
                    ef = particlesPool.Spawn(effect._useEffect, posi.position, posi.rotation);
                }
            }


            Vector3 ls = ef.transform.localScale;

            //�����������ĂĒǏ]�����ĂȂ��Ȃ�
            //���̂����ł͕��������Ŕ��]����̂��Ƃ����ƁA����̓L�����̃��[�g���ɃG�t�F�N�g��������R���|�[�l���g
            //�����邹���Ńv���C���[�̃��[�J���X�P�[���ɃG�t�F�N�g���e�����󂯂Ă��܂�����
            if (effect.ignoreDirection && !effect._isFollow && transform.localScale.x < 0)
            {
              
                ls.x = ls.x * -1;
            }

            //�T�C�Y�{��������Ȃ�
            if (sizeMultipler != 1)
            {
                ls *= sizeMultipler;
            }
            ef.transform.localScale = ls;

            return ef;
        }


        /// <summary>
        /// ���̃X�e�[�^�X�ɍ��킹�ēK�؂ɖ炷
        /// </summary>
        /// <param name="sound"></param>
        void SoundPlay(EffectCondition.StateSound sound)
        {
            //�A�j���̍Đ����x�ɍ��킹�Ȃ��Ȃ�
            if (!sound._matchAnime)
            {
                if (sound._isFollow)
                {
                    GManager.instance.FollowSound(sound._useSound, transform);
                }
                else
                {
                    GManager.instance.PlaySound(sound._useSound, transform.position);
                }
            }
            else
            {
                if (sound._isFollow)
                {
                    GManager.instance.FollowSound(sound._useSound, effecter[0], pitch: speedMultipler);
                }
                else
                {
                    GManager.instance.PlaySound(sound._useSound, transform.position, pitch: speedMultipler);
                }
            }
        }



        /// <summary>
        /// �X�e�[�g�ɂƂǂ܂邩����G�t�F�N�g���J��Ԃ�
        /// </summary>
        /// <param name="effect">�g�p����G�t�F�N�g�̏��</param>
        /// <param name="waitTime">�Đ��Ԋu</param>
        /// <param name="_state">���݂̏��</param>
        /// <returns></returns>
        async UniTaskVoid EffectRepeat(EffectCondition.StateEffect effect, float waitTime, SelectState _state,int key)
        {

            var token = this.GetCancellationTokenOnDestroy();


            //�w��b���҂�
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);


            //���s�[�g���Ă�G�t�F�N�g�̃X�e�[�g���ێ�����Ă����͌J��Ԃ�������
            //������Ԃł��������ς���Ă��āA��x��Ԃ��X�V����Ă�Ȃ�r��
            if (_state == _useState && oneTimeKey == key)
            { 
                EffectSpawn(effect);
                EffectRepeat(effect, waitTime, _state,key).Forget();
            }


        }



        async UniTaskVoid GeneralERepeat(Transform pos, SelectState state, float multipler, bool isMetal, MyCode.SoundManager.SizeTag _size, MyCharacter.GroundFeature ground, MyCode.SoundManager.PreviousSound prevS, float waitTime,int key)
        {
            var token = this.GetCancellationTokenOnDestroy();

            //�w��b���҂�
            await (UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token));

            //���s�[�g���Ă�G�t�F�N�g�̃X�e�[�g���ێ�����Ă����͌J��Ԃ�������
            //������Ԃł��������ς���Ă��āA��x��Ԃ��X�V����Ă�Ȃ�r��
            if (state == _useState && oneTimeKey == key)
            {
               
                prevE = MyCode.SoundManager.instance.GeneralEffectPlay(effecter[0], _useState, bodySize, _character.nowGround, prevE, sizeMultipler, transform.localScale.x > 0);
                GeneralERepeat(pos, state, multipler, isMetal, _size, ground, prevS, waitTime,key).Forget();
            }

        }



        /// <summary>
        /// �X�e�[�g�ɂƂǂ܂邩���艹�����J��Ԃ�
        /// </summary>
        /// <param name="sound"></param>
        /// <param name="waitTime"></param>
        /// <param name="_state"></param>
        /// <returns></returns>
        async UniTaskVoid SoundRepeat(EffectCondition.StateSound sound, float waitTime, SelectState _state,int key)
        {

            var token = this.GetCancellationTokenOnDestroy();

            //�w��b���҂�
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime),cancellationToken:token);

            //���s�[�g���Ă鉹���̃X�e�[�g���ێ�����Ă����͌J��Ԃ�������
            //������Ԃł��������ς���Ă��āA��x��Ԃ��X�V����Ă�Ȃ�r��
            if (_state == _useState && oneTimeKey == key)
            {
                SoundPlay(sound);
                SoundRepeat(sound,waitTime,_state,key).Forget();
            }
        }

        async UniTaskVoid GeneralSRepeat(Transform pos, SelectState state, float multipler, bool isMetal, MyCode.SoundManager.SizeTag _size, MyCharacter.GroundFeature ground, MyCode.SoundManager.PreviousSound prevS,float waitTime,int key)
        {
            var token = this.GetCancellationTokenOnDestroy();

            //�w��b���҂�
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);

            //���s�[�g���Ă鉹���̃X�e�[�g���ێ�����Ă����͌J��Ԃ�������
            //������Ԃł��������ς���Ă��āA��x��Ԃ��X�V����Ă�Ȃ�r��
            if (state == _useState && oneTimeKey == key)
            {            
                prevS = MyCode.SoundManager.instance.GeneralSoundPlay(effecter[0], _useState, speedMultipler, isMetal, bodySize, _character.nowGround, prevS);
                GeneralSRepeat(pos,state,multipler,isMetal,_size,ground,prevS,waitTime,key).Forget();
            }
        }




        #endregion


        ///<summary>
        /// �G�t�F�N�g�≹�����Z�b�g������̊Ǘ��R�[�h
        /// </summary>
        #region


        ///<summary>
        ///  �Đ����鉹��G�t�F�N�g�����Z�b�g����
        ///  
        /// </summary>
        public void ResorceReset(List<EffectCondition>�@_newList,List<PrefabPool> _newPrefab)
        {
            //�G�t�F�N�g�����Z�b�g
            particlesPool.CleanUp();
            _stateList.Clear();

            if (_newList.Any())
            {
                _stateList = _newList;
            }

            //�X�e�[�g���Ĕ��f
            prevE.state = 10000;
            prevS.state = 10000;

            if (_newPrefab.Any())
            {
                for (int i = 0; i < _newPrefab.Count; i++)
                {

                    particlesPool.CreatePrefabPool(particlesPool.ObjectSetting(_newPrefab[i]));
                    
                }

            }

            
        }


        #endregion



        #endregion


    }
}
