using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class EnemyBase : MonoBehaviour
{
	//「//sAni.」を「sAni.」で置き換える。アニメ作ったら
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

	public GameObject Guard;

	[Header("射出地点")]
	///<summary>
	///弾丸出す場所
	///</summary>
	public Transform firePosition;

	[HideInInspector] public bool cameraRendered;

	[Header("エネミーのステータス")]
	public EnemyStatus status;
    // === 外部パラメータ ======================================
    /*[HideInInspector]*/ public bool isAggressive;//攻撃モードの敵
	[HideInInspector] public bool isAttack;//これがTrueの時ひるまない
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
	protected int initialLayer;
	protected bool jumpTrigger;
	protected bool isWait;
	protected float stayTime;//攻撃後の待機時間
	protected int attackNumber;//何番の攻撃をしているのか
	protected bool isAtEnable = true;
	protected bool isDamage;

	// === キャッシュ ==========================================
	public Animator animator;
	public SimpleAnimation sAni;
	
	
	public Rigidbody2D rb;//継承先で使うものはprotected

	// === 内部パラメータ ======================================
	protected float xSpeed;
	protected float ySpeed;
	protected int direction;
	protected int directionY;
	protected float moveDirectionX;
	protected float moveDirectionY;
	protected float jumpTime;//ジャンプのクールタイム。処理自体はインパルスで
	protected bool disenableJump;//ジャンプ不可能状態
	protected AudioSource[] seAnimationList;
	protected Vector3 startPosition;//開始位置と待機時テリトリーの起点
	protected Vector3 basePosition;
	protected Vector3 baseDirection;
	protected bool enableFire;
	protected float waitCast;
	protected bool isDown;
	protected float randomTime = 2.5f;//ランダム移動測定のため
	protected bool isGuard;
	[HideInInspector]public bool guardHit;
	protected float stateJudge;//ステート判断間隔
	protected Rigidbody2D dRb;//ドッグパイルのリジッドボディ
							  //	protected float nowDirection;//今の敵の方向

	[HideInInspector]public float damageDelay;
	[HideInInspector]public bool isHit;//ヒット中
	[HideInInspector]public bool isHitable;
	[HideInInspector] public GameObject lastHit;

	// === コード（Monobehaviour基本機能の実装） ================
	protected virtual void Start()
	{
		initialLayer = this.gameObject.layer;
		ArmorReset();
		GravitySet(status.firstGravity);//重力設定
		rb = this.gameObject.GetComponent<Rigidbody2D>();
		startPosition = transform.position;
		firstDirection = transform.localScale;
		HPReset();

		if(status.dogPile != null)
        {
			basePosition = status.dogPile.transform.position;
			baseDirection = status.dogPile.transform.localScale;
			dRb = status.dogPile.GetComponent<Rigidbody2D>();
        }
        else
        {
			basePosition = startPosition;
			baseDirection = firstDirection;

        }
		if(status.enemyFire != null)
        {
			enableFire = true;
        }
	}



	protected virtual void Update()
	{
		// 落下チェック

	}

	protected virtual void FixedUpdate()
	{
        if (!cameraRendered)
        {
			if (!status.unBaind)
			{
				return;
			}
        }
		if (status.dogPile != null)
		{
			basePosition = status.dogPile.transform.position;
			baseDirection = status.dogPile.transform.localScale;
		}
		else
		{
			basePosition = startPosition;
			baseDirection = firstDirection;

		}

		targetPosition = player.transform.position;
		Vector2 myPosition = this.transform.position;
		distance = targetPosition - myPosition;
		direction = distance.x >= 0 ? 1 : -1;//距離が正、または0の時1。そうでないとき-1。方向
		directionY = distance.y >= 0 ? 1 : -1;//弓構えるときのアニメの判定	にも使えそう

		////Debug.Log($"ジャンプ中{nowJump}");
		////Debug.Log($"回避中{isAvoid}");
		if (isAggressive)
        {
			posiReset = true;
			Serch.SetActive(false);
			Serch2.SetActive(false);

			targetPosition = player.transform.position;

			EscapeCheck();

            if (jumpTrigger)
            {
				GroundJump(status.addSpeed.x * direction);
				jumpTrigger = false;
            }
		}

		else if (!isAggressive)
        {


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
		HitCheck();
		WaitAttack();
		DamageAvoid();
	}
	public void ActionFire(int i,float random = 0.0f)
	{//ランダムに入れてもいいけど普通に入れてもいい
		i = i < 0 ? 0 : i;
		i = i > status.enemyFire.Count - 1 ? status.enemyFire.Count : i;

		if (!isStop && !isAvoid)
		{
			waitCast += Time.fixedDeltaTime;
			if(waitCast >= status.enemyFire[i].castTime)
			if (random != 0)
			{
				firePosition.position = new Vector3
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

	public void SetVelocity()
    {


		if (rb.velocity.x > status.velocityMax.x || rb.velocity.x < status.velocityMin.x)
        {
			Mathf.Clamp(rb.velocity.x, status.velocityMin.x, status.velocityMax.x);
		}
		if (rb.velocity.y > status.velocityMax.y || rb.velocity.y < status.velocityMin.y)
		{
			Mathf.Clamp(rb.velocity.y, status.velocityMin.y, status.velocityMax.y);
		}

	}

	/// <summary>
	/// プレイヤーの物理攻撃によるダメージ
	/// </summary>
	public void WeponDamage()
	{
		//isDamage = true;
		//SetLayer(10);
		//Debug.Log($"ガード中か否か{guardHit}");
		if (!guardHit /*&& GManager.instance.pStatus.equipWeapon.hitLimmit > 0 */&& !isDamage)
		{
			isDamage = true;
		//	GManager.instance.pStatus.equipWeapon.hitLimmit--;

			//Debug.Log("終了");
			float damage = 0;//バフデバフ処理用にdamageとして保持する
			float mValue = GManager.instance.pStatus.equipWeapon.mValue;
			//float damage;//バフデバフ処理用にdamageとして保持する
			if (GManager.instance.pStatus.phyAtk > 0)
			{
				//斬撃刺突打撃を管理
				if (GManager.instance.pStatus.equipWeapon.atType == Wepon.AttackType.Slash)
				{ damage += (Mathf.Pow(GManager.instance.pStatus.phyAtk, 2) * mValue) / (GManager.instance.pStatus.phyAtk + status.Def); }
				else if (GManager.instance.pStatus.equipWeapon.atType == Wepon.AttackType.Stab)
				{ damage += (Mathf.Pow(GManager.instance.pStatus.phyAtk, 2) * mValue) / (GManager.instance.pStatus.phyAtk + status.pierDef); }
				else if (GManager.instance.pStatus.equipWeapon.atType == Wepon.AttackType.Strike)
				{ damage += (Mathf.Pow(GManager.instance.pStatus.phyAtk, 2) * mValue) / (GManager.instance.pStatus.phyAtk + status.strDef); }

			}
			//神聖
			if (GManager.instance.pStatus.holyAtk > 0)
			{
				damage += (Mathf.Pow(GManager.instance.pStatus.holyAtk, 2) * mValue) / (GManager.instance.pStatus.holyAtk + status.holyDef);
			}
			//闇
			if (GManager.instance.pStatus.darkAtk > 0)
			{
				damage += (Mathf.Pow(GManager.instance.pStatus.darkAtk, 2) * mValue) / (GManager.instance.pStatus.darkAtk + status.darkDef);
			}
			//炎
			if (GManager.instance.pStatus.fireAtk > 0)
			{
				damage += (Mathf.Pow(GManager.instance.pStatus.fireAtk, 2) * mValue) / (GManager.instance.pStatus.fireAtk + status.fireDef);
			}
			//雷
			if (GManager.instance.pStatus.thunderAtk > 0)
			{
				damage += (Mathf.Pow(GManager.instance.pStatus.thunderAtk, 2) * mValue) / (GManager.instance.pStatus.thunderAtk + status.thunderDef);
			}

			Debug.Log($"{ damage * GManager.instance.pStatus.attackBuff}ダメージ");
			//Debug.Log($"{status.nowArmor}a-mor");
			status.hp -= damage * GManager.instance.pStatus.attackBuff;//HP減らす

			if (!isAttack)
			{
				status.nowArmor -= GManager.instance.pStatus.equipWeapon.shock;

			}
			else
			{
				if (status.aditionalArmor > 0)
				{
					status.nowArmor -= (GManager.instance.pStatus.equipWeapon.shock - status.aditionalArmor) < 0 ? 0 : (GManager.instance.pStatus.equipWeapon.shock - status.aditionalArmor);
					status.aditionalArmor -= GManager.instance.pStatus.equipWeapon.shock;
				}
			}
			ArmorControll();
			if (isDown && GManager.instance.pStatus.equipWeapon.isBlow)
			{
				rb.AddForce(new Vector2(GManager.instance.pStatus.equipWeapon.blowPower.x * -direction, GManager.instance.pStatus.equipWeapon.blowPower.y), ForceMode2D.Impulse);
				Invoke("Down", 1f);
			}
			else if (isDown)
			{
				NockBack();
			}
		}
	}

	public void WeponGuard()
	{
		if (/*&& GManager.instance.pStatus.equipWeapon.hitLimmit > 0 */!isDamage)
		{
			isDamage = true;
			//GManager.instance.pStatus.equipWeapon.hitLimmit--;

			//Debug.Log("終了");
			float damage = 0;//バフデバフ処理用にdamageとして保持する
			float mValue = GManager.instance.pStatus.equipWeapon.mValue;
			//float damage;//バフデバフ処理用にdamageとして保持する
			if (GManager.instance.pStatus.phyAtk > 0)
			{
				//斬撃刺突打撃を管理
				if (GManager.instance.pStatus.equipWeapon.atType == Wepon.AttackType.Slash)
				{ damage += (Mathf.Pow(GManager.instance.pStatus.phyAtk, 2) * mValue) / (GManager.instance.pStatus.phyAtk + status.Def); }
				else if (GManager.instance.pStatus.equipWeapon.atType == Wepon.AttackType.Stab)
				{ damage += (Mathf.Pow(GManager.instance.pStatus.phyAtk, 2) * mValue) / (GManager.instance.pStatus.phyAtk + status.pierDef); }
				else if (GManager.instance.pStatus.equipWeapon.atType == Wepon.AttackType.Strike)
				{ damage += (Mathf.Pow(GManager.instance.pStatus.phyAtk, 2) * mValue) / (GManager.instance.pStatus.phyAtk + status.strDef); }

				damage *= (100 - status.phyCut)/100;
			}
			//神聖
			if (GManager.instance.pStatus.holyAtk > 0)
			{
				damage += (Mathf.Pow(GManager.instance.pStatus.holyAtk, 2) * mValue) / (GManager.instance.pStatus.holyAtk + status.holyDef) * (100 - status.holyCut)/100;
			}
			//闇
			if (GManager.instance.pStatus.darkAtk > 0)
			{
				damage += (Mathf.Pow(GManager.instance.pStatus.darkAtk, 2) * mValue) / (GManager.instance.pStatus.darkAtk + status.darkDef) * (100 - status.darkCut)/100;
			}
			//炎
			if (GManager.instance.pStatus.fireAtk > 0)
			{
				damage += (Mathf.Pow(GManager.instance.pStatus.fireAtk, 2) * mValue) / (GManager.instance.pStatus.fireAtk + status.fireDef) * (100 - status.fireCut)/100;
			}
			//雷
			if (GManager.instance.pStatus.thunderAtk > 0)
			{
				damage += (Mathf.Pow(GManager.instance.pStatus.thunderAtk, 2) * mValue) / (GManager.instance.pStatus.thunderAtk + status.thunderDef) * (100 - status.thunderCut)/100;
			}

			//Debug.Log($"{ damage * GManager.instance.pStatus.attackBuff}ダメージ");
			//Debug.Log($"{status.nowArmor}a-mor");
			status.hp -= damage * GManager.instance.pStatus.attackBuff;//HP減らす

			status.nowArmor -= (GManager.instance.pStatus.equipWeapon.shock * 2) * ((100 - status.guardPower) / 100);

			if(GManager.instance.pStatus.equipWeapon.isLight && status.guardPower >= 50)
            {
				GManager.instance.isBounce = true;//受け値50以上で弾く

			}
			if (status.nowArmor <= 0)
			{
				GuardBreak();
			}
		}
	}

	/// <summary>
	/// プレイヤーの魔法で受けるダメージ
	/// </summary>
	public void PlayerMagicDamage()
    {
		if (!guardHit /*&& GManager.instance.pStatus.equipWeapon.hitLimmit > 0 */&& !isDamage)
		{
			isDamage = true;
			//GManager.instance.pStatus.equipWeapon.hitLimmit--;
			float damage = 0;//バフデバフ処理用にdamageとして保持する
			float mValue = GManager.instance.pStatus.useMagic.mValue;
			//float damage;//バフデバフ処理用にdamageとして保持する
			if (GManager.instance.pStatus.useMagic.phyAtk > 0)
			{
				//斬撃刺突打撃を管理
				if (GManager.instance.pStatus.useMagic.atType == Magic.AttackType.Slash)
				{ damage += (Mathf.Pow(GManager.instance.pStatus.useMagic.phyAtk, 2) * mValue) / (GManager.instance.pStatus.useMagic.phyAtk + status.Def); }
				else if (GManager.instance.pStatus.useMagic.atType == Magic.AttackType.Stab)
				{ damage += (Mathf.Pow(GManager.instance.pStatus.useMagic.phyAtk, 2) * mValue) / (GManager.instance.pStatus.useMagic.phyAtk + status.pierDef); }
				else if (GManager.instance.pStatus.useMagic.atType == Magic.AttackType.Strike)
				{ damage += (Mathf.Pow(GManager.instance.pStatus.useMagic.phyAtk, 2) * mValue) / (GManager.instance.pStatus.useMagic.phyAtk + status.strDef); }
			}
			//神聖
			if (GManager.instance.pStatus.useMagic.holyAtk > 0)
			{
				damage += (Mathf.Pow(GManager.instance.pStatus.useMagic.holyAtk, 2) * mValue) / (GManager.instance.pStatus.useMagic.holyAtk + status.holyDef);
			}
			//闇
			if (GManager.instance.pStatus.useMagic.darkAtk > 0)
			{
				damage += (Mathf.Pow(GManager.instance.pStatus.useMagic.darkAtk, 2) * mValue) / (GManager.instance.pStatus.useMagic.darkAtk + status.darkDef);
			}
			//炎
			if (GManager.instance.pStatus.useMagic.fireAtk > 0)
			{
				damage += (Mathf.Pow(GManager.instance.pStatus.useMagic.fireAtk, 2) * mValue) / (GManager.instance.pStatus.useMagic.fireAtk + status.fireDef);
			}
			//雷
			if (GManager.instance.pStatus.useMagic.thunderAtk > 0)
			{
				damage += (Mathf.Pow(GManager.instance.pStatus.useMagic.thunderAtk, 2) * mValue) / (GManager.instance.pStatus.useMagic.thunderAtk + status.thunderDef);
			}
			status.hp -= damage * GManager.instance.pStatus.attackBuff;//HP引いてる
			if (!isAttack)
			{
				status.nowArmor -= GManager.instance.pStatus.useMagic.shock;
			}
			else
			{
				if (status.aditionalArmor > 0)
				{
					status.nowArmor -= (GManager.instance.pStatus.equipWeapon.shock - status.aditionalArmor) < 0 ? 0 : (GManager.instance.pStatus.equipWeapon.shock - status.aditionalArmor);
					status.aditionalArmor -= GManager.instance.pStatus.equipWeapon.shock;
				}
			}
			ArmorControll();
			if (isDown && GManager.instance.pStatus.useMagic.isBlow)
			{
				rb.AddForce(new Vector2(GManager.instance.pStatus.useMagic.blowPower.x * -direction, GManager.instance.pStatus.useMagic.blowPower.y), ForceMode2D.Impulse);
				Invoke("Down",1f);
			}
			else if (isDown)
			{
				NockBack();
			}
		}
	}
	public void PlayerMagicGuard()
	{
		if (/*&& GManager.instance.pStatus.equipWeapon.hitLimmit > 0 */!isDamage)
		{
			isDamage = true;

			//GManager.instance.pStatus.equipWeapon.hitLimmit--;
			float damage = 0;//バフデバフ処理用にdamageとして保持する
			float mValue = GManager.instance.pStatus.useMagic.mValue;
			//float damage;//バフデバフ処理用にdamageとして保持する
			if (GManager.instance.pStatus.useMagic.phyAtk > 0)
			{
				//斬撃刺突打撃を管理
				if (GManager.instance.pStatus.useMagic.atType == Magic.AttackType.Slash)
				{ damage += (Mathf.Pow(GManager.instance.pStatus.useMagic.phyAtk, 2) * mValue) / (GManager.instance.pStatus.useMagic.phyAtk + status.Def); }
				else if (GManager.instance.pStatus.useMagic.atType == Magic.AttackType.Stab)
				{ damage += (Mathf.Pow(GManager.instance.pStatus.useMagic.phyAtk, 2) * mValue) / (GManager.instance.pStatus.useMagic.phyAtk + status.pierDef); }
				else if (GManager.instance.pStatus.useMagic.atType == Magic.AttackType.Strike)
				{ damage += (Mathf.Pow(GManager.instance.pStatus.useMagic.phyAtk, 2) * mValue) / (GManager.instance.pStatus.useMagic.phyAtk + status.strDef); }
				damage *= (100 - status.phyCut)/100;
			}
			//神聖
			if (GManager.instance.pStatus.useMagic.holyAtk > 0)
			{
				damage += (Mathf.Pow(GManager.instance.pStatus.useMagic.holyAtk, 2) * mValue) / (GManager.instance.pStatus.useMagic.holyAtk + status.holyDef) * (100 - status.holyCut)/100;
			}
			//闇
			if (GManager.instance.pStatus.useMagic.darkAtk > 0)
			{
				damage += (Mathf.Pow(GManager.instance.pStatus.useMagic.darkAtk, 2) * mValue) / (GManager.instance.pStatus.useMagic.darkAtk + status.darkDef * status.darkCut) * (100 - status.darkCut)/100;
			}
			//炎
			if (GManager.instance.pStatus.useMagic.fireAtk > 0)
			{
				damage += (Mathf.Pow(GManager.instance.pStatus.useMagic.fireAtk, 2) * mValue) / (GManager.instance.pStatus.useMagic.fireAtk + status.fireDef) * status.fireCut * (100 - status.fireCut)/100;
			}
			//雷
			if (GManager.instance.pStatus.useMagic.thunderAtk > 0)
			{
				damage += (Mathf.Pow(GManager.instance.pStatus.useMagic.thunderAtk, 2) * mValue) / (GManager.instance.pStatus.useMagic.thunderAtk + status.thunderDef) * (100 - status.thunderCut)/100;
			}
			status.hp -= damage * GManager.instance.pStatus.attackBuff;//HP引いてる
			if (GManager.instance.pStatus.equipWeapon.isBlow)
			{
				status.nowArmor -= ((GManager.instance.pStatus.useMagic.shock * 2) * status.guardPower / 100) * 1.2f;
			}
			else
			{
				status.nowArmor -= (GManager.instance.pStatus.useMagic.shock * 2) * ((100 - status.guardPower) / 100);
			}

			if (status.nowArmor <= 0)
			{
				GuardBreak();
			}
		}
	}
	public void SisterMagicDamage()
	{
		if (!guardHit /*&& GManager.instance.pStatus.equipWeapon.hitLimmit > 0 */&& !isDamage)
		{
			isDamage = true;
			//GManager.instance.pStatus.equipWeapon.hitLimmit--;
			float damage = 0;//バフデバフ処理用にdamageとして保持する
			float mValue = SManager.instance.sisStatus.useMagic.mValue;
			//float damage;//バフデバフ処理用にdamageとして保持する
			if (SManager.instance.sisStatus.useMagic.phyAtk > 0)
			{
				//斬撃刺突打撃を管理
				if (SManager.instance.sisStatus.useMagic.atType == Magic.AttackType.Slash)
				{ damage += (Mathf.Pow(SManager.instance.sisStatus.useMagic.phyAtk, 2) * mValue) / (SManager.instance.sisStatus.useMagic.phyAtk + status.Def); }
				else if (SManager.instance.sisStatus.useMagic.atType == Magic.AttackType.Stab)
				{ damage += (Mathf.Pow(SManager.instance.sisStatus.useMagic.phyAtk, 2) * mValue) / (SManager.instance.sisStatus.useMagic.phyAtk + status.pierDef); }
				else if (SManager.instance.sisStatus.useMagic.atType == Magic.AttackType.Strike)
				{ damage += (Mathf.Pow(SManager.instance.sisStatus.useMagic.phyAtk, 2) * mValue) / (SManager.instance.sisStatus.useMagic.phyAtk + status.strDef); }
			}
			//神聖
			if (SManager.instance.sisStatus.useMagic.holyAtk > 0)
			{
				damage += (Mathf.Pow(SManager.instance.sisStatus.useMagic.holyAtk, 2) * mValue) / (SManager.instance.sisStatus.useMagic.holyAtk + status.holyDef);
			}
			//闇
			if (SManager.instance.sisStatus.useMagic.darkAtk > 0)
			{
				damage += (Mathf.Pow(SManager.instance.sisStatus.useMagic.darkAtk, 2) * mValue) / (SManager.instance.sisStatus.useMagic.darkAtk + status.darkDef);
			}
			//炎
			if (SManager.instance.sisStatus.useMagic.fireAtk > 0)
			{
				damage += (Mathf.Pow(SManager.instance.sisStatus.useMagic.fireAtk, 2) * mValue) / (SManager.instance.sisStatus.useMagic.fireAtk + status.fireDef);
			}
			//雷
			if (SManager.instance.sisStatus.useMagic.thunderAtk > 0)
			{
				damage += (Mathf.Pow(SManager.instance.sisStatus.useMagic.thunderAtk, 2) * mValue) / (SManager.instance.sisStatus.useMagic.thunderAtk + status.thunderDef);
			}
			status.hp -= damage * SManager.instance.sisStatus.attackBuff;
			if (!isAttack)
			{
				status.nowArmor -= SManager.instance.sisStatus.useMagic.shock;
			}
			else
			{
				if (status.aditionalArmor > 0)
				{
					status.nowArmor -= (GManager.instance.pStatus.equipWeapon.shock - status.aditionalArmor) < 0 ? 0 : (GManager.instance.pStatus.equipWeapon.shock - status.aditionalArmor);
					status.aditionalArmor -= GManager.instance.pStatus.equipWeapon.shock;
				}
			}
			ArmorControll();
			if (isDown && SManager.instance.sisStatus.useMagic.isBlow)
			{
				rb.AddForce(new Vector2(SManager.instance.sisStatus.useMagic.blowPower.x * direction, SManager.instance.sisStatus.useMagic.blowPower.y), ForceMode2D.Impulse);
				Invoke("Down", 1f);
			}
			else if (isDown)
			{
				NockBack();
			}
		}
	}
	public void SisterMagicGuard()
	{
		if (/*&& GManager.instance.pStatus.equipWeapon.hitLimmit > 0 */!isDamage)
		{
			isDamage = true;
			//GManager.instance.pStatus.equipWeapon.hitLimmit--;
			float damage = 0;//バフデバフ処理用にdamageとして保持する
			float mValue = SManager.instance.sisStatus.useMagic.mValue;
			//float damage;//バフデバフ処理用にdamageとして保持する
			if (SManager.instance.sisStatus.useMagic.phyAtk > 0)
			{
				//斬撃刺突打撃を管理
				if (SManager.instance.sisStatus.useMagic.atType == Magic.AttackType.Slash)
				{ damage += (Mathf.Pow(SManager.instance.sisStatus.useMagic.phyAtk, 2) * mValue) / (SManager.instance.sisStatus.useMagic.phyAtk + status.Def); }
				else if (SManager.instance.sisStatus.useMagic.atType == Magic.AttackType.Stab)
				{ damage += (Mathf.Pow(SManager.instance.sisStatus.useMagic.phyAtk, 2) * mValue) / (SManager.instance.sisStatus.useMagic.phyAtk + status.pierDef); }
				else if (SManager.instance.sisStatus.useMagic.atType == Magic.AttackType.Strike)
				{ damage += (Mathf.Pow(SManager.instance.sisStatus.useMagic.phyAtk, 2) * mValue) / (SManager.instance.sisStatus.useMagic.phyAtk + status.strDef); }

				damage *= (100 - status.phyCut)/100;
			}
			//神聖
			if (SManager.instance.sisStatus.useMagic.holyAtk > 0)
			{
				damage += (Mathf.Pow(SManager.instance.sisStatus.useMagic.holyAtk, 2) * mValue) / (SManager.instance.sisStatus.useMagic.holyAtk + status.holyDef) * (100 - status.holyCut)/100;
			}
			//闇
			if (SManager.instance.sisStatus.useMagic.darkAtk > 0)
			{
				damage += (Mathf.Pow(SManager.instance.sisStatus.useMagic.darkAtk, 2) * mValue) / (SManager.instance.sisStatus.useMagic.darkAtk + status.darkDef) * (100 - status.darkCut)/100;
			}
			//炎
			if (SManager.instance.sisStatus.useMagic.fireAtk > 0)
			{
				damage += (Mathf.Pow(SManager.instance.sisStatus.useMagic.fireAtk, 2) * mValue) / (SManager.instance.sisStatus.useMagic.fireAtk + status.fireDef) * (100 - status.fireCut)/100;
			}
			//雷
			if (SManager.instance.sisStatus.useMagic.thunderAtk > 0)
			{
				damage += (Mathf.Pow(SManager.instance.sisStatus.useMagic.thunderAtk, 2) * mValue) / (SManager.instance.sisStatus.useMagic.thunderAtk + status.thunderDef) * (100 -status.thunderCut)/100;
			}
			status.hp -= damage * SManager.instance.sisStatus.attackBuff;

			if (GManager.instance.pStatus.equipWeapon.isBlow)
			{
				status.nowArmor -= ((SManager.instance.sisStatus.useMagic.shock * 2) * status.guardPower / 100) * 1.2f;
			}
			else
			{
				status.nowArmor -= (SManager.instance.sisStatus.useMagic.shock * 2) * ((100 - status.guardPower) / 100);
			}
			
            if (status.nowArmor <= 0)
			{
				GuardBreak();
			}
		}
	}
	public void AttackDamage()
    {
		if (!GManager.instance.isDamage)
		{
			GManager.instance.isDamage = true;
			//status.hitLimmit--;
								 //mValueはモーション値
			float damage = 0;//バフデバフ処理用にdamageとして保持する
			if (status.phyAtk > 0)
			{
				//斬撃刺突打撃を管理
				if (status.atType == EnemyStatus.AttackType.Slash)
				{ damage += (Mathf.Pow(status.phyAtk, 2) * status.mValue) / (status.phyAtk + GManager.instance.pStatus.Def); }
				else if (status.atType == EnemyStatus.AttackType.Stab)
				{ damage += (Mathf.Pow(status.phyAtk, 2) * status.mValue) / (status.phyAtk + GManager.instance.pStatus.pierDef); }
				else if (status.atType == EnemyStatus.AttackType.Strike)
				{ damage += (Mathf.Pow(status.phyAtk, 2) * status.mValue) / (status.phyAtk + GManager.instance.pStatus.strDef); }
			}
			//神聖
			if (status.holyAtk > 0)
			{
				damage += (Mathf.Pow(status.holyAtk, 2) * status.mValue) / (status.holyAtk + GManager.instance.pStatus.holyDef);
			}
			//闇
			if (status.darkAtk > 0)
			{
				damage += (Mathf.Pow(status.darkAtk, 2) * status.mValue) / (status.darkAtk + GManager.instance.pStatus.darkDef);
			}
			//炎
			if (status.fireAtk > 0)
			{
				damage += (Mathf.Pow(status.fireAtk, 2) * status.mValue) / (status.fireAtk + GManager.instance.pStatus.fireDef);
			}
			//雷
			if (status.thunderAtk > 0)
			{
				damage += (Mathf.Pow(status.thunderAtk, 2) * status.mValue) / (status.thunderAtk + GManager.instance.pStatus.thunderDef);
			}
			if (!GManager.instance.isAttack)
			{
				GManager.instance.pStatus.nowArmor -= status.Shock;
			}
			else
			{
				GManager.instance.pStatus.nowArmor -= (status.Shock - GManager.instance.pStatus.equipWeapon.atAromor) < 0 ? 0 : (status.Shock - GManager.instance.pStatus.equipWeapon.atAromor);
			}
			GManager.instance.pStatus.hp -= damage * status.attackBuff;
		}
		if (status.isBlow == true && GManager.instance.pStatus.nowArmor <= 0)
		{
			GManager.instance.isBlow = true;

			if (direction > 0)
			{
				GManager.instance.blowVector = status.blowVector;

			}
			if (direction < 0)
			{
				GManager.instance.blowVector = new Vector2(-status.blowVector.x, status.blowVector.y);

			}
		}
}

	public void PlayerGuard()
    {
		if (!GManager.instance.isDamage && GManager.instance.isGuard)
		{
			//status.hitLimmit--;
			//mValueはモーション値
			float damage = 0;//バフデバフ処理用にdamageとして保持する
			if (status.phyAtk > 0)
			{
				//斬撃刺突打撃を管理
				if (status.atType == EnemyStatus.AttackType.Slash)
				{ damage += (Mathf.Pow(status.phyAtk, 2) * status.mValue) / (status.phyAtk + GManager.instance.pStatus.Def); }
				else if (status.atType == EnemyStatus.AttackType.Stab)
				{ damage += (Mathf.Pow(status.phyAtk, 2) * status.mValue) / (status.phyAtk + GManager.instance.pStatus.pierDef); }
				else if (status.atType == EnemyStatus.AttackType.Strike)
				{ damage += (Mathf.Pow(status.phyAtk, 2) * status.mValue) / (status.phyAtk + GManager.instance.pStatus.strDef); }

				damage *= (100 - GManager.instance.pStatus.phyCut)/100;
			}
			//神聖
			if (status.holyAtk > 0)
			{
				damage += ((Mathf.Pow(status.holyAtk, 2) * status.mValue) / (status.holyAtk + GManager.instance.pStatus.holyDef)) * (100 - GManager.instance.pStatus.holyCut)/100;
			}
			//闇
			if (status.darkAtk > 0)
			{
				damage += ((Mathf.Pow(status.darkAtk, 2) * status.mValue) / (status.darkAtk + GManager.instance.pStatus.darkDef)) * (100 - GManager.instance.pStatus.darkCut)/100;
			}
				//炎
				if (status.fireAtk > 0)
			{
				damage += ((Mathf.Pow(status.fireAtk, 2) * status.mValue) / (status.fireAtk + GManager.instance.pStatus.fireDef)) * (100 - GManager.instance.pStatus.fireCut)/100;
			}
			//雷
			if (status.thunderAtk > 0)
			{
				damage += ((Mathf.Pow(status.thunderAtk, 2) * status.mValue) / (status.thunderAtk + GManager.instance.pStatus.thunderDef)) * (100 - GManager.instance.pStatus.thunderCut)/100;
			}
			if (status.isLight && GManager.instance.pStatus.guardPower >= 50)
			{//受け値50以上の盾は軽い攻撃をはじく
				//sAni.Play(status.motionIndex["弾かれ"]);
			}
			    GManager.instance.pStatus.stamina -= (status.Shock * 2) * (1 - (GManager.instance.pStatus.guardPower / 100));

			if(GManager.instance.pStatus.stamina <= 0)
            {
				GManager.instance.isGBreak = true;
				GManager.instance.isGuard = false;
            }

			GManager.instance.pStatus.hp -= damage * status.attackBuff;
		}
		if (status.isBlow == true && GManager.instance.pStatus.nowArmor <= 0)
		{
			GManager.instance.isBlow = true;

			if (direction > 0)
			{
				GManager.instance.blowVector = status.blowVector;

			}
			if (direction < 0)
			{
				GManager.instance.blowVector = new Vector2(-status.blowVector.x, status.blowVector.y);

			}
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
		if (posiReset && !isDown && !isStop)
		{
			if (transform.position.x <= basePosition.x)
			{
				if (transform.position.y <= basePosition.y)
				{
					Flip(1);
					//sAni.Play(status.motionIndex[status.dogPile == null ? "歩き":"走り"]);
					rb.AddForce(new Vector2(status.addSpeed.x * ((status.dogPile == null ? status.patrolSpeed.x : status.combatSpeed.x) - rb.velocity.x), 0));
					rb.AddForce(new Vector2(0, (status.addSpeed.y * (status.dogPile == null ? status.patrolSpeed.y : status.combatSpeed.y)) - rb.velocity.y));

					if (((transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)
						&& (transform.position.y >= basePosition.y - 5 && transform.position.y <= basePosition.y + 5) &&
						(status.kind == EnemyStatus.KindofEnemy.Fly)) || ((status.kind != EnemyStatus.KindofEnemy.Fly) &&
							(transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)))
					{
						//sAni.Play(status.motionIndex["待機"]);
						//スタートが地面だから大丈夫。落ちてくる系のやつにはDogpileを与えてやれ
						rb.velocity = Vector2.zero;
						posiReset = false;
						isRight = true;
						transform.localScale = baseDirection;
					}
				}
				else if (transform.position.y >= basePosition.y)
				{
					Flip(1);
					rb.AddForce(new Vector2(status.addSpeed.x * ((status.dogPile == null ? status.patrolSpeed.x : status.combatSpeed.x) - rb.velocity.x), 0));
					rb.AddForce(new Vector2(0, (status.addSpeed.y * (status.dogPile == null ? -status.patrolSpeed.y : -status.combatSpeed.y)) - rb.velocity.y));
					if (((transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)
						&& (transform.position.y >= basePosition.y - 5 && transform.position.y <= basePosition.y + 5) &&
						(status.kind == EnemyStatus.KindofEnemy.Fly)) || ((status.kind != EnemyStatus.KindofEnemy.Fly) &&
							(transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)))
					{
						//sAni.Play(status.motionIndex["待機"]);
						rb.velocity = Vector2.zero;
						posiReset = false;
						isRight = true;
						transform.localScale = baseDirection;
					}
				}
			}
			else if (transform.position.x >= basePosition.x)
			{
				if (transform.position.y <= basePosition.y)
				{
					Flip(-1);
					rb.AddForce(new Vector2(status.addSpeed.x * ((status.dogPile == null ? -status.patrolSpeed.x : -status.combatSpeed.x) - rb.velocity.x), 0));
					rb.AddForce(new Vector2(0, (status.addSpeed.y * (status.dogPile == null ? status.patrolSpeed.y : status.combatSpeed.y)) - rb.velocity.y));
					if (((transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)
						&& (transform.position.y >= basePosition.y - 5 && transform.position.y <= basePosition.y + 5) &&
						(status.kind == EnemyStatus.KindofEnemy.Fly)) || ((status.kind != EnemyStatus.KindofEnemy.Fly) &&
						(transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)))
					{
						//sAni.Play(status.motionIndex["待機"]);
						rb.velocity = Vector2.zero;
						posiReset = false;
						isRight = true;
						transform.localScale = baseDirection;
					}
				}
				else if (transform.position.y >= basePosition.y)
				{
					Flip(-1);
					rb.AddForce(new Vector2(status.addSpeed.x * ((status.dogPile == null ? -status.patrolSpeed.x : -status.combatSpeed.x) - rb.velocity.x), 0));
					rb.AddForce(new Vector2(0, (status.addSpeed.y * (status.dogPile == null ? -status.patrolSpeed.y : -status.combatSpeed.y)) - rb.velocity.y));
					if (((transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)
						&& (transform.position.y >= basePosition.y - 5 && transform.position.y <= basePosition.y + 5) &&
						(status.kind == EnemyStatus.KindofEnemy.Fly)) || ((status.kind != EnemyStatus.KindofEnemy.Fly) &&
						(transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)))
					{
						//sAni.Play(status.motionIndex["待機"]);
						rb.velocity = Vector2.zero;
						posiReset = false;
						isRight = true;
						transform.localScale = baseDirection;
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

		if (!isStop && !posiReset && !nowJump && !isDown) {
			if (transform.position.x <= startPosition.x + status.waitDistance.x && isRight)
			{
				//sAni.Play(status.motionIndex["歩き"]);
				Flip(1);
				rb.AddForce(new Vector2(status.addSpeed.x * (status.patrolSpeed.x - rb.velocity.x), 0));

			}
			else if (transform.position.x >= startPosition.x - status.waitDistance.x && !isRight)
			{
				//sAni.Play(status.motionIndex["歩き"]);
				Flip(-1);

				rb.AddForce(new Vector2(status.addSpeed.x * (-status.patrolSpeed.x - rb.velocity.x), 0));

			}
			else
			{
					////Debug.Log("ああああ");
					waitTime += Time.fixedDeltaTime;
					rb.velocity = new Vector2(0, rb.velocity.y);
					if (waitTime >= status.waitRes)
					{
						isRight = !isRight;
						waitTime = 0.0f;
					//	//Debug.Log("ああああ");
					
				}
			}
		}
    }

	/// <summary>
	/// 待機中空を飛ぶ
	/// </summary>
	public void PatrolFly()
    {
		if (!isStop && !posiReset && !nowJump && !isDown)
		{
			if (transform.position.y <= startPosition.y + status.waitDistance.y && isUp)
			{

				rb.AddForce(new Vector2(0, status.addSpeed.y * (status.patrolSpeed.y - rb.velocity.y)));
			}
			else if(transform.position.y >= startPosition.y - status.waitDistance.y && !isUp)
			{
				rb.AddForce(new Vector2(0, status.addSpeed.y * (-status.patrolSpeed.y - rb.velocity.y)));
			}
            else
            {
				isUp = !isUp;
				rb.velocity = new Vector2(rb.velocity.x,0);
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
			if (dRb == null)
			{
				//sAni.Play(status.motionIndex["待機"]);
				waitTime += Time.fixedDeltaTime;
				rb.velocity = rb.velocity = new Vector2(0, rb.velocity.y);
				if (waitTime >= status.waitRes)
				{
					Flip(-transform.localScale.x);//反転させまーす
					waitTime = 0.0f;
				}
			}
			if(dRb != null)
            {
				//sAni.Play(status.motionIndex["歩き"]);
				rb.velocity = dRb.velocity;
				transform.localScale = status.dogPile.transform.localScale;
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
			escapeTime += Time.fixedDeltaTime;
			if (escapeTime >= status.chaseRes)
            {
				Serch.SetActive(true);
				Serch2.SetActive(true);
				isAggressive = false;
				status.ground = EnemyStatus.MoveState.wakeup;
				status.air = EnemyStatus.MoveState.wakeup;
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
		stateJudge += Time.fixedDeltaTime;
		#region//判断
		if ((status.ground == EnemyStatus.MoveState.wakeup || stateJudge >= status.judgePace) && status.ground != EnemyStatus.MoveState.escape)
		//escapeだけはスクリプトから動かす
		{
			if (Mathf.Abs(distance.x) <= status.agrDistance.x + status.adjust && Mathf.Abs(distance.x) >= status.agrDistance.x - status.adjust)
			{
				status.ground = EnemyStatus.MoveState.stay;
			}
			else if (Mathf.Abs(distance.x) > status.agrDistance.x)//近づく方じゃね？
			{
				if (Mathf.Abs(distance.x) <= status.walkDistance.x || isGuard)
				{
					status.ground = EnemyStatus.MoveState.accessWalk;
				}
				else
				{
					status.ground = EnemyStatus.MoveState.accessDash;
				}
			}
			else if (Mathf.Abs(distance.x) < status.agrDistance.x)//遠ざかる
			{
				//歩き距離なら敵を見たまま撃つ
				//動かない弓兵とかは移動速度ゼロに
				if (Mathf.Abs((Mathf.Abs(distance.x) - status.agrDistance.x)) <= -status.walkDistance.x || isGuard)
				{
					status.ground = EnemyStatus.MoveState.leaveWalk;
				}
				else
				{
					status.ground = EnemyStatus.MoveState.leaveDash;
				}
			}
			stateJudge = 0;
		}
		#endregion

		if (!isStop && !nowJump && !isAvoid && !isDown)
		{
			if (status.ground == EnemyStatus.MoveState.stay)
			{
				//sAni.Play(status.motionIndex["構え"]);
				Flip(direction);
				rb.velocity = new Vector2(0, rb.velocity.y);
				isReach = true;
			}
			else if (status.ground == EnemyStatus.MoveState.accessWalk)
			{
				Flip(direction);
				isReach = (Mathf.Abs(distance.x) - status.agrDistance.x) <= status.walkDistance.x ? true : false;
				rb.AddForce(new Vector2(status.addSpeed.x * (direction * status.walkSpeed.x - rb.velocity.x), 0));
				//sAni.Play(status.motionIndex["歩き"]);
			}
			else if (status.ground == EnemyStatus.MoveState.accessDash) 
			{
				Flip(direction);
				isReach = false;
				//sAni.Play(status.motionIndex["走り"]);
				rb.AddForce(new Vector2(status.addSpeed.x * (direction * status.combatSpeed.x - rb.velocity.x),0));
                if (Mathf.Abs(distance.x) <= status.agrDistance.x + status.adjust && Mathf.Abs(distance.x) >= status.agrDistance.x - status.adjust)
                {
					status.ground = EnemyStatus.MoveState.accessWalk;
					stateJudge = 0.0f;
				}
			}
			else if (status.ground == EnemyStatus.MoveState.leaveWalk)//遠ざかる
			{
				//近距離の場合歩き範囲をダッシュで離れるのより大きく
				//歩き距離なら敵を見たまま撃つ
				//動かない弓兵とかは移動速度ゼロに
					Flip(direction);
					isReach = true;
				//sAni.Play(status.motionIndex["後ずさり"]);
				rb.AddForce(new Vector2(status.addSpeed.x * (-direction * status.walkSpeed.x - rb.velocity.x), 0));
			}
				else if(status.ground == EnemyStatus.MoveState.leaveDash)
				{
					//sAni.Play(status.motionIndex["走り"]);
					Flip(-direction);
					isReach = false;
				rb.AddForce(new Vector2(status.addSpeed.x * (-direction * status.combatSpeed.x - rb.velocity.x),0));
				}
			}
	}
	
	/// <summary>
	/// 空を飛ぶタイプのエネミーに戦闘中乗せる。空を飛ぶ
	/// </summary>
	public void AgrFly()
	{
		float judgeDistance = transform.position.y - targetPosition.y;

		if (!isStop && !nowJump && !isAvoid && !isDown)
		{
			if (judgeDistance　== status.agrDistance.y)//ゆとりの範疇なら我慢
			{
				rb.velocity = new Vector2(rb.velocity.x, 0);
			}
			else if (judgeDistance < status.agrDistance.y)//距離はなす
			{
				if (judgeDistance >= (status.agrDistance.y - status.walkDistance.y))
				{
					if (Mathf.Abs(rb.velocity.y) >=  status.walkSpeed.y)
					{
						rb.AddForce(new Vector2(0,-status.addSpeed.y));
					}
					else if (Mathf.Abs(rb.velocity.y) <= status.walkSpeed.y)
					{
						rb.AddForce(new Vector2(0, status.addSpeed.y));
					}
				}
				else
				{
					if (Mathf.Abs(rb.velocity.y) >= status.combatSpeed.y)
					{
						rb.AddForce(new Vector2(0, -status.addSpeed.y));
					}
					else if (Mathf.Abs(rb.velocity.y) <= status.combatSpeed.y)
					{
						rb.AddForce(new Vector2(0, status.addSpeed.y));
					}
				}
			}
			else if (judgeDistance > status.agrDistance.y)//距離近づく
			{
				if (judgeDistance <= (status.agrDistance.y + status.walkDistance.y))
				{
					if (Mathf.Abs(rb.velocity.y) >= status.walkSpeed.y)
					{
						rb.AddForce(new Vector2(0, status.addSpeed.y));
					}
					else if (Mathf.Abs(rb.velocity.y) <= status.walkSpeed.y)
					{
						rb.AddForce(new Vector2(0, -status.addSpeed.y));
					}
				}
				else
				{
					if (Mathf.Abs(rb.velocity.y) >= status.combatSpeed.y)
					{
						rb.AddForce(new Vector2(0, status.addSpeed.y));
					}
					if (Mathf.Abs(rb.velocity.y) <= status.combatSpeed.y)
					{
						rb.AddForce(new Vector2(0, -status.addSpeed.y));
					}
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
		if (!isStop && !nowJump && !isDown)
		{
			if (isAvoid)
			{
				//sAni.Play(status.motionIndex["回避"]);
				avoidTime += Time.fixedDeltaTime;
				rb.AddForce(new Vector2(direction * status.avoidSpeed.x, status.avoidSpeed.y));
				Flip(direction);
				SetLayer(10);
				if (avoidTime >= status.avoidRes)
				{
					isAvoid = false;
					avoidTime = 0;
					rb.velocity = new Vector2(0, rb.velocity.y);
					SetLayer(initialLayer);
					////Debug.Log("初期化");
					AllStop(0.3f);
				}
			}
        }
	
	}

	/// <summary>
	/// 何度も当たり判定が検出されるのを防ぐためのもの
	/// </summary>
	public void DamageAvoid()
	{
		if (isDamage) {
			avoidTime += Time.fixedDeltaTime;
			SetLayer(10);
			if (avoidTime >= 0.1)
			{
				isDamage = false;
				avoidTime = 0;
				SetLayer(initialLayer);
			}
		} 
	}




	/// <summary>
	///	地上キャラ用のジャンプ。ジャンプ状態は自動解除。ジャンプキャンセルとセット
	/// </summary>	
	public void GroundJump(float jumpMove)
       {
		if (!isStop && !isAvoid && !isDown)
		{
			if (isJump)
			{
				//sAni.Play(status.motionIndex["ジャンプ"]);
				jumpTime += Time.fixedDeltaTime;
				rb.AddForce(new Vector2(jumpMove, status.jumpSpeed));
				GravitySet(0);
				nowJump = true;
			}
			if (jumpTime >= status.jumpRes)
			{
				//sAni.Play(status.motionIndex["落下"]);
				isJump = false;
				jumpTime = 0.0f;
				GravitySet(status.firstGravity);
			}
		}
        }

	/// <summary>
	/// ジャンプ終了着地判定。地上キャラのみ
	/// </summary>
	public void JumpCancel()
	{
		if (nowJump && !isJump && rb.IsTouching(status.filter))
		{
			nowJump = false;
		}
	}
    
	/// <summary>
	///	飛行キャラ用のジャンプ。ジャンプ状態は自動解除
	/// </summary>	
	public void AirJump(float jumpMove)
	{
		if (!isStop && !isAvoid && !isDown)
		{
			if (isJump)
			{
				jumpTime += Time.fixedDeltaTime;
				rb.AddForce(new Vector2(jumpMove, status.jumpSpeed));
			}
			if (jumpTime >= status.jumpRes)
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
		SetLayer(initialLayer);
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
		SetLayer(17);
		isStop = false;
		rb.velocity = Vector2.zero;
	}

	/// <summary>
	/// アニメイベントと攻撃後に呼び出す。スーパーアーマー状態を反転させる
	/// </summary>
	public void EnemySuperAmor()
    {
		isAttack = !isAttack;
    }
	/// <summary>
	/// アーマーをリセット
	/// </summary>
	public void ArmorReset()
    {
		status.nowArmor = status.Armor;
    }

	/// <summary>
	/// アーマー値に応じてイベントとばす
	/// </summary>
	public void ArmorControll()
    {
		if(status.nowArmor <= 0) 
		{
			isDown = true;
			isAttack = false;
		}
		else
		{
			isDown = false;
		}
	}



	public void EnemyArmorControll()
	{
		if (GManager.instance.pStatus.nowArmor <= 0 && status.isBlow)
		{
			GManager.instance.isDown = true;
		}
		else
		{
			GManager.instance.isDown = false;
		}
	}

	/// <summary>
	/// ノックバックする。
	/// </summary>
	public void NockBack()
    {
		if (isDown)
		{
			//sAni.Play(status.motionIndex["ノックバック"]);
			rb.AddForce(new Vector2(status.walkSpeed.x * -direction, 0));
	/*		if (CheckEnd(status.motionIndex["ノックバック"]))
            {
				ArmorReset();
				isDown = false;
			}*/
		}
	}

	/// <summary>
	/// ダウン状態のメソッド
	/// </summary>
	public void Down()
	{
		//Debug.Log("吹き飛ぶ");

			if (isDown && rb.IsTouching(status.filter))
			{
			//sAni.Play(status.motionIndex["ダウン"]);
			GravitySet(status.firstGravity);
				AllStop(1f);
				ArmorReset();
				isDown = false;
				//ダウンアニメ
			}
			else if (isDown && !rb.IsTouching(status.filter) && status.kind == EnemyStatus.KindofEnemy.Fly)
			{
			//sAni.Play(status.motionIndex["落下"]);
			SetLayer(17);
				GravitySet(15);
			}
	}


    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
		//Debug.Log($"{collision.tag}タグ");
		////Debug.Log("開始");
		if(collision.tag == status.AttackTag && (isHitable || lastHit != collision.gameObject))
        {
			WeponDamage();
			if (!isAggressive)
			{
				Serch.SetActive(false);
				Serch2.SetActive(false);
				isAggressive = true;
			}
			lastHit = collision.gameObject;
		}
		else if (collision.tag == status.PMagicTag && (isHitable || lastHit != collision.gameObject))
		{

			PlayerMagicDamage();
			if (!isAggressive)
			{
				Serch.SetActive(false);
				Serch2.SetActive(false);
				isAggressive = true;
			}
			lastHit = collision.gameObject;
		}
		else if (collision.tag == status.SMagicTag && (isHitable || lastHit != collision.gameObject)) 
		{

			SisterMagicDamage();
			if (!isAggressive)
			{
				Serch.SetActive(false);
				Serch2.SetActive(false);
				isAggressive = true;
			}
			lastHit = collision.gameObject;
		}

		if(isAggressive && collision.tag == status.JumpTag)
        {
			jumpTrigger = true;
        }

	}
	protected virtual void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.tag == status.AttackTag && (isHitable || lastHit != collision.gameObject))
		{

			WeponDamage();
			isHit = true;
			isHitable = false;
			lastHit = collision.gameObject;
		}
		else if (collision.tag == status.PMagicTag && (isHitable || lastHit != collision.gameObject))
		{

			PlayerMagicDamage();
			isHit = true;
			isHitable = false;
			lastHit = collision.gameObject;
		}
		else if (collision.tag == status.SMagicTag && (isHitable || lastHit != collision.gameObject))
		{

			SisterMagicDamage();
			isHit = true;
			isHitable = false;
			lastHit = collision.gameObject;
		}


		if (status.kind != EnemyStatus.KindofEnemy.Fly && collision.tag == status.JumpTag && isAggressive)
		{
			jumpTrigger = true;
		}

	}

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {

		if (collision.tag == status.PMagicTag)
		{
			//ここで最終弾だけ吹き飛ばすようにする？
			PlayerMagicDamage();
			isHit = false;
		}
		else if (collision.tag == status.SMagicTag)
		{

			SisterMagicDamage();
			isHit = false;
		}

	}

	public void HitCheck()
    {
        if (isHit && !isHitable)
        {
			damageDelay += Time.fixedDeltaTime;
			if(damageDelay >= 0.3)
            {
				isHitable = true;
            }

        }
        else
        {
			damageDelay = 0.0f;
			isHitable = true;
        }

	}

	/*	/// <summary>
		/// プレイヤーの魔法による攻撃
		/// </summary>
		public void MagicBlow(Collider2D c)
		{

			rb.AddForce(new Vector2(GManager.instance.pStatus.equipWeapon.blowPower.x * -direction, GManager.instance.pStatus.equipWeapon.blowPower.y));
		}*/

	public void HPReset()
    {
		status.hp = status.maxHp;
    }
	public void Die()
    {
        if (status.hp <= 0)
        {
			Destroy(this.gameObject);
        }
    }

	/// <summary>
	/// 攻撃前の初期化
	/// </summary>
	public void AttackPrepare()
    {
		isAttack = true;
		status.coolTime = status.atValue[attackNumber].coolList;
		status.isBlow = status.atValue[attackNumber].isBlow;
		status.mValue = status.atValue[attackNumber].mvalue;
		status.aditionalArmor = status.atValue[attackNumber].addArmor;
		status.isLight = status.atValue[attackNumber].isLight;
		//status.hitLimmit = status.atValue[attackNumber].hitLimmit;
		status.blowVector = status.atValue[attackNumber].blowPower;
		status.Shock = status.atValue[attackNumber].shock;
		status.atType = status.atValue[attackNumber].type;
		status.isCombo = status.atValue[attackNumber].isCombo;
	}


	/// <summary>
	/// 攻撃
	/// </summary>
	public void Attack()
	{
		if (isAtEnable)
		{
			AttackPrepare();
			//sAni.Play(status.attackName[attackNumber]);
			//isAttack = true;
			isAtEnable = false;
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
	/// ランダムに移動
	/// </summary>
	public void Feint()
    {
		randomTime += Time.fixedDeltaTime;

		if (randomTime >= 2.5) {
		    moveDirectionX = RandomValue(0, 100) >= 50 ? 1 : -1;
			moveDirectionY = RandomValue(0, 100) >= 50 ? -1 : 1;
			randomTime = 0.0f;
		}
		rb.AddForce(new Vector2(status.combatSpeed.x * moveDirectionX, status.combatSpeed.y * moveDirectionY));

	}

	/// <summary>
	/// ガードする
	/// </summary>
	public void GuardAct()
    {
        if (isGuard)
        {
			Guard.SetActive(true) ;
        }
		if (!isGuard)
		{
			Guard.SetActive(false);
		}
	}
	/// <summary>
	/// ガードブレイク
	/// </summary>
	public void GuardBreak()
    {
		//Debug.Log("敗れた");
		isGuard = false;
	}

	/// <summary>
	/// クールタイムの間次の攻撃を待つ
	/// </summary>
	public void WaitAttack()
    {
		if (isAttack)
		{
			if (!CheckEnd(status.attackName[attackNumber]))
			{
				isAtEnable = true;
				isAttack = false;
			}
		}

		else if (!isAttack)
		{
			if (!isAtEnable && !status.isCombo)
			{
				stayTime += Time.fixedDeltaTime;
				if (stayTime >= status.coolTime)
				{
					isAtEnable = false;
					stayTime = 0.0f;
				}
			}
			if (isAtEnable && status.isCombo)
			{
				isAtEnable = false;
				attackNumber++;
				//AttackPrepare();
				Attack();
			}
		}
    }

	bool CheckEnd(string _currentStateName)
	{
		return sAni.IsPlaying(_currentStateName);
	}
}


