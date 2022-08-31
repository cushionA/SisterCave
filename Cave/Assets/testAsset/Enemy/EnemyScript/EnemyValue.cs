using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[System.Serializable] //これを書くとinspectorに表示される。
public class EnemyValue
{
    [Header("モーション値")]
    public float mValue;
    [Header("攻撃時のアーマー")]
    public float aditionalArmor = 1;
    [Header("強靭削り")]
    public float shock;
 //   [Header("ヒット数")]
 //   public int hitLimmit;
    [Header("吹き飛ばせるかどうか")]
    public bool isBlow;
    [Header("弾かれるかどうか")]
    public bool isLight;
    [Header("攻撃タイプ")]
    public EnemyStatus.AttackType type;
    [Header("クールタイム")]
    public float coolTime;
    [Header("吹き飛ばす力")]
    public Vector2 blowPower;
    [Header("コンボするかどうか")]
    public bool isCombo;
    [Header("遠距離攻撃かどうか")]
    public bool isShoot;
    [Header("パリィ不可")]
    public bool disParry;
    [Header("パリィ抵抗値")]
    public float parryResist;
    [Header("落下攻撃")]
    public int suppleNumber;

    [Header("戦闘時に攻撃後退く可能性")]
    [Tooltip("飛行タイプはこの数値が1ならダッシュで逃げる。2ならゆっくり。3なら上昇せずに走る。4なら上昇せずに歩く")]
    ///<summary>
    ///この確率で攻撃後退く
    ///</summary>
    public float escapePercentage;

    [Header("攻撃エフェクト")]
    [AssetReferenceUILabelRestriction("AttackEffect")]
    public AssetReference attackEffect;
}
