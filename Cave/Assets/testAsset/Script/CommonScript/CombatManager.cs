using Guirao.UltimateTextDamage;
using MoreMountains.CorgiEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterStatus;
using static ControllAbillity;
using static EnemyStatus;

public class CombatManager : MonoBehaviour
{



#region 定義






/// <summary>
/// 
/// 戦闘関連の情報をまとめて管理するクラス
/// 敵やプレイヤー側など陣営に一つずつこのクラスを継承したクラスを用意する
/// </summary>
public class TargetData
{


    /// <summary>
    /// 敵の種類
    /// 騎士とか射手とかそういうの
    /// みたいな不変データ
    /// </summary>
    public CharacterData _baseData;

    /// <summary>
    /// 体力の割合など可変データ
    /// </summary>
    public ConditionData _condition;



    /// <summary>
    /// レベルごとに何人にターゲットされているかを見る
    /// </summary>
    public int[] targetingCount = new int[4];

    /// <summary>
    /// 合計何人にターゲットにされてるか
    /// これを利用して今何人に見られてるかを問い合わせ可能
    /// </summary>
    public int targetAllCount;

    /// <summary>
    /// これを使って最初攻撃してきたのとか、見つけてきたのが誰なのか見つける
    /// </summary>
    public GameObject targetObj;

        /// <summary>
        /// 攻撃状態であるか
        /// </summary>
        public bool isAgressive;

        /// <summary>
        /// 今どの敵をターゲットしてるか
        /// </summary>
        public int targetNum;

        /// <summary>
        /// 個体判別用のID
        /// 死亡時の関係整理などに使う
        /// </summary>
        public int targetID;
}

#endregion







#region　データ関連

/// <summary>
/// 標的候補のデータ
/// データが連続してるから配列に入れてる
/// アクセスはやし
/// シスターさんたちも使えるようにするか
/// 通信先のクラスから獲得する
/// </summary>
public List<TargetData> _targetList;


/// <summary>
/// 専用味方リスト
/// ここからイベント飛ばしたり
/// </summary>
public List<ControllAbillity> AllyList;




/// <summary>
/// 自陣営の味方や洗脳敵の数
/// </summary>
protected int count;


    /// <summary>
    /// 敵側のコンバットマネージャー
    /// データをやり取りする
    /// </summary>
    protected CombatManager TargetManagerInstance;

    #endregion




    #region ターゲットリスト管理

    /// <summary>
    /// ターゲットリストを作成する
    /// 以後このリストの内容が変更されるまで使う
    /// NPCが消えたり洗脳魔法とけたり
    /// これ使う機会ありません
    /// TargetAddでやってくださいそうしよう
    /// </summary>
    public void SetTargetList()
    {
        count = EnemyManager.instance.AllyList.Count;


        _targetList = new List<TargetData>(count);

        TargetListUpdate();
    }


    /// <summary>
    /// オーバーライドしない
    /// 
    /// リストをアプデする
    ///敵側のマネージャーに働きかけて、そちらのAllyリストから
    ///こっちのターゲットリストを埋めてもらう
    ///
    /// </summary>
    public void TargetListUpdate()
    {

        if (count > 0)
        {

            for (int i = 0; i < count; i++)
            {
                //敵の友軍リストを通じて指示して
                //敵のターゲットデータを更新させる
                TargetManagerInstance.AllyList[i].TargetDataUpdate(i);

            }

        }
    }


    //こちらの味方死にましたよイベントとそちらのあなた狙われてますよイベント
    //あと何人にヘイト向けられてるかのカウンターをうまく機能に組み込みたい
    //そしてシスターさんのターゲティングも
    //まぁ別にプレイヤー側は人数で攻撃制限とかしないからカウントの方法はもっと単純でいいかも
    //そちらのあなた狙われてますよイベントはいらないか
    //自分のターゲットデータに今何人に狙われてますよカウントがあるからそっちを調べればいい


    /// <summary>
    /// オーバーライドしない
    /// 
    /// ターゲットセットするときにエネミーが呼び出すメソッド
    /// データ管理に必要
    /// 誰を狙ってたか、誰に狙いを変えたかなど
    /// ターゲットを報告したらIDを返してあげる
    /// </summary>
    /// <param name="newTarget"></param>
    /// <param name="prevTarget"></param>
    /// <param name="_event"></param>
    /// <param name="level"></param>
    /// <param name="isFirst"></param>
    public int TargetSet(int newTarget, int prevTarget, TargetingEvent _event, int level, int id, bool isFirst)
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



        //指令イベントあるならイベント開始
        if (_event != TargetingEvent.なし)
        {
            int count = AllyList.Count;
            for (int i = 0; i < count; i++)
            {
                AllyList[i].CommandEvent(_event, newTarget, level, id);
            }
        }

