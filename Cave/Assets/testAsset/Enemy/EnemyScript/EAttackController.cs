using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EAttackController : MonoBehaviour
{
    [SerializeField] EnemyBase eb;

    // Start is called before the first frame update


    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*       if (collision.tag == GManager.instance.guardTag)
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
               }*/
        if (GManager.instance.isGuard)
        {
            //Vector3 relativePoint = transform.InverseTransformPoint(co.point);
            bool success;

            if (GManager.instance.Player.transform.localScale.x > 0)
            {
                success = GManager.instance.Player.transform.position.x <= transform.position.x ? true : false;
            }
            else
            {
                success = GManager.instance.Player.transform.position.x > transform.position.x ? true : false;
            }

            if (success)
            {
                if (GManager.instance.isParry && !eb.atV.disParry)
                {
                    eb.PlayerParry();
                }
                else
                {
                    eb.PlayerGuard();
                }
return;
            }
            
        }
        if (collision.tag == GManager.instance.playerTag)
        {
            eb.AttackDamage();
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {


        /*       if (collision.tag == GManager.instance.guardTag)
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
              }*/
        if (GManager.instance.isGuard)
        {
            //Vector3 relativePoint = transform.InverseTransformPoint(co.point);
            bool success;

            if (GManager.instance.Player.transform.localScale.x > 0)
            {
                success = GManager.instance.Player.transform.position.x <= transform.position.x ? true : false;
            }
            else
            {
                success = GManager.instance.Player.transform.position.x > transform.position.x ? true : false;
            }

            if (success)
            {
                if (GManager.instance.isParry && !eb.atV.disParry)
                {
                    eb.PlayerParry();
                }
                else
                {
                    eb.PlayerGuard();
                }
                return;
            }
            
        }
        if (collision.tag == GManager.instance.playerTag)
        {
            eb.AttackDamage();
        }


    }
}
