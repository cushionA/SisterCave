﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// シスターさんの戦闘中の挙動を決めるクラス
/// ・敵との距離をどのくらいにするか（これは近い、遠い、中くらい、みたいなオプションと詳細設定を用意）
///・どの敵を基準に距離を取るか（一番近い敵、一番強い敵、それを飛ぶ敵とか、あるいはプレイヤーね）
///・回復、支援、攻撃それぞれで変えれるようにする（攻撃は敵に近寄りたいけど回復はそうでもないよね。でもめんどくさいだろうから、この状態の移動設定を別の状態にコピーする…みたいなのを用意）
///・立ち位置をどうするか（高所を取るとか？　こっちのほうが優先すべき行動だよね。敵との距離とかを保つ移動をしたあと、ごく狭い範囲内に登れるような場所があったら…ってことにするか）
///・再移動判断条件。これはお好みで複数選んで増やしていく形にする。（基準の相手が維持距離より離れたら、基準の相手が維持距離より近づいてきたら、敵意を向けられていたら…とか）
///・それと今この場の敵の弱点属性で一番多いやつをマネージャーに持たせるのもあり？）
///・攻撃を食らったとき回復モードに移行とかモード移行もできる。そのままその場で戦闘
///・魔法詠唱中に攻撃された時、被弾イベント呼ぶかどうかを選べる。
///・イベントはモードチェンジ、逃走、緊急回避（MP20消費で攻撃してきたやつから維持距離分離れて次回判定時まで移動完了になり行動続行、そのままなにもしない）
///・あとは攻撃してきた相手をターゲットにして攻撃とかもありかも。この場合攻撃条件とかを設定する？（いやでもこれは攻撃にモードチェンジして狙ってくる敵がいる時ってすればいいな）
///・被弾時移動と被弾時モードチェンジは別々にするか
///・こうなるとアクセサリで魔法詠唱中被ダメージ減少とかもありかも
///・MP減少でなにかあってもいいかも。いや、これはふつうの行動判断でできるよな？
///・モードごとに移動条件違うしモード切り替え後は移動判断おこなうようにする？
/// 
/// ワープを多用するならレイキャストでそこに行けるか調べないと
/// ついでにシスターさんが魔法使ったら敵攻撃マネージャーにイベント飛ぶようにする？
/// それか魔法自体にヘイト値をつけて唱えた瞬間敵全体のヘイト値が増えるようにする？
/// でも攻撃ならちゃんと敵のヘイト向くし、支援とかならむしろ別に強化されたプレイヤーを狙ってくれないと弱いし、ゲーム的にもせっかく強化したのに別の相手狙う敵を追うのはつまらないよな
/// でも回復は流石にヘイト来ていいと思う？
/// 
/// これの設定はレベルアップで徐々に開放
/// 勇敢や慎重、バランス、臆病みたいなテンプレがたくさん
/// 慎重（回復重視）、慎重（支援重視）みたいなのあってもいいかも
/// コミュニケーション頑張ると順次解放、詳細編集が最後に解放されてできるようになる
/// 詳細もイチから設定はめんどいのでベースを選べるようにする
/// 
/// 
/// 
/// 整理しよ
///・NPCの立ち位置は基準となるキャラクターから設定した距離だけ離れた場所になる
///
///・NPCが立ち位置を決める基準にするキャラクターは選べる（プレイヤー、一番近い敵、強い敵…とか色々。そのキャラから設定した距離だけ離れる）
///
///・NPCが立ち位置を決めたあと、再判断する条件は選べる（敵に狙われた時とか、敵が近づいてきた時とか、時間経過、確率、周辺敵密度、その他いくつか。三つまで選んで組み合わせ可能）
///
///・被弾時にイベントを設定できる（被弾時逃走、被弾時ワープ、被弾時モードチェンジ（後述）、そのまま戦う）
///
///・立ち位置設定（プレイヤーの近くの位置に立つか、基準キャラとの背後に回るか…しか考えつかなかった）
///・あと敵が少ない（多い）方向に行くかとかもあり。基準キャラの位置より右にいる敵と左にいる敵どちらが多いかをエネミーマネージャーに問い合わせるわけ
///
///・ワープ移動条件（特定の条件を満たした時は、立ち位置を変える際にワープで移動する。被弾時ワープよりMP消費が少ない）
///
///・攻撃モード、支援モード、回復モードの三つにそれぞれ一つずつこれらの設定を付与できる。被弾時モードチェンジは被弾した時攻撃から回復に切り替えて下がるというような感じ。
///
///・壁にぶつかった時どうするか(立ち位置設定を上書きする)
///
///補足:「プレイヤーの近くの位置に立つか」というのは、たとえば基準キャラから20m離れた位置に立つなら+20と-20の二つの選択肢があって、＋と−でプレイヤーに近い方に行く感じ
/// 
/// 高所とか無理そうだから自分の足元に時間経過で壊れるブロックを作る魔法を実装するとかどうだろう
/// 
/// ・移動完了後イベントを作るのはどうだろ。移動完了後敵密度が多いならさらに離れるとかできるよ
/// 
/// </summary>


