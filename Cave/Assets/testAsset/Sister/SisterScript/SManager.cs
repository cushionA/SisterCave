using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public List<EnemyBase> targetCondition;
    [HideInInspector]public float closestEnemy;
    [HideInInspector] public GameObject playObject;
    [HideInInspector] public bool isEscape;//プレイヤー離れたフラグ。
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
    [HideInInspector] public bool castNow;
    [HideInInspector] public bool actNow;
    //  public Slider MpSlider;//シスターさんのMP管理

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
        sisStatus.mp = sisStatus.maxMp;
        MagicClassify();
        //SetMagicAtk();
    }

    public void SetMagicAtk()
    {
        if (sisStatus.useMagic.mType == SisMagic.MagicType.Attack)
        {
            if (sisStatus.useMagic.phyBase >= 1)
            {
                sisStatus.useMagic.phyAtk = sisStatus.useMagic.phyBase + (sisStatus.useMagic.intCurve.Evaluate(sisStatus._int)) +
                                   sisStatus.useMagic.faithCurve.Evaluate(sisStatus.faith);
            }
            if (sisStatus.useMagic.holyBase >= 1)
            {
                sisStatus.useMagic.holyAtk = sisStatus.useMagic.phyBase + (sisStatus.useMagic.intCurve.Evaluate(sisStatus._int)) +
                                   sisStatus.useMagic.faithCurve.Evaluate(sisStatus.faith);
            }
            if (sisStatus.useMagic.darkBase >= 1)
            {
                sisStatus.useMagic.darkAtk = sisStatus.useMagic.phyBase + (sisStatus.useMagic.intCurve.Evaluate(sisStatus._int)) +
                                   sisStatus.useMagic.faithCurve.Evaluate(sisStatus.faith);
            }
            if (sisStatus.useMagic.fireBase >= 1)
            {
                sisStatus.useMagic.fireAtk = sisStatus.useMagic.phyBase + (sisStatus.useMagic.intCurve.Evaluate(sisStatus._int)) +
                                   sisStatus.useMagic.faithCurve.Evaluate(sisStatus.faith);
            }
            if (sisStatus.useMagic.thunderBase >= 1)
            {
                sisStatus.useMagic.thunderAtk = sisStatus.useMagic.phyBase + (sisStatus.useMagic.intCurve.Evaluate(sisStatus._int)) +
                                   sisStatus.useMagic.faithCurve.Evaluate(sisStatus.faith);
            }
        }
    }
    public float SetRecoverAmount(SisMagic s)
    {
        if (sisStatus.useMagic.mType == SisMagic.MagicType.Recover)
        {
            sisStatus.useMagic.recoverAmount = sisStatus.useMagic.recoverBase + (sisStatus.useMagic.intCurve.Evaluate(sisStatus._int)) +
                   sisStatus.useMagic.faithCurve.Evaluate(sisStatus.faith);
        }
        return sisStatus.useMagic.recoverAmount;
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
        //float nowPosition;
        //位置は最初に固定する

   /*     for (int i = 0; i >= SManager.instance.targetList.Count; i++)
        {
            if (i == 0)
            {
                SManager.instance.closestEnemy = SManager.instance.targetList[0].transform.position.x;
            }
            else
            {
                if (Mathf.Abs(Sister.transform.position.x - SManager.instance.targetList[i].transform.position.x) < Mathf.Abs(Sister.transform.position.x - SManager.instance.closestEnemy))
                {
                    SManager.instance.closestEnemy = SManager.instance.closestEnemy = SManager.instance.targetList[i].transform.position.x;
                }
            }


        }*/

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
        targetCondition.Remove(enemy.GetComponent<EnemyBase>());
        if (targetList.Count == 0)
        {
            isBattleEnd = true;
        }
    }
    public void TargetAdd(GameObject target)
    {
        if (!SManager.instance.targetList.Contains(target))
        {
            SManager.instance.targetList.Add(target);
            target.GetComponent<EnemyBase>().TargetEffectCon();
        }
    }

}
