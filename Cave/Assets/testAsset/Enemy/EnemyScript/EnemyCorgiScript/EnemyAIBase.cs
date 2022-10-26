
using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
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

	protected float waitTime;//�p�g���[�����̑҂����Ԑ�����
	protected bool posiReset;//�퓬�Ƃ��ŏ�𗣂ꂽ�Ƃ��߂邽�߂̃t���O
	protected Vector3 firstDirection;//�ŏ��Ɍ����Ă����
	//protected bool nowJump;
	protected bool isReach;//�ԍ����̒��ɂ���
	protected Vector2 targetPosition;//�G�̏ꏊ
	protected bool isUp;
	protected int initialLayer;
	protected bool jumpTrigger;

	//�K�[�h���肷�邩�ǂ���
	protected bool guardJudge;
	protected float stayTime;//�U����̑ҋ@����
	protected int attackNumber;//���Ԃ̍U�������Ă���̂�
	protected bool isAtEnable = true;
	protected bool isDamage;
	protected bool isMovable;
	protected bool isAnimeStart;//�A�j���̊J�n�Ɏg��

	/// <summary>
	/// �ړ��ݒ�̎��Ɍ��߂�i�s����
	/// </summary>



	[HideInInspector] public int bulletDirection;//�G�e���ǂ��炩�痈����

	//�U������҂�����
	protected float flipWaitTime = 0f;
	protected float lastDirection;

	protected float attackBuff = 1;//�U���{��



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

		/// <summary>
		/// �ړ��ݒ�̎��Ɍ��߂�i�s����
		/// </summary>
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
		/// <summary>
		/// �X�[�p�[�A�[�}�[�@�\�����ǂ���
		/// </summary>
		bool isArmor;

		/// <summary>
		/// �����U������ːi�ȂǍU�����I��点�����Ȃ��Ƃ���
		/// </summary>
		protected int attackContinue;

		/// <summary>
		/// �K�[�h����m��
		/// </summary>
		float guardProballity = 100;


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


		[HideInInspector]
		public float nowArmor;


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

		public PlayerJump _jump;
	�@//�@protected int _numberOfJumps = 0;

		public PlayerRoll _rolling;

		public PlayerRunning _characterRun;

		public EnemyFly _flying;

		public GuardAbillity _guard;

		public MyWakeUp _wakeup;

		public EAttackCon _attack;

		[SerializeField]
		public MyDamageOntouch _damage;

		public ESensorAbillity _sensor;
        private bool isVertical;

		public ParryAbility _parry;

		public MyAttackMove _rush;

		//	protected Hittable _hitInfo;


		/// <summary>
		/// �����ƐU������I�������
		/// ���ꂪ�^�ɂȂ�܂œ����Ȃ�
		/// </summary>
		bool flipComp;
		#endregion

		protected const string _suppleParameterName = "suppleAct";
		protected int _suppleAnimationParameter;

		protected const string _combatParameterName = "isCombat";
		protected int _combatAnimationParameter;

		/// <summary>
		/// �⑫�s��������
		/// 
		/// </summary>
		int suppleNumber;

		CharacterStates.MovementStates lastState = CharacterStates.MovementStates.Nostate;


		bool flyNow;


		// === �R�[�h�iMonobehaviour��{�@�\�̎����j ================
		protected override void Initialization()
		{
			base.Initialization();
		initialLayer = this.gameObject.layer;
			ParameterSet(status);
		ArmorReset();

		//���Ƃ�����Ƃ������邵�U������͂������ŊǗ�

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
			//MaterialSet();
	}


		/// <summary>
		/// �ŏ��̃}�e���A���̖����L���ݒ�
		/// </summary>
		protected void MaterialSet()
        {
			//�e�}�e���A���̏�������
			//	Debug.Log($"{parentMatt.material}");
			//�S���̃X�v���C�g���W�߂Đݒ肷��
			for(int i = 0;  i < spriteList.Length;i++)
			{

				GetAllChildren(spriteList[i]);
				//	await UniTask.WaitForFixedUpdate();
			}

			Material coppy = controllTarget[0].material;

			//���炩�̏����ł�������ς���
            if (true)
            {
				coppy.EnableKeyword("Fade_ON");
				coppy.DisableKeyword("BLUR_ON");
				coppy.DisableKeyword("MOTIONBLUR_ON");
			}

			for (int i = 0; i <= controllTarget.Count - 1; i++)
			{
				controllTarget[i].material.CopyPropertiesFromMaterial(coppy);

			}

			controllTarget.Clear();
		}

	public override void ProcessAbility()
		{
			base.ProcessAbility();
			Brain();
			ActionSound();
		//	Debug.Log($"���ǂ���{_animator.name}");


	}


	public void Brain()
        {
			//Debug.Log($"dsfwe");
			//�J�����Ɏʂ��ĂȂ��Ƃ��͎~�܂�܂��B
			if (!cameraRendered && !status.unBaind)
			{

						
					//�����Ȃ��Ȃ���
					return;
			}



			//�ړ��\���ǂ����m�F

				if ( guardHit || _condition.CurrentState != CharacterStates.CharacterConditions.Normal || guardBreak)
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

				///�ύX�_�E����̓X�e�[�g���ς�鎞�����ł���
				//Debug.Log($"ggggggg");

				////////Debug.log($"�W�����v��{nowJump}");
				////////Debug.log($"���{_movement.CurrentState != CharacterStates.MovementStates.Rolling}");
				if (isAggressive)
				{
					//�U���s�����B
					//�����ɓG�Ƃ̋����Ƃ����f����HorizontalMove�Ƃ��N�����郁�\�b�h���B
					
					
					//�ق�ƂɃv���C���[�����ł������H
					//targetPosition = GManager.instance.Player.transform.position;
					targetPosition = GManager.instance.Player.transform.position;

					distance = targetPosition - (Vector2)transform.position;
					//Debug.Log($"�m�肽���̂�{targetPosition.x}");
					direction = distance.x >= 0 ? 1 : -1;//���������A�܂���0�̎�1�B�����łȂ��Ƃ�-1�B����
					directionY = distance.y >= 0 ? 1 : -1;//�|�\����Ƃ��̃A�j���̔���	�ɂ��g������
					EscapeCheck();
					if (_movement.CurrentState == CharacterStates.MovementStates.Attack && _condition.CurrentState == CharacterStates.CharacterConditions.Stunned)
					{
						AttackEnd();
					}
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
		//	Parry();
			Die();//�w���X�ɕς���
			MaterialControll();



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
			_controller.DefaultParameters.Gravity = -gravity;
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
	/// �U���̓r���ŃX�[�p�[�A�[�}�[�o����
	/// isArmor����H
	/// </summary>
	public void EnemySuperAmor()
	{
			if (_movement.CurrentState == CharacterStates.MovementStates.Attack)
			{
				
			}
            else
            {
			//	_movement.ChangeState(CharacterStates.MovementStates.Attack);
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
            if ((_movement.CurrentState == CharacterStates.MovementStates.GuardMove || _movement.CurrentState == CharacterStates.MovementStates.Guard) && isBack)
            {
				_guard.GuardEnd();
            }
			if (!isBack && (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove))
            {
				_guard.GuardHit();
				if (isDown)
				{
					nowArmor -= ((shock * 2) * status.guardPower / 100) * 1.2f;

				}
				else
				{
					nowArmor -= (shock * 2) * ((100 - status.guardPower) / 100);
				}
				if(nowArmor <= 0)
                {
					_guard.GuardEnd();
				}
			}

            else
            {
				if (!isArmor || atV.aditionalArmor == 0)
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
				if (_movement.CurrentState == CharacterStates.MovementStates.Attack)
				{
					AttackEnd();
                }

                if (isDown)
                {
					result = (MyWakeUp.StunnType.Down);
                }
			    else
                {

					if (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove)
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
		/// �A�[�}�[�l�ɉ����ăC�x���g�Ƃ΂�
		/// </summary>
		public bool ParryArmorJudge()
		{

			nowArmor -= Mathf.Ceil(status.Armor * ((100 - atV.parryResist) / 100));
			
			if (nowArmor <= 0)
			{
				AttackEnd();
			//	_wakeup.StartStunn(MyWakeUp.StunnType.Parried);
				return true;
			}
			else
			{Debug.Log($"����{nowArmor}");
				return false;
			}
		}






		protected virtual void OnTriggerEnter2D(Collider2D collision)
	{



				if (isAggressive && collision.tag == EnemyManager.instance.JumpTag && _controller.State.IsGrounded)
			{
				//�W�����v���������Ă�Ȃ�
				if (collision.gameObject.MMGetComponentNoAlloc<JumpTrigger>().jumpDirection == transform.localScale.x)
				{
					JumpAct();
					Debug.Log($"sss");
				}
			}
		
	}
	protected virtual void OnTriggerStay2D(Collider2D collision)
	{
			if (_condition.CurrentState != CharacterStates.CharacterConditions.Dead)
			{

				if (isAggressive && collision.tag == EnemyManager.instance.JumpTag && _controller.State.IsGrounded)
				{
					if (collision.gameObject.MMGetComponentNoAlloc<JumpTrigger>().jumpDirection == transform.localScale.x)
					{
						JumpAct();
						Debug.Log($"dfggrferfer");
					}
				}
			}
	}




		/// <summary>
		/// �X�e�[�^�X�֘A
		/// </summary>
		#region

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
				//	rb.velocity = Vector2.zero;
					//("DDie");
					isAnimeStart = true;
				}
				else
				{
				//	rb.velocity = Vector2.zero;
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
	 protected void ParameterSet(EnemyStatus status)
        {
			///<summary>
			///�@���X�g
			/// </summary>
			/// 
			_characterHorizontalMovement.FlipCharacterToFaceDirection = false;

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
				_flying.FastFly(false,false);

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

			_health.InitialHealth = (int)maxHp;
			_health.MaximumHealth = (int)maxHp;
			_health.CurrentHealth= _health.InitialHealth;
		//	Debug.Log($"tanomu{_health.CurrentHealth}");
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

			float mainDamage = 0;
			if (status.phyAtk > 0)
			{
				_damage._attackData.phyAtk = status.phyAtk * attackFactor;

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
				_damage._attackData.holyAtk =  status.holyAtk * holyATFactor;
				if (_damage._attackData.holyAtk > mainDamage)
				{
					_damage._attackData._attackType = 8;
					mainDamage = _damage._attackData.holyAtk;
				}
			}
			//��
			if (status.darkAtk > 0)
			{
				_damage._attackData.darkAtk =  status.darkAtk * darkATFactor;
				if (_damage._attackData.darkAtk > mainDamage)
				{
					_damage._attackData._attackType = 16;
					mainDamage = _damage._attackData.darkAtk;
				}
			}
			//��
			if (status.fireAtk > 0)
			{
				_damage._attackData.fireAtk =  status.fireAtk * fireATFactor;
				if (_damage._attackData.fireAtk > mainDamage)
				{
					_damage._attackData._attackType = 32;
					mainDamage = _damage._attackData.fireAtk;
				}
			}
			//��
			if (status.thunderAtk > 0)
			{
				_damage._attackData.thunderAtk =  status.thunderAtk * thunderATFactor;
				if (_damage._attackData.thunderAtk > mainDamage)
				{
					_damage._attackData._attackType = 64;
				//	mainDamage = _damage._attackData.Atk;
				}
			}
			_damage._attackData.shock = atV.shock;
			_damage._attackData.mValue = atV.mValue;
			_damage._attackData.disParry = atV.disParry;
			_damage._attackData.attackBuff = attackBuff;
			//damage = Mathf.Floor(damage * attackBuff);
			_damage._attackData._parryResist = atV.parryResist;
			_damage._attackData.isBlow = atV.isBlow;

			_damage._attackData.isLight = atV.isLight;
			_damage._attackData.blowPower.Set(atV.blowPower.x, atV.blowPower.y);
		}

		/// <summary>
		/// �����̃_���[�W���t���O���ĂĂ�����̖h��͂������Ă������
		/// </summary>
		public void DefCalc()
		{

			if (!isAggressive)
			{
				StartCombat();
			}
			_health.InitialHealth = (int)maxHp;
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
			
		}

		public void GuardReport()
        {
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

		public void ParryStart()
        {
			_parry.ParryStart();
			_guard.GuardEnd();
			nowArmor += 40;
        }

		#endregion



		///<sammary>
		/// �@�U���֘A�̏���
		/// �@�U���ɕK�v�Ȃ���
		/// �@�܂��U���̃p�����[�^��ݒ�B�ԍ����w�肵�U���J�n
		/// �@�����U���Ȃǂɂ͈����t���̍U���I�������ݒ�A�j���C�x���g������B����ŃA�[�}�[���L���ɂ���
		/// �@�A�j���C�x���g�ɂ͂ق��ɃG�t�F�N�g���o����Ɖ����o���������B
		/// �@�����čU���I�������𖞂����Ă邩�A���邢�̓A�j���I��������ōU���I���m�F
		/// �@�����ĕ⑫�s������Ȃ��邪�A�⑫�s���͌����ɂ͍U�������Ɛ؂藣���čs���B
		/// �@�⑫�s���t���O�͉��p����Β����Ƃ��G�ɂ������
		/// �@�U�����Ƃɂ���N�[���^�C�����I��������܂��U������悤��
		/// �@���p�n�̍U���̉r���͍U������Ȃ��U���ɂ��ăR���{�Ŗ��p������
		/// �@�U���l��isShoot�Ƃ�����邩
		///</sammary>
		#region

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
			atV.suppleNumber = status.atValue[attackNumber].suppleNumber;

			//�ːi�p�̏�����
			atV._moveDuration = status.atValue[attackNumber]._moveDuration;
			atV._moveDistance = status.atValue[attackNumber]._moveDistance;
			atV._contactType = status.atValue[attackNumber]._contactType;
			atV.fallAttack = status.atValue[attackNumber].fallAttack;
			atV.startMoveTime = status.atValue[attackNumber].startMoveTime;
			atV.lockAttack = status.atValue[attackNumber].lockAttack;

			//�q�b�g������֘A
			_damage._attackData._hitLimit = status.atValue[attackNumber]._hitLimit;
			_damage.CollidRestoreResset();


		}


		/// <summary>
		/// �e�ۂ𔭎˂���
		/// �r���A�j����ɂ���ł悭�ˁH
		/// </summary>
		/// <param name="i"></param>
		/// <param name="random"></param>
		public void ActionFire(int i, float random = 0.0f)
	{
			//�����_���ɓ���Ă��������Ǖ��ʂɓ���Ă�����
		i = i < 0 ? 0 : i;
		i = i > status.enemyFire.Count - 1 ? status.enemyFire.Count : i;


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
		//�e�ۏ����̗�
		//���[�h����
		#region
		/*
		 		/// <summary>
		/// for���ł͂Ȃ���	bcount�𒴂���܂�useMagic���^�Ȃ̂Ŕ�����������
		/// �e�ۂ���郁�\�b�h
		/// </summary>
		/// <param name="hRandom"></param>
		/// <param name="vRandom"></param>
		 void MagicUse(int hRandom, int vRandom)
		{

			if (!fireStart || delayNow)
			{
				return;
			}
			bCount += 1;
			Debug.Log("���Ă�");
			//	Debug.Log($"�n�U�[�h{SManager.instance.useMagic.name}�W�I{SManager.instance.target}����{sister.nowMove}");
			//���@�g�p��MagicUse�ł��e�ې������łȂ����

			//�e�̔��˂Ƃ����������ʒu
			Vector3 goFire = sb.firePosition.position;
			//�e���ꔭ�ڂȂ�
			if (bCount == 1)
			{
				//   MyInstantiate(SManager.instance.useMagic.fireEffect, goFire, Quaternion.identity).Forget();
				//Addressables.InstantiateAsync(SManager.instance.useMagic.fireEffect, goFire, Quaternion.identity);
				if (SManager.instance.useMagic.fireType == SisMagic.FIREBULLET.RAIN)
				{
					//�R�Ȃ�̒e���őł������Ƃ��Ƃ��ˏo�p�x���߂ꂽ�炢������
					//�ʒu�������_���ɂ���Ίp�x�͂ǂ��ł�������������
					SManager.instance.useMagic.angle = GetAim(sb.firePosition.position, SManager.instance.target.transform.position);

				}
				sb.mp -= SManager.instance.useMagic.useMP;
			}

			//�G�̈ʒu�ɃT�[�`�U������Ƃ�
			if (SManager.instance.useMagic.isChaice)
			{
				goFire.Set(SManager.instance.target.transform.position.x, SManager.instance.target.transform.position.y, SManager.instance.target.transform.position.y);

			}
			//�����_���Ȉʒu�ɔ�������Ƃ�
			else if (hRandom != 0 || vRandom != 0)
			{
				//Transform goFire = firePosition;


				float xRandom = 0;
				float yRandom = 0;
				if (hRandom != 0)
				{

					xRandom = RandomValue(-hRandom, hRandom);

				}
				if (vRandom != 0)
				{
					yRandom = RandomValue(-vRandom, vRandom);
				}
				//	xRandom = RandomValue(RandomValue(-random,0),RandomValue(0, random));
				//	yRandom = RandomValue(RandomValue(-random, 0), RandomValue(0, random));

				goFire = new Vector3(sb.firePosition.position.x + xRandom, sb.firePosition.position.y + yRandom, 0);//�e������

			}
			//Debug.Log($"���@�̖��O5{SManager.instance.useMagic.hiraganaName}");
			//    MyInstantiate(SManager.instance.useMagic.effects, goFire, Quaternion.identity).Forget();//.Result;//�����ʒu��Player
			//�����ɔ�������e�ۂ̈ꔭ�ڂȂ�
			if (SManager.instance.useMagic.delayTime == 0 || bCount == 1)
			{
				Debug.Log("aaa");
                UnityEngine.Object h = Addressables.LoadAssetAsync<UnityEngine.Object>(SManager.instance.useMagic.effects).Result;
				 GameObject t =  Instantiate(h, goFire, Quaternion.Euler(SManager.instance.useMagic.startRotation)) as GameObject;//.MMGetComponentNoAlloc<FireBullet>().InitializedBullet(this.gameObject, SManager.instance.target);
				t.MMGetComponentNoAlloc<FireBullet>().InitializedBullet(this.gameObject,SManager.instance.target);
			}
			//2���ڈȍ~�̒e�Ő���������Ȃ��Ȃ�
			else if (bCount > 1 && !delayNow)
			{
				DelayInstantiate(SManager.instance.useMagic.effects, goFire, Quaternion.Euler(SManager.instance.useMagic.startRotation)).Forget();
			}
			//�e�ۂ𐶐����I�������
			if (bCount >= SManager.instance.useMagic.bulletNumber)
			{
				//Debug.Log($"�e���y�X�g{SManager.instance.useMagic.name}�W�I{SManager.instance.target}����{sister.nowMove}");
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				//disEnable = true;
				coolTime = SManager.instance.useMagic.coolTime;
				bCount = 0;
				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);

				actionNum = 0;

				SManager.instance.useMagic = null;
				fireStart = false;
				SManager.instance.target.GetComponent<EnemyAIBase>().TargetEffectCon(3);
			}
				
		}


		 */
		#endregion



		/// <summary>
		/// �U��
		/// ���C�v��
		/// isShoot = true�̎��̏�������H�B����Ȃ��H�܂肽�܂������Ă�������
		/// �����@�[�X�ŋt������
		/// </summary>
		public void Attack(bool select = false, int number = 1, bool reverse = false)
	{

            if (!isAtEnable || !isMovable || number > status.atValue.Count || number <= 0)
            {//Debug.Log($"��񂫂�{isAtEnable}{isMovable}{ number > status.atValue.Count}{number <= 0}");
				return;
            }
			//	
			isMovable = false;
			if (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove)
			{
				_guard.GuardEnd();
			}

			guardJudge = false;



			if (select)
			{
				attackNumber = number - 1;
			}

			AttackPrepare();
		//	Debug.Log($"����1{attackNumber}");
			_attack.AttackTrigger(number);

			isAtEnable = false;

			_characterHorizontalMovement.SetHorizontalMove(0);
			_controller.SetHorizontalForce(0);
			//�������ړ��͈͓��ŁA���b�N�I������Ȃ狗����ς���
			float moveDistance = (atV.lockAttack && Mathf.Abs(distance.x) < atV._moveDistance) ? distance.x : atV._moveDistance;	

			_rush.RushStart(atV._moveDuration,moveDistance * direction,atV._contactType,atV.fallAttack,atV.startMoveTime);


			if (!reverse)
			{
			//	Debug.Log($"������{transform.lossyScale.x}{direction}");
					NormalFlip(direction,true);

			}
			else
			{

					NormalFlip(-direction);

			}
		}


		public void attackEffect()
		{
			// Debug.Log($"�A�C�C�C�C�C�C{atEffect.SubObjectName}");
			Addressables.InstantiateAsync(atV.attackEffect, effectController.transform);
		}




		/// <summary>
		/// �U���I���̏��������A�[�}�[��ݒ�
		/// �A�j���[�V�����C�x���g
		/// </summary>
		public void ConditionAttack(int continueNumber = 0)
        {
			//���̃i���o�[�ɂ���ďd�͂�������Ƃ����H

				attackContinue = continueNumber;

			isArmor = true;
        }

		/// <summary>
		/// �⑫�s���̋N�����s�����\�b�h
		/// </summary>
		/// <param name="useNumber"></param>
		public void SActTrigger(int useNumber)
        {
			suppleNumber = atV.suppleNumber;
			_condition.ChangeState(CharacterStates.CharacterConditions.Moving);
		}

		/// <summary>
		/// �ǂ̃��[�V��������邩�ǂ���
		/// </summary>
		/// <param name="number"></param>
		public void SetAttackNumber(int number)
		{
			number = number > status.serectableNumber.Count - 1 ? status.serectableNumber.Count - 1 : number;
			number = number < 0 ? 0 : number;
			attackNumber = status.serectableNumber[number];
		}

		/// <summary>
		/// �N�[���^�C���̊Ԏ��̍U����҂�
		/// </summary>
		public async  void WaitAttack()
	{
		if (_attack.nowAttack)
		{
			//	Debug.Log($"tubu����{attackNumber}");
				if (attackContinue == 0)
			{
		//			Debug.Log($"���Ȃ���{isMovable}����{_movement.CurrentState}");
				await ExecuteAttack();
	
			}
				else if (attackContinue != 0)
				{
					//�P�͋󒆍U��
					if (attackContinue == 1)
					{
						//�ڒn������0��
						if (_controller.State.IsGrounded)
						{
							attackContinue = 0;
							//	NormalFlip(direction);
							//	isAtEnable = true;����N�[���^�C���ŊǗ����悤��
							_movement.ChangeState(CharacterStates.MovementStates.Idle);
							atV.aditionalArmor = 0;
							_attack.AttackEnd();
							isArmor = false;
						}
					}

				}

			}

		else if (!isAtEnable && _condition.CurrentState != CharacterStates.CharacterConditions.Stunned)
		{

					/// �⊮�s���̏I���҂�
					if(suppleNumber != 0)
                    {
						if (!CheckEnd($"SuppleAction{suppleNumber}"))
						{
							_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
						}
					}
					if (!atV.isCombo)
					{
						stayTime += _controller.DeltaTime;
						if (stayTime >= atV.coolTime)
						{
							isAtEnable = true;
							stayTime = 0.0f;
						}
					}

					else if (atV.isCombo)
					{

						isAtEnable = true;
					attackNumber+= 2;
				//	Debug.Log($"����2{attackNumber}");
					//AttackPrepare();
					isMovable = true;
						Attack(true,attackNumber);
					}
				
		}
	}
		/// <summary>
		/// �A�j�����f
		/// </summary>
		public void AttackEnd()
        {

			_movement.ChangeState(CharacterStates.MovementStates.Idle);
			if(_condition.CurrentState != CharacterStates.CharacterConditions.Stunned)
            {
				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
            }
			atV.aditionalArmor = 0;
			isAtEnable = true;
			stayTime = 0.0f;
			attackNumber = 0;

			attackContinue = 0;
			_attack.AttackEnd();
			attackComp = true;
			GravitySet(status.firstGravity);
			isArmor = false;
		}

		/// <summary>
		/// �p���B�s�̍s��
		/// �G�t�F�N�g�o�����Ԃ��w��ł���悤�ɂ�������
		/// LifTime�����邩
		/// </summary>
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

		//�w���X�̂��߂ɍU����Ԃ��ۂ���Ԃ�
		public bool AttackCheck()
		{
			return _movement.CurrentState == CharacterStates.MovementStates.Attack;
		}
		#endregion




		//	bool CheckEnd(string _currentStateName)
		//{
		//	return sAni.IsPlaying(_currentStateName);
		//}
		bool CheckEnd(string Name)
	{
//Debug.Log($"����1�w��{Name}");
		if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(Name))// || sAni.GetCurrentAnimatorStateInfo(0).IsName("OStand"))
		{   // �����ɓ��B�����normalizedTime��"Default"�̌o�ߎ��Ԃ��E���Ă��܂��̂ŁAResult�ɑJ�ڊ�������܂ł�return����B
				
				return true;
		}
		if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
		{   // �ҋ@���Ԃ���肽���Ȃ�΁A�����̒l��傫������B
			//	Debug.Log("����2");
				return true;
		}
		//AnimatorClipInfo[] clipInfo = sAni.GetCurrentAnimatorClipInfo(0);

		////Debug.Log($"�A�j���I��");

		return false;

		// return !(sAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
		//  (_currentStateName);
	}
		private async UniTask ExecuteAttack()
		{
			if (_movement.CurrentState != CharacterStates.MovementStates.Attack)
			{
				Debug.Log($"ddd{_movement.CurrentState}{_condition.CurrentState}");
				return;
			}

			// ���[�V���������s(���s���Animator�X�V�̂���1�t���[���҂�)
		//	animator.Play("Attack");
			await UniTask.DelayFrame(1);



			// ���[�V�����I���܂őҋ@
			await UniTask.WaitUntil(() => {
				var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
				return 1.0f <= stateInfo.normalizedTime;
			});
			_attack.AttackEnd();
		//	
			NormalFlip(direction);


			atV.aditionalArmor = 0;
			isArmor = false;

			//			_movement.RestorePreviousState();

			_movement.ChangeState(CharacterStates.MovementStates.Idle);

			//�⑫�s���w�肪0�łȂ��Ƃ�
			if (atV.suppleNumber != 0 && !atV.isCombo)
			{
				SActTrigger(atV.suppleNumber);
			}
			// �ҋ@���[�V�������Đ�
			//	_animator.CrossFade("Idle", 0.1f);

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


		/// <summary>
		/// �A�j���C�x���g
		/// ���֘A
		/// </summary>
		#region

		public virtual void FlySound()
	{
		if ((_movement.CurrentState == CharacterStates.MovementStates.Flying || _movement.CurrentState == CharacterStates.MovementStates.FastFlying)
				&& _condition.CurrentState != CharacterStates.CharacterConditions.Normal)
		{
			GManager.instance.PlaySound(status.walkSound, transform.position);

		}
		else
		{
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

		/// <summary>
		/// ���݂̏󋵂ɍ��킹���T�E���h��񋟂��܂�
		/// �ړ��ƃX�^���ȊO
		/// �ŗL�̉��ɕς������ꍇ�̓I�[�o�[���C�h
		/// �v���C���[�ɈڐA���Ă�����
		/// </summary>
		/// <param name="i">����̓o���G�[�V��������ꍇ�̃p�����[�^</param>
		public void ActionSound(int i = 0)
		{
            if (_controller.State.JustGotGrounded)
            {
				MyCode.SoundManager.instance.ShakeSound(status.isMetal, status._bodySize, transform);
			}

			if (status.kind == EnemyStatus.KindofEnemy.Fly)
			{
				if (!flyNow && (_condition.CurrentState != CharacterStates.CharacterConditions.Dead && _condition.CurrentState != CharacterStates.CharacterConditions.Stunned))
				{
				//	Debug.Log("����������");
					GManager.instance.FollowSound(status.walkSound, transform);
					flyNow = true;
					return;
				}
                else if (flyNow && (_condition.CurrentState == CharacterStates.CharacterConditions.Dead || _condition.CurrentState == CharacterStates.CharacterConditions.Stunned))
				{
			//		Debug.Log("��������");
					GManager.instance.StopSound(status.walkSound, 0.5f);
					flyNow = false;
					return;
				}
			}
			if(lastState == _movement.CurrentState || _condition.CurrentState != CharacterStates.CharacterConditions.Normal)
            {
				return;
            }
			lastState = _movement.CurrentState;
			//Debug.Log($"sf{_movement.CurrentState}");
			if (_movement.CurrentState == CharacterStates.MovementStates.Rolling)
			{
				//�����Fly�Ƃ��ŕ����邩
				//GManager.instance.PlaySound(MyCode.SoundManager.instance.armorRollSound[1], transform.position);
			}
			else if (_movement.CurrentState == CharacterStates.MovementStates.Jumping || _movement.CurrentState == CharacterStates.MovementStates.DoubleJumping)
			{
				MyCode.SoundManager.instance.JumpSound(status.isMetal, status._bodySize, transform);
			}
			else if (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.Crouching)
			{
				MyCode.SoundManager.instance.ShakeSound(status.isMetal,MyCode.SoundManager.SizeTag.small,transform);
			}


		}


		public void GuardSound()
        {
			GManager.instance.PlaySound(status.guardSound, transform.position);
		}

		public void UseSound(string useName)
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
	public void FirstSound(int type = 0)
	{
		GManager.instance.PlaySound(MyCode.SoundManager.instance.fistSound[type], transform.position);

		//�G���`�����Ă�ꍇ��

	}

		#endregion


		/// <summary>
		/// �ړ��֘A����
		/// </summary>
		#region

		public void NormalFlip(float direction,bool tes = false)
		{

                if (direction != MathF.Sign(transform.localScale.x))
                {

					Vector3 flip = transform.localScale;
					flip.Set(direction, flip.y,flip.z);
					transform.localScale = flip;
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
				if (status.kind == EnemyStatus.KindofEnemy.Fly)
				{
					if (transform.position.x <= basePosition.x)
					{
						 NormalFlip(1);
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
						 NormalFlip(-1);

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

						 NormalFlip(1);
						if (transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)
						{
							_characterHorizontalMovement.SetHorizontalMove(0);
							posiReset = false;
							isRight = true;
							// NormalFlip(baseDirection);
							//transform.localScale = baseDirection;
						}
						else
						{
							_characterHorizontalMovement.SetHorizontalMove(1);
						}
					}

					else if (transform.position.x >= basePosition.x)
					{

						 NormalFlip(-1);
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
						 NormalFlip(1);
						_characterHorizontalMovement.SetHorizontalMove(1);

					}
					else if (transform.position.x >= startPosition.x - status.waitDistance.x && !isRight)
					{
						 NormalFlip(-1);
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
						 NormalFlip(1);
						_flying.SetHorizontalMove(1);

					}
					else if (transform.position.x >= startPosition.x - status.waitDistance.x && !isRight)
					{
						 NormalFlip(-1);
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
				if (transform.position.x <= startPosition.x + status.waitDistance.x && isRight)
				{
					
					NormalFlip(1);
					_flying.SetHorizontalMove(1);

				}
				else if (transform.position.x >= startPosition.x - status.waitDistance.x && !isRight)
				{
					NormalFlip(-1);
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
							 NormalFlip(-Math.Sign(transform.localScale.x));//���]�����܁[��
							waitTime = 0.0f;
						}
					}
					if (dRb != null)
					{
						_characterHorizontalMovement.SetHorizontalMove(Mathf.Abs(dogPile.transform.localScale.x));
						 NormalFlip(Mathf.Sign(dogPile.transform.localScale.x));
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
							 NormalFlip(-Math.Sign(transform.localScale.x));//���]�����܁[��
							waitTime = 0.0f;
						}
					}
					if (dRb != null)
					{
						_flying.SetHorizontalMove(Mathf.Abs(dogPile.transform.localScale.x));
						 NormalFlip(Mathf.Sign(dogPile.transform.localScale.x));
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
					CombatEnd();
					escapeTime = 0.0f;
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
             GuardJudge();
		//	Debug.Log($"�m�肽��{ground}");
			stateJudge += _controller.DeltaTime;
			#region//���f



			if ((ground == EnemyStatus.MoveState.wakeup || stateJudge >= status.judgePace) && ground != EnemyStatus.MoveState.escape)
			//escape�����̓X�N���v�g���瓮����
			{
				

				bool isDashable = status.combatSpeed.x > 0;
				int dire = direction;
				if (((Mathf.Abs(distance.x) <= status.agrDistance[disIndex].x + status.adjust && Mathf.Abs(distance.x) >= status.agrDistance[disIndex].x - status.adjust) && RandomValue(0, 100) >= 40) || guardHit)
				{
					flipWaitTime = 1f;
					ground = EnemyStatus.MoveState.stay;
					//	flipWaitTime = 10;
				}
				else if (Mathf.Abs(distance.x) > status.agrDistance[disIndex].x)//�߂Â�������ˁH
				{
					if (Mathf.Abs(distance.x) <= status.walkDistance.x || !isDashable)
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
					if (Mathf.Abs((Mathf.Abs(distance.x) - status.agrDistance[disIndex].x)) <= status.walkDistance.x / 2 || !isDashable)
					{
						ground = EnemyStatus.MoveState.leaveWalk;
					}
					else
					{
						ground = EnemyStatus.MoveState.leaveDash;

					}
				}

				if(attackComp && atV.escapePercentage > 0 && isMovable)
                {
                    if (RandomValue(0,100) <= atV.escapePercentage)
                    {
						if (status.agrDistance[disIndex].x <= 30 || !isDashable)
						{
							ground = EnemyStatus.MoveState.leaveWalk;
						}
						else if (Mathf.Abs((Mathf.Abs(distance.x) / status.agrDistance[disIndex].x)) >= 0.6)
						{
							//�����鎞�̓K�[�h�I��点��
							ground = EnemyStatus.MoveState.leaveDash;

						}
					}
					attackComp = false;
                }

				if(ground == EnemyStatus.MoveState.leaveDash || ground == EnemyStatus.MoveState.leaveWalk)
                {
					dire *= -1;
                }

				BattleFlip(dire);

                if (!flipComp)
                {
               //     if (ground != EnemyStatus.MoveState.stay)
                  //  {
					//	Debug.Log($"dddd{dire != lastDirection}");
				//	}
				//	ground = EnemyStatus.MoveState.stay;
					
                }
				
				stateJudge = 0;
				
				if (_movement.CurrentState != CharacterStates.MovementStates.Running && isDashable)
				{
					if (ground == EnemyStatus.MoveState.accessDash || ground == EnemyStatus.MoveState.leaveDash)
					{
						if ((_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove))
						{
							_guard.GuardEnd();
						}
						_characterRun.RunStart();
					}
				}
				else
				{
					if ((ground != EnemyStatus.MoveState.accessDash && ground != EnemyStatus.MoveState.leaveDash) || (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove))
					{
						_characterRun.RunStop();
					}
				}
				moveDirectionX = direction;

			}
			#endregion

			if (isMovable)
			{
				if(_movement.CurrentState == CharacterStates.MovementStates.Attack)
                {
					return;
                }
				if (ground == EnemyStatus.MoveState.stay)
				{
					//�o�g���t���b�v�̓X�e�C�������ɂ���
					
					_characterHorizontalMovement.SetHorizontalMove(0f);
					isReach = true;
					return;

					//Debug.Log($"�˂���{blowM.x}");


				}
				else if (ground == EnemyStatus.MoveState.accessWalk)
				{
					
					isReach = (Mathf.Abs(distance.x) - status.agrDistance[disIndex].x) <= status.walkDistance.x ? true : false;
					_characterHorizontalMovement.SetHorizontalMove(moveDirectionX);

				}
				else if (ground == EnemyStatus.MoveState.accessDash)
				{
					
					isReach = false;
					_characterHorizontalMovement.SetHorizontalMove(moveDirectionX);
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
					
					isReach = true;

					_characterHorizontalMovement.SetHorizontalMove(-moveDirectionX);
				}
				else if (ground == EnemyStatus.MoveState.leaveDash)
				{

					
					isReach = false;
					_characterHorizontalMovement.SetHorizontalMove(-moveDirectionX);
				}
			}
		}

		/// <summary>
		/// ����ԃ^�C�v�̃G�l�~�[�ɐ퓬���悹��B�����
		/// </summary>
		/// <param name="disIndex">�g�p����퓬����</param>
		/// <param name="stMove">�܂������i�ނƂ��Ɏg���t���O</param>
		public void AgrFly(int disIndex = 0, int stMove = 0)
		{
			
			stateJudge += _controller.DeltaTime;
			#region//���f
			if ((ground == EnemyStatus.MoveState.wakeup || stateJudge >= status.judgePace) && ground != EnemyStatus.MoveState.escape && _condition.CurrentState != CharacterStates.CharacterConditions.Stunned)
			//escape�����̓X�N���v�g���瓮����
			{
		//		Debug.Log($"�A�C{air}");// {air}");//{_condition.CurrentState}
				bool isSet = false;

			int dire = direction;

				//���̏ꍇ�̓p�[�Z���e�[�W�͕�����̂Ɏg����
				//�U���I���㓦����
				if (stMove == 0 || attackComp)
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
							if (Mathf.Abs(distance.x) <= status.walkDistance.x)
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
							if (Mathf.Abs((Mathf.Abs(distance.x) - status.agrDistance[disIndex].x)) <= status.walkDistance.x / 2)
							{
								ground = EnemyStatus.MoveState.leaveWalk;
							}
							else
							{

								ground = EnemyStatus.MoveState.leaveDash;
							}
						}



					
				}
				else if (stMove != 0)
				{
					ground = EnemyStatus.MoveState.straight;

				}

				if (ground == EnemyStatus.MoveState.leaveDash || ground == EnemyStatus.MoveState.leaveWalk)
				{
					dire *= -1;
				}
				if (ground != EnemyStatus.MoveState.straight)
				{
					BattleFlip(dire);
					if (!flipComp)
					{
						ground = EnemyStatus.MoveState.stay;
					}
				}

				if (ground == EnemyStatus.MoveState.leaveDash || ground == EnemyStatus.MoveState.accessDash || ground == EnemyStatus.MoveState.straight)
				{
					if (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove)
					{
						_guard.GuardEnd();
					}
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

					//�}�C�i�X�ɂ��邱�ƂŕW�I���猩�������̋����ɂȂ�
					//�W�I�����ɂ��邩���ɂ��邩
					float targetHeight = -distance.y;


					if ((targetHeight <= status.agrDistance[disIndex].y + status.adjust) && (targetHeight >= status.agrDistance[disIndex].y - status.adjust) || guardHit)
					{

						air = EnemyStatus.MoveState.stay;
						_flying.FastFly(true, true);
					}

					//�㏸����
					else if (targetHeight <= status.agrDistance[disIndex].y)//�߂Â�������ˁH
					{
						if (status.agrDistance[disIndex].y - targetHeight <= status.walkDistance.y)
						{
							air = EnemyStatus.MoveState.accessWalk;
							_flying.FastFly(true, true);
						}
						else
						{
							air = EnemyStatus.MoveState.accessDash;
							_flying.FastFly(true, false);
							if (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove)
							{
								_guard.GuardEnd();
							}
						}
					}
					//�~������
					else if (targetHeight > status.agrDistance[disIndex].y)//��������
					{
						//���������Ȃ�G�������܂܌���
						//�����Ȃ��|���Ƃ��͈ړ����x�[����
						if (targetHeight - status.agrDistance[disIndex].y <= status.walkDistance.y)
						{
							air = EnemyStatus.MoveState.leaveWalk;
							_flying.FastFly(true, true);
						}
						else
						{
							if (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove)
							{
								_guard.GuardEnd();
							}
							air = EnemyStatus.MoveState.leaveDash;
							_flying.FastFly(true, false);
						}
					}
				//	Debug.Log($"�A{air}");
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

				if (ground == EnemyStatus.MoveState.stay || !flipComp)
				{
					
					_flying.SetHorizontalMove(0);

					isReach = true;

				}
				else if (ground == EnemyStatus.MoveState.accessWalk)
				{

					
					isReach = (Mathf.Abs(distance.x) - status.agrDistance[disIndex].x) <= status.walkDistance.x ? true : false;
					_flying.SetHorizontalMove(direction);

				}
				else if (ground == EnemyStatus.MoveState.accessDash)
				{
					
					isReach = false;
					_flying.SetHorizontalMove(direction);
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
					
					isReach = true;
					_flying.SetHorizontalMove(-direction);

				}
				else if (ground == EnemyStatus.MoveState.leaveDash)
				{
					
					isReach = false;
					_flying.SetHorizontalMove(-direction);
				}
				else if (ground == EnemyStatus.MoveState.straight)
				{

					if (stMove > 0)
					{
						 NormalFlip(1);
						isReach = false;
						_flying.SetHorizontalMove(stMove);
					}
					else
					{
						 NormalFlip(-1);
						isReach = false;
						_flying.SetHorizontalMove(-1);
					}
				}
			}

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
		///	�o�b�N�W�����v���^�Ȃ���ɐU������Ĕ��ł���
		/// </summary>	
		public void JumpAct(bool verticalJump = false,bool backJump = false)
		{

			if (isMovable)
			{ 
				if (!verticalJump)
                {
					isVertical = verticalJump;
					if (backJump)
					{
						float dire = -1;
						if(transform.localScale.x > 0)
                        {
							dire = 1;
                        }
						 NormalFlip(dire);
					}
                }
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
			if (_movement.CurrentState == CharacterStates.MovementStates.Jumping || _movement.CurrentState == CharacterStates.MovementStates.DoubleJumping)
			{
				if (!isVertical)
				{
					_characterHorizontalMovement.SetHorizontalMove(Mathf.Sign(transform.localScale.x));
				}
				
			}
		
			//�_�u���W�����v���邩�ǂ����͔C�ӂŌ��߂܂�
			/*else if (!_controller.State.IsGrounded)
			{
				if (_jump.JumpEnableJudge() == true)
				{
					Debug.Log($"fffffffffff");
					_jump.JumpStart();
				}
			}*/
		}

		public void BattleFlip(float direction)
	{
			if(direction == 0)
            {
				return;
            }
			if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
			{
                 flipWaitTime += _controller.DeltaTime;
				if (lastDirection != direction)
				{
					
					flipComp = false;

					if (flipWaitTime >= 0.2f)
					{

						flipWaitTime = 0f;
						if (direction != MathF.Sign(transform.localScale.x)) 
						{
							NormalFlip(direction);
						}
						lastDirection = direction;
						flipComp = true;
					}
				}
				else
				{
					flipComp = true;
				//	flipWaitTime = 0;
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
				_flying.SetVerticalMove(1);
				//move.Set(0, status.addSpeed.y * (status.combatSpeed.y - rb.velocity.y));
				//move.y = ;
				_flying.FastFly(true, false);
				
			}
		else if (air == EnemyStatus.MoveState.accessWalk)

		{
				_flying.SetVerticalMove(1);
				_flying.FastFly(true, true);
			}
		else if (air == EnemyStatus.MoveState.leaveDash)
		{
				_flying.SetVerticalMove(-1);
				//move.Set(0, status.addSpeed.y * (-status.combatSpeed.y - rb.velocity.y));
				_flying.FastFly(true, false);
			//move.y = ;
		}
		else if (air == EnemyStatus.MoveState.leaveWalk)

		{
				_flying.SetVerticalMove(-1);
				_flying.FastFly(true, true);
			}
	}

        #endregion

	/// <summary>
	/// FixedUpdate�ɒu���ăK�[�h������
	/// �����̓K�[�h����m��
	/// �K�[�h���ړ����Ă���
	/// </summary>
	protected void GroundGuardAct(float Proballity = 100)
	{

			//�������K�[�h�ǂ�����H
			guardProballity = Proballity;
			guardJudge = true;
	}

		public void GuardJudge()
        {
			if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal && _controller.State.IsGrounded && guardJudge)
			{

					if (RandomValue(0, 100) >= (100 - guardProballity))
					{
						//flipWaitTime = 100;
					//	ground = EnemyStatus.MoveState.stay;
							Debug.Log("��");
						_guard.ActGuard();

					}
					
					guardJudge = false;

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

				GetAllChildren(spriteList[mattControllNum]);
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



		/// <summary>
		/// �}�e���A�����擾������
		/// Weapon�͕����T���t���O
		/// �X�v���C�g���X�g�̊O���畐��������Ă���
		/// </summary>
		/// <param name="parent">�}�e���A�������W����I�u�W�F�N�g�̐e</param>
		/// <param name="transforms">�g�p���郊�X�g</param>
		/// <param name="Weapon">��ԏ�̃I�u�W�F�N�g�ł��邩�ǂ����B�I�������X�v���C�g���X�g����Ȃ��Ƃ��ɕ���̃}�e���A��������</param>
	private void GetAllChildren(Transform parent, bool Weapon = false)
	{
		for (int i = 0; i < parent.childCount;i++)
		{
				//�܂��q�I�u�W�F�N�g���g�����X�t�H�[���̃��X�g�ɒǉ�
				//�q�I�u�W�F�N�g�̎q�I�u�W�F�N�g�܂ŒT��

				Transform child = parent.GetChild(i);

			GetAllChildren(child, true);
			//�����_���[�����o���i���Ƃł����𑀍삷��j
			Renderer sr = child.gameObject.MMGetComponentNoAlloc<Renderer>();
			if (sr != null)
			{
				//���X�g�ɒǉ�
				//Debug.Log(sr.name);
				controllTarget.Add(sr);
			}
		}
		
		//��ԏ�̃I�u�W�F�N�g�̎�����Ə���T��
		if (!Weapon)
		{
			Transform die = transform.MMFindDeepChildBreadthFirst("Attack");
			if (die != null)
			{
				GetAllChildren(die, true);
			}
			die = transform.Find("Guard");
			if (die != null)
			{
				GetAllChildren(die, true);
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





		/// <summary>
		/// �U����ԊJ�n
		/// </summary>
		public void StartCombat()
        {
			//�U���s�����B
			//�����ɓG�Ƃ̋����Ƃ����f����HorizontalMove�Ƃ��N�����郁�\�b�h���B
			posiReset = true;
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

			//�퓬�J�n����direction�͂O�ł�

			targetPosition = GManager.instance.Player.transform.position;
			distance = targetPosition - (Vector2)transform.position;
			//Debug.Log($"�m�肽���̂�{targetPosition.x}");
			direction = distance.x >= 0 ? 1 : -1;
			isAggressive = true;
			NormalFlip(direction);
			_sensor.RangeChange();
		}

		/// <summary>
		/// �퓬�I��
		/// </summary>
		public void CombatEnd()
        {
			Debug.Log("���񂶂�");
			isAggressive = false;
			if (status.kind == EnemyStatus.KindofEnemy.Fly)
			{
				_flying.FastFly(false, true);
				_flying.FastFly(true, true);
			}
			else
			{
				_characterRun.RunStop();
				_guard.GuardEnd();
			}
			ground = EnemyStatus.MoveState.wakeup;
			air = EnemyStatus.MoveState.wakeup;
			_sensor.RangeChange();
		}




		/// <summary>
		///  �K�v�ȃA�j���[�^�[�p�����[�^�[������΁A�A�j���[�^�[�p�����[�^�[���X�g�ɒǉ����܂��B
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			//    RegisterAnimatorParameter(_attackParameterName, AnimatorControllerParameterType.Bool, out _attackAnimationParameter);
			RegisterAnimatorParameter(_suppleParameterName, AnimatorControllerParameterType.Int, out _suppleAnimationParameter);
			RegisterAnimatorParameter(_combatParameterName, AnimatorControllerParameterType.Bool, out _combatAnimationParameter);
		}

		/// <summary>
		/// �A�r���e�B�̃T�C�N�����I���������_�B
		/// ���݂̂��Ⴊ�ށA�����̏�Ԃ��A�j���[�^�[�ɑ���B
		/// </summary>
		public override void UpdateAnimator()
		{
			//���̃X�e�[�g��Attack�ł��邩�ǂ�����Bool����ւ��Ă�

			MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _suppleAnimationParameter,suppleNumber, _character._animatorParameters);
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _combatAnimationParameter, isAggressive, _character._animatorParameters);
		}





	}
}

