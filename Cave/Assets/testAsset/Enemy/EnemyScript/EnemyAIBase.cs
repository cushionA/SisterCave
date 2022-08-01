
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using MoreMountains.CorgiEngine;
using Guirao.UltimateTextDamage;
namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
	public class EnemyAIBase : MyAbillityBase
{

    //�A�r���e�B�Z�b�g�����[�g���ɂ����Ă��

    //�u//sAni.�v���usAni.�v�Œu��������B�A�j���������

    //�A�j���̎��
    //�K�{
    //�ړ��iMove�j�A�����iWalk�j�A����iDash�j�A���݁iFalter�j�A�_�E���iDown�j�A�N���オ��iWakeup�j
    //������сiBlow�j�A�e����iBounce�j�A�������iBackMove�j�A�퓬�ҋ@�iPose�j�A���iStand�j�A���S�iNDie,DDie�j
    //
    //�K�v�ɉ�����
    //�����iFall�j�A�K�[�h�iGuard�j�A�K�[�h�ړ��iGuardMove�j�A���K�[�h�ړ��iBackGuard�j�A�K�[�h�u���C�N�iGuardBreak�j
    //����iAvoid�j�A�W�����v�iJump�j
    //
	//���p�����[�^
    #region
    [SerializeField] GameObject effectController;

	[Header("���o�T�m")]
	///<summary>
	///���o�B����Ȃ��̓G�����Ă��ʔ�������
	///</summary>
	public GameObject Serch;

	[Header("�S���ʂ̋C�z�T�m")]
	///<summary>
	///���Ⴊ��ł��甲������S���ʊ��m��
	///</summary>
	public GameObject Serch2;

	public GameObject Guard;

	[Header("�ˏo�n�_")]
	///<summary>
	///�e�ۏo���ꏊ
	///</summary>
	public Transform firePosition;
	[SerializeField]
	Transform dispary;

	/// <summary>
	/// �J�����Ɏʂ��Ă邩
	/// </summary>
	[HideInInspector] public bool cameraRendered;
	/// <summary>
	/// ������ł邩�ǂ���
	/// </summary>
	[HideInInspector] public bool isWater;


	[Header("�G�l�~�[�̃X�e�[�^�X")]
	public EnemyStatus status;
	// === �O���p�����[�^ ======================================
	/*[HideInInspector]*/
	public bool isAggressive;//�U�����[�h�̓G
//	[HideInInspector] public bool _movement.CurrentState != CharacterStates.MovementStates.Attack;//���ꂪTrue�̎��Ђ�܂Ȃ�
	protected bool isRight = true;//�E�Ƀp�g���[�����邩�ǂ���
	//protected bool _controller.State.IsGrounded;
	protected Vector2 distance;//�v���C���[�Ƃ̋���
	protected float escapeTime;//�����邾��
	protected float stopTime;//��~���Ԃ𐔂���
	protected bool isStop;//��~�t���O
	protected float waitTime;//�p�g���[�����̑҂����Ԑ�����
	protected bool posiReset;//�퓬�Ƃ��ŏ�𗣂ꂽ�Ƃ��߂邽�߂̃t���O
	protected Vector3 firstDirection;//�ŏ��Ɍ����Ă����
	//protected bool nowJump;
	protected bool isReach;//�ԍ����̒��ɂ���
	protected Vector2 targetPosition;//�G�̏ꏊ
	protected bool isUp;
	protected int initialLayer;
	protected bool jumpTrigger;
	//�K�[�h���肵�����ǂ���
	protected bool guardJudge;
	protected float stayTime;//�U����̑ҋ@����
	protected int attackNumber;//���Ԃ̍U�������Ă���̂�
	protected bool isAtEnable = true;
	protected bool isDamage;
	protected bool isMovable;
	protected bool isAnimeStart;//�A�j���̊J�n�Ɏg��



	[HideInInspector] public int bulletDirection;//�G�e���ǂ��炩�痈����

	//�U������҂�����
	protected float flipWaitTime = 0f;
	protected float lastDirection;

	protected float attackBuff = 1;//�U���{��

	bool flyNow;

	/// <summary>
	/// �U���̒l
	/// </summary>
	protected string useSound;

	List<Transform> mattTrans = new List<Transform>();

	/// <summary>
	/// �U���̒l
	/// </summary>
	[HideInInspector] public EnemyValue atV;
	[HideInInspector] public EnemyStatus.MoveState ground = EnemyStatus.MoveState.wakeup;
	[HideInInspector] public EnemyStatus.MoveState air = EnemyStatus.MoveState.wakeup;
	// === �L���b�V�� ==========================================
	//public Animator animator;
	public Animator sAni;


	public Rigidbody2D rb;//�p����Ŏg�����̂�protected

		[Header("�h�b�O�p�C��")]
		///<summary>
		///�ړ��̒��S�ƂȂ�I�u�W�F�N�g�B�G�������A�ꂽ��
		///</summary>
		public GameObject dogPile;

		// === �����p�����[�^ ======================================
		protected float xSpeed;
	protected float ySpeed;
	protected int direction;
	protected int directionY;
	protected float moveDirectionX;
	protected float moveDirectionY;
	protected float jumpTime;//�W�����v�̃N�[���^�C���B
	protected bool disenableJump;//�W�����v�s�\���
	protected AudioSource[] seAnimationList;
	protected Vector3 startPosition;//�J�n�ʒu�Ƒҋ@���e���g���[�̋N�_
	protected Vector3 basePosition;
	protected Vector3 baseDirection;
	protected bool enableFire;
	protected float waitCast;
	protected float randomTime = 2.5f;//�����_���ړ�����̂���
	protected bool isGuard;
	protected bool guardBreak;//�K�[�h�u���C�N
	[HideInInspector] public bool guardHit;
	protected float stateJudge;//�X�e�[�g���f�Ԋu
	protected Rigidbody2D dRb;//�h�b�O�p�C���̃��W�b�h�{�f�B
							  //	protected float nowDirection;//���̓G�̕���

	[HideInInspector] public float damageDelay;
	[HideInInspector] public bool isHit;//�q�b�g��
	[HideInInspector] public bool isHitable;
	[HideInInspector] public GameObject lastHit;
	protected float jumpWait;
	protected bool isWakeUp;//�_�E���I����N���オ��t���O
	protected bool blowDown;


	protected float blowTime;
	protected float recoverTime;//�A�[�}�[�񕜂܂łɂ����鎞��
	protected int lastArmor = 1;
		
		//-----------------------------------------------------

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

		//�X�e�[�^�X
		//HP�̓w���X����
		public float maxHp = 100;

		//���̕ӂ���Ȃ�����
		//�ʂɎ��������{���ő��삷��΂悭�ˁH

		//�@�������h��́B�̗͂ŏオ��
		public float Def = 70;
		//�h�˖h��B�ؗ͂ŏオ��
		public float pierDef = 70;
		//�Ō��h��A�Z�ʂŏオ��
		public float strDef = 70;
		//�_���h��A�؂ƌ����ŏオ��B
		public float holyDef = 70;
		//�Ŗh��B�����ŏオ��
		public float darkDef = 70;
		//���h��B�����Ɛ����ŏオ��
		public float fireDef = 70;
		//���h��B�����Ǝ��v�ŏオ��B
		public float thunderDef = 70;


		protected float nowArmor;


		//-------------------------------------------
		[Header("�퓬���̈ړ����x")]
		public Vector2 combatSpeed;
		[Header("�Q�Ɨp�̍U����")]
		/// <summary>
		/// �Q�Ɨp�̍U����
		/// </summary>
		public int atkDisplay;
		[Header("�Q�Ɨp�̖h���")]
		/// <summary>
		/// �Q�Ɨp�̖h���
		/// </summary>
		public int defDisplay;
		//---------------------------------------------



	[SerializeField]
	protected SpriteRenderer td;

	[SerializeField]
	protected Transform atBlock;
	/// <summary>
	/// �U����t���O�B�U����̋����Ĕ��苓���Ŏg��
	/// </summary>
	protected bool attackComp;

	/// <summary>
	/// �G�t�F�N�g����p�̃}�e���A��
	/// </summary>
	[SerializeField]
	protected Renderer parentMatt;
	/// <summary>
	/// �}�e���A������n���t���O
	/// </summary>
	protected int materialSet;
	/// <summary>
	/// ����Ώۂ̃}�e���A���Ǘ��i���o�[�B0����
	/// </summary>
	int mattControllNum;
	/// <summary>
	/// ����Ώۂ̃X�v���C�g�ꗗ
	/// </summary>
	public Transform[] spriteList;
	/*
	/// <summary>
	/// �}�e���A�����Z�b�g���ꂽ��
	/// </summary>
	protected bool spriteSet;*/
	protected List<Renderer> controllTarget = new List<Renderer>();

		#endregion

		//�V�p�����[�^
		#region
		/// the number of jumps to perform while in this state
		//	[Tooltip("the number of jumps to perform while in this state")]
		//	public int NumberOfJumps = 1;
		
		///<summary>
		///�ǂ���̕������������̂���B
		/// </summary>
		int horizontalDirection;

