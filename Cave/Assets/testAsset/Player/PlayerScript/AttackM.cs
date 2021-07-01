using UnityEngine;


public class AttackM : MonoBehaviour
{
   // public GameObject Player;

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
    bool isAttackable;
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
    int useStamina;

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
    void Start()
    {

      // GManager.instance.pm = Player.GetComponent<PlayerMove>();
      //  anim = GetComponent<Animator>();
      //  GManager.instance.pm.rb = GetComponent<Rigidbody2D>();
       // GManager.instance.SetAtk();
    }

    // Update is called once per frame

    private void Update()
    {
        //    //////Debug.log($"攻撃可能？{isAttackable}");
        //   //////Debug.log($"攻撃中？{GManager.instance.isAttack}");

        //  DownKeyCheck();
        //     ControllerCheck();
     //   ////Debug.Log($"状態は{GManager.instance.pStatus.equipWeapon.twinHand}です");
        horizontalKey = GManager.instance.InputR.GetAxisRaw(MainUI.instance.rewiredAction0);

        changeKey = GManager.instance.InputR.GetButtonUp(MainUI.instance.rewiredAction13);
        if (!GManager.instance.InputR.GetButton(MainUI.instance.rewiredAction13) && GManager.instance.isEnable && !smallTrigger && !(GManager.instance.isAttack && !isAttackable))
        {
            fire1Key = GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction9);
            //lastAttack = true;
        }
        else
        {
            fire1Key = false;
        }
        if(GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction9) && GManager.instance.InputR.GetButton(MainUI.instance.rewiredAction13) && !GManager.instance.isAttack)
        {
            //武器切り替え

            //さらに初期は必ず片手持ちに
            GManager.instance.pStatus.equipWeapon.twinHand = false;
            equipChange = true;
            //武器切り替え後は一回だけボタン離しても持ち手変更が反応しないようにする
        }
        else if (changeKey && !equipChange)
        {
            GManager.instance.pStatus.equipWeapon.twinHand = !GManager.instance.pStatus.equipWeapon.twinHand;
        }
        else if(equipChange && changeKey)
        {
            equipChange = false;
        }

        if (GManager.instance.isEnable && !(GManager.instance.isAttack && !isAttackable))
        {
            if (!bigTrigger)
            {

                fire2Key = GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction10);

                //fire2Axis = GManager.instance.InputR.GetAxisRaw("Fire2Axis");
            }
            else
            {
                fire2Key = false;
            }

            if (!artsTrigger)
            {
                artsKey = GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction12);
               // ////Debug.Log("入力");
            }
            else
            {
                artsKey = false;

            }
        }
        else
        {
            fire2Key = false;
            artsKey = false;
        }
        anyKey = AnyKey();




        if (isCharging && !bigAttack && !chargeAttack)
        {
            chargeKey = GManager.instance.InputR.GetButton((MainUI.instance.rewiredAction10));////////Debug.log("入力");
        }
        else
        {
            chargeKey = false;
            //攻撃中は溜められない

        }
        if (chargeKey && !chargeAttack && !bigAttack)
        {
            chargeTime += Time.deltaTime;
            //チャージ中
            if (chargeTime >= GManager.instance.pStatus.equipWeapon.chargeRes)
            {
                // isCharging = false;
                chargeTime = 0.0f;
                chargeAttack = true;
            }
        }
        else if (chargeTime < GManager.instance.pStatus.equipWeapon.chargeRes && !chargeKey && isCharging && !bigAttack && !chargeAttack)
        {
             chargeTime = 0.0f;
            //isCharging = false;
            bigAttack = true;

        }

    }




    private void FixedUpdate()
    {






        /*    if (!GManager.instance.isEnable)
            {
                attackNumber = 0;
                alterNumber = 0;
                artsNumber = 0;
                smallTrigger = false;
                    bigTrigger = false;
                artsTrigger = false;
                //lastAttack = true;
                //GManager.instance.isAttack = false;
            }*/

        Parry();
        ///<summary>
        ///攻撃中停止
        /// </summary>
        #region
        /*if (GManager.instance.pm.isGround && isAttackable && GManager.instance.isAttack)
        {
            
            GManager.instance.pm.rb.velocity = Vector2.zero;
        }*/
        #endregion
        ///<summary>
        ///モーション番号のリセット
        /// </summary>
#region
        /*
        if (GManager.instance.pm.isGround)
        {
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {

                if (attackNumber >= GManager.instance.pStatus.equipWeapon.sValue.Count)
                {
                    attackNumber = 0;//モーション番号のリセット
                    isSComboEnd = true;
                }
                else if (alterNumber >= GManager.instance.pStatus.equipWeapon.bValue.Count)
                {
                    alterNumber = 0;//モーション番号のリセット
                    isBComboEnd = true;
                }
                else if (GManager.instance.pStatus.equipShield.weponArts)
                {
                    if (artsNumber >= GManager.instance.pStatus.equipWeapon.artsValue.Count)
                    {
                        artsNumber = 0;//モーション番号のリセット
                        isAComboEnd = true;
                    }
                }
                else
                {
                    if (artsNumber >= GManager.instance.pStatus.equipShield.artsValue.Count)
                    {
                        artsNumber = 0;//モーション番号のリセット
                        isAComboEnd = true;
                    }
                }
            }
            else
            {
                if (attackNumber >= GManager.instance.pStatus.equipWeapon.twinSValue.Count)
                {
                    attackNumber = 0;//モーション番号のリセット
                    isSComboEnd = true;
                }
                else if (alterNumber >= GManager.instance.pStatus.equipWeapon.twinBValue.Count)
                {
                    alterNumber = 0;//モーション番号のリセット
                    isBComboEnd = true;
                }

               else if (artsNumber >= GManager.instance.pStatus.equipWeapon.artsValue.Count)
                {
                    artsNumber = 0;//モーション番号のリセット
                    isAComboEnd = true;
                }

            }
        }
        else
        {
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {

                if (attackNumber >= GManager.instance.pStatus.equipWeapon.airValue.Count)
                {
                    attackNumber = 0;//モーション番号のリセット
                    isDisEnable = true;
                    isSComboEnd = true;
                }
                if (alterNumber >= GManager.instance.pStatus.equipWeapon.strikeValue.Count)
                {
                    alterNumber = 0;//モーション番号のリセット
                    GManager.instance.fallAttack = true;
                    isDisEnable = true;
                }

            }
            else
            {
                if (attackNumber >= GManager.instance.pStatus.equipWeapon.twinAirValue.Count)
                {
                    attackNumber = 0;//モーション番号のリセット
                    isDisEnable = true;
                }
                if (alterNumber >= GManager.instance.pStatus.equipWeapon.twinStrikeValue.Count)
                {
                    alterNumber = 0;//モーション番号のリセット
                    GManager.instance.fallAttack = true;
                    isDisEnable = true;
                }

            }

        }*/
        #endregion
        if (GManager.instance.pm.isEnAt && !GManager.instance.isAttack && GManager.instance.isEnable && !GManager.instance.parrySuccess)
        {
            //攻撃可能で弱攻撃ボタン押されてて攻撃してなくてスタミナが使えるなら以下の処理
            //delayTime = 0.0f;
            if (GManager.instance.pm.isGround)
            {
                isDisEnable = false;
                NormalAttack();
                ArtsAttack();
                #region//ため攻撃
                if (fire2Key || bigTrigger)
                {

                    isCharging = true;
                    isAttackable = false;
                    GManager.instance.isAttack = true;
                  //  GManager.instance.pm.rb.velocity = Vector2.zero;
                    bigTrigger = false;

                    #endregion
                }
            }
            else //if (GManager.instance.pm.isEnAt && !GManager.instance.isAttack && GManager.instance.isEnable)
            {
                AirAttack();
            }
            // lastAttack = true;

        }

        if (GManager.instance.isAttack && isCharging)
        {
            if (!GManager.instance.pStatus.equipWeapon.isMagic)
            {
                ChargeAttack();
            }
            else
            {
                //ため時間を発射保留と照準に回した射撃メソッドを

            }
        }

        NumberControll();

        #region//連撃入力とキャンセル待ち
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
        }


        #endregion
        #region//地上モーション終了検査
        if (!GManager.instance.airAttack)
        {
            if (attackNumber >= 1 && !isSComboEnd)
            {
                if (!GManager.instance.pStatus.equipWeapon.twinHand)
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
                if (!GManager.instance.pStatus.equipWeapon.twinHand)
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
                if (!GManager.instance.pStatus.equipWeapon.twinHand)
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
            else if(artsNumber >= 1 && !isAComboEnd)
            {
               if (!GManager.instance.pStatus.equipWeapon.twinHand && !GManager.instance.pStatus.equipShield.weponArts)
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
                if (!GManager.instance.pStatus.equipWeapon.twinHand)
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
                if (!GManager.instance.pStatus.equipWeapon.twinHand)
                {
                    if (CheckEnd($"OSAttack{GManager.instance.pStatus.equipWeapon.sValue.Count}") == false)
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
                    if (CheckEnd($"TSAttack{GManager.instance.pStatus.equipWeapon.twinSValue.Count}") == false)
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
           else if(isBComboEnd && cAttack)
            {
                if (!GManager.instance.pStatus.equipWeapon.twinHand)
                {
                    if (CheckEnd($"OCAttack{GManager.instance.pStatus.equipWeapon.chargeValue.Count}") == false)
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
                    if (CheckEnd($"TCAttack{GManager.instance.pStatus.equipWeapon.twinChargeValue.Count}") == false)
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
                if (!GManager.instance.pStatus.equipWeapon.twinHand)
                {
                    if (CheckEnd($"OBAttack{GManager.instance.pStatus.equipWeapon.bValue.Count}") == false)
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
                    if (CheckEnd($"TBAttack{GManager.instance.pStatus.equipWeapon.twinBValue.Count}") == false)
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
                if (!GManager.instance.pStatus.equipWeapon.twinHand && !GManager.instance.pStatus.equipShield.weponArts)
                {
                    if (CheckEnd($"OArts{GManager.instance.pStatus.equipShield.artsValue.Count}") == false)
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
                    if (CheckEnd($"TArts{GManager.instance.pStatus.equipWeapon.artsValue.Count}") == false)
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
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
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
                                                attackNumber =GManager.instance.pm.isGround ? 0 : attackNumber;
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
                                           attackNumber =GManager.instance.pm.isGround ? 0 : attackNumber;
                                           //isGroundの時attackNumberを0、違うならそのまま
                                       }*/
                }
                else if (isDisEnable)
                {
                    if (CheckEnd($"OAAttack{GManager.instance.pStatus.equipWeapon.airValue.Count}") == false)
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
                    /*     if (CheckEnd($"OFAttack{GManager.instance.pStatus.equipWeapon.strikeValue.Count}") == false)
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
                            attackNumber =GManager.instance.pm.isGround ? 0 : attackNumber;
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
                                           attackNumber =GManager.instance.pm.isGround ? 0 : attackNumber;
                                           //isGroundの時attackNumberを0、違うならそのまま
                                       }*/
                }
                else if (isDisEnable)
                    {
                        if (CheckEnd($"TAAttack{GManager.instance.pStatus.equipWeapon.twinAirValue.Count}") == false)
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
                  /*      if (CheckEnd($"TFAttack{GManager.instance.pStatus.equipWeapon.twinStrikeValue.Count}") == false)
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
            if (GManager.instance.pm.isGround)
            { 

                if (GManager.instance.pStatus.equipWeapon.twinHand)
                {
                    GManager.instance.pm.anim.Play("TLanding");
                }
                else
                {
                    GManager.instance.pm.anim.Play("OLanding");
                }
                groundTime += Time.fixedDeltaTime;
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
            if (GManager.instance.isAttack && !isAttackable && (!GManager.instance.pm.isGround || GManager.instance.pm.isSloopDown))
            {
                GManager.instance.pm.move.Set(0, -GManager.instance.pm.gravity);
                GManager.instance.pm.rb.velocity = GManager.instance.pm.move;
            }
            else if (GManager.instance.isAttack && isAttackable && GManager.instance.pm.isGround)
            {
                GManager.instance.pm.rb.velocity = Vector2.zero;
            }
            else if(GManager.instance.isAttack && isAttackable && !GManager.instance.pm.isGround)
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

        if (isAttackable)//攻撃後の方向転換
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
    ///<Summally>
    ///アニメイベントで呼ぶやつ？
    ///</Summally>
    public void Continue()
    {
       // GManager.instance.pm.anim.Play("OArts1");

        ////Debug.Log("攻撃可能に");
        
        GManager.instance.StaminaUse(useStamina);

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
        if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction4) || GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction11)
            || GManager.instance.InputR.GetAxisRaw(MainUI.instance.rewiredAction1) != 0 || GManager.instance.InputR.GetButton(MainUI.instance.rewiredAction13))
        {

            return true;
        }
        else if(GManager.instance.InputR.GetAxisRaw(MainUI.instance.rewiredAction2) != 0 && GManager.instance.isAttack && isAttackable)
        {
            afterTime += Time.deltaTime;
            if(afterTime >= afterJudge)
            {
                afterTime = 0;
                return true;
            }
            return false;
        }
        else
        {
            afterTime = 0;
            return false;
        }
    }
    void NormalAttack()
    {
        #region//通常攻撃
        if (attackNumber == 0 && (fire1Key || smallTrigger))
        {

            isSComboEnd = false;
            GManager.instance.isAttack = true;
           // GManager.instance.pm.rb.velocity = Vector2.zero;
            sAttackPrepare();
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
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
        }
        else if (attackNumber != 0 && (GManager.instance.pStatus.equipWeapon.isCombo || (fire1Key || smallTrigger)))
        {
            sAttackPrepare();
            GManager.instance.isAttack = true;
          //  GManager.instance.pm.rb.velocity = Vector2.zero;
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {

                GManager.instance.pm.anim.Play($"OSAttack{attackNumber + 1}");
        // // GManager.instance.StaminaUse(useStamina);

            }
            else
            {
                GManager.instance.pm.anim.Play($"TSAttack{attackNumber + 1}");
               // // GManager.instance.StaminaUse(useStamina);

            }
            isAttackable = false;
            attackNumber++;
            smallTrigger = false;
        }
        #endregion
    }
    void ChargeAttack()
    {
        if (!GManager.instance.pStatus.equipWeapon.isMagic)
        {
            if (alterNumber == 0)
            {
                

                if (chargeAttack)
                {
                    isBComboEnd = false;
                    chargeAttackPrepare();
                    if (!GManager.instance.pStatus.equipWeapon.twinHand)
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
                    isBComboEnd = false;
                    bAttackPrepare();
                    if (!GManager.instance.pStatus.equipWeapon.twinHand)
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
                    if (!GManager.instance.pStatus.equipWeapon.twinHand)
                    {
                        GManager.instance.pm.anim.Play($"OCharge{alterNumber + 1}");//チャージアニメ    
                         GManager.instance.pm.rb.velocity = Vector2.zero;
                      //  GManager.instance.pm.move.Set(0,0);
                       // GManager.instance.pm.rb.velocity = GManager.instance.pm.move;
                    }
                    else
                    {
                        GManager.instance.pm.rb.velocity = Vector2.zero;
                        GManager.instance.pm.anim.Play($"TCharge{alterNumber + 1}");//チャージアニメ 
                        //GManager.instance.pm.move.Set(0,0);
                       // GManager.instance.pm.rb.velocity = GManager.instance.pm.move;
                    }
                }
            }
            else if (alterNumber != 0 || GManager.instance.pStatus.equipWeapon.isCombo)
            {

                if (chargeAttack)
                {
                    chargeAttackPrepare();
                    if (!GManager.instance.pStatus.equipWeapon.twinHand)
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
                //もし途中で手を離したら普通に外に出る、2Stayが否定されるから
                else if (bigAttack)
                {
                    bAttackPrepare();
                    if (!GManager.instance.pStatus.equipWeapon.twinHand)
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
                    bigAttack = false;
                    isCharging = false;
                    chargeTime = 0.0f;
                }
                else
                {
                    if (!GManager.instance.pStatus.equipWeapon.twinHand)
                    {
                        GManager.instance.pm.anim.Play($"OCharge{alterNumber + 1}");//チャージアニメ     
                    }
                    else
                    {
                        GManager.instance.pm.anim.Play($"TCharge{alterNumber + 1}");//チャージアニメ 
                    }
                }
            }
        }
        else
        {
            //魔法の処理
            //チャージ時間が魔法発射保留できる時間
            //詠唱時間短めでコンボに混ぜれるようなの多めにする？
        }
    }
    void AirAttack()
    {


        #region//空中弱攻撃
        if (attackNumber == 0 && (fire1Key || smallTrigger) && !isDisEnable && !bigTrigger)
        {
           // GManager.instance.airAttack = true;
            airAttackPrepare();
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {
                GManager.instance.pm.anim.Play("OAAttack1");//空中弱攻撃
        // // GManager.instance.StaminaUse(useStamina);
            }
            else
            {
                GManager.instance.pm.anim.Play("TAAttack1");//空中弱攻撃
        // // GManager.instance.StaminaUse(useStamina);
            }
            GManager.instance.isAttack = true;
            GManager.instance.pm.rb.velocity = Vector2.zero;
            //コンボにする仕様上軽めの攻撃に
            isAttackable = false;
            attackNumber++;
            smallTrigger = false;
        }
        else if (attackNumber != 0 && (fire1Key || smallTrigger || GManager.instance.pStatus.equipWeapon.isCombo) && !isDisEnable && !bigTrigger )
        {
           // GManager.instance.airAttack = true;
            airAttackPrepare();
            GManager.instance.isAttack = true;

            smallTrigger = false;
            GManager.instance.pm.rb.velocity = Vector2.zero;
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {
                if (attackNumber + 1 == GManager.instance.pStatus.equipWeapon.airValue.Count)
                {
                    isDisEnable = true;
                }
                GManager.instance.pm.anim.Play($"OAAttack{attackNumber + 1}");
        // // GManager.instance.StaminaUse(useStamina);
            }
            else
            {
              //  GManager.instance.airAttack = true;
                if (attackNumber + 1 == GManager.instance.pStatus.equipWeapon.twinAirValue.Count)
                {
                    isDisEnable = true;
                }
                GManager.instance.pm.anim.Play($"TAAttack{attackNumber + 1}");
        // // GManager.instance.StaminaUse(useStamina);
            }
            isAttackable = false;
            attackNumber++;
            smallTrigger = false;

        }
        #endregion
        //空中強攻撃
        else if ((bigTrigger || fire2Key || GManager.instance.pStatus.equipWeapon.isCombo) && !GManager.instance.fallAttack)
        {
           // GManager.instance.airAttack = true;
            strikeAttackPrepare();
            GManager.instance.isAttack = true;
            bigTrigger = false;
            GManager.instance.pm.rb.velocity = Vector2.zero;
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
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
            alterNumber ++;
        }
    }

    void ArtsAttack()
    {

        #region//特殊行動
        if (artsNumber == 0 && (artsKey || artsTrigger))
        {

            ArtsPrepare();
            GManager.instance.isAttack = true;
            //GManager.instance.pm.rb.velocity = Vector2.zero;
            ////Debug.Log("正常攻撃");

            if (!GManager.instance.pStatus.equipWeapon.twinHand && !GManager.instance.pStatus.equipShield.weponArts)
            {
                GManager.instance.pm.anim.Play("OArts1");
                ////Debug.Log($"アニメ確認{isAttackable}");
                GManager.instance.MpReduce(GManager.instance.pStatus.equipShield.artsMP[artsNumber]);
            }
            else
            {
                GManager.instance.pm.anim.Play("TArts1");

                GManager.instance.MpReduce(GManager.instance.pStatus.equipWeapon.artsMP[artsNumber]);

            }
            AnimatorClipInfo[] clipInfo = GManager.instance.pm.anim.GetCurrentAnimatorClipInfo(0);
          //  ////Debug.Log($"アニメ確認{isAComboEnd}{artsNumber}{GManager.instance.isAttack}");
        //{clipInfo[0].clip.name}");
            isAttackable = false;

            artsNumber++;
            artsTrigger = false;
        }
        else if (artsNumber != 0 && (artsKey || artsTrigger) || GManager.instance.pStatus.equipWeapon.isCombo)
        {

            ArtsPrepare();
            GManager.instance.isAttack = true;
          //  GManager.instance.pm.rb.velocity = Vector2.zero;
            if (!GManager.instance.pStatus.equipWeapon.twinHand && !GManager.instance.pStatus.equipShield.weponArts)
            {
               
                GManager.instance.pm.anim.Play($"OArts{artsNumber + 1}");
        // // GManager.instance.StaminaUse(useStamina);//シールドのパリィにする
                GManager.instance.MpReduce(GManager.instance.pStatus.equipShield.artsMP[artsNumber]);

            }
            else
            {
                GManager.instance.pm.anim.Play($"TArts{artsNumber + 1}");
        // // GManager.instance.StaminaUse(useStamina);
                GManager.instance.MpReduce(GManager.instance.pStatus.equipWeapon.artsMP[artsNumber]);
            }
            isAttackable = false;
            artsNumber++;
            artsTrigger = false;
            #endregion
        }
    }

    void GroundCheck()
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
        else if (fire1Key)
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
        else if (fire2Key)
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
        else if (artsKey)
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
    void AirCheck()
    {
            if (anyKey || GManager.instance.pm.isGround)
            {
                GManager.instance.isAttack = false;

                attackNumber = 0;
                alterNumber = 0;
                GManager.instance.airAttack = false;
            }
            else if (fire1Key)
            {
                GManager.instance.isAttack = false;
                smallTrigger = true;
            bigTrigger = false;
                alterNumber = 0;
                GManager.instance.airAttack = false;
            }
            else if (fire2Key)
            {
                GManager.instance.isAttack = false;
                bigTrigger = true;
            smallTrigger = false;
                attackNumber = 0;
                GManager.instance.airAttack = false;
            }
            
    }




    //攻撃するときに呼ぶ
    public void sAttackPrepare()//デフォが斬撃
    {
        if (attackNumber != 0)
        {
            
            GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
            transform.localScale = GManager.instance.pm.theScale;
        }
        if (!GManager.instance.pStatus.equipWeapon.twinHand)
        {
            GManager.instance.isGuard = false;
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipWeapon.sValue[attackNumber].type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.sValue[attackNumber].x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.sValue[attackNumber].y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipWeapon.sValue[attackNumber].z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipWeapon.sValue[attackNumber].isBlow;
          GManager.instance.pStatus.equipWeapon.isCombo = GManager.instance.pStatus.equipWeapon.sValue[attackNumber].isCombo;
            GManager.instance.pStatus.equipWeapon.blowPower = GManager.instance.pStatus.equipWeapon.sValue[attackNumber].blowPower;
            useStamina = GManager.instance.pStatus.equipWeapon.sValue[attackNumber].useStamina;
        }
        else
        {
            GManager.instance.isGuard = false;
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipWeapon.twinSValue[attackNumber].type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.twinSValue[attackNumber].x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.twinSValue[attackNumber].y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipWeapon.twinSValue[attackNumber].z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipWeapon.twinSValue[attackNumber].isBlow;
            GManager.instance.pStatus.equipWeapon.isCombo = GManager.instance.pStatus.equipWeapon.twinSValue[attackNumber].isCombo;
            GManager.instance.pStatus.equipWeapon.blowPower = GManager.instance.pStatus.equipWeapon.twinSValue[attackNumber].blowPower;
            useStamina = GManager.instance.pStatus.equipWeapon.twinSValue[attackNumber].useStamina;
        }
    }

    public void bAttackPrepare()//デフォが斬撃。強攻撃
    {
        if (alterNumber != 0)
        {
            GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
            transform.localScale = GManager.instance.pm.theScale;
        }
        if (!GManager.instance.pStatus.equipWeapon.twinHand)
        {
            GManager.instance.isGuard = false;
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipWeapon.bValue[alterNumber].type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.bValue[alterNumber].x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.bValue[alterNumber].y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipWeapon.bValue[alterNumber].z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipWeapon.bValue[alterNumber].isBlow;
            GManager.instance.pStatus.equipWeapon.isCombo = GManager.instance.pStatus.equipWeapon.bValue[alterNumber].isCombo;
            GManager.instance.pStatus.equipWeapon.blowPower = GManager.instance.pStatus.equipWeapon.bValue[alterNumber].blowPower;
            useStamina = GManager.instance.pStatus.equipWeapon.bValue[alterNumber].useStamina;
        }
        else
        {
            GManager.instance.isGuard = false;
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipWeapon.twinBValue[alterNumber].type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.twinBValue[alterNumber].x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.twinBValue[alterNumber].y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipWeapon.twinBValue[alterNumber].z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipWeapon.twinBValue[alterNumber].isBlow;
            GManager.instance.pStatus.equipWeapon.isCombo = GManager.instance.pStatus.equipWeapon.twinBValue[alterNumber].isCombo;
            GManager.instance.pStatus.equipWeapon.blowPower = GManager.instance.pStatus.equipWeapon.twinBValue[alterNumber].blowPower;
            useStamina = GManager.instance.pStatus.equipWeapon.twinBValue[alterNumber].useStamina;
        }
    }

    public void chargeAttackPrepare()//デフォが斬撃
    {
        if (alterNumber != 0)
        {
            GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
            transform.localScale = GManager.instance.pm.theScale;
        }
        if (!GManager.instance.pStatus.equipWeapon.twinHand)
        {
            GManager.instance.isGuard = false;
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipWeapon.chargeValue[alterNumber].type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.chargeValue[alterNumber].x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.chargeValue[alterNumber].y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipWeapon.chargeValue[alterNumber].z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipWeapon.chargeValue[alterNumber].isBlow;
            GManager.instance.pStatus.equipWeapon.isCombo = GManager.instance.pStatus.equipWeapon.chargeValue[alterNumber].isCombo;
            GManager.instance.pStatus.equipWeapon.blowPower = GManager.instance.pStatus.equipWeapon.chargeValue[alterNumber].blowPower;
            useStamina = GManager.instance.pStatus.equipWeapon.chargeValue[alterNumber].useStamina;
        }
        else
        {
            GManager.instance.isGuard = false;
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipWeapon.twinChargeValue[alterNumber].type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.twinChargeValue[alterNumber].x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.twinChargeValue[alterNumber].y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipWeapon.twinChargeValue[alterNumber].z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipWeapon.twinChargeValue[alterNumber].isBlow;
           GManager.instance.pStatus.equipWeapon.isCombo = GManager.instance.pStatus.equipWeapon.twinChargeValue[alterNumber].isCombo;
            GManager.instance.pStatus.equipWeapon.blowPower = GManager.instance.pStatus.equipWeapon.twinChargeValue[alterNumber].blowPower;
            useStamina = GManager.instance.pStatus.equipWeapon.twinChargeValue[alterNumber].useStamina;
        }
    }
    public void airAttackPrepare()//デフォが斬撃
    {
        GManager.instance.airAttack = true;

        if (attackNumber != 0)
        {
            GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
            transform.localScale = GManager.instance.pm.theScale;
        }
        if (!GManager.instance.pStatus.equipWeapon.twinHand)
        {
            GManager.instance.isGuard = false;
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipWeapon.airValue[attackNumber].type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.airValue[attackNumber].x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.airValue[attackNumber].y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipWeapon.airValue[attackNumber].z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipWeapon.airValue[attackNumber].isBlow;
            GManager.instance.pStatus.equipWeapon.isCombo = GManager.instance.pStatus.equipWeapon.airValue[attackNumber].isCombo;
            GManager.instance.pStatus.equipWeapon.blowPower = GManager.instance.pStatus.equipWeapon.airValue[attackNumber].blowPower;
            useStamina = GManager.instance.pStatus.equipWeapon.airValue[attackNumber].useStamina;
        }
        else
        {
            GManager.instance.isGuard = false;
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipWeapon.twinAirValue[attackNumber].type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.twinAirValue[attackNumber].x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.twinAirValue[attackNumber].y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipWeapon.twinAirValue[attackNumber].z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipWeapon.twinAirValue[attackNumber].isBlow;
            GManager.instance.pStatus.equipWeapon.isCombo = GManager.instance.pStatus.equipWeapon.twinAirValue[attackNumber].isCombo;
            GManager.instance.pStatus.equipWeapon.blowPower = GManager.instance.pStatus.equipWeapon.twinAirValue[attackNumber].blowPower;
            useStamina = GManager.instance.pStatus.equipWeapon.twinAirValue[attackNumber].useStamina;
        }
    }
    public void strikeAttackPrepare()//デフォが斬撃
    {
        GManager.instance.airAttack = true;

        if (alterNumber != 0)
        {
            GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
            transform.localScale = GManager.instance.pm.theScale;
        }
        if (!GManager.instance.pStatus.equipWeapon.twinHand)
        {
            GManager.instance.isGuard = false;
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipWeapon.strikeValue[alterNumber].type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.strikeValue[alterNumber].x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.strikeValue[alterNumber].y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipWeapon.strikeValue[alterNumber].z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipWeapon.strikeValue[alterNumber].isBlow;
            GManager.instance.pStatus.equipWeapon.isCombo = GManager.instance.pStatus.equipWeapon.strikeValue[alterNumber].isCombo;
            GManager.instance.pStatus.equipWeapon.blowPower = GManager.instance.pStatus.equipWeapon.strikeValue[alterNumber].blowPower;
            useStamina = GManager.instance.pStatus.equipWeapon.strikeValue[alterNumber].useStamina;
        }
        else
        {
            GManager.instance.isGuard = false;
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipWeapon.twinStrikeValue[alterNumber].type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.twinStrikeValue[alterNumber].x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.twinStrikeValue[alterNumber].y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipWeapon.twinStrikeValue[alterNumber].z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipWeapon.twinStrikeValue[alterNumber].isBlow;
            GManager.instance.pStatus.equipWeapon.isCombo = GManager.instance.pStatus.equipWeapon.twinStrikeValue[alterNumber].isCombo;
            GManager.instance.pStatus.equipWeapon.blowPower = GManager.instance.pStatus.equipWeapon.twinStrikeValue[alterNumber].blowPower;
            useStamina = GManager.instance.pStatus.equipWeapon.twinStrikeValue[alterNumber].useStamina;
        }
    }

    public void ArtsPrepare()//デフォが斬撃
    {

        if (artsNumber != 0)
        {
            GManager.instance.pm.theScale.Set(attackDirection, transform.localScale.y, transform.localScale.z);
            transform.localScale = GManager.instance.pm.theScale;
        }
        if (!GManager.instance.pStatus.equipWeapon.twinHand && !GManager.instance.pStatus.equipShield.weponArts)
        {
            GManager.instance.isGuard = false;
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipShield.artsValue[artsNumber].type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipShield.artsValue[artsNumber].x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipShield.artsValue[artsNumber].y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipShield.artsValue[artsNumber].z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipShield.artsValue[artsNumber].isBlow;
            GManager.instance.pStatus.equipWeapon.isCombo = GManager.instance.pStatus.equipShield.artsValue[artsNumber].isCombo;
            GManager.instance.pStatus.equipWeapon.blowPower = GManager.instance.pStatus.equipShield.artsValue[artsNumber].blowPower;
            useStamina = GManager.instance.pStatus.equipShield.artsValue[artsNumber].useStamina;
        }
        else
        {
            GManager.instance.isGuard = false;
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipWeapon.artsValue[artsNumber].type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.artsValue[artsNumber].x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.artsValue[artsNumber].y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipWeapon.artsValue[artsNumber].z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipWeapon.artsValue[artsNumber].isBlow;
            GManager.instance.pStatus.equipWeapon.isCombo = GManager.instance.pStatus.equipWeapon.artsValue[artsNumber].isCombo;
            GManager.instance.pStatus.equipWeapon.blowPower = GManager.instance.pStatus.equipWeapon.artsValue[artsNumber].blowPower;
            useStamina = GManager.instance.pStatus.equipWeapon.artsValue[artsNumber].useStamina;
        }
    }
    ///<summary>
    ///モーション番号のリセット
    /// </summary>
    public void NumberControll()
    {

        #region
        if (!GManager.instance.airAttack)
        {
            if (!isAComboEnd && !isBComboEnd && !isSComboEnd)
            {
                if (!GManager.instance.pStatus.equipWeapon.twinHand)
                {

                    if (attackNumber >= GManager.instance.pStatus.equipWeapon.sValue.Count)
                    {
                        attackNumber = 0;//モーション番号のリセット
                        isSComboEnd = true;
                    }
                    else if (alterNumber >= GManager.instance.pStatus.equipWeapon.bValue.Count)
                    {
                        alterNumber = 0;//モーション番号のリセット
                        isBComboEnd = true;
                    }

                    else if (GManager.instance.pStatus.equipShield.weponArts)
                    {
                        if (artsNumber >= GManager.instance.pStatus.equipWeapon.artsValue.Count)
                        {
                            artsNumber = 0;//モーション番号のリセット
                                           //      if(GManager.instance.pStatus.equipWeapon.artsValue.Count > 1)
                                           //         {
                            isAComboEnd = true;
                            // }
                        }
                    }
                    else
                    {
                        if (artsNumber >= GManager.instance.pStatus.equipShield.artsValue.Count)
                        {
                            artsNumber = 0;//モーション番号のリセット
                                           //  if (GManager.instance.pStatus.equipShield.artsValue.Count > 1)
                                           //     {
                            isAComboEnd = true;
                            //     }
                            ////Debug.Log($"コンボ終了");
                        }
                    }
                }
                else
                {
                    if (attackNumber >= GManager.instance.pStatus.equipWeapon.twinSValue.Count)
                    {
                        attackNumber = 0;//モーション番号のリセット
                        isSComboEnd = true;
                    }
                    else if (alterNumber >= GManager.instance.pStatus.equipWeapon.twinBValue.Count)
                    {
                        alterNumber = 0;//モーション番号のリセット
                        isBComboEnd = true;
                    }

                    else if (artsNumber >= GManager.instance.pStatus.equipWeapon.artsValue.Count)
                    {
                        artsNumber = 0;//モーション番号のリセット
                                       // if (GManager.instance.pStatus.equipWeapon.artsValue.Count > 1)
                                       //   {
                        isAComboEnd = true;
                        //  }
                    }

                }
            }
        }
        else
        {
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {

                if (attackNumber >= GManager.instance.pStatus.equipWeapon.airValue.Count)
                {
                    attackNumber = 0;//モーション番号のリセット
                    isDisEnable = true;
                    isSComboEnd = true;
                }
                if (alterNumber >= GManager.instance.pStatus.equipWeapon.strikeValue.Count)
                {
                    alterNumber = 0;//モーション番号のリセット
                   GManager.instance.fallAttack = true;
                    //isDisEnable = true;
                }

            }
            else
            {
                if (attackNumber >= GManager.instance.pStatus.equipWeapon.twinAirValue.Count)
                {
                    attackNumber = 0;//モーション番号のリセット
                //    isDisEnable = true;
                }
                if (alterNumber >= GManager.instance.pStatus.equipWeapon.twinStrikeValue.Count)
                {
                    alterNumber = 0;//モーション番号のリセット
                    GManager.instance.fallAttack = true;
                   // isDisEnable = true;
                }

            }

        }
        #endregion
    }

    public void PlayerArmor()
    {
        GManager.instance.isArmor = true;

    }//判定出す直前にアニメイベントで呼び出す。
    /*  void DownKeyCheck()
 {
      if (GManager.instance.InputR.anyKeyDown)
           {
               foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
               {
                   if (GManager.instance.InputR.GetKeyDown(code))
                   {
                       //処理を書く
                       //////Debug.log($"入力されたのは{code}");
                       break;
                   }
               }
           }
       }*/

    /*   void ControllerCheck()
       {
           //ゲームパッドが繋がれてるか確認
           var controllerNames = GManager.instance.InputR.GetJoystickNames();

           //////Debug.log($"ゲームパッドが接続されてる{padConnect}");
           if (controllerNames != null)
           {
         //      //////Debug.log($"ゲームパッドName{controllerNames[0]}");
           }
           // 一台もコントローラが接続されていなければエラー
           if (controllerNames[0] == "")
           {
               padConnect = false;
           }
           else
           {
               padConnect = true;
           }
       }*/
    void Parry()
    {
        if (GManager.instance.parrySuccess && !isParring)
        {
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {
                GManager.instance.pm.anim.Play("OParry");
            }
            else
            {
                GManager.instance.pm.anim.Play("TParry");
            }

            isParring = true;
            GManager.instance.guardDisEnable = true;
            //パリィ
        }
        else if (GManager.instance.parrySuccess && isParring)
        {
            if (!GManager.instance.pStatus.equipWeapon.twinHand && !CheckEnd("OParry"))
            {
                isParring = false;
                GManager.instance.parrySuccess = false;
                GManager.instance.pm.SetLayer(11);
                GManager.instance.guardDisEnable = false;
            }
            if(GManager.instance.pStatus.equipWeapon.twinHand && !CheckEnd("TParry"))
            {
                isParring = false;
                GManager.instance.parrySuccess = false;
                GManager.instance.pm.SetLayer(11);
                GManager.instance.guardDisEnable = false;
            }
        }

    }
}


