using UnityEngine;
using System.Collections;
using UnityEngine.AddressableAssets;


public class SisterFireBullet : MonoBehaviour
{
	// === 外部パラメータ（インスペクタ表示） =====================

	public SisMagic em;

	[Header("標的")]
	/// <summary>
	/// 標的。狙うもの
	/// </summary>
	public GameObject targetObject;//これはシスターさんのGマネージャーから取得させる
	                               //targetObje=gmanager.kみたいなそれか最初からGManager.kみたいなのでもいい

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
			case SisMagic.FIREBULLET.ANGLE:
				speed = (ownwer.localScale.x < 0.0f) ? -em.speedV : +em.speedV;
				break;
			case SisMagic.FIREBULLET.HOMING:
				speed = em.speedV;
				homingRotate = Quaternion.LookRotation(posTarget - transform.position);
				break;
			case SisMagic.FIREBULLET.HOMING_Z:
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
			case SisMagic.FIREBULLET.ANGLE: // 指定した角度に発射
				rb.velocity = Quaternion.Euler(0.0f, 0.0f, em.angle) * new Vector3(speed, 0.0f, 0.0f);
				break;

			case SisMagic.FIREBULLET.HOMING: // 完璧にホーミング
				{
					if (homing)
					{
						homingRotate = Quaternion.LookRotation(posTarget - transform.position);
					}
					Vector3 vecMove = (homingRotate * Vector3.forward) * speed;
					rb.velocity = Quaternion.Euler(0.0f, 0.0f, em.angle) * vecMove;
				}
				break;

			case SisMagic.FIREBULLET.HOMING_Z: // 指定した角度内でホーミング
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

}
