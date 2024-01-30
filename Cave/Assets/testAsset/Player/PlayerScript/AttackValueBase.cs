using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Equip;

/// <summary>
/// 構造体にする
/// 構造体なら値を代入できるから可読性上がる
/// </summary>
public struct AttackValueBase
{
    #region 定義

    /// <summary>
    /// 行動ごとに入れ替える必要のあるデータ
    /// とりわけダメージ計算に使うものの構造体
    /// 状態異常もどうにかしないとね
    /// </summary>
    public struct ActionData
    {
        /// <summary>
        /// モーション値
        /// </summary>
        [Header("モーション値")]
        public float mValue;

        /// <summary>
        /// アーマー削り
        /// </summary>
        [Header("アーマー削り")]
        public float shock;//


        /// <summary>
        /// アーマー
        /// </summary>
        [Header("攻撃時のアーマー")]
        public float additionalArmor;


        /// <summary>
        /// 吹き飛ばす力
        /// これが0以上なら吹き飛ばし攻撃を行う
        /// </summary>
        [Header("吹き飛ばす力")]
        public Vector2 blowPower;


        /// <summary>
        /// パリィのアーマー削りに対する抵抗
        /// </summary>
        [Header("パリィのアーマー削りに対する抵抗")]
        public float _parryResist;

        /// <summary>
        /// ヒット回数制限
        /// </summary>
        [Header("ヒット回数制限")]
        public int _hitLimit;


        [Header("攻撃のメイン属性")]
        public MoreMountains.CorgiEngine.AtEffectCon.Element mainElement;


        /// <summary>
        /// ダメージ計算で使う
        /// </summary>
        [Header("攻撃の全属性")]
        [EnumFlags]
        public MoreMountains.CorgiEngine.AtEffectCon.Element useElement;




        /// <summary>
        /// パリィされるか、軽いか重いかなど
        /// </summary>
        [Header("攻撃の性質")]
        public AttackFeature feature;

    }


    /// <summary>
    /// 攻撃移動に関するデータの構造体
    /// 魔法でも踏み込みがあったりしたら使う
    /// </summary>
    public struct AttackMoveData
    {
        /// <summary>
        /// 移動する時間
        /// </summary>
        public float _moveDuration;

        /// <summary>
        /// 攻撃の移動距離
        /// ロックオンする場合はこの範囲内で敵との距離を入れる
        /// </summary>
        public float _moveDistance;

        /// <summary>
        /// 攻撃移動中に敵と接触時の挙動
        /// </summary>
        public MoreMountains.CorgiEngine.MyAttackMove.AttackContactType _contactType;



        /// <summary>
        /// 攻撃時に移動開始するまでの時間
        /// </summary>
        public float startMoveTime;

        /// <summary>
        /// 敵をロックオンして移動する攻撃かどうか
        /// ロックオンしたら移動距離が伸びたりして何としてでも敵の前に行く
        /// </summary>
        public bool lockAttack;



        /// <summary>
        /// 背中向ける攻撃かどうか
        /// </summary>
        public bool backAttack;

    }

    /// <summary>
    /// 攻撃の性質
    /// </summary>
    public enum AttackFeature
    {
        light = 1 << 0,
        heavy = 1 << 1,
        disPariable = 1 << 2,
        selfRecover = 1 << 3,//命中時自分が回復
        hitRecover = 1 << 4,//命中した味方が回復。回復魔法
        superArmor = 1 << 5,//モーションがスーパーアーマー
        guardAttack = 1 << 6,
        fallAttack = 1 << 7,
        positiveEffect = 1 << 8,
        badEffect = 1 << 9,
        nothing = 0
    }


    /// <summary>
    /// アクションの強度
    /// エフェクトの判断に使う
    /// </summary>
    public enum AttackLevel
    {
        Weak,
        Normal,
        Strong,
        Fatal,
        Special,
        SEOnly
    }

    /// <summary>
    /// モーションのタイプ
    /// 音の判定に使う
    /// </summary>
    public enum MotionType
    {
        slash,
        stab,
        strike,
        shoot
    }






    #endregion


    [Header("アクションの情報")]
    public ActionData actionImfo;


    #region モーション再生関連

    /// <summary>
    /// どんなエフェクトや音をもらうか
    /// </summary>
    public AttackLevel EffectLevel;

    /// <summary>
    /// どんなモーションであるか
    /// </summary>
    public MotionType motionType;



    [Header("コンボ攻撃かどうか")]
    public bool isCombo;




    #endregion



    #region　攻撃移動関連

    /// <summary>
    /// 攻撃移動に関するデータ
    /// </summary>
    public AttackMoveData moveData;


    #endregion

}
