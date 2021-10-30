using UnityEngine;
using System.Collections;
using UnityEngine.AddressableAssets;


public class PlayerFireBullet : MonoBehaviour
{
	// === 外部パラメータ（インスペクタ表示） =====================

	public PlayerMagic em;

	[Header("標的")]
	/// <summary>
	/// 標的。狙うもの
	/// </summary>
	public GameObject targetObject;//常に一番近いやつを

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
		// オーナーチェック
		if (!ownwer)
		{
			return;
		}

		// 初期化
		posTarget = targetObject.transform.position + new Vector3(0.0f, 1.0f, 0.0f);

		switch (em.fireType)
		{
			case PlayerMagic.FIREBULLET.ANGLE:
				speed = (ownwer.localScale.x < 0.0f) ? -em.speedV : +em.speedV;
				break;
			case PlayerMagic.FIREBULLET.HOMING:
				speed = em.speedV;
				homingRotate = Quaternion.LookRotation(posTarget - transform.position);
				break;
			case PlayerMagic.FIREBULLET.HOMING_Z:
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


		// 壁アタリをチェック
		if (!em.penetration)
		{
			GameObject next = Addressables.InstantiateAsync(em.hitEffect, this.gameObject.transform.position, Quaternion.identity).Result;//次のエフェクトを
			//next.transform.localScale = em.hitEffectScale;
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
			case PlayerMagic.FIREBULLET.ANGLE: // 指定した角度に発射
				rb.velocity = Quaternion.Euler(0.0f, 0.0f, em.angle) * new Vector3(speed, 0.0f, 0.0f);
				break;

			case PlayerMagic.FIREBULLET.HOMING: // 完璧にホーミング
				{
					if (homing)
					{
						homingRotate = Quaternion.LookRotation(posTarget - transform.position);
					}
					Vector3 vecMove = (homingRotate * Vector3.forward) * speed;
					rb.velocity = Quaternion.Euler(0.0f, 0.0f, em.angle) * vecMove;
				}
				break;

			case PlayerMagic.FIREBULLET.HOMING_Z: // 指定した角度内でホーミング
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
		//transform.localScale += em.bulletScaleV;
		em.bulletScaleV += em.bulletScaleA * Time.fixedDeltaTime;
		if (transform.localScale.x < 0.0f || transform.localScale.y < 0.0f || transform.localScale.z < 0.0f)
		{
			Addressables.ReleaseInstance(this.gameObject);
		}
	}
/*	public void SetAtk()
	{

		if (em.phyBase >= 1)
		{
			em.phyAtk = em.phyBase + (em.powerCurve.Evaluate(GManager.instance.power)) +
							   em.skillCurve.Evaluate(GManager.instance.skill) +
							   em.intCurve.Evaluate(GManager.instance._int) * GManager.instance.equipWeapon.MagicAssist;//	物理の賢さ補正は低くする
		}
		if (em.holyBase >= 1)
		{
			em.holyAtk = em.holyBase + (em.powerCurve.Evaluate(GManager.instance.power)) +
							   em.intCurve.Evaluate(GManager.instance._int) * GManager.instance.equipWeapon.MagicAssist;
		}
		if (em.darkBase >= 1)
		{
			em.darkAtk = em.darkBase + (em.intCurve.Evaluate(GManager.instance._int)) *GManager.instance.equipWeapon.MagicAssist +
							   em.skillCurve.Evaluate(GManager.instance.skill);
		}
		if (em.fireBase >= 1)
		{
			em.fireAtk = (em.fireBase + em.intCurve.Evaluate(GManager.instance._int))
				* GManager.instance.equipWeapon.MagicAssist;
		}
		if (em.thunderBase >= 1)
		{
			em.thunderAtk = em.thunderBase + em.intCurve.Evaluate(GManager.instance._int) * GManager.instance.equipWeapon.MagicAssist;
		}


	}//攻撃力設定*/
}
