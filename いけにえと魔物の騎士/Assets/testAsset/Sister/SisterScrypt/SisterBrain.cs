using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class SisterBrain : MonoBehaviour
{
	//「//sAni.」を「sAni.」で置き換える。アニメ作ったら
	[Header("プレーヤーオブジェクト")]
	///<summary>
	///プレイヤー
	///</summary>
	public GameObject player;
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
	/*[HideInInspector]*/
	 Vector2 distance;//プレイヤーとの距離
	 bool isJump;//ジャンプしてるかどうか
	 float stopTime;//停止時間を数える
	 bool isStop;//停止フラグ
	 float waitTime;//パトロール時の待ち時間数える
	 bool posiReset;//戦闘とかで場を離れたとき戻るためのフラグ
	 bool nowJump;

	 Vector2 targetPosition;//敵の場所
	 int initialLayer;
	 bool jumpTrigger;
	 bool isWait;
	[HideInInspector]public bool isPegion;//おんぶフラグ
	[HideInInspector] public bool nowPegion;//おんぶフラグ
	Vector2 myPosition;


	// === キャッシュ ==========================================
	Animator animator;
	 SimpleAnimation sAni;


	public Rigidbody2D rb;//継承先で使うものは

	// === 内部パラメータ ======================================
	 float xSpeed;
	 float ySpeed;
	 int direction;
	 int directionY;
	 float moveDirectionX;
	 float moveDirectionY;
	 float jumpTime;//ジャンプのクールタイム。処理自体はインパルスで
	 bool disenableJump;//ジャンプ不可能状態
	 AudioSource[] seAnimationList;
	 Vector2 basePosition;
	 Vector2 baseDirection;
	 bool isDown;
	 float randomTime = 2.5f;//ランダム移動測定のため
	[HideInInspector] public bool guardHit;
	float stateJudge;//ステート判断間隔
	public Rigidbody2D pRb;//プレイヤーのリジッドボディ
	PlayerMove pm;
	bool isSquat;
	string squatTag = "SquatWall";
	float pegionJudge;
	bool isClose;//一度近くまで行く
	[HideInInspector]public bool isPlay;
	[HideInInspector] public float playPosition;
	[HideInInspector] public float playDirection;//遊ぶ時の方向
	string jumpTag = "JumpTrigger";
	bool reJump;//もう一回飛ぶ
	bool doJump;//ジャンプ終了
	Vector3 nowPos;//ジャンプ前の地点記録
	float patrolJudge;//警戒時間を数える

	// === コード（Monobehaviour基本機能の実装） ================
	 void Start()
	{
		//rb = GetComponent<Rigidbody2D>();
		pRb = player.gameObject.GetComponent<Rigidbody2D>();
		pm = player.gameObject.GetComponent<PlayerMove>();

		/*		if (SManager.instance.sisStatus.equipMagic != null)
				{
					enableFire = true;
				}
		*/
	}



	 void Update()
	{
		// 落下チェック

	}

	 void FixedUpdate()
	{
		basePosition = player.transform.position;
		baseDirection = player.transform.localScale;

		 myPosition = this.transform.position;
		distance = basePosition - myPosition;
		direction = distance.x >= 0 ? 1 : -1;//距離が正、または0の時1。そうでないとき-1。方向
		directionY = distance.y >= 0 ? 1 : -1;//弓構えるときのアニメの判定	にも使えそう

		Debug.Log($"現在の状態{nowState}");
		//Debug.Log($"回避中{isAvoid}");
		if (nowState == SisterState.戦い)
		{
			if(Serch3.activeSelf == false)
            {
				Serch3.SetActive(true);
            }
			isPegion = true;
			Pegion();
			if (!nowPegion)
			{
				TriggerJump();
			}
		}

		else if (nowState == SisterState.警戒)
		{
			patrolJudge += Time.fixedDeltaTime;

			//Serch.SetActive(true);ほかのステートに移動するときに
			//Serch2.SetActive(true);
			if (!nowPegion)
			{
				if (!isPegion)
				{
					PatrolMove();
				}
				else
				{
					Pegion();
				}
				TriggerJump();
			}
			else if (nowPegion)
            {
				StopPegion();
			}

			PegionChange();
			if(patrolJudge >= patrolTime)
            {
				nowState = SisterState.のんびり;
			}

		}
		else if(nowState == SisterState.のんびり)
        {
			if (!nowPegion)
			{
				if (!isPegion)
				{
					PlayMove();
				}
				else
				{
					Pegion();
				}
				TriggerJump();
			}
			else if (nowPegion)
			{
				StopPegion();
			}


			PegionChange();
		}


		//SetVelocity();
	}

	public void Pegion()
    {
		if (Serch3.activeSelf == true)
		{
			Serch3.SetActive(false);
		}
		if (isPegion)
		{
			if (baseDirection.x > 0)
			{
				if (myPosition.x <= basePosition.x && myPosition.x >= basePosition.x - status.pegionDis)
				{
					rb.velocity = Vector2.zero;
					Flip(direction);
					//おんぶする
					nowPegion = true;
				}
				else if (myPosition.x > basePosition.x - 1)
				{
					Flip(-1);
					rb.AddForce(new Vector2(status.addSpeed * (-status.dashSpeed - rb.velocity.x), 0));
				}
				else if (myPosition.x < basePosition.x - 1)
				{
					Flip(1);
					rb.AddForce(new Vector2(status.addSpeed * (status.dashSpeed - rb.velocity.x), 0));
				}
			}
			else if (baseDirection.x < 0)
			{
				if (myPosition.x >= basePosition.x && myPosition.x <= basePosition.x + status.pegionDis)
				{
					rb.velocity = Vector2.zero;
					Flip(direction);
					//おんぶする
				}
				else if (myPosition.x < basePosition.x + 1)
				{
					Flip(1);
					rb.AddForce(new Vector2(status.addSpeed * (status.dashSpeed + rb.velocity.x), 0));
				}
				else if (myPosition.x > basePosition.x + 1)
				{
					Flip(-1);
					rb.AddForce(new Vector2(status.addSpeed * (-status.dashSpeed - rb.velocity.x), 0));
				}
			}
		}
	}

	public void StopPegion()
    {

            if (!isPegion)
            {

            }
    }

	public void PegionChange()
    {
        if (pm.isSquat && !isSquat && pRb.velocity == Vector2.zero)
        {
			pegionJudge += Time.fixedDeltaTime;
            if (nowPegion)
            {
				if(pegionJudge >= 2.0f)
                {
					isPegion = false;
					pegionJudge = 0.0f;
				}
            }
			else if (!nowPegion)
            {
				if (pegionJudge >= 5.0f)
				{
					isPegion = true; ;
					pegionJudge = 0.0f;
				}
			}
        }
        else
        {
			pegionJudge = 0.0f;
        }

    }

/*	public void SetVelocity()
	{

		if (rb.velocity.x > SManager.instance.sisStatus.velocityMax.x || rb.velocity.x < SManager.instance.sisStatus.velocityMin.x)
		{
			Mathf.Clamp(rb.velocity.x, SManager.instance.sisStatus.velocityMin.x, SManager.instance.sisStatus.velocityMax.x);
		}
		if (rb.velocity.y > SManager.instance.sisStatus.velocityMax.y || rb.velocity.y < SManager.instance.sisStatus.velocityMin.y)
		{
			Mathf.Clamp(rb.velocity.y, SManager.instance.sisStatus.velocityMin.y, SManager.instance.sisStatus.velocityMax.y);
		}

	}*/

	public void Flip(float direction)
	{
		if (!isStop)
		{
			// Switch the way the player is labelled as facing.

			// Multiply the player's x local scale by -1.
			Vector3 theScale = transform.localScale;
			theScale.x = direction;
			transform.localScale = theScale;
		}
	}

	/// <summary>
	/// 待機中の哨戒行動
	/// </summary>
	public void PatrolMove()
	{
		if(Mathf.Abs(distance.x) > status.walkDistance)
        {
			Flip(direction);
			rb.AddForce(new Vector2(status.addSpeed * (status.patrolSpeed * direction - rb.velocity.x), 0));
			isWait = false;
		}
	else if((Mathf.Abs(distance.x) <= status.walkDistance && Mathf.Abs(transform.position.x) > status.patrolDistance) || pRb.velocity.x != 0)
        {
			Flip(direction);
			rb.AddForce(new Vector2(status.addSpeed * (status.walkSpeed * direction - rb.velocity.x), 0));
			isWait = false;
		}
        else if(pRb.velocity.x == 0 && Mathf.Abs(transform.position.x) > status.patrolDistance)
        {
			rb.velocity = Vector2.zero;
			isWait = true;
        }
	}
	public void PlayMove()
	{//のんびり
		if (Mathf.Abs(distance.x) >= status.playDistance || isClose)
		{
			Flip(direction);
			rb.AddForce(new Vector2(status.addSpeed * (status.dashSpeed * direction - rb.velocity.x), 0));
			isWait = false;//立ち止まってる。
			isClose = true;
			if (Mathf.Abs(distance.x) <= status.patrolDistance)
			{
				//くっついたら接近フラグ解除
				isClose = false;
			}
		}
		else if (Mathf.Abs(distance.x) < status.playDistance)
		{
            if (isPlay)
            {
                if (myPosition.x >= playPosition - 2 && myPosition.x <= playPosition + 2)
                {
					rb.velocity = new Vector2(0, rb.velocity.y);
                }
				else if(myPosition.x < playPosition)
                {
					rb.AddForce(new Vector2(status.addSpeed * (status.walkSpeed - rb.velocity.x), 0));
				}
				else if(myPosition.x > playPosition)
                {
					rb.AddForce(new Vector2(status.addSpeed * (-status.walkSpeed - rb.velocity.x), 0));
				}
            }
            else
            {
				isWait = true;
            }

			//Flip(direction);
			//rb.AddForce(new Vector2(status.addSpeed * (status.playSpeed * direction - rb.velocity.x), 0));

		}
		else if (pRb.velocity.x == 0 && Mathf.Abs(transform.position.x) > status.patrolDistance)
		{
			rb.velocity = Vector2.zero;
			isWait = true;
		}
	}


	/// <summary>
	/// 待機中停止してきょろきょろ振り向きだけ。こいつはパトロールしないからwaitTimeは使いまわしでいい
	/// </summary>
	public void Wait()
	{

				//sAni.Play(SManager.instance.sisStatus.motionIndex["待機"]);
				waitTime += Time.fixedDeltaTime;
				rb.velocity = rb.velocity = new Vector2(0, rb.velocity.y);
				if (waitTime >= SManager.instance.sisStatus.waitRes)
				{
					Flip(-transform.localScale.x);//反転させまーす
					waitTime = 0.0f;
				}
	}


    public void TriggerJump()
    {
		if (jumpTrigger)
		{
			if (!reJump)
            {
				nowPos = transform.position;
				GroundJump(status.jumpSpeed,status.jumpPower);//status.addSpeed * transform.localScale.x/3);
			}
            else
            {
				GroundJump(status.jumpSpeed,status.jumpPower * 1.2f); //status.addSpeed * transform.localScale.x/3);
			}
			if (doJump)
			{
				if(nowPos != transform.position)
                {
					reJump = false;
					doJump = false;
					jumpTrigger = false;
                }
				else if(nowPos== transform.position)
                {
					reJump = true;
					doJump = false;

                }
			}
		}
    }

	public void TriggerCancel()
    {

    }

	/// <summary>
	///	地上キャラ用のジャンプ。ジャンプ状態は自動解除。ジャンプキャンセルとセット
	/// </summary>	
	public void GroundJump(float jumpMove, float jumpPower)
	{
		if (!isStop&& !isDown)
		{
			if (isJump)
			{
				//sAni.Play(SManager.instance.sisStatus.motionIndex["ジャンプ"]);
				jumpTime += Time.fixedDeltaTime;
				rb.AddForce(new Vector2(jumpMove, jumpPower));
				GravitySet(0);
				nowJump = true;
			}
			if (jumpTime >= status.jumpRes)
			{
				//sAni.Play(SManager.instance.sisStatus.motionIndex["落下"]);
				isJump = false;
				jumpTime = 0.0f;
				GravitySet(SManager.instance.sisStatus.firstGravity);
			}
		}
	}

	/// <summary>
	/// ジャンプ終了着地判定。地上キャラのみ
	/// </summary>
	public void JumpCancel()
	{
		if (nowJump && !isJump && rb.IsTouching(SManager.instance.sisStatus.filter))
		{
			nowJump = false;
            if (jumpTrigger)
            {
				doJump = true;
            }
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
		rb.gravityScale = gravity;
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
		rb.velocity = Vector2.zero;
		Invoke("StopBreak", stopRes);
	}

	/// <summary>
	/// 停止解除。単体では使わない
	/// </summary>
	public void StopBreak()
	{
		SetLayer(17);
		isStop = false;
		rb.velocity = Vector2.zero;
	}



	/// <summary>
	/// ダウン状態のメソッド
	/// </summary>
	public void Down()
	{
		Debug.Log("吹き飛ぶ");

		if (isDown && rb.IsTouching(SManager.instance.sisStatus.filter))
		{
			//sAni.Play(SManager.instance.sisStatus.motionIndex["ダウン"]);
			GravitySet(SManager.instance.sisStatus.firstGravity);
			AllStop(1f);

			isDown = false;
			//ダウンアニメ
		}

	}


	private void OnTriggerEnter2D(Collider2D collision)
	{
		//トンネルの中をしゃがみトリガーで満たす
		if (collision.tag == squatTag && pm.isSquat)
		{
			isSquat = true;
		}
		if (collision.tag == jumpTag)
		{
			jumpTrigger = true;
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.tag == squatTag && pm.isSquat)
		{
			isSquat = true;
		}
		if (collision.tag == jumpTag)
		{
			jumpTrigger = true;
		}
	}
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.tag == squatTag && pm.isSquat)
		{
			isSquat = false;
		}
	}





	/// <summary>
	/// ランダムに移動
	/// </summary>
	public void Feint()
	{
		randomTime += Time.fixedDeltaTime;

		if (randomTime >= 3)
		{
			moveDirectionX = RandomValue(0, 100) >= 50 ? 1 : -1;
			moveDirectionY = RandomValue(0, 100) >= 50 ? -1 : 1;
			randomTime = 0.0f;
		}
		rb.AddForce(new Vector2(SManager.instance.sisStatus.addSpeed * (-SManager.instance.sisStatus.walkSpeed - rb.velocity.x) * moveDirectionX,0));

	}


	bool CheckEnd(string _currentStateName)
	{
		return sAni.IsPlaying(_currentStateName);
	}
}



