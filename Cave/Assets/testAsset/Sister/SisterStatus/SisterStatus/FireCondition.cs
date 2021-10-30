using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class FireCondition
{

    /// <summary>
    /// これは魔法を使用した後再判定を行わずに使うためのキャッシュ
    /// その条件と魔法構成で判断したのは保存しとく
    ///  五個ある条件一つずつについてるから消すのは装備とAI設定しなおしたときだけ
    /// </summary>
    [HideInInspector] public SisMagic UseMagic;

    public bool AutoWait;//自動で使用魔法の使用MPが回復するクールタイムを作る
    public int CoolTime;//魔法を使った後のクールタイム

    [HideInInspector]
    public enum ActJudge
    {
        斬撃属性,
        刺突属性,
        打撃属性,
        聖属性,
        闇属性,
        炎属性,
        雷属性,
        毒属性,
        移動速度低下攻撃,
        攻撃力低下攻撃,
        防御力低下攻撃,
        属性指定なし,
        支援行動に移行,
        回復行動に移行,
        なにもしない
    }

    public ActJudge condition = ActJudge.なにもしない;

    //[HideInInspector] public int percentage;
 //   [HideInInspector] public bool highOrLow;//その項目が以上か以下か。
    //  public byte WeakPointJudge;

    [HideInInspector]
    public enum FirstCondition
    {
        敵を吹き飛ばす,
        貫通する,
        設置攻撃,
        範囲攻撃,
        追尾する,
        サーチ攻撃,
        指定なし

    }

    public FirstCondition firstCondition = FirstCondition.指定なし;

    public enum AdditionalCondition
    {
        //  敵の弱点,//状態異常含む
　　　　発射数,
        詠唱時間,
        攻撃力,
        削り値,
        MP使用量,
        指定なし
    }

    public AdditionalCondition nextCondition = AdditionalCondition.指定なし;

    [Tooltip("低い方か多い方か。あるほうかでない方か。Falseでないほう")]
     public bool upDown;//あるいは低い方か多い方か。Falseでないほう





}
