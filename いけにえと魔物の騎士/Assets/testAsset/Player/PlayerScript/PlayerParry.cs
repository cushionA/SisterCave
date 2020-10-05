using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    //public bool guardHit;//ガードにヒット中にスタミナブレイクしたらってことか
    public GameObject Player;
    Rigidbody2D rb;
    BoxCollider2D parry;
    float parryTime;
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
            parryTime = Time.fixedDeltaTime;
            if (parryTime >= GManager.instance.pStatus.equipShield.parryStartTime)
            {
                parry.enabled = true;
                pm.isStop = true;
                pm.Stop();
            }
            if (parryTime - GManager.instance.pStatus.equipShield.parryStartTime >= GManager.instance.pStatus.equipShield.parringTime)
            {
                pm.isStop = true;
                pm.Stop();
            }
            if (parryTime - (GManager.instance.pStatus.equipShield.parryStartTime + GManager.instance.pStatus.equipShield.parringTime) >= GManager.instance.pStatus.equipShield.stopTime)
            {
                parry.enabled = false;
                pm.isStop = false;
                GManager.instance.isParry = false;
                parryTime = 0.0f;
            }
        }
        else
        {
            parry.enabled = false;
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
