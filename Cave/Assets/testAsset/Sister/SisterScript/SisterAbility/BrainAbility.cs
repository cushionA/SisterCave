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
    [AddComponentMenu("Corgi Engine/Character/Abilities/BrainAbility")]
    public class BrainAbility : CharacterAbility
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

		[Header("�퓬��Ԃ̊��m��")]
		///<summary>
		///����Ŋ����Ƃ��ɔ���
		///</summary>
		public GameObject Serch3;

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


		Vector2 targetPosition;//�G�̏ꏊ
		int initialLayer;
		bool jumpTrigger;

		bool isWait;//���̃t���O���^�Ȃ�ҋ@���[�V��������B�}���g����������V�񂾂�B
					//���[�V�����͍D���x�Ƃ��i�s�x�Ń��[�V���������X�g���؂�ւ���Č��܂�B
		bool isWarp;//���[�v��

		[HideInInspector] public bool isPosition;//������t���O
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
		bool isDown;
		float randomTime = 2.5f;//�����_���ړ�����̂���
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
		float jumpWait;//�W�����v�\�ɂȂ�܂ł̑ҋ@����
		float verticalWait;//�����W�����v�\�ɂȂ�܂ł̑ҋ@����
		bool isVertical;//������т���t���O
		Vector2 waitPosition;
		//������Ĕ��f
		[HideInInspector] public float reJudgePositionTime;
		[HideInInspector] public int stateNumber;
		[HideInInspector] public int beforeNumber;
		[HideInInspector] public float reJudgeTime;
		[HideInInspector] public bool changeable;//���s����_�b�V���ȂǕύX�\��

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

		protected new MyHealth _health;

		//�V�X�^�[����̌ŗL�A�N�V����


		//	protected Hittable _hitInfo;


		#endregion

		// === �R�[�h�iAI�̓��e�j ================


		protected RewiredCorgiEngineInputManager ReInput;



        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            // randomBool = false;
            ReInput = (RewiredCorgiEngineInputManager)_inputManager;
			Serch3.SetActive(false);
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


			if (isWarp || isStop || _movement.CurrentState != CharacterStates.MovementStates.Rolling || _condition.CurrentState == CharacterStates.CharacterConditions.Stunned || _movement.CurrentState == CharacterStates.MovementStates.Attack
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

				if (!isPosition)
				{
					isPosition = true;
				}
				PositionSetting();
				if (!nowPosition)
				{
					

				}
			}

			else if (nowState == SisterState.�x��)
			{
				patrolJudge += Time.fixedDeltaTime;


				//Serch.SetActive(true);�ق��̃X�e�[�g�Ɉړ�����Ƃ���
				//Serch2.SetActive(true);

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
					//Serch3.SetActive(false);
					//Serch2.SetActive(true);
					//Serch.SetActive(true);
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
			if (Serch3.activeSelf == false)
			{
				Serch3.SetActive(true);
			}
			#region

			#endregion
			if (!SManager.instance.actNow && isPosition && _controller.State.IsGrounded && !disEnable)
			{
				//�|�W�V�����ɂ��ɍs���B�����ĂȂ��Ēn�ʂɂ���
				if (!nowPosition)
				{
					SManager.instance.GetClosestEnemyX();
					int mDirection = (int)Mathf.Sign(SManager.instance.closestEnemy - myPosition.x);
					//��ԋ߂��G���E�ɂ���Ƃ�
					reJudgeTime += Time.fixedDeltaTime;
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

						reJudgePositionTime += Time.fixedDeltaTime;
						if (SManager.instance.target != null)
						{
							atDirection = (int)Mathf.Sign(SManager.instance.target.transform.position.x - myPosition.x);
							_characterHorizontalMovement.SetHorizontalMove(0f);
							
							BattleFlip(atDirection);
						}

						//����Ԃ���
					}
					if (reJudgePositionTime >= SManager.instance.sisStatus.escapeTime)
					{
						reJudgePositionTime = 0.0f;
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


		/// <summary>
		/// �퓬���̈ʒu���
		/// ����ȂɁH
		/// </summary>
		public void PositionChange()
		{
			if (GManager.instance.pm.isSquat && !isSquat && GManager.instance.pm.rb.velocity == Vector2.zero)
			{
				PositionJudge += Time.fixedDeltaTime;
				if (nowPosition)
				{
					if (PositionJudge >= 2.0f)
					{
						isPosition = false;
						PositionJudge = 0.0f;
					}
				}
				else if (!nowPosition)
				{
					if (PositionJudge >= 5.0f)
					{
						isPosition = true; ;
						PositionJudge = 0.0f;
					}
				}
			}
			else
			{
				PositionJudge = 0.0f;
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
					reJudgeTime += Time.fixedDeltaTime;
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
					reJudgeTime += Time.fixedDeltaTime;
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
					waitTime += Time.fixedDeltaTime;
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
			waitTime += Time.fixedDeltaTime;
			_characterHorizontalMovement.SetHorizontalMove(0);
			if (waitTime >= SManager.instance.sisStatus.waitRes)
			{
				Flip(-transform.localScale.x);//���]�����܂�
				waitTime = 0.0f;
			}
		}





		public void JumpController()
		{
			if (!_controller.State.IsGrounded)
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
			//stopTime += Time.fixedDeltaTime;
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
				if (!isWarp)
				{
					//("Fall");
				}
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
				if (!isWarp)
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
						isWarp = true;
						nowPosition = false;
						isPosition = false;
						SManager.instance.isEscape = true;
						//escape��true�̎��x������퓬�ɂȂ�Ȃ�
						stateNumber = 0;
						beforeNumber = 0;
						reJudgeTime = 0;
						changeable = true;

						//("WarpPray");

						if (status.mp >= 5)
						{
							status.mp -= 5;
						}

						if (nowState == SisterState.�킢)
						{
							reJudgeTime = 0;
							nowState = SisterState.�x��;
							Serch3.SetActive(false);
							Serch2.SetActive(true);
							Serch.SetActive(true);
							reJudgePositionTime = 0;
							for (int i = 0; i < SManager.instance.targetList.Count; i++)
							{
								SManager.instance.targetList[i].GetComponent<EnemyBase>().TargetEffectCon(1);
							}
							SManager.instance.targetList.Clear();

							SManager.instance.targetCondition.Clear();
							SManager.instance.target = null;
						}
						//EscapeWarp();
						//�������[�v���\�b�h�Ō��ɖ߂�
					}
				}
				else
				{
					battleEndTime += Time.fixedDeltaTime;
					//("WarpPray");
					if (!CheckEnd("WarpPray") || battleEndTime >= 6)
					{

						battleEndTime = 0;
						isWarp = false;
					}

				}
			}
		}

		/// <summary>
		/// �퓬�I��
		/// </summary>
		private void BattleEnd()
		{

			//�G�����Ȃ�������
			if (SManager.instance.isBattleEnd && !(SManager.instance.targetList.Count == 0))
			{
				battleEndTime += Time.fixedDeltaTime;

				//���Ԍv�����Čx���Ɉڍs
				if (battleEndTime >= 2 && !isReset)
				{

					isReset = true;
					//�Z�b�ȏ�G���m���񂸂�Όx���t�F�C�Y��
					nowPosition = false;
					isPosition = false;
					SManager.instance.isEscape = true;
					nowState = SisterState.�x��;
					//escape��true�̎��x������퓬�ɂȂ�Ȃ�
					stateNumber = 3;
					beforeNumber = 0;
					reJudgeTime = 0;
					reJudgePositionTime = 0;
					changeable = true;
					Serch3.SetActive(false);
					Serch2.SetActive(true);
					Serch.SetActive(true);
					battleEndTime = 0;

					SManager.instance.targetList.Clear();
					SManager.instance.targetCondition.Clear();
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
		public void WarpEffect()
		{
			Transform gofire = firePosition;

			//Transform rotate = SManager.instance.useMagic.castEffect.LoadAssetAsync<Transform>().Result as Transform;

			Addressables.InstantiateAsync("WarpCircle", gofire.position, gofire.rotation);//.Result;//�����ʒu��Player
			GManager.instance.PlaySound("Warp", transform.position);
		}

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
			#region
			/*
					/// the speed of the character when it's walking
	[Tooltip("the speed of the character when it's walking")]
	public float WalkSpeed = 6f;
	/// the multiplier to apply to the horizontal movement
	//�@�ǂݎ���p�B�R�[�h����ς��Ă�
	[MMReadOnly]
	[Tooltip("���������̈ړ��ɓK�p����{��")]
	public float MovementSpeedMultiplier = 1f;
	/// the multiplier to apply to the horizontal movement, dedicated to abilities
	[MMReadOnly]
	[Tooltip("���������̈ړ��ɓK�p����{���ŁA�A�r���e�B�ɓ������Ă��܂��B")]
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
	[Tooltip("if this is true, the character will automatically flip to face its movement direction")]
	public bool FlipCharacterToFaceDirection = true;


	/// the current horizontal movement force
	public float HorizontalMovementForce { get { return _horizontalMovementForce; }}
	/// if this is true, movement will be forbidden (as well as flip)
	public bool MovementForbidden { get; set; }

	[Header("Input")]

	/// if this is true, will get input from an input source, otherwise you'll have to set it via SetHorizontalMove()
	//  ���ꂪ�^�̏ꍇ�A���̓\�[�X����̓��͂��擾���܂��B�����łȂ��ꍇ�́ASetHorizontalMove() �Őݒ肷��K�v������܂��B
	[Tooltip("if this is true, will get input from an input source, otherwise you'll have to set it via SetHorizontalMove()")]
	public bool ReadInput = true;
	/// if this is true, no acceleration will be applied to the movement, which will instantly be full speed (think Megaman movement). Attention : a character with instant acceleration won't be able to get knockbacked on the x axis as a regular character would, it's a tradeoff
	/// ���ꂪ�^�̏ꍇ�A�����x�͓K�p���ꂸ�A�u���ɑS���͂ƂȂ�܂��i���b�N�}���̓�����z�����Ă��������j�B
	/// ���ӁF�u�ԓI�ȉ��������L�����N�^�[�́A�ʏ�̃L�����N�^�[�̂悤��X���Ńm�b�N�o�b�N���󂯂邱�Ƃ͂ł��܂���B
	[Tooltip("if this is true, no acceleration will be applied to the movement, which will instantly be full speed (think Megaman movement). Attention : a character with instant acceleration won't be able to get knockbacked on the x axis as a regular character would, it's a tradeoff")]
	public bool InstantAcceleration = false;
	/// the threshold after which input is considered (usually 0.1f to eliminate small joystick noise)
	/// ���͂��l������臒l�i�����ȃW���C�X�e�B�b�N�m�C�Y���������邽�ߒʏ�0.1f�j(���m���Ȃ��l���m�F)
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
	/// �L�����N�^�[���n�ʂɏՓ˂����Ƃ��ɍĐ������MMFeedbacks
	[Tooltip("the MMFeedbacks to play when the character hits the ground")]
	public MMFeedbacks TouchTheGroundFeedback;
	/// the duration (in seconds) during which the character has to be airborne before a feedback can be played when touching the ground
	/// �L�����N�^���󒆂ɂ���ԁA�n�ʂɐG��Ă��t�B�[�h�o�b�N���Đ������܂ł̎��ԁi�b�j�ł��B
	[Tooltip("the duration (in seconds) during which the character has to be airborne before a feedback can be played when touching the ground")]
	public float MinimumAirTimeBeforeFeedback = 0.2f;

	[Header("Walls")]
	/// Whether or not the state should be reset to Idle when colliding laterally with a wall
	/// �ǂɉ�����Փ˂����Ƃ��ɏ�Ԃ�Idle�ɖ߂����ǂ���
	[Tooltip("Whether or not the state should be reset to Idle when colliding laterally with a wall")]
	public bool StopWalkingWhenCollidingWithAWall = false;
			 */
			#endregion
			_characterHorizontalMovement.WalkSpeed = status.walkSpeed;
			if (_characterRun != null)
			{
				_characterRun.RunSpeed = status.dashSpeed;
			}



			if (_jump != null)
			{
				#region
				/*
						/// the maximum number of jumps allowed (0 : no jump, 1 : normal jump, 2 : double jump, etc...)
		[Tooltip("the maximum number of jumps allowed (0 : no jump, 1 : normal jump, 2 : double jump, etc...)")]
		public int NumberOfJumps = 2;
		/// defines how high the character can jump
		[Tooltip("defines how high the character can jump")]
		public float JumpHeight = 3.025f;
		/// basic rules for jumps : where can the player jump ?
		[Tooltip("basic rules for jumps : where can the player jump ?")]
		public JumpBehavior JumpRestrictions = JumpBehavior.CanJumpAnywhere;
		/// if this is true, camera offset will be reset on jump
		[Tooltip("if this is true, camera offset will be reset on jump")]
		public bool ResetCameraOffsetOnJump = false;
		/// if this is true, this character can jump down one way platforms by doing down + jump
		[Tooltip("if this is true, this character can jump down one way platforms by doing down + jump")]
		public bool CanJumpDownOneWayPlatforms = true;

		[Header("Proportional jumps")]

		/// if true, the jump duration/height will be proportional to the duration of the button's press
		[Tooltip("if true, the jump duration/height will be proportional to the duration of the button's press")]
		public bool JumpIsProportionalToThePressTime = true;
		/// the minimum time in the air allowed when jumping - this is used for pressure controlled jumps
		[Tooltip("the minimum time in the air allowed when jumping - this is used for pressure controlled jumps")]
		public float JumpMinimumAirTime = 0.1f;
		/// the amount by which we'll modify the current speed when the jump button gets released
		[Tooltip("the amount by which we'll modify the current speed when the jump button gets released")]
		public float JumpReleaseForceFactor = 2f;

		[Header("Quality of Life")]

		/// a timeframe during which, after leaving the ground, the character can still trigger a jump
		[Tooltip("a timeframe during which, after leaving the ground, the character can still trigger a jump")]
		public float CoyoteTime = 0f;

		/// if the character lands, and the jump button's been pressed during that InputBufferDuration, a new jump will be triggered 
		[Tooltip("�L�����N�^�[�����n���A����InputBufferDuration�̊ԂɃW�����v�{�^���������ꂽ�ꍇ�A�V�����W�����v���J�n����܂��B")]
		public float InputBufferDuration = 0f;

		[Header("Collisions")]

		/// duration (in seconds) we need to disable collisions when jumping down a 1 way platform
		[Tooltip("duration (in seconds) we need to disable collisions when jumping down a 1 way platform")]
		public float OneWayPlatformsJumpCollisionOffDuration = 0.3f;
		/// duration (in seconds) we need to disable collisions when jumping off a moving platform
		[Tooltip("duration (in seconds) we need to disable collisions when jumping off a moving platform")]
		public float MovingPlatformsJumpCollisionOffDuration = 0.05f;

		[Header("Air Jump")]

		/// the MMFeedbacks to play when jumping in the air
		[Tooltip("the MMFeedbacks to play when jumping in the air")]
		public MMFeedbacks AirJumpFeedbacks;

		/// the number of jumps left to the character
		[MMReadOnly]
		[Tooltip("the number of jumps left to the character")]
		public int NumberOfJumpsLeft;

		/// whether or not the jump happened this frame
		public bool JumpHappenedThisFrame { get; set; }
		/// whether or not the jump can be stopped
		public bool CanJumpStop { get; set; }

				 */
				#endregion
				_jump.CoyoteTime = status.jumpCool;
				_jump.JumpHeight = status.jumpRes;
				_jump.NumberOfJumps = 2;

			}

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

		

	}
}
