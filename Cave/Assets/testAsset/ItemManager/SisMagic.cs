using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SisterMagic", menuName = "SisterMagic")]
public class SisMagic : Magic
{



    public SupportType sType;

    [Header("爆発するかどうか")]
    public bool isExprode;//爆発するかどうか

    public bool isSlip;//継続してダメージを与える
    [Header("状態異常を治すか")]
    public bool cureCondition;//状態異常
    public bool isWide;//範囲攻撃。混沌の嵐とかみたいな
    [Header("リジェネ回復量")]
    public float regeneAmount;//リジェネ回復の量
    public float coolTime;
//    public float useLimit;//使用可能回数

    public AnimationCurve faithCurve;//信仰補。シスターさんは魔力と信仰

    new AnimationCurve powerCurve;
    new AnimationCurve skillCurve;

}
