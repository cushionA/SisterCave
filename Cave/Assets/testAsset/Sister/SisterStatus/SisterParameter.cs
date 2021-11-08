using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "SisterParameter", menuName = "CreateSisterParameter")]
public class SisterParameter : ScriptableObject
{
    [Header("作戦名")]
    public string oparationName;

    [Header("作戦説明")]
    public string oparationDescription;

    [Header("何番目の作戦か示す")]
    public int FairstNumber;

    [Header("先制攻撃するかどうか")]
    public bool isPreemtive;
    [Header("最初に支援するかどうか")]
    public bool startSupport;//最初に支援させて優先ステートに戻る。戦闘ステート解除の時に再セットだな
    [Header("道中で勝手に回復させるかどうか")]
    public bool autoHeal;
    [Header("初期ステートに戻って判断しなおす時間")]
    ///<summary>
    ///　一秒から三十秒までで設定できるように選べる
    /// </summary>
    public float stateResetRes;
    [Header("ターゲットの再設定間隔")]
    ///<summary>
    ///　三秒から三十秒までで設定できるように選べる
    /// </summary>
    public float targetResetRes;

    [Header("攻撃のクールタイム")]
    ///<Summary>
    /// 攻撃のクールタイム
    ///</Summary>
    public List<float> attackCT;

    [Header("支援のクールタイム")]
    ///<Summary>
    /// 支援のクールタイム
    ///</Summary>
    public List<float> supportCT;

    [Header("回復のクールタイム")]
    ///<Summary>
    /// 回復のクールタイム
    ///</Summary>
    public List<float> healCT;


    public enum MoveType
    {
        攻撃,
        回復,
        支援
    }
    [Header("どのタイプの魔法を優先するか")]
    public MoveType priority;
    [Header("現在どのタイプの魔法を使おうとしてるのか")]
    [HideInInspector] public MoveType nowMove;

    #region//nowStateが攻撃の時

  /*    [HideInInspector]
  public enum TargetJudge
    {
        兵士,
        飛行,
        射撃,
        騎士,
        待ち伏せ,

        敵タイプ,
        距離近いのから,
        距離遠いのから,
        敵のHP少ないのから,
        敵のHP多いのから,
        移動速度早い,
        移動速度遅い,
        強敵がいる,
        プレイヤーのHPが規定値に達した際,//ここで回復させれば緊急回復になるし、MP多いとかにすれば切り札を運用できる
                         　　　　　　　　//プレイヤーの体力関連は前のtargetをそのまま使う。前のが死滅したか初回なら一個次の条件で狙う
        プレイヤーのMPが規定値に達した際,
        自分のMPが規定値に達した際,
        プレイヤーが状態異常にかかった時,
        強敵かどうか,
        斬撃属性が弱点,
        刺突属性が弱点,
        打撃属性が弱点,
        聖属性が弱点,
        闇属性が弱点,
        炎属性が弱点,
        雷属性が弱点,
        なし

    }*/
    public AttackJudge firstTarget;//判断条件セット
    public AttackJudge secondTarget;//判断条件セット
    public AttackJudge thirdTarget;//判断条件セット
    public AttackJudge forthTarget;//判断条件セット
    public AttackJudge fiveTarget;


    /// <summary>
    /// 弱点で判断した上にかぶせられる
    /// </summary>
 /*   public enum AdditionalJudge
    {
      //  敵の弱点,//状態異常含む
        敵のHP,
        敵の距離,
        敵の高度,
      //  敵の移動速度,
        なし
    }*/

    /*  public enum KindofEnemy
      {
          Soldier,//陸の雑兵
          Fly,//飛ぶやつ
          Shooter,//遠距離
          Knight,//盾持ち
          Trap//待ち構えてるやつ
        //  Strong
      }*/
    //  [Header("狙う敵のタイプ")]
    //  public KindofEnemy TargetKind;

