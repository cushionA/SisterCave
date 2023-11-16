using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Net.Sockets;
using MoreMountains.CorgiEngine;

[System.Serializable] //これを書くとinspectorに表示される。
public class AttackJudge:SisterConditionBase
{

    #region 定義


    /// <summary>
    /// 攻撃魔法使用の挙動設定
    /// </summary>
    public enum AttackWaitCondition
    {
        即時発射,
        任意の秒数条件を待って発射,
        任意の秒数条件を待ってキャンセル
    }

    /// <summary>
    /// 攻撃魔法使用で待つ条件
    /// </summary>
    public enum WaitCondition
    {
        stun,//ターゲットのスタンを待つ
        attack,//ターゲットへのプレイヤーの攻撃を待つ
        range,//射程距離に入るのを待つ
        hit,//射線通るのを待つ
        enemyAttack,//敵が攻撃しようとするのを待つ
    }


    /// <summary>
    /// 攻撃前に待機するかの条件
    /// </summary>
    public struct AttackWaitSetting
    {
        [Header("何かを待ってから攻撃するか")]
        public AttackWaitCondition waitCondition;


        [Header("待機する条件")]
        public WaitCondition waitEvent;

        [Header("待つ秒数")]
        public float waitTime;

        /// <summary>
        /// ヘイトを向けてきた敵がいたらターゲット切り替えて撃つかどうか
        /// </summary>
        [Header("待機中攻撃してくる敵に標的を変えるか")]
        public bool targetHateChange;

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






    //[HideInInspector] public int percentage;
    //   [HideInInspector] public bool highOrLow;//その項目が以上か以下か。
    //  public byte WeakPointJudge;


    /// <summary>
    /// 使う魔法の属性
    /// これが攻撃に固有の行動選択条件
    /// </summary>
    public AtEffectCon.Element useElement;


    [Header("攻撃前の待機挙動を決める")]
    public AttackWaitSetting waitSetting;


}
