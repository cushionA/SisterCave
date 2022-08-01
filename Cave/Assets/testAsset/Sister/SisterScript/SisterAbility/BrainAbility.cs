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
    /// 普通に状況判断して動くためのスクリプト
	/// ワープ、攻撃、コンビネーション（各動作は分ける？）は別スクリプトに
	/// あるいはコンビネーションは主人公に持たせる？
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/BrainAbility")]
    public class BrainAbility : MyAbillityBase
    {
        /// このメソッドは、ヘルプボックスのテキストを表示するためにのみ使用されます。
        /// 能力のインスペクタの冒頭にある
        public override string HelpBoxText() { return "TODO_HELPBOX_TEXT."; }

		//   [Header("武器データ")]
		/// declare your parameters here
		///WeaponHandle参考にして 


		//public GameObject player;
		[Header("警戒ステートの時間")]
		public float patrolTime;
		[Header("視覚探知")]
		///<summary>
		///敵検知用
		///</summary>
		public GameObject Serch;

		[Header("全方位の気配探知")]
		///<summary>
		///これで環境物とかに反応
		///</summary>
		public GameObject Serch2;



		[Header("射出地点")]
		///<summary>
		///弾丸出す場所
		///</summary>
		public Transform firePosition;

		[SerializeField]
		private LayerMask layerMask;

		bool disEnable;
		bool isReset;


		public enum SisterState
		{
			のんびり,
			警戒,
			戦い
		}

		[HideInInspector] public SisterState nowState;


		[Header("シスターさんのステータス")]
		public SisterStatus status;



		// === 外部パラメータ ======================================

		Vector2 distance;//プレイヤーとの距離
		float stopTime;//停止時間を数える
		bool isStop;//停止フラグ
		float waitTime;//パトロール時の待ち時間数える
					   //	 bool posiReset;//戦闘の持ち場再設定するフラグ
		bool nowJump;


		int initialLayer;
		bool jumpTrigger;

		bool isWait;//このフラグが真なら待機モーションする。マントいじったり遊んだり。
					//モーションは好感度とか進行度でモーション名リストが切り替わって決まる。

		
	
		[HideInInspector] public bool nowPosition;//持ち場フラグ
		Vector2 myPosition;


		// === キャッシュ ======================================
		public Animator sAni;




		// === 内部パラメータ ======================================

		int direction;
		int directionY;
		float moveDirectionX;
		float moveDirectionY;
		float jumpTime;//ジャンプのクールタイム。処理自体はインパルスで


		Vector2 basePosition;
		/// <summary>
		/// プレイヤーが向いてる方向。基準
		/// </summary>
		Vector3 baseDirection;

		[HideInInspector] public bool guardHit;

		//	public Rigidbody2D GManager.instance.GManager.instance.pm.rb;//プレイヤーのリジッドボディ

		bool isSquat;
		string squatTag = "SquatWall";
		float PositionJudge;
		bool isClose;//一度近くまで行く
		[HideInInspector] public bool isPlay;
		[HideInInspector] public float playPosition;//環境物の場所
		[HideInInspector] public float playDirection;//遊ぶ時の方向
		string jumpTag = "JumpTrigger";
		float patrolJudge;//警戒時間を数える
		bool isVertical;//垂直飛びするフラグ
		Vector2 waitPosition;
		[HideInInspector] public int stateNumber;
		[HideInInspector] public int beforeNumber;
		/// <summary>
		/// 移動状態を再判定するための時間測定
		/// </summary>
		[HideInInspector] public float reJudgeTime;
		/// <summary>
		/// 歩行からダッシュなどに変更可能か。
		/// これがないと歩行と走行のはざまでガタガタ
		/// </summary>
		[HideInInspector] public bool changeable;//

		float battleEndTime;
		Vector2 move;
		private float flipWaitTime;
        private int lastDirection;


		// === アビリティ ================
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

		protected GuardAbillity _guard;

		protected MyWakeUp _wakeup;

		protected WeaponAbillity _attack;

		protected MyDamageOntouch _damage;

		protected WarpAbillity _warp;

		protected FireAbility _fire;

		protected new MyHealth _health;

		//シスターさんの固有アクション

		protected SensorAbility _sensor;
		//	protected Hittable _hitInfo;

		protected RewiredCorgiEngineInputManager ReInput;


		// アニメーションパラメーター
		protected const string _stateParameterName = "_nowState";
		protected int _stateAnimationParameter;

		#endregion

		// === ステータスパラメータ ======================================

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

		[HideInInspector]
		//ステータス
		//hpはヘルスでいい
		public float maxHp;
		[HideInInspector]
		public float maxMp;


		[HideInInspector]
		public float mp;

		// === コード（AIの内容） ================





		/// <summary>
		///　ここで、パラメータを初期化する必要があります。
		/// </summary>
		protected override void Initialization()
        {
            base.Initialization();
            // randomBool = false;
            ReInput = (RewiredCorgiEngineInputManager)_inputManager;
			//センサーアビリティの切り替え動作に変更
			_sensor.RangeChange();
			_characterHorizontalMovement.FlipCharacterToFaceDirection = false;
			GravitySet(status.firstGravity);
		}



        /// <summary>
        /// 1フレームごとに、しゃがんでいるかどうか、まだしゃがんでいるべきかをチェックします
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
			//↑プレイヤーが向いてる方向とプレイヤーの位置

			myPosition = this.transform.position;
			distance = basePosition - myPosition;
			direction = distance.x >= 0 ? 1 : -1;//距離が正、または0の時1。そうでないとき-1。方向
			directionY = distance.y >= 0 ? 1 : -1;//ほかのAIで弓構えるときのアニメの判定	にも使えそう

			//	Debug.Log($"現在のステート{nowState}");
			SisterFall();
			EscapeWarp();
			if (nowState == SisterState.戦い)
			{
				//Debug.Log($"位置についてますかー{nowPosition}");
				SManager.instance.BattleEndCheck();
				BattleEnd();


				PositionSetting();

			}

			else if (nowState == SisterState.警戒)
			{
				patrolJudge += _controller.DeltaTime;


				////Serch.SetActive(true);ほかのステートに移動するときに
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
					nowState = SisterState.のんびり;
					//_sensor.RangeChange();
					////Serch2.SetActive(true);
					////Serch.SetActive(true);
				}


			}
			else if (nowState == SisterState.のんびり)
			{
				if (!SManager.instance.actNow)
				{
					PlayMove();
				}
				WaitJudge();
				//待機状態には色々する

				//PositionChange();

				//if()

				//	SisterFall();
				//SetVelocity();

			}

			if(Mathf.Sign(transform.localScale.x) ==　Mathf.Sign(_characterHorizontalMovement.MovementSpeed) && !disEnable)
            {
				//移動方向とキャラの方向合わないなら静止
				_characterHorizontalMovement.SetHorizontalMove(0);
				_characterRun.RunStop();
            }

			JumpController();
		}

        /// <summary>
        /// アビリティサイクルの開始時に呼び出され、ここで入力の有無を確認します。
        /// </summary>
        protected override void HandleInput()
        {
            //ここで何ボタンが押されているかによって引数渡すか

            // here as an example we check if we're pressing down
            // on our main stick/direction pad/keyboard
            if (ReInput.CombinationButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {
				//ワープとかの連携を置く
            }
			//長押しの処理も入れる？
        }

		/// <summary>
		/// 支援位置につかせるメソッド
		/// </summary>
		public void PositionSetting()
		{
			
			if (!SManager.instance.actNow && _controller.State.IsGrounded && !disEnable)
			{
				//ポジションにつきに行く。かつついてなくて地面にいる
				if (!nowPosition)
				{
					SManager.instance.GetClosestEnemyX();
					int mDirection = (int)Mathf.Sign(SManager.instance.closestEnemy - myPosition.x);
					//一番近い敵が右にいるとき
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
							//	Debug.Log("停止");
							_characterHorizontalMovement.SetHorizontalMove(0f);
							Flip(mDirection);
							//おんぶする
							//ポジションに到着
							nowPosition = true;
							_characterRun.RunStop();
							//("Stand");
							reJudgeTime = 100;
						}
						else if (stateNumber == 1)
						{
							//	Debug.Log($"逃げる{mDirection}");
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
							//おんぶする
							//ポジションに到着
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
					//再判定するかどうか
					//範囲内にて聞いたら再判定
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

						//おんぶする
					}
					if (reJudgeTime >= SManager.instance.sisStatus.escapeTime)
					{
						reJudgeTime = 0.0f;
						reJudgeTime = 150;
						SManager.instance.GetClosestEnemyX();
						//プレイヤーがワープ上限より離れていたら

						//敵が逃げるゾーンより近かったから

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
		/// 待機中の哨戒行動
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
					//Debug.Log("でそ");
					Flip(direction);
					_characterHorizontalMovement.SetHorizontalMove(lastDirection);
					
					isWait = false;
					//("Walk");
				}
				else if (stateNumber == 3)
				{
					//	Debug.Log("です");
					_characterHorizontalMovement.SetHorizontalMove(0f);
					isWait = true;
					//("Stand");
				}


			}
		}
		public void PlayMove()
		{//のんびり
			if (_controller.State.IsGrounded && !disEnable)
			{
				if (reJudgeTime >= 1.5 && changeable)
				{

					if (Mathf.Abs(distance.x) > status.playDistance || isClose)
					{
						stateNumber = 1;
						isClose = true;//接近しようっていうフラグ
						if (Mathf.Abs(distance.x) <= status.patrolDistance)
						{
							//くっついたら接近指示フラグ解除
							isClose = false;
						}
						_characterRun.RunStart();
					}
					else if (Mathf.Abs(distance.x) <= status.playDistance && Mathf.Abs(distance.x) > status.patrolDistance)
					{
						if (isPlay)
						{
							//環境物に接触してる時

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
						//くっついてるとき
						//近くにいる
						if (isPlay)
						{
							//環境物に接触
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
					//遊びでうろついていい範囲から離れてるならくっつくまで走る。
					//isCloseは接近しなければならないというフラグ
					//いったんすぐ近くまで来よう
					Flip(direction);
					_characterHorizontalMovement.SetHorizontalMove(lastDirection);
					
					
					//	isWait = false;//立ち止まってる。

				}

				//環境物に接触してる時

				else if (stateNumber == 2)
				{

					//環境物のすぐそばにいるとき
					_characterHorizontalMovement.SetHorizontalMove(0); 
					//("Stand");
				}
				else if (stateNumber == 3)
				{
					Flip(1);
					//環境物より後ろにいるとき
					_characterHorizontalMovement.SetHorizontalMove(lastDirection);
					
					
					//("Move");
				}
				else if (stateNumber == 4)
				{
					Flip(-1);
					//環境物より前にいるとき
					_characterHorizontalMovement.SetHorizontalMove(lastDirection);

					
					//("Move");
				}

				else if (stateNumber == 5)
				{
					//遊んでいい範囲にいて、くっついてなくて環境物がないとき。小走りで行く
					//警戒距離まで来る
					//isWait = true;
					Flip(direction);
					_characterHorizontalMovement.SetHorizontalMove(lastDirection);
					
					//isWait = false;//立ち止まってる。
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



				//環境物に接触
				//	waitTime = 0.0f;
				else if (stateNumber == 7)
				{
					//環境物のすぐそばにいるとき
					_characterHorizontalMovement.SetHorizontalMove(0);
					
					//("Stand");
				}
				else if (stateNumber == 8)
				{
					Flip(1);
					//環境物より後ろにいるとき
					_characterHorizontalMovement.SetHorizontalMove(lastDirection);
					
					//("Move");
				}
				else if (stateNumber == 9)
				{
					Flip(-1);
					//環境物より前にいるとき
					_characterHorizontalMovement.SetHorizontalMove(lastDirection);
					
					//("Move");
				}

				else if (stateNumber == 10)
				{
					waitTime += _controller.DeltaTime;
					Flip(direction);
					//プレイヤーが立ち止まってて環境物がないとき
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
		/// 待機中停止してきょろきょろ振り向きだけ。こいつはパトロールしないからwaitTimeは使いまわしでいい
		/// </summary>
		public void Wait()
		{
			//("Stand");
			waitTime += _controller.DeltaTime;
			_characterHorizontalMovement.SetHorizontalMove(0);
			if (waitTime >= SManager.instance.sisStatus.waitRes)
			{
				Flip(-transform.localScale.x);//反転させます
				waitTime = 0.0f;
			}
		}





		public void JumpController()
		{

			//isVerticalは自分でオンオフする
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
			return Random.Range(X, Y + 1);

		}

		/// <summary>
		/// stopResを数えた後にisStop解除のメソッドを呼ぶ。isStop中はすべて止まる。硬直やダウンにどうぞ
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
		/// 停止解除。単体では使わない
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

				//トンネルの中をしゃがみトリガーで満たす
				if (collision.tag == squatTag && GManager.instance.pm.isSquat)
				{
					isSquat = true;
				}
				//elseでこれしゃがみ解除あるぞ
				if (collision.tag == jumpTag && _controller.State.IsGrounded)
				{
					//GetComponentはなるべくせぬように
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
				//トンネルの中をしゃがみトリガーで満たす
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
		/// 待機状態解除判断のためのメソッド
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
		/// 戦闘時に逃げるワープ
		/// </summary>
		private void EscapeWarp()
		{
			if (_movement.CurrentState != CharacterStates.MovementStates.Jumping)
			{
				if (!disEnable)
				{
					//転送後
					if ((Mathf.Abs(distance.x) >= SManager.instance.sisStatus.warpDistance.x || Mathf.Abs(distance.y) >= SManager.instance.sisStatus.warpDistance.y))// && GManager.instance.pm._controller.State.IsGrounded)
					{

						//方向でxに2ずらした数値にワープ
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
						//escapeがtrueの時警戒から戦闘にならない
						stateNumber = 0;
						beforeNumber = 0;
						reJudgeTime = 0;
						changeable = true;

						_warp.WarpStart(move);



						if (nowState == SisterState.戦い)
						{
							reJudgeTime = 0;
							nowState = SisterState.警戒;
							_sensor.RangeChange();
							reJudgeTime = 0;
							_sensor.ReSerch();
						}
					}
				}
			}
		}

		/// <summary>
		/// 戦闘終了
		/// 敵が二秒間いなければ警戒フェイズに移る
		/// この時点で攻撃は中止しよう。Fire編集時に攻撃中止メソッドを作る
		/// </summary>
		private void BattleEnd()
		{

			//敵がいなかったら
			if (SManager.instance.isBattleEnd && _movement.CurrentState != CharacterStates.MovementStates.Warp)
			{

				if (SManager.instance.targetList.Count == 0)
				{
					battleEndTime += _controller.DeltaTime;

					//時間計測して警戒に移行
					if (battleEndTime >= 2 && !isReset)
					{

						isReset = true;
						//六秒以上敵検知せんずれば警戒フェイズへ
						nowPosition = false;
						SManager.instance.isEscape = true;
						nowState = SisterState.警戒;
						//escapeがtrueの時警戒から戦闘にならない
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

			Addressables.InstantiateAsync("WarpCircle", gofire.position, gofire.rotation);//.Result;//発生位置をPlayer
			GManager.instance.PlaySound("Warp", transform.position);
		}
		//ワープ専用の地面探知
		public float RayGroundCheck(Vector2 position)
		{

			//真下にレイを飛ばしましょう
			RaycastHit2D onHitRay = Physics2D.Raycast(position, Vector2.down, Mathf.Infinity, layerMask.value);

			//  ////Debug.log($"{onHitRay.transform.gameObject}");
			////Debug.DrawRay(i_fromPosition,i_toTargetDir * SerchRadius);

			//Debug.Log($"当たったもの{onHitRay.transform.gameObject.name}");
			return onHitRay.point.y;
		}
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
		///  必要なアニメーターパラメーターがあれば、アニメーターパラメーターリストに追加します。
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter(_stateParameterName, AnimatorControllerParameterType.Int, out _stateAnimationParameter);
		}

		/// <summary>
		/// これをオーバーライドすると、キャラクターのアニメーターにパラメータを送信することができます。
		/// これは、Characterクラスによって、Early、normal、Late process()の後に、1サイクルごとに1回呼び出される。
		/// </summary>
		public override void UpdateAnimator()
		{
			//のんびり1、警戒2、戦い3
			MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _stateAnimationParameter,(int)nowState , _character._animatorParameters);
		}


	}
}
