using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using System;
using MoreMountains.CorgiEngine;
public class FireBullet : MonoBehaviour
{

	/// <summary>
	/// ���@�̎g�p�҂��N��
	/// �_���[�W�����Ƃ��Ŏg������
	/// �����������ł��g�����A�g�p�҂̍U���{���Ƃ������Ȃ��Ƃ����Ȃ���
	/// </summary>
	[Header("���@�̎g�p�҂��N��")]
	[SerializeField]
	MasicUser _user;


	MyDamageOntouch _damage;

	public enum MasicUser
    {
		Player,
		Sister,
		Others
    }

	// === �O���p�����[�^�i�C���X�y�N�^�\���j =====================

	public Magic em;

	[Header("�W�I")]
	/// <summary>
	/// �W�I�B�_������
	/// </summary>


	// === �O���p�����[�^ ======================================
	[System.NonSerialized] Transform owner;
	/// <summary>
	/// �悭�킩��񑶍�
	/// </summary>
	[System.NonSerialized] public bool attackEnabled;

	// === �����p�����[�^ ======================================
	[SerializeField] Rigidbody2D rb;
	/// <summary>
	/// �e�ۂ��ǂꂭ�炢�̎��ԑ��݂��Ă邩�v������B
	/// </summary>
	float fireTime;
	[HideInInspector] public GameObject target;
	Vector3 posTarget;
	float homingAngle;
	Quaternion homingRotate;
	float speed;
	float homingRange;
	/// <summary>
	/// ���݉��Ȃ炵����
	/// ����炷�̂Ɏg��
	/// </summary>
	bool loud;//��ɉ����邩�ǂ���
	bool isAct;
	//	bool hitSound;
	/// <summary>
	/// ���݉��Ȃ炵����
	/// ���������̂Ɏg��
	/// </summary>
	bool isExPlay;
	GameObject next;
	//���ɏo���e�ۂ̓���X�N���v�g
	FireBullet sm;
	/// <summary>
	/// �L�������E�ƍ��ǂ��炩��e����������
	/// </summary>
	[HideInInspector] public int direction;
	Vector2 bulletScale;//�e�ۂ̊g�嗦
	bool playerEffect;//�T�|�[�g��񕜂����ʂ��y�ڂ�
	float effectWait;//�T�|�[�g��񕜂��ēx���ʂ������܂�

	/// <summary>
	/// �e�ۂɐ�����̑҂����Ԃ�����ꍇ�ɉ��b�҂�����������
	/// WaitTime��HormingTime�𓯂�������ȉ��ɂ���Β�~�������G��_����ɂȂ�
	/// </summary>
	float waitNow;
	Collider2D col;
	/// <summary>
	/// �����e�ۂ������Ȃ��҂����Ԃ�����Ȃ�~�߂�t���O
	/// </summary>
	bool movable;

	bool _initialized;

	AttackData _atk;

	/// <summary>
	/// �U���{��
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

	protected float attackBuff = 1;//�U���{��

