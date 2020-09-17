using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackM : MonoBehaviour
{
    public GameObject Player;
    public float chargeRes;
    //チャージ時間長くしたり減らしたりできる状態があるとおもろいからパブリック

    [SerializeField] List<string> smallName;
    [SerializeField] List<string> bigName;
    [SerializeField] List<string> airName;
    //チェックのためにairにも強を含む三つ格納

    PlayerMove pm;
    //attackNumberと連動してステート名を獲得
    bool fire1Key;
    bool fire2Key;
    bool fire2Exit;
    //ため押し
    float chargeTime;
    float gravity;//重力を入れる

    bool isDisEnable;//空中弱攻撃を二回までに制限
    bool fallAttack;//空中強攻撃の落下終了までisAttackをキープするためのフラグ

    SimpleAnimation sAni;
    float delayTime;
    int attackNumber;
    int alterNumber;
    Animation anim;
    bool isAttackable;
    bool smallTrigger;
    bool bigTrigger;
    bool MagicTrigger;
    //連撃のトリガーになる
    bool bigAttack;
    //強攻撃
    bool chargeAttack;
    bool isCharging;
    //チャージ中
    float chargeKey;
    bool anyKey;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {

        pm = Player.GetComponent<PlayerMove>();
        sAni = GetComponent<SimpleAnimation>();
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame

    private void Update()
    {
        fire1Key = Input.GetButtonDown("Fire1");
        fire2Key = Input.GetButtonDown("Fire2");
        anyKey = AnyKey();
        if (isCharging && !bigAttack && !chargeAttack)
        {
            chargeKey = Input.GetAxisRaw("Fire2");//Debug.Log("入力");
        }
        else
        {
            chargeKey = 0.0f;
            //攻撃中は溜められない
        }
        if (chargeKey > 0　&& !chargeAttack && !bigAttack)
        {
            chargeTime += Time.deltaTime;
            //チャージ中
            if (chargeTime >= chargeRes)
            {
               // isCharging = false;
                //chargeTime = 0.0f;
                chargeAttack = true;
            }
        }
            else if(chargeTime < chargeRes && chargeKey == 0 && isCharging && !bigAttack && !chargeAttack)
            {
               // chargeTime = 0.0f;
              //isCharging = false;
                bigAttack = true;
            //Debug.Log("現況");
        }
        }




    private void FixedUpdate()
    {
        if (attackNumber >= smallName.Count)
        {
            attackNumber = 0;//モーション番号のリセット
        }
        if (alterNumber >= bigName.Count)
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
                    if (CheckEnd(smallName[attackNumber - 1]) == false)
                    {
                        // Debug.Log("機能してます");
                        attackNumber = 0;
                        GManager.instance.isAttack = false;
                    smallTrigger = false;
                    }
                }
                else if (alterNumber >= 1 && !isCharging)
                {

                    if (CheckEnd(bigName[alterNumber - 1]) == false)
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

                    if (CheckEnd(airName[attackNumber - 1]) == false)
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
        if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Avoid") || Input.GetButtonDown("Guard")
            || Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
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
            GManager.instance.isAttack = true;
            sAni.Play("Attack");
            GManager.instance.StaminaUse(15);
            isAttackable = false;
            attackNumber++;
            smallTrigger = false;
        }
        else if (attackNumber == 1 && smallTrigger)
        {
            GManager.instance.isAttack = true;
            smallTrigger = false;
            sAni.Play("Attack2");
            GManager.instance.StaminaUse(15);
            isAttackable = false;
            attackNumber++;
        }
        #endregion
    }
    void ChargeAttack()
    {
        if (alterNumber == 0)
        {
            if (chargeAttack)
            {
                sAni.Play("Attack3");//チャージ攻撃のアニメ
                GManager.instance.StaminaUse(40);
                alterNumber++;
                Debug.Log("あああああいあいあい");
                chargeAttack = false;
                isCharging = false;
                chargeTime = 0.0f;
            }
            else if (bigAttack)
            {
                sAni.Play("Attack3");
                GManager.instance.StaminaUse(25);
                alterNumber++;
                // Debug.Log("どういうこと");
                bigAttack = false;
                isCharging = false;
                chargeTime = 0.0f;
            }
            else
            {
                sAni.Play("Squat");//チャージアニメ     
            }
        }
        else if (alterNumber == 1)
        {
            if (chargeAttack)
            {
                sAni.Play("Attack");//チャージ攻撃のアニメ
                GManager.instance.StaminaUse(40);
                alterNumber++;
                Debug.Log("あああああいあいあい");
                chargeAttack = false;
                isCharging = false;
                chargeTime = 0.0f;
            }
            //もし途中で手を離したら普通に外に出る、2Stayが否定されるから
            else if (bigAttack)
            {
                sAni.Play("Attack");
                GManager.instance.StaminaUse(25);
                alterNumber++;
                Debug.Log("どういうこと");
                bigAttack = false;
                isCharging = false;
                chargeTime = 0.0f;
            }
            else
            {
                sAni.Play("Squat");//チャージアニメ     
            }
        }
    }
    void AirAttack()
    {
        #region//空中弱攻撃
        if (attackNumber == 0 && (fire1Key || smallTrigger) && !isDisEnable && !bigTrigger)
        {
            GManager.instance.isAttack = true;
            sAni.Play("Attack");//空中弱攻撃
            GManager.instance.StaminaUse(10);
            //コンボにする仕様上軽めの攻撃に
            isAttackable = false;
            attackNumber++;
            smallTrigger = false;
        }
        else if (attackNumber == 1 && (fire1Key || smallTrigger) && !isDisEnable && !bigTrigger)
        {
            GManager.instance.isAttack = true;
            smallTrigger = false;
            sAni.Play("Attack3");
            GManager.instance.StaminaUse(15);
            isAttackable = false;
            isDisEnable = true;
            attackNumber = 0;
            Debug.Log("Elial");
        }
        #endregion
        //空中強攻撃
        else if (bigTrigger || fire2Key)
        {
            GManager.instance.isAttack = true;
            bigTrigger = false;
            sAni.Play("Attack");//空中強攻撃
            GManager.instance.StaminaUse(30);
            isAttackable = false;
            isDisEnable = true;
            attackNumber = 0;
            fallAttack = true;
        }
    }
    void GroundCheck()
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
            //強攻撃コンボを白紙化

        }
        else if (fire2Key)
        {
            GManager.instance.isAttack = false;
            bigTrigger = true;
            attackNumber = 0;
            //弱攻撃コンボを白紙化
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
     public void PlayerAttackPrepre(int i, Wepon.AttackType type = Wepon.AttackType.Slash)//デフォが斬撃
     {
          Mathf.Clamp(i, 0, GManager.instance.pStatus.equipWeapon.motionValue.Count - 1);
         GManager.instance.pStatus.equipWeapon.atType = type;
        GManager.instance.pStatus.equipWeapon.mValue = GManager.instance.pStatus.equipWeapon.motionValue[i];
        GManager.instance.pStatus.equipWeapon.atAromor = GManager.instance.pStatus.equipWeapon.attackAromor[i];
    }

    public void PlayerArmor()
    {
        GManager.instance.isArmor = true;

    }//判定出す直前にアニメイベントで呼び出す。

}