 /*   [HideInInspector]
    public enum TargetPoint
    {
        Slash,
        Pier,
        Strike,
        Holy,
        Dark,
        Fire,
        Thunder
    }
    public TargetPoint tp;//標的にする弱点属性*/

 /*   [HideInInspector]
    public enum AttackConditional
    {
        爆発する,
        敵を吹き飛ばせる,
        貫通する,
        斬撃属性,
        刺突属性,
        打撃属性,
        聖属性,
        闇属性,
        炎属性,
        雷属性,
        範囲攻撃,//設置火炎放射とかも含める?
        発射数が多い,
        状態異常つき,
        弾速が早い,
        MP消費が少ない,
        MP消費が多い,
        詠唱時間が短い,
     //   攻撃魔法,//再判断
        支援魔法,
        回復魔法,
        何もしない
        //攻撃優先にして第一条件を強敵、回復にすれば強敵がいるときは回復してくれるよ
    }*/

    public FireCondition firstAttack;//一個目の条件に当てはまるやつ
    public FireCondition secondAttack;
    public FireCondition thirdAttack;
    public FireCondition fourthAttack;
    public FireCondition fiveAttack;
    public FireCondition nonAttack;//なにも当てはまらないとき
    #endregion

    #region//nowStateが支援の時
  /*  public enum SupportCondition
    {
        状態異常にかかった時,
        攻撃強化がない,
        防御強化がない,
        アクション強化がない,
        バリアがない,
        エンチャントがない,
        プレイヤーの体力がマックス,
        プレイヤーの体力が半分以下,
        プレイヤーの体力が二割以下,
        かかっていない支援がある,//全てかかってるのは当てはまらないとき
        なし
    }*/

    public SupportCondition firstPlan;
    public SupportCondition secondPlan;
    public SupportCondition thirdPlan;
    public SupportCondition forthPlan;
    public SupportCondition fivePlan;
    public SupportCondition sixPlan;

    /*   public enum SupportConditional
       {
           攻撃強化,//かけ直しは時間が延びる
           防御強化,
           アクション強化,
           バリア,
           エンチャント,
           攻撃魔法,
           回復魔法,
           何もしない

       }*/
    /*    public SupportCondition firstSupport;//一個目の条件に当てはまるやつ
        public SupportCondition secondSupport;
        public SupportCondition thirdSupport;
        public SupportCondition forthSupport;
        public SupportCondition fiveSupport;
        public SupportCondition nonSupport;//なにも当てはまらないとき
        */
    #endregion

    #region//nowStateが回復の時
    public RecoverCondition firstRecover;//一個目の条件に当てはまるやつ
    public RecoverCondition secondRecover;
    public RecoverCondition thirdRecover;
    public RecoverCondition forthRecover;
    public RecoverCondition fiveRecover;
    public RecoverCondition nonRecover;//なにも当てはまらないとき
    #endregion

    public RecoverCondition nFirstRecover;//一個目の条件に当てはまるやつ
    public RecoverCondition nSecondRecover;
    public RecoverCondition nThirdRecover;


    /*    public enum RecoverJudge
        {
            リジェネが切れたとき,//回復量少ないけどたくさんリジェネするやつとか状態異常解除するやつとか
            状態異常にかかった時,
            プレイヤーの体力がマックス,
            プレイヤーの体力が半分以下,
            プレイヤーの体力が二割以下,//ここで回復させれば緊急回復になるし、MP多いとかにすれば切り札を運用できる
            なし
        }

        public RecoverJudge firstCondition;
        public RecoverJudge secondCondition;
        public RecoverJudge thirdCondition;
        public RecoverJudge forthCondition;
        public RecoverJudge fiveCondition;

        public enum RecoverConditional
        {
            リジェネ時間が長い回復,
            回復量が大きい回復,
            MP消費が少ない回復,//リジェネ時間も短くて回復量がそこそこのが当てはまる
            詠唱時間が短い回復,
            状態異常を解除する回復,
            攻撃魔法,
            支援魔法,
            何もしない
        }


        #endregion*/

}
