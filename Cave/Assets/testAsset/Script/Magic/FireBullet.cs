using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using System;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine.Jobs;
using Unity.Jobs;

/// <summary>
/// 最初にDamageOnTouchと回復量を初期化する。
/// タグ関連の仕様変更
/// 
/// まず攻撃者にかかってるバフを取得
/// そして攻撃力などはステータスから獲得
/// これは通り飛んで当たるだけ

/// 最初は追尾弱くして上向きに飛ばす弾丸を徐々に追尾強くしたりすれば曲射とか特殊な軌道の弾丸作れそう
/// </summary>

public class FireBullet : MonoBehaviour
{

	/// <summary>
	/// 魔法の使用者が誰か
	/// ダメージ処理とかで使うかな
	/// 初期化処理でも使うか、使用者の攻撃倍率とか聞かないといけないし
	/// </summary>
	[Header("魔法の使用者が誰か")]
	[SerializeField]
	MasicUser _user;

	[SerializeField]
	MyDamageOntouch _damage;

	public enum MasicUser
    {
		Player,
		Sister,
		Others,
		Child//子どもの弾丸
    }

	// === 外部パラメータ（インスペクタ表示） =====================

	public Magic em;

	[Header("標的")]
	/// <summary>
	/// 標的。狙うもの
	/// </summary>


	// === 外部パラメータ ======================================
	[System.NonSerialized] Transform owner;
	/// <summary>
	/// よくわからん存在
	/// </summary>
	[System.NonSerialized] public bool attackEnabled;

	// === 内部パラメータ ======================================
	[SerializeField] Rigidbody2D rb;
	/// <summary>
	/// 弾丸がどれくらいの時間存在してるか計測する。
	/// </summary>
	float fireTime;


	public GameObject target;

	TransformAccessArray myTransform;




	/// <summary>
	/// 存在音ならしたか
	/// 音を鳴らすのに使う
	/// </summary>
	bool loud;//常に音が鳴るかどうか
	bool isAct;
	//	bool hitSound;
	/// <summary>
	/// 存在音ならしたか
	/// 音を消すのに使う
	/// </summary>
	bool isExPlay;


	GameObject next;


	//次に出す弾丸の動作スクリプト
	FireBullet sm;
	/// <summary>
	/// キャラが右と左どちらから弾を撃ったか
	/// </summary>
	[HideInInspector] public int direction;



	float effectWait;//サポートや回復が再度効果を現すまで

	/// <summary>
	/// 弾丸に精製後の待ち時間がある場合に何秒待ったか数える
	/// WaitTimeとHormingTimeを同じかそれ以下にすれば停止中だけ敵を狙うやつになる
	/// </summary>
	float waitNow;
	Collider2D col;

	/// <summary>
	/// もし弾丸が動かない待ち時間があるなら止めるフラグ
	/// </summary>
	bool movable;


	FireJob job;

	JobHandle _handler;


	/// <summary>
	/// 初期化されてますか？
	/// </summary>
	bool _initialized;

	/// <summary>
	/// すでに衝突したもの
	/// </summary>
	List<Transform> collisionList = new List<Transform>();


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

	protected float attackBuff = 1;//攻撃倍率

	string _healTag;



    private void Awake()
    {
        if(_user == MasicUser.Player || _user == MasicUser.Sister)
        {
			InitializePlayerBullet();
        }

		myTransform = new TransformAccessArray(0);
		myTransform.Add(transform);
    }


