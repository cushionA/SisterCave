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
        [Header("このオブジェクトが魔法であるか")]
        MyDamageOntouch.TypeOfSubject _defender;

        /// <summary>
        /// キャラ関連のクラス
        /// </summary>
        ControllAbillity _charaCon;

        /// <summary>
        /// ダメージ機能から渡される吹き飛び距離
        /// </summary>
        [HideInInspector]
        public Vector2 blowVector;



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




            if (_defender == MyDamageOntouch.TypeOfSubject.Magic)
            {

            }
            else if(_defender == MyDamageOntouch.TypeOfSubject.Gimic)
            {
                //罠のダメージ
            }
            else
            {
                _charaCon = _character.FindAbility<ControllAbillity>();

                um = EnemyManager.instance.um;
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

           // _autoRespawn = this.gameObject.GetComponent<AutoRespawn>();
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




            damage = (int)DamageCalc(_damageData, back);

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

                _charaCon.StartStun(stunnState);

            }


            // 体力ゼロの時
            //死亡処理
            if (CurrentHealth <= 0)
            {

                GManager.instance.PlaySound("Kill",transform.position);

                // ヘルスを0にします。（ヘルスバーに便利です。）
                CurrentHealth = 0;

                Invulnerable = true;

                if (_character != null)
                {
                    //ダウンして死ぬのかそうでないか
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
            
            //死んでいなければ被弾後の処理を呼び出す
            else
            {
                //スタンしてるかどうかで処理を分けれる
                //敵のゲームオブジェクトも渡します
                _charaCon.DamageEvent(stunnState == MyWakeUp.StunnType.notStunned,instigator,damage,back);
            }
        }

        public float DamageCalc(AttackData _damageData, bool back)
        {
            bool stab = false;
            //isDamage = true;
            //SetLayer(28);
            //////Debug.log($"ガード中か否か{guardHit}");

            //	GManager.instance.equipWeapon.hitLimmit--;

            //	Equip _damageData;



            //////Debug.log("終了");
            float damage = 0;//バフデバフ処理用にdamageとして保持する
            float mValue = _damageData.actionData.mValue;
            //float damage;//バフデバフ処理用にdamageとして保持する

            bool isGuard = ((HealthStateJudge(DefState.ガード中)) && !back);

            //ガード時
            //ガード方向とか見れるようにしたいな


            //ガード時


            float multipler = 0;

                //斬撃刺突打撃を管理
                if (ElementalJudge(_damageData.actionData.useElement, AtEffectCon.Element.斬撃属性))
                {

                //攻撃倍率を防御倍率でかける。ダメージ0.7倍にするバフと攻撃側の1.2倍バフをかけ合わせるみたいな
                multipler = _damageData.multipler.phyAtkMultipler * _defData.multipler.phyDefMultipler;
                damage +=CalcuExe(_damageData.attackStatus.phyAtk, _defData.status.Def, mValue, multipler,isGuard ? _defData.guardStatus.phyCut:0);

                 
                }
                if (ElementalJudge(_damageData.actionData.useElement, AtEffectCon.Element.刺突属性))
                {
                multipler = _damageData.multipler.phyAtkMultipler * _defData.multipler.phyDefMultipler;
                damage += CalcuExe(_damageData.attackStatus.phyAtk, _defData.status.pierDef, mValue, multipler, isGuard ? _defData.guardStatus.phyCut : 0);

            }
                if (ElementalJudge(_damageData.actionData.useElement, AtEffectCon.Element.打撃属性))
                {
                multipler = _damageData.multipler.phyAtkMultipler * _defData.multipler.phyDefMultipler;
                damage += CalcuExe(_damageData.attackStatus.phyAtk, _defData.status.strDef, mValue, multipler, isGuard ? _defData.guardStatus.phyCut : 0);

            }

                //神聖
                if (ElementalJudge(_damageData.actionData.useElement, AtEffectCon.Element.聖属性))
                {
                multipler = _damageData.multipler.holyAtkMultipler * _defData.multipler.holyDefMultipler;
                damage += CalcuExe(_damageData.attackStatus.holyAtk, _defData.status.holyDef, mValue, multipler, isGuard ? _defData.guardStatus.holyCut : 0);

            }
                //闇
                if (ElementalJudge(_damageData.actionData.useElement, AtEffectCon.Element.闇属性))
                {
                multipler = _damageData.multipler.darkAtkMultipler * _defData.multipler.darkDefMultipler;
                damage += CalcuExe(_damageData.attackStatus.darkAtk, _defData.status.darkDef, mValue, multipler, isGuard ? _defData.guardStatus.darkCut : 0);
            }
                //炎
                if (ElementalJudge(_damageData.actionData.useElement, AtEffectCon.Element.炎属性))
                {
                multipler = _damageData.multipler.fireAtkMultipler * _defData.multipler.fireDefMultipler;
                damage += CalcuExe(_damageData.attackStatus.fireAtk, _defData.status.fireDef, mValue, multipler, isGuard ? _defData.guardStatus.fireCut : 0);

            }
                //雷
                if (ElementalJudge(_damageData.actionData.useElement, AtEffectCon.Element.雷属性))
                {
                multipler = _damageData.multipler.thunderAtkMultipler * _defData.multipler.thunderDefMultipler;
                damage += CalcuExe(_damageData.attackStatus.thunderAtk, _defData.status.thunderDef, mValue, multipler, isGuard ? _defData.guardStatus.thunderCut : 0);
            }


            if (isGuard)
            {
                //魔法の盾や紗う面ガードしてるギミックではこういうこともあるかも
                //ヘルスつけてる以上はね
                _charaCon.GuardSound();
            }


            //刺突カウンター
            if (HealthStateJudge(DefState.攻撃中) && stab)
            {
                damage *= 1.15f;
            }
            //スタン時
            else if (HealthStateJudge(DefState.被ダメージ増大))
            {
                damage *= 1.2f;
            }

            multipler = _damageData.multipler.allAtkMultipler * _defData.multipler.allDefMultipler;
            return Mathf.Floor(damage * multipler);


        }


        /// <summary>
        /// 計算実行
        /// 共通の式で行う
        /// </summary>
        /// <param name="atk"></param>
        /// <param name="def"></param>
        /// <param name="mValue"></param>
        /// <param name="guardCut">ガードのカット率。ガードしないなら0</param>
        /// <param name="multipler"></param>
        /// <returns></returns>
        float CalcuExe(float atk,float def,float mValue,float multipler,float guardCut = 0)
        {
           return ((Mathf.Pow(atk, 2) * mValue) / (atk + def)) * multipler * ((100 - guardCut) / 100);
        }



        /// <summary>
        /// 属性が攻撃属性に当てはまるかのビット演算の結果を返してくれるメソッド
        /// </summary>
        /// <returns></returns>
        bool ElementalJudge(AtEffectCon.Element judgeElement,AtEffectCon.Element condition)
        {
            //かつで0以上になるか
            return (judgeElement & condition) > 0;
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

            //ガード攻撃はガードとスーパーアーマーをつける
            if (HealthStateJudge(DefState.スーパーアーマー)  || shock <= 0)
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
        /// 敵の場合はプレイヤーの方を向く、立ち上がり時
        /// </summary>
        public void ArmorReset()
        {

                _charaCon.ArmorReset();
        }


        /// <summary>
        /// 自分がプレイヤーに攻撃した時にパリィ演出はいるかをはかる
        /// 攻撃側のヘルスで出る
        /// </summary>
        /// <returns></returns>
        public bool ParryArmorCheck()
        {

            return _charaCon.ParryArmorJudge();


        }

        /// <summary>
        /// 攻撃された時に空中にいたら特殊ダウンする
        /// </summary>
        /// <param name="stunnState"></param>
        /// <returns></returns>
        public bool AirDownJudge(MyWakeUp.StunnType stunState)
        {
            //空中にいる時は少し浮かせてダウンに
            //攻撃中はなんとか
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
        /// 攻撃された側のヘルスで呼ぶ
        /// パリィモーション開始
        /// </summary>
        /// <param name="isArmorBreak"></param>
        public void ParryStart(bool isArmorBreak = false)
        {
            _charaCon.ParryStart(isArmorBreak);

        }


        /// <summary>
        /// エンチャ時はエンチャントタイプを参照
        /// </summary>
        /// <param name="damageType"></param>
        public void DamageSound(AtEffectCon.Element damageType, bool heavy)
        {
            if (damageType == AtEffectCon.Element.斬撃属性)
            {
                GManager.instance.PlaySound("SlashDamage", transform.position);
            }
            else if (damageType == AtEffectCon.Element.刺突属性)
            {
                GManager.instance.PlaySound("StabDamage", transform.position);
            }
            else if (damageType == AtEffectCon.Element.打撃属性)
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
            else if (damageType == AtEffectCon.Element.聖属性)
            {
                GManager.instance.PlaySound("HolyDamage", transform.position);
            }
            else if (damageType == AtEffectCon.Element.闇属性)
            {
                GManager.instance.PlaySound("DarkDamage", transform.position);
            }
            else if (damageType == AtEffectCon.Element.炎属性)
            {


                GManager.instance.PlaySound("FireDamage", transform.position);
            }
            else if (damageType == AtEffectCon.Element.雷属性)
            {

                GManager.instance.PlaySound("ThunderDamage", transform.position);
            }
        }
        public void Die()
        {
            _charaCon.Die();
        }


        /// <summary>
        /// ヘルスの状態変更するメソッド
        /// isEndが真なら状態を終わらせる
        /// </summary>
        /// <param name="isEnd"></param>
        /// <param name="changeState"></param>
        public void HealthStateChange(bool isEnd, DefState changeState)
        {
            //消す
            if (isEnd)
            {
                //両方ないやつは消す
                //チェンジ対象の反転ビットにつける
                //001001 と　01では桁数変わらない？　動作不良に注意
                _defData.state &= ~changeState;
            }
            //つける
            else
            {
                //どちらか片方あるやつ、の演算
                //チェンジ対象をつける
                _defData.state |= changeState;
            }


        }

        /// <summary>
        /// ヘルスの状態が当てはまるかのビット演算の結果を返してくれるメソッド
        /// </summary>
        /// <returns></returns>
        bool HealthStateJudge(DefState judgeState)
        {
            //かつで0以上になるか
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