		//���ړ��͌p�����Ƃɂ�����

	�@�@protected PlayerJump _jump;
	�@//�@protected int _numberOfJumps = 0;

		protected PlayerRoll _rolling;

		protected CharacterRun _characterRun;

		protected EnemyFly _flying;

		protected GuardAbillity _guard;

		protected MyWakeUp _wakeup;

		protected EAttackCon _attack;

		protected MyDamageOntouch _damage;

		[HideInInspector]
		public new MyHealth _health;
        private bool isVertical;

        //	protected Hittable _hitInfo;


        #endregion


        // === �R�[�h�iMonobehaviour��{�@�\�̎����j ================
        protected override void Initialization()
		{
		initialLayer = this.gameObject.layer;
			ParameterSet(status);
		ArmorReset();
			_health = (MyHealth)base._health;
			_characterHorizontalMovement.FlipCharacterToFaceDirection = false;
			if (status.kind != EnemyStatus.KindofEnemy.Fly)
		{
				//GravitySet(status.firstGravity);//�d�͐ݒ�
				_flying.StartFlight();
		}
		else
		{
			GravitySet(status.firstGravity);//�d�͐ݒ��ŕς��悤��
		}
		//	rb = this.gameObject.GetComponent<Rigidbody2D>();
		startPosition = transform.position;
		firstDirection = transform.localScale;
		HPReset();
		
		if (dogPile != null)
		{
			basePosition = dogPile.transform.position;
			baseDirection = dogPile.transform.localScale;
			dRb = dogPile.GetComponent<Rigidbody2D>();
		}
		else
		{
			basePosition = startPosition;
			baseDirection = firstDirection;

		}
		if (status.enemyFire != null)
		{
			enableFire = true;
		}
		//parentMatt = GetComponent<SpriteRenderer>().material;
		//td = GetComponent<TargetDisplay>();
	}





	public override void ProcessAbility()
		{
			Brain();

	}


	public void Brain()
        {

			//�J�����Ɏʂ��ĂȂ��Ƃ��͎~�܂�܂��B
			if (!cameraRendered)
			{
				if (!status.unBaind)
				{
					//	Debug.Log("��~");
					//�����Ȃ��Ȃ���
					return;
				}
			}

			//	Debug.Log($"�m�肽��");

			//�ړ��\���ǂ����m�F

				if (_condition.CurrentState == CharacterStates.CharacterConditions.Stunned || guardHit || _movement.CurrentState == CharacterStates.MovementStates.Rolling || _movement.CurrentState == CharacterStates.MovementStates.Attack
						|| _movement.CurrentState == CharacterStates.MovementStates.Jumping || _movement.CurrentState == CharacterStates.MovementStates.DoubleJumping || guardBreak || _condition.CurrentState == CharacterStates.CharacterConditions.Dead)
				{

						isMovable = false;
					
				}
				else if (!_controller.State.IsGrounded && status.kind != EnemyStatus.KindofEnemy.Fly)
				{
					isMovable = false;
				}
                else
                {
					isMovable = true;
                }
			
			//�h�b�O�p�C������܂���



			//�������鎞����
			if (isMovable)
			{
				
			if (dogPile != null)
			{
				basePosition = dogPile.transform.position;
				baseDirection = dogPile.transform.localScale;
			}
			else
			{
				basePosition = startPosition;
				baseDirection = firstDirection;

			}

				////////Debug.log($"�W�����v��{nowJump}");
				////////Debug.log($"���{_movement.CurrentState != CharacterStates.MovementStates.Rolling}");
				if (isAggressive)
				{
					//�U���s�����B
					//�����ɓG�Ƃ̋����Ƃ����f����HorizontalMove�Ƃ��N�����郁�\�b�h���B
					posiReset = true;
					Serch.SetActive(false);
					Serch2.SetActive(false);

					//targetPosition = GManager.instance.Player.transform.position;
					targetPosition = GManager.instance.Player.transform.position;

					distance = targetPosition - (Vector2)transform.position;
					//Debug.Log($"�m�肽���̂�{targetPosition.x}");
					direction = distance.x >= 0 ? 1 : -1;//���������A�܂���0�̎�1�B�����łȂ��Ƃ�-1�B����
					directionY = distance.y >= 0 ? 1 : -1;//�|�\����Ƃ��̃A�j���̔���	�ɂ��g������
					EscapeCheck();

					//TriggerJump();
				}

				else if (!isAggressive)
				{

					if (posiReset)
					{
						PositionReset();
					}

				}
			}

			//���̂ւ�ǂ����邩�Ȃ�

			WaitAttack();//���̂܂܂ł�������
			ArmorRecover();//���̂܂�

			//�g���K�[�ŌĂт܂��傤
			JumpController();
			Parry();
			Die();//�w���X�ɕς���
			MaterialControll();



		}
		public void ActionFire(int i, float random = 0.0f)
	{//�����_���ɓ���Ă��������Ǖ��ʂɓ���Ă�����
		i = i < 0 ? 0 : i;
		i = i > status.enemyFire.Count - 1 ? status.enemyFire.Count : i;

		if (!isStop && _movement.CurrentState != CharacterStates.MovementStates.Rolling)
		{
			waitCast += _controller.DeltaTime;
			if (waitCast >= status.enemyFire[i].castTime)
				if (random != 0)
				{
					firePosition.position.Set
						(firePosition.position.x + random, firePosition.position.y + random, firePosition.position.z);//�e������
				}
			Transform goFire = firePosition;

			for (int x = 0; x >= status.enemyFire[i].bulletNumber; x++)
			{
				Addressables.InstantiateAsync(status.enemyFire[i].effects, goFire.position, Quaternion.identity);//.Result;//�����ʒu��Player
			}
			//go.GetComponent<EnemyFireBullet>().ownwer = transform;

		}
	}




        public void Flip(float direction)
	{
		if (!isStop)
		{
			//	Debug.Log("�m��");
			// Switch the way the player is labelled as facing.

			// Multiply the player's x local scale by -1.
			Vector3 theScale = transform.localScale;
			theScale.x = direction * Mathf.Abs(theScale.x);
			transform.localScale = theScale;
		}
	}

	/// <summary>
	/// �A�j���̓s���㔽�Α����������Ƃ��̃��\�b�h
	/// </summary>
	public void AnimeFlip()
	{
			//_characterFly.SetHorizontalMove(1f);
			//���̂ւ�g���ĐU������s����

			Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		lastDirection = direction;
	}

