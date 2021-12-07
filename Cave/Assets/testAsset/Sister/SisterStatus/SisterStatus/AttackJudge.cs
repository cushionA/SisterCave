using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable] //これを書くとinspectorに表示される。
public class AttackJudge
{

    [HideInInspector]
    public enum TargetJudge
    {
        敵タイプ,
        プレイヤーのHPが規定値に達した際,//ここで回復させれば緊急回復になるし、MP多いとかにすれば切り札を運用できる
                          //プレイヤーの体力関連は前のtargetをそのまま使う。前のが死滅したか初回なら一個次の条件で狙う
        プレイヤーのMPが規定値に達した際,
        自分のMPが規定値に達した際,
        プレイヤーが状態異常にかかった時,//これ未実装
        強敵の存在,
        状態異常にかかってる敵,//状態異常はスローと攻撃力防御力低下も付けるか。これ未実装
        かかってない支援がある,
        指定なし

    }

    public TargetJudge condition = TargetJudge.指定なし;
   
      //soldir兵士1000000
      //Fly,//飛ぶやつ0100000
      //Knight,//盾持ち0010000
      //Trap//待ち構えてるやつ00010000
      //UIチェックボックス入れて数字変える
      [Header("選択する敵とパーセントの数字")]
      [Tooltip("兵士1,飛ぶやつ2,Shooter4,Knight8,Trap,問わず0")]
    public int percentage;
    [Tooltip("trueで上、Falseで下")]
     public bool highOrLow;//その項目が以上か以下か。
    //public char SelectType;
    //  public byte WeakPointJudge;

    [HideInInspector]public enum WeakPoint
    {
        斬撃属性,
        刺突属性,
        打撃属性,
        聖属性,
        闇属性,
        炎属性,
        雷属性,
        毒属性,
        指定なし
    }

    [Tooltip("狙いたい弱点の指定")]
    public WeakPoint wp = WeakPoint.指定なし;

    [HideInInspector]
    public enum AdditionalJudge
    {
        //  敵の弱点,//状態異常含む
        敵のHP,
        敵の距離,
        敵の高度,
        //  敵の移動速度,
        敵の攻撃力,
        敵の防御力,
        指定なし
    }

    public AdditionalJudge nextCondition = AdditionalJudge.指定なし;

     public bool upDown;//あるいは低い方か多い方か



  /*  public byte SlashSet()
    {
        WeakPointJudge |= 0x80;
        return WeakPointJudge;
    }
    public byte StabSet()
    {
        WeakPointJudge |= 0x40;
        return WeakPointJudge;
    }
    public byte StrikeSet()
    {
        WeakPointJudge |= 0x20;
        return WeakPointJudge;
    }
    public byte HolySet()
    {
        WeakPointJudge |= 0x10;
        return WeakPointJudge;
    }
    public byte DarkSet()
    {
        WeakPointJudge |= 0x08;
        return WeakPointJudge;
    }
    public byte FireSet()
    {
        WeakPointJudge |= 0x04;
        return WeakPointJudge;
    }
    public byte ThunderSet()
    {
        WeakPointJudge |= 0x02;
        return WeakPointJudge;
    }
    public byte PoisonSet()
    {
        WeakPointJudge |= 0x01;
        return WeakPointJudge;
    }*/


}