    // === コード（Monobehaviour基本機能の実装） ================
    async void Start()
	{
		Func<bool> s = () => 
		{
			//初期化が完了したかどうかをはかるだけの処理
			//初期化完了は発射側がやる
			return _initialized;
		};
		_damage.CollidRestoreResset();
		_damage._attackData._hitLimit = em._hitLimit;
		//初期化完了が真を返すまで待つ

		var token = this.GetCancellationTokenOnDestroy();
		await UniTask.WaitUntil(s,cancellationToken:token);

		fireTime = 0;

		if (this.gameObject != null)
		{
			col = this.gameObject.MMGetComponentNoAlloc<Collider2D>();
		}

		//子供の弾丸は親のターゲット引き継ぐ
		if (em._moveSt.fireType != Magic.FIREBULLET.STOP || !em.isChild)
		{
			//	Debug.Log($"標的設定{SManager.instance.restoreTarget.name}");
			
			//
			direction = (owner.localScale.x < 0.0f) ? -1 : 1;
		}






		//Debug.Log($"標的{gameObject.name}");



		// 存在してる間のサウンドがあるなら
		if (em.existSound != null)
		{
			loud = true;
			isExPlay = true;
		}


		attackEnabled = true;
		GManager.instance.PlaySound(em.fireSound, transform.position);

		//	hitSound = em.hitSound != null ? true : false;
		
		//ヒット時に他の弾丸を出すなら
		if (em.isHit)
		{
			next = await LoadBullet();
			sm = next.MMGetComponentNoAlloc<FireBullet>();
			//smが存在してるなら
			//Debug.Log($"名前{target.name}");

			
		}


		if (em.isChild)
		{
			Vector3 setScale = transform.localScale;
			if (direction < 0)
			{
				setScale.x = -setScale.x;
			}
			transform.localScale = setScale;

		}
		if(this.gameObject != null)
		DamageCalc();
	}



	void OnTriggerEnter2D(Collider2D other)
	{
	//Debug.Log($"sdddssdsdsd{other.gameObject.name}");
	
		BulletHit(other);
	}
	void OnTriggerStay2D(Collider2D other)
	{
		//Debug.Log($"sdddssdsdsd{other.gameObject.name}");
		BulletHit(other);
	}

    private void OnDisable()
    {
		myTransform.Dispose();
    }

    void FixedUpdate()
	{

        Debug.Log($"名前{this.gameObject.name}標的{target == null}");
		//初期化してないなら戻りましょう
        if (!_initialized)
        {
			return;
        }

		fireTime += Time.fixedDeltaTime;
		if (collisionList.Count > 0)
		{
			effectWait += Time.fixedDeltaTime;
            //三秒以上ならプレイヤーエフェクトを再設定
            if (em.mType == Magic.MagicType.Attack)
            {
				//貫通弾なら。速度0以上かつ弾丸も一秒以上生きるなら爆発とかではない
                if (em.penetration && em._moveSt.speedV > 0 && em._moveSt.lifeTime > 1)
                {
                    if (effectWait >= 1.5f)
                    {
						collisionList = null;
                    }
                }

            }
			else if (effectWait >= 3)
            {
				collisionList = null;
			}
			if(collisionList == null)
            {
				collisionList = new List<Transform>();
            }

		}

		//うるさい弾なら音を鳴らす
		if (loud)
		{
			GManager.instance.PlaySound(em.existSound, transform.position);
			loud = false;
		}

		//弾丸の生存時間終わりなら
		//あるいは追尾弾の標的消えたら
		//それか直進でいいか
		if (fireTime >= em._moveSt.lifeTime) // ((em._moveSt.fireType == Magic.FIREBULLET.HOMING || em._moveSt.fireType == Magic.FIREBULLET.HOMING) && target == null))
		{

			//   存在中の音声がなってるなら消す
			if (isExPlay)
			{
				GManager.instance.StopSound(em.existSound, 1f);
			}
            //子弾丸であるなら消える

				

				Addressables.ReleaseInstance(this.gameObject);
				Destroy(this.gameObject);
			
		}



		if (em._moveSt.fireType == Magic.FIREBULLET.STOP)
		{
			return;
		}


		// ターゲット設定
		//追尾時間以内なら追いかける
		bool homing = ((fireTime < em._moveSt.homingTime) && target != null);//((Time.fixedTime - fireTime) < em.homingTime);


		//弾丸が始動するまでの待ち時間がある場合、動かず当たり判定もなくその場にとどまる
		if (em._moveSt.waitTime > 0 && !movable)
		{
			if (col != null)
			{
				col.enabled = false;
			}
			if (fireTime >= em._moveSt.waitTime)
			{
				if (em.moveSound != null)
				{
					GManager.instance.PlaySound(em.moveSound, transform.position);
				}
				movable = true;
				col.enabled = true;
			}

		}
        else if(!movable)
        {
			movable = true;
        }


		// ホーミング処理
		//ここでジョブ呼び出し
		//movableは渡す


		if (target != null)
        {
			job.targetLost = false;
    		job.posTarget = target.transform.position;
        }
        else
        {
			job.targetLost = true;
        }
		Debug.Log($"おああああ{job.ToString()}");
		job.movable = movable;
		job.homing = homing;

		this._handler = this.job.Schedule(myTransform);
		_handler.Complete();


		if (movable)
		{
			rb.velocity = job._velocity;
		}
        else
        {
			rb.velocity = Vector2.zero;
        }



	}
	public int RandomValue(int X, int Y)
	{
		return UnityEngine.Random.Range(X, Y + 1);

	}

