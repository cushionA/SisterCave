using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using PathologicalGames;
using System.Linq;
using static EnemyStatus;

public class SManager : CombatManager
{
    public static SManager instance = null;

    [HideInInspector]public string reactionTag = "Dialog";
    [HideInInspector] public string dangerTag = "Danger";
    [HideInInspector] public string enemyTag = "Enemy";
    public GameObject Sister;
    //プレイヤーオブジェクト
    public SisterStatus sisStatus;
    //プレイヤーのステータスを取得
    //[HideInInspector] public GameObject targetObj;
    public　List<SisMagic> attackMagi;
    public List<SisMagic> supportMagi;
    public List<SisMagic> recoverMagi;
    public List<GameObject> targetList = new List<GameObject>();
   // [HideInInspector] public List<GameObject> targetRecord;
    //[HideInInspector]
    public List<EnemyAIBase> targetCondition = new List<EnemyAIBase>();
    [HideInInspector]public float closestEnemy;
    [HideInInspector] public GameObject playObject;





    /// <summary>
    /// 敵死んだフラグ。これが立つとパルス飛ばして敵検索
    /// </summary>
 //  /* [HideInInspector] */public bool enemyDead;

    /// <summary>
    /// 敵をサーチしたフラグ
    /// サーチした後の状況で行動を変える
    /// </summary>
    [HideInInspector] public bool isSerch;
    /// <summary>
    /// 戦闘終了フラグ
    /// </summary>
    [HideInInspector] public bool isBattleEnd;
    //[HideInInspector] 
    public GameObject target;//攻撃対象

    [HideInInspector]
    /// <summary>
    /// 射撃中消えないように保持するターゲット
    /// </summary>
    public GameObject restoreTarget;
    //詠唱中かどうか
    [HideInInspector] public bool castNow;
    /// <summary>
    /// 何かしている
    /// </summary>
    [HideInInspector] public bool actNow;
    //  public Slider MpSlider;//シスターさんのMP管理
    [HideInInspector] public SisMagic useMagic;

    [HideInInspector] public float useAngle;


    private readonly UniTaskCompletionSource
    uniTaskCompletionSource = new UniTaskCompletionSource();

    public UniTask _addAsync => uniTaskCompletionSource.Task;


    #region　敵のデータ関連

    /// <summary>
    /// 標的候補のデータ
    /// データが連続してるから配列に入れてる
    /// アクセスはやし
    /// シスターさんたちも使えるようにするか
    /// </summary>
    public new List<TargetData> _targetList;


    /// <summary>
    /// プレイヤー側専用味方リスト
    /// ここからイベント飛ばしたり
    /// </summary>
    public new List<ControllAbillity> AllyList;




    #endregion



    private void Awake()
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


    
    #region 魔法設定

    /// <summary>
    /// 魔法の攻撃力を設定
    /// 表示火力もここで設定するか
    /// </summary>
    public void SetMagicAtk()
    {
        if (useMagic.mType == SisMagic.MagicType.Attack)
        {
            if (useMagic.phyBase >= 1)
            {
                useMagic.phyAtk = useMagic.phyBase + (useMagic.intCurve.Evaluate(sisStatus._int)) +
                                   useMagic.faithCurve.Evaluate(sisStatus.faith);
            }
            if (useMagic.holyBase >= 1)
            {
                useMagic.holyAtk = useMagic.phyBase + (useMagic.intCurve.Evaluate(sisStatus._int)) +
                                   useMagic.faithCurve.Evaluate(sisStatus.faith);
            }
            if (useMagic.darkBase >= 1)
            {
                useMagic.darkAtk = useMagic.phyBase + (useMagic.intCurve.Evaluate(sisStatus._int)) +
                                   useMagic.faithCurve.Evaluate(sisStatus.faith);
            }
            if (useMagic.fireBase >= 1)
            {
                useMagic.fireAtk = useMagic.phyBase + (useMagic.intCurve.Evaluate(sisStatus._int)) +
                                   useMagic.faithCurve.Evaluate(sisStatus.faith);
            }
            if (useMagic.thunderBase >= 1)
            {
                useMagic.thunderAtk = useMagic.phyBase + (useMagic.intCurve.Evaluate(sisStatus._int)) +
                                   useMagic.faithCurve.Evaluate(sisStatus.faith);
            }
        }
    }

