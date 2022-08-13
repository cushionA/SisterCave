using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using UnityEngine.AddressableAssets;
using Rewired.Integration.CorgiEngine;
using DarkTonic.MasterAudio;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// TODO_DESCRIPTION
    /// </summary>
  //  [AddComponentMenu("Corgi Engine/Character/Abilities/Rewight")]
    public class RewrightAttack : MyAbillityBase
    {

        /// このメソッドは、ヘルプボックスのテキストを表示するためにのみ使用されます。
        /// 能力のインスペクタの冒頭にある
        public override string HelpBoxText() { return "TODO_HELPBOX_TEXT."; }

        //   [Header("武器データ")]
        /// declare your parameters here
        ///WeaponHandle参考にして 


        // Animation parameters
        //       protected const string _attackParameterName = "AttackNow";
        //     protected int _attackAnimationParameter;

        protected const string _typeParameterName = "AttackType";
        protected int _typeAnimationParameter;

        protected const string _numberParameterName = "AttackNumber";
        protected int _numberAnimationParameter;

        public enum ActType
        {
            noAttack,//なにもなし
            sAttack,
            bAttack,
            cCharge,
            cAttack,//チャージ完了
            aAttack,//空中弱
            fAttack,//空中強
            arts,
            magic

        }

        ActType atType;


        //内部パラメータ
        #region
        //------------------------------------------内部パラメータ



        /// <summary>
        /// 武器の両手持ち切り替えがされてるかどうか
        /// </summary>
        bool changeKey;


        /// <summary>
        /// ため押し時間測る入れ物
        /// </summary>
        float chargeTime;
        //   float gravity;//重力を入れる

        bool isAirEnd;//空中弱攻撃を二回までに制限


        // float delayTime;
        int attackNumber;

        //現在攻撃の番号がどれか
        int nowNumber;

        //Animator anim;
        [HideInInspector] public bool isAttackable;
        bool smallTrigger;
        bool bigTrigger;
        bool artsTrigger;
        bool MagicTrigger;
        //連撃のトリガーになる



        //武器変更後の両手切り替え暴発予防
        bool equipChange;
        //チャージ中
        float horizontalKey;
        bool anyKey;
        float attackDirection;


        /// <summary>
        /// コンボ振り切った
        /// </summary>
        bool isComboEnd;

        //コンボ制限。バリューの数
        int comboLimit;


        /// <summary>
        /// 横攻撃後に方向転換か移動か判断する
        /// </summary>
        float afterTime;

        float groundTime;

        public float afterJudge = 0.35f;

        //   Rigidbody2D GManager.instance.pm.rb;

        /// <summary>
        /// ほんとに落ち始めるフラグ
        /// </summary>
        bool startFall;
        // Vector3 theScale = new Vector3();
        bool isParring;//パリィ中
                       // Start is called before the first frame update

        /// <summary>
        /// 真ならチャージを開始する
        /// </summary>
        bool isCharging;


        bool fire1Key;
        bool fire2Key;
        bool artsKey;

        #endregion

        /// <summary>
        ///　ここで、パラメータを初期化する必要があります。
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();

            // randomBool = false;
        }


        /// <summary>
        /// 1フレームごとに、しゃがんでいるかどうか、まだしゃがんでいるべきかをチェックします
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();

            //入力に基づいて動作
            InputReceiver();

            if (atType != ActType.noAttack)
            {
                AttackFallController();
                //     NumberControll();

                if (_condition.CurrentState == CharacterStates.CharacterConditions.Stunned)
                {
                    GManager.instance.isAttack = false;
                    _movement.ChangeState(CharacterStates.MovementStates.Idle);
                    attackNumber = 0;
                    atType = ActType.noAttack;
                    startFall = false;
                    isDisenable = false;
                    isComboEnd = false;
                    isAttackable = false;
                }

                if (isAttackable)
                {
                    AttackCheck();
                    AnimationEndReserch();
                }
                
            }

            Debug.Log($"平常検査、ディスエナ{isDisenable}攻撃可能{isAttackable}、攻撃タイプ{atType}、移動可能{_characterHorizontalMovement.ReadInput}、攻撃番号{attackNumber}、チャージ時間{chargeTime}");

        }






        /// <summary>
        /// アビリティサイクルの開始時に呼び出され、ここで入力の有無を確認します。
        /// </summary>
        protected override void HandleInput()
        {
            //ここで何ボタンが押されているかによって引数渡すか
            //引数によってチャージ状態にしたり秒数数えたりする
            //インプットについて調べる
            //
            // here as an example we check if we're pressing down
            // on our main stick/direction pad/keyboard


            fire1Key = (_inputManager.sAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonDown || _inputManager.sAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed);
            fire2Key = (_inputManager.bAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonDown || _inputManager.bAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed);
            artsKey = (_inputManager.ArtsButton.State.CurrentState == MMInput.ButtonStates.ButtonDown || _inputManager.ArtsButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed);


            //    Debug.Log("アイイイ");


            //攻撃終了ェック
            if (_movement.CurrentState == CharacterStates.MovementStates.Attack && isAttackable)
            {
                anyKey = AnyKey();
                
            }
            //  UpdateAnimator();
            WeaponChange();
        }

        public void WeaponChange()
        {
            if (_inputManager.WeaponChangeButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed && _movement.CurrentState != CharacterStates.MovementStates.Attack)
            {
                //武器切り替え
                //ガードボタンで盾、攻撃ボタンで武器を切り替え

                //さらに初期は必ず片手持ちに
                GManager.instance.equipWeapon.twinHand = false;
                //このフラグは武器切り替えか両手持ち切り替えかで区別するもの
                equipChange = true;
                //武器切り替え後は一回だけボタン離しても持ち手変更が反応しないようにする
            }
            else if (changeKey && !equipChange)
            {
                GManager.instance.equipWeapon.twinHand = !GManager.instance.equipWeapon.twinHand;
            }
            else if (equipChange && changeKey)
            {
                equipChange = false;
            }
        }




        /// <summary>
        ///  必要なアニメーターパラメーターがあれば、アニメーターパラメーターリストに追加します。
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            //    RegisterAnimatorParameter(_attackParameterName, AnimatorControllerParameterType.Bool, out _attackAnimationParameter);
            RegisterAnimatorParameter(_typeParameterName, AnimatorControllerParameterType.Int, out _typeAnimationParameter);
            RegisterAnimatorParameter(_numberParameterName, AnimatorControllerParameterType.Int, out _numberAnimationParameter);
        }

        /// <summary>
        /// アビリティのサイクルが終了した時点。
        /// 現在のしゃがむ、這うの状態をアニメーターに送る。
        /// </summary>
        public override void UpdateAnimator()
        {
            //今のステートがAttackであるかどうかでBool入れ替えてる
            // MMAnimatorExtensions.UpdateAnimatorBool(_animator, _attackAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Attack), _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _typeAnimationParameter, (int)atType, _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _numberAnimationParameter, attackNumber, _character._animatorParameters);
        }




        //アニメーションイベント
        #region
        public void Continue()
        {
            // GManager.instance.pm.anim.Play("OArts1");

            ////Debug.Log("攻撃可能に");

            GManager.instance.StaminaUse(GManager.instance.useAtValue.useStamina);
            isAirEnd = false;
            if (atType == ActType.fAttack)
            {
                startFall = true;
            }
            else
            {
                GManager.instance.isArmor = false;
                isAttackable = true;
            }
        }
        public void SwingSound(int type = 0)
        {
            //斬撃刺突打撃を管理
            if (GManager.instance.useAtValue.type == MyCode.Weapon.AttackType.Stab)
            {
                GManager.instance.PlaySound(MyCode.SoundManager.instance.stabSound[type], transform.position);
            }
            else
            {
                GManager.instance.PlaySound(MyCode.SoundManager.instance.swingSound[type], transform.position);
            }

            //エンチャしてる場合も

        }

        public void attackEffect()
        {

            Addressables.InstantiateAsync(GManager.instance.useAtValue.attackEffect, GManager.instance.pm.eContoroller.transform);
        }
        #endregion

        //挙動操作系
        #region 

        //落下攻撃の落下を制御する
        void AttackFallController()
        {
            //落下攻撃の時
            if (atType == ActType.fAttack)
            {
                //もしContinueで落下開始してるなら
                if (startFall)
                {
                    // gravity =GManager.instance.pm.gravity * 3f;
                    //重力1.5倍
                    _controller.DefaultParameters.Gravity = GManager.instance.pStatus.firstGravity * -1.5f;
                }
                else
                {
                    //落下開始までは無重力で
                    _controller.DefaultParameters.Gravity = 0;
                }
                // 着地したら
                if (startFall && _controller.State.IsGrounded)
                {
                    //重力をもとに戻す
                    _controller.DefaultParameters.Gravity = -GManager.instance.pStatus.firstGravity;

                    groundTime += _controller.DeltaTime;

                    GManager.instance.isArmor = false;

                    // GManager.instance.pm.jumpTime = 0.0f;

                    //キャンセル可能に
                    if (groundTime >= 0.1f)
                    {
                        isAttackable = true;
                        //GManager.instance.isAttack = false;
                        //  
                        // GManager.instance.isArmor = false;
                        groundTime = 0;
                        // //////Debug.log("機能してます");
                        attackNumber = 0;

                        smallTrigger = false;
                        bigTrigger = false;

                        // GManager.instance.pm.jumpTime = 0.0f;

                    }
                }
            }


        }


        #endregion


        //管理系
        #region 

        //入力を受け取る
        //fire1Keyとかで
        void InputReceiver()
        {

            // やらない条件
            if (!AbilityPermitted
                // or if we're not in our normal stance
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                // or if we're grounded
                || (_movement.CurrentState == CharacterStates.MovementStates.Rolling)
                // or if we're gripping
                || (_movement.CurrentState == CharacterStates.MovementStates.Gripping))
            {
                // we do nothing and exit
                return;
            }

            //スタミナ利用可能なら
            if (GManager.instance.isEnable && !isDisenable && _movement.CurrentState != CharacterStates.MovementStates.Attack && !GManager.instance.equipWeapon.isCombo)
            {

                // 1通常攻撃、2は空中弱、3は強、4は空中強、5は戦技
                if (fire1Key || smallTrigger)
                {
                    // DoSomething();
                    smallTrigger = false;
                    
                    Debug.Log($"1{attackNumber}");
                    //接地してないなら
                    if (!_controller.State.IsGrounded)
                    {
                        atType = ActType.aAttack;
                    }
                    else
                    {
                        atType = ActType.sAttack;
                    }
                    isDisenable = true;
                }
                else if (fire2Key || bigTrigger)
                {
                    //7はチャージ中
                    
                    bigTrigger = false;
                    if (_controller.State.IsGrounded)
                    {
                        isCharging = true;
                        atType = ActType.cCharge;
                    }
                    else
                    {
                        atType = ActType.fAttack;
                    }
                    isDisenable = true;
                }
                else if (artsKey || artsTrigger)
                {
                    atType = ActType.arts;
                    artsTrigger = false;
                    //   isDisenable = true;
                }


            }
            //攻撃中じゃなくてコンボ属性じゃないなら
            if (GManager.instance.equipWeapon.isCombo && _movement.CurrentState != CharacterStates.MovementStates.Attack)
            {

                //コンボ入力
                //artsトリガーとかを判断に使うか
            }

            if (isCharging)
            {
                if (fire2Key)
                {
                    Debug.Log("アイ矢―");
                    chargeTime += _controller.DeltaTime;
                    //チャージ中
                    atType = ActType.cCharge;
                    if (chargeTime >= GManager.instance.equipWeapon.chargeRes)
                    {
                        isCharging = false;

                        //チャージアタックメソッド発動を一回きりにするために使う
                        //   chargeTime = 0.0f;
                        atType = ActType.cAttack;
                        chargeTime = 0;
                    }
                }
                else
                {
                    //  Debug.Log("ddeferfer");
                    //一応足す
                    chargeTime += _controller.DeltaTime;
                    if (chargeTime >= GManager.instance.equipWeapon.chargeRes)
                    {   
                        atType = ActType.cAttack;
                    }
                    else
                    {
                        atType = ActType.bAttack;
                    }
                    isCharging = false;
                     chargeTime = 0.0f;
                }
            }


            //攻撃中でなく攻撃状態にある時
            if (atType != ActType.noAttack && _movement.CurrentState != CharacterStates.MovementStates.Attack)
            {



                AttackAct();
            }
        }




        //アニメの終了探知
        //終了時trueを返す
        bool CheckEnd(string Name)
        {
            //      Debug.Log("ｄｄｆ");
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(Name))// || GManager.instance.pm.anim.GetCurrentAnimatorStateInfo(0).IsName("OStand"))
            {   // ここに到達直後はnormalizedTimeが"Default"の経過時間を拾ってしまうので、Resultに遷移完了するまではreturnする。

                return false;
            }
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {   // 待機時間を作りたいならば、ここの値を大きくする。
                return false;
            }
            //      AnimatorClipInfo[] clipInfo = GManager.instance.pm.anim.GetCurrentAnimatorClipInfo(0);

            ////Debug.Log($"アニメ終了");
            //  Debug.Log("ｆ");
            return true;

            // return !(GManager.instance.pm.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
            //  (_currentStateName);
        }

        bool AnyKey()
        {
            //Anyは軸も行ける
            //キーコンフィグで反映されなかったりしたらInputRの参照そろえてないのを確認
            if (GManager.instance.InputR.GetAnyButton() && !fire1Key && !fire2Key && !artsKey)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void AnimationEndReserch()
        {

            //モーション終了したかどうかの検査
            if (attackNumber > 0 && isAttackable)
            {

                #region//モーション終了検査

                    if (atType == ActType.sAttack)
                    {
                        //     Debug.Log("アイイイ");
                        if (CheckEnd($"SAttack{attackNumber}"))
                        {
                            //      Debug.Log("アイ");
                            Debug.Log($"4");
                            AttackEnd();
                        }
                    }
                    else if (atType == ActType.bAttack)
                    {
                        if (CheckEnd($"BAttack{attackNumber}"))
                        {
                            AttackEnd();

                        }
                    }
                    else if (atType == ActType.cAttack)
                    {
                        if (CheckEnd($"CAttack{attackNumber}"))
                        {

                            AttackEnd();

                        }
                    }
                    else if (atType == ActType.arts)
                    {
                        if (CheckEnd($"Arts{attackNumber}"))
                        {
                            AttackEnd();
                        }
                    }
                   else if (atType == ActType.aAttack)
                    {

                        if (CheckEnd($"SAAttack{attackNumber}"))
                        {
                            AttackEnd();
                        }

                    }
                    else if (atType == ActType.fAttack)
                    {
                        //  Debug.Log($"3sdfg");
                        //落下開始しててすでに着地してるなら
                        if (_controller.State.IsGrounded)
                        {
                            Debug.Log($"pie");
                            //着地アニメ終わってるなら解放
                            if (CheckEnd($"Landing"))
                            {
                                Debug.Log("機能してます");
                                AttackEnd();
                            }
                        }
                }
                #endregion

            }

        }







        void GroundCheck()
        {
            //何かしらの入力があれば
            if (anyKey)
            {
                AttackEnd();
            }
            //攻撃入力が続けばコンボに
            else if (fire1Key)
            {
                if (atType == ActType.sAttack)
                {
                    //  ////Debug.Log($"連撃");
                    AttackEnd(1, isComboEnd);
                }
                //他の攻撃の後小攻撃きたらattackNumberはリセット
                else
                {

                    AttackEnd(1, true);
                }
            }
            else if (fire2Key)
            {
                if (atType == ActType.bAttack || atType == ActType.cAttack)
                {
                    AttackEnd(2, isComboEnd);
                }
                //他の攻撃の後大攻撃きたらattackNumberはリセット
                else
                {

                    AttackEnd(2, true);
                }

            }
            else if (artsKey)
            {
                if (atType == ActType.arts)
                {
                    AttackEnd(3, isComboEnd);
                }
                //他の攻撃の後戦技きたらattackNumberはリセット
                else
                {

                    AttackEnd(3, true);
                }
            }
        }
        void AirCheck()
        {
            if (anyKey)
            {
                AttackEnd();

            }
            else if (fire1Key)
            {
                if (atType == ActType.sAttack)
                {
                    AttackEnd(1, isComboEnd);
                }
                //他の攻撃の後小攻撃きたらattackNumberはリセット
            }
            else if (_inputManager.bAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {

                AttackEnd(2, true);
  }
        }
        void AttackCheck()
        {
            if (atType != ActType.aAttack && atType != ActType.fAttack && atType != ActType.noAttack)
            {
                GroundCheck();
            }

            //空中連撃入力とキャンセル待ち
            else if (atType == ActType.aAttack)
            {
                AirCheck();
            }

        }


        /// <summary>
        ///  0で終了、1でスモールトリガーで継続、2でビッグトリガー、3でアーツトリガー
        ///  コンボエンドはisComboEndだけではなくTrueとかをいれても操作としていいね
        /// </summary>
        /// <param name="conti"></param>
        void AttackEnd(int conti = 0, bool comboEnd = true)
        {
            GManager.instance.isAttack = false;
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
            isComboEnd = false;
            isDisenable = false;
            _characterHorizontalMovement.ReadInput = true;
            startFall = false;
            atType = ActType.noAttack;
            if (conti == 1)
            {
                smallTrigger = true;
            }
            else if (conti == 2)
            {
                bigTrigger = true;
            }
            else if (conti == 3)
            {
                artsTrigger = true;
            }
            //もし連撃じゃないなら攻撃番号はリセット

            if (comboEnd)
            {
                attackNumber = 0;
               // comboLimit = 0;
            }
        }

        #endregion


        //攻撃実行系
        #region

        /// <summary>
        /// 攻撃の開始と
        /// </summary>
        void AttackAct()
        {

            _movement.ChangeState(CharacterStates.MovementStates.Attack);
            //攻撃開始、アーマー発生
            GManager.instance.isArmor = true;
            GManager.instance.isAttack = true;
            isAttackable = false;
            //横移動不可
            _characterHorizontalMovement.ReadInput = false;
            isDisenable = true;
            //攻撃可能で弱攻撃ボタン押されてて攻撃してなくてスタミナが使えるなら以下の処理
            //delayTime = 0.0f;


            //どの攻撃を呼び出すか
            #region
            if (atType == ActType.sAttack)
            {
sAttackPrepare();
            }
            else if (atType == ActType.bAttack)
            {
            bAttackPrepare();
            }
            else if (atType == ActType.cAttack)
            {
            chargeAttackPrepare();
            }
            else if (atType == ActType.arts)
            {
            ArtsPrepare();
            }
            else if (atType == ActType.aAttack)
            {
                isAirEnd = (attackNumber + 1 == comboLimit);
                airAttackPrepare();
            }
            else if (atType == ActType.fAttack)
            {
                strikeAttackPrepare();
                GManager.instance.fallAttack = true;
            }
            #endregion

            if (attackNumber >= comboLimit - 1 && comboLimit != 0)
            {
                isComboEnd = true;

            }
            //  攻撃不可能状態
            isAttackable = false;
            //攻撃番号加算
            //prepareで0から始まる番号使うので先に加算はダメです
            attackNumber++;

            nowNumber = attackNumber;
        }



        void Parry()
        {

            //無敵処理あたり内容書き換える
            if (GManager.instance.parrySuccess && !isParring)
            {
                if (!GManager.instance.blocking)
                {

                    GManager.instance.PlaySound("ParrySuccess", transform.position);
                    //  GManager.instance.PlaySound("ParrySuccess2", transform.position);
                }
                else if (GManager.instance.blocking)
                {
                    //   Debug.Log("s");

                    GManager.instance.PlaySound("Blocking", transform.position);
                }
                isParring = true;
                GManager.instance.guardDisEnable = true;
                //パリィ
            }
            else if (!GManager.instance.blocking && isParring)
            {
                // Debug.Log("sssssss");
                if (!GManager.instance.twinHand && CheckEnd("OParry"))
                {
                    // Debug.Log("sss");
                    isParring = false;
                    GManager.instance.parrySuccess = false;
                    GManager.instance.pm.SetLayer(11);
                    GManager.instance.guardDisEnable = false;
                    // GManager.instance.isDown = false;
                }
                if (GManager.instance.twinHand && CheckEnd("TParry"))
                {
                    isParring = false;
                    GManager.instance.parrySuccess = false;
                    GManager.instance.pm.SetLayer(11);
                    GManager.instance.guardDisEnable = false;

                }
            }
            else if (GManager.instance.blocking && isParring)
            {
                if (!GManager.instance.twinHand && CheckEnd("OBlock"))
                {
                    isParring = false;
                    GManager.instance.parrySuccess = false;
                    GManager.instance.pm.SetLayer(11);
                    GManager.instance.guardDisEnable = false;
                    //GManager.instance.isDown = false;
                }
                if (GManager.instance.twinHand && CheckEnd("TBlock"))
                {
                    isParring = false;
                    GManager.instance.parrySuccess = false;
                    GManager.instance.pm.SetLayer(11);
                    GManager.instance.guardDisEnable = false;
                }
            }

        }

        #endregion

        //Prepare系統
        #region
        //攻撃するときに呼ぶ
        public void sAttackPrepare()//デフォが斬撃
        {
            GManager.instance.isShieldAttack = false;
            //コンボの最初にコンボ何回繋がるか確認する。



            //ガード中ならガード解除
            //ひよおおおおおおおおおおお
            if (attackNumber != 0)
            {

                GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
                transform.localScale = GManager.instance.pm.theScale;
            }
            if (!GManager.instance.twinHand)
            {

                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.sValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.sValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.sValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.sValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.sValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.sValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.sValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.sValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.sValue[attackNumber].attackEffect;
                comboLimit = GManager.instance.equipWeapon.sValue.Count;
            }
            else
            {

                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.twinSValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.twinSValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.twinSValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.twinSValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.twinSValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.twinSValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.twinSValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.twinSValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.twinSValue[attackNumber].attackEffect;
                comboLimit = GManager.instance.equipWeapon.twinSValue.Count;
            }
        }

        public void bAttackPrepare()//デフォが斬撃。強攻撃
        {
            GManager.instance.isShieldAttack = false;


            if (attackNumber != 0)
            {
                GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
                transform.localScale = GManager.instance.pm.theScale;
            }
            if (!GManager.instance.twinHand)
            {

                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.bValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.bValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.bValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.bValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.bValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.bValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.bValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.bValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.bValue[attackNumber].attackEffect;
                comboLimit = GManager.instance.equipWeapon.bValue.Count;
            }
            else
            {

                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.twinBValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.twinBValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.twinBValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.twinBValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.twinBValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.twinBValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.twinBValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.twinBValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.twinBValue[attackNumber].attackEffect;
                comboLimit = GManager.instance.equipWeapon.twinBValue.Count;
            }
        }

        public void chargeAttackPrepare()//デフォが斬撃
        {
            GManager.instance.isShieldAttack = false;


            if (attackNumber != 0)
            {
                //   GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
                //        transform.localScale = GManager.instance.pm.theScale;
            }
            if (!GManager.instance.twinHand)
            {

                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.chargeValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.chargeValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.chargeValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.chargeValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.chargeValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.chargeValue[attackNumber].isCombo;
                GManager.instance.equipWeapon.blowPower = GManager.instance.equipWeapon.chargeValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.chargeValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.chargeValue[attackNumber].attackEffect;
                comboLimit = GManager.instance.equipWeapon.bValue.Count;
            }
            else
            {

                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.twinChargeValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.twinChargeValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.twinChargeValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.twinChargeValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.twinChargeValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.twinChargeValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.twinChargeValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.twinChargeValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.twinChargeValue[attackNumber].attackEffect;
                comboLimit = GManager.instance.equipWeapon.twinBValue.Count;
            }
        }
        public void airAttackPrepare()//デフォが斬撃
        {

            GManager.instance.isShieldAttack = false;


            if (attackNumber != 0)
            {
                GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
                transform.localScale = GManager.instance.pm.theScale;
            }
            if (!GManager.instance.twinHand)
            {

                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.airValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.airValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.airValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.airValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.airValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.airValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.airValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.airValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.airValue[attackNumber].attackEffect;
                comboLimit = GManager.instance.equipWeapon.airValue.Count;
            }
            else
            {

                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.twinAirValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.twinAirValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.twinAirValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.twinAirValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.twinAirValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.twinAirValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.twinAirValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.twinAirValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.twinAirValue[attackNumber].attackEffect;
                comboLimit = GManager.instance.equipWeapon.twinAirValue.Count;
            }
        }
        public void strikeAttackPrepare()//デフォが斬撃
        {

            GManager.instance.isShieldAttack = false;


            if (attackNumber != 0)
            {
                GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
                transform.localScale = GManager.instance.pm.theScale;
            }
            if (!GManager.instance.twinHand)
            {

                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.strikeValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.strikeValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.strikeValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.strikeValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.strikeValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.strikeValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.strikeValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.strikeValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.strikeValue[attackNumber].attackEffect;

            }
            else
            {

                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].attackEffect;
               
            }
            comboLimit = 1;
        }

        public void ArtsPrepare()//デフォが斬撃
        {


            if (attackNumber != 0)
            {
                GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
                transform.localScale = GManager.instance.pm.theScale;
            }
            if (!GManager.instance.twinHand && !GManager.instance.equipShield.weaponArts)
            {

                GManager.instance.useAtValue.type = GManager.instance.equipShield.artsValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipShield.artsValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipShield.artsValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipShield.artsValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipShield.artsValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipShield.artsValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipShield.artsValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipShield.artsValue[attackNumber].useStamina;
                GManager.instance.isShieldAttack = true;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipShield.artsValue[attackNumber].attackEffect;
                comboLimit = GManager.instance.equipShield.artsValue.Count;
            }
            else
            {
                GManager.instance.useAtValue.isShield = true;
                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.artsValue[attackNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.artsValue[attackNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.artsValue[attackNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.artsValue[attackNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.artsValue[attackNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.artsValue[attackNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.artsValue[attackNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.artsValue[attackNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.artsValue[attackNumber].attackEffect;
                GManager.instance.isShieldAttack = false;
                comboLimit = GManager.instance.equipWeapon.artsValue.Count;
            }
        }
        #endregion




    }
}