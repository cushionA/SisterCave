using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using UnityEngine.AddressableAssets;
using Rewired.Integration.CorgiEngine;
using DarkTonic.MasterAudio;
using static DefenseData;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// TODO_DESCRIPTION
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/GuardAbillity")]
    public class GuardAbillity : MyAbillityBase
    {
        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "�K�[�h��Ԃɐ؂�ւ����"; }

        //   [Header("����f�[�^")]
        /// declare your parameters here
        ///WeaponHandle�Q�l�ɂ��� 
        
        //�K�[�h�����Ƃ��̕��@�l���Ƃ���

        // Animation parameters
        protected const string _guardParameterName = "GuardState";
        protected int _guardAnimationParameter;



        [HideInInspector]
        public bool guardHit;

        float hitTime;
        //0�K�[�h���ĂȂ��A�P�K�[�h�A�Q�ړ��K�[�h
        int state;


        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            // randomBool = false;
           // _inputManager = (RewiredCorgiEngineInputManager)_inputManager;
        }

        /// <summary>
        /// 1�t���[�����ƂɁA���Ⴊ��ł��邩�ǂ����A�܂����Ⴊ��ł���ׂ������`�F�b�N���܂�
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            DetermineState();
            HitCheck();
        }

        /// <summary>
        /// �A�r���e�B�T�C�N���̊J�n���ɌĂяo����A�����œ��̗͂L�����m�F���܂��B
        /// </summary>
        protected override void HandleInput()
        {
            //�����ŉ��{�^����������Ă��邩�ɂ���Ĉ����n����

            // here as an example we check if we're pressing down
            // on our main stick/direction pad/keyboard
            // Debug.Log($"guibu{_horizontalInput}");
            if (_controller.State.IsGrounded)
                {
                    if ((_inputManager.GuardButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed || guardHit) && GManager.instance.isEnable)
                    {
                        ActGuard();
                    }
                    //�{�^���𗣂����X�^�~�i�g�p�s�Ȃ�
                    else if(_inputManager.GuardButton.State.CurrentState == MMInput.ButtonStates.ButtonUp || !GManager.instance.isEnable)
                    {
                        GuardEnd();
                    } 
                }
                else if(_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove)
                {
                    GuardEnd();
                }



        }

        /// <summary>
        /// ��������ł���ꍇ�́A�������̏����𖞂����Ă��邩�ǂ������`�F�b�N���āA�A�N�V���������s�ł��邩�ǂ������m�F���܂��B
        /// </summary>
        public virtual void ActGuard()
        {
            // if the ability is not permitted
            if (!AbilityPermitted
                // or if we're not in our normal stance
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
            {
                // we do nothing and exit
                GuardEnd();
                return;
            }

            _movement.ChangeState(CharacterStates.MovementStates.Guard);

            //�K�[�h��Ԃ�
            _health.HealthStateChange(false, DefState.�K�[�h��);

            //���ʈړ��̑��x�ł�����
            /*
            if (_characterHorizontalMovement != null)
            {
                _characterHorizontalMovement.MovementSpeed = RunSpeed;
            }*/
        }

        /// <summary>
        /// ���Ⴊ��ł����Ԃ��甇���悤�Ɉړ����邩�A���̋t���s�����ǂ����𖈃t���[���m�F����B
        /// </summary>
        protected virtual void DetermineState()
        {
            float threshold = (_inputManager != null) ? _inputManager.Threshold.x : 0f;

               state = 0;
            if ((_movement.CurrentState == CharacterStates.MovementStates.Guard) || (_movement.CurrentState == CharacterStates.MovementStates.GuardMove))
            {
                
                if ((Mathf.Abs(_horizontalInput) > threshold) && !guardHit)
                {
                    _movement.ChangeState(CharacterStates.MovementStates.GuardMove);
                    state = 2;
                }
                else
                {
                    _movement.ChangeState(CharacterStates.MovementStates.Guard);
                    state = 1;
                }
            }

        }

        //�K�[�h�q�b�g���̓`�F�b�N�؂�Ȃ��悤��
        //�K�[�h�u���C�N�̏����܂œ���邩
        void HitCheck()
        {
            if (guardHit && _condition.CurrentState != CharacterStates.CharacterConditions.Stunned)
            {
                _condition.ChangeState(CharacterStates.CharacterConditions.Moving);
                _movement.ChangeState(CharacterStates.MovementStates.Guard);


                hitTime += _controller.DeltaTime;
                if (isPlayer)
                {
                    // �����v���C���[�ɃK�[�h�q�b�g�����Ȃ�X�^�~�i�͉񕜂��Ȃ��Ȃ�
                    //��~���ԏ��̎󂯒l����ނŕς��Ă���������
                    //�K�[�h�q�b�g�������Ȃ��悤�ɂ��Ȃ���
                    GManager.instance.isStUse = true;
                    //�K�[�h����
                    GManager.instance.isGuard = true;
                    //�K�[�h�Ńq�b�g�����ۂ͉��ړ��ƃW�����v�𕕂���
                    //����ł����ʂɃK�[�h�q�b�g������Ԃɂ��������悳����
                    //�X�^���ł������H
                    //�X�^���̎Q�ƌ������ăX�^���ɂ���΂������Ȃ��悤�ɂł��邩���ׂ�
                    _characterHorizontalMovement.MovementForbidden = true;
                    _inputManager.JumpButton.State.ChangeState(MMInput.ButtonStates.Off);
                    _inputManager.AvoidButton.State.ChangeState(MMInput.ButtonStates.Off);
                   // _inputManager.JumpButton.State.ChangeState(MMInput.ButtonStates.Off);
                }
                else
                {
                    _characterHorizontalMovement.MovementForbidden = true;
                }
                //�w���X�̕��ŉ������Ă�������
                if (hitTime >= 0.1)
                {
                    _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
                    _characterHorizontalMovement.MovementForbidden = false;
                    guardHit = false;
                    hitTime = 0;
                    GManager.instance.isStUse = false;
                }
            }
        }

         public void GuardEnd()
        {
            if (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove)
            {
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
                //�K�[�h�I��
                _health.HealthStateChange(true,DefState.�K�[�h��);
            }
        } 

        /// <summary>
        ///  �K�v�ȃA�j���[�^�[�p�����[�^�[������΁A�A�j���[�^�[�p�����[�^�[���X�g�ɒǉ����܂��B
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
           // Debug.Log($"�ē�{_animator.runtimeAnimatorController.name}");
            RegisterAnimatorParameter(_guardParameterName, AnimatorControllerParameterType.Int, out _guardAnimationParameter);
        }

        /// <summary>
        /// ������I�[�o�[���C�h����ƁA�L�����N�^�[�̃A�j���[�^�[�Ƀp�����[�^�𑗐M���邱�Ƃ��ł��܂��B
        /// ����́ACharacter�N���X�ɂ���āAEarly�Anormal�ALate process()�̌�ɁA1�T�C�N�����Ƃ�1��Ăяo�����B
        /// </summary>
        public override void UpdateAnimator()
        {

            //�N���E�`���O�ɋC�������
            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _guardAnimationParameter, (state), _character._animatorParameters);
        }

        public void GuardHit()
        {
            if (!guardHit)
            {
                guardHit = true;
                
            }
            else
            {
                guardHit = true;
                hitTime = 0;
            }
        }

    }
}
