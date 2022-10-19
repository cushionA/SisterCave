using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	/// ���̃N���X���L�����N�^�[�ɒǉ�����ƁACorgiController�𓋍ڂ����I�u�W�F�N�g����������������肷�邱�Ƃ��ł���悤�ɂȂ�B
	/// �A�j���[�^�[�p�����[�^�[�F����(bool)�A����(bool)
	/// ����{���I�ɂ�Pushable�Ɏ����̃R�[�M�[�R���g���[���[��n���A�����đO�Ƀ��C���o���ăI�u�W�F�N�g�����邩�T�����炢�����Ȃ�
	[AddComponentMenu("Corgi Engine/Character/Abilities/MyCollision")]
	public class MyCollision : MyAbillityBase
	{
		public override string HelpBoxText() { return "This component allows your character to push blocks. This is not a mandatory component, it will just override CorgiController push settings, and allow you to have a dedicated push animation."; }


		/// If this is set to true, the Character will be able to push blocks
		[Tooltip("true�ɐݒ肷��ƁA�L�����N�^�[���u���b�N��������悤�ɂȂ�܂��B")]
		public bool CanPush = true;
		/// If this is set to true, the Character will be able to pull blocks. Note that this requires ButtonBased to be true.
		[Tooltip("true�ɐݒ肷��ƁA�L�����N�^�[���u���b�N���������邱�Ƃ��ł���悤�ɂȂ�܂��B�������AButtonBased��true�ł���K�v������܂��B")]
		public bool CanPull = true;
		/// if this is true, the Character will only be able to push objects while grounded
		[Tooltip("���̒l�� true �̏ꍇ�A�L�����N�^�͐ڒn���Ă���Ԃ̂݃I�u�W�F�N�g���������Ƃ��ł��܂��B")]
		public bool PushWhenGroundedOnly = true;
		/// the length of the raycast used to detect if we're colliding with a pushable object. Increase this if your animation is flickering.
		[Tooltip("������I�u�W�F�N�g�ƏՓ˂��Ă��邩�ǂ��������o���邽�߂Ɏg�p���郌�C�L���X�g�̒����B�A�j���[�V������������ꍇ�́A���̒l��傫�����Ă��������B")]
		public float DetectionRaycastLength = 0.2f;
		/// the minimum horizontal speed below which we don't consider the character pushing anymore
		[Tooltip("����ȉ��̑��x�ł́A�L�����N�^�[�������Ă���Ƃ݂Ȃ���Ȃ��ŏ��̐������x")]
		public float MinimumPushSpeed = 0.05f;

		protected bool _collidingWithPushable = false;
		protected Vector3 _raycastDirection;
		protected Vector3 _raycastOrigin;
		protected Pushable _pushedObject;
		protected float _movementMultiplierStorage;
		protected bool _pulling = false;
		protected PlayerRunning _characterRun;

		// animation parameters
		protected const string _pushingAnimationParameterName = "Pushing";
		protected const string _pullingAnimationParameterName = "Pulling";
		protected int _pushingAnimationParameter;
		protected int _pullingAnimationParameter;
		bool pushNow;

		/// <summary>
		/// On Start(), we initialize our various flags
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			_characterRun = _character?.FindAbility<PlayerRunning>();
		}

		/// <summary>
		/// Every frame we override parameters if needed and cast a ray to see if we're actually pushing anything
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();

			if (!CanPush || !AbilityAuthorized)
			{
				return;
			}

			CheckForPushEnd();

			if (_movement.CurrentState != CharacterStates.MovementStates.Attack && _movement.CurrentState != CharacterStates.MovementStates.Guard
				&& _movement.CurrentState != CharacterStates.MovementStates.GuardMove)
			{
				return;
			}

			// we set our flag to false
			_collidingWithPushable = false;

			// �v�b�V���\�ȃI�u�W�F�N�g�ƏՓ˂��Ă��邩�ǂ������m�F���邽�߂ɁA�O���Ɍ����𓊎˂��܂��B
			_raycastDirection = _character.IsFacingRight ? transform.right : -transform.right;
			_raycastOrigin = _controller.ColliderCenterPosition + _raycastDirection * (_controller.Width() / 2);

			// we cast our ray to see if we're hitting something
			//�����������Ă邩�m�F���邽�ߌ����𓖂Ă�
			RaycastHit2D hit = MMDebug.RayCast(_raycastOrigin, _raycastDirection, DetectionRaycastLength, _controller.PlatformMask, Color.green, _controller.Parameters.DrawRaycastsGizmos);
			if (hit)
			{
				//�q�b�g��������Pushable�����Ă�����
				if (hit.collider.gameObject.MMGetComponentNoAlloc<Pushable>() != null)
				{
					_collidingWithPushable = true;
				}
			}

			// �v�b�V���u���ɏՓ˂��āA�����������΁A�v�b�V�����J�n���܂��B
			//�n�ʂɂ���Pushable��������̂ɏՓ˂��āA���x��������ɏ\���Ȃ��������ĉ�����󋵂Ȃ�
			//�����������x���t���Ă��܂葁���ʂ肷����n�̋Z�̓X���[�Ƃ�����������
			if (_controller.State.IsGrounded
				&& _collidingWithPushable
				&& Mathf.Abs(_controller.ExternalForce.x) >= MinimumPushSpeed
				&& _condition.CurrentState != CharacterStates.CharacterConditions.Stunned)
			{
				if (_movement.CurrentState == CharacterStates.MovementStates.Running)
				{
					if (_characterRun != null)
					{
						_characterRun.RunStop();
					}
				}
				PlayAbilityStartFeedbacks();
				//���ɂ���Ă͉������[�V�������Ă�������
				//_movement.ChangeState(CharacterStates.MovementStates.Pushing);
				pushNow = true;
			}


			if (hit && pushNow && (_pushedObject == null))
			{
				_pushedObject = hit.collider.gameObject.MMGetComponentNoAlloc<Pushable>();
				//pushable�ɍ����݂��̃I�u�W�F�N�g�������Ă�R���g���[���[��ǂݍ��܂���B
				_pushedObject.Attach(_controller);
				//_character.CanFlip = false;
				_movementMultiplierStorage = _characterHorizontalMovement.PushSpeedMultiplier;
				_characterHorizontalMovement.PushSpeedMultiplier = _pushedObject.PushSpeed;
			}

			//�ʒu�֌W�Ɛi�s�����I�Ɉ����Ă�Ɣ��f�ł�����
			if (((_controller.Speed.x > MinimumPushSpeed)
				 && (pushNow)
				 && (_pushedObject.transform.position.x < this.transform.position.x))
				||
				((_controller.Speed.x < -MinimumPushSpeed)
				 && (pushNow)
				 && (_pushedObject.transform.position.x > this.transform.position.x)))
			{
				//�����Ȃ��Ȃ牟���̂���߂�
				if (!CanPull)
				{
					StopPushing();
				}
				else
				{
					_pulling = true;
				}
			}
			else
			{
				_pulling = false;
			}
		}

		/// <summary>
		/// �v�b�V�����~���ď�Ԃ�ύX���ׂ����ǂ������`�F�b�N����
		/// </summary>
		protected virtual void CheckForPushEnd()
		{
			if (_movement.CurrentState != CharacterStates.MovementStates.Attack && _movement.CurrentState != CharacterStates.MovementStates.Guard
				&& _movement.CurrentState != CharacterStates.MovementStates.GuardMove)
			{
				StopPushing();
			}

			if (!_collidingWithPushable && pushNow)
			{
				StopPushing();
			}

			if (((_pushedObject == null) && pushNow)
				|| ((_pushedObject != null) && Mathf.Abs(_controller.Speed.x) <= MinimumPushSpeed && pushNow))
			{
				// we reset the state
				//	_movement.ChangeState(CharacterStates.MovementStates.Idle);
				pushNow = false;
				PlayAbilityStopFeedbacks();
				StopStartFeedbacks();
			}

			if ((!pushNow) && _startFeedbackIsPlaying)
			{
				PlayAbilityStopFeedbacks();
				StopStartFeedbacks();
			}
		}

		/// <summary>
		/// Stops the character from pushing or pulling
		/// </summary>
		protected virtual void StopPushing()
		{
			if (_pushedObject == null)
			{
				return;
			}
			_pushedObject.Detach(_controller);
			_pushedObject = null;
			_character.CanFlip = true;
			_characterHorizontalMovement.PushSpeedMultiplier = _movementMultiplierStorage;
			_pulling = false;
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			//RegisterAnimatorParameter(_pushingAnimationParameterName, AnimatorControllerParameterType.Bool, out _pushingAnimationParameter);
			//RegisterAnimatorParameter(_pullingAnimationParameterName, AnimatorControllerParameterType.Bool, out _pullingAnimationParameter);
		}

		/// <summary>
		/// Sends the current state of the push and pull states to the character's animator
		/// </summary>
		public override void UpdateAnimator()
		{
			//MMAnimatorExtensions.UpdateAnimatorBool(_animator, _pushingAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Pushing), _character._animatorParameters, _character.PerformAnimatorSanityChecks);
			//	MMAnimatorExtensions.UpdateAnimatorBool(_animator, _pullingAnimationParameter, _pulling, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
		}

		/// <summary>
		/// On reset ability, we cancel all the changes made
		/// </summary>
		public override void ResetAbility()
		{
			base.ResetAbility();
			//	if (_animator != null)
			//{
			//MMAnimatorExtensions.UpdateAnimatorBool(_animator, _pushingAnimationParameter, false, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
			//MMAnimatorExtensions.UpdateAnimatorBool(_animator, _pullingAnimationParameter, false, _character._animatorParameters, _character.PerformAnimatorSanityChecks);
			//}
		}
	}
}