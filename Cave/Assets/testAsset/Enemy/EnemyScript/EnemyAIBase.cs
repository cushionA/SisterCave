
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

	[Header("視覚探知")]
	///<summary>
	///視覚。これなしの敵がいても面白いかも
	///</summary>
	public GameObject Serch;

	[Header("全方位の気配探知")]
	///<summary>
	///しゃがんでたら抜けられる全方位感知網
	///</summary>
	public GameObject Serch2;

	public GameObject Guard;

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
	protected bool isStop;//停止フラグ
	protected float waitTime;//パトロール時の待ち時間数える
	protected bool posiReset;//戦闘とかで場を離れたとき戻るためのフラグ
	protected Vector3 firstDirection;//最初に向いてる向き
	//protected bool nowJump;
	protected bool isReach;//間合いの中にいる
	protected Vector2 targetPosition;//敵の場所
	protected bool isUp;
	protected int initialLayer;
	protected bool jumpTrigger;
	//ガード判定したかどうか
	protected bool guardJudge;
	protected float stayTime;//攻撃後の待機時間
	protected int attackNumber;//何番の攻撃をしているのか
	protected bool isAtEnable = true;
	protected bool isDamage;
	protected bool isMovable;
	protected bool isAnimeStart;//アニメの開始に使う



	[HideInInspector] public int bulletDirection;//敵弾がどちらから来たか

	//振り向き待ち時間
	protected float flipWaitTime = 0f;
	protected float lastDirection;

	protected float attackBuff = 1;//攻撃倍率

	bool flyNow;

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
	//public Animator animator;
	public Animator sAni;


	public Rigidbody2D rb;//継承先で使うものはprotected

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
	protected bool isGuard;
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


		protected float nowArmor;


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

	　　protected PlayerJump _jump;
	　//　protected int _numberOfJumps = 0;

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


        // === コード（Monobehaviour基本機能の実装） ================
        protected override void Initialization()
		{
		initialLayer = this.gameObject.layer;
			ParameterSet(status);
		ArmorReset();
			_health = (MyHealth)base._health;
			_characterHorizontalMovement.FlipCharacterToFaceDirection = false;
			if (status.kind != EnemyStatus.KindofEnemy.Fly)
		{
				//GravitySet(status.firstGravity);//重力設定
				_flying.StartFlight();
		}
		else
		{
			GravitySet(status.firstGravity);//重力設定後で変えようね
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

			//カメラに写ってないときは止まります。
			if (!cameraRendered)
			{
				if (!status.unBaind)
				{
					//	Debug.Log("停止");
					//縛られないなら先に
					return;
				}
			}

			//	Debug.Log($"知りたい");

			//移動可能かどうか確認

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
			
			//ドッグパイルありますか



			//うごける時だけ
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

				////////Debug.log($"ジャンプ中{nowJump}");
				////////Debug.log($"回避中{_movement.CurrentState != CharacterStates.MovementStates.Rolling}");
				if (isAggressive)
				{
					//攻撃行動中。
					//ここに敵との距離とか判断してHorizontalMoveとか起動するメソッドを。
					posiReset = true;
					Serch.SetActive(false);
					Serch2.SetActive(false);

					//targetPosition = GManager.instance.Player.transform.position;
					targetPosition = GManager.instance.Player.transform.position;

					distance = targetPosition - (Vector2)transform.position;
					//Debug.Log($"知りたいのだ{targetPosition.x}");
					direction = distance.x >= 0 ? 1 : -1;//距離が正、または0の時1。そうでないとき-1。方向
					directionY = distance.y >= 0 ? 1 : -1;//弓構えるときのアニメの判定	にも使えそう
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

			//このへんどうするかなぁ

			WaitAttack();//このままでいいかも
			ArmorRecover();//このまま

			//トリガーで呼びましょう
			JumpController();
			Parry();
			Die();//ヘルスに変える
			MaterialControll();



		}
		public void ActionFire(int i, float random = 0.0f)
	{//ランダムに入れてもいいけど普通に入れてもいい
		i = i < 0 ? 0 : i;
		i = i > status.enemyFire.Count - 1 ? status.enemyFire.Count : i;

		if (!isStop && _movement.CurrentState != CharacterStates.MovementStates.Rolling)
		{
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
	}




        public void Flip(float direction)
	{
		if (!isStop)
		{
			//	Debug.Log("士官");
			// Switch the way the player is labelled as facing.

			// Multiply the player's x local scale by -1.
			Vector3 theScale = transform.localScale;
			theScale.x = direction * Mathf.Abs(theScale.x);
			transform.localScale = theScale;
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
					//スタートが地面だから大丈夫。落ちてくる系のやつにはDogpileを与えてやれ
					rb.velocity = Vector2.zero;
					posiReset = false;
					isRight = true;
					transform.localScale = baseDirection;
				}
				else
				{
					////(status.motionIndex[dogPile == null ? "歩き":"走り"]);
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
							Flip(-Math.Abs(transform.localScale.x));//反転させまーす
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
							Flip(-Math.Abs(transform.localScale.x));//反転させまーす
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
	/// //プレイヤーが逃げたか確認
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
	}//落下死確認。改造してまた使う

	/// <summary>
	/// 戦闘中の距離の取り方
	/// disIndexで距離の取り方のパターンを選択できるね
	/// </summary>
	public void AgrMove(int disIndex = 0)
	{

		//if()
		stateJudge += _controller.DeltaTime;
		#region//判断
		if ((ground == EnemyStatus.MoveState.wakeup || stateJudge >= status.judgePace) && ground != EnemyStatus.MoveState.escape)
		//escapeだけはスクリプトから動かす
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
			//20パーセントの確率で停止以外に
			else if (((Mathf.Abs(distance.x) <= status.agrDistance[disIndex].x + status.adjust && Mathf.Abs(distance.x) >= status.agrDistance[disIndex].x - status.adjust) && RandomValue(0, 100) >= 40) || guardHit)
			{
					flipWaitTime = 1f;
				ground = EnemyStatus.MoveState.stay;
				//	flipWaitTime = 10;
			}
			else if (Mathf.Abs(distance.x) > status.agrDistance[disIndex].x)//近づく方じゃね？
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
			else if (Mathf.Abs(distance.x) < status.agrDistance[disIndex].x)//遠ざかる
			{
				//歩き距離なら敵を見たまま撃つ
				//動かない弓兵とかは移動速度ゼロに
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
					//バトルフリップはステイ中だけにする
				BattleFlip(direction);
				_characterHorizontalMovement.SetHorizontalMove(0f);
				isReach = true;
					return;

					//Debug.Log($"ねずみ{blowM.x}");

				
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
	/// 空を飛ぶタイプのエネミーに戦闘中乗せる。空を飛ぶ
	/// </summary>
	public void AgrFly(int disIndex = 0,int stMove = 0)
	{
		stateJudge += _controller.DeltaTime;
		#region//判断
		if ((ground == EnemyStatus.MoveState.wakeup || stateJudge >= status.judgePace) && ground != EnemyStatus.MoveState.escape && _condition.CurrentState != CharacterStates.CharacterConditions.Stunned)
		//escapeだけはスクリプトから動かす
		{

			bool isSet = false;

			//この場合はパーセンテージは分けるのに使おう
			//攻撃終了後逃げる
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
					//20パーセントの確率で停止以外に
					else if ((Mathf.Abs(distance.x) <= status.agrDistance[disIndex].x + status.adjust && Mathf.Abs(distance.x) >= status.agrDistance[disIndex].x - status.adjust) || guardHit)
					{

						ground = EnemyStatus.MoveState.stay;
						//	flipWaitTime = 10;
					}
					else if (Mathf.Abs(distance.x) > status.agrDistance[disIndex].x)//近づく方じゃね？
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
					else if (Mathf.Abs(distance.x) < status.agrDistance[disIndex].x)//遠ざかる
					{
						//歩き距離なら敵を見たまま撃つ
						//動かない弓兵とかは移動速度ゼロに
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
				float targetHigh = -distance.y;

				if ((targetHigh <= status.agrDistance[disIndex].y + status.adjust && targetHigh >= status.agrDistance[disIndex].y - status.adjust) || guardHit)
				{

					air = EnemyStatus.MoveState.stay;
						_flying.FastFly(true, true);
						//	flipWaitTime = 10;
					}
				else if (targetHigh <= status.agrDistance[disIndex].y)//近づく方じゃね？
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
				else if (targetHigh > status.agrDistance[disIndex].y)//遠ざかる
				{
					//歩き距離なら敵を見たまま撃つ
					//動かない弓兵とかは移動速度ゼロに
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
			else if (ground == EnemyStatus.MoveState.leaveWalk)//遠ざかる
			{
				//近距離の場合歩き範囲をダッシュで離れるのより大きく
				//歩き距離なら敵を見たまま撃つ
				//動かない弓兵とかは移動速度ゼロに
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
						if (judgeDistance　== status.agrDistance[disIndex].y)//ゆとりの範疇なら我慢
						{
							move.Set(rb.velocity.x,0);
							rb.velocity = move;
						}
						else if (judgeDistance < status.agrDistance[disIndex].y)//距離はなす
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
						else if (judgeDistance > status.agrDistance[disIndex].y)//距離近づく
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
		/// トリガーで呼ぶように
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
				//////Debug.log($"ジャンプトリガー{jumpTrigger}");
					_jump.JumpStart();
				
			}
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
			_controller.DefaultParameters.Gravity = gravity;
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
	/// 攻撃をはじかれノックバックする。
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

					//ステータスでジャンプタグ設定しなければジャンプできない敵が作れる
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

						//ステータスでジャンプタグ設定しなければジャンプできない敵が作れる
						jumpTrigger = true;
					}
				}
			}
	}





	/*	/// <summary>
		/// プレイヤーの魔法による攻撃
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
	}


	/// <summary>
	/// 攻撃
	/// 改修要請
	/// isShoot = trueの時の処理つくる？。いらない？つまりたまうちってこったな
	/// リヴァースで逆向く
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
	/// どのモーションをやるかどうか
	/// </summary>
	/// <param name="number"></param>
	public void SetAttackNumber(int number)
	{
		attackNumber = status.serectableNumber[number];
	}



	/// <summary>
	/// クールタイムの間次の攻撃を待つ
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
				//	isAtEnable = true;これクールタイムで管理しようよ
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
		{   // ここに到達直後はnormalizedTimeが"Default"の経過時間を拾ってしまうので、Resultに遷移完了するまではreturnする。
			return true;
		}
		if (sAni.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
		{   // 待機時間を作りたいならば、ここの値を大きくする。
			return true;
		}
		//AnimatorClipInfo[] clipInfo = sAni.GetCurrentAnimatorClipInfo(0);

		////Debug.Log($"アニメ終了");

		return false;

		// return !(sAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
		//  (_currentStateName);
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
				//水の足音
			}
		}

	}

	public void ActionSound(string useName)
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
	public void FistSound(int type = 0)
	{
		GManager.instance.PlaySound(MyCode.SoundManager.instance.fistSound[type], transform.position);

		//エンチャしてる場合も

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
		// Debug.Log($"アイイイイイイ{atEffect.SubObjectName}");
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
	/// FixedUpdateに置いてガードさせる
	/// 引数はガードする確率
	/// </summary>
	protected void GroundGuardAct(float guardProballity)
	{
		//後ろ歩きガードどうする？
		isGuard = ground == EnemyStatus.MoveState.stay && _condition.CurrentState != CharacterStates.CharacterConditions.Stunned && _movement.CurrentState != CharacterStates.MovementStates.Attack ? true : false;
		if (_movement.CurrentState != CharacterStates.MovementStates.Attack && _condition.CurrentState != CharacterStates.CharacterConditions.Stunned && guardJudge && !isGuard)
		{
			if ((Mathf.Sign(transform.localScale.x) != Mathf.Sign(GManager.instance.Player.transform.localScale.x)))
			{
				if (RandomValue(0, 100) >= (100 - guardProballity))
				{
					//flipWaitTime = 100;
					ground = EnemyStatus.MoveState.stay;
					//	Debug.Log("ｆ");
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


			//親マテリアルの情報を見る
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
					 Debug.Log($"マテリアル名{controllTarget[i].name}{parentMatt.name}");//
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


	/*	private void GetAllChildren(Transform parent)
		{
			int x = 0;
			Debug.Log($"確認{parent.name}");
			for (int i = 0; i >= parent.childCount; i++)
			{
				Debug.Log("ｓｓｓｓ");
				Transform child = parent.GetChild(i);
				SpriteRenderer matt = child.GetComponent<SpriteRenderer>();
				if (matt != null)
				{
					controllTarget.Add(matt.material);
				}
				//さらに子オブジェクト探索
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
			if (status.kind != EnemyStatus.KindofEnemy.Fly)
			{
				GravitySet(0);
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

			_health.CurrentHealth = (int)maxHp;

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


				if (status.phyAtk > 0)
				{
				_damage._attackData.phyAtk = (Mathf.Pow(status.phyAtk, 2) * atV.mValue) * attackFactor;

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
					_damage._attackData.holyAtk = (Mathf.Pow(status.holyAtk, 2) * atV.mValue) * holyATFactor;

				}
				//闇
				if (status.darkAtk > 0)
				{
				_damage._attackData.darkAtk = (Mathf.Pow(status.holyAtk, 2) * atV.mValue) * darkATFactor;

				}
			    //炎
			    if (status.fireAtk > 0)
			    {
				 _damage._attackData.fireAtk = (Mathf.Pow(status.holyAtk, 2) * atV.mValue) * fireATFactor;

		       	}
				//雷
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
		/// 自分のダメージ中フラグ立ててこちらの防御力を教えてあげるの
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




	}
}

