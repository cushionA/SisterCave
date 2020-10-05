using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGuard : MonoBehaviour
{
    //public bool guardHit;//ガードにヒット中にスタミナブレイクしたらってことか
    public GameObject Player;
    Rigidbody2D rb;
    BoxCollider2D guard;

    private void Start()
    {
        rb = Player.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (GManager.instance.isGuard)
        {
            guard.enabled = true;
        }
        else
        {
            guard.enabled = false;
        }
    }

    // Start is called before the first frame update
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GManager.instance.guardHit = true;
         //collision.gameObject.layer = 25;
        rb.AddForce(new Vector2(-Player.transform.localScale.x * 5, 0));//ノックバック
        //GManager.instance.guardHit = false;
    }
    /* private void OnCollisionStay2D(Collision2D collision)
     {
         //GManager.instance.guardHit = true;
         //  collision.gameObject.layer = 25;
         rb.AddForce(new Vector2(-Player.transform.localScale.x * 5, 0));
         //GManager.instance.guardHit = false;
     }*/
    private void OnCollisionExit2D(Collision2D collision)
    {
        GManager.instance.guardHit = false;

    }
}
