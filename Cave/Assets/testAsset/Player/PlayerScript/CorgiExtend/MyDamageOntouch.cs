using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine.Serialization;
using UnityEngine.AddressableAssets;

namespace MoreMountains.CorgiEngine
{
    public class MyDamageOntouch : DamageOnTouch
    {
        [HideInInspector]
        public AttackData _attackData = new AttackData();

        [SerializeField]
        [Header("���̃I�u�W�F�N�g�����@�ł��邩")]
        TypeOfSubject _attacker;

        [SerializeField]
        ControllAbillity _cCon;

        /// <summary>
        /// ���L��
        /// </summary>
        [SerializeField]
        new GameObject Owner;

        [SerializeField]
        protected new MyHealth _health;
        protected new MyHealth _colliderHealth;

        [HideInInspector]
        public List<MyHealth> _restoreHealth = new List<MyHealth>();

        List<int> _restoreCount = new List<int>();
        /// <summary>
        /// �Փ˂����w���X�̔ԍ�
        /// </summary>
        int collideNum;

        /// <summary>
        /// ���̍U�����I�������j�󂷂�Ƃ����t���O
        /// �G�Ƀq�b�g������^�ɂȂ莩���j�󂷂�
        /// </summary>
        public bool isBreake;

        /// <summary>
        /// �U�������̂̃^�C�v
        /// </summary>
        public enum TypeOfSubject
        {
            Enemy,
            Player,
            Magic,
            Gimic
        }

        protected override void Awake()
        {
            base.Awake();


           


            DamageTakenInvincibilityDuration = 0.15f;
            DamageCausedKnockbackType = KnockbackStyles.AddForce;
            DamageCausedKnockbackDirection = CausedKnockbackDirections.BasedOnOwnerPosition;
            InvincibilityDuration = 0.25f;
         //   _attackData
         //   _health = Owner.gameObject.GetComponent<MyHealth>();
        }
        protected override void OnCollideWithDamageable(Health health)
        {




            //�w��ɓ����������ǂ������m�F���邽�߂̃A�^���n���e�C�������W
            float basePosition = 0;


            //�^����_���[�W���v�Z

                _cCon.DamageCalc();
                basePosition = transform.position.x;

            float enemyPosi = _colliderHealth.transform.position.x;
            bool back = false;
            bool isRight = false;

            //�G�������Ă����
            int enemyDirection = (int)Mathf.Sign(_colliderHealth.transform.localScale.x);


            //�_���[�W�����x�̒��S�Ɣ�e��������̃��[�J���X�P�[���̐����Ŋm�F
            //������������E�œ��Ă������
            if ((basePosition <= enemyPosi && enemyDirection > 0) ||
                //�����������
                (basePosition > enemyPosi && enemyDirection < 0))
            {

                isRight = basePosition < enemyPosi;

                back = true;
                _attackData.actionData.shock *= 1.05f;
            }
            else
            {
          //      Debug.Log($"fh{basePosition}{enemyPosi}");
                isRight = basePosition < enemyPosi;
                back = false;
            }

            //�A�[�}�[���𐮐���
            _attackData.actionData.shock = Mathf.Floor(_attackData.actionData.shock);
            
            
            
            // �Փ˂������肪 CorgiController �̏ꍇ�A�m�b�N�o�b�N�͂�K�p����
            _colliderCorgiController = health.gameObject.MMGetComponentNoAlloc<CorgiController>();

            _colliderHealth.GuardReport();

            //�p���B�������邩�ǂ���
            if (!back && !_attackData.disParry && _colliderHealth._parryNow)
            {

                //�p���B�����������Ȃ�

                bool ParryDown = false;

                //�v���C���[���G�l�~�[�Ȃ�p���B
                if((int)_attacker <= 1)
                {
                    //�X�^�~�i��

                    //

                    //�G���v���C���[�ɍU���������A�_�E�����邩���͂���B
                    ParryDown = _health.ParryArmorCheck();
               //�G�ȊO�p���B���ꂽ��X�^��
 �@�@�@�@�@�@�@ ParryStunn(ParryDown);
                }
 
                //�p���B��������̓{�[�i�X

                    _colliderHealth.ParryStart(ParryDown);

                return;
            }
            MyWakeUp.StunnType stunnState = ApplyDamageCausedKnockback(back,isRight);



            OnHitDamageable?.Invoke();

            HitDamageableFeedback?.PlayFeedbacks(this.transform.position);

            if ((FreezeFramesOnHitDuration > 0) && (Time.timeScale > 0))
            {
                MMFreezeFrameEvent.Trigger(Mathf.Abs(FreezeFramesOnHitDuration));
            }

            GameObject attacker = null;
            //�w���X�ɓn���U���҂̃I�u�W�F�N�g���l����
            if(_attacker == TypeOfSubject.Magic)
            {

            }
            else if (_attacker == TypeOfSubject.Gimic)
            {

            }
            else
            {
                attacker = transform.root.gameObject;
            }


            // �Ԃ��������̂Ƀ_���[�W��^����B
            //�t���b�J�[�͖��Ŏ��Ԃ�����_���[�W�㖳�G���ԂƓ����ł���
            _colliderHealth.Damage(_attackData, attacker, InvincibilityDuration, InvincibilityDuration, _damageDirection,back,stunnState);

            if (_colliderHealth.CurrentHealth <= 0)
            {
                OnKill?.Invoke();
            }

            //�����Ńw���X�����_���[�W�v�Z�ĂԁH

            SelfDamage(DamageTakenEveryTime + DamageTakenDamageable);

            if (isBreake)
            {
                Addressables.ReleaseInstance(this.gameObject);
            }
        }

