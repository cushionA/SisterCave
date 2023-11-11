using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable] //これを書くとinspectorに表示される。
public class SisterConditionBase
{

    #region 定義


    /// <summary>
    /// 行動を起こす条件
    /// </summary>
    public enum ActCondition
    {
        プレイヤーのHPが比率の条件を満たす際,//ここで回復させれば緊急回復になるし、MP多いとかにすれば切り札を運用できる
        プレイヤーのMPが比率の条件を満たす際,
        自分のHPが比率の条件を満たす際,
        自分のMPが比率の条件を満たす際,
        誰かのHPが比率の条件を満たす際,
        誰かのMPが比率の条件を満たす際,
        シスターさんとプレイヤーを除く誰かのHPが比率の条件を満たす際,
        シスターさんとプレイヤーを除く誰かのMPが比率の条件を満たす際,
        プレイヤーの特定の支援が切れた時,
        シスターさんとプレイヤーを除く誰かの特定の支援が切れた時,
        誰かの特定の支援が切れた時,
        シスターさんの特定の支援が切れた時,
        プレイヤーが特定の状態異常にかかった時,//これ未実装
        誰かが特定の状態異常にかかった時,
        シスターさんとプレイヤーを除く誰かが特定の状態異常にかかった時,


        敵がHP比率の条件を満たす際,
        敵がアーマー値の比率の条件を満たす時,
        敵タイプ,
        強敵の存在,
        特定の状態異常にかかってる敵がいる際,//状態異常はスローと攻撃力防御力低下も付けるか。これ未実装
        特定の弱点の敵がいる際,
        指定なし

    }

    /// <summary>
    /// 調査する範囲
    /// 敵全体とか味方とか
    /// この変数で照会メソッドの対象変わる
    /// </summary>
    public enum CheckRange
    {
        Player,
        Sister,
        Ally,
        OtherAlly,
    }


    [HideInInspector]
    public enum Element
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
        指定なし
    }


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



    public enum UseAction
    {
        攻撃行動を継続,
        支援行動に移行,
        回復行動に移行,
        なにもしない,
        現在の設定で行動
    }




    /// <summary>
    /// 行動を起こすか決めるための条件
    /// 全てのステートで使う
    /// </summary>
    public class ActJudgeCondition
    {


        public ActCondition actCondition = ActCondition.指定なし;


        //soldir兵士1000000
        //Fly,//飛ぶやつ0100000
        //Knight,//盾持ち0010000
        //Trap//待ち構えてるやつ00010000
        //UIチェックボックス入れて数字変える

        [Header("選択する敵とパーセントの数字")]
        [Tooltip("兵士1,飛ぶやつ2,Shooter4,Knight8,Trap,問わず0")]
        public int percentage;

        /// <summary>
        /// これはActconditionに関わる
        /// </summary>
        [Tooltip("trueで上、Falseで下")]
        public bool highOrLow;//その項目が以上か以下か。




        public AdditionalJudge nextCondition = AdditionalJudge.指定なし;

        /// <summary>
        /// これはAdditionalでつかう
        /// </summary>
        public bool upDown;//あるいは低い方か多い方か
    }



    #endregion





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




    /// <summary>
    /// これは魔法を使用した後再判定を行わずに使うためのキャッシュ
    /// その条件と魔法構成で判断したのは保存しとく
    ///  五個ある条件一つずつについてるから消すのは装備とAI設定しなおしたときだけ
    /// </summary>
    [HideInInspector] public SisMagic UseMagic;

    public bool AutoWait;//自動で使用魔法の使用MPが回復するクールタイムを作る


}