public class SisterMoveSetting
{


    /// <summary>
    /// 回復モード、攻撃モードなどの状態ごとの移動ステータス
    /// これ一個に情報をまとめる
    /// </summary>
    public class StateMoveStatus
    {

        /// <summary>
        /// ヘイトを集めやすい攻撃モードは速いペースで判断するとかあると思う
        /// 行動してない間だけ時間計測する
        /// </summary>
        [Header("何秒ごとに移動を判断する時間")]
        public float judgePace;


        [Header("基準にする対象の条件")]
        public MarkCharacterCondition markTarget;


        /// <summary>
        /// 基準となる相手と維持する距離
        /// </summary>
        [Header("維持する距離")]
        public float keepDistance;

        /// <summary>
        /// 基準距離からプラマイどれくらい離れていてもいいか
        /// </summary>
        [Header("維持する距離の範囲")]
        public float adjustRange;


        /// <summary>
        /// もし基準距離が絡む条件の場合
        /// 死んだ敵の報告が基準敵なら強制再判断
        /// 基準敵の情報は確保しておく
        /// </summary>
        [Header("再判断する条件")]
        public RejudgeStruct[] rejudgeCondition = new RejudgeStruct[3];


        /// <summary>
        /// これが真ならrejudgeCondition配列の
        /// どれか一つでもあてはまると条件通る
        /// </summary>
        [Header("真ならOR条件")]
        public bool orCondition;


        /// <summary>
        /// 被弾時にどんな条件があると特殊な行動をするか
        /// </summary>
        [Header("ダメージイベントの判断条件")]
        public RejudgeStruct damageCondition;


        [Header("条件真の時のダメージイベント")]
        public DamageMoveEvent damageTrueEvent;

        [Header("条件偽の時のダメージイベント")]
        public DamageMoveEvent damageFalseEvent;


        /// <summary>
        /// 壁に触れた時のイベントを示す
        /// 壁だけではなく穴にも対応する
        /// </summary>
        [Header("壁際に触れた時の行動を起こす条件")]
        public RejudgeStruct wallCondition;


        [Header("条件が真のときの壁イベント")]
        public WallCollisionEvent trueWallEvent;

        [Header("真の行動の前に壁のぼりチェックをするか")]
        public bool trueWallClimb;

        [Header("条件が偽の時の壁イベント")]
        public WallCollisionEvent falseWallEvent;

        [Header("偽の行動の前に壁のぼりチェックをするか")]
        public bool falseWallClimb;


        /// <summary>
        /// 再判断した時ワープ移動する条件
        /// enumFlagにしていいかも？
        /// 真ならワープ移動
        /// mp足りないときは歩き？
        /// それとも1以上あればできる
        /// </summary>
        [Header("ワープ移動発動条件")]
        public RejudgeStruct warpCondition;



        /// <summary>
        /// このオプションに従って立ち位置が変わる
        /// </summary>
        [Header("位置取りの際の移動オプション")]
        public PositionJudgeOption moveOption;


        /// <summary>
        /// 移動完了後にさらに移動するか決めるオプションとかアリだね
        /// </summary>
        [Header("移動後に再移動する条件")]
        public RejudgeStruct RelocationCondition;
            

        /// <summary>
        /// 移動中に停止する条件
        /// </summary>
        [Header("移動中に停止する条件")]
        public RejudgeStruct MoveStopCondition;

    }

    #region 定義



    /// <summary>
    /// 条件と判断に必要な変数をまとめた構造体
    /// </summary>
    public struct RejudgeStruct
    {

        //条件三つずつを三つ、第三条件まで持つ
        //そしてその条件ごとに距離と移動オプションとワープ移動するかを選べる？
        //いや、ワープ移動は単一の条件で決めるか
        //第一判断が優先、第二判断がその次
        //ワープ条件には何番目の移動はワープとかもできるようにする？

        /// <summary>
        /// useWarpMoveが真の時
        /// 再判断を満たしてこの一つの条件を
        /// 満たしたときはワープして移動する
        /// </summary>
        public RejudgeCondition condition;

