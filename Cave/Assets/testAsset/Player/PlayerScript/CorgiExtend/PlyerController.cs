
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using MoreMountains.CorgiEngine;
using Guirao.UltimateTextDamage;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
	public class PlyerController : MyAbillityBase
{
    //�K�v�ȏ��������o��
    //�U���󂯂����̃_���[�W����B�d�͕ω��B
    //�X�e�[�^�X�ݒ�Ƃ��_���[�W�{���Ȃǂ̊Ǘ��A��e���U������Calc�n�̏���
    //���Ɖ�b������̏����H
	protected UltimateTextDamageManager um;
		[HideInInspector]
		public Animator anim;
        /// <summary>
        /// �U���{��
        /// </summary>
        #region
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

		protected float attackBuff = 1;//�U���{��
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


		#endregion
		//���������Ɏg�����
		#region
		protected float recoverTime;//�A�[�}�[�񕜂܂łɂ����鎞��
		protected int lastArmor = 1;
		protected bool isDamage;
		protected float nowArmor;
		#endregion


		//�v���C���[�̃A�r���e�B�Ǘ��Ɏg��
		#region
		[SerializeField]
		protected PlayerRoll _rolling;
		[SerializeField]
		protected PlayerRunning _running;
		[SerializeField]
		protected GuardAbillity _guard;
		[SerializeField]
		protected PlayerJump _jump;
		[SerializeField]
		protected WeaponAbillity _weapon;
		public MyWakeUp _wakeup;
		[SerializeField]
		protected WeaponAbillity _attack;
		[SerializeField]
		protected MyDamageOntouch _damage;

		[SerializeField]
		protected PlayerCrouch _squat;

		[SerializeField]
		protected ParryAbility _parry;

        #endregion



        protected override void Initialization()
		{
			base.Initialization();
			anim = _animator;
           
			ParameterSet(GManager.instance.pStatus);
			
		//	SetComponennt();
			GManager.instance.StatusSetting();
            if (!GManager.instance.twinHand)
            {
				GManager.instance.AnimationSetting();
				//Debug.Log($"asidk");
			}
			ArmorReset();

			//	rb = this.gameObject.GetComponent<Rigidbody2D>();
			GManager.instance.HPReset();

			//_characterHorizontalMovement.FlipCharacterToFaceDirection = false;
			//parentMatt = GetComponent<SpriteRenderer>().material;
			//td = GetComponent<TargetDisplay>();
		}


		public void ParameterSet(PlayerStatus status)
	{

			
			if(status == null)
            {
				Debug.Log("��������");
				return;
            }
            else if(_controller == null)
            {
				Debug.Log("���ł����ӂ�����������");
            }

			_health.MaximumHealth = GManager.instance.maxHp;
			_health.InitialHealth = _health.MaximumHealth;
			_health.CurrentHealth = _health.MaximumHealth;

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
			_characterHorizontalMovement.MovementSpeed = status.moveSpeed;
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
			_characterHorizontalMovement.WalkSpeed = status.moveSpeed;
			if (_running != null)
			{
				_running.RunSpeed = status.dashSpeed;
			}



		if (_rolling != null)
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

			_rolling.RollDuration = GManager.instance.pStatus.avoidRes;
			_rolling.RollSpeed = GManager.instance.pStatus.avoidSpeed;
			_rolling.BlockHorizontalInput = true;
			_rolling.PreventDamageCollisionsDuringRoll = true;
			_rolling.RollCooldown = GManager.instance.pStatus.avoidCool;


		}
			///<summary>
			///�W�����v�ݒ�
			/// </summary>
			#region
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
		[Tooltip("�W�����v�{�^���������ꂽ�Ƃ��ɁA���݂̑��x��ύX����ʂł��B")]
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
				_jump.CoyoteTime = GManager.instance.pStatus.jumpCool;
				_jump.JumpHeight = GManager.instance.pStatus.jumpRes;
				_jump.NumberOfJumps = GManager.instance.pStatus.jumpLimit;

