using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	/// このクラスをキャラクターに追加すると、CorgiControllerを搭載したオブジェクトを押したり引いたりすることができるようになる。
	/// アニメーターパラメーター：押す(bool)、引く(bool)
	/// これ本質的にはPushableに自分のコーギーコントローラーを渡す、そして前にレイを出してオブジェクトがあるか探すくらいしかない
	[AddComponentMenu("Corgi Engine/Character/Abilities/MyCollision")]
	public class MyCollision : MyAbillityBase
	{
		public override string HelpBoxText() { return "This component allows your character to push blocks. This is not a mandatory component, it will just override CorgiController push settings, and allow you to have a dedicated push animation."; }


		/// If this is set to true, the Character will be able to push blocks
		[Tooltip("trueに設定すると、キャラクターがブロックを押せるようになります。")]
		public bool CanPush = true;
		/// If this is set to true, the Character will be able to pull blocks. Note that this requires ButtonBased to be true.
		[Tooltip("trueに設定すると、キャラクターがブロックを引っ張ることができるようになります。ただし、ButtonBasedがtrueである必要があります。")]
		public bool CanPull = true;
		/// if this is true, the Character will only be able to push objects while grounded
		[Tooltip("この値が true の場合、キャラクタは接地している間のみオブジェクトを押すことができます。")]
		public bool PushWhenGroundedOnly = true;
		/// the length of the raycast used to detect if we're colliding with a pushable object. Increase this if your animation is flickering.
		[Tooltip("押せるオブジェクトと衝突しているかどうかを検出するために使用するレイキャストの長さ。アニメーションがちらつく場合は、この値を大きくしてください。")]
		public float DetectionRaycastLength = 0.2f;
		/// the minimum horizontal speed below which we don't consider the character pushing anymore
		[Tooltip("それ以下の速度では、キャラクターを押しているとみなされない最小の水平速度")]
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

			// プッシュ可能なオブジェクトと衝突しているかどうかを確認するために、前方に光線を投射します。
			_raycastDirection = _character.IsFacingRight ? transform.right : -transform.right;
			_raycastOrigin = _controller.ColliderCenterPosition + _raycastDirection * (_controller.Width() / 2);

			// we cast our ray to see if we're hitting something
			//何か当たってるか確認するため光線を当てる
			RaycastHit2D hit = MMDebug.RayCast(_raycastOrigin, _raycastDirection, DetectionRaycastLength, _controller.PlatformMask, Color.green, _controller.Parameters.DrawRaycastsGizmos);
			if (hit)
			{
				//ヒットした物にPushableがついていたら
				if (hit.collider.gameObject.MMGetComponentNoAlloc<Pushable>() != null)
				{
					_collidingWithPushable = true;
				}
			}

			// プッシャブルに衝突して、条件が揃えば、プッシュを開始します。
			//地面にいてPushableがあるものに衝突して、速度が押せるに十分なだけあって押せる状況なら
			//押せる上限速度も付けてあまり早く通りすぎる系の技はスルーとかもいいかも
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
				//物によっては押すモーションつけていいかも
				//_movement.ChangeState(CharacterStates.MovementStates.Pushing);
				pushNow = true;
			}


			if (hit && pushNow && (_pushedObject == null))
			{
				_pushedObject = hit.collider.gameObject.MMGetComponentNoAlloc<Pushable>();
				//pushableに今現在そのオブジェクトを押してるコントローラーを読み込ませる。
				_pushedObject.Attach(_controller);
				//_character.CanFlip = false;
				_movementMultiplierStorage = _characterHorizontalMovement.PushSpeedMultiplier;
				_characterHorizontalMovement.PushSpeedMultiplier = _pushedObject.PushSpeed;
			}

			//位置関係と進行方向的に引いてると判断できたら
			if (((_controller.Speed.x > MinimumPushSpeed)
				 && (pushNow)
				 && (_pushedObject.transform.position.x < this.transform.position.x))
				||
				((_controller.Speed.x < -MinimumPushSpeed)
				 && (pushNow)
				 && (_pushedObject.transform.position.x > this.transform.position.x)))
			{
				//引けないなら押すのをやめる
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
		/// プッシュを停止して状態を変更すべきかどうかをチェックする
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