	// === �R�[�h�iMonobehaviour��{�@�\�̎����j ================
	async UniTaskVoid Start()
	{
		Func<bool> s = () => 
		{
			//�������������������ǂ������͂��邾���̏���
			//�����������͔��ˑ������
			return _initialized;
		};

		//�������������^��Ԃ��܂ő҂�
		await UniTask.WaitUntil(s);
			

		//ownner�͒e�ێg�p���Őݒ�
		fireTime = 0;

		col = GetComponent<Collider2D>();


		//�q���̒e�ۂ͐e�̃^�[�Q�b�g�����p��
		if (em.fireType != Magic.FIREBULLET.STOP || !em.isChild)
		{
			//	Debug.Log($"�W�I�ݒ�{SManager.instance.target.name}");
			
			//
			direction = (owner.localScale.x < 0.0f) ? -1 : 1;
		}



		//Debug.Log($"�W�I{gameObject.name}");
		// ������
		if (em.fireType != Magic.FIREBULLET.STOP)
		{
			posTarget = target.transform.position + new Vector3(0.0f, 1.0f, 0.0f);
		}
		switch (em.fireType)
		{


			case Magic.FIREBULLET.ANGLE:
				speed = (direction == -1) ? -em.speedV : +em.speedV;

				break;
			case Magic.FIREBULLET.HOMING:
				speed = em.speedV;
				homingRotate = Quaternion.LookRotation(posTarget - transform.position);
				//	transform.localScale = 

				break;
			case Magic.FIREBULLET.HOMING_Z:
				speed = em.speedV;
				homingRotate = Quaternion.LookRotation(posTarget - transform.position);
				homingRange = em.homingAngleV;
				homingAngle = transform.position.x < target.transform.position.x ? em.angle : em.angle + (180 - em.angle);
				break;
			case Magic.FIREBULLET.RAIN:

				speed = em.speedV;
				//em.angle = 
				break;
		}
		// ���݂��Ă�Ԃ̃T�E���h������Ȃ�
		if (em.existSound != null)
		{
			loud = true;
			isExPlay = true;
		}


		attackEnabled = true;
		bulletScale = em.bulletScaleV;
		GManager.instance.PlaySound(em.fireSound, transform.position);

		//	hitSound = em.hitSound != null ? true : false;
		
		//�q�b�g���ɑ��̒e�ۂ��o���Ȃ�
		if (em.isHit)
		{
			next = await LoadBullet();
			sm = next.GetComponent<FireBullet>();
			//Debug.Log($"���O{target.name}");
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
			//Debug.Log($"�A���Q�[�^�[{target.name}");
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
			// �X�v���C�g�摜�̉�]����
			transform.Rotate(0.0f, 0.0f, Time.deltaTime * em.rotateVt);
		}
	}

