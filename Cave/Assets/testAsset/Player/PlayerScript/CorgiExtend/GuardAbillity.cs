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
    [AddComponentMenu("Corgi Engine/Character/Abilities/TODO_REPLACE_WITH_ABILITY_NAME")]
    public class GuardAbillity : CharacterAbility
    {
        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "�K�[�h��Ԃɐ؂�ւ����"; }

        //   [Header("����f�[�^")]
        /// declare your parameters here
        ///WeaponHandle�Q�l�ɂ��� 
        
        //�K�[�h�����Ƃ��̕��@�l���Ƃ���

        // Animation parameters
        protected const string _todoParameterName = "GuardNow";
        protected int _todoAnimationParameter;

        protected RewiredCorgiEngineInputManager ReInput;

        [HideInInspector]
        public bool guardHit;

        float hitTime;

        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            // randomBool = false;
            ReInput = (RewiredCorgiEngineInputManager)_inputManager;
        }

        /// <summary>
        /// 1�t���[�����ƂɁA���Ⴊ��ł��邩�ǂ����A�܂����Ⴊ��ł���ׂ������`�F�b�N���܂�
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
        }

        /// <summary>
        /// �A�r���e�B�T�C�N���̊J�n���ɌĂяo����A�����œ��̗͂L�����m�F���܂��B
        /// </summary>
        protected override void HandleInput()
        {
            //�����ŉ��{�^����������Ă��邩�ɂ���Ĉ����n����
            
            // here as an example we check if we're pressing down
            // on our main stick/direction pad/keyboard

                if (_controller.State.IsGrounded)
                {
                    if (ReInput.GuardButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed || guardHit)
                    {
                        ActGuard();
                    }
                    else if(ReInput.GuardButton.State.CurrentState == MMInput.ButtonStates.ButtonUp)
                    {
                        GuardEnd();
                    } 
                }
                else if (_movement.CurrentState == CharacterStates.MovementStates.Guard)
                {
                    GuardEnd();
                }

            if (guardHit)
            {
                hitTime += _controller.DeltaTime;
                if (hitTime >= 0.1)
                {
                    guardHit = false;
                    hitTime = 0;
                }
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
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                // or if we're grounded
                // or if we're gripping
                || (_movement.CurrentState != CharacterStates.MovementStates.Gripping))
            {
                // we do nothing and exit
                return;
            }

            _movement.ChangeState(CharacterStates.MovementStates.Guard);

            //���ʈړ��̑��x�ł�����
            /*
            if (_characterHorizontalMovement != null)
            {
                _characterHorizontalMovement.MovementSpeed = RunSpeed;
            }*/
        }

        public void GuardEnd()
        {
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
        } 

        /// <summary>
        ///  �K�v�ȃA�j���[�^�[�p�����[�^�[������΁A�A�j���[�^�[�p�����[�^�[���X�g�ɒǉ����܂��B
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_todoParameterName, AnimatorControllerParameterType.Bool, out _todoAnimationParameter);
        }

        /// <summary>
        /// ������I�[�o�[���C�h����ƁA�L�����N�^�[�̃A�j���[�^�[�Ƀp�����[�^�𑗐M���邱�Ƃ��ł��܂��B
        /// ����́ACharacter�N���X�ɂ���āAEarly�Anormal�ALate process()�̌�ɁA1�T�C�N�����Ƃ�1��Ăяo�����B
        /// </summary>
        public override void UpdateAnimator()
        {
            //�N���E�`���O�ɋC�������
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _todoAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Guard), _character._animatorParameters);
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
