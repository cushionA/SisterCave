using DarkTonic.MasterAudio;
using Rewired;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using MyCode;

public class GManager : MonoBehaviour
{
    public static GManager instance = null;

    public GameObject Player;
    //プレイヤーオブジェクト
    public PlayerStatus pStatus;
    //プレイヤーのステータスを取得
    //セーブデータごとにステータスがある。

    public float stRecover = 1.5f;
    public float additionalStRecover;
    //スタミナ回復量
    public Slider stSlider;
    //スタミナスライダー
    public Slider HpSlider;
    //HPスライダー
    public Slider MpSlider;
    //MPスライダー
    public Slider sisMpSlider;
    //MPスライダー



    /// <summary>
    /// ステータス関連
    /// </summary>
    #region
    public float maxHp = 100;

    //　HP
    public float hp = 100;

    //　最大MP
    public float maxMp = 30;

    //　MP
    public float mp = 30;

    //　最大スタミナ
    public float maxStamina = 60;
    //　スタミナ
    //[HideInInspector]
    public float stamina = 60;

    //生命力
    public float Vitality;
    //持久力
    public float Endurance;
    //MPと魔法もてる数
    public float capacity;
    //　力
    public float power;
    //技量
    public float skill;
    //　魔法力。賢さ
    public float _int;




    //　無属性防御力。体力で上がる
    public float Def = 70;
    //刺突防御。筋力で上がる
    public float pierDef = 70;
    //打撃防御、技量で上がる
    public float strDef = 70;
    //神聖防御、筋と賢さで上がる。
    public float holyDef = 70;
    //闇防御。賢さで上がる
    public float darkDef = 70;
    //炎防御。賢さと生命で上がる
    public float fireDef = 70;
    //雷防御。賢さと持久で上がる。
    public float thunderDef = 70;

    public float equipWeight;//装備してる重量
    public float capacityWeight;//装備可能重量
    public int magicNumber = 1;//装備できる魔法
    //isGuardの時使う

    //小怯み。普段は基本ゼロ。攻撃時だけ
    public float Armor = 1;
    //   [HideInInspector]public float nowArmor;

    public float nockBackPower;

    //大怯み、吹っ飛び。基本均一でアーマー値を足す

    public float attackBuff = 1.0f;
    //攻撃バフ値

    public Weapon equipWeapon;
    public Shield equipShield;
    public CoreItem equipCore;
    //　装備している
    public List<PlayerMagic> equipMagic = null;
    [HideInInspector] public PlayerMagic useMagic;

    /// <summary>
    /// 装備重量で変化する要素
    /// </summary>

    public float lightSpeed;
    public float middleSpeed;
    public float heavySpeed;
    public float overSpeed;

    public float lightDash;
    public float middleDash;
    public float heavyDash;
    public float overDash;

    public float lightAvoid;
    public float middleAvoid;
    public float heavyAvoid;
    public float overAvoid;

    public float lightStRecover;
    public float middleStRecover;
    public float heavyStRecover;
    public float overStRecover;

    #endregion

    [HideInInspector]
    public AttackValue useAtValue;


    public ToolItem[] useList = new ToolItem[7];
    public Weapon[] setWeapon = new Weapon[2];
    public Shield[] setShield = new Shield[2];

    public float initialHpSl;
    public float initialMpSl;
    public float initialStaminaSl;

    public string playerTag = "Player";
    public string guardTag = "Guard";

    [HideInInspector] public Player InputR;
    [HideInInspector] public Rigidbody2D rb;

    RectTransform hpSl;
    RectTransform mpSl;
    RectTransform staminaSl;

    [HideInInspector] public bool isEnable;
    //スタミナが回復するかどうか
    [HideInInspector] public bool isAttack;
    [HideInInspector] public bool airAttack;
    //攻撃中か否か
    [HideInInspector] public bool isDown;
    //ダウン中
    // [HideInInspector] public bool isBlow;
    //if(isBlow && nowArmor<=0)とelseでisBlowFalseに
    [HideInInspector] public bool isWater;
    [HideInInspector] public bool isGBreak;//ガードブレイク
    [HideInInspector] public bool isGuard;
    [HideInInspector] public bool guardDisEnable;
    [HideInInspector] public bool isParry;//ジャスガ
    [HideInInspector] public bool parrySuccess;//パリィ成功
    [HideInInspector] public bool isBounce;//攻撃を跳ね返されたフラグ
    [HideInInspector] public bool onGimmick;//ギミック利用中かどうか
    [HideInInspector] public bool guardHit;//ガードにヒットした
    [HideInInspector] public bool isFalter;
    [HideInInspector] public bool badCondition;//状態異常
    [HideInInspector] public bool isDamage;//ダメージ受けたかどうか
    [HideInInspector] public bool blowDown;//吹き飛ばされたフラグ
    [HideInInspector] public bool fallAttack;//空中強攻撃の落下終了までisAttackをキープするためのフラグ。敵判定回避にも使う

