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
        [Header("このオブジェクトが魔法であるか")]
        MyDamageOntouch.TypeOfSubject _defender;

        EnemyAIBase eData;
        PlyerController pCon;

        [HideInInspector]
        public Vector2 blowVector;

        [HideInInspector]
        public bool _parryNow;

        [HideInInspector]
        public bool _guardAttack;

        [HideInInspector]
        public bool _superArumor;


        /// <summary>
        /// 今吹き飛ばされてるか
        /// 吹き飛ばされてるなら押されたりするのから自由になれる
        /// </summary>
        [HideInInspector]
        public bool _blowNow;

        protected new MyCharacter _character;

        protected UltimateTextDamageManager um;

        protected void StatusSet()
        {

            if (_defender == MyDamageOntouch.TypeOfSubject.Enemy)
            {


                eData = GetComponent<EnemyAIBase>();

                um = EnemyManager.instance.um;
            }
            else if (_defender == MyDamageOntouch.TypeOfSubject.Player)
            {
                pCon = GetComponent<PlyerController>();
                um = EnemyManager.instance.um;
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
        /// 使用するコンポーネントを獲得しダメージを有効化し初期色を取得する。
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

            // アニメーターを獲得
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

            _autoRespawn = this.gameObject.GetComponent<AutoRespawn>();
            _controller = this.gameObject.GetComponent<CorgiController>();
            _healthBar = this.gameObject.GetComponent<MMHealthBar>();
            _collider2D = this.gameObject.GetComponent<Collider2D>();

            StatusSet();

            StoreInitialPosition();
            _initialized = true;
            //操作側でやる
            //   
            DamageEnabled();
            DisablePostDamageInvulnerability();
            UpdateHealthBar(false);
        }



        /// <summary>
        /// ダメージを受けた時に呼ばれる
        /// </summary>
        /// <param name="damage">失われる体力ポイントの量</param>
        /// <param name="instigator">ダメージを引き起こしたオブジェクト</param>
        /// <param name="flickerDuration">ダメージを受けた後、オブジェクトが明滅する時間（秒）を指定します。</param>
        /// <param name="invincibilityDuration">ヒット後の短い無敵時間の長さ。</param>
        public void Damage(AttackData _damageData, GameObject instigator, float flickerDuration,
            float invincibilityDuration, Vector3 damageDirection, bool back, MyWakeUp.StunnType stunnState)
        {

            // Debug.Log($"知りたい{stunnState}");

            // オブジェクトが無敵であれば、何もせずに終了します。

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

            damage = (int)Calc(_damageData, back);

        //  Debug.Log($"おせーて{damage}と{CurrentHealth}{stunnState}");
            if (um != null)
            {
               //   Debug.Log($"ｈｈ{um.name}{damage}");
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



            // 少しの間無敵に
            if (invincibilityDuration > 0)
            {
                EnablePostDamageInvulnerability();
                DisablePostDamageInvulnerabilityTasks(invincibilityDuration).Forget();
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

            if (_defender != MyDamageOntouch.TypeOfSubject.Gimic && stunnState != MyWakeUp.StunnType.notStunned && CurrentHealth > 0)
            {

                //吹き飛ばしじゃなくかつ命尽きてる時以外なら吹き飛ばしはする
                //スタン開始。
                //パリィと弾かれは処理を共通化する

                if (_defender == MyDamageOntouch.TypeOfSubject.Player)
                {
                    pCon.GravitySet(GManager.instance.pStatus.firstGravity);
                    pCon.MoveReset();
                    pCon._wakeup.StartStunn(stunnState);
                }
                else if (_defender == MyDamageOntouch.TypeOfSubject.Enemy)
                {
                    eData.AttackEnd(true);
                    eData._wakeup.StartStunn(stunnState);
                }
            }


            // 体力ゼロの時
            //死亡処理
            if (CurrentHealth <= 0)
            {
                // ヘルスを0にします。（ヘルスバーに便利です。）
                CurrentHealth = 0;

                Invulnerable = true;

                if (_character != null)
                {

                    if (stunnState == MyWakeUp.StunnType.Down)
                    {
                        stunnState = MyWakeUp.StunnType.BlowDie;
                    }
                    else
                    {
                        stunnState = MyWakeUp.StunnType.NDie;
                    }
                    // Debug.Log($"だの{stunnState}");
                    if (_defender == MyDamageOntouch.TypeOfSubject.Player)
                    {
                        pCon._wakeup.StartStunn(stunnState);
                    }
                    else if (_defender == MyDamageOntouch.TypeOfSubject.Enemy)
                    {
                        //体力ゼロの時点で感知されないレイヤーに
                        this.gameObject.layer = 15;

                        //ターゲットではなくなる
                        if (SManager.instance.target == this.gameObject)
                        {
                            SManager.instance.target = null;
                        }

                        //ターゲットリストから自分を削除
                            SManager.instance.RemoveEnemy(this.gameObject);

                        //エフェクトも消して
                        eData.TargetEffectCon(1);



                        //空飛ぶ敵はダウンして死ぬ
                        if ((eData.status.kind == EnemyStatus.KindofEnemy.Fly))
                        {
                            _controller.SetHorizontalForce(0);
                            stunnState = MyWakeUp.StunnType.BlowDie;
                        }

                        eData._wakeup.StartStunn(stunnState);


                    }
                }


            }
        }

        public float Calc(AttackData _damageData, bool back)
        {
            bool stab = false;
            //isDamage = true;
            //SetLayer(28);
            //////Debug.log($"ガード中か否か{guardHit}");

            //	GManager.instance.equipWeapon.hitLimmit--;

            //	Equip _damageData;



            //////Debug.log("終了");
            float damage = 0;//バフデバフ処理用にdamageとして保持する
            float mValue = _damageData.mValue;
            //float damage;//バフデバフ処理用にdamageとして保持する

            //ガード時
            if ((_defData.isGuard || _guardAttack) && !back)
            {  

                //ガード時
                if (_damageData.phyAtk > 0)
                {

                    //斬撃刺突打撃を管理
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
                        //						Debug.Log("皿だ");

                    }

                }
                //神聖
                if (_damageData.holyAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.holyAtk, 2) * mValue) / (_damageData.holyAtk + _defData.holyDef) * ((100 - _defData.holyCut) / 100);


                }
                //闇
                if (_damageData.darkAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.darkAtk, 2) * mValue) / (_damageData.darkAtk + _defData.darkDef) * ((100 - _defData.darkCut) / 100);

                }
                //炎
                if (_damageData.fireAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.fireAtk, 2) * mValue) / (_damageData.fireAtk + _defData.fireDef) * ((100 - _defData.fireCut) / 100);

                }
                //雷
                if (_damageData.thunderAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.thunderAtk, 2) * mValue) / (_damageData.thunderAtk + _defData.thunderDef) * ((100 - _defData.thunderCut) / 100);

                }

                if (_defender == MyDamageOntouch.TypeOfSubject.Enemy)
                {

                    eData.GuardSound();
                }
                else if (_defender == MyDamageOntouch.TypeOfSubject.Player)
                {
                    pCon.GuardSound();
                }
            }
            else
            {
           //     Debug.Log($"ｆｆ{_defData.isGuard}ｄ{!back}");
                if (_damageData.phyAtk > 0)
                {

                    //斬撃刺突打撃を管理
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
                        //						Debug.Log("皿だ");
                        if (_damageData.shock >= 40)
                        {
                            _damageData.isHeavy = true;
                        }

                    }

                }
                //神聖
                if (_damageData.holyAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.holyAtk, 2) * mValue) / (_damageData.holyAtk + _defData.holyDef);

                }
                //闇
                if (_damageData.darkAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.darkAtk, 2) * mValue) / (_damageData.darkAtk + _defData.darkDef);

                }
                //炎
                if (_damageData.fireAtk > 0)
                {
                    damage += (Mathf.Pow(_damageData.fireAtk, 2) * mValue) / (_damageData.fireAtk + _defData.fireDef);

                }
                //雷
                if (_damageData.thunderAtk > 0)
                {

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

                    //アタックデータをセットするメソッドとかほしい
                    //ヒット時ダメージ計算でヒット時やるか
                    DamageSound(_damageData._attackType, _damageData.isHeavy);


                }
                else if (_defender == MyDamageOntouch.TypeOfSubject.Player)
                {
                    DamageSound(_damageData._attackType, _damageData.isHeavy);
                }
                else if (_defender == MyDamageOntouch.TypeOfSubject.Magic)
                {

                }
                else
                {
                    //オブジェクトがダメージ受ける
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






            //    Debug.Log($"ええ{damage}");

            return Mathf.Floor(damage * GManager.instance.attackBuff);



        }

        /// <summary>
        /// スタンしてるかどうかを確認
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

            else if (_defender == MyDamageOntouch.TypeOfSubject.Enemy)
            {
                //アタックデータをセットするメソッドとかほしい
                //ヒット時ダメージ計算でヒット時やるか
                result = eData.ArmorControll(shock, isBlow, isBack);
            }
            else if (_defender == MyDamageOntouch.TypeOfSubject.Player)
            {
                result = pCon.ArmorControll(shock, isBlow, isBack, !GManager.instance.twinHand);
            }
            else if (_defender == MyDamageOntouch.TypeOfSubject.Magic)
            {

            }
            else
            {
                //オブジェクトがダメージ受ける
            }

            return result;
        }



        public EffectControllAbility.SelectState GetStanState()
        {
            int stanSt = 0;
            if (_character != null)
            {
               
                // Debug.Log($"だの{stunnState}");
                if (_defender == MyDamageOntouch.TypeOfSubject.Player)
                {
                    stanSt = pCon._wakeup.GetStanState();
                }
                else if (_defender == MyDamageOntouch.TypeOfSubject.Enemy)
                {


                   stanSt  = eData._wakeup.GetStanState();


                }
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
        /// 敵の場合はプレイヤーの方を向く、立ち上がり時
        /// </summary>
        public void ArmorReset()
        {



            if (_defender == MyDamageOntouch.TypeOfSubject.Enemy)
            {
                eData.ArmorReset();
            }
            else if (_defender == MyDamageOntouch.TypeOfSubject.Player)
            {
                pCon.ArmorReset();
            }
            else
            {
                //オブジェクトがダメージ受ける
            }

        }


        /// <summary>
        /// 自分がプレイヤーに攻撃した時にパリィ演出はいるかをはかる
        /// 攻撃側のヘルスで出る
        /// </summary>
        /// <returns></returns>
        public bool ParryArmorCheck()
        {


            if (_defender == MyDamageOntouch.TypeOfSubject.Enemy)
            {
                return eData.ParryArmorJudge();
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 攻撃された時に空中にいたら特殊ダウンする
        /// </summary>
        /// <param name="stunnState"></param>
        /// <returns></returns>
        public bool AirDownJudge(MyWakeUp.StunnType stunnState)
        {
            //空中にいる時は少し浮かせてダウンに
            //攻撃中はなんとか
            if (!_controller.State.IsGrounded)
            {
                if (_defender == MyDamageOntouch.TypeOfSubject.Player)
                {
                    if (stunnState == MyWakeUp.StunnType.Falter)
                    {
                        //      stunnState = MyWakeUp.StunnType.Down;
                        //吹き飛ばし処理
                        return true;
                    }
                    else if (stunnState == MyWakeUp.StunnType.notStunned)
                    {
                        //攻撃中かどうか
                        if (!pCon.AttackCheck())
                        {
                            return true;
                            //吹き飛ばし処理
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (_defender == MyDamageOntouch.TypeOfSubject.Enemy)
                {
                    if (eData.status.kind != EnemyStatus.KindofEnemy.Fly)
                    {
                        if (stunnState == MyWakeUp.StunnType.Falter)
                        {
                            return true;
                            //吹き飛ばし処理
                        }
                        else if (stunnState == MyWakeUp.StunnType.notStunned)
                        {
                            //攻撃中かどうか
                            if (!eData.AttackCheck())
                            {
                                return true;
                                //吹き飛ばし処理
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }

                }

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
        /// 攻撃された側のヘルスで呼ぶ
        /// パリィモーション
        /// </summary>
        /// <param name="isArmorBreak"></param>
        public void ParryStart(bool isArmorBreak = false)
        {

            if (_defender == MyDamageOntouch.TypeOfSubject.Enemy)
            {
                eData.ParryStart();
            }
            else if (_defender == MyDamageOntouch.TypeOfSubject.Player)
            {

                if (!GManager.instance.equipWeapon.twinHand)
                {
                    GManager.instance.stamina += GManager.instance.equipShield.parryRecover;
                }
                else
                {
                    GManager.instance.stamina += GManager.instance.equipWeapon.parryRecover;
                }

                if (isArmorBreak)
                {
                    pCon.ParryStart(2);
                }
                else
                {
                    pCon.ParryStart(1);
                }
            }
            else if (_defender == MyDamageOntouch.TypeOfSubject.Magic)
            {

            }
            else
            {
                //オブジェクトがダメージ受ける
            }


        }


        /// <summary>
        /// エンチャ時はエンチャントタイプを参照
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
                    //	Debug.Log("チキン");
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

            if (_defender == MyDamageOntouch.TypeOfSubject.Player)
            {
                pCon.testReset();
            }
            else if (_defender == MyDamageOntouch.TypeOfSubject.Enemy)
            {
                if (SManager.instance.target == this.gameObject)
                {
                    SManager.instance.target = null;
                    
                }
　　　　　　　　　
            }
        }


        public void GuardReport()
        {
            if (_defender == MyDamageOntouch.TypeOfSubject.Player)
            {
                pCon.GuardReport();
            }
            else if (_defender == MyDamageOntouch.TypeOfSubject.Enemy)
            {
                eData.GuardReport();
            }
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
        /// 今ヘルスが無敵であるかどうか
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