

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using Guirao.UltimateTextDamage;

public class EnemyBase : MonoBehaviour
{
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
	[HideInInspector] public float hp;

	[Header("エネミーのステータス")]
	public EnemyStatus status;


	[Header("ドッグパイル")]
	///<summary>
	///移動の中心となるオブジェクト。敵を引き連れたり
	///</summary>
	public GameObject dogPile;

	// === 外部パラメータ ======================================
	/*[HideInInspector]*/
	public bool isAggressive;//攻撃モードの敵
	[HideInInspector] public bool isAttack;//これがTrueの時ひるまない
	protected bool isRight = true;//右にパトロールするかどうか
	protected bool isGround;
	protected Vector2 distance;//プレイヤーとの距離
	protected float escapeTime;//数えるだけ
	protected bool isJump;//ジャンプしてるかどうか
	protected bool isAvoid;
	protected float avoidTime;
	protected float guardTime;
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
	//ガード判定したかどうか
	protected bool guardJudge;
	protected float stayTime;//攻撃後の待機時間
	protected int attackNumber;//何番の攻撃をしているのか
	protected bool isAtEnable = true;
	protected bool isDamage;
	protected bool isMovable;
	protected bool isAnimeStart;//アニメの開始に使う
	protected bool isFalter;//よろめき
	protected bool isBounce;//攻撃をはじかれた
	protected byte damageType;
	protected bool heavy;

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
	protected Vector2 move;
	protected Vector2 blowM;
	protected float blowTime;
	protected float recoverTime;//アーマー回復までにかかる時間
	protected int lastArmor = 1;
	protected float nowArmor;
	protected UltimateTextDamageManager um;
	[HideInInspector] public SisMagic sm;
	[HideInInspector] public bool insert;
	protected bool isDie;

	[SerializeField]
	protected SpriteRenderer td;

	[SerializeField]
	protected Transform atBlock;
	/// <summary>
	/// 攻撃後フラグ。攻撃後の挙動で使う
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

	// === コード（Monobehaviour基本機能の実装） ================
	protected virtual void Start()
	{
		initialLayer = this.gameObject.layer;
		ArmorReset();

	//	rb = this.gameObject.GetComponent<Rigidbody2D>();
		startPosition = transform.position;
		firstDirection = transform.localScale;
		HPReset();
		um = EnemyManager.instance.um;
		if(dogPile != null)
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
		if(status.enemyFire != null)
        {
			enableFire = true;
        }
		//parentMatt = GetComponent<SpriteRenderer>().material;
		//td = GetComponent<TargetDisplay>();
	}



	protected virtual void Update()
	{
		// 落下チェック

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




	}