	/// <summary>
	/// �ŏ��̈ʒu�ɖ߂�B
	/// </summary>
	public void PositionReset()
	{
		if (isMovable)
		{
				if(status.kind == EnemyStatus.KindofEnemy.Fly)
                {
					if (transform.position.x <= basePosition.x)
					{
						Flip(1);
						if (transform.position.y <= basePosition.y)
						{
							
							if ((transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)
								&& (transform.position.y >= basePosition.y - 5 && transform.position.y <= basePosition.y + 5))
							{
								_flying.SetHorizontalMove(0);
								_flying.SetVerticalMove(0);
								//�X�^�[�g���n�ʂ�������v�B�����Ă���n�̂�ɂ�Dogpile��^���Ă��
								
								posiReset = false;
								isRight = true;
								//transform.localScale = baseDirection;
							}
							else
							{
								_flying.SetHorizontalMove(1);
								//rb.AddForce(move(0, ;
								_flying.SetVerticalMove(1);
							}
						}
						else if (transform.position.y >= basePosition.y)
						{
							
							if ((transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)
								&& (transform.position.y >= basePosition.y - 5 && transform.position.y <= basePosition.y + 5))
							{
								_flying.SetHorizontalMove(0);
								_flying.SetVerticalMove(0);
								posiReset = false;
								isRight = true;
								//transform.localScale = baseDirection;
							}
							else
							{
								_flying.SetHorizontalMove(1);
								_flying.SetVerticalMove(-1);

							}
						}
					}
					else if (transform.position.x >= basePosition.x)
					{
						Flip(-1);
						
						if (transform.position.y <= basePosition.y)
						{
							
							if ((transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)
								&& (transform.position.y >= basePosition.y - 5 && transform.position.y <= basePosition.y + 5))
							{
								_flying.SetHorizontalMove(0);
								_flying.SetVerticalMove(0);
								posiReset = false;
								isRight = true;
								//transform.localScale = baseDirection;
							}
							else
							{
								_flying.SetHorizontalMove(-1);
								_flying.SetVerticalMove(1);

							}
						}
						else if (transform.position.y >= basePosition.y)
						{
							
							if ((transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)
								&& (transform.position.y >= basePosition.y - 5 && transform.position.y <= basePosition.y + 5))
							{
								_flying.SetHorizontalMove(0);
								_flying.SetVerticalMove(0);
								posiReset = false;
								isRight = true;
								//transform.localScale = baseDirection;
							}
							else
							{
								_flying.SetHorizontalMove(-1);
								_flying.SetVerticalMove(-1);
							}
						}
					}
				}
                else
                {
					if (transform.position.x <= basePosition.x)
					{
						
							Flip(1);
							if (transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)
							{
							_characterHorizontalMovement.SetHorizontalMove(0);
								posiReset = false;
								isRight = true;
								//Flip(baseDirection);
								//transform.localScale = baseDirection;
							}
							else
							{
							_characterHorizontalMovement.SetHorizontalMove(1);
							}
						}
					
					else if (transform.position.x >= basePosition.x)
					{

							Flip(-1);
							if (transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)
							{
						_characterHorizontalMovement.SetHorizontalMove(0);
						posiReset = false;
								isRight = true;
								//transform.localScale = baseDirection;
							}
							else
							{
							_characterHorizontalMovement.SetHorizontalMove(-1);
							}
					
				    }
                  }
                #region
                /*
		if (transform.position.x <= basePosition.x)
		{
			if (transform.position.y <= basePosition.y)
			{
				Flip(1);
				if (((transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)
					&& (transform.position.y >= basePosition.y - 5 && transform.position.y <= basePosition.y + 5) &&
					(status.kind == EnemyStatus.KindofEnemy.Fly)) || ((status.kind != EnemyStatus.KindofEnemy.Fly) &&
						(transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)))
				{
					//("Stand");
					//�X�^�[�g���n�ʂ�������v�B�����Ă���n�̂�ɂ�Dogpile��^���Ă��
					rb.velocity = Vector2.zero;
					posiReset = false;
					isRight = true;
					transform.localScale = baseDirection;
				}
				else
				{
					////(status.motionIndex[dogPile == null ? "����":"����"]);
					move.Set(status.addSpeed.x * ((dogPile == null ? status.patrolSpeed.x : status.combatSpeed.x) - rb.velocity.x), (status.addSpeed.y * (dogPile == null ? status.patrolSpeed.y : status.combatSpeed.y)) - rb.velocity.y);
					rb.AddForce(move);
					//rb.AddForce(move(0, ;
					//($"{(dogPile == null ? "Move" : "Dash")}");
				}
			}
			else if (transform.position.y >= basePosition.y)
			{
				Flip(1);
				if ((transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)
					&& (transform.position.y >= basePosition.y - 5 && transform.position.y <= basePosition.y + 5) &&
					(status.kind == EnemyStatus.KindofEnemy.Fly))
				{
					//("Stand");
					rb.velocity = Vector2.zero;
					posiReset = false;
					isRight = true;
					transform.localScale = baseDirection;
				}
				else
				{
					move.Set(status.addSpeed.x * ((dogPile == null ? status.patrolSpeed.x : status.combatSpeed.x) - rb.velocity.x), (status.addSpeed.y * (dogPile == null ? -status.patrolSpeed.y : -status.combatSpeed.y)) - rb.velocity.y);
					rb.AddForce(move);
					//rb.AddForce(move(0);
					//($"{(dogPile == null ? "Move" : "Dash")}");
				}
			}
		}
		else if (transform.position.x >= basePosition.x)
		{
			if (transform.position.y <= basePosition.y)
			{
				Flip(-1);
				if (((transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)
					&& (transform.position.y >= basePosition.y - 5 && transform.position.y <= basePosition.y + 5) &&
					(status.kind == EnemyStatus.KindofEnemy.Fly)) || ((status.kind != EnemyStatus.KindofEnemy.Fly) &&
					(transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)))
				{
					//("Stand");
					rb.velocity = Vector2.zero;
					posiReset = false;
					isRight = true;
					transform.localScale = baseDirection;
				}
				else
				{
					//($"{(dogPile == null ? "Move" : "Dash")}");
					move.Set(status.addSpeed.x * ((dogPile == null ? -status.patrolSpeed.x : -status.combatSpeed.x) - rb.velocity.x), (status.addSpeed.y * (dogPile == null ? status.patrolSpeed.y : status.combatSpeed.y)) - rb.velocity.y);
					rb.AddForce(move);
					//rb.AddForce(move(0, ();
				}
			}
			else if (transform.position.y >= basePosition.y)
			{
				Flip(-1);
				if (((transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)
					&& (transform.position.y >= basePosition.y - 5 && transform.position.y <= basePosition.y + 5) &&
					(status.kind == EnemyStatus.KindofEnemy.Fly)) || ((status.kind != EnemyStatus.KindofEnemy.Fly) &&
					(transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)))
				{
					//("Stand");
					rb.velocity = Vector2.zero;
					posiReset = false;
					isRight = true;
					transform.localScale = baseDirection;
				}
				else
				{
					//($"{(dogPile == null ? "Move" : "Dash")}");
					move.Set(status.addSpeed.x * ((dogPile == null ? -status.patrolSpeed.x : -status.combatSpeed.x) - rb.velocity.x), (status.addSpeed.y * (dogPile == null ? -status.patrolSpeed.y : -status.combatSpeed.y)) - rb.velocity.y);
					rb.AddForce(move);
					//rb.AddForce(move(0, );
				}
			}
		}
		*/
                #endregion
            }
        }
	/// <summary>
	/// �ҋ@���̏����s��
	/// </summary>
	public void PatrolMove()
	{

		if (!posiReset && isMovable)
		{
				if (status.kind != EnemyStatus.KindofEnemy.Fly)
				{



					if (transform.position.x <= startPosition.x + status.waitDistance.x && isRight)
					{
						////("Move");
						Flip(1);
						_characterHorizontalMovement.SetHorizontalMove(1);

					}
					else if (transform.position.x >= startPosition.x - status.waitDistance.x && !isRight)
					{
						Flip(-1);
						_characterHorizontalMovement.SetHorizontalMove(-1);
					}
					else
					{
						
						////////Debug.log("��������");
						waitTime += _controller.DeltaTime;
						_characterHorizontalMovement.SetHorizontalMove(0);
						if (waitTime >= status.waitRes)
						{
							isRight = !isRight;
							waitTime = 0.0f;
							//	//////Debug.log("��������");

						}
					}
				}
                else
                {
					if (transform.position.x <= startPosition.x + status.waitDistance.x && isRight)
					{
						////("Move");
						Flip(1);
						_flying.SetHorizontalMove(1);

					}
					else if (transform.position.x >= startPosition.x - status.waitDistance.x && !isRight)
					{
						Flip(-1);
						_flying.SetHorizontalMove(-1);
					}
					else
					{

						////////Debug.log("��������");
						waitTime += _controller.DeltaTime;
						_flying.SetHorizontalMove(0);
						if (waitTime >= status.waitRes)
						{
							isRight = !isRight;
							waitTime = 0.0f;
							//	//////Debug.log("��������");

						}
					}
				}
		}
	}

	/// <summary>
	/// �ҋ@�������
	/// �ҋ@�ƑI���Őς�
	/// </summary>
	public void PatrolFly()
	{
		if (!posiReset && isMovable)
		{

			if (transform.position.y <= startPosition.y + status.waitDistance.y && isUp)
			{
					_flying.SetVerticalMove(1);
			}
			else if (transform.position.y >= startPosition.y - status.waitDistance.y && !isUp)
			{
					_flying.SetVerticalMove(-1);
			}
			else
			{
				isUp = !isUp;
				_flying.SetVerticalMove(0);
			}
		}
	}

	/// <summary>
	/// �ҋ@����~���Ă���낫���U����������B�����̓p�g���[�����Ȃ�����waitTime�͎g���܂킵�ł���
	/// �p�g���[���ƑI���łǂ������ς�
	/// �Ȃɂ��Ɏ~�܂��Ă�n�G�Ƃ����\���ł���
	/// </summary>
	public void Wait()
	{
			if (!posiReset && isMovable)
			{
				if (status.kind != EnemyStatus.KindofEnemy.Fly)
				{
					if (dRb == null)
					{

						waitTime += _controller.DeltaTime;
						_characterHorizontalMovement.SetHorizontalMove(0);
						if (waitTime >= status.waitRes)
						{
							Flip(-Math.Abs(transform.localScale.x));//���]�����܁[��
							waitTime = 0.0f;
						}
					}
					if (dRb != null)
					{
						_characterHorizontalMovement.SetHorizontalMove(Mathf.Abs(dogPile.transform.localScale.x));
						Flip(Mathf.Abs(dogPile.transform.localScale.x));
					}
				}
                else
                {
					if (dRb == null)
					{

						waitTime += _controller.DeltaTime;
						_flying.SetHorizontalMove(0);
						if (waitTime >= status.waitRes)
						{
							Flip(-Math.Abs(transform.localScale.x));//���]�����܁[��
							waitTime = 0.0f;
						}
					}
					if (dRb != null)
					{
						_flying.SetHorizontalMove(Mathf.Abs(dogPile.transform.localScale.x));
						Flip(Mathf.Abs(dogPile.transform.localScale.x));
					}
				}
			}
	}

