using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;

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
    public List<GameObject> targetList;
   // [HideInInspector] public List<GameObject> targetRecord;
    //[HideInInspector]
    public List<EnemyAIBase> targetCondition;
    [HideInInspector]public float closestEnemy;
    [HideInInspector] public GameObject playObject;
    /// <summary>
    /// プレイヤーから離れたフラグ。
    /// </summary>
    [HideInInspector] public bool isEscape;
    /// <summary>
    /// 敵死んだフラグ。これが立つとパルス飛ばして敵検索
    /// </summary>
 //  /* [HideInInspector] */public bool enemyDead;

    /// <summary>
    /// 敵をサーチしたフラグ
    /// </summary>
    [HideInInspector] public bool isSerch;
    /// <summary>
    /// 戦闘終了フラグ
    /// </summary>
    [HideInInspector] public bool isBattleEnd;
    //[HideInInspector] 
    public GameObject target;//攻撃対象
    //詠唱中かどうか
    [HideInInspector] public bool castNow;
    /// <summary>
    /// 何かしている
    /// </summary>
    [HideInInspector] public bool actNow;
    //  public Slider MpSlider;//シスターさんのMP管理
    [HideInInspector] public SisMagic useMagic;
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
                   targetCondition.Add(e.GetComponent<EnemyBase>());
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
        targetCondition.Remove(enemy.GetComponent<EnemyAIBase>());
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
            targetCondition.Add(target.GetComponent<EnemyAIBase>());
            //認識した敵に黄色のマークをつける
            targetCondition[targetCondition.Count - 1].TargetEffectCon();
        }
    }

}
