using DarkTonic.MasterAudio;
using Rewired;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using MyCode;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;

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

    public MyCode.Weapon equipWeapon;
    public Shield equipShield;
    public CoreItem equipCore;
    //　装備している
    public List<PlayerMagic> equipMagic = null;
    [HideInInspector] public PlayerMagic useMagic;

    #endregion

    /// <summary>
    /// 装備重量で変化する要素
    /// </summary>
    #region
    [SerializeField] float lightSpeed;
    [SerializeField] float middleSpeed;
    [SerializeField] float heavySpeed;
    [SerializeField] float overSpeed;

    [SerializeField] float lightDash;
    [SerializeField] float middleDash;
    [SerializeField] float heavyDash;
    [SerializeField] float overDash;

    [SerializeField] float lightAvoid;
    [SerializeField] float middleAvoid;
    [SerializeField] float heavyAvoid;
    [SerializeField] float overAvoid;



    [SerializeField] float lightStRecover;
    [SerializeField] float middleStRecover;
    [SerializeField] float heavyStRecover;
    [SerializeField] float overStRecover;

    /// <summary>
    /// 以下は装備重量によるスタミナ使用量の倍率
    /// </summary>
    [SerializeField] float lightStRatio;
    [SerializeField] float middleStRatio;
    [SerializeField] float heavyStRatio;
    [SerializeField] float overStRatio;

    /// <summary>
    /// スタミナの使用倍率
    /// </summary>
    [HideInInspector]
    public float stRatio;

    /// <summary>
    /// 各共通アクションで使うスタミナ
    /// </summary>
    public float dashSt;
    public float rollSt;
    public float jumpSt;
   // public float 
    #endregion

    [HideInInspector]
    public AttackValue useAtValue;

    /// <summary>
    /// 装備とアニメーション管理関連
    /// </summary>
    #region

    public MyCode.Weapon[] setWeapon = new MyCode.Weapon[2];
    public Shield[] setShield = new Shield[2];

    /// <summary>
    /// 使用しているのはどちらの武器か
    /// </summary>
    int weaponNum = 1;

    /// <summary>
    /// 使用しているのはどちらの盾か
    /// </summary>
    int shieldNum = 1;

    #endregion
    public ToolItem[] useList = new ToolItem[7];


    public float initialHpSl;
    public float initialMpSl;
    public float initialStaminaSl;

    public string playerTag = "Player";
    public string guardTag = "Guard";

    [HideInInspector] public Player InputR;
    [HideInInspector] public Rigidbody2D rb;

    /// <summary>
    /// 状態管理用のフラグ
    /// </summary>
    #region

    RectTransform hpSl;
    RectTransform mpSl;
    RectTransform staminaSl;

    /// <summary>
    /// スタミナが回復するかどうか
    /// これが不能ならスタミナゼロ以下
    /// なのでこれが真ならアビリティ有効に
    /// </summary>
    [HideInInspector] public bool isEnable;
    
    [HideInInspector] public bool isAttack;

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


    public int shit;

    //スタミナが回復する間隔の時間が経過したかどうか
    float disEnaTime;
    //スタミナ回復不能時間
    [HideInInspector] public bool blocking;//ブロッキング
    
    /// <summary>
    /// stBreakはスタミナが一回切れたらしばらくゼロのままのコードから脱出するためのフラグ。
    /// </summary>
    bool stBreake;
    //スタミナ回復不能状態終わりフラグ
    public bool isArmor;//強靭ついてるかどうか
    float stTime;