        /// <summary>
        /// マイナスなら以下、プラスなら以上ってことにする
        /// </summary>
        [Header("判断に使う数値")]
        public float value;


        /// <summary>
        /// たとえば80なら条件を満たしても20パーセントの確率で再移動しない
        /// </summary>
        [Header("条件満たした後に真になる確率")]
        public float percentage;


    }


    /// <summary>
    /// 移動の基準になる相手を選ぶ条件
    /// 一つだけ使う
    /// どうしても完璧は無理なので汎用的になるように
    /// 抽象的な条件を一つ
    /// 
    /// </summary>
    public enum MarkCharacterCondition
    {
        一番近い敵,
        一番遠い敵,
        ランダムな敵,
        強敵,
        一番高いところにいる敵,
        プレイヤー

    }


    /// <summary>
    /// 立ち位置を再判断するための条件
    /// ダメージを受けた時にも設定を流用できる（ダメージを受けたかつ時間が…）とか
    /// 
    /// 三つまで組み合わせられる
    /// 敵に狙われた時とか、敵が近づいてきた時とか、時間経過、確率、周辺敵密度
    /// 移動後累計ダメージ、怯んだ時（これは移動再判断には表示しない）
    /// 基準距離系は基準の敵が死んだら再判断する
    /// 全部Or条件でいい？
    /// </summary>
    public enum RejudgeCondition
    {
        基準の距離から外れた時,//距離関係なく基準の敵が死んだら再評価するよ
        敵に狙われた時,
        敵の数が変動,
        一定の距離に敵がいる時,
        時間経過,//未満とつける？
        周辺に敵が何体いるか,
        自分のMPが指定の割合の時,
        移動後に指定のダメージを受けた時,
        背後からの攻撃を受けた時,
        プレイヤーのHPが指定の割合の時,
        なし,
        判定しない,
        逃走中であるか//壁イベントでだけ使う
    }


    /// <summary>
    /// ダメージを受けた際にどんな行動をとるか
    /// 設定できるイベントは一つだけ
    /// （被弾時逃走、被弾時ワープ、被弾時モードチェンジ（後述）、そのまま戦う）
    /// これは条件を二つ持って追加条件を満たした場合と満たさなかった場合を設定できる
    /// 追加条件を持つと出てくる
    /// </summary>
    public enum DamageMoveEvent
    {
        逃走,
        緊急逃走ワープ,
        プレイヤーのもとに行く,
        プレイヤーのもとに緊急ワープ,
        攻撃モードに移行,
        支援モードに移行,
        回復モードに移行,
        ランダムにモードチェンジ,
        再移動,//判定なしで再移動
        そのまま戦う
       
        
     
    }


    /// <summary>
    /// 壁にぶつかった時どうするか
    /// この設定も条件満たしたときとで二つ持つ
    /// </summary>
    public enum WallCollisionEvent
    {
        停止,
        反対側に移動,
        反対側にワープ
    }


    /// <summary>
    /// 移動の再判断が通った時
    /// 追加の判断をしてどんな動きをするか
    /// </summary>
    public enum PositionJudgeOption
    {
        プレイヤーの近くの位置に行く,
        プレイヤーの遠くの位置に行く,
        基準キャラの背後に回る,
        基準キャラの正面に立つ,
        基準キャラから敵が多い方に行く,//要は基準キャラのxポジションから右と左どっちが敵が多いかってことね
        基準キャラから敵が少ない方に行く,
        壁が近い方に行く,
        壁が遠い方に行く,
        オプション無し
    }



    #endregion


    #region　簡単設定用

    /// <summary>
    /// 簡易設定であらかじめ決めた値を入れる
    /// 詳細設定だけ自分でカスタムできる
    /// </summary>
    public enum MoveSettingType
    {
        臆病,
        慎重_回復重視,
        慎重_支援重視,
        慎重_攻撃もする,
        バランス,
        勇敢,
        世話焼き,
        優しい,
        魔物狩り,
        守護者,
        聖女,
        詳細設定
        

    }


    #endregion


    #region フィールド


    [Header("ムーブのテンプレート設定")]
    public MoveSettingType _type = MoveSettingType.臆病;

    [Header("攻撃状態の移動ステータス")]
    public StateMoveStatus AttackMoveSetting;

    [Header("支援状態の移動ステータス")]
    public StateMoveStatus SupportMoveSetting;

    [Header("回復状態の移動ステータス")]
    public StateMoveStatus HealMoveSetting;

    #endregion



}
