using MoreMountains.CorgiEngine;
using UnityEngine;
[System.Serializable]
public class SupportCondition:SisterConditionBase
{




    /// <summary>
    /// どんな種類のサポートを使用するか
    /// これはソートより先に判断
    /// 含むか含まないかどちらで判断するかフラグみたいなのはいらない
    /// </summary>
    [Header("使用するサポートのタイプ")]
    [EnumFlags]
    public Magic.SupportType secondActCondition = Magic.SupportType.なし;




    /// <summary>
    /// 使う魔法の属性
    ///　エンチャや設置、召喚で使う
    /// </summary>
    public AtEffectCon.Element useElement = AtEffectCon.Element.指定なし;





}
