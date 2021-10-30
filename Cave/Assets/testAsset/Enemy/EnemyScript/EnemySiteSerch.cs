using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySiteSerch : MonoBehaviour
{
    //public
        //GameObject player;

    // string GManager.instance.playerTag = "Player";
    [SerializeField] EnemyBase enemy;
    public float SerchRadius;
    [SerializeField]
    private LayerMask layerMask;

    // private int layerMask = 1 << 11;//1 << 8 | 1 << 10 | 1<< 11 | 1 << 16 | 1 <<�@9;

    void Start()
    {
        //     enemy = GetComponentInParent<EnemyBase>();
       
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == GManager.instance.playerTag)
        {

            if (CheckFoundObject(GManager.instance.Player))
            {
                enemy.isAggressive = true;
                enemy.Serch.SetActive(false);
                enemy.Serch2.SetActive(false);
                
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == GManager.instance.playerTag)
        {

            if (CheckFoundObject(GManager.instance.Player))
            {
                enemy.isAggressive = true;
                enemy.Serch.SetActive(false);
                enemy.Serch2.SetActive(false);
            }
        }
    }




    private bool CheckFoundObject(GameObject i_target)
    {
        Vector2 targetPosition = i_target.transform.position;//target�̈ʒu���擾
        Vector2 myPosition = transform.position;//�����̈ʒu




        Vector2 toTargetDir = (targetPosition - myPosition).normalized;

        if (!IsHitRay(myPosition, toTargetDir, i_target))
        {//IsHitsRay���^�Ȃ�^��Ԃ�
            return false;
        }

        return true;


    }

    private bool IsHitRay(Vector2 i_fromPosition, Vector2 i_toTargetDir, GameObject i_target)
    {
        // �����x�N�g���������ꍇ�́A���ʒu�ɂ�����̂��Ɣ��f����B
        if (i_toTargetDir.sqrMagnitude <= Mathf.Epsilon)
        {//sqr�̓x�N�g���̒�����Ԃ�
            return true;

        }

        RaycastHit2D onHitRay = Physics2D.Raycast(i_fromPosition, i_toTargetDir, SerchRadius, layerMask.value);
        if (!onHitRay.collider)
        {
            return false;
        }
        //  //Debug.log($"{onHitRay.transform.gameObject}");
        //Debug.DrawRay(i_fromPosition,i_toTargetDir * SerchRadius);
        if (onHitRay.transform.gameObject != i_target)
        {//onHitRay�͓��������ꏊ
         //���������ꏊ��Player�̈ʒu�łȂ����
         ////Debug.log("������");
            return false;
        }

        return true;
    }
}