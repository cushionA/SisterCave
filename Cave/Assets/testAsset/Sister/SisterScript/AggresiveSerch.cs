﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SensorToolkit;

public class AggresiveSerch : MonoBehaviour
{

    [SerializeField] SisterBrain sister;
    [SerializeField] float pulseWait;
    float pulseTime;

    //  public float SerchRadius;
  //  [SerializeField]
   // private LayerMask layerMask;

    RangeSensor2D se;

    // private int layerMask = 1 << 11;//1 << 8 | 1 << 10 | 1<< 11 | 1 << 16 | 1 <<　9;

    void Start()
    {
     //   sister = GetComponentInParent<SisterBrain>();
        se = GetComponent<RangeSensor2D>();
    }

    public void SerchEnemy()
    {
        
      //  Debug.Log("調査");
        SManager.instance.targetList.Clear();
        for (int i = 0; i < SManager.instance.targetList.Count; i++)
        {
            SManager.instance.targetList[i].GetComponent<EnemyBase>().TargetEffectCon(1);
        }
        SManager.instance.targetCondition.Clear();
        for (int i = 0; i < se.DetectedObjectsOrderedByDistance.Count; i++)
        {
            SManager.instance.TargetAdd(se.DetectedObjectsOrderedByDistance[i]);
          //  SManager.instance.targetCondition.Add(SManager.instance.targetList[i].GetComponent<EnemyBase>());
        }
      //  SManager.instance.isTChange = true;
    }

    private void FixedUpdate()
    {
        pulseTime += Time.fixedDeltaTime;
        if (pulseTime >= pulseWait)
        {
            //Debug.Log("機能してますよー");
            se.Pulse();
            SManager.instance.isSerch = true;
            pulseTime = 0;
        }

    }

    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == enemyTag)
        {
            if (CheckFoundObject(collision.gameObject))
            {
                if (!SManager.instance.targetList.Contains(collision.gameObject))
                  {
                    SManager.instance.targetList.Add(collision.gameObject);
                  }

            }
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == enemyTag)
        {
            if (CheckFoundObject(collision.gameObject))
            {
                if (!SManager.instance.targetList.Contains(collision.gameObject))
                {
                    SManager.instance.targetList.Add(collision.gameObject);
                }
            }
        }
    }




    private bool CheckFoundObject(GameObject i_target)
    {
        Vector2 targetPosition = i_target.transform.position;//targetの位置を取得
        Vector2 myPosition = transform.position;//自分の位置




        Vector2 toTargetDir = (targetPosition - myPosition).normalized;

        if (!IsHitRay(myPosition, toTargetDir, i_target))
        {//IsHitsRayが真なら真を返す
            return false;
        }

        return true;


    }

    private bool IsHitRay(Vector2 i_fromPosition, Vector2 i_toTargetDir, GameObject i_target)
    {
        // 方向ベクトルが無い場合は、同位置にあるものだと判断する。
        if (i_toTargetDir.sqrMagnitude <= Mathf.Epsilon)
        {//sqrはベクトルの長さを返す
            return true;

        }

        RaycastHit2D onHitRay = Physics2D.Raycast(i_fromPosition, i_toTargetDir, layerMask.value);
        if (!onHitRay.collider)
        {
            return false;
        }
        //  ////Debug.log($"{onHitRay.transform.gameObject}");
        ////Debug.DrawRay(i_fromPosition,i_toTargetDir * SerchRadius);
        if (onHitRay.transform.gameObject != i_target)
        {//onHitRayは当たった場所
         //当たった場所がPlayerの位置でなければ
            return false;
        }

        return true;
    }*/
}
