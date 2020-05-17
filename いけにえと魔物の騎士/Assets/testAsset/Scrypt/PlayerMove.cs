using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

public class PlayerMove : MonoBehaviour { 

    //インスペクタで設定
public float speed;
public float dashSpeed;
    public float squatSpeed;
    public float fallMSpeed;
    public float gravity;
public float jumpSpeed;
public float jumpRes;
    public float avoidRes;
    public float avoidWait;
public AnimationCurve dashCurve;
    public AnimationCurve rushCurve;
    public AnimationCurve jumpCurve;
    public float avoidSpeed;
public GameObject Player;
public GameObject attack;
MoveObject moveObj;
[HideInInspector] public bool isContinue = false;
[HideInInspector] public bool isRMove;
[HideInInspector] public bool isLMove;
[HideInInspector] public bool isDown;
[HideInInspector] public bool isRight;
[HideInInspector] public bool isAvoid;
[HideInInspector] public bool isDash;
[HideInInspector] public bool isSquat;
[HideInInspector] public bool isUp;
    public PlayMakerFSM fsm;


    //変数を宣言

    Animator anim;
Rigidbody2D rb;
string groundTag = "Ground";
bool isGround = false;
bool isGroundEnter, isGroundStay, isGroundExit;
string moveFloorTag = "MoveFloor";
bool isJump = false;
bool isAttack = false;
float dashTime, jumpTime,rushTime;
float beforeAttack;
float stepOnHeight;
float judgePos;
CapsuleCollider2D capcol;
string enemyTag = "Enemy";
float avoidJudge;
    float horizontalkey;
    float verticalkey;
    float xSpeed;
    float ySpeed;
    float avoidTime;
    bool AKey;

    private void Awake()
    {
        Application.targetFrameRate = 30;
    }
    void Start()
{
    anim = GetComponent<Animator>();
    rb = GetComponent<Rigidbody2D>();

}

    // Update is called once per frame

    public void Update()
    {
        
        horizontalkey = Input.GetAxisRaw("Horizontal");
        verticalkey = Input.GetAxisRaw("Vertical");
        if (Input.GetButton("Avoid"))
        {
            avoidTime = 0.0f;
            avoidJudge += Time.deltaTime;
            isDash = true;

        }
        if (Input.GetButtonUp("Avoid"))
        {
            if (avoidJudge > 0.0f && avoidJudge < 1.5f)
            {
                isAvoid = true;
                isDash = false;
                avoidJudge = 0.0f;

            }
          else if(avoidJudge >= 1.5)
                {
                avoidJudge = 0.0f;
                isDash = false;
            }

        }

    }

