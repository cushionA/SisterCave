using Guirao.UltimateTextDamage;
using MoreMountains.CorgiEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterStatus;
using static ControllAbillity;
using static EnemyStatus;

public abstract class CombatManager : MonoBehaviour
{



#region 定義






/// <summary>
/// 敵情報パック
/// 敵配列を作ってそこから情報を獲得する感じ
/// そのデータが敵配列の何番目なのかと言うのを記録しておくことで順番入れ替わってもいい
/// 
/// 位置、種類、HP割合、MP割合、バフついてるか、デバフか、攻撃力、防御力
/// 何体の敵にタゲ取られてるか、レベルごとに管理
/// あとは本体への参照をどうするかな
/// ゲームオブジェクトのリストを出そう。そこでintでリストに結びつける？
/// でもシスターさんが並べ替えちゃうよね
/// どうしよう
/// センサーで取得したゲームオブジェクトをDictionaryにツッコんでその分だけ敵視するか？
/// シスターさんの場合な
/// 
/// これってEnemy側からするとプレイヤーたちだよな
/// 
/// 
/// 敵はほぼ追わないようにする？
/// 決めた範囲だけ、セグメントだけ
/// セグメントはでない。そんな追いかけてくるゲーム無かったろ
/// セグメントから離れたら子オブジェクトから切り離される
/// シスターさんと味方はSManagerにアクセスしろよ。NPCマネージャーに変えて
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
/// 魔物たち専用味方リスト
/// ここからイベント飛ばしたり
/// </summary>
public List<EnemyAIBase> AllyList;




/// <summary>
/// 自陣営の味方や洗脳敵の数
/// </summary>
protected int count;


    #endregion




    #region ターゲットリスト管理

    /// <summary>
    /// ターゲットリストを作成する
    /// 以後このリストの内容が変更されるまで使う
    /// NPCが消えたり洗脳魔法とけたり
    /// ここでどうやって情報の順番を保証するか
    /// ヘイトとかも全部バラバラになる
    /// 敵が死んだりしたのを通知して参照変えるか
    /// </summary>
    public abstract void SetTargetList();


    /// <summary>
    /// リストをアプデする
    /// 基本的にターゲットリストの内容は変わらない
    /// なんでヘイトの配列に関しては勝手にエネミー側でやってくれ
    /// </summary>
    public abstract void TargetListUpdate();


    /// <summary>
    /// ターゲットセットするときにエネミーが呼び出すメソッド
    /// どれをターゲットにするのかをこちらに伝える
    /// データ管理に必要
    /// </summary>
    /// <param name="newTarget"></param>
    /// <param name="prevTarget"></param>
    /// <param name="_event"></param>
    /// <param name="level"></param>
    /// <param name="isFirst"></param>
    public abstract void TargetSet(int newTarget, int prevTarget, TargetingEvent _event, int level, int id, bool isFirst);


    public abstract int GetTargetNumber(GameObject enemy);



    #endregion



    #region 味方管理用メソッド



    /// <summary>
    /// バトルに参加する敵がマネージャーの
    /// 管理下に入る
    /// </summary>
    /// <param name="_inst"></param>
    public abstract int JoinBattle(EnemyAIBase _inst);

    /// <summary>
    /// マネージャーの管理下から出る
    /// </summary>
    /// <param name="_inst"></param>
    public abstract void EndBattle(EnemyAIBase _inst);

    /// <summary>
    /// 死んだエネミーが呼び出すためのメソッド
    /// 攻撃的エネミーのリスト排除から死亡状態の保存まで行う
    /// </summary>
    public abstract void Die(int ID, EnemyAIBase inst);

    /// <summary>
    /// 同じターゲットを敵視してる
    /// 同レベル、あるいは上レベルの味方が何人いるか
    /// </summary>
    /// <returns></returns>
    public abstract int TargettingCount(int level, int target);


    /// <summary>
    /// 攻撃停止する可動かを判断する
    /// </summary>
    /// <returns></returns>
    public abstract bool AttackStopCheck(int target, int level, float cDistance, int needCount, int id);

    public abstract void TargetListSort(int deleteEnemy);

    #endregion


    #region 敵陣営のマネージャーとの連携


    /// <summary>
    /// こちら側のAllyListから敵に送信する
    /// 敵のターゲットリストに情報を入れる
    /// でもこれだといまのターゲットリストの順番が保証されないよね
    /// 特に敵側のエネミーが死んだ時とか
    /// </summary>
   // public abstract void SendData();


    #endregion

}
