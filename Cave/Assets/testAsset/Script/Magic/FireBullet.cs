using UnityEngine;
using Cysharp.Threading.Tasks;

using System;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;
using FunkyCode.Buffers;
using static CombatManager;

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

    #region ��`


    /// <summary>
    /// ���@�̓���ɕK�v�ȃf�[�^���܂Ƃ߂�
    /// </summary>
    public class MagicActionData
	{

		/// <summary>
		/// ���@�g�p�҂Ɋւ�����
		/// </summary>
		public CharacterIdentify ownerData;


		/// <summary>
		/// �^�[�Q�b�g�Ɋւ�����
		/// </summary>
		public CharacterIdentify targetData;


		/// <summary>
		/// �^�[�Q�b�g�̈ʒu
		/// </summary>
        public Vector2 targetPosition;

        /// <summary>
        /// �L�����f�[�^�̃i���o�[�����������̂ł��邩���Ď�����
        /// ������ƎQ�Ƃł��Ă��邩�𒲂ׂ�
        /// </summary>
        /// <param name="data"></param>
        /// <returns>�V�����L�����̃i���o�[�B���ꂪ-1���ƃL��������</returns>
        public int CharacterReferenceCheck(CharacterIdentify data)
		{
            //���肪�v���C���[���Ȃ�G�̃R���o�b�g�}�l�[�W���[�ɂ���͂�
            if (data.side == CharacterSide.Player)
            {
				//�L�^���Ă�ID���Ⴄ�Ȃ�
				//�V�����i���o�[�ɓ���ւ�
				if (EnemyManager.instance._targetList[data.nunber].targetID != data.ID)
				{
					return EnemyManager.instance.GetTargetNumberByID(data.ID);
                }
            }
            //�G�Ȃ�
            else
            {
                //�L�^���Ă�ID���Ⴄ�Ȃ�
                //�V�����i���o�[�ɓ���ւ�
                if (SManager.instance._targetList[data.nunber].targetID != data.ID)
                {
                    return SManager.instance.GetTargetNumberByID(data.ID);
                }
            }

			return data.nunber;
        }


		/// <summary>
		/// �f�[�^�����ƂɃL�����N�^�[�̈ʒu���擾����
		/// </summary>
		/// <param name="data">�����Ώۂ̃L�����̃f�[�^</param>
		/// <returns></returns>
		public Vector2 GetCharacterPosition(CharacterIdentify data)
		{
			//���肪�v���C���[���Ȃ�G�̃R���o�b�g�}�l�[�W���[�ɂ���͂�
			if(data.side == CharacterSide.Player)
			{
				return EnemyManager.instance._targetList[data.nunber]._condition.targetPosition;
			}
			//�G�Ȃ�
			else
            {
                return SManager.instance._targetList[data.nunber]._condition.targetPosition;
            }
		}

        /// <summary>
        /// �f�[�^�����ƂɃL�����N�^�[�̃R���g���[���A�r���e�B���擾
        /// </summary>
        /// <param name="data">�����Ώۂ̃L�����̃f�[�^</param>
        /// <returns></returns>
        public ControllAbillity GetCharacterController(CharacterIdentify data)
        {
            //���肪�v���C���[���Ȃ�Smanager�̗F�R���X�g����Ăяo��
            if (data.side == CharacterSide.Player)
            {
                return SManager.instance.AllyList[data.nunber];
            }
			//�G�Ȃ�
            else
            {
                return EnemyManager.instance.AllyList[data.nunber];
            }

        }

    }


	/// <summary>
	/// �L�����N�^�[����肷�邽�߂ɕK�v�ȗv�f
	/// �z��ŉ��Ԗڂ���ID�Ɛw�c
	/// </summary>
	public struct CharacterIdentify
	{

		/// <summary>
		/// �L�����̏���
		/// </summary>
		public CharacterSide side;

		/// <summary>
		/// ���Ԗڂɂ��邩�Ƃ����i���o�[
		/// �����ID�`�F�b�N���s�����玟��ID�ŒT��
		/// �����ăi���o�[���X�V����
		/// ID�ł�������Ȃ���΂����I���
		/// </summary>
		public int nunber;

		/// <summary>
		/// �L������ID
		/// -1�Ȃ�Q�Ɛ؂�
		/// </summary>
		public int ID;

	}




