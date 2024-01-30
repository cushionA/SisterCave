using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using static Magic;

[System.Serializable] //これを書くとinspectorに表示される。
public class SisterConditionBase
{

    #region 定義


    /// <summary>
    /// 行動を起こす条件
    /// これは設定に使うだけにする？
    /// これの設定に基づいて調査ステータスを決定する
    /// 
    /// 調査ステータスcheckRangeとcheckContentを入れろ
    /// </summary>
    public enum ActCondition
    {
        プレイヤーのHPが比率の条件を満たす時,//ここで回復させれば緊急回復になる
        プレイヤーのMPが比率の条件を満たす時,
        自分のHPが比率の条件を満たす時,
        自分のMPが比率の条件を満たす時,
        味方のHPが比率の条件を満たす時,
        味方のMPが比率の条件を満たす時,
        シスターさんとプレイヤーを除く味方のHPが比率の条件を満たす時,
        シスターさんとプレイヤーを除く味方のMPが比率の条件を満たす時,
        プレイヤーの特定の支援が切れた時,
        シスターさんとプレイヤーを除く味方の特定の支援が切れた時,
        味方の特定の支援が切れた時,
        シスターさんの特定の支援が切れた時,
        プレイヤーが特定の状態異常にかかった時,//これを選んだ時、パーセンテージがそれぞれの状態異常と同じビットになるように操作する
        味方が特定の状態異常にかかった時,
        シスターさんとプレイヤーを除く味方が特定の状態異常にかかった時,
        指定距離にプレイヤーがいる時,
        指定距離に味方がいる時,

        //ここから敵関連の条件に
        敵がHP比率の条件を満たす時,
        敵がアーマー値の比率の条件を満たす時,
        敵タイプ,
        強敵の存在,
        特定の状態異常にかかってる敵がいる時,//状態異常はスローと攻撃力防御力低下も付けるか。これ未実装
        特定の攻撃属性を持つ敵がいる時,
        特定の状態異常を使う敵がいる時,
        特定の弱点属性の敵がいる時,
        特定の状態異常が弱点の敵がいる時,
        連携中の敵がいる時,//指揮機能
        指定距離に敵がいる時,
        周辺の敵が指定の数である時,//周辺敵密度をセンサーで出す
        プレイヤーを狙ってる敵がいる,
        シスターさんを狙ってる敵がいる,//こういうのは自分の番号がターゲットになってる敵が何人いるかを取得する
        シスターさんとプレイヤー以外を狙う敵がいる,
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
        Enemy
    }

    /// <summary>
    /// ターゲットを強制指定する
    /// 使わんかも
    /// </summary>
    public enum TargetForceSet
    {

    }


    /// <summary>
    /// なにについて調べるのか
    /// 体力とか
    /// </summary>
    public enum CheckContent
    {
        HP,
        MP,
        type,
        strength,//強いかどうか
        posiCondition,//バフ
        negaCondition,//状態異常
        weakPoint,
        distance,
        armor,
        playerHate,//誰を狙ってるか
        sisterHate,
        otherHate,
        enemyCount//周辺の敵が何体いるか、以上か以下か
    }


    /// <summary>
    /// 誰をターゲットにするのか直接指定する
    /// 回復とか支援で特定の条件でシスターさんやプレイヤーなど
    /// 対象を直接指定したいときが出てくるよね
    /// 攻撃時はUIから隠す
    /// </summary>
  //  [Header("攻撃以外の時ターゲットを直接指定")]
  //  public CheckRange targetSelect;





    /// <summary>
    /// ターゲットをさらに絞り込むための条件
    /// 比較可能なものを一つだけ使う
    /// </summary>
    [HideInInspector]
    public enum AdditionalJudge
    {
        ターゲットのHP割合,
        ターゲットのHP数値,
        ターゲットのMP割合,//敵のMPの扱いどうするよ。攻撃魔法の時はUIに表示しなければいい
        ターゲットの距離,
        ターゲットの高度,
        ターゲットの攻撃力,
        ターゲットの防御力,
        ターゲットのアーマー値,
        ターゲットのバフ数,
        ターゲットのデバフ数,
        プレイヤー,
        シスターさん,
        指定なし
    }




    /// <summary>
    /// 条件の判断方法
    /// アンドならかつ、ということになるので判定失敗時点で条件判断は失敗
    /// </summary>
    public enum JudgeRuLe
    {
        and条件,
        or条件,
        //   結合and条件,//他の結合and条件と結びついて一つの条件になる
        //   結合or条件
    }



    /// <summary>
    /// 行動を起こすか決めるための条件
    /// 全てのステートで使う
    /// </summary>
    public struct ActJudgeCondition
    {


        public ActCondition actCondition;

        /// <summary>
        /// 調査対象の範囲
        /// </summary>
        [HideInInspector]
        public CheckRange range;

        /// <summary>
        /// 調査対象の範囲
        /// </summary>
        [HideInInspector]
        public CheckContent content;


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


        /// <summary>
        /// この条件がandかorかを判断する
        /// アンドは絶対に満たしてないとダメ
        /// orは他を満たしてればいい。全部満たしてないとかは流石に?
        /// </summary>
        [Header("条件間の結合条件")]
        public JudgeRuLe rule;

    }

    /// <summary>
    /// ターゲット候補からターゲットを決めるための条件
    /// </summary>
    public struct TargetSelectCondition
    {
        /// <summary>
        /// これでターゲットの優先度を決める
        /// </summary>
        [Header("二個目のターゲット絞り込み条件")]
        public AdditionalJudge SecondCondition;

