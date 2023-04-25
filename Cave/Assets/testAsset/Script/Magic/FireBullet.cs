using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using System;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections.Generic;

/// <summary>
/// �ŏ���DamageOnTouch�Ɖ񕜗ʂ�����������B
/// �^�O�֘A�̎d�l�ύX
/// 
/// �܂��U���҂ɂ������Ă�o�t���擾
/// �����čU���͂Ȃǂ̓X�e�[�^�X����l��
/// �����ʂ���œ����邾��
/// ���Ƃ͖��@�Ƀq�b�g�㓖���蔻���r�����鏈��������H
/// </summary>

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

	[SerializeField]
	MyDamageOntouch _damage;

	public enum MasicUser
    {
		Player,
		Sister,
		Others,
		Child//�q�ǂ��̒e��
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

	/// <summary>
	/// ����������Ă܂����H
	/// </summary>
	bool _initialized;

	/// <summary>
	/// ���łɏՓ˂�������
	/// </summary>
	List<Transform> collisionList = new List<Transform>();


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

	string _healTag;



    private void Awake()
    {
        if(_user == MasicUser.Player || _user == MasicUser.Sister)
        {
			InitializePlayerBullet();
        }
    }


    // === �R�[�h�iMonobehaviour��{�@�\�̎����j ================
    async UniTaskVoid Start()
	{
		Func<bool> s = () => 
		{
			//�������������������ǂ������͂��邾���̏���
			//�����������͔��ˑ������
			return _initialized;
		};
		_damage.CollidRestoreResset();
		_damage._attackData._hitLimit = em._hitLimit;
		//�������������^��Ԃ��܂ő҂�
		await UniTask.WaitUntil(s);


		//ownner�͒e�ێg�p���Őݒ�
		fireTime = 0;

		col = this.gameObject.MMGetComponentNoAlloc<Collider2D>();


		//�q���̒e�ۂ͐e�̃^�[�Q�b�g�����p��
		if (em.fireType != Magic.FIREBULLET.STOP || !em.isChild)
		{
			//	Debug.Log($"�W�I�ݒ�{SManager.instance.restoreTarget.name}");
			
			//
			direction = (owner.localScale.x < 0.0f) ? -1 : 1;
		}





		if (target == null)
		{
			speed = (direction == -1) ? -em.speedV : +em.speedV;
		}
		else
		{
			//Debug.Log($"�W�I{gameObject.name}");
			// ������
			if (em.fireType != Magic.FIREBULLET.STOP)
			{
				posTarget = target.transform.position + new Vector3(0.0f, 1.0f, 0.0f);
			}

			switch (em.fireType)
			{


				case Magic.FIREBULLET.ANGLE:
					speed = em.speedV;
//(direction == -1) ? -em.speedV : +
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
					homingAngle = em.homingAngleA;
					break;
				case Magic.FIREBULLET.RAIN:

					speed = em.speedV;
					em.angle =  SManager.instance.useMagic.angle;
					break;
			}
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
			sm = next.MMGetComponentNoAlloc<FireBullet>();
			//sm�����݂��Ă�Ȃ�
			//Debug.Log($"���O{target.name}");

			
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
		if (collisionList.Count > 0)
		{
			effectWait += Time.fixedDeltaTime;
            //�O�b�ȏ�Ȃ�v���C���[�G�t�F�N�g���Đݒ�
            if (em.mType == Magic.MagicType.Attack)
            {
				//�ђʒe�Ȃ�B���x0�ȏォ�e�ۂ���b�ȏ㐶����Ȃ甚���Ƃ��ł͂Ȃ�
                if (em.penetration && em.speedV > 0 && em.lifeTime > 1)
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
		//���邳���e�Ȃ特��炷
		if (loud)
		{
			GManager.instance.PlaySound(em.existSound, transform.position);
			loud = false;
		}
		//�e�ۂ̐������ԏI���Ȃ�
		//���邢�͒ǔ��e�̕W�I��������
		//���ꂩ���i�ł�����
		if (fireTime >= em.lifeTime) // ((em.fireType == Magic.FIREBULLET.HOMING || em.fireType == Magic.FIREBULLET.HOMING) && target == null))
		{
	//Debug.Log("������");
			//   ���ݒ��̉������Ȃ��Ă�Ȃ����
			if (isExPlay)
			{
				GManager.instance.StopSound(em.existSound, 1f);
			}
            //�q�e�ۂł���Ȃ������

				

				Addressables.ReleaseInstance(this.gameObject);
				Destroy(this.gameObject);
			
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
		bool homing = ((fireTime < em.homingTime) && target != null);//((Time.fixedTime - fireTime) < em.homingTime);
		if (homing)
		{

			posTarget = target.transform.position + Vector3.up;
		}

		//�e�ۂ��n������܂ł̑҂����Ԃ�����ꍇ�A�����������蔻����Ȃ����̏�ɂƂǂ܂�
		if (em.waitTime > 0 && !movable)
		{
			if (col != null)
			{
				col.enabled = false;
			}
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
		//�����Ń^�[�Q�b�g�Ȃ��ꍇ�͒��i��
		if (target == null)
		{
			if (movable)
			{
				//�i�s�p�x�ɏ]���đ��x��ω�������
				rb.velocity = Quaternion.Euler(0.0f, 0.0f, em.angle) * new Vector3(speed, 0.0f, 0.0f);
			}
		}
		else
		{
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
						rb.velocity = Quaternion.Euler(0.0f, 0.0f, em.angle) * new Vector3(speed, 0.0f, 0.0f) +  (Math.Sign(target.transform.position.x - transform.position.x) > 0 ? Vector3.zero : new Vector3(0,0,180 - em.angle));
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
	public void InitializedBullet(GameObject _owner, GameObject _target,int _direction = 1)
    {
		owner = _owner.transform;
		target = _target;

		//�g�p�҂ɏ]����
        if (_user == MasicUser.Child)
        {
			//����̈З͏C���ł��H
			_owner.MMGetComponentNoAlloc<FireBullet>().BuffCalc(this);
			direction = _direction;
		}
        else
        {
			//�G�A���̑��̏ꍇEnemyMagic��ATK�����̂܂܂Ԃ�����
			_owner.MMGetComponentNoAlloc<EnemyAIBase>().BuffCalc(this);
			_healTag = "Enemy";

		}

		//����������
		_initialized = true;
	}

    void InitializePlayerBullet()
    {

		//�g�p�҂ɏ]����
		if (_user == MasicUser.Player)
		{
			owner = GManager.instance.Player.transform;
			//target = GManager.instance.;
			//����̈З͏C���ł��H
			owner.gameObject.MMGetComponentNoAlloc<PlyerController>().BuffCalc(this);

		}
		else if (_user == MasicUser.Sister)
		{
			owner = SManager.instance.Sister.transform;
			target = SManager.instance.restoreTarget;
			owner.gameObject.MMGetComponentNoAlloc<FireAbility>().BuffCalc(this);
			_healTag = "Player";
		}

		//����������
		_initialized = true;
	}


	/// <summary>
	/// //���̃G�t�F�N�g�����炩���߃��[�h���Ă�������
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
	/// �e�ۓ����������̏���
	/// </summary>
	void BulletHit(Collider2D other)
	{


		// �I�[�i�[�`�F�b�N�B���邩�ǂ����ANull�Ȃ�߂�
		if (owner == other.gameObject.transform || isAct || other == null)
		{
			//Debug.Log($"�Փ�{owner == other.gameObject.transform}{isAct}{other != null}");// {gameObject.name}");
			return;
		}
		// �������g�Ƀq�b�g���Ȃ��悤�Ƀ`�F�b�N


		/// <summary>
		/// ���łɌ��ʂ��Ă���̂Ŗ���
		/// </summary>
		bool alreadyEffect = false;//�T�|�[�g��񕜂����ʂ��y�ڂ��B������񂾂�
						   // �ǃA�^�����`�F�b�N

		isAct = true;

		//�q�e����Ȃ甭��
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

					Instantiate(next, goFire.position, goFire.rotation).MMGetComponentNoAlloc<FireBullet>().InitializedBullet(this.gameObject, target, direction);

				}
			}
			else
			{
				Instantiate(next, this.gameObject.transform.position, next.transform.rotation).GetComponent<FireBullet>().InitializedBullet(this.gameObject, target, direction);
			}

		}
		else if (em.hitEffect != null)
		{

			Addressables.InstantiateAsync(em.hitEffect, this.gameObject.transform.position, transform.rotation);

		}

	

		//�����Ȃ��̂ƏՓ˂��Ă�Ȃ�
		if (collisionList.Count > 0)
        {//Debug.Log("ssssdf");
			for (int i = 0; i < collisionList.Count;i++)
            {
				//�������łɂԂ��������̂ƈ�v������
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
					//����ђʂł��邩�݂����Ȑ��l����Ă���������

					//���̂Ƃ��ł͏Փ˂ŏ�����ł���
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
			//Debug.Log($"{this.gameObject.name}��{other.transform.gameObject.name}");
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
	/// �X�e�[�^�X�ƃo�t��e����擾���ă_���[�W�v�Z
	/// </summary>
	/// <param name="isFriend">�^�Ȃ疡��</param>
	public void DamageCalc()
	{
		//GManager.instance.isDamage = true;
		//useEquip.hitLimmit--;
		//mValue�̓��[�V�����l

          _damage._attackData._attackType = em.attackType;

		if (em.phyAtk > 0)
		{
			_damage._attackData.phyAtk = em.phyAtk * attackFactor;

			//�a���h�ˑŌ����Ǘ�
          if (em.attackType == 4)
		{


				//_damage._attackData._attackType = 4;

				//						Debug.Log("�M��");
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
		//�_��
		if (em.holyAtk > 0)
		{
			_damage._attackData.holyAtk = em.holyAtk * holyATFactor;

		}
		//��
		if (em.darkAtk > 0)
		{
			_damage._attackData.darkAtk = em.darkAtk * darkATFactor;

		}
		//��
		if (em.fireAtk > 0)
		{
			_damage._attackData.fireAtk = em.fireAtk * fireATFactor;

		}
		//��
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
	/// �o�t�̐��l��^����
	/// �e�ۂ���Ă�
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
