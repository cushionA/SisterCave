using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using UnityEngine.AddressableAssets;
using Rewired.Integration.CorgiEngine;
using DarkTonic.MasterAudio;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// TODO_DESCRIPTION
    /// </summary>
  //  [AddComponentMenu("Corgi Engine/Character/Abilities/Rewight")]
    public class RewrightAttack : MyAbillityBase
    {

        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "TODO_HELPBOX_TEXT."; }

        //   [Header("����f�[�^")]
        /// declare your parameters here
        ///WeaponHandle�Q�l�ɂ��� 


        // Animation parameters
        //       protected const string _attackParameterName = "AttackNow";
        //     protected int _attackAnimationParameter;

        protected const string _typeParameterName = "AttackType";
        protected int _typeAnimationParameter;

        protected const string _numberParameterName = "AttackNumber";
        protected int _numberAnimationParameter;

        public enum ActType
        {
            noAttack,//�Ȃɂ��Ȃ�
            sAttack,
            bAttack,
            cCharge,
            cAttack,//�`���[�W����
            aAttack,//�󒆎�
            fAttack,//�󒆋�
            arts,
            magic

        }

        ActType atType;


        //�����p�����[�^
        #region
        //------------------------------------------�����p�����[�^



        /// <summary>
        /// ����̗��莝���؂�ւ�������Ă邩�ǂ���
        /// </summary>
        bool changeKey;


        /// <summary>
        /// ���߉������ԑ�����ꕨ
        /// </summary>
        float chargeTime;
        //   float gravity;//�d�͂�����

        bool isAirEnd;//�󒆎�U������܂łɐ���


        // float delayTime;
        int attackNumber;

        //���ݍU���̔ԍ����ǂꂩ
        int nowNumber;

        //Animator anim;
        [HideInInspector] public bool isAttackable;
        bool smallTrigger;
        bool bigTrigger;
        bool artsTrigger;
        bool MagicTrigger;
        //�A���̃g���K�[�ɂȂ�



        //����ύX��̗���؂�ւ��\���\�h
        bool equipChange;
        //�`���[�W��
        float horizontalKey;
        bool anyKey;
        float attackDirection;


        /// <summary>
        /// �R���{�U��؂���
        /// </summary>
        bool isComboEnd;

        //�R���{�����B�o�����[�̐�
        int comboLimit;


        /// <summary>
        /// ���U����ɕ����]�����ړ������f����
        /// </summary>
        float afterTime;

        float groundTime;

        public float afterJudge = 0.35f;

        //   Rigidbody2D GManager.instance.pm.rb;

        /// <summary>
        /// �ق�Ƃɗ����n�߂�t���O
        /// </summary>
        bool startFall;
        // Vector3 theScale = new Vector3();
        bool isParring;//�p���B��
                       // Start is called before the first frame update

        /// <summary>
        /// �^�Ȃ�`���[�W���J�n����
        /// </summary>
        bool isCharging;


        bool fire1Key;
        bool fire2Key;
        bool artsKey;

        #endregion

        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();

            // randomBool = false;
        }


        /// <summary>
        /// 1�t���[�����ƂɁA���Ⴊ��ł��邩�ǂ����A�܂����Ⴊ��ł���ׂ������`�F�b�N���܂�
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();

            //���͂Ɋ�Â��ē���
            InputReceiver();

            if (atType != ActType.noAttack)
            {
                AttackFallController();
                //     NumberControll();

                if (_condition.CurrentState == CharacterStates.CharacterConditions.Stunned)
                {
                    GManager.instance.isAttack = false;
                    _movement.ChangeState(CharacterStates.MovementStates.Idle);
                    attackNumber = 0;
                    atType = ActType.noAttack;
                    startFall = false;
                    isDisenable = false;
                    isComboEnd = false;
                    isAttackable = false;
                }

                if (isAttackable)
                {
                    AttackCheck();
                    AnimationEndReserch();
                }
                
            }

            Debug.Log($"���팟���A�f�B�X�G�i{isDisenable}�U���\{isAttackable}�A�U���^�C�v{atType}�A�ړ��\{_characterHorizontalMovement.ReadInput}�A�U���ԍ�{attackNumber}�A�`���[�W����{chargeTime}");

        }






        /// <summary>
        /// �A�r���e�B�T�C�N���̊J�n���ɌĂяo����A�����œ��̗͂L�����m�F���܂��B
        /// </summary>
        protected override void HandleInput()
        {
            //�����ŉ��{�^����������Ă��邩�ɂ���Ĉ����n����
            //�����ɂ���ă`���[�W��Ԃɂ�����b���������肷��
            //�C���v�b�g�ɂ��Ē��ׂ�
            //
            // here as an example we check if we're pressing down
            // on our main stick/direction pad/keyboard


            fire1Key = (_inputManager.sAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonDown || _inputManager.sAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed);
            fire2Key = (_inputManager.bAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonDown || _inputManager.bAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed);
            artsKey = (_inputManager.ArtsButton.State.CurrentState == MMInput.ButtonStates.ButtonDown || _inputManager.ArtsButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed);


            //    Debug.Log("�A�C�C�C");


            //�U���I���F�b�N
            if (_movement.CurrentState == CharacterStates.MovementStates.Attack && isAttackable)
            {
                anyKey = AnyKey();
                
            }
            //  UpdateAnimator();
            WeaponChange();
        }

        public void WeaponChange()
        {
            if (_inputManager.WeaponChangeButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed && _movement.CurrentState != CharacterStates.MovementStates.Attack)
            {
                //����؂�ւ�
                //�K�[�h�{�^���ŏ��A�U���{�^���ŕ����؂�ւ�

                //����ɏ����͕K���Ў莝����
                GManager.instance.equipWeapon.twinHand = false;
                //���̃t���O�͕���؂�ւ������莝���؂�ւ����ŋ�ʂ������
                equipChange = true;
                //����؂�ւ���͈�񂾂��{�^�������Ă�������ύX���������Ȃ��悤�ɂ���
            }
            else if (changeKey && !equipChange)
            {
                GManager.instance.equipWeapon.twinHand = !GManager.instance.equipWeapon.twinHand;
            }
            else if (equipChange && changeKey)
            {
                equipChange = false;
            }
        }




        /// <summary>
        ///  �K�v�ȃA�j���[�^�[�p�����[�^�[������΁A�A�j���[�^�[�p�����[�^�[���X�g�ɒǉ����܂��B
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            //    RegisterAnimatorParameter(_attackParameterName, AnimatorControllerParameterType.Bool, out _attackAnimationParameter);
            RegisterAnimatorParameter(_typeParameterName, AnimatorControllerParameterType.Int, out _typeAnimationParameter);
            RegisterAnimatorParameter(_numberParameterName, AnimatorControllerParameterType.Int, out _numberAnimationParameter);
        }

        /// <summary>
        /// �A�r���e�B�̃T�C�N�����I���������_�B
        /// ���݂̂��Ⴊ�ށA�����̏�Ԃ��A�j���[�^�[�ɑ���B
        /// </summary>
        public override void UpdateAnimator()
        {
            //���̃X�e�[�g��Attack�ł��邩�ǂ�����Bool����ւ��Ă�
            // MMAnimatorExtensions.UpdateAnimatorBool(_animator, _attackAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Attack), _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _typeAnimationParameter, (int)atType, _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _numberAnimationParameter, attackNumber, _character._animatorParameters);
        }




        //�A�j���[�V�����C�x���g
        #region
        public void Continue()
        {
            // GManager.instance.pm.anim.Play("OArts1");

            ////Debug.Log("�U���\��");

            GManager.instance.StaminaUse(GManager.instance.useAtValue.useStamina);
            isAirEnd = false;
            if (atType == ActType.fAttack)
            {
                startFall = true;
            }
            else
            {
                GManager.instance.isArmor = false;
                isAttackable = true;
            }
        }
        public void SwingSound(int type = 0)
        {
            //�a���h�ˑŌ����Ǘ�
            if (GManager.instance.useAtValue.type == MyCode.Weapon.AttackType.Stab)
            {
                GManager.instance.PlaySound(MyCode.SoundManager.instance.stabSound[type], transform.position);
            }
            else
            {
                GManager.instance.PlaySound(MyCode.SoundManager.instance.swingSound[type], transform.position);
            }

            //�G���`�����Ă�ꍇ��

        }

        public void attackEffect()
        {

            Addressables.InstantiateAsync(GManager.instance.useAtValue.attackEffect, GManager.instance.pm.eContoroller.transform);
        }
        #endregion

        //��������n
        #region 

        //�����U���̗����𐧌䂷��
        void AttackFallController()
        {
            //�����U���̎�
            if (atType == ActType.fAttack)
            {
                //����Continue�ŗ����J�n���Ă�Ȃ�
                if (startFall)
                {
                    // gravity =GManager.instance.pm.gravity * 3f;
                    //�d��1.5�{
                    _controller.DefaultParameters.Gravity = GManager.instance.pStatus.firstGravity * -1.5f;
                }
                else
                {
                    //�����J�n�܂ł͖��d�͂�
                    _controller.DefaultParameters.Gravity = 0;
                }
                // ���n������
                if (startFall && _controller.State.IsGrounded)
                {
                    //�d�͂����Ƃɖ߂�
                    _controller.DefaultParameters.Gravity = -GManager.instance.pStatus.firstGravity;

                    groundTime += _controller.DeltaTime;

                    GManager.instance.isArmor = false;

                    // GManager.instance.pm.jumpTime = 0.0f;

                    //�L�����Z���\��
                    if (groundTime >= 0.1f)
                    {
                        isAttackable = true;
                        //GManager.instance.isAttack = false;
                        //  
                        // GManager.instance.isArmor = false;
                        groundTime = 0;
                        // //////Debug.log("�@�\���Ă܂�");
                        attackNumber = 0;

                        smallTrigger = false;
                        bigTrigger = false;

                        // GManager.instance.pm.jumpTime = 0.0f;

                    }
                }
            }


        }


        #endregion


        //�Ǘ��n
        #region 

        //���͂��󂯎��
        //fire1Key�Ƃ���
        void InputReceiver()
        {

            // ���Ȃ�����
            if (!AbilityPermitted
                // or if we're not in our normal stance
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                // or if we're grounded
                || (_movement.CurrentState == CharacterStates.MovementStates.Rolling)
                // or if we're gripping
                || (_movement.CurrentState == CharacterStates.MovementStates.Gripping))
            {
                // we do nothing and exit
                return;
            }

            //�X�^�~�i���p�\�Ȃ�
            if (GManager.instance.isEnable && !isDisenable && _movement.CurrentState != CharacterStates.MovementStates.Attack && !GManager.instance.equipWeapon.isCombo)
            {

                // 1�ʏ�U���A2�͋󒆎�A3�͋��A4�͋󒆋��A5�͐�Z
                if (fire1Key || smallTrigger)
                {
                    // DoSomething();
                    smallTrigger = false;
                    
                    Debug.Log($"1{attackNumber}");
                    //�ڒn���ĂȂ��Ȃ�
                    if (!_controller.State.IsGrounded)
                    {
                        atType = ActType.aAttack;
                    }
                    else
                    {
                        atType = ActType.sAttack;
                    }
                    isDisenable = true;
                }
                else if (fire2Key || bigTrigger)
                {
                    //7�̓`���[�W��
                    
                    bigTrigger = false;
                    if (_controller.State.IsGrounded)
                    {
                        isCharging = true;
                        atType = ActType.cCharge;
                    }
                    else
                    {
                        atType = ActType.fAttack;
                    }
                    isDisenable = true;
                }
                else if (artsKey || artsTrigger)
                {
                    atType = ActType.arts;
                    artsTrigger = false;
                    //   isDisenable = true;
                }


            }
            //�U��������Ȃ��ăR���{��������Ȃ��Ȃ�
            if (GManager.instance.equipWeapon.isCombo && _movement.CurrentState != CharacterStates.MovementStates.Attack)
            {

                //�R���{����
                //arts�g���K�[�Ƃ��𔻒f�Ɏg����
            }

            if (isCharging)
            {
                if (fire2Key)
                {
                    Debug.Log("�A�C��\");
                    chargeTime += _controller.DeltaTime;
                    //�`���[�W��
                    atType = ActType.cCharge;
                    if (chargeTime >= GManager.instance.equipWeapon.chargeRes)
                    {
                        isCharging = false;

                        //�`���[�W�A�^�b�N���\�b�h��������񂫂�ɂ��邽�߂Ɏg��
                        //   chargeTime = 0.0f;
                        atType = ActType.cAttack;
                        chargeTime = 0;
                    }
                }
                else
                {
                    //  Debug.Log("ddeferfer");
                    //�ꉞ����
                    chargeTime += _controller.DeltaTime;
                    if (chargeTime >= GManager.instance.equipWeapon.chargeRes)
                    {   
                        atType = ActType.cAttack;
                    }
                    else
                    {
                        atType = ActType.bAttack;
                    }
                    isCharging = false;
                     chargeTime = 0.0f;
                }
            }


            //�U�����łȂ��U����Ԃɂ��鎞
            if (atType != ActType.noAttack && _movement.CurrentState != CharacterStates.MovementStates.Attack)
            {



                AttackAct();
            }
        }




        //�A�j���̏I���T�m
        //�I����true��Ԃ�
        bool CheckEnd(string Name)
        {
            //      Debug.Log("������");
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(Name))// || GManager.instance.pm.anim.GetCurrentAnimatorStateInfo(0).IsName("OStand"))
            {   // �����ɓ��B�����normalizedTime��"Default"�̌o�ߎ��Ԃ��E���Ă��܂��̂ŁAResult�ɑJ�ڊ�������܂ł�return����B

                return false;
            }
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {   // �ҋ@���Ԃ���肽���Ȃ�΁A�����̒l��傫������B
                return false;
            }
            //      AnimatorClipInfo[] clipInfo = GManager.instance.pm.anim.GetCurrentAnimatorClipInfo(0);

            ////Debug.Log($"�A�j���I��");
            //  Debug.Log("��");
            return true;

            // return !(GManager.instance.pm.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
            //  (_currentStateName);
        }

        bool AnyKey()
        {
            //Any�͎����s����
            //�L�[�R���t�B�O�Ŕ��f����Ȃ������肵����InputR�̎Q�Ƃ��낦�ĂȂ��̂��m�F
            if (GManager.instance.InputR.GetAnyButton() && !fire1Key && !fire2Key && !artsKey)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void AnimationEndReserch()
        {

            //���[�V�����I���������ǂ����̌���
            if (attackNumber > 0 && isAttackable)
            {

                #region//���[�V�����I������

                    if (atType == ActType.sAttack)
                    {
                        //     Debug.Log("�A�C�C�C");
                        if (CheckEnd($"SAttack{attackNumber}"))
                        {
                            //      Debug.Log("�A�C");
                            Debug.Log($"4");
                            AttackEnd();
                        }
                    }
                    else if (atType == ActType.bAttack)
                    {
                        if (CheckEnd($"BAttack{attackNumber}"))
                        {
                            AttackEnd();

                        }
                    }
                    else if (atType == ActType.cAttack)
                    {
                        if (CheckEnd($"CAttack{attackNumber}"))
                        {

                            AttackEnd();

                        }
                    }
                    else if (atType == ActType.arts)
                    {
                        if (CheckEnd($"Arts{attackNumber}"))
                        {
                            AttackEnd();
                        }
                    }
                   else if (atType == ActType.aAttack)
                    {

                        if (CheckEnd($"SAAttack{attackNumber}"))
                        {
                            AttackEnd();
                        }

                    }
                    else if (atType == ActType.fAttack)
                    {
                        //  Debug.Log($"3sdfg");
                        //�����J�n���ĂĂ��łɒ��n���Ă�Ȃ�
                        if (_controller.State.IsGrounded)
                        {
                            Debug.Log($"pie");
                            //���n�A�j���I����Ă�Ȃ���
                            if (CheckEnd($"Landing"))
                            {
                                Debug.Log("�@�\���Ă܂�");
                                AttackEnd();
                            }
                        }
                }
                #endregion

            }

        }







        void GroundCheck()
        {
            //��������̓��͂������
            if (anyKey)
            {
                AttackEnd();
            }
            //�U�����͂������΃R���{��
            else if (fire1Key)
            {
                if (atType == ActType.sAttack)
                {
                    //  ////Debug.Log($"�A��");
                    AttackEnd(1, isComboEnd);
                }
                //���̍U���̌㏬�U��������attackNumber�̓��Z�b�g
                else
                {

                    AttackEnd(1, true);
                }
            }
            else if (fire2Key)
            {
                if (atType == ActType.bAttack || atType == ActType.cAttack)
                {
                    AttackEnd(2, isComboEnd);
                }
                //���̍U���̌��U��������attackNumber�̓��Z�b�g
                else
                {

                    AttackEnd(2, true);
                }

            }
            else if (artsKey)
            {
                if (atType == ActType.arts)
                {
                    AttackEnd(3, isComboEnd);
                }
                //���̍U���̌��Z������attackNumber�̓��Z�b�g
                else
                {

                    AttackEnd(3, true);
                }
            }
        }
        void AirCheck()
        {
            if (anyKey)
            {
                AttackEnd();

            }
            else if (fire1Key)
            {
                if (atType == ActType.sAttack)
                {
                    AttackEnd(1, isComboEnd);
                }
                //���̍U���̌㏬�U��������attackNumber�̓��Z�b�g
            }
            else if (_inputManager.bAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {

                AttackEnd(2, true);
  }
        }
        void AttackCheck()
        {
            if (atType != ActType.aAttack && atType != ActType.fAttack && atType != ActType.noAttack)
            {
                GroundCheck();
            }

            //�󒆘A�����͂ƃL�����Z���҂�
            else if (atType == ActType.aAttack)
            {
                AirCheck();
            }

        }


        /// <summary>
        ///  0�ŏI���A1�ŃX���[���g���K�[�Ōp���A2�Ńr�b�O�g���K�[�A3�ŃA�[�c�g���K�[
        ///  �R���{�G���h��isComboEnd�����ł͂Ȃ�True�Ƃ�������Ă�����Ƃ��Ă�����
        /// </summary>
        /// <param name="conti"></param>
        void AttackEnd(int conti = 0, bool comboEnd = true)
        {
            GManager.instance.isAttack = false;
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
            isComboEnd = false;
            isDisenable = false;
            _characterHorizontalMovement.ReadInput = true;
            startFall = false;
            atType = ActType.noAttack;
            if (conti == 1)
            {
                smallTrigger = true;
            }
            else if (conti == 2)
            {
                bigTrigger = true;
            }
            else if (conti == 3)
            {
                artsTrigger = true;
            }
            //�����A������Ȃ��Ȃ�U���ԍ��̓��Z�b�g

            if (comboEnd)
            {
                attackNumber = 0;
               // comboLimit = 0;
            }
        }

        #endregion


        //�U�����s�n
        #region

        /// <summary>
        /// �U���̊J�n��
        /// </summary>
        void AttackAct()
        {

            _movement.ChangeState(CharacterStates.MovementStates.Attack);
            //�U���J�n�A�A�[�}�[����
            GManager.instance.isArmor = true;
            GManager.instance.isAttack = true;
            isAttackable = false;
            //���ړ��s��
            _characterHorizontalMovement.ReadInput = false;
            isDisenable = true;
            //�U���\�Ŏ�U���{�^��������ĂčU�����ĂȂ��ăX�^�~�i���g����Ȃ�ȉ��̏���
            //delayTime = 0.0f;


            //�ǂ̍U�����Ăяo����
            #region
            if (atType == ActType.sAttack)
            {
sAttackPrepare();
            }
            else if (atType == ActType.bAttack)
            {
            bAttackPrepare();
            }
            else if (atType == ActType.cAttack)
            {
            chargeAttackPrepare();
            }
            else if (atType == ActType.arts)
            {
            ArtsPrepare();
            }
            else if (atType == ActType.aAttack)
            {
                isAirEnd = (attackNumber + 1 == comboLimit);
                airAttackPrepare();
            }
            else if (atType == ActType.fAttack)
            {
                strikeAttackPrepare();
                GManager.instance.fallAttack = true;
            }
            #endregion

            if (attackNumber >= comboLimit - 1 && comboLimit != 0)
            {
                isComboEnd = true;

            }
            //  �U���s�\���
            isAttackable = false;
            //�U���ԍ����Z
            //prepare��0����n�܂�ԍ��g���̂Ő�ɉ��Z�̓_���ł�
            attackNumber++;

            nowNumber = attackNumber;
        }



        void Parry()
        {

            //���G������������e����������
            if (GManager.instance.parrySuccess && !isParring)
            {
                if (!GManager.instance.blocking)
                {

                    GManager.instance.PlaySound("ParrySuccess", transform.position);
                    //  GManager.instance.PlaySound("ParrySuccess2", transform.position);
                }
                else if (GManager.instance.blocking)
                {
                    //   Debug.Log("s");

                    GManager.instance.PlaySound("Blocking", transform.position);
                }
                isParring = true;
                GManager.instance.guardDisEnable = true;
                //�p���B
            }
            else if (!GManager.instance.blocking && isParring)
            {
                // Debug.Log("sssssss");
                if (!GManager.instance.twinHand && CheckEnd("OParry"))
                {
                    // Debug.Log("sss");
                    isParring = false;
                    GManager.instance.parrySuccess = false;
                    GManager.instance.pm.SetLayer(11);
                    GManager.instance.guardDisEnable = false;
                    // GManager.instance.isDown = false;
                }
                if (GManager.instance.twinHand && CheckEnd("TParry"))
                {
                    isParring = false;
                    GManager.instance.parrySuccess = false;
                    GManager.instance.pm.SetLayer(11);
                    GManager.instance.guardDisEnable = false;

                }
            }
            else if (GManager.instance.blocking && isParring)
            {
                if (!GManager.instance.twinHand && CheckEnd("OBlock"))
                {
                    isParring = false;
                    GManager.instance.parrySuccess = false;
                    GManager.instance.pm.SetLayer(11);
                    GManager.instance.guardDisEnable = false;
                    //GManager.instance.isDown = false;
                }
                if (GManager.instance.twinHand && CheckEnd("TBlock"))
                {
                    isParring = false;
                    GManager.instance.parrySuccess = false;
                    GManager.instance.pm.SetLayer(11);
                    GManager.instance.guardDisEnable = false;
                }
            }

        }

        #endregion

        //Prepare�n��
        #region
        //�U������Ƃ��ɌĂ�
        public void sAttackPrepare()//�f�t�H���a��
        {
            GManager.instance.isShieldAttack = false;
            //�R���{�̍ŏ��ɃR���{����q���邩�m�F����B



            //�K�[�h���Ȃ�K�[�h����
            //�Ђ您��������������������
            if (attackNumber != 0)
            {

                GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
                transform.localScale = GManager.instance.pm.theScale;
            }
            if (!GManager.instance.twinHand)
            {

                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.sValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.sValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.sValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.sValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.sValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.sValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.sValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.sValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.sValue[attackNumber].attackEffect;
                comboLimit = GManager.instance.equipWeapon.sValue.Count;
            }
            else
            {

                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.twinSValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.twinSValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.twinSValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.twinSValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.twinSValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.twinSValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.twinSValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.twinSValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.twinSValue[attackNumber].attackEffect;
                comboLimit = GManager.instance.equipWeapon.twinSValue.Count;
            }
        }

        public void bAttackPrepare()//�f�t�H���a���B���U��
        {
            GManager.instance.isShieldAttack = false;


            if (attackNumber != 0)
            {
                GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
                transform.localScale = GManager.instance.pm.theScale;
            }
            if (!GManager.instance.twinHand)
            {

                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.bValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.bValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.bValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.bValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.bValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.bValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.bValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.bValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.bValue[attackNumber].attackEffect;
                comboLimit = GManager.instance.equipWeapon.bValue.Count;
            }
            else
            {

                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.twinBValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.twinBValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.twinBValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.twinBValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.twinBValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.twinBValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.twinBValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.twinBValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.twinBValue[attackNumber].attackEffect;
                comboLimit = GManager.instance.equipWeapon.twinBValue.Count;
            }
        }

        public void chargeAttackPrepare()//�f�t�H���a��
        {
            GManager.instance.isShieldAttack = false;


            if (attackNumber != 0)
            {
                //   GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
                //        transform.localScale = GManager.instance.pm.theScale;
            }
            if (!GManager.instance.twinHand)
            {

                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.chargeValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.chargeValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.chargeValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.chargeValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.chargeValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.chargeValue[attackNumber].isCombo;
                GManager.instance.equipWeapon.blowPower = GManager.instance.equipWeapon.chargeValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.chargeValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.chargeValue[attackNumber].attackEffect;
                comboLimit = GManager.instance.equipWeapon.bValue.Count;
            }
            else
            {

                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.twinChargeValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.twinChargeValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.twinChargeValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.twinChargeValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.twinChargeValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.twinChargeValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.twinChargeValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.twinChargeValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.twinChargeValue[attackNumber].attackEffect;
                comboLimit = GManager.instance.equipWeapon.twinBValue.Count;
            }
        }
        public void airAttackPrepare()//�f�t�H���a��
        {

            GManager.instance.isShieldAttack = false;


            if (attackNumber != 0)
            {
                GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
                transform.localScale = GManager.instance.pm.theScale;
            }
            if (!GManager.instance.twinHand)
            {

                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.airValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.airValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.airValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.airValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.airValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.airValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.airValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.airValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.airValue[attackNumber].attackEffect;
                comboLimit = GManager.instance.equipWeapon.airValue.Count;
            }
            else
            {

                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.twinAirValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.twinAirValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.twinAirValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.twinAirValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.twinAirValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.twinAirValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.twinAirValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.twinAirValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.twinAirValue[attackNumber].attackEffect;
                comboLimit = GManager.instance.equipWeapon.twinAirValue.Count;
            }
        }
        public void strikeAttackPrepare()//�f�t�H���a��
        {

            GManager.instance.isShieldAttack = false;


            if (attackNumber != 0)
            {
                GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
                transform.localScale = GManager.instance.pm.theScale;
            }
            if (!GManager.instance.twinHand)
            {

                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.strikeValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.strikeValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.strikeValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.strikeValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.strikeValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.strikeValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.strikeValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.strikeValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.strikeValue[attackNumber].attackEffect;

            }
            else
            {

                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].attackEffect;
               
            }
            comboLimit = 1;
        }

        public void ArtsPrepare()//�f�t�H���a��
        {


            if (attackNumber != 0)
            {
                GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
                transform.localScale = GManager.instance.pm.theScale;
            }
            if (!GManager.instance.twinHand && !GManager.instance.equipShield.weaponArts)
            {

                GManager.instance.useAtValue.type = GManager.instance.equipShield.artsValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipShield.artsValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipShield.artsValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipShield.artsValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipShield.artsValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipShield.artsValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipShield.artsValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipShield.artsValue[attackNumber].useStamina;
                GManager.instance.isShieldAttack = true;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipShield.artsValue[attackNumber].attackEffect;
                comboLimit = GManager.instance.equipShield.artsValue.Count;
            }
            else
            {
                GManager.instance.useAtValue.isShield = true;
                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.artsValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.artsValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.artsValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.artsValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.artsValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.artsValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.artsValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.artsValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.artsValue[attackNumber].attackEffect;
                GManager.instance.isShieldAttack = false;
                comboLimit = GManager.instance.equipWeapon.artsValue.Count;
            }
        }
        #endregion




    }
}