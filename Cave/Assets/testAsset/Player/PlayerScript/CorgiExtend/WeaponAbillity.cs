using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// TODO_DESCRIPTION
    /// </summary>
  //  [AddComponentMenu("Corgi Engine/Character/Abilities/WeaponAbillity")]
    public class WeaponAbillity : MyAbillityBase
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

        [SerializeField]
        //当たり判定が出るまでは振り返り可能にする
        CircleCollider2D _attackCircle;

        [SerializeField]
        BoxCollider2D _attackBox;

        [SerializeField]
        MyDamageOntouch _damage;

        [SerializeField]
        MyAttackMove _rush;

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

        /// <summary>
        /// 事前入力
        /// </summary>
        int _preInput;

        float _inputTime;

        //内部パラメータ
        #region
        //------------------------------------------内部パラメータ






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



        //チャージ中

        bool anyKey;



        /// <summary>
        /// コンボ振り切った
        /// </summary>
        bool isComboEnd;

        //コンボ制限。バリューの数
        int comboLimit;




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
        bool chargeKey;

        /// <summary>
        /// 着地時とジャンプ時に初期化する
        /// </summary>
        bool isgroundReset;

        /// <summary>
        /// 真のとき振り返り可能
        /// </summary>
        bool _flipable;

        #endregion

        /// <summary>
        ///　ここで、パラメータを初期化する必要があります。
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();

            // randomBool = false;
        }

        int test;
        /// <summary>
        /// 1フレームごとに、しゃがんでいるかどうか、まだしゃがんでいるべきかをチェックします
        /// </summary>
        public override void ProcessAbility()
        {

            base.ProcessAbility();

 
            //入力に基づいて動作
            InputReceiver();

            if (_movement.CurrentState == CharacterStates.MovementStates.Attack)
            {
                AttackFallController();
                //     NumberControll();

  


                if (isAttackable)
                {



                    AttackCheck();
                    AnimationEndReserch();

                }

            }

            // Debug.Log($"平常検査、攻撃可能{isAttackable}、移動可能{atType}、行動{_movement.CurrentState}");


        }

        public override void EarlyProcessAbility()
        {
            base.EarlyProcessAbility();
            if (_controller.State.IsGrounded && !isgroundReset && atType == ActType.aAttack)
            {
                attackNumber = 0;
                isgroundReset = true;
            }
            else if (!_controller.State.IsGrounded && isgroundReset && atType != ActType.aAttack)
            {
                attackNumber = 0;
                isgroundReset = false;
            }
            

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


            fire1Key = (_inputManager.sAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonDown);
            fire2Key = (_inputManager.bAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonDown);
            artsKey = (_inputManager.ArtsButton.State.CurrentState == MMInput.ButtonStates.ButtonDown);

            ///攻撃中事前入力をはかる
            if (_movement.CurrentState == CharacterStates.MovementStates.Attack && !isCharging)
            {
                if (fire1Key)
                {
                    _preInput = 1;
                    _inputTime = 0;
                }
                else if (fire2Key)
                {
                    _preInput = 2;
                    _inputTime = 0;
                }
                else if (artsKey)
                {
                    _preInput = 3;
                    _inputTime = 0;
                }
                else
                {
                    _inputTime =+Time.deltaTime;
                    if (_inputTime >= 0.3)
                    {
                       _preInput = 0;
                    }
                    
                }
            }

           // Debug.Log($"ああああ{fire1Key}");
            if (isCharging)
            {
                chargeKey = _inputManager.bAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed;
            }

            //    


            //攻撃終了ェック
            if (_movement.CurrentState == CharacterStates.MovementStates.Attack && isAttackable)
            {
                anyKey = AnyKey();

            }

            //攻撃判定出るまでは振り向ける
            //移動系の技に入れないかもです
            //それか攻撃判定出すだけ出すか
            if (_flipable)
            { 
                if (_attackBox.enabled || _attackCircle.enabled)
                {
                    _flipable = false;
                }
                else if (-1 * _inputManager.PrimaryMovement.x == _character.CharacterModel.transform.localScale.x)
                {
                   
                    _character.Flip();
                    
                }
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

           //Debug.Log("攻撃可能に");

            
            if (atType == ActType.fAttack)
            {
                startFall = true;
            }
            else
            {
                GManager.instance.isArmor = false;
                isAttackable = true;
            if(atType == ActType.aAttack)
                {
                    _controller.DefaultParameters.Gravity = -GManager.instance.pStatus.firstGravity;
                }
            }
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
                if (startFall && attackNumber == 1)
                {

                    // gravity =GManager.instance.pm.gravity * 3f;
                    //重力1.5倍
                    _controller.DefaultParameters.Gravity = GManager.instance.pStatus.firstGravity * -20f;
                }
                else if(attackNumber == 1)
                {

                    //落下開始までは無重力で
                    _controller.DefaultParameters.Gravity = 0;
                    _controller.SetForce(Vector2.zero);
                }
                // 着地したら
                if (startFall && _controller.State.IsGrounded)
                {
               //     Debug.Log("suuuu");
                    _controller.SetForce(Vector2.zero);
                    attackNumber = 2;
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
            if (!AbilityPermitted)
            {
                // we do nothing and exit
                return;
            }

            //スタミナ利用可能なら
            if (GManager.instance.isEnable && atType == ActType.noAttack && _condition.CurrentState == CharacterStates.CharacterConditions.Normal && !GManager.instance.equipWeapon.isCombo && !isCharging)
            {

                // 1通常攻撃、2は空中弱、3は強、4は空中強、5は戦技
                if (fire1Key || smallTrigger)
                {

                        smallTrigger = false;

                  //  Debug.Log($"1");
                    //接地してないなら
                    if (!_controller.State.IsGrounded)
                    {
                        if (!isAirEnd)
                        {
                            atType = ActType.aAttack;
                            _controller.SetForce(Vector2.zero);
                            _controller.DefaultParameters.Gravity = -15f;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else if(_controller.State.IsGrounded)
                    {
                        atType = ActType.sAttack;
                    }
                    
                }
                else if (fire2Key || bigTrigger)
                {
                    //7はチャージ中

                    bigTrigger = false;
                    if (_controller.State.IsGrounded)
                    {
                        isCharging = true;
                        atType = ActType.cCharge;
                        chargeKey = true;

                    }
                    else
                    {
                        atType = ActType.fAttack;

                    }
                }
                else if (artsKey || artsTrigger)
                {
                    atType = ActType.arts;
                    artsTrigger = false;
                }

                
            }
            //攻撃中じゃなくてコンボ属性じゃないなら
            if (GManager.instance.equipWeapon.isCombo && _condition.CurrentState == CharacterStates.CharacterConditions.Normal)
            {

                //コンボ入力
                //artsトリガーとかを判断に使うか
                //コンボになっててかつトリガーあるなら
            }

            if (isCharging)
            {
                _controller.SetHorizontalForce(0);
                if (chargeKey)
                {
                   // 
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
                        _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
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
                    _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
                }
            }

            if(_controller.State.IsGrounded && isAirEnd)
            {
                isAirEnd = false;
            }

            //攻撃中でなく攻撃状態にある時
            if (atType != ActType.noAttack && _condition.CurrentState == CharacterStates.CharacterConditions.Normal && atType != ActType.cCharge)
            {
                AttackAct();
            }
            else if (atType == ActType.cCharge && _condition.CurrentState == CharacterStates.CharacterConditions.Normal)
            {
                _condition.ChangeState(CharacterStates.CharacterConditions.Moving);
            //    attackNumber++;
                _characterHorizontalMovement.SetHorizontalMove(0);
               // _movement.ChangeState(CharacterStates.MovementStates.Attack);
            }
        }




        private async UniTask AttackEndWait()
        {

            // モーションを実行(実行後にAnimator更新のため1フレーム待つ)
            await UniTask.DelayFrame(1);



            // モーション終了まで待機
            await UniTask.WaitUntil(() => {
                var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
                return 1.0f <= stateInfo.normalizedTime;
            });

            AttackEnd();

        }



        bool AnyKey()
        {
           // Debug.Log($"あええええあ{_inputManager.CheckButtonUsing()}");
            //Anyは軸も行ける
            //キーコンフィグで反映されなかったりしたらInputRの参照そろえてないのを確認
            if (!fire1Key && !fire2Key && !artsKey && _inputManager.CombinationButton.State.CurrentState == MMInput.ButtonStates.Off)
            {
                if (_inputManager.CheckButtonUsing())
                {
                    return true;
                }
                else
                {
                    if (_controller.State.IsGrounded && (_horizontalInput != 0 || _verticalInput != 0))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {

                return false;
            }
        }

        async void AnimationEndReserch()
        {

            //モーション終了したかどうかの検査


                #region//モーション終了検査
                if (atType == ActType.fAttack)
                {

                //落下開始しててすでに着地してるなら
                if (_controller.State.IsGrounded)
                    {
                    // Debug.Log($"pie");
                    //着地アニメ終わってるなら解放
                  await  AttackEndWait();
                    }
                }

                else if (atType != ActType.cCharge && atType != ActType.noAttack)
                {

                    await AttackEndWait();
                }
                #endregion


        }







        void GroundCheck()
        {

            test = 5;
            //何かしらの入力があれば
            if (anyKey && _preInput == 0)
            {

                AttackEnd();
            }
            //攻撃入力が続けばコンボに
            else if (_preInput == 1)
            {
                if (atType == ActType.sAttack)
                {
                  //  Debug.Log($"4連撃{isComboEnd}{attackNumber}");
                    AttackEnd(1, isComboEnd);
                }
                //他の攻撃の後小攻撃きたらattackNumberはリセット
                else
                {

                    AttackEnd(1, true);
                }
            }
            else if (_preInput == 2)
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
            else if (_preInput == 3)
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

            if ((anyKey && _preInput == 0) || _controller.State.IsGrounded)
            {
                AttackEnd();

            }
            else if (_preInput == 1)
            {

                if (isAirEnd)
                {
                    AttackEnd();
                }
                else if (atType == ActType.aAttack)
                {
                    AttackEnd(1, isComboEnd);
                }
                //他の攻撃の後小攻撃きたらattackNumberはリセット
            }
            else if (_preInput == 2)
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
               // Debug.Log("あああ");
                AirCheck();
            }

        }


        /// <summary>
        ///  0で終了、1でスモールトリガーで継続、2でビッグトリガー、3でアーツトリガー
        ///  コンボエンドはisComboEndだけではなくTrueとかをいれても操作としていいね
        /// </summary>
        /// <param name="conti"></param>
       public void AttackEnd(int conti = 0, bool comboEnd = true)
        {
            test = 0;
            GManager.instance.isAttack = false;


           if(_condition.CurrentState != CharacterStates.CharacterConditions.Stunned)
            {
                if (_controller.State.IsGrounded)
                {
                    
                    _movement.ChangeState(CharacterStates.MovementStates.Idle);
                }
                else
                {
                    _movement.ChangeState(CharacterStates.MovementStates.Falling);
                }
                _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
            }
            isCharging = false;
            startFall = false;
            atType = ActType.noAttack; 
       
            if (comboEnd)
            {
               

                attackNumber = 0;
                isComboEnd = false;
                // comboLimit = 0;
            }
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
            _preInput = 0;
    // Debug.Log($"いあいいｓ{comboEnd}{attackNumber}{bigTrigger}");
        }

        #endregion


        //攻撃実行系
        #region


        /// <summary>
        /// 攻撃前の振り向き
        /// </summary>
        void AttackFlip()
        {

        }


        /// <summary>
        /// 攻撃の開始と
        /// </summary>
        void AttackAct()
        {
            _condition.ChangeState(CharacterStates.CharacterConditions.Moving);
            _flipable = true;
            _characterHorizontalMovement.SetHorizontalMove(0);
            _movement.ChangeState(CharacterStates.MovementStates.Attack);
            //攻撃開始、アーマー発生
            GManager.instance.isArmor = true;
            GManager.instance.isAttack = true;
            isAttackable = false;
            //横移動不可

            //isDisenable = true;
            //攻撃可能で弱攻撃ボタン押されてて攻撃してなくてスタミナが使えるなら以下の処理
            //delayTime = 0.0f;
            //Debug.Log($"1やめろ{attackNumber}");

            //どの攻撃を呼び出すか
            #region

            //int osixtuko = 1000;
            _damage.CollidRestoreResset();

            GManager.instance.useAtValue.isShield = false;
            //落下攻撃かどうか
            GManager.instance.useAtValue.fallAttack = atType == ActType.fAttack;
            if (atType == ActType.sAttack)
            {
                sAttackPrepare();
                //osixtuko = 1;
            }
            else if (atType == ActType.bAttack)
            {
                bAttackPrepare();
                //osixtuko = 2;
            }
            else if (atType == ActType.cAttack)
            {
                chargeAttackPrepare();
                //osixtuko = 3;
            }
            else if (atType == ActType.arts)
            {
                ArtsPrepare();
                //osixtuko = 4;
            }
            else if (atType == ActType.aAttack)
            {

                airAttackPrepare();
                isAirEnd = (attackNumber + 1 == comboLimit);
            }
            else if (atType == ActType.fAttack)
            {
                strikeAttackPrepare();
                GManager.instance.fallAttack = true;
                //Debug.Log("つづくよ２");
                //osixtuko = 5;
                
            }

            #endregion
            attackNumber++;

            if (attackNumber >= comboLimit && comboLimit != 0)
            {
                
                isComboEnd = true;

            }

            //距離が移動範囲内で、ロックオンするなら距離を変える
            //ロックオン機能がまだできてない
           // float moveDistance = (GManager.instance.useAtValue.lockAttack && distance.x < GManager.instance.useAtValue._moveDistance) ? distance.x : GManager.instance.useAtValue._moveDistance;

            _rush.RushStart(GManager.instance.useAtValue._moveDuration, GManager.instance.useAtValue._moveDistance, GManager.instance.useAtValue._contactType, GManager.instance.useAtValue.fallAttack, GManager.instance.useAtValue.startMoveTime);
            GManager.instance.StaminaUse(GManager.instance.useAtValue.useStamina);

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
                if (!GManager.instance.twinHand)// && CheckEnd("OParry"))
                {
                    // Debug.Log("sss");
                    isParring = false;
                    GManager.instance.parrySuccess = false;
                    GManager.instance.pm.SetLayer(11);
                    GManager.instance.guardDisEnable = false;
                    // GManager.instance.isDown = false;
                }
                if (GManager.instance.twinHand)// && CheckEnd("TParry"))
                {
                    isParring = false;
                    GManager.instance.parrySuccess = false;
                    GManager.instance.pm.SetLayer(11);
                    GManager.instance.guardDisEnable = false;

                }
            }
            else if (GManager.instance.blocking && isParring)
            {
                if (!GManager.instance.twinHand)// && CheckEnd("OBlock"))
                {
                    isParring = false;
                    GManager.instance.parrySuccess = false;
                    GManager.instance.pm.SetLayer(11);
                    GManager.instance.guardDisEnable = false;
                    //GManager.instance.isDown = false;
                }
                if (GManager.instance.twinHand)// && CheckEnd("TBlock"))
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
                _damage._attackData._hitLimit = GManager.instance.equipWeapon.sValue[attackNumber]._hitLimit;

                GManager.instance.useAtValue._moveDuration = GManager.instance.equipWeapon.sValue[attackNumber]._moveDuration;
                GManager.instance.useAtValue._moveDistance = GManager.instance.equipWeapon.sValue[attackNumber]._moveDistance;
                GManager.instance.useAtValue._contactType = GManager.instance.equipWeapon.sValue[attackNumber]._contactType;
                GManager.instance.useAtValue.fallAttack = GManager.instance.equipWeapon.sValue[attackNumber].fallAttack;
                GManager.instance.useAtValue.startMoveTime = GManager.instance.equipWeapon.sValue[attackNumber].startMoveTime;
                GManager.instance.useAtValue.lockAttack = GManager.instance.equipWeapon.sValue[attackNumber].lockAttack;
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
                _damage._attackData._hitLimit = GManager.instance.equipWeapon.twinSValue[attackNumber]._hitLimit;

                GManager.instance.useAtValue._moveDuration = GManager.instance.equipWeapon.twinSValue[attackNumber]._moveDuration;
                GManager.instance.useAtValue._moveDistance = GManager.instance.equipWeapon.twinSValue[attackNumber]._moveDistance;
                GManager.instance.useAtValue._contactType = GManager.instance.equipWeapon.twinSValue[attackNumber]._contactType;
                GManager.instance.useAtValue.fallAttack = GManager.instance.equipWeapon.twinSValue[attackNumber].fallAttack;
                GManager.instance.useAtValue.startMoveTime = GManager.instance.equipWeapon.twinSValue[attackNumber].startMoveTime;
                GManager.instance.useAtValue.lockAttack = GManager.instance.equipWeapon.twinSValue[attackNumber].lockAttack;
            }
        }

        public void bAttackPrepare()//デフォが斬撃。強攻撃
        {
            GManager.instance.isShieldAttack = false;


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
                _damage._attackData._hitLimit = GManager.instance.equipWeapon.bValue[attackNumber]._hitLimit;

                GManager.instance.useAtValue._moveDuration = GManager.instance.equipWeapon.bValue[attackNumber]._moveDuration;
                GManager.instance.useAtValue._moveDistance = GManager.instance.equipWeapon.bValue[attackNumber]._moveDistance;
                GManager.instance.useAtValue._contactType = GManager.instance.equipWeapon.bValue[attackNumber]._contactType;
                GManager.instance.useAtValue.fallAttack = GManager.instance.equipWeapon.bValue[attackNumber].fallAttack;
                GManager.instance.useAtValue.startMoveTime = GManager.instance.equipWeapon.bValue[attackNumber].startMoveTime;
                GManager.instance.useAtValue.lockAttack = GManager.instance.equipWeapon.bValue[attackNumber].lockAttack;
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
                _damage._attackData._hitLimit = GManager.instance.equipWeapon.twinBValue[attackNumber]._hitLimit;

                //突進用の初期化
                GManager.instance.useAtValue._moveDuration = GManager.instance.equipWeapon.twinBValue[attackNumber]._moveDuration;
                GManager.instance.useAtValue._moveDistance = GManager.instance.equipWeapon.twinBValue[attackNumber]._moveDistance;
                GManager.instance.useAtValue._contactType = GManager.instance.equipWeapon.twinBValue[attackNumber]._contactType;
                GManager.instance.useAtValue.fallAttack = GManager.instance.equipWeapon.twinBValue[attackNumber].fallAttack;
                GManager.instance.useAtValue.startMoveTime = GManager.instance.equipWeapon.twinBValue[attackNumber].startMoveTime;
                GManager.instance.useAtValue.lockAttack = GManager.instance.equipWeapon.twinBValue[attackNumber].lockAttack;
            }
        }

        public void chargeAttackPrepare()//デフォが斬撃
        {
            GManager.instance.isShieldAttack = false;


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
                _damage._attackData._hitLimit = GManager.instance.equipWeapon.chargeValue[attackNumber]._hitLimit;

                //突進用の初期化
                GManager.instance.useAtValue._moveDuration = GManager.instance.equipWeapon.chargeValue[attackNumber]._moveDuration;
                GManager.instance.useAtValue._moveDistance = GManager.instance.equipWeapon.chargeValue[attackNumber]._moveDistance;
                GManager.instance.useAtValue._contactType = GManager.instance.equipWeapon.chargeValue[attackNumber]._contactType;
                GManager.instance.useAtValue.fallAttack = GManager.instance.equipWeapon.chargeValue[attackNumber].fallAttack;
                GManager.instance.useAtValue.startMoveTime = GManager.instance.equipWeapon.chargeValue[attackNumber].startMoveTime;
                GManager.instance.useAtValue.lockAttack = GManager.instance.equipWeapon.chargeValue[attackNumber].lockAttack;

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
                _damage._attackData._hitLimit = GManager.instance.equipWeapon.twinChargeValue[attackNumber]._hitLimit;

                //突進用の初期化
                GManager.instance.useAtValue._moveDuration = GManager.instance.equipWeapon.twinChargeValue[attackNumber]._moveDuration;
                GManager.instance.useAtValue._moveDistance = GManager.instance.equipWeapon.twinChargeValue[attackNumber]._moveDistance;
                GManager.instance.useAtValue._contactType = GManager.instance.equipWeapon.twinChargeValue[attackNumber]._contactType;
                GManager.instance.useAtValue.fallAttack = GManager.instance.equipWeapon.twinChargeValue[attackNumber].fallAttack;
                GManager.instance.useAtValue.startMoveTime = GManager.instance.equipWeapon.twinChargeValue[attackNumber].startMoveTime;
                GManager.instance.useAtValue.lockAttack = GManager.instance.equipWeapon.twinChargeValue[attackNumber].lockAttack;
            }
        }
        public void airAttackPrepare()//デフォが斬撃
        {

            GManager.instance.isShieldAttack = false;



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
                _damage._attackData._hitLimit = GManager.instance.equipWeapon.airValue[attackNumber]._hitLimit;

                //突進用の初期化
                GManager.instance.useAtValue._moveDuration = GManager.instance.equipWeapon.airValue[attackNumber]._moveDuration;
                GManager.instance.useAtValue._moveDistance = GManager.instance.equipWeapon.airValue[attackNumber]._moveDistance;
                GManager.instance.useAtValue._contactType = GManager.instance.equipWeapon.airValue[attackNumber]._contactType;
                GManager.instance.useAtValue.fallAttack = GManager.instance.equipWeapon.airValue[attackNumber].fallAttack;
                GManager.instance.useAtValue.startMoveTime = GManager.instance.equipWeapon.airValue[attackNumber].startMoveTime;
                GManager.instance.useAtValue.lockAttack = GManager.instance.equipWeapon.airValue[attackNumber].lockAttack;

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
                _damage._attackData._hitLimit = GManager.instance.equipWeapon.twinAirValue[attackNumber]._hitLimit;

                //突進用の初期化
                GManager.instance.useAtValue._moveDuration = GManager.instance.equipWeapon.twinAirValue[attackNumber]._moveDuration;
                GManager.instance.useAtValue._moveDistance = GManager.instance.equipWeapon.twinAirValue[attackNumber]._moveDistance;
                GManager.instance.useAtValue._contactType = GManager.instance.equipWeapon.twinAirValue[attackNumber]._contactType;
                GManager.instance.useAtValue.fallAttack = GManager.instance.equipWeapon.twinAirValue[attackNumber].fallAttack;
                GManager.instance.useAtValue.startMoveTime = GManager.instance.equipWeapon.twinAirValue[attackNumber].startMoveTime;
                GManager.instance.useAtValue.lockAttack = GManager.instance.equipWeapon.twinAirValue[attackNumber].lockAttack;

            }
        }
        public void strikeAttackPrepare()//デフォが斬撃
        {

            GManager.instance.isShieldAttack = false;



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
                _damage._attackData._hitLimit = GManager.instance.equipWeapon.strikeValue[attackNumber]._hitLimit;

                //突進用の初期化
                GManager.instance.useAtValue._moveDuration = GManager.instance.equipWeapon.strikeValue[attackNumber]._moveDuration;
                GManager.instance.useAtValue._moveDistance = GManager.instance.equipWeapon.strikeValue[attackNumber]._moveDistance;
                GManager.instance.useAtValue._contactType = GManager.instance.equipWeapon.strikeValue[attackNumber]._contactType;
                GManager.instance.useAtValue.fallAttack = GManager.instance.equipWeapon.strikeValue[attackNumber].fallAttack;
                GManager.instance.useAtValue.startMoveTime = GManager.instance.equipWeapon.strikeValue[attackNumber].startMoveTime;
                GManager.instance.useAtValue.lockAttack = GManager.instance.equipWeapon.strikeValue[attackNumber].lockAttack;
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
                _damage._attackData._hitLimit = GManager.instance.equipWeapon.twinStrikeValue[attackNumber]._hitLimit;

                //突進用の初期化
                GManager.instance.useAtValue._moveDuration = GManager.instance.equipWeapon.twinStrikeValue[attackNumber]._moveDuration;
                GManager.instance.useAtValue._moveDistance = GManager.instance.equipWeapon.twinStrikeValue[attackNumber]._moveDistance;
                GManager.instance.useAtValue._contactType = GManager.instance.equipWeapon.twinStrikeValue[attackNumber]._contactType;
                GManager.instance.useAtValue.fallAttack = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].fallAttack;
                GManager.instance.useAtValue.startMoveTime = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].startMoveTime;
                GManager.instance.useAtValue.lockAttack = GManager.instance.equipWeapon.twinStrikeValue[attackNumber].lockAttack;
            }
            comboLimit = 1;
        }

        public void ArtsPrepare()//デフォが斬撃
        {


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
                _damage._attackData._hitLimit = GManager.instance.equipShield.artsValue[attackNumber]._hitLimit;

                //突進用の初期化
                GManager.instance.useAtValue._moveDuration = GManager.instance.equipShield.artsValue[attackNumber]._moveDuration;
                GManager.instance.useAtValue._moveDistance = GManager.instance.equipShield.artsValue[attackNumber]._moveDistance;
                GManager.instance.useAtValue._contactType = GManager.instance.equipShield.artsValue[attackNumber]._contactType;
                GManager.instance.useAtValue.fallAttack = GManager.instance.equipShield.artsValue[attackNumber].fallAttack;
                GManager.instance.useAtValue.startMoveTime = GManager.instance.equipShield.artsValue[attackNumber].startMoveTime;
                GManager.instance.useAtValue.lockAttack = GManager.instance.equipShield.artsValue[attackNumber].lockAttack;
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
                _damage._attackData._hitLimit = GManager.instance.equipWeapon.artsValue[attackNumber]._hitLimit;

                //突進用の初期化
                GManager.instance.useAtValue._moveDuration = GManager.instance.equipWeapon.artsValue[attackNumber]._moveDuration;
                GManager.instance.useAtValue._moveDistance = GManager.instance.equipWeapon.artsValue[attackNumber]._moveDistance;
                GManager.instance.useAtValue._contactType = GManager.instance.equipWeapon.artsValue[attackNumber]._contactType;
                GManager.instance.useAtValue.fallAttack = GManager.instance.equipWeapon.artsValue[attackNumber].fallAttack;
                GManager.instance.useAtValue.startMoveTime = GManager.instance.equipWeapon.artsValue[attackNumber].startMoveTime;
                GManager.instance.useAtValue.lockAttack = GManager.instance.equipWeapon.artsValue[attackNumber].lockAttack;
            }
        }
        #endregion




    }











}