    /// <summary>
    /// 回復量セット
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public float SetRecoverAmount(SisMagic s)
    {
        if (useMagic.mType == SisMagic.MagicType.Recover)
        {
            useMagic.recoverAmount = useMagic.recoverBase + (useMagic.intCurve.Evaluate(sisStatus._int)) +
                   useMagic.faithCurve.Evaluate(sisStatus.faith);
        }
        return useMagic.recoverAmount;
    }


    /// <summary>
    /// 魔法の分類
    /// </summary>
    public void MagicClassify()
    {
        attackMagi.Clear();
        supportMagi.Clear();
        recoverMagi.Clear();

        for (int i = 0; i < sisStatus.equipMagic.Count;i++)
        {
        
            if (sisStatus.equipMagic[i].mType == SisMagic.MagicType.Attack)
            {
         //       Debug.Log($"ssss{sisStatus.equipMagic[i].name}");
                if(sisStatus.equipMagic[i] != null)
                {
      attackMagi.Add(sisStatus.equipMagic[i]);
                }

            }
            else if (sisStatus.equipMagic[i].mType == SisMagic.MagicType.Support)
            {
                supportMagi.Add(sisStatus.equipMagic[i]);
            }
            else if (sisStatus.equipMagic[i].mType == SisMagic.MagicType.Recover)
            {
                recoverMagi.Add(sisStatus.equipMagic[i]);
            }
        }
    }

    /// <summary>
    /// 魔法装備変更時にエフェクトを積みなおす
    /// </summary>
    /// <param name="ac"></param>
    public void MagicEffectSet(AtEffectCon ac)
    {
       // AtEffectCon ac = Sister.MMGetComponentNoAlloc<AtEffectCon>();
        List<PrefabPool> _newPrefab = new List<PrefabPool>();

        //魔法の設定
        if (sisStatus.equipMagic.Any())
        {
            for (int i = 0; i < sisStatus.equipMagic.Count; i++)
            {

                //使用する魔法のエフェクトをパッキング
                for (int s = 0; s < sisStatus.equipMagic[i]._usePrefab.Length; s++)
                {
                    PrefabPool pool = new PrefabPool();
                    pool.prefab = sisStatus.equipMagic[i]._usePrefab[s].prefab;
                    pool.preloadAmount = sisStatus.equipMagic[i]._usePrefab[s].preloadAmount;
                    _newPrefab.Add(pool);
                }

                //ここから子弾見ていく
                Magic magi = sisStatus.equipMagic[i];

                //子どもがなくなるまで
                while (magi.childM != null)
                {
                    for (int s = 0; s < magi.childM._usePrefab.Length; s++)
                    {
                        PrefabPool pool = new PrefabPool();
                        pool.prefab = magi.childM._usePrefab[s].prefab;
                        pool.preloadAmount = magi.childM._usePrefab[s].preloadAmount;
                        _newPrefab.Add(pool);
                    }
                    magi = magi.childM;
                }

                ac.MagicResorceReset(_newPrefab);

                _newPrefab.Clear();
            }
        }
        else
        {
            ac.MagicResorceReset(null);
        }

        //ここからコンビネーションエフェクトも格納？
    }

    #endregion


    public void BattleEndCheck()
    {
        if (isSerch)
        {
            if (targetList.Count == 0)
            {
               // Debug.Log("サーチ");
                isBattleEnd = true;
                //isSerch = false;
            }

                isSerch = false;
        }
    }

