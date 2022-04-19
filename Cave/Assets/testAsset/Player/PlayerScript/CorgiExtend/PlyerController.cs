
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using MoreMountains.CorgiEngine;
using Guirao.UltimateTextDamage;
namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
	public class PlyerController : CharacterAbility
{
    //必要な処理書き出し
    //攻撃受けた時のダメージ回避。重力変化。
    //ステータス設定とかダメージ倍率などの管理、被弾時攻撃時のCalc系の処理
    //あと会話あたりの処理？
	protected UltimateTextDamageManager um;



		/// <summary>
		/// 攻撃倍率
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

		protected float attackBuff = 1;//攻撃倍率
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

		protected PlayerJump _jump;
		//　protected int _numberOfJumps = 0;


		//　protected int _numberOfJumps = 0;

		protected PlayerRoll _rolling;

		protected CharacterRun _characterRun;

		protected EnemyFly _flying;

		protected GuardAbillity _guard;

		protected MyWakeUp _wakeup;

		protected WeaponAbillity _attack;

		protected MyDamageOntouch _damage;

		protected new MyHealth _health;


		//	protected Hittable _hitInfo;


		#endregion
		//内部処理に使うやつ
		#region
		protected float recoverTime;//アーマー回復までにかかる時間
		protected int lastArmor = 1;
		protected bool isDamage;
		protected float nowArmor;
		#endregion

		protected override void Initialization()
		{
			ParameterSet(GManager.instance.pStatus);
			ArmorReset();
			_health = (MyHealth)base._health;

			//	rb = this.gameObject.GetComponent<Rigidbody2D>();
			GManager.instance.HPReset();

			_characterHorizontalMovement.FlipCharacterToFaceDirection = false;
			//parentMatt = GetComponent<SpriteRenderer>().material;
			//td = GetComponent<TargetDisplay>();

			GManager.instance.SetParameter();
			GManager.instance.ActionSet();
			GManager.instance.SetAtk(GManager.instance.equipWeapon);
			GManager.instance.SetAtk(GManager.instance.equipShield);
			GManager.instance.SetGuard(GManager.instance.equipWeapon);
			GManager.instance.SetGuard(GManager.instance.equipShield);

			GManager.instance.SetMagicAssist();
			GManager.instance.SetMagicAtk();
		}


		protected void ParameterSet(PlayerStatus status)
	{
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
			_characterHorizontalMovement.WalkSpeed = GManager.instance.pStatus.moveSpeed;
			if (_characterRun != null)
			{
				_characterRun.RunSpeed = GManager.instance.pStatus.dashSpeed;
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
			_rolling.RollSpeed = GManager.instance.pStatus.avoidSpeed;
			_rolling.BlockHorizontalInput = true;
			_rolling.PreventDamageCollisionsDuringRoll = true;
			_rolling.RollCooldown = GManager.instance.pStatus.avoidCool;


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

		}

	}


		/// <summary>
		/// 重力設定
		/// </summary>
		/// <param name="gravity"></param>
		public void GravitySet(float gravity)
		{
			//rb.gravityScale = gravity;
			_controller.DefaultParameters.Gravity = gravity;
		}


		/// <summary>
		/// ダメージ計算
		/// </summary>
		/// <param name="isFriend">真なら味方</param>
		public void DamageCalc(bool isShield)
	{
		//GManager.instance.isDamage = true;
		//useEquip.hitLimmit--;
		//mValueはモーション値



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
			_damage._attackData.phyAtk = (Mathf.Pow(useEquip.phyAtk, 2) * GManager.instance.useAtValue.x) * attackFactor;

                //斬撃刺突打撃を管理
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
            }
            //神聖
            if (useEquip.holyAtk > 0)
		{
			_damage._attackData.holyAtk = (Mathf.Pow(useEquip.holyAtk, 2) * GManager.instance.useAtValue.x) * holyATFactor;

		}
		//闇
		if (useEquip.darkAtk > 0)
		{
			_damage._attackData.darkAtk = (Mathf.Pow(useEquip.holyAtk, 2) * GManager.instance.useAtValue.x) * darkATFactor;

		}
		//炎
		if (useEquip.fireAtk > 0)
		{
			_damage._attackData.fireAtk = (Mathf.Pow(useEquip.holyAtk, 2) * GManager.instance.useAtValue.x) * fireATFactor;

		}
		//雷
		if (useEquip.thunderAtk > 0)
		{
			_damage._attackData.thunderAtk = (Mathf.Pow(useEquip.holyAtk, 2) * GManager.instance.useAtValue.x) * thunderATFactor;

		}
		_damage._attackData.shock = GManager.instance.useAtValue.z;


		_damage._attackData.attackBuff = attackBuff;
		//damage = Mathf.Floor(damage * attackBuff);

		_damage._attackData.isBlow = GManager.instance.useAtValue.isBlow;
		_damage._attackData.isLight = GManager.instance.useAtValue.isLight;
		_damage._attackData.blowPower.Set(GManager.instance.useAtValue.blowPower.x, GManager.instance.useAtValue.blowPower.y);
	}

	/// <summary>
	/// 自分のダメージ中フラグ立ててこちらの防御力を教えてあげるの
	/// </summary>
	public void DefCalc(bool isTwinHand)
	{
			Equip useEquip;
            if (isTwinHand)
			{
				useEquip = GManager.instance.equipShield;
			}
			else
			{
				useEquip = GManager.instance.equipWeapon;
			}


			_health._defData.maxHp = GManager.instance.maxHp;
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
			_health._defData.isGuard = _movement.CurrentState == CharacterStates.MovementStates.Guard ? true : false;

		}

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
		public void ArmorReset()
		{
			nowArmor = GManager.instance.Armor;
		}

		/// <summary>
		/// アーマー値に応じてイベントとばす
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

			if (!isBack && _movement.CurrentState == CharacterStates.MovementStates.Guard)
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
					nowArmor -= (shock - GManager.instance.useAtValue.y) < 0 ? 0 : (shock - GManager.instance.useAtValue.y);
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

					if (_guard.guardHit)
					{
						result = MyWakeUp.StunnType.GuardBreake;
					}
					//パリィは別発生
					else
					{
						result = MyWakeUp.StunnType.Faltter;
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


		/// <summary>
		/// エンチャ時はエンチャントタイプを参照
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
					//	Debug.Log("チキン");
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
	}
}