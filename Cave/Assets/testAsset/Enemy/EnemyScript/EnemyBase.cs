using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Guirao.UltimateTextDamage;

public class EnemyBase : MonoBehaviour
{
	//「//sAni.」を「sAni.」で置き換える。アニメ作ったら

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

	[HideInInspector] public bool cameraRendered;
	[HideInInspector] public float hp;

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
	protected bool isMovable;
	protected bool isAnimeStart;//アニメの開始に使う
	protected bool isFalter;//よろめき
	protected bool isBounce;//攻撃をはじかれた
	/// <summary>
	/// 攻撃の値
	/// </summary>
	[HideInInspector]public EnemyValue atV;
	// === キャッシュ ==========================================
	//public Animator animator;
	public Animator sAni;
	
	
	public Rigidbody2D rb;//継承先で使うものはprotected

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
	protected bool isDown;
	protected float randomTime = 2.5f;//ランダム移動測定のため
	protected bool isGuard;
	protected bool guardBreak;//ガードブレイク
	[HideInInspector]public bool guardHit;
	protected float stateJudge;//ステート判断間隔
	protected Rigidbody2D dRb;//ドッグパイルのリジッドボディ
							  //	protected float nowDirection;//今の敵の方向

	[HideInInspector]public float damageDelay;
	[HideInInspector]public bool isHit;//ヒット中
	[HideInInspector]public bool isHitable;
	[HideInInspector] public GameObject lastHit;
	protected float jumpWait;
	protected bool isWakeUp;//ダウン終了後起き上がるフラグ
	protected bool blowDown;
	protected Vector2 move;
	protected Vector2 blowM;
	protected float blowTime;
	protected float recoverTime;//アーマー回復までにかかる時間
	protected float lastArmor;
	protected float nowArmor;
	[SerializeField]protected UltimateTextDamageManager um;

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
		isGround = rb.IsTouching(EnemyManager.instance.filter);