    void FixedUpdate()
    {
      
        if (isGroundEnter || isGroundStay)
        {
            isGround = true;

        }
        else if (isGroundExit)
        {
            isGround = false;
        }
        isGroundEnter = false;
        isGroundStay = false;
        isGroundExit = false;


        if (!isDown && !isAvoid)
        {
            Debug.Log("Getback");
            if (isGround && !isJump)
            {
                if (horizontalkey > 0)
                {
                    anim.SetBool("run", true);
                    transform.localScale = new Vector3(1, 1, 1);
                    Debug.Log("右入力");
                    isRight = true;
                    if (isDash)
                    {
                        xSpeed = dashSpeed;
                        rushTime += Time.fixedDeltaTime;

                    }
                    else if (isSquat)
                    {
                        xSpeed = squatSpeed;
             
                    }
                    else
                    {
                        xSpeed = speed;
                        dashTime += Time.fixedDeltaTime;
                    }
                }
                else if (horizontalkey < 0)
                {
                    anim.SetBool("run", true);
                    transform.localScale = new Vector3(-1, 1, 1);

                    isRight = false;

                    if (isDash)
                    {
                        xSpeed = -dashSpeed;
                        rushTime += Time.fixedDeltaTime;

                    }
                    else if (isSquat)
                    {
                        xSpeed = -squatSpeed;
                     
                    }
                    else
                    {
                        xSpeed = -speed;
                        dashTime += Time.fixedDeltaTime;
                    }
                }

                else
                {

                    xSpeed = 0.0f;
                    dashTime = 0.0f;
                    rushTime = 0.0f;
                }



                if (verticalkey > 0)
                {
                    isJump = true;
                    Debug.Log("↑");

                }
                else if (verticalkey < 0)
                {
                    isSquat = true;
                    SetLayer(9);
                }
                else
                {

                    isSquat = false;
                    SetLayer(0);
                }


                if (isDash)
                {
                    xSpeed *= rushCurve.Evaluate(rushTime);
                }
                else if (!isSquat)
                {
                    xSpeed *= dashCurve.Evaluate(dashTime);
                }
            }



            else if (isJump)

            {
                jumpTime += Time.fixedDeltaTime;
                SetLayer(0);
                if (jumpTime <= jumpRes)
                {
                    ySpeed = jumpSpeed;
                    ySpeed *= jumpCurve.Evaluate(jumpTime);
                   

                }
                else
                {
                    isJump = false;
                }

            }
            else if (!isGround)
            {
                jumpTime = 0.0f;
                ySpeed = -gravity;
 
            }
            //移動速度を設定
            Vector2 addVelocity = Vector2.zero;
            if (moveObj != null)
            {
                addVelocity = moveObj.GetVelocity();
            }

            rb.velocity = new Vector2(xSpeed, ySpeed) + addVelocity;

        }


        if (isGround && isAvoid)
        {
            avoidTime += Time.fixedDeltaTime;

            if (isRight)
            {
                //プレイヤーが右側、かつクールタイム消化。
               if(avoidTime < avoidRes)
                {
                    SetLayer(10);
                    rb.velocity = new Vector2(avoidSpeed, 0);

                }
                else
                {
            
                    rb.velocity = new Vector2(0, 0);

                    SetLayer(0);

                    Invoke("AChange", 0.1f);
                }


            }

            else if (!isRight)
            { //プレイヤーが左側、かつクールタイム消化。
                if(avoidTime < avoidRes)
                {
                    SetLayer(10);
                    //突進の制限時間三秒である以内は方向転換を禁じて進み続ける。
                    rb.velocity = new Vector2(-avoidSpeed, 0);
 
                }
                else
                {
             
                    rb.velocity = new Vector2(0, 0);

                    SetLayer(0);

                    Invoke("AChange", 0.1f);
                }

            }





        }


            //アタックを入れるとこ


        }
    
   void AChange (){
        Debug.Log("avoid");
        isAvoid = false;

    }



/// <summary>
/// ダウンアニメーションが終わっているかどうか
/// </summary>
/// <returns>終了しているかどうか</returns> 
public bool IsDownAnimEnd()
{
    if (isDown && anim != null)
    {
        AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
        if (currentState.IsName("SisterDamage"))
        {
            if (currentState.normalizedTime >= 1)
            {
                return true;
            }
        }
    }
    return false;
}

/// <summary>
/// コンティニューする
/// </summary>
public void ContinuePlayer()
{
    isDown = false;
    anim.Play("SisterStand");
    isJump = false;
    isContinue = true;


}

private void OnTriggerEnter2D(Collider2D collision)
{
    
        if (collision.tag == groundTag)
    {
        isGroundEnter = true;
         
        }


    //動く床
    if (collision.tag == moveFloorTag)
    {
            isGroundEnter = true;
            moveObj = collision.gameObject.GetComponent<MoveObject>();

    }
}

private void OnTriggerStay2D(Collider2D collision)
{
    if (collision.tag == groundTag || collision.tag == moveFloorTag)
    {
        isGroundStay = true;

        }

}
private void OnTriggerExit2D(Collider2D collision)
{
    if (collision.tag == groundTag || collision.tag == moveFloorTag)
    {
        isGroundExit = true;
    }
    if (collision.tag == moveFloorTag)
    {
        //動く床から離れた
        moveObj = null;
    }
}

public void SetLayer(int layerNumber)
{

        Player.layer = layerNumber;

}

}






