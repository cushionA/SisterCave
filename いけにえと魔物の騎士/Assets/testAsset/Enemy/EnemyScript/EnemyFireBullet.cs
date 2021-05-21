using UnityEngine;
using System.Collections;
using UnityEngine.AddressableAssets;


public class EnemyFireBullet : MonoBehaviour
{


	// === 外部パラメータ（インスペクタ表示） =====================

	public EnemyMagic em;

	[Header("標的")]
	/// <summary>
	/// 標的。狙うもの
	/// </summary>
	public GameObject targetObject;


	// === 外部パラメータ ======================================
	[System.NonSerialized] public Transform ownwer;
	[System.NonSerialized] public bool attackEnabled;

	// === 内部パラメータ ======================================
	Rigidbody2D rb;
	float fireTime;
	Vector3 posTarget;
	float homingAngle;
	Quaternion homingRotate;
	float speed;

	// === コード（Monobehaviour基本機能の実装） ================
	void Start()
	{
		targetObject = GameObject.Find("Player");
		rb = GetComponent<Rigidbody2D>();
		// オーナーチェック
		if (!ownwer)
		{
			return;
		}

		// 初期化
		posTarget = targetObject.transform.position + new Vector3(0.0f, 1.0f, 0.0f);

		switch (em.fireType)
		{
			case EnemyMagic.FIREBULLET.ANGLE:
				speed = (ownwer.localScale.x < 0.0f) ? -em.speedV : +em.speedV;
				break;
			case EnemyMagic.FIREBULLET.HOMING:
				speed = em.speedV;
				homingRotate = Quaternion.LookRotation(posTarget - transform.position);
				break;
			case EnemyMagic.FIREBULLET.HOMING_Z:
				speed = em.speedV;
				break;
		}

		fireTime = Time.fixedTime;
		homingAngle = em.angle;
		attackEnabled = true;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		// オーナーチェック。あるかどうか、Nullなら戻る
		if (!ownwer)
		{
			return;
		}
		// 自分自身にヒットしないようにチェック

		if (other.tag == "Player")
		{
			Attack();

		}
		if (other.tag == "Guard")
		{
			Guard();

		}

		// アタリをチェック
		if (!em.penetration)
		{
			GameObject next = Addressables.InstantiateAsync(em.hitEffect, this.gameObject.transform.position, Quaternion.identity).Result;//次のエフェクトを
			next.transform.localScale = em.hitEffectScale;
			Addressables.ReleaseInstance(this.gameObject);
		}

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
		// ターゲット設定
		bool homing = ((Time.fixedTime - fireTime) < em.homingTime);
		if (homing)
		{
			posTarget = targetObject.transform.position + new Vector3(0.0f, 1.0f, 0.0f);
		}
		if (fireTime >= em.lifeTime)
		{
			Addressables.ReleaseInstance(this.gameObject);
		}

		// ホーミング処理
		switch (em.fireType)
		{
			case EnemyMagic.FIREBULLET.ANGLE: // 指定した角度に発射
				rb.velocity = Quaternion.Euler(0.0f, 0.0f, em.angle) * new Vector3(speed, 0.0f, 0.0f);
				break;

			case EnemyMagic.FIREBULLET.HOMING: // 完璧にホーミング
				{
					if (homing)
					{
						homingRotate = Quaternion.LookRotation(posTarget - transform.position);
					}
					Vector3 vecMove = (homingRotate * Vector3.forward) * speed;
					rb.velocity = Quaternion.Euler(0.0f, 0.0f, em.angle) * vecMove;
				}
				break;

			case EnemyMagic.FIREBULLET.HOMING_Z: // 指定した角度内でホーミング
				if (homing)
				{
					float targetAngle = Mathf.Atan2(posTarget.y - transform.position.y,
														posTarget.x - transform.position.x) * Mathf.Rad2Deg;
					float deltaAngle = Mathf.DeltaAngle(targetAngle, homingAngle);
					float deltaHomingAngle = em.homingAngleV * Time.fixedDeltaTime;
					if (Mathf.Abs(deltaAngle) >= deltaHomingAngle)
					{
						homingAngle += (deltaAngle < 0.0f) ? +deltaHomingAngle : -deltaHomingAngle;
					}
					em.homingAngleV += (em.homingAngleA * Time.fixedDeltaTime);
					homingRotate = Quaternion.Euler(0.0f, 0.0f, homingAngle);
				}
				rb.velocity = (homingRotate * Vector3.right) * speed;
				break;
		}

		// 加速度計算
		speed += em.speedA * Time.fixedDeltaTime;

		// スケール計算
		transform.localScale += em.bulletScaleV;
		em.bulletScaleV += em.bulletScaleA * Time.fixedDeltaTime;
		if (transform.localScale.x < 0.0f || transform.localScale.y < 0.0f || transform.localScale.z < 0.0f)
		{
			Addressables.ReleaseInstance(this.gameObject);
		}
	}

