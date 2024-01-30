using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[System.Serializable] //これを書くとinspectorに表示される。
public struct EnemyValue
{

    /// <summary>
    /// 攻撃の終了条件
    /// </summary>
    public enum AttackEndCondition
    {
        モーション終了,
        着地か時間経過,
        移動か時間経過,
        ヒットかモーション終了,
        ヒットか時間経過,
        補足行動の終了
        
    }

    /// <summary>
    /// 攻撃モーションに関する共通データ
    /// </summary>
    [Header("攻撃モーションに関する共通データ")]
    public AttackValueBase baseData;

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
    public int escapePercentage;

    [Header("クールタイム")]
    public float coolTime;


    /// <summary>
    /// 落下攻撃中や突進など攻撃を終わらせたくないときに
    /// 終了条件を表す
    /// </summary>
    public AttackEndCondition endCondition;
}