				//�W�����v�̑����Ƃ��͏d�͂Ō��܂�
				//�W�����v������HorizontalSpeed�ς��Ă����
				//�����ړ����x�ς�鏈�������A���₢���
				//���ʂɉ��ړ����x�̉��������傤�ǂ������Ō��߂�
			}
			#endregion


			///<summary>
			///�N���オ��ݒ�
			///</summary>
			#region
			if (_wakeup != null)
			{

			}
			#endregion

		}




		//�ėp�s���̔��f
		protected override void HandleInput()
		{

			//	Debug.Log($"����������{_inputManager.CombinationButton.State.CurrentState}");


				if (
				_movement.CurrentState == CharacterStates.MovementStates.Guard ||
				_movement.CurrentState == CharacterStates.MovementStates.GuardMove ||
				_movement.CurrentState == CharacterStates.MovementStates.Warp ||
				_condition.CurrentState != CharacterStates.CharacterConditions.Normal
				)
            {
				return;
            }
			WeaponChange();
        }




        public override void ProcessAbility()
        {
            base.ProcessAbility();

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

		///�_���[�W�v�Z�֘A
        #region

		///���݂̗̑�
		public int ReturnHealth()
        {
			return (int)_health.CurrentHealth;
        }

        /// <summary>
        /// �_���[�W�v�Z
        /// </summary>
        /// <param name="isFriend">�^�Ȃ疡��</param>
        public void DamageCalc(bool isShield)
	{
		//GManager.instance.isDamage = true;
		//useEquip.hitLimmit--;
		//mValue�̓��[�V�����l



			Equip useEquip;

            if (isShield)
            {
				useEquip = GManager.instance.equipShield;
				GManager.instance.useAtValue.isShield = false;
            }
            else
            {
				useEquip = GManager.instance.equipWeapon;
            }

			if (useEquip.phyAtk > 0)
		{
			_damage._attackData.phyAtk = useEquip.phyAtk * attackFactor;

                //�a���h�ˑŌ����Ǘ�
                if (GManager.instance.useAtValue.type == Equip.AttackType.Slash)
                {
                    _damage._attackData._attackType = 0;
                }
                else if (GManager.instance.useAtValue.type == Equip.AttackType.Stab)
                {
                    _damage._attackData._attackType = 2;
                }
                else if (GManager.instance.useAtValue.type == Equip.AttackType.Strike)
                {


                    _damage._attackData._attackType = 4;

                    //						Debug.Log("�M��");
                    if (GManager.instance.useAtValue.z >= 40)
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
            if (useEquip.holyAtk > 0)
		{
			_damage._attackData.holyAtk = useEquip.holyAtk * holyATFactor;

		}
		//��
		if (useEquip.darkAtk > 0)
		{
			_damage._attackData.darkAtk = useEquip.darkAtk * darkATFactor;

		}
		//��
		if (useEquip.fireAtk > 0)
		{
			_damage._attackData.fireAtk = useEquip.fireAtk * fireATFactor;

		}
		//��
		if (useEquip.thunderAtk > 0)
		{
			_damage._attackData.thunderAtk = useEquip.thunderAtk * thunderATFactor;

		}
		_damage._attackData.shock = GManager.instance.useAtValue.z;


		_damage._attackData.attackBuff = attackBuff;
			//damage = Mathf.Floor(damage * attackBuff);
			_damage._attackData.mValue = GManager.instance.useAtValue.x;
			_damage._attackData.isBlow = GManager.instance.useAtValue.isBlow;
		_damage._attackData.isLight = GManager.instance.useAtValue.isLight;
		_damage._attackData.blowPower.Set(GManager.instance.useAtValue.blowPower.x, GManager.instance.useAtValue.blowPower.y);
	}

	/// <summary>
	/// �����̃_���[�W���t���O���ĂĂ�����̖h��͂������Ă������
	/// </summary>
	public void DefCalc(bool isTwinHand)
	{
		//	Debug.Log("��������������������������");
			Equip useEquip;
            if (isTwinHand)
			{
				useEquip = GManager.instance.equipShield;
			}
			else
			{
				useEquip = GManager.instance.equipWeapon;
			}


			_health.InitialHealth = (int)GManager.instance.maxHp;
		_health._defData.Def = GManager.instance.Def;
		_health._defData.pierDef = GManager.instance.pierDef;
		_health._defData.strDef = GManager.instance.strDef;
		_health._defData.fireDef = GManager.instance.fireDef;
		_health._defData.holyDef = GManager.instance.holyDef;
		_health._defData.darkDef = GManager.instance.darkDef;

		_health._defData.phyCut = useEquip.phyCut;
		_health._defData.fireCut = useEquip.fireCut;
		_health._defData.holyCut = useEquip.holyCut;
		_health._defData.darkCut = useEquip.darkCut;

		_health._defData.guardPower = useEquip.guardPower;

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

		public void ArmorRecover()
		{
			//	Debug.Log($"���̃A�[�}�[{nowArmor}");
			if (_condition.CurrentState != CharacterStates.CharacterConditions.Stunned && !isDamage && _movement.CurrentState != CharacterStates.MovementStates.Attack)
			{
				recoverTime += _controller.DeltaTime;
				if (recoverTime >= 15 || nowArmor > GManager.instance.Armor)
				{
					ArmorReset();
					recoverTime = 0.0f;
					lastArmor = 1;
					//	lastArmor = nowArmor; 
				}
				else if (nowArmor < GManager.instance.Armor && recoverTime >= 3 * lastArmor)
				{
					//recoverTime = 0.0f;
					nowArmor += GManager.instance.Armor / 6;
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
		/// �A�[�}�[�����Z�b�g
		/// </summary>
		public void ArmorReset()
		{
			nowArmor = GManager.instance.Armor;
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

		#endregion

		//�X�e�[�^�X�֘A
		#region


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

		public void SetLayer(int layerNumber)
		{

			this.gameObject.layer = layerNumber;

		}

		/// <summary>
		/// �X�^�~�i�؂ꂽ���̃A�r���e�B������
		/// </summary>
		public void StaminaExhaust()
        {
            if (GManager.instance.isEnable)
            {

            }
            else
            {
				_rolling.AbilityPermitted = false;

				_running.AbilityPermitted = false;

				//_flying.AbilityPermitted = false;

				_guard.AbilityPermitted = false;

				_jump.AbilityPermitted = false;

				_weapon.AbilityPermitted = false;

				_wakeup.AbilityPermitted = false;

				_attack.AbilityPermitted = false;
			}
        }

        #endregion


        public void SetComponennt()
        {
			_jump = GetComponent<PlayerJump>();
			_running = GetComponent<PlayerRunning>();
			_rolling  = GetComponent<PlayerRoll>();
			_attack = GetComponent<WeaponAbillity>();
			_guard = GetComponent<GuardAbillity>();
			_wakeup = GetComponent<MyWakeUp>();
			_damage = GetComponentInParent<MyDamageOntouch>();
		}

		//�U�h�֘A
        #region


		/// <summary>
		/// �A�[�}�[�l�ɉ����ăC�x���g�Ƃ΂�
		/// �X�^���U�����f��������
		/// </summary>
		public MyWakeUp.StunnType ArmorControll(float shock, bool isDown, bool isBack,bool isShield)
		{
			MyWakeUp.StunnType result = 0;
			Equip useEquip;
			if (isShield)
			{
				useEquip = GManager.instance.equipShield;
			}
			else
			{
				useEquip = GManager.instance.equipWeapon;
			}
			if (_movement.CurrentState != CharacterStates.MovementStates.Attack)
			{
				GManager.instance.useAtValue.y = 0;
			}
			if ((_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove) && isBack)
			{
				Debug.Log("���������ł�");
				_guard.GuardEnd();
			}

			if (!isBack && (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove))
			{
				_guard.GuardHit();
				GManager.instance.stamina -= (shock * 3) * (1 - (useEquip.guardPower / 100));
                if (GManager.instance.stamina <= 0)
                {
					nowArmor = -1;
                }
			}
			else
			{
				if (GManager.instance.useAtValue.y == 0)
				{
					nowArmor -= shock;

				}
				else
				{
					//�U���A�[�}�[�̐��l���A�[�}�[�����傫���Ȃ�A�[�}�[�̐��l�͕ς��Ȃ��B
					//�U���A�[�}�[�̐��l���������A�[�}�[���ŃA�[�}�[�����
					nowArmor -= (shock - GManager.instance.useAtValue.y) < 0 ? 0 : (shock - GManager.instance.useAtValue.y);
					//�U���A�[�}�[���ꉞ���
					GManager.instance.useAtValue.y = (GManager.instance.useAtValue.y - shock) < 0 ? 0 : GManager.instance.useAtValue.y - shock;

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

					if (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove)
					{
						result = MyWakeUp.StunnType.GuardBreake;
					//	Debug.Log("dkdl");
					}
					//�p���B�͕ʔ���
					else
					{
						result = MyWakeUp.StunnType.Falter;
					//	Debug.Log("�������ł���");
					}

				}

			}
			else
			{
				result = MyWakeUp.StunnType.notStunned;
			}

			/*
			if (hp <= 0)
			{
				// _condition.ChangeState(CharacterStates.CharacterConditions.Dead);
				atBlock.gameObject.SetActive(false);
				//atBlock.gameObject.SetActive(false);
				isAnimeStart = false;

			}
			*/
			return result;
		}


        //�w���X�̂��߂ɍU����Ԃ��ۂ���Ԃ�
        public bool AttackCheck()
        {
			return _movement.CurrentState == CharacterStates.MovementStates.Attack;
        }

        public void WeaponChange()
		{

			if (_inputManager.WeaponChangeButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
			{
				_weapon.AttackEnd();
				_condition.ChangeState(CharacterStates.CharacterConditions.Moving);
				//���̃t���O�͕���؂�ւ������莝���؂�ւ����ŋ�ʂ������
				//����؂�ւ���͈�񂾂��{�^�������Ă�������ύX���������Ȃ��悤�ɂ���
				Debug.Log("���������������˂�");
			}
            else
            {

				return;
            }

			//�������ւ�
			if (_inputManager.sAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed)
			{
				GManager.instance.EquipSwap(1);
			}
			//������ւ�
			else if (_inputManager.bAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed)
			{
				GManager.instance.EquipSwap(2);
			}
			//���莝���؂�ւ�
            else
            {
				GManager.instance.EquipSwap(0);
            }
			if (_condition.CurrentState != CharacterStates.CharacterConditions.Stunned)
			{
				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
			}


		}


		public void ParryStart(int num)
		{
			_parry.ParryStart(num);
			_guard.GuardEnd();
		}

        #endregion

        //�A�j���[�V�����C�x���g
        #region
        public void AtackContinue()
        {

			_weapon.Continue();

        }
        #endregion

		//�v���C���[����
        #region

        /// <summary>
        /// �O�����������ς���B
        /// �R���r�l�[�V�����Ȃǂ�
        /// </summary>
        public void PlayerFlip()
        {
			_character.Flip();
        }

		/// <summary>
		/// true�Ń��b�N�Afalse�ŉ���
		/// </summary>
		public void EventLock(bool Lock)
        {
            if (Lock)
            {
				_condition.ChangeState(CharacterStates.CharacterConditions.Moving);
				_characterHorizontalMovement.SetHorizontalMove(0);
				_controller.SetForce(Vector2.zero);
            }
            else
            {
				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
			}
        }

        #endregion

        //��Ԋm�F
        #region

        //�^�Ȃ��~��
        public float NowSpeed()
        {
			return Mathf.Abs(_controller.Speed.x);
        }

		public bool CheckPLayerNeutral()
        {
            if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal && _controller.State.IsGrounded)
            {
				return true;
            }
            else
            {
				return false;
            }
        }


        #endregion




    }
}