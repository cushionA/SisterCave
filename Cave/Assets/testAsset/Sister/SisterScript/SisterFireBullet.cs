using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;


public class SisterFireBullet : MonoBehaviour
{
	// === 外部パラメータ（インスペクタ表示） =====================

	public SisMagic em;

	[Header("標的")]
	/// <summary>
	/// 標的。狙うもの
	/// </summary>


	// === 外部パラメータ ======================================
	[System.NonSerialized] Transform ownwer;
	[System.NonSerialized] public bool attackEnabled;

	// === 内部パラメータ ======================================
	[SerializeField] Rigidbody2D rb;
	float fireTime;
	[HideInInspector]public GameObject target;
	Vector3 posTarget;
	float homingAngle;
	Quaternion homingRotate;
	float speed;
	float homingRange;
	bool loud;//常に音が鳴るかどうか
	bool isAct;
//	bool hitSound;
	bool isExPlay;//存在音ならしたか
	GameObject next;
	SisterFireBullet sm;
	[HideInInspector]public int direction;
	Vector2 bulletScale;//弾丸の拡大率
	bool playerEffect;//サポートや回復が効果を及ぼす
	float effectWait;//サポートや回復が再度効果を現すまで

	/// <summary>
	/// 何秒待ったかなー
	/// </summary>
	float waitNow;
	Collider2D col;
    bool movable;
	// === コード（Monobehaviour基本機能の実装） ================
	async UniTaskVoid Start()
	{
		ownwer = SManager.instance.Sister.transform;
		fireTime = 0;

		col = GetComponent<Collider2D>();
		//em = SManager.instance.sisStatus.useMagic;

		// オーナーチェック
		//		if (!ownwer)
		//	{
		//return;
		//}
		//子供の弾丸は親のターゲット引き継ぐ
		if (em.fireType != SisMagic.FIREBULLET.STOP || !em.isChild)
		{
		//	Debug.Log($"標的設定{SManager.instance.target.name}");
			target = SManager.instance.target;
            direction = (ownwer.localScale.x < 0.0f) ? -1 : 1;
		//	Debug.Log($"最初の名前{this.gameObject.name}標的{target.name}");
		}



		//Debug.Log($"標的{gameObject.name}");
		// 初期化
		if (em.fireType != SisMagic.FIREBULLET.STOP)
		{
			posTarget = target.transform.position + new Vector3(0.0f, 1.0f, 0.0f);
		}
		switch (em.fireType)
		{


			case SisMagic.FIREBULLET.ANGLE:
				speed = (ownwer.localScale.x < 0.0f) ? -em.speedV : +em.speedV;

				break;
			case SisMagic.FIREBULLET.HOMING:
				speed = em.speedV;
				homingRotate = Quaternion.LookRotation(posTarget - transform.position);
			//	transform.localScale = 

				break;
			case SisMagic.FIREBULLET.HOMING_Z:
				speed = em.speedV;
				homingRotate = Quaternion.LookRotation(posTarget - transform.position);
				homingRange = em.homingAngleV;
				homingAngle = transform.position.x < target.transform.position.x ? em.angle : em.angle + (180 - em.angle);
				break;
			case SisMagic.FIREBULLET.RAIN:
				
				speed = em.speedV;
				//em.angle = 
				break;
		}

	   if(em.existSound != null)
        {
			loud = true;
			isExPlay = true;
        }	

		
		attackEnabled = true;
		bulletScale = em.bulletScaleV;
		GManager.instance.PlaySound(em.fireSound, transform.position);

		//	hitSound = em.hitSound != null ? true : false;
		if (em.isHit)
		{
			next = await LoadBullet();
		//	Debug.Log($"キマシタワー{next.transform.rotation.y}");
			//Debug.Log("キマシタワー1");
			//		next = Addressables.LoadAssetAsync<GameObject>(em.hitEffect).Result;//次のエフェクトを
			//	Debug.Log("キマシタワー1");
			//sm = next.GetComponent<SisterFireBullet>();
			//	Debug.Log($"名前{gameObject.name}");
			//sm.target = target;
			sm = next.GetComponent<SisterFireBullet>();
			//Debug.Log($"名前{target.name}");
			sm.target = target;
		}


		if (em.isChild)
		{
			Vector3 setScale = transform.localScale;
			if (direction < 0)
			{
				setScale.x = -setScale.x;
			}
			transform.localScale = setScale;
			//Debug.Log($"アリゲーター{target.name}");
		}
	}




	void OnTriggerEnter2D(Collider2D other)
	{
		BulletHit(other);
	}
	void OnTriggerStay2D(Collider2D other)
	{
		BulletHit(other);
	}

	void Update()
	{
		if (em.isRotate)
	    {
			// スプライト画像の回転処理
			transform.Rotate(0.0f, 0.0f, Time.deltaTime * em.rotateVt);
		}
	}

