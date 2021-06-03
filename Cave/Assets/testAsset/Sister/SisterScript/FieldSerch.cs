﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SensorToolkit;


/// <summary>
/// 環境物を確認
/// </summary>
public class FieldSerch : MonoBehaviour
{


    string reactionTag = "Dialog";
    string dangerTag = "Danger";
    [SerializeField] SisterBrain sister;
   // public float SerchRadius;
  //  [SerializeField]
  //  private LayerMask layerMask;
     RangeSensor2D se;

    // private int layerMask = 1 << 11;//1 << 8 | 1 << 10 | 1<< 11 | 1 << 16 | 1 <<　9;

    void Start()
    {
        sister = GetComponentInParent<SisterBrain>();
        se = GetComponent<RangeSensor2D>();
    }


    public void DetectObject()
    {

        if (se.GetDetectedByTag("Enemy") != null)
        {

            sister.nowState = SisterBrain.SisterState.戦い;//この辺はまた後で設定できるようにしよう
            SManager.instance.playObject = null;
            sister.isPlay = false;
            SManager.instance.targetList.Add(se.GetNearestToPointByTag(SManager.instance.Sister.transform.position, "Enemy"));
            //最初は近いやつ入れよ
            sister.Serch3.SetActive(true);
            sister.Serch.SetActive(false);
            sister.Serch2.SetActive(false);
            SManager.instance.isTChange = true;
            SManager.instance.playObject = null;
        }
        else if (se.GetDetectedByTag("Danger") != null)
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
        else
        {
            SManager.instance.playObject = se.GetNearestToPoint(SManager.instance.Sister.transform.position);
            sister.isPlay = true;
            sister.playPosition = SManager.instance.playObject.transform.position.x;
            sister.playDirection = SManager.instance.playObject.transform.localScale.x;
        }
    }


 /*   private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == reactionTag && sister.nowState == SisterBrain.SisterState.のんびり)
        {
            if (CheckFoundObject(collision.gameObject))
            {
//環境物の種類にもよる？？？
                    //会話イベント起きるやつと起きないやつで分ける
                    //sister.isPosition = false;
                    sister.isPlay = true;
                    sister.playPosition = collision.gameObject.transform.position.x;
                    sister.playDirection = collision.gameObject.transform.localScale.x;
                
            }
        }
        if (collision.tag == dangerTag)
        {//危険物を発見した時

            if (CheckFoundObject(collision.gameObject))
            {
                sister.isPosition = true;

            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == reactionTag && sister.nowState == SisterBrain.SisterState.のんびり)
        {
            if (CheckFoundObject(collision.gameObject))
            {

                   // sister.isPosition = false;
                    sister.isPlay = true;
                    sister.playPosition = collision.gameObject.transform.position.x;
                    sister.playDirection = collision.gameObject.transform.localScale.x;
                
            }
        }
        if (collision.tag == dangerTag)
        {
            if (CheckFoundObject(collision.gameObject))
            {
                sister.isPosition = true;

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
    }
 */

}
