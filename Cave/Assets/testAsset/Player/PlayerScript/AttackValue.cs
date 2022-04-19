using UnityEngine;
using UnityEngine.AddressableAssets;
[System.Serializable] //これを書くとinspectorに表示される。

public class AttackValue
{

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
    [Header("攻撃タイプ")]
    public MyCode.Weapon.AttackType type;
    [Header("吹っ飛ばす力")]
     public Vector2 blowPower;
    [Header("スタミナ消費")]
    public int useStamina;

    [Header("攻撃エフェクト")]
  // [AssetReferenceUILabelRestriction("AttackEffect")]
    public AssetReference attackEffect;

    [HideInInspector]
    public bool isShield;

}
