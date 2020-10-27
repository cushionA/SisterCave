using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackM : MonoBehaviour
{
    public GameObject Player;

    //------------------------------------------内部パラメータ

    PlayerMove pm;
    bool padConnect;
    //attackNumberと連動してステート名を獲得
    /// <summary>
    /// 弱攻撃ボタンが押されてるかどうか
    /// </summary>
    bool fire1Key;

    /// <summary>
    /// 強攻撃ボタンが押されてるかどうか
    /// </summary>
    bool fire2Key;
    float fire2Axis;
    bool fire2Assist;

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

    SimpleAnimation sAni;
    float delayTime;
    int attackNumber;
    int alterNumber;
    int artsNumber;
    Animation anim;
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
    float chargeKey;
    float horizontalKey;
    bool anyKey;
    float attackDirection;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {

        pm = Player.GetComponent<PlayerMove>();
        sAni = GetComponent<SimpleAnimation>();
        rb = GetComponent<Rigidbody2D>();
        GManager.instance.SetAtk();
    }

    // Update is called once per frame

    private void Update()
    {

        DownKeyCheck();
   //     ControllerCheck();

        horizontalKey = GManager.instance.InputR.GetAxisRaw("Horizontal");
        fire1Key = GManager.instance.InputR.GetButtonDown("Fire1");
        if (!padConnect)
        {
            fire2Key = GManager.instance.InputR.GetButtonDown("Fire2");
        }
        fire2Axis = GManager.instance.InputR.GetAxisRaw("Fire2Axis");
        if (padConnect)
        {
            if (fire2Axis < 0 && !fire2Assist)
            {
                fire2Key = false;
                artsKey = true;
                fire2Assist = true;
            }
            else if (fire2Axis > 0 && !fire2Assist)
            {
                fire2Key = true;
                artsKey = false;
                fire2Assist = true;
            }
            else if(fire2Axis == 0)
            {
                fire2Key = false;
                artsKey = false;
                fire2Assist = false;
            }
        }
        if (!padConnect)
        {
            artsKey = GManager.instance.InputR.GetButtonDown("Arts");
        }
        anyKey = AnyKey();
        if (isCharging && !bigAttack && !chargeAttack && !padConnect)
        {
            chargeKey = GManager.instance.InputR.GetAxisRaw("Fire2");//Debug.Log("入力");
        }
        else if(isCharging && !bigAttack && !chargeAttack && padConnect)
        {
            chargeKey = GManager.instance.InputR.GetAxisRaw("Fire2Axis");//Debug.Log("入力");
        }
        else
        {
            chargeKey = 0.0f;
            //攻撃中は溜められない
        }
        if (chargeKey > 0 && !chargeAttack && !bigAttack)
        {
            chargeTime += Time.deltaTime;
            //チャージ中
            if (chargeTime >= GManager.instance.pStatus.equipWeapon.chargeRes)
            {
                // isCharging = false;
                //chargeTime = 0.0f;
                chargeAttack = true;
            }
        }
        else if (chargeTime < GManager.instance.pStatus.equipWeapon.chargeRes && chargeKey == 0 && isCharging && !bigAttack && !chargeAttack)
        {
            // chargeTime = 0.0f;
            //isCharging = false;
            bigAttack = true;
            //Debug.Log("現況");
        }
    }




    private void FixedUpdate()
    {
        if (attackNumber >= GManager.instance.pStatus.equipWeapon.smallName.Count)
        {
            attackNumber = 0;//モーション番号のリセット
        }
        if (alterNumber >= GManager.instance.pStatus.equipWeapon.bigName.Count)
        {
            alterNumber = 0;//モーション番号のリセット
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
                if (CheckEnd(GManager.instance.pStatus.equipWeapon.smallName[attackNumber - 1]) == false)
                {
                    // Debug.Log("機能してます");
                    attackNumber = 0;
                    GManager.instance.isAttack = false;
                    smallTrigger = false;
                }
            }
            else if (alterNumber >= 1 && !isCharging)
            {

                if (CheckEnd(GManager.instance.pStatus.equipWeapon.bigName[alterNumber - 1]) == false)
                {
                    Debug.Log("機能してます");
                    alterNumber = 0;
                    bigAttack = false;
                    chargeAttack = false;
                    GManager.instance.isAttack = false;
                    bigTrigger = false;
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
                    //Debug.Log("機能してます");
                    GManager.instance.isAttack = false;
                    smallTrigger = false;
                }
                if (isDisEnable)
                {

                    attackNumber = pm.isGround ? 0 : attackNumber;
                    //isGroundの時attackNumberを0、違うならそのまま
                }
            }
        }
        #endregion



        // Debug.Log($"判定{GManager.instance.isAttack}");
        // Debug.Log($"空中攻撃は{attackNumber}");

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

    void Continue()
    {
        isAttackable = true;
        GManager.instance.isArmor = false;
    }


    bool CheckEnd(string _currentStateName)
    {
        return sAni.IsPlaying(_currentStateName);

    }

    bool AnyKey()
    {
        if (GManager.instance.InputR.GetButtonDown("Avoid") || GManager.instance.InputR.GetButtonDown("Guard")
            || GManager.instance.InputR.GetAxisRaw("Vertical") != 0 /*|| GManager.instance.InputR.GetAxisRaw("Horizontal") != 0*/)
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
                sAni.Play(GManager.instance.pStatus.equipWeapon.smallName[attackNumber]);
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.normalStamina);
            }
            else
            {
                sAni.Play(GManager.instance.pStatus.equipWeapon.twinSmallName[attackNumber]);
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.normalStaminaT);
            }
            isAttackable = false;
            attackNumber++;
            smallTrigger = false;
        }
        else if (attackNumber != 0 && smallTrigger)
        {
            sAttackPrepare();
            GManager.instance.isAttack = true;
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {
                smallTrigger = false;
                sAni.Play(GManager.instance.pStatus.equipWeapon.smallName[attackNumber]);
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.normalStamina);
                isAttackable = false;
                attackNumber++;
            }
            else
            {
                sAni.Play(GManager.instance.pStatus.equipWeapon.twinSmallName[attackNumber]);
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
        if (alterNumber == 0)
        {
            if (chargeAttack)
            {
                chargeAttackPrepare();
                if (!GManager.instance.pStatus.equipWeapon.twinHand)
                {
                    sAni.Play(GManager.instance.pStatus.equipWeapon.bigName[alterNumber]);//チャージ攻撃のアニメ
                    GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.chargeStamina);
                }
                else
                {
                    sAni.Play(GManager.instance.pStatus.equipWeapon.twinBigName[alterNumber]);//チャージ攻撃のアニメ
                    GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.chargeStaminaT);
                }
                alterNumber++;
                //Debug.Log("あああああいあいあい");
                chargeAttack = false;
                isCharging = false;
                chargeTime = 0.0f;
            }
            else if (bigAttack)
            {
                bAttackPrepare();
                if (!GManager.instance.pStatus.equipWeapon.twinHand)
                {
                    sAni.Play(GManager.instance.pStatus.equipWeapon.bigName[alterNumber]);
                    GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.bigStamina);
                }
                else
                {
                    sAni.Play(GManager.instance.pStatus.equipWeapon.twinBigName[alterNumber]);
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
                    sAni.Play(GManager.instance.pStatus.equipWeapon.chargeName[alterNumber]);//チャージアニメ     
                }
                else
                {
                    sAni.Play(GManager.instance.pStatus.equipWeapon.twinChargeName[alterNumber]);//チャージアニメ 
                }
            }
        }
        else if (alterNumber != 0)
        {
            if (chargeAttack)
            {
                chargeAttackPrepare();
                if (!GManager.instance.pStatus.equipWeapon.twinHand)
                {
                    sAni.Play(GManager.instance.pStatus.equipWeapon.bigName[alterNumber]);//チャージ攻撃のアニメ
                    GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.chargeStamina);
                }
                else
                {
                    sAni.Play(GManager.instance.pStatus.equipWeapon.twinBigName[alterNumber]);//チャージ攻撃のアニメ
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
                    sAni.Play(GManager.instance.pStatus.equipWeapon.bigName[alterNumber]);
                    GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.bigStamina);
                }
                else
                {
                    sAni.Play(GManager.instance.pStatus.equipWeapon.twinBigName[alterNumber]);
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
                    sAni.Play(GManager.instance.pStatus.equipWeapon.chargeName[alterNumber]);//チャージアニメ     
                }
                else
                {
                    sAni.Play(GManager.instance.pStatus.equipWeapon.twinChargeName[alterNumber]);//チャージアニメ 
                }
            }
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
                sAni.Play(GManager.instance.pStatus.equipWeapon.airName[0]);//空中弱攻撃
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.airStamina);
            }
            else
            {
                sAni.Play(GManager.instance.pStatus.equipWeapon.twinAirName[0]);//空中弱攻撃
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.airStaminaT);
            }
            GManager.instance.isAttack = true;
            //コンボにする仕様上軽めの攻撃に
            isAttackable = false;
            attackNumber++;
            smallTrigger = false;
        }
        else if (attackNumber != 0 && (fire1Key || smallTrigger) && !isDisEnable && !bigTrigger)
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
                sAni.Play(GManager.instance.pStatus.equipWeapon.airName[attackNumber]);
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.airStamina);
            }
            else
            {
                if (attackNumber == GManager.instance.pStatus.equipWeapon.twinAirName.Count - 2)
                {
                    isDisEnable = true;
                }
                sAni.Play(GManager.instance.pStatus.equipWeapon.twinAirName[attackNumber]);
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.airStaminaT);
            }
            isAttackable = false;
            attackNumber = 0;
            //   Debug.Log("Elial");
        }
        #endregion
        //空中強攻撃
        else if (bigTrigger || fire2Key)
        {
            strikeAttackPrepare();
            GManager.instance.isAttack = true;
            bigTrigger = false;
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {
                sAni.Play(GManager.instance.pStatus.equipWeapon.airName[GManager.instance.pStatus.equipWeapon.airName.Count - 1]);//空中強攻撃
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.bigStamina);
            }
            else
            {
                sAni.Play(GManager.instance.pStatus.equipWeapon.airName[GManager.instance.pStatus.equipWeapon.twinAirName.Count - 1]);//空中強攻撃
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
                sAni.Play(GManager.instance.pStatus.equipShield.artsName);
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipShield.artsStamina);//シールドのパリィにする
                GManager.instance.isParry = true;
            }
            else
            {
                sAni.Play(GManager.instance.pStatus.equipWeapon.artsName[artsNumber]);
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.artsStamina);
            }
            isAttackable = false;
            artsNumber++;
            artsTrigger = false;
        }
        else if (artsNumber != 0 && artsTrigger)
        {
            ArtsPrepare();
            GManager.instance.isAttack = true;
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {
                sAni.Play(GManager.instance.pStatus.equipWeapon.artsName[artsNumber]);
                GManager.instance.StaminaUse(GManager.instance.pStatus.equipWeapon.artsStamina);//シールドのパリィにする
            }
            else
            {
                sAni.Play(GManager.instance.pStatus.equipWeapon.artsName[artsNumber]);
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
            GManager.instance.pStatus.equipWeapon.hitLimmit = GManager.instance.pStatus.equipWeapon.sValue[attackNumber].hitLimmit;
        }
        else
        {
            GManager.instance.isGuard = false;
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipWeapon.twinSValue[attackNumber].type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.twinSValue[attackNumber].x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.twinSValue[attackNumber].y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipWeapon.twinSValue[attackNumber].z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipWeapon.twinSValue[attackNumber].isBlow;
            GManager.instance.pStatus.equipWeapon.hitLimmit = GManager.instance.pStatus.equipWeapon.twinSValue[attackNumber].hitLimmit;
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
            GManager.instance.pStatus.equipWeapon.hitLimmit = GManager.instance.pStatus.equipWeapon.bValue[alterNumber].hitLimmit;
        }
        else
        {
            GManager.instance.isGuard = false;
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipWeapon.twinBValue[alterNumber].type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.twinBValue[alterNumber].x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.twinBValue[alterNumber].y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipWeapon.twinBValue[alterNumber].z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipWeapon.twinBValue[alterNumber].isBlow;
            GManager.instance.pStatus.equipWeapon.hitLimmit = GManager.instance.pStatus.equipWeapon.twinBValue[alterNumber].hitLimmit;
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
            GManager.instance.pStatus.equipWeapon.hitLimmit = GManager.instance.pStatus.equipWeapon.chargeValue[alterNumber].hitLimmit;
        }
        else
        {
            GManager.instance.isGuard = false;
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipWeapon.twinChargeValue[alterNumber].type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.twinChargeValue[alterNumber].x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.twinChargeValue[alterNumber].y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipWeapon.twinChargeValue[alterNumber].z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipWeapon.twinChargeValue[alterNumber].isBlow;
            GManager.instance.pStatus.equipWeapon.hitLimmit = GManager.instance.pStatus.equipWeapon.twinChargeValue[alterNumber].hitLimmit;
        }
    }
    public void airAttackPrepare()//デフォが斬撃
    {
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
            GManager.instance.pStatus.equipWeapon.hitLimmit = GManager.instance.pStatus.equipWeapon.airValue[attackNumber].hitLimmit;
        }
        else
        {
            GManager.instance.isGuard = false;
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipWeapon.twinAirValue[attackNumber].type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.twinAirValue[attackNumber].x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.twinAirValue[attackNumber].y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipWeapon.twinAirValue[attackNumber].z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipWeapon.twinAirValue[attackNumber].isBlow;
            GManager.instance.pStatus.equipWeapon.hitLimmit = GManager.instance.pStatus.equipWeapon.twinAirValue[attackNumber].hitLimmit;
        }
    }
    public void strikeAttackPrepare()//デフォが斬撃
    {
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
            GManager.instance.pStatus.equipWeapon.hitLimmit = GManager.instance.pStatus.equipWeapon.strikeValue.hitLimmit;
        }
        else
        {
            GManager.instance.isGuard = false;
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipWeapon.twinStrikeValue.type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.twinStrikeValue.x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.twinStrikeValue.y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipWeapon.twinStrikeValue.z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipWeapon.twinStrikeValue.isBlow;
            GManager.instance.pStatus.equipWeapon.hitLimmit = GManager.instance.pStatus.equipWeapon.twinStrikeValue.hitLimmit;
        }
    }

    public void ArtsPrepare()//デフォが斬撃
    {
        if (artsNumber != 0)
        {
            transform.localScale = new Vector3(attackDirection, transform.localScale.y, transform.localScale.z);
        }
        if (!GManager.instance.pStatus.equipWeapon.twinHand)
        {
            GManager.instance.isGuard = false;
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipWeapon.artsValue.type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.artsValue.x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.artsValue.y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipWeapon.artsValue.z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipWeapon.artsValue.isBlow;
            GManager.instance.pStatus.equipWeapon.hitLimmit = GManager.instance.pStatus.equipWeapon.artsValue.hitLimmit;
        }
        else
        {
            GManager.instance.isGuard = false;
            GManager.instance.pStatus.equipWeapon.atType = GManager.instance.pStatus.equipWeapon.artsValue.type;
            GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.artsValue.x;
            GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.artsValue.y;
            GManager.instance.pStatus.equipWeapon.shock = GManager.instance.pStatus.equipWeapon.artsValue.z;
            GManager.instance.pStatus.equipWeapon.isBlow = GManager.instance.pStatus.equipWeapon.artsValue.isBlow;
            GManager.instance.pStatus.equipWeapon.hitLimmit = GManager.instance.pStatus.equipWeapon.artsValue.hitLimmit;
        }
    }
    public void PlayerArmor()
    {
        GManager.instance.isArmor = true;

    }//判定出す直前にアニメイベントで呼び出す。
    void DownKeyCheck()
    {
        if (GManager.instance.InputR.anyKeyDown)
        {
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (GManager.instance.InputR.GetKeyDown(code))
                {
                    //処理を書く
                    Debug.Log($"入力されたのは{code}");
                    break;
                }
            }
        }
    }

 /*   void ControllerCheck()
    {
        //ゲームパッドが繋がれてるか確認
        var controllerNames = GManager.instance.InputR.GetJoystickNames();

        Debug.Log($"ゲームパッドが接続されてる{padConnect}");
        if (controllerNames != null)
        {
      //      Debug.Log($"ゲームパッドName{controllerNames[0]}");
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
    
