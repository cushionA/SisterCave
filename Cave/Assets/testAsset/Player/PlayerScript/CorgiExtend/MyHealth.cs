using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine.Serialization;
using Guirao.UltimateTextDamage;

namespace MoreMountains.CorgiEngine
{
    public class MyHealth : Health
    {
        [HideInInspector]
        public DefenseData _defData;



        [SerializeField]
        [Header("���̃I�u�W�F�N�g�����@�ł��邩")]
        MyDamageOntouch.TypeOfSubject _defender;

        EnemyAIBase eData;
        PlyerController pCon;

        [HideInInspector]
        public Vector2 blowVector;


        protected UltimateTextDamageManager um;

        protected void Awake()
        {


            if (_defender == MyDamageOntouch.TypeOfSubject.Enemy)
            {
                

                eData = GetComponent<EnemyAIBase>();
                //�A�^�b�N�f�[�^���Z�b�g���郁�\�b�h�Ƃ��ق���
                //�q�b�g���_���[�W�v�Z�Ńq�b�g����邩
                eData.DefCalc();
                um = EnemyManager.instance.um;
            }
            else if (_defender == MyDamageOntouch.TypeOfSubject.Player)
            {
                pCon = GetComponent<PlyerController>();
            }
            else if (_defender == MyDamageOntouch.TypeOfSubject.Magic)
            {
                
            }
            else
            {
                //㩂̃_���[�W
            }
        }


        /// <summary>
        /// �_���[�W���󂯂����ɌĂ΂��
        /// </summary>
        /// <param name="damage">������̗̓|�C���g�̗�</param>
        /// <param name="instigator">�_���[�W�������N�������I�u�W�F�N�g</param>
        /// <param name="flickerDuration">�_���[�W���󂯂���A�I�u�W�F�N�g�����ł��鎞�ԁi�b�j���w�肵�܂��B</param>
        /// <param name="invincibilityDuration">�q�b�g��̒Z�����G���Ԃ̒����B</param>
        public void Damage(AttackData _damageData, GameObject instigator, float flickerDuration,
            float invincibilityDuration, Vector3 damageDirection,bool back,MyWakeUp.StunnType stunnState)
        {

                     // �I�u�W�F�N�g�����G�ł���΁A���������ɏI�����܂��B
            if (TemporarilyInvulnerable || Invulnerable || ImmuneToDamage || PostDamageInvulnerable)
            {
                OnHitZero?.Invoke();
                return;
            }
			// ���ł�0��������Ă���ꍇ�́A���������ɏI�����܂��B
            if ((CurrentHealth <= 0) && (InitialHealth != 0))
            {
                return;
            }
			int damage = 0;

            if (_defender == MyDamageOntouch.TypeOfSubject.Enemy)
            {
                //�A�^�b�N�f�[�^���Z�b�g���郁�\�b�h�Ƃ��ق���
                //�q�b�g���_���[�W�v�Z�Ńq�b�g����邩
                eData.DefCalc();
            }
            else if (_defender == MyDamageOntouch.TypeOfSubject.Player)
            {
                pCon.DefCalc(!GManager.instance.twinHand);
            }
            else if (_defender == MyDamageOntouch.TypeOfSubject.Magic)
            {

            }
            else
            {
                //�I�u�W�F�N�g���_���[�W�󂯂�
            }

            damage = (int)Calc(_damageData,back);

            

            if (damage <= 0)
            {
                OnHitZero?.Invoke();
                return;
            }
            um.AddStack(damage, this.transform);


            if (!this.enabled)
            {
                return;
            }



            // we decrease the character's health by the damage
            float previousHealth = CurrentHealth;
            CurrentHealth -= damage;

            LastDamage = damage;
            LastDamageDirection = damageDirection;
            OnHit?.Invoke();

            if (CurrentHealth < 0)
            {
                CurrentHealth = 0;
            }

            // Projectile�APlayer�AEnemies�Ƃ̏Փ˂�h�����Ƃ��ł��܂��B
            if (invincibilityDuration > 0)
            {
                EnablePostDamageInvulnerability();
                StartCoroutine(DisablePostDamageInvulnerability(invincibilityDuration));
            }

            // �_���[�W���������ɂȂ�C�x���g
            MMDamageTakenEvent.Trigger(_character, instigator, CurrentHealth, damage, previousHealth);

            if (_animator != null)
            {
                _animator.SetTrigger("Damage");
            }

            // we play the damage feedback
            if (FeedbackIsProportionalToDamage)
            {
                DamageFeedbacks?.PlayFeedbacks(this.transform.position, damage);
            }
            else
            {
                DamageFeedbacks?.PlayFeedbacks(this.transform.position);
            }

            if (FlickerSpriteOnHit)
            {
                // We make the character's sprite flicker
                if (_renderer != null)
                {
                    StartCoroutine(MMImage.Flicker(_renderer, _initialColor, FlickerColor, 0.05f, flickerDuration));
                }
            }

            // �w���X�o�[�X�V
            UpdateHealthBar(true);

            if (_defender != MyDamageOntouch.TypeOfSubject.Gimic && stunnState != MyWakeUp.StunnType.notStunned)
            {
                if(!(stunnState != MyWakeUp.StunnType.Down && CurrentHealth <= 0))
                {
                    //������΂�����Ȃ������s���Ă鎞�ȊO�Ȃ琁����΂��͂���
                    //�X�^���J�n�B
                    //�p���B�ƒe����͏��������ʉ�����
                    GetComponent<MyWakeUp>().StartStunn(stunnState);
                }

            }


            // �̗̓[���̎�
            if (CurrentHealth <= 0)
            {
                // �w���X��0�ɂ��܂��B�i�w���X�o�[�ɕ֗��ł��B�j
                CurrentHealth = 0;

                if (_character != null)
                {
                    if (_character.CharacterType == Character.CharacterTypes.Player)
                    {
                        LevelManager.Instance.KillPlayer(_character);
                        return;
                    }

                }

                Kill();
            }
        }

