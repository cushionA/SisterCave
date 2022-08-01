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
    [AddComponentMenu("Corgi Engine/Character/Abilities/MyAbillityBase")]
    public class MyAbillityBase : CharacterAbility
    {
        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "�A�r���e�B�̃x�[�X"; }



        /// <summary>
        /// ���̃A�N�V�����̎g�p�҂��v���C���[�ł��邩�ǂ���
        /// </summary>
        [SerializeField]
        protected bool isPlayer = false;

        protected new PlayerHorizontalMove _characterHorizontalMovement;

        protected new MyHealth _health;

        protected new RewiredCorgiEngineInputManager _inputManager;

        protected new MyCharacter _character;

        /// <summary>
        /// Sets a new input manager for this ability to get input from
        /// </summary>
        /// <param name="inputManager"></param>
        public virtual void SetInputManager(RewiredCorgiEngineInputManager inputManager)
        {
            _inputManager = inputManager;
        }



        /// <summary>
        /// Gets and stores components for further use
        /// </summary>
        protected override void Initialization()
        {
            _character = this.gameObject.GetComponentInParent<MyCharacter>();
            _controller = this.gameObject.GetComponentInParent<CorgiController>();
            _characterHorizontalMovement = _character?.FindAbility<PlayerHorizontalMove>();
            _characterGravity = _character?.FindAbility<CharacterGravity>();
            _spriteRenderer = this.gameObject.GetComponentInParent<SpriteRenderer>();
            _health = this.gameObject.GetComponentInParent<MyHealth>();
            BindAnimator();
            if (_character != null)
            {
                _sceneCamera = _character.SceneCamera;
                _inputManager = _character.LinkedInputManager;
                _state = _character.CharacterState;
                _movement = _character.MovementState;
                _condition = _character.ConditionState;
            }
            _abilityInitialized = true;
        }

        /// <summary>
        /// The first of the 3 passes you can have in your ability. Think of it as EarlyUpdate() if it existed
        /// </summary>
        public override void EarlyProcessAbility()
        {
            InternalHandleInput();


        }


        /// <summary>
        /// Internal method to check if an input manager is present or not
        /// </summary>
        protected override void InternalHandleInput()
        {

                 //        Debug.Log($"����{this.GetType().Name}");
            if (_inputManager == null) {// Debug.Log("����������");
                                        return; }

            _verticalInput = _inputManager.PrimaryMovement.y;
            _horizontalInput = _inputManager.PrimaryMovement.x;

            if (_characterGravity != null)
            {
                if (_characterGravity.ShouldReverseInput())
                {
                    if (_characterGravity.ReverseVerticalInputWhenUpsideDown)
                    {
                        _verticalInput = -_verticalInput;
                    }
                    if (_characterGravity.ReverseHorizontalInputWhenUpsideDown)
                    {
                        _horizontalInput = -_horizontalInput;
                    }
                }
            }



            HandleInput();
        }

        /// <summary>
        /// ��������ł���ꍇ�́A�������̏����𖞂����Ă��邩�ǂ������`�F�b�N���āA�A�N�V���������s�ł��邩�ǂ������m�F���܂��B
        /// </summary>
        protected virtual void DoSomething()
        {
            // if the ability is not permitted
            if (!AbilityPermitted
                // or if we're not in our normal stance
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                // or if we're grounded
                || (!_controller.State.IsGrounded)
                // or if we're gripping
                || (_movement.CurrentState == CharacterStates.MovementStates.Gripping))
            {
                // we do nothing and exit
                return;
            }

            // if we're still here, we display a text log in the console
            MMDebug.DebugLogTime("We're doing something yay!");
        }



    }
}
