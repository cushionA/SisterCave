
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using MoreMountains.CorgiEngine;
using Guirao.UltimateTextDamage;
using MoreMountains.Tools;
using static AnyPortrait.apMaterialSet;
using UnityEditor.U2D.Animation;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
	public class PlyerController : ControllAbillity 
	{ 

    //必要な処理書き出し
    //攻撃受けた時のダメージ回避。重力変化。
    //ステータス設定とかダメージ倍率などの管理、被弾時攻撃時のCalc系の処理
    //あと会話あたりの処理？
	protected UltimateTextDamageManager um;
		[HideInInspector]
		public Animator anim;
        /// <summary>
        /// 攻撃倍率
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

		protected float attackBuff = 1;//攻撃倍率
        #endregion


        //新パラメータ
        #region
        /// the number of jumps to perform while in this state
        //	[Tooltip("the number of jumps to perform while in this state")]
        //	public int NumberOfJumps = 1;

        ///<summary>
        ///どちらの方向を向くかのあれ。
        /// </summary>
        int horizontalDirection;


		//横移動は継承もとにあるよん


		#endregion
		//内部処理に使うやつ
		#region
		protected float recoverTime;//アーマー回復までにかかる時間
		protected int lastArmor = 1;
		protected bool isDamage;
		protected float nowArmor;
		#endregion


		//プレイヤーのアビリティ管理に使う
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

		[SerializeField]
		GameObject eController;

		[SerializeField]
		EnemyController eCon;

		protected override void Initialization()
		{
			base.Initialization();
			anim = _animator;

			
		//	SetComponennt();
			GManager.instance.StatusSetting();
           
			ParameterSet(GManager.instance.pStatus);

			//Background analysis of asset and meta files has been disabled, because this project seems too large. Do not warn me again 

			if (!GManager.instance.twinHand)
            {
				GManager.instance.AnimationSetting();
				//Debug.Log($"asidk");
			}
			ArmorReset();

			//	rb = this.gameObject._character.FindAbility<Rigidbody2D>();
			GManager.instance.HPReset();

			//_characterHorizontalMovement.FlipCharacterToFaceDirection = false;
			//parentMatt = _character.FindAbility<SpriteRenderer>().material;
			//td = _character.FindAbility<TargetDisplay>();
		}


		public void ParameterSet(PlayerStatus status)
	{

			
			if(status == null)
            {
				Debug.Log("ｓｆｆｇ");
				return;
            }
            else if(_controller == null)
            {
				Debug.Log("ｈでｒｈふせｒｌｆｐ＠");
            }

			_health.MaximumHealth = GManager.instance.maxHp;
			_health.InitialHealth = _health.MaximumHealth;
			_health.CurrentHealth = _health.MaximumHealth;

			///<summary>
			///　リスト
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
	//　読み取り専用。コードから変えてね
	[MMReadOnly]
	[Tooltip("水平方向の移動に適用する倍率")]
	public float MovementSpeedMultiplier = 1f;
	/// the multiplier to apply to the horizontal movement, dedicated to abilities
	[MMReadOnly]
	[Tooltip("水平方向の移動に適用する倍率で、アビリティに特化しています。")]
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
	//  これが真の場合、入力ソースからの入力を取得します。そうでない場合は、SetHorizontalMove() で設定する必要があります。
	[Tooltip("if this is true, will get input from an input source, otherwise you'll have to set it via SetHorizontalMove()")]
	public bool ReadInput = true;
	/// if this is true, no acceleration will be applied to the movement, which will instantly be full speed (think Megaman movement). Attention : a character with instant acceleration won't be able to get knockbacked on the x axis as a regular character would, it's a tradeoff
	/// これが真の場合、加速度は適用されず、瞬時に全速力となります（ロックマンの動きを想像してください）。
	/// 注意：瞬間的な加速を持つキャラクターは、通常のキャラクターのようにX軸でノックバックを受けることはできません。
	[Tooltip("if this is true, no acceleration will be applied to the movement, which will instantly be full speed (think Megaman movement). Attention : a character with instant acceleration won't be able to get knockbacked on the x axis as a regular character would, it's a tradeoff")]
	public bool InstantAcceleration = false;
	/// the threshold after which input is considered (usually 0.1f to eliminate small joystick noise)
	/// 入力を考慮する閾値（小さなジョイスティックノイズを除去するため通常0.1f）(検知しない値を確認)
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
	/// キャラクターが地面に衝突したときに再生されるMMFeedbacks
	[Tooltip("the MMFeedbacks to play when the character hits the ground")]
	public MMFeedbacks TouchTheGroundFeedback;
	/// the duration (in seconds) during which the character has to be airborne before a feedback can be played when touching the ground
	/// キャラクタが空中にいる間、地面に触れてもフィードバックが再生されるまでの時間（秒）です。
	[Tooltip("the duration (in seconds) during which the character has to be airborne before a feedback can be played when touching the ground")]
	public float MinimumAirTimeBeforeFeedback = 0.2f;

	[Header("Walls")]
	/// Whether or not the state should be reset to Idle when colliding laterally with a wall
	/// 壁に横から衝突したときに状態をIdleに戻すかどうか
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
					 /// 何秒転がる
	[Tooltip("ローリング時間")]
	public float RollDuration = 0.5f;
	/// the speed of the roll (a multiplier of the regular walk speed)
	[Tooltip("転がる速さが通常の歩く速さの何倍か")]
	public float RollSpeed = 3f;
	/// if this is true, horizontal input won't be read, and the character won't be able to change direction during a roll
	[Tooltip("trueの場合、水平方向の入力は読み込まれず、ロール中に方向を変えることはできません。")]
	public bool BlockHorizontalInput = false;
	/// if this is true, no damage will be applied during the roll, and the character will be able to go through enemies
	/// //このパラメーターがかかわる処理を見れば無敵処理がわかる
	[Tooltip("trueの場合、ロール中にダメージが与えられず、敵をスルーできるようになります。")]
	public bool PreventDamageCollisionsDuringRoll = false;

	//方向
	[Header("Direction")]

	/// the roll's aim properties
	[Tooltip("the roll's aim properties")]
	public MMAim Aim;
	/// the minimum amount of input required to apply a direction to the roll
	[Tooltip(" ロールに方向を与えるために必要な最小限の入力量")]
	public float MinimumInputThreshold = 0.1f;
	/// if this is true, the character will flip when rolling and facing the roll's opposite direction
	[Tooltip("これが真なら、キャラクターはロール時に反転し、ロールの反対側を向きます。")]
	public bool FlipCharacterIfNeeded = true;

	//これコルーチンなんだ
	public enum SuccessiveRollsResetMethods { Grounded, Time }

	[Header("Cooldown")]
	/// the duration of the cooldown between 2 rolls (in seconds)
	[Tooltip("次のローリングまでに必要な時間")]
	public float RollCooldown = 1f;

	[Header("Uses")]
	/// whether or not rolls can be performed infinitely
	[Tooltip("無限にローリングできるか")]
	public bool LimitedRolls = false;
	/// the amount of successive rolls a character can perform, only if rolls are not infinite
	[Tooltip("the amount of successive rolls a character can perform, only if rolls are not infinite")]
	[MMCondition("LimitedRolls", true)]
	public int SuccessiveRollsAmount = 1;
	/// the amount of rollss left (runtime value only), only if rolls are not infinite
	[Tooltip("ローリングの残り回数")]
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

			//rollSpeedはローリングのスピードが通常移動の何倍か
			_rolling.RollSpeed = GManager.instance.pStatus.avoidSpeed;
			_rolling.BlockHorizontalInput = true;
			_rolling.PreventDamageCollisionsDuringRoll = true;
			_rolling.RollCooldown = GManager.instance.pStatus.avoidCool;


		}
			///<summary>
			///ジャンプ設定
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
		[Tooltip("ジャンプボタンが離されたときに、現在の速度を変更する量です。")]
		public float JumpReleaseForceFactor = 2f;

		[Header("Quality of Life")]

		/// a timeframe during which, after leaving the ground, the character can still trigger a jump
		[Tooltip("a timeframe during which, after leaving the ground, the character can still trigger a jump")]
		public float CoyoteTime = 0f;

		/// if the character lands, and the jump button's been pressed during that InputBufferDuration, a new jump will be triggered 
		[Tooltip("キャラクターが着地し、そのInputBufferDurationの間にジャンプボタンが押された場合、新しいジャンプが開始されます。")]
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

				//ジャンプの速さとかは重力で決まる
				//ジャンプ中だけHorizontalSpeed変えてもろて
				//水平移動速度変わる処理入れる、いやいらんか
				//普通に横移動速度の何割がちょうどいいかで決めよ
			}
			#endregion


			///<summary>
			///起き上がり設定
			///</summary>
			#region
			if (_wakeup != null)
			{

			}
			#endregion

		}


        public override ConditionData ConditionDataSet()
        {
            ConditionData _data = new ConditionData();

			//    _data.hpRatio = _health.CurrentHealth / status.maxHp;
			return _data;
        }


        public override CharacterStatus.CharacterData CharacterDataSet()
        {
            CharacterStatus.CharacterData _data = new CharacterStatus.CharacterData();

            //    _data.hpRatio = _health.CurrentHealth / status.maxHp;
            return _data;
        }


        //汎用行動の判断
        protected override void HandleInput()
		{

			//	Debug.Log($"ｄｄｋｄｌ{_inputManager.CombinationButton.State.CurrentState}");


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
            if (_health.CurrentHealth <= 0)
            {
				Debug.Log($"死んだ{_condition.CurrentState}");
            }
			if (_controller.State.JustGotGrounded)
            {
				GManager.instance.PlaySound(MyCode.SoundManager.instance.armorShakeSound[2], transform.position);
			}
		}


        #region イベント関係


        /// <summary>
        /// ターゲットリストから削除されたエネミーを消し去る
        /// そしてヘイトリストやらを調整
        /// プレイヤーはなんか別の処理入れてもいいかもな
        /// あと敵の死を通知するメソッドとしても使える
        /// </summary>
        /// <param name="deletEnemy"></param>
        public override void TargetListChange(int deleteEnemy)
		{

		}

        #endregion


        #region 体力・ダメージ関連


        public override void DamageEvent(bool isStun, GameObject enemy)
		{

		}

        /// <summary>
        /// 死亡アニメーションを再生する
        /// 吹っ飛んで死ぬのか普通に死ぬのか
        /// </summary>
        /// <param name="stunState"></param>
        public override void DeadMotionStart(MyWakeUp.StunnType stunState)
        {
            _wakeup.StartStunn(stunState);
        }
        ///現在の体力
        public int ReturnHealth()
        {
			return (int)_health.CurrentHealth;
        }
        public void HPReset()
        {
			_health.CurrentHealth = GManager.instance.pStatus.maxHp;
        }

       public void testReset()
        {
            if (eCon.EnemyExist())
            {
				eCon.ResetEnemy();
            }
			transform.position = new Vector2(182.8f,transform.position.y + 10);
			SManager.instance.Sister.transform.position = new Vector2(187.8f, transform.position.y + 10);
			if (SManager.instance.Sister.activeSelf)
			{
			SManager.instance.Sister.GetComponent<BrainAbility>().MPReset();
			}


		}
		/// <summary>
		/// 死亡時の処理
		/// </summary>
        public override void Die()
        {
			testReset();
        }

        #endregion





        ///ダメージ計算関連
        #region



        /// <summary>
        /// ダメージ計算
        /// </summary>
        /// <param name="isFriend">真なら味方</param>
        public override void DamageCalc()
	{
		//GManager.instance.isDamage = true;
		//useEquip.hitLimmit--;
		//mValueはモーション値

			Equip useEquip;
			bool isShield = GManager.instance.useAtValue.isShield;


            if (isShield)
            {
				useEquip = GManager.instance.equipShield;
				
            }
            else
            {
				useEquip = GManager.instance.equipWeapon;
            }
			
			_damage._attackData._attackType = GManager.instance.useAtValue.mainElement;
			_damage._attackData.phyType = GManager.instance.useAtValue.phyElement;

			if (useEquip.phyAtk > 0)
			{
				_damage._attackData.phyAtk = useEquip.phyAtk * attackFactor;


				//						Debug.Log("皿だ");
				if (GManager.instance.useAtValue.z >= 40)
				{
					_damage._attackData.isHeavy = true;
				}
				else
				{
					_damage._attackData.isHeavy = false;
				}
			}
			//神聖
			if (useEquip.holyAtk > 0)
			{
				_damage._attackData.holyAtk = useEquip.holyAtk * holyATFactor;

			}
			//闇
			if (useEquip.darkAtk > 0)
			{
				_damage._attackData.darkAtk = useEquip.darkAtk * darkATFactor;

			}
			//炎
			if (useEquip.fireAtk > 0)
			{
				_damage._attackData.fireAtk = useEquip.fireAtk * fireATFactor;

			}
			//雷
			if (useEquip.thunderAtk > 0)
			{
				_damage._attackData.thunderAtk = useEquip.thunderAtk * thunderATFactor;

			}

			_damage._attackData.shock = GManager.instance.useAtValue.z;


		_damage._attackData.attackBuff = attackBuff;
		//	_damage._attackData.disParry = GManager.instance.useAtValue.disParry;
			_damage._attackData.mValue = GManager.instance.useAtValue.x;
			_damage._attackData.isBlow = GManager.instance.useAtValue.isBlow;
		_damage._attackData.isLight = GManager.instance.useAtValue.isLight;
		_damage._attackData.blowPower.Set(GManager.instance.useAtValue.blowPower.x, GManager.instance.useAtValue.blowPower.y);
	}

        /// <summary>
        /// 自分のダメージ中フラグ立ててこちらの防御力を教えてあげるの
        /// </summary>
        public override void DefCalc()
        {
            bool isTwinHand = !GManager.instance.twinHand;
            //	Debug.Log("あああああぢいいぢぢぢぢｄ");
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

        public override void GuardReport()
        {
			_health._defData.isGuard = _movement.CurrentState == CharacterStates.MovementStates.Guard ? true : false;
		}


		/// <summary>
		/// 全てのアビリティをキャンセル
		/// </summary>
		public void MoveReset()
        {
		//	Debug.Log("oooooo");
			_attack.AttackEnd();
			_guard.GuardEnd();

        }



        /// <summary>
        /// 攻撃がヒットした時にどのようにヒットしたかを含めて教える
        /// </summary>
        /// <param name="isBack">当てた相手が自分の後ろにいる時は真</param>
        public override void HitReport(bool isBack)
        {

        }


        #endregion

        //
        #region　ステータス関連


        /// <summary>
        /// バフの数値を与える
        /// 弾丸から呼ぶ
        /// </summary>
        public override void BuffCalc(FireBullet _fire)
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
		/// スタミナ切れた時のアビリティ無効化
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

		/// <summary>
		/// いやインスペクタでやれよ
		/// </summary>
        public void SetComponennt()
        {
			_jump = _character.FindAbility<PlayerJump>();
			_running = _character.FindAbility<PlayerRunning>();
			_rolling  = _character.FindAbility<PlayerRoll>();
			_attack = _character.FindAbility<WeaponAbillity>();
			_guard = _character.FindAbility<GuardAbillity>();
			_wakeup = _character.FindAbility<MyWakeUp>();
			_damage = GetComponentInParent<MyDamageOntouch>();
		}


        #region アーマー関連

        public void ArmorRecover()
        {
            //	Debug.Log($"今のアーマー{nowArmor}");
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
        /// アーマーをリセット
        /// </summary>
        public override void ArmorReset()
        {
            nowArmor = GManager.instance.Armor;
        }

        /// <summary>
        /// アーマー値に応じてイベントとばす
        /// スタン攻撃中断もここで
        /// </summary>
        public override MyWakeUp.StunnType ArmorControll(float shock, bool isDown, bool isBack)
        {
            bool isShield = !GManager.instance.twinHand;
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

                _guard.GuardEnd();
            }

            if (!isBack && (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove || _health._guardAttack))
            {
                if (!_health._guardAttack)
                {
                    _guard.GuardHit();
                }
                GManager.instance.stamina -= (shock * 3) * (1 - (useEquip.guardPower / 100));
                if (GManager.instance.stamina <= 0)
                {
                    if (!_health._guardAttack)
                    {
                        _guard.GuardEnd();
                    }

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
                    //攻撃アーマーの数値がアーマー削りより大きいならアーマーの数値は変わらない。
                    //攻撃アーマーの数値引いた分アーマー削りでアーマーを削る
                    nowArmor -= (shock - GManager.instance.useAtValue.y) < 0 ? 0 : (shock - GManager.instance.useAtValue.y);
                    //攻撃アーマーも一応削る
                    GManager.instance.useAtValue.y = (GManager.instance.useAtValue.y - shock) < 0 ? 0 : GManager.instance.useAtValue.y - shock;

                }
            }

            if (nowArmor <= 0)
            {
                if (isDown)
                {
                    //Debug.Log($"ｓｓｓｓｓｓｓｓｓｓｓ{_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove}");
                    result = (MyWakeUp.StunnType.Down);
                }
                else
                {

                    if (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove)
                    {
                        result = MyWakeUp.StunnType.GuardBreake;
                    }
                    //パリィは別発生
                    else
                    {
                        result = MyWakeUp.StunnType.Falter;

                    }

                }

            }
            else
            {
                result = MyWakeUp.StunnType.notStunned;
            }


            return result;
        }

        /// <summary>
        /// 現在のスタン状況を返す
        /// </summary>
        /// <returns></returns>
        public override int GetStunState()
        {
            return _wakeup.GetStunState();
        }



        /// <summary>
		/// 自分がパリィされた時アーマーブレイクするかどうかを伝える
        /// プレイヤーはジャスガ一発でパリィにする？
        /// </summary>
        public override bool ParryArmorJudge()
        {
            return true;
            /*
            nowArmor -= Mathf.Ceil(status.Armor * ((100 - atV.parryResist) / 100));

            if (nowArmor <= 0)
            {
                AttackEnd(true);
                //	_wakeup.StartStunn(MyWakeUp.StunnType.Parried);
                return true;
            }
            else
            {
                Debug.Log($"あと{nowArmor}");
                return false;
            }*/
        }

        /// <summary>
        /// 空中で攻撃を受けた時ダウンするかどうかの判断
        /// </summary>
        /// <param name="stunnState"></param>
        /// <returns></returns>
        public override bool AirDownJudge(MyWakeUp.StunnType stunnState)
        {
            if (stunnState == MyWakeUp.StunnType.Falter)
            {
                //      stunnState = MyWakeUp.StunnType.Down;
                //吹き飛ばし処理
                return true;
            }
            else if (stunnState == MyWakeUp.StunnType.notStunned)
            {
                //攻撃中でなければ
                if (!AttackCheck())
                {
                    return true;
                    //吹き飛ばし処理
                }
                //攻撃中なら吹き飛ばない
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region　攻撃関連




        /// <summary>
        /// ヘルスのために攻撃状態か否かを返す
        /// アーマー0の攻撃は落とされてもいいかも
        /// それは普通に特殊ダウンしなくても攻撃で削られるかな
        /// </summary>
        /// <returns></returns>
        //
        public bool AttackCheck()
        {
			return _movement.CurrentState == CharacterStates.MovementStates.Attack;
        }

        public void WeaponChange()
		{

			if (_inputManager.WeaponChangeButton.State.CurrentState == MMInput.ButtonStates.ButtonDown && _condition.CurrentState == CharacterStates.CharacterConditions.Normal)
			{
				//_weapon.AttackEnd();
				_condition.ChangeState(CharacterStates.CharacterConditions.Moving);
				//このフラグは武器切り替えか両手持ち切り替えかで区別するもの
				//武器切り替え後は一回だけボタン離しても持ち手変更が反応しないようにする

			}
            else
            {

				return;
            }

			//武器入れ替え
			if (_inputManager.sAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed)
			{
				GManager.instance.EquipSwap(1);
			}
			//盾入れ替え
			else if (_inputManager.bAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed)
			{
				GManager.instance.EquipSwap(2);
			}
			//両手持ち切り替え
            else
            {
				GManager.instance.EquipSwap(0);
            }
			if (_condition.CurrentState != CharacterStates.CharacterConditions.Stunned)
			{
				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
			}


		}

		/// <summary>
		/// パリィモーション開始
		/// </summary>
		/// <param name="isBreake"></param>
		public override void ParryStart(bool isBreake)
		{

            if (!GManager.instance.equipWeapon.twinHand)
            {
                GManager.instance.stamina += GManager.instance.equipShield.parryRecover;
            }
            else
            {
                GManager.instance.stamina += GManager.instance.equipWeapon.parryRecover;
            }
            _parry.ParryStart(isBreake);
			_guard.GuardEnd();
		}





        #endregion

        //アニメーションイベント
        #region
        public void AtackContinue()
        {

			_weapon.Continue();

        }

        #endregion

		//プレイヤー制御
        #region

		/// <summary>
		/// 重力設定
		/// </summary>
		/// <param name="gravity"></param>
		public void GravitySet(float gravity)
		{
			//rb.gravityScale = gravity;
			_controller.DefaultParameters.Gravity = -gravity;
		}

        /// <summary>
        /// 外側から向きを変える。
        /// コンビネーションなどに
        /// </summary>
        public void PlayerFlip()
        {
			_character.Flip();
        }

		/// <summary>
		/// trueでロック、falseで解除
		/// </summary>
		public void EventLock(bool Lock)
        {
            if (Lock)
            {

				_characterHorizontalMovement.SetHorizontalMove(0);
				_controller.SetForce(Vector2.zero);
				if(_movement.CurrentState == CharacterStates.MovementStates.Attack)
                {
					return;
                }

				if (_controller.State.IsGrounded)
				{
					_movement.ChangeState(CharacterStates.MovementStates.Idle);
				}
                else
                {
					_movement.ChangeState(CharacterStates.MovementStates.Falling);
				}
				_condition.ChangeState(CharacterStates.CharacterConditions.Moving);
            }
            else
            {
				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
			}
			
        }

		/// <summary>
		/// プレイヤーを停止させる
		/// </summary>
		public void PlayerStop()
        {
			_controller.SetForce(Vector2.zero);
        }


        public override void StartStun(MyWakeUp.StunnType stunState)
        {
            GravitySet(GManager.instance.pStatus.firstGravity);
            MoveReset();
            _wakeup.StartStunn(stunState);
        }


        #endregion

        //状態確認
        #region

        //真なら停止中
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

		/// <summary>
		/// 体力が何割あるか知らせる
		/// </summary>
		/// <returns></returns>
		public float HPRatio()
        {
		    return _health.CurrentHealth / _health.MaximumHealth;
		}


		#endregion

		//アニメイベント
		//音とエフェクト
		#region


		public void attackEffect()
		{
		//	if (!string.IsNullOrEmpty(GManager.instance.useAtValue.attackEffect.AssetGUID))
		//	{

		//		Addressables.InstantiateAsync(GManager.instance.useAtValue.attackEffect, eController.transform);
		//	}
		}






		public override void GuardSound()
		{


			if (GManager.instance.twinHand)
			{
				MyCode.SoundManager.instance.GuardSound(GManager.instance.equipWeapon.isMetal,GManager.instance.equipWeapon.shieldType,transform.position);
			}
            else
            {
				MyCode.SoundManager.instance.GuardSound(GManager.instance.equipShield.isMetal, GManager.instance.equipShield.shieldType, transform.position);
			}
		}


		/// <summary>
		/// 燃えてる剣だったりして音が聞こえる場合
		/// </summary>
		/// <param name="useSoundNum"></param>
		/// <param name="isChase"></param>
		public void LeftSound(int useSoundNum, bool isChase = false)
		{

		}



		#endregion



	




	}
}