        protected override void ApplyDamageTakenKnockback()
        {
            if ((_corgiController != null) && (DamageTakenKnockbackForce != Vector2.zero) && (!_health.Invulnerable) && (!_health.PostDamageInvulnerable) && (!_health.ImmuneToKnockback))
            {
                _knockbackForce.x = DamageCausedKnockbackForce.x;
                if (DamageTakenKnockbackDirection == TakenKnockbackDirections.BasedOnSpeed)
                {
                    Vector2 totalVelocity = _corgiController.Speed + _velocity;
                    _knockbackForce.x *= -1 * Mathf.Sign(totalVelocity.x);
                }
                if (DamageTakenKnockbackDirection == TakenKnockbackDirections.BasedOnDamagerPosition)
                {
                    Vector2 relativePosition = _corgiController.transform.position - _collidingCollider.bounds.center;
                    //�m�b�N�o�b�N����͂Ƀ_���[�W�̈�ɏՓ˂����R���C�_�[�̒����Ǝ����̈ʒu�Ŋ���o���������������Ă�
                    _knockbackForce.x *= Mathf.Sign(relativePosition.x);
                }

                _knockbackForce.y = DamageCausedKnockbackForce.y;

                if (DamageTakenKnockbackType == KnockbackStyles.SetForce)
                {
                    _corgiController.SetForce(_knockbackForce);
                }
                if (DamageTakenKnockbackType == KnockbackStyles.AddForce)
                {
                    _corgiController.AddForce(_knockbackForce);
                }
            }
        }

        /// <summary>
        /// �������Փˎ��̏�����
        /// </summary>
        /// <param name="collider"></param>
        protected override void Colliding(Collider2D collider)
        {

            if (!this.isActiveAndEnabled)
            {
                return;
            }

            // �Փ˂��Ă���I�u�W�F�N�g���������X�g�Ɋ܂܂�Ă���ꍇ�́A���������ɏI�����܂��B
            if (_ignoredGameObjects.Contains(collider.gameObject))
            {
                Debug.Log($"{this.gameObject.name}��{collider.transform.gameObject.name}");
                return;
            }

            // �Փ˂��Ă�����̂��^�[�Q�b�g���C���Ɋ܂܂�Ȃ��ꍇ�́A���������ɏI�����܂��B
            if (!MMLayers.LayerInLayerMask(collider.gameObject.layer, TargetLayerMask))
            {
                return;
            }

            //�����܂őΏۊO��e��

            _collidingCollider = collider;


            //�܂�łȂ��Ȃ�
            if (_restoreHealth.Count != 0)
            {
                collideNum = 100;
                for (int i = 0;i<_restoreHealth.Count;i++)
                {
                    //�Q�[���I�u�W�F�N�g���L���b�V�����Ă���̂Ȃ�
                    if (_restoreHealth[i].gameObject == collider.gameObject)
                    {
                        _colliderHealth = _restoreHealth[i];
                        collideNum = i;
                        break;
                    }
                }
                //�܂܂�ĂȂ��ꍇ
                if (collideNum == 100)
                {
                    _colliderHealth = collider.gameObject.MMGetComponentNoAlloc<MyHealth>();
                    _restoreHealth.Add(_colliderHealth);
                    collideNum = _restoreHealth.Count - 1;
                     _restoreCount.Add(0);
                }
            }
            else
            {
                _colliderHealth = collider.gameObject.MMGetComponentNoAlloc<MyHealth>();
                _restoreHealth.Add(_colliderHealth);
                collideNum = 0;
                _restoreCount.Add(0);
            }

            //�����Ńw���X�̖��G�m�F���邩
            if (_colliderHealth.InvulnerableCheck())
            {
                //   Debug.Log($"{this.gameObject.name}��{collider.transform.gameObject.name}");
                return;
            }

            if (_restoreCount[collideNum] >= _attackData.actionData._hitLimit)
            {
                Debug.Log($"{this.gameObject.name}��{collider.transform.gameObject.name}");
                return;
            }
            else
            {
                _restoreCount[collideNum]++;
            }
            

            OnHit?.Invoke();

            // �Ԃ�����̂�������̂ł����
            if ((_colliderHealth != null) && (_colliderHealth.enabled))
            {
                if (_colliderHealth.CurrentHealth > 0)
                {
                 
                    OnCollideWithDamageable(_colliderHealth);
                }
            }
            // �Ԃ�����̂����Ȃ��̂ł����
            else
            {

                OnCollideWithNonDamageable();
            }
        }

