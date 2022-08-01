using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SisterCombination", menuName = "SisterCombinationCreate")]

　//設計
　//ターゲットいるかどうか判断
  //段数見て連続切れるまで実行
  //切れたら段数リセットしてクールタイム計測開始
  //段数多く使うとクールタイム伸びたりする？いや爽快感落ちるからやめようかな。伸びるにせよほんのわずかでいい
  //段数についてはむしろ条件配列かリストのCountではかればいいんじゃね
  //コンビネーションタイプについては接地系とかそういうので再思考
  //サブとメインの条件リストは段数で初期化
  //壁はプレイヤーから見て少し上に来るように調整？
  //クールタイムを待たずに連携するとMP使うようにする？

public class SisterCombination : Magic
{

    /* [HideInInspector]
     public enum CombinationType
     {
         Attack,//攻撃
         Recover,//回復
         Support,//支援
         help//条件で勝手に発動？
     }
     public CombinationType cType;
    */

    [Header("プレイヤー以外の標的を設定するかどうか")]
    public bool isTargeting;
    [Header("何回までつながるか")]
    public int chainNumber = 1;
    [Header("クールタイム")]
    public float coolTime;

    [Header("メインの敵選択条件")]
    public List<AttackJudge> mainTarget;//判断条件セット

    [Header("サブの敵選択条件")]
    public List<AttackJudge> subTarget;//判断条件セット

    [Header("いろんなことに使う真偽")]
    bool isUtility;

    [Header("始動タイプ")]
    public ActType _combiType;

    public enum ActType
    {
        soon = 1,
        cast = 2,
        longPress = 3,
        castAndPress = 4
    }

    //条件としては強敵、飛行タイプの敵…いやクールタイムあるし攻撃並みに細かくてもいいよ
    //同等の細かさで設定
    //ワープは壁に埋まらないように気を付けたい
    //壁は先の地面に現れるという性質上足場にもできるか。へこんだ地形用意して
    //じゃあ一番上はGroundタグ付けるか
    //ボタン長押しで壁の位置が変わる。壁の位置どこになるかはガイド表示される
    //レイキャストで地面探るスクリプト利用
    //最初の場所を起点に移動する
    //なら標的とかいらなくね？。わからん





}
