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

		[Header("戦闘状態の感知網")]
		///<summary>
		///これで環境物とかに反応
		///</summary>
		public GameObject Serch3;

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


		Vector2 targetPosition;//敵の場所
		int initialLayer;
		bool jumpTrigger;

		bool isWait;//このフラグが真なら待機モーションする。マントいじったり遊んだり。
					//モーションは好感度とか進行度でモーション名リストが切り替わって決まる。
		bool isWarp;//ワープ中

		[HideInInspector] public bool isPosition;//持ち場フラグ
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
		bool isDown;
		float randomTime = 2.5f;//ランダム移動測定のため
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
		float jumpWait;//ジャンプ可能になるまでの待機時間
		float verticalWait;//垂直ジャンプ可能になるまでの待機時間
		bool isVertical;//垂直飛びするフラグ
		Vector2 waitPosition;
		//持ち場再判断
		[HideInInspector] public float reJudgePositionTime;
		[HideInInspector] public int stateNumber;
		[HideInInspector] public int beforeNumber;
		[HideInInspector] public float reJudgeTime;
		[HideInInspector] public bool changeable;//歩行からダッシュなど変更可能か

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

		protected new MyHealth _health;

		//シスターさんの固有アクション


		//	protected Hittable _hitInfo;


		#endregion

		// === コード（AIの内容） ================


		protected RewiredCorgiEngineInputManager ReInput;



        /// <summary>
        ///　ここで、パラメータを初期化する必要があります。
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
        /// 1フレームごとに、しゃがんでいるかどうか、まだしゃがんでいるべきかをチェックします
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

				if (!isPosition)
				{
					isPosition = true;
				}
				PositionSetting();
				if (!nowPosition)
				{
					

				}
			}

			else if (nowState == SisterState.警戒)
			{
				patrolJudge += Time.fixedDeltaTime;


				//Serch.SetActive(true);ほかのステートに移動するときに
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
					nowState = SisterState.のんびり;
					//Serch3.SetActive(false);
					//Serch2.SetActive(true);
					//Serch.SetActive(true);
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
			if (Serch3.activeSelf == false)
			{
				Serch3.SetActive(true);
			}
			#region

			#endregion
			if (!SManager.instance.actNow && isPosition && _controller.State.IsGrounded && !disEnable)
			{
				//ポジションにつきに行く。かつついてなくて地面にいる
				if (!nowPosition)
				{
					SManager.instance.GetClosestEnemyX();
					int mDirection = (int)Mathf.Sign(SManager.instance.closestEnemy - myPosition.x);
					//一番近い敵が右にいるとき
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

						reJudgePositionTime += Time.fixedDeltaTime;
						if (SManager.instance.target != null)
						{
							atDirection = (int)Mathf.Sign(SManager.instance.target.transform.position.x - myPosition.x);
							_characterHorizontalMovement.SetHorizontalMove(0f);
							
							BattleFlip(atDirection);
						}

						//おんぶする
					}
					if (reJudgePositionTime >= SManager.instance.sisStatus.escapeTime)
					{
						reJudgePositionTime = 0.0f;
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


		/// <summary>
		/// 戦闘中の位置取り
		/// これなに？
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
					reJudgeTime += Time.fixedDeltaTime;
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
					waitTime += Time.fixedDeltaTime;
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
			waitTime += Time.fixedDeltaTime;
			_characterHorizontalMovement.SetHorizontalMove(0);
			if (waitTime >= SManager.instance.sisStatus.waitRes)
			{
				Flip(-transform.localScale.x);//反転させます
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
			//stopTime += Time.fixedDeltaTime;
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
		/// 戦闘時に逃げるワープ
		/// </summary>
		private void EscapeWarp()
		{
			if (_movement.CurrentState != CharacterStates.MovementStates.Jumping)
			{
				if (!isWarp)
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
						isWarp = true;
						nowPosition = false;
						isPosition = false;
						SManager.instance.isEscape = true;
						//escapeがtrueの時警戒から戦闘にならない
						stateNumber = 0;
						beforeNumber = 0;
						reJudgeTime = 0;
						changeable = true;

						//("WarpPray");

						if (status.mp >= 5)
						{
							status.mp -= 5;
						}

						if (nowState == SisterState.戦い)
						{
							reJudgeTime = 0;
							nowState = SisterState.警戒;
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
						//逃走ワープメソッドで元に戻す
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
		/// 戦闘終了
		/// </summary>
		private void BattleEnd()
		{

			//敵がいなかったら
			if (SManager.instance.isBattleEnd && !(SManager.instance.targetList.Count == 0))
			{
				battleEndTime += Time.fixedDeltaTime;

				//時間計測して警戒に移行
				if (battleEndTime >= 2 && !isReset)
				{

					isReset = true;
					//六秒以上敵検知せんずれば警戒フェイズへ
					nowPosition = false;
					isPosition = false;
					SManager.instance.isEscape = true;
					nowState = SisterState.警戒;
					//escapeがtrueの時警戒から戦闘にならない
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

			Addressables.InstantiateAsync("WarpCircle", gofire.position, gofire.rotation);//.Result;//発生位置をPlayer
			GManager.instance.PlaySound("Warp", transform.position);
		}

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
