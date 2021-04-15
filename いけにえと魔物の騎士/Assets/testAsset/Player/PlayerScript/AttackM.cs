﻿using UnityEngine;


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
    //チャージ中
    bool chargeKey;
    float horizontalKey;
    bool anyKey;
    float attackDirection;

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
        if (!GManager.instance.InputR.GetButton(MainUI.instance.rewiredAction13))
        {
            fire1Key = GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction9);
        }
        if(GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction9) && GManager.instance.InputR.GetButton(MainUI.instance.rewiredAction13))
        {
            //武器切り替え

            //さらに初期は必ず片手持ちに
            GManager.instance.pStatus.equipWeapon.twinHand = false;
        }
        else if (changeKey)
        {
           // Debug.Log("借金");
            GManager.instance.pStatus.equipWeapon.twinHand = !GManager.instance.pStatus.equipWeapon.twinHand;
        }
            fire2Key = GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction10);

        //fire2Axis = GManager.instance.InputR.GetAxisRaw("Fire2Axis");

        
            artsKey = GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction12);
        
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
        if (alterNumber != 0)
        {
            Debug.Log($"alterは{alterNumber}");
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
        if (!GManager.instance.pStatus.equipWeapon.twinHand)
        {
            if (attackNumber >= GManager.instance.pStatus.equipWeapon.smallName.Count)
            {
                attackNumber = 0;//モーション番号のリセット
            }
            if (alterNumber >= GManager.instance.pStatus.equipWeapon.bigName.Count)
            {
                alterNumber = 0;//モーション番号のリセット
            }
            if (GManager.instance.pStatus.equipShield.weponArts)
            {
                if (artsNumber >= GManager.instance.pStatus.equipWeapon.artsName.Count)
                {
                    artsNumber = 0;//モーション番号のリセット
                }
            }
            else
            {
                if (artsNumber >= GManager.instance.pStatus.equipShield.artsName.Count)
                {
                    artsNumber = 0;//モーション番号のリセット
                }
            }
        }
        else
        {
            if (attackNumber >= GManager.instance.pStatus.equipWeapon.twinSmallName.Count)
            {
                attackNumber = 0;//モーション番号のリセット
            }
            if (alterNumber >= GManager.instance.pStatus.equipWeapon.twinBigName.Count)
            {
                alterNumber = 0;//モーション番号のリセット
            }

            if (artsNumber >= GManager.instance.pStatus.equipWeapon.artsName.Count)
            {
                    artsNumber = 0;//モーション番号のリセット
            }
  
        }

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

        }
        if (GManager.instance.isAttack && isCharging)
        {
            ChargeAttack();
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
            if (attackNumber >= 1)
            {
                if (!GManager.instance.pStatus.equipWeapon.twinHand)
                {
                    if (CheckEnd(GManager.instance.pStatus.equipWeapon.smallName[attackNumber - 1]) == false)
                    {
                        // //Debug.log("機能してます");
                        attackNumber = 0;
                        GManager.instance.isAttack = false;
                        smallTrigger = false;
                    }
                }
                else
                {
                    if (CheckEnd(GManager.instance.pStatus.equipWeapon.twinSmallName[attackNumber - 1]) == false)
                    {
                        // //Debug.log("機能してます");
                        attackNumber = 0;
                        GManager.instance.isAttack = false;
                        smallTrigger = false;
                    }
                }
            }
            else if (alterNumber >= 1 && !isCharging)
            {

                if (CheckEnd(GManager.instance.pStatus.equipWeapon.bigName[alterNumber - 1]) == false)
                {
                    ////Debug.log("機能してます");
                    alterNumber = 0;
                    bigAttack = false;
                    chargeAttack = false;
                    GManager.instance.isAttack = false;
                    bigTrigger = false;
                }
            }
            else if(artsNumber >= 1)
            {
                if (!GManager.instance.pStatus.equipWeapon.twinHand && !GManager.instance.pStatus.equipShield.weponArts)
                {
                    if (CheckEnd(GManager.instance.pStatus.equipShield.artsName[artsNumber - 1]) == false)
                    {
                        // //Debug.log("機能してます");
                        artsNumber = 0;
                        GManager.instance.isAttack = false;
                        artsTrigger = false;
                    }
                }
                else
                {
                    if (CheckEnd(GManager.instance.pStatus.equipWeapon.artsName[artsNumber - 1]) == false)
                    {
                        // //Debug.log("機能してます");
                        artsNumber = 0;
                        GManager.instance.isAttack = false;
                        artsTrigger = false;
                    }

                }
            }
        }
        #endregion
        #region//空中モーション終了検査
        else if (!pm.isGround)
        {
            if (attackNumber >= 1)
            {

                if (CheckEnd(GManager.instance.pStatus.equipWeapon.airName[attackNumber - 1]) == false)
                {
                    ////Debug.log("機能してます");
                    GManager.instance.isAttack = false;
                    GManager.instance.airAttack = false;
                    smallTrigger = false;
                }
                if (isDisEnable)
                {
                    GManager.instance.airAttack = false;
                    attackNumber = pm.isGround ? 0 : attackNumber;
                    //isGroundの時attackNumberを0、違うならそのまま
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
      //  Debug.Log("やぁ");
        isAttackable = true;
        GManager.instance.isArmor = false;
    }

    //アニメの終了探知
    bool CheckEnd(string _currentStateName)
    {

        return !(anim.GetCurrentAnimatorStateInfo(0).normalizedTime == 1);

          //  (_currentStateName);

    }

    bool AnyKey()
    {
        if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction4) || GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction11)
            || GManager.instance.InputR.GetAxisRaw(MainUI.instance.rewiredAction1) != 0 /*|| GManager.instance.InputR.GetAxisRaw("Horizontal") != 0*/)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void NormalAttack()
    {
        #region//通常攻撃
        if (attackNumber == 0 && (fire1Key || smallTrigger))
        {
            sAttackPrepare();
            GManager.instance.isAttack = true;
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {
                anim.Play(GManager.instance.pStatus.equipWeapon.smallName[attackNumber]);
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.normalStamina);
            }
            else
            {
                anim.Play(GManager.instance.pStatus.equipWeapon.twinSmallName[attackNumber]);
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.normalStaminaT);
            }
            isAttackable = false;
            attackNumber++;
            smallTrigger = false;
        }
        else if (attackNumber != 0 && (GManager.instance.pStatus.equipWeapon.isCombo || smallTrigger))
        {
            sAttackPrepare();
            GManager.instance.isAttack = true;
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {
                smallTrigger = false;
                anim.Play(GManager.instance.pStatus.equipWeapon.smallName[attackNumber]);
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.normalStamina);
                isAttackable = false;
                attackNumber++;
            }
            else
            {
                anim.Play(GManager.instance.pStatus.equipWeapon.twinSmallName[attackNumber]);
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.normalStaminaT);
                isAttackable = false;
                attackNumber++;
                smallTrigger = false;
            }
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
                        anim.Play(GManager.instance.pStatus.equipWeapon.maxName[alterNumber]);//チャージ攻撃のアニメ
                        GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.chargeStamina);
                    }
                    else
                    {
                        anim.Play(GManager.instance.pStatus.equipWeapon.twinMaxName[alterNumber]);//チャージ攻撃のアニメ
                        GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.chargeStaminaT);
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
                        anim.Play(GManager.instance.pStatus.equipWeapon.bigName[alterNumber]);
                        GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.bigStamina);
                    }
                    else
                    {
                        anim.Play(GManager.instance.pStatus.equipWeapon.twinBigName[alterNumber]);
                        GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.bigStaminaT);
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
                        anim.Play(GManager.instance.pStatus.equipWeapon.chargeName[alterNumber]);//チャージアニメ    
                        rb.velocity = Vector2.zero;
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                        anim.Play(GManager.instance.pStatus.equipWeapon.twinChargeName[alterNumber]);//チャージアニメ 
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
                        anim.Play(GManager.instance.pStatus.equipWeapon.bigName[alterNumber]);//チャージ攻撃のアニメ
                        GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.chargeStamina);
                    }
                    else
                    {
                        anim.Play(GManager.instance.pStatus.equipWeapon.twinBigName[alterNumber]);//チャージ攻撃のアニメ
                        GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.chargeStaminaT);
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
                        anim.Play(GManager.instance.pStatus.equipWeapon.bigName[alterNumber]);
                        GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.bigStamina);
                    }
                    else
                    {
                        anim.Play(GManager.instance.pStatus.equipWeapon.twinBigName[alterNumber]);
                        GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.bigStaminaT);
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
                        anim.Play(GManager.instance.pStatus.equipWeapon.chargeName[alterNumber]);//チャージアニメ     
                    }
                    else
                    {
                        anim.Play(GManager.instance.pStatus.equipWeapon.twinChargeName[alterNumber]);//チャージアニメ 
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
                anim.Play(GManager.instance.pStatus.equipWeapon.airName[0]);//空中弱攻撃
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.airStamina);
            }
            else
            {
                anim.Play(GManager.instance.pStatus.equipWeapon.twinAirName[0]);//空中弱攻撃
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.airStaminaT);
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
                if (attackNumber == GManager.instance.pStatus.equipWeapon.airName.Count - 2)
                {
                    isDisEnable = true;
                }
                anim.Play(GManager.instance.pStatus.equipWeapon.airName[attackNumber]);
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.airStamina);
            }
            else
            {
                if (attackNumber == GManager.instance.pStatus.equipWeapon.twinAirName.Count - 2)
                {
                    isDisEnable = true;
                }
                anim.Play(GManager.instance.pStatus.equipWeapon.twinAirName[attackNumber]);
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.airStaminaT);
            }
            isAttackable = false;
            attackNumber = 0;
            //   //Debug.log("Elial");
        }
        #endregion
        //空中強攻撃
        else if (bigTrigger || fire2Key || GManager.instance.pStatus.equipWeapon.isCombo)
        {
            strikeAttackPrepare();
            GManager.instance.isAttack = true;
            bigTrigger = false;
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {
                anim.Play(GManager.instance.pStatus.equipWeapon.airName[GManager.instance.pStatus.equipWeapon.airName.Count - 1]);//空中強攻撃
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.bigStamina);
            }
            else
            {
                anim.Play(GManager.instance.pStatus.equipWeapon.airName[GManager.instance.pStatus.equipWeapon.twinAirName.Count - 1]);//空中強攻撃
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.bigStaminaT);
            }
            isAttackable = false;
            isDisEnable = true;
            attackNumber = 0;
            fallAttack = true;
        }
    }

    void ArtsAttack()
    {
        #region//特殊行動
        if (artsNumber == 0 && (artsKey || artsTrigger))
        {
            GManager.instance.isAttack = true;
            ArtsPrepare();
            GManager.instance.isAttack = true;
            if (!GManager.instance.pStatus.equipWeapon.twinHand && !GManager.instance.pStatus.equipShield.weponArts)
            {
                anim.Play(GManager.instance.pStatus.equipShield.artsName[artsNumber]);
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipShield.artsStamina);//シールドのパリィにする
                GManager.instance.isParry = true;
            }
            else
            {
                anim.Play(GManager.instance.pStatus.equipWeapon.artsName[artsNumber]);
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.artsStamina);
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
                anim.Play(GManager.instance.pStatus.equipShield.artsName[artsNumber]);
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipShield.artsStamina);//シールドのパリィにする
            }
            else
            {
                anim.Play(GManager.instance.pStatus.equipWeapon.artsName[artsNumber]);
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.artsStamina);
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
        }
        else if (fire1Key)
        {
            GManager.instance.isAttack = false;
            smallTrigger = true;
            alterNumber = 0;
            artsNumber = 0;
            //強攻撃コンボを白紙化

        }
        else if (fire2Key)
        {
            Debug.Log($"トリガー追加でalterは{alterNumber}");
            GManager.instance.isAttack = false;
            bigTrigger = true;
            attackNumber = 0;
            artsNumber = 0;
            //弱攻撃コンボを白紙化
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
        }
        else if (fire1Key)
        {
            GManager.instance.isAttack = false;
            smallTrigger = true;
        }
        else if (fire2Key)
        {
            GManager.instance.isAttack = false;
            bigTrigger = true;
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
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipWeapon.strikeValue.type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.strikeValue.x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.strikeValue.y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipWeapon.strikeValue.z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipWeapon.strikeValue.isBlow;
            GManager.instance.pStatus.equipWeapon.isCombo = GManager.instance.pStatus.equipWeapon.strikeValue.isCombo;
            GManager.instance.pStatus.equipWeapon.blowPower = GManager.instance.pStatus.equipWeapon.strikeValue.blowPower;
        }
        else
        {
            GManager.instance.isGuard = false;
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipWeapon.twinStrikeValue.type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.twinStrikeValue.x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.twinStrikeValue.y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipWeapon.twinStrikeValue.z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipWeapon.twinStrikeValue.isBlow;
            GManager.instance.pStatus.equipWeapon.isCombo = GManager.instance.pStatus.equipWeapon.twinStrikeValue.isCombo;
            GManager.instance.pStatus.equipWeapon.blowPower = GManager.instance.pStatus.equipWeapon.twinStrikeValue.blowPower;
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
        }
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


