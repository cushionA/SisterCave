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
/// �ŏ���DamageOnTouch�Ɖ񕜗ʂ�����������B
/// �^�O�֘A�̎d�l�ύX
/// 
/// �܂��U���҂ɂ������Ă�o�t���擾
/// �����čU���͂Ȃǂ̓X�e�[�^�X����l��
/// ����͒ʂ���œ����邾��

/// �ŏ��͒ǔ��キ���ď�����ɔ�΂��e�ۂ����X�ɒǔ����������肷��΋Ȏ˂Ƃ�����ȋO���̒e�ۍ�ꂻ��
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


	public GameObject target;

	TransformAccessArray myTransform;




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


	FireJob job;

	JobHandle _handler;


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

		myTransform = new TransformAccessArray(0);
		myTransform.Add(transform);
    }


    // === �R�[�h�iMonobehaviour��{�@�\�̎����j ================
    async void Start()
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

		var token = this.GetCancellationTokenOnDestroy();
		await UniTask.WaitUntil(s,cancellationToken:token);

		fireTime = 0;

		if (this.gameObject != null)
		{
			col = this.gameObject.MMGetComponentNoAlloc<Collider2D>();
		}

		//�q���̒e�ۂ͐e�̃^�[�Q�b�g�����p��
		if (em._moveSt.fireType != Magic.FIREBULLET.STOP || !em.isChild)
		{
			//	Debug.Log($"�W�I�ݒ�{SManager.instance.restoreTarget.name}");
			
			//
			direction = (owner.localScale.x < 0.0f) ? -1 : 1;
		}






		//Debug.Log($"�W�I{gameObject.name}");



		// ���݂��Ă�Ԃ̃T�E���h������Ȃ�
		if (em.existSound != null)
		{
			loud = true;
			isExPlay = true;
		}


		attackEnabled = true;
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

        Debug.Log($"���O{this.gameObject.name}�W�I{target == null}");
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

		//���邳���e�Ȃ特��炷
		if (loud)
		{
			GManager.instance.PlaySound(em.existSound, transform.position);
			loud = false;
		}

		//�e�ۂ̐������ԏI���Ȃ�
		//���邢�͒ǔ��e�̕W�I��������
		//���ꂩ���i�ł�����
		if (fireTime >= em._moveSt.lifeTime) // ((em._moveSt.fireType == Magic.FIREBULLET.HOMING || em._moveSt.fireType == Magic.FIREBULLET.HOMING) && target == null))
		{

			//   ���ݒ��̉������Ȃ��Ă�Ȃ����
			if (isExPlay)
			{
				GManager.instance.StopSound(em.existSound, 1f);
			}
            //�q�e�ۂł���Ȃ������

				

				Addressables.ReleaseInstance(this.gameObject);
				Destroy(this.gameObject);
			
		}



		if (em._moveSt.fireType == Magic.FIREBULLET.STOP)
		{
			return;
		}


		// �^�[�Q�b�g�ݒ�
		//�ǔ����Ԉȓ��Ȃ�ǂ�������
		bool homing = ((fireTime < em._moveSt.homingTime) && target != null);//((Time.fixedTime - fireTime) < em.homingTime);


		//�e�ۂ��n������܂ł̑҂����Ԃ�����ꍇ�A�����������蔻����Ȃ����̏�ɂƂǂ܂�
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


		// �z�[�~���O����
		//�����ŃW���u�Ăяo��
		//movable�͓n��


		if (target != null)
        {
			job.targetLost = false;
    		job.posTarget = target.transform.position;
        }
        else
        {
			job.targetLost = true;
        }
		Debug.Log($"����������{job.ToString()}");
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
	/// �g�p�҂��ƂɈقȂ�e�ۂ̏�����
	/// �g�p�҂��Ăяo��
	/// �q�e�̂��߂ɂ��̃X�N���v�g���ŌĂяo��
	/// </summary>
	public void InitializeNextBullet(GameObject _owner, GameObject _target, int _direction = 1)
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


		Debug.Log("��");
		//����������
		_initialized = true;
	}

    void InitializePlayerBullet()
    {


		//�g�p�҂ɏ]����
		if (_user == MasicUser.Player)
		{
			owner = GManager.instance.Player.transform;
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

		em._moveSt.angle = SManager.instance.useAngle;
		job = new FireJob
		{ _status = em._moveSt };

		job.Initialize(target.transform.position, transform.position);
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
