using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[System.Serializable] //これを書くとinspectorに表示される。
public class EnemyValue
{

    /// <summary>
    /// どんなエフェクトや音をもらうか
    /// 攻撃の威力レベル
    /// </summary>
    public AttackValue.AttackLevel EffectLevel;

    /// <summary>
    /// モーションの詳細
    /// </summary>
    public AttackValue.MotionType motionType;

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

    /// <summary>
    /// 落下攻撃などの場合、攻撃のあとで使う補足モーションの番号を入れておけば後で使える
    /// </summary>
    [Header("落下攻撃")]
    public int suppleNumber;

    [Header("戦闘時に攻撃後退く可能性")]
    [Tooltip("飛行タイプはこの数値が1ならダッシュで逃げる。2ならゆっくり。3なら上昇せずに走る。4なら上昇せずに歩く")]
    ///<summary>
    ///この確率で攻撃後退く
    ///</summary>
    public float escapePercentage;


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
    /// 背面で出す、背中向ける攻撃かどうか
    /// </summary>
    public bool backAttack;

    [Tooltip("攻撃のメイン属性")]
    public MoreMountains.CorgiEngine.AtEffectCon.Element mainElement;


    [Tooltip("攻撃の物理属性")]
    public Equip.AttackType phyElement;
}
