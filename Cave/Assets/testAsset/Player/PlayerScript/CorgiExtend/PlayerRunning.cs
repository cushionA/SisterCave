using Cysharp.Threading.Tasks;
using MoreMountains.Tools;
using System;
using UnityEngine;

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

		[Tooltip("���b�Ń{�^�������Ή���ɂȂ邩�̎w�W")]
		public float avoidBuffer = 0.22f;

		// animation parameters
		protected const string _runningAnimationParameterName = "Running";
		protected int _runningAnimationParameter;
		protected bool _runningStarted = false;

		protected PlayerRoll roll;

		//��𔻒�̂��߂̃{�^����������
		float pressTime;

		/// <summary>
		/// �X�^�~�i�؂ꎞ�Ɉ�b�_�b�V�����֎~����
		/// </summary>
		bool isStaminaLock;



		/// <summary>
		///�@�����ŁA�p�����[�^������������K�v������܂��B
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
            //�������̓X�^�������p�ł��Ȃ��悤��

            if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal || !_controller.State.IsGrounded || _movement.CurrentState == CharacterStates.MovementStates.Nostate)
            {
				pressTime = 0;
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

			if (_inputManager.AvoidButton.State.CurrentState == MMInput.ButtonStates.ButtonDown || _inputManager.AvoidButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed)
			{
				if (pressTime == 0)
				{
					pressTime = Time.time;
				}
				//�X�^�~�i�؂ꂵ���炵�΂炭����Ȃ��B
                if (isStaminaLock)
                {
					return;
                }

				if (_horizontalInput != 0)
				{
					Debug.Log("���ɂ�");
					RunStart();
				}
				
			}
			if (_inputManager.AvoidButton.State.CurrentState == MMInput.ButtonStates.ButtonUp)
			{


				if (_movement.CurrentState == CharacterStates.MovementStates.Running && Time.time - pressTime > avoidBuffer)
				{
					DelayRunSttop().Forget();
				}

				//�w�肵�����Ԉȓ��Ƀ{�^����������Ă�������
				if(Time.time - pressTime < avoidBuffer)
                {
					Debug.Log($"sdddsds{Time.time - pressTime < avoidBuffer}wdwww{pressTime}dddd{Time.time - pressTime}");
					roll.actRoll = true;
					pressTime = 0;
				}

				
			}
			//�X�^�~�i�؂�
			else if ((_horizontalInput == 0 || GManager.instance.stamina <= 0) && _movement.CurrentState == CharacterStates.MovementStates.Running)
            {

				RunStop();


				if (GManager.instance.stamina <= 0 && !isStaminaLock)
				{
					
					Debug.Log("��jjjjj");
					isStaminaLock = true;

					//���b�N�͎�������
					UnLock().Forget();
				}


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
			if (isPlayer)
			{
			//	Debug.Log($"�����݂�{isStaminaLock}");
                if (isStaminaLock)
                {
					Debug.Log($"��������������{_movement.CurrentState == CharacterStates.MovementStates.Running}");
                }
			
			}
			base.ProcessAbility();
			HandleRunningExit();

		}

		/// <summary>
		/// Tests if run state should be exited
		/// </summary>
		protected virtual void HandleRunningExit()
		{


			// ����
			if (!_controller.State.IsGrounded && (_movement.CurrentState == CharacterStates.MovementStates.Running) && _startFeedbackIsPlaying)
			{

				_movement.ChangeState(CharacterStates.MovementStates.Falling);
				StopFeedbacks();
			}
			/*
			 if we're not moving fast enough, we go back to idle
			if ((Mathf.Abs(_controller.Speed.x) < RunSpeed / 10) && (_movement.CurrentState == CharacterStates.MovementStates.Running) && _startFeedbackIsPlaying)
			{
				Debug.Log("��aaaaaaaaaa");
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				StopFeedbacks();
			}
			*/
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
				|| (_condition.CurrentState != CharacterStates.CharacterConditions.Normal) || _movement.CurrentState == CharacterStates.MovementStates.Nostate) // or if we're not walking
			{

				// we do nothing and exit
				return;
			}

			//  �v���C���[�����s�{�^���������A�n�ʂɂ��Ă��Ⴊ��ł��炸�A���R�ɓ������Ԃł���ꍇ�B 
			// ���ɁA�R���g���[���̃p�����[�^�ňړ����x��ύX���܂��B
			//�v��Horizonta��Movement�ɃA�N�Z�X���đ��x��ς��Ă���ă��P
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
		/// �����҂��ăX�^�~�i���b�N����
		/// </summary>
		/// <param name="isFirst"></param>
		/// <returns></returns>
		async UniTaskVoid UnLock(bool isFirst = true)
        {
			var token = this.GetCancellationTokenOnDestroy();

			if (isFirst)
			{
				//�w��b���҂�
				await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: token);
			}
            else
            {
				await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
			}

			//�{�^�������Ă��烍�b�N����
            if (_inputManager.AvoidButton.State.CurrentState == MMInput.ButtonStates.Off)
            {
                isStaminaLock = false;
            }
			//�����ĂȂ��Ȃ�܂����b�N
            else
            {
				UnLock(false).Forget();
            }
		}

		/// <summary>
		/// ������ƃ_�b�V���{�^��������������Ƃ��ł��_�b�V���p���ł���悤��
		/// </summary>
		/// <returns></returns>
		async UniTaskVoid DelayRunSttop()
		{
			var token = this.GetCancellationTokenOnDestroy();


			await UniTask.Delay(TimeSpan.FromSeconds(0.03), cancellationToken: token);

			//�����҂��ă{�^��������ĂȂ��Ȃ�
            if (_inputManager.AvoidButton.State.CurrentState == MMInput.ButtonStates.Off)
			{
				RunStop();
				pressTime = 0;
            }

		}



		/// <summary>
		/// At the end of each cycle, we send our Running status to the character's animator
		/// </summary>
		public override void UpdateAnimator()
		{
			if (isPlayer)
			{
			//	Debug.Log($"�Ȃ�Ȃ�{_animator.GetParameter(_runningAnimationParameter)}");
			}
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
				Debug.Log("jjghfhf");
				RunStop();
			}
			if (_animator != null)
			{
				MMAnimatorExtensions.UpdateAnimatorBool(_animator, _runningAnimationParameter, false, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
			}
		}




	}
}