		if (!cameraRendered)
        {
			if (!status.unBaind)
			{
				Debug.Log("停止");
				//縛られないなら先に
				return;
			}
        }
		if (status.kind != EnemyStatus.KindofEnemy.Fly)
		{
			if (isDown || isDown || !isGround || isGuard || isAvoid || isAttack || isJump || guardBreak)
			{
				isMovable = false;
			}
            else
            {
				isMovable = true;
            }
		}
        else
        {
			if (isDown || isDown || isGuard || isAvoid || isAttack || isJump || guardBreak)
			{
				isMovable = false;
			}
			else
			{
				isMovable = true;
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

		targetPosition = GManager.instance.Player.transform.position;
		Vector2 myPosition = this.transform.position;
		distance = targetPosition - myPosition;
		direction = distance.x >= 0 ? 1 : -1;//距離が正、または0の時1。そうでないとき-1。方向
		directionY = distance.y >= 0 ? 1 : -1;//弓構えるときのアニメの判定	にも使えそう

		////////Debug.log($"ジャンプ中{nowJump}");
		////////Debug.log($"回避中{isAvoid}");
		if (isAggressive)
        {
			posiReset = true;
			Serch.SetActive(false);
			Serch2.SetActive(false);

			targetPosition = GManager.instance.Player.transform.position;

			EscapeCheck();

			//TriggerJump();
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
	//	SetVelocity();
		HitCheck();
		WaitAttack();
		ArmorRecover();
		DamageAvoid();
		EnemyFall();
		GuardBreak();
		Parry();
		NockBack();
		Blow();
		Down();
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
		//////Debug.log($"ガード中か否か{guardHit}");
		if (!guardHit /*&& GManager.instance.pStatus.equipWeapon.hitLimmit > 0 */&& !isDamage)
		{
			isDamage = true;
		//	GManager.instance.pStatus.equipWeapon.hitLimmit--;

			//////Debug.log("終了");
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

			////Debug.log($"{ damage * GManager.instance.pStatus.attackBuff}ダメージ");
			//////Debug.log($"{nowArmor}a-mor");
			damage = Mathf.Floor(damage * GManager.instance.pStatus.attackBuff);
			hp -= damage;//HP引いてる
					//				Debug.Log($"ダメージ{damage}とトランスフォーム{this.transform.position}");

			um.AddStack(damage, this.transform);
			if (!isAttack)
			{
				nowArmor -= GManager.instance.pStatus.equipWeapon.shock;

			}
			else
			{
				if (atV.aditionalArmor > 0)
				{
					nowArmor -= (GManager.instance.pStatus.equipWeapon.shock - atV.aditionalArmor) < 0 ? 0 : (GManager.instance.pStatus.equipWeapon.shock - atV.aditionalArmor);
					atV.aditionalArmor -= GManager.instance.pStatus.equipWeapon.shock;
				}
			}
			ArmorControll();
			if (isDown && GManager.instance.pStatus.equipWeapon.isBlow)
			{
			//	Debug.Log("aaaa");
				blowM.Set(GManager.instance.pStatus.equipWeapon.blowPower.x * -direction, GManager.instance.pStatus.equipWeapon.blowPower.y);

				//rb.AddForce(move);
				blowDown = true;
				//Invoke("Down", 1f);
			}
            else
            {
				isFalter = true;
			}
		}
	}

	public void WeponGuard()
	{
		if (/*&& GManager.instance.pStatus.equipWeapon.hitLimmit > 0 */!isDamage)
		{
			isDamage = true;
			//GManager.instance.pStatus.equipWeapon.hitLimmit--;

			//////Debug.log("終了");
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

			//////Debug.log($"{ damage * GManager.instance.pStatus.attackBuff}ダメージ");
			damage = Mathf.Floor(damage * GManager.instance.pStatus.attackBuff);
			hp -= damage;//HP引いてる
			um.AddStack(damage, this.transform);
			nowArmor -= (GManager.instance.pStatus.equipWeapon.shock * 2) * ((100 - status.guardPower) / 100);

			if(GManager.instance.pStatus.equipWeapon.isLight && status.guardPower >= 50)
            {
				GManager.instance.isBounce = true;//受け値50以上で弾く


			}
			if (nowArmor <= 0 && (GManager.instance.pStatus.equipWeapon.isBlow))
			{
				blowM.Set(GManager.instance.pStatus.equipWeapon.blowPower.x * -direction, GManager.instance.pStatus.equipWeapon.blowPower.y);
				blowDown = true;
			}
            else
            {
				guardBreak = true;
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
			damage = Mathf.Floor(damage * GManager.instance.pStatus.attackBuff);
			hp -= damage;//HP引いてる
			um.AddStack(damage, this.transform);
			if (!isAttack)
			{
				nowArmor -= GManager.instance.pStatus.useMagic.shock;
			}
			else
			{
				if (atV.aditionalArmor > 0)
				{
					nowArmor -= (GManager.instance.pStatus.equipWeapon.shock - atV.aditionalArmor) < 0 ? 0 : (GManager.instance.pStatus.equipWeapon.shock - atV.aditionalArmor);
					atV.aditionalArmor -= GManager.instance.pStatus.equipWeapon.shock;
				}
			}
			ArmorControll();
			if (isDown && GManager.instance.pStatus.useMagic.isBlow)
			{
				blowM.Set(GManager.instance.pStatus.useMagic.blowPower.x * -direction, GManager.instance.pStatus.useMagic.blowPower.y);
				blowDown = true;
				//Invoke("Down",1f);
			}
			else
			{
				isFalter = true;
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
			damage = Mathf.Floor(damage * GManager.instance.pStatus.attackBuff);
			hp -= damage;//HP引いてる
			um.AddStack(damage, this.transform);
			if (GManager.instance.pStatus.equipWeapon.isBlow)
			{
				nowArmor -= ((GManager.instance.pStatus.useMagic.shock * 2) * status.guardPower / 100) * 1.2f;
			}
			else
			{
				nowArmor -= (GManager.instance.pStatus.useMagic.shock * 2) * ((100 - status.guardPower) / 100);
			}

			if (nowArmor <= 0 && GManager.instance.pStatus.useMagic.isBlow)
			{
				blowM.Set(GManager.instance.pStatus.useMagic.blowPower.x * -direction, GManager.instance.pStatus.useMagic.blowPower.y);
				//rb.AddForce(move, ForceMode2D.Impulse);
				blowDown = true;
			}
			else
			{
				guardBreak = true;
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
			damage = Mathf.Floor(damage * SManager.instance.sisStatus.attackBuff);
			hp -= damage;
			//Mathf.Floor(hp);
			um.AddStack(damage, this.transform);
			if (!isAttack)
			{
				nowArmor -= SManager.instance.sisStatus.useMagic.shock;
			}
			else
			{
				if (atV.aditionalArmor > 0)
				{
					nowArmor -= (GManager.instance.pStatus.equipWeapon.shock - atV.aditionalArmor) < 0 ? 0 : (GManager.instance.pStatus.equipWeapon.shock - atV.aditionalArmor);
					atV.aditionalArmor -= GManager.instance.pStatus.equipWeapon.shock;
				}
			}
			ArmorControll();
			if (isDown && SManager.instance.sisStatus.useMagic.isBlow)
			{
				blowM.Set(SManager.instance.sisStatus.useMagic.blowPower.x * direction, SManager.instance.sisStatus.useMagic.blowPower.y);
			//	rb.AddForce(move, ForceMode2D.Impulse);
				blowDown = true;
			}
			else
			{
				isFalter = true;
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
			damage = Mathf.Floor(damage * SManager.instance.sisStatus.attackBuff);
			hp -= damage;
			//Mathf.Floor(hp);
			um.AddStack(damage, this.transform);


			if (SManager.instance.sisStatus.useMagic.isBlow)
			{
				nowArmor -= ((SManager.instance.sisStatus.useMagic.shock * 2) * status.guardPower / 100) * 1.2f;
			}
			else
			{
				nowArmor -= (SManager.instance.sisStatus.useMagic.shock * 2) * ((100 - status.guardPower) / 100);
			}

			if (nowArmor <= 0 && SManager.instance.sisStatus.useMagic.isBlow)
			{
				blowM.Set(SManager.instance.sisStatus.useMagic.blowPower.x * direction, SManager.instance.sisStatus.useMagic.blowPower.y);
				//rb.AddForce(move, ForceMode2D.Impulse);
				blowDown = true;
			}
			else
			{
				guardBreak = true;
			}
		}
	}

	public void PlayerParry()
    {
		if (!GManager.instance.isDamage && !GManager.instance.guardHit)
		{
			if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {
				GManager.instance.pStatus.stamina += GManager.instance.pStatus.equipShield.parryRecover;
            }
            else
            {
				GManager.instance.pStatus.stamina += GManager.instance.pStatus.equipWeapon.parryRecover;
			}
			GManager.instance.isDamage = true;
			GManager.instance.isGuard = false;
			GManager.instance.isParry = false;
			GManager.instance.parrySuccess = true;
			atV.isCombo = false;
			isAttack = false;
			isBounce = true;
			isDown = true;
		}
	}
	public void AttackDamage()
    {

		if (!GManager.instance.isDamage && !GManager.instance.guardHit)
		{
			//GManager.instance.isDamage = true;
			//status.hitLimmit--;
								 //mValueはモーション値
			float damage = 0;//バフデバフ処理用にdamageとして保持する
			if (status.phyAtk > 0)
			{
				//斬撃刺突打撃を管理
				if (atV.type == EnemyStatus.AttackType.Slash)
				{ damage += (Mathf.Pow(status.phyAtk, 2) * atV.mValue) / (status.phyAtk + GManager.instance.pStatus.Def); }
				else if (atV.type == EnemyStatus.AttackType.Stab)
				{ damage += (Mathf.Pow(status.phyAtk, 2) * atV.mValue) / (status.phyAtk + GManager.instance.pStatus.pierDef); }
				else if (atV.type == EnemyStatus.AttackType.Strike)
				{ damage += (Mathf.Pow(status.phyAtk, 2) * atV.mValue) / (status.phyAtk + GManager.instance.pStatus.strDef); }
			}
			//神聖
			if (status.holyAtk > 0)
			{
				damage += (Mathf.Pow(status.holyAtk, 2) * atV.mValue) / (status.holyAtk + GManager.instance.pStatus.holyDef);
			}
			//闇
			if (status.darkAtk > 0)
			{
				damage += (Mathf.Pow(status.darkAtk, 2) * atV.mValue) / (status.darkAtk + GManager.instance.pStatus.darkDef);
			}
			//炎
			if (status.fireAtk > 0)
			{
				damage += (Mathf.Pow(status.fireAtk, 2) * atV.mValue) / (status.fireAtk + GManager.instance.pStatus.fireDef);
			}
			//雷
			if (status.thunderAtk > 0)
			{
				damage += (Mathf.Pow(status.thunderAtk, 2) * atV.mValue) / (status.thunderAtk + GManager.instance.pStatus.thunderDef);
			}
			if (!GManager.instance.isAttack)
			{
				GManager.instance.nowArmor -= atV.shock;
			}
			else
			{
				GManager.instance.nowArmor -= (atV.shock - GManager.instance.pStatus.equipWeapon.atAromor) < 0 ? 0 : (atV.shock - GManager.instance.pStatus.equipWeapon.atAromor);
			}
			damage = Mathf.Floor(damage * status.attackBuff);
			//GManager.instance.HpReduce;
			GManager.instance.pStatus.hp = GManager.instance.pStatus.hp - damage;
			//GManager.instance.pStatus.hp = 10;
			Debug.Log($"ダメージ{damage}");
			Debug.Log($"残り{GManager.instance.pStatus.hp}");
			GManager.instance.isDamage = true;
		}
		if (GManager.instance.nowArmor <= 0)
		{
			GManager.instance.isDown = true;
			if (atV.isBlow == true)
			{
				GManager.instance.blowDown = true;
				if (direction > 0)
				{
					GManager.instance.blowVector = atV.blowPower;

				}
				if (direction < 0)
				{
					GManager.instance.blowVector.Set(-atV.blowPower.x, atV.blowPower.y);

				}
			}
            else
            {
				GManager.instance.isFalter = true;
            }
		}
}

	public void PlayerGuard()
    {
		if (!GManager.instance.isDamage && !GManager.instance.guardHit)
		{
			//status.hitLimmit--;
			//mValueはモーション値
			float damage = 0;//バフデバフ処理用にdamageとして保持する
			if (status.phyAtk > 0)
			{
				//斬撃刺突打撃を管理
				if (atV.type == EnemyStatus.AttackType.Slash)
				{ damage += (Mathf.Pow(status.phyAtk, 2) * atV.mValue) / (status.phyAtk + GManager.instance.pStatus.Def); }
				else if (atV.type == EnemyStatus.AttackType.Stab)
				{ damage += (Mathf.Pow(status.phyAtk, 2) * atV.mValue) / (status.phyAtk + GManager.instance.pStatus.pierDef); }
				else if (atV.type == EnemyStatus.AttackType.Strike)
				{ damage += (Mathf.Pow(status.phyAtk, 2) * atV.mValue) / (status.phyAtk + GManager.instance.pStatus.strDef); }

				damage *= (100 - GManager.instance.pStatus.phyCut)/100;
			}
			//神聖
			if (status.holyAtk > 0)
			{
				damage += ((Mathf.Pow(status.holyAtk, 2) * atV.mValue) / (status.holyAtk + GManager.instance.pStatus.holyDef)) * (100 - GManager.instance.pStatus.holyCut)/100;
			}
			//闇
			if (status.darkAtk > 0)
			{
				damage += ((Mathf.Pow(status.darkAtk, 2) * atV.mValue) / (status.darkAtk + GManager.instance.pStatus.darkDef)) * (100 - GManager.instance.pStatus.darkCut)/100;
			}
				//炎
				if (status.fireAtk > 0)
			{
				damage += ((Mathf.Pow(status.fireAtk, 2) * atV.mValue) / (status.fireAtk + GManager.instance.pStatus.fireDef)) * (100 - GManager.instance.pStatus.fireCut)/100;
			}
			//雷
			if (status.thunderAtk > 0)
			{
				damage += ((Mathf.Pow(status.thunderAtk, 2) * atV.mValue) / (status.thunderAtk + GManager.instance.pStatus.thunderDef)) * (100 - GManager.instance.pStatus.thunderCut)/100;
			}
			if (status.isLight && GManager.instance.pStatus.guardPower >= 50)
			{//受け値50以上の盾は軽い攻撃をはじく
				//sAni.Play(status.motionIndex["弾かれ"]);
			}
			    GManager.instance.pStatus.stamina -= (atV.shock * 2) * (1 - (GManager.instance.pStatus.guardPower / 100));


			damage = Mathf.Floor(damage * status.attackBuff);
			GManager.instance.pStatus.hp -= damage * status.attackBuff;
			//GManager.instance.isDamage = true;
		}
		if (GManager.instance.pStatus.stamina <= 0)
		{
			if (atV.isBlow == true)
			{
				GManager.instance.isDown = true;
				GManager.instance.blowDown = true;
				GManager.instance.isGuard = false;
				GManager.instance.isDamage = true;
				if (direction > 0)
				{
					GManager.instance.blowVector = atV.blowPower;

				}
				if (direction < 0)
				{
					GManager.instance.blowVector.Set(-atV.blowPower.x, atV.blowPower.y);

				}
			}

			else
			{
				GManager.instance.isGBreak = true;
				GManager.instance.isGuard = false;
			}
		}
        else
        {
			GManager.instance.isDamage = true;
			GManager.instance.guardHit = true;
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
					if (((transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)
						&& (transform.position.y >= basePosition.y - 5 && transform.position.y <= basePosition.y + 5) &&
						(status.kind == EnemyStatus.KindofEnemy.Fly)) || ((status.kind != EnemyStatus.KindofEnemy.Fly) &&
							(transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)))
					{
						sAni.Play("Stand");
						//スタートが地面だから大丈夫。落ちてくる系のやつにはDogpileを与えてやれ
						rb.velocity = Vector2.zero;
						posiReset = false;
						isRight = true;
						transform.localScale = baseDirection;
					}
                    else
                    {
						//sAni.Play(status.motionIndex[status.dogPile == null ? "歩き":"走り"]);
						move.Set(status.addSpeed.x * ((status.dogPile == null ? status.patrolSpeed.x : status.combatSpeed.x) - rb.velocity.x),(status.addSpeed.y * (status.dogPile == null ? status.patrolSpeed.y : status.combatSpeed.y)) - rb.velocity.y);
						rb.AddForce(move);
						//rb.AddForce(move(0, ;
						sAni.Play($"{(status.dogPile == null ? "Move" : "Dash")}");
					}
				}
				else if (transform.position.y >= basePosition.y)
				{
					Flip(1);
					if (((transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)
						&& (transform.position.y >= basePosition.y - 5 && transform.position.y <= basePosition.y + 5) &&
						(status.kind == EnemyStatus.KindofEnemy.Fly)) || ((status.kind != EnemyStatus.KindofEnemy.Fly) &&
							(transform.position.x >= basePosition.x - 5 && transform.position.x <= basePosition.x + 5)))
					{
						sAni.Play("Stand");
						rb.velocity = Vector2.zero;
						posiReset = false;
						isRight = true;
						transform.localScale = baseDirection;
					}
                    else
                    {
						move.Set(status.addSpeed.x * ((status.dogPile == null ? status.patrolSpeed.x : status.combatSpeed.x) - rb.velocity.x), (status.addSpeed.y * (status.dogPile == null ? -status.patrolSpeed.y : -status.combatSpeed.y)) - rb.velocity.y);
						rb.AddForce(move);
						//rb.AddForce(move(0);
						sAni.Play($"{(status.dogPile == null ? "Move" : "Dash")}");
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
						sAni.Play("Stand");
						rb.velocity = Vector2.zero;
						posiReset = false;
						isRight = true;
						transform.localScale = baseDirection;
					}
                    else
                    {
						sAni.Play($"{(status.dogPile == null ? "Move" : "Dash")}");
						move.Set(status.addSpeed.x * ((status.dogPile == null ? -status.patrolSpeed.x : -status.combatSpeed.x) - rb.velocity.x),(status.addSpeed.y * (status.dogPile == null ? status.patrolSpeed.y : status.combatSpeed.y)) - rb.velocity.y);
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
						sAni.Play("Stand");
						rb.velocity = Vector2.zero;
						posiReset = false;
						isRight = true;
						transform.localScale = baseDirection;
					}
                    else
                    {
						sAni.Play($"{(status.dogPile == null ? "Move" : "Dash")}");
						move.Set(status.addSpeed.x * ((status.dogPile == null ? -status.patrolSpeed.x : -status.combatSpeed.x) - rb.velocity.x),(status.addSpeed.y * (status.dogPile == null ? -status.patrolSpeed.y : -status.combatSpeed.y)) - rb.velocity.y);
						rb.AddForce(move);
						//rb.AddForce(move(0, );
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
				sAni.Play("Move");
				Flip(1);
				move.Set(status.addSpeed.x * (status.patrolSpeed.x - rb.velocity.x), 0);
				rb.AddForce(move);

			}
			else if (transform.position.x >= startPosition.x - status.waitDistance.x && !isRight)
			{
				sAni.Play("Move");
				Flip(-1);
				move.Set(status.addSpeed.x * (-status.patrolSpeed.x - rb.velocity.x), 0);
				rb.AddForce(move);
			}
			else
			{
				sAni.Play("Stand");
					////////Debug.log("ああああ");
					waitTime += Time.fixedDeltaTime;
                    move.Set(0, rb.velocity.y);
				    rb.velocity = move;
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
	/// 待機中空を飛ぶ
	/// 待機と選択で積む
	/// </summary>
	public void PatrolFly()
    {
		if (!isStop && !posiReset && !nowJump && !isDown)
		{

			if (transform.position.y <= startPosition.y + status.waitDistance.y && isUp)
			{
				move.Set(0, status.addSpeed.y * (status.patrolSpeed.y - rb.velocity.y));
				rb.AddForce(move);
				sAni.Play("Move");
			}
			else if(transform.position.y >= startPosition.y - status.waitDistance.y && !isUp)
			{
				move.Set(0, status.addSpeed.y * (-status.patrolSpeed.y - rb.velocity.y));
				rb.AddForce(move);
				sAni.Play("Move");
			}
            else
            {
				isUp = !isUp;
				move.Set(rb.velocity.x, 0);
				rb.velocity = move;
				sAni.Play("Move");
			}
		}
	}

	/// <summary>
	/// 待機中停止してきょろきょろ振り向きだけ。こいつはパトロールしないからwaitTimeは使いまわしでいい
	/// パトロールと選択で積む
	/// なにかに止まってるハエとかも表現できる
	/// </summary>
	public void Wait()
    {
		if (!posiReset)
		{
			if (dRb == null)
			{
				sAni.Play("Stand");
				waitTime += Time.fixedDeltaTime;
				move.Set(0, rb.velocity.y);
				rb.velocity = move;
				if (waitTime >= status.waitRes)
				{
					Flip(-transform.localScale.x);//反転させまーす
					waitTime = 0.0f;
				}
			}
			if(dRb != null)
            {
				sAni.Play("Move");
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

		//if()
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

		if (!isStop && !nowJump && !isAvoid && !isDown && isGround)
		{

            if (isGuard)
            {
                if (status.ground == EnemyStatus.MoveState.stay)
                {
					sAni.Play("Guard");
					Flip(direction);
					move.Set(0, rb.velocity.y);

					rb.velocity = move;
					isReach = true;
				}
				else if (status.ground == EnemyStatus.MoveState.leaveWalk || status.ground == EnemyStatus.MoveState.leaveDash)//遠ざかる
				{
					//近距離の場合歩き範囲をダッシュで離れるのより大きく
					//歩き距離なら敵を見たまま撃つ
					//動かない弓兵とかは移動速度ゼロに
					Flip(direction);
					isReach = status.ground == EnemyStatus.MoveState.leaveWalk;
					sAni.Play(status.motionIndex["BackGuard"]);
					move.Set(status.addSpeed.x * (-direction * status.patrolSpeed.x - rb.velocity.x), 0);
					rb.AddForce(move);
				}
                else
                {
					Flip(direction);
					isReach = (Mathf.Abs(distance.x) - status.agrDistance.x) <= status.walkDistance.x ? true : false;
					move.Set(status.addSpeed.x * (direction * status.patrolSpeed.x - rb.velocity.x), 0);
					rb.AddForce(move);
					sAni.Play("GuardMove");
				}
			}
			else if (status.ground == EnemyStatus.MoveState.stay)
			{
				sAni.Play("Stand");
				Flip(direction);
				move.Set(0, rb.velocity.y);

			//Debug.Log($"ねずみ{blowM.x}");
				rb.velocity = move;
				isReach = true;
			}
			else if (status.ground == EnemyStatus.MoveState.accessWalk)
			{
				Flip(direction);
				isReach = (Mathf.Abs(distance.x) - status.agrDistance.x) <= status.walkDistance.x ? true : false;
				move.Set(status.addSpeed.x * (direction * status.patrolSpeed.x - rb.velocity.x), 0);
				rb.AddForce(move);
				sAni.Play("Move");
			}
			else if (status.ground == EnemyStatus.MoveState.accessDash) 
			{
				Flip(direction);
				isReach = false;
				sAni.Play("Dash");
				move.Set(status.addSpeed.x * (direction * status.combatSpeed.x - rb.velocity.x),0);
				rb.AddForce(move);
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
				sAni.Play("BackMove");
				move.Set(status.addSpeed.x * (-direction * status.patrolSpeed.x - rb.velocity.x), 0);
				rb.AddForce(move);
			}
				else if(status.ground == EnemyStatus.MoveState.leaveDash)
				{
					sAni.Play("Dash");
					Flip(-direction);
					isReach = false;
				move.Set(status.addSpeed.x * (-direction * status.combatSpeed.x - rb.velocity.x),0);
				rb.AddForce(move);
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
			sAni.Play("Move");
			if (judgeDistance　== status.agrDistance.y)//ゆとりの範疇なら我慢
			{
				move.Set(rb.velocity.x,0);
				rb.velocity = move;
			}
			else if (judgeDistance < status.agrDistance.y)//距離はなす
			{
				if (judgeDistance >= (status.agrDistance.y - status.walkDistance.y))
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
			else if (judgeDistance > status.agrDistance.y)//距離近づく
			{
				if (judgeDistance <= (status.agrDistance.y + status.walkDistance.y))
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
				//アニメを時間に合わせろ
				sAni.Play("Avoid");
				avoidTime += Time.fixedDeltaTime;
				move.Set(direction * status.avoidSpeed.x, status.avoidSpeed.y);
				rb.AddForce(move);
				Flip(direction);
				SetLayer(10);
				if (avoidTime >= status.avoidRes)
				{
					isAvoid = false;
					avoidTime = 0;
					move.Set(0, rb.velocity.y);
					rb.velocity = move;
					SetLayer(initialLayer);
					////////Debug.log("初期化");
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
		if (isDamage) 
		{
			avoidTime += Time.fixedDeltaTime;
			SetLayer(10);
			if (avoidTime >= 0.15 && !GManager.instance.fallAttack)
			{
				isDamage = false;
				avoidTime = 0;
				SetLayer(initialLayer);
			}
		}
        else if(isDown)
        {
            if (blowDown && !isWakeUp)
            {
				SetLayer(10);
			}
            else
            {
				SetLayer(initialLayer);
			}
        }
	}
	//status.kind == EnemyStatus.KindofEnemy.Fly

	public void EnemyFall()
	{
		if (status.kind == EnemyStatus.KindofEnemy.Fly)
		{
			if (!isGround && blowDown)
			{
				GravitySet(status.firstGravity);
			}
			else
			{
				GravitySet(0);
			}
		}
        else
        {
			if (!isGround && !isAttack && !isJump && !isDown)
			{
					sAni.Play("Fall");

			}
			else if (isJump)
            {
				sAni.Play("Jump");
			}
			
		}
	}

	/// <summary>
	///	地上キャラ用のジャンプ。ジャンプ状態は自動解除。ジャンプキャンセルとセット
	/// </summary>	
	public void GroundJump(float jumpMove, float jumpSpeed)
       {

		if (!isStop && !isAvoid && !isDown)
		{
			if (isJump)
			{
				jumpTime += Time.fixedDeltaTime;
				move.Set(jumpMove * EnemyManager.instance.jMove.Evaluate(jumpTime), jumpSpeed * EnemyManager.instance.jPower.Evaluate(jumpTime));
				rb.AddForce(move);
				sAni.Play("Jump");
				if (isGround && jumpTime > 0.1)//ここJumpRessとかにしてもいいかもね
				{//
					rb.velocity = Vector2.zero;
					isJump = false;
					jumpTime = 0.0f;
					jumpTrigger = false;
					//isVertical = false;
				}

			}
		}
        }

	public void TriggerJump()
    {
		if (jumpWait <= 3.0f)
		{
			jumpWait += Time.fixedDeltaTime;

		}
		//////Debug.log($"ジャンプトリガー{jumpTrigger}");
		if (jumpTrigger && jumpWait >= 2.0f && isGround)
		{
			
			isJump = true;
			jumpWait = 0;
			jumpTrigger = false;
		}
	}



/*	/// <summary>
	/// ジャンプ終了着地判定。地上キャラのみ
	/// </summary>
	public void JumpCancel()
	{
		if (nowJump && !isJump && rb.IsTouching(status.filter))
		{
			nowJump = false;
		}
	}*/
    
	/// <summary>
	///	飛行キャラ用のジャンプ。ジャンプ状態は自動解除
	/// </summary>	
	public void AirJump(float jumpMove,float jumpSpeed)
	{
		if (!isStop && !isAvoid && !isDown)
		{
			if (isJump)
			{
				sAni.Play("Jump");
				jumpTime += Time.fixedDeltaTime;
				move.Set(jumpMove * EnemyManager.instance.jMove.Evaluate(jumpTime), jumpSpeed * EnemyManager.instance.jPower.Evaluate(jumpTime));
				rb.AddForce(move);
				if (jumpTime > 1)
				{//

					isJump = false;
					jumpTime = 0.0f;
					jumpTrigger = false;
					//isVertical = false;
				}

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
		return UnityEngine.Random.Range(X, Y + 1);

	}

	/// <summary>
	/// stopResを数えた後にisStop解除のメソッドを呼ぶ。isStop中はすべて止まる。硬直やダウンに
	/// </summary>
	/// <param name="stopRes"></param>
    public void AllStop(float stopRes)
    {
		//stopTime += Time.fixedDeltaTime;
		//SetLayer(initialLayer);
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
		//SetLayer(17);
		isStop = false;
		rb.velocity = Vector2.zero;
        if (blowDown)
        {
			isWakeUp = true;
        }
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
		nowArmor = status.Armor;
    }

	public void ArmorRecover()
	{
		if(!isDown && nowArmor == lastArmor)
        {
			recoverTime += Time.fixedDeltaTime;
			if(recoverTime >= 15)
            {
				ArmorReset();
				recoverTime = 0.0f;
				lastArmor = nowArmor; 
			}
            else if(nowArmor < status.Armor - 5 && recoverTime >= 3)
            {
				recoverTime = 0.0f;
				nowArmor += 3;
				lastArmor = nowArmor;
			}
        }
        else
        {
			recoverTime = 0.0f;
			lastArmor = nowArmor;
		}

	}


	/// <summary>
	/// アーマー値に応じてイベントとばす
	/// </summary>
	public void ArmorControll()
    {
		if(nowArmor <= 0) 
		{
			isDown = true;
			isAttack = false;
			isAtEnable = true;
		}
		else
		{
			isDown = false;
		}
	}



/*	public void EnemyArmorControll()
	{
		if (GManager.instance.nowArmor <= 0 && status.isBlow)
		{
			GManager.instance.isDown = true;
			GManager.instance.blowDown = true;
		}
		else
		{
			GManager.instance.isDown = false;
		}
	}*/

	/// <summary>
	/// ノックバックする。
	/// </summary>
	public void NockBack()
    {
		if (blowDown && isFalter)
		{
			isFalter = false;
			isAnimeStart = false;
		}
		if (isDown && !blowDown && isFalter)
		{
			if (!isAnimeStart)
				{
				//isFalter = true;
				isAnimeStart = true;
				sAni.Play("Falter");
			    }
            else if (!CheckEnd("Falter"))
            {
				isDown = false;
				isAnimeStart = false;
				isFalter = false;
				
            }


		}
	}
	/// <summary>
	/// 攻撃をはじかれノックバックする。
	/// </summary>
	public void Parry()
	{
		if (isBounce && (blowDown || isFalter))
		{
			isBounce = false;
			isAnimeStart = false;

		}
		if (isBounce && !blowDown)
		{
			if (!isAnimeStart)
			{
				isAttack = false;
				isDown = true;
				//isFalter = true;
				isAnimeStart = true;
				sAni.Play("Bounce");
				//Debug.Log("弾かれ");
			}
			　else if (!CheckEnd("Bounce"))
			{
				isDown = false;
				isAnimeStart = false;
				isBounce = false;
			}
		}
	}
	public void Blow()
    {
		if (blowDown)
		{

			blowTime += Time.fixedDeltaTime;
			if(blowTime <= 0.2)
			{
				//	Debug.Log($"速度{blowM.x}");
				if (blowTime <= 0.1)
				{
					isGround = false;
				}
					rb.AddForce(blowM,ForceMode2D.Impulse); 
			}
            else if(blowTime <= 1.5)// || !isGround)
            {
				blowM.Set(blowM.x,0);
				rb.AddForce(blowM, ForceMode2D.Impulse);

			}
		}
	}

	/// <summary>
	/// ダウン状態のメソッド
	/// </summary>
	public void Down()
	{
		//////Debug.log("吹き飛ぶ");

		if (blowDown)
		{
				if (!isWakeUp)
				{
					if (!isGround)
					{
						sAni.Play("Blow");
						//SetLayer(17);//回避レイヤーの場所変わってるかもな。追加や削除で
					//GravitySet(15);

				    }
					else if (isGround)
					{
					rb.velocity = Vector2.zero;
					if (!isAnimeStart)
					{
						sAni.Play("Down");
						isAnimeStart = true;
					}
					//GravitySet(status.firstGravity);
					else if (!CheckEnd("Down"))
					{
						AllStop(status.downRes);
						//Debug.Log("醤油");
						
					}

						//isDown = false;
						//ダウンアニメ
					}
				}
			else if (isDown && isGround && isWakeUp)
			{				
				//前のでisAnimeStartが真のままだから
					if (isAnimeStart)
					{
						isAnimeStart = false;
						sAni.Play("WakeUp");
					}
					else if (!CheckEnd("WakeUp"))
					{
					//	isAnimeStart = false;
							isDown = false;
							isWakeUp = false;
						blowDown = false;
						blowTime = 0;
						ArmorReset();
				    }
					
			}

		}
	}


    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
		//////Debug.log($"{collision.tag}タグ");
		////////Debug.log("開始");
		if(collision.tag == EnemyManager.instance.AttackTag && (isHitable || lastHit != collision.gameObject))
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
		else if (collision.tag == EnemyManager.instance.PMagicTag && (isHitable || lastHit != collision.gameObject))
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
		else if (collision.tag == EnemyManager.instance.SMagicTag && (isHitable || lastHit != collision.gameObject)) 
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

		if(isAggressive && collision.tag == EnemyManager.instance.JumpTag && isGround)
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
		if (collision.tag == EnemyManager.instance.AttackTag && (isHitable || lastHit != collision.gameObject))
		{
			//Debug.Log("きたで");
			WeponDamage();
			isHit = true;
			isHitable = false;
			lastHit = collision.gameObject;
		}
		else if (collision.tag == EnemyManager.instance.PMagicTag && (isHitable || lastHit != collision.gameObject))
		{

			PlayerMagicDamage();
			isHit = true;
			isHitable = false;
			lastHit = collision.gameObject;
		}
		else if (collision.tag == EnemyManager.instance.SMagicTag && (isHitable || lastHit != collision.gameObject))
		{

			SisterMagicDamage();
			isHit = true;
			isHitable = false;
			lastHit = collision.gameObject;
		}



		if (isAggressive && collision.tag == EnemyManager.instance.JumpTag && isGround)
		{
			if (collision.gameObject.GetComponent<JumpTrigger>().jumpDirection == transform.localScale.x)
			{

				//ステータスでジャンプタグ設定しなければジャンプできない敵が作れる
				jumpTrigger = true;
			}
		}

	}

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {

		if (collision.tag == EnemyManager.instance.PMagicTag)
		{
			//ここで最終弾だけ吹き飛ばすようにする？
			PlayerMagicDamage();
			isHit = false;
		}
		else if (collision.tag == EnemyManager.instance.SMagicTag)
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

			rb.AddForce(move(GManager.instance.pStatus.equipWeapon.blowPower.x * -direction, GManager.instance.pStatus.equipWeapon.blowPower.y));
		}*/

	public void HPReset()
    {
		hp = status.maxHp;
    }
	public void Die()
    {
        if (hp <= 0)
        {
			Destroy(this.gameObject);
        }
    }

	/// <summary>
	/// 攻撃前の初期化
	/// </summary>
	public void AttackPrepare()
    {
		//isAttack = true;
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
	}


	/// <summary>
	/// 攻撃
	/// 改修要請
	/// isShoot = trueの時の処理
	/// </summary>
	public void Attack()
	{
		if (isAtEnable && !isDown && !isAttack)
		{
			AttackPrepare();
			sAni.Play(status.attackName[attackNumber]);
			isAttack = true;
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
	/// 主に空中のもの
	/// </summary>
	public void Feint()
    {
		randomTime += Time.fixedDeltaTime;

		if (randomTime >= 2.5) {
		    moveDirectionX = RandomValue(0, 100) >= 50 ? 1 : -1;
			moveDirectionY = RandomValue(0, 100) >= 50 ? -1 : 1;
			randomTime = 0.0f;
		}
		move.Set(status.combatSpeed.x * moveDirectionX, status.combatSpeed.y * moveDirectionY);
		rb.AddForce(move);

	}

	/// <summary>
	/// ガードする
	/// </summary>
	public void GuardAct()
    {
		//isGuardは個別AIでいじる
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
	/// 必要ならisGuard点火とセットでFUpdateに入れる
	/// </summary>
	public void GuardBreak()
    {
        if (guardBreak)
        {
			if (guardBreak && (blowDown || isFalter))
			{
				guardBreak = false;
				isAnimeStart = false;
			}
			isGuard = false;
			if (!isAnimeStart)
			{
				isDown = true;
				sAni.Play("GuardBreak");
				isAnimeStart = true;
			}
           else if (!CheckEnd("GuardBreak") && isAnimeStart)
            {
				guardBreak = false;
				isAnimeStart = false;
				isDown = false;
            }
        }

	}

	/// <summary>
	/// クールタイムの間次の攻撃を待つ
	/// </summary>
	public void WaitAttack()
    {
		if (isAttack)
		{
			if (!CheckEnd(status.attackName[attackNumber]) || isDown)
			{
			//	isAtEnable = true;これクールタイムで管理しようよ
				isAttack = false;
			}
		}

		else if (!isAttack)
		{
			if (!isAtEnable && !atV.isCombo && !isDown)
			{
				stayTime += Time.fixedDeltaTime;
				if (stayTime >= atV.coolTime)
				{
					isAtEnable = true;
					stayTime = 0.0f;
				}
			}
			if (!isAtEnable && atV.isCombo && !isDown)
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



}