     void FixedUpdate()
	{

		//Debug.Log($"名前{this.gameObject.name}標的{target.name}");nnbutu

		fireTime += Time.fixedDeltaTime;
        if (playerEffect)
        {
			effectWait += Time.fixedDeltaTime;
			//三秒以上ならプレイヤーエフェクトを再設定
			playerEffect = effectWait >= 3.0f ? false : true;

        }
        if (loud)
        {
			GManager.instance.PlaySound(em.existSound, transform.position);
			loud = false;
		}
		if (fireTime >= em.lifeTime)
		{
        //    Debug.Log($"弾丸時間{fireTime}設定時間{em.lifeTime}");
            if (isExPlay)
            {
				GManager.instance.StopSound(em.existSound, 1f);
            }
			if (em.isChild)
			{
				
				Destroy(this.gameObject);
			}
			else
			{
				Addressables.ReleaseInstance(this.gameObject);
			}
		}

		if (rb.velocity.x > 0f)
		{
			Vector3 theScale = transform.localScale;
			theScale.x = Mathf.Abs(theScale.x);
			transform.localScale = theScale;
		}
        else if (rb.velocity.x < 0f)
		{
			Vector3 theScale = transform.localScale;
			theScale.x = -1 * Mathf.Abs(theScale.x);
			transform.localScale = theScale;

		}

        if (em.fireType == SisMagic.FIREBULLET.STOP)
        {
			return;
        }


		// ターゲット設定
		bool homing = ((fireTime) < em.homingTime);//((Time.fixedTime - fireTime) < em.homingTime);
		if (homing)
		{
			//Debug.Log($"あああああ設定{target.name}");
			posTarget = target.transform.position + Vector3.up;
		}
		
		if(em.waitTime > 0 && !movable)
        {
			col.enabled = false;
			//movable = false;
			waitNow += Time.fixedDeltaTime;
            if (waitNow >= em.waitTime)
            {
				if(em.moveSound != null)
                {
					GManager.instance.PlaySound(em.moveSound,transform.position);
                }
				movable = true;
				col.enabled = true;
				waitNow = 0;
			}

        }
        else if(em.waitTime == 0)
        {
			movable = true;
        }

		// ホーミング処理
		switch (em.fireType)
		{
			case SisMagic.FIREBULLET.ANGLE: // 指定した角度に発射
				if (movable)
				{
					rb.velocity = Quaternion.Euler(0.0f, 0.0f, em.angle) * new Vector3(speed, 0.0f, 0.0f);
				}
				break;

			case SisMagic.FIREBULLET.RAIN: // 指定した角度に発射
				if (movable)
				{
					rb.velocity = Quaternion.Euler(0.0f, 0.0f, em.angle) * new Vector3(speed, 0.0f, 0.0f);
				}
				break;

			case SisMagic.FIREBULLET.HOMING: // 完璧にホーミング
				{
					//WaitTimeとHormingTimeを同じかそれ以下にすれば停止中だけ敵を狙うやつになる
					if (homing)
					{
						homingRotate = Quaternion.LookRotation(posTarget - transform.position);
						//transform.rotation = 
						transform.rotation = Quaternion.Euler(new Vector3(0,0,(Quaternion.FromToRotation(Vector3.up, posTarget - transform.position).eulerAngles.z - 90)));
						
						// Quaternion.Euler(new Vector3(0,0,Mathf.Atan2(posTarget.x - transform.position.x, posTarget.y - transform.position.y)));
					//	Debug.Log($"アイイイ{Quaternion.FromToRotation(Vector3.up, posTarget - transform.position).eulerAngles.z}");
					}

					// 対象物へ回転する

					//transform.rotation =
					
					Vector3 vecMove = (homingRotate * Vector3.forward) * speed;
					if (movable)
					{
						rb.velocity = Quaternion.Euler(0.0f, 0.0f, em.angle) * vecMove;
					}
				}
				break;

			case SisMagic.FIREBULLET.HOMING_Z: // 指定した角度内でホーミング
				if (homing)
				{
					//敵と弾丸の角度を求めて度に変換
					float targetAngle = Mathf.Atan2(posTarget.y - transform.position.y,
														posTarget.x - transform.position.x) * Mathf.Rad2Deg;

					float deltaAngle = Mathf.DeltaAngle(targetAngle, homingAngle);
					float deltaHomingAngle = homingRange * Time.fixedDeltaTime;
					if (Mathf.Abs(deltaAngle) >= deltaHomingAngle)
					{
						homingAngle += (deltaAngle < 0.0f) ? +deltaHomingAngle : -deltaHomingAngle;
					}
					homingRange += (em.homingAngleA * Time.fixedDeltaTime);
					homingRotate = Quaternion.Euler(0.0f, 0.0f, homingAngle);
				}
				if (movable)
				{
					rb.velocity = (homingRotate * Vector3.right) * speed;
				}
				break;
		}

        if (!movable)
        {
			rb.velocity = Vector3.zero;

		}
			speed += em.speedA * Time.fixedDeltaTime;


		// スケール計算
		//どんどん大きくなったり小さくなったり
		//小さくなりすぎたら破壊する
		if (bulletScale != Vector2.zero)
		{
			Vector2 changeScale = new Vector2();
			changeScale.Set(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
			changeScale.x = (bulletScale.x + changeScale.x) > 0 ? (bulletScale.x + changeScale.x) : changeScale.x;
			changeScale.y = (bulletScale.y + changeScale.y) > 0 ? (bulletScale.y + changeScale.y) : changeScale.y;


			changeScale.x = transform.localScale.x >= 0 ? changeScale.x : -changeScale.x;
			transform.localScale = changeScale;
		}
        bulletScale += em.bulletScaleA * Time.fixedDeltaTime;
/*		if (transform.localScale.x < 0.0f || transform.localScale.y < 0.0f || transform.localScale.z < 0.0f)
		{
			if(isExPlay)
			{
				GManager.instance.StopSound(em.existSound, 1f);
			}
			Addressables.ReleaseInstance(this.gameObject);
		//	Debug.Log("スケール");
		}*/

	}
	public int RandomValue(int X, int Y)
	{
		return Random.Range(X, Y + 1);

	}

/*	async UniTaskVoid MyInstantiate(object key, Vector3 firePosi, Quaternion rotate)
	{
		await Addressables.InstantiateAsync(key, firePosi, rotate);
		try 
		{
			var result = await Addressables.InstantiateAsync(key,firePosi,rotate);

		}
        catch
        {

        }
	}*/


	void Damage(EnemyBase enemy)
    {
		if(enemy.lastHit != null && !enemy.isHitable)
        {
			enemy.isHitable = enemy.lastHit != this.gameObject ? true : false;

		}

		if (enemy.isHitable)
		{

			//EnemyBase enemy = other.GetComponent<EnemyBase>();
			enemy.sm = em;

		//		Debug.Log($"お肉焼けたね{em.name}");

			
			enemy.lastHit = this.gameObject;
			enemy.bulletDirection = direction;
            enemy.insert = true;
		}

	}

	async UniTask<GameObject> LoadBullet()
    {


             return await  Addressables.LoadAssetAsync<GameObject>(em.hitEffect);//次のエフェクトを
		
	}

	void HealMagic()
    {
		if (!playerEffect)
		{
	//		Debug.Log("いああ");
			GManager.instance.HpRecover(em.recoverAmount);
			playerEffect = true;
			effectWait = 0;
		}
    }

	/// <summary>
	/// 弾丸当たった時の処理
	/// </summary>
	void BulletHit(Collider2D other)
    {
		//	Debug.Log("あるの");

		// オーナーチェック。あるかどうか、Nullなら戻る
		if (ownwer == other.gameObject.transform || isAct)
		{
			Debug.Log($"衝突{gameObject.name}");
			return;
		}
		// 自分自身にヒットしないようにチェック

		// 壁アタリをチェック

		isAct = true;

		if (em.isHit)
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

					//GameObject add = Addressables.
					Instantiate(next, goFire.position, goFire.rotation);//次のエフェクトを
                                                                        //add.GetComponent<SisterFireBullet>().target = target;
                    if (x == sm.em.bulletNumber - 2)
                    {
						Addressables.Release(next);

					}
				}
			}
			else
			{
				//Addressables.
				Instantiate(next, this.gameObject.transform.position, next.transform.rotation);
				Addressables.Release(next);
			}
		}
		if (other.gameObject.tag == SManager.instance.enemyTag)
		{

			direction = other.transform.position.x >= transform.position.x ? 1 : -1;

			if (em.hitSound != null)
			{
				GManager.instance.PlaySound(em.hitSound, transform.position);
			}
			if (!em.penetration && other != null)
			{

				Damage(other.GetComponent<EnemyBase>());
				if (isExPlay)
				{
					GManager.instance.StopSound(em.existSound, 1f);
				}
				Addressables.ReleaseInstance(this.gameObject);

				//sm.em
				//next.transform.localScale = em.hitEffectScale;
			}
			else
			{

				//何回貫通できるかみたいな数値入れてもいいかも
				Damage(other.GetComponent<EnemyBase>());
				//他のとこでは衝突で消えるでしょ
				isAct = false;
				//	Addressables.ReleaseInstance(this.gameObject);
			}
		}
		else if (other.gameObject.tag == GManager.instance.playerTag)
		{
			if (SisMagic.MagicType.Recover == em.mType)
			{
				HealMagic();
			}
		}
		else
		{

			if (em.fireType != SisMagic.FIREBULLET.STOP)
			{
				GManager.instance.PlaySound(em.hitSound, transform.position);
			}
			if (!em.penetration)
			{
				Addressables.ReleaseInstance(this.gameObject);

				if (isExPlay)
				{
					GManager.instance.StopSound(em.existSound, 1f);
				}
			}
			isAct = false;
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
}
