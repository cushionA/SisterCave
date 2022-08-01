using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    public GameObject player;

    bool isGimmickOn;
    bool isPush;
    GimmickAct ga;
    PlayerMove pm;
    bool isFirst;
    Transform ladderTransform;

    [SerializeField]CircleCollider2D upLimit;
    [SerializeField]CapsuleCollider2D DownLimit;
    [SerializeField] BoxCollider2D trigger;
    [SerializeField]float moveSpeed = 6;
    [SerializeField] float ClimbDirection;
    [SerializeField]Vector3 downDistance;
    [SerializeField]Vector3 upDistance;
    [SerializeField] Vector3 upSlideDistance;
    [SerializeField] float space;
    [SerializeField] float delayRes;

    bool isLow;
    bool isLimit;
    float VerticalKey;
    Vector3 pos;
    Rigidbody2D rb;
    string playerTag = "Player";
    bool isGJudge;
    float delayTime;

    private void Start()
    {
        ladderTransform = transform;
        ga = this.gameObject.GetComponent<GimmickAct>();
        pm = player.GetComponent<PlayerMove>();
        rb = player.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //ボタンが押されているかどうかの確認

        if (GManager.instance.InputR.GetButtonDown(MainUICon.instance.rewiredAction5))
        {

            isPush = true;
        }
        else
        {
            isPush = false;

        }
        VerticalKey = GManager.instance.InputR.GetAxisRaw(MainUICon.instance.rewiredAction1);


    }

    private void FixedUpdate()
    {
        pos = new Vector3(ladderTransform.position.x - (space * ClimbDirection), player.transform.position.y, 0);

        if (ga.isGimmick && isPush && !isGimmickOn)
        {
            GManager.instance.isLadder = true;
            isGJudge = GrounndJudge();
            pm.AllStop();
            pm.enabled = false;
            player.transform.localScale = new Vector3(ClimbDirection, 1, 1);

            //ギミック利用可能でボタンが押されていて、ギミック利用中でなければ以下の処理。ギミック利用開始
            isGimmickOn = true;
            isPush = false;
            //ボタンを押されてないことにすることで下で誤反応を起こすのを防いだ

            trigger.enabled = false;
            player.transform.position = pos;
            //Xの座標だけはしごに合わせる

            Judge();
        }

        else if (isGimmickOn && isPush)
        {
            Release();
            pm.enabled = true;
        }

        if (isGimmickOn)
        {
            if(VerticalKey > 0)
            {
                isGJudge = false;
                rb.velocity = new Vector2(0, moveSpeed);
            }
            else if (VerticalKey < 0)
            {
                rb.velocity = new Vector2(0, -moveSpeed);
            }
            else
            {
                rb.velocity = Vector2.zero;
            }

            Judge();

        }
        if (isLimit)
        {
            ////Debug.log("侵入");
            delayTime +=　Time.fixedDeltaTime;
            if(delayTime >= delayRes)
            {
               ////Debug.log("成功");
                pm.enabled = true;
                isLimit = false;
                delayTime = 0.0f;
                //本番ではアニメーションが終わるまでに置き換える？
            }

        }
        

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isGimmickOn) 
        {
            //ギミックを利用可能か判定
            if (collision.tag == playerTag && isLow && !isGJudge)
            {
                Release();
                player.transform.Translate(downDistance);
                pm.enabled = true;
            }
            else if(collision.tag == playerTag && !isLow)
            {
                Release();
                player.transform.Translate(upDistance);
                player.transform.Translate(upSlideDistance);
                isLimit = true;

            }
        
        }

    }



    void Judge()
    {
        if (player.transform.position.y <= ladderTransform.position.y / 2)
        {
            isLow = true;
            DownLimit.enabled = true;
            upLimit.enabled = false;
        }
        else
        {
            isLow = false;
            DownLimit.enabled = false;
            upLimit.enabled = true;
        }

    }

    void Release()
    {
        rb.velocity = Vector2.zero;
        //ギミック利用中にボタンが押されたら以下の処理。ギミック利用停止
        isGimmickOn = false;
        isPush = false;
        trigger.enabled = true;
        upLimit.enabled = false;
        DownLimit.enabled = false;
    }

    bool GrounndJudge()
    {
      return  pm.isGround;

    }

}
