using UnityEngine;


public class AttackM : MonoBehaviour
{
    public GameObject Player;

    //------------------------------------------内部パラメータ

    PlayerMove pm;
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
    float gravity;//重力を入れる

    bool isDisEnable;//空中弱攻撃を二回までに制限
    bool fallAttack;//空中強攻撃の落下終了までisAttackをキープするためのフラグ


    float delayTime;
    int attackNumber;
    int alterNumber;
    int artsNumber;
    Animator anim;
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

    ///<summary>
    ///攻撃の発生保障
    /// </summary>
    bool lastAttack;
    float testtime;


    /// <summary>
    /// 横攻撃後に方向転換か移動か判断する
    /// </summary>
    float afterTime;

    public float afterJudge = 0.35f;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {

        pm = Player.GetComponent<PlayerMove>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
       // GManager.instance.SetAtk();
    }

    // Update is called once per frame

    private void Update()
    {
        //    //Debug.log($"攻撃可能？{isAttackable}");
        //   //Debug.log($"攻撃中？{GManager.instance.isAttack}");

        //  DownKeyCheck();
        //     ControllerCheck();
     //   Debug.Log($"状態は{GManager.instance.pStatus.equipWeapon.twinHand}です");
        horizontalKey = GManager.instance.InputR.GetAxisRaw(MainUI.instance.rewiredAction0);

        changeKey = GManager.instance.InputR.GetButtonUp(MainUI.instance.rewiredAction13);
        if (!GManager.instance.InputR.GetButton(MainUI.instance.rewiredAction13) && GManager.instance.isEnable && !smallTrigger)
        {
            fire1Key = GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction9);
            //lastAttack = true;
        }
        if(GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction9) && GManager.instance.InputR.GetButton(MainUI.instance.rewiredAction13))
        {
            //武器切り替え

            //さらに初期は必ず片手持ちに
            GManager.instance.pStatus.equipWeapon.twinHand = false;
            equipChange = true;
            //武器切り替え後は一回だけボタン離しても持ち手変更が反応しないようにする
        }
        else if (changeKey && !equipChange)
        {
           // Debug.Log("借金");
            GManager.instance.pStatus.equipWeapon.twinHand = !GManager.instance.pStatus.equipWeapon.twinHand;
        }
        else if(equipChange && changeKey)
        {
            equipChange = false;
        }

        if (GManager.instance.isEnable )
        {
            if (!bigTrigger)
            {

                fire2Key = GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction10);

                //fire2Axis = GManager.instance.InputR.GetAxisRaw("Fire2Axis");
            }

            if (!artsTrigger)
            {
                artsKey = GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction12);
            }
        }
        anyKey = AnyKey();




        if (isCharging && !bigAttack && !chargeAttack)
        {
            chargeKey = GManager.instance.InputR.GetButton((MainUI.instance.rewiredAction10));////Debug.log("入力");
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
            ////Debug.log("現況");
        }

    }




    private void FixedUpdate()
    {
        NumberControll();

     //     Debug.Log($"どうすか？{anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1}");

        if (!GManager.instance.isEnable)
        {
            attackNumber = 0;
            alterNumber = 0;
            artsNumber = 0;
            smallTrigger = false;
                bigTrigger = false;
            artsTrigger = false;
            //lastAttack = true;
            GManager.instance.isAttack = false;
        }


        ///<summary>
        ///攻撃中停止
        /// </summary>
        #region
        if (GManager.instance.pm.isGround && isAttackable && GManager.instance.isAttack)
        {
            // rb.velocity = new Vector2(0, GManager.instance.pm.ySpeed);
            rb.velocity = Vector2.zero;
        }
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
                    fallAttack = true;
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
                    fallAttack = true;
                    isDisEnable = true;
                }

            }

        }*/
        #endregion
        if (pm.isEnAt && !GManager.instance.isAttack && GManager.instance.isEnable)
        {
            //攻撃可能で弱攻撃ボタン押されてて攻撃してなくてスタミナが使えるなら以下の処理
            //delayTime = 0.0f;
            if (pm.isGround)
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
                    bigTrigger = false;

                    #endregion
                }
            }
            else if (!pm.isGround)
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

        #region//連撃入力とキャンセル待ち
        if (GManager.instance.isAttack && isAttackable && pm.isGround && !isCharging)
        {
            GroundCheck();
        }
        #endregion
        #region//空中連撃入力とキャンセル待ち
        else if (GManager.instance.isAttack && isAttackable && !pm.isGround && !fallAttack)
        {
            AirCheck();
        }
        #endregion
        #region//地上モーション終了検査
        if (pm.isGround)
        {
            if (attackNumber >= 1 && !isSComboEnd)
            {
                if (!GManager.instance.pStatus.equipWeapon.twinHand)
                {
                    if (CheckEnd() == false)
                    {
                        // //Debug.log("機能してます");
                        attackNumber = 0;
                        GManager.instance.isAttack = false;
                        smallTrigger = false;
                            //保障された攻撃をしました
                            
                    }
                }
                else
                {
                    if (CheckEnd() == false)
                    {
                        // //Debug.log("機能してます");
                        attackNumber = 0;
                        GManager.instance.isAttack = false;
                        smallTrigger = false;
                            //保障された攻撃をしました
                            
                    }
                }
            }
            else if (alterNumber >= 1 && !isCharging && !isBComboEnd)
            {
                if (!GManager.instance.pStatus.equipWeapon.twinHand)
                {
                    if (CheckEnd() == false)
                    {
                        ////Debug.log("機能してます");
                        alterNumber = 0;
                        bigAttack = false;
                        chargeAttack = false;
                        GManager.instance.isAttack = false;
                        bigTrigger = false;
                            //保障された攻撃をしました
                            
                    }
                }
                else 
                {
                    if (CheckEnd() == false)
                    {
                        ////Debug.log("機能してます");
                        alterNumber = 0;
                        bigAttack = false;
                        chargeAttack = false;
                        GManager.instance.isAttack = false;
                        bigTrigger = false;
                            //保障された攻撃をしました
                            
                    }
                }
                
            }
            else if (alterNumber >= 1 && isCharging && !isBComboEnd)
            {
                if (!GManager.instance.pStatus.equipWeapon.twinHand)
                {
                    if (CheckEnd() == false)
                    {
                        ////Debug.log("機能してます");
                        alterNumber = 0;
                        bigAttack = false;
                        chargeAttack = false;
                        GManager.instance.isAttack = false;
                        bigTrigger = false;
                            //保障された攻撃をしました
                            
                    }
                }
                else
                {
                    if (CheckEnd() == false)
                    {
                        ////Debug.log("機能してます");
                        alterNumber = 0;
                        bigAttack = false;
                        chargeAttack = false;
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
                    if (CheckEnd() == false)
                    {
                        // //Debug.log("機能してます");
                        artsNumber = 0;
                        GManager.instance.isAttack = false;
                        artsTrigger = false;
                            //保障された攻撃をしました
                            
                    }
                }
                else
                {
                    if (CheckEnd() == false)
                    {
                        // //Debug.log("機能してます");
                        artsNumber = 0;
                        GManager.instance.isAttack = false;
                        artsTrigger = false;
                        

                    }
                        //保障された攻撃をしました

                }
            }
            
            ///<Summary>
            /// コンボ終了後のアニメ終了確認
            ///</Summary>
            #region
            else if (isSComboEnd)
            {
                if (!GManager.instance.pStatus.equipWeapon.twinHand)
                {
                    if (CheckEnd() == false)
                    {
                        // //Debug.log("機能してます");
                       attackNumber = 0;
                        GManager.instance.isAttack = false;
                        smallTrigger = false;
                        isSComboEnd = false;
                            //保障された攻撃をしました
                            
                    }

                }
                else
                {
                    if (CheckEnd() == false)
                    {
                        // //Debug.log("機能してます");
                        attackNumber = 0;
                        GManager.instance.isAttack = false;
                        smallTrigger = false;
                        isSComboEnd = false;
                            //保障された攻撃をしました
                            
                        
                    }
                }
            }
           else if(isBComboEnd && isCharging)
            {
                if (!GManager.instance.pStatus.equipWeapon.twinHand)
                {
                    if (CheckEnd() == false)
                    {
                        ////Debug.log("機能してます");
                        alterNumber = 0;
                        bigAttack = false;
                        chargeAttack = false;
                        GManager.instance.isAttack = false;
                        bigTrigger = false;
                        isBComboEnd = false; 
                            //保障された攻撃をしました
                            
                        

                    }
                }
                else
                {
                    if (CheckEnd() == false)
                    {
                        ////Debug.log("機能してます");
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
            else if (isBComboEnd && !isCharging)
            {
                if (!GManager.instance.pStatus.equipWeapon.twinHand)
                {
                    if (CheckEnd() == false)
                    {
                        ////Debug.log("機能してます");
                        alterNumber = 0;
                        bigAttack = false;
                        chargeAttack = false;
                        GManager.instance.isAttack = false;
                        bigTrigger = false;
                        isBComboEnd = false;
                            //保障された攻撃をしました
                            
                    }
                }
                else
                {
                    if (CheckEnd() == false)
                    {
                        ////Debug.log("機能してます");
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
                    if (CheckEnd() == false)
                    {
                        // //Debug.log("機能してます");
                        artsNumber = 0;
                        GManager.instance.isAttack = false;
                        artsTrigger = false;
                        isAComboEnd = false;
                    }
                }
                else
                {
                    if (CheckEnd() == false)
                    {
                        // //Debug.log("機能してます");
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
        else if (!pm.isGround)
        {
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {
                if (attackNumber >= 1 && !isDisEnable)
                {

                    if (CheckEnd() == false)
                    {
                        ////Debug.log("機能してます");
                        GManager.instance.isAttack = false;
                        GManager.instance.airAttack = false;
                        smallTrigger = false;
                    }
                    /* if (isDisEnable)
                                            {
                                                GManager.instance.airAttack = false;
                                                attackNumber = pm.isGround ? 0 : attackNumber;
                                                //isGroundの時attackNumberを0、違うならそのまま
                                            }*/
                }
                else if (alterNumber >= 1 && !fallAttack)
                {

                    if (CheckEnd() == false)
                    {
                        ////Debug.log("機能してます");
                        GManager.instance.isAttack = false;
                        GManager.instance.airAttack = false;
                        bigTrigger = false;
                    }
                    /*                   if (fallAttack)
                                       {
                                           GManager.instance.airAttack = false;
                                           attackNumber = pm.isGround ? 0 : attackNumber;
                                           //isGroundの時attackNumberを0、違うならそのまま
                                       }*/
                }
                else if (isDisEnable)
                {
                    if (CheckEnd() == false)
                    {
                        // //Debug.log("機能してます");
                        attackNumber = 0;
                        GManager.instance.isAttack = false;
                        smallTrigger = false;
                        GManager.instance.airAttack = false;
                        isSComboEnd = false;
                    }
                }
                else if (isBComboEnd)
                {
                    /*     if (CheckEnd($"OFAttack{GManager.instance.pStatus.equipWeapon.strikeValue.Count}") == false)
                         {
                             ////Debug.log("機能してます");
                             alterNumber = 0;
                             bigAttack = false;
                             // chargeAttack = false;
                             GManager.instance.isAttack = false;
                             bigTrigger = false;
                             isBComboEnd = false;
                             GManager.instance.airAttack = false;
                         }
                     }    */
                }
            }

            else
            {
                    if (attackNumber >= 1 && !isDisEnable)
                    {

                        if (CheckEnd() == false)
                        {
                            ////Debug.log("機能してます");
                            GManager.instance.isAttack = false;
                            GManager.instance.airAttack = false;
                            smallTrigger = false;
                        }
/*                        if (isDisEnable)
                        {
                            GManager.instance.airAttack = false;
                            attackNumber = pm.isGround ? 0 : attackNumber;
                            //isGroundの時attackNumberを0、違うならそのまま
                        }*/
                    }
                    else if (alterNumber >= 1 && !isBComboEnd)
                    {

                        if (CheckEnd() == false)
                        {
                            ////Debug.log("機能してます");
                            GManager.instance.isAttack = false;
                            GManager.instance.airAttack = false;
                            bigTrigger = false;
                        }
                    /*                   if (fallAttack)
                                       {
                                           GManager.instance.airAttack = false;
                                           attackNumber = pm.isGround ? 0 : attackNumber;
                                           //isGroundの時attackNumberを0、違うならそのまま
                                       }*/
                }
                else if (isDisEnable)
                    {
                        if (CheckEnd() == false)
                        {
                            // //Debug.log("機能してます");
                            attackNumber = 0;
                            GManager.instance.isAttack = false;
                            smallTrigger = false;
                            
                            GManager.instance.airAttack = false;
                        }
                    }
                    else if (fallAttack)
                    {
                  /*      if (CheckEnd($"TFAttack{GManager.instance.pStatus.equipWeapon.twinStrikeValue.Count}") == false)
                        {
                            ////Debug.log("機能してます");
                            alterNumber = 0;
                            bigAttack = false;
                            //chargeAttack = false;
                            GManager.instance.isAttack = false;
                            bigTrigger = false;
                            isBComboEnd = false;
                        GManager.instance.airAttack = false;
                    }*/
                    }
            }
        }
        #endregion



        // //Debug.log($"判定{GManager.instance.isAttack}");
        // //Debug.log($"空中攻撃は{attackNumber}");

        if (fallAttack)
        {
            gravity = pm.gravity * 1.3f;
            rb.velocity = new Vector2(0, -gravity);
            if (pm.isGround)
            {
                GManager.instance.isAttack = false;
                fallAttack = false;
                isBComboEnd = true;
            }
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

       Debug.Log("攻撃可能に");
        isAttackable = true;
        GManager.instance.isArmor = false;
        GManager.instance.StaminaUse(useStamina);
    }

    //アニメの終了探知
    bool CheckEnd()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {


            Debug.Log($"アニメ終了{anim.GetCurrentAnimatorStateInfo(0).IsName($"TSAttack{attackNumber + 1}")}");
            return false;
        }
        // return !(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        //  (_currentStateName);
        return true;
    }

    bool AnyKey()
    {
        if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction4) || GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction11)
            || GManager.instance.InputR.GetAxisRaw(MainUI.instance.rewiredAction1) != 0 /*|| GManager.instance.InputR.GetAxisRaw(MainUI.instance.rewiredAction2) != 0*/)
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
        if (attackNumber == 0 && (fire1Key || smallTrigger) && !GManager.instance.isAttack)
        {
            GManager.instance.isAttack = true;
            sAttackPrepare();
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {
                anim.Play("OSAttack1");

        // // GManager.instance.StaminaUse(useStamina);
            }
            else
            {
                anim.Play("TSAttack1");
                //// GManager.instance.StaminaUse(useStamina);
            }
            isAttackable = false;
            attackNumber++;
            smallTrigger = false;
        }
        else if (attackNumber != 0 && (GManager.instance.pStatus.equipWeapon.isCombo || smallTrigger) && !GManager.instance.isAttack)
        {
            sAttackPrepare();
            GManager.instance.isAttack = true;
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {

                anim.Play($"OSAttack{attackNumber + 1}");
        // // GManager.instance.StaminaUse(useStamina);

            }
            else
            {
                anim.Play($"TSAttack{attackNumber + 1}");
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
             //   Debug.Log("戦況");
                if (chargeAttack)
                {
                    chargeAttackPrepare();
                    if (!GManager.instance.pStatus.equipWeapon.twinHand)
                    {
                        anim.Play($"OCAttack{alterNumber + 1}");//チャージ攻撃のアニメ
                // // GManager.instance.StaminaUse(useStamina);
                    }
                    else
                    {
                        anim.Play($"TCAttack{alterNumber + 1}");//チャージ攻撃のアニメ
                // // GManager.instance.StaminaUse(useStamina);
                    }
                    alterNumber++;
                    ////Debug.log("あああああいあいあい");
                    chargeAttack = false;
                    isCharging = false;
                    chargeTime = 0.0f;
                }
                else if (bigAttack)
                {
                    bAttackPrepare();
                    if (!GManager.instance.pStatus.equipWeapon.twinHand)
                    {
                        anim.Play($"OBAttack{alterNumber + 1}");
                // // GManager.instance.StaminaUse(useStamina);
                    }
                    else
                    {
                        anim.Play($"TBAttack{alterNumber + 1}");
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
                        anim.Play($"OCharge{alterNumber + 1}");//チャージアニメ    
                        rb.velocity = Vector2.zero;
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                        anim.Play($"TCharge{alterNumber + 1}");//チャージアニメ 
                    }
                }
            }
            else if (alterNumber != 0 || GManager.instance.pStatus.equipWeapon.isCombo)
            {
            //    Debug.Log("現況");
                if (chargeAttack)
                {
                    chargeAttackPrepare();
                    if (!GManager.instance.pStatus.equipWeapon.twinHand)
                    {
                        anim.Play($"OCAttack{alterNumber + 1}");//チャージ攻撃のアニメ
                // // GManager.instance.StaminaUse(useStamina);
                    }
                    else
                    {
                        anim.Play($"TCAttack{alterNumber + 1}");//チャージ攻撃のアニメ
                // // GManager.instance.StaminaUse(useStamina);
                    }
                    alterNumber++;
                    chargeAttack = false;
                    isCharging = false;
                    chargeTime = 0.0f;
                }
                //もし途中で手を離したら普通に外に出る、2Stayが否定されるから
                else if (bigAttack)
                {
                    bAttackPrepare();
                    if (!GManager.instance.pStatus.equipWeapon.twinHand)
                    {
                        anim.Play($"OBAttack{alterNumber + 1}");
                // // GManager.instance.StaminaUse(useStamina);
                    }
                    else
                    {
                        anim.Play($"TBAttack{alterNumber + 1}");
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
                        anim.Play($"OCharge{alterNumber + 1}");//チャージアニメ     
                    }
                    else
                    {
                        anim.Play($"TCharge{alterNumber + 1}");//チャージアニメ 
                    }
                }
            }
        }
        else
        {
         //   Debug.Log("つうじゅん況");
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

            airAttackPrepare();
            rb.velocity = Vector2.zero;
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {
                anim.Play("OAAttack1");//空中弱攻撃
        // // GManager.instance.StaminaUse(useStamina);
            }
            else
            {
                anim.Play("TAAttack1");//空中弱攻撃
        // // GManager.instance.StaminaUse(useStamina);
            }
            GManager.instance.isAttack = true;
            //コンボにする仕様上軽めの攻撃に
            isAttackable = false;
            attackNumber++;
            smallTrigger = false;
        }
        else if (attackNumber != 0 && (fire1Key || smallTrigger || GManager.instance.pStatus.equipWeapon.isCombo) && !isDisEnable && !bigTrigger )
        {
            airAttackPrepare();
            GManager.instance.isAttack = true;
            smallTrigger = false;
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {
                if (attackNumber == GManager.instance.pStatus.equipWeapon.airValue.Count)
                {
                    isDisEnable = true;
                }
                anim.Play($"OAAttack{attackNumber + 1}");
        // // GManager.instance.StaminaUse(useStamina);
            }
            else
            {
                if (attackNumber == GManager.instance.pStatus.equipWeapon.twinAirValue.Count)
                {
                    isDisEnable = true;
                }
                anim.Play($"TAAttack{attackNumber + 1}");
        // // GManager.instance.StaminaUse(useStamina);
            }
            isAttackable = false;
            attackNumber++;
            smallTrigger = false;
            //   //Debug.log("Elial");
        }
        #endregion
        //空中強攻撃
        else if ((bigTrigger || fire2Key || GManager.instance.pStatus.equipWeapon.isCombo) && !fallAttack)
        {
            strikeAttackPrepare();
            GManager.instance.isAttack = true;
            bigTrigger = false;
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {
                anim.Play($"OFAttack{attackNumber + 1}");//空中強攻撃
        // // GManager.instance.StaminaUse(useStamina);

            }
            else
            {
                anim.Play($"TFAttack{alterNumber}");//空中強攻撃
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
            GManager.instance.isAttack = true;
            ArtsPrepare();

            if (!GManager.instance.pStatus.equipWeapon.twinHand && !GManager.instance.pStatus.equipShield.weponArts)
            {
                anim.Play("OArts1");
        // // GManager.instance.StaminaUse(useStamina);//シールドのパリィにする
               // GManager.instance.isParry = true;
                GManager.instance.MpReduce(GManager.instance.pStatus.equipShield.artsMP[artsNumber]);
            }
            else
            {
                anim.Play("TArts1");
        // // GManager.instance.StaminaUse(useStamina);
                GManager.instance.MpReduce(GManager.instance.pStatus.equipWeapon.artsMP[artsNumber]);
            }
            isAttackable = false;
            artsNumber++;
            artsTrigger = false;
        }
        else if (artsNumber != 0 && artsTrigger || GManager.instance.pStatus.equipWeapon.isCombo)
        {
            ArtsPrepare();
            GManager.instance.isAttack = true;
            if (!GManager.instance.pStatus.equipWeapon.twinHand && !GManager.instance.pStatus.equipShield.weponArts)
            {
               
                anim.Play($"OArts{artsNumber + 1}");
        // // GManager.instance.StaminaUse(useStamina);//シールドのパリィにする
                GManager.instance.MpReduce(GManager.instance.pStatus.equipShield.artsMP[artsNumber]);

            }
            else
            {
                anim.Play($"TArts{artsNumber + 1}");
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
            attackNumber = 0;
            alterNumber = 0;
            artsNumber = 0;

            //保障された攻撃をしました

        }
        else if (fire1Key)
        {
            Debug.Log($"連撃");
            GManager.instance.isAttack = false;
            smallTrigger = true;
            alterNumber = 0;
            artsNumber = 0;
            //強攻撃コンボを白紙化
                //保障された攻撃をしました
                
        }
        else if (fire2Key)
        {

            GManager.instance.isAttack = false;
            bigTrigger = true;
            attackNumber = 0;
            artsNumber = 0;
            //弱攻撃コンボを白紙化
                //保障された攻撃をしました
                
        }
        else if (artsKey)
        {
            GManager.instance.isAttack = false;
            artsTrigger = true;
            attackNumber = 0;
            alterNumber = 0;
            
        }
    }
    void AirCheck()
    {
        if (anyKey)
        {
            GManager.instance.isAttack = false;
            attackNumber = 0;
            alterNumber = 0;
            
        }
        else if (fire1Key)
        {
            GManager.instance.isAttack = false;
            smallTrigger = true;
            alterNumber = 0;
            
        }
        else if (fire2Key)
        {
            GManager.instance.isAttack = false;
            bigTrigger = true;
            attackNumber = 0;
            
        }
    }

    /// <summary>
    /// 攻撃保障
    /// </summary>
    void AttackSecurity()
    {
        if(useStamina >= GManager.instance.pStatus.stamina && !lastAttack)
        {
            
        }

    }


    //攻撃するときに呼ぶ
    public void sAttackPrepare()//デフォが斬撃
    {
        if (attackNumber != 0)
        {
            transform.localScale = new Vector3(attackDirection, transform.localScale.y, transform.localScale.z);
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
            transform.localScale = new Vector3(attackDirection, transform.localScale.y, transform.localScale.z);
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
            transform.localScale = new Vector3(attackDirection, transform.localScale.y, transform.localScale.z);
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
            transform.localScale = new Vector3(attackDirection, transform.localScale.y, transform.localScale.z);
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
            transform.localScale = new Vector3(attackDirection, transform.localScale.y, transform.localScale.z);
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
            transform.localScale = new Vector3(attackDirection, transform.localScale.y, transform.localScale.z);
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
                    fallAttack = true;
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
                    fallAttack = true;
                    isDisEnable = true;
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
                       //Debug.log($"入力されたのは{code}");
                       break;
                   }
               }
           }
       }*/

    /*   void ControllerCheck()
       {
           //ゲームパッドが繋がれてるか確認
           var controllerNames = GManager.instance.InputR.GetJoystickNames();

           //Debug.log($"ゲームパッドが接続されてる{padConnect}");
           if (controllerNames != null)
           {
         //      //Debug.log($"ゲームパッドName{controllerNames[0]}");
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
}


