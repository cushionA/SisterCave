using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GManager : MonoBehaviour
{
    public static GManager instance = null;

    public GameObject Player;
    //プレイヤーオブジェクト
    public PlayerStatus pStatus;
    //プレイヤーのステータスを取得
    public int stRecover = 3;
    //スタミナ回復量
    public Slider stSlider;
    //スタミナスライダー
    public Slider HpSlider;
    //HPスライダー
    public Slider MpSlider;
    //MPスライダー

    public ToolItem[] useList = new ToolItem[7];
    public Wepon[] setWepon = new Wepon[2];
    public Shield[] setShield = new Shield[2];

    public float initialHpSl;
    public float initialMpSl;
    public float initialStaminaSl;



    RectTransform hpSl;
    RectTransform mpSl;
    RectTransform staminaSl;

    [HideInInspector] public bool isEnable;
    //スタミナが回復するかどうか
    [HideInInspector] public bool isAttack;
    //攻撃中か否か
    [HideInInspector] public bool isDown;
    //ダウン中
    [HideInInspector] public bool isBlow;
    //if(isBlow && nowArmor<=0)とelseでisBlowFalseに
    [HideInInspector] public bool isGBreak;//ガードブレイク
    [HideInInspector] public bool isGuard;
    [HideInInspector] public bool guardEnable;
    [HideInInspector]public bool isParry;
    [HideInInspector] public bool isBounce;//攻撃を跳ね返されたフラグ
    [HideInInspector] public bool onGimmick;//ギミック利用中かどうか
    [HideInInspector]public bool guardHit;//ガードにヒットした
    [HideInInspector] public bool badCondition;//状態異常

    #region//シスターさんのためのフラグ
    [HideInInspector] public bool isLadder;
    [HideInInspector] public bool isRopeJump;
    #endregion

    public Vector2 blowVector;

    float stTime;
    //スタミナが回復する間隔の時間が経過したかどうか
    float disEnaTime;
    //スタミナ回復不能時間
    float parryTime;
    AttackM at;
    PlayerMove pm;
    bool stBreake;
    //スタミナ回復不能状態終わりフラグ
    public bool isArmor;//強靭ついてるかどうか

    //プレイヤーがレベルアップしたらHPとスタミナのスライダーの長さをチェックして伸ばす。
    //あとステータス画面に格納する値のチェックも

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        at = Player.GetComponent<AttackM>();
        pm = Player.GetComponent<PlayerMove>();
        //スライダーを満タンに
        stSlider.value = 1;
        HpSlider.value = 1;
        //HPなどを最大と同じに
        pStatus.hp = pStatus.maxHp;
        pStatus.stamina = pStatus.maxStamina;
        SetAtk();
        SetMagicAssist();
        SetMagicAtk();
        SetParameter();
        SetSlider();
        hpSl = HpSlider.GetComponent<RectTransform>();
        mpSl = MpSlider.GetComponent<RectTransform>();
        staminaSl = stSlider.GetComponent<RectTransform>();
        SetSlider();
    }

    // Update is called once per frame
    void Update()
    {
       SetSlider();
        stTime += Time.deltaTime;

        if(pStatus.isParalyze || pStatus.isPoison)
        {
            badCondition = true;
        }
        else
        {
            badCondition = false;
        }

        if (stTime >= 0.1f && !pm.isStUse && isEnable)
        {
            //前回のスタミナ回復から0.1秒経っててスタミナ使ってなくてスタミナ回復できるフラグあるなら
            disEnaTime = 0.0f;

            pStatus.stamina += stRecover;
            stTime = 0.0f;
            stBreake = false;

        }




        else if (pStatus.stamina >= pStatus.maxStamina)
        {

            pStatus.stamina = pStatus.maxStamina;
            isEnable = true;
        }

        else if (pStatus.stamina <= 0 && !stBreake)
        {

            disEnaTime += Time.deltaTime;

            isEnable = false;

            if (disEnaTime < 1.5f || pm.isStUse)
            {

                pStatus.stamina = 0;
                //ここ
            }
            else
            {
                stBreake = true;

            }

        }
        else
        {
            isEnable = true;
        }

        stSlider.value = pStatus.stamina / pStatus.maxStamina;
        HpSlider.value = pStatus.hp / pStatus.maxHp;
    }

    public void StaminaUse(int useStamina)
    {
        pStatus.stamina -= useStamina;
    }

    public void HpReduce(float damage)
    {
        pStatus.hp -= damage;
    }
    public void HpRecover(float Recovery)
    {
        pStatus.hp += Recovery;
    }
    public void SetAtk()
    {


        int n = pStatus.equipWeapon.wLevel;

        if (pStatus.equipWeapon.phyBase[n] >= 1)
        {
            pStatus.phyAtk = pStatus.equipWeapon.phyBase[n] + (pStatus.equipWeapon.powerCurve[n].Evaluate(pStatus.power)) +
                               pStatus.equipWeapon.skillCurve[n].Evaluate(pStatus.skill);
            pStatus.Atk += pStatus.phyAtk;
        }
        if (pStatus.equipWeapon.holyBase[n] >= 1)
        {
            pStatus.holyAtk = pStatus.equipWeapon.holyBase[n] + (pStatus.equipWeapon.powerCurve[n].Evaluate(pStatus.power)) +
                               pStatus.equipWeapon.intCurve[n].Evaluate(pStatus._int);
            pStatus.Atk += pStatus.holyAtk;
        }
        if (pStatus.equipWeapon.darkBase[n] >= 1)
        {
            pStatus.darkAtk = pStatus.equipWeapon.darkBase[n] + (pStatus.equipWeapon.intCurve[n].Evaluate(pStatus._int)) +
                               pStatus.equipWeapon.skillCurve[n].Evaluate(pStatus.skill);
            pStatus.Atk += pStatus.darkAtk;
        }
        if (pStatus.equipWeapon.fireBase[n] >= 1)
        {
            pStatus.fireAtk = pStatus.equipWeapon.fireBase[n] + pStatus.equipWeapon.intCurve[n].Evaluate(pStatus._int);
            pStatus.Atk += pStatus.fireAtk;
        }
        if (pStatus.equipWeapon.thunderBase[n] >= 1)
        {
            pStatus.thunderAtk = pStatus.equipWeapon.thunderBase[n] + pStatus.equipWeapon.intCurve[n].Evaluate(pStatus._int);
            pStatus.Atk += pStatus.thunderAtk;
        }


    }//攻撃力設定

    //攻撃力や防御力に倍率かけるメソッドがあっていい。
    //上の設定メソッドは装備が変更された時とレベル上がった時だけでいい。

    public void SetShield()
    {
        //nullの時は素手を装備させる
        if(!pStatus.equipWeapon.twinHand && pStatus.equipShield != null)
        {
            pStatus.phyCut = pStatus.equipShield.phyCut[pStatus.equipShield.sLevel];//カット率
            pStatus.holyCut = pStatus.equipShield.holyCut[pStatus.equipShield.sLevel];//光。
            pStatus.darkCut = pStatus.equipShield.darkCut[pStatus.equipShield.sLevel];//闇。
            pStatus.fireCut = pStatus.equipShield.fireCut[pStatus.equipShield.sLevel];//魔力
            pStatus.thunderCut = pStatus.equipShield.thunderCut[pStatus.equipShield.sLevel];//魔力
            pStatus.guardPower = pStatus.equipShield.guardPower[pStatus.equipShield.sLevel];//受け値
            guardEnable = true;
        }
        else if ((pStatus.equipWeapon.twinHand && pStatus.equipWeapon.shieldAct) || pStatus.equipShield == null)
        {
            pStatus.phyCut = pStatus.equipWeapon.phyCut[pStatus.equipWeapon.wLevel];//カット率
            pStatus.holyCut = pStatus.equipWeapon.holyCut[pStatus.equipWeapon.wLevel];//光。
            pStatus.darkCut = pStatus.equipWeapon.darkCut[pStatus.equipWeapon.wLevel];//闇。
            pStatus.fireCut = pStatus.equipWeapon.fireCut[pStatus.equipWeapon.wLevel];//魔力
            pStatus.thunderCut = pStatus.equipWeapon.thunderCut[pStatus.equipWeapon.wLevel];//魔力
            pStatus.guardPower = pStatus.equipWeapon.guardPower[pStatus.equipWeapon.wLevel];//受け値
            guardEnable = true;
        }
        else if(pStatus.equipWeapon.twinHand && !pStatus.equipWeapon.shieldAct)
        {
            guardEnable = false;
        }

    }

    public void SetParameter()
    {

        //Debug.Log("はいく");
        pStatus.maxHp = pStatus.initialHp + pStatus.HpCurve.Evaluate(pStatus.Vitality);
        pStatus.maxMp = pStatus.initialMp + pStatus.MpCurve.Evaluate(pStatus.capacity);
        pStatus.maxStamina = pStatus.initialStamina + pStatus.StaminaCurve.Evaluate(pStatus.Endurance);
      //  Debug.Log($"テスト数値{pStatus.StaminaCurve.Evaluate(pStatus.Endurance)}");
        pStatus.capacityWeight = pStatus.initialWeight + pStatus.weightCurve.Evaluate(pStatus.Endurance+pStatus.power);

        if(pStatus.capacity >= 0 && pStatus.capacity < 7)
        {
            pStatus.magicNumber = 1;
        }
        else if (pStatus.capacity >= 7)
        {
            pStatus.magicNumber = 2;
        }
        else if (pStatus.capacity >= 15)
        {
            pStatus.magicNumber = 3;
        }
        else if (pStatus.capacity >= 20)
        {
            pStatus.magicNumber = 4;
        }
        else if (pStatus.capacity >= 30)
        {
            pStatus.magicNumber = 5;
        }
        else if (pStatus.capacity >= 38)
        {
            pStatus.magicNumber = 6;
        }
        else if (pStatus.capacity >= 45)
        {
            pStatus.magicNumber = 7;
        }
        pStatus.equipMagic = new List<PlayerMagic>(pStatus.magicNumber);

    }

    public void ActionSet()
    {
        float weightState = pStatus.equipWeight / pStatus.capacityWeight;
        if (weightState <= 0.3 && weightState >= 0)
        {
            pm.speed = pStatus.lightSpeed;
            pm.dashSpeed = pStatus.lightDash;
            pm.avoidRes = pStatus.lightAvoid;
        }
        else if (weightState > 0.3 && weightState <= 0.7)
        {
            pm.speed = pStatus.middleSpeed;
            pm.dashSpeed = pStatus.middleDash;
            pm.avoidRes = pStatus.middleAvoid;
        }
        else if (weightState > 0.7 && weightState <= 1)
        {
            pm.speed = pStatus.heavySpeed;
            pm.dashSpeed = pStatus.heavyDash;
            pm.avoidRes = pStatus.heavyAvoid;
        }
        else if(weightState > 1)
        {
            pm.speed = pStatus.overSpeed;
            pm.dashSpeed = pStatus.overDash;
            pm.avoidRes = pStatus.overAvoid;
        }
    }
    public void SetMagicAssist()
    {
        int n = pStatus.equipWeapon.wLevel;
        pStatus.equipWeapon.MagicAssist = pStatus.equipWeapon.MagicAssistBase + pStatus.equipWeapon.MAssistCurve[n].Evaluate(pStatus._int);

        pStatus.equipWeapon.castSkill = 1;
        pStatus.equipWeapon.castSkill -= pStatus.equipWeapon.CastCurve.Evaluate(pStatus.skill) / 100;
    }
    public void SetMagicAtk()
    {
        if(pStatus.useMagic == null)
        {
            return;
        }
        if (pStatus.useMagic.phyBase >= 1)
        {
            pStatus.useMagic.phyAtk = pStatus.useMagic.phyBase + (pStatus.useMagic.powerCurve.Evaluate(pStatus.power)) +
                               pStatus.useMagic.skillCurve.Evaluate(pStatus.skill);
        }
        if (pStatus.useMagic.holyBase >= 1)
        {
            pStatus.useMagic.holyAtk = pStatus.useMagic.holyBase + (pStatus.useMagic.powerCurve.Evaluate(pStatus.power)) +
                               pStatus.useMagic.intCurve.Evaluate(pStatus._int);
        }
        if (pStatus.useMagic.darkBase >= 1)
        {
            pStatus.useMagic.darkAtk = pStatus.useMagic.darkBase + (pStatus.useMagic.intCurve.Evaluate(pStatus._int)) +
                               pStatus.useMagic.skillCurve.Evaluate(pStatus.skill);
        }
        if (pStatus.useMagic.fireBase >= 1)
        {
            pStatus.useMagic.fireAtk = pStatus.useMagic.fireBase + pStatus.useMagic.intCurve.Evaluate(pStatus._int);
        }
        if (pStatus.useMagic.thunderBase >= 1)
        {
            pStatus.useMagic.thunderAtk = pStatus.useMagic.thunderBase + pStatus.useMagic.intCurve.Evaluate(pStatus._int);

        }
    }
    public void SetSlider()
    {
        if (staminaSl == null)
        {
            return;
        }

        hpSl.offsetMax = new Vector2(-initialHpSl + (pStatus.maxHp - pStatus.initialHp), hpSl.offsetMax.y);
        staminaSl.offsetMax = new Vector2((-initialStaminaSl + (pStatus.maxStamina - pStatus.initialStamina)), staminaSl.offsetMax.y);
        mpSl.offsetMax = new Vector2(-initialMpSl + (pStatus.maxMp - pStatus.initialMp), mpSl.offsetMax.y);
    }



}