    public void GetClosestEnemyX()
    {

        if(targetList.Count != 0)
        closestEnemy = targetList[0].transform.position.x;

    }

 /*   private void FixedUpdate()
    {
        if (isDEEEp)
        {
            EnemyDeath(NIdoit);
            isDEEEp = false;
        }
    }*/


    /// <summary>
    /// ターゲットとそのAIを格納して黄色マークつけてあげる
    /// </summary>
    /// <param name="target"></param>
    public void TargetAdd(List<GameObject> next)
    {
        //以前のターゲットリストを作成
        //敵が自分で何秒センサーに入ってないと計算してくれるので
        //継続して持つ必要はなくローカル変数に
        List<GameObject> LastTarget = new List<GameObject>();
        List<EnemyAIBase> LastCondition = new List<EnemyAIBase>();

        for (int i = 0;i < targetList.Count;i++)
        {
            //検知したオブジェクトにターゲットの要素が含まれてないなら追加
            //これによりさっきまで検知されてたけど今は検知されてないオブジェクトのリストができる
            if (!next.Contains(targetList[i]))
            {
                LastTarget.Add(targetList[i]);
                LastCondition.Add(targetCondition[i]);

                //この敵は認識されてない
                targetCondition[i].SisterRecognition(false);
            }
        }
        //検出した敵からリストを作成し、情報はいったん消す

        targetList　= new List<GameObject>(next);
       
        targetCondition.Clear();

        //センサーに検知された敵の情報取得
        for (int i = 0; i < targetList.Count;i++)
        {
            targetCondition.Add(targetList[i].MMGetComponentNoAlloc<EnemyAIBase>());

            //この敵は認識されてるものとする
            targetCondition[i].SisterRecognition(true);

            //ついでにロックカーソルも
            targetCondition[i].TargetEffectCon(0);
        }   
        //Debug.Log($"かず{targetList.Count}d{targetCondition.Count}");
            //次は検出されなかった敵がまだ削除猶予範囲内にいるかどうかを確認
            for (int i = 0; i < LastCondition.Count; i++)
        {
            //シスターさんに見られてなければ削除
            if (LastCondition[i].SisterCheck())
            {
                LastTarget[i] = null;

                //ロックカーソルを消す
                LastCondition[i].TargetEffectCon(1);

                LastCondition[i] = null;
            }
        }



        //ついでにちゃんと削除猶予された敵がいるなら情報ごとリストにぶち込む
        if (LastTarget.Count > 0)
        {
            for (int i = 0; i < LastTarget.Count;i++)
            {
                if (LastTarget[i] != null)
                {
                    targetList.Add(LastTarget[i]);
                    targetCondition.Add(LastCondition[i]);
                }
            }
        }

        //処理終了通知
        uniTaskCompletionSource.TrySetResult();
    }

    /// <summary>
    /// 最初に敵を見つけた時のやつ
    /// </summary>
    /// <param name="next"></param>
    public void InitialAdd(List<GameObject> next)
    {
       //// Debug.Log($"最初");
        targetList = new List<GameObject>(next);
        //センサーに検知された敵の情報取得
        for (int i = 0; i < targetList.Count; i++)
        {
            targetCondition.Add(targetList[i].MMGetComponentNoAlloc<EnemyAIBase>());

            //この敵は認識されてるものとする
            targetCondition[i].SisterRecognition(true);

            //ついでにロックカーソルも
            targetCondition[i].TargetEffectCon(0);
        }
       // Debug.Log($"最初の数{targetList.Count}dd{targetCondition.Count}");
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (targetList.Contains(enemy))
        {
            int index = targetList.IndexOf(enemy);
            List<GameObject> delList  = new List<GameObject>();
            List<EnemyAIBase> delCondition = new List<EnemyAIBase>();

            for (int i = 0;i < targetList.Count;i++)
            {
                if(i == index)
                {
                    continue;
                }
                delList.Add(targetList[i]);
                delCondition.Add(targetCondition[i]);
            }

            targetList = new List<GameObject>(delList);
            targetCondition = new List<EnemyAIBase>(delCondition);
        }
    }