	void Attack()
	{
		if (em.hitLimmit > 0)
		{
			em.hitLimmit--;

			//mValueはモーション値
			float damage = 0;//バフデバフ処理用にdamageとして保持する
			if (em.phyAtk > 0)
			{
				//斬撃刺突打撃を管理
				if (em.atType == EnemyMagic.AttackType.Slash)
				{ damage += (Mathf.Pow(em.phyAtk, 2) * em.mValue) / (em.phyAtk + GManager.instance.pStatus.Def); }
				else if (em.atType == EnemyMagic.AttackType.Stab)
				{ damage += (Mathf.Pow(em.phyAtk, 2) * em.mValue) / (em.phyAtk + GManager.instance.pStatus.pierDef); }
				else if (em.atType == EnemyMagic.AttackType.Strike)
				{ damage += (Mathf.Pow(em.phyAtk, 2) * em.mValue) / (em.phyAtk + GManager.instance.pStatus.strDef); }
			}
			//神聖
			if (em.holyAtk > 0)
			{
				damage += (Mathf.Pow(em.holyAtk, 2) * em.mValue) / (em.holyAtk + GManager.instance.pStatus.holyDef);
			}
			//闇
			if (em.darkAtk > 0)
			{
				damage += (Mathf.Pow(em.darkAtk, 2) * em.mValue) / (em.darkAtk + GManager.instance.pStatus.darkDef);
			}
			//炎
			if (em.fireAtk > 0)
			{
				damage += (Mathf.Pow(em.fireAtk, 2) * em.mValue) / (em.fireAtk + GManager.instance.pStatus.fireDef);
			}
			//雷
			if (em.thunderAtk > 0)
			{
				damage += (Mathf.Pow(em.thunderAtk, 2) * em.mValue) / (em.thunderAtk + GManager.instance.pStatus.thunderDef);
			}
			if (!GManager.instance.isAttack)
			{
				GManager.instance.nowArmor -= em.Shock;
			}
			else
			{
				GManager.instance.nowArmor -= (em.Shock - GManager.instance.pStatus.equipWeapon.atAromor) < 0 ? 0 : (em.Shock - GManager.instance.pStatus.equipWeapon.atAromor);
			}
			damage = Mathf.Floor(damage * em.attackBuff);
			GManager.instance.pStatus.hp -= damage;
		}
		if(em.isBlow == true && GManager.instance.nowArmor <= 0)
        {
			GManager.instance.blowDown = true;

			if(rb.velocity.x > 0)
            {
				GManager.instance.blowVector = em.blowVector;

			}
			if (rb.velocity.x < 0)
			{
				GManager.instance.blowVector = new Vector2(-em.blowVector.x,em.blowVector.y);

			}
		}
	}
	void Guard()
    {
			if (em.hitLimmit > 0 && GManager.instance.isGuard)
			{
				em.hitLimmit--;
				//mValueはモーション値
				float damage = 0;//バフデバフ処理用にdamageとして保持する
				if (em.phyAtk > 0)
				{
					//斬撃刺突打撃を管理
					if (em.atType == EnemyMagic.AttackType.Slash)
					{ damage += (Mathf.Pow(em.phyAtk, 2) * em.mValue) / (em.phyAtk + GManager.instance.pStatus.Def); }
					else if (em.atType == EnemyMagic.AttackType.Stab)
					{ damage += (Mathf.Pow(em.phyAtk, 2) * em.mValue) / (em.phyAtk + GManager.instance.pStatus.pierDef); }
					else if (em.atType == EnemyMagic.AttackType.Strike)
					{ damage += (Mathf.Pow(em.phyAtk, 2) * em.mValue) / (em.phyAtk + GManager.instance.pStatus.strDef); }

					damage *= (100 - GManager.instance.pStatus.phyCut) / 100;
				}
				//神聖
				if (em.holyAtk > 0)
				{
					damage += ((Mathf.Pow(em.holyAtk, 2) * em.mValue) / (em.holyAtk + GManager.instance.pStatus.holyDef)) * (100 - GManager.instance.pStatus.holyCut) / 100;
				}
				//闇
				if (em.darkAtk > 0)
				{
					damage += ((Mathf.Pow(em.darkAtk, 2) * em.mValue) / (em.darkAtk + GManager.instance.pStatus.darkDef)) * (100 - GManager.instance.pStatus.darkCut) / 100;
				}
				//炎
				if (em.fireAtk > 0)
				{
					damage += ((Mathf.Pow(em.fireAtk, 2) * em.mValue) / (em.fireAtk + GManager.instance.pStatus.fireDef)) * (100 - GManager.instance.pStatus.fireCut) / 100;
				}
				//雷
				if (em.thunderAtk > 0)
				{
					damage += ((Mathf.Pow(em.thunderAtk, 2) * em.mValue) / (em.thunderAtk + GManager.instance.pStatus.thunderDef)) * (100 - GManager.instance.pStatus.thunderCut) / 100;
				}

				GManager.instance.pStatus.stamina -= (em.Shock * 2) * ((100 - GManager.instance.pStatus.guardPower) / 100);
			

				GManager.instance.pStatus.hp -= damage * em.attackBuff;
			}
		if (GManager.instance.pStatus.stamina <= 0)
		{
			if (em.isBlow == true)
			{
				GManager.instance.isDown = true;
				GManager.instance.blowDown = true;
				GManager.instance.isGuard = false;
				if (transform.localScale.x > 0)
				{
					GManager.instance.blowVector = em.blowVector;

				}
				if (transform.localScale.x < 0)
				{
					GManager.instance.blowVector = new Vector2(-em.blowVector.x, em.blowVector.y);

				}
			}
			else
			{
				GManager.instance.isGBreak = true;
				GManager.instance.isGuard = false;
			}
		}

	}

}
