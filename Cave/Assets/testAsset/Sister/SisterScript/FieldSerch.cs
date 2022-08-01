using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SensorToolkit;


/// <summary>
/// 環境物を確認
/// </summary>
public class FieldSerch : MonoBehaviour
{



  //  string dangerTag = "Danger";
    [SerializeField] SisterBrain sister;
   // public float SerchRadius;
  //  [SerializeField]
  //  private LayerMask layerMask;
     RangeSensor2D se;
    [SerializeField] float pulseWait = 3;
    float pulseTime;
    bool isSerch;

    // private int layerMask = 1 << 11;//1 << 8 | 1 << 10 | 1<< 11 | 1 << 16 | 1 <<　9;

    void Start()
    {
        sister = GetComponentInParent<SisterBrain>();
        se = GetComponent<RangeSensor2D>();
    }

    private void FixedUpdate()
    {
        pulseTime += Time.fixedDeltaTime;
        if (pulseTime >= pulseWait)
        {
            //Debug.Log("機能してますよー");
            se.Pulse();
            isSerch = true;
            pulseTime = 0;
        }

        if (isSerch)
        {
            DetectObject();
        }

    }
    public void DetectObject()
    {
        if (se.GetDetectedByTag(SManager.instance.enemyTag).Count >= 1)
        {

          //  Debug.Log($"配信{se.GetDetectedByTag(SManager.instance.enemyTag)[0].name}");
            sister.nowState = SisterBrain.SisterState.戦い;//この辺はまた後で設定できるようにしよう
            SManager.instance.playObject = null;
            sister.isPlay = false;
            SManager.instance.TargetAdd(se.GetNearestToPointByTag(SManager.instance.Sister.transform.position, "Enemy"));
         //   SManager.instance.targetCondition.Add(SManager.instance.targetList[0].GetComponent<EnemyBase>());
            //最初は近いやつ入れよ
            SManager.instance.GetClosestEnemyX();
            sister.Serch3.SetActive(true);
            sister.Serch.SetActive(false);
            sister.Serch2.SetActive(false);
            sister.reJudgeTime = 150;
            sister.reJudgePositionTime = SManager.instance.sisStatus.escapeTime;
           // SManager.instance.isTChange = true;
            SManager.instance.playObject = null;
        }
        else if (se.GetDetectedByTag(SManager.instance.dangerTag).Count >= 1)
        {
            sister.nowState = SisterBrain.SisterState.警戒;
            // sister.Serch.SetActive(false);
            // sister.Serch2.SetActive(false);
            sister.stateNumber = 3;
            sister.beforeNumber = 0;
            sister.reJudgeTime = 0;
            sister.changeable = true;
            SManager.instance.playObject = null;
            sister.isPlay = false;
        }
        else if (se.GetDetectedByTag(SManager.instance.reactionTag).Count >= 1)
        {
            SManager.instance.playObject = se.GetNearestToPoint(SManager.instance.Sister.transform.position);
            sister.isPlay = true;
            sister.playPosition = SManager.instance.playObject.transform.position.x;
            sister.playDirection = SManager.instance.playObject.transform.localScale.x;
        }
    }


}

