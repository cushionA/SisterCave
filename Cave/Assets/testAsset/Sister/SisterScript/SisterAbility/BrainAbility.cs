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
	/// 移動機能の改修
	/// ・移動と方向転換は共通のコントローラーに集める
	/// ・各メソッドでは状態変化と移動方向だけ指定
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



		[Header("射出地点")]
		///<summary>
		///弾丸出す場所
		///</summary>
		public Transform firePosition;

		/// <summary>
		/// ワープの判定に使う
		/// </summary>
		[SerializeField]
		private LayerMask layerMask;

		bool disEnable;
		/// <summary>
		/// プレイヤーが逃げてるフラグ。プレイヤーが逃げてたら戦わない
		/// プレイヤーが逃げた後のワープ後にオン
		/// </summary>
		[HideInInspector] public bool isEscape;

		public PlyerController pc;

		public enum SisterState
		{
			のんびり,
			警戒,
			戦い
		}

		public enum MoveState
		{
			停止,
			歩き,
			走り,
			最初
		}

		//[HideInInspector]
		public SisterState nowState = SisterState.のんびり;
		[HideInInspector] public MoveState nowMove = MoveState.最初;
		MoveState lastState = MoveState.最初;


		[Header("シスターさんのステータス")]
		public SisterStatus status;



		// === 外部パラメータ ======================================

		Vector2 distance;//プレイヤーとの距離

		bool isStop;//停止フラグ
		float waitTime;//パトロール時の待ち時間数える
					   //	 bool posiReset;//戦闘の持ち場再設定するフラグ



		int initialLayer;
		bool jumpTrigger;

		bool isWait;//このフラグが真なら待機モーションする。マントいじったり遊んだり。
					//モーションは好感度とか進行度でモーション名リストが切り替わって決まる。

		
	
		[HideInInspector] public bool nowPosition;//持ち場フラグ

		/// <summary>
		/// nowPosition設定のために使う
		/// </summary>
		Vector2 judgePosition = new Vector2();

		/// <summary>
		/// 戦闘時持ち場を離れていいか判断する時間。
		/// </summary>
		float escapeTime;

		// === キャッシュ ======================================





		// === 内部パラメータ ======================================

		int direction;
		int directionY;

		/// <summary>
		/// 最終的にどちらに動くか
		/// </summary>
		float moveDirection;


		//方向系のパラメータ

		Vector2 basePosition;
		/// <summary>
		/// プレイヤーが向いてる方向。基準
		/// </summary>
		Vector3 baseDirection;

		[HideInInspector] public float playPosition;//環境物の場所
		[HideInInspector] public float playDirection;//遊ぶ時の方向


		[HideInInspector] public float enemyDirection;//敵時の方向

		//	public Rigidbody2D GManager.instance.GManager.instance.pm.rb;//プレイヤーのリジッドボディ


		string squatTag = "SquatWall";

		bool isClose;//一度近くまで行く
		[HideInInspector] public bool isPlay;

		string jumpTag = "JumpTrigger";
		float patrolJudge;//警戒時間を数える
		bool isVertical;//垂直飛びするフラグ
		Vector2 waitPosition;



		/// <summary>
		/// 移動状態を再判定するための時間測定
		/// </summary>
		[HideInInspector] public float reJudgeTime =3;
		/// <summary>
		/// 歩行からダッシュなどに変更可能か。
		/// これがないと歩行と走行のはざまでガタガタ
		/// </summary>
		[HideInInspector] public bool changeable;//

		float battleEndTime;
		Vector2 move = new Vector2();



		// === アビリティ ================
		#region
		/// the number of jumps to perform while in this state
		//	[Tooltip("the number of jumps to perform while in this state")]
		//	public int NumberOfJumps = 1;




		//横移動は継承もとにある

		public PlayerJump _jump;
		//　public int _numberOfJumps = 0;


		//　public int _numberOfJumps = 0;


		public PlayerRunning _characterRun;


	//	public MyWakeUp _wakeup;

		//public WeaponAbillity _attack;

		public MyDamageOntouch _damage;

		public WarpAbility _warp;

		public FireAbility _fire;
		public PlayerCrouch _squat;
		//	public new MyHealth _health;

		//シスターさんの固有アクション

		public SensorAbility _sensor;
		//	protected Hittable _hitInfo;

		//protected RewiredCorgiEngineInputManager _inputManager;


		// アニメーションパラメーター
		protected const string _stateParameterName = "_nowPlay";
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
            
			//センサーアビリティの切り替え動作に変更
			_sensor.RangeChange();
			//_characterHorizontalMovement.FlipCharacterToFaceDirection = false;
			ParameterSet(status);
		}



        /// <summary>
        /// 1フレームごとに、しゃがんでいるかどうか、まだしゃがんでいるべきかをチェックします
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
			//↑プレイヤーが向いてる方向とプレイヤーの位置


			distance = basePosition - (Vector2)this.transform.position;
			direction = distance.x >= 0 ? 1 : -1;//距離が正、または0の時1。そうでないとき-1。方向
			directionY = distance.y >= 0 ? 1 : -1;//ほかのAIで弓構えるときのアニメの判定	にも使えそう

			//	Debug.Log($"現在のステート{nowState}");

			EscapeWarp();
			if (nowState == SisterState.戦い)
			{
				

				BattleEnd();

				PositionSetting();

			}

			else if (nowState == SisterState.警戒)
			{
				patrolJudge += _controller.DeltaTime;


				////Serch.SetActive(true);ほかのステートに移動するときに
				////Serch2.SetActive(true);


					PatrolMove();



				//	PositionChange();
				if (patrolJudge >= patrolTime)
				{
				    changeable = true;
					nowState = SisterState.のんびり;
					//_sensor.RangeChange();
					////Serch2.SetActive(true);
					////Serch.SetActive(true);
				}


			}
			else if (nowState == SisterState.のんびり)
			{
					PlayMove();

				WaitJudge();
				//待機状態には色々する

				//PositionChange();

				//if()

				//	SisterFall();
				//SetVelocity();

			}

			MoveController();
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
            if (_inputManager.CombinationButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
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
			if (SManager.instance.targetList.Count > 0)
			{
				if (_controller.State.IsGrounded && !disEnable)
				{
					//ポジションにつきに行く。かつついてなくて地面にいる
					//ベクターのxが小さい方
					SManager.instance.EscapePosition(ref judgePosition);

					//位置についていないとき
					if (!nowPosition)
					{


						if (changeable)
						{

							if (SManager.instance.targetList.Count > 0) 
							{
								//敵位置の左端より左に近いとき、左に逃げる（差が多いほうが遠いやん）
								//移動の基準点はjudgeposition.xになる
								if (Mathf.Abs(judgePosition.x - transform.position.x) < Mathf.Abs(judgePosition.y - transform.position.x))
								{

									nowMove = MoveState.走り;
									moveDirection = -1;

								}
								//壁にぶつかった時
								else if ((_controller.State.IsCollidingLeft) || (_controller.State.IsCollidingRight))
								{
									nowPosition = true;
									nowMove = MoveState.停止;
									moveDirection = Mathf.Sign(SManager.instance.targetList[0].transform.position.x - this.transform.position.x);
								}
								//敵位置の右端に近い時、右に逃げる
								//移動の基準点はjudgeposition.yになる
								else
								{
									nowMove = MoveState.走り;
									moveDirection = 1;

								}
							}
                            else
                            {
							//	nowPosition = true;
								nowMove = MoveState.停止;
								moveDirection = direction;
							}
							SisFlip(moveDirection);
						}
						if (transform.position.x < judgePosition.x - status.battleDis)
						{
							nowPosition = true;
							nowMove = MoveState.停止;
							//	moveDirection = 1;
							SisFlip(1);
						}
						else if (transform.position.x > judgePosition.y + status.battleDis)
						{
							nowPosition = true;
							nowMove = MoveState.停止;
							//moveDirection = -1;
							SisFlip(-1);
						}
					}

					else if (nowPosition)
					{
						//ここで使うのはターゲットの敵

						//再判定するかどうか
						//詠唱してないときに敵が逃亡範囲内にて聞いたら再判定

					
						if (SManager.instance.target != null && changeable)
						{
							float atDirection = Mathf.Sign(SManager.instance.target.transform.position.x - this.transform.position.x);
							nowMove = MoveState.停止;

							SisFlip(atDirection);

						}
					}

				}
				if (nowPosition)
				{
					escapeTime += _controller.DeltaTime;

				}
			}
            else
            {
				PatrolMove();

			}
		}
		/// <summary>
		/// 今の立ち位置が正しいかどうか確かめるために攻撃後に呼ぶ
		/// </summary>
		public void PositionJudge()
        {
			if (escapeTime >= SManager.instance.sisStatus.escapeTime && Mathf.Abs(SManager.instance.targetList[0].transform.position.x - this.transform.position.x) < status.escapeZone)
			{
				escapeTime = 0.0f;
			//	Debug.Log("ddddd");
				nowPosition = false;
				SManager.instance.target = null;
				// = 99;
			}

		}

		public void SisFlip(float dire)
        {
            if (!changeable)
            {
				return;
            }
			//方向が0の時は振り向かない
			if (transform.localScale.x != dire && dire != 0)
			{
				_character.Flip();
			}
			changeable = false;
		}

		/// <summary>
		/// ダッシュと歩行の遷移を司る
		/// </summary>
		public void MoveController()
        {

			if (!changeable)
			{
				reJudgeTime += _controller.DeltaTime;
				if (reJudgeTime >= 0.8f)
				{
					//状態を変化できるように
					changeable = true;
					reJudgeTime = 0;
				}
			}
		//	Debug.Log($"今{nowMove}");

			//動ける場合は移動状態に基づいてダッシュ発動やダッシュ状態の解除を
			if (!disEnable)
			{



				//ダッシュ状態の解除と発動
				#region


				float dire = transform.localScale.x;
				if (nowMove == MoveState.停止)
				{
					if (_movement.CurrentState == CharacterStates.MovementStates.Running)
					{
						_characterRun.RunStop();
					}

					_characterHorizontalMovement.SetHorizontalMove(0);
				}
				else if (nowMove == MoveState.歩き)
				{
					if (_movement.CurrentState == CharacterStates.MovementStates.Running)
					{
						_characterRun.RunStop();
					}
					isWait = false;
					_characterHorizontalMovement.SetHorizontalMove(dire);

				}
				else if (nowMove == MoveState.走り)
				{
					if (_movement.CurrentState != CharacterStates.MovementStates.Running)
					{
						_characterRun.RunStart();
						isWait = false;
					}
					_characterHorizontalMovement.SetHorizontalMove(dire);
				}

				/*
				//移動方向とキャラの方向合わないなら静止
				if (Mathf.Sign(transform.localScale.x) != Mathf.Sign(_characterHorizontalMovement.MovementSpeed) && _characterHorizontalMovement.MovementSpeed != 0)
				{

					Debug.Log($"sddf{Mathf.Sign(transform.localScale.x)}ddd{Mathf.Sign(_characterHorizontalMovement.MovementSpeed)}dsf{dire}");
					nowMove = MoveState.停止;

				}
				*/
                #endregion
            }
			//移動できないときは止まる
            else
            {
				if (_movement.CurrentState == CharacterStates.MovementStates.Running)
				{
					_characterRun.RunStop();
				}
				isWait = false;
				_characterHorizontalMovement.SetHorizontalMove(0);
			}
            lastState = nowMove;




		}


		/// <summary>
		/// 追い越し防止メソッド
		/// </summary>
		/// <param name="targ">追い越しちゃいけない相手との距離</param>
		public void PassingPrevention(float targ)
        {
            if (Mathf.Abs(targ) < 10)
            {
				nowMove = MoveState.停止;
            }
        }

		/// <summary>
		/// 待機中の哨戒行動
		/// 遊び歩きより接近基準が厳しめ
		/// </summary>
		public void PatrolMove()
		{
			if (_controller.State.IsGrounded && !disEnable)
			{
				if (changeable)
				{
					if (Mathf.Abs(distance.x) > status.walkDistance)
					{
						nowMove = MoveState.走り;
					}
					//近くにいる時
					else if (Mathf.Abs(distance.x) < status.patrolDistance)
					{
                        if (Mathf.Sign(GManager.instance.Player.transform.localScale.x) == Mathf.Sign(transform.localScale.x) && pc.NowSpeed() >= 100)
                        {
							nowMove = MoveState.走り;
                        }
                        else
                        {

							nowMove = MoveState.停止;
							isWait = true;
                        }
						
					}
					SisFlip(direction);
				}
				PassingPrevention(distance.x);

			}
		}
		public void PlayMove()
		{//のんびり
			if (_controller.State.IsGrounded && !disEnable)
			{
				//	

				//イベントオブジェクトは動かないので常に判断していい？
				//isPlay中は目的物に向かってまっすぐ行くので別に移動制御するか？
				if (isPlay)
				{
					float playDistance = playPosition - this.transform.position.x;
					playDirection = playDistance >= 0 ? 1 : -1;
					playDistance = Mathf.Abs(playDistance);
					if (Mathf.Abs(distance.x) <= status.playDistance && !isClose)
					{

						//環境物に接触
						waitTime = 0.0f;

						//オブジェクトに十分近づいたら
						if (playDistance <= 2)
						{
							nowMove = MoveState.停止;

						}
						else if (playDistance <= status.walkDistance)
						{
							nowMove = MoveState.歩き;
						}
						else
						{
							nowMove = MoveState.走り;

						}
						//	SisFlip(playDirection);
					}
					else
					{
						//一回ちゃんと接近しよう
						isClose = true;
						nowMove = MoveState.走り;
						moveDirection = direction;
						if (Mathf.Abs(distance.x) <= status.patrolDistance)
						{
							//くっついたら接近指示フラグ解除
							isClose = false;
						}

						//ある程度離れたら遊ばなくなる
						if (playDistance >= 100)
						{
							isPlay = false;
						}
						moveDirection = direction;
					}
				}

				//ここからは他の移動と同じに処理

				else
				{
					if (changeable)
					{
						//Debug.Log($"ｓｄｓｄｄｓ{pc.NowSpeed()}");
						//距離が遊んでいていい範囲内で一度離れて接近中でなければ
						if (Mathf.Abs(distance.x) <= status.patrolDistance + status.adjust && !isClose)
						{

							//プレイヤーが止まってる時は停止
							if (pc.NowSpeed() <= 70)
							{
								nowMove = MoveState.停止;
								isWait = true;
								//isWaitで時間経過で一人で遊んだりする？
								SisFlip(0);
								//Debug.Log("sd");
							}
							//動いてるなら歩く
							else
							{
								nowMove = MoveState.歩き;
							//	Debug.Log("dd");
								SisFlip(direction);
							}
						}

						//遊んでいていい距離から離れたら
						else if (Mathf.Abs(distance.x) > status.walkDistance || isClose)
						{
							//Debug.Log("ff");
							isClose = true;//接近しようっていうフラグ
							nowMove = MoveState.走り;
							SisFlip(direction);
						}
						//遊んでていい距離の中で、プレイヤーにくっついてないとき
						else if (Mathf.Abs(distance.x) > status.patrolDistance)
						{
						//	Debug.Log("ffg");
							nowMove = MoveState.歩き;
							SisFlip(direction);
						}

						if (Mathf.Abs(distance.x) <= status.patrolDistance)
						{
							//Debug.Log("hhh");
							//くっついたら接近指示フラグ解除
							isClose = false;

						}
					}
					PassingPrevention(distance.x);
				}

　　　　　　　　//isWaitの活用例
				#region
				
				/*
				else if (stateNumber == 10)
				{
					waitTime += _controller.DeltaTime;

					//プレイヤーが立ち止まってて環境物がないとき
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
		/// 待機中停止してきょろきょろ振り向きだけ。こいつはパトロールしないからwaitTimeは使いまわしでいい
		/// </summary>
		public void Wait()
		{
			//("Stand");
			waitTime += _controller.DeltaTime;
			_characterHorizontalMovement.SetHorizontalMove(0);
			if (waitTime >= SManager.instance.sisStatus.waitRes)
			{

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
				if (collision.tag == squatTag && _movement.CurrentState != CharacterStates.MovementStates.Crouching && _movement.CurrentState != CharacterStates.MovementStates.Crawling)
				{
					_squat.Crouch();
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
				//トンネルの中をしゃがみトリガーで満たす
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
							move.Set(basePosition.x - 6, basePosition.y + 5);
							move.y = RayGroundCheck(move) + 10;
							transform.position = move;

						}
						else
						{
							move.Set(basePosition.x + 6, basePosition.y + 5);
							move.y = RayGroundCheck(move) + 10;
							transform.position = move;

						}
						_controller.SetForce(Vector2.zero);
						nowPosition = false;

						//escapeがtrueの時警戒から戦闘にならない

						reJudgeTime = 0;
						changeable = true;

						_warp.WarpStart();



						if (nowState == SisterState.戦い)
						{
							reJudgeTime = 0;
							nowState = SisterState.警戒;
							_sensor.RangeChange();
							patrolTime = 0;
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
			if (_movement.CurrentState != CharacterStates.MovementStates.Warp)
			{

				if (SManager.instance.targetList.Count == 0)
				{
					battleEndTime += _controller.DeltaTime;

					//時間計測して警戒に移行
					if (battleEndTime >= 2)
					{
						StateChange();
						nowState = SisterState.警戒;

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

			Addressables.InstantiateAsync("WarpCircle", gofire);//.Result;//発生位置をPlayer
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

			if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(Name))// || _animator.GetCurrentAnimatorStateInfo(0).IsName("OStand"))
			{   // ここに到達直後はnormalizedTimeが"Default"の経過時間を拾ってしまうので、Resultに遷移完了するまではreturnする。
				return true;
			}
			if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
			{   // 待機時間を作りたいならば、ここの値を大きくする。
				return true;
			}
			//AnimatorClipInfo[] clipInfo = _animator.GetCurrentAnimatorClipInfo(0);

			////Debug.Log($"アニメ終了");

			return false;

			// return !(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
			//  (_currentStateName);
		}

        /// <summary>
        /// アニメイベント
		/// 音
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
        /// ステート変化前の処理
        /// </summary>
        /// <param name="nextState"></param>
        public void StateChange(SisterState nextState = SisterState.のんびり)
        {
			if (nextState == SisterState.戦い)
			{

				patrolTime = 0;
				SManager.instance.playObject = null;
				isPlay = false;
				//即座にポジション判断できるように
				reJudgeTime = 150;
				//Debug.Log("機能してますよー");

				nowState = BrainAbility.SisterState.戦い;//この辺はまた後で設定できるようにしよう
			}
            else
            {
				_fire.isReset = true;

				//六秒以上敵検知せんずれば警戒フェイズへ
				nowPosition = false;

				//escapeがtrueの時警戒から戦闘にならない
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
			_health.MaximumHealth = status.maxHp;
			_health.CurrentHealth = (int)maxHp;
		}

		public void MPReset()
        {
			mp = maxMp;
		}


		public void Flip(int direction)
		{

		}
		/// <summary>
		///  必要なアニメーターパラメーターがあれば、アニメーターパラメーターリストに追加します。
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter(_stateParameterName, AnimatorControllerParameterType.Bool, out _stateAnimationParameter);
		}

		/// <summary>
		/// これをオーバーライドすると、キャラクターのアニメーターにパラメータを送信することができます。
		/// これは、Characterクラスによって、Early、normal、Late process()の後に、1サイクルごとに1回呼び出される。
		/// </summary>
		public override void UpdateAnimator()
		{
			//のんびり1、警戒2、戦い3
			MMAnimatorExtensions.UpdateAnimatorBool(_animator, _stateAnimationParameter,nowState != SisterState.のんびり , _character._animatorParameters);
		}


	}
}