	/// <summary>
	/// //�v���C���[�����������m�F
	/// </summary>
	public void EscapeCheck()
	{


		if (Mathf.Abs(distance.x) >= status.escapeDistance.x || Mathf.Abs(distance.y) >= status.escapeDistance.y)
		{
			escapeTime += _controller.DeltaTime;
			if (escapeTime >= status.chaseRes)
			{
				Serch.SetActive(true);
				Serch2.SetActive(true);
				isAggressive = false;
					if(status.kind == EnemyStatus.KindofEnemy.Fly)
                    {
						_flying.FastFly(false,true);
						_flying.FastFly(true, true);
					}
                    else
                    {
						_characterRun.RunStop();
                    }
				ground = EnemyStatus.MoveState.wakeup;
				air = EnemyStatus.MoveState.wakeup;
			}
		}
		else
		{
			escapeTime = 0.0f;
		}
	}

	public void fallCheck()
	{
		if (transform.position.y < -30.0f)
		{

		}
	}//�������m�F�B�������Ă܂��g��

	/// <summary>
	/// �퓬���̋����̎���
	/// disIndex�ŋ����̎����̃p�^�[����I���ł����
	/// </summary>
	public void AgrMove(int disIndex = 0)
	{

		//if()
		stateJudge += _controller.DeltaTime;
		#region//���f
		if ((ground == EnemyStatus.MoveState.wakeup || stateJudge >= status.judgePace) && ground != EnemyStatus.MoveState.escape)
		//escape�����̓X�N���v�g���瓮����
		{
			if (attackComp && RandomValue(0, 100) <= atV.escapePercentage)
			{
				if (status.agrDistance[disIndex].x <= 30)
				{
					ground = EnemyStatus.MoveState.leaveWalk;
				}
				else if (Mathf.Abs((Mathf.Abs(distance.x) / status.agrDistance[disIndex].x)) >= 0.6)
				{
					ground = EnemyStatus.MoveState.leaveDash;

				}

			}
			//20�p�[�Z���g�̊m���Œ�~�ȊO��
			else if (((Mathf.Abs(distance.x) <= status.agrDistance[disIndex].x + status.adjust && Mathf.Abs(distance.x) >= status.agrDistance[disIndex].x - status.adjust) && RandomValue(0, 100) >= 40) || guardHit)
			{
					flipWaitTime = 1f;
				ground = EnemyStatus.MoveState.stay;
				//	flipWaitTime = 10;
			}
			else if (Mathf.Abs(distance.x) > status.agrDistance[disIndex].x)//�߂Â�������ˁH
			{
				if (Mathf.Abs(distance.x) <= status.walkDistance.x || isGuard)
				{
					ground = EnemyStatus.MoveState.accessWalk;
				}
				else
				{
					ground = EnemyStatus.MoveState.accessDash;
						_guard.GuardEnd();
					}
			}
			else if (Mathf.Abs(distance.x) < status.agrDistance[disIndex].x)//��������
			{
				//���������Ȃ�G�������܂܌���
				//�����Ȃ��|���Ƃ��͈ړ����x�[����
				if (Mathf.Abs((Mathf.Abs(distance.x) - status.agrDistance[disIndex].x)) <= status.walkDistance.x / 2 || isGuard)
				{
					ground = EnemyStatus.MoveState.leaveWalk;
				}
				else
				{
					ground = EnemyStatus.MoveState.leaveDash;
						_guard.GuardEnd();
				}
			}
			stateJudge = 0;
			attackComp = false;
				if (_movement.CurrentState != CharacterStates.MovementStates.Running)
				{
					if (ground == EnemyStatus.MoveState.accessDash || ground == EnemyStatus.MoveState.leaveDash)
					{
						_characterRun.RunStart();
					}
				}
				else
				{
					if (ground != EnemyStatus.MoveState.accessDash && ground != EnemyStatus.MoveState.leaveDash)
					{
						_characterRun.RunStop();
					}
				}

		}
		#endregion

		if (!isStop && isMovable)
		{
			if (ground == EnemyStatus.MoveState.stay)
			{
					//�o�g���t���b�v�̓X�e�C�������ɂ���
				BattleFlip(direction);
				_characterHorizontalMovement.SetHorizontalMove(0f);
				isReach = true;
					return;

					//Debug.Log($"�˂���{blowM.x}");

				
			}
			else if (ground == EnemyStatus.MoveState.accessWalk)
			{
				BattleFlip(direction);
				isReach = (Mathf.Abs(distance.x) - status.agrDistance[disIndex].x) <= status.walkDistance.x ? true : false;
					_characterHorizontalMovement.SetHorizontalMove(lastDirection);

			}
			else if (ground == EnemyStatus.MoveState.accessDash)
			{
				BattleFlip(direction);
				isReach = false;
				_characterHorizontalMovement.SetHorizontalMove(lastDirection);
					//Running�t���O�g�D���[�̎��̏���������
					_characterRun.RunStart();
					if (Mathf.Abs(distance.x) <= status.agrDistance[disIndex].x + status.adjust && Mathf.Abs(distance.x) >= status.agrDistance[disIndex].x - status.adjust)
				{
					ground = EnemyStatus.MoveState.accessWalk;
					stateJudge = 0.0f;
						_characterRun.RunStop();
				}
			}
			else if (ground == EnemyStatus.MoveState.leaveWalk)//��������
			{
				//�ߋ����̏ꍇ�����͈͂��_�b�V���ŗ����̂��傫��
				//���������Ȃ�G�������܂܌���
				//�����Ȃ��|���Ƃ��͈ړ����x�[����
				BattleFlip(direction);
				isReach = true;

			    _characterHorizontalMovement.SetHorizontalMove(-lastDirection);
		    }
			else if (ground == EnemyStatus.MoveState.leaveDash)
			{
					_characterRun.RunStart();
					BattleFlip(-direction);
				isReach = false;
				_characterHorizontalMovement.SetHorizontalMove(-lastDirection);
			}
		}
	}

