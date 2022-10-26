
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

    //アビリティセットをルート下においてやる

    //「//sAni.」を「sAni.」で置き換える。アニメ作ったら

    //アニメの種類
    //必須
    //移動（Move）、歩き（Walk）、走り（Dash）、怯み（Falter）、ダウン（Down）、起き上がり（Wakeup）
    //吹き飛び（Blow）、弾かれ（Bounce）、後ろ歩き（BackMove）、戦闘待機（Pose）、立つ（Stand）、死亡（NDie,DDie）
    //
    //必要に応じて
    //落下（Fall）、ガード（Guard）、ガード移動（GuardMove）、後ろガード移動（BackGuard）、ガードブレイク（GuardBreak）
    //回避（Avoid）、ジャンプ（Jump）
    //
	//旧パラメータ
    #region
    [SerializeField] GameObject effectController;


	[Header("射出地点")]
	///<summary>
	///弾丸出す場所
	///</summary>
	public Transform firePosition;
	[SerializeField]
	Transform dispary;

	/// <summary>
	/// カメラに写ってるか
	/// </summary>
	[HideInInspector] public bool cameraRendered;
	/// <summary>
	/// 水踏んでるかどうか
	/// </summary>
	[HideInInspector] public bool isWater;


	[Header("エネミーのステータス")]
	public EnemyStatus status;



	// === 外部パラメータ ======================================
	/*[HideInInspector]*/
	public bool isAggressive;//攻撃モードの敵
