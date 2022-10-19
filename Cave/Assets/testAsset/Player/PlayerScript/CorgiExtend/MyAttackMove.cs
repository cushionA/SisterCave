using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	///���̃N���X�Ńv���C���[�͍U�����ɑO�i����
	///�U���J�n���牽�b��ɉ��b�Ԉړ�����݂����Ȋ���
	///���C�L���X�g�ŏ�Q�����m�F������~�܂邩�G������
	///�O������ړ��������w�肵������ł���i���b�N�I���œG�̈ʒu�E���j
	///�ڐG���͒�~�������đO�i���̓��I�ׂ�
	///�����đO�i���͑��x�����ɂȂ�A�\�肵�Ă������ԂŎ~�܂�
	///�����U���p�ɋ����������Ői�ݑ�����悤�ȃI�v�V������
	///�R���C�_�[�̃T�C�Y�����C�L���X�g�����ɑ���
	///�K�[�h�͂���Ȃ�
	///�u�u�u�u�u���[�g�I�u�W�F�N�g�ɂ���v�v�v�v�v
	///�ȒP�Ɍ����ƍU�����O�i����@�\�A�Փ˂����Ԃő��l�⎩�����~������R�[�h������A�Ƃ���������
	///
	[AddComponentMenu("Corgi Engine/Character/Abilities/MyAttackMove")]
	public class MyAttackMove : MyAbillityBase
	{
		public override string HelpBoxText() { return "This component allows your character to push blocks. This is not a mandatory component, it will just override CorgiController push settings, and allow you to have a dedicated push animation."; }

		 enum RushState
        {
			��~,
			�ҋ@,
			�ړ�
        }

		public enum AttackContactType
        {
		�@�@�ʉ�,//�ʂ蔲����B�ڐG�Ȃ�
		  �@��~,//�G�ƐڐG������~�܂�
		   ����//�G�������Đi��ł���
        }


		/// if this is true, the Character will only be able to push objects while grounded
		[Tooltip("���̒l�� true �̏ꍇ�A�L�����N�^�͐ڒn���Ă���Ԃ̂݃I�u�W�F�N�g���������Ƃ��ł��܂��B")]
		public bool PushWhenGroundedOnly = false;

		/// the length of the raycast used to detect if we're colliding with a pushable object. Increase this if your animation is flickering.
		[Tooltip("������I�u�W�F�N�g�ƏՓ˂��Ă��邩�ǂ��������o���邽�߂Ɏg�p���郌�C�L���X�g�̒����B�A�j���[�V������������ꍇ�́A���̒l��傫�����Ă��������B")]
		public float DetectionRaycastLength = 5f;

		/// <summary>
		/// ���ꂪ�^�Ȃ牟���Ȃ�
		/// </summary>
		[SerializeField]
		protected bool _cantPush;

		public bool Pushable
        {
            get { return _cantPush; }
        }

		///�O������ݒ肷����
        #region
        /// <summary>
        /// �ːi���G�ƂԂ��������ǂ�ȑΉ������邩
        /// </summary>
        [HideInInspector]
		public AttackContactType _contactType;

		/// <summary>
		/// ���b��Ɉړ��J�n���邩
		/// </summary>
		[HideInInspector]
		public float _moveStartTime;
		/// <summary>
		/// ���b�Ԉړ����邩
		/// </summary>
		[HideInInspector]
		public float _moveDuration;


		/// <summary>
		/// �����U���Ŗ������ɏՓ˂ƑO�i����t���O
		/// ���������Ȃ��i��
		/// </summary>
		bool fallMove;


        #endregion

        protected Vector3 _raycastDirection;
		protected Vector3 _raycastOrigin;



		/// <summary>
		/// �����������Ă邩�ǂ���
		/// </summary>
		bool pushNow;

		/// <summary>
		/// �ړ����Ԍv��
		/// ���̎��Ԃ��o�߂��邩�A
		/// </summary>
		float _moveTime;




		/// <summary>
		/// ���b�����Ăǂꂭ�炢�̋����ړ����邩�Ŋ���o�������x
		/// </summary>
		float _moveSpeed;

		/// <summary>
		/// ���݂̓ːi�̏��
		/// </summary>
		RushState nowState;

		[SerializeField]
		LayerMask HitObjectMask;

		/// <summary>
		/// �ێ����鋗��
		/// �Փ˂���Ƃ��̋����̕����������������
		/// </summary>
		float keepDistance;

		/// <summary>
		/// �����Ώۂ̂���
		/// </summary>
		MyAttackMove pushObject;



		/// <summary>
		/// ���ڐG���Ă邩�ǂ���
		/// </summary>
		bool contactNow;

		/// <summary>
		/// �K�[�h���Ƃ��̂������瓮���Ȃ��Ƃ����n�_
		/// </summary>
		float anchorPosition;


		Vector2 posi = new Vector2();

		/// <summary>
		/// �A�i�U�[���b�N�Ń��b�N���ꂽ�����E���ɂ��邩�ǂ���
		/// </summary>
		bool isRightLock;

		/// <summary>
		/// ��O�҂ɂ��s���𐧌�����Ă���ꍇ
		/// �ڂ̑O�ɃK�[�h���Ē�~���Ă���G��������
		/// </summary>
		public bool anotherLock;

		/// <summary>
		/// On Start(), we initialize our various flags
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
		//	HitObjectMask |= _controller.PlatformMask;
			
		}

		/// <summary>
		/// Every frame we override parameters if needed and cast a ray to see if we're actually pushing anything
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();

			if (!AbilityAuthorized)
			{
				return;
			}


			//�Փˍs������Ȃ��Ƃ��͖���
			//����ł鎞�Ƃ���������
			if ((_movement.CurrentState != CharacterStates.MovementStates.Attack && _movement.CurrentState != CharacterStates.MovementStates.Guard
				&& _movement.CurrentState != CharacterStates.MovementStates.GuardMove) || _condition.CurrentState == CharacterStates.CharacterConditions.Stunned ||
				_condition.CurrentState == CharacterStates.CharacterConditions.Dead)
			{
				if(pushObject != null)
                {

					//���b�N�������ďՓ˃I�u�W�F�N�g���̂Ă�
					pushObject.anotherLock = false;
					pushObject = null;
					contactNow = false;
					anchorPosition = transform.position.x;

					//�����̂���߂�
					pushNow = false;
				}
                if (nowState != RushState.��~)
                {
					StopRush();
                }
                if (anotherLock)
                {
				//	Debug.Log($"����������{isRightLock}");
					//���񂾂肵�������
					if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                    {
						anotherLock = false;
						return;
					}
					//�E�ɂ��Č�ނ��A���ɂ��đO�i��
					if((isRightLock && _controller.Speed.x > 0) || (!isRightLock && _controller.Speed.x < 0))
                    {
						anotherLock = false;
						return;
					}

					posi.Set(anchorPosition, transform.position.y);
						transform.position = posi;
					//_controller.SetHorizontalForce(0);

				}

				return;
			}


			//�ːi�@�\����
			AttackRush();

			//��Q�����m����
			CollideObjectCheck();





		}

		/// <summary>
		/// RushState���O������ς��Ȃ��Ɠ����Ȃ�
		/// �ړ��������ǂ�������
		/// </summary>
		 void AttackRush()
        {
			//Debug.Log($"a{nowState}");
			if (nowState != RushState.��~)
			{
			//	Debug.Log($"b{(_moveTime)}ggg{_moveStartTime}{nowState}����{_moveTime >= _moveStartTime && nowState == RushState.�ҋ@}");
				//b0.0893481�@0.73�@�ҋ@�@����False
				_moveTime += _controller.DeltaTime;

				//�ړ��J�n���Ԃ�������ړ��J�n
				if (_moveTime >= _moveStartTime && nowState == RushState.�ҋ@)
				{

					nowState = RushState.�ړ�;
					_moveTime = 0;
				}
				else if (nowState == RushState.�ړ�)
				{
					//�����U��������Ȃ��Ĉړ�������Ԓ��������~
					//�����U�����Œn�ʂɂ��Ă��I���
					if ((!fallMove && _moveTime >= _moveDuration) || (fallMove && _controller.State.IsGrounded))
					{
						StopRush();
					}
					//�ːi���̏���
					else
					{

						//�����Ă鎞�̏���
						//�����Ă鑊��̃|�W�V�����𑀍�
						if (pushNow)
						{

							_raycastDirection = _controller.Speed.x > 0 ? transform.right : -transform.right;
							_raycastOrigin = _controller.ColliderCenterPosition + _raycastDirection * (_controller.Width() / 2);
							RaycastHit2D hit = MMDebug.RayCast(_raycastOrigin, _raycastDirection, keepDistance, _controller.PlatformMask, Color.green, _controller.Parameters.DrawRaycastsGizmos);

							//�ǂɓ�����܂ł͉����Đi��
							//���Ƒ��肪������΂���Ă��Ȃ��Ȃ�
                            if (hit.collider == null && !BlowNow())
                            {
								
								posi.Set(_raycastOrigin.x + (pushObject.ReturnSizeX() / 2), pushObject.transform.position.y);
									transform.position = posi;
								//_controller.SetHorizontalForce(0);
							}
                            else if(hit.collider != null)
                            {
								
								StopRush();
								return;
                            }
						}

						int direction = _character.IsFacingRight ? 1 : -1;
					//	Debug.Log("������");
						//��������ړ�����
						//_controller.SetHorizontalForce(1000000000000000000 * direction);
						_controller.SetHorizontalForce(_moveSpeed * direction);
					}
				}
			}
        }





		/// <summary>
		/// Stops the character from pushing or pulling
		/// </summary>
		protected virtual void StopRush()
		{
			_controller.SetHorizontalForce(0);
			nowState = RushState.��~;
			_moveTime = 0;
			anchorPosition = transform.position.x;
		}

		/// <summary>
		/// ���C�L���X�g���΂��ďՓ˂��Ă��邩�ǂ����m�F����B
		/// �i�s�����ƌ��݂̃X�s�[�h�ō��E�̂ǂ���ɏo�������m�F
		/// �Փ˂������~����������I��
		/// �܂��̓��C�L���X�g�ŏՓˊm�F�����Ƃ���
		/// </summary>
		public void CollideObjectCheck()
		{

			//�ʉߓːi�̎��͏������s��Ȃ�
			if (nowState == RushState.�ړ� && _contactType == AttackContactType.�ʉ�)
			{
                if (pushObject != null)
                {
					pushObject.anotherLock = false;
					pushObject = null;
				}
				return;
			}
				// we set our flag to false

				// ���x�����鎞�A�v�b�V���\�ȃI�u�W�F�N�g�ƏՓ˂��Ă��邩�ǂ������m�F���邽�߂ɑO���Ɍ����𓊎˂��܂��B

				//�~�܂��Ă鎞�͂ނ��Ă���A�����Ă鎞�͓����Ă���Ƀ��C��
				if (_controller.Speed.x == 0)
			{
				_raycastDirection = _character.IsFacingRight ? transform.right : -transform.right;
			}
			else
			{
				_raycastDirection = _controller.Speed.x > 0 ? transform.right : -transform.right;
			}

			//�����̃R���C�_�[�̐�[����������΂��Ă�
			_raycastOrigin = _controller.ColliderCenterPosition + _raycastDirection * (_controller.Width() / 2);


			// we cast our ray to see if we're hitting something
			//�����������Ă邩�m�F���邽�ߌ����𓖂Ă�
			//����HitObject�}�X�N��n�`�ɓ�����Ȃ��悤�ɂ��Ȃ��ƒn�`�����o���ă��b�N���ꑱ����
			RaycastHit2D hit = MMDebug.RayCast(_raycastOrigin, _raycastDirection, DetectionRaycastLength, HitObjectMask, Color.green, _controller.Parameters.DrawRaycastsGizmos);

			//��������Ƀq�b�g������
			if (hit)
			{


				//�Փ˂Ȃ�����Ȃ�����Փ˃I�u�W�F�N�g���ς������
				if (!contactNow || hit.collider.gameObject != pushObject.gameObject)
				{

					//���ڐG���ɏ�����������
					if (hit.collider.gameObject.MMGetComponentNoAlloc<CorgiController>() != null)
					{
						//���b�N�������Ă����Ă������ւ���
						if (pushObject != null)
						{
							pushObject.anotherLock = false;
						}
						pushObject = hit.collider.gameObject.MMGetComponentNoAlloc<MyAttackMove>();
						keepDistance = pushObject.ReturnSizeX() + DetectionRaycastLength;
						contactNow = true;
						anchorPosition = transform.position.x;
					}
					
				}
				//�q�b�g�������ɃR���g���[���[�����Ă�����
				if (contactNow)
				{
					//�U���̈ړ��ł��蔲���Ȃ��悤�ɂ��鏈��
					//rushNow������
					if (nowState == RushState.�ړ�)
					{

						//�U���̐ڐG�^�C�v����~�Ȃ烉�b�V����߂�
						if (_contactType == AttackContactType.��~)
						{
							StopRush();
						}
						//�����Ȃ牟���n�߂�
						else if (_contactType == AttackContactType.����)
						{
							//�擾�����I�u�W�F�N�g��������Ȃ�
							if (!pushObject.Pushable)
							{
								pushNow = true;
							}
							//�����Ȃ��Ȃ��~
                            else
                            {
								StopRush();
							}
						}
					}

					//�K�[�h��K�[�h�ړ��A�ːi�����ɍU�����Ă鎞�̏���
					else
					{
						//�Փ˂������肪�Փ˔�����o���悤�ȍs�������Ă��Ȃ��Ƃ�
						//�����������Ƀ��b�N����
						//�����Փ˔��肪�o��s�������Ă���Ȃ玩���͎����Œ�~����B
                        if (pushObject.ReturnNowCollide())
						{
						//	Debug.Log($"����������");
							//���肪�������E�ɂ���Ȃ�
							bool isRight = transform.position.x < pushObject.transform.position.x;

							pushObject.AnotherLock(isRight);
                        }


						if(_controller.Speed.x != 0)
                        {
							//�E�����Ă鎞�E�A�������Ă鎞�ɍ��ɐi��ł���ʒu�𓮂��Ȃ��悤��
                            if ((_raycastDirection.x > 0 && _controller.Speed.x > 0) ||
								(_raycastDirection.x < 0 && _controller.Speed.x < 0))
                            {
								posi.Set(anchorPosition, transform.position.y);
								transform.position = posi;
								//_controller.SetHorizontalForce(0);
							}
							
                        }
						//�������~�܂��Ă�Ȃ瑊����~�܂��Ă�̂�else�͂���Ȃ�


					}
					

				}
			}
			//����ɃR���g���[���[�Ȃ��ēːi������Ȃ��Ȃ�P���ɓːi�Ƃ߂邾��
			//Platform����Ȃ炻�����������Ȃ�
			else if(contactNow)
			{
				//�Փ˃I�u�W�F�N�g��Null����Ȃ��Ȃ�

				//���b�N�������ďՓ˃I�u�W�F�N�g���̂Ă�
				pushObject.anotherLock = false;
				pushObject = null;
				contactNow = false;
				anchorPosition = transform.position.x;

				//�����̂���߂�
				pushNow = false;
			}
		}


		/// <summary>
		///  �ːi�J�n
		/// �K�v�ȏ���������
		/// �O������Ăяo��
		/// �ړ����鋗���͌v�Z�Ɏg��
		/// ���b�����Ăǂꂭ�炢�̋����ړ����邩�A�ł��������̑��x���o����
		/// </summary>
		/// <param name="duration">�ړ����鎞�Ԃ̒���</param>
		/// <param name="distance">�ړ����鋗���B���b�N�I�������G�Ƃ̋��������Ă�����</param>
		/// <param name="_type">�G�ƐڐG�������̑Ή�</param>
		/// <param name="infinityMove">�����U���Ŗ����ɉ��ړ��������邩�ǂ���</param>
		/// <param name="startTime">�ړ����J�n����܂ł̎���</param>
		public void RushStart(float duration, float distance, AttackContactType _type,bool fallAttack = false, float startTime = 0)
        {

			if (!fallAttack)
			{
				nowState = RushState.�ҋ@;
				_moveDuration = duration;
				_moveStartTime = startTime;
				_contactType = _type;
				_moveSpeed = distance / _moveDuration;
				fallMove = false;
			}
            else
            {
				nowState = RushState.�ҋ@;
				_moveDuration = duration;
				_moveStartTime = startTime;
				_contactType = AttackContactType.�ʉ�;
				_moveSpeed = distance / _moveDuration;
				fallMove = true;
			}
		//	Debug.Log($"�������{_moveSpeed}");
		}


        #region//���̃I�u�W�F�N�g�ɏ���^���郁�\�b�h

        /// <summary>
        /// �ːi���Ɠːi�����Ԃ����ĉ����������̏����͍l���Ȃ��A���܂�
        /// </summary>
        /// <returns></returns>
        public int ReturnRushState()
        {
			return (int)nowState;
        }

		/// <summary>
		/// ���f�ɕK�v�ȏ����܂Ƃ߂ċᖡ���Č��ʂ��������Ă����
		/// �O������ǂݎ��
		/// </summary>
		public float ReturnSizeX()
		{
			return _controller.Width();
		}

		/// <summary>
		/// �^�Ȃ�Փ˂���悤�ȍs���͂��Ă��Ȃ�
		/// </summary>
		/// <returns></returns>
		public bool ReturnNowCollide()
        {
			return (_movement.CurrentState != CharacterStates.MovementStates.Attack && _movement.CurrentState != CharacterStates.MovementStates.Guard
				&& _movement.CurrentState != CharacterStates.MovementStates.GuardMove);
        }

		/// <summary>
		/// �O������̃��b�N
		/// </summary>
		public void AnotherLock(bool isRight)
        {
			anotherLock = true;
			isRightLock = isRight;
			anchorPosition = transform.position.x;
		}
		public CharacterStates.MovementStates Returntest()
		{
			return _movement.CurrentState;
		}


		/// <summary>
		/// ��������΂���Ă��邩
		/// </summary>
		/// <returns></returns>
		public bool BlowNow()
        {
			return _health._blowNow;
        }
        #endregion
    }

}
