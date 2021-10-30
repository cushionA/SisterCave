using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SisterMagic", menuName = "SisterMagic")]
public class SisMagic : Magic
{
   [HideInInspector]
    public enum MagicType
    {
        Attack,//攻撃
        Recover,//回復
        Support,//支援
        help//条件で勝手に発動？
    }
    public MagicType mType;

    [HideInInspector]
    public enum SupportType
    {
        攻撃強化,
        防御強化,
        エンチャント,
        アクション強化,
        バリア,
        リジェネ,
        なし
    }
    public SupportType sType;

    [Header("爆発するかどうか")]
    public bool isExprode;//爆発するかどうか

    public bool isSlip;//継続してダメージを与える
    [Header("状態異常を治すか")]
    public bool cureCondition;//状態異常
    public bool isWide;//範囲攻撃。混沌の嵐とかみたいな
    [Header("リジェネ回復量")]
    public float regeneAmount;//リジェネ回復の制限時間
    public float coolTime;
//    public float useLimit;//使用可能回数

    public AnimationCurve faithCurve;//信仰補。シスターさんは魔力と信仰

    new AnimationCurve powerCurve;
    new AnimationCurve skillCurve;

}
