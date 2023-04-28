using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
public class SManager : MonoBehaviour
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

    //[SerializeField] GameObject NIdoit;
    //[SerializeField] bool isDEEEp;

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
      //  sisStatus.mp = sisStatus.maxMp;
        MagicClassify();
        //SetMagicAtk();
    }

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

    /*   public void GetEnemyCondition()
       {
           if (targetList == targetRecord)
           {
               return;
           }
           else
           {
               //最初は絶対こっち
               //ターゲットの状態を取得
               targetRecord = targetList;
               targetCondition.Clear();
               foreach (GameObject e in targetRecord)
               {
                   targetCondition.Add(e.MMGetComponentNoAlloc<EnemyBase>());
               }
           }
       }*/



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

    public void EnemyDeath(GameObject enemy)
    {
        targetList.Remove(enemy);
        targetCondition.Remove(enemy.MMGetComponentNoAlloc<EnemyAIBase>());
        if (targetList.Count == 0)
        {
            isSerch = true;
        }
    }
    /// <summary>
    /// ターゲットとそのAIを格納して黄色マークつけてあげる
    /// </summary>
    /// <param name="target"></param>
    public void TargetAdd(GameObject target)
    {
        if (!targetList.Contains(target))
        {
            targetList.Add(target);
            targetCondition.Add(target.MMGetComponentNoAlloc<EnemyAIBase>());
            //認識した敵に黄色のマークをつける
            targetCondition[targetCondition.Count - 1].TargetEffectCon();
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



}