    [HideInInspector] public bool isShieldAttack;
    [HideInInspector] public float nockBack;//ガードした時に下がる数値
    [HideInInspector] public bool  twinHand;//片手かどうか。真なら両手


    /// <summary>
    /// ノックバックやふっとびなどほかの動きをしたいとき
    /// </summary>
    [HideInInspector] public bool anotherMove;

    /// <summary>
    /// プレイヤーに各種魔法陣系エフェクトをつける
    /// </summary>
    public GameObject PlayerEffector;

    //isDown && blowDown の判定とisDownのみとisGBreakの使い分けで
    //具体的処理は敵AIを参考に

    #region//シスターさんのためのフラグ
    [HideInInspector] public bool isLadder;
    [HideInInspector] public bool isRopeJump;
    /// <summary>
    /// トラップ地帯にいるときワープの挙動変わる
    /// </summary>
    [HideInInspector] public bool isTrap;
    /// <summary>
    /// ワープする場所。isTrapの時トラップがtrueになった場所のそばにワープする
    /// </summary>
    [HideInInspector] public float warpPosition;
    #endregion

    [HideInInspector] public Vector2 blowVector;

    float stTime;
    //スタミナが回復する間隔の時間が経過したかどうか
    float disEnaTime;
    //スタミナ回復不能時間
    [HideInInspector] public bool blocking;//ブロッキング
    public AttackM at;
    public PlayerMove pm;
    /// <summary>
    /// stBreakはスタミナが一回切れたらしばらくゼロのままのコードから脱出するためのフラグ。
    /// </summary>
    bool stBreake;
    //スタミナ回復不能状態終わりフラグ
    public bool isArmor;//強靭ついてるかどうか
    float avoidTime;

    //プレイヤーがレベルアップしたらHPとスタミナのスライダーの長さをチェックして伸ばす。
    //あとステータス画面に格納する値のチェックも
    bool isSoundFirst;
    bool statusChange;

    #region//ダウン関連の奴

    float recoverTime;
    float lastArmor;
    bool isAnimeStart;
    [HideInInspector] public float blowTime;
    //  [HideInInspector]public bool isWakeUp;
    // [HideInInspector] public Vector2 blowVector;
    [HideInInspector] public bool isDie;
    [HideInInspector] public bool heavy;
    [HideInInspector] public byte attackType;

    public int avoidLayer;

