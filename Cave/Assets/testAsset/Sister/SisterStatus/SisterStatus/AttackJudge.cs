using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable] //これを書くとinspectorに表示される。
public class AttackJudge:SisterConditionBase
{

    #region 定義


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





    //ここからはターゲット設定後の挙動---------------------------------------------------


    public UseAction condition = UseAction.なにもしない;

    //[HideInInspector] public int percentage;
    //   [HideInInspector] public bool highOrLow;//その項目が以上か以下か。
    //  public byte WeakPointJudge;


    Element useElement;


    public FirstCondition firstCondition = FirstCondition.指定なし;

    /// <summary>
    /// 真なら含まないのを選ぶ
    /// 爆発しないとか
    /// </summary>
    public bool notContain;

    public AdditionalCondition secondCondition = AdditionalCondition.指定なし;

    [Tooltip("低い方か多い方か。あるほうかでない方か。Falseでないほう")]
    public bool secondUpDown;//あるいは低い方か多い方か。Falseでないほう



}
