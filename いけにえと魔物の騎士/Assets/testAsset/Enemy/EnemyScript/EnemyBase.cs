using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{

    // === 外部パラメータ（インスペクタ表示） ===================
    #region

	[Header("待機中のスピード")]
	///<summary>
	///y方向に小刻みに小さな半径で揺らせば翼で飛んでるように見える
	///</summary>
	public Vector2 patrolSpeed;

	[Header("待機状態のテリトリー")]
	///<summary>
	///この間をうろうろしたりする
	///</summary>
	public Vector2 waitDistance;

	[Header("待機状態で縄張りの端に来た時停止する時間")]
	///<summary>
	///端まで来たら一定時間そのまま待ってる。
	///</summary>
	public float waitRes;

	[Header("戦闘中の移動速度")]
	public Vector2 combatSpeed;

	[Header("戦闘時のテリトリー")]
	///<summary>
	///この距離より遠くなるとプレイヤーを見失う
	///</summary>
	public Vector2 escapeDistance;//プレイヤーを取り逃がす距離
	[Header("追跡時間")]
	///<summary>
	///この時間が経過するまで、範囲外に逃げたプレイヤーを追跡する
	///</summary>
	public float chaseRes;//逃げたプレイヤーを追いかける時間

	[Header("攻撃状態で維持する距離")]
	///<summary>
	///戦闘中はこの距離を目指して動く
	///</summary>
	public Vector2 agrDistance;

	[Header("距離の誤差範囲")]
	///<summary>
	///目的距離にゆとり持たせる。近接は小さく遠距離は大きい
	///</summary>
	public float adjust;

	[Header("ジャンプ力")]
	///<summary>
	///AddForceでやる。空飛ぶキャラはきもちジャンプ力低め？
	///</summary>
	public float jumpSpeed;

	[Header("ジャンプの継続時間")]
	public float jumpRes;

	[Header("ジャンプのクールタイム")]
	///<summary>
	///飛行キャラはほぼなし。
	///</summary>
	public float jumpCool;

	[Header("回避時の速度")]
	public Vector2 avoidSpeed;

	[Header("回避の継続時間")]
	public float avoidRes;

	[Header("回避のクールタイム")]
	public float avoidCool;

	[Header("デフォルトの重力")]
	///<summary>
	///最初の重力。飛行キャラは小さく
	///</summary>
	public float firstGravity;//重力。gravityScaleに入れる？　空中キャラはダウンした時だけ重力入る

	[Header("負の最高速度")]
	///<summary>
	///これを超えると修正される
	///</summary>
	public Vector2 velocityMin = new Vector2(-100.0f, -100.0f);

	[Header("正の最高速度")]
	///<summary>
	///これを超えると修正される
	///</summary>
	public Vector2 velocityMax = new Vector2(+100.0f, +50.0f);

	[Header("プレーヤーオブジェクト")]
	///<summary>
	///憎きプレイヤー
	///</summary>
	public GameObject player;
	[Header("視覚探知")]
	///<summary>
	///視覚。これなしの敵がいても面白いかも
	///</summary>
	public GameObject Serch;

	[Header("全方位の気配探知")]
	///<summary>
	///しゃがんで抜けられるほう
	///</summary>
	public GameObject Serch2;

	[Header("エネミーのステータス")]
	public EnemyStatus status;


	[Header("射撃オブジェクトリスト")]
	///<summary>
	///射撃で打ち出すオブジェクト
	///</summary>
	public GameObject[] fireObjectList;
	[Header("接地判定用のフィルター")]
	///<summary>
	///マスクしてやれば地面しか検出しない。
	///</summary>
	[SerializeField] ContactFilter2D filter;

    #endregion
    // === 外部パラメータ ======================================
    /*[HideInInspector]*/ public bool isAggressive;//攻撃モードの敵
	[HideInInspector] public bool superArmor;//これがTrueの時ひるまない
	protected bool isRight = true;//右にパトロールするかどうか
	protected bool isGround;
	protected Vector2 distance;//プレイヤーとの距離
	protected float escapeTime;//数えるだけ
	protected　bool isJump;//ジャンプしてるかどうか
	protected bool isAvoid;
	protected float avoidTime;
	protected float stopTime;//停止時間を数える
	protected bool isStop;//停止フラグ
	protected float waitTime;//パトロール時の待ち時間数える
	protected bool posiReset;//戦闘とかで場を離れたとき戻るためのフラグ
	protected Vector3 firstDirection;//最初に向いてる向き
	protected bool nowJump;
	protected bool isReach;//間合いの中にいる
	protected Vector2 targetPosition;//敵の場所
	protected bool isUp;

	// === キャッシュ ==========================================
	[System.NonSerialized] public Animator animator;
	
	
	
	public Rigidbody2D rb;//継承先で使うものはprotected

	// === 内部パラメータ ======================================
	protected float xSpeed;
	protected float ySpeed;
	protected int direction;
	protected int directionY;
	protected float jumpTime;//ジャンプのクールタイム。処理自体はインパルスで
	protected bool disenableJump;//ジャンプ不可能状態
	protected AudioSource[] seAnimationList;
	protected Vector2 startPosition;//開始位置と待機時テリトリーの起点

	// === コード（Monobehaviour基本機能の実装） ================
	protected virtual void Start()
	{
		animator = GetComponent<Animator>();
		GravitySet(firstGravity);//重力設定
		rb = this.gameObject.GetComponent<Rigidbody2D>();
		startPosition = transform.position;
		firstDirection = transform.localScale;
	}



	protected virtual void Update()
	{
		// 落下チェック

	}

	protected virtual void FixedUpdate()
	{

		Debug.Log($"ジャンプ中{nowJump}");
		Debug.Log($"回避中{isAvoid}");
		if (isAggressive)
        {
			posiReset = true;
			Serch.SetActive(false);
			Serch2.SetActive(false);

			targetPosition = player.transform.position;
			Vector2 myPosition = this.transform.position;
			distance = targetPosition - myPosition;
			direction = distance.x >= 0 ? 1 : -1;//距離が正、または0の時1。そうでないとき-1。方向
			directionY = distance.y >= 0 ? 1 : -1;//弓構えるときのアニメにも使えそう

			Flip(direction);
			AgrMove();
			EscapeCheck();


		}

		else if (!isAggressive)
        {
			Serch.SetActive(true);
			Serch2.SetActive(true);

			PositionReset();

		}
/*		if (isJump && !nowJump)
		{
			jumpTime += Time.fixedDeltaTime;
			if (jumpTime >= jumpCool)
			{
				isJump = false;
				jumpTime = 0;
			}
		}
		if (isAvoid)
		{
			avoidTime += Time.fixedDeltaTime;
			if (avoidTime >= avoidCool)
			{
				isAvoid = false;
				avoidTime = 0;
			}
		}*/
		SetVelocity();
	}
	public void ActionFire()
	{
		if (!isStop && !isAvoid)
		{
			Transform goFire = transform.Find("Muzzle");//銃口から飛んでく
			foreach (GameObject fireObject in fireObjectList)
			{
				GameObject go = Instantiate(fireObject, goFire.position, Quaternion.identity) as GameObject;
				go.GetComponent<FireBullet>().ownwer = transform;
			}
		}
	}

	public void SetVelocity()
    {


		if (rb.velocity.x > velocityMax.x || rb.velocity.x < velocityMin.x)
        {
			Mathf.Clamp(rb.velocity.x, velocityMin.x, velocityMax.x);
		}
		if (rb.velocity.y > velocityMax.y || rb.velocity.y < velocityMin.y)
		{
			Mathf.Clamp(rb.velocity.y, velocityMin.y, velocityMax.y);
		}
		rb.velocity = new Vector2(xSpeed, ySpeed);

	}
	public void Damage()
	{
		float mValue = GManager.instance.pStatus.equipWeapon.mValue;
		//float damage;//バフデバフ処理用にdamageとして保持する
		if (GManager.instance.pStatus.phyAtk > 0)
		{
			//斬撃刺突打撃を管理
			if (GManager.instance.pStatus.equipWeapon.atType == Wepon.AttackType.Slash)
			{ status.hp -= (Mathf.Pow(GManager.instance.pStatus.phyAtk, 2) * mValue) / (GManager.instance.pStatus.phyAtk + status.Def); }
			else if (GManager.instance.pStatus.equipWeapon.atType == Wepon.AttackType.Stab)
			{ status.hp -= (Mathf.Pow(GManager.instance.pStatus.phyAtk, 2) * mValue) / (GManager.instance.pStatus.phyAtk + status.pierDef); }
			else if (GManager.instance.pStatus.equipWeapon.atType == Wepon.AttackType.Strike)
			{ status.hp -= (Mathf.Pow(GManager.instance.pStatus.phyAtk, 2) * mValue) / (GManager.instance.pStatus.phyAtk + status.strDef); }
		}
		//神聖
		if (GManager.instance.pStatus.holyAtk > 0)
		{
			status.hp -= (Mathf.Pow(GManager.instance.pStatus.holyAtk, 2) * mValue) / (GManager.instance.pStatus.holyAtk + status.holyDef);
		}
		//闇
		if (GManager.instance.pStatus.darkAtk > 0)
		{
			status.hp -= (Mathf.Pow(GManager.instance.pStatus.darkAtk, 2) * mValue) / (GManager.instance.pStatus.darkAtk + status.darkDef);
		}
		//炎
		if (GManager.instance.pStatus.fireAtk > 0)
		{
			status.hp -= (Mathf.Pow(GManager.instance.pStatus.fireAtk, 2) * mValue) / (GManager.instance.pStatus.fireAtk + status.fireDef);
		}
		//雷
		if (GManager.instance.pStatus.thunderAtk > 0)
		{
			status.hp -= (Mathf.Pow(GManager.instance.pStatus.thunderAtk, 2) * mValue) / (GManager.instance.pStatus.thunderAtk + status.thunderDef);
		}
	} 

	public void Attack(float mValue, EnemyStatus.AttackType type = EnemyStatus.AttackType.Slash)
    {
		status.atType = type;//まず攻撃タイプを設定
		//mValueはモーション値
		//float damage;//バフデバフ処理用にdamageとして保持する
		if (status.phyAtk > 0)
			{
				//斬撃刺突打撃を管理
				if (status.atType == EnemyStatus.AttackType.Slash)
				{ GManager.instance.pStatus.hp -= (Mathf.Pow(status.phyAtk, 2) * mValue) / (status.phyAtk + GManager.instance.pStatus.Def); }
				else if (status.atType == EnemyStatus.AttackType.Stab)
				{ GManager.instance.pStatus.hp -= (Mathf.Pow(status.phyAtk, 2) * mValue) / (status.phyAtk + GManager.instance.pStatus.pierDef); }
				else if (status.atType == EnemyStatus.AttackType.Strike)
				{ GManager.instance.pStatus.hp -= (Mathf.Pow(status.phyAtk, 2) * mValue) / (status.phyAtk + GManager.instance.pStatus.strDef); }
			}
			//神聖
			if (status.holyAtk > 0)
			{
				GManager.instance.pStatus.hp -= (Mathf.Pow(status.holyAtk, 2) * mValue) / (status.holyAtk + GManager.instance.pStatus.holyDef);
			}
			//闇
			if (status.darkAtk > 0)
			{
			    GManager.instance.pStatus.hp -= (Mathf.Pow(status.darkAtk, 2) * mValue) / (status.darkAtk + GManager.instance.pStatus.darkDef);
			}
			//炎
			if (status.fireAtk > 0)
			{
			    GManager.instance.pStatus.hp -= (Mathf.Pow(status.fireAtk, 2) * mValue) / (status.fireAtk + GManager.instance.pStatus.fireDef);
			}
			//雷
			if (status.thunderAtk > 0)
			{
			    GManager.instance.pStatus.hp -= (Mathf.Pow(status.thunderAtk, 2) * mValue) / (status.thunderAtk + GManager.instance.pStatus.thunderDef);
			}
		}

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
	/// 最初の位置に戻る。
	/// </summary>
	public void PositionReset()
	{
		if (posiReset)
		{
			if (transform.position.x <= startPosition.x)
			{
				if (transform.position.y <= startPosition.y)
				{
					Flip(1);
					rb.AddForce(new Vector2(patrolSpeed.x, patrolSpeed.y));
					if ((transform.position.x >= startPosition.x - 5 && transform.position.x <= startPosition.x + 5)
						&& (transform.position.y >= startPosition.y - 5 && transform.position.y <= startPosition.y + 5))
					{
						posiReset = false;
						isRight = true;
						transform.localScale = firstDirection;
					}
				}
				else if (transform.position.y >= startPosition.y)
				{
					Flip(1);
					rb.AddForce(new Vector2(patrolSpeed.x, - patrolSpeed.y));
					if ((transform.position.x >= startPosition.x - 5 && transform.position.x <= startPosition.x + 5)
						&& (transform.position.y >= startPosition.y - 5 && transform.position.y <= startPosition.y + 5))
					{
						posiReset = false;
						isRight = true;
						transform.localScale = firstDirection;
					}
				}
			}
			else if (transform.position.x >= startPosition.x)
			{
				if (transform.position.y <= startPosition.y)
				{
					Flip(-1);
					rb.AddForce(new Vector2(-patrolSpeed.x, patrolSpeed.y));
					if ((transform.position.x >= startPosition.x - 5 && transform.position.x <= startPosition.x + 5)
						&& (transform.position.y >= startPosition.y - 5 && transform.position.y <= startPosition.y + 5))
					{
						posiReset = false;
						isRight = true;
						transform.localScale = firstDirection;
					}
				}
				else if (transform.position.y >= startPosition.y)
				{
					Flip(-1);
					rb.AddForce(new Vector2(-patrolSpeed.x, -patrolSpeed.y));
					if ((transform.position.x >= startPosition.x - 5 && transform.position.x <= startPosition.x + 5)
						&& (transform.position.y >= startPosition.y - 5 && transform.position.y <= startPosition.y + 5))
					{
						posiReset = false;
						isRight = true;
						transform.localScale = firstDirection;
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

		if (!isStop && !posiReset && !nowJump) {
			if (transform.position.x <= startPosition.x + waitDistance.x && isRight)
			{
				Flip(1);
				rb.AddForce(new Vector2(patrolSpeed.x, 0));


			}
			else if (transform.position.x >= startPosition.x - waitDistance.x && !isRight)
			{
				Flip(-1);
				rb.AddForce(new Vector2(-patrolSpeed.x, 0));

			}
			else
			{
					//Debug.Log("ああああ");
					waitTime += Time.fixedDeltaTime;
					rb.velocity = rb.velocity = new Vector2(0, rb.velocity.y);
					if (waitTime >= waitRes)
					{
						isRight = !isRight;
						waitTime = 0.0f;
					//	Debug.Log("ああああ");
					
				}
			}
		}
    }

	/// <summary>
	/// 待機中空を飛ぶ
	/// </summary>
	public void PatrolFly()
    {
		if (!isStop && !posiReset && !nowJump)
		{
			if (transform.position.y <= startPosition.y + waitDistance.y && isUp)
			{
				rb.AddForce(new Vector2(0,patrolSpeed.y));
			}
			else if(transform.position.y >= startPosition.y - waitDistance.y && !isUp)
			{
				rb.AddForce(new Vector2(0,-patrolSpeed.y));

			}
            else
            {
				isUp = !isUp;
				rb.velocity = Vector2.zero;
            }
		}


	}

	/// <summary>
	/// 待機中停止してきょろきょろ振り向きだけ。こいつはパトロールしないからwaitTimeは使いまわしでいい
	/// </summary>
	public void Wait()
    {
		if (!posiReset)
		{
			waitTime += Time.fixedDeltaTime;
			rb.velocity = rb.velocity = new Vector2(0, rb.velocity.y);
			if (waitTime >= waitRes)
			{
				Flip(-transform.localScale.x);//反転させまーす
				waitTime = 0.0f;						  
			}
		}
	}

	/// <summary>
	/// //プレイヤーが逃げたか確認
	/// </summary>
	public void EscapeCheck()
    {


		if (Mathf.Abs(distance.x) >= escapeDistance.x || Mathf.Abs(distance.y) >= escapeDistance.y)
		{
			escapeTime += Time.fixedDeltaTime;
			if (escapeTime >= chaseRes)
            {
				isAggressive = false;
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
	/// </summary>
	public void AgrMove()
    {
		if (!isStop && !nowJump && !isAvoid)
		{
			if (Mathf.Abs(Mathf.Abs(distance.x) - agrDistance.x) <= adjust)
			{
				rb.velocity = new Vector2(0, rb.velocity.y);
				isReach = true;
			}
			else if (Mathf.Abs(distance.x) - agrDistance.x > 0)
			{
				rb.AddForce(new Vector2(direction * combatSpeed.x, 0));
				isReach = false;
			}
			else if (Mathf.Abs(distance.x) - agrDistance.x < 0)
			{
				rb.AddForce(new Vector2(-direction * combatSpeed.x, 0));
				isReach = false;
			}
		}
	}
	
	/// <summary>
	/// 空を飛ぶタイプのエネミーに戦闘中乗せる。空を飛ぶ
	/// </summary>
	public void AgrFly()
	{
		if (!isStop && !nowJump && !isAvoid)
		{
			if (Mathf.Abs(distance.y - agrDistance.y) <= adjust)//ゆとりの範疇なら我慢
			{
				rb.velocity = new Vector2(rb.velocity.x, 0);
			}
			else if (transform.position.y - targetPosition.y < agrDistance.y)//距離縮めるとき
			{
				rb.AddForce(new Vector2(0,combatSpeed.y));
			}
			else if (transform.position.y - targetPosition.y > agrDistance.y)//距離離すとき
			{
				rb.AddForce(new Vector2(0, -1 * combatSpeed.y));
			}
		}
	}
	/// <summary>
	/// 回避。方向指定も可能だが戦闘時に限りdirectionで前方。-directionで後ろに
	/// </summary>
	/// <param name="direction"></param>
	public void Avoid(float direction) 
	{
		if (!isStop && !nowJump)
		{
			if (isAvoid)
			{
				avoidTime += Time.fixedDeltaTime;
				rb.AddForce(new Vector2(direction * avoidSpeed.x,avoidSpeed.y));
				Flip(direction);
				SetLayer(10);
				if (avoidTime >= avoidRes)
				{
					isAvoid = false;
					avoidTime = 0;
					rb.velocity = new Vector2(0, rb.velocity.y);
					SetLayer(17);
					Debug.Log("初期化");
				}
			}
        }
	
	}

	/// <summary>
	///	地上キャラ用のジャンプ。ジャンプ状態は自動解除。ジャンプキャンセルとセット
	/// </summary>	
	public void GroundJump(float jumpMove)
       {
		if (!isStop && !isAvoid)
		{
			if (isJump)
			{
				jumpTime += Time.fixedDeltaTime;
				rb.AddForce(new Vector2(jumpMove, jumpSpeed));
				GravitySet(0);
				nowJump = true;
			}
			if (jumpTime >= jumpRes)
			{
				isJump = false;
				jumpTime = 0.0f;
				GravitySet(firstGravity);
			}
		}
        }

	/// <summary>
	/// ジャンプ終了着地判定。地上キャラのみ
	/// </summary>
	public void JumpCancel()
	{
		if (nowJump && !isJump && rb.IsTouching(filter))
		{
			nowJump = false;
		}
	}
    
	/// <summary>
	///	飛行キャラ用のジャンプ。ジャンプ状態は自動解除
	/// </summary>	
	public void AirJump(float jumpMove)
	{
		if (!isStop && !isAvoid)
		{
			if (isJump)
			{
				jumpTime += Time.fixedDeltaTime;
				rb.AddForce(new Vector2(jumpMove, jumpSpeed));
			}
			if (jumpTime >= jumpRes)
			{
				isJump = false;
				jumpTime = 0.0f;
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
	public int RandomValue(int X,int Y)
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
		SetLayer(17);
		isStop = true;
		isAvoid = false;
		nowJump = false;
		rb.velocity = Vector2.zero;
		Invoke("StopBreak", stopRes);
    }

	/// <summary>
	/// 停止解除。単体では使わない
	/// </summary>
    public void StopBreak()
    {
		isStop = false;
	}

	/// <summary>
	/// アニメイベントと攻撃後に呼び出す。スーパーアーマー状態を反転させる
	/// </summary>
	public void EnemySuperAmor()
    {
		superArmor = !superArmor;
    }

}


