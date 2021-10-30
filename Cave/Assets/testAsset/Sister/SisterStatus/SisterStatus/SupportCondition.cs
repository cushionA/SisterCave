using UnityEngine;
[System.Serializable]
public struct SupportCondition
{

    /// <summary>
    /// これは魔法を使用した後再判定を行わずに使うためのキャッシュ
    /// </summary>
    [HideInInspector] public SisMagic UseMagic;

    [HideInInspector]
    public enum SupportStatus
    {
        プレイヤーが状態異常にかかった時,//これは毒とか個別にしてもいいかもね
　　　　任意の支援が切れているとき,
        プレイヤーの体力が規定値の時,
        プレイヤーのMPが規定値に達した際,
        自分のMPが規定値に達した際,
        敵タイプ,
        強敵がいるかどうか,
        //状態異常が切れている敵がいる,
        かかっていない支援がある,//全てかかってるのは当てはまらないとき
        指定なし

    }

    public SupportStatus sCondition;
    public EnemyStatus.KindofEnemy Type;
    public SisMagic.SupportType needSupport;

     public int percentage;
    [Tooltip("trueで上、Falseで下")]
    public bool highOrLow;//その項目が以上か以下か。
    //  public byte WeakPointJudge;

    [HideInInspector]
    public enum MagicJudge
    {
        各種支援魔法,
        攻撃ステートに,
        回復ステートに,
        なにもしない

    }
    public MagicJudge ActBase;
    public SisMagic.SupportType useSupport;

    public enum AdditionalJudge
    {
        詠唱時間,
        持続効果時間,
        MP使用量,
        指定なし
    }

    public AdditionalJudge nextCondition;

     public bool upDown;//あるいは低い方か多い方か




}
