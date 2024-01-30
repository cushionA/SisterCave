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
        [Header("このオブジェクトが魔法であるか")]
        TypeOfSubject _attacker;

        [SerializeField]
        ControllAbillity _cCon;

        /// <summary>
        /// 所有者
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
        /// 衝突したヘルスの番号
        /// </summary>
        int collideNum;

        /// <summary>
        /// この攻撃が終わったら破壊するというフラグ
        /// 敵にヒットした後真になり自らを破壊する
        /// </summary>
        public bool isBreake;

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


           


            DamageTakenInvincibilityDuration = 0.15f;
            DamageCausedKnockbackType = KnockbackStyles.AddForce;
            DamageCausedKnockbackDirection = CausedKnockbackDirections.BasedOnOwnerPosition;
            InvincibilityDuration = 0.25f;
         //   _attackData
         //   _health = Owner.gameObject.GetComponent<MyHealth>();
        }
        protected override void OnCollideWithDamageable(Health health)
        {




            //背後に当たったかどうかを確認するためのアタリハンテイ検査座標
            float basePosition = 0;


            //与えるダメージを計算

                _cCon.DamageCalc();
                basePosition = transform.position.x;

            float enemyPosi = _colliderHealth.transform.position.x;
            bool back = false;
            bool isRight = false;

            //敵が向いてる方向
            int enemyDirection = (int)Mathf.Sign(_colliderHealth.transform.localScale.x);


            //ダメージ判定のxの中心と被弾した相手のローカルスケールの正負で確認
            //当たったやつが右で当てたやつが左
            if ((basePosition <= enemyPosi && enemyDirection > 0) ||
                //当たったやつが
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

            //アーマー削りを整数に
            _attackData.actionData.shock = Mathf.Floor(_attackData.actionData.shock);
            
            
            
            // 衝突した相手が CorgiController の場合、ノックバック力を適用する
            _colliderCorgiController = health.gameObject.MMGetComponentNoAlloc<CorgiController>();

            _colliderHealth.GuardReport();

            //パリィ発生するかどうか
            if (!back && !_attackData.disParry && _colliderHealth._parryNow)
            {

                //パリィが発生したなら

                bool ParryDown = false;

                //プレイヤーかエネミーならパリィ
                if((int)_attacker <= 1)
                {
                    //スタミナ回復

                    //

                    //敵がプレイヤーに攻撃した時、ダウンするかをはかる。
                    ParryDown = _health.ParryArmorCheck();
               //敵以外パリィされたらスタン
 　　　　　　　 ParryStunn(ParryDown);
                }
 
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

            GameObject attacker = null;
            //ヘルスに渡す攻撃者のオブジェクトを考える
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


            // ぶつかったものにダメージを与える。
            //フリッカーは明滅時間だからダメージ後無敵時間と同じでいい
            _colliderHealth.Damage(_attackData, attacker, InvincibilityDuration, InvincibilityDuration, _damageDirection,back,stunnState);

            if (_colliderHealth.CurrentHealth <= 0)
            {
                OnKill?.Invoke();
            }

            //ここでヘルスから被ダメージ計算呼ぶ？

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

            if (!this.isActiveAndEnabled)
            {
                return;
            }

            // 衝突しているオブジェクトが無視リストに含まれている場合は、何もせずに終了します。
            if (_ignoredGameObjects.Contains(collider.gameObject))
            {
                Debug.Log($"{this.gameObject.name}が{collider.transform.gameObject.name}");
                return;
            }

            // 衝突しているものがターゲットレイヤに含まれない場合は、何もせずに終了します。
            if (!MMLayers.LayerInLayerMask(collider.gameObject.layer, TargetLayerMask))
            {
                return;
            }

            //ここまで対象外を弾く

            _collidingCollider = collider;


            //含んでないなら
            if (_restoreHealth.Count != 0)
            {
                collideNum = 100;
                for (int i = 0;i<_restoreHealth.Count;i++)
                {
                    //ゲームオブジェクトがキャッシュしてるものなら
                    if (_restoreHealth[i].gameObject == collider.gameObject)
                    {
                        _colliderHealth = _restoreHealth[i];
                        collideNum = i;
                        break;
                    }
                }
                //含まれてない場合
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

            //ここでヘルスの無敵確認するか
            if (_colliderHealth.InvulnerableCheck())
            {
                //   Debug.Log($"{this.gameObject.name}が{collider.transform.gameObject.name}");
                return;
            }

            if (_restoreCount[collideNum] >= _attackData.actionData._hitLimit)
            {
                Debug.Log($"{this.gameObject.name}が{collider.transform.gameObject.name}");
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
            MyWakeUp.StunnType result = _colliderHealth.ArmorCheck(_attackData.actionData.shock,_attackData.actionData.blowPower != Vector2.zero, isBack);

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
                    float blowDire = isRight ? _attackData.actionData.blowPower.x : _attackData.actionData.blowPower.x * -1;

                    DamageCausedKnockbackForce.Set(blowDire, _attackData.actionData.blowPower.y);
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


            _cCon.StartStun(MyWakeUp.StunnType.Parried);
            
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