        public float Calc(AttackData _damageData,bool back)
        {
			bool stab = false;
			//isDamage = true;
			//SetLayer(28);
			//////Debug.log($"�K�[�h�����ۂ�{guardHit}");

				//	GManager.instance.equipWeapon.hitLimmit--;

			//	Equip _damageData;
				byte _attackType = 0;
			    float mainDamage = 0;

			//////Debug.log("�I��");
			float damage = 0;//�o�t�f�o�t�����p��damage�Ƃ��ĕێ�����
				float mValue = GManager.instance.equipWeapon.mValue;
            //float damage;//�o�t�f�o�t�����p��damage�Ƃ��ĕێ�����
            if (_defData.isGuard && !back)
            {
                //�K�[�h��
                if (_damageData.phyAtk > 0)
                {
                    mainDamage = _damageData.phyAtk;
                    //�a���h�ˑŌ����Ǘ�
                    if (_damageData._attackType == 0)
                    {
                        damage += (Mathf.Pow(_damageData.phyAtk, 2) * mValue) / (_damageData.phyAtk + _defData.Def) * ((100 - _defData.phyCut) / 100);
                        
                    }
                    else if (_damageData._attackType == 2)
                    {
                        damage += (Mathf.Pow(_damageData.phyAtk, 2) * mValue) / (_damageData.phyAtk + _defData.pierDef) * ((100 - _defData.phyCut) / 100);
                        stab = true;
                        _attackType = 2;
                    }
                    else
                    {
                        damage += (Mathf.Pow(_damageData.phyAtk, 2) * mValue) / (_damageData.phyAtk + _defData.strDef) * ((100 - _defData.phyCut) / 100);
                        //	_damageData.phyType = 4;
                        //						Debug.Log("�M��");
                        if (GManager.instance.equipWeapon.shock >= 40)
                        {
                            _damageData.isHeavy = true;
                        }
                        _attackType = 4;
                    }

                }
                //�_��
                if (_damageData.holyAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.holyAtk, 2) * mValue) / (_damageData.holyAtk + _defData.holyDef) * ((100 - _defData.holyCut) / 100);

                    if (_damageData.holyAtk > mainDamage)
                    {
                        _attackType = 8;
                        mainDamage = _damageData.holyAtk;

                    }
                }
                //��
                if (_damageData.darkAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.darkAtk, 2) * mValue) / (_damageData.darkAtk + _defData.darkDef) * ((100 - _defData.darkCut) / 100);
                    if (_damageData.holyAtk > mainDamage)
                    {
                        _damageData._attackType = 16;
                        mainDamage = _damageData.darkAtk;
                    }
                }
                //��
                if (_damageData.fireAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.fireAtk, 2) * mValue) / (_damageData.fireAtk + _defData.fireDef) * ((100 - _defData.fireCut) / 100);
                    if (_damageData.fireAtk > mainDamage)
                    {
                        _damageData._attackType = 32;
                        mainDamage = _damageData.fireAtk;
                    }
                }
                //��
                if (_damageData.thunderAtk > 0)
                {
                    if (_damageData.thunderAtk > mainDamage)
                    {
                        _damageData._attackType = 64;
                        mainDamage = _damageData.thunderAtk;
                    }
                    damage += (Mathf.Pow(_damageData.thunderAtk, 2) * mValue) / (_damageData.thunderAtk + _defData.thunderDef) * ((100 - _defData.thunderCut) / 100);

                }

                if (_defender == MyDamageOntouch.TypeOfSubject.Enemy)
                {

                }
                else if (_defender == MyDamageOntouch.TypeOfSubject.Player)
                {

                }
            }
            else
            {

                if (_damageData.phyAtk > 0)
                {
                    mainDamage = _damageData.phyAtk;
                    //�a���h�ˑŌ����Ǘ�
                    if (_damageData._attackType == 0)
                    {
                        damage += (Mathf.Pow(_damageData.phyAtk, 2) * mValue) / (_damageData.phyAtk + _defData.Def);

                    }
                    else if (_damageData._attackType == 2)
                    {
                        _damageData.phyAtk += (Mathf.Pow(_damageData.phyAtk, 2) * mValue) / (_damageData.phyAtk + _defData.pierDef);
                        stab = true;
                    }
                    else
                    {
                        damage += (Mathf.Pow(_damageData.phyAtk, 2) * mValue) / (_damageData.phyAtk + _defData.strDef);
                        //	_damageData.phyType = 4;
                        //						Debug.Log("�M��");
                        if (GManager.instance.equipWeapon.shock >= 40)
                        {
                            _damageData.isHeavy = true;
                        }

                    }

                }
                //�_��
                if (_damageData.holyAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.holyAtk, 2) * mValue) / (_damageData.holyAtk + _defData.holyDef);

                    if (_damageData.holyAtk > mainDamage)
                    {
                        _attackType = 8;
                        mainDamage = _damageData.holyAtk;

                    }
                }
                //��
                if (_damageData.darkAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.darkAtk, 2) * mValue) / (_damageData.darkAtk + _defData.darkDef);
                    if (_damageData.holyAtk > mainDamage)
                    {
                        _damageData._attackType = 16;
                        mainDamage = _damageData.darkAtk;
                    }
                }
                //��
                if (_damageData.fireAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.fireAtk, 2) * mValue) / (_damageData.fireAtk + _defData.fireDef);
                    if (_damageData.fireAtk > mainDamage)
                    {
                        _damageData._attackType = 32;
                        mainDamage = _damageData.fireAtk;
                    }
                }
                //��
                if (_damageData.thunderAtk > 0)
                {
                    if (_damageData.thunderAtk > mainDamage)
                    {
                        _damageData._attackType = 64;
                        mainDamage = _damageData.thunderAtk;
                    }
                    damage += (Mathf.Pow(_damageData.thunderAtk, 2) * mValue) / (_damageData.thunderAtk + _defData.thunderDef);

                }


                if (back && _defender != MyDamageOntouch.TypeOfSubject.Gimic)
                {
                    damage *= 1.08f;
                    _damageData.shock *= 1.1f;
                }

                //�o�b�N�A�^�b�N

                if (_defender == MyDamageOntouch.TypeOfSubject.Enemy)
                {

                }
                else if (_defender == MyDamageOntouch.TypeOfSubject.Player)
                {

                }
                }

			if (_defData.attackNow && stab)
			{
				damage *= 1.1f;
			}
            else if (_defData.isDangerous)
            {
                damage *= 1.2f;
            }

            damage = Mathf.Floor(damage * GManager.instance.attackBuff);



            if (_defender == MyDamageOntouch.TypeOfSubject.Enemy)
			{
                
                //�A�^�b�N�f�[�^���Z�b�g���郁�\�b�h�Ƃ��ق���
                //�q�b�g���_���[�W�v�Z�Ńq�b�g����邩
                eData.DamageSound(_attackType,_damageData.isHeavy);


			}
			else if (_defender == MyDamageOntouch.TypeOfSubject.Player)
			{
                pCon.DamageSound(_attackType, _damageData.isHeavy);
			}
			else if (_defender == MyDamageOntouch.TypeOfSubject.Magic)
			{

			}
			else
			{
				//�I�u�W�F�N�g���_���[�W�󂯂�
			}




			return Mathf.Floor(damage * GManager.instance.attackBuff);



		}

        public MyWakeUp.StunnType ArmorCheck(float shock,bool isBlow,bool isBack)
        {



            if (_defender == MyDamageOntouch.TypeOfSubject.Enemy)
            {
                //�A�^�b�N�f�[�^���Z�b�g���郁�\�b�h�Ƃ��ق���
                //�q�b�g���_���[�W�v�Z�Ńq�b�g����邩
                return eData.ArmorControll(shock,isBlow,isBack);
            }
            else if (_defender == MyDamageOntouch.TypeOfSubject.Player)
            {
                return pCon.ArmorControll(shock,isBlow,isBack,!GManager.instance.twinHand);
            }
            else if (_defender == MyDamageOntouch.TypeOfSubject.Magic)
            {

            }
            else
            {
                //�I�u�W�F�N�g���_���[�W�󂯂�
            }
            //�o�O�����̃��^�[��
            return MyWakeUp.StunnType.Falter;
            
        }



    }
}