	/// <summary>
	/// ����ԃ^�C�v�̃G�l�~�[�ɐ퓬���悹��B�����
	/// </summary>
	public void AgrFly(int disIndex = 0,int stMove = 0)
	{
		stateJudge += _controller.DeltaTime;
		#region//���f
		if ((ground == EnemyStatus.MoveState.wakeup || stateJudge >= status.judgePace) && ground != EnemyStatus.MoveState.escape && _condition.CurrentState != CharacterStates.CharacterConditions.Stunned)
		//escape�����̓X�N���v�g���瓮����
		{

			bool isSet = false;

			//���̏ꍇ�̓p�[�Z���e�[�W�͕�����̂Ɏg����
			//�U���I���㓦����
			if(stMove == 0 || attackComp) 
				{
					if (attackComp && atV.escapePercentage > 0)
					{
						if (atV.escapePercentage % 2 == 0)
						{
							ground = EnemyStatus.MoveState.leaveWalk;

							air = atV.escapePercentage == 2 ? EnemyStatus.MoveState.leaveWalk : EnemyStatus.MoveState.stay;

						}
						else
						{
							ground = EnemyStatus.MoveState.leaveDash;
							air = atV.escapePercentage == 1 ? EnemyStatus.MoveState.leaveDash : EnemyStatus.MoveState.stay;
						}
						isSet = true;
					}
					//20�p�[�Z���g�̊m���Œ�~�ȊO��
					else if ((Mathf.Abs(distance.x) <= status.agrDistance[disIndex].x + status.adjust && Mathf.Abs(distance.x) >= status.agrDistance[disIndex].x - status.adjust) || guardHit)
					{

						ground = EnemyStatus.MoveState.stay;
						//	flipWaitTime = 10;
					}
					else if (Mathf.Abs(distance.x) > status.agrDistance[disIndex].x)//�߂Â�������ˁH
					{
						if (Mathf.Abs(distance.x) <= status.walkDistance.x || isGuard)
						{
							ground = EnemyStatus.MoveState.accessWalk;
						}
						else
						{
							ground = EnemyStatus.MoveState.accessDash;
						}
					}
					else if (Mathf.Abs(distance.x) < status.agrDistance[disIndex].x)//��������
					{
						//���������Ȃ�G�������܂܌���
						//�����Ȃ��|���Ƃ��͈ړ����x�[����
						if (Mathf.Abs((Mathf.Abs(distance.x) - status.agrDistance[disIndex].x)) <= status.walkDistance.x / 2 || isGuard)
						{
							ground = EnemyStatus.MoveState.leaveWalk;
						}
						else
						{
							ground = EnemyStatus.MoveState.leaveDash;
						}
					}
			}
			else if(stMove != 0)
                {
					ground = EnemyStatus.MoveState.straight;
					
				}

			if(ground == EnemyStatus.MoveState.leaveDash || ground == EnemyStatus.MoveState.accessDash || ground == EnemyStatus.MoveState.straight)
                {
					_flying.FastFly(false, false);
				}
                else
                {
					_flying.FastFly(false, true);
				}
			//�c
			if (!isSet)
			{
				//�����܂Ƃ�
				//�����ɂ���Ƃ�     lvD
				//����ɂ��鎞       acD
				//���傢���ɂ���Ƃ� lvW
				//���傢��ɂ���Ƃ� acW
				//�����Ȃ��Ă����Ƃ� stay

				//���݂̃v���C���[���x�ƍ��킹�ĖڕW���x�����肾���B
				float targetHigh = -distance.y;

				if ((targetHigh <= status.agrDistance[disIndex].y + status.adjust && targetHigh >= status.agrDistance[disIndex].y - status.adjust) || guardHit)
				{

					air = EnemyStatus.MoveState.stay;
						_flying.FastFly(true, true);
						//	flipWaitTime = 10;
					}
				else if (targetHigh <= status.agrDistance[disIndex].y)//�߂Â�������ˁH
				{
					if (status.agrDistance[disIndex].y - targetHigh <= status.walkDistance.y || isGuard)
					{
						air = EnemyStatus.MoveState.accessWalk;
							_flying.FastFly(true,true);
						}
					else
					{
						air = EnemyStatus.MoveState.accessDash;
							_flying.FastFly(true, false);
						}
				}
				else if (targetHigh > status.agrDistance[disIndex].y)//��������
				{
					//���������Ȃ�G�������܂܌���
					//�����Ȃ��|���Ƃ��͈ړ����x�[����
					if (targetHigh - status.agrDistance[disIndex].y <= status.walkDistance.y || isGuard)
					{
						air = EnemyStatus.MoveState.leaveWalk;
							_flying.FastFly(true, true);
						}
					else
					{
						air = EnemyStatus.MoveState.leaveDash;
							_flying.FastFly(true, false);
					}
				}
			}
			stateJudge = 0;
			attackComp = false;
			//	Debug.Log($"��{air}��{ground}");
			//	Debug.Log($"��{distance.y}");
			//��Ԑ؂�ւ����ɂ͐U������悤��
			flipWaitTime = 3;
		}
		#endregion




		if (isMovable)
		{

			VerTicalMoveJudge();


				//ground��air�̑g�ݍ��킹�ŌĂяo�����̕ς��悤

			if (ground == EnemyStatus.MoveState.stay)
			{
					BattleFlip(direction);
					_flying.SetHorizontalMove(0);
					//move.x = 0;
					//_flying.SetVerticalMove(d);
					//rb.velocity = move;
					isReach = true;

			}
			else if (ground == EnemyStatus.MoveState.accessWalk)
			{
				BattleFlip(direction);
				isReach = (Mathf.Abs(distance.x) - status.agrDistance[disIndex].x) <= status.walkDistance.x ? true : false;
				_flying.SetHorizontalMove(lastDirection);

			}
			else if (ground == EnemyStatus.MoveState.accessDash)
			{
				BattleFlip(direction);
				isReach = false;
					_flying.SetHorizontalMove(lastDirection);
					if (Mathf.Abs(distance.x) <= status.agrDistance[disIndex].x + status.adjust && Mathf.Abs(distance.x) >= status.agrDistance[disIndex].x - status.adjust)
				{
					ground = EnemyStatus.MoveState.accessWalk;
					stateJudge = 0.0f;
				}
			}
			else if (ground == EnemyStatus.MoveState.leaveWalk)//��������
			{
				//�ߋ����̏ꍇ�����͈͂��_�b�V���ŗ����̂��傫��
				//���������Ȃ�G�������܂܌���
				//�����Ȃ��|���Ƃ��͈ړ����x�[����
				BattleFlip(-direction);
				isReach = true;
				_flying.SetHorizontalMove(-lastDirection);

			}
			else if (ground == EnemyStatus.MoveState.leaveDash)
			{

				BattleFlip(-direction);
				isReach = false;
				_flying.SetHorizontalMove(-lastDirection);
			}
			else if (ground == EnemyStatus.MoveState.straight)
            {
					if(stMove == 1)
                    {
						Flip(stMove);
						isReach = false;
						_flying.SetHorizontalMove(stMove);
					}
                    else
                    {
						Flip(-1);
						isReach = false;
						_flying.SetHorizontalMove(-1);
					}
            }
		}
            #region
            /*		float judgeDistance = transform.position.y - targetPosition.y;

					if (!isStop && !nowJump && !_movement.CurrentState != CharacterStates.MovementStates.Rolling && _condition.CurrentState != CharacterStates.CharacterConditions.Stunned &&_condition.CurrentState != CharacterStates.CharacterConditions.Dead)
					{
						//("Move");
						if (judgeDistance�@== status.agrDistance[disIndex].y)//��Ƃ�̔��e�Ȃ�䖝
						{
							move.Set(rb.velocity.x,0);
							rb.velocity = move;
						}
						else if (judgeDistance < status.agrDistance[disIndex].y)//�����͂Ȃ�
						{
							if (judgeDistance >= (status.agrDistance[disIndex].y - status.walkDistance.y))
							{
								if (Mathf.Abs(rb.velocity.y) >=  status.patrolSpeed.y)
								{
									move.Set(0,-status.addSpeed.y);
									rb.AddForce(move);
								}
								else if (Mathf.Abs(rb.velocity.y) <= status.patrolSpeed.y)
								{
									move.Set(0, status.addSpeed.y);
									rb.AddForce(move);
								}
							}
							else
							{
								if (Mathf.Abs(rb.velocity.y) >= status.combatSpeed.y)
								{
									move.Set(0, -status.addSpeed.y);
									rb.AddForce(move);
								}
								else if (Mathf.Abs(rb.velocity.y) <= status.combatSpeed.y)
								{
									move.Set(0, status.addSpeed.y);
									rb.AddForce(move);
								}
							}
						}
						else if (judgeDistance > status.agrDistance[disIndex].y)//�����߂Â�
						{
							if (judgeDistance <= (status.agrDistance[disIndex].y + status.walkDistance.y))
							{
								if (Mathf.Abs(rb.velocity.y) >= status.patrolSpeed.y)
								{
									move.Set(0, status.addSpeed.y);
									rb.AddForce(move);
								}
								else if (Mathf.Abs(rb.velocity.y) <= status.patrolSpeed.y)
								{
									move.Set(0, -status.addSpeed.y);
									rb.AddForce(move);
								}
							}
							else
							{
								if (Mathf.Abs(rb.velocity.y) >= status.combatSpeed.y)
								{
									move.Set(0, status.addSpeed.y);
									rb.AddForce(move);
								}
								if (Mathf.Abs(rb.velocity.y) <= status.combatSpeed.y)
								{
									move.Set(0, -status.addSpeed.y);
									rb.AddForce(move);
								}
							}
						}
					}*/
            #endregion
        }
        /// <summary>
        /// ����B�����w����\�����퓬���Ɍ���direction�őO���B-direction�Ō���
        /// </summary>
        /// <param name="direction"></param>
        public void Avoid(float direction)
	{
		if (!isMovable)
		{
				_rolling.StartRoll();
		}

	}





	/// <summary>
	///	�n��L�����p�̃W�����v�B�W�����v��Ԃ͎��������B�W�����v�L�����Z���ƃZ�b�g
	/// </summary>	
	public void JumpAct(float jumpMove, float jumpSpeed)
	{

			if (!isStop && _movement.CurrentState != CharacterStates.MovementStates.Rolling && _condition.CurrentState != CharacterStates.CharacterConditions.Stunned)
			{
				if (status.kind != EnemyStatus.KindofEnemy.Fly)
				{
                    if (_controller.State.IsGrounded)
                    {
						_jump.JumpStart();
                    }
				}
                else
                {
					_jump.JumpStart();
				}
			}
	}

