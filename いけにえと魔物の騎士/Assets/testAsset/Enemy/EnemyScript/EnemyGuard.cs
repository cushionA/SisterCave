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
        if(collision.gameObject.tag == eb.status.AttackTag)
        {
            eb.guardHit = true;
            eb.WeponGuard();
            collision.gameObject.layer = 25;
            eb.guardHit = false;
        }
        else if (collision.gameObject.tag == eb.status.PMagicTag)
        {
            eb.guardHit = true;
            eb.PlayerMagicGuard();
            collision.gameObject.layer = 25;
            eb.guardHit = false;
        }
        else if (collision.gameObject.tag == eb.status.SMagicTag)
        {
            eb.guardHit = true;
            eb.SisterMagicGuard();
            collision.gameObject.layer = 25;
            eb.guardHit = false;
        }
    }

}
