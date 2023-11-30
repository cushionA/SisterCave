using Guirao.UltimateTextDamage;
using Micosmo.SensorToolkit.Example;
using MoreMountains.CorgiEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static CharacterStatus;
using static Cinemachine.DocumentationSortingAttribute;
using static ControllAbillity;
using static EnemyStatus;

/// <summary>
/// 改造・コンバットAIの役割を持たせる
/// 
/// エネミー側がマネージャーに自分で登録
/// ここでは登録された情報をもとに指示を出す
/// 攻撃可能、攻撃不可の状態の振り分け
/// 他に射撃タイプの味方がいる場合は守るなどの高度な作戦もやりたいな
/// まぁまずは攻撃可能かどうかだけ
/// 後はヘイト
/// 
/// 攻撃優先は
/// ・魔物としての強さ
/// ・タイプ別？
/// ・ヘイト
/// ・距離？
/// 
/// ヘイトは攻撃、距離、あとは近くの味方の死などできまる
/// キャラタイプごとに攻撃できる数が決まってる
/// また、全体のアクティブな魔物の数でも攻撃可能数は減る？
/// また他のタイプがいるとそのタイプの攻撃可能数が減るタイプもある（雑兵とか）
/// 最低ラインみたいなのはある
/// 他のタイプのキャラいないと四人まで攻撃に参加できるが、他もたくさんいて総数からあふれる時は二人になるとか
/// 攻撃できないとかじゃなく攻撃頻度落とす？
///  
/// エリアごとに作戦決める？
/// それかキャラごとに作戦くっつける？
///　仲間が何体いるかを確認して、時間当たりの攻撃回数が制限されるように攻撃間隔を遅らせる？
///　味方数は毎フレーム確認する？
/// それか死亡や敵活性で敵数に変動起きた時だけ各エネミーのロジックを更新する？
/// あとは「体力低いエネミー」が配列の何番目かを示すようなint配列をいくつか持つ？
/// 
/// それかこのマネージャーは情報を管理するだけにするか
/// 情報を受け取った側がその情報に基づいて勝手に行動する
/// 管理する情報は各エネミーのヘイト数値、攻撃している味方の数、あとは味方の攻撃優先度
/// そして死亡イベントの発行なども行う
/// 攻撃間隔経過時、被弾時、味方死亡時、味方数変動時にマネージャーに問い合わせる
/// それか味方数変動時とかに全てのAIにこっちで計算したデータを渡す？　問い合わせされるたびに計算するのは良くない
/// そうでなくても情報変動したら用意しなおすか
/// 
/// 受け取った側の対応は例えば「自分より行動優先度が高い味方がいる時」「ヘイト値がそこまで高くないとき」「被弾時」「攻撃対象が同じ時」＝回避とか
/// これで攻撃対象が違う（シスターさん狙ってる）とか自分のヘイトが高いとかだと結果が変わる？　あとは雑兵は騎士系がプレイヤー狙ってるとシスターさん狙いやすくなるとか
/// これらは性格という形で持たせる？
/// 必要な情報は攻撃優先度、ヘイト、自分より攻撃優先が高い敵がいくらいるか
/// 属性で持たせるか？攻撃頻度については「追従（上がいると黙る）」「協力（同格や同格以上がいると攻撃頻度を分割）」「独立（勝手に攻撃し続ける）」
/// イベントに対しては各イベントごとに対応を決める
/// 味方増加イベントで増えた味方の種類ごとに対応があったり
/// 味方死亡イベントで怒り始めたり
/// 特定の味方被弾イベントでなんか起こったり（体力半分とかにする？）
/// 特定の味方がいるなら連携もあり。連携の成立条件を調べて成立してるならイベントや問い合わせからの行動が変わったりモードチェンジしたりする
/// 協力属性の敵は他の味方がメインアタッカーの時は補助的な攻撃したり黙ったり、騎士なら味方守ったり
/// あらかじめ射撃属性防衛連携やサブアタッカー連携だったりで積むか
/// なんにせよ情報を受け取って勝手に動くのが基本
/// 
/// 必要なイベントとデータを洗い出す
/// 
/// エネミーのリストに情報連動
/// プレイヤー側のNPCが死んで敵の数が減ったりしたらそれを全エネミーにフィードバック
/// しないといけない
/// 
/// </summary>
public class EnemyManager : CombatManager
{
    public static EnemyManager instance = null;






    #region 色、タグ、ダメージテキスト
    [HideInInspector] public string PMagicTag = "PlayerMagic";
    [HideInInspector] public string SMagicTag = "SisAttack";
    [HideInInspector] public string AttackTag = "Attack";
    [HideInInspector] public string JumpTag = "JumpTrigger";
    public UltimateTextDamageManager um;

    /// <summary>
    /// シスターさんの敵発見などの矢印の色
    /// </summary>
    public Color[] stateClor;

    #endregion





    #region　プレイヤー陣営のデータ関連






    /// <summary>
    /// プレイヤー陣営のNPCや洗脳敵の数
    /// </summary>
    new int count;



    #endregion

    private  void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        
    }


    private void Start()
    {
        //敵側のマネージャーを参照
        TargetManagerInstance = SManager.instance;
    }


    #region ターゲットリスト管理











    #endregion



    #region 味方管理用メソッド





    /// <summary>
    /// マネージャーの管理下から出る
    /// これ必要か？
    /// たとえ戦闘状態解除されても相手の標的であることはかわらないよね
    /// 死以外ではマネージャーから抜けれない？
    /// あるいはセグメント抜けるまで
    /// あとは洗脳で抜けたりもするか
    /// 寝返る場合のフラグも引数に入れる？個別に用意するか、メソッド
    /// </summary>

    /// <param name="_inst"></param>
    public override void EndBattle(ControllAbillity _inst)
    {
        AllyList.Remove(_inst);
    }





    /// <summary>
    /// 攻撃停止する可動かを判断する
    /// </summary>
    /// <returns></returns>
    public override bool AttackStopCheck(int target, int level, float cDistance,int needCount,int id)
    {
        int count = AllyList.Count;

        int checker = 0;

        for (int i=0;i<count;i++)
        {
            //もし真ならチェッカーを加算
            if (AllyList[i].ATBlockDistanceCheck(target,level,cDistance,id))
            {
                checker++;
            }

            //チェッカーが必要数を満たしたらもう終了
            if(checker == needCount)
            {
                return true;
            }

        }

        //必要数満たせずにループ抜けたら間違いなくfalse
        return false;
    }


    #endregion
}
