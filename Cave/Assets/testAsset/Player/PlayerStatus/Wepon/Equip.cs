using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.InventoryEngine;
using PathologicalGames;
using RenownedGames.Apex;
using System;

public class Equip : Item
{

    #region 定義


    public enum AttackType
    {
        Slash,//斬撃。ほどほどに通るやつが多い
        Stab,//刺突。弱点のやつと耐えるやつがいる
        Strike//打撃。弱点のやつと耐えるやつがいる。アーマーとひるませ強く
    }

    public enum GuardType
    {
        small,
        normal,
        tower,
        wall
    }

    /// <summary>
    /// モーションごとの入力タイプ
    /// 種類は4種類
    /// 
    /// ・入力時即時発動
    /// ・押している間チャージ、チャージ時間経過で発動
    /// ・押したら一定時間後に発動、しかしボタン押してる間は待機で照準が可能（魔法）
    /// ・押したら一定時間後に発動、しかしボタン押してる間は待機でチャージ攻撃が出る（待てるのは20秒まで）
    /// 
    /// 
    /// </summary>
    public enum InputType
    {
        normal,
        chargeAttack,
        waitableCharge,
        magic,
        non//なにも入力されてないとき

    }

    /// <summary>
    /// 入力に関する情報
    /// 入力方式と溜め時間
    /// </summary>
    public struct InputData
    {
        [Header("攻撃の入力タイプ")]
        public InputType motionInput;

        [Header("モーションのチャージ時間")]
        public float chargeTime;
    }

    /// <summary>
    /// モーションとチャージ攻撃に関するデータ
    /// </summary>
    public struct MotionChargeImfo
    {
        [Header("技の値")]
        /// <summary>
        /// 攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public AttackValue[] normalValue;

        [Header("チャージした技の値")]
        /// <summary>
        /// 武器固有攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public AttackValue[] chargeValue;

        [Header("入力関連のデータ")]
        public InputData[] inputData;

        [Header("通常攻撃のコンボ数")]
        public int normalComboLimit;

        [Header("チャージ攻撃のコンボ数")]
        public int chargeComboLimit;

    }

    /// <summary>
    /// ガード時の性能
    /// ヘルスにそのまま渡す
    /// </summary>
    public struct GuardStatus
    {
        [Header("物理カット防御力")]
        public float phyCut;//カット率

        [Header("聖カット防御力")]
        public float holyCut;//光。
        [Header("闇カット防御力")]
        public float darkCut;//闇。
        [Header("炎カット防御力")]
        public float fireCut;//魔力
        [Header("雷カット防御力")]
        public float thunderCut;//魔力

        [Header("攻撃による盾削りをどれくらい減らせるか")]
        public float guardPower;//受け値
    }

    /// <summary>
    /// 攻撃力のまとめ構造体
    /// </summary>
    public struct AttackStatus
    {
        [Header("合計攻撃力")]
        public float Atk;

        [Header("攻撃力")]
        public float phyAtk;
        [Header("聖攻撃力")]
        public float holyAtk;
        [Header("闇攻撃力")]
        public float darkAtk;
        [Header("炎攻撃力")]
        public float fireAtk;
        [Header("雷攻撃力")]
        public float thunderAtk;
    }

    #endregion

    //画像
    #region


    [Foldout("スプライト")]
    [Header("表示する正面のスプライト")]
    public Sprite[] front = new Sprite[2];

    [Foldout("スプライト")]
    [Header("表示する側面のスプライト")]
    public Sprite[] Side = new Sprite[2];

    [Foldout("スプライト")]
    [Header("表示する斜めのスプライト")]
    public Sprite[] Naname = new Sprite[2];

    #endregion

    //内部データ
    //こいつらは武器レベルとか能力値で算出するから表示しない
    #region
    /// <summary>
    /// 装備レベル
    /// 強化によって変動
    /// </summary>
    [HideInInspector] public int wLevel = 0;


    [Header("攻撃ステータス")]
    public AttackStatus atStatus;

    [Header("ガード性能")]
    public GuardStatus guardStatus;
   
    /// <summary>
    /// アーマー削り
    /// </summary>
    [HideInInspector]
    public float shock;


    #endregion