		/// <summary>
		/// �g���K�[�ŌĂԂ悤��
		/// </summary>
		/// <param name="isVertical"></param>
	public void JumpController()
	{
			if (!_controller.State.IsGrounded)
				{
				if (!isVertical)
				{
					_horizontalInput = Mathf.Sign(transform.localScale.x);
				}
				if (_jump.JumpEnableJudge() == true)
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
			if (!isStop && _movement.CurrentState != CharacterStates.MovementStates.Rolling && _condition.CurrentState != CharacterStates.CharacterConditions.Stunned && _controller.State.IsGrounded)
			{
				//////Debug.log($"�W�����v�g���K�[{jumpTrigger}");
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
		return UnityEngine.Random.Range(X, Y + 1);

	}

	/// <summary>
	/// �A�j���C�x���g�ƍU����ɌĂяo���B�X�[�p�[�A�[�}�[��Ԃ𔽓]������
	/// </summary>
	public void EnemySuperAmor()
	{
			if (_movement.CurrentState == CharacterStates.MovementStates.Attack)
			{
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
			}
            else
            {
				_movement.ChangeState(CharacterStates.MovementStates.Attack);
            }
	}
	/// <summary>
	/// �A�[�}�[�����Z�b�g
	/// </summary>
	public void ArmorReset()
	{
		nowArmor = status.Armor;
	}

	public void ArmorRecover()
	{
		//	Debug.Log($"���̃A�[�}�[{nowArmor}");
		if (_condition.CurrentState != CharacterStates.CharacterConditions.Stunned && !isDamage && _movement.CurrentState != CharacterStates.MovementStates.Attack)
		{
			recoverTime += _controller.DeltaTime;
			if (recoverTime >= 15 || nowArmor > status.Armor)
			{
				ArmorReset();
				recoverTime = 0.0f;
				lastArmor = 1;
				//	lastArmor = nowArmor; 
			}
			else if (nowArmor < status.Armor && recoverTime >= 3 * lastArmor)
			{
				//recoverTime = 0.0f;
				nowArmor += status.Armor / 6;
				lastArmor++;
			}
		}
		else
		{
			recoverTime = 0.0f;
			lastArmor = 1;
			isDamage = false;
		}

	}


	/// <summary>
	/// �A�[�}�[�l�ɉ����ăC�x���g�Ƃ΂�
	/// </summary>
	public MyWakeUp.StunnType ArmorControll(float shock, bool isDown, bool isBack)
	{
			MyWakeUp.StunnType result = 0;

			if (_movement.CurrentState != CharacterStates.MovementStates.Attack)
			{
				atV.aditionalArmor = 0;
			}

			if (!isBack && _movement.CurrentState == CharacterStates.MovementStates.Guard)
            {
				_guard.GuardHit();
				nowArmor -= (GManager.instance.equipWeapon.shock * 3f) * ((100 - status.guardPower) / 100);
			}
            else
            {
				if (atV.aditionalArmor ==0)
				{
					nowArmor -= shock;

				}
				else
				{
						nowArmor -= (shock - atV.aditionalArmor) < 0 ? 0 : (shock - atV.aditionalArmor);
						atV.aditionalArmor = (atV.aditionalArmor - shock) < 0 ? 0 : atV.aditionalArmor - shock;

				}
			}

		if (nowArmor <= 0)
		{
                if (isDown)
                {
					result = (MyWakeUp.StunnType.Down);
                }
			    else
                {

					if (guardHit)
                    {
						result = MyWakeUp.StunnType.GuardBreake;
					}
					//�p���B�͕ʔ���
					else
					{ 
						result = MyWakeUp.StunnType.Falter;
                    }

				}
				isAtEnable = false;
		}
            else
            {
				result = MyWakeUp.StunnType.notStunned;
            }	


			return result;
		}



	/// <summary>
	/// �U�����͂�����m�b�N�o�b�N����B
	/// </summary>
	public void Parry()
	{
			_wakeup.StartStunn(MyWakeUp.StunnType.Parried);
		}



	protected virtual void OnTriggerEnter2D(Collider2D collision)
	{



				if (isAggressive && collision.tag == EnemyManager.instance.JumpTag && _controller.State.IsGrounded)
			{
				if (collision.gameObject.GetComponent<JumpTrigger>().jumpDirection == transform.localScale.x)
				{

					//�X�e�[�^�X�ŃW�����v�^�O�ݒ肵�Ȃ���΃W�����v�ł��Ȃ��G������
					jumpTrigger = true;
				}
			}
		
	}
	protected virtual void OnTriggerStay2D(Collider2D collision)
	{
			if (_condition.CurrentState != CharacterStates.CharacterConditions.Dead)
			{

				if (isAggressive && collision.tag == EnemyManager.instance.JumpTag && _controller.State.IsGrounded)
				{
					if (collision.gameObject.GetComponent<JumpTrigger>().jumpDirection == transform.localScale.x)
					{

						//�X�e�[�^�X�ŃW�����v�^�O�ݒ肵�Ȃ���΃W�����v�ł��Ȃ��G������
						jumpTrigger = true;
					}
				}
			}
	}





	/*	/// <summary>
		/// �v���C���[�̖��@�ɂ��U��
		/// </summary>
		public void MagicBlow(Collider2D c)
		{

			rb.AddForce(move(GManager.instance.equipWeapon.blowPower.x * -direction, GManager.instance.equipWeapon.blowPower.y));
		}*/

	public void HPReset()
	{
		_health.SetHealth((int)status.maxHp,this.gameObject);
	}
	public void Die()
	{
		if (_condition.CurrentState == CharacterStates.CharacterConditions.Dead)
		{

			if (_condition.CurrentState != CharacterStates.CharacterConditions.Stunned && blowDown)
			{
				if (!isAnimeStart)
				{
					rb.velocity = Vector2.zero;
					//("DDie");
					isAnimeStart = true;
				}
				else
				{
					rb.velocity = Vector2.zero;
					if (!CheckEnd("DDie"))
					{
						SManager.instance.EnemyDeath(this.gameObject);
						if (SManager.instance.target == this.gameObject)
						{
							SManager.instance.target = null;
							TargetEffectCon(1);
						}
						Destroy(this.gameObject);
					}

				}
			}

			else if (_condition.CurrentState != CharacterStates.CharacterConditions.Stunned && !blowDown)
			{

				if (!isAnimeStart)
				{

					//("NDie");
					isAnimeStart = true;
				}
				else
				{
					if (!CheckEnd("NDie"))
					{
						SManager.instance.EnemyDeath(this.gameObject);
						if (SManager.instance.target == this.gameObject)
						{
							SManager.instance.target = null;
							TargetEffectCon(1);
						}
						Destroy(this.gameObject);
					}

				}

			}
		}
	}

	/// <summary>
	/// �U���O�̏�����
	/// </summary>
	public void AttackPrepare()
	{
		//_movement.CurrentState != CharacterStates.MovementStates.Attack = true;
		atV.coolTime = status.atValue[attackNumber].coolTime;
		atV.isBlow = status.atValue[attackNumber].isBlow;
		atV.mValue = status.atValue[attackNumber].mValue;
		atV.aditionalArmor = status.atValue[attackNumber].aditionalArmor;
		atV.isLight = status.atValue[attackNumber].isLight;
		atV.disParry = status.atValue[attackNumber].disParry;
		atV.blowPower = status.atValue[attackNumber].blowPower;
		atV.shock = status.atValue[attackNumber].shock;
		atV.type = status.atValue[attackNumber].type;
		atV.isCombo = status.atValue[attackNumber].isCombo;
		atV.escapePercentage = status.atValue[attackNumber].escapePercentage;
		atV.parryResist = status.atValue[attackNumber].parryResist;
		atV.attackEffect = status.atValue[attackNumber].attackEffect;
	}


	/// <summary>
	/// �U��
	/// ���C�v��
	/// isShoot = true�̎��̏�������H�B����Ȃ��H�܂肽�܂������Ă�������
	/// �����@�[�X�ŋt����
	/// </summary>
	public void Attack(bool select = false, int number = 0, bool reverse = false)
	{


		if (isAtEnable && isMovable)
		{
			if (!reverse)
			{
				Flip(direction);
			}
			else
			{
				Flip(-direction);
			}

			if (select)
			{
				attackNumber = number;
			}

			AttackPrepare();
			_attack.AttackTrigger(number);
			
			isAtEnable = false;
			attackComp = true;
		}
	}

	/// <summary>
	/// �ǂ̃��[�V��������邩�ǂ���
	/// </summary>
	/// <param name="number"></param>
	public void SetAttackNumber(int number)
	{
		attackNumber = status.serectableNumber[number];
	}



	/// <summary>
	/// �N�[���^�C���̊Ԏ��̍U����҂�
	/// </summary>
	public void WaitAttack()
	{
		if (_movement.CurrentState == CharacterStates.MovementStates.Attack)
		{
			if (!CheckEnd(status.attackName[attackNumber]) || _condition.CurrentState == CharacterStates.CharacterConditions.Stunned)
			{
				if (_condition.CurrentState != CharacterStates.CharacterConditions.Stunned)
				{
					Flip(direction);
				}
				//	isAtEnable = true;����N�[���^�C���ŊǗ����悤��
				 _movement.ChangeState(CharacterStates.MovementStates.Idle);
					atV.aditionalArmor = 0;
			}
		}

		else if (_movement.CurrentState != CharacterStates.MovementStates.Attack)
		{
			if (!isAtEnable && !atV.isCombo && _condition.CurrentState != CharacterStates.CharacterConditions.Stunned)
			{
				stayTime += _controller.DeltaTime;
				if (stayTime >= atV.coolTime)
				{
					isAtEnable = true;
					stayTime = 0.0f;
				}
			}
			if (!isAtEnable && atV.isCombo && _condition.CurrentState != CharacterStates.CharacterConditions.Stunned)
			{
				isAtEnable = true;
				attackNumber++;
				//AttackPrepare();
				Attack();
			}
		}
	}

	//	bool CheckEnd(string _currentStateName)
	//{
	//	return sAni.IsPlaying(_currentStateName);
	//}
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

	/// <summary>
	/// �G���`�����̓G���`�����g�^�C�v���Q��
	/// </summary>
	/// <param name="damageType"></param>
	public void DamageSound(byte damageType,bool heavy)
	{
		if (damageType == 1)
		{
			GManager.instance.PlaySound("SlashDamage", transform.position);
		}
		else if (damageType == 2)
		{
			GManager.instance.PlaySound("StabDamage", transform.position);
		}
		else if (damageType == 4)
		{
			if (!heavy)
			{
				//	Debug.Log("�`�L��");
				GManager.instance.PlaySound("StrikeDamage", transform.position);
			}
			else
			{
				GManager.instance.PlaySound("HeavyStrikeDamage", transform.position);
				heavy = false;
			}
		}
		else if (damageType == 8)
		{
			GManager.instance.PlaySound("HolyDamage", transform.position);
		}
		else if (damageType == 16)
		{
			GManager.instance.PlaySound("DarkDamage", transform.position);
		}
		else if (damageType == 32)
		{


			GManager.instance.PlaySound("FireDamage", transform.position);
		}
		else if (damageType == 64)
		{
			Debug.Log("sdfgg");
			GManager.instance.PlaySound("ThunderDamage", transform.position);
		}
	}


	public void FlySound(int fly)
	{
		if (fly == 1 && !flyNow)
		{
			GManager.instance.PlaySound(status.walkSound, transform.position);
			flyNow = true;
		}
		else if (fly == 0)
		{
			flyNow = false;
			GManager.instance.StopSound(status.walkSound, 0.5f);
		}
	}

	public void MoveSound(int type)
	{
		if (status.kind != EnemyStatus.KindofEnemy.Fly)
		{
			if (!isAggressive)
			{
				GManager.instance.PlaySound(status.footStepSound, transform.position);
			}
			else
			{
				if (ground == EnemyStatus.MoveState.accessDash || ground == EnemyStatus.MoveState.leaveDash)
				{
					if (status.isMetal)
					{
						GManager.instance.PlaySound(MyCode.SoundManager.instance.armorFootSound[type], transform.position);
					}
					else
					{
						GManager.instance.PlaySound(MyCode.SoundManager.instance.bareFootSound[type], transform.position);
					}
				}
				else
				{
					if (status.isMetal)
					{
						GManager.instance.PlaySound(MyCode.SoundManager.instance.armorWalkSound[type], transform.position);
					}
					else
					{
						GManager.instance.PlaySound(MyCode.SoundManager.instance.bareWalkSound[type], transform.position);
					}
				}
			}
			if (isWater)
			{
				//���̑���
			}
		}

	}

	public void ActionSound(string useName)
	{
		GManager.instance.PlaySound(useName, transform.position);
	}

	public void SwingSound(int type = 0)
	{
		//�a���h�ˑŌ����Ǘ�
		if (atV.type == EnemyStatus.AttackType.Stab)
		{
			GManager.instance.PlaySound(MyCode.SoundManager.instance.stabSound[type], transform.position);
		}
		else
		{
			GManager.instance.PlaySound(MyCode.SoundManager.instance.swingSound[type], transform.position);
		}

		//�G���`�����Ă�ꍇ��

	}
	public void FistSound(int type = 0)
	{
		GManager.instance.PlaySound(MyCode.SoundManager.instance.fistSound[type], transform.position);

		//�G���`�����Ă�ꍇ��

	}

	public void DisParriableAct()
	{
		GManager.instance.PlaySound("DisenableParry", transform.position);
		Vector3 Scale = Vector3.zero;
		if (transform.localScale.x > 0)
		{
			Scale.Set(status.disparriableScale.x, status.disparriableScale.y, status.disparriableScale.z);
		}
		else
		{
			Scale.Set(status.disparriableScale.x * -1f, status.disparriableScale.y, status.disparriableScale.z);
		}

		dispary.transform.localScale = Scale;
		dispary.transform.rotation = Quaternion.Euler(-10, 0, 0);
		Addressables.InstantiateAsync("DisParriableEffect", dispary.transform);
	}
	public void attackEffect()
	{
		// Debug.Log($"�A�C�C�C�C�C�C{atEffect.SubObjectName}");
		Addressables.InstantiateAsync(atV.attackEffect, effectController.transform);
	}

	public void BattleFlip(float direction)
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
	/// FixedUpdate�ɒu���ăK�[�h������
	/// �����̓K�[�h����m��
	/// </summary>
	protected void GroundGuardAct(float guardProballity)
	{
		//�������K�[�h�ǂ�����H
		isGuard = ground == EnemyStatus.MoveState.stay && _condition.CurrentState != CharacterStates.CharacterConditions.Stunned && _movement.CurrentState != CharacterStates.MovementStates.Attack ? true : false;
		if (_movement.CurrentState != CharacterStates.MovementStates.Attack && _condition.CurrentState != CharacterStates.CharacterConditions.Stunned && guardJudge && !isGuard)
		{
			if ((Mathf.Sign(transform.localScale.x) != Mathf.Sign(GManager.instance.Player.transform.localScale.x)))
			{
				if (RandomValue(0, 100) >= (100 - guardProballity))
				{
					//flipWaitTime = 100;
					ground = EnemyStatus.MoveState.stay;
					//	Debug.Log("��");
				}
				guardJudge = false;
			}
		}

	}

	protected void VerTicalMoveJudge()
	{
		if (air == EnemyStatus.MoveState.stay)
		{

				_flying.SetVerticalMove(0);
				//move.Set(0, status.addSpeed.y * (0 - rb.velocity.y));
				//move.y = 0;
				_flying.FastFly(false, true);
			}
		else if (air == EnemyStatus.MoveState.accessDash)
		{
				_flying.SetVerticalMove(directionY);
				//move.Set(0, status.addSpeed.y * (status.combatSpeed.y - rb.velocity.y));
				//move.y = ;
				_flying.FastFly(true, false);
				
			}
		else if (air == EnemyStatus.MoveState.accessWalk)

		{
				_flying.SetVerticalMove(directionY);
				_flying.FastFly(true, true);
			}
		else if (air == EnemyStatus.MoveState.leaveDash)
		{
				_flying.SetVerticalMove(-directionY);
				//move.Set(0, status.addSpeed.y * (-status.combatSpeed.y - rb.velocity.y));
				_flying.FastFly(true, false);
			//move.y = ;
		}
		else if (air == EnemyStatus.MoveState.leaveWalk)

		{
				_flying.SetVerticalMove(-directionY);
				//move.y = ;
				_flying.FastFly(true, true);
			}
	}

	protected void MaterialControll()
	{
		if (materialSet > 0)
		{


			//�e�}�e���A���̏�������
			//	Debug.Log($"{parentMatt.material}");
			if (materialSet == 1)
			{

				GetAllChildren(spriteList[mattControllNum], ref mattTrans);
				//	await UniTask.WaitForFixedUpdate();

				materialSet++;
			}
			if (materialSet > 1)
			{
				//Debug.Log($"Hello{controllTarget[2].material.name}");
				jumpTime += _controller.DeltaTime;
				float test = Mathf.Lerp(0f, 1, jumpTime / 2);
				for (int i = 0; i <= controllTarget.Count - 1; i++)
				{


					controllTarget[i].material.SetFloat("_FadeAmount", test);

				}

				/*	for (int i = 0; i >= mattTrans.Count; i++)
					{
						Debug.Log("Hi");
						//	controllTarget[i].CopyPropertiesFromMaterial(parentMatt);
					//	SpriteRenderer sr = mattTrans[i].GetComponent<SpriteRenderer>();
					 Debug.Log($"�}�e���A����{controllTarget[i].name}{parentMatt.name}");//
						controllTarget[i].material.CopyPropertiesFromMaterial(parentMatt.material);
					/*	if(sr != null)
						{

							//sr.material = parentMatt;
						}
					}*/
			}

		}

	}
	/// <summary>
	/// ���ʂƂ��̃}�e���A������n���p���\�b�h�B���X�g�̃[���͂悭�g����ɂ���̂�����
	/// ��b�ŏ�����
	/// </summary>
	/// <param name="controllNumber"></param>
	protected void MattControllStart(int controllNumber = 0)
	{
		materialSet = 1;

		if (controllNumber != mattControllNum)
		{
			jumpTime = 0;
			controllTarget = null;
			controllTarget = new List<Renderer>();
			mattControllNum = controllNumber;
			mattTrans = null;
		}
	}


	/*	private void GetAllChildren(Transform parent)
		{
			int x = 0;
			Debug.Log($"�m�F{parent.name}");
			for (int i = 0; i >= parent.childCount; i++)
			{
				Debug.Log("��������");
				Transform child = parent.GetChild(i);
				SpriteRenderer matt = child.GetComponent<SpriteRenderer>();
				if (matt != null)
				{
					controllTarget.Add(matt.material);
				}
				//����Ɏq�I�u�W�F�N�g�T��
				GetAllChildren(child);
				x++;
			}

		}*/

	private void GetAllChildren(Transform parent, ref List<Transform> transforms, bool Weapon = false)
	{
		foreach (Transform child in parent)
		{
			transforms.Add(child);
			GetAllChildren(child, ref transforms, true);
			Renderer sr = child.GetComponent<Renderer>();
			if (sr != null)
			{
				//Debug.Log(sr.name);
				controllTarget.Add(sr);
			}
		}
		if (!Weapon)
		{
			Transform die = transform.Find("Attack");
			if (die != null)
			{
				GetAllChildren(die, ref transforms, true);
			}
			die = transform.Find("Guard");
			if (die != null)
			{
				GetAllChildren(die, ref transforms, true);
			}

		}

	}


	public void TargetEffectCon(int state = 0)
	{
		if (state == 0)
		{
			td.gameObject.SetActive(true);
			td.color = EnemyManager.instance.stateClor[0];
		}
		else if (state == 1)
		{
			td.gameObject.SetActive(false);
		}
		else if (state == 2)
		{
			td.gameObject.SetActive(true);
			td.color = EnemyManager.instance.stateClor[1];
		}
		else
		{
			// td.enabled = true;
			td.color = EnemyManager.instance.stateClor[0];
		}
	}

	 protected void ParameterSet(EnemyStatus status)
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
			if (status.kind != EnemyStatus.KindofEnemy.Fly)
			{
				GravitySet(0);
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
				_characterHorizontalMovement.WalkSpeed = status.patrolSpeed.x;
				if(_characterRun != null)
                {
					_characterRun.RunSpeed = status.combatSpeed.x;
                }

			}
			else
            {
				GravitySet(status.firstGravity);
				_flying.nFlySpeed.Set(status.patrolSpeed.x,status.patrolSpeed.y);
				_flying.FastSpeed.Set(status.combatSpeed.x, status.combatSpeed.y);
            }

			if(_rolling != null)
            {
				#region
				/*
				         /// ���b�]����
		[Tooltip("���[�����O����")]
        public float RollDuration = 0.5f;
        /// the speed of the roll (a multiplier of the regular walk speed)
        [Tooltip("�]���鑬�����ʏ�̕��������̉��{��")]
        public float RollSpeed = 3f;
        /// if this is true, horizontal input won't be read, and the character won't be able to change direction during a roll
        [Tooltip("true�̏ꍇ�A���������̓��͓͂ǂݍ��܂ꂸ�A���[�����ɕ�����ς��邱�Ƃ͂ł��܂���B")]
        public bool BlockHorizontalInput = false;
        /// if this is true, no damage will be applied during the roll, and the character will be able to go through enemies
        /// //���̃p�����[�^�[��������鏈��������Ζ��G�������킩��
        [Tooltip("true�̏ꍇ�A���[�����Ƀ_���[�W���^����ꂸ�A�G���X���[�ł���悤�ɂȂ�܂��B")]
        public bool PreventDamageCollisionsDuringRoll = false;

        //����
        [Header("Direction")]

        /// the roll's aim properties
        [Tooltip("the roll's aim properties")]
        public MMAim Aim;
        /// the minimum amount of input required to apply a direction to the roll
        [Tooltip(" ���[���ɕ�����^���邽�߂ɕK�v�ȍŏ����̓��͗�")]
        public float MinimumInputThreshold = 0.1f;
        /// if this is true, the character will flip when rolling and facing the roll's opposite direction
        [Tooltip("���ꂪ�^�Ȃ�A�L�����N�^�[�̓��[�����ɔ��]���A���[���̔��Α��������܂��B")]
        public bool FlipCharacterIfNeeded = true;

        //����R���[�`���Ȃ�
        public enum SuccessiveRollsResetMethods { Grounded, Time }

        [Header("Cooldown")]
        /// the duration of the cooldown between 2 rolls (in seconds)
        [Tooltip("���̃��[�����O�܂łɕK�v�Ȏ���")]
        public float RollCooldown = 1f;

        [Header("Uses")]
        /// whether or not rolls can be performed infinitely
        [Tooltip("�����Ƀ��[�����O�ł��邩")]
        public bool LimitedRolls = false;
        /// the amount of successive rolls a character can perform, only if rolls are not infinite
        [Tooltip("the amount of successive rolls a character can perform, only if rolls are not infinite")]
        [MMCondition("LimitedRolls", true)]
        public int SuccessiveRollsAmount = 1;
        /// the amount of rollss left (runtime value only), only if rolls are not infinite
        [Tooltip("���[�����O�̎c���")]
        [MMCondition("LimitedRolls", true)]
        [MMReadOnly]
        public int SuccessiveRollsLeft = 1;
        /// when in time reset mode, the duration, in seconds, after which the amount of rolls left gets reset, only if rolls are not infinite
        [Tooltip("when in time reset mode, the duration, in seconds, after which the amount of rolls left gets reset, only if rolls are not infinite")]
        [MMCondition("LimitedRolls", true)]
        public float SuccessiveRollsResetDuration = 2f;

				*/
				#endregion

				_rolling.RollDuration = status.avoidRes;
				_rolling.RollSpeed = status.avoidSpeed;
				_rolling.BlockHorizontalInput = true;
				_rolling.PreventDamageCollisionsDuringRoll = true;
				_rolling.RollCooldown = status.avoidCool;


			}

			if(_jump != null)
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
				_jump.NumberOfJumps = status.jumpLimit;
				
			}
		 maxHp = status.maxHp;

		//���̕ӂ���Ȃ�����
		//�ʂɎ��������{���ő��삷��΂悭�ˁH

		//�@�������h��́B�̗͂ŏオ��
		 Def = status.Def;
		//�h�˖h��B�ؗ͂ŏオ��
		 pierDef = status.pierDef;
		//�Ō��h��A�Z�ʂŏオ��
		 strDef = status.strDef;
		//�_���h��A�؂ƌ����ŏオ��B
		 holyDef = status.holyDef;
		//�Ŗh��B�����ŏオ��
		 darkDef = status.darkDef;
		//���h��B�����Ɛ����ŏオ��
		 fireDef = 70;
		//���h��B�����Ǝ��v�ŏオ��B
		 thunderDef = 70;

			_health.CurrentHealth = (int)maxHp;

		nowArmor = status.Armor;
	}




