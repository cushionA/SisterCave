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
    /// 標的候補のデータ
    /// データが連続してるから配列に入れてる
    /// アクセスはやし
    /// シスターさんたちも使えるようにするか
    /// </summary>
    public new List<TargetData> _targetList;


    /// <summary>
    /// 魔物たち専用味方リスト
    /// ここからイベント飛ばしたり
    /// </summary>
    public new List<EnemyAIBase> AllyList;




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


    #region ターゲットリスト管理

    /// <summary>
    /// ターゲットリストを作成する
    /// 以後このリストの内容が変更されるまで使う
    /// NPCが消えたり洗脳魔法とけたり
    /// </summary>
    public override void SetTargetList()
    {
        count = SManager.instance.AllyList.Count;


            _targetList = new List<TargetData>(count);

        TargetListUpdate();
    }

    /// <summary>
    /// リストをアプデする
    /// 基本的にターゲットリストの内容は変わらない
    /// なんでヘイトの配列に関しては勝手にエネミー側でやってくれ
    /// </summary>
    public override void TargetListUpdate()
    {

        if (count > 0)
        {

            for (int i = 0; i < count; i++)
            {


                SManager.instance.AllyList[i].TargetDataSet(i);

            }

        }

    }


    /// <summary>
    /// ターゲットセットするときにエネミーが呼び出すメソッド
    /// データ管理に必要
    /// </summary>
    /// <param name="newTarget"></param>
    /// <param name="prevTarget"></param>
    /// <param name="_event"></param>
    /// <param name="level"></param>
    /// <param name="isFirst"></param>
    public override void TargetSet(int newTarget,int prevTarget, TargetingEvent _event,int level,int id,bool isFirst)
    {
        //初回はターゲットナンバーが絶対ゼロなので
        //マイナスにならないように
        if (isFirst) 
        {
            //旧ターゲットから減らす
            _targetList[prevTarget].targetingCount[level]--;
            _targetList[prevTarget].targetAllCount--;
        }

        //敵視加算
        _targetList[newTarget].targetingCount[level]++;
        _targetList[prevTarget].targetAllCount++;

        
        //イベントあるならイベント開始
        if (_event != TargetingEvent.なし)
        {
            int count = AllyList.Count;
            for (int i =0; i < count;i++)
            {
                AllyList[i].CommandEvent(_event,newTarget,level,id);
            }
        }
    }

    /// <summary>
    /// ゲームオブジェクトからナンバーを獲得
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>

    public override int GetTargetNumber(GameObject enemy)
    {

        int count = _targetList.Count;

        //なんもなかったらとりあえずプレイヤー狙おうという意味も込めた0
        int target=0;

        for (int i = 0; i < count; i++)
        {
            //ターゲットいたら
            if (enemy == _targetList[i].targetObj)
            {
                target = i;
                break;
            }
        }

        return target;
    }


    /// <summary>
    /// 敵要素の削除に応じて
    /// ターゲットリストの並び替えを行う
    /// キャラにもそれを反映
    /// </summary>
    /// <param name="deleteEnemy">消される敵の番号</param>
    public override void TargetListSort(int deleteEnemy)
    {
        /* for(int i = deleteEnemy ;i< count + sCount;i++)
         {
             //最後なら
             if (i == count + sCount-1)
             {
                 _targetList.RemoveAt(i);
                 break;
             }

             //消え去るエネミーを参照している敵からは
             //タゲ外す
             if (i == deleteEnemy)
             {

             }

             //次の要素を一つ前に
             _targetList[i] = _targetList[i + 1];

             //そのままdeleteEnemyをRemoveの引数にすると
             //配列のサイズ変更にタイムラグがある可能性がある？
             //いやそのままRemoveでいい
         }
        */

        _targetList.RemoveAt(deleteEnemy);

        int aCount = AllyList.Count;
        for (int i = 0; i < aCount; i++)
        {
            AllyList[i].TargetListChange(deleteEnemy);
        }
    }



    #endregion



    #region 味方管理用メソッド



    /// <summary>
    /// バトルに参加する敵がマネージャーの管理下に入る
    /// 
    /// これisAggressiveではなくセグメントから管理するか
    /// 非戦闘状態の敵もシスターさんたちにとっては攻撃対象ですから
    /// それか距離だな
    /// あるいはisRenderで管理開始して？　画面外で非アクティブになったら消える？
    /// </summary>
    /// <param name="_inst"></param>
    public override int JoinBattle(EnemyAIBase _inst)
    {


        //IDを発行
        int newID = UnityEngine.Random.Range(1, 300);
        bool isUnique = false;
            int count = AllyList.Count;
        while (!isUnique)
        {



            for (int i = 0; i < count; i++)
            {
                if (AllyList[i].ReturnID() == newID)
                {
                    isUnique = false;
                    newID = UnityEngine.Random.Range(1, 300);
                    break;
                }
                //最後まで行けたら真
                else if (i == count - 1)
                {
                    isUnique = true;
                }
            }
        }


        //ID発行したら仲間
        AllyList.Add(_inst);

        //さらにシスターさんのターゲットリストも更新
        AllyList[count].TargetDataAdd();


        //IDを差し上げる
        return newID;
    }


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
    public override void EndBattle(EnemyAIBase _inst)
    {
        AllyList.Remove(_inst);
    }

    /// <summary>
    /// 死んだエネミーが呼び出すためのメソッド
    /// 攻撃的エネミーのリスト排除から死亡状態の保存まで行う
    /// </summary>
    public override void Die(int ID,EnemyAIBase inst)
    {

        int num = AllyList.IndexOf(inst);

        //攻撃的オブジェクトのリストから破棄
        //不意打ち一撃死亡の場合は処理分ける？
        AllyList.RemoveAt(num);

        //ここから敵のターゲットリスト並び替え
        SManager.instance.TargetListSort(num);


        //ここから味方が死んだとかイベント飛ばす？
        //殺した相手にヘイトが向くとか
        //強いやつだったら逃げるとか

    }

    /// <summary>
    /// 同じターゲットを敵視してる
    /// 同レベル、あるいは上レベルの味方が何人いるか
    /// </summary>
    /// <returns></returns>
    public override int TargettingCount(int level,int target)
    {
        //この回数だけ繰り返す
        int count = 4 - level;

        //absoluteは邪魔されない
        if(count == 1)
        {
            return 0;
        }

        int sum = 0;

        for (int i = level; i <= count; i++)
        {
            sum += _targetList[target].targetingCount[i];
        }

        return sum;
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
