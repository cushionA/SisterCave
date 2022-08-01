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
        protected const string _attackParameterName = "AttackNow";
        protected int _attackAnimationParameter;

        //内部パラメータ
        #region
        //------------------------------------------内部パラメータ

        //PlayerMoveGManager.instance.pm;
        //attackNumberと連動してステート名を獲得
        /// <summary>
        /// 弱攻撃ボタンが押されてるかどうか
        /// </summary>
        bool fire1Key;

        /// <summary>
        /// 強攻撃ボタンが押されてるかどうか
        /// </summary>
        bool fire2Key;
        /// <summary>
        /// 武器の両手持ち切り替えがされてるかどうか
        /// </summary>
        bool changeKey;


        /// <summary>
        /// 特殊攻撃攻撃ボタンが押されてるかどうか
        /// </summary>
        bool artsKey;

        /// <summary>
        /// ため押し時間測る入れ物
        /// </summary>
        float chargeTime;
        //   float gravity;//重力を入れる

        bool isDisEnable;//空中弱攻撃を二回までに制限


        // float delayTime;
        int attackNumber;
        int alterNumber;
        int artsNumber;
        //Animator anim;
        [HideInInspector] public bool isAttackable;
        bool smallTrigger;
        bool bigTrigger;
        bool artsTrigger;
        bool MagicTrigger;
        //連撃のトリガーになる
        bool bigAttack;
        //強攻撃
        bool chargeAttack;
        bool isCharging;
        //武器変更後の両手切り替え暴発予防
        bool equipChange;
        //チャージ中
        bool chargeKey;
        float horizontalKey;
        bool anyKey;
        float attackDirection;


        /// <summary>
        /// コンボ振り切った
        /// </summary>
        bool isSComboEnd;
        bool isBComboEnd;
        bool isAComboEnd;

        // ///<summary>
        ///   ///攻撃の発生保障
        //    /// </summary>
        //  bool lastAttack;
        //  float testtime;
        bool cAttack;

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

        //   AttackValue atV;

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
            int state = 0;

            //スタミナ利用可能なら
            if (!GManager.instance.isEnable)
            {
                if (_inputManager.sAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonDown || smallTrigger)
                {
                    state = 1;
                    // DoSomething();
                    smallTrigger = false;
                }
                else if (_inputManager.sAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonDown || bigTrigger)
                {
                    state = 3;
                    bigTrigger = false;
                }
                else if (_inputManager.ArtsButton.State.CurrentState == MMInput.ButtonStates.ButtonDown || artsTrigger)
                {
                    state = 5;
                    artsTrigger = false;
                }
            }
           if (GManager.instance.equipWeapon.isCombo)
            {
                if(attackNumber > 0)
                {
                    state = 1;
                }
                else if(alterNumber > 0)
                {
                    state = 3;

                }
                else
                {
                    state = 5;
                }
            //  DoSomething(state);
            }

            if(state != 0)
            {
                if (!_controller.State.IsGrounded)
                {
                    state++;
                    //Artsは6になるね
                }
                DoSomething(state);
               // UpdateAnimator();
            }

            //攻撃終了ェック
            if (GManager.instance.isAttack && isAttackable)
            {
                anyKey = AnyKey();
                AttackCheck(state);
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
        /// 押し込んでいる場合は、いくつかの条件を満たしているかどうかをチェックして、アクションを実行できるかどうかを確認します。
        /// </summary>
        protected virtual void DoSomething(int state)
        {
            // やらない条件
            if (!AbilityPermitted
                // or if we're not in our normal stance
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                // or if we're grounded
                || (!_controller.State.IsGrounded)
                // or if we're gripping
                || (_movement.CurrentState == CharacterStates.MovementStates.Gripping))
            {
                // we do nothing and exit
                return;
            }

            if(state == 3)
            {
                chargeTime += Time.deltaTime;
                //チャージ中
                if (chargeTime >= GManager.instance.equipWeapon.chargeRes)
                {
                    // isCharging = false;
                    chargeTime = 0.0f;
                    chargeAttack = true;
                }
            }
            else
            {
                chargeTime = 0.0f;
            }

            AttackAct(state);
            NumberControll();
            // if we're still here, we display a text log in the console
            MMDebug.DebugLogTime("We're doing something yay!");
        }

        /// <summary>
        ///  必要なアニメーターパラメーターがあれば、アニメーターパラメーターリストに追加します。
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_attackParameterName, AnimatorControllerParameterType.Bool, out _attackAnimationParameter);
        }

        /// <summary>
        /// アビリティのサイクルが終了した時点。
        /// 現在のしゃがむ、這うの状態をアニメーターに送る。
        /// </summary>
        public override void UpdateAnimator()
        {
            //今のステートがAttackであるかどうかでBool入れ替えてる
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _attackAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Attack), _character._animatorParameters);
        }

       // 番号管理何とかしないとな
        void AttackAct(int state)
        {
            if (GManager.instance.pm.isEnAt && !GManager.instance.isAttack && GManager.instance.isEnable && !GManager.instance.parrySuccess)
            {
                 _movement.ChangeState(CharacterStates.MovementStates.Attack);


                //攻撃可能で弱攻撃ボタン押されてて攻撃してなくてスタミナが使えるなら以下の処理
                //delayTime = 0.0f;
                if (state% 2== 1)
                {
                    isDisEnable = false;
                    if (state == 1)
                    {
                        NormalAttack();
                    }
                    else if (state == 5)
                    {
                        ArtsAttack();
                    }

                    #region//ため攻撃
                    if (state == 3)
                    {

                        isCharging = true;
                        isAttackable = false;
                        GManager.instance.isAttack = true;
                        //  GManager.instance.pm.rb.velocity = Vector2.zero;
                     // bigTrigger = false;

                        #endregion
                    }
                }
                else //if (GManager.instance.pm.isEnAt && !GManager.instance.isAttack && GManager.instance.isEnable)
                {
                    AirAttack(state);
                }
                // lastAttack = true;

            }

            if (GManager.instance.isAttack && isCharging)
            {
                if (!GManager.instance.equipWeapon.isMagic)
                {
                    ChargeAttack();
                }
                else
                {
                    //ため時間を発射保留と照準に回した射撃メソッドを

                }
            }

            //NumberControll();
           // await UniTask.RunOnThreadPool(() => NumberControll());
            //UniTask.co
           
            #region//連撃入力とキャンセル待ち
/*
            if (GManager.instance.isAttack && isAttackable && !GManager.instance.airAttack && !isCharging && !GManager.instance.fallAttack)
            {
                GroundCheck();
            }
            #endregion
            #region//空中連撃入力とキャンセル待ち
            else if (GManager.instance.isAttack && isAttackable && GManager.instance.airAttack && !GManager.instance.fallAttack)
            {
                AirCheck();
                GManager.instance.pm.rb.velocity = Vector2.zero;
            }*/


            #endregion
            #region//地上モーション終了検査
            if (!GManager.instance.airAttack)
            {
                if (attackNumber >= 1 && !isSComboEnd)
                {
                    if (!GManager.instance.twinHand)
                    {
                        if (CheckEnd($"OSAttack{attackNumber}") == false)
                        {
                            // //////Debug.log("機能してます");
                            attackNumber = 0;
                            GManager.instance.isAttack = false;
                            smallTrigger = false;
                            //保障された攻撃をしました

                        }
                    }
                    else
                    {
                        if (CheckEnd($"TSAttack{attackNumber}") == false)
                        {
                            // //////Debug.log("機能してます");
                            attackNumber = 0;
                            GManager.instance.isAttack = false;
                            smallTrigger = false;
                            //保障された攻撃をしました

                        }
                    }
                }
                else if (alterNumber >= 1 && !cAttack && !isBComboEnd)
                {
                    if (!GManager.instance.twinHand)
                    {
                        if (CheckEnd($"OBAttack{alterNumber}") == false)
                        {
                            ////////Debug.log("機能してます");
                            alterNumber = 0;
                            bigAttack = false;
                            //  chargeAttack = false;
                            GManager.instance.isAttack = false;
                            bigTrigger = false;
                            //保障された攻撃をしました

                        }
                    }
                    else
                    {
                        if (CheckEnd($"TBAttack{alterNumber}") == false)
                        {
                            ////////Debug.log("機能してます");
                            alterNumber = 0;
                            bigAttack = false;
                            //    chargeAttack = false;
                            GManager.instance.isAttack = false;
                            bigTrigger = false;
                            //保障された攻撃をしました

                        }
                    }

                }
                else if (alterNumber >= 1 && cAttack && !isBComboEnd)
                {
                    if (!GManager.instance.twinHand)
                    {
                        if (CheckEnd($"OCAttack{alterNumber}") == false)
                        {
                            ////////Debug.log("機能してます");
                            alterNumber = 0;
                            bigAttack = false;
                            //   chargeAttack = false;
                            GManager.instance.isAttack = false;
                            bigTrigger = false;
                            //保障された攻撃をしました
                            cAttack = false;
                        }
                    }
                    else
                    {
                        if (CheckEnd($"TCAttack{alterNumber}") == false)
                        {
                            ////////Debug.log("機能してます");
                            alterNumber = 0;
                            bigAttack = false;
                            //   chargeAttack = false;
                            GManager.instance.isAttack = false;
                            bigTrigger = false;
                            //保障された攻撃をしました

                        }
                    }

                }
                else if (artsNumber >= 1 && !isAComboEnd)
                {
                    if (!GManager.instance.twinHand && !GManager.instance.equipShield.weaponArts)
                    {
                        if (CheckEnd($"OArts{artsNumber}") == false)
                        {
                            // //////Debug.log("機能してます");
                            artsNumber = 0;
                            GManager.instance.isAttack = false;
                            artsTrigger = false;
                            //保障された攻撃をしました

                        }
                    }
                    else
                    {
                        if (CheckEnd($"TArts{artsNumber}") == false)
                        {
                            // //////Debug.log("機能してます");
                            artsNumber = 0;
                            GManager.instance.isAttack = false;
                            artsTrigger = false;


                        }
                        //保障された攻撃をしました

                    }
                }
                else if (startFall)
                {
                    if (!GManager.instance.twinHand)
                    {
                        if (CheckEnd($"OLanding") == false)
                        {
                            // //////Debug.log("機能してます");
                            //   artsNumber = 0;
                            GManager.instance.isAttack = false;
                            //   artsTrigger = false;
                            //保障された攻撃をしました
                            startFall = false;
                            GManager.instance.fallAttack = false;
                        }
                    }
                    else
                    {
                        if (CheckEnd($"TLanding") == false)
                        {
                            // //////Debug.log("機能してます");
                            //   artsNumber = 0;
                            GManager.instance.isAttack = false;
                            //  artsTrigger = false;
                            GManager.instance.fallAttack = false;
                            startFall = false;
                        }
                        //保障された攻撃をしました

                    }
                    //GManager.instance.pm.rb.velocity = Vector2.zero;
                }

                ///<Summary>
                /// コンボ終了後のアニメ終了確認
                ///</Summary>
                #region
                else if (isSComboEnd)
                {
                    if (!GManager.instance.twinHand)
                    {
                        if (CheckEnd($"OSAttack{GManager.instance.equipWeapon.sValue.Count}") == false)
                        {
                            // //////Debug.log("機能してます");
                            attackNumber = 0;
                            GManager.instance.isAttack = false;
                            smallTrigger = false;
                            isSComboEnd = false;
                            //保障された攻撃をしました

                        }

                    }
                    else
                    {
                        if (CheckEnd($"TSAttack{GManager.instance.equipWeapon.twinSValue.Count}") == false)
                        {
                            // //////Debug.log("機能してます");
                            attackNumber = 0;
                            GManager.instance.isAttack = false;
                            smallTrigger = false;
                            isSComboEnd = false;
                            //保障された攻撃をしました


                        }
                    }
                }
                else if (isBComboEnd && cAttack)
                {
                    if (!GManager.instance.twinHand)
                    {
                        if (CheckEnd($"OCAttack{GManager.instance.equipWeapon.chargeValue.Count}") == false)
                        {
                            //Debug.Log("機能してます");
                            alterNumber = 0;
                            bigAttack = false;
                            //  chargeAttack = false;
                            GManager.instance.isAttack = false;
                            bigTrigger = false;
                            isBComboEnd = false;
                            //保障された攻撃をしました

                            cAttack = false;

                        }
                    }
                    else
                    {
                        if (CheckEnd($"TCAttack{GManager.instance.equipWeapon.twinChargeValue.Count}") == false)
                        {
                            ////////Debug.log("機能してます");
                            alterNumber = 0;
                            bigAttack = false;
                            //chargeAttack = false;
                            GManager.instance.isAttack = false;
                            bigTrigger = false;
                            isBComboEnd = false;
                            //保障された攻撃をしました
                            cAttack = false;
                        }
                    }
                }
                else if (isBComboEnd && !cAttack)
                {
                    if (!GManager.instance.twinHand)
                    {
                        if (CheckEnd($"OBAttack{GManager.instance.equipWeapon.bValue.Count}") == false)
                        {
                            ////////Debug.log("機能してます");
                            alterNumber = 0;
                            bigAttack = false;
                            //   chargeAttack = false;
                            GManager.instance.isAttack = false;
                            bigTrigger = false;
                            isBComboEnd = false;
                            //保障された攻撃をしました

                        }
                    }
                    else
                    {
                        if (CheckEnd($"TBAttack{GManager.instance.equipWeapon.twinBValue.Count}") == false)
                        {
                            ////////Debug.log("機能してます");
                            alterNumber = 0;
                            bigAttack = false;
                            chargeAttack = false;
                            GManager.instance.isAttack = false;
                            bigTrigger = false;
                            isBComboEnd = false;
                            //保障された攻撃をしました

                        }
                    }
                }

                else if (isAComboEnd)
                {
                    if (!GManager.instance.twinHand && !GManager.instance.equipShield.weaponArts)
                    {
                        if (CheckEnd($"OArts{GManager.instance.equipShield.artsValue.Count}") == false)
                        {
                            ////Debug.Log("終了リセット");
                            // //////Debug.log("機能してます");
                            artsNumber = 0;
                            GManager.instance.isAttack = false;
                            artsTrigger = false;
                            isAComboEnd = false;
                        }
                    }
                    else
                    {
                        if (CheckEnd($"TArts{GManager.instance.equipWeapon.artsValue.Count}") == false)
                        {
                            // //////Debug.log("機能してます");
                            artsNumber = 0;
                            GManager.instance.isAttack = false;
                            artsTrigger = false;
                            isAComboEnd = false;
                        }

                    }
                }


                #endregion

            }
            #endregion
           #region//空中モーション終了検査
            else if (GManager.instance.airAttack)
            {
                if (!GManager.instance.twinHand)
                {
                    if (attackNumber >= 1 && !isDisEnable)
                    {

                        if (CheckEnd($"OAAttack{attackNumber}") == false)
                        {
                            ////////Debug.log("機能してます");
                            GManager.instance.isAttack = false;
                            GManager.instance.airAttack = false;
                            smallTrigger = false;
                            attackNumber = 0;
                        }
                        /* if (isDisEnable)
                                                {
                                                    GManager.instance.airAttack = false;
                                                    attackNumber =_controller.State.IsGrounded ? 0 : attackNumber;
                                                    //isGroundの時attackNumberを0、違うならそのまま
                                                }*/
                    }
                    else if (alterNumber >= 1 && !GManager.instance.fallAttack)
                    {

                        if (CheckEnd($"OFAttack{alterNumber}") == false)
                        {
                            ////////Debug.log("機能してます");
                            GManager.instance.isAttack = false;
                            GManager.instance.airAttack = false;
                            bigTrigger = false;
                            alterNumber = 0;
                        }
                        /*                   if (GManager.instance.fallAttack)
                                           {
                                               GManager.instance.airAttack = false;
                                               attackNumber =_controller.State.IsGrounded ? 0 : attackNumber;
                                               //isGroundの時attackNumberを0、違うならそのまま
                                           }*/
                    }                
                    else if (isDisEnable)
                    {
                        if (CheckEnd($"OAAttack{GManager.instance.equipWeapon.airValue.Count}") == false)
                        {
                            // //////Debug.log("機能してます");
                            attackNumber = 0;
                            GManager.instance.isAttack = false;
                            smallTrigger = false;
                            GManager.instance.airAttack = false;
                            isSComboEnd = false;
                        }
                    }
                    //       else if (isBComboEnd)
                    //       {
                    /*     if (CheckEnd($"OFAttack{GManager.instance.equipWeapon.strikeValue.Count}") == false)
                         {
                             ////////Debug.log("機能してます");
                             alterNumber = 0;
                             bigAttack = false;
                             // chargeAttack = false;
                             GManager.instance.isAttack = false;
                             bigTrigger = false;
                             isBComboEnd = false;
                             GManager.instance.airAttack = false;
                         }
                     }    */
                    //    }
                }

                else
                {
                    if (attackNumber >= 1 && !isDisEnable)
                    {

                        if (CheckEnd($"TAAttack{attackNumber}") == false)
                        {
                            ////////Debug.log("機能してます");
                            GManager.instance.isAttack = false;
                            GManager.instance.airAttack = false;
                            smallTrigger = false;
                            attackNumber = 0;
                        }
                        /*                        if (isDisEnable)
                                                {
                                                    GManager.instance.airAttack = false;
                                                    attackNumber =_controller.State.IsGrounded ? 0 : attackNumber;
                                                    //isGroundの時attackNumberを0、違うならそのまま
                                                }*/
                    }
                    else if (alterNumber >= 1 && !isBComboEnd)
                    {

                        if (CheckEnd($"TFAttack{alterNumber}") == false)
                        {
                            ////////Debug.log("機能してます");
                            GManager.instance.isAttack = false;
                            GManager.instance.airAttack = false;
                            bigTrigger = false;
                            alterNumber = 0;
                        }
                        /*                   if (GManager.instance.fallAttack)
                                           {
                                               GManager.instance.airAttack = false;
                                               attackNumber =_controller.State.IsGrounded ? 0 : attackNumber;
                                               //isGroundの時attackNumberを0、違うならそのまま
                                           }*/
                    }
                    else if (isDisEnable)
                    {
                        if (CheckEnd($"TAAttack{GManager.instance.equipWeapon.twinAirValue.Count}") == false)
                        {
                            // //////Debug.log("機能してます");
                            attackNumber = 0;
                            GManager.instance.isAttack = false;
                            smallTrigger = false;

                            GManager.instance.airAttack = false;
                        }
                    }
                    //else if (GManager.instance.fallAttack)
                    //  {
                    /*      if (CheckEnd($"TFAttack{GManager.instance.equipWeapon.twinStrikeValue.Count}") == false)
                          {
                              ////////Debug.log("機能してます");
                              alterNumber = 0;
                              bigAttack = false;
                              //chargeAttack = false;
                              GManager.instance.isAttack = false;
                              bigTrigger = false;
                              isBComboEnd = false;
                          GManager.instance.airAttack = false;
                      }*/
                    //}
                }
                if(GManager.instance.isAttack == false)
                {
                    _movement.ChangeState(CharacterStates.MovementStates.Idle);
                }
            }
            #endregion



            // //////Debug.log($"判定{GManager.instance.isAttack}");
            // //////Debug.log($"空中攻撃は{attackNumber}");

            if (GManager.instance.fallAttack)
            {
                if (startFall)
                {
                    // gravity =GManager.instance.pm.gravity * 3f;
                    GManager.instance.pm.move.Set(0, -GManager.instance.pm.gravity * 3);
                    GManager.instance.pm.rb.velocity = GManager.instance.pm.move;
                }
                else
                {
                    GManager.instance.pm.rb.velocity = Vector2.zero;
                }
                //  ////Debug.Log($"着地後{groundTime}");
                if (_controller.State.IsGrounded)
                {

                    if (GManager.instance.twinHand)
                    {
                        GManager.instance.pm.anim.Play("TLanding");
                    }
                    else
                    {
                        GManager.instance.pm.anim.Play("OLanding");
                    }
                    groundTime += _controller.DeltaTime;
                    GManager.instance.pm.rb.velocity = Vector2.zero;
                    GManager.instance.isArmor = false;
                    GManager.instance.airAttack = false;
                    // GManager.instance.pm.jumpTime = 0.0f;
                    if (groundTime >= 0.1f)
                    {
                        isAttackable = true;
                        //GManager.instance.isAttack = false;
                        //  GManager.instance.airAttack = false;
                        // GManager.instance.isArmor = false;
                        isBComboEnd = false;
                        groundTime = 0;
                        // //////Debug.log("機能してます");
                        attackNumber = 0;
                        alterNumber = 0;
                        //GManager.instance.isAttack = false;
                        smallTrigger = false;
                        bigTrigger = false;

                        // GManager.instance.pm.jumpTime = 0.0f;
                        GManager.instance.fallAttack = false;
                    }
                }
            }

            if (!GManager.instance.airAttack && !startFall)
            {
                if (GManager.instance.isAttack && !isAttackable && (!_controller.State.IsGrounded || GManager.instance.pm.isSloopDown))
                {
                    GManager.instance.pm.move.Set(0, -GManager.instance.pm.gravity);
                    GManager.instance.pm.rb.velocity = GManager.instance.pm.move;
                }
                else if (GManager.instance.isAttack && isAttackable && _controller.State.IsGrounded)
                {
                    GManager.instance.pm.rb.velocity = Vector2.zero;
                }
                else if (GManager.instance.isAttack && isAttackable && !_controller.State.IsGrounded)
                {
                    GManager.instance.pm.move.Set(0, -GManager.instance.pm.gravity);
                    GManager.instance.pm.rb.velocity = GManager.instance.pm.move;
                }
                /*     else if(GManager.instance.isAttack && !isAttackable && !GManager.instance.pm.isSloopDown)
                     {
                         GManager.instance.pm.move.Set(GManager.instance.pm.rb.velocity.x, 0);
                         GManager.instance.pm.rb.velocity = GManager.instance.pm.move;
                     }*/

            }

            if (GManager.instance.isAttack)//攻撃後の方向転換
            {
                if (horizontalKey > 0)
                {
                    attackDirection = 1;
                }
                else if (horizontalKey < 0)
                {
                    attackDirection = -1;
                }
                else
                {
                    attackDirection = transform.localScale.x;
                }
            }
        }


        //アニメーションイベント
        #region
        public void Continue()
        {
            // GManager.instance.pm.anim.Play("OArts1");

            ////Debug.Log("攻撃可能に");

            GManager.instance.StaminaUse(GManager.instance.useAtValue.useStamina);

            if (GManager.instance.fallAttack)
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



        //管理系
        #region 

        //アニメの終了探知
        bool CheckEnd(string Name)
        {

            if (!GManager.instance.pm.anim.GetCurrentAnimatorStateInfo(0).IsName(Name))// || GManager.instance.pm.anim.GetCurrentAnimatorStateInfo(0).IsName("OStand"))
            {   // ここに到達直後はnormalizedTimeが"Default"の経過時間を拾ってしまうので、Resultに遷移完了するまではreturnする。
                return true;
            }
            if (GManager.instance.pm.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {   // 待機時間を作りたいならば、ここの値を大きくする。
                return true;
            }
            //      AnimatorClipInfo[] clipInfo = GManager.instance.pm.anim.GetCurrentAnimatorClipInfo(0);

            ////Debug.Log($"アニメ終了");

            return false;

            // return !(GManager.instance.pm.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
            //  (_currentStateName);
        }
        bool AnyKey()
        {
            //Anyは軸も行ける
            //キーコンフィグで反映されなかったりしたらInputRの参照そろえてないのを確認
            if (GManager.instance.InputR.GetAnyButton())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void NumberControll()
        {

            #region
            if (!GManager.instance.airAttack)
            {
                if (!isAComboEnd && !isBComboEnd && !isSComboEnd)
                {
                    if (!GManager.instance.twinHand)
                    {

                        if (attackNumber >= GManager.instance.equipWeapon.sValue.Count)
                        {
                            attackNumber = 0;//モーション番号のリセット
                            isSComboEnd = true;
                        }
                        else if (alterNumber >= GManager.instance.equipWeapon.bValue.Count)
                        {
                            alterNumber = 0;//モーション番号のリセット
                            isBComboEnd = true;
                        }

                        else if (GManager.instance.equipShield.weaponArts)
                        {
                            if (artsNumber >= GManager.instance.equipWeapon.artsValue.Count)
                            {
                                artsNumber = 0;//モーション番号のリセット
                                               //      if(GManager.instance.equipWeapon.artsValue.Count > 1)
                                               //         {
                                isAComboEnd = true;
                                // }
                            }
                        }
                        else
                        {
                            if (artsNumber >= GManager.instance.equipShield.artsValue.Count)
                            {
                                artsNumber = 0;//モーション番号のリセット
                                               //  if (GManager.instance.equipShield.artsValue.Count > 1)
                                               //     {
                                isAComboEnd = true;
                                //     }
                                ////Debug.Log($"コンボ終了");
                            }
                        }
                    }
                    else
                    {
                        if (attackNumber >= GManager.instance.equipWeapon.twinSValue.Count)
                        {
                            attackNumber = 0;//モーション番号のリセット
                            isSComboEnd = true;
                        }
                        else if (alterNumber >= GManager.instance.equipWeapon.twinBValue.Count)
                        {
                            alterNumber = 0;//モーション番号のリセット
                            isBComboEnd = true;
                        }

                        else if (artsNumber >= GManager.instance.equipWeapon.artsValue.Count)
                        {
                            artsNumber = 0;//モーション番号のリセット
                                           // if (GManager.instance.equipWeapon.artsValue.Count > 1)
                                           //   {
                            isAComboEnd = true;
                            //  }
                        }

                    }
                }
            }
            else
            {
                if (!GManager.instance.twinHand)
                {

                    if (attackNumber >= GManager.instance.equipWeapon.airValue.Count)
                    {
                        attackNumber = 0;//モーション番号のリセット
                        isDisEnable = true;
                        isSComboEnd = true;
                    }
                    if (alterNumber >= GManager.instance.equipWeapon.strikeValue.Count)
                    {
                        alterNumber = 0;//モーション番号のリセット
                        GManager.instance.fallAttack = true;
                        //isDisEnable = true;
                    }

                }
                else
                {
                    if (attackNumber >= GManager.instance.equipWeapon.twinAirValue.Count)
                    {
                        attackNumber = 0;//モーション番号のリセット
                                         //    isDisEnable = true;
                    }
                    if (alterNumber >= GManager.instance.equipWeapon.twinStrikeValue.Count)
                    {
                        alterNumber = 0;//モーション番号のリセット
                        GManager.instance.fallAttack = true;
                        // isDisEnable = true;
                    }

                }

            }
            #endregion
        }
        void GroundCheck(int state)
        {
            if (anyKey)
            {
                GManager.instance.isAttack = false;
                GManager.instance.fallAttack = false;
                attackNumber = 0;
                alterNumber = 0;
                artsNumber = 0;
                cAttack = false;
                startFall = false;
                //保障された攻撃をしました
                if (isBComboEnd || isSComboEnd || isAComboEnd)
                {
                    isBComboEnd = false;
                    isSComboEnd = false;
                    isAComboEnd = false;

                }
            }
            else if (state == 1)
            {
                //  ////Debug.Log($"連撃");
                GManager.instance.isAttack = false;
                smallTrigger = true;
                alterNumber = 0;
                artsNumber = 0;
                //強攻撃コンボを白紙化
                //保障された攻撃をしました
                startFall = false;
                GManager.instance.fallAttack = false;
                if (isSComboEnd)
                {
                    isSComboEnd = false;
                    attackNumber = 0;

                }
                cAttack = false;
            }
            else if (state == 3)
            {

                GManager.instance.isAttack = false;
                bigTrigger = true;
                attackNumber = 0;
                artsNumber = 0;
                //弱攻撃コンボを白紙化
                //保障された攻撃をしました
                startFall = false;
                GManager.instance.fallAttack = false;
                if (isBComboEnd)
                {
                    isBComboEnd = false;
                    alterNumber = 0;

                }
                cAttack = false;
            }
            else if (state == 5)
            {
                GManager.instance.isAttack = false;
                artsTrigger = true;
                attackNumber = 0;
                alterNumber = 0;
                GManager.instance.fallAttack = false;
                if (isAComboEnd)
                {
                    isAComboEnd = false;
                    artsNumber = 0;
                    ////Debug.Log($"連撃判定");
                    //artsTrigger = false;
                }


                // ////Debug.Log($"アニメ確認{clipInfo[0].clip.name}");
                cAttack = false;
            }
        }
        void AirCheck(int state)
        {
            if (anyKey || _controller.State.IsGrounded)
            {
                GManager.instance.isAttack = false;

                attackNumber = 0;
                alterNumber = 0;
                GManager.instance.airAttack = false;
            }
            else if (state == 2)
            {
                GManager.instance.isAttack = false;
                smallTrigger = true;
                bigTrigger = false;
                alterNumber = 0;
                GManager.instance.airAttack = false;
            }
            else if (state == 4)
            {
                GManager.instance.isAttack = false;
                bigTrigger = true;
                smallTrigger = false;
                attackNumber = 0;
                GManager.instance.airAttack = false;
            }

        }

        void AttackCheck(int state)
        {
            if (!GManager.instance.airAttack && !isCharging && !GManager.instance.fallAttack)
            {
                GroundCheck(state);
            }
          
            //空中連撃入力とキャンセル待ち
            else if (GManager.instance.airAttack && !GManager.instance.fallAttack)
            {
                AirCheck(state);
                //何らかの停止処理
               // GManager.instance.pm.rb.velocity = Vector2.zero;
            }
            if (GManager.instance.isAttack == false)
            {
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
            }
        }

        #endregion


        //攻撃実行系
        #region
        void NormalAttack()
        {
            #region//通常攻撃
            if (attackNumber == 0)
            {
                isSComboEnd = false;
            }
                GManager.instance.isAttack = true;
                // GManager.instance.pm.rb.velocity = Vector2.zero;
                sAttackPrepare();
                if (!GManager.instance.twinHand)
                {
                    GManager.instance.pm.anim.Play($"OSAttack{attackNumber + 1}");

                    // // GManager.instance.StaminaUse(useStamina);
                }
                else
                {
                    GManager.instance.pm.anim.Play($"TSAttack{attackNumber + 1}");
                    //// GManager.instance.StaminaUse(useStamina);
                }
                isAttackable = false;
                attackNumber++;
                smallTrigger = false;
            
            #endregion
        }
        void ChargeAttack()
        {

                if (alterNumber == 0)
                {
                isBComboEnd = false;
                }
                    if (chargeAttack)
                    {
                        
                        chargeAttackPrepare();
                        if (!GManager.instance.twinHand)
                        {
                            GManager.instance.pm.anim.Play($"OCAttack{alterNumber + 1}");//チャージ攻撃のアニメ
                                                                                         // // GManager.instance.StaminaUse(useStamina);
                        }
                        else
                        {
                            GManager.instance.pm.anim.Play($"TCAttack{alterNumber + 1}");//チャージ攻撃のアニメ
                                                                                         // // GManager.instance.StaminaUse(useStamina);
                        }
                        alterNumber++;

                        chargeAttack = false;
                        isCharging = false;
                        chargeTime = 0.0f;
                        cAttack = true;
                    }
                    else if (bigAttack)
                    {
                       //sBComboEnd = false;
                        bAttackPrepare();
                        if (!GManager.instance.twinHand)
                        {
                            GManager.instance.pm.anim.Play($"OBAttack{alterNumber + 1}");
                            // // GManager.instance.StaminaUse(useStamina);
                        }
                        else
                        {
                            GManager.instance.pm.anim.Play($"TBAttack{alterNumber + 1}");
                            // // GManager.instance.StaminaUse(useStamina);
                        }
                        alterNumber++;
                        // 違う効果音や画面揺らしたり、エフェクトも
                        bigAttack = false;
                        isCharging = false;
                        chargeTime = 0.0f;
                    }
                    else
                    {
                        if (!GManager.instance.twinHand)
                        {
                            GManager.instance.pm.anim.Play($"OCharge{alterNumber + 1}");//チャージアニメ    
                          //GManager.instance.pm.rb.velocity = Vector2.zero;
                          // 何らかの停止処理
                            //  GManager.instance.pm.move.Set(0,0);
                            // GManager.instance.pm.rb.velocity = GManager.instance.pm.move;
                        }
                        else
                        {
                          //GManager.instance.pm.rb.velocity = Vector2.zero;
                       // 何らかの停止処理
                            GManager.instance.pm.anim.Play($"TCharge{alterNumber + 1}");//チャージアニメ 
                                                                                        //GManager.instance.pm.move.Set(0,0);
                                                                                        // GManager.instance.pm.rb.velocity = GManager.instance.pm.move;
                        }
                    }
        }
        void AirAttack(int state)
        {


            #region//空中弱攻撃
            if (attackNumber != 0 && state == 2 && !isDisEnable)
            {
                // GManager.instance.airAttack = true;
                airAttackPrepare();
                GManager.instance.isAttack = true;

              //smallTrigger = false;
              //GManager.instance.pm.rb.velocity = Vector2.zero;
            //何らかの停止処理
            //いっそ空中で攻撃中（小）の時は停止とかで組み込むか
            //そしたらアニメで動けないのでなしね
                if (!GManager.instance.twinHand)
                {
                    if (attackNumber + 1 == GManager.instance.equipWeapon.airValue.Count)
                    {
                        isDisEnable = true;
                    }
                    GManager.instance.pm.anim.Play($"OAAttack{attackNumber + 1}");
                    // // GManager.instance.StaminaUse(useStamina);
                }
                else
                {
                    //  GManager.instance.airAttack = true;
                    if (attackNumber + 1 == GManager.instance.equipWeapon.twinAirValue.Count)
                    {
                        isDisEnable = true;
                    }
                    GManager.instance.pm.anim.Play($"TAAttack{attackNumber + 1}");
                    // // GManager.instance.StaminaUse(useStamina);
                }
                isAttackable = false;
                attackNumber++;
              //smallTrigger = false;

            }
            #endregion
            //空中強攻撃 
            else if (state == 4 && !GManager.instance.fallAttack)
            {
                // GManager.instance.airAttack = true;
                strikeAttackPrepare();
                GManager.instance.isAttack = true;
              //bigTrigger = false;
                GManager.instance.pm.rb.velocity = Vector2.zero;
                if (!GManager.instance.twinHand)
                {
                    GManager.instance.pm.anim.Play($"OFAttack{attackNumber + 1}");//空中強攻撃
                                                                                  // // GManager.instance.StaminaUse(useStamina);

                }
                else
                {
                    GManager.instance.pm.anim.Play($"TFAttack{alterNumber + 1}");//空中強攻撃
                                                                                 // // GManager.instance.StaminaUse(useStamina);
                }
                isAttackable = false;
                alterNumber++;
            }
        }

        void ArtsAttack()
        {

            #region//特殊行動
            if (artsNumber == 0)
            {
                isAComboEnd = false;
            }
                ArtsPrepare();
                GManager.instance.isAttack = true;
                //  GManager.instance.pm.rb.velocity = Vector2.zero;
                if (!GManager.instance.twinHand && !GManager.instance.equipShield.weaponArts)
                {

                    GManager.instance.pm.anim.Play($"OArts{artsNumber + 1}");
                    // // GManager.instance.StaminaUse(useStamina);//シールドのパリィにする
                    GManager.instance.MpReduce(GManager.instance.equipShield.artsMP[artsNumber]);

                }
                else
                {
                    GManager.instance.pm.anim.Play($"TArts{artsNumber + 1}");
                    // // GManager.instance.StaminaUse(useStamina);
                    GManager.instance.MpReduce(GManager.instance.equipWeapon.artsMP[artsNumber]);
                }
                isAttackable = false;
                artsNumber++;
          //    artsTrigger = false;
                #endregion
            
        }

        void Parry()
        {

            //無敵処理あたり内容書き換える
            if (GManager.instance.parrySuccess && !isParring)
            {
                if (!GManager.instance.blocking)
                {
                    //      Debug.Log("t");
                    if (!GManager.instance.twinHand)
                    {
                        GManager.instance.pm.anim.Play("OParry");
                    }
                    else
                    {
                        GManager.instance.pm.anim.Play("TParry");
                    }
                    GManager.instance.PlaySound("ParrySuccess", transform.position);
                    //  GManager.instance.PlaySound("ParrySuccess2", transform.position);
                }
                else if (GManager.instance.blocking)
                {
                    //   Debug.Log("s");
                    if (!GManager.instance.twinHand)
                    {
                        GManager.instance.pm.anim.Play("OBlock");
                    }
                    else
                    {
                        GManager.instance.pm.anim.Play("TBlock");
                    }
                    GManager.instance.PlaySound("Blocking", transform.position);
                }
                isParring = true;
                GManager.instance.guardDisEnable = true;
                //パリィ
            }
            else if (!GManager.instance.blocking && isParring)
            {
                // Debug.Log("sssssss");
                if (!GManager.instance.twinHand && !CheckEnd("OParry"))
                {
                    Debug.Log("sss");
                    isParring = false;
                    GManager.instance.parrySuccess = false;
                    GManager.instance.pm.SetLayer(11);
                    GManager.instance.guardDisEnable = false;
                    // GManager.instance.isDown = false;
                }
                if (GManager.instance.twinHand && !CheckEnd("TParry"))
                {
                    isParring = false;
                    GManager.instance.parrySuccess = false;
                    GManager.instance.pm.SetLayer(11);
                    GManager.instance.guardDisEnable = false;

                }
            }
            else if (GManager.instance.blocking && isParring)
            {
                if (!GManager.instance.twinHand && !CheckEnd("OBlock"))
                {
                    isParring = false;
                    GManager.instance.parrySuccess = false;
                    GManager.instance.pm.SetLayer(11);
                    GManager.instance.guardDisEnable = false;
                    //GManager.instance.isDown = false;
                }
                if (GManager.instance.twinHand && !CheckEnd("TBlock"))
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

            //ガード中ならガード解除

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
            }
        }

        public void bAttackPrepare()//デフォが斬撃。強攻撃
        {
            GManager.instance.isShieldAttack = false;
            if (alterNumber != 0)
            {
                GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
                transform.localScale = GManager.instance.pm.theScale;
            }
            if (!GManager.instance.twinHand)
            {
                
                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.bValue[alterNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.bValue[alterNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.bValue[alterNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.bValue[alterNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.bValue[alterNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.bValue[alterNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.bValue[alterNumber].blowPower;
               GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.bValue[alterNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.bValue[alterNumber].attackEffect;
            }
            else
            {
                
                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.twinBValue[alterNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.twinBValue[alterNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.twinBValue[alterNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.twinBValue[alterNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.twinBValue[alterNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.twinBValue[alterNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.twinBValue[alterNumber].blowPower;
               GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.twinBValue[alterNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.twinBValue[alterNumber].attackEffect;
            }
        }

        public void chargeAttackPrepare()//デフォが斬撃
        {
            GManager.instance.isShieldAttack = false;
            if (alterNumber != 0)
            {
                GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
                transform.localScale = GManager.instance.pm.theScale;
            }
            if (!GManager.instance.twinHand)
            {
                
                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.chargeValue[alterNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.chargeValue[alterNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.chargeValue[alterNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.chargeValue[alterNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.chargeValue[alterNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.chargeValue[alterNumber].isCombo;
                GManager.instance.equipWeapon.blowPower = GManager.instance.equipWeapon.chargeValue[alterNumber].blowPower;
               GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.chargeValue[alterNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.chargeValue[alterNumber].attackEffect;
            }
            else
            {
                
                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.twinChargeValue[alterNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.twinChargeValue[alterNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.twinChargeValue[alterNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.twinChargeValue[alterNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.twinChargeValue[alterNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.twinChargeValue[alterNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.twinChargeValue[alterNumber].blowPower;
               GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.twinChargeValue[alterNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.twinChargeValue[alterNumber].attackEffect;
            }
        }
        public void airAttackPrepare()//デフォが斬撃
        {
            GManager.instance.airAttack = true;
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
            }
        }
        public void strikeAttackPrepare()//デフォが斬撃
        {
            GManager.instance.airAttack = true;
            GManager.instance.isShieldAttack = false;
            if (alterNumber != 0)
            {
                GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
                transform.localScale = GManager.instance.pm.theScale;
            }
            if (!GManager.instance.twinHand)
            {
                
                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.strikeValue[alterNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.strikeValue[alterNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.strikeValue[alterNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.strikeValue[alterNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.strikeValue[alterNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.strikeValue[alterNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.strikeValue[alterNumber].blowPower;
               GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.strikeValue[alterNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.strikeValue[alterNumber].attackEffect;

            }
            else
            {
                
                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.twinStrikeValue[alterNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.twinStrikeValue[alterNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.twinStrikeValue[alterNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.twinStrikeValue[alterNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.twinStrikeValue[alterNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.twinStrikeValue[alterNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.twinStrikeValue[alterNumber].blowPower;
               GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.twinStrikeValue[alterNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.twinStrikeValue[alterNumber].attackEffect;

            }
        }

        public void ArtsPrepare()//デフォが斬撃
        {

            if (artsNumber != 0)
            {
                GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
                transform.localScale = GManager.instance.pm.theScale;
            }
            if (!GManager.instance.twinHand && !GManager.instance.equipShield.weaponArts)
            {
                
                GManager.instance.useAtValue.type = GManager.instance.equipShield.artsValue[artsNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipShield.artsValue[artsNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipShield.artsValue[artsNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipShield.artsValue[artsNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipShield.artsValue[artsNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipShield.artsValue[artsNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipShield.artsValue[artsNumber].blowPower;
               GManager.instance.useAtValue.useStamina = GManager.instance.equipShield.artsValue[artsNumber].useStamina;
                GManager.instance.isShieldAttack = true;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipShield.artsValue[artsNumber].attackEffect;
            }
            else
            {
                GManager.instance.useAtValue.isShield = true;
                GManager.instance.useAtValue.type = GManager.instance.equipWeapon.artsValue[artsNumber].type;
                GManager.instance.useAtValue.x = GManager.instance.equipWeapon.artsValue[artsNumber].x;
                GManager.instance.useAtValue.y = GManager.instance.equipWeapon.artsValue[artsNumber].y;
                GManager.instance.useAtValue.z = GManager.instance.equipWeapon.artsValue[artsNumber].z;
                GManager.instance.useAtValue.isBlow = GManager.instance.equipWeapon.artsValue[artsNumber].isBlow;
                GManager.instance.useAtValue.isCombo = GManager.instance.equipWeapon.artsValue[artsNumber].isCombo;
                GManager.instance.useAtValue.blowPower = GManager.instance.equipWeapon.artsValue[artsNumber].blowPower;
                GManager.instance.useAtValue.useStamina = GManager.instance.equipWeapon.artsValue[artsNumber].useStamina;
                GManager.instance.useAtValue.attackEffect = GManager.instance.equipWeapon.artsValue[artsNumber].attackEffect;
                GManager.instance.isShieldAttack = false;
            }
        }
        #endregion

    }











}