	/// <summary>
	/// 使用者ごとに異なる弾丸の初期化
	/// 使用者が呼び出す
	/// 子弾のためにこのスクリプト内で呼び出す
	/// </summary>
	public void InitializeNextBullet(GameObject _owner, GameObject _target, int _direction = 1)
	{

		owner = _owner.transform;
		target = _target;
		//使用者に従って
		if (_user == MasicUser.Child)
        {
			//武器の威力修正でやる？
			_owner.MMGetComponentNoAlloc<FireBullet>().BuffCalc(this);
			direction = _direction;
		}
        else
        {
			//敵、その他の場合EnemyMagicのATKをそのままぶち込む
			_owner.MMGetComponentNoAlloc<EnemyAIBase>().BuffCalc(this);
			_healTag = "Enemy";

		}


		Debug.Log("ぇ");
		//初期化完了
		_initialized = true;
	}

    void InitializePlayerBullet()
    {


		//使用者に従って
		if (_user == MasicUser.Player)
		{
			owner = GManager.instance.Player.transform;
			//武器の威力修正でやる？
			owner.gameObject.MMGetComponentNoAlloc<PlyerController>().BuffCalc(this);

		}
		else if (_user == MasicUser.Sister)
		{
			owner = SManager.instance.Sister.transform;
			target = SManager.instance.restoreTarget;
			owner.gameObject.MMGetComponentNoAlloc<FireAbility>().BuffCalc(this);
			_healTag = "Player";
		}

		em._moveSt.angle = SManager.instance.useAngle;
		job = new FireJob
		{ _status = em._moveSt };

		job.Initialize(target.transform.position, transform.position);
		_initialized = true;
	}


	/// <summary>
	/// //次のエフェクトをあらかじめロードしておく処理
	/// </summary>
	/// <returns></returns>
	async UniTask<GameObject> LoadBullet()
	{


		return await Addressables.LoadAssetAsync<GameObject>(em.hitEffect);

	}

	void HealMagic(GameObject target)
	{
		target.MMGetComponentNoAlloc<MyHealth>().Heal(em.recoverAmount);
		effectWait = 0;

	}

