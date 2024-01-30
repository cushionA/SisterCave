using Cysharp.Threading.Tasks;
using Guirao.UltimateTextDamage;
using MoreMountains.Tools;
using UnityEngine;
using static DefenseData;

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

        /// <summary>
        /// �_���[�W�@�\����n����鐁����ы���
        /// </summary>
        [HideInInspector]
        public Vector2 blowVector;



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




            damage = (int)DamageCalc(_damageData, back);

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

        public float DamageCalc(AttackData _damageData, bool back)
        {
            bool stab = false;
            //isDamage = true;
            //SetLayer(28);
            //////Debug.log($"�K�[�h�����ۂ�{guardHit}");

            //	GManager.instance.equipWeapon.hitLimmit--;

            //	Equip _damageData;



            //////Debug.log("�I��");
            float damage = 0;//�o�t�f�o�t�����p��damage�Ƃ��ĕێ�����
            float mValue = _damageData.actionData.mValue;
            //float damage;//�o�t�f�o�t�����p��damage�Ƃ��ĕێ�����

            bool isGuard = ((HealthStateJudge(DefState.�K�[�h��)) && !back);

            //�K�[�h��
            //�K�[�h�����Ƃ������悤�ɂ�������


            //�K�[�h��


            float multipler = 0;

                //�a���h�ˑŌ����Ǘ�
                if (ElementalJudge(_damageData.actionData.useElement, AtEffectCon.Element.�a������))
                {

                //�U���{����h��{���ł�����B�_���[�W0.7�{�ɂ���o�t�ƍU������1.2�{�o�t���������킹��݂�����
                multipler = _damageData.multipler.phyAtkMultipler * _defData.multipler.phyDefMultipler;
                damage +=CalcuExe(_damageData.attackStatus.phyAtk, _defData.status.Def, mValue, multipler,isGuard ? _defData.guardStatus.phyCut:0);

                 
                }
                if (ElementalJudge(_damageData.actionData.useElement, AtEffectCon.Element.�h�ˑ���))
                {
                multipler = _damageData.multipler.phyAtkMultipler * _defData.multipler.phyDefMultipler;
                damage += CalcuExe(_damageData.attackStatus.phyAtk, _defData.status.pierDef, mValue, multipler, isGuard ? _defData.guardStatus.phyCut : 0);

            }
                if (ElementalJudge(_damageData.actionData.useElement, AtEffectCon.Element.�Ō�����))
                {
                multipler = _damageData.multipler.phyAtkMultipler * _defData.multipler.phyDefMultipler;
                damage += CalcuExe(_damageData.attackStatus.phyAtk, _defData.status.strDef, mValue, multipler, isGuard ? _defData.guardStatus.phyCut : 0);

            }

                //�_��
                if (ElementalJudge(_damageData.actionData.useElement, AtEffectCon.Element.������))
                {
                multipler = _damageData.multipler.holyAtkMultipler * _defData.multipler.holyDefMultipler;
                damage += CalcuExe(_damageData.attackStatus.holyAtk, _defData.status.holyDef, mValue, multipler, isGuard ? _defData.guardStatus.holyCut : 0);

            }
                //��
                if (ElementalJudge(_damageData.actionData.useElement, AtEffectCon.Element.�ő���))
                {
                multipler = _damageData.multipler.darkAtkMultipler * _defData.multipler.darkDefMultipler;
                damage += CalcuExe(_damageData.attackStatus.darkAtk, _defData.status.darkDef, mValue, multipler, isGuard ? _defData.guardStatus.darkCut : 0);
            }
                //��
                if (ElementalJudge(_damageData.actionData.useElement, AtEffectCon.Element.������))
                {
                multipler = _damageData.multipler.fireAtkMultipler * _defData.multipler.fireDefMultipler;
                damage += CalcuExe(_damageData.attackStatus.fireAtk, _defData.status.fireDef, mValue, multipler, isGuard ? _defData.guardStatus.fireCut : 0);

            }
                //��
                if (ElementalJudge(_damageData.actionData.useElement, AtEffectCon.Element.������))
                {
                multipler = _damageData.multipler.thunderAtkMultipler * _defData.multipler.thunderDefMultipler;
                damage += CalcuExe(_damageData.attackStatus.thunderAtk, _defData.status.thunderDef, mValue, multipler, isGuard ? _defData.guardStatus.thunderCut : 0);
            }


            if (isGuard)
            {
                //���@�̏���т��ʃK�[�h���Ă�M�~�b�N�ł͂����������Ƃ����邩��
                //�w���X���Ă�ȏ�͂�
                _charaCon.GuardSound();
            }


            //�h�˃J�E���^�[
            if (HealthStateJudge(DefState.�U����) && stab)
            {
                damage *= 1.15f;
            }
            //�X�^����
            else if (HealthStateJudge(DefState.��_���[�W����))
            {
                damage *= 1.2f;
            }

            multipler = _damageData.multipler.allAtkMultipler * _defData.multipler.allDefMultipler;
            return Mathf.Floor(damage * multipler);


        }


        /// <summary>
        /// �v�Z���s
        /// ���ʂ̎��ōs��
        /// </summary>
        /// <param name="atk"></param>
        /// <param name="def"></param>
        /// <param name="mValue"></param>
        /// <param name="guardCut">�K�[�h�̃J�b�g���B�K�[�h���Ȃ��Ȃ�0</param>
        /// <param name="multipler"></param>
        /// <returns></returns>
        float CalcuExe(float atk,float def,float mValue,float multipler,float guardCut = 0)
        {
           return ((Mathf.Pow(atk, 2) * mValue) / (atk + def)) * multipler * ((100 - guardCut) / 100);
        }



        /// <summary>
        /// �������U�������ɓ��Ă͂܂邩�̃r�b�g���Z�̌��ʂ�Ԃ��Ă���郁�\�b�h
        /// </summary>
        /// <returns></returns>
        bool ElementalJudge(AtEffectCon.Element judgeElement,AtEffectCon.Element condition)
        {
            //����0�ȏ�ɂȂ邩
            return (judgeElement & condition) > 0;
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

            //�K�[�h�U���̓K�[�h�ƃX�[�p�[�A�[�}�[������
            if (HealthStateJudge(DefState.�X�[�p�[�A�[�}�[)  || shock <= 0)
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
            if (damageType == AtEffectCon.Element.�a������)
            {
                GManager.instance.PlaySound("SlashDamage", transform.position);
            }
            else if (damageType == AtEffectCon.Element.�h�ˑ���)
            {
                GManager.instance.PlaySound("StabDamage", transform.position);
            }
            else if (damageType == AtEffectCon.Element.�Ō�����)
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
            else if (damageType == AtEffectCon.Element.������)
            {
                GManager.instance.PlaySound("HolyDamage", transform.position);
            }
            else if (damageType == AtEffectCon.Element.�ő���)
            {
                GManager.instance.PlaySound("DarkDamage", transform.position);
            }
            else if (damageType == AtEffectCon.Element.������)
            {


                GManager.instance.PlaySound("FireDamage", transform.position);
            }
            else if (damageType == AtEffectCon.Element.������)
            {

                GManager.instance.PlaySound("ThunderDamage", transform.position);
            }
        }
        public void Die()
        {
            _charaCon.Die();
        }


        /// <summary>
        /// �w���X�̏�ԕύX���郁�\�b�h
        /// isEnd���^�Ȃ��Ԃ��I��点��
        /// </summary>
        /// <param name="isEnd"></param>
        /// <param name="changeState"></param>
        public void HealthStateChange(bool isEnd, DefState changeState)
        {
            //����
            if (isEnd)
            {
                //�����Ȃ���͏���
                //�`�F���W�Ώۂ̔��]�r�b�g�ɂ���
                //001001 �Ɓ@01�ł͌����ς��Ȃ��H�@����s�ǂɒ���
                _defData.state &= ~changeState;
            }
            //����
            else
            {
                //�ǂ��炩�Е������A�̉��Z
                //�`�F���W�Ώۂ�����
                _defData.state |= changeState;
            }


        }

        /// <summary>
        /// �w���X�̏�Ԃ����Ă͂܂邩�̃r�b�g���Z�̌��ʂ�Ԃ��Ă���郁�\�b�h
        /// </summary>
        /// <returns></returns>
        bool HealthStateJudge(DefState judgeState)
        {
            //����0�ȏ�ɂȂ邩
            return (_defData.state & judgeState) > 0;
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