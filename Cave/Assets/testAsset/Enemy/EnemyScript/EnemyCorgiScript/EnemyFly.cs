using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// This component allows your character to fly by moving gravity-free on both x and y axis. Here you can define the flight speed, as well as whether or not the character is always flying (in which case you don't have to press a button to fly). Important note : slope ceilings are not supported for now.
    /// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/EnemyFly")]
    public class EnemyFly : MyAbillityBase
    {
        public override string HelpBoxText() { return "This component allows your character to fly by moving gravity-free on both x and y axis. Here you can define the flight speed, as well as whether or not the character is always flying (in which case you don't have to press a button to fly). Important note : slope ceilings are not supported for now."; }

        /// the speed at which the character should fly
        [Tooltip("�ʏ펞��s�X�s�[�h")]
        public Vector2 nFlySpeed;

        [Tooltip("������s�̃X�s�[�h")]
        public  Vector2 FastSpeed;

        Vector2 FlySpeed;

        /// a multiplier you can target to increase/reduce the flight speed
        public float MovementSpeedMultiplier { get; set; }
        /// whether or not the Character is always flying, in which case it'll start immune to gravity 
        [Tooltip("whether or not the Character is always flying, in which case it'll start immune to gravity ")]
        public bool AlwaysFlying = false;
        /// whether or not the Character should stop flying on death
        [Tooltip("whether or not the Character should stop flying on death")]
        public bool StopFlyingOnDeath = true;

        protected float _horizontalMovement;
        protected float _verticalMovement;
        protected bool _flying;

        // animation parameters
        protected const string _flyingAnimationParameterName = "FlyingState";
        protected const string _flySpeedAnimationParameterName = "FlySpeed";
        protected int _flyingAnimationParameter;
        protected int _flySpeedAnimationParameter;



        /*
        /// the feedbacks to play when the ability starts
        [Tooltip("the feedbacks to play when the ability starts")]
        public MMFeedbacks AddtionalStartFeedbacks;
        /// the feedbacks to play when the ability stops
        [Tooltip("the feedbacks to play when the ability stops")]
        public MMFeedbacks AddtionalStopFeedbacks;
        */
        //�͂₭���ł��邩
        bool isFast;

        /// <summary>
        /// On Start, we initialize our flight if needed
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();

            MovementSpeedMultiplier = 1f;

            if (AlwaysFlying)
            {
                StartFlight();
            }
        }

        /// <summary>
        /// Looks for hztal and vertical input, and for flight button if needed
        /// </summary>
        protected override void HandleInput()
        {
            _horizontalMovement = _horizontalInput;
            _verticalMovement = _verticalInput;




        }

        /// <summary>
		/// Sets the horizontal move value.
		/// </summary>
		/// <param name="value">Horizontal move value, between -1 and 1 - positive : will move to the right, negative : will move left </param>
		public virtual void SetHorizontalMove(float value)
        {
            _horizontalMovement = value;
        }

        /// <summary>
		/// Sets the horizontal move value.
		/// </summary>
		/// <param name="value">Horizontal move value, between -1 and 1 - positive : will move to the right, negative : will move left </param>
		public virtual void SetVerticalMove(float value)
        {
            _verticalMovement = value;
        }

        /// <summary>
        /// Starts the flight sequence
        /// </summary>
        public virtual void StartFlight()
        {
            if ((!AbilityAuthorized) // if the ability is not permitted
                || (_movement.CurrentState == CharacterStates.MovementStates.Dashing) // or if we're dashing
                || (_movement.CurrentState == CharacterStates.MovementStates.Attack) // or if we're in the gripping state
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)) // or if we're not in normal conditions
            {
                return;
            }

            // if this is the first time we're here, we trigger our sounds
            if (_movement.CurrentState != CharacterStates.MovementStates.Flying)
            {
                // we play the jetpack start sound 
                PlayAbilityStartFeedbacks();
                _flying = true;
                FlySpeed = nFlySpeed;
            }

            // we set the various states
            _movement.ChangeState(CharacterStates.MovementStates.Flying);
            isFast = false;
            MovementSpeedMultiplier = 1f;
            _controller.GravityActive(false);
        }

        /// <summary>
        /// Stops the flight
        /// </summary>
        public virtual void StopFlight()
        {
            if (_movement.CurrentState == CharacterStates.MovementStates.Flying)
            {
                StopStartFeedbacks();
                PlayAbilityStopFeedbacks();
            }
            _controller.GravityActive(true);
            _flying = false;

            if (_movement.CurrentState != CharacterStates.MovementStates.Attack) 
            {
                _movement.ChangeState(CharacterStates.MovementStates.Idle); 
            }
        }

        /// <summary>
        /// On Update, checks if we should stop flying
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();

            if ((_condition.CurrentState != CharacterStates.CharacterConditions.Normal)) 
            {
                StopFlight();
            }

         //   Debug.Log($"dddddddddd{_verticalMovement}");

            if (StopFlyingOnDeath && (_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead))
            {
                return;
            }

            if (AlwaysFlying)
            {
                if (_movement.CurrentState != CharacterStates.MovementStates.Flying && _condition.CurrentState == CharacterStates.CharacterConditions.Normal)
                {
                    _movement.ChangeState(CharacterStates.MovementStates.Flying);
                }
                _flying = true;
            }

            if (_flying)
            {
                _controller.GravityActive(false);
            }

            HandleMovement();

            // if we're not walking anymore, we stop our walking sound
            if (_movement.CurrentState != CharacterStates.MovementStates.Flying && _startFeedbackIsPlaying)
            {
                StopStartFeedbacks();
            }

            if (_movement.CurrentState != CharacterStates.MovementStates.Flying && _flying)
            {
                StopFlight();
            }

            if (_controller.State.IsCollidingAbove && (_movement.CurrentState != CharacterStates.MovementStates.Flying))
            {
                _controller.SetVerticalForce(0);
            }
        }


        /// <summary>
        /// Makes the character move in the air
        /// </summary>
        protected virtual void HandleMovement()
        {
            // if we're not walking anymore, we stop our walking sound
            if (_movement.CurrentState != CharacterStates.MovementStates.Flying && _startFeedbackIsPlaying)
            {
                StopStartFeedbacks();
            }

            // if movement is prevented, or if the character is dead/frozen/can't move, we exit and do nothing
            if (!AbilityAuthorized
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                || (_movement.CurrentState == CharacterStates.MovementStates.Gripping))
            {
                return;
            }

            // If the value of the horizontal axis is positive, the character must face right.
            if (_horizontalMovement > 0.1f)
            {
                if (!_character.IsFacingRight)
                    _character.Flip();
            }
            // If it's negative, then we're facing left
            else if (_horizontalMovement < -0.1f)
            {
                if (_character.IsFacingRight)

                _character.Flip();
            }

            if (_flying)
            {
           //     Debug.Log($"��������{FlySpeed}");
                // we pass the horizontal force that needs to be applied to the controller.
                float horizontalMovementSpeed = _horizontalMovement * FlySpeed.x * _controller.Parameters.SpeedFactor * MovementSpeedMultiplier;
                float verticalMovementSpeed = _verticalMovement * FlySpeed.y * _controller.Parameters.SpeedFactor * MovementSpeedMultiplier;

                // we set our newly computed speed to the controller
                _controller.SetHorizontalForce(horizontalMovementSpeed);
                _controller.SetVerticalForce(verticalMovementSpeed);
            }
        }

        /// <summary>
        /// When the character respawns we reinitialize it
        /// </summary>
        protected virtual void OnRevive()
        {
            Initialization();
        }

        /// <summary>
        /// On death the character stops flying if needed
        /// </summary>
        protected override void OnDeath()
        {
            base.OnDeath();
            if (StopFlyingOnDeath)
            {
                StopFlight();
            }
        }

        /// <summary>
        /// When the player respawns, we reinstate this agent.
        /// </summary>
        /// <param name="checkpoint">Checkpoint.</param>
        /// <param name="player">Player.</param>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (gameObject.GetComponentInParent<Health>() != null)
            {
                gameObject.GetComponentInParent<Health>().OnRevive += OnRevive;
            }
        }

        /// <summary>
        /// Stops listening for revive events
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (_health != null)
            {
                _health.OnRevive -= OnRevive;
            }
        }

        /// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_flyingAnimationParameterName, AnimatorControllerParameterType.Int, out _flyingAnimationParameter);
            RegisterAnimatorParameter(_flySpeedAnimationParameterName, AnimatorControllerParameterType.Float, out _flySpeedAnimationParameter);
        }

        /// <summary>
        /// At the end of each cycle, we send our character's animator the current flying status
        /// </summary>
        public override void UpdateAnimator()
        {
            int state = 0;
            
            if(_movement.CurrentState == CharacterStates.MovementStates.Flying || _movement.CurrentState == CharacterStates.MovementStates.FastFlying)
            {
                state = _movement.CurrentState == CharacterStates.MovementStates.Flying ? 1 : 2;
            }

            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _flyingAnimationParameter, (state), _character._animatorParameters, _character.PerformAnimatorSanityChecks);

            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _flySpeedAnimationParameter, Mathf.Abs(_controller.Speed.magnitude), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
        }

        /// <summary>
        /// On reset ability, we cancel all the changes made
        /// </summary>
        public override void ResetAbility()
        {
            base.ResetAbility();
            StopFlight();
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _flyingAnimationParameter, false, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _flySpeedAnimationParameter, 0f, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
        }

        public  void FastFly(bool vertical = false,bool end = false)
        {
            //vertical�̎��c
            if ((!AbilityAuthorized) // if the ability is not permitted
    || (_movement.CurrentState == CharacterStates.MovementStates.Dashing) // or if we're dashing
    || (_movement.CurrentState == CharacterStates.MovementStates.Gripping) // or if we're in the gripping state
    || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)) // or if we're not in normal conditions
            {
                return;
            }
            //�ʏ��s�s�������Ƃ���StartFlight�ł���
            //�G���h����Ȃ��Ȃ�͂₭���
            if (!end)
            {
                // if this is the first time we're here, we trigger our sounds
                if (!isFast)
                {
                    // we play the jetpack start sound 
                   // PlayAbilityStartFeedbacks();
                 //   AlterStartFeedbacks();
                    _flying = true;
                    if(!vertical)
                    {
                      FlySpeed.x = FastSpeed.x;
                    }
                    else
                    {
                        FlySpeed.y = FastSpeed.y;
                    }
                }

                // we set the various states
                // _movement.ChangeState(CharacterStates.MovementStates.FastFlying);
                isFast = true;
                MovementSpeedMultiplier = 1f;
                _controller.GravityActive(false);
            }
            else
            {
                /*    if (isFast)
                    {

                        AlterPlayAbilityStopFeedbacks();
                    }*/
                if (!vertical)
                {
                    FlySpeed.x = nFlySpeed.x;
                }
                else
                {
                    FlySpeed.y = nFlySpeed.y;
                }
                _controller.GravityActive(true);
                _flying = false;
                _movement.RestorePreviousState();
            }

        }


    }
}