	/// <summary>
	/// 弾丸当たった時の処理
	/// </summary>
	void BulletHit(Collider2D other)
	{


		// オーナーチェック。あるかどうか、Nullなら戻る
		if (owner == other.gameObject.transform || isAct || other == null)
		{
			//Debug.Log($"衝突{owner == other.gameObject.transform}{isAct}{other != null}");// {gameObject.name}");
			return;
		}
		// 自分自身にヒットしないようにチェック


		/// <summary>
		/// すでに効果しているので無効
		/// </summary>
		bool alreadyEffect = false;//サポートや回復が効果を及ぼす。原則一回だけ
						   // 壁アタリをチェック

		isAct = true;

		//子弾あるなら発射
		//	bool needInitialize = sm == null ? false :true;
		if (sm != null)
		{
			sm.direction = direction;

			//next.transform.localScale.Set(0, 0,0);
			if (sm.em.bulletNumber > 1)
			{
				Transform goFire = this.gameObject.transform;
				goFire.rotation = next.transform.rotation;
				for (int x = 0; x >= sm.em.bulletNumber - 1; x++)
				{
					if (sm.em.VRandom != 0 || sm.em.HRandom != 0)
					{
						goFire.position = new Vector3(this.gameObject.transform.position.x + RandomValue(-sm.em.HRandom, sm.em.HRandom), this.gameObject.transform.position.y + RandomValue(-sm.em.VRandom, sm.em.VRandom), this.gameObject.transform.position.z);
					}

					Instantiate(next, goFire.position, goFire.rotation).MMGetComponentNoAlloc<FireBullet>().InitializeNextBullet(this.gameObject, target, direction);

				}
			}
			else
			{
				Instantiate(next, this.gameObject.transform.position, next.transform.rotation).GetComponent<FireBullet>().InitializeNextBullet(this.gameObject, target, direction);
			}

		}
		else if (!string.IsNullOrEmpty(em.hitEffect.AssetGUID))
		{

				Addressables.InstantiateAsync(em.hitEffect, this.gameObject.transform.position, transform.rotation);


		}

	

		//いろんなものと衝突してるなら
		if (collisionList.Count > 0)
        {//Debug.Log("ssssdf");
			for (int i = 0; i < collisionList.Count;i++)
            {
				//もしすでにぶつかったものと一致したら
				if(other.transform == collisionList[i])
                {
					alreadyEffect = true;
					break;
                }
            }

        }
		if (!alreadyEffect)
		{
			if (Magic.MagicType.Attack == em.mType)
			{

				direction = other.transform.position.x >= transform.position.x ? 1 : -1;

				if (em.hitSound != null)
				{
					GManager.instance.PlaySound(em.hitSound, transform.position);
				}
				if (!em.penetration)
				{
					if (isExPlay)
					{
						GManager.instance.StopSound(em.existSound, 1f);
					}
		

					if(other.gameObject.tag == SManager.instance.enemyTag || other.gameObject.tag == GManager.instance.playerTag)
                    {
                        _damage.isBreake = true;
                    }
                    else
                    {

						Addressables.ReleaseInstance(this.gameObject);
                       // Destroy(this.gameObject);
					}
					//
					//sm.em
					//next.transform.localScale = em.hitEffectScale;
				}
				else
				{
					collisionList.Add(other.transform);
					//何回貫通できるかみたいな数値入れてもいいかも

					//他のとこでは衝突で消えるでしょ
					isAct = false;
					//	Addressables.ReleaseInstance(this.gameObject);
				}
			}
			else if (other.gameObject.tag == _healTag)
			{
				collisionList.Add(other.transform);
				if (Magic.MagicType.Recover == em.mType)
				{

					HealMagic(other.gameObject);
				}
				else if (Magic.MagicType.Support == em.mType)
				{

				}
			}
			//Debug.Log($"{this.gameObject.name}が{other.transform.gameObject.name}");
		}


	}
	/// <summary>
	/// 二点間の角度を求める
	/// </summary>
	/// <param name="p1">自分の座標</param>
	/// <param name="p2">相手の座標</param>
	/// <returns></returns>
	float GetAim(Vector2 p1, Vector2 p2)
	{
		float dx = p2.x - p1.x;
		float dy = p2.y - p1.y;
		float rad = Mathf.Atan2(dy, dx);
		return rad * Mathf.Rad2Deg;
	}


	/// <summary>
	/// ダメージ計算
	/// ステータスとバフを親から取得してダメージ計算
	/// </summary>
	/// <param name="isFriend">真なら味方</param>
	public void DamageCalc()
	{
		//GManager.instance.isDamage = true;
		//useEquip.hitLimmit--;
		//mValueはモーション値

          _damage._attackData._attackType = em.attackType;

		if (em.phyAtk > 0)
		{
			_damage._attackData.phyAtk = em.phyAtk * attackFactor;

			//斬撃刺突打撃を管理
          if (em.attackType == 4)
		{


				//_damage._attackData._attackType = 4;

				//						Debug.Log("皿だ");
				if (em.shock >= 40)
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
		if (em.holyAtk > 0)
		{
			_damage._attackData.holyAtk = em.holyAtk * holyATFactor;

		}
		//闇
		if (em.darkAtk > 0)
		{
			_damage._attackData.darkAtk = em.darkAtk * darkATFactor;

		}
		//炎
		if (em.fireAtk > 0)
		{
			_damage._attackData.fireAtk = em.fireAtk * fireATFactor;

		}
		//雷
		if (em.thunderAtk > 0)
		{
			_damage._attackData.thunderAtk = em.thunderAtk * thunderATFactor;

		}
		_damage._attackData.shock = em.shock;

		_damage._attackData.mValue = em.mValue;
		_damage._attackData.attackBuff = attackBuff;
		//damage = Mathf.Floor(damage * attackBuff);

		_damage._attackData.isBlow = em.isBlow;

		_damage._attackData.blowPower.Set(em.blowPower.x, em.blowPower.y);

		

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