	/// <summary>
	/// プレイヤーの物理攻撃によるダメージ
	/// </summary>
	public void WeaponDamage()
	{


		//isDamage = true;
		//SetLayer(28);
		//////Debug.log($"ガード中か否か{guardHit}");
/*			bool back = false;
		bool stab = false;
if (!isDamage)
		{
			isDamage = true;
			//	GManager.instance.equipWeapon.hitLimmit--;

			Equip useEquip;
			byte attackType = 0;
            if (!GManager.instance.isShieldAttack)
            {
				useEquip = GManager.instance.equipWeapon;
            }
            else
            {
				useEquip = GManager.instance.equipShield;
			}

			//////Debug.log("終了");
			float damage = 0;//バフデバフ処理用にdamageとして保持する
			float mValue = GManager.instance.equipWeapon.mValue;
			//float damage;//バフデバフ処理用にdamageとして保持する
			if (useEquip.phyAtk > 0)
			{
				//斬撃刺突打撃を管理
				if (GManager.instance.equipWeapon.atType == MyCode.Weapon.AttackType.Slash)
				{
					damage += (Mathf.Pow(useEquip.phyAtk, 2) * mValue) / (useEquip.phyAtk + status.Def);
					if(useEquip.attackType == 1 || useEquip.attackType == 0)
                    {
						attackType = 1;
                    }
				}
				else if (GManager.instance.equipWeapon.atType == MyCode.Weapon.AttackType.Stab)
				{
					damage += (Mathf.Pow(useEquip.phyAtk, 2) * mValue) / (useEquip.phyAtk + status.pierDef);
					stab = true;
					if (useEquip.attackType == 1 || useEquip.attackType == 0)
					{
						attackType = 2;
					}
				}
				else
				{
					damage += (Mathf.Pow(useEquip.phyAtk, 2) * mValue) / (useEquip.phyAtk + status.strDef);
					if (useEquip.attackType == 1 || useEquip.attackType == 0)
						{
						attackType = 4;
                        //						Debug.Log("皿だ");
                        if (GManager.instance.equipWeapon.shock >= 40)
                        {
							heavy = true;
                        }
					}
				}

			}
			//神聖
			if (useEquip.holyAtk > 0)
			{
				damage += (Mathf.Pow(useEquip.holyAtk, 2) * mValue) / (useEquip.holyAtk + status.holyDef);

			}
			//闇
			if (useEquip.darkAtk > 0)
			{
				damage += (Mathf.Pow(useEquip.darkAtk, 2) * mValue) / (useEquip.darkAtk + status.darkDef);

			}
			//炎
			if (useEquip.fireAtk > 0)
			{
				damage += (Mathf.Pow(useEquip.fireAtk, 2) * mValue) / (useEquip.fireAtk + status.fireDef);
			}
			//雷
			if (useEquip.thunderAtk > 0)
			{
				damage += (Mathf.Pow(useEquip.thunderAtk, 2) * mValue) / (useEquip.thunderAtk + status.thunderDef);

			}
			if (useEquip.attackType > 4)
			{
				attackType = useEquip.attackType;
			}
				DamageSound(attackType);

			if ((transform.position.x <= GManager.instance.Player.transform.position.x && transform.localScale.x < 0) ||
	(transform.position.x > GManager.instance.Player.transform.position.x && transform.localScale.x > 0))
			{
				back = true;
			}

			////Debug.log($"{ damage * GManager.instance.attackBuff}ダメージ");
			//////Debug.log($"{nowArmor}a-mor");
			damage = Mathf.Floor(damage * GManager.instance.attackBuff);

            if (back)
            {
				damage *= 1.08f;
            }
            if (isDown && !isWakeUp)
            {
				damage *= 1.2f;
            }
			else if(isDown && isWakeUp)
            {
				damage *= 0.9f;
            }
			else if (isAttack && stab)
            {
				damage *= 1.1f;
            }

			hp -= damage;//HP引いてる

				//			Debug.Log($"背後{back}");

				um.AddStack(damage, this.transform);
			if (!isAttack)
			{

				float shock = GManager.instance.equipWeapon.shock;
				if (back)
				{
					shock *= 1.2f;
				}
				nowArmor -= shock;

			}
			else
			{
				if (atV.aditionalArmor != 0)
				{
					//バックアタック

					float shock = GManager.instance.equipWeapon.shock;
					if(back)
                    {
						shock *= 1.2f;
                    }

					nowArmor -= (shock - atV.aditionalArmor) < 0 ? 0 : (shock - atV.aditionalArmor);
					atV.aditionalArmor = (atV.aditionalArmor - shock) < 0 ? 0 : atV.aditionalArmor - shock;

				}
			}
			ArmorControll();
			if (isDown)
			{
				if (GManager.instance.equipWeapon.isBlow)
				{
					//	Debug.Log("aaaa");
					blowM.Set(GManager.instance.equipWeapon.blowPower.x * -direction, GManager.instance.equipWeapon.blowPower.y);

					//rb.AddForce(move);
					blowDown = true;
					//Invoke("Down", 1f);
				}
				else
				{
					isFalter = true;
				}
			}
			if (hp <= 0)
			{
				isDie = true;
				atBlock.gameObject.SetActive(false);
				//atBlock.gameObject.SetActive(false);
				isAnimeStart = false;
                if (!isGround)
                {
					isDown = true;
                }
			}
		}*/
	}

	public void WeaponGuard()
	{
	
	}

