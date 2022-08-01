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
        [Header("このオブジェクトが魔法であるか")]
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
                //アタックデータをセットするメソッドとかほしい
                //ヒット時ダメージ計算でヒット時やるか
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
                //罠のダメージ
            }
        }


        /// <summary>
        /// ダメージを受けた時に呼ばれる
        /// </summary>
        /// <param name="damage">失われる体力ポイントの量</param>
        /// <param name="instigator">ダメージを引き起こしたオブジェクト</param>
        /// <param name="flickerDuration">ダメージを受けた後、オブジェクトが明滅する時間（秒）を指定します。</param>
        /// <param name="invincibilityDuration">ヒット後の短い無敵時間の長さ。</param>
        public void Damage(AttackData _damageData, GameObject instigator, float flickerDuration,
            float invincibilityDuration, Vector3 damageDirection,bool back,MyWakeUp.StunnType stunnState)
        {

                     // オブジェクトが無敵であれば、何もせずに終了します。
            if (TemporarilyInvulnerable || Invulnerable || ImmuneToDamage || PostDamageInvulnerable)
            {
                OnHitZero?.Invoke();
                return;
            }
			// すでに0を下回っている場合は、何もせずに終了します。
            if ((CurrentHealth <= 0) && (InitialHealth != 0))
            {
                return;
            }
			int damage = 0;

            if (_defender == MyDamageOntouch.TypeOfSubject.Enemy)
            {
                //アタックデータをセットするメソッドとかほしい
                //ヒット時ダメージ計算でヒット時やるか
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
                //オブジェクトがダメージ受ける
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

            // Projectile、Player、Enemiesとの衝突を防ぐことができます。
            if (invincibilityDuration > 0)
            {
                EnablePostDamageInvulnerability();
                StartCoroutine(DisablePostDamageInvulnerability(invincibilityDuration));
            }

            // ダメージが引き金になるイベント
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

            // ヘルスバー更新
            UpdateHealthBar(true);

            if (_defender != MyDamageOntouch.TypeOfSubject.Gimic && stunnState != MyWakeUp.StunnType.notStunned)
            {
                if(!(stunnState != MyWakeUp.StunnType.Down && CurrentHealth <= 0))
                {
                    //吹き飛ばしじゃなくかつ命尽きてる時以外なら吹き飛ばしはする
                    //スタン開始。
                    //パリィと弾かれは処理を共通化する
                    GetComponent<MyWakeUp>().StartStunn(stunnState);
                }

            }


            // 体力ゼロの時
            if (CurrentHealth <= 0)
            {
                // ヘルスを0にします。（ヘルスバーに便利です。）
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
			//////Debug.log($"ガード中か否か{guardHit}");

				//	GManager.instance.equipWeapon.hitLimmit--;

			//	Equip _damageData;
				byte _attackType = 0;
			    float mainDamage = 0;

			//////Debug.log("終了");
			float damage = 0;//バフデバフ処理用にdamageとして保持する
				float mValue = GManager.instance.equipWeapon.mValue;
            //float damage;//バフデバフ処理用にdamageとして保持する
            if (_defData.isGuard && !back)
            {
                //ガード時
                if (_damageData.phyAtk > 0)
                {
                    mainDamage = _damageData.phyAtk;
                    //斬撃刺突打撃を管理
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
                        //						Debug.Log("皿だ");
                        if (GManager.instance.equipWeapon.shock >= 40)
                        {
                            _damageData.isHeavy = true;
                        }
                        _attackType = 4;
                    }

                }
                //神聖
                if (_damageData.holyAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.holyAtk, 2) * mValue) / (_damageData.holyAtk + _defData.holyDef) * ((100 - _defData.holyCut) / 100);

                    if (_damageData.holyAtk > mainDamage)
                    {
                        _attackType = 8;
                        mainDamage = _damageData.holyAtk;

                    }
                }
                //闇
                if (_damageData.darkAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.darkAtk, 2) * mValue) / (_damageData.darkAtk + _defData.darkDef) * ((100 - _defData.darkCut) / 100);
                    if (_damageData.holyAtk > mainDamage)
                    {
                        _damageData._attackType = 16;
                        mainDamage = _damageData.darkAtk;
                    }
                }
                //炎
                if (_damageData.fireAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.fireAtk, 2) * mValue) / (_damageData.fireAtk + _defData.fireDef) * ((100 - _defData.fireCut) / 100);
                    if (_damageData.fireAtk > mainDamage)
                    {
                        _damageData._attackType = 32;
                        mainDamage = _damageData.fireAtk;
                    }
                }
                //雷
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
                    //斬撃刺突打撃を管理
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
                        //						Debug.Log("皿だ");
                        if (GManager.instance.equipWeapon.shock >= 40)
                        {
                            _damageData.isHeavy = true;
                        }

                    }

                }
                //神聖
                if (_damageData.holyAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.holyAtk, 2) * mValue) / (_damageData.holyAtk + _defData.holyDef);

                    if (_damageData.holyAtk > mainDamage)
                    {
                        _attackType = 8;
                        mainDamage = _damageData.holyAtk;

                    }
                }
                //闇
                if (_damageData.darkAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.darkAtk, 2) * mValue) / (_damageData.darkAtk + _defData.darkDef);
                    if (_damageData.holyAtk > mainDamage)
                    {
                        _damageData._attackType = 16;
                        mainDamage = _damageData.darkAtk;
                    }
                }
                //炎
                if (_damageData.fireAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.fireAtk, 2) * mValue) / (_damageData.fireAtk + _defData.fireDef);
                    if (_damageData.fireAtk > mainDamage)
                    {
                        _damageData._attackType = 32;
                        mainDamage = _damageData.fireAtk;
                    }
                }
                //雷
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

                //バックアタック

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
                
                //アタックデータをセットするメソッドとかほしい
                //ヒット時ダメージ計算でヒット時やるか
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
				//オブジェクトがダメージ受ける
			}




			return Mathf.Floor(damage * GManager.instance.attackBuff);



		}

        public MyWakeUp.StunnType ArmorCheck(float shock,bool isBlow,bool isBack)
        {



            if (_defender == MyDamageOntouch.TypeOfSubject.Enemy)
            {
                //アタックデータをセットするメソッドとかほしい
                //ヒット時ダメージ計算でヒット時やるか
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
                //オブジェクトがダメージ受ける
            }
            //バグ消しのリターン
            return MyWakeUp.StunnType.Falter;
            
        }



    }
}