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



		[Header("�ˏo�n�_")]
		///<summary>
		///�e�ۏo���ꏊ
		///</summary>
		public Transform firePosition;

		/// <summary>
		/// ���[�v�̔���Ɏg��
		/// </summary>
		[SerializeField]
		private LayerMask layerMask;

		bool disEnable;
		/// <summary>
		/// �v���C���[�������Ă�t���O�B�v���C���[�������Ă�����Ȃ�
		/// �v���C���[����������̃��[�v��ɃI��
		/// </summary>
		[HideInInspector] public bool isEscape;


		public enum SisterState
		{
			�̂�т�,
			�x��,
			�킢
		}

		public enum MoveState
		{
			��~,
			����,
			����,
			�ŏ�
		}

		//[HideInInspector]
		public SisterState nowState = SisterState.�̂�т�;
		[HideInInspector] public MoveState nowMove = MoveState.�ŏ�;
		MoveState lastState = MoveState.�ŏ�;


		[Header("�V�X�^�[����̃X�e�[�^�X")]
		public SisterStatus status;



		// === �O���p�����[�^ ======================================

		Vector2 distance;//�v���C���[�Ƃ̋���

		bool isStop;//��~�t���O
		float waitTime;//�p�g���[�����̑҂����Ԑ�����
					   //	 bool posiReset;//�퓬�̎�����Đݒ肷��t���O



		int initialLayer;
		bool jumpTrigger;

		bool isWait;//���̃t���O���^�Ȃ�ҋ@���[�V��������B�}���g����������V�񂾂�B
					//���[�V�����͍D���x�Ƃ��i�s�x�Ń��[�V���������X�g���؂�ւ���Č��܂�B

		
	
		[HideInInspector] public bool nowPosition;//������t���O
		Vector2 myPosition;

		/// <summary>
		/// �퓬��������𗣂�Ă��������f���鎞�ԁB
		/// </summary>
		float escapeTime;

		// === �L���b�V�� ======================================





		// === �����p�����[�^ ======================================

		int direction;
		int directionY;

		/// <summary>
		/// �ŏI�I�ɂǂ���ɓ�����
		/// </summary>
		float moveDirection;


		//�����n�̃p�����[�^

		Vector2 basePosition;
		/// <summary>
		/// �v���C���[�������Ă�����B�
		/// </summary>
		Vector3 baseDirection;

		[HideInInspector] public float playPosition;//�����̏ꏊ
		[HideInInspector] public float playDirection;//�V�Ԏ��̕���

		[HideInInspector] public float enemyPosition;//�G�̏ꏊ
		[HideInInspector] public float enemyDirection;//�G���̕���

		//	public Rigidbody2D GManager.instance.GManager.instance.pm.rb;//�v���C���[�̃��W�b�h�{�f�B


		string squatTag = "SquatWall";

		bool isClose;//��x�߂��܂ōs��
		[HideInInspector] public bool isPlay;

		string jumpTag = "JumpTrigger";
		float patrolJudge;//�x�����Ԃ𐔂���
		bool isVertical;//������т���t���O
		Vector2 waitPosition;



		/// <summary>
		/// �ړ���Ԃ��Ĕ��肷�邽�߂̎��ԑ���
		/// </summary>
		[HideInInspector] public float reJudgeTime =3;
		/// <summary>
		/// ���s����_�b�V���ȂǂɕύX�\���B
		/// ���ꂪ�Ȃ��ƕ��s�Ƒ��s�̂͂��܂ŃK�^�K�^
		/// </summary>
		[HideInInspector] public bool changeable;//

		float battleEndTime;
		Vector2 move = new Vector2();



		// === �A�r���e�B ================
		#region
		/// the number of jumps to perform while in this state
		//	[Tooltip("the number of jumps to perform while in this state")]
		//	public int NumberOfJumps = 1;




		//���ړ��͌p�����Ƃɂ�����

		public PlayerJump _jump;
		//�@public int _numberOfJumps = 0;


		//�@public int _numberOfJumps = 0;


		public PlayerRunning _characterRun;


	//	public MyWakeUp _wakeup;

		//public WeaponAbillity _attack;

		public MyDamageOntouch _damage;

		public WarpAbility _warp;

		public FireAbility _fire;
		public PlayerCrouch _squat;
		//	public new MyHealth _health;

		//�V�X�^�[����̌ŗL�A�N�V����

		public SensorAbility _sensor;
		//	protected Hittable _hitInfo;

		//protected RewiredCorgiEngineInputManager _inputManager;


		// �A�j���[�V�����p�����[�^�[
		protected const string _stateParameterName = "_nowPlay";
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
            
			//�Z���T�[�A�r���e�B�̐؂�ւ�����ɕύX
			_sensor.RangeChange();
			//_characterHorizontalMovement.FlipCharacterToFaceDirection = false;
			ParameterSet(status);
		}



        /// <summary>
        /// 1�t���[�����ƂɁA���Ⴊ��ł��邩�ǂ����A�܂����Ⴊ��ł���ׂ������`�F�b�N���܂�
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
			GManager.instance.sisMpSlider.value = mp / maxMp;
			Brain();
        }
		public void Brain()
        {


			if (isStop || _condition.CurrentState != CharacterStates.CharacterConditions.Normal)
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

			EscapeWarp();
			if (nowState == SisterState.�킢)
			{
				

				BattleEnd();

				PositionSetting();

			}

			else if (nowState == SisterState.�x��)
			{
				patrolJudge += _controller.DeltaTime;


				////Serch.SetActive(true);�ق��̃X�e�[�g�Ɉړ�����Ƃ���
				////Serch2.SetActive(true);

				if (!disEnable)
				{
					PatrolMove();


					//	PositionSetting();
					
				}

				//	PositionChange();
				if (patrolJudge >= patrolTime && !isEscape)
				{
				    changeable = true;
					nowState = SisterState.�̂�т�;
					//_sensor.RangeChange();
					////Serch2.SetActive(true);
					////Serch.SetActive(true);
				}


			}
			else if (nowState == SisterState.�̂�т�)
			{
				if (!disEnable)
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
			MoveController();
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
            if (_inputManager.CombinationButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
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
			
			if ( _controller.State.IsGrounded && !disEnable)
			{
				//�|�W�V�����ɂ��ɍs���B�����ĂȂ��Ēn�ʂɂ���
					SManager.instance.GetClosestEnemyX();
					float enemyDistance = SManager.instance.closestEnemy - myPosition.x;
					int mDirection = (int)Mathf.Sign(enemyDistance);
					enemyDistance = Mathf.Abs(enemyDistance);
		//		Debug.Log($"iiiiiiiiiiii{nowPosition}");
				if (!nowPosition)
				{
					//�����Ŏg���͈̂�ԋ߂��G

					//Debug.Log("dddsssdd");
					//��ԋ߂��G���E�ɂ���Ƃ�

					if (changeable)
					{

				//		Debug.Log("����");
						//�G����\���ɗ��ꂽ��
						if (enemyDistance >= status.battleDis)
						{
						//	Debug.Log("��������");
							nowPosition = true;
							nowMove = MoveState.��~;
							moveDirection = mDirection;
						}
						else// if (SManager.instance.closestEnemy - myPosition.x < status.battleDis)
						{
					//		Debug.Log("�ł���ˁ[");
							nowMove = MoveState.����;
							moveDirection = -mDirection;
						}
					}


				}
				else if (nowPosition)
				{
					//�����Ŏg���̂̓^�[�Q�b�g�̓G

					//�Ĕ��肷�邩�ǂ���
					//�r�����ĂȂ��Ƃ��ɓG�����S�͈͓��ɂĕ�������Ĕ���

						escapeTime += _controller.DeltaTime;
						if (SManager.instance.target != null && changeable)
						{
							int atDirection = (int)Mathf.Sign(SManager.instance.target.transform.position.x - myPosition.x);
							nowMove = MoveState.��~;

							moveDirection = atDirection;
							

						}

					if (escapeTime >= SManager.instance.sisStatus.escapeTime && enemyDistance < status.battleDis)
					{
						escapeTime = 0.0f;
						Debug.Log("ddddd");
						nowPosition = false;
						SManager.instance.target = null;
						// = 99;
					}

				}
				//	}
			}

		}



		public void MoveController()
        {

			if (!changeable)
			{
				reJudgeTime += _controller.DeltaTime;
				if (reJudgeTime >= 1f)
				{
					changeable = true;
					reJudgeTime = 0;
				}
			}
		//	Debug.Log($"��{nowMove}");


			if (!disEnable)
			{
			//	Debug.Log($"��������");
				if (nowMove == MoveState.��~)
				{
					if (_movement.CurrentState == CharacterStates.MovementStates.Running)
					{
						_characterRun.RunStop();
					}
					if (transform.localScale.x != moveDirection)
					{
						_character.Flip();
					}
					_characterHorizontalMovement.SetHorizontalMove(0);
				}
				else if (nowMove == MoveState.����)
				{
					if (_movement.CurrentState == CharacterStates.MovementStates.Running)
					{
						_characterRun.RunStop();
					}
					isWait = false;
					_characterHorizontalMovement.SetHorizontalMove(moveDirection);
				}
				else if (nowMove == MoveState.����)
				{
					if (_movement.CurrentState != CharacterStates.MovementStates.Running)
					{
						_characterRun.RunStart();
						isWait = false;
					}
					_characterHorizontalMovement.SetHorizontalMove(moveDirection);
				}
			}
				lastState = nowMove;

			


        }



		/// <summary>
		/// �ҋ@���̏����s��
		/// �V�ѕ������ڋߊ��������
		/// </summary>
		public void PatrolMove()
		{
			if (_controller.State.IsGrounded && !disEnable)
			{
				if (changeable)
				{
					if (Mathf.Abs(distance.x) > status.walkDistance)
					{
						nowMove = MoveState.����;
					}
					//�߂��ɂ��鎞
					else if (Mathf.Abs(distance.x) < status.patrolDistance)
					{
                        if (Mathf.Sign(GManager.instance.Player.transform.localScale.x) == Mathf.Sign(transform.localScale.x) && GManager.instance.pc.NowSpeed() >= 100)
                        {
							nowMove = MoveState.����;
                        }
                        else
                        {
							isEscape = false;
							nowMove = MoveState.��~;
							isWait = true;
                        }
						
					}
					changeable = false;
					moveDirection = direction;
				}

			}
		}
		public void PlayMove()
		{//�̂�т�
			if (_controller.State.IsGrounded && !disEnable)
			{
			//	Debug.Log($"������������{GManager.instance.pc.NowSpeed()}");
				//�C�x���g�I�u�W�F�N�g�͓����Ȃ��̂ŏ�ɔ��f
				if (isPlay)
				{
					    float playDistance = playPosition - myPosition.x;
						playDirection = playDistance >= 0 ? 1 : -1;
						playDistance = Mathf.Abs(playDistance);
					if (Mathf.Abs(distance.x) <= status.playDistance && !isClose)
					{
						
						//�����ɐڐG
						waitTime = 0.0f;

						//�I�u�W�F�N�g�ɏ\���߂Â�����
						if (playDistance <= 2)
						{
							nowMove = MoveState.��~;

						}
						else if (playDistance <= status.walkDistance)
						{
							nowMove = MoveState.����;
						}
						else
						{
							nowMove = MoveState.����;

						}
						moveDirection = playDirection;
					}
					else
					{
						//��񂿂��Ɛڋ߂��悤
						isClose = true;
						nowMove = MoveState.����;
						moveDirection = direction;
						if (Mathf.Abs(distance.x) <= status.patrolDistance)
						{
							//����������ڋߎw���t���O����
							isClose = false;
						}

						//������x���ꂽ��V�΂Ȃ��Ȃ�
                        if (playDistance >= 100)
                        {
							isPlay = false;
                        }
						moveDirection = direction;
					}
				}
				else if (Mathf.Abs(distance.x) <= status.patrolDistance + status.adjust)
				{

					//�v���C���[���~�܂��Ă鎞�͒�~
					if (GManager.instance.pc.NowSpeed() <= 100)
					{
						nowMove = MoveState.��~;
						isWait = true;
						//isWait�Ŏ��Ԍo�߂ň�l�ŗV�񂾂肷��H
					}
					//�����Ă�Ȃ����
					else
					{
						nowMove = MoveState.����;
						

					}

					changeable = false;
					reJudgeTime = 0;
				}
				else if (changeable)
				{
					//�V��ł��Ă����������痣�ꂽ��
					if (Mathf.Abs(distance.x) > status.walkDistance)//|| isClose)
					{
				//		Debug.Log($"ggggg");
						isClose = true;//�ڋ߂��悤���Ă����t���O
						if (Mathf.Abs(distance.x) <= status.patrolDistance)
						{
							//����������ڋߎw���t���O����
							isClose = false;
						}
						nowMove = MoveState.����;

					}
					//�v���C���[�ɂ������Ă鎞
					
					//�V��łĂ��������̒��ŁA�v���C���[�ɂ������ĂȂ��Ƃ�
					else if (Mathf.Abs(distance.x) > status.patrolDistance)
					{

						nowMove = MoveState.����;

					}

					moveDirection = direction;
					changeable = false;
				}
				

�@�@�@�@�@�@�@�@//isWait�̊��p��
				#region
				
				/*
				else if (stateNumber == 10)
				{
					waitTime += _controller.DeltaTime;
					//Flip(direction);
					//�v���C���[�������~�܂��ĂĊ������Ȃ��Ƃ�
					_characterHorizontalMovement.SetHorizontalMove(0); 
					//("Stand");
					if (waitTime >= SManager.instance.sisStatus.waitRes && !isWait)
					{
						waitPosition = basePosition;
						isWait = true;
						waitTime = 0.0f;
					}
				}*/





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
				//Flip(-transform.localScale.x);//���]�����܂�
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
				if (collision.tag == squatTag && _movement.CurrentState != CharacterStates.MovementStates.Crouching && _movement.CurrentState != CharacterStates.MovementStates.Crawling)
				{
					_squat.Crouch();
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

				if (collision.tag == squatTag && _movement.CurrentState != CharacterStates.MovementStates.Crouching && _movement.CurrentState != CharacterStates.MovementStates.Crawling)
				{
					_squat.Crouch();
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
				//�g���l���̒������Ⴊ�݃g���K�[�Ŗ�����
				if (collision.tag == squatTag && (_movement.CurrentState != CharacterStates.MovementStates.Crouching || _movement.CurrentState == CharacterStates.MovementStates.Crawling))
				{
					_squat.ExitCrouch();
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
							//Flip(1);
						}
						else
						{
							move.Set(basePosition.x + 6, basePosition.y);
							move.y = RayGroundCheck(move) + 10;
							transform.position = move;
							//Flip(-1);
						}
						_controller.SetForce(Vector2.zero);
						nowPosition = false;
						isEscape = true;
						//escape��true�̎��x������퓬�ɂȂ�Ȃ�

						reJudgeTime = 0;
						changeable = true;

						_warp.WarpStart();



						if (nowState == SisterState.�킢)
						{
							reJudgeTime = 0;
							nowState = SisterState.�x��;
							_sensor.RangeChange();
							patrolTime = 0;
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
			if (_movement.CurrentState != CharacterStates.MovementStates.Warp)
			{

				if (SManager.instance.targetList.Count == 0)
				{
					battleEndTime += _controller.DeltaTime;

					//���Ԍv�����Čx���Ɉڍs
					if (battleEndTime >= 2)
					{
						StateChange();
						nowState = SisterState.�x��;

					}

				}
				else
				{
					
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

			if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(Name))// || _animator.GetCurrentAnimatorStateInfo(0).IsName("OStand"))
			{   // �����ɓ��B�����normalizedTime��"Default"�̌o�ߎ��Ԃ��E���Ă��܂��̂ŁAResult�ɑJ�ڊ�������܂ł�return����B
				return true;
			}
			if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
			{   // �ҋ@���Ԃ���肽���Ȃ�΁A�����̒l��傫������B
				return true;
			}
			//AnimatorClipInfo[] clipInfo = _animator.GetCurrentAnimatorClipInfo(0);

			////Debug.Log($"�A�j���I��");

			return false;

			// return !(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
			//  (_currentStateName);
		}

        /// <summary>
        /// �A�j���C�x���g
		/// ��
        /// </summary>
        #region
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

        #endregion

        /// <summary>
        /// �X�e�[�g�ω��O�̏���
        /// </summary>
        /// <param name="nextState"></param>
        public void StateChange(SisterState nextState = SisterState.�̂�т�)
        {
			if (nextState == SisterState.�킢)
			{
                if (isEscape)
                {
					return;
                }
				patrolTime = 0;
				SManager.instance.playObject = null;
				isPlay = false;
				//�����Ƀ|�W�V�������f�ł���悤��
				reJudgeTime = 150;
				//Debug.Log("�@�\���Ă܂���[");

				nowState = BrainAbility.SisterState.�킢;//���̕ӂ͂܂���Őݒ�ł���悤�ɂ��悤
			}
            else
            {
				_fire.isReset = true;

				//�Z�b�ȏ�G���m���񂸂�Όx���t�F�C�Y��
				nowPosition = false;

				//escape��true�̎��x������퓬�ɂȂ�Ȃ�
				reJudgeTime = 0;
				patrolTime = 0;
				changeable = true;
				_sensor.RangeChange();
				//Serch2.SetActive(true);
				//Serch.SetActive(true);
				battleEndTime = 0;

				SManager.instance.targetList = null;
				SManager.instance.targetCondition = null;
				SManager.instance.target = null;
				SManager.instance.targetList = new List<GameObject>();
				SManager.instance.targetCondition = new List<EnemyAIBase>();
			}
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
			_health.MaximumHealth = status.maxHp;
			_health.CurrentHealth = (int)maxHp;
		}

		public void Flip(int direction)
		{

		}
		/// <summary>
		///  �K�v�ȃA�j���[�^�[�p�����[�^�[������΁A�A�j���[�^�[�p�����[�^�[���X�g�ɒǉ����܂��B
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter(_stateParameterName, AnimatorControllerParameterType.Bool, out _stateAnimationParameter);
		}

		/// <summary>
		/// ������I�[�o�[���C�h����ƁA�L�����N�^�[�̃A�j���[�^�[�Ƀp�����[�^�𑗐M���邱�Ƃ��ł��܂��B
		/// ����́ACharacter�N���X�ɂ���āAEarly�Anormal�ALate process()�̌�ɁA1�T�C�N�����Ƃ�1��Ăяo�����B
		/// </summary>
		public override void UpdateAnimator()
		{
			//�̂�т�1�A�x��2�A�킢3
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _stateAnimationParameter,nowState != SisterState.�̂�т� , _character._animatorParameters);
		}


	}
}