	/// <summary>
	/// プレイヤーの魔法で受けるダメージ
	/// </summary>
	public void PlayerMagicDamage()
    {
		if (/*&& GManager.instance.equipWeapon.hitLimmit > 0 &&*/ !isDamage)
		{
			isDamage = true;
			//GManager.instance.equipWeapon.hitLimmit--;
			float damage = 0;//バフデバフ処理用にdamageとして保持する
			float mValue = GManager.instance.useMagic.mValue;
			//float damage;//バフデバフ処理用にdamageとして保持する

			if (GManager.instance.useMagic.phyAtk > 0)
			{

			}
			//神聖
			if (GManager.instance.useMagic.holyAtk > 0)
			{
				damage += (Mathf.Pow(GManager.instance.useMagic.holyAtk, 2) * mValue) / (GManager.instance.useMagic.holyAtk + status.holyDef);
			}
			//闇
			if (GManager.instance.useMagic.darkAtk > 0)
			{
				damage += (Mathf.Pow(GManager.instance.useMagic.darkAtk, 2) * mValue) / (GManager.instance.useMagic.darkAtk + status.darkDef);
			}
			//炎
			if (GManager.instance.useMagic.fireAtk > 0)
			{
				damage += (Mathf.Pow(GManager.instance.useMagic.fireAtk, 2) * mValue) / (GManager.instance.useMagic.fireAtk + status.fireDef);
			}
			//雷
			if (GManager.instance.useMagic.thunderAtk > 0)
			{
				damage += (Mathf.Pow(GManager.instance.useMagic.thunderAtk, 2) * mValue) / (GManager.instance.useMagic.thunderAtk + status.thunderDef);
			}

			
		//	DamageSound();

			damage = Mathf.Floor(damage * GManager.instance.attackBuff);
			hp -= damage;//HP引いてる

			um.AddStack(damage, this.transform);
			if (!isAttack)
			{
				nowArmor -= GManager.instance.useMagic.shock;
			}
			else
			{
				if (atV.aditionalArmor != 0)
				{

					nowArmor -= (GManager.instance.useMagic.shock - atV.aditionalArmor) < 0 ? 0 : (GManager.instance.useMagic.shock - atV.aditionalArmor);
					atV.aditionalArmor = (atV.aditionalArmor - GManager.instance.useMagic.shock) < 0 ? 0 : atV.aditionalArmor - GManager.instance.useMagic.shock;

				}
			}
			ArmorControll();
			if (isDown && GManager.instance.useMagic.isBlow)
			{
				blowM.Set(GManager.instance.useMagic.blowPower.x * -direction, GManager.instance.useMagic.blowPower.y);
				blowDown = true;
				//Invoke("Down",1f);
			}
			else
			{
				isFalter = true;
			}
			if (hp <= 0)
			{
				isDie = true;isAnimeStart = false;
				atBlock.gameObject.SetActive(false);
				if (!isGround)
				{
					isDown = true;
				}
			}
		}
	}
	public void PlayerMagicGuard()
	{

	}
	public void SisterMagicDamage()
	{
	//	Debug.Log($"起動{sm.name}邪魔者{isDamage}");
		if (/*&& GManager.instance.equipWeapon.hitLimmit > 0 &&*/ !isDamage && !isDie)
		{

		//	Debug.Log($"怪異{sm.name}");

			isDamage = true;
			//GManager.instance.equipWeapon.hitLimmit--;
			float damage = 0;//バフデバフ処理用にdamageとして保持する
			float mValue = sm.mValue;
			//float damage;//バフデバフ処理用にdamageとして保持する

			//神聖
			if (sm.holyAtk > 0)
			{
				damage += (Mathf.Pow(sm.holyAtk, 2) * mValue) / (sm.holyAtk + status.holyDef);
			}
			//闇
			if (sm.darkAtk > 0)
			{
				damage += (Mathf.Pow(sm.darkAtk, 2) * mValue) / (sm.darkAtk + status.darkDef);
			}
			//炎
			if (sm.fireAtk > 0)
			{
				damage += (Mathf.Pow(sm.fireAtk, 2) * mValue) / (sm.fireAtk + status.fireDef);
			}
			//雷
			if (sm.thunderAtk > 0)
			{
				damage += (Mathf.Pow(sm.thunderAtk, 2) * mValue) / (sm.thunderAtk + status.thunderDef);
			}

			damage = Mathf.Floor(damage * SManager.instance.sisStatus.attackBuff);
			hp -= damage;

			//Mathf.Floor(hp);
			um.AddStack(damage, this.transform);
			if (!isAttack)
			{
				nowArmor -= sm.shock;
			}
			else
			{
				if (atV.aditionalArmor != 0)
				{

					nowArmor -= (sm.shock - atV.aditionalArmor) < 0 ? 0 : (sm.shock - atV.aditionalArmor);
					atV.aditionalArmor = (atV.aditionalArmor - sm.shock) < 0 ? 0 : atV.aditionalArmor - sm.shock;

				}
			}
			ArmorControll();
			if (isDown && sm.isBlow)
			{
				blowM.Set(sm.blowPower.x * bulletDirection, sm.blowPower.y);
			//	rb.AddForce(move, ForceMode2D.Impulse);
				blowDown = true;
			}
			else
			{
				isFalter = true;
			}
			if (hp <= 0)
			{
				isDie = true;
				atBlock.gameObject.SetActive(false);
				isAnimeStart = false;
				if (!isGround)
				{
					isDown = true;
				}
			}
		}
	}
	public void SisterMagicGuard()
	{
		if (/*&& GManager.instance.equipWeapon.hitLimmit > 0 */!isDamage)
		{
			isDamage = true;
			//GManager.instance.equipWeapon.hitLimmit--;
			float damage = 0;//バフデバフ処理用にdamageとして保持する
			float mValue = SManager.instance.useMagic.mValue;
			//float damage;//バフデバフ処理用にdamageとして保持する
			if (SManager.instance.useMagic.phyAtk > 0)
			{


				damage *= (100 - status.phyCut)/100;
			}
			//神聖
			if (SManager.instance.useMagic.holyAtk > 0)
			{
				damage += (Mathf.Pow(SManager.instance.useMagic.holyAtk, 2) * mValue) / (SManager.instance.useMagic.holyAtk + status.holyDef) * (100 - status.holyCut)/100;
			}
			//闇
			if (SManager.instance.useMagic.darkAtk > 0)
			{
				damage += (Mathf.Pow(SManager.instance.useMagic.darkAtk, 2) * mValue) / (SManager.instance.useMagic.darkAtk + status.darkDef) * (100 - status.darkCut)/100;
			}
			//炎
			if (SManager.instance.useMagic.fireAtk > 0)
			{
				damage += (Mathf.Pow(SManager.instance.useMagic.fireAtk, 2) * mValue) / (SManager.instance.useMagic.fireAtk + status.fireDef) * (100 - status.fireCut)/100;
			}
			//雷
			if (SManager.instance.useMagic.thunderAtk > 0)
			{
				damage += (Mathf.Pow(SManager.instance.useMagic.thunderAtk, 2) * mValue) / (SManager.instance.useMagic.thunderAtk + status.thunderDef) * (100 -status.thunderCut)/100;
			}
			damage = Mathf.Floor(damage * SManager.instance.sisStatus.attackBuff);
			hp -= damage;

			//Mathf.Floor(hp);
			um.AddStack(damage, this.transform);


			if (SManager.instance.useMagic.isBlow)
			{
				nowArmor -= ((SManager.instance.useMagic.shock * 2) * status.guardPower / 100) * 1.2f;
			}
			else
			{
				nowArmor -= (SManager.instance.useMagic.shock * 2) * ((100 - status.guardPower) / 100);
			}

			if (nowArmor <= 0 && SManager.instance.useMagic.isBlow)
			{
				blowM.Set(SManager.instance.useMagic.blowPower.x * bulletDirection, SManager.instance.useMagic.blowPower.y);
				//rb.AddForce(move, ForceMode2D.Impulse);
				blowDown = true;
			}
			else
			{
				guardBreak = true;
			}
			if (hp <= 0)
			{
				isDie = true;
				atBlock.gameObject.SetActive(false);
				isAnimeStart = false;
				if (!isGround)
				{
					isDown = true;
				}
			}
		}
	}

