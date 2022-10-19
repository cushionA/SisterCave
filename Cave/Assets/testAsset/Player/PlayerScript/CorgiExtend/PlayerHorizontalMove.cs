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
    [AddComponentMenu("Corgi Engine/Character/Abilities/PlayerHorizontalMove")]
    public class PlayerHorizontalMove : MyAbillityBase
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component handles basic left/right movement, friction, and ground hit detection. Here you can define standard movement speed, walk speed, and what effects to use when the character hits the ground after a jump/fall."; }

		/// 現在の基準移動速度
		public float MovementSpeed { get; set; }

		[Header("Speed")]

		/// the speed of the character when it's walking
		[Tooltip("the speed of the character when it's walking")]
		public float WalkSpeed = 6f;
		/// the multiplier to apply to the horizontal movement
		//　読み取り専用。コードから変えてね
		[MMReadOnly]
		[Tooltip("水平方向の移動に適用する倍率")]
		public float MovementSpeedMultiplier = 1f;
		/// the multiplier to apply to the horizontal movement, dedicated to abilities
		[MMReadOnly]
		[Tooltip("水平方向の移動に適用する倍率で、アビリティに特化しています。")]
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
		[Tooltip("これが true の場合、キャラクターは自動的にその移動方向に反転します。")]
		public bool FlipCharacterToFaceDirection = true;


		/// the current horizontal movement force
		public float HorizontalMovementForce { get { return _horizontalMovementForce; } }
		/// if this is true, movement will be forbidden (as well as flip)
		public bool MovementForbidden { get; set; }

		[Header("Input")]

		/// if this is true, will get input from an input source, otherwise you'll have to set it via SetHorizontalMove()
		//  これが真の場合、入力ソースからの入力を取得します。そうでない場合は、SetHorizontalMove() で設定する必要があります。
		[Tooltip("if this is true, will get input from an input source, otherwise you'll have to set it via SetHorizontalMove()")]
		public bool ReadInput = true;
		/// if this is true, no acceleration will be applied to the movement, which will instantly be full speed (think Megaman movement). Attention : a character with instant acceleration won't be able to get knockbacked on the x axis as a regular character would, it's a tradeoff
		/// これが真の場合、加速度は適用されず、瞬時に全速力となります（ロックマンの動きを想像してください）。
		/// 注意：瞬間的な加速を持つキャラクターは、通常のキャラクターのようにX軸でノックバックを受けることはできません。
		[Tooltip("if this is true, no acceleration will be applied to the movement, which will instantly be full speed (think Megaman movement). Attention : a character with instant acceleration won't be able to get knockbacked on the x axis as a regular character would, it's a tradeoff")]
		public bool InstantAcceleration = false;
		/// the threshold after which input is considered (usually 0.1f to eliminate small joystick noise)
		/// 入力を考慮する閾値（小さなジョイスティックノイズを除去するため通常0.1f）(検知しない値を確認)
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
		/// キャラクターが地面に衝突したときに再生されるMMFeedbacks
		[Tooltip("the MMFeedbacks to play when the character hits the ground")]
		public MMFeedbacks TouchTheGroundFeedback;
		/// the duration (in seconds) during which the character has to be airborne before a feedback can be played when touching the ground
		/// キャラクタが空中にいる間、地面に触れてもフィードバックが再生されるまでの時間（秒）です。
		[Tooltip("the duration (in seconds) during which the character has to be airborne before a feedback can be played when touching the ground")]
		public float MinimumAirTimeBeforeFeedback = 0.2f;

		[Header("Walls")]
		/// Whether or not the state should be reset to Idle when colliding laterally with a wall
		/// 壁に横から衝突したときに状態をIdleに戻すかどうか
		[Tooltip("Whether or not the state should be reset to Idle when colliding laterally with a wall")]
		public bool StopWalkingWhenCollidingWithAWall = false;

		protected float _horizontalMovement;
		protected float _lastGroundedHorizontalMovement;
		protected float _horizontalMovementForce;
		protected float _normalizedHorizontalSpeed;
		protected float _lastTimeGrounded = 0f;

		// animation parameters
		protected const string _speedAnimationParameterName = "Speed";
		protected const string _movingAnimationParameterName = "Moving";
		protected const string _fallAnimationParameterName = "Falling";
		protected int _speedAnimationParameter;
		protected int _movingAnimationParameter;
		protected int _fallAnimationParameter;


		protected float fallTime;


		/// <summary>
		/// On Initialization, we set our movement speed to WalkSpeed.
		/// 最初にパラメータを入れる
		/// </summary>
		protected override void Initialization()
		{

			base.Initialization();
			MovementSpeed = WalkSpeed;
			MovementSpeedMultiplier = 1f;
			AbilityMovementSpeedMultiplier = 1f;
			MovementForbidden = false;
		}

		/// <summary>
		/// The second of the 3 passes you can have in your ability. Think of it as Update()
		/// アップデート部分に該当？
		/// </summary>
		public override void ProcessAbility()
		{
			
			//base.ProcessAbility();

			HandleHorizontalMovement();
			DetectWalls();
		}

		/// <summary>
		/// Called at the very start of the ability's cycle, and intended to be overridden, looks for input and calls
		/// methods if conditions are met
		/// 条件を満たした場合フレームの最初に呼ばれて入力を検知する。
		/// </summary>
		protected override void HandleInput()
		{

			//base.HandleInput();
			if (!ReadInput || _condition.CurrentState != CharacterStates.CharacterConditions.Normal)
			{
				//_horizontalMovement = 0;
				return;
			}


			_horizontalMovement = _horizontalInput;
//Debug.Log($"はろー{_horizontalInput}");
			if ((AirControl < 1f) && !_controller.State.IsGrounded)
			{
			
				//もし空中では横移動速度が落ちるなら、つまり空気移動係数が1以下であるなら横入力を調整する
				//０なら無効化する
				//つまりエアコントロールは空中移動可能かどうかというのも兼ねている。
				_horizontalMovement = Mathf.Lerp(_lastGroundedHorizontalMovement, _horizontalInput, AirControl);
			}
		}

		/// <summary>
		/// When using low (or null) air control, this method lets you externally set the direction air control should consider as the base value
		/// ロー（またはヌル）エアーコントロール使用時に、エアーコントロールが基準値とみなすべき方向を外部から設定する方法です
		/// </summary>
		/// <param name="newInputValue"></param>
		public virtual void SetAirControlDirection(float newInputValue)
		{
			_lastGroundedHorizontalMovement = newInputValue;
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
		/// Called at Update(), handles horizontal movement
		/// アップデートで呼ばれるアクションの本体
		/// </summary>
		protected virtual void HandleHorizontalMovement()
		{
			// if we're not walking anymore, we stop our walking sound
			if ((_movement.CurrentState != CharacterStates.MovementStates.moving) && _startFeedbackIsPlaying)
			{
				StopStartFeedbacks();
			}


			// if movement is prevented, or if the character is dead/frozen/can't move, we exit and do nothing
			if (!ActiveAfterDeath)
			{
				if (!AbilityAuthorized
					|| (_condition.CurrentState != CharacterStates.CharacterConditions.Normal && _movement.CurrentState != CharacterStates.MovementStates.Rolling)
					|| (_movement.CurrentState == CharacterStates.MovementStates.Gripping))
				{
					return;
				}
			}

			// check if we just got grounded
			CheckJustGotGrounded();
			StoreLastTimeGrounded();

			bool canFlip = true;

			if (MovementForbidden)
			{
				_horizontalMovement = _character.Airborne ? _controller.Speed.x * Time.deltaTime : 0f;
				canFlip = false;
			}

			if (!_controller.State.IsGrounded && !AllowFlipInTheAir)
			{
				canFlip = false;
			}
           //           Debug.Log($"あいら{_character.IsFacingRight}え{canFlip}あ{FlipCharacterToFaceDirection}で{_horizontalMovement}");
			// If the value of the horizontal axis is positive, the character must face right.
			if (_horizontalMovement > InputThreshold)
			{
				_normalizedHorizontalSpeed = _horizontalMovement;

				//左向いてて振り向けて振り向く設定になってるなら
				if (!_character.IsFacingRight && canFlip && FlipCharacterToFaceDirection)
				{

					_character.Flip();
				}
			}
			// If it's negative, then we're facing left
			else if (_horizontalMovement < -InputThreshold)
			{
				_normalizedHorizontalSpeed = _horizontalMovement;
				if (_character.IsFacingRight && canFlip && FlipCharacterToFaceDirection)
				{

					_character.Flip();
				}
			}
			else
			{
				_normalizedHorizontalSpeed = 0;
			}

			/// if we're dashing, we stop there
			if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
			{
				return;
			}

			// if we're grounded and moving, and currently Idle, Dangling or Falling, we become Walking
			if ((_controller.State.IsGrounded)
				&& (_normalizedHorizontalSpeed != 0)
				&& ((_movement.CurrentState == CharacterStates.MovementStates.Idle)
					|| (_movement.CurrentState == CharacterStates.MovementStates.Dangling)
					|| (_movement.CurrentState == CharacterStates.MovementStates.Falling)))
			{
				_movement.ChangeState(CharacterStates.MovementStates.moving);
				PlayAbilityStartFeedbacks();
			}

			// if we're grounded, jumping but not moving up, we become idle
			if ((_controller.State.IsGrounded)
				&& (_movement.CurrentState == CharacterStates.MovementStates.Jumping)
				&&(_movement.CurrentState != CharacterStates.MovementStates.Nostate)
				&& (_controller.TimeAirborne >= _character.AirborneMinimumTime))
			{
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
			}

			// if we're walking and not moving anymore, we go back to the Idle state
			if ((_movement.CurrentState == CharacterStates.MovementStates.moving)
			&& (_normalizedHorizontalSpeed == 0))
			{
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				PlayAbilityStopFeedbacks();
			}

			// if the character is not grounded, but currently idle or walking, we change its state to Falling
			if (!_controller.State.IsGrounded
				&& 
					(_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
					 && ((_movement.CurrentState == CharacterStates.MovementStates.moving)
					 || (_movement.CurrentState == CharacterStates.MovementStates.Idle))
					)
					
			{
				fallTime += _controller.DeltaTime;
				if (fallTime > 0.0f) 
				{

					fallTime = 0;
					_movement.ChangeState(CharacterStates.MovementStates.Falling);
				}
			}

			// we apply instant acceleration if needed
			if (InstantAcceleration)
			{
				if (_normalizedHorizontalSpeed > 0f) { _normalizedHorizontalSpeed = 1f; }
				if (_normalizedHorizontalSpeed < 0f) { _normalizedHorizontalSpeed = -1f; }
			}

			// we pass the horizontal force that needs to be applied to the controller.
			float groundAcceleration = _controller.Parameters.SpeedAccelerationOnGround;
			float airAcceleration = _controller.Parameters.SpeedAccelerationInAir;

			if (_controller.Parameters.UseSeparateDecelerationOnGround && (Mathf.Abs(_horizontalMovement) < InputThreshold))
			{
				groundAcceleration = _controller.Parameters.SpeedDecelerationOnGround;
			}
			if (_controller.Parameters.UseSeparateDecelerationInAir && (Mathf.Abs(_horizontalMovement) < InputThreshold))
			{
				airAcceleration = _controller.Parameters.SpeedDecelerationInAir;
			}
			//空中かどうかファクターを分けてる
			float movementFactor = _controller.State.IsGrounded ? groundAcceleration : airAcceleration;
			float movementSpeed = _normalizedHorizontalSpeed * MovementSpeed * _controller.Parameters.SpeedFactor * MovementSpeedMultiplier * AbilityMovementSpeedMultiplier * StateSpeedMultiplier * PushSpeedMultiplier;

			if (InstantAcceleration && (_movement.CurrentState != CharacterStates.MovementStates.WallJumping))
			{
				// if we are in instant acceleration mode, we just apply our movement speed
				_horizontalMovementForce = movementSpeed;

				// and any external forces that may be active right now
				if (Mathf.Abs(_controller.ExternalForce.x) > 0)
				{
					_horizontalMovementForce += _controller.ExternalForce.x;
				}
			}
			else
			{
				// if we are not in instant acceleration mode, we lerp towards our movement speed
				_horizontalMovementForce = Mathf.Lerp(_controller.Speed.x, movementSpeed, Time.deltaTime * movementFactor);
			}

			//ここで移動。空中か地上かで分けずに移動
			// we handle friction
			_horizontalMovementForce = HandleFriction(_horizontalMovementForce);

			// we set our newly computed speed to the controller
			_controller.SetHorizontalForce(_horizontalMovementForce);

			if (_controller.State.IsGrounded)
			{
				_lastGroundedHorizontalMovement = _horizontalMovement;
			}
		}

		protected virtual void DetectWalls()
		{
			if (!StopWalkingWhenCollidingWithAWall)
			{
				return;
			}

			if ((_movement.CurrentState == CharacterStates.MovementStates.moving) || (_movement.CurrentState == CharacterStates.MovementStates.Running))
			{
				if ((_controller.State.IsCollidingLeft) || (_controller.State.IsCollidingRight))
				{
					_movement.ChangeState(CharacterStates.MovementStates.Idle);
				}
			}
		}

		/// <summary>
		/// Every frame, checks if we just hit the ground, and if yes, changes the state and triggers a particle effect
		/// </summary>
		protected virtual void CheckJustGotGrounded()
		{
			// if the character just got grounded
			if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
			{
				return;
			}


			if (_controller.State.JustGotGrounded)
			{
				if ((_movement.CurrentState != CharacterStates.MovementStates.Jumping))
				{
					if (_controller.State.ColliderResized)
					{
						_movement.ChangeState(CharacterStates.MovementStates.Crouching);
					}
					else if(_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
					{
						//Debug.Log("わたしがわるうございました");
						_movement.ChangeState(CharacterStates.MovementStates.Idle);
					}
				}

				_controller.SlowFall(0f);
				if (Time.time - _lastTimeGrounded > MinimumAirTimeBeforeFeedback)
				{
					TouchTheGroundFeedback?.PlayFeedbacks();
				}

			}
		}

		/// <summary>
		/// Computes and stores the last time we were grounded
		/// </summary>
		protected virtual void StoreLastTimeGrounded()
		{
			if ((_controller.State.IsGrounded)
				|| (_character.MovementState.CurrentState == CharacterStates.MovementStates.LadderClimbing)
				|| (_character.MovementState.CurrentState == CharacterStates.MovementStates.LedgeClimbing)
				|| (_character.MovementState.CurrentState == CharacterStates.MovementStates.LedgeHanging)
				|| (_character.MovementState.CurrentState == CharacterStates.MovementStates.Gripping)
				|| (_character.MovementState.CurrentState == CharacterStates.MovementStates.SwimmingIdle))
			{
				_lastTimeGrounded = Time.time;
			}
		}

		/// <summary>
		/// Handles surface friction.
		/// </summary>
		/// <returns>The modified current force.</returns>
		/// <param name="force">the force we want to apply friction to.</param>
		protected virtual float HandleFriction(float force)
		{
			// if we have a friction above 1 (mud, water, stuff like that), we divide our speed by that friction
			if (_controller.Friction > 1)
			{
				force = force / _controller.Friction;
			}

			// if we have a low friction (ice, marbles...) we lerp the speed accordingly
			if (_controller.Friction < 1 && _controller.Friction > 0)
			{
				force = Mathf.Lerp(_controller.Speed.x, force, Time.deltaTime * _controller.Friction * 10);
			}

			return force;
		}

		/// <summary>
		/// A public method to reset the horizontal speed
		/// </summary>
		public virtual void ResetHorizontalSpeed()
		{
			MovementSpeed = WalkSpeed;
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter(_speedAnimationParameterName, AnimatorControllerParameterType.Float, out _speedAnimationParameter);
			RegisterAnimatorParameter(_movingAnimationParameterName, AnimatorControllerParameterType.Bool, out _movingAnimationParameter);
			RegisterAnimatorParameter(_fallAnimationParameterName, AnimatorControllerParameterType.Bool, out _fallAnimationParameter);
		}

		/// <summary>
		/// Sends the current speed and the current value of the Walking state to the animator
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _speedAnimationParameter, Mathf.Abs(_normalizedHorizontalSpeed), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _movingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.moving), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _fallAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Falling), _character._animatorParameters, _character.PerformAnimatorSanityChecks);

		}

		/// <summary>
		/// When the character gets revived we reinit it again
		/// </summary>
		protected virtual void OnRevive()
		{
			Initialization();
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

		protected override void OnDisable()
		{
			base.OnDisable();
			if (_health != null)
			{
				_health.OnRevive -= OnRevive;
			}
		}

		public float GetHorizontal()
		{
			return _horizontalMovement;
		}
	}
}