		/// <summary>
		/// �_���[�W�v�Z
		/// </summary>
		/// <param name="isFriend">�^�Ȃ疡��</param>
		public void DamageCalc()
		{
			//GManager.instance.isDamage = true;
			//status.hitLimmit--;
			//mValue�̓��[�V�����l


				if (status.phyAtk > 0)
				{
				_damage._attackData.phyAtk = (Mathf.Pow(status.phyAtk, 2) * atV.mValue) * attackFactor;

				//�a���h�ˑŌ����Ǘ�
				if (atV.type == EnemyStatus.AttackType.Slash)
					{
					_damage._attackData._attackType = 0;
				}
					else if (atV.type == EnemyStatus.AttackType.Stab)
					{
					_damage._attackData._attackType = 2;
				}
					else if (atV.type == EnemyStatus.AttackType.Strike)
					{
				

					_damage._attackData._attackType = 4;
					
					//						Debug.Log("�M��");
					if (atV.shock >= 40)
					{
						_damage._attackData.isHeavy = true;
					}
                    else
                    {
						_damage._attackData.isHeavy = false;
                    }
					}
				}
				//�_��
				if (status.holyAtk > 0)
				{
					_damage._attackData.holyAtk = (Mathf.Pow(status.holyAtk, 2) * atV.mValue) * holyATFactor;

				}
				//��
				if (status.darkAtk > 0)
				{
				_damage._attackData.darkAtk = (Mathf.Pow(status.holyAtk, 2) * atV.mValue) * darkATFactor;

				}
			    //��
			    if (status.fireAtk > 0)
			    {
				 _damage._attackData.fireAtk = (Mathf.Pow(status.holyAtk, 2) * atV.mValue) * fireATFactor;

		       	}
				//��
				if (status.thunderAtk > 0)
				{
				_damage._attackData.thunderAtk = (Mathf.Pow(status.holyAtk, 2) * atV.mValue) * thunderATFactor;

				}
			     _damage._attackData.shock = atV.shock;


			_damage._attackData.attackBuff = attackBuff;
			//damage = Mathf.Floor(damage * attackBuff);

			_damage._attackData.isBlow = atV.isBlow;

			_damage._attackData.isLight = atV.isLight;
			_damage._attackData.blowPower.Set(atV.blowPower.x,atV.blowPower.y);
		}

