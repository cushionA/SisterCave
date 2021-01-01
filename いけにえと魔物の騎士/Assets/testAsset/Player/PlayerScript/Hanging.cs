using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hanging : MonoBehaviour
{
    public GameObject Player;
    public CircleCollider2D col;
    PlayerMove pm;
    Animator an;
    Rigidbody2D rb;
    bool isHang;
    [Header("Xで崖から離れる距離")]
    public float distaX;
    [Header("Yで崖から離れる距離")]
    public float distaY;
    bool climb;
    SimpleAnimation sAni;
    bool fall;
    bool isFirst;
    bool isSecond;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        pm = GetComponentInParent<PlayerMove>();
        an = GetComponentInParent<Animator>();
        sAni = GetComponentInParent<SimpleAnimation>();
    }

    private void Update()
    {
        if (isHang && Input.GetAxisRaw("Vertical") > 0)
        {
            climb = true;
            isHang = false;
        }
        else if(isHang && Input.GetAxisRaw("Vertical") < 0)
        {
            fall = true;
            isHang = false;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isHang)
        {
            
            sAni.Play("Hang");
            rb.velocity = Vector2.zero;
        }
        else if(climb)
        {
            if (!isFirst)
            {
                sAni.Play("Climb");
                isFirst = true;
            }
            if (CheckEnd("Climb"))
            {
                rb.velocity = new Vector2(0, 3);
            }
            else
            {
                if (!isSecond)
                {
                    sAni.Play("Climb2");
                    isSecond = true;
                }
            }
            if (!CheckEnd("Climb") && CheckEnd("Climb2"))
                {
                    rb.velocity = new Vector2(3,3);
                }
            else if(!CheckEnd("Climb") && !CheckEnd("Climb2"))
            {
                isSecond = false;
                isFirst = false;
                climb = false;
                pm.enabled = true;
                Invoke("CollRecover", 0.5f);
            }

        }   //2.39//4.75

        if (fall)
        {
            fall = false;
            pm.enabled = true;
            Invoke("CollRecover", 0.5f);
        }

    }
    private void OnTriggerEnter2D(Collider2D coll)
    {
        //Debug.log("愛してる");
        if (!pm.isGround && coll.gameObject.tag == "Hanging")
        {
            col.enabled = false;
            pm.Stop();
            pm.enabled = false;   //プレーヤーコントローラーを停止
            Player.transform.position = new Vector3(coll.transform.position.x + (distaX * -Player.transform.localScale.x),coll.transform.position.y - distaY,0);
            isHang = true;
        }

    }
    bool CheckEnd(string _currentStateName)
    {
        return sAni.IsPlaying(_currentStateName);

    }
    void CollRecover()
    {
        col.enabled = true;
    }
}
