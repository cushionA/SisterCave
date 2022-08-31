using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using Rewired.Integration.CorgiEngine;
using DarkTonic.MasterAudio;
namespace MoreMountains.CorgiEngine
{
	/// <summary>
	/// Add this component to a character and it'll be able to run
	/// Animator parameters : Running
	/// </summary>
	//[AddComponentMenu("Corgi Engine/Character/Abilities/Character Run")]
	public class PlayerRunning : MyAbillityBase
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component allows your character to change speed (defined here) when pressing the run button."; }

		[Header("Speed")]
		/// the speed of the character when it's running
		[Tooltip("the speed of the character when it's running")]
		public float RunSpeed = 16f;

		[Header("Input")]
		/// if this is set to false, will ignore input (use methods via script instead)
		[Tooltip("if this is set to false, will ignore input (use methods via script instead)")]
		public bool ReadInput = true;
		public bool ShouldRun { get; protected set; }

		[Header("AutoRun")]
		/// whether or not run should auto trigger if you move the joystick far enough
		[Tooltip("whether or not run should auto trigger if you move the joystick far enough")]
		public bool AutoRun = false;
		/// the input threshold on the joystick (normalized)
		[Tooltip("the input threshold on the joystick (normalized)")]
		public float AutoRunThreshold = 0.6f;

		[Tooltip("何秒でボタン離せば回避になるかの指標")]
		public float avoidBuffer = 0.22f;

		// animation parameters
		protected const string _runningAnimationParameterName = "Running";
		protected int _runningAnimationParameter;
		protected bool _runningStarted = false;

		protected PlayerRoll roll;

		//回避判定のためのボタン押し時間
		float pressTime;

		/// <summary>
		///　ここで、パラメータを初期化する必要があります。
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			roll = GetComponent<PlayerRoll>();
		}

		/// <summary>
		/// At the beginning of each cycle, we check if we've pressed or released the run button
		/// </summary>
		protected override void HandleInput()
		{
            //こっちはスタン中利用できないように

            if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
            {
				return;
            }

			#region
			/*
			if (!ReadInput)
			{
				if (!ShouldRun && (_movement.CurrentState == CharacterStates.MovementStates.Running))
				{
					RunStop();
				}

				if ((_movement.CurrentState != CharacterStates.MovementStates.Running) && ShouldRun)
				{
					RunStart();
				}
				return;
			}
			*/
			#endregion

			if (_inputManager.AvoidButton.State.CurrentState == MMInput.ButtonStates.ButtonDown || _inputManager.RunButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed)
			{
				RunStart();
				
			}
			if (_inputManager.AvoidButton.State.CurrentState == MMInput.ButtonStates.ButtonUp)
			{
				RunStop();
			//	Debug.Log($"{pressTime}秒");
				//指定した時間以内にボタンが離されていたら回避
				if(pressTime < avoidBuffer)
                {
					roll.actRoll = true;
					
                }
				pressTime = 0;
			}

			if (AutoRun)
			{
				if (_inputManager.PrimaryMovement.magnitude > AutoRunThreshold)
				{
					_inputManager.RunButton.State.ChangeState(MMInput.ButtonStates.ButtonPressed);
				}
				else
				{
					_inputManager.RunButton.State.ChangeState(MMInput.ButtonStates.ButtonUp);
					RunStop();
				}
			}
		}

		//Update
		public override void ProcessAbility()
		{
			base.ProcessAbility();
			HandleRunningExit();
			//走っている時、のコード
			//スタミナ減らしたりできるよぉ？
			if(_movement.CurrentState == CharacterStates.MovementStates.Running)
            {
				pressTime += _controller.DeltaTime;
			}
		}

		/// <summary>
		/// Tests if run state should be exited
		/// </summary>
		protected virtual void HandleRunningExit()
		{
			// if we're running and not grounded, we change our state to Falling
			if (!_controller.State.IsGrounded && (_movement.CurrentState == CharacterStates.MovementStates.Running) && _startFeedbackIsPlaying)
			{
				_movement.ChangeState(CharacterStates.MovementStates.Falling);
				StopFeedbacks();
			}

			// if we're not moving fast enough, we go back to idle
			if ((Mathf.Abs(_controller.Speed.x) < RunSpeed / 10) && (_movement.CurrentState == CharacterStates.MovementStates.Running) && _startFeedbackIsPlaying)
			{
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				StopFeedbacks();
			}

			if ((!_controller.State.IsGrounded) && _startFeedbackIsPlaying)
			{
				StopFeedbacks();
			}

			if ((_movement.CurrentState != CharacterStates.MovementStates.Running) && _startFeedbackIsPlaying)
			{
				StopFeedbacks();
			}
		}

		/// <summary>
		/// Causes the character to start running.
		/// </summary>
		public virtual void RunStart()
		{
			if (!AbilityAuthorized // if the ability is not permitted
				|| (!_controller.State.IsGrounded) // or if we're not grounded
				|| (_condition.CurrentState != CharacterStates.CharacterConditions.Normal) // or if we're not in normal conditions
				|| (_movement.CurrentState != CharacterStates.MovementStates.moving)) // or if we're not walking
			{
				// we do nothing and exit
				return;
			}

			//  プレイヤーが実行ボタンを押し、地面にいてしゃがんでおらず、自由に動ける状態である場合。 
			// 次に、コントローラのパラメータで移動速度を変更します。
			//要はHorizontaｌMovementにアクセスして速度を変えてるってワケ
			if (_characterHorizontalMovement != null)
			{
				_characterHorizontalMovement.MovementSpeed = RunSpeed;
			}

			// if we're not already running, we trigger our sounds
			if (_movement.CurrentState != CharacterStates.MovementStates.Running)
			{
				PlayAbilityStartFeedbacks();
			}

			_movement.ChangeState(CharacterStates.MovementStates.Running);
		}

		/// <summary>
		/// Causes the character to stop running.
		/// </summary>
		public virtual void RunStop()
		{
			// if the run button is released, we revert back to the walking speed.
			if ((_characterHorizontalMovement != null) && (_movement.CurrentState != CharacterStates.MovementStates.Crouching))
			{
				_characterHorizontalMovement.ResetHorizontalSpeed();
			}
			if (_movement.CurrentState == CharacterStates.MovementStates.Running)
			{
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
			}
			StopFeedbacks();
		}

		/// <summary>
		/// Forces run state or not (if we're not in ReadInput mode)
		/// </summary>
		/// <param name="state"></param>
		public virtual void ForceRun(bool state)
		{
			ShouldRun = state;
		}

		/// <summary>
		/// Stops all run feedbacks
		/// </summary>
		protected virtual void StopFeedbacks()
		{
			if (_startFeedbackIsPlaying)
			{
				StopStartFeedbacks();
				PlayAbilityStopFeedbacks();
			}
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter(_runningAnimationParameterName, AnimatorControllerParameterType.Bool, out _runningAnimationParameter);
		}

		/// <summary>
		/// At the end of each cycle, we send our Running status to the character's animator
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _runningAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Running), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
		}

		/// <summary>
		/// On reset ability, we cancel all the changes made
		/// </summary>
		public override void ResetAbility()
		{
			base.ResetAbility();
			if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
			{
				RunStop();
			}
			if (_animator != null)
			{
				MMAnimatorExtensions.UpdateAnimatorBool(_animator, _runningAnimationParameter, false, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
			}
		}
	}
}

