using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "Wepon", menuName = "CreateWepon")]

public class Wepon : Item
{
    public int wLevel = 0;

    public enum AttackType
    {
        Slash,//斬撃。ほどほどに通るやつが多い
        Stab,//刺突。弱点のやつと耐えるやつがいる
        Strike//打撃。弱点のやつと耐えるやつがいる。アーマーとひるませ強く
    }
    /// <summary>
    /// 剣で刺突するときとかはアニメイベントで変える
    /// </summary>
    public AttackType atType;
    

    public List<float> phyBase;//物理攻撃。これが1以上ならモーションにアニメイベントとかで斬撃打撃の属性つける
    public List<float> holyBase;//光。筋力と賢さが関係。生命力だから
    public List<float> darkBase;//闇。魔力と技量が関係
    public List<float> fireBase;//魔力
    public List<float> thunderBase;//魔力
    public float needPower;//必要筋力
    public float needSkill;//必要技量
    public float needInt;//必要な賢さ

    public List<float> phyCut;//カット率
    public List<float> holyCut;//光。
    public List<float> darkCut;//闇。
    public List<float> fireCut;//魔力
    public List<float> thunderCut;//魔力

    public List<float> guardPower;//受け値

    [HideInInspector]public float MagicAssistBase = 100;
    public float MagicAssist;//魔法威力修正
    public float castSkill = 1;

    //各能力補正
    public List<AnimationCurve> powerCurve;
    public List<AnimationCurve> skillCurve;
    public List<AnimationCurve> intCurve;
    public List<AnimationCurve> MAssistCurve;
    public AnimationCurve CastCurve;

    [HideInInspector]public float mValue;//攻撃するときリストの中からその都度モーション値設定

    [HideInInspector] public float atAromor;//攻撃するときリストの中からその都度攻撃アーマー設定

    [Header("強攻撃のチャージ時間")]
    public float chargeRes;
    //チャージ時間長くしたり減らしたりできる状態があるとおもろいからパブリック

    public int normalStamina;
    public int bigStamina;
    public int chargeStamina;


    //-----------------------------------------------モーション名の管理
    [Header("弱攻撃のモーション名リスト")]
    /// <summary>
    /// 弱攻撃のモーション名リスト
    /// </summary>
     public List<string> smallName;

    [Header("強攻撃のモーション名リスト")]
    /// <summary>
    /// 強攻撃のモーション名リスト
    /// </summary>
     public List<string> bigName;

    [Header("空中弱+強の名前リスト")]
    /// <summary>
    /// 空中弱の名前リスト
    /// </summary>
     public List<string> airName;

    [Header("チャージモーションの名前リスト")]
    /// <summary>
    /// チャージモーションリスト
    /// </summary>
     public List<string> chargeName;

    //--------------------------------------------モーション値、追加アーマー、強靭削りの管理

    [Header("弱攻撃の値")]
    /// <summary>
    /// 弱攻撃のXモーション値、Y追加アーマー、Z強靭削り
    /// </summary>
    public List<AttackValue> sValue;

    [Header("強攻撃の値")]
    /// <summary>
    /// 強攻撃のXモーション値、Y追加アーマー、Z強靭削り
    /// </summary>
    public List<AttackValue> bValue;

    [Header("ため攻撃の値")]
    /// <summary>
    /// 強攻撃のXモーション値、Y追加アーマー、Z強靭削り
    /// </summary>
    public List<AttackValue> chargeValue;

    [Header("空中弱攻撃の値")]
    /// <summary>
    /// 空中弱攻撃のXモーション値、Y追加アーマー、Z強靭削り
    /// </summary>
    public List<AttackValue> airValue;

    [Header("空中強攻撃の値")]
    /// <summary>
    /// 空中強攻撃のXモーション値、Y追加アーマー、Z強靭削り
    /// </summary>
    public AttackValue strikeValue;

    //------------------------------------------内部パラメータ

    [HideInInspector]
    public bool isLight;
    //軽い攻撃。盾にはじかれる

    [HideInInspector]public bool twinHand;//後々拡張する

    [HideInInspector]
    public float shock;

    [HideInInspector]
    /// <summary>
    /// //吹っ飛ばし攻撃
    /// </summary>
    public bool isBlow;

    [HideInInspector]
    /// <summary>
    /// 衝突できる回数。毎回設定しなおす
    /// </summary>
    public int hitLimmit = 1;

    [Header("吹っ飛ばす力")]
    public Vector2 blowPower;

    public float guardSpeed;//ガード中の移動速度

}
