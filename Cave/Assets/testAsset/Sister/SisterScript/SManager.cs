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

    public Vector2 sisterPosition;



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

    private void Start()
    {
        //敵側のマネージャーを参照
        TargetManagerInstance = EnemyManager.instance;
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


    #region シスターさんの移動判断に使う

    /// <summary>
    /// ターゲットリストから一番近い、あるいは遠い敵を返す
    /// パラメータが真なら遠い敵を返す
    /// </summary>
    /// <param name="isFar"></param>
    /// <returns></returns>
    public TargetData GetClosestEnemy(bool isFar = false)
    {
        Vector2 container = Vector2.zero;


        //距離の二乗を求めて遠近判断すると
        //平方根が走らないからエコだよ
        container.MMSetY(Vector2.SqrMagnitude(sisterPosition-_targetList[0]._condition.targetPosition));

        int count = _targetList.Count;

        if(count > 1)
        {
            float distance;
            //0はもう済ませてる
            for (int i = 1;i< count;i++)
            {
                distance = Vector2.SqrMagnitude(sisterPosition - _targetList[i]._condition.targetPosition);

                //遠い方がいい、isFarなら遠ければ入れ替え
                //近い方がいいなら近ければ入れ替え
                if (isFar ? (container.y < distance) : (container.y > distance))
                {
                    container.Set(i,distance);
                }
            }
        }

        //条件に合う敵を返す
        return _targetList[(int)container.x];
    }

    /// <summary>
    /// ターゲットリストで一番高い敵を返す
    /// パラメータが真なら低い敵を返す
    /// </summary>
    /// <param name="isLow"></param>
    /// <returns></returns>
    public TargetData GetHighestEnemy(bool isLow = false)
    {
        Vector2 container = Vector2.zero;


        //距離の二乗を求めて遠近判断すると
        //平方根が走らないからエコだよ
        container.MMSetY(_targetList[0]._condition.targetPosition.y);

        int count = _targetList.Count;

        if (count > 1)
        {
            //どの高度にいるか
            float point;
            //0はもう済ませてる
            for (int i = 1; i < count; i++)
            {
                point = _targetList[i]._condition.targetPosition.y;

                //低い方がいい、isLowなら低ければ入れ替え
                //高い方がいいなら高ければ入れ替え
                if (isLow ? (container.y > point) : (container.y < point))
                {
                    container.Set(i, point);
                }
            }
        }

        //条件に合う敵を返す
        return _targetList[(int)container.x];
    }


    /// <summary>
    /// 一番強い敵を返すメソッド
    /// </summary>
    /// <param name="isFar"></param>
    /// <returns></returns>
    public TargetData GetStrongestEnemy()
    {

        int count = _targetList.Count;

        int useNum = 0;

        if (count > 1)
        {

            //0はもう済ませてる
            for (int i = 0; i < count; i++)
            {
                //仮にまだ強敵が見つかってないなら強敵かボスで上書き
                if (useNum == 0)
                {
                    if (_targetList[i]._baseData._type == CharacterStatus.CharaType.Boss || _targetList[i]._baseData.isStrong)
                    {
                        useNum = i;
                    }

                }
                //仮に初期化済みならボスしか上書きできない
                else if (_targetList[i]._baseData._type == CharacterStatus.CharaType.Boss)
                {
                    //ボスならそれを返す
                    //そしてもう続ける意味もない
                    return _targetList[i];

                }
            }
        }

        return _targetList[useNum];

    }


    /// <summary>
    /// 指定のポイントからどちらに行けば敵が少ないかを見る
    /// 真なら左側が敵多い
    /// </summary>
    public bool MoreEnemySide(float basePoint)
    {

        int count = _targetList.Count();


        int left = 0;

        //基準より左のやつを数える
        for(int i = 0; i < count; i++)
        {
            //ベースポイントより左ならカウント
            if (_targetList[i]._condition.targetPosition.x < basePoint)
            {
                left++;
            }

        }

        //左の方が敵の数多いなら左サイドを返す
        return left > (count - left);


    }

    #endregion

    
    
    #region プレイヤーのロックオンに使う


    /// <summary>
    /// プレイヤーのロック処理を行う
    /// ターゲットリストの何番目が送られてくるのか
    /// </summary>
    /// <param name="number">今現在ターゲットにしてる番号</param>
    /// <param name="range">ロックオン限界距離</param>
    /// <param name="isRight">プレイヤーが右を向いてるか</param>
    /// <param name="isFar">ロックオン変更先は今の敵より遠いか近いか</param>
    /// <returns>99は何も相手がいない番号</returns>
    public int PlayerLockEnemySelect(int number,float range,bool isRight,bool isFar)
    {

        int count = _targetList.Count;

        Vector2 position = GManager.instance.PlayerPosition;


        if (count > 1)
        { 

                 Vector2 container = Vector2.zero;
               
            float distance;

            //最大最小を求める
            if (number == 99)
            {

                //xを99に書き変え
                container.MMSetX(99);


                //まず0の敵の判断を行う

                //右か左か
                //右なら右にいるキャラだけ
                //プレイヤーのx座標から敵のx座標を引いた数値が0以下になる（＝右にいる）ことの真偽と
                //右向いてるかどうかフラグが一致しないなら次のループに
                //向いてる方向にいる敵だけ判断する
                //0の敵が右にいるかをまずチェック
                if ((position.x - _targetList[0]._condition.targetPosition.x <= 0) != isRight)
                {
                    distance = Vector2.SqrMagnitude(position - _targetList[0]._condition.targetPosition);

                    //距離をクリアできるなら
                    if (distance <= Mathf.Pow(range, 2))
                    {
                    container.Set(0,Vector2.SqrMagnitude(position - _targetList[0]._condition.targetPosition));
                    }

                }

                //0はもう済ませてる
                for (int i = 1; i < count; i++)
                {

                    //向いてる方向にいる敵だけ判断する
                    if ((position.x - _targetList[i]._condition.targetPosition.x <= 0) != isRight)
                    {
                        continue;
                    }

                    //距離の二乗
                    distance = Vector2.SqrMagnitude(position - _targetList[i]._condition.targetPosition);

                    //距離検査をクリアできないなら次のループに
                    if (distance > Mathf.Pow(range, 2))
                    {
                        continue;
                    }

                    //最初のループなら
                    if (container.x == 99)
                    {
                        container.Set(i, distance);
                    }
                    //遠い方がいい、isFarなら遠ければ入れ替え
                    //近い方がいいなら近ければ入れ替え
                    else if (isFar ? (container.y < distance) : (container.y > distance))
                    {
                        container.Set(i, distance);
                    }
                }

            }
            //一つずらす
            else
            {
                //ベースとなる距離を入れる
                float baseDistance = Vector2.SqrMagnitude(position - _targetList[number]._condition.targetPosition);

                //最初であることを判別するための数値
                container.MMSetX(999);

                //ループで差を見ていく
                for (int i = 0; i < count; i++)
                {

                    //右か左か
                    //右なら右にいるキャラだけ
                    //プレイヤーのx座標から敵のx座標を引いた数値が0以下になる（＝右にいる）ことの真偽と
                    //右向いてるかどうかフラグが一致しないなら次のループに
                    //向いてる方向にいる敵だけ判断する
                    //または現在の番号と同じ敵なら
                    if (i == number || (position.x - _targetList[i]._condition.targetPosition.x <= 0) != isRight)
                    {
                        continue;
                    }

                    //距離の差を入れる
                    //absしちゃだめ
                    //遠いか近いかを判断する上で大切だから
                    distance = Vector2.SqrMagnitude(position - _targetList[i]._condition.targetPosition);

                    //距離検査をクリアできないなら次のループに
                    if (distance > Mathf.Pow(range, 2))
                    {
                        continue;
                    }

                    //距離を基準距離との差に変換
                    distance = distance - baseDistance;

                    //最初なら
                    if (container.x == 999)
                    {
                        container.Set(i,Mathf.Abs(distance));
                    }

                    //距離が基準と同じなら、あるいは近いか遠いかを判断して合格なら
                    //遠いに当てはまるならisFarが真で(距離‐現在の基準距離)が0以上になる
                    //近いならマイナスになるはず
                    else if (distance ==0 || isFar == (distance > 0))
                    {
                        //絶対値を取得
                        distance = Mathf.Abs(distance);

                        //現在の保留値より差が小さいなら
                        //数値を入れ替え
                        if(distance <= container.y)
                        {
                            //つまり今は自分の向いてる方にいて
                            //なおかつ基準となる距離より近く（遠く）
                            //それでいてベースの距離に最も近い敵が選ばれてる
                        container.Set(i, distance);
                        }


                    }
                }
            }



        //条件に合う敵を返す
        return (int)container.x;


        }
        //一人だけしか敵がいないなら
        //方向と範囲だけチェック
        else
        {
            //右か左か
            //右なら右にいるキャラだけ
            //プレイヤーのx座標から敵のx座標を引いた数値が0以下になる（＝右にいる）ことの真偽と
            //右向いてるかどうかフラグが一致するならこいつが唯一のターゲット
            if ((position.x - _targetList[0]._condition.targetPosition.x <= 0) == isRight)
            {
                //距離もクリアするなら
                if(Vector2.SqrMagnitude(position - _targetList[0]._condition.targetPosition) <= Mathf.Pow(range, 2))
                {
                    return 0;
                }

            }

            //何もターゲットがいない番号
            return 99;
        }


    }



    #endregion


    /*   private void FixedUpdate()
       {
           if (isDEEEp)
           {
               EnemyDeath(NIdoit);
               isDEEEp = false;
           }
       }*/


    #region 旧ターゲット関連

    /// <summary>
    /// ターゲットとそのAIを格納して黄色マークつけてあげる
    /// 
    /// 黄色マークつける機能だけ残して
    /// 
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

#endregion


    #region ターゲットリスト管理



    /// <summary>
    /// オーバーライドする
    /// 
    /// 同じターゲットを敵視してる
    /// 同レベル、あるいは上レベルの味方が何人いるか数える
    /// こっちはもうちょい単純でいいかも
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






    #endregion



    #region 味方管理用メソッド





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