	public void PlayerParry()
    {
		if (!GManager.instance.isDamage)
		{
			if (!GManager.instance.equipWeapon.twinHand)
            {
				GManager.instance.stamina += GManager.instance.equipShield.parryRecover;
            }
            else
            {
				GManager.instance.stamina += GManager.instance.equipWeapon.parryRecover;
			}

			GManager.instance.isDamage = true;
			GManager.instance.isGuard = false;
			GManager.instance.isParry = false;

			//Debug.Log($"mae{nowArmor}");
			nowArmor -= Mathf.Ceil(status.Armor * ((100 - atV.parryResist) / 100));
			Debug.Log($"あと{Mathf.Ceil(status.Armor * ((100 - atV.parryResist) / 100))}");
			//Debug.Log($"tellme{nowArmor}{atV.parryResist}");

			if(nowArmor <= 0)
            {
			atV.isCombo = false;
			isAttack = false;
			isBounce = true;
			isDown = true;
			GManager.instance.parrySuccess = true;
            }
            else
            {
				GManager.instance.blocking = true;
				GManager.instance.parrySuccess = true;
			}


		}
	}
	public void AttackDamage()
    {

		if (!GManager.instance.isDamage)// && !GManager.instance.guardHit)
		{
			//GManager.instance.isDamage = true;
			//status.hitLimmit--;
								 //mValueはモーション値
			float damage = 0;//バフデバフ処理用にdamageとして保持する
			if (status.phyAtk > 0)
			{
				//斬撃刺突打撃を管理
				if (atV.type == EnemyStatus.AttackType.Slash)
				{
					damage += (Mathf.Pow(status.phyAtk, 2) * atV.mValue) / (status.phyAtk + GManager.instance.Def);
					GManager.instance.attackType = 1;
				}
				else if (atV.type == EnemyStatus.AttackType.Stab)
				{
					damage += (Mathf.Pow(status.phyAtk, 2) * atV.mValue) / (status.phyAtk + GManager.instance.pierDef);
					GManager.instance.attackType = 2;
				}
				else if (atV.type == EnemyStatus.AttackType.Strike)
				{
					damage += (Mathf.Pow(status.phyAtk, 2) * atV.mValue) / (status.phyAtk + GManager.instance.strDef);
					GManager.instance.attackType = 4;
					//						Debug.Log("皿だ");
					if (atV.shock >= 40)
					{
						GManager.instance.heavy = true;
					}
				}
			}
			//神聖
			if (status.holyAtk > 0)
			{
				damage += (Mathf.Pow(status.holyAtk, 2) * atV.mValue) / (status.holyAtk + GManager.instance.holyDef);
				GManager.instance.attackType = 8;
			}
			//闇
			if (status.darkAtk > 0)
			{
				damage += (Mathf.Pow(status.darkAtk, 2) * atV.mValue) / (status.darkAtk + GManager.instance.darkDef);
				GManager.instance.attackType = 16;
			}
			//炎
			if (status.fireAtk > 0)
			{
				damage += (Mathf.Pow(status.fireAtk, 2) * atV.mValue) / (status.fireAtk + GManager.instance.fireDef);
				GManager.instance.attackType = 32;
			}
			//雷
			if (status.thunderAtk > 0)
			{
				damage += (Mathf.Pow(status.thunderAtk, 2) * atV.mValue) / (status.thunderAtk + GManager.instance.thunderDef);
				GManager.instance.attackType = 64;
			}
			if (!GManager.instance.isAttack)
			{
			//	GManager.instance.nowArmor -= atV.shock;
			}
			else
			{
			//	GManager.instance.nowArmor -= (atV.shock - GManager.instance.equipWeapon.atAromor) < 0 ? 0 : (atV.shock - GManager.instance.equipWeapon.atAromor);
			}

			GManager.instance.DamageSound(GManager.instance.attackType);
		//	GManager.instance.ArmorControll();

			damage = Mathf.Floor(damage * attackBuff);
			//GManager.instance.HpReduce;
			GManager.instance.hp -= damage;
			//Debug.Log($"攻撃時のダメージ{damage}");
			//GManager.instance.hp = 10;
			//Debug.Log($"ダメージ{damage}");
			//Debug.Log($"残り{GManager.instance.hp}");
			GManager.instance.isDamage = true;
		}
		if (1 <= 0)
		{
		//	GManager.instance.isDown = true;
			/*if (atV.isBlow == true || !GManager.instance.pm.isGround)
			{
				GManager.instance.blowDown = true;
				if (atV.isBlow)
				{
					if (direction >= 0)
					{
						GManager.instance.blowVector = atV.blowPower;

					}
					else if (direction < 0)
					{
						
						GManager.instance.blowVector.Set(-atV.blowPower.x, atV.blowPower.y);

					}
				}
                else
                {
					GManager.instance.pm.rb.velocity = Vector2.zero;
					GManager.instance.blowVector.Set(0, 18f);
				}
			}
            else
            {
				GManager.instance.isFalter = true;
            }*/
		}
}

