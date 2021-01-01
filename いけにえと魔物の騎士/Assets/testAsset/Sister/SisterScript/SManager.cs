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
    [HideInInspector] public GameObject targetObj;//攻撃対象
    public　List<SisMagic> attackMagi;
    public List<SisMagic> supportMagi;
    public List<SisMagic> recoverMagi;
    [HideInInspector]public List<GameObject> targetList;
    [HideInInspector] public List<GameObject> targetRecord;
    [HideInInspector]public List<EnemyBase> targetCondition;


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
        attackMagi.Clear();
        supportMagi.Clear();
        recoverMagi.Clear();

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

    public void GetEnemyCondition()
    {
        if (targetList == targetRecord)
        {
            return;
        }
        else
        {
            targetRecord = targetList;
            targetCondition.Clear();
            foreach (GameObject e in targetRecord)
            {
                targetCondition.Add(e.GetComponent<EnemyBase>());
            }
        }
    }
}
