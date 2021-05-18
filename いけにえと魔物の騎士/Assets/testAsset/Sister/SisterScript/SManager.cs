using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SManager : MonoBehaviour
{
    public static SManager instance = null;

    public GameObject Sister;
    //プレイヤーオブジェクト
    public SisterStatus sisStatus;
    //プレイヤーのステータスを取得
    //[HideInInspector] public GameObject targetObj;
    public　List<SisMagic> attackMagi;
    public List<SisMagic> supportMagi;
    public List<SisMagic> recoverMagi;
    [HideInInspector]public List<GameObject> targetList = new List<GameObject>();
   // [HideInInspector] public List<GameObject> targetRecord;
    [HideInInspector]public List<EnemyStatus> targetCondition;
    [HideInInspector]public float closestEnemy;
    [HideInInspector] public GameObject playObject;
    [HideInInspector] public bool isEscape;//プレイヤー離れたフラグ。
                                           // [HideInInspector] public bool enemyDead;//敵死んだフラグ。これが立つとパルス飛ばして敵検索
    [HideInInspector] public bool isTChange;

    [HideInInspector] public GameObject target;//攻撃対象

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
        attackMagi = null;
        supportMagi = null;
        recoverMagi = null;

        foreach (SisMagic m in sisStatus.equipMagic)
        {
            if (m.mType == SisMagic.MagicType.Attack)
            {
                attackMagi.Add(m);
            }
            else if (m.mType == SisMagic.MagicType.Support)
            {
                supportMagi.Add(m);
            }
            else if (m.mType == SisMagic.MagicType.Recover)
            {
                recoverMagi.Add(m);
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

    public void GetEnemyCondition()
    {
        if (isTChange)
        {

            //最初は絶対こっち
            //ターゲットの状態を取得
            //        targetRecord = targetList;
            
            for (int i = 0; i < targetList.Count; i++)
            {
                targetCondition.Add(targetList[i].GetComponent<EnemyBase>().status);
                if(i == targetList.Count - 1)
                {
                    isTChange = false;
                }
            }
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

}
