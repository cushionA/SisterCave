﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSerch : MonoBehaviour
{
    public GameObject player;

    string playerTag = "Player";
    [SerializeField]EnemyBase enemy;
    public float SerchRadius;
    [SerializeField]
    private LayerMask layerMask;

   // private int layerMask = 1 << 11;//1 << 8 | 1 << 10 | 1<< 11 | 1 << 16 | 1 <<　9;

    void Start()
    {
        enemy = GetComponentInParent<EnemyBase>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == playerTag)
        {

            if (CheckFoundObject(player))
            {
                enemy.isAggressive = true;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == playerTag)
        {

            if (CheckFoundObject(player))
            {
                enemy.isAggressive = true;
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

        RaycastHit2D onHitRay = Physics2D.Raycast(i_fromPosition, i_toTargetDir,SerchRadius,layerMask.value);
        if (!onHitRay.collider)
        {
            return false;
        }
        Debug.Log($"{onHitRay.transform.gameObject}");
        Debug.DrawRay(i_fromPosition,i_toTargetDir * SerchRadius);
        if (onHitRay.transform.gameObject != i_target)
        {//onHitRayは当たった場所
         //当たった場所がPlayerの位置でなければ
            //Debug.Log("あいに");
            return false;
        }
        
        return true;
    }
}
