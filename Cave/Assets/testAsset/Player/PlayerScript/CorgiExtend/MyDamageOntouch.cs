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
        public AttackData _attackData = new AttackData();

        [SerializeField]
        [Header("このオブジェクトが魔法であるか")]
        TypeOfSubject _attacker;

        EnemyAIBase eData;
        protected PlyerController pCon;
        [SerializeField]
        protected new MyHealth _health;
        protected new MyHealth _colliderHealth;

        [HideInInspector]
        public List<MyHealth> _restoreHealth = new List<MyHealth>();

        List<int> _restoreCount = new List<int>();
        /// <summary>
        /// 衝突したヘルスの番号
        /// </summary>
        int collideNum;

        /// <summary>
        /// 攻撃する主体のタイプ
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

            if (_attacker == TypeOfSubject.Enemy)
            {
                Owner = transform.root.gameObject;
                
                eData = Owner.GetComponent<EnemyAIBase>();
                //アタックデータをセットするメソッドとかほしい
                //ヒット時ダメージ計算でヒット時やるか
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
         //   _attackData
         //   _health = Owner.gameObject.GetComponent<MyHealth>();
        }
        protected override void OnCollideWithDamageable(Health health)
        {
         //   Debug.Log($"知りた");
            //背後に当たったかどうかを確認するためのアタリハンテイ検査座標
            float basePosition = 0;
      //      Debug.Log($"asdf{_attacker}");
            //与えるダメージを計算
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
                basePosition = transform.position.x;

            }
            else
            {
                basePosition = transform.position.x;
                //罠のダメージ
            }
            bool back = false;
            //当たったやつが右にいるかどうか
            bool isRight = false; ;

            //ダメージ判定のxの中心と被弾した相手のローカルスケールの正負で確認
            //当たったやつが右で当てたやつが左
            if ((basePosition <= _colliderHealth.transform.position.x && _colliderHealth.transform.localScale.x > 0) ||
                //当たったやつが
                (basePosition > _colliderHealth.transform.position.x && _colliderHealth.transform.localScale.x < 0))
            {

                isRight = basePosition < _colliderHealth.transform.position.x;

                back = true;
                _attackData.shock *= 1.05f;
            }
            else
            {
          //      Debug.Log($"fh{basePosition}{_colliderHealth.transform.position.x}");
                isRight = basePosition < _colliderHealth.transform.position.x;
                back = false;
            }
            //アーマー削りを整数に
            _attackData.shock = Mathf.Floor(_attackData.shock);
            //isRight = (Owner.transform.position.x <= transform.position.x) ? true : false;
            // 衝突した相手が CorgiController の場合、ノックバック力を適用する
            _colliderCorgiController = health.gameObject.MMGetComponentNoAlloc<CorgiController>();

            _colliderHealth.GuardReport();

            //パリィ発生するかどうか
            if (!back && !_attackData.disParry && _colliderHealth._parryNow)
            {

                //パリィが発生したなら

                bool ParryDown = false;
                if(_attacker == TypeOfSubject.Enemy)
                {
                    //スタミナ回復

                    //

                    //敵がプレイヤーに攻撃した時、ダウンするかをはかる。
                    ParryDown = _health.ParryArmorCheck();

                }
                //敵以外パリィされたらスタン
 　　　　　　　 ParryStunn(ParryDown);
                //パリィした相手はボーナス

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


            // ぶつかったものにダメージを与える。
            //フリッカーは明滅時間だからダメージ後無敵時間と同じでいい
            _colliderHealth.Damage(_attackData, gameObject, InvincibilityDuration, InvincibilityDuration, _damageDirection,back,stunnState);

            if (_colliderHealth.CurrentHealth <= 0)
            {
                OnKill?.Invoke();
            }
            //ここでヘルスから被ダメージ計算呼ぶ？

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
                    //ノックバックする力にダメージ領域に衝突したコライダーの中央と自分の位置で割り出した方向をかけてる
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
        /// こいつが衝突時の処理か
        /// </summary>
        /// <param name="collider"></param>
        protected override void Colliding(Collider2D collider)
        {
          //  Debug.Log("最高");
            if (!this.isActiveAndEnabled)
            {
                return;
            }

            // 衝突しているオブジェクトが無視リストに含まれている場合は、何もせずに終了します。
            if (_ignoredGameObjects.Contains(collider.gameObject))
            {
                return;
            }

            // 衝突しているものがターゲットレイヤに含まれない場合は、何もせずに終了します。
            if (!MMLayers.LayerInLayerMask(collider.gameObject.layer, TargetLayerMask))
            {
                return;
            }

            //ここまで対象外を弾く

            _collidingCollider = collider;
            _colliderHealth = collider.gameObject.MMGetComponentNoAlloc<MyHealth>();

            //ここでヘルスの無敵確認するか
            if (_colliderHealth.InvulnerableCheck())
            {
                return;
            }

            //含んでないなら
            if (_restoreHealth.Count != 0)
            {
                collideNum = 100;
                for (int i = 0;i<_restoreHealth.Count;i++)
                {
                    if (_restoreHealth[i] == _colliderHealth)
                    {
                        collideNum = i;
                        break;
                    }
                }
                //含まれてない場合
                if (collideNum == 100)
                {
                    _restoreHealth.Add(_colliderHealth);
                    collideNum = _restoreHealth.Count - 1;
                     _restoreCount.Add(0);
                }
            }
            else
            {
                _restoreHealth.Add(_colliderHealth);
                collideNum = 0;
                _restoreCount.Add(0);
            }

            if (_restoreCount[collideNum] >= _attackData._hitLimit)
            {
                return;
            }
            else
            {
                _restoreCount[collideNum]++;
            }
            

            OnHit?.Invoke();

            // ぶつかるものが壊れるものであれば
            if ((_colliderHealth != null) && (_colliderHealth.enabled))
            {
                if (_colliderHealth.CurrentHealth > 0)
                {
                 
                    OnCollideWithDamageable(_colliderHealth);
                }
            }
            // ぶつかるものが壊れないのであれば
            else
            {

                OnCollideWithNonDamageable();
            }
        }

        /// <summary>
        /// 敵をノックバックさせるメソッド
        /// </summary>
        protected virtual MyWakeUp.StunnType ApplyDamageCausedKnockback(bool isBack,bool isRight)
        {//
            _colliderCorgiController.SetForce(Vector2.zero);
            MyWakeUp.StunnType result = _colliderHealth.ArmorCheck(_attackData.shock,_attackData.isBlow, isBack);

            bool isAirDown = false;

            //空中特殊ダウンが発生するなら
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
                    float blowDire = isRight ? _attackData.blowPower.x : _attackData.blowPower.x * -1;

                    DamageCausedKnockbackForce.Set(blowDire, _attackData.blowPower.y);
                }
                else
                {
                    
                        //少しだけ浮く
                    DamageCausedKnockbackForce.Set(0, 60);
                }
            }
            else if(result == MyWakeUp.StunnType.Falter)
            {
                //アニメで動かす

                    float blowDire = isRight ? 160 : -160;

                DamageCausedKnockbackForce.Set(blowDire, 0);

                //吹き飛ばさないときBlowPowerで怯み方向を確認
                //基本的に0（初期値）なら普通に吹き飛ばす

                //怯む方向を大事ですよこいつ
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

                if (_attacker == TypeOfSubject.Enemy)
                {
                    if (isDown)
                    {
                        eData._wakeup.StartStunn(MyWakeUp.StunnType.Parried);
                    }
                }
                else if (_attacker == TypeOfSubject.Player)
                {
                    pCon._wakeup.StartStunn(MyWakeUp.StunnType.Parried);
                }
                else if (_attacker == TypeOfSubject.Magic)
                {

                }
                else
                {

                }
            
        }

        /// <summary>
        /// 衝突状況をリセット
        /// </summary>
        public void CollidRestoreResset()
        {
            _restoreHealth.Clear();
            _restoreCount.Clear();
        }


    }
}