#endregion



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


	MagicActionData useCharaData;

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


	/// <summary>
	/// ���@�I�u�W�F�N�g���Ǘ����Ă���A�r���e�B
	/// </summary>
	AtEffectCon atEf;

	string _healTag;




	/// 
	/// �V�e�ۏ���
	/// �E�G���Q�Ɗm�F�����X�V�i�������Q�ƂƂ��̊m�F�͕K�v�Ȏ������j
	/// �E�e�ۈړ�
	/// �E�Փˎ�����
	/// �E���̑��펞����
	/// �E�������I������
	/// 
	/// �ɏ����𕪂���
	/// 
	/// ///



    private void OnEnable()
    {

	�@�@InitializeBullet();

		//�N�����ԏ�����
		fireTime = GManager.instance.nowTime;


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
		_damage._attackData.actionData._hitLimit = em._hitLimit;

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

		float nowTime = GManager.instance.nowTime - fireTime;


		//�ՓˊǗ�
		CollisionObjectController();


        //���邳���e�Ȃ特��炷
        if (loud)
		{
			GManager.instance.PlaySound(em.existSound, transform.position);
			loud = false;
		}




		BulletMoveController(nowTime);


	}


    #region �L�������֘A����



	/// <summary>
	/// ���@�����삷�邽�߂̃f�[�^������ē����
	/// </summary>
	/// <param name="owner"></param>
	/// <param name="ownerNum"></param>
	/// <param name="target"></param>
	/// <param name="targetNum"></param>
	public void ActionDataSet(TargetData owner,int ownerNum,TargetData target,int targetNum)
	{
		useCharaData = new MagicActionData();

		useCharaData.ownerData = GetCharacterData(owner, ownerNum);

        useCharaData.targetData = GetCharacterData(target, targetNum);
    }


	/// <summary>
	/// �L�����̃f�[�^���쐬���ĕԂ�
	/// </summary>
	/// <param name="charaData"></param>
	/// <param name="number"></param>
	/// <returns></returns>
	CharacterIdentify GetCharacterData(TargetData charaData,int number)
	{
		CharacterIdentify data = new CharacterIdentify();

		data.ID = charaData.targetID;
		data.side = charaData._baseData.side;
		data.nunber = number;

		return data;

	}



	/// <summary>
	/// ���t���[���ŏ��Ƀ^�[�Q�b�g�̈ʒu���擾
	/// �Q�Ƃł��Ȃ���΂����^�[�Q�b�g�͂��Ȃ�
	/// </summary>
	void TargetPositionCheck()
	{
		//������ƎQ�Ƃł��Ă�Ȃ�
		if(useCharaData.CharacterReferenceCheck(useCharaData.targetData) != -1)
		{
			//�ʒu���X�V
		useCharaData.targetPosition = useCharaData.GetCharacterPosition(useCharaData.targetData);
		}

	}



    #endregion


    #region �ړ�����

	void BulletMoveController(float nowTime)
	{

        if (em._moveSt.fireType == Magic.FIREBULLET.STOP)
        {
            return;
        }


        // �^�[�Q�b�g�ݒ�
        //�ǔ����Ԉȓ��A���G�ւ̎Q�Ƃ�����Ȃ�ǂ�������
        bool homing = ((nowTime < em._moveSt.homingTime) && useCharaData.targetData.nunber != -1);//((Time.fixedTime - fireTime) < em.homingTime);


        //�e�ۂ��n������܂ł̑҂����Ԃ�����ꍇ�A�����������蔻����Ȃ����̏�ɂƂǂ܂�
		//Onenable����em._moveSt.waitTime > 0 &&  ���g����movable�������Ɠ����蔻�����
        if (!movable)
        {
            //if (col != null)
            //{
              //  col.enabled = false;
            //}

			//�����o�����Ԃ������Ȃ�
            if (nowTime >= em._moveSt.waitTime)
            {
                if (em.moveSound != null)
                {
					//�n����
                    GManager.instance.PlaySound(em.moveSound, transform.position);
                }
                movable = true;
                col.enabled = true;
            }

        }



        // �z�[�~���O����
        //�����ŃW���u�Ăяo��
        //movable�͓n��


		//�^�[�Q�b�g�������ĂȂ��Ȃ�
        if (useCharaData.targetData.nunber != -1)
        {
            job.targetLost = false;
            job.posTarget = useCharaData.targetPosition;
        }
        else
        {
            job.targetLost = true;
        }

		//��������
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



    #endregion

    #region �Ǘ����\�b�h


	/// <summary>
	/// ���t���[�����s
	/// �Փ˂������̂��L�^���ĊǗ����郁�\�b�h
	/// ���łɓ����������̂ɂ͓�����Ȃ��悤�ɂ���
	/// </summary>
	void CollisionObjectController()
	{
        if (collisionList.Count > 0)
        {

            //�U�����@�Ȃ���Z�����ԂŏՓ˂����Z�b�g
            if (em.mType == Magic.MagicType.Attack)
            {
                //�ђʒe�Ȃ�B���x0�ȏォ�e�ۂ���b�ȏ㐶����Ȃ甚���Ƃ��ł͂Ȃ�
                if (em.penetration && em._moveSt.speedV > 0 && em._moveSt.lifeTime > 1)
                {
                    if (GManager.instance.nowTime - effectWait >= 0.5f)
                    {
                        collisionList = null;
                    }
                }

            }
			//�����łȂ��Ȃ�O�b�Ɉ����ʂ���
            else if (GManager.instance.nowTime - effectWait >= 3)
            {
                collisionList = null;
            }
            if (collisionList == null)
            {
                collisionList = new List<Transform>();
            }

        }

    }


	/// <summary>
	/// �������Ԃ��o�߂����������
	/// �����ĉ����������肷��
	/// </summary>
	/// <param name="nowTime"></param>
	void LifeTimeController(float nowTime)
	{
        //�e�ۂ̐������ԏI���Ȃ�
        //���邢�͒ǔ��e�̕W�I��������
        //���ꂩ���i�ł�����
        if (nowTime >= em._moveSt.lifeTime) // ((em._moveSt.fireType == Magic.FIREBULLET.HOMING || em._moveSt.fireType == Magic.FIREBULLET.HOMING) && target == null))
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
    }


	/// <summary>
	/// �e�ۂ̏�����
	/// �����鏉����
	/// �^�[�Q�b�g���Ȃǂ���������O��Ői�߂�
	/// ���C���[�ݒ�������ł��邩
	/// �������Ɩ��@�̎�ނ��Z�b�g����
	/// </summary>
    void InitializeBullet()
    {

		//�{�����Z�b�g
		_damage._attackData.multipler = useCharaData.GetCharacterController(useCharaData.ownerData).BulletBuffCalc();
			atEf = fa.atEf;

if (_user == MasicUser.Sister)
		{


			_healTag = "Player";
			em._moveSt.angle = SManager.instance.useAngle;
		}




		//�e�̑��ݎ��ԂȂǂ̃X�e�[�^�X������
		fireTime = GManager.instance.nowTime;
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

		///�@
		/// �g�p�ҁi�e�j���q�ɓn����
		/// 
		/// �G�Ǝg�p�҂̏��
		/// (�o�t�������œn���H)
		/// �����̃I�u�W�F�N�g
		/// �U���G�t�F�N�g�Ǘ��@�\
		/// �_���Ă�p�x�i�����̌����Ƃ��p�x���H�j
		/// 
		/// ///

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


    #endregion



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





	void HealMagic(GameObject target)
	{
		target.MMGetComponentNoAlloc<MyHealth>().Heal(em.recoverAmount);
		effectWait = GManager.instance.nowTime;

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