	void FixedUpdate()
	{

        //Debug.Log($"���O{this.gameObject.name}�W�I{target.name}");nnbutu
		//���������ĂȂ��Ȃ�߂�܂��傤
        if (!_initialized)
        {
			return;
        }

		fireTime += Time.fixedDeltaTime;
		if (playerEffect)
		{
			effectWait += Time.fixedDeltaTime;
			//�O�b�ȏ�Ȃ�v���C���[�G�t�F�N�g���Đݒ�
			playerEffect = effectWait >= 3.0f ? false : true;

		}
		//���邳���e�Ȃ特��炷
		if (loud)
		{
			GManager.instance.PlaySound(em.existSound, transform.position);
			loud = false;
		}
		//�e�ۂ̐������ԏI���Ȃ�
		if (fireTime >= em.lifeTime)
		{
			//   ���ݒ��̉������Ȃ��Ă�Ȃ����
			if (isExPlay)
			{
				GManager.instance.StopSound(em.existSound, 1f);
			}
			//�q�e�ۂł���Ȃ������
			if (em.isChild)
			{

				Destroy(this.gameObject);
			}
			else
			{
				Addressables.ReleaseInstance(this.gameObject);
			}
		}

		//�e�ۂ����E���x�ɉ����ĐU���������
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

		if (em.fireType == Magic.FIREBULLET.STOP)
		{
			return;
		}


		// �^�[�Q�b�g�ݒ�
		//�ǔ����Ԉȓ��Ȃ�ǂ�������
		bool homing = ((fireTime) < em.homingTime);//((Time.fixedTime - fireTime) < em.homingTime);
		if (homing)
		{

			posTarget = target.transform.position + Vector3.up;
		}

		//�e�ۂ��n������܂ł̑҂����Ԃ�����ꍇ�A�����������蔻����Ȃ����̏�ɂƂǂ܂�
		if (em.waitTime > 0 && !movable)
		{
			col.enabled = false;
			waitNow += Time.fixedDeltaTime;
			if (waitNow >= em.waitTime)
			{
				if (em.moveSound != null)
				{
					GManager.instance.PlaySound(em.moveSound, transform.position);
				}
				movable = true;
				col.enabled = true;
				waitNow = 0;
			}

		}
		else if (!movable)
		{
			movable = true;
		}

		// �z�[�~���O����
		switch (em.fireType)
		{
			case Magic.FIREBULLET.ANGLE: // �w�肵���p�x�ɔ���
				if (movable)
				{
					//�i�s�p�x�ɏ]���đ��x��ω�������
					rb.velocity = Quaternion.Euler(0.0f, 0.0f, em.angle) * new Vector3(speed, 0.0f, 0.0f);
				}
				break;

			case Magic.FIREBULLET.RAIN: // �w�肵���p�x�ɔ���
				if (movable)
				{   
					//�i�s�p�x�ɏ]���đ��x��ω�������
					rb.velocity = Quaternion.Euler(0.0f, 0.0f, em.angle) * new Vector3(speed, 0.0f, 0.0f);
				}
				break;

			case Magic.FIREBULLET.HOMING: // �����Ƀz�[�~���O
				{
					//WaitTime��HormingTime�𓯂�������ȉ��ɂ���Β�~�������G��_����ɂȂ�
					if (homing)
					{
						homingRotate = Quaternion.LookRotation(posTarget - transform.position);
						//transform.rotation = 
						transform.rotation = Quaternion.Euler(new Vector3(0, 0, (Quaternion.FromToRotation(Vector3.up, posTarget - transform.position).eulerAngles.z - 90)));

						// Quaternion.Euler(new Vector3(0,0,Mathf.Atan2(posTarget.x - transform.position.x, posTarget.y - transform.position.y)));
						//	Debug.Log($"�A�C�C�C{Quaternion.FromToRotation(Vector3.up, posTarget - transform.position).eulerAngles.z}");
					}

					// �Ώە��։�]����

					//transform.rotation =

					Vector3 vecMove = (homingRotate * Vector3.forward) * speed;
					if (movable)
					{
						rb.velocity = Quaternion.Euler(0.0f, 0.0f, em.angle) * vecMove;
					}
				}
				break;

			case Magic.FIREBULLET.HOMING_Z: // �w�肵���p�x���Ńz�[�~���O
				if (homing)
				{
					//�G�ƒe�ۂ̊p�x�����߂ēx�ɕϊ�
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
				//�������Ȃ��ꍇ�͊p�x�����ύX
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


		// �X�P�[���v�Z
		//�ǂ�ǂ�傫���Ȃ����菬�����Ȃ�����
		//�������Ȃ肷������j�󂷂�
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
				//	Debug.Log("�X�P�[��");
				}*/

	}
	public int RandomValue(int X, int Y)
	{
		return UnityEngine.Random.Range(X, Y + 1);

	}

	/// <summary>
	/// �g�p�҂��ƂɈقȂ�e�ۂ̏�����
	/// �g�p�҂��Ăяo��
	/// �q�e�̂��߂ɂ��̃X�N���v�g���ł��Ăяo��
	/// </summary>
	public void InitializedBullet(GameObject _owner, GameObject _target)
    {
		owner = _owner.transform;
		target = _target;

		//�g�p�҂ɏ]����
        if (_user == MasicUser.Player)
        {
			//����̈З͏C���ł��H
			_owner.GetComponent<PlyerController>().BuffCalc(this);

		}
		else if (_user == MasicUser.Sister)
		{
			_owner.GetComponent<FireAbility>().BuffCalc(this);
		}
        else
        {
			//�G�A���̑��̏ꍇEnemyMagic��ATK�����̂܂܂Ԃ�����
			_owner.GetComponent<EnemyAIBase>().BuffCalc(this);

		}

		//����������
		_initialized = true;
	}


	void Damage(EnemyBase enemy)
	{
		if (enemy.lastHit != null && !enemy.isHitable)
		{
			enemy.isHitable = enemy.lastHit != this.gameObject ? true : false;

		}

		if (enemy.isHitable)
		{

			//EnemyBase enemy = other.GetComponent<EnemyBase>();
			//enemy.sm = em;

			//		Debug.Log($"�����Ă�����{em.name}");


			enemy.lastHit = this.gameObject;
			enemy.bulletDirection = direction;
			enemy.insert = true;
		}

	}
	/// <summary>
	/// //���̃G�t�F�N�g�����炩���߃��[�h���Ă�������
	/// </summary>
	/// <returns></returns>
	async UniTask<GameObject> LoadBullet()
	{


		return await Addressables.LoadAssetAsync<GameObject>(em.hitEffect);

	}

	void HealMagic()
	{
		if (!playerEffect)
		{
			//		Debug.Log("������");
			GManager.instance.HpRecover(em.recoverAmount);
			playerEffect = true;
			effectWait = 0;
		}
	}

	/// <summary>
	/// �e�ۓ����������̏���
	/// </summary>
	void BulletHit(Collider2D other)
	{
		//	Debug.Log("�����");

		// �I�[�i�[�`�F�b�N�B���邩�ǂ����ANull�Ȃ�߂�
		if (owner == other.gameObject.transform || isAct)
		{
			Debug.Log($"�Փ�{gameObject.name}");
			return;
		}
		// �������g�Ƀq�b�g���Ȃ��悤�Ƀ`�F�b�N

		// �ǃA�^�����`�F�b�N

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
					Instantiate(next, goFire.position, goFire.rotation);//���̃G�t�F�N�g��
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

				//����ђʂł��邩�݂����Ȑ��l����Ă���������
				Damage(other.GetComponent<EnemyBase>());
				//���̂Ƃ��ł͏Փ˂ŏ�����ł���
				isAct = false;
				//	Addressables.ReleaseInstance(this.gameObject);
			}
		}
		else if (other.gameObject.tag == GManager.instance.playerTag)
		{
			if (Magic.MagicType.Recover == em.mType)
			{
				HealMagic();
			}
		}
		else
		{

			if (em.fireType != Magic.FIREBULLET.STOP)
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
	/// ��_�Ԃ̊p�x�����߂�
	/// </summary>
	/// <param name="p1">�����̍��W</param>
	/// <param name="p2">����̍��W</param>
	/// <returns></returns>
	float GetAim(Vector2 p1, Vector2 p2)
	{
		float dx = p2.x - p1.x;
		float dy = p2.y - p1.y;
		float rad = Mathf.Atan2(dy, dx);
		return rad * Mathf.Rad2Deg;
	}


	/// <summary>
	/// �_���[�W�v�Z
	/// </summary>
	/// <param name="isFriend">�^�Ȃ疡��</param>
	public void DamageCalc(bool isShield)
	{
		//GManager.instance.isDamage = true;
		//useEquip.hitLimmit--;
		//mValue�̓��[�V�����l



		Equip useEquip;

		if (isShield)
		{
			useEquip = GManager.instance.equipShield;
			GManager.instance.useAtValue.isShield = false;
		}
		else
		{
			useEquip = GManager.instance.equipWeapon;
		}

		if (useEquip.phyAtk > 0)
		{
			_damage._attackData.phyAtk = (Mathf.Pow(useEquip.phyAtk, 2) * GManager.instance.useAtValue.x) * attackFactor;

			//�a���h�ˑŌ����Ǘ�
			if (GManager.instance.useAtValue.type == Equip.AttackType.Slash)
			{
				_damage._attackData._attackType = 0;
			}
			else if (GManager.instance.useAtValue.type == Equip.AttackType.Stab)
			{
				_damage._attackData._attackType = 2;
			}
			else if (GManager.instance.useAtValue.type == Equip.AttackType.Strike)
			{


				_damage._attackData._attackType = 4;

				//						Debug.Log("�M��");
				if (GManager.instance.useAtValue.z >= 40)
				{
					_damage._attackData.isHeavy = true;
				}
				else
				{
					_damage._attackData.isHeavy = false;
				}
			}
		}
		//�_��
		if (useEquip.holyAtk > 0)
		{
			_damage._attackData.holyAtk = (Mathf.Pow(useEquip.holyAtk, 2) * GManager.instance.useAtValue.x) * holyATFactor;

		}
		//��
		if (useEquip.darkAtk > 0)
		{
			_damage._attackData.darkAtk = (Mathf.Pow(useEquip.holyAtk, 2) * GManager.instance.useAtValue.x) * darkATFactor;

		}
		//��
		if (useEquip.fireAtk > 0)
		{
			_damage._attackData.fireAtk = (Mathf.Pow(useEquip.holyAtk, 2) * GManager.instance.useAtValue.x) * fireATFactor;

		}
		//��
		if (useEquip.thunderAtk > 0)
		{
			_damage._attackData.thunderAtk = (Mathf.Pow(useEquip.holyAtk, 2) * GManager.instance.useAtValue.x) * thunderATFactor;

		}
		_damage._attackData.shock = GManager.instance.useAtValue.z;


		_damage._attackData.attackBuff = attackBuff;
		//damage = Mathf.Floor(damage * attackBuff);

		_damage._attackData.isBlow = GManager.instance.useAtValue.isBlow;
		_damage._attackData.isLight = GManager.instance.useAtValue.isLight;
		_damage._attackData.blowPower.Set(GManager.instance.useAtValue.blowPower.x, GManager.instance.useAtValue.blowPower.y);
	}




}
