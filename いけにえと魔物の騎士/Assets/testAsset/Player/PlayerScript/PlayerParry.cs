using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    //public bool guardHit;//ガードにヒット中にスタミナブレイクしたらってことか
    public GameObject Player;
    Rigidbody2D rb;
    BoxCollider2D parry;
    float defenceTime;
    PlayerMove pm;

    private void Start()
    {
        rb = Player.GetComponent<Rigidbody2D>();
        pm = GetComponentInParent<PlayerMove>();
    }

    private void FixedUpdate()
    {
        if (GManager.instance.isParry)
        {
            defenceTime = Time.fixedDeltaTime;
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {
                if (defenceTime >= GManager.instance.pStatus.equipShield.parryStart)
                {
                    parry.enabled = true;
                    pm.isStop = true;
                    pm.Stop();
                }
                else if (defenceTime - GManager.instance.pStatus.equipShield.parryStart <= GManager.instance.pStatus.equipShield.parryTime)
                {
                    pm.isStop = true;
                    pm.Stop();
                }
                else if (defenceTime - GManager.instance.pStatus.equipShield.parryStart > GManager.instance.pStatus.equipShield.parryTime)
                {
                    parry.enabled = false;
                    pm.isStop = false;
                    GManager.instance.isParry = false;
                    defenceTime = 0.0f;
                }
            }
            else
            {
                if (defenceTime >= GManager.instance.pStatus.equipWeapon.parryStart)
                {
                    parry.enabled = true;
                    pm.isStop = true;
                    pm.Stop();
                }
               else if (defenceTime - GManager.instance.pStatus.equipWeapon.parryStart <= GManager.instance.pStatus.equipWeapon.parryTime)
                {
                    pm.isStop = true;
                    pm.Stop();
                }
                else if (defenceTime - GManager.instance.pStatus.equipWeapon.parryStart > GManager.instance.pStatus.equipWeapon.parryTime)
                {
                    parry.enabled = false;
                    pm.isStop = false;
                    GManager.instance.isParry = false;
                    defenceTime = 0.0f;
                }
            }
        }
        else
        {
            if (parry.enabled == true)
            {
                parry.enabled = false;
            }
        }
    }

    // Start is called before the first frame update
 /*   private void OnCollisionEnter2D(Collision2D collision)
    {
        GManager.instance.guardHit = true;
        //collision.gameObject.layer = 25;
        rb.AddForce(new Vector2(-Player.transform.localScale.x * 5, 0));//ノックバック
        //GManager.instance.guardHit = false;
    }
     private void OnCollisionStay2D(Collision2D collision)
     {
         //GManager.instance.guardHit = true;
         //  collision.gameObject.layer = 25;
         rb.AddForce(new Vector2(-Player.transform.localScale.x * 5, 0));
         //GManager.instance.guardHit = false;
     }
    private void OnCollisionExit2D(Collision2D collision)
    {
        GManager.instance.guardHit = false;

    }
   */
}
