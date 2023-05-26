using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.InventoryEngine;
using PathologicalGames;

public class Equip : Item
{

    [Header("ゲーム画面で表示するスプライト")]
    public Sprite[] front = new Sprite[2];
    public Sprite[] Side = new Sprite[2];
    public Sprite[] Naname = new Sprite[2];

    /// <summary>
    /// 装備レベル
    /// </summary>
    [HideInInspector] public int wLevel = 0;

    [HideInInspector] public float Atk;
    //　無属性
    [HideInInspector] public float phyAtk;
    //神聖
    [HideInInspector] public float holyAtk;
    //闇
    [HideInInspector] public float darkAtk;
    //炎
    [HideInInspector] public float fireAtk;
    //雷
    [HideInInspector] public float thunderAtk;

    [Header("基礎攻撃力")]
    public List<float> phyBase;//物理攻撃。これが1以上ならモーションにアニメイベントとかで斬撃打撃の属性つける
    public List<float> holyBase;//光。筋力と賢さが関係。生命力だから
    public List<float> darkBase;//闇。魔力と技量が関係
    public List<float> fireBase;//魔力
    public List<float> thunderBase;//魔力
    public float needPower;//必要筋力
    public float needSkill;//必要技量
    public float needInt;//必要な賢さ


    [HideInInspector] public float phyCut;//カット率
    [HideInInspector] public float holyCut;//光。
    [HideInInspector] public float darkCut;//闇。
    [HideInInspector] public float fireCut;//魔力
    [HideInInspector] public float thunderCut;//魔力
    [HideInInspector] public float guardPower;//受け値
    [Header("カット率")]
    // Start is called before the first frame update
    public List<float> phyCutSet;//カット率
    public List<float> holyCutSet;//光。
    public List<float> darkCutSet;//闇。
    public List<float> fireCutSet;//魔力
    public List<float> thunderCutSet;//魔力
    
    public List<AnimationCurve> powerCurve;
    public List<AnimationCurve> skillCurve;
    public List<AnimationCurve> intCurve;

    /// <summary>
    /// この数値でガードの音変わる
    /// 35まで小盾、70から大盾
    /// </summary>
    [Header("ガード力")]
    public List<float> guardPowerSet;//受け値

    [Header("ジャスガ開始時間")]
    public float parryStart;

    [Header("ジャスガ受付時間")]
    public float parryTime;

    [Header("パリィでのスタミナ回復量")]
    public float parryRecover;//パリィでのスタミナ回復量
    /// <summary>
    /// アーマー削り
    /// </summary>
    [HideInInspector]
    public float shock;

    [Header("固有技の消費MP")]
    public List<float> artsMP;




    ///<Summary>
    ///　使用する音声のリスト。
    ///　特殊な攻撃の音とか燃える音とか
    ///　Status.useSound[i]という形でアニメイベントで指定
    ///　useListに入れ替え
    ///</Summary>
  //  public List<string> useSound;

    public enum AttackType
    {
        Slash,//斬撃。ほどほどに通るやつが多い
        Stab,//刺突。弱点のやつと耐えるやつがいる
        Strike//打撃。弱点のやつと耐えるやつがいる。アーマーとひるませ強く
    }
    /// <summary>
    /// 剣で刺突するときとかはアニメイベントで変える
    /// </summary>
    [HideInInspector] public AttackType atType;

    public float _weight;

    public List<EffectCondition> _useList;
    public List<PrefabPool> _usePrefab;

}
