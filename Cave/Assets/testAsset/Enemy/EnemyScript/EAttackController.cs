using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttackController : MonoBehaviour
{
    [SerializeField]EnemyBase eb;

    // Start is called before the first frame update


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == GManager.instance.guardTag)
        {
            if (GManager.instance.isParry && !eb.atV.disParry)
            {
                eb.PlayerParry();
            }
            else
            {
                eb.PlayerGuard();
            }
        }
       else  if(collision.tag == GManager.instance.playerTag)
        {
            eb.AttackDamage();
        }
        

    }
    private void OnTriggerStay2D(Collider2D collision)
    {


        if (collision.tag == GManager.instance.guardTag)
        {
            if (GManager.instance.isParry && eb.atV.disParry)
            {
                eb.PlayerParry();
            }
            else
            {
                eb.PlayerGuard();
            }
        }
        else if (collision.tag == GManager.instance.playerTag)
        {
            eb.AttackDamage();
        }
    }

}
