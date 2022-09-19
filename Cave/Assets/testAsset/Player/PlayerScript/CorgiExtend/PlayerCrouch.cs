using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	/// <summary>
	/// ���̃R���|�[�l���g���L�����N�^�[�ɒǉ�����ƁA���Ⴊ�񂾂�A�������肷�邱�Ƃ��ł���悤�ɂȂ�܂��B
	/// �A�j���[�^�[�p�����[�^�[�F���Ⴊ�ށA����
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/Player Crouch")]
	public class PlayerCrouch : MyAbillityBase
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "This component handles crouch and crawl behaviours. Here you can determine the crouch speed, and whether or not the collider should resize when crouched (to crawl into tunnels for example). If it should, please setup its new size here."; }

		[Header("Crawl")]

		/// if this is set to false, the character won't be able to crawl, just to crouch
		[Tooltip("false�ɐݒ肷��ƁA�L�����N�^�[�͔������Ƃ��ł����A���Ⴊ�ނ����ƂȂ�܂��B")]
		public bool CrawlAuthorized = true;
		/// the speed of the character when it's crouching
		[Tooltip("���Ⴊ�݈ړ��̑��x")]
		public float CrawlSpeed = 4f;

		[Space(10)]

		[Header("Crouching")]

		/// if this is true, the collider will be resized when crouched
		[Tooltip("���ꂪ true �̏ꍇ�A�R���C�_�[�͂��Ⴊ�񂾂Ƃ��Ƀ��T�C�Y����܂��B")]
		public bool ResizeColliderWhenCrouched = false;
		/// the size to apply to the collider when crouched (if ResizeColliderWhenCrouched is true, otherwise this will be ignored)
		/// note that changing the width of your collider when crouching will likely result in glitches when initiating a crouch on a slope, it's best not to change it
		[Tooltip("���Ⴊ�񂾂Ƃ��ɃR���C�_�ɓK�p����T�C�Y (ResizeColliderWhenCrouched �� true �̏ꍇ�B����ȊO�̏ꍇ�͖�������܂�)"�@+
		"���Ⴊ�ނƂ��ɃR���C�_�̕���ύX����ƁA�Ζʂł��Ⴊ�݂��J�n�����Ƃ��ɃO���b�`����������\���������̂ŁA�ύX���Ȃ��ق����悢���Ƃɒ��ӂ��Ă��������B")]
		public Vector2 CrouchedBoxColliderSize = new Vector2(1, 1);
		/// if this is false, the character will have to stop moving to start crouching
		[Tooltip("false�̏ꍇ�A�L�����N�^�[�͂��Ⴊ�ݎn�߂邽�߂Ɉړ����~����K�v������܂��B")]
		public bool CanCrouchWhileMoving = true;
		/// if this is true, the character is crouched and has an obstacle over its head that prevents it from getting back up again
		[MMReadOnly]
		[Tooltip("���������Ȃ�A�L�����N�^�[�͂��Ⴊ��ł��āA���̏�ɏ�Q��������A�Ăї����オ�邱�Ƃ��ł��܂���B")]
		public bool InATunnel;

		[Header("Cinemachine")]

		/// Whether or not to move the camera target, that will be used as the focus point for the Cinemachine virtual camera
		[Tooltip("Cinemachine���z�J�����̃t�H�[�J�X�|�C���g�Ƃ��Ďg�p�����J�����^�[�Q�b�g���ړ����邩�ǂ���")]
		public bool MoveCameraTarget = true;
		/// the offset to apply to the camera target
		[Tooltip("the offset to apply to the camera target")]
		public Vector3 CameraTargetOffset = new Vector3(0f, -3f, 0f);

		// animation parameters
		protected const string _crouchingAnimationParameterName = "Squat";
		protected const string _crawlingAnimationParameterName = "Crawling";
		protected int _crouchingAnimationParameter;
		protected int _crawlingAnimationParameter;
		protected bool _wasInATunnelLastFrame;

		/// <summary>
		/// On Start(), we set our tunnel flag to false
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			InATunnel = false;
		}

		/// <summary>
		/// Every frame, we check if we're crouched and if we still should be
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();
			DetermineState();
			CheckExitCrouch();
		}

		/// <summary>
		/// At the start of the ability's cycle, we check if we're pressing down. If yes, we call Crouch()
		/// </summary>
		protected override void HandleInput()
		{
			// Crouch Detection : if the player is pressing "down" and if the character is grounded and the crouch action is enabled
			//������Ă��炵�Ⴊ��
			if (_verticalInput < -_inputManager.Threshold.y)
			{
				Crouch();
			}
		}

		/// <summary>
		/// If we're pressing down, we check if we can crouch or crawl, and change states accordingly
		/// </summary>
		public virtual void Crouch()
		{
			if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
			{
				return;
			}

			if (!AbilityAuthorized // if the ability is not permitted
				|| (_condition.CurrentState != CharacterStates.CharacterConditions.Normal) // or if we're not in our normal stance
				|| (!_controller.State.IsGrounded) // or if we're grounded
				|| (_movement.CurrentState == CharacterStates.MovementStates.Gripping)) // or if we're gripping
			{
				// we do nothing and exit
				return;
			}

			if (!CanCrouchWhileMoving && (Mathf.Abs(_horizontalInput) > _inputManager.Threshold.x))
			{
				return;
			}

			// if this is the first time we're here, we trigger our sounds
			if ((_movement.CurrentState != CharacterStates.MovementStates.Crouching) && (_movement.CurrentState != CharacterStates.MovementStates.Crawling))
			{
				// we play the crouch start sound 
				PlayAbilityStartFeedbacks();
			}

			// we set the character's state to Crouching and if it's also moving we set it to Crawling
			_movement.ChangeState(CharacterStates.MovementStates.Crouching);
			if ((Mathf.Abs(_horizontalInput) > 0) && (CrawlAuthorized))
			{
				_movement.ChangeState(CharacterStates.MovementStates.Crawling);
			}

			// we resize our collider to match the new shape of our character (it's usually smaller when crouched)
			if (ResizeColliderWhenCrouched)
			{
				_controller.ResizeCollider(CrouchedBoxColliderSize);
				Invoke("RecalculateRays", Time.deltaTime * 10);
			}

			// we change our character's speed
			if (_characterHorizontalMovement != null)
			{
				_characterHorizontalMovement.MovementSpeed = CrawlSpeed;
			}

			// we prevent movement if we can't crawl
			if (!CrawlAuthorized)
			{
				_characterHorizontalMovement.MovementSpeed = 0f;
			}

			// we make our camera look down
			if (_sceneCamera != null)
			{
				_sceneCamera.LookDown();
			}
			if (MoveCameraTarget)
			{
				_character.SetCameraTargetOffset(CameraTargetOffset);
			}
		}

		/// <summary>
		/// ���Ⴊ��ł����Ԃ��甇���悤�Ɉړ����邩�A���̋t���s�����ǂ����𖈃t���[���m�F����B
		/// </summary>
		protected virtual void DetermineState()
		{
			float threshold = (_inputManager != null) ? _inputManager.Threshold.x : 0f;

			if ((_movement.CurrentState == CharacterStates.MovementStates.Crouching) || (_movement.CurrentState == CharacterStates.MovementStates.Crawling))
			{
				if ((Mathf.Abs(_horizontalInput) > threshold) && (CrawlAuthorized))
				{
					_movement.ChangeState(CharacterStates.MovementStates.Crawling);
				}
				else
				{
					_movement.ChangeState(CharacterStates.MovementStates.Crouching);
				}
			}
		}

		/// <summary>
		/// Every frame, we check to see if we should exit the Crouching (or Crawling) state
		/// </summary>
		protected virtual void CheckExitCrouch()
		{
			if (_inputManager == null)
			{
				if ((_movement.CurrentState == CharacterStates.MovementStates.Crouching)
				|| (_movement.CurrentState == CharacterStates.MovementStates.Crawling))
				{
					ExitCrouch();
				}
			}

			if ((_movement.CurrentState == CharacterStates.MovementStates.Crouching)
				|| (_movement.CurrentState == CharacterStates.MovementStates.Crawling))
			{
				// but we're not pressing down anymore, or we're not grounded anymore
				if ((!_controller.State.IsGrounded) || (_verticalInput >= -_inputManager.Threshold.y))
				{
					ExitCrouch();
				}
			}

			// if we're currently grounded
			if (_wasInATunnelLastFrame && (_movement.CurrentState == CharacterStates.MovementStates.Pushing))
			{
				if ((!_controller.State.IsGrounded) || (_verticalInput >= -_inputManager.Threshold.y))
				{
					ExitCrouch();
				}
			}
		}

		/// <summary>
		/// ���Ⴊ�񂾏�Ԃ���ޏo
		/// ���ɏ�Q�����Ȃ��Ȃ��
		/// </summary>
		public virtual void ExitCrouch()
		{
			// �ʏ�̃T�C�Y�ɖ߂邾���̗]�T�����邩�ǂ����A��̃��C�L���X�g�𓊉e���Ă݂܂����B
			InATunnel = !_controller.CanGoBackToOriginalSize();
			_wasInATunnelLastFrame = InATunnel;
			// �g���l�����łȂ��ꍇ�́A�ʏ�̃T�C�Y�ɖ߂����Ƃ��ł��܂��B
			if (!InATunnel)
			{
				// we return to normal walking speed
				if (_characterHorizontalMovement != null)
				{
					_characterHorizontalMovement.ResetHorizontalSpeed();
				}

				if (_sceneCamera != null)
				{
					_sceneCamera.ResetLookUpDown();
				}
				if (MoveCameraTarget)
				{
					_character.SetCameraTargetOffset(Vector3.zero);
				}

				// we play our exit feedback
				StopStartFeedbacks();
				PlayAbilityStopFeedbacks();

				// we go back to Idle state and reset our collider's size
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				_controller.ResetColliderSize();
				Invoke("RecalculateRays", Time.deltaTime * 10);
			}
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter(_crouchingAnimationParameterName, AnimatorControllerParameterType.Bool, out _crouchingAnimationParameter);
			RegisterAnimatorParameter(_crawlingAnimationParameterName, AnimatorControllerParameterType.Bool, out _crawlingAnimationParameter);
		}

		/// <summary>
		/// At the end of the ability's cycle, we send our current crouching and crawling states to the animator
		/// </summary>
		public override void UpdateAnimator()
		{
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _crouchingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Crouching), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _crawlingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Crawling), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
		}

		/// <summary>
		/// Recalculates the raycast's origin points.
		/// </summary>
		protected virtual void RecalculateRays()
		{
			_character.RecalculateRays();
		}

		/// <summary>
		/// On reset ability, we cancel all the changes made
		/// </summary>
		public override void ResetAbility()
		{
			base.ResetAbility();
			if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
			{
				ExitCrouch();
			}

			if (_animator != null)
			{
				MMAnimatorExtensions.UpdateAnimatorBool(_animator, _crouchingAnimationParameter, false, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
				MMAnimatorExtensions.UpdateAnimatorBool(_animator, _crawlingAnimationParameter, false, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
			}
		}
	}
}