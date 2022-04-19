using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine.Serialization;


namespace MoreMountains.CorgiEngine
{
    public class MyDamageOntouch : DamageOnTouch
    {
        [HideInInspector]
        public AttackData _attackData;

        [SerializeField]
        [Header("���̃I�u�W�F�N�g�����@�ł��邩")]
        TypeOfSubject _attacker;

        EnemyAIBase eData;
        protected PlyerController pCon;

        protected new MyHealth _health;
        protected new MyHealth _colliderHealth;
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
            _health = this.gameObject.GetComponent<MyHealth>();
            if (_attacker == TypeOfSubject.Enemy)
            {
                Owner = transform.root.gameObject;
                
                eData = Owner.GetComponent<EnemyAIBase>();
                //�A�^�b�N�f�[�^���Z�b�g���郁�\�b�h�Ƃ��ق���
                //�q�b�g���_���[�W�v�Z�Ńq�b�g����邩
            }
            else if (_attacker == TypeOfSubject.Player)
            {
                Owner = GManager.instance.Player;
                pCon = Owner.GetComponent<PlyerController>();
            }
            else if (_attacker == TypeOfSubject.Magic)
            {
                Owner = this.gameObject;
            }
            else
            {

            }
            DamageTakenInvincibilityDuration = 0.15f;
            DamageCausedKnockbackType = KnockbackStyles.AddForce;
            DamageCausedKnockbackDirection = CausedKnockbackDirections.BasedOnOwnerPosition;
        }
        protected override void OnCollideWithDamageable(Health health)
        {
            //�w��ɓ����������ǂ������m�F���邽�߂̃A�^���n���e�C�������W
            float basePosition = 0;

            //�^����_���[�W���v�Z
            if (_attacker == TypeOfSubject.Enemy)
            {
                eData.DamageCalc();
                basePosition = transform.position.x;
            }
            else if (_attacker == TypeOfSubject.Player)
            {
                pCon.DamageCalc(GManager.instance.useAtValue.isShield);
                basePosition = transform.position.x;
            }
            else if (_attacker == TypeOfSubject.Magic)
            {
                basePosition = _collidingCollider.bounds.center.x;

            }
            else
            {
                basePosition = transform.position.x;
                //㩂̃_���[�W
            }
            bool back = false;
            //�E�ɂ��邩�ǂ���
            bool isRight = false; ;

            //�_���[�W�����x�̒��S�Ɣ�e��������̃��[�J���X�P�[���̐����Ŋm�F
            if ((basePosition < _health.transform.position.x && _health.transform.localScale.x > 0) ||
                (basePosition > _health.transform.position.x && _health.transform.localScale.x < 0))
            {
                back = true;
                _attackData.shock *= 1.05f;
            }
            //�A�[�}�[���𐮐���
            _attackData.shock = Mathf.Floor(_attackData.shock);
            isRight = (_health.transform.position.x <= transform.position.x) ? true : false;
            // �Փ˂������肪 CorgiController �̏ꍇ�A�m�b�N�o�b�N�͂�K�p����
            _colliderCorgiController = health.gameObject.MMGetComponentNoAlloc<CorgiController>();

            //������΂�����
            MyWakeUp.StunnType stunnState = ApplyDamageCausedKnockback(back);

            OnHitDamageable?.Invoke();

            HitDamageableFeedback?.PlayFeedbacks(this.transform.position);

            if ((FreezeFramesOnHitDuration > 0) && (Time.timeScale > 0))
            {
                MMFreezeFrameEvent.Trigger(Mathf.Abs(FreezeFramesOnHitDuration));
            }


            // �Ԃ��������̂Ƀ_���[�W��^����B
            //�t���b�J�[�͖��Ŏ��Ԃ�����_���[�W�㖳�G���ԂƓ����ł���
            _colliderHealth.Damage(_attackData, gameObject, InvincibilityDuration, InvincibilityDuration, _damageDirection,back,stunnState);

            if (_colliderHealth.CurrentHealth <= 0)
            {
                OnKill?.Invoke();
            }
            //�����Ńw���X�����_���[�W�v�Z�ĂԁH

            SelfDamage(DamageTakenEveryTime + DamageTakenDamageable);

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
                return;
            }

            // �Փ˂��Ă�����̂��^�[�Q�b�g���C���Ɋ܂܂�Ȃ��ꍇ�́A���������ɏI�����܂��B
            if (!MMLayers.LayerInLayerMask(collider.gameObject.layer, TargetLayerMask))
            {
                return;
            }

            //�����܂őΏۊO��e��

            _collidingCollider = collider;
            _colliderHealth = collider.gameObject.MMGetComponentNoAlloc<MyHealth>();

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
        protected virtual MyWakeUp.StunnType ApplyDamageCausedKnockback(bool isBack)
        {
            MyWakeUp.StunnType result = _health.ArmorCheck(_attackData.shock,_attackData.isBlow, isBack);

            if (result == MyWakeUp.StunnType.Down)
            {
                DamageCausedKnockbackForce.Set(_attackData.blowPower.x, _attackData.blowPower.y);
            }
            else if(result == MyWakeUp.StunnType.Faltter)
            {

                float fDire = 10;

                //������΂��Ȃ��Ƃ�BlowPower�ŋ��ݕ������m�F
                //��{�I��0�i�����l�j�Ȃ畁�ʂɐ�����΂�
                if (_attackData.blowPower.x < 0)
                {
                    fDire *= -1;
                }
                if (isBack)
                {
                    fDire *= -1;

                }

                //���ޕ�����厖�ł��悱����
                DamageCausedKnockbackForce.Set(fDire, 0);
            }
            else
            {
                DamageCausedKnockbackForce.Set(0,0);
            }

            if ((_colliderCorgiController != null) && (DamageCausedKnockbackForce != Vector2.zero) && (!_colliderHealth.Invulnerable) && (!_colliderHealth.PostDamageInvulnerable) && (!_colliderHealth.ImmuneToKnockback))
            {
                _knockbackForce.x = DamageCausedKnockbackForce.x;
              /*
                if (DamageCausedKnockbackDirection == CausedKnockbackDirections.BasedOnSpeed)
                {
                    Vector2 totalVelocity = _colliderCorgiController.Speed + _velocity;
                    _knockbackForce.x *= -1 * Mathf.Sign(totalVelocity.x);
                }
                if (DamageCausedKnockbackDirection == CausedKnockbackDirections.BasedOnOwnerPosition)
                {
                    if (Owner == null) { Owner = this.gameObject; }
                    Vector2 relativePosition = _colliderCorgiController.transform.position - Owner.transform.position;
                    _knockbackForce.x *= Mathf.Sign(relativePosition.x);
                }
              */
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






    }
}