    //攻撃力と補正値
    #region
    [Foldout("基礎攻撃力設定")]
    public float[] phyBase;//物理攻撃。これが1以上ならモーションにアニメイベントとかで斬撃打撃の属性つける
    [Foldout("基礎攻撃力設定")]
    public float[] holyBase;//光。筋力と賢さが関係。生命力だから
    [Foldout("基礎攻撃力設定")]
    public float[] darkBase;//闇。魔力と技量が関係
    [Foldout("基礎攻撃力設定")]
    public float[] fireBase;//魔力

    [Foldout("基礎攻撃力設定")]
    public float[] thunderBase;//魔力


    //カーブはまず、武器レベルごとの最大補正と最低補正を考えて緩急つけていく
    //最初の伸びがいいとかそういうコンセプトで
    //そしてカーブをコピーして次の武器レベル作る

    [Foldout("基礎攻撃力設定")]
    public AnimationCurve[] powerCurve;

    [Foldout("基礎攻撃力設定")]
    public AnimationCurve[] skillCurve;

    [Foldout("基礎攻撃力設定")]
    public AnimationCurve[] intCurve;

    #endregion







    //ガード関連
    #region

    [Foldout("ガード関連")]
    [Header("物理カット率")]
    public float[] phyCutSet;//カット率

    [Foldout("ガード関連")]
    [Header("光カット率")]
    public float[] holyCutSet;//光。

    [Foldout("ガード関連")]
    [Header("闇カット率")]
    public float[] darkCutSet;//闇。

    [Foldout("ガード関連")]
    [Header("炎カット率")]
    public float[] fireCutSet;//魔力

    [Foldout("ガード関連")]
    [Header("雷カット率")]
    public float[] thunderCutSet;//魔力


    /// <summary>
    /// この数値でガードの音変わる
    /// 35まで小盾、70から大盾
    /// </summary>
    [Header("ガード力")]
    [Foldout("ガード関連")]
    public float[] guardPowerSet;//受け値

    [Foldout("ガード関連")]
    [Header("ジャスガ開始時間")]
    public float parryStart;

    [Foldout("ガード関連")]
    [Header("ジャスガ受付時間")]
    public float parryTime;

    [Foldout("ガード関連")]
    [Header("パリィでのスタミナ回復量")]
    public float parryRecover;//パリィでのスタミナ回復量

    [Foldout("ガード関連")]
    [Header("盾の種類")]
    public GuardType shieldType;

    [Foldout("ガード関連")]
    [Header("金属装備か")]
    public bool isMetal;

    #endregion



    //装備負荷など
    #region


    [Foldout("装備負荷関連")]
    [Header("必要筋力")]
    public float needPower;//

    [Foldout("装備負荷関連")]
    [Header("必要技量")]
    public float needSkill;//

    [Foldout("装備負荷関連")]
    [Header("必要な賢さ")]
    public float needInt;//

    [Foldout("装備負荷関連")]
    [Header("固有技の消費MP")]
    public float[] artsMP;

    [Foldout("装備負荷関連")]
    [Header("重量")]
    public float _weight;


    #endregion


    //音とエフェクト
    #region

    /// <summary>
    /// 武器の存在音は盾に優先する
    /// if(盾存在あるなら)とかで先に盾を初期化
    /// その後武器の存在音を出してあるなら上書き
    /// </summary>
    [Foldout("サウンドとエフェクト")]
    [Header("常に鳴る音")]
    [SoundGroup]
    public String ExistSound;

    [Foldout("サウンドとエフェクト")]
    [Header("通常ムーブエフェクトの設定")]
    public EffectCondition[] _useList;

    [Foldout("サウンドとエフェクト")]
    [Header("通常ムーブエフェクトのプレハブ")]
    public PrefabPool[] usePrefab;


    [Foldout("サウンドとエフェクト")]
    [Header("固有エフェクトとサウンドリスト")]
    public MoreMountains.CorgiEngine.AtEffectCon.EffectAndSound[] AttackEffect;

    [Foldout("サウンドとエフェクト")]
    [Header("攻撃エフェクトのプレハブたち")]
    public PrefabPool[] AttackPrefab;
    
    #endregion


}