//	[HideInInspector] public bool _movement.CurrentState != CharacterStates.MovementStates.Attack;//これがTrueの時ひるまない
	protected bool isRight = true;//右にパトロールするかどうか
	//protected bool _controller.State.IsGrounded;
	protected Vector2 distance;//プレイヤーとの距離
	protected float escapeTime;//数えるだけ
	protected float stopTime;//停止時間を数える

	protected float waitTime;//パトロール時の待ち時間数える
	protected bool posiReset;//戦闘とかで場を離れたとき戻るためのフラグ
	protected Vector3 firstDirection;//最初に向いてる向き
	//protected bool nowJump;
	protected bool isReach;//間合いの中にいる
	protected Vector2 targetPosition;//敵の場所
	protected bool isUp;
	protected int initialLayer;
	protected bool jumpTrigger;

	//ガード判定するかどうか
	protected bool guardJudge;
	protected float stayTime;//攻撃後の待機時間
	protected int attackNumber;//何番の攻撃をしているのか
	protected bool isAtEnable = true;
	protected bool isDamage;
	protected bool isMovable;
	protected bool isAnimeStart;//アニメの開始に使う

	/// <summary>
	/// 移動設定の時に決める進行方向
	/// </summary>



	[HideInInspector] public int bulletDirection;//敵弾がどちらから来たか

	//振り向き待ち時間
	protected float flipWaitTime = 0f;
	protected float lastDirection;

	protected float attackBuff = 1;//攻撃倍率



	/// <summary>
	/// 攻撃の値
	/// </summary>
	protected string useSound;

	List<Transform> mattTrans = new List<Transform>();

	/// <summary>
	/// 攻撃の値
	/// </summary>
	[HideInInspector] public EnemyValue atV;
	[HideInInspector] public EnemyStatus.MoveState ground = EnemyStatus.MoveState.wakeup;
	[HideInInspector] public EnemyStatus.MoveState air = EnemyStatus.MoveState.wakeup;
	// === キャッシュ ==========================================



	

		[Header("ドッグパイル")]
		///<summary>
		///移動の中心となるオブジェクト。敵を引き連れたり
		///</summary>
		public GameObject dogPile;

		// === 内部パラメータ ======================================
		protected float xSpeed;
	protected float ySpeed;
	protected int direction;
	protected int directionY;

		/// <summary>
		/// 移動設定の時に決める進行方向
		/// </summary>
		protected float moveDirectionX;
	protected float moveDirectionY;
	protected float jumpTime;//ジャンプのクールタイム。
	protected bool disenableJump;//ジャンプ不可能状態
	protected AudioSource[] seAnimationList;
	protected Vector3 startPosition;//開始位置と待機時テリトリーの起点
	protected Vector3 basePosition;
	protected Vector3 baseDirection;
	protected bool enableFire;
	protected float waitCast;
	protected float randomTime = 2.5f;//ランダム移動測定のため
	protected bool guardBreak;//ガードブレイク
	[HideInInspector] public bool guardHit;
	protected float stateJudge;//ステート判断間隔
	protected Rigidbody2D dRb;//ドッグパイルのリジッドボディ
							  //	protected float nowDirection;//今の敵の方向

	[HideInInspector] public float damageDelay;
	[HideInInspector] public bool isHit;//ヒット中
	[HideInInspector] public bool isHitable;
	[HideInInspector] public GameObject lastHit;
	protected float jumpWait;
	protected bool isWakeUp;//ダウン終了後起き上がるフラグ
	protected bool blowDown;


	protected float blowTime;
	protected float recoverTime;//アーマー回復までにかかる時間
	protected int lastArmor = 1;
		/// <summary>
		/// スーパーアーマー機能中かどうか
		/// </summary>
		bool isArmor;

		/// <summary>
		/// 落下攻撃中や突進など攻撃を終わらせたくないときに
		/// </summary>
		protected int attackContinue;

		/// <summary>
		/// ガードする確率
		/// </summary>
		float guardProballity = 100;


		 //-----------------------------------------------------

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

		//ステータス
		//HPはヘルスだろ
		public float maxHp = 100;

		//この辺いらないかも
		//個別に持たせた倍率で操作すればよくね？

		//　無属性防御力。体力で上がる
		public float Def = 70;
		//刺突防御。筋力で上がる
		public float pierDef = 70;
		//打撃防御、技量で上がる
		public float strDef = 70;
		//神聖防御、筋と賢さで上がる。
		public float holyDef = 70;
		//闇防御。賢さで上がる
		public float darkDef = 70;
		//炎防御。賢さと生命で上がる
		public float fireDef = 70;
		//雷防御。賢さと持久で上がる。
		public float thunderDef = 70;


		[HideInInspector]
		public float nowArmor;


		//-------------------------------------------
		[Header("戦闘中の移動速度")]
		public Vector2 combatSpeed;
		[Header("参照用の攻撃力")]
		/// <summary>
		/// 参照用の攻撃力
		/// </summary>
		public int atkDisplay;
		[Header("参照用の防御力")]
		/// <summary>
		/// 参照用の防御力
		/// </summary>
		public int defDisplay;
		//---------------------------------------------



	[SerializeField]
	protected SpriteRenderer td;

	[SerializeField]
	protected Transform atBlock;
	/// <summary>
	/// 攻撃後フラグ。攻撃後の距離再判定挙動で使う
	/// </summary>
	protected bool attackComp;

	/// <summary>
	/// エフェクト操作用のマテリアル
	/// </summary>
	[SerializeField]
	protected Renderer parentMatt;
	/// <summary>
	/// マテリアル操作始動フラグ
	/// </summary>
	protected int materialSet;
	/// <summary>
	/// 操作対象のマテリアル管理ナンバー。0から
	/// </summary>
	int mattControllNum;
	/// <summary>
	/// 操作対象のスプライト一覧
	/// </summary>
	public Transform[] spriteList;
	/*
	/// <summary>
	/// マテリアルがセットされたか
	/// </summary>
	protected bool spriteSet;*/
	protected List<Renderer> controllTarget = new List<Renderer>();

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

		public PlayerJump _jump;
	　//　protected int _numberOfJumps = 0;

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
		/// ちゃんと振り向き終わったか
		/// これが真になるまで動けない
		/// </summary>
		bool flipComp;
		#endregion

		protected const string _suppleParameterName = "suppleAct";
		protected int _suppleAnimationParameter;

		protected const string _combatParameterName = "isCombat";
		protected int _combatAnimationParameter;

		/// <summary>
		/// 補足行動をする
		/// 
		/// </summary>
		int suppleNumber;

		CharacterStates.MovementStates lastState = CharacterStates.MovementStates.Nostate;


		bool flyNow;


		// === コード（Monobehaviour基本機能の実装） ================
		protected override void Initialization()
		{
			base.Initialization();
		initialLayer = this.gameObject.layer;
			ParameterSet(status);
		ArmorReset();

		//あとずさりとかもあるし振り向きはこっちで管理

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
		/// 最初のマテリアルの無効有効設定
		/// </summary>
		protected void MaterialSet()
        {
			//親マテリアルの情報を見る
			//	Debug.Log($"{parentMatt.material}");
			//全部のスプライトを集めて設定する
			for(int i = 0;  i < spriteList.Length;i++)
			{

				GetAllChildren(spriteList[i]);
				//	await UniTask.WaitForFixedUpdate();
			}

			Material coppy = controllTarget[0].material;

			//何らかの条件でいじり方変える
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
		//	Debug.Log($"かどうか{_animator.name}");


	}


	public void Brain()
        {
			//Debug.Log($"dsfwe");
			//カメラに写ってないときは止まります。
			if (!cameraRendered && !status.unBaind)
			{

						
					//縛られないなら先に
					return;
			}



			//移動可能かどうか確認

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
			
			//ドッグパイルありますか



			//うごける時だけ
			if (isMovable)
			{

				///変更点・これはステートが変わる時だけでいい
				//Debug.Log($"ggggggg");

				////////Debug.log($"ジャンプ中{nowJump}");
				////////Debug.log($"回避中{_movement.CurrentState != CharacterStates.MovementStates.Rolling}");
				if (isAggressive)
				{
					//攻撃行動中。
					//ここに敵との距離とか判断してHorizontalMoveとか起動するメソッドを。
					
					
					//ほんとにプレイヤーだけでええか？
					//targetPosition = GManager.instance.Player.transform.position;
					targetPosition = GManager.instance.Player.transform.position;

					distance = targetPosition - (Vector2)transform.position;
					//Debug.Log($"知りたいのだ{targetPosition.x}");
					direction = distance.x >= 0 ? 1 : -1;//距離が正、または0の時1。そうでないとき-1。方向
					directionY = distance.y >= 0 ? 1 : -1;//弓構えるときのアニメの判定	にも使えそう
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

			//このへんどうするかなぁ

			WaitAttack();//このままでいいかも
			ArmorRecover();//このまま

			//トリガーで呼びましょう
			JumpController();
		//	Parry();
			Die();//ヘルスに変える
			MaterialControll();



		}

	




	

	/// <summary>
	/// レイヤー変更。Avoidなんかと使ってもいいが単体でも使える
	/// </summary>
	/// <param name="layerNumber"></param>
	public void SetLayer(int layerNumber)
	{

		this.gameObject.layer = layerNumber;

	}

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
	/// XとYの間で乱数を出す
	/// </summary>
	/// <param name="X"></param>
	/// <param name="Y"></param>
	/// <returns></returns>
	public int RandomValue(int X, int Y)
	{
		return UnityEngine.Random.Range(X, Y + 1);

	}

	/// <summary>
	/// アニメイベントと攻撃後に呼び出す。スーパーアーマー状態を反転させる
	/// 攻撃の途中でスーパーアーマー出せる
	/// isArmorつける？
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
	/// アーマーをリセット
	/// </summary>
	public void ArmorReset()
	{
		nowArmor = status.Armor;
	}

	public void ArmorRecover()
	{
		//	Debug.Log($"今のアーマー{nowArmor}");
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
	/// アーマー値に応じてイベントとばす
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
					//パリィは別発生
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
		/// アーマー値に応じてイベントとばす
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
			{Debug.Log($"あと{nowArmor}");
				return false;
			}
		}






		protected virtual void OnTriggerEnter2D(Collider2D collision)
	{



				if (isAggressive && collision.tag == EnemyManager.instance.JumpTag && _controller.State.IsGrounded)
			{
				//ジャンプ方向合ってるなら
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
		/// ステータス関連
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
			///　リスト
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
				_jump.CoyoteTime = status.jumpCool;
				_jump.JumpHeight = status.jumpRes;
				_jump.NumberOfJumps = status.jumpLimit;

			}
			maxHp = status.maxHp;

		//この辺いらないかも
		//個別に持たせた倍率で操作すればよくね？

		//　無属性防御力。体力で上がる
		 Def = status.Def;
		//刺突防御。筋力で上がる
		 pierDef = status.pierDef;
		//打撃防御、技量で上がる
		 strDef = status.strDef;
		//神聖防御、筋と賢さで上がる。
		 holyDef = status.holyDef;
		//闇防御。賢さで上がる
		 darkDef = status.darkDef;
		//炎防御。賢さと生命で上がる
		 fireDef = 70;
		//雷防御。賢さと持久で上がる。
		 thunderDef = 70;

			_health.InitialHealth = (int)maxHp;
			_health.MaximumHealth = (int)maxHp;
			_health.CurrentHealth= _health.InitialHealth;
		//	Debug.Log($"tanomu{_health.CurrentHealth}");
			nowArmor = status.Armor;
	}

		/// <summary>
		/// ダメージ計算
		/// </summary>
		/// <param name="isFriend">真なら味方</param>
		public void DamageCalc()
		{
			//GManager.instance.isDamage = true;
			//status.hitLimmit--;
			//mValueはモーション値

			float mainDamage = 0;
			if (status.phyAtk > 0)
			{
				_damage._attackData.phyAtk = status.phyAtk * attackFactor;

				//斬撃刺突打撃を管理
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

					//						Debug.Log("皿だ");
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
			//神聖
			if (status.holyAtk > 0)
			{
				_damage._attackData.holyAtk =  status.holyAtk * holyATFactor;
				if (_damage._attackData.holyAtk > mainDamage)
				{
					_damage._attackData._attackType = 8;
					mainDamage = _damage._attackData.holyAtk;
				}
			}
			//闇
			if (status.darkAtk > 0)
			{
				_damage._attackData.darkAtk =  status.darkAtk * darkATFactor;
				if (_damage._attackData.darkAtk > mainDamage)
				{
					_damage._attackData._attackType = 16;
					mainDamage = _damage._attackData.darkAtk;
				}
			}
			//炎
			if (status.fireAtk > 0)
			{
				_damage._attackData.fireAtk =  status.fireAtk * fireATFactor;
				if (_damage._attackData.fireAtk > mainDamage)
				{
					_damage._attackData._attackType = 32;
					mainDamage = _damage._attackData.fireAtk;
				}
			}
			//雷
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
		/// 自分のダメージ中フラグ立ててこちらの防御力を教えてあげるの
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
		/// バフの数値を与える
		/// 弾丸から呼ぶ
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
		/// 　攻撃関連の処理
		/// 　攻撃に必要なこと
		/// 　まず攻撃のパラメータを設定。番号を指定し攻撃開始
		/// 　落下攻撃などには引数付きの攻撃終了条件設定アニメイベントを入れる。これでアーマーも有効にする
		/// 　アニメイベントにはほかにエフェクトを出すやつと音を出すやつがある。
		/// 　そして攻撃終了条件を満たしてるか、あるいはアニメ終わったかで攻撃終了確認
		/// 　そして補足行動あるならやるが、補足行動は厳密には攻撃処理と切り離して行う。
		/// 　補足行動フラグは応用すれば挑発とか敵にさせれる
		/// 　攻撃ごとにつけるクールタイムが終了したらまた攻撃するように
		/// 　魔術系の攻撃の詠唱は攻撃判定ない攻撃にしてコンボで魔術発動に
		/// 　攻撃値にisShootとか入れるか
		///</sammary>
		#region

		/// <summary>
		/// 攻撃前の初期化
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

			//突進用の初期化
			atV._moveDuration = status.atValue[attackNumber]._moveDuration;
			atV._moveDistance = status.atValue[attackNumber]._moveDistance;
			atV._contactType = status.atValue[attackNumber]._contactType;
			atV.fallAttack = status.atValue[attackNumber].fallAttack;
			atV.startMoveTime = status.atValue[attackNumber].startMoveTime;
			atV.lockAttack = status.atValue[attackNumber].lockAttack;

			//ヒット数制御関連
			_damage._attackData._hitLimit = status.atValue[attackNumber]._hitLimit;
			_damage.CollidRestoreResset();


		}


		/// <summary>
		/// 弾丸を発射する
		/// 詠唱アニメ後にこれでよくね？
		/// </summary>
		/// <param name="i"></param>
		/// <param name="random"></param>
		public void ActionFire(int i, float random = 0.0f)
	{
			//ランダムに入れてもいいけど普通に入れてもいい
		i = i < 0 ? 0 : i;
		i = i > status.enemyFire.Count - 1 ? status.enemyFire.Count : i;


			waitCast += _controller.DeltaTime;
			if (waitCast >= status.enemyFire[i].castTime)
				if (random != 0)
				{
					firePosition.position.Set
						(firePosition.position.x + random, firePosition.position.y + random, firePosition.position.z);//銃口から
				}
			Transform goFire = firePosition;

			for (int x = 0; x >= status.enemyFire[i].bulletNumber; x++)
			{
				Addressables.InstantiateAsync(status.enemyFire[i].effects, goFire.position, Quaternion.identity);//.Result;//発生位置をPlayer
			}
			//go.GetComponent<EnemyFireBullet>().ownwer = transform;

	}
		//弾丸処理の例
		//ロードして
		#region
		/*
		 		/// <summary>
		/// for文ではないが	bcountを超えるまでuseMagicが真なので発動し続ける
		/// 弾丸を作るメソッド
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
			Debug.Log("きてる");
			//	Debug.Log($"ハザード{SManager.instance.useMagic.name}標的{SManager.instance.target}動作{sister.nowMove}");
			//魔法使用中MagicUseでかつ弾丸生成中でなければ

			//弾の発射というか生成位置
			Vector3 goFire = sb.firePosition.position;
			//弾が一発目なら
			if (bCount == 1)
			{
				//   MyInstantiate(SManager.instance.useMagic.fireEffect, goFire, Quaternion.identity).Forget();
				//Addressables.InstantiateAsync(SManager.instance.useMagic.fireEffect, goFire, Quaternion.identity);
				if (SManager.instance.useMagic.fireType == SisMagic.FIREBULLET.RAIN)
				{
					//山なりの弾道で打ちたいときとか射出角度決めれたらいいかも
					//位置をランダムにすれば角度はどうでもいい説もある
					SManager.instance.useMagic.angle = GetAim(sb.firePosition.position, SManager.instance.target.transform.position);

				}
				sb.mp -= SManager.instance.useMagic.useMP;
			}

			//敵の位置にサーチ攻撃するとき
			if (SManager.instance.useMagic.isChaice)
			{
				goFire.Set(SManager.instance.target.transform.position.x, SManager.instance.target.transform.position.y, SManager.instance.target.transform.position.y);

			}
			//ランダムな位置に発生するとき
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

				goFire = new Vector3(sb.firePosition.position.x + xRandom, sb.firePosition.position.y + yRandom, 0);//銃口から

			}
			//Debug.Log($"魔法の名前5{SManager.instance.useMagic.hiraganaName}");
			//    MyInstantiate(SManager.instance.useMagic.effects, goFire, Quaternion.identity).Forget();//.Result;//発生位置をPlayer
			//即座に発生する弾丸の一発目なら
			if (SManager.instance.useMagic.delayTime == 0 || bCount == 1)
			{
				Debug.Log("aaa");
                UnityEngine.Object h = Addressables.LoadAssetAsync<UnityEngine.Object>(SManager.instance.useMagic.effects).Result;
				 GameObject t =  Instantiate(h, goFire, Quaternion.Euler(SManager.instance.useMagic.startRotation)) as GameObject;//.MMGetComponentNoAlloc<FireBullet>().InitializedBullet(this.gameObject, SManager.instance.target);
				t.MMGetComponentNoAlloc<FireBullet>().InitializedBullet(this.gameObject,SManager.instance.target);
			}
			//2発目以降の弾で生成中じゃないなら
			else if (bCount > 1 && !delayNow)
			{
				DelayInstantiate(SManager.instance.useMagic.effects, goFire, Quaternion.Euler(SManager.instance.useMagic.startRotation)).Forget();
			}
			//弾丸を生成し終わったら
			if (bCount >= SManager.instance.useMagic.bulletNumber)
			{
				//Debug.Log($"テンペスト{SManager.instance.useMagic.name}標的{SManager.instance.target}動作{sister.nowMove}");
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
		/// 攻撃
		/// 改修要請
		/// isShoot = trueの時の処理つくる？。いらない？つまりたまうちってこったな
		/// リヴァースで逆向きで
		/// </summary>
		public void Attack(bool select = false, int number = 1, bool reverse = false)
	{

            if (!isAtEnable || !isMovable || number > status.atValue.Count || number <= 0)
            {//Debug.Log($"一回きり{isAtEnable}{isMovable}{ number > status.atValue.Count}{number <= 0}");
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
		//	Debug.Log($"検査1{attackNumber}");
			_attack.AttackTrigger(number);

			isAtEnable = false;

			_characterHorizontalMovement.SetHorizontalMove(0);
			_controller.SetHorizontalForce(0);
			//距離が移動範囲内で、ロックオンするなら距離を変える
			float moveDistance = (atV.lockAttack && Mathf.Abs(distance.x) < atV._moveDistance) ? distance.x : atV._moveDistance;	

			_rush.RushStart(atV._moveDuration,moveDistance * direction,atV._contactType,atV.fallAttack,atV.startMoveTime);


			if (!reverse)
			{
			//	Debug.Log($"あああ{transform.lossyScale.x}{direction}");
					NormalFlip(direction,true);

			}
			else
			{

					NormalFlip(-direction);

			}
		}


		public void attackEffect()
		{
			// Debug.Log($"アイイイイイイ{atEffect.SubObjectName}");
			Addressables.InstantiateAsync(atV.attackEffect, effectController.transform);
		}




		/// <summary>
		/// 攻撃終了の条件を入れつつアーマーを設定
		/// アニメーションイベント
		/// </summary>
		public void ConditionAttack(int continueNumber = 0)
        {
			//このナンバーによって重力かけたりとかやる？

				attackContinue = continueNumber;

			isArmor = true;
        }

		/// <summary>
		/// 補足行動の起動を行うメソッド
		/// </summary>
		/// <param name="useNumber"></param>
		public void SActTrigger(int useNumber)
        {
			suppleNumber = atV.suppleNumber;
			_condition.ChangeState(CharacterStates.CharacterConditions.Moving);
		}

		/// <summary>
		/// どのモーションをやるかどうか
		/// </summary>
		/// <param name="number"></param>
		public void SetAttackNumber(int number)
		{
			number = number > status.serectableNumber.Count - 1 ? status.serectableNumber.Count - 1 : number;
			number = number < 0 ? 0 : number;
			attackNumber = status.serectableNumber[number];
		}

		/// <summary>
		/// クールタイムの間次の攻撃を待つ
		/// </summary>
		public async  void WaitAttack()
	{
		if (_attack.nowAttack)
		{
			//	Debug.Log($"tubuあん{attackNumber}");
				if (attackContinue == 0)
			{
		//			Debug.Log($"かなしい{isMovable}ｓｓ{_movement.CurrentState}");
				await ExecuteAttack();
	
			}
				else if (attackContinue != 0)
				{
					//１は空中攻撃
					if (attackContinue == 1)
					{
						//接地したら0に
						if (_controller.State.IsGrounded)
						{
							attackContinue = 0;
							//	NormalFlip(direction);
							//	isAtEnable = true;これクールタイムで管理しようよ
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

					/// 補完行動の終了待ち
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
				//	Debug.Log($"検査2{attackNumber}");
					//AttackPrepare();
					isMovable = true;
						Attack(true,attackNumber);
					}
				
		}
	}
		/// <summary>
		/// アニメ中断
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
		/// パリィ不可の行動
		/// エフェクト出す時間も指定できるようにしたいね
		/// LifTimeいじるか
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

		//ヘルスのために攻撃状態か否かを返す
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
//Debug.Log($"あん1指定{Name}");
		if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(Name))// || sAni.GetCurrentAnimatorStateInfo(0).IsName("OStand"))
		{   // ここに到達直後はnormalizedTimeが"Default"の経過時間を拾ってしまうので、Resultに遷移完了するまではreturnする。
				
				return true;
		}
		if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
		{   // 待機時間を作りたいならば、ここの値を大きくする。
			//	Debug.Log("あん2");
				return true;
		}
		//AnimatorClipInfo[] clipInfo = sAni.GetCurrentAnimatorClipInfo(0);

		////Debug.Log($"アニメ終了");

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

			// モーションを実行(実行後にAnimator更新のため1フレーム待つ)
		//	animator.Play("Attack");
			await UniTask.DelayFrame(1);



			// モーション終了まで待機
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

			//補足行動指定が0でないとき
			if (atV.suppleNumber != 0 && !atV.isCombo)
			{
				SActTrigger(atV.suppleNumber);
			}
			// 待機モーションを再生
			//	_animator.CrossFade("Idle", 0.1f);

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


		/// <summary>
		/// アニメイベント
		/// 音関連
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
				//水の足音
			}
		}

	}

		/// <summary>
		/// 現在の状況に合わせたサウンドを提供します
		/// 移動とスタン以外
		/// 固有の音に変えたい場合はオーバーライド
		/// プレイヤーに移植してもいい
		/// </summary>
		/// <param name="i">これはバリエーションある場合のパラメータ</param>
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
				//	Debug.Log("あいいいい");
					GManager.instance.FollowSound(status.walkSound, transform);
					flyNow = true;
					return;
				}
                else if (flyNow && (_condition.CurrentState == CharacterStates.CharacterConditions.Dead || _condition.CurrentState == CharacterStates.CharacterConditions.Stunned))
				{
			//		Debug.Log("いいいい");
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
				//これはFlyとかで分けるか
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
		//斬撃刺突打撃を管理
		if (atV.type == EnemyStatus.AttackType.Stab)
		{
			GManager.instance.PlaySound(MyCode.SoundManager.instance.stabSound[type], transform.position);
		}
		else
		{
			GManager.instance.PlaySound(MyCode.SoundManager.instance.swingSound[type], transform.position);
		}

		//エンチャしてる場合も

	}
	public void FirstSound(int type = 0)
	{
		GManager.instance.PlaySound(MyCode.SoundManager.instance.fistSound[type], transform.position);

		//エンチャしてる場合も

	}

		#endregion


		/// <summary>
		/// 移動関連処理
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
		/// アニメの都合上反対側向きたいときのメソッド
		/// </summary>
		public void AnimeFlip()
		{
			//_characterFly.SetHorizontalMove(1f);
			//このへん使って振り向き行おう

			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
			lastDirection = direction;
		}

		/// <summary>
		/// 最初の位置に戻る。
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
								//スタートが地面だから大丈夫。落ちてくる系のやつにはDogpileを与えてやれ

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
		/// 待機中の哨戒行動
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

						////////Debug.log("ああああ");
						waitTime += _controller.DeltaTime;
						_characterHorizontalMovement.SetHorizontalMove(0);
						if (waitTime >= status.waitRes)
						{
							isRight = !isRight;
							waitTime = 0.0f;
							//	//////Debug.log("ああああ");

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

						////////Debug.log("ああああ");
						waitTime += _controller.DeltaTime;
						_flying.SetHorizontalMove(0);
						if (waitTime >= status.waitRes)
						{
							isRight = !isRight;
							waitTime = 0.0f;
							//	//////Debug.log("ああああ");

						}
					}
				}
			}
		}

		/// <summary>
		/// 待機中空を飛ぶ
		/// 待機と選択で積む
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

					////////Debug.log("ああああ");
					waitTime += _controller.DeltaTime;
					_flying.SetHorizontalMove(0);
					if (waitTime >= status.waitRes)
					{
						isRight = !isRight;
						waitTime = 0.0f;
						//	//////Debug.log("ああああ");

					}
				}

			}
		}

		/// <summary>
		/// 待機中停止してきょろきょろ振り向きだけ。こいつはパトロールしないからwaitTimeは使いまわしでいい
		/// パトロールと選択でどっちか積む
		/// なにかに止まってるハエとかも表現できる
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
							 NormalFlip(-Math.Sign(transform.localScale.x));//反転させまーす
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
							 NormalFlip(-Math.Sign(transform.localScale.x));//反転させまーす
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
		/// //プレイヤーが逃げたか確認
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
		}//落下死確認。改造してまた使う

		/// <summary>
		/// 戦闘中の距離の取り方
		/// disIndexで距離の取り方のパターンを選択できるね
		/// </summary>
		public void AgrMove(int disIndex = 0)
		{
             GuardJudge();
		//	Debug.Log($"知りたい{ground}");
			stateJudge += _controller.DeltaTime;
			#region//判断



			if ((ground == EnemyStatus.MoveState.wakeup || stateJudge >= status.judgePace) && ground != EnemyStatus.MoveState.escape)
			//escapeだけはスクリプトから動かす
			{
				

				bool isDashable = status.combatSpeed.x > 0;
				int dire = direction;
				if (((Mathf.Abs(distance.x) <= status.agrDistance[disIndex].x + status.adjust && Mathf.Abs(distance.x) >= status.agrDistance[disIndex].x - status.adjust) && RandomValue(0, 100) >= 40) || guardHit)
				{
					flipWaitTime = 1f;
					ground = EnemyStatus.MoveState.stay;
					//	flipWaitTime = 10;
				}
				else if (Mathf.Abs(distance.x) > status.agrDistance[disIndex].x)//近づく方じゃね？
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
				else if (Mathf.Abs(distance.x) < status.agrDistance[disIndex].x)//遠ざかる
				{
					//歩き距離なら敵を見たまま撃つ
					//動かない弓兵とかは移動速度ゼロに
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
							//逃げる時はガード終わらせる
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
					//バトルフリップはステイ中だけにする
					
					_characterHorizontalMovement.SetHorizontalMove(0f);
					isReach = true;
					return;

					//Debug.Log($"ねずみ{blowM.x}");


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
					//Runningフラグトゥルーの時の処理を見る
					_characterRun.RunStart();
					if (Mathf.Abs(distance.x) <= status.agrDistance[disIndex].x + status.adjust && Mathf.Abs(distance.x) >= status.agrDistance[disIndex].x - status.adjust)
					{
						ground = EnemyStatus.MoveState.accessWalk;
						stateJudge = 0.0f;
						_characterRun.RunStop();
					}
				}
				else if (ground == EnemyStatus.MoveState.leaveWalk)//遠ざかる
				{
					//近距離の場合歩き範囲をダッシュで離れるのより大きく
					//歩き距離なら敵を見たまま撃つ
					//動かない弓兵とかは移動速度ゼロに
					
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
		/// 空を飛ぶタイプのエネミーに戦闘中乗せる。空を飛ぶ
		/// </summary>
		/// <param name="disIndex">使用する戦闘距離</param>
		/// <param name="stMove">まっすぐ進むときに使うフラグ</param>
		public void AgrFly(int disIndex = 0, int stMove = 0)
		{
			
			stateJudge += _controller.DeltaTime;
			#region//判断
			if ((ground == EnemyStatus.MoveState.wakeup || stateJudge >= status.judgePace) && ground != EnemyStatus.MoveState.escape && _condition.CurrentState != CharacterStates.CharacterConditions.Stunned)
			//escapeだけはスクリプトから動かす
			{
		//		Debug.Log($"アイ{air}");// {air}");//{_condition.CurrentState}
				bool isSet = false;

			int dire = direction;

				//この場合はパーセンテージは分けるのに使おう
				//攻撃終了後逃げる
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
						//20パーセントの確率で停止以外に
						else if ((Mathf.Abs(distance.x) <= status.agrDistance[disIndex].x + status.adjust && Mathf.Abs(distance.x) >= status.agrDistance[disIndex].x - status.adjust) || guardHit)
						{

							ground = EnemyStatus.MoveState.stay;
							//	flipWaitTime = 10;
						}
						else if (Mathf.Abs(distance.x) > status.agrDistance[disIndex].x)//近づく方じゃね？
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
						else if (Mathf.Abs(distance.x) < status.agrDistance[disIndex].x)//遠ざかる
						{
							//歩き距離なら敵を見たまま撃つ
							//動かない弓兵とかは移動速度ゼロに
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
				//縦
				if (!isSet)
				{
					//条件まとめ
					//超下にいるとき     lvD
					//超上にいる時       acD
					//ちょい下にいるとき lvW
					//ちょい上にいるとき acW
					//動かなくていいとき stay

					//現在のプレイヤー高度と合わせて目標高度を割りだす。

					//マイナスにすることで標的から見た自分の距離になる
					//標的から上にいるか下にいるか
					float targetHeight = -distance.y;


					if ((targetHeight <= status.agrDistance[disIndex].y + status.adjust) && (targetHeight >= status.agrDistance[disIndex].y - status.adjust) || guardHit)
					{

						air = EnemyStatus.MoveState.stay;
						_flying.FastFly(true, true);
					}

					//上昇する
					else if (targetHeight <= status.agrDistance[disIndex].y)//近づく方じゃね？
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
					//降下する
					else if (targetHeight > status.agrDistance[disIndex].y)//遠ざかる
					{
						//歩き距離なら敵を見たまま撃つ
						//動かない弓兵とかは移動速度ゼロに
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
				//	Debug.Log($"ア{air}");
				}

				stateJudge = 0;
				attackComp = false;
				//	Debug.Log($"空{air}陸{ground}");
				//	Debug.Log($"空{distance.y}");
				//状態切り替え時には振り向くように
				flipWaitTime = 3;
			}
			#endregion




			if (isMovable)
			{

				VerTicalMoveJudge();


				//groundとairの組み合わせで呼び出すもの変えよう

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
				else if (ground == EnemyStatus.MoveState.leaveWalk)//遠ざかる
				{

					//近距離の場合歩き範囲をダッシュで離れるのより大きく
					//歩き距離なら敵を見たまま撃つ
					//動かない弓兵とかは移動速度ゼロに
					
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
		/// 回避。方向指定も可能だが戦闘時に限りdirectionで前方。-directionで後ろに
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
		///	地上キャラ用のジャンプ。ジャンプ状態は自動解除。ジャンプキャンセルとセット
		///	バックジャンプが真なら後ろに振り向いて飛んでいく
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
		/// トリガーで呼ぶように
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
		
			//ダブルジャンプするかどうかは任意で決めます
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
	/// FixedUpdateに置いてガードさせる
	/// 引数はガードする確率
	/// ガード中移動してたら
	/// </summary>
	protected void GroundGuardAct(float Proballity = 100)
	{

			//後ろ歩きガードどうする？
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
							Debug.Log("ｆ");
						_guard.ActGuard();

					}
					
					guardJudge = false;

			}
		}


	protected void MaterialControll()
	{
		if (materialSet > 0)
		{


			//親マテリアルの情報を見る
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
	/// 死ぬときのマテリアル操作始動用メソッド。リストのゼロはよく使うやつにするのが無難
	/// 二秒で消える
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
		/// マテリアルを取得するやつ
		/// Weaponは武器を探すフラグ
		/// スプライトリストの外から武器を持ってくる
		/// </summary>
		/// <param name="parent">マテリアルを収集するオブジェクトの親</param>
		/// <param name="transforms">使用するリスト</param>
		/// <param name="Weapon">一番上のオブジェクトであるかどうか。選択したスプライトリストじゃないとこに武器のマテリアルがある</param>
	private void GetAllChildren(Transform parent, bool Weapon = false)
	{
		for (int i = 0; i < parent.childCount;i++)
		{
				//まず子オブジェクトをトランスフォームのリストに追加
				//子オブジェクトの子オブジェクトまで探索

				Transform child = parent.GetChild(i);

			GetAllChildren(child, true);
			//レンダラーを取り出す（あとでこいつを操作する）
			Renderer sr = child.gameObject.MMGetComponentNoAlloc<Renderer>();
			if (sr != null)
			{
				//リストに追加
				//Debug.Log(sr.name);
				controllTarget.Add(sr);
			}
		}
		
		//一番上のオブジェクトの時武器と盾を探す
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
		/// 攻撃状態開始
		/// </summary>
		public void StartCombat()
        {
			//攻撃行動中。
			//ここに敵との距離とか判断してHorizontalMoveとか起動するメソッドを。
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

			//戦闘開始時はdirectionは０です

			targetPosition = GManager.instance.Player.transform.position;
			distance = targetPosition - (Vector2)transform.position;
			//Debug.Log($"知りたいのだ{targetPosition.x}");
			direction = distance.x >= 0 ? 1 : -1;
			isAggressive = true;
			NormalFlip(direction);
			_sensor.RangeChange();
		}

		/// <summary>
		/// 戦闘終了
		/// </summary>
		public void CombatEnd()
        {
			Debug.Log("もんじゃ");
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
		///  必要なアニメーターパラメーターがあれば、アニメーターパラメーターリストに追加します。
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			//    RegisterAnimatorParameter(_attackParameterName, AnimatorControllerParameterType.Bool, out _attackAnimationParameter);
			RegisterAnimatorParameter(_suppleParameterName, AnimatorControllerParameterType.Int, out _suppleAnimationParameter);
			RegisterAnimatorParameter(_combatParameterName, AnimatorControllerParameterType.Bool, out _combatAnimationParameter);
		}

		/// <summary>
		/// アビリティのサイクルが終了した時点。
		/// 現在のしゃがむ、這うの状態をアニメーターに送る。
		/// </summary>
		public override void UpdateAnimator()
		{
			//今のステートがAttackであるかどうかでBool入れ替えてる

			MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _suppleAnimationParameter,suppleNumber, _character._animatorParameters);
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _combatAnimationParameter, isAggressive, _character._animatorParameters);
		}





	}
}