		/// <summary>
		/// �����̃_���[�W���t���O���ĂĂ�����̖h��͂������Ă������
		/// </summary>
		public void DefCalc()
        {

			if (!isAggressive)
			{
				Serch.SetActive(false);
				Serch2.SetActive(false);
				isAggressive = true;
				Flip(direction);
			}
			_health._defData.maxHp = maxHp;
			_health._defData.Def = Def;
			_health._defData.pierDef = pierDef;
			_health._defData.strDef = strDef;
			_health._defData.fireDef = fireDef;
			_health._defData.holyDef = holyDef;
			_health._defData.darkDef = darkDef;

			_health._defData.phyCut = status.phyCut;
			_health._defData.fireCut = status.fireCut;
			_health._defData.holyCut = status.holyCut;
			_health._defData.darkCut = status.darkCut;

			_health._defData.guardPower = status.guardPower;

			isDamage = true;

            if (_condition.CurrentState == CharacterStates.CharacterConditions.Stunned)
            {
				_health._defData.isDangerous = true;

			}
			_health._defData.attackNow = _movement.CurrentState == CharacterStates.MovementStates.Attack ? true : false;
			_health._defData.isGuard = _movement.CurrentState == CharacterStates.MovementStates.Guard ? true : false;
		}

		/// <summary>
		/// �o�t�̐��l��^����
		/// �e�ۂ���Ă�
		/// </summary>
		public void BuffCalc(FireBullet _fire)
		{
			_fire.attackFactor = attackFactor;
			_fire.fireATFactor = fireATFactor;
			_fire.thunderATFactor = thunderATFactor;
			_fire.darkATFactor = darkATFactor;
			_fire.holyATFactor = holyATFactor;
		}




	}
}