    #endregion


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
        InputR = ReInput.players.GetPlayer(0);

    }

    void Start()
    {
        
        //       at = Player.GetComponent<AttackM>();
        //     pm = Player.GetComponent<PlayerMove>();
        //スライダーを満タンに
        //HPなどを最大と同じに

        

        // SetSlider();
        initialSetting();
        
        hpSl = HpSlider.GetComponent<RectTransform>();
        mpSl = MpSlider.GetComponent<RectTransform>();
        staminaSl = stSlider.GetComponent<RectTransform>();
        stSlider.value = 1;
        HpSlider.value = hp / maxHp;
        MpSlider.value = mp / maxMp;
        SetSlider();
        if (!isSoundFirst)
        {
            MasterAudio.SetBusVolumeByName("BGM", 0.5f);
            MasterAudio.SetBusVolumeByName("SE", 0.5f);
            //こいつらはゲームの最初に入れるべきでは
            isSoundFirst = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log($"フラグの効果{pm.isAvoid}");
        // DamageAvoid();
        if (statusChange)
        {
            SetSlider();
            statusChange = false;
        }

        stTime += Time.deltaTime;

        if (pStatus.isParalyze || pStatus.isPoison)
        {
            badCondition = true;
        }
        else
        {
            badCondition = false;
        }

        if (stTime >= 0.1f && !pm.isStUse && isEnable && !isDown)
        {
            //前回のスタミナ回復から0.1秒経っててスタミナ使ってなくてスタミナ回復できるフラグあるなら
            disEnaTime = 0.0f;

            stamina += stRecover + additionalStRecover;
            stTime = 0.0f;
            if (stBreake)
            {
                stBreake = false;
            }
           
        }




        if (stamina >= maxStamina)
        {

            stamina = maxStamina;
            isEnable = true;
        }

        else if (stamina <= 0 && !stBreake)
        {

            disEnaTime += Time.deltaTime;

            isEnable = false;
            //スタミナ回復できなくなった。

            if (disEnaTime < 1.5f || pm.isStUse)
            {
                //一定時間0のままかつスタミナ使用状態にしても0のまま。
                stamina = 0;
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

        stSlider.value = stamina / maxStamina;
        HpSlider.value = hp / maxHp;
        MpSlider.value = mp / maxMp;
        sisMpSlider.value = SManager.instance.sisStatus.mp / SManager.instance.sisStatus.maxMp;
    }

    /// <summary>
    /// スタミナ使用
    /// </summary>
    /// <param name="useStamina"></param>
    public void StaminaUse(int useStamina)
    {
        if (stamina >= useStamina)
        {
            stamina -= useStamina;
        }
        else
        {
            stamina = 0;
        }
    }
    /// <summary>
    /// MP回復
    /// </summary>
    /// <param name="recover"></param>
    public void HpRecover(float recover)
    {
        hp += recover;
    }

    /// <summary>
    /// HP削り
    /// </summary>
    /// <param name="damage"></param>
    public void HpReduce(float damage)
    {
        hp -= damage;
    }

    /// <summary>
    /// HP回復
    /// </summary>
    /// <param name="Recovery"></param>
    public void MpRecover(float Recovery)
    {
        mp += Recovery;
    }

    /// <summary>
    /// MP消費
    /// </summary>
    /// <param name="Use"></param>
    public void MpReduce(float UseMp)
    {
        if (mp >= UseMp)
        {
            mp -= UseMp;
        }
        else
        {
            mp = 0;
        }
    }




    public void SetAtk(Equip equip)
    {

        int n = equip.wLevel;

        if (equip.phyBase[n] >= 1)
        {
            equip.phyAtk = equip.phyBase[n] + (equip.powerCurve[n].Evaluate(power)) +
                               equip.skillCurve[n].Evaluate(skill);
            equip.Atk += equip.phyAtk;
        }
        if (equip.holyBase[n] >= 1)
        {
            equip.holyAtk = equip.holyBase[n] + (equip.powerCurve[n].Evaluate(power)) +
                               equip.intCurve[n].Evaluate(_int);
            equip.Atk += equip.holyAtk;
        }
        if (equip.darkBase[n] >= 1)
        {
            equip.darkAtk = equip.darkBase[n] + (equip.intCurve[n].Evaluate(_int)) +
                               equip.skillCurve[n].Evaluate(skill);
            equip.Atk += equip.darkAtk;
        }
        if (equip.fireBase[n] >= 1)
        {
            equip.fireAtk = equip.fireBase[n] + equip.intCurve[n].Evaluate(_int);
            equip.Atk += equip.fireAtk;
        }
        if (equip.thunderBase[n] >= 1)
        {
            equip.thunderAtk = equip.thunderBase[n] + equip.intCurve[n].Evaluate(_int);
            equip.Atk += equip.thunderAtk;
        }


    }//攻撃力設定

    //攻撃力や防御力に倍率かけるメソッドがあっていい。
    //上の設定メソッドは装備が変更された時とレベル上がった時だけでいい。

    public void SetGuard(Equip equip)
    {
        equip.phyCut = equip.phyCutSet[equip.wLevel];//カット率
        equip.holyCut = equip.holyCutSet[equip.wLevel];//光。
        equip.darkCut = equip.darkCutSet[equip.wLevel];//闇。
        equip.fireCut = equip.fireCutSet[equip.wLevel];//魔力
        equip.thunderCut = equip.thunderCutSet[equip.wLevel];//魔力
        equip.guardPower = equip.guardPowerSet[equip.wLevel];//受け値
                                                             //nullの時は素手を装備させる
        /*      if (!equipWeapon.twinHand && equipShield != null)
              {
                  pStatus.phyCut = equipShield.phyCut[equip.wLevel];//カット率
                  pStatus.holyCut = equipShield.holyCut[equip.wLevel];//光。
                  pStatus.darkCut = equipShield.darkCut[equip.wLevel];//闇。
                  pStatus.fireCut = equipShield.fireCut[equip.wLevel];//魔力
                  pStatus.thunderCut = equipShield.thunderCut[equip.wLevel];//魔力
                  pStatus.guardPower = equipShield.guardPower[equip.wLevel];//受け値
                 // guardEnable = true;
              }
              else if (equipWeapon.twinHand && equipShield == null)
              {

                  //guardEnable = true;
              }*/
        //    else if(equipWeapon.twinHand && !equipWeapon.shieldAct)
        //   {
        //        guardEnable = false;
        // }

    }
    /// <summary>
    /// 持久力と
    /// </summary>
    public void SetParameter()
    {

      Vitality = pStatus.Vitality + equipCore.additionalVitality;
    //持久力
      Endurance = pStatus.Endurance + equipCore.additionalEndurance;
  
    //　力
      power = pStatus.power + equipCore.additionalPower;
    //技量
      skill = pStatus.skill + equipCore.additionalSkill;
    //　魔法力。賢さ
      _int = pStatus._int + equipCore.additionalInt;
    capacityWeight = pStatus.initialWeight + pStatus.weightCurve.Evaluate(Endurance + power/2) + equipCore.additionalWeight;
        Armor = equipCore.additionalArmor;
        maxHp = pStatus.initialHp + pStatus.HpCurve.Evaluate(Vitality) + equipCore.additionalHp;
        maxMp = pStatus.initialMp + pStatus.MpCurve.Evaluate(pStatus.capacity) + equipCore.additionalMp;
        maxStamina = pStatus.initialStamina + pStatus.StaminaCurve.Evaluate(Endurance) + equipCore.additionalStamina;
        //  ////Debug.log($"テスト数値{pStatus.StaminaCurve.Evaluate(Endurance)}");




        





        if (pStatus.capacity >= 0 && pStatus.capacity < 7)
        {
            magicNumber = 1;
        }
        else if (pStatus.capacity >= 7)
        {
            magicNumber = 2;
        }
        else if (pStatus.capacity >= 15)
        {
            magicNumber = 3;
        }
        else if (pStatus.capacity >= 20)
        {
            magicNumber = 4;
        }
        else if (pStatus.capacity >= 30)
        {
            magicNumber = 5;
        }
        else if (pStatus.capacity >= 38)
        {
            magicNumber = 6;
        }
        else if (pStatus.capacity >= 45)
        {
            magicNumber = 7;
        }
        if (equipMagic.Count > magicNumber)
        {
            equipMagic.RemoveRange(magicNumber, equipMagic.Count - magicNumber);

        }
        //リストの要素数を再設定
        equipMagic.Capacity = (magicNumber);

    }
    /// <summary>
    /// 装備重量確認と重量状態による変更
    /// </summary>
    public void ActionSet()
    {
        equipWeight = 0;
        equipWeight += equipWeapon._weight;
        equipWeight += equipShield._weight;


        float weightState = equipWeight / capacityWeight;
        if (weightState <= 0.3 && weightState >= 0)
        {
            pStatus.moveSpeed = lightSpeed;
            pStatus.dashSpeed = lightDash;
            pStatus.avoidRes = lightAvoid;
            stRecover = lightStRecover;
            pStatus._weightState = PlayerStatus.PlayerWeightState.軽装備;
        }
        else if (weightState > 0.3 && weightState <= 0.7)
        {

            pStatus.moveSpeed = middleSpeed;
            pStatus.dashSpeed = middleDash;
            pStatus.avoidRes = middleAvoid;
            stRecover = middleStRecover;
            pStatus._weightState = PlayerStatus.PlayerWeightState.通常装備;
        }
        else if (weightState > 0.7 && weightState <= 1)
        {

            pStatus.moveSpeed = heavySpeed;
            pStatus.dashSpeed = heavyDash;
            pStatus.avoidRes = heavyAvoid;
            stRecover = heavyStRecover;
            pStatus._weightState = PlayerStatus.PlayerWeightState.重装備;
        }
        else if (weightState > 1)
        {

            pStatus.moveSpeed = overSpeed;
            pStatus.dashSpeed = overDash;
            pStatus.avoidRes = overAvoid;
            stRecover = overStRecover;
            pStatus._weightState = PlayerStatus.PlayerWeightState.重量オーバー;
        }
    }
    public void SetMagicAssist()
    {
        int n = equipWeapon.wLevel;
        equipWeapon.MagicAssist = equipWeapon.MagicAssistBase + equipWeapon.MAssistCurve[n].Evaluate(_int);

        equipWeapon.castSkill = 1;
        equipWeapon.castSkill -= equipWeapon.CastCurve.Evaluate(skill) / 100;
    }


    public void SetMagicAtk()
    {
        if (useMagic == null)
        {
            return;
        }
        if (useMagic.phyBase >= 1)
        {
            useMagic.phyAtk = useMagic.phyBase + (useMagic.powerCurve.Evaluate(power)) +
                               useMagic.skillCurve.Evaluate(skill);
        }
        if (useMagic.holyBase >= 1)
        {
            useMagic.holyAtk = useMagic.holyBase + (useMagic.powerCurve.Evaluate(power)) +
                               useMagic.intCurve.Evaluate(_int);
        }
        if (useMagic.darkBase >= 1)
        {
            useMagic.darkAtk = useMagic.darkBase + (useMagic.intCurve.Evaluate(_int)) +
                               useMagic.skillCurve.Evaluate(skill);
        }
        if (useMagic.fireBase >= 1)
        {
            useMagic.fireAtk = useMagic.fireBase + useMagic.intCurve.Evaluate(_int);
        }
        if (useMagic.thunderBase >= 1)
        {
            useMagic.thunderAtk = useMagic.thunderBase + useMagic.intCurve.Evaluate(_int);

        }
    }





    public void SetSlider()
    {


        // initialHpSl = 
        Vector2 sliderRength = Vector2.zero;
        sliderRength.Set(-initialHpSl + (maxHp - pStatus.initialHp), hpSl.offsetMax.y);
        hpSl.offsetMax = sliderRength;
        sliderRength.Set(-initialStaminaSl + (maxStamina - pStatus.initialStamina), staminaSl.offsetMax.y);
        staminaSl.offsetMax = sliderRength;
        sliderRength.Set(-initialMpSl + (maxMp - pStatus.initialMp), mpSl.offsetMax.y);
        mpSl.offsetMax = sliderRength;
    }

    /// <summary>
    /// 何度も当たり判定が検出されるのを防ぐためのもの
    /// </summary>
    public void DamageAvoid()
    {
        if (isDamage)
        {
            avoidTime += Time.fixedDeltaTime;
            pm.SetLayer(10);
            if (avoidTime >= 0.1)
            {
                isDamage = false;
                avoidTime = 0;
                pm.SetLayer(11);
            }
        }
    }

    public void HPReset()
    {
        hp = pStatus.maxHp;
    }





    /*	public void EnemyArmorControll()
        {
            if (GManager.instance.nowArmor <= 0 && pStatus.isBlow)
            {
                GManager.instance.isDown = true;
                GManager.instance.blowDown = true;
            }
            else
            {
                GManager.instance.isDown = false;
            }
        }*/

    /// <summary>
    /// ノックバックする。
    /// </summary>
    public void NockBack()
    {
        if ((blowDown || isGBreak) && isFalter)
        {
            isFalter = false;
            isAnimeStart = false;
        }
        if (isDown && !blowDown && isFalter)
        {
            if (!isAnimeStart)
            {
                //isFalter = true;
                isAnimeStart = true;
                pm.anim.Play("TLongSwordFalter");
            }
            else if (!CheckEnd("TLongSwordFalter"))
            {
                isDown = false;
                isAnimeStart = false;
                isFalter = false;
                

            }


        }
    }
    /// <summary>
    /// 攻撃をはじかれノックバックする。
    /// </summary>
    public void Parry()
    {
        if (isBounce && (blowDown || isFalter))
        {
            isBounce = false;
            isAnimeStart = false;



        }
        if (isBounce && !blowDown)
        {
            if (!isAnimeStart)
            {
                isAttack = false;
                isDown = true;
                //isFalter = true;
                isAnimeStart = true;
                pm.anim.Play("Bounce");
                //Debug.Log("弾かれ");
            }
            else if (!CheckEnd("Bounce"))
            {
                isDown = false;
                isAnimeStart = false;
                isBounce = false;
            }
        }
    }
    public void Blow()
    {

        if (blowDown && isDown)
        {
            //  pm.rb.gravityScale = 20;
            blowTime += Time.fixedDeltaTime;
            if (blowTime <= 0.2)
            {
                pm.rb.gravityScale = 40;
                // Debug.Log($"{blowTime}");
                if (blowTime <= 0.1)
                {
                    pm.isGround = false;
                }
                pm.rb.AddForce(blowVector, ForceMode2D.Impulse);
            }
            else if (blowTime <= 1.5)// || !isGround)
            {
                //こいつ重力ないわ
                //Debug.Log($"速度{blowVector.y}");
                blowVector.Set(blowVector.x, 0);
                pm.rb.AddForce(blowVector, ForceMode2D.Impulse);

            }

        }
    }

    ///<summary>
    ///ダウン解除
    ///</summary>
    public void DownDel()
    {
        if (GManager.instance.blowDown)
        {

            GManager.instance.blowDown = false;
            GManager.instance.isDown = false;
            pm.SetLayer(11);
            //	isAnimeStart = false;
            pm.isStop = false;
            pm.isWakeUp = false;
            GManager.instance.blowTime = 0;
            isAnimeStart = false;
        }
    }

    /// <sammary>
    /// ダウン状態のメソッド
    /// </sammary>
    public void Down()
    {
        //////Debug.log("吹き飛ぶ");

        if (blowDown && isDown)
        {
            if (!pm.isWakeUp)
            {
                if (!pm.isGround)
                {
                    pm.anim.Play("TLongSwordBlow");
                    //SetLayer(17);//回避レイヤーの場所変わってるかもな。追加や削除で
                    //GravitySet(15);

                }
                else if (pm.isGround)
                {
                    pm.rb.gravityScale = 0;
                    pm.rb.velocity = Vector2.zero;
                    if (!isAnimeStart)
                    {
                            GManager.instance.PlaySound(SoundManager.instance.downSound[0], transform.position);

                        pm.anim.Play("TLongSwordDown");

                        isAnimeStart = true;
                    }
                    //GravitySet(pStatus.firstGravity);
                    else if (!CheckEnd("TLongSwordDown"))
                    {
                        if (isDie)
                        {
                            isDown = false;
                            isAnimeStart = false;
                            return;
                        }
                        pm.AllStop(0.7f,true);
                        //Debug.Log("醤油");

                    }

                    //isDown = false;
                    //ダウンアニメ
                }
            }
            else if (isDown && pm.isGround && pm.isWakeUp)
            {
                //前のでisAnimeStartが真のままだから
                if (isAnimeStart)
                {
                    isAnimeStart = false;
                    pm.anim.Play("TLongSwordWakeup");
                }
                else if (!CheckEnd("TLongSwordWakeup"))
                {
                    pm.SetLayer(11);
                    //	isAnimeStart = false;
                    isDown = false;
                    pm.isWakeUp = false;
                    blowDown = false;
                    blowTime = 0;
                    
                }

            }

        }
    }
    /*    /// <summary>
        /// 改修しようね
        /// 全部まとめるか否か
        /// </summary>
        /// <returns></returns>
        async UniTaskVoid SetPlayerData()
        {
            //仮の姿
            await UniTask.RunOnThreadPool(()=> SetAtk());

        }*/

    /*       /// <summary>
           /// 音声再生。音源に追随しない
           /// 普通は再生する音声と
           /// </summary>
           /// <param name="sType">再生する音の名前。バリエーションありのやつ。
           /// <param name="sourcePosition">音を鳴らしたい位置。必須です。 </param>
           /// <param name="volumePercentage"><b>Optional</b> - 音量を下げて再生したい場合に使用します（0～1の間）。
           /// <param name="pitch"><b>Optional</b> - 特定のピッチで音を再生したい場合に使用します。</param> <param name="pitch"><b>Optional</b> - 特定の音程で再生したい場合に使用します。そうすると、バリエーションの中のpichとrandom pitchを上書きします。
           /// <param name="delaySoundTime"><b>Optional</b> - すぐにではなく、X秒後に音を鳴らしたい場合に使用します。
           /// <param name="variationName"><b>Optional</b> - 特定のバリエーション（またはクリップID）の名前で再生したい場合に使用します。それ以外の場合は、ランダムなバリエーションが再生されます。
           /// <param name="timeToSchedulePlay"><b>Optional</b> - サウンドを再生するためのDSP時間を渡すために使用します。通常はこれを使用せず、代わりにdelaySoundTimeパラメータを使用します。
           /// <param name="isRemember"><b>Optional</b> - PlaySoundResultを取得するかどうか。
           /// <returns>PlaySoundResult - このオブジェクトは、サウンドが再生されたかどうかを読み取るために使用され、使用されたVariationオブジェクトへのアクセスも可能です。

           public async UniTaskVoid PlaySoundAsync(string sType, Vector3 sourceTrans, float volumePercentage= 1f, float? pitch= null, float delaySoundTime= 0f, string variationName = null, double? timeToSchedulePlay = null,bool isRemember = false)
           {

               if (isRemember)
               {
                   MasterAudio.PlaySound3DAtVector3(sType, sourceTrans, volumePercentage, pitch, delaySoundTime, variationName, timeToSchedulePlay);
               }
               else
               {
                   MasterAudio.PlaySound3DAtVector3AndForget(sType, sourceTrans, volumePercentage, pitch, delaySoundTime, variationName, timeToSchedulePlay);

               }

           }*/


    /// <summary>
    /// 音声再生。音源に追随しない
    /// 普通は再生する音声と
    /// </summary>
    /// <param name="sType">再生する音の名前。バリエーションありのやつ。
    /// <param name="sourcePosition">音を鳴らしたい位置。必須です。 </param>
    /// <param name="volumePercentage"><b>Optional</b> - 音量を下げて再生したい場合に使用します（0～1の間）。
    /// <param name="pitch"><b>Optional</b> - 特定のピッチで音を再生したい場合に使用します。</param> <param name="pitch"><b>Optional</b> - 特定の音程で再生したい場合に使用します。そうすると、バリエーションの中のpichとrandom pitchを上書きします。
    /// <param name="delaySoundTime"><b>Optional</b> - すぐにではなく、X秒後に音を鳴らしたい場合に使用します。
    /// <param name="variationName"><b>Optional</b> - 特定のバリエーション（またはクリップID）の名前で再生したい場合に使用します。それ以外の場合は、ランダムなバリエーションが再生されます。
    /// <param name="timeToSchedulePlay"><b>Optional</b> - サウンドを再生するためのDSP時間を渡すために使用します。通常はこれを使用せず、代わりにdelaySoundTimeパラメータを使用します。
    /// <param name="isRemember"><b>Optional</b> - PlaySoundResultを取得するかどうか。
    /// <returns>PlaySoundResult - このオブジェクトは、サウンドが再生されたかどうかを読み取るために使用され、使用されたVariationオブジェクトへのアクセスも可能です。

    public void PlaySound(string sType, Vector3 sourceTrans, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null, double? timeToSchedulePlay = null, bool isRemember = false)
    {

        if (isRemember)
        {
            MasterAudio.PlaySound3DAtVector3(sType, sourceTrans, volumePercentage, pitch, delaySoundTime, variationName, timeToSchedulePlay);
        }
        else
        {
            MasterAudio.PlaySound3DAtVector3AndForget(sType, sourceTrans, volumePercentage, pitch, delaySoundTime, variationName, timeToSchedulePlay);
        }

    }
    /// <summary>
    /// 音声再生。音源に追随する
    /// </summary>
    /// <param name="sType">再生する音の名前。バリエーションありのやつ。
    /// <param name="sourcePosition">音を鳴らしたい位置。必須です。 </param>
    /// <param name="volumePercentage"><b>Optional</b> - 音量を下げて再生したい場合に使用します（0～1の間）。
    /// <param name="pitch"><b>Optional</b> - 特定のピッチで音を再生したい場合に使用します。</param> <param name="pitch"><b>Optional</b> - 特定の音程で再生したい場合に使用します。そうすると、バリエーションの中のpichとrandom pitchを上書きします。
    /// <param name="delaySoundTime"><b>Optional</b> - すぐにではなく、X秒後に音を鳴らしたい場合に使用します。
    /// <param name="variationName"><b>Optional</b> - 特定のバリエーション（またはクリップID）の名前で再生したい場合に使用します。それ以外の場合は、ランダムなバリエーションが再生されます。
    /// <param name="timeToSchedulePlay"><b>Optional</b> - サウンドを再生するためのDSP時間を渡すために使用します。通常はこれを使用せず、代わりにdelaySoundTimeパラメータを使用します。
    ///     /// <param name="isRemember"><b>Optional</b> - PlaySoundResultを取得するかどうか。
    /// <returns>PlaySoundResult - このオブジェクトは、サウンドが再生されたかどうかを読み取るために使用され、使用されたVariationオブジェクトへのアクセスも可能です。
    public void FollowSound(string sType, Transform sourceTrans, float volumePercentage = 1f, float? pitch = null, float delaySoundTime = 0f, string variationName = null, bool isRemember = false)
    {

        if (isRemember)
        {
            MasterAudio.PlaySound3DFollowTransform(sType, sourceTrans, volumePercentage, pitch, delaySoundTime, variationName);
        }
        else
        {
            MasterAudio.PlaySound3DFollowTransformAndForget(sType, sourceTrans, volumePercentage, pitch, delaySoundTime, variationName);
        }
    }

    /*   /// <summary>
       /// 音声再生。音源に追随する
       /// </summary>
       /// <param name="sType">再生する音の名前。バリエーションありのやつ。
       /// <param name="sourcePosition">音を鳴らしたい位置。必須です。 </param>
       /// <param name="volumePercentage"><b>Optional</b> - 音量を下げて再生したい場合に使用します（0～1の間）。
       /// <param name="pitch"><b>Optional</b> - 特定のピッチで音を再生したい場合に使用します。</param> <param name="pitch"><b>Optional</b> - 特定の音程で再生したい場合に使用します。そうすると、バリエーションの中のpichとrandom pitchを上書きします。
       /// <param name="delaySoundTime"><b>Optional</b> - すぐにではなく、X秒後に音を鳴らしたい場合に使用します。
       /// <param name="variationName"><b>Optional</b> - 特定のバリエーション（またはクリップID）の名前で再生したい場合に使用します。それ以外の場合は、ランダムなバリエーションが再生されます。
       /// <param name="timeToSchedulePlay"><b>Optional</b> - サウンドを再生するためのDSP時間を渡すために使用します。通常はこれを使用せず、代わりにdelaySoundTimeパラメータを使用します。
       ///     /// <param name="isRemember"><b>Optional</b> - PlaySoundResultを取得するかどうか。
       /// <returns>PlaySoundResult - このオブジェクトは、サウンドが再生されたかどうかを読み取るために使用され、使用されたVariationオブジェクトへのアクセスも可能です。
       public async UniTaskVoid FollowSoundAsync(string sType, Transform sourceTrans, float volumePercentage, float? pitch, float delaySoundTime, string variationName, bool isRemember = false)
       {

           if (isRemember)
           {
               await UniTask.RunOnThreadPool(() => MasterAudio.PlaySound3DFollowTransform(sType, sourceTrans, volumePercentage, pitch, delaySoundTime, variationName));
           }
           else
           {
               await UniTask.RunOnThreadPool(() => MasterAudio.PlaySound3DFollowTransformAndForget(sType, sourceTrans, volumePercentage, pitch, delaySoundTime, variationName));
           }
       }*/


    /// <summary>
    /// 最後のtrueにしないならフェードが基本
    /// </summary>
    /// <param name="soundGroupName"></param>
    /// <param name="fadeTime"></param>
    /// <param name="isStop"></param>
    public void StopSound(string soundGroupName, float fadeTime = 1,bool isStop = false)
    {
        if (!isStop)
        {
            MasterAudio.FadeOutAllOfSound(soundGroupName, fadeTime);
        }
        else
        {
            MasterAudio.StopAllOfSound(soundGroupName);
        }
    }

    /// <summary>
    /// 体力やMP設定
    /// </summary>
    public void initialSetting()
    {

        hp = maxHp;
        mp = maxMp;
        stamina = maxStamina;
    }

    // public
    /// <summary>
    /// エンチャ時はエンチャントタイプを参照
    /// </summary>
    /// <param name="damageType"></param>
    public void DamageSound(byte damageType)
    {
        if (damageType == 1)
        {
            GManager.instance.PlaySound("SlashDamage", transform.position);
        }
        else if (damageType == 2)
        {
            GManager.instance.PlaySound("StabDamage", transform.position);
        }
        else if (damageType == 4)
        {
            if (!heavy)
            {
              //  Debug.Log("チキン");
                GManager.instance.PlaySound("StrikeDamage", transform.position);
            }
            else
            {
                GManager.instance.PlaySound("HeavyStrikeDamage", transform.position);
                heavy = false;
            }
        }
        else if (damageType == 8)
        {
            GManager.instance.PlaySound("HolyDamage", transform.position);
        }
        else if (damageType == 16)
        {
            GManager.instance.PlaySound("DarkDamage", transform.position);
        }
        else if (damageType == 32)
        {
            GManager.instance.PlaySound("FireDamage", transform.position);
        }
        else if (damageType == 64)
        {
            GManager.instance.PlaySound("ThunderDamage", transform.position);
        }
    }
    /// <summary>
    /// アニメが終了したかどうか
    /// </summary>
    /// <param name="Name"></param>
    /// <returns></returns>
    bool CheckEnd(string Name)
    {

        if (!pm.anim.GetCurrentAnimatorStateInfo(0).IsName(Name))// || sAni.GetCurrentAnimatorStateInfo(0).IsName("OStand"))
        {   // ここに到達直後はnormalizedTimeが"Default"の経過時間を拾ってしまうので、Resultに遷移完了するまではreturnする。
            return true;
        }
        if (pm.anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {   // 待機時間を作りたいならば、ここの値を大きくする。
            return true;
        }
        //AnimatorClipInfo[] clipInfo = sAni.GetCurrentAnimatorClipInfo(0);

        ////Debug.Log($"アニメ終了");

        return false;

        // return !(sAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        //  (_currentStateName);
    }
}