/// <summary>
/// アニメーター
/// </summary>
    Animator _anim;

    /// <summary>
    /// 真ならスタミナ使用中
    /// </summary>
    [HideInInspector]
    public bool isStUse;

    #endregion


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

    /// <summary>
    /// 外部コンポーネント
    /// </summary>
    #region
    public AttackM at;
    public PlayerMove pm;
    public PlyerController pc;
    #endregion



    //プレイヤーがレベルアップしたらHPとスタミナのスライダーの長さをチェックして伸ばす。
    //あとステータス画面に格納する値のチェックも
    bool isSoundFirst;
    bool statusChange;

    #region//ダウン関連の奴

    bool isAnimeStart;
    [HideInInspector] public float blowTime;
    //  [HideInInspector]public bool isWakeUp;
    // [HideInInspector] public Vector2 blowVector;
    [HideInInspector] public bool isDie;
    [HideInInspector] public bool heavy;
    [HideInInspector] public byte attackType;

    public int avoidLayer = 15;

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
        

    }

    void Start()
    {
        
        //       at = Player.GetComponent<AttackM>();
        //     pm = Player.GetComponent<PlayerMove>();
        //スライダーを満タンに
        //HPなどを最大と同じに

        InputR = ReInput.players.GetPlayer(0);

        // SetSlider();
        initialSetting();

        _anim = Player.GetComponent<Animator>();

        //     myAnimatorOverride = new AnimatorOverrideController(_anim.runtimeAnimatorController);
        //   _anim.runtimeAnimatorController = myAnimatorOverride.runtimeAnimatorController;

      //  hp = pc.ReturnHealth();

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
       // Debug.Log($"フラグの効果{pc.isAvoid}");
        // DamageAvoid();
        if (statusChange)
        {
            SetSlider();
            statusChange = false;
        }

        //状態異常
        if (pStatus.isParalyze || pStatus.isPoison)
        {
            badCondition = true;
        }
        else
        {
            badCondition = false;
        }

        StaminaRecover();
        hp = pc.ReturnHealth();

        stSlider.value = stamina / maxStamina;
        HpSlider.value = hp / maxHp;
        MpSlider.value = mp / maxMp;
      //そのうちエンジンに適合させる
        //  sisMpSlider.value =  / SManager.instance.sisStatus.maxMp;
    }

    ///<summary>
    ///　ステータス設定関連のメソッド
    /// </summary>
    #region

    /// <summary>
    /// 開始時に行う設定
    /// </summary>
    public void initialSetting()
    {

        StatusSetting();
    }
    /// <summary>
    /// ステータスと魔法の記憶数をセット
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
    /// プレイヤーと武器のステータスをセットする
    /// コアと装備の合致も確認？
    /// </summary>
    public void StatusSetting()
    {
        //すべて変更された時
        //最初にやるやつ

        SetParameter();
        ActionSet();
        if (setWeapon[0] != null)
        {
            SetAtk(setWeapon[0]);
            SetGuard(setWeapon[0]);
        }
        if (setWeapon[1] != null)
        {
            SetAtk(setWeapon[1]);
            SetGuard(setWeapon[1]);
        }
        if (setShield[0] != null)
        {
            SetAtk(setShield[0]);
            SetGuard(setShield[0]);
        }
        if (setShield[1] != null)
        {
            SetAtk(setShield[1]);
            SetGuard(setShield[1]);
        }

        SetMagicAssist();
        SetMagicAtk();

    }

    /// <summary>
    /// 引数の装備の攻撃性能を設定する。
    /// このメソッドは装備が変更された時と武器レベル上がった時以外も呼ぶ。
    /// コアや魔法やアイテムでステータスが上がったりするから。
    /// </summary>
    /// <param name="equip"></param>
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


    /// <summary>
    /// 引数の装備のガード時の性能を設定する。
    /// このメソッドは装備が変更された時と武器レベル上がった時だけでいい。
    /// </summary>
    /// <param name="equip"></param>
    public void SetGuard(Equip equip)
    {
        equip.phyCut = equip.phyCutSet[equip.wLevel];//カット率
        equip.holyCut = equip.holyCutSet[equip.wLevel];//光。
        equip.darkCut = equip.darkCutSet[equip.wLevel];//闇。
        equip.fireCut = equip.fireCutSet[equip.wLevel];//魔力
        equip.thunderCut = equip.thunderCutSet[equip.wLevel];//魔力
        equip.guardPower = equip.guardPowerSet[equip.wLevel];//受け値
                                                             //nullの時は素手を装備させる

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
            stRatio = lightStRatio;
            pStatus._weightState = PlayerStatus.PlayerWeightState.軽装備;
        }
        else if (weightState > 0.3 && weightState <= 0.7)
        {
            
            pStatus.moveSpeed = middleSpeed;
            pStatus.dashSpeed = middleDash;
            pStatus.avoidRes = middleAvoid;
            stRecover = middleStRecover;
            stRatio = middleStRatio;
            pStatus._weightState = PlayerStatus.PlayerWeightState.通常装備;
        }
        else if (weightState > 0.7 && weightState <= 1)
        {

            pStatus.moveSpeed = heavySpeed;
            pStatus.dashSpeed = heavyDash;
            pStatus.avoidRes = heavyAvoid;
            stRecover = heavyStRecover;
            stRatio = heavyStRatio;
            pStatus._weightState = PlayerStatus.PlayerWeightState.重装備;
        }
        else if (weightState > 1)
        {

            pStatus.moveSpeed = overSpeed;
            pStatus.dashSpeed = overDash;
            pStatus.avoidRes = overAvoid;
            stRecover = overStRecover;
            stRatio = overStRatio;
            pStatus._weightState = PlayerStatus.PlayerWeightState.重量オーバー;
        }
    }


    //魔法威力修正の設定
    public void SetMagicAssist()
    {
        int n = equipWeapon.wLevel;
        equipWeapon.MagicAssist = equipWeapon.MagicAssistBase + equipWeapon.MAssistCurve[n].Evaluate(_int);

        equipWeapon.castSkill = 1;
        equipWeapon.castSkill -= equipWeapon.CastCurve.Evaluate(skill) / 100;
    }

    /// <summary>
    /// 魔法関連の設定
    /// 魔法そのものの攻撃力か
    /// </summary>
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


    #endregion

    ///<summary>
    ///　HPなどのパラメータ変更関連のメソッド
    /// </summary>
    #region
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
        isStUse = true;

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

    public void StaminaRecover()
    {
        //スタミナの回復の基準になる時間測定
        stTime += Time.deltaTime;



        if (stTime >= 0.1f && !isStUse && isEnable)
        {
            //前回のスタミナ回復から0.1秒経ってて、スタミナ使用中ではなく、ダウンもしてなくてスタミナ回復できるフラグあるなら

            //スタミナ回復不能時間を計測する変数を０にリセット
            disEnaTime = 0.0f;
            //ガード時以外のスタミナ管理
            if (!isGuard)
            {
                stamina += stRecover + additionalStRecover;
            }

            //ガード中は回復量半減。盾の種類で変えていいかも
            else
            {
                stamina += Mathf.Ceil(stRecover + additionalStRecover) / 2;
            }

            //スタミナ回復時間をリセットして再び時間計測
            stTime = 0.0f;

            //スタミナ０でスタミナ破壊状態を解除
            if (stBreake)
            {
                stBreake = false;
            }

        }



        //スタミナが上限超えたら上限に戻す
        if (stamina >= maxStamina)
        {

            stamina = maxStamina;
            isEnable = true;
        }

        //スタミナがゼロ、かつスタミナ破壊されてないなら
        //スタミナ破壊の処理をする
        else if (stamina <= 0 && !stBreake)
        {
            //スタミナ機能不全の時間計測
            disEnaTime += Time.deltaTime;

            //スタミナ回復できなくなる。
            isEnable = false;

            //スタミナ
            if (disEnaTime < 1.5f || isStUse)
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
    }


#endregion



    
    ///<summary>
    ///　装備変更関連のメソッド
    /// </summary>
    #region

    /// <summary>
    /// 装備交換時のアニメーション管理とEquipWeapon入れ替えとステータス変更
    /// スロット空の場合は素手になる
    /// </summary>
    /// <param name="setNumber"></param>
    public void EquipSwap(int setNumber)
    {
        //両手/片手持ち変更
        //装備自体は変わらない
        if (setNumber == 0)
        {
            //両手持ちに
            if (!twinHand)
            {
                twinHand = true;
               // Debug.Log($"asidk{_anim.runtimeAnimatorController.name}");
                _anim.runtimeAnimatorController = equipWeapon._useContoroller[1];
            }
            //片手持ちに
            else
            {
                twinHand = false;
                _anim.runtimeAnimatorController = equipWeapon._useContoroller[0];
                AnimationSetting();
            }
        }

        //武器のスロット間での装備入れ替え
        else if (setNumber == 1)
        {
            if (weaponNum == 1)
            {
                weaponNum = 2;
                equipWeapon = setWeapon[1];
            }
            else
            {
                weaponNum = 1;
                equipWeapon = setWeapon[0];
            }
            _anim.runtimeAnimatorController = equipWeapon._useContoroller[0];
            twinHand = false;
            EquipChangedSetting();
            AnimationSetting();
        }
        //盾のスロット間での装備入れ替え
        else if (setNumber == 2)
        {
            if (weaponNum == 1)
            {
                shieldNum = 2;
                equipShield = setShield[1];
            }
            else
            {
                shieldNum = 1;
                equipShield = setShield[0];

            }
            //武器を片手持ちに
            _anim.runtimeAnimatorController = equipWeapon._useContoroller[0];
            twinHand = false;
            EquipChangedSetting();
            AnimationSetting();
        }
    }

    /// <summary>
    /// 武器と盾の変更メソッド
    /// 武器を変えた後はまずコアと武器の相性チェック、アニメーションコントローラーを入れ替えて
    /// アニメーションをセットしたあとステータスを設定
    /// </summary>
    /// <param name="setNumber"></param>
    /// <param name="slotNumber"></param>
    /// <param name="_equip"></param>
    public void EquipChange(int setNumber, int slotNumber, Equip _equip)
    {
           //普通に装備変更、武器
        if (setNumber == 1)
        {
            //引数の装備を武器として格納
            MyCode.Weapon _weapon = (MyCode.Weapon)_equip;

            //一つ目のスロットに新しい武器を詰める
            if (slotNumber == 1)
            {
                setWeapon[0] = _weapon;

                //そして現在装備中の武器が第一スロットにあるものなら装備武器も変更
                // 武器も片手持ちにしてアニメーションの設定も変える
                if (weaponNum == 1)
                {
                    equipWeapon = setWeapon[0];
                    //コア確認
                    CoreConfirm();
                    
                    twinHand = false;
                    _anim.runtimeAnimatorController = equipWeapon._useContoroller[0];
                    AnimationSetting();
                    EquipChangedSetting();

                }
                //変更した武器の分だけステータス設定
                else
                {
                    SetAtk(setWeapon[0]);
                    SetGuard(setWeapon[0]);
                }
            }
            else
            {
                setWeapon[1] = _weapon;

                //そして現在装備中の武器が第二スロットにあるものなら装備武器も変更
                // 武器も片手持ちにしてアニメーションの設定も変える
                if (weaponNum == 2)
                {
                    equipWeapon = setWeapon[1];
                    //コア確認
                    CoreConfirm();
                    twinHand = false;
                    _anim.runtimeAnimatorController = equipWeapon._useContoroller[0];
                    AnimationSetting();
                    EquipChangedSetting();
                }
                //変更した武器の分だけステータス設定
                else
                {

                    SetAtk(setWeapon[1]);
                    SetGuard(setWeapon[1]);
                }
            }
        }
        //盾変更
        else if (setNumber == 2)
        {
            //引数の装備を盾として格納
            Shield _shield = (Shield)_equip;

            //一つ目のスロットに新しい武器を詰める
            if (slotNumber == 1)
            {
                setShield[0] = _shield;

                //そして現在装備中の盾が第一スロットにあるものなら装備も変更
                // 武器も片手持ちにしてアニメーションの設定も変える
                if (shieldNum == 1)
                {
                    equipShield = setShield[0];
                    twinHand = false;
                    _anim.runtimeAnimatorController = equipWeapon._useContoroller[0];
                }
                else
                {

                    SetAtk(setShield[0]);
                    SetGuard(setShield[0]);
                }
            }
            else
            {
                setShield[1] = _shield;

                //そして現在装備中の盾が第一スロットにあるものなら装備も変更
                // 武器も片手持ちにしてアニメーションの設定も変える
                if (shieldNum == 2)
                {
                    equipShield = setShield[1];
                    twinHand = false;
                    _anim.runtimeAnimatorController = equipWeapon._useContoroller[0];
                }
                else
                {

                    SetAtk(setShield[1]);
                    SetGuard(setShield[1]);
                }
            }
        }

    }

    /// <summary>
    /// コアの変更メソッド
    /// コアを変えた後はまずコアと武器の相性チェック、アニメーションコントローラーを入れ替えて
    /// アニメーションをセットしたあとステータスを設定
    /// </summary>
    /// <param name="_core"></param>
    public void CoreChange(CoreItem _core)
    {
        equipCore = _core;
        if (CoreConfirm())
        {
            _anim.runtimeAnimatorController = equipWeapon._useContoroller[0];
            AnimationSetting();
        }
        EquipChangedSetting();
    }

    /// <summary>
    /// コアと装備の特殊相性を確認
    /// </summary>
    public bool CoreConfirm()
    {
        if (equipWeapon.ExCore == equipCore)
        {
            equipWeapon = equipWeapon._alterWeapon;
            //コア合致によりもたらされる効果を記述

            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// 装備変更時の処理
    /// 盾戦技のアニメの入れ替えやステータスの再設定、コアの合致確認まで
    /// </summary>
    public void EquipChangedSetting()
    {
        //まずステータスを再セッティングする
        StatusSetting();
        pc.ParameterSet(pStatus);
    }

    /// <summary>
    /// 片手アニメのコントローラーにアニメーションを装備変更に合わせてセットする
    /// AnimatorOverrideControllerはAnimatorと基本的に同じ
    /// RuntimeAnimatorControllerを武器のモノ（片手）に差し替える
    /// そしてその中に盾の戦技を仕込む
    /// 戦技はクリップの最初の五つとか(配列の0～4)、それか.animationClips[n].nameでArts0とかで照会して特定
    /// 余りが出てもいいからアニメステートは多めにする
    /// 空っぽのステートが増えても、アニメーション遷移はコード側でやるのでNullを引かない
    /// ステートはとにかく多く
    /// もし盾の戦技が武器戦技なら武器の戦技を詰めていく
    /// アニメーターはベースのモノを一つ作り、以後は武器ごとにオーバーライド
    /// オーバーライドコントローラーの仕様を調べる
    /// </summary>
    public void AnimationSetting()
    {
        //盾の戦技ナシなら
        if(equipShield.artsAnime == null)
        {
            //equipShield.artsAnimeの数だけ片手アニメーターに両手のアニメーターから戦技のアニメを仕込んでいく
            for (int i = 0; i < equipWeapon.artsAnime.Length; i++)
            {
                //     _anim.runtimeAnimatorController.animationClips[i] = equipWeapon._useContoroller[1].animationClips[i];
                ChangeClip($"Arts{i + 1}", equipWeapon.artsAnime[i]);
            }
        }
        //盾の戦技ありなら
        else
        {
            //equipShield.artsAnimeの数だけ片手のアニメコントローラーに戦技のアニメを仕込んでいく
            for (int i = 0; i < equipShield.artsAnime.Length; i++)
            {
                ChangeClip($"Arts{i + 1}", equipShield.artsAnime[i]);
                //   _anim.runtimeAnimatorController.animationClips[i] = equipShield.artsAnime[i];
                //  Debug.Log($"さいあく{equipWeapon._useContoroller[0].animationClips[i].name}ですよ{equipShield.artsAnime[i].name}");

            }
        }
        

    }

    /// <summary>
    /// アニメーションクリップを変える処理
    /// </summary>
    /// <param name="clip"></param>
    /// <summary>
    /// Sub in real animations for stubs
    /// </summary>
    /// <param name="animator">Reference to animator</param>
    public void ChangeClip(string overrideClipName, AnimationClip animClip)
    {
        AnimatorOverrideController test = (AnimatorOverrideController)_anim.runtimeAnimatorController;

        test[overrideClipName] = animClip;
    }


    /// <summary>
    /// コンボ制限をセットする。
    /// </summary>


    #endregion

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



    public void HPReset()
    {
        hp = pStatus.maxHp;
    }

    //いらないやつ
    #region
    /*
    /// <summary>
    /// 何度も当たり判定が検出されるのを防ぐためのもの
    /// </summary>
    public void DamageAvoid()
    {
        if (isDamage)
        {
            avoidTime += Time.fixedDeltaTime;
            pc.SetLayer(10);
            if (avoidTime >= 0.1)
            {
                isDamage = false;
                avoidTime = 0;
                pc.SetLayer(11);
            }
        }
    }

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
                _anim.Play("TLongSwordFalter");
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
                _anim.Play("Bounce");
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

    */


    #endregion


    ///<summary>
    ///　音声制御関連のメソッド
    /// </summary>
    #region
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

    #endregion


    //状態確認
    #region

    /// <summary>
    /// プレイヤーが通常状態でいるかどうか。
    /// イベントを引き起こせる状態であるかどうか
    /// </summary>
    public bool PlayerStateCheck()
    {
        return pc.CheckPLayerNeutral();
    }

    #endregion

    //プレイヤー制御
    #region

    /// <summary>
    /// trueでロック、falseで解除
    /// </summary>
    public void PlayerEventLock(bool Lock)
    {
        pc.EventLock(Lock);
    }

    #endregion


}
