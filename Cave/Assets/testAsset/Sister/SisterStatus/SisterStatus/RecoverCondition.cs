using UnityEngine;
[System.Serializable]
public class RecoverCondition:SisterConditionBase
{



    /// <summary>
    /// 回復魔法の独自行動選択
    /// 回復魔法で何を重視するのかを細かく見る
    /// これに一番当てはまるものを見る
    /// 回復量が最も高いか、毒を回復するかなど
    /// ソート前にこれで省く
    /// </summary>
    [Header("回復魔法を絞り込む二個目の条件")]
    [EnumFlags]
    public Magic.HealEffectType secondActJudge;




    /// <summary>
    /// 原則無し
    /// UIにも非常に少ない項目だけ表示
    /// バリア、サテライト、防御強化くらいせいぜい
    /// 選択可能な項目は少ない
    /// </summary>
    [Tooltip("支援効果で回復魔法を検索する場合に使う")]
    [EnumFlags]
    public Magic.SupportType healSupport = Magic.SupportType.なし;



}
