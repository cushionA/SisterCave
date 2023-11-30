using Cysharp.Threading.Tasks;
using Guirao.UltimateTextDamage;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class MyHealth : Health
    {
        [HideInInspector]
        public DefenseData _defData = new DefenseData();



        [SerializeField]
        [Header("���̃I�u�W�F�N�g�����@�ł��邩")]
        MyDamageOntouch.TypeOfSubject _defender;

        /// <summary>
        /// �L�����֘A�̃N���X
        /// </summary>
        ControllAbillity _charaCon;

        [HideInInspector]
        public Vector2 blowVector;

        [HideInInspector]
        public bool _parryNow;

        [HideInInspector]
        public bool _guardAttack;

        [HideInInspector]
        public bool _superArumor;


        /// <summary>
        /// ��������΂���Ă邩
        /// ������΂���Ă�Ȃ牟���ꂽ�肷��̂��玩�R�ɂȂ��
        /// </summary>
        [HideInInspector]
        public bool _blowNow;

        protected new MyCharacter _character;

        protected UltimateTextDamageManager um;

        protected void StatusSet()
        {




            if (_defender == MyDamageOntouch.TypeOfSubject.Magic)
            {

            }
            else if(_defender == MyDamageOntouch.TypeOfSubject.Gimic)
            {
                //㩂̃_���[�W
            }
            else
            {
                _charaCon = _character.FindAbility<ControllAbillity>();

                um = EnemyManager.instance.um;
            }

        }

        /// <summary>
        /// �g�p����R���|�[�l���g���l�����_���[�W��L�����������F���擾����B
        /// </summary>
        protected override void Initialization()
        {
            _character = this.gameObject.GetComponent<MyCharacter>();
            _characterPersistence = this.gameObject.GetComponent<CharacterPersistence>();

            if (this.gameObject.MMGetComponentNoAlloc<SpriteRenderer>() != null)
            {
                _renderer = this.gameObject.GetComponent<SpriteRenderer>();
            }

            if (_character != null)
            {
                if (_character.CharacterModel != null)
                {
                    if (_character.CharacterModel.GetComponentInChildren<Renderer>() != null)
                    {
                        _renderer = _character.CharacterModel.GetComponentInChildren<Renderer>();
                    }
                }
            }

            // �A�j���[�^�[���l��
            if (_character != null)
            {
                if (_character.CharacterAnimator != null)
                {
                    _animator = _character.CharacterAnimator;
                }
                else
                {
                    _animator = this.gameObject.GetComponent<Animator>();
                }
            }
            else
            {
                _animator = this.gameObject.GetComponent<Animator>();
            }

            if (_animator != null)
            {
                _animator.logWarnings = false;
            }

           // _autoRespawn = this.gameObject.GetComponent<AutoRespawn>();
            _controller = this.gameObject.GetComponent<CorgiController>();
            _healthBar = this.gameObject.GetComponent<MMHealthBar>();
            _collider2D = this.gameObject.GetComponent<Collider2D>();

            StatusSet();

            StoreInitialPosition();
            _initialized = true;
            //���쑤�ł��
            //   
            DamageEnabled();
            DisablePostDamageInvulnerability();
            UpdateHealthBar(false);
        }



        /// <summary>
        /// �_���[�W���󂯂����ɌĂ΂��
        /// </summary>
        /// <param name="damage">������̗̓|�C���g�̗�</param>
        /// <param name="instigator">�_���[�W�������N�������I�u�W�F�N�g</param>
        /// <param name="flickerDuration">�_���[�W���󂯂���A�I�u�W�F�N�g�����ł��鎞�ԁi�b�j���w�肵�܂��B</param>
        /// <param name="invincibilityDuration">�q�b�g��̒Z�����G���Ԃ̒����B</param>
        public void Damage(AttackData _damageData, GameObject instigator, float flickerDuration,
            float invincibilityDuration, Vector3 damageDirection, bool back, MyWakeUp.StunnType stunnState)
        {

            // Debug.Log($"�m�肽��{stunnState}");

            // �I�u�W�F�N�g�����G�ł���΁A���������ɏI�����܂��B

            // ���ł�0��������Ă���ꍇ�́A���������ɏI�����܂��B
            if ((CurrentHealth <= 0) && (InitialHealth != 0))
            {
                return;
            }
            int damage = 0;


                //�A�^�b�N�f�[�^���Z�b�g���郁�\�b�h�Ƃ��ق���
                //�q�b�g���_���[�W�v�Z�Ńq�b�g����邩
                _charaCon.DefCalc();


            damage = (int)Calc(_damageData, back);

        //  Debug.Log($"�����[��{damage}��{CurrentHealth}{stunnState}");
            if (um != null)
            {
               //   Debug.Log($"����{um.name}{damage}");
                um.AddStack(damage, this.gameObject.transform);
            }
            if (damage <= 0)
            {
                OnHitZero?.Invoke();
              //  return;
            }


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



            // �����̊Ԗ��G��
            if (invincibilityDuration > 0)
            {
                EnablePostDamageInvulnerability();
                DisablePostDamageInvulnerabilityTasks(invincibilityDuration).Forget();
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

            if (_defender != MyDamageOntouch.TypeOfSubject.Gimic && stunnState != MyWakeUp.StunnType.notStunned && CurrentHealth > 0)
            {

                //������΂�����Ȃ������s���Ă鎞�ȊO�Ȃ琁����΂��͂���
                //�X�^���J�n�B
                //�p���B�ƒe����͏��������ʉ�����

                _charaCon.StartStun(stunnState);

            }


            // �̗̓[���̎�
            //���S����
            if (CurrentHealth <= 0)
            {

                GManager.instance.PlaySound("Kill",transform.position);

                // �w���X��0�ɂ��܂��B�i�w���X�o�[�ɕ֗��ł��B�j
                CurrentHealth = 0;

                Invulnerable = true;

                if (_character != null)
                {
                    //�_�E�����Ď��ʂ̂������łȂ���
                    if (stunnState == MyWakeUp.StunnType.Down)
                    {
                        stunnState = MyWakeUp.StunnType.BlowDie;
                    }
                    else
                    {
                        stunnState = MyWakeUp.StunnType.NDie;
                    }

                    _charaCon.DeadMotionStart(stunnState);


                }


            }
            
            //����ł��Ȃ���Δ�e��̏������Ăяo��
            else
            {
                //�X�^�����Ă邩�ǂ����ŏ����𕪂����
                //�G�̃Q�[���I�u�W�F�N�g���n���܂�
                _charaCon.DamageEvent(stunnState == MyWakeUp.StunnType.notStunned,instigator,damage,back);
            }
        }

        public float Calc(AttackData _damageData, bool back)
        {
            bool stab = false;
            //isDamage = true;
            //SetLayer(28);
            //////Debug.log($"�K�[�h�����ۂ�{guardHit}");

            //	GManager.instance.equipWeapon.hitLimmit--;

            //	Equip _damageData;



            //////Debug.log("�I��");
            float damage = 0;//�o�t�f�o�t�����p��damage�Ƃ��ĕێ�����
            float mValue = _damageData.mValue;
            //float damage;//�o�t�f�o�t�����p��damage�Ƃ��ĕێ�����

            //�K�[�h��
            if ((_defData.isGuard || _guardAttack) && !back)
            {  

                //�K�[�h��
                if (_damageData.phyAtk > 0)
                {

                    //�a���h�ˑŌ����Ǘ�
                    if (_damageData.phyType == Equip.AttackType.Slash)
                    {
                        damage += (Mathf.Pow(_damageData.phyAtk, 2) * mValue) / (_damageData.phyAtk + _defData.Def) * ((100 - _defData.phyCut) / 100);

                    }
                    else if (_damageData.phyType == Equip.AttackType.Stab)
                    {
                        damage += (Mathf.Pow(_damageData.phyAtk * mValue, 2)) / (_damageData.phyAtk + _defData.pierDef) * ((100 - _defData.phyCut) / 100);
                        stab = true;

                    }
                    else
                    {
                        damage += (Mathf.Pow(_damageData.phyAtk, 2) * mValue) / (_damageData.phyAtk + _defData.strDef) * ((100 - _defData.phyCut) / 100);
                        //	_damageData.phyType = 4;
                        //						Debug.Log("�M��");

                    }

                }
                //�_��
                if (_damageData.holyAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.holyAtk, 2) * mValue) / (_damageData.holyAtk + _defData.holyDef) * ((100 - _defData.holyCut) / 100);


                }
                //��
                if (_damageData.darkAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.darkAtk, 2) * mValue) / (_damageData.darkAtk + _defData.darkDef) * ((100 - _defData.darkCut) / 100);

                }
                //��
                if (_damageData.fireAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.fireAtk, 2) * mValue) / (_damageData.fireAtk + _defData.fireDef) * ((100 - _defData.fireCut) / 100);

                }
                //��
                if (_damageData.thunderAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.thunderAtk, 2) * mValue) / (_damageData.thunderAtk + _defData.thunderDef) * ((100 - _defData.thunderCut) / 100);

                }

                //���@�̏���т��ʃK�[�h���Ă�M�~�b�N�ł͂����������Ƃ����邩��
                //�w���X���Ă�ȏ�͂�
                 _charaCon.GuardSound();

            }
            else
            {
           //     Debug.Log($"����{_defData.isGuard}��{!back}");
                if (_damageData.phyAtk > 0)
                {

                    //�a���h�ˑŌ����Ǘ�
                    if (_damageData.phyType ==  Equip.AttackType.Slash)
                    {
                        damage += (Mathf.Pow(_damageData.phyAtk, 2) * mValue) / (_damageData.phyAtk + _defData.Def);

                    }
                    else if (_damageData.phyType ==  Equip.AttackType.Stab)
                    {

                        damage += (Mathf.Pow(_damageData.phyAtk, 2) * mValue) / (_damageData.phyAtk + _defData.pierDef);
                        stab = true;

                    }
                    else
                    {
                        damage += (Mathf.Pow(_damageData.phyAtk, 2) * mValue) / (_damageData.phyAtk + _defData.strDef);
                        //	_damageData.phyType = 4;
                        //						Debug.Log("�M��");
                        if (_damageData.shock >= 40)
                        {
                            _damageData.isHeavy = true;
                        }

                    }

                }
                //�_��
                if (_damageData.holyAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.holyAtk, 2) * mValue) / (_damageData.holyAtk + _defData.holyDef);

                }
                //��
                if (_damageData.darkAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.darkAtk, 2) * mValue) / (_damageData.darkAtk + _defData.darkDef);

                }
                //��
                if (_damageData.fireAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.fireAtk, 2) * mValue) / (_damageData.fireAtk + _defData.fireDef);

                }
                //��
                if (_damageData.thunderAtk > 0)
                {

                    damage += (Mathf.Pow(_damageData.thunderAtk, 2) * mValue) / (_damageData.thunderAtk + _defData.thunderDef);

                }


                if (back && _defender != MyDamageOntouch.TypeOfSubject.Gimic)
                {
                    damage *= 1.08f;
                    _damageData.shock *= 1.1f;
                }

                //�o�b�N�A�^�b�N


                    //�A�^�b�N�f�[�^���Z�b�g���郁�\�b�h�Ƃ��ق���
                    //�q�b�g���_���[�W�v�Z�Ńq�b�g����邩
                    DamageSound(_damageData._attackType, _damageData.isHeavy);

                //���@��I�u�W�F�N�g�ɂ͌ʂ̃_���[�W�T�E���h������̂ł�
                if (_defender == MyDamageOntouch.TypeOfSubject.Magic)
                {

                }
                else
                {
                    //�I�u�W�F�N�g���_���[�W�󂯂�
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






            //    Debug.Log($"����{damage}");

            return Mathf.Floor(damage * GManager.instance.attackBuff);



        }

        /// <summary>
        /// �X�^�����Ă邩�ǂ������m�F
        /// </summary>
        /// <param name="shock"></param>
        /// <param name="isBlow"></param>
        /// <param name="isBack"></param>
        /// <returns></returns>
        public MyWakeUp.StunnType ArmorCheck(float shock, bool isBlow, bool isBack)
        {

            MyWakeUp.StunnType result = MyWakeUp.StunnType.notStunned;

            if (_superArumor || (_guardAttack && !isBack) || shock <= 0)
            {
                result = MyWakeUp.StunnType.notStunned;
            }
            else
            {
               result = _charaCon.ArmorControll(shock, isBlow,isBack);
            }


            return result;
        }



        public EffectControllAbility.SelectState GetStunState()
        {
            int stanSt = 0;
            if (_character != null)
            {


                stanSt = _charaCon.GetStunState();


            }

            //fall
            if(stanSt == 1)
            {
                return EffectControllAbility.SelectState.Faltter;
            }
            //blow
            else if (stanSt == 4)
            {
                return EffectControllAbility.SelectState.Blow;
            }
            //gb
            else if (stanSt == 3)
            {
                return EffectControllAbility.SelectState.GBreake;
            }
            else if (stanSt == 7)
            {
                return EffectControllAbility.SelectState.BlowDead;
            }
            return EffectControllAbility.SelectState.Null;
        }


        /// <summary>
        /// �G�̏ꍇ�̓v���C���[�̕��������A�����オ�莞
        /// </summary>
        public void ArmorReset()
        {

                _charaCon.ArmorReset();
        }


        /// <summary>
        /// �������v���C���[�ɍU���������Ƀp���B���o�͂��邩���͂���
        /// �U�����̃w���X�ŏo��
        /// </summary>
        /// <returns></returns>
        public bool ParryArmorCheck()
        {

            return _charaCon.ParryArmorJudge();


        }

        /// <summary>
        /// �U�����ꂽ���ɋ󒆂ɂ��������_�E������
        /// </summary>
        /// <param name="stunnState"></param>
        /// <returns></returns>
        public bool AirDownJudge(MyWakeUp.StunnType stunState)
        {
            //�󒆂ɂ��鎞�͏����������ă_�E����
            //�U�����͂Ȃ�Ƃ�
            if (!_controller.State.IsGrounded)
            {
               return  _charaCon.AirDownJudge(stunState);

            }

                return false;

        }


        public void AirDown()
        {
            _controller.SetHorizontalForce(0);

        }


        public void Heal(float recoverAmount)
        {
            CurrentHealth += recoverAmount;

            if (CurrentHealth > MaximumHealth)
            {
                CurrentHealth = MaximumHealth;
            }
        }


        /// <summary>
        /// �U�����ꂽ���̃w���X�ŌĂ�
        /// �p���B���[�V�����J�n
        /// </summary>
        /// <param name="isArmorBreak"></param>
        public void ParryStart(bool isArmorBreak = false)
        {
            _charaCon.ParryStart(isArmorBreak);

        }


        /// <summary>
        /// �G���`�����̓G���`�����g�^�C�v���Q��
        /// </summary>
        /// <param name="damageType"></param>
        public void DamageSound(AtEffectCon.Element damageType, bool heavy)
        {
            if (damageType == AtEffectCon.Element.slash)
            {
                GManager.instance.PlaySound("SlashDamage", transform.position);
            }
            else if (damageType == AtEffectCon.Element.stab)
            {
                GManager.instance.PlaySound("StabDamage", transform.position);
            }
            else if (damageType == AtEffectCon.Element.strike)
            {
                if (!heavy)
                {
                    //	Debug.Log("�`�L��");
                    GManager.instance.PlaySound("StrikeDamage", transform.position);
                }
                else
                {
                    GManager.instance.PlaySound("HeavyStrikeDamage", transform.position);
                    heavy = false;
                }
            }
            else if (damageType == AtEffectCon.Element.holy)
            {
                GManager.instance.PlaySound("HolyDamage", transform.position);
            }
            else if (damageType == AtEffectCon.Element.dark)
            {
                GManager.instance.PlaySound("DarkDamage", transform.position);
            }
            else if (damageType == AtEffectCon.Element.fire)
            {


                GManager.instance.PlaySound("FireDamage", transform.position);
            }
            else if (damageType == AtEffectCon.Element.thunder)
            {

                GManager.instance.PlaySound("ThunderDamage", transform.position);
            }
        }
        public void Die()
        {
            _charaCon.Die();
        }

        /// <summary>
        /// �K�[�h���Ă邩���m�F����
        /// </summary>
        public void GuardReport()
        {
            _defData.isGuard = _charaCon.GuardReport();
        }
        /// <summary>
        /// Allows the character to take damage
        /// </summary>
        public async UniTaskVoid DisablePostDamageInvulnerabilityTasks(float delay)
        {
            await MMCoroutine.WaitFor(delay);
            PostDamageInvulnerable = false;
        }

        /// <summary>
        /// ���w���X�����G�ł��邩�ǂ���
        /// </summary>
        /// <returns></returns>
        public bool InvulnerableCheck()
        {
            if (TemporarilyInvulnerable || Invulnerable || ImmuneToDamage || PostDamageInvulnerable)
            {
                OnHitZero?.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}