        [Header("昇順か降順か")]
        /// <summary>
        /// 真なら昇順
        /// </summary>
        public bool targetASOrder;


        //第二条件かぶった時のための第三条件入れるか？

        /// <summary>
        /// 第二条件で並ぶやつがいた時のための条件
        /// </summary>
        [Header("呼びのターゲット絞り込み条件")]
        public AdditionalJudge spareCondition;

        [Header("昇順か降順か")]
        /// <summary>
        /// 真なら昇順
        /// </summary>
        public bool spareASOrder;
    }



    #region ここから魔法決定条件



    public enum UseAction
    {
        攻撃行動に移行,
        支援行動に移行,
        回復行動に移行,
        なにもしない,
        現在の設定で行動
    }


    /// <summary>
    /// 順位をつける条件
    /// これは一つだけ
    /// UIでは差をつける？
    /// たとえば同じ「効果の大きさ」でも
    /// 支援では倍率にしたり攻撃では攻撃力にする？表示を変える
    /// </summary>
    public enum MagicSortCondition
    {
        魔法レベル,
        発射数,
        詠唱時間,
        効果時間,
        追尾性能,
        効果の大きさ,
        削り値,
        MP使用量,
        リジェネ回復速度,//リジェネとかはUIで表示しなければいい。
        リジェネ総回復量,//とりあえず含んどけば全てのステートで使える
        弾速,
        弾丸の大きさ,//爆発系であるなら爆発半径の大きさでもある
        指定なし
    }





    #endregion


    /// <summary>
    /// クールタイムをスキップする条件
    /// 複数選択可能
    /// </summary>
    [Flags]
    public enum SkipCondition
    {
        第一条件 = 1<<0,
        第二条件 = 1 << 1,
        第三条件 = 1 << 2,
        第四条件 = 1 << 3,
        第五条件 = 1 << 4,
        補欠条件 = 1 << 5,
        なし= 0
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

    #region ターゲット決定


    [Header("行動を起こすか判断する条件")]
    public ActJudgeCondition[] judgeCondition = new ActJudgeCondition[3];


    [Header("絞り込んだ敵からターゲットを選ぶ条件")]
    public TargetSelectCondition selectCondition;



    #endregion


    #region 魔法選択


    /// <summary>
    /// 選択する行動
    /// </summary>
    [Header("どういう行動をするか")]
    public UseAction selectAction = UseAction.なにもしない;


    /// <summary>
    /// 使用する行動の射程距離をチェックするか
    /// 使用する距離は決定したターゲット
    /// 設置系は射程距離は無視
    //・距離チェック失敗した時どうするかとかも入れる？
    ///　その場合は距離チェックなし、距離チェックしてキャンセル、距離チェックして移動とかenumで入れられるようにするか
    /// </summary>
    [Header("射程距離チェック")]
    public bool rangeCheck;

    /// <summary>
    /// 使用する行動のMPをチェックするか
    /// これあると一番MP消費高い魔術とかでも制限が出てくるから毎回使う魔術変わってキャッシュ意味なくなるよね
    /// 
    /// これも挙動変えられるようにするか
    /// mpチェックしてキャンセル、mpチェックして他の使えるやつ使う、mpチェックしてクールタイムに移行とか（クールタイム中は回復速い）
    /// </summary>
    [Header("射程距離チェック")]
    public bool mpCheck;



    /// <summary>
    /// 射線が通ってるかどうか
    /// 視線チェックする
    /// 
    /// 未実装
    /// 敵密度と視線チェックはセンサーアビリティで
    /// </summary>
    public bool rayCheck;



    /// <summary>
    /// 使用する弾丸の条件
    /// 回復魔法でも設置したり降り注いだりするかも
    /// いくつでも選べる
    /// 使用魔法をこれで絞れる
    /// ビット検査するか
    /// </summary>
    [Header("使用する魔法の弾丸特性")]
    [EnumFlags]
    public BulletType bulletCondition = BulletType.指定なし;

  
    /// 弾丸の性質が当てはまるものを選ぶ
    /// 偽なら当てはまらないものを選ぶ
    /// これはいらない、当てはまらないの選びたいなら一つ以外全部選ぶとかすればいい
 //   public bool matchCheck = true;



    /// <summary>
    /// 使用する弾丸の条件
    /// 回復魔法でも設置したり降り注いだりするかも
    /// いくつでも選べる
    /// 使用魔法をこれで絞れる
    /// </summary>
    [Header("弾丸の使用優先決定の条件")]
    public MagicSortCondition magicSort = MagicSortCondition.指定なし;


    [Header("昇順か降順か")]
    /// <summary>
    /// 真なら昇順
    /// </summary>
    public bool bulletASOder;


    #endregion

    //ここからは各魔法の独自基準で




    public bool AutoWait;//自動で使用魔法の使用MPが回復するクールタイムを作る

    /// <summary>
    /// この行動のクールタイム
    /// クールタイムゼロで何もしないなら判断はやらない
    /// </summary>
    [Header("クールタイム")]
    public float coolTime;


    //つかわないかもたぶん
    [Header("行動後この条件で判断を繰り返す回数")]
    public int reJudgeCount = 0;


    //UIチェックボックス入れて数字変える
    [Header("クールタイムをスキップする条件")]
    [Tooltip("第一1,第二2,第三4,第四8,第五16,問わず0")]
    [EnumFlags]
    /// </summary>
    public SkipCondition skipList;

}
