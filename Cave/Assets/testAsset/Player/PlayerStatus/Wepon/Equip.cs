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
    public enum AttackType
    {
        Slash,//斬撃。ほどほどに通るやつが多い
        Stab,//刺突。弱点のやつと耐えるやつがいる
        Strike//打撃。弱点のやつと耐えるやつがいる。アーマーとひるませ強く
    }

    public enum GuardType
    {
        small,//斬撃。ほどほどに通るやつが多い
        normal,//刺突。弱点のやつと耐えるやつがいる
        tower,
        wall
    }




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
    /// </summary>
    [HideInInspector] public int wLevel = 0;

    [HideInInspector] public float Atk;
    //　無属性
    [HideInInspector] public float phyAtk;
    //神聖
    [HideInInspector] public float holyAtk;
    //闇
    [HideInInspector] public float darkAtk;
    //炎
    [HideInInspector] public float fireAtk;
    //雷
    [HideInInspector] public float thunderAtk;


    [HideInInspector] public float phyCut;//カット率
    [HideInInspector] public float holyCut;//光。
    [HideInInspector] public float darkCut;//闇。
    [HideInInspector] public float fireCut;//魔力
    [HideInInspector] public float thunderCut;//魔力
    [HideInInspector] public float guardPower;//受け値
   
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
    public PrefabPool[] _usePrefab;


    [Foldout("サウンドとエフェクト")]
    [Header("固有エフェクトとサウンドリスト")]
    public MoreMountains.CorgiEngine.AtEffectCon.EffectAndSound[] AttackEffect;

    [Foldout("サウンドとエフェクト")]
    [Header("攻撃エフェクトのプレハブたち")]
    public PrefabPool[] AttackPrefab;
    
    #endregion


}
