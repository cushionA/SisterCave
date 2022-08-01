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
    /// ���ʂɏ󋵔��f���ē������߂̃X�N���v�g
	/// ���[�v�A�U���A�R���r�l�[�V�����i�e����͕�����H�j�͕ʃX�N���v�g��
	/// ���邢�̓R���r�l�[�V�����͎�l���Ɏ�������H
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/BrainAbility")]
    public class BrainAbility : MyAbillityBase
    {
        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "TODO_HELPBOX_TEXT."; }

		//   [Header("����f�[�^")]
		/// declare your parameters here
		///WeaponHandle�Q�l�ɂ��� 


		//public GameObject player;
		[Header("�x���X�e�[�g�̎���")]
		public float patrolTime;
		[Header("���o�T�m")]
		///<summary>
		///�G���m�p
		///</summary>
		public GameObject Serch;

		[Header("�S���ʂ̋C�z�T�m")]
		///<summary>
		///����Ŋ����Ƃ��ɔ���
		///</summary>
		public GameObject Serch2;



		[Header("�ˏo�n�_")]
		///<summary>
		///�e�ۏo���ꏊ
		///</summary>
		public Transform firePosition;

		[SerializeField]
		private LayerMask layerMask;

		bool disEnable;
		bool isReset;


		public enum SisterState
		{
			�̂�т�,
			�x��,
			�킢
		}

		[HideInInspector] public SisterState nowState;


		[Header("�V�X�^�[����̃X�e�[�^�X")]
		public SisterStatus status;



		// === �O���p�����[�^ ======================================

		Vector2 distance;//�v���C���[�Ƃ̋���
		float stopTime;//��~���Ԃ𐔂���
		bool isStop;//��~�t���O
		float waitTime;//�p�g���[�����̑҂����Ԑ�����
					   //	 bool posiReset;//�퓬�̎�����Đݒ肷��t���O
		bool nowJump;


		int initialLayer;
		bool jumpTrigger;

		bool isWait;//���̃t���O���^�Ȃ�ҋ@���[�V��������B�}���g����������V�񂾂�B
					//���[�V�����͍D���x�Ƃ��i�s�x�Ń��[�V���������X�g���؂�ւ���Č��܂�B

		
	
		[HideInInspector] public bool nowPosition;//������t���O
		Vector2 myPosition;


		// === �L���b�V�� ======================================
		public Animator sAni;




		// === �����p�����[�^ ======================================

		int direction;
		int directionY;
		float moveDirectionX;
		float moveDirectionY;
		float jumpTime;//�W�����v�̃N�[���^�C���B�������̂̓C���p���X��


		Vector2 basePosition;
		/// <summary>
		/// �v���C���[�������Ă�����B�
		/// </summary>
		Vector3 baseDirection;

		[HideInInspector] public bool guardHit;

		//	public Rigidbody2D GManager.instance.GManager.instance.pm.rb;//�v���C���[�̃��W�b�h�{�f�B

		bool isSquat;
		string squatTag = "SquatWall";
		float PositionJudge;
		bool isClose;//��x�߂��܂ōs��
		[HideInInspector] public bool isPlay;
		[HideInInspector] public float playPosition;//�����̏ꏊ
		[HideInInspector] public float playDirection;//�V�Ԏ��̕���
		string jumpTag = "JumpTrigger";
		float patrolJudge;//�x�����Ԃ𐔂���
		bool isVertical;//������т���t���O
		Vector2 waitPosition;
		[HideInInspector] public int stateNumber;
		[HideInInspector] public int beforeNumber;
		/// <summary>
		/// �ړ���Ԃ��Ĕ��肷�邽�߂̎��ԑ���
		/// </summary>
		[HideInInspector] public float reJudgeTime;
		/// <summary>
		/// ���s����_�b�V���ȂǂɕύX�\���B
		/// ���ꂪ�Ȃ��ƕ��s�Ƒ��s�̂͂��܂ŃK�^�K�^
		/// </summary>
		[HideInInspector] public bool changeable;//

		float battleEndTime;
		Vector2 move;
		private float flipWaitTime;
        private int lastDirection;


		// === �A�r���e�B ================
		#region
		/// the number of jumps to perform while in this state
		//	[Tooltip("the number of jumps to perform while in this state")]
		//	public int NumberOfJumps = 1;

		///<summary>
		///�ǂ���̕������������̂���B
		/// </summary>
		int horizontalDirection;


		//���ړ��͌p�����Ƃɂ�����

		protected PlayerJump _jump;
		//�@protected int _numberOfJumps = 0;


		//�@protected int _numberOfJumps = 0;

		protected PlayerRoll _rolling;

		protected CharacterRun _characterRun;

		protected GuardAbillity _guard;

		protected MyWakeUp _wakeup;

		protected WeaponAbillity _attack;

		protected MyDamageOntouch _damage;

		protected WarpAbillity _warp;

		protected FireAbility _fire;

		protected new MyHealth _health;

		//�V�X�^�[����̌ŗL�A�N�V����

		protected SensorAbility _sensor;
		//	protected Hittable _hitInfo;

		protected RewiredCorgiEngineInputManager ReInput;


		// �A�j���[�V�����p�����[�^�[
		protected const string _stateParameterName = "_nowState";
		protected int _stateAnimationParameter;

		#endregion

		// === �X�e�[�^�X�p�����[�^ ======================================

		/// <summary>
		/// �U���{��
		/// </summary>
		[HideInInspector]
		public float attackFactor = 1;
		[HideInInspector]
		public float fireATFactor = 1;
		[HideInInspector]
		public float thunderATFactor = 1;
		[HideInInspector]
		public float darkATFactor = 1;
		[HideInInspector]
		public float holyATFactor = 1;

		[HideInInspector]
		//�X�e�[�^�X
		//hp�̓w���X�ł���
		public float maxHp;
		[HideInInspector]
		public float maxMp;


		[HideInInspector]
		public float mp;

		// === �R�[�h�iAI�̓��e�j ================





		/// <summary>
		///�@�����ŁA�p�����[�^������������K�v������܂��B
		/// </summary>
		protected override void Initialization()
        {
            base.Initialization();
            // randomBool = false;
            ReInput = (RewiredCorgiEngineInputManager)_inputManager;
			//�Z���T�[�A�r���e�B�̐؂�ւ�����ɕύX
			_sensor.RangeChange();
			_characterHorizontalMovement.FlipCharacterToFaceDirection = false;
			GravitySet(status.firstGravity);
		}



        /// <summary>
        /// 1�t���[�����ƂɁA���Ⴊ��ł��邩�ǂ����A�܂����Ⴊ��ł���ׂ������`�F�b�N���܂�
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
			Brain();
        }
		public void Brain()
        {


			if (_movement.CurrentState == CharacterStates.MovementStates.Warp || isStop || _movement.CurrentState != CharacterStates.MovementStates.Rolling || _condition.CurrentState == CharacterStates.CharacterConditions.Stunned || _movement.CurrentState == CharacterStates.MovementStates.Attack
				||	_movement.CurrentState == CharacterStates.MovementStates.Falling)
			{
				disEnable = true;
			}
			else
			{
				disEnable = false;
			}

			basePosition = GManager.instance.Player.transform.position;
			baseDirection = GManager.instance.Player.transform.localScale;
			//���v���C���[�������Ă�����ƃv���C���[�̈ʒu

			myPosition = this.transform.position;
			distance = basePosition - myPosition;
			direction = distance.x >= 0 ? 1 : -1;//���������A�܂���0�̎�1�B�����łȂ��Ƃ�-1�B����
			directionY = distance.y >= 0 ? 1 : -1;//�ق���AI�ŋ|�\����Ƃ��̃A�j���̔���	�ɂ��g������

			//	Debug.Log($"���݂̃X�e�[�g{nowState}");
			SisterFall();
			EscapeWarp();
			if (nowState == SisterState.�킢)
			{
				//Debug.Log($"�ʒu�ɂ��Ă܂����[{nowPosition}");
				SManager.instance.BattleEndCheck();
				BattleEnd();


				PositionSetting();

			}

			else if (nowState == SisterState.�x��)
			{
				patrolJudge += _controller.DeltaTime;


				////Serch.SetActive(true);�ق��̃X�e�[�g�Ɉړ�����Ƃ���
				////Serch2.SetActive(true);

				if (!SManager.instance.actNow)
				{
					PatrolMove();


					//	PositionSetting();
					
				}

				//	PositionChange();
				if (patrolJudge >= patrolTime)
				{
					beforeNumber = 0;
					reJudgeTime = 0;
					stateNumber = 2;
					changeable = true;
					nowState = SisterState.�̂�т�;
					//_sensor.RangeChange();
					////Serch2.SetActive(true);
					////Serch.SetActive(true);
				}


			}
			else if (nowState == SisterState.�̂�т�)
			{
				if (!SManager.instance.actNow)
				{
					PlayMove();
				}
				WaitJudge();
				//�ҋ@��Ԃɂ͐F�X����

				//PositionChange();

				//if()

				//	SisterFall();
				//SetVelocity();

			}

			if(Mathf.Sign(transform.localScale.x) ==�@Mathf.Sign(_characterHorizontalMovement.MovementSpeed) && !disEnable)
            {
				//�ړ������ƃL�����̕�������Ȃ��Ȃ�Î~
				_characterHorizontalMovement.SetHorizontalMove(0);
				_characterRun.RunStop();
            }

			JumpController();
		}

        /// <summary>
        /// �A�r���e�B�T�C�N���̊J�n���ɌĂяo����A�����œ��̗͂L�����m�F���܂��B
        /// </summary>
        protected override void HandleInput()
        {
            //�����ŉ��{�^����������Ă��邩�ɂ���Ĉ����n����

            // here as an example we check if we're pressing down
            // on our main stick/direction pad/keyboard
            if (ReInput.CombinationButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {
				//���[�v�Ƃ��̘A�g��u��
            }
			//�������̏����������H
        }

		/// <summary>
		/// �x���ʒu�ɂ����郁�\�b�h
		/// </summary>
		public void PositionSetting()
		{
			
			if (!SManager.instance.actNow && _controller.State.IsGrounded && !disEnable)
			{
				//�|�W�V�����ɂ��ɍs���B�����ĂȂ��Ēn�ʂɂ���
				if (!nowPosition)
				{
					SManager.instance.GetClosestEnemyX();
					int mDirection = (int)Mathf.Sign(SManager.instance.closestEnemy - myPosition.x);
					//��ԋ߂��G���E�ɂ���Ƃ�
					reJudgeTime += _controller.DeltaTime;
					if (mDirection >= 0)
					{


						if (reJudgeTime >= 1.5)
						{
							Flip(mDirection);
							if (SManager.instance.closestEnemy - myPosition.x >= status.battleDis)
							{
								stateNumber = 0;
								_characterRun.RunStop();
							}
							else// if (SManager.instance.closestEnemy - myPosition.x < status.battleDis)
							{
								stateNumber = 1;
								_characterRun.RunStart();

							}
							reJudgeTime = 0;
						}
						if (stateNumber == 0)
						{
							//	Debug.Log("��~");
							_characterHorizontalMovement.SetHorizontalMove(0f);
							Flip(mDirection);
							//����Ԃ���
							//�|�W�V�����ɓ���
							nowPosition = true;
							_characterRun.RunStop();
							//("Stand");
							reJudgeTime = 100;
						}
						else if (stateNumber == 1)
						{
							//	Debug.Log($"������{mDirection}");
							BattleFlip(-mDirection);
							_characterHorizontalMovement.SetHorizontalMove(lastDirection);
							//("Dash");
						}

						else
						{
							reJudgeTime = 100;
						}
					}
					else
					{


						if (reJudgeTime >= 1.5)
						{
							if (Mathf.Abs(SManager.instance.closestEnemy - myPosition.x) >= status.battleDis)
							{
								stateNumber = 2;
								_characterRun.RunStop();
							}
							else// if (Mathf.Abs(SManager.instance.closestEnemy - myPosition.x) < status.battleDis)
							{
								stateNumber = 3;
								_characterRun.RunStart();
							}
							reJudgeTime = 0;
						}
						if (stateNumber == 2)
						{
											_characterHorizontalMovement.SetHorizontalMove(0f);
							Flip(mDirection);
							//����Ԃ���
							//�|�W�V�����ɓ���
							nowPosition = true;
							//("Stand");
							reJudgeTime = 100;
						}
						else if (stateNumber == 3)
						{
							BattleFlip(-mDirection);

							_characterHorizontalMovement.SetHorizontalMove(lastDirection);
							
							//("Dash");
						}
						else
						{
							reJudgeTime = 100;
						}
					}
				}
				else if (nowPosition)
				{
					//�Ĕ��肷�邩�ǂ���
					//�͈͓��ɂĕ�������Ĕ���
					if (!SManager.instance.castNow)
					{
						int atDirection;

						reJudgeTime += _controller.DeltaTime;
						if (SManager.instance.target != null)
						{
							atDirection = (int)Mathf.Sign(SManager.instance.target.transform.position.x - myPosition.x);
							_characterHorizontalMovement.SetHorizontalMove(0f);
							
							BattleFlip(atDirection);
						}

						//����Ԃ���
					}
					if (reJudgeTime >= SManager.instance.sisStatus.escapeTime)
					{
						reJudgeTime = 0.0f;
						reJudgeTime = 150;
						SManager.instance.GetClosestEnemyX();
						//�v���C���[�����[�v�����藣��Ă�����

						//�G��������]�[�����߂���������

						nowPosition = false;
						// = 99;
					}

				}
				//	}
			}
		}





		public void Flip(float direction)
		{
			if (!isStop && disEnable)
			{

				Vector3 theScale = transform.localScale;
				theScale.x *= direction;
				transform.localScale = theScale;

			}

		}

		/// <summary>
		/// �ҋ@���̏����s��
		/// </summary>
		public void PatrolMove()
		{
			if (_controller.State.IsGrounded && !disEnable)
			{
				if (reJudgeTime >= 1 && changeable)
				{
					if (Mathf.Abs(distance.x) > status.walkDistance)
					{
						stateNumber = 1;
						_characterRun.RunStart();
					}

					else if ((Mathf.Abs(distance.x) <= status.walkDistance && Mathf.Abs(distance.x) > status.patrolDistance) || (Mathf.Sign(GManager.instance.pm.rb.velocity.x) == Mathf.Sign(transform.localScale.x) && GManager.instance.pm.rb.velocity.x != 0))
					{
						stateNumber = 2;
						_characterRun.RunStop();
					}
					else if ((Mathf.Sign(GManager.instance.pm.rb.velocity.x) != Mathf.Sign(transform.localScale.x) || GManager.instance.pm.rb.velocity.x == 0) && Mathf.Abs(distance.x) <= status.patrolDistance)
					{

						stateNumber = 3;
						_characterRun.RunStop();
					}
					if (beforeNumber == 0)
					{
						beforeNumber = stateNumber;
						changeable = true;
					}
					else if (beforeNumber == stateNumber)
					{
						changeable = true;
					}
					else if (beforeNumber != stateNumber)
					{
						changeable = false;
						beforeNumber = 0;
						// = 99;
					}

				}
				else if (reJudgeTime >= 1 && !changeable)
				{
					reJudgeTime = 0;
				}
				else if (reJudgeTime < 1)
				{
					reJudgeTime += _controller.DeltaTime;
					changeable = true;
				}
				if (stateNumber == 1)
				{

					Flip(direction);
					_characterHorizontalMovement.SetHorizontalMove(lastDirection);
					
					isWait = false;
					//("Move");
				}
				else if (stateNumber == 2)
				{
					//Debug.Log("�ł�");
					Flip(direction);
					_characterHorizontalMovement.SetHorizontalMove(lastDirection);
					
					isWait = false;
					//("Walk");
				}
				else if (stateNumber == 3)
				{
					//	Debug.Log("�ł�");
					_characterHorizontalMovement.SetHorizontalMove(0f);
					isWait = true;
					//("Stand");
				}


			}
		}
		public void PlayMove()
		{//�̂�т�
			if (_controller.State.IsGrounded && !disEnable)
			{
				if (reJudgeTime >= 1.5 && changeable)
				{

					if (Mathf.Abs(distance.x) > status.playDistance || isClose)
					{
						stateNumber = 1;
						isClose = true;//�ڋ߂��悤���Ă����t���O
						if (Mathf.Abs(distance.x) <= status.patrolDistance)
						{
							//����������ڋߎw���t���O����
							isClose = false;
						}
						_characterRun.RunStart();
					}
					else if (Mathf.Abs(distance.x) <= status.playDistance && Mathf.Abs(distance.x) > status.patrolDistance)
					{
						if (isPlay)
						{
							//�����ɐڐG���Ă鎞

							if (myPosition.x >= playPosition - 2 && myPosition.x <= playPosition + 2)
							{
								stateNumber = 2;
							}
							else if (myPosition.x < playPosition)
							{
								stateNumber = 3;
								_characterRun.RunStart();
							}
							else if (myPosition.x > playPosition)
							{
								stateNumber = 4;
								_characterRun.RunStart();
							}
						}
						else if (Mathf.Abs(distance.x) > status.patrolDistance + status.walkDistance)
						{
							stateNumber = 5;
						}
						else if (Mathf.Abs(distance.x) <= status.patrolDistance + status.walkDistance && Mathf.Abs(distance.x) > status.patrolDistance)
						{
							stateNumber = 6;
							_characterRun.RunStop();
						}

						//Flip(direction);

					}
					else if (Mathf.Abs(distance.x) <= status.patrolDistance)
					{
						//�������Ă�Ƃ�
						//�߂��ɂ���
						if (isPlay)
						{
							//�����ɐڐG
							waitTime = 0.0f;
							if (myPosition.x >= playPosition - 2 && myPosition.x <= playPosition + 2)
							{
								stateNumber = 7;
							}
							else if (myPosition.x < playPosition)
							{
								stateNumber = 8;
								_characterRun.RunStart();
							}
							else if (myPosition.x > playPosition)
							{
								stateNumber = 9;
								_characterRun.RunStart();
							}
						}
						else if (GManager.instance.pm.rb.velocity.x == 0 && !isPlay)
						{
							stateNumber = 10;
							_characterRun.RunStop();
						}
						else
						{
							stateNumber = 11;
							_characterRun.RunStop();
						}

						if (beforeNumber == 0)
						{
							beforeNumber = stateNumber;
							changeable = true;
						}
						else if (beforeNumber == stateNumber)
						{
							changeable = true;
						}
						else if (beforeNumber != stateNumber)
						{
							changeable = false;
							beforeNumber = 0;
							// = 99;
						}

					}
				}
				else if (reJudgeTime >= 1.5 && !changeable)
				{
					reJudgeTime = 0;
				}
				else if (reJudgeTime < 1.5f)
				{
					reJudgeTime += _controller.DeltaTime;
					changeable = true;
				}

				if (stateNumber == 1)
				{
					//�V�тł�����Ă����͈͂��痣��Ă�Ȃ炭�����܂ő���B
					//isClose�͐ڋ߂��Ȃ���΂Ȃ�Ȃ��Ƃ����t���O
					//�������񂷂��߂��܂ŗ��悤
					Flip(direction);
					_characterHorizontalMovement.SetHorizontalMove(lastDirection);
					
					
					//	isWait = false;//�����~�܂��Ă�B

				}

				//�����ɐڐG���Ă鎞

				else if (stateNumber == 2)
				{

					//�����̂������΂ɂ���Ƃ�
					_characterHorizontalMovement.SetHorizontalMove(0); 
					//("Stand");
				}
				else if (stateNumber == 3)
				{
					Flip(1);
					//���������ɂ���Ƃ�
					_characterHorizontalMovement.SetHorizontalMove(lastDirection);
					
					
					//("Move");
				}
				else if (stateNumber == 4)
				{
					Flip(-1);
					//�������O�ɂ���Ƃ�
					_characterHorizontalMovement.SetHorizontalMove(lastDirection);

					
					//("Move");
				}

				else if (stateNumber == 5)
				{
					//�V��ł����͈͂ɂ��āA�������ĂȂ��Ċ������Ȃ��Ƃ��B������ōs��
					//�x�������܂ŗ���
					//isWait = true;
					Flip(direction);
					_characterHorizontalMovement.SetHorizontalMove(lastDirection);
					
					//isWait = false;//�����~�܂��Ă�B
					//("Move");
				}
				else if (stateNumber == 6)
				{

					//("Walk");
					//waitTime = 0.0f;
					Flip(direction);
					_characterHorizontalMovement.SetHorizontalMove(lastDirection);
					
				}

				//Flip(direction);
				//rb.AddForce(move(status.addSpeed * (status.dashSpeed * direction - rb.velocity.x), 0));



				//�����ɐڐG
				//	waitTime = 0.0f;
				else if (stateNumber == 7)
				{
					//�����̂������΂ɂ���Ƃ�
					_characterHorizontalMovement.SetHorizontalMove(0);
					
					//("Stand");
				}
				else if (stateNumber == 8)
				{
					Flip(1);
					//���������ɂ���Ƃ�
					_characterHorizontalMovement.SetHorizontalMove(lastDirection);
					
					//("Move");
				}
				else if (stateNumber == 9)
				{
					Flip(-1);
					//�������O�ɂ���Ƃ�
					_characterHorizontalMovement.SetHorizontalMove(lastDirection);
					
					//("Move");
				}

				else if (stateNumber == 10)
				{
					waitTime += _controller.DeltaTime;
					Flip(direction);
					//�v���C���[�������~�܂��ĂĊ������Ȃ��Ƃ�
					_characterHorizontalMovement.SetHorizontalMove(0); 
					//("Stand");
					if (waitTime >= SManager.instance.sisStatus.waitRes && !isWait)
					{
						waitPosition = basePosition;
						isWait = true;
						waitTime = 0.0f;
					}
				}
				else
				{
					//("Walk");
					waitTime = 0.0f;
					Flip(direction);
					_characterHorizontalMovement.SetHorizontalMove(lastDirection);
					
				}



				#region

				#endregion
			}
		}


		/// <summary>
		/// �ҋ@����~���Ă���낫���U����������B�����̓p�g���[�����Ȃ�����waitTime�͎g���܂킵�ł���
		/// </summary>
		public void Wait()
		{
			//("Stand");
			waitTime += _controller.DeltaTime;
			_characterHorizontalMovement.SetHorizontalMove(0);
			if (waitTime >= SManager.instance.sisStatus.waitRes)
			{
				Flip(-transform.localScale.x);//���]�����܂�
				waitTime = 0.0f;
			}
		}





		public void JumpController()
		{

			//isVertical�͎����ŃI���I�t����
			if (!_controller.State.IsGrounded && _movement.CurrentState == CharacterStates.MovementStates.Jumping)
			{
				if (!isVertical)
				{
					_horizontalInput = Mathf.Sign(transform.localScale.x);
				}
				if (!disEnable && _jump.JumpEnableJudge() == true)
				{
					_jump.JumpStart();
				}
			}
			else
			{
				isVertical = false;
			}

		}

		public void JumpStart()
		{
			if (!disEnable && _controller.State.IsGrounded)
			{
				_jump.JumpStart();
			}
		}

		/// <summary>
		/// ���C���[�ύX�BAvoid�Ȃ񂩂Ǝg���Ă��������P�̂ł��g����
		/// </summary>
		/// <param name="layerNumber"></param>
		public void SetLayer(int layerNumber)
		{

			this.gameObject.layer = layerNumber;

		}

		/// <summary>
		/// �d�͐ݒ�
		/// </summary>
		/// <param name="gravity"></param>
		public void GravitySet(float gravity)
		{
			//rb.gravityScale = gravity;
			_controller.DefaultParameters.Gravity = gravity;
		}

		/// <summary>
		/// X��Y�̊Ԃŗ������o��
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <returns></returns>
		public int RandomValue(int X, int Y)
		{
			return Random.Range(X, Y + 1);

		}

		/// <summary>
		/// stopRes�𐔂������isStop�����̃��\�b�h���ĂԁBisStop���͂��ׂĎ~�܂�B�d����_�E���ɂǂ���
		/// </summary>
		/// <param name="stopRes"></param>
		public void AllStop(float stopRes)
		{
			//stopTime += _controller.DeltaTime;
			SetLayer(initialLayer);
			isStop = true;
			nowJump = false;
			_characterHorizontalMovement.SetHorizontalMove(0f);
			Invoke("StopBreak", stopRes);
		}

		/// <summary>
		/// ��~�����B�P�̂ł͎g��Ȃ�
		/// </summary>
		public void StopBreak()
		{
			SetLayer(17);
			isStop = false;
			_characterHorizontalMovement.SetHorizontalMove(0f);
		}






		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (_controller.State.IsGrounded)
			{

				//�g���l���̒������Ⴊ�݃g���K�[�Ŗ�����
				if (collision.tag == squatTag && GManager.instance.pm.isSquat)
				{
					isSquat = true;
				}
				//else�ł��ꂵ�Ⴊ�݉������邼
				if (collision.tag == jumpTag && _controller.State.IsGrounded)
				{
					//GetComponent�͂Ȃ�ׂ����ʂ悤��
					if (collision.gameObject.GetComponent<JumpTrigger>().jumpDirection == transform.localScale.x)
					{
						jumpTrigger = true;

					}
				}

			}
		}

		private void OnTriggerStay2D(Collider2D collision)
		{
			if (_controller.State.IsGrounded)
			{
				//�g���l���̒������Ⴊ�݃g���K�[�Ŗ�����
				if (collision.tag == squatTag && GManager.instance.pm.isSquat)
				{
					isSquat = true;
				}
				if (collision.tag == jumpTag && _controller.State.IsGrounded)
				{
					if (collision.gameObject.GetComponent<JumpTrigger>().jumpDirection == transform.localScale.x)
					{
						jumpTrigger = true;

					}
				}

			}
		}
		private void OnTriggerExit2D(Collider2D collision)
		{
			if (_controller.State.IsGrounded)
			{
				if (collision.tag == squatTag && !GManager.instance.pm.isSquat)
				{
					isSquat = false;
				}
				if (collision.tag == jumpTag)
				{
					jumpTrigger = false;

				}
			}
		}

		/// <summary>
		/// �ҋ@��ԉ������f�̂��߂̃��\�b�h
		/// </summary>
		public void WaitJudge()
		{
			if (isWait)
			{
				if (Mathf.Abs(basePosition.x) - Mathf.Abs(waitPosition.x) > Mathf.Abs(status.patrolDistance) / 2 || basePosition.y != waitPosition.y)
				{
					isWait = false;
				}
			}
		}

		public void SisterFall()
		{

			if (!_controller.State.IsGrounded)
			{
				GravitySet(SManager.instance.sisStatus.firstGravity);

			}
			else
			{
				//GravitySet(0);
			}
		}








		/// <summary>
		/// �퓬���ɓ����郏�[�v
		/// </summary>
		private void EscapeWarp()
		{
			if (_movement.CurrentState != CharacterStates.MovementStates.Jumping)
			{
				if (!disEnable)
				{
					//�]����
					if ((Mathf.Abs(distance.x) >= SManager.instance.sisStatus.warpDistance.x || Mathf.Abs(distance.y) >= SManager.instance.sisStatus.warpDistance.y))// && GManager.instance.pm._controller.State.IsGrounded)
					{

						//������x��2���炵�����l�Ƀ��[�v
						if (baseDirection.x >= 0)
						{
							move.Set(basePosition.x - 6, basePosition.y);
							move.y = RayGroundCheck(move) + 10;
							transform.position = move;
							Flip(1);
						}
						else
						{
							move.Set(basePosition.x + 6, basePosition.y);
							move.y = RayGroundCheck(move) + 10;
							transform.position = move;
							Flip(-1);
						}

						nowPosition = false;
						SManager.instance.isEscape = true;
						//escape��true�̎��x������퓬�ɂȂ�Ȃ�
						stateNumber = 0;
						beforeNumber = 0;
						reJudgeTime = 0;
						changeable = true;

						_warp.WarpStart(move);



						if (nowState == SisterState.�킢)
						{
							reJudgeTime = 0;
							nowState = SisterState.�x��;
							_sensor.RangeChange();
							reJudgeTime = 0;
							_sensor.ReSerch();
						}
					}
				}
			}
		}

		/// <summary>
		/// �퓬�I��
		/// �G����b�Ԃ��Ȃ���Όx���t�F�C�Y�Ɉڂ�
		/// ���̎��_�ōU���͒��~���悤�BFire�ҏW���ɍU�����~���\�b�h�����
		/// </summary>
		private void BattleEnd()
		{

			//�G�����Ȃ�������
			if (SManager.instance.isBattleEnd && _movement.CurrentState != CharacterStates.MovementStates.Warp)
			{

				if (SManager.instance.targetList.Count == 0)
				{
					battleEndTime += _controller.DeltaTime;

					//���Ԍv�����Čx���Ɉڍs
					if (battleEndTime >= 2 && !isReset)
					{

						isReset = true;
						//�Z�b�ȏ�G���m���񂸂�Όx���t�F�C�Y��
						nowPosition = false;
						SManager.instance.isEscape = true;
						nowState = SisterState.�x��;
						//escape��true�̎��x������퓬�ɂȂ�Ȃ�
						stateNumber = 3;
						beforeNumber = 0;
						reJudgeTime = 0;
						reJudgeTime = 0;
						changeable = true;
						_sensor.RangeChange();
						//Serch2.SetActive(true);
						//Serch.SetActive(true);
						battleEndTime = 0;

						SManager.instance.targetList = null;
						SManager.instance.targetCondition = null;
						SManager.instance.target = null;
						SManager.instance.isBattleEnd = false;
					}

				}
				else
				{
					isReset = false;
					SManager.instance.isBattleEnd = false;
					battleEndTime = 0.0f;
				}
			}
		}
		public void WarpEffect()
		{
			Transform gofire = firePosition;

			//Transform rotate = SManager.instance.useMagic.castEffect.LoadAssetAsync<Transform>().Result as Transform;

			Addressables.InstantiateAsync("WarpCircle", gofire.position, gofire.rotation);//.Result;//�����ʒu��Player
			GManager.instance.PlaySound("Warp", transform.position);
		}
		//���[�v��p�̒n�ʒT�m
		public float RayGroundCheck(Vector2 position)
		{

			//�^���Ƀ��C���΂��܂��傤
			RaycastHit2D onHitRay = Physics2D.Raycast(position, Vector2.down, Mathf.Infinity, layerMask.value);

			//  ////Debug.log($"{onHitRay.transform.gameObject}");
			////Debug.DrawRay(i_fromPosition,i_toTargetDir * SerchRadius);

			//Debug.Log($"������������{onHitRay.transform.gameObject.name}");
			return onHitRay.point.y;
		}
		bool CheckEnd(string Name)
		{

			if (!sAni.GetCurrentAnimatorStateInfo(0).IsName(Name))// || sAni.GetCurrentAnimatorStateInfo(0).IsName("OStand"))
			{   // �����ɓ��B�����normalizedTime��"Default"�̌o�ߎ��Ԃ��E���Ă��܂��̂ŁAResult�ɑJ�ڊ�������܂ł�return����B
				return true;
			}
			if (sAni.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
			{   // �ҋ@���Ԃ���肽���Ȃ�΁A�����̒l��傫������B
				return true;
			}
			//AnimatorClipInfo[] clipInfo = sAni.GetCurrentAnimatorClipInfo(0);

			////Debug.Log($"�A�j���I��");

			return false;

			// return !(sAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
			//  (_currentStateName);
		}

		public void StepSound(int type)
		{
			if (type == 0)
			{
				GManager.instance.PlaySound("LightFootStep", transform.position);
			}
			else
			{
				GManager.instance.PlaySound("LightWalkStep", transform.position);
			}
			if (GManager.instance.isWater)
			{
				GManager.instance.PlaySound("WaterStep", transform.position);
			}
		}
		public void AnimeSound(string useSoundName)
		{

			GManager.instance.PlaySound(useSoundName, transform.position);


		}
		public void AnimeChaise(string useSoundName)
		{

			GManager.instance.FollowSound(useSoundName, transform);

		}

		protected void ParameterSet(SisterStatus status)
		{
			///<summary>
			///�@���X�g
			/// </summary>
			#region
			/*
		 CharacterJump _characterJump;
		 PlayerRoll _rolling;
		 CharacterRun _characterRun;
		 EnemyFly _flying;
		 GuardAbillity _guard;
		 MyWakeUp _wakeup;
		 EAttackCon _attack;

			*/
			#endregion

			GravitySet(status.firstGravity);

			_characterHorizontalMovement.WalkSpeed = status.walkSpeed;
			if (_characterRun != null)
			{
				_characterRun.RunSpeed = status.dashSpeed;
			}



			if (_jump != null)
			{
				_jump.CoyoteTime = status.jumpCool;
				_jump.JumpHeight = status.jumpRes;
				_jump.NumberOfJumps = 2;

			}

			maxMp = status.maxMp;
			mp = maxMp;
			maxHp = status.maxHp;
			_health.CurrentHealth = (int)maxHp;
		}

		public void BattleFlip(int direction)
		{
			if (lastDirection != direction)
			{
				flipWaitTime += _controller.DeltaTime;

				if (flipWaitTime >= 0.8f)
				{

					flipWaitTime = 0f;
					Flip(direction);
					lastDirection = direction;
				}
			}
			else
			{
				flipWaitTime = 0;
			}
		}
		/// <summary>
		///  �K�v�ȃA�j���[�^�[�p�����[�^�[������΁A�A�j���[�^�[�p�����[�^�[���X�g�ɒǉ����܂��B
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter(_stateParameterName, AnimatorControllerParameterType.Int, out _stateAnimationParameter);
		}

		/// <summary>
		/// ������I�[�o�[���C�h����ƁA�L�����N�^�[�̃A�j���[�^�[�Ƀp�����[�^�𑗐M���邱�Ƃ��ł��܂��B
		/// ����́ACharacter�N���X�ɂ���āAEarly�Anormal�ALate process()�̌�ɁA1�T�C�N�����Ƃ�1��Ăяo�����B
		/// </summary>
		public override void UpdateAnimator()
		{
			//�̂�т�1�A�x��2�A�킢3
			MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _stateAnimationParameter,(int)nowState , _character._animatorParameters);
		}


	}
}
