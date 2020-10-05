using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuard : MonoBehaviour
{
    EnemyBase eb;

    void Start()
    {
        eb = GetComponentInParent<EnemyBase>();
    }

    // Update is called once per frame


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == eb.status.AttackTag && (eb.isHitable || eb.lastHit != collision.gameObject))
        {
            eb.guardHit = true;
            eb.WeponGuard();
          //  collision.gameObject.layer = 25;
            eb.guardHit = false;
            eb.lastHit = collision.gameObject;
        }
        else if (collision.gameObject.tag == eb.status.PMagicTag && (eb.isHitable || eb.lastHit != collision.gameObject))
        {
            eb.guardHit = true;
            eb.PlayerMagicGuard();
         //   collision.gameObject.layer = 25;
            eb.guardHit = false;
            eb.lastHit = collision.gameObject;
        }
        else if (collision.gameObject.tag == eb.status.SMagicTag && (eb.isHitable || eb.lastHit != collision.gameObject))
        {
            eb.guardHit = true;
            eb.SisterMagicGuard();
       //     collision.gameObject.layer = 25;
            eb.guardHit = false;
            eb.lastHit = collision.gameObject;
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {


        if (collision.tag == eb.status.AttackTag && (eb.isHitable || eb.lastHit != collision.gameObject))
        {

            Debug.Log("継続");
            eb.WeponGuard();
            eb.lastHit = collision.gameObject;
        }
        else if (collision.tag == eb.status.PMagicTag && (eb.isHitable || eb.lastHit != collision.gameObject))
        {

            eb.PlayerMagicGuard();
            eb.isHit = true;
            eb.isHitable = false;
            eb.lastHit = collision.gameObject;
        }
        else if (collision.tag == eb.status.SMagicTag && (eb.isHitable || eb.lastHit != collision.gameObject))
        {

            eb.SisterMagicGuard();
            eb.isHit = true;
            eb.isHitable = false;
            eb.lastHit = collision.gameObject;
        }


    }

    protected void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.tag == eb.status.PMagicTag)
        {
            //ここで最終弾だけ吹き飛ばすようにする？
            eb.PlayerMagicGuard();
            eb.isHit = false;
        }
        else if (collision.tag == eb.status.SMagicTag)
        {

            eb.SisterMagicGuard();
            eb.isHit = false;
        }

    }

}