        /// <summary>
        /// �G���m�b�N�o�b�N�����郁�\�b�h
        /// </summary>
        protected virtual MyWakeUp.StunnType ApplyDamageCausedKnockback(bool isBack,bool isRight)
        {//
            _colliderCorgiController.SetForce(Vector2.zero);
            MyWakeUp.StunnType result = _colliderHealth.ArmorCheck(_attackData.actionData.shock,_attackData.actionData.blowPower != Vector2.zero, isBack);

            bool isAirDown = false;

            //�󒆓���_�E������������Ȃ�
            if (_colliderHealth.AirDownJudge(result))
            {

                isAirDown = true;
                result = MyWakeUp.StunnType.Down;
                _colliderHealth.AirDown();
            }

            if (result == MyWakeUp.StunnType.Down)
            {

                if (!isAirDown)
                {
                    float blowDire = isRight ? _attackData.actionData.blowPower.x : _attackData.actionData.blowPower.x * -1;

                    DamageCausedKnockbackForce.Set(blowDire, _attackData.actionData.blowPower.y);
                }
                else
                {
                    
                        //������������
                    DamageCausedKnockbackForce.Set(0, 60);
                }
            }
            else if(result == MyWakeUp.StunnType.Falter)
            {
                //�A�j���œ�����

                    float blowDire = isRight ? 160 : -160;

                DamageCausedKnockbackForce.Set(blowDire, 0);

                //������΂��Ȃ��Ƃ�BlowPower�ŋ��ݕ������m�F
                //��{�I��0�i�����l�j�Ȃ畁�ʂɐ�����΂�

                //���ޕ�����厖�ł��悱����
                // DamageCausedKnockbackForce.Set(fDire, 0);
                //  DamageCausedKnockbackForce.Set(0, 0);
            }
            else
            {
                DamageCausedKnockbackForce.Set(0,0);
            }

            if ((_colliderCorgiController != null) && (DamageCausedKnockbackForce != Vector2.zero) && (!_colliderHealth.Invulnerable) && (!_colliderHealth.PostDamageInvulnerable) && (!_colliderHealth.ImmuneToKnockback))
            {
                _knockbackForce.x = DamageCausedKnockbackForce.x;

                _knockbackForce.y = DamageCausedKnockbackForce.y;

                if (DamageCausedKnockbackType == KnockbackStyles.SetForce)
                {
                    _colliderCorgiController.SetForce(_knockbackForce);
                }
                if (DamageCausedKnockbackType == KnockbackStyles.AddForce)
                {
                    _colliderCorgiController.AddForce(_knockbackForce);
                }
            }
            return result;
        }


        public void ParryStunn(bool isDown = false)
        {


            _cCon.StartStun(MyWakeUp.StunnType.Parried);
            
        }

        /// <summary>
        /// �Փˏ󋵂����Z�b�g
        /// </summary>
        public void CollidRestoreResset()
        {
            _restoreHealth.Clear();
            _restoreCount.Clear();
        }


    }
}