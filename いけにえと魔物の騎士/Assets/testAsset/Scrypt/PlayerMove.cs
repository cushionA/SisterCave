using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

public class PlayerMove : MonoBehaviour
{

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
    public AnimationCurve jumpMCurve;
    public AnimationCurve fallCurve;
    public AnimationCurve fallMCurve;
    public float avoidSpeed;
    public GameObject Player;
    public GameObject attack;

    [HideInInspector] public bool isGround;
    [HideInInspector] public bool isContinue = false;
    [HideInInspector] public bool isRMove;
    [HideInInspector] public bool isLMove;
    [HideInInspector] public bool isDown;
    [HideInInspector] public bool isRight;
    [HideInInspector] public bool isAvoid;
    [HideInInspector] public bool isDash;
    [HideInInspector] public bool isSquat;
    [HideInInspector] public bool isUp;
    [HideInInspector] public bool isEnAt;

    public PlayMakerFSM fsm;



    //変数を宣言

    Animator anim;
    Rigidbody2D rb;
    string groundTag = "Ground";
    bool isGroundEnter, isGroundStay, isGroundExit;
    string moveFloorTag = "MoveFloor";
    public bool isJump = false;
    float dashTime, jumpTime, rushTime, fallTime;
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
    SimpleAnimation sAni;
    PlayMakerFSM pm;
    AttackM at;

    MoveObject moveObj;



    private void Awake()
    {
        Application.targetFrameRate = 30;
    }
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sAni = GetComponent<SimpleAnimation>();
        pm = GetComponent<PlayMakerFSM>();
        at = Player.GetComponent<AttackM>();


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
            else if (avoidJudge >= 1.5)
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



        if (!isDown && !isAvoid && !at.isAttack)
        {
            isEnAt = true;


            if (isGround && !isJump)
            {
                ySpeed = 0.0f;


                fallTime = 0.0f;
                if (horizontalkey > 0)
                {

                    transform.localScale = new Vector3(1, 1, 1);
                    Debug.Log("右入力");
                    isRight = true;
                    if (isDash)
                    {
                        sAni.Play("Dash");
                        xSpeed = dashSpeed;
                        rushTime += Time.fixedDeltaTime;

                    }
                    else if (isSquat)
                    {
                        sAni.Play("SquatM");
                        xSpeed = squatSpeed;

                    }
                    else
                    {
                        sAni.Play("Move");
                        xSpeed = speed;
                        dashTime += Time.fixedDeltaTime;
                    }
                }
                else if (horizontalkey < 0)
                {

                    transform.localScale = new Vector3(-1, 1, 1);

                    isRight = false;

                    if (isDash)
                    {
                        sAni.Play("Dash");
                        xSpeed = -dashSpeed;
                        rushTime += Time.fixedDeltaTime;

                    }
                    else if (isSquat)
                    {
                        sAni.Play("SquatM");
                        xSpeed = -squatSpeed;

                    }
                    else
                    {
                        sAni.Play("Move");
                        xSpeed = -speed;
                        dashTime += Time.fixedDeltaTime;
                    }
                }

                else
                {
                    sAni.Play("Default");
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
                    if (horizontalkey == 0)
                    {
                        sAni.Play("Squat");
                    }
                    isSquat = true;
                    SetLayer(9);
                }
                else
                {
                    isSquat = false;
                    isJump = false;
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
                if (jumpTime <= jumpRes && verticalkey > 0)
                {
                    ySpeed = jumpSpeed;
                    xSpeed *= jumpMCurve.Evaluate(jumpTime);
                    ySpeed *= jumpCurve.Evaluate(jumpTime);
                    sAni.Play("Jump");
                    Debug.Log("Getback");


                }
                else
                {
                    isJump = false;
                }

            }
            else if (!isJump && !isGround)
            {
                fallTime += Time.fixedDeltaTime;
                sAni.Play("Fall");
                jumpTime = 0.0f;
                ySpeed = -gravity;
                ySpeed *= fallCurve.Evaluate(fallTime);
                xSpeed *= fallMCurve.Evaluate(fallTime);


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
            isEnAt = false;

            avoidTime += Time.fixedDeltaTime;

            if (isRight)
            {
                //プレイヤーが右側、かつクールタイム消化。
                if (avoidTime < avoidRes)
                {
                    sAni.Play("Roll");

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
                if (avoidTime < avoidRes)
                {
                    sAni.Play("Roll");
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

    void AChange()
    {
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

    // public bool isEnableAt()
    // {
    //   if(!isAvoid && isGround)

    // }

}