    /// <summary>
    /// 逃げるべき標的の位置を教える
    /// </summary>
    /// <returns></returns>
    public void EscapePosition(ref Vector2 setVector)
    {
        if (!(targetList.Count > 0 && targetList != null))
        {
            setVector.Set(0f,0f);
            return;
        }
        else
        {
            //一番Xポジションが大きいやつ
            float highest = 0;
            //一番Xポジションが小さいやつ
            float lowest = 0;
            bool first = false;

            for (int i = 0;i < targetList.Count; i++)
            {
                if (targetList == null)
                {
                    highest = 0;
                    lowest = highest;
                    break;
                }

                if (targetList[i] == null)
                {
                    continue;
                }

                if (!first)
                {
                    highest = targetList[i].transform.position.x;
                    lowest = highest;
                    first = true;
                }
                else
                {
                    highest = targetList[i].transform.position.x > highest ? targetList[i].transform.position.x : highest;
                    lowest = targetList[i].transform.position.x < lowest ? targetList[i].transform.position.x : lowest;
                }
            }
            setVector.Set(lowest,highest);



        }
    }




    #region ターゲットリスト管理

    /// <summary>
    /// ターゲットリストを作成する
    /// 以後このリストの内容が変更されるまで使う
    /// NPCが消えたり洗脳魔法とけたり
    /// これ使う機会ありません
    /// TargetAddでやってくださいそうしよう
    /// </summary>
    public override void SetTargetList()
    {
        count = EnemyManager.instance.AllyList.Count;


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

                EnemyManager.instance.AllyList[i].TargetDataSet(i);

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
    public override void TargetSet(int newTarget, int prevTarget, TargetingEvent _event, int level, int id, bool isFirst)
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
        //でも多分こっちにはコマンドイベント…あるか
        //NPCとかは特に、複数だと
        if (_event != TargetingEvent.なし)
        {
            int count = AllyList.Count;
            for (int i = 0; i < count; i++)
            {
                AllyList[i].CommandEvent(_event, newTarget, level, id);
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
        int target = 0;

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
    /// バトルに参加するキャラがマネージャーの管理下に入る
    /// Sマネジャーでは最初にプレイヤーとシスターさんが入る
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
                //かぶった時点でブレイク
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

        //さらに敵のターゲットリストも更新
        AllyList[count].TargetDataAdd();


        //IDを差し上げる
        return newID;
    }


    /// <summary>
    /// マネージャーの管理下から出る
    /// これ必要か？
    /// たとえ戦闘状態解除されても相手の標的であることはかわらないよね
    /// 死以外ではマネージャーから抜けれない？
    /// あるいはセグメント抜けるまで？
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
    public override void Die(int ID, EnemyAIBase inst)
    {
        int num = AllyList.IndexOf(inst);

        //攻撃的オブジェクトのリストから破棄
        //不意打ち一撃死亡の場合は処理分ける？
        AllyList.RemoveAt(num);

        //ここから敵のターゲットリスト並び替え
        EnemyManager.instance.TargetListSort(num);


        //ここから味方が死んだとかイベント飛ばす？
        //殺した相手にヘイトが向くとか
        //強いやつだったら逃げるとか

    }

    /// <summary>
    /// 同じターゲットを敵視してる
    /// 同レベル、あるいは上レベルの味方が何人いるか
    /// </summary>
    /// <returns></returns>
    public override int TargettingCount(int level, int target)
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
    /// これはこっちにはいらない
    /// </summary>
    /// <returns></returns>
    public override bool AttackStopCheck(int target, int level, float cDistance, int needCount, int id)
    {
        //常にお控えしない
        return false;
    }


    #endregion
}
