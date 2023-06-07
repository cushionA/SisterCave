﻿using UnityEngine;
using UnityEngine.AddressableAssets;
using static Equip;

[System.Serializable] //これを書くとinspectorに表示される。

public class AttackValue
{

    /// <summary>
    /// アクションの強度など
    /// </summary>
    public enum AttackLevel
    {
        Weak,
        Normal,
        Strong,
        Fatal,
        Special
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

    /// <summary>
    /// どんなエフェクトや音をもらうか
    /// </summary>
    public AttackLevel EffectLevel;

    /// <summary>
    /// どんなモーションであるか
    /// </summary>
    public MotionType motionType;

    /// <summary>
    /// モーション値
    /// </summary>
    [Header("モーション値")]
    public float x;

    /// <summary>
    /// アーマー
    /// </summary>
    [Header("攻撃時のアーマー")]
    public float y;

    /// <summary>
    /// 強靭削り
    /// </summary>
    [Header("強靭削り")]
    public float z;
    [Header("コンボ攻撃かどうか")]
    public bool isCombo;
    [Header("吹き飛ばせるかどうか")]
    public bool isBlow;
    [Header("弾かれるかどうか")]
    public bool isLight;

    [Header("吹っ飛ばす力")]
     public Vector2 blowPower;
    [Header("スタミナ消費")]
    public int useStamina;


    /// <summary>
    /// 盾の攻撃力を参照する攻撃かどうか
    /// </summary>
    [HideInInspector]
    public bool isShield;

    /// <summary>
    /// ヒット回数制限
    /// </summary>
    public int _hitLimit = 1;


    /// <summary>
    /// 移動する時間
    /// </summary>
    public float _moveDuration = 0.2f;

    /// <summary>
    /// 攻撃の移動距離
    /// ロックオンする場合はこの範囲内で敵との距離を入れる
    /// </summary>
    public float _moveDistance = 10f;

    /// <summary>
    /// 攻撃移動中に敵と接触時の挙動
    /// </summary>
    public MoreMountains.CorgiEngine.MyAttackMove.AttackContactType _contactType = MoreMountains.CorgiEngine.MyAttackMove.AttackContactType.通過;

    /// <summary>
    /// 落下攻撃かどうか
    /// </summary>
    [HideInInspector]
    public bool fallAttack = false;

    /// <summary>
    /// 攻撃時に移動開始するまでの時間
    /// </summary>
    public float startMoveTime = 0;

    /// <summary>
    /// 敵をロックオンして移動する攻撃かどうか
    /// </summary>
    public bool lockAttack = false;

    /// <summary>
    /// スーパーアーマー付きかどうか
    /// </summary>
    public bool superArmor;

    /// <summary>
    /// ガード攻撃かどうか
    /// </summary>
    public bool guardAttack;

    /// <summary>
    /// 背中向ける攻撃かどうか
    /// </summary>
    public bool backAttack;


    [Tooltip("攻撃のメイン属性")]
    public MoreMountains.CorgiEngine.AtEffectCon.Element mainElement;

    [Tooltip("攻撃の物理属性")]
    public AttackType phyElement;
}