	public void PlayerGuard()
    {
		if (!GManager.instance.isDamage) //&& !GManager.instance.guardHit)
		{

			Equip useEquip;
            if (!GManager.instance.equipWeapon.twinHand)
            {
				useEquip = GManager.instance.equipShield;
			}
            else
            {
				useEquip = GManager.instance.equipWeapon;
			}

			//status.hitLimmit--;
			//mValueはモーション値
			float damage = 0;//バフデバフ処理用にdamageとして保持する
			if (status.phyAtk > 0)
			{
				//斬撃刺突打撃を管理
				if (atV.type == EnemyStatus.AttackType.Slash)
				{ damage += (Mathf.Pow(status.phyAtk, 2) * atV.mValue) / (status.phyAtk + GManager.instance.Def); }
				else if (atV.type == EnemyStatus.AttackType.Stab)
				{ damage += (Mathf.Pow(status.phyAtk, 2) * atV.mValue) / (status.phyAtk + GManager.instance.pierDef); }
				else if (atV.type == EnemyStatus.AttackType.Strike)
				{ damage += (Mathf.Pow(status.phyAtk, 2) * atV.mValue) / (status.phyAtk + GManager.instance.strDef); }

				damage *= (100 - useEquip.phyCut)/100;
			}
			//神聖
			if (status.holyAtk > 0)
			{
				damage += ((Mathf.Pow(status.holyAtk, 2) * atV.mValue) / (status.holyAtk + GManager.instance.holyDef)) * (100 - useEquip.holyCut)/100;
			}
			//闇
			if (status.darkAtk > 0)
			{
				damage += ((Mathf.Pow(status.darkAtk, 2) * atV.mValue) / (status.darkAtk + GManager.instance.darkDef)) * (100 - useEquip.darkCut)/100;
			}
				//炎
				if (status.fireAtk > 0)
			{
				damage += ((Mathf.Pow(status.fireAtk, 2) * atV.mValue) / (status.fireAtk + GManager.instance.fireDef)) * (100 - useEquip.fireCut)/100;
			}
			//雷
			if (status.thunderAtk > 0)
			{
				damage += ((Mathf.Pow(status.thunderAtk, 2) * atV.mValue) / (status.thunderAtk + GManager.instance.thunderDef)) * (100 - useEquip.thunderCut)/100;
			}
			if (atV.isLight && useEquip.guardPower >= 50)
			{//受け値50以上の盾は軽い攻撃をはじく
				////(status.motionIndex["弾かれ"]);
			}
			    GManager.instance.stamina -= (atV.shock * 3) * (1 - (useEquip.guardPower / 100));



			damage = Mathf.Floor(damage * attackBuff);
			//Debug.Log($"ガード時のダメージ{damage}");
			GManager.instance.hp -= damage * attackBuff;

			if (!GManager.instance.equipWeapon.twinHand)
			{

				GManager.instance.pm.anim.Play("OGuard");
				//Debug.Log("ガード");
			}
			else
			{
				GManager.instance.pm.anim.Play("TGuard");
				//Debug.Log("ガード");
			}
			//GManager.instance.isDamage = true;

		if (GManager.instance.stamina <= 0)
		{
			if (atV.isBlow == true)
			{
				GManager.instance.isDown = true;
				GManager.instance.blowDown = true;
				GManager.instance.isGuard = false;
				GManager.instance.isDamage = true;
				if (direction > 0)
				{
					GManager.instance.blowVector.Set(atV.blowPower.x, atV.blowPower.y);

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
				GManager.instance.isDamage = true;

			}
		}
        else
        {

			GManager.instance.isDamage = true;
			GManager.instance.guardHit = true;
			if (GManager.instance.Player.transform.localScale.x > 0)
			{
					//退かせたい
					GManager.instance.nockBack = (atV.shock * -200) * (1f - (useEquip.guardPower / 100));

			}
			else
			{
					GManager.instance.nockBack = (atV.shock * 200) * (1f - (useEquip.guardPower / 100));

				}
		}

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
		
	
	}
	/// <summary>
	/// 待機中の哨戒行動
	/// </summary>
	public void PatrolMove()
    {
    }

	/// <summary>
	/// 待機中空を飛ぶ
	/// 待機と選択で積む
	/// </summary>
	public void PatrolFly()
    {
		if (!isStop && !posiReset && !nowJump && !isDown && !isDie)
		{


		}
	}

	/// <summary>
	/// 待機中停止してきょろきょろ振り向きだけ。こいつはパトロールしないからwaitTimeは使いまわしでいい
	/// パトロールと選択で積む
	/// なにかに止まってるハエとかも表現できる
	/// </summary>
	public void Wait()
    {
		if (!posiReset && !isDie)
		{
			if (dRb == null)
			{
				//("Stand");
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
				//("Move");
				rb.velocity = dRb.velocity;
				transform.localScale = dogPile.transform.localScale;
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
	/// </summary>
	public void AgrMove(int disIndex = 0)
    {

	}
	
	/// <summary>
	/// 空を飛ぶタイプのエネミーに戦闘中乗せる。空を飛ぶ
	/// </summary>
	public void AgrFly(int disIndex = 0)
	{

	}
	/// <summary>
	/// 回避。方向指定も可能だが戦闘時に限りdirectionで前方。-directionで後ろに
	/// </summary>
	/// <param name="direction"></param>
	public void Avoid(float direction) 
	{
		if (!isStop && !nowJump && !isDown && !isDie)
		{
			if (isAvoid)
			{
				//アニメを時間に合わせろ
				//("Avoid");
				avoidTime += Time.fixedDeltaTime;
			//	move.Set(direction * status.avoidSpeed.x, status.avoidSpeed.y);
				rb.AddForce(move);
				Flip(direction);
				SetLayer(28);
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

		if (isDamage || isDie) 
		{
			avoidTime += Time.fixedDeltaTime;

			SetLayer(28);
			if (avoidTime >= 0.15 &&  !isDie)// &&!GManager.instance.fallAttack
			{
				guardJudge = true;
				isDamage = false;
                if (guardHit)
                {
					guardTime = avoidTime;
                }
				avoidTime = 0;
				SetLayer(initialLayer);
			}
		}
        else if(isDown)
        {
            if (blowDown && isWakeUp)
            {
			
				SetLayer(28);
			}
            else
            {
				SetLayer(initialLayer);
			}
        }

        if (guardHit && !isDamage)
        {
			guardTime += Time.fixedDeltaTime;
            if (guardTime >= 0.5 && !GManager.instance.fallAttack)
            {
				guardHit = false;
				guardTime = 0;
            }
		}

	}
	//status.kind == EnemyStatus.KindofEnemy.Fly

	public void EnemyFall()
	{

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
				//move.Set(jumpMove * EnemyManager.instance.jMove.Evaluate(jumpTime), jumpSpeed * EnemyManager.instance.jPower.Evaluate(jumpTime));
				rb.AddForce(move);
				//("Jump");
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
				//("Jump");
				jumpTime += Time.fixedDeltaTime;
				//move.Set(jumpMove * EnemyManager.instance.jMove.Evaluate(jumpTime), jumpSpeed * EnemyManager.instance.jPower.Evaluate(jumpTime));
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
	//	Debug.Log($"今のアーマー{nowArmor}");
		if(!isDown && !isDamage && !isAttack)
        {
			recoverTime += Time.fixedDeltaTime;
			if(recoverTime >= 15 || nowArmor > status.Armor)
            {
				ArmorReset();
				recoverTime = 0.0f;
				lastArmor = 1;
			//	lastArmor = nowArmor; 
			}
            else if(nowArmor < status.Armor && recoverTime >= 3 * lastArmor)
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
		if ((blowDown || guardBreak) && isFalter)
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
				//("Falter");
			    }
            else if (!CheckEnd("Falter"))
            {
				isDown = false;
				isAnimeStart = false;
				isFalter = false;
				ArmorReset();
				
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
				//("Bounce");
				//Debug.Log("弾かれ");
			}
			　else if (!CheckEnd("Bounce"))
			{
				isDown = false;
				isAnimeStart = false;
				isBounce = false;
				ArmorReset();
			}
		}
	}
	public void Blow()
    {
		if (blowDown && isDown)
		{

			blowTime += Time.fixedDeltaTime;
			if(blowTime <= 0.2)
			{
				isAnimeStart = false;
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

		if (blowDown && isDown)
		{
				if (!isWakeUp)
				{
					if (!isGround)
					{
						//("Blow");
						//SetLayer(17);//回避レイヤーの場所変わってるかもな。追加や削除で
					//GravitySet(15);

				    }
					else if (isGround)
					{
					rb.velocity = Vector2.zero;
					if (!isAnimeStart)
					{
						if (status.isMetal)
						{
							GManager.instance.PlaySound( MyCode.SoundManager.instance.downSound[0], transform.position);
						}
						else
						{
							GManager.instance.PlaySound( MyCode.SoundManager.instance.downSound[1], transform.position);
						}
						//("Down");

						isAnimeStart = true;
					}
					//GravitySet(status.firstGravity);
					else if (!CheckEnd("Down"))
					{
						if(isDie)
                        {
							isDown = false;
							isAnimeStart = false;
							return;
                        }
					//	AllStop(status.downRes);
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
						//("WakeUp");
					}
					else if (!CheckEnd("WakeUp"))
					{
					SetLayer(initialLayer);
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


		if (!isDie)
		{
			//////Debug.log($"{collision.tag}タグ");
			////////Debug.log("開始");
			if (collision.tag == EnemyManager.instance.AttackTag && (isHitable || lastHit != collision.gameObject))
			{

				bool success;
				if (isGuard)
				{
					if (transform.localScale.x > 0)
					{
						success = transform.position.x <= GManager.instance.Player.transform.position.x ? true : false;
					}
					else
					{
						success = transform.position.x > GManager.instance.Player.transform.position.x ? true : false;
					}

					if (success)
					{
						WeaponGuard();
						return;
					}
				}
				WeaponDamage();
				if (!isAggressive)
				{
					Serch.SetActive(false);
					Serch2.SetActive(false);
					isAggressive = true;
					Flip(direction);
				}
				lastHit = collision.gameObject;
				isHit = true;
				isHitable = false;
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
					Flip(direction);
				}
				lastHit = collision.gameObject;
			}
			else if (collision.tag == EnemyManager.instance.SMagicTag) //&& (isHitable || lastHit != collision.gameObject))
			{
				//SisMagic sm = collision.gameObject.GetComponent<SisMagic>();

				//		SisterMagicDamage();

				//	if (!isAggressive)
				//	{
		//		Debug.Log($"衝突したもの{collision.gameObject.name}");
			//	}
			//	lastHit = collision.gameObject;
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
	}
	protected virtual void OnTriggerStay2D(Collider2D collision)
	{
		if (!isDie)
		{
			//Debug.Log("きたで");
			if (collision.tag == EnemyManager.instance.AttackTag && (isHitable || lastHit != collision.gameObject))
			{

				bool success;
				if (isGuard)
				{
					if (transform.localScale.x > 0)
					{
						success = transform.position.x <= GManager.instance.Player.transform.position.x ? true : false;
					}
					else
					{
						success = transform.position.x > GManager.instance.Player.transform.position.x ? true : false;
					}

					if (success)
					{
						WeaponGuard();
						return;
					}
				}
				//WeaponDamage();
				WeaponDamage();
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
			else if (collision.tag == EnemyManager.instance.SMagicTag)// && !isAggressive) //&& (isHitable || lastHit != collision.gameObject))
			{
				//SisMagic sm = collision.gameObject.GetComponent<SisMagic>();

				//		SisterMagicDamage();

				//	if (!isAggressive)
				//	{
			//	Debug.Log($"衝突したもの{collision.gameObject.name}");
				//	}
				//	lastHit = collision.gameObject;
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
	}

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {

	/*	if (collision.tag == EnemyManager.instance.PMagicTag)
		{
			//ここで最終弾だけ吹き飛ばすようにする？
			PlayerMagicDamage();
			isHit = false;
		}
		else if (collision.tag == EnemyManager.instance.SMagicTag)
		{

			SisterMagicDamage(collision.gameObject);
			isHit = false;
		}*/

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

			rb.AddForce(move(GManager.instance.equipWeapon.blowPower.x * -direction, GManager.instance.equipWeapon.blowPower.y));
		}*/

	public void HPReset()
    {
		hp = status.maxHp;
    }
	public void Die()
    {
        if (isDie)
        {
			
            if(!isDown && blowDown)
            {

				
			}
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
		atV.escapePercentage = status.atValue[attackNumber].escapePercentage;
		atV.parryResist = status.atValue[attackNumber].parryResist;
	//	atV.attackEffect = status.atValue[attackNumber].attackEffect;
	}


	/// <summary>
	/// 攻撃
	/// 改修要請
	/// isShoot = trueの時の処理つくる？。いらない？つまりたまうちってこったな
	/// リヴァースで逆向く
	/// </summary>
	public void Attack(bool select = false,int number = 0,bool reverse = false)
	{

		
		if (isAtEnable && !isDown && !isAttack && !isDie && !guardHit)
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
			//(status.attackName[attackNumber]);
			isAttack = true;
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
		number = number > status.serectableNumber.Count - 1 ? status.serectableNumber.Count - 1 : number;
		number = number < 0 ? 0 : number;
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
	/*
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
	}*/
	/// <summary>
	/// ガードブレイク
	/// 必要ならisGuard点火とセットでFUpdateに入れる
	/// </summary>
	public void GuardBreak()
    {
        if (guardBreak)
        {
			if (guardBreak && blowDown)
			{
				guardBreak = false;
				isAnimeStart = false;
			}
			isGuard = false;
			if (!isAnimeStart)
			{
				isDown = true;
				//("GuardBreak");
				isAnimeStart = true;
			}
           else if (!CheckEnd("GuardBreak"))
            {
				ArmorReset();
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
			if (isDown)//!CheckEnd(status.attackName[attackNumber]) || 
			{
				if (!isDown)
				{
					Flip(direction);
				}
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



	public void DisParriableAct()
    {
		GManager.instance.PlaySound("DisenableParry",transform.position);
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
		Addressables.InstantiateAsync("DisParriableEffect",dispary.transform);
    }
	public void attackEffect()
	{
		// Debug.Log($"アイイイイイイ{atEffect.SubObjectName}");
	//	Addressables.InstantiateAsync(atV.attackEffect, effectController.transform);
	}

	public void BattleFlip(float direction)
    {
		if(lastDirection != direction)
        {
		flipWaitTime += Time.fixedDeltaTime;

		if(flipWaitTime >= 1f)
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
		isGuard = ground == EnemyStatus.MoveState.stay && !isDown && !isAttack ? true : false;
		if (!isAttack && !isDown && guardJudge && !isGuard)
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
jumpTime += Time.fixedDeltaTime;
				float test = Mathf.Lerp(0f, 1, jumpTime/2);
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

	private void GetAllChildren(Transform parent, ref List<Transform> transforms,bool Weapon = false)
	{
		foreach (Transform child in parent)
		{
			transforms.Add(child);
			GetAllChildren(child, ref transforms,true);
			Renderer sr = child.GetComponent<Renderer>();
			if(sr != null)
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
				GetAllChildren(die,ref transforms,true);
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
		if(state == 0)
        {
			td.gameObject.SetActive(true);
			td.color = EnemyManager.instance.stateClor[0];
		}
		else if(state == 1)
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

}