        //IDを返してあげる
        return _targetList[newTarget].targetID;
    }

    /// <summary>
    /// オーバーライドしない
    /// 
    /// 敵オブジェクトからナンバーを獲得
    /// 何番目の配列にいるか
    /// なにもいなければ-1
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>

    public int GetTargetNumberByObject(GameObject triggerEnemy)
    {

        int count = _targetList.Count;

        //なんもなかったら-1
        int target = -1;

        for (int i = 0; i < count; i++)
        {
            
            //ターゲットいたら
            if (triggerEnemy == _targetList[i].targetObj)
            {
                target = i;
                break;
            }
        }

        return target;
    }


    /// <summary>
    /// オーバーライドしない
    /// 
    /// 敵IDからナンバーを獲得
    /// 何番目の配列にいるかのナンバー
    /// もしいなければ-1を返す
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>

    public int GetTargetNumberByID(int targetID)
    {

        int count = _targetList.Count;

        //なんもなかったらとりあえずプレイヤー狙おうという意味も込めた0
        int target = -1;

        for (int i = 0; i < count; i++)
        {

            //ターゲットいたら
            if (targetID == _targetList[i].targetID)
            {
                target = i;
                break;
            }
        }

        return target;
    }


    /// <summary>
    /// オーバーライドしない
    /// 
    /// 敵要素の削除に応じて敵の戦闘マネージャーから呼び出される
    /// 敵がこっちの仲間死にましたよと教えてくれる形
    /// こちら側のターゲットリストの並び替えを行う
    /// キャラにもそれを反映
    /// </summary>
    /// <param name="deleteEnemy">消される敵の番号</param>
    public void TargetListSort(int deleteEnemy)
    {

        //死亡した敵のIDも渡す
        //これによって現在ターゲティングしてる敵が
        int deadID = _targetList[deleteEnemy].targetID;

        _targetList.RemoveAt(deleteEnemy);

        int aCount = AllyList.Count;
        for (int i = 0; i < aCount; i++)
        {
            AllyList[i].TargetListChange(deleteEnemy, deadID);
        }

    }



    #endregion



    #region 味方管理用メソッド


    /// <summary>
    /// バトルに参加する敵がマネージャーの管理下に入る
    /// 
    /// これisAggressiveではなくセグメントから管理するか
    /// 非戦闘状態の敵もシスターさんたちにとっては攻撃対象
    /// それか距離
    /// あるいはisRenderで管理開始して？　画面外で非アクティブになったら消える？
    /// 
    /// 共通化可能
    /// </summary>
    /// <param name="_inst"></param>
    public void JoinBattle(ControllAbillity _inst)
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

        //ターゲットリストも更新
        //Smanagerに自分の情報を渡す
        AllyList[count].TargetDataAdd(newID);

    }


    /// <summary>
    /// マネージャーの管理下から出る
    /// これいらないかも
    /// たとえ戦闘状態解除されても相手の標的であることはかわらないよね
    /// 死以外ではマネージャーから抜けれない？
    /// あるいはセグメント抜けるまで
    /// あとは洗脳で抜けたりもするか
    /// 寝返る場合のフラグも引数に入れる？個別に用意するか、メソッド
    /// </summary>
    /// <param name="_inst"></param>
    public virtual void EndBattle(ControllAbillity _inst)
    {

    }




    /// <summary>
    /// オーバーライドしない
    /// 
    /// 死んだエネミーが呼び出すためのメソッド
    /// 攻撃的エネミーのリスト排除から死亡状態の保存まで行う
    /// </summary>
    public void Die(int ID, EnemyAIBase inst)
    {
        int num = AllyList.IndexOf(inst);

        //攻撃的オブジェクトのリストから破棄
        //不意打ち一撃死亡の場合は処理分ける？
        AllyList.RemoveAt(num);

        //ここから敵のターゲットリスト並び替え
        TargetManagerInstance.TargetListSort(num);


        //ここから味方が死んだとかイベント飛ばす？
        //殺した相手にヘイトが向くとか
        //強いやつだったら逃げるとか

    }




    /// <summary>
    /// オーバーライドする
    /// 
    /// 同じターゲットを敵視してる
    /// 同レベル、あるいは上レベルの味方が何人いるか数える
    /// </summary>
    /// <returns></returns>
    public virtual int TargettingCount(int level, int target)
    {
        //この回数だけ繰り返す
        int count = 4 - level;

        //absoluteは邪魔されない
        if (count == 1)
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
    /// オーバーライドする
    /// </summary>
    /// <returns></returns>
    public virtual bool AttackStopCheck(int target, int level, float cDistance, int needCount, int id)
    {
        return false;
    }


    #endregion



}
