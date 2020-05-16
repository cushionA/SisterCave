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
public AnimationCurve dashCurve;
    public AnimationCurve rushCurve;
    public AnimationCurve jumpCurve;
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
    float avoidKey;
    float verticalkey;
    float xSpeed;
    float ySpeed;

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
        avoidKey = Input.GetAxisRaw("Avoid");
        verticalkey = Input.GetAxisRaw("Vertical");

    }

    void FixedUpdate()
    {

        if (isGroundEnter || isGroundStay)
        {
            isGround = true;
            Debug.Log("接地");
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
            if (isGround && !isJump)
            {

                Debug.Log("ddd");
                if (avoidKey > 0)
                {
                    avoidJudge += Time.fixedDeltaTime;
                    isDash = true;

                }

                if (avoidJudge > 0.0f && avoidJudge < 0.5f)
                {
                    isAvoid = true;
                    isDash = false;
                    avoidJudge = 0.0f;

                }
                else if(avoidJudge >= 0.5f){
                    isDash = false;
                    avoidJudge = 0.0f;
                }

                if (horizontalkey > 0)
                {
                    anim.SetBool("run", true);
                    //transform.localScale = new Vector3(1, 1, 1);
                    Debug.Log("右入力");
                    if (isDash)
                    {
                        xSpeed = dashSpeed;
                        rushTime += Time.deltaTime;

                    }
                    else if (isSquat)
                    {
                        xSpeed = squatSpeed;
                        return;
                    }
                    else
                    {
                        xSpeed = speed;
                        dashTime += Time.deltaTime;
                    }
                }
                else if (horizontalkey < 0)
                {
                    anim.SetBool("run", true);
                    //transform.localScale = new Vector3(-1, 1, 1);

                    isRight = false;

                    if (isDash)
                    {
                        xSpeed = -dashSpeed;
                        rushTime += Time.deltaTime;

                    }
                    else if (isSquat)
                    {
                        xSpeed = -squatSpeed;
                        return;
                    }
                    else
                    {
                        xSpeed = -speed;
                        dashTime += Time.deltaTime;
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
                    SetLayer(10);
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


        }
            else if (isJump)

            {
                SetLayer(0);
                if (jumpTime <= jumpRes)
                {
                    ySpeed = jumpSpeed;
                    ySpeed *= jumpCurve.Evaluate(jumpTime);
                    jumpTime += Time.fixedDeltaTime;
                    Debug.Log("跳躍");
                }
                else
                {
                    isJump = false;
                }

                jumpTime = 0.0f;

            }
            else
            {
                ySpeed = -gravity;
                jumpTime = 0.0f;
            }

         
            //移動速度を設定
            Vector2 addVelocity = Vector2.zero;
            if (moveObj != null)
            {
                addVelocity = moveObj.GetVelocity();
            }
            rb.velocity = new Vector2(xSpeed, ySpeed) + addVelocity;







            //アタックを入れるとこ

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






