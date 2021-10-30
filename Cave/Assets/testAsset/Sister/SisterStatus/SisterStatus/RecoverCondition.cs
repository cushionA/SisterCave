using UnityEngine;
[System.Serializable]
public struct RecoverCondition
{
    /// <summary>
    /// これは魔法を使用した後再判定を行わずに使うためのキャッシュ
    /// </summary>
    [HideInInspector] public SisMagic UseMagic;

    [HideInInspector]
    public enum RecoverStatus
    {
        プレイヤーが状態異常にかかった時,//これは毒とか個別にしてもいいかもね
        任意の支援が切れているとき,
        プレイヤーのHPが規定値の時,
        プレイヤーのMPが規定値に達した際,
        自分のMPが規定値に達した際,
        敵タイプ,
        強敵がいるかどうか,
       // 状態異常が切れている敵がいる,
        かかっていない支援がある,//全てかかってるのは当てはまらないとき
        指定なし

    }

    public RecoverStatus condition;
    public EnemyStatus.KindofEnemy Type;

    [Tooltip("任意の支援が切れてるとき条件で照合に使う")]
    public SisMagic.SupportType needSupport;

    public int percentage;
    [Tooltip("trueで上、Falseで下")]
    public bool highOrLow;//その項目が以上か以下か。
    //  public byte WeakPointJudge;

    [HideInInspector]
    public enum MagicJudge
    {
        治癒魔法,
        攻撃ステートに,
        支援ステートに,
        なにもしない

    }
    public MagicJudge ActBase;
    [Tooltip("支援効果で回復魔法を検索する場合に使う")]
    public SisMagic.SupportType useSupport;

    public enum AdditionalJudge
    {
        詠唱時間,//実装
        持続効果時間,//実装
        リジェネ回復量,//実装
        リジェネ総回復量,//実装
        状態異常回復,//実装
        回復量,//実装
        MP使用量,//実装
        指定なし//実装
    }

    public AdditionalJudge nextCondition;

     public bool upDown;//あるいは低い方か多い方か




}
