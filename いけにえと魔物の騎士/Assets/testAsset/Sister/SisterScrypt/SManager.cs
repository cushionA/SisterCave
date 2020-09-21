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
        if (sisStatus.useMagic.phyBase >= 1)
        {
            sisStatus.useMagic.phyAtk = sisStatus.useMagic.phyBase + (sisStatus.useMagic.powerCurve.Evaluate(sisStatus.power)) +
                               sisStatus.useMagic.skillCurve.Evaluate(sisStatus.skill);
        }
        if (sisStatus.useMagic.holyBase >= 1)
        {
            sisStatus.useMagic.holyAtk = sisStatus.useMagic.holyBase + (sisStatus.useMagic.powerCurve.Evaluate(sisStatus.power)) +
                               sisStatus.useMagic.intCurve.Evaluate(sisStatus._int);
        }
        if (sisStatus.useMagic.darkBase >= 1)
        {
            sisStatus.useMagic.darkAtk = sisStatus.useMagic.darkBase + (sisStatus.useMagic.intCurve.Evaluate(sisStatus._int)) +
                               sisStatus.useMagic.skillCurve.Evaluate(sisStatus.skill);
        }
        if (sisStatus.useMagic.fireBase >= 1)
        {
            sisStatus.useMagic.fireAtk = sisStatus.useMagic.fireBase + sisStatus.useMagic.intCurve.Evaluate(sisStatus._int);
        }
        if (sisStatus.useMagic.thunderBase >= 1)
        {
            sisStatus.useMagic.thunderAtk = sisStatus.useMagic.thunderBase + sisStatus.useMagic.intCurve.Evaluate(sisStatus._int);

        }
    }
}
