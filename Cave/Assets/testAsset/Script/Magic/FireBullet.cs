using UnityEngine;
using Cysharp.Threading.Tasks;

using System;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;

/// <summary>
/// �ŏ���DamageOnTouch�Ɖ񕜗ʂ�����������B
/// �^�O�֘A�̎d�l�ύX
/// 
/// �܂��U���҂ɂ������Ă�o�t���擾
/// �����čU���͂Ȃǂ̓X�e�[�^�X����l��
/// ����͒ʂ���œ����邾��
/// �ŏ��̃T�C�Y�ۑ����ă^�C�����Z�b�g���ă��X�g�����ꂢ��

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




	// === �O���p�����[�^ ======================================
	[System.NonSerialized] GameObject owner;



	// === �����p�����[�^ ======================================


	[SerializeField]
	Rigidbody2D rb;


	//���Z�b�g�Ώ�
    #region
    /// <summary>
    /// �e�ۂ��ǂꂭ�炢�̎��ԑ��݂��Ă邩�v������B
    /// </summary>
    float fireTime;

	/// <summary>
	/// �W�I�B�_������
	/// </summary>
	[HideInInspector]
	public GameObject target;

	/// <summary>
	/// ���ʂ��邩�ǂ����B
	/// �A�����Č��ʔ������Ȃ��悤��
	/// </summary>
	bool isAct;	
	float effectWait;//�T�|�[�g��񕜂��ēx���ʂ������܂�
    

	
	/// <summary>
	/// ���łɏՓ˂�������
	/// </summary>
	List<Transform> collisionList = new List<Transform>();
	#endregion




	TransformAccessArray myTransform;

	NativeArray<Vector3> result;


	/// <summary>
	/// ���݉��Ȃ炵����
	/// ����炷�̂Ɏg��
	/// </summary>
	bool loud;//��ɉ����邩�ǂ���



	/// <summary>
	/// ���݉��Ȃ炵����
	/// ���������̂Ɏg��
	/// </summary>
	bool isExPlay;


	/// <summary>
	/// �L�������E�ƍ��ǂ��炩��e����������
	/// </summary>
	[HideInInspector]int direction;




	Collider2D col;

	/// <summary>
	/// �����e�ۂ������Ȃ��҂����Ԃ�����Ȃ�~�߂�t���O
	/// </summary>
	bool movable;


	FireJob job;

	JobHandle _handler;

	AtEffectCon atEf;


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


    private void OnEnable()
    {

	�@�@InitializeBullet();



		myTransform = new TransformAccessArray(0);
		myTransform.Add(transform);
		result = new NativeArray<Vector3>(1, Allocator.Persistent);
		job.result = result;
	}


    // === �R�[�h�iMonobehaviour��{�@�\�̎����j ================
    void Start()
	{

		//�ŏ�������鏈��--------------------------------------------------------------

		//�q�b�g�\�񐔂��Z�b�g
		_damage._attackData._hitLimit = em._hitLimit;
		if (this.gameObject != null)
		{
			col = this.gameObject.MMGetComponentNoAlloc<Collider2D>();
		}
		//�W���u�̏�����
		//��x�����ł���
		job = new FireJob
		{ _status = em._moveSt };
		//-------------------------------------------------------------------------



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
		fireTime = 0;

		target = null;

		isAct = false;
		effectWait = 0;//�T�|�[�g��񕜂��ēx���ʂ������܂�

		collisionList.Clear();
		
		//�����܂ŏ�����


		myTransform.Dispose();
		result.Dispose();
	}

	
    void FixedUpdate()
	{

        Debug.Log($"���O{this.gameObject.name}�W�I{target == null}");


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



			atEf.BulletClear(transform);
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

        if (em._moveSt.speedA != 0)
        {
			job.speed += (em._moveSt.speedA * Time.fixedDeltaTime);
		}

		
		job.movable = movable;
		job.homing = homing;
		job.time = Time.fixedDeltaTime;
	//	job.homingAngle = transform.rotation; 
		this._handler = this.job.Schedule(myTransform);
		_handler.Complete();


		if (movable)
		{
			rb.velocity = result[0];
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
	public void InitializeNextBullet(GameObject _owner, GameObject _target, int _direction,AtEffectCon con)
	{

		owner = _owner;
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

		atEf = con;


	}

    void InitializeBullet()
    {

		//�܂��o�t���m�F�ƃ^�[�Q�b�g�ݒ�
		//�g�p�҂ɏ]����
		if (_user == MasicUser.Player)
		{
			owner = GManager.instance.Player;
			//����̈З͏C���ł��H
			owner.MMGetComponentNoAlloc<PlyerController>().BuffCalc(this);

		}
		else if (_user == MasicUser.Sister)
		{
			owner = SManager.instance.Sister;
			target = SManager.instance.restoreTarget;
			FireAbility fa = owner.MMGetComponentNoAlloc<FireAbility>();
			fa.BuffCalc(this);
			atEf = fa.atEf;

			_healTag = "Player";
			em._moveSt.angle = SManager.instance.useAngle;
		}




		//�e�̑��ݎ��ԂȂǂ̃X�e�[�^�X������
		fireTime = 0;
		//�����蔻�菉����
		_damage.CollidRestoreResset();

		//Debug.Log($"�W�I{gameObject.name}");

		//���ˉ�
		GManager.instance.PlaySound(em.fireSound, transform.position);

		// ���݂��Ă�Ԃ̃T�E���h������Ȃ�
		if (em.existSound != null)
		{
			loud = true;
			isExPlay = true;
		}



		//�q�̒e�ۂȂ�e�̕����Ō�����ς���
		if (em.isChild)
		{
			Vector3 setScale = transform.localScale;
			if (direction < 0)
			{
				setScale.x = -setScale.x;
			}
			transform.localScale = setScale;
		}

		//���̒e�ۂ̃_���[�W���Z�o
		if (this.gameObject != null)
			DamageCalc();



		//�W���u�̏�����
		job.Initialize(target.transform.position, transform.position);


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
		if (owner == other.gameObject || isAct || other == null)
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


		if (em.hitEffect != null)
		{

			//�q�e�̖��@����Ȃ�
			if (em.childM != null)
			{

				//next.transform.localScale.Set(0, 0,0);

				Vector3 goFire = transform.position;
				Vector2 random = Vector2.zero;

				FireBullet nb = atEf.BulletCall(em.hitEffect, this.gameObject.transform.position, transform.rotation,em.childM.flashEffect).gameObject.MMGetComponentNoAlloc<FireBullet>();

				nb.InitializeNextBullet(this.gameObject, target, direction, atEf);
				transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, em.childM._moveSt.angle);
				nb.transform.rotation = transform.rotation;

				float num = em.childM.bulletNumber;
				if (num > 1)
				{

					random.Set(em.childM.HRandom, em.childM.VRandom);



					for (int x = 1; x < num; x++)
					{
						if (x == 0)
						{
						}

						else
						{
							if (random.x != 0 || random.y != 0)
							{
								goFire.Set(goFire.x + RandomValue((int)-random.x, (int)random.x), goFire.y + RandomValue((int)-random.y, (int)random.y), this.gameObject.transform.position.z);
							}

							atEf.BulletCall(em.hitEffect, goFire, transform.rotation, em.childM.flashEffect).gameObject.MMGetComponentNoAlloc<FireBullet>().InitializeNextBullet(this.gameObject, target, direction, atEf);
						}
					}
				}
			}
			else
				{
					atEf.BulletCall(em.hitEffect, this.gameObject.transform.position, transform.rotation);
				}



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

						atEf.BulletClear(transform);
                       // Destroy(this.gameObject);
					}

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

          _damage._attackData._attackType = em.magicElement;
		_damage._attackData.phyType = em.phyElement;

		if (em.phyAtk > 0)
		{
			_damage._attackData.phyAtk = em.phyAtk * attackFactor;

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
