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
    public GameObject Serch;
    //public GameObject attack;

    [HideInInspector] public bool isGround;
    [HideInInspector] public bool isContinue = false;
    [HideInInspector] public bool isRMove;
    [HideInInspector] public bool isLMove;
    [HideInInspector] public bool isDown;
    [HideInInspector] public bool isRight = true;
    [HideInInspector] public bool isAvoid;
    [HideInInspector] public bool isDash;
    [HideInInspector] public bool isSquat;
    [HideInInspector] public bool isUp;
    [HideInInspector] public bool isEnAt;
    [HideInInspector] public bool isStUse;

    bool isStartRoll;
    bool isStop;
    


    public PlayMakerFSM fsm;



    //変数を宣言

    Animator anim;
    Rigidbody2D rb;
    GimmickAct ga;

    string groundTag = "Ground";
    bool isGroundEnter, isGroundStay, isGroundExit;
    string moveFloorTag = "MoveFloor";
    [HideInInspector]public bool isJump = false;
    float dashTime, jumpTime, rushTime, fallTime;
    string enemyTag = "Enemy";
    float avoidJudge;
    float horizontalkey;
    float verticalkey;
    float avoidKey;
    float xSpeed;
    float ySpeed;
    float avoidTime;
    float rushSt;

 


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
        at = this.gameObject.GetComponent<AttackM>();
        ga = Serch.GetComponent<GimmickAct>();

    }

    // Update is called once per frame

    public void Update()
    {
        #region//入力系
        if (Time.timeScale != 0.0f && !ga.isGimmickOn)
        {

            horizontalkey = Input.GetAxisRaw("Horizontal");
            verticalkey = Input.GetAxisRaw("Vertical");
            if (!isAvoid)
            {
                avoidKey = Input.GetAxisRaw("Avoid");
            }
            else if (isAvoid)
            {

                avoidKey = 0.0f;

            }
            if (avoidKey == 1 && GManager.instance.isEnable)
            {

                avoidJudge += Time.deltaTime;
                if (horizontalkey != 0)
                {
                    isDash = true;

                    //isDashをfalseにできてなかった。
                }
                else
                {
                    isDash = false;

                }

                }
            if (avoidKey == 0)
            {
                

                if (avoidJudge > 0.0f && avoidJudge < 1.0f && GManager.instance.isEnable)
                {
                  

                    //回避の最初のところは七回呼ばれてる。ここも七回呼ばれてる。なぜか動かない
                    GManager.instance.currentSt -= 18.0f;
                   
                    
                    isDash = false;
                    avoidJudge = 0.0f;
                    isAvoid = true;
  
                }
                else
                {
                    avoidJudge = 0.0f;
                    isDash = false;

                }

            }
        }
        #endregion

    }

    void FixedUpdate()
    {


        #region//フラグ管理

        if (isAvoid || isJump || isDash || at.isAttack)
        {

            isStUse = true;


        }
        else
        {
            isStUse = false;

        }

        if (ga.isGimmickOn)
        {
            isStop = true;
        }
        else
        {
            isStop = false;
        }


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

        #endregion

        if (isStop)
        {
            rb.velocity = new Vector2(0, 0);

            isJump = false;
            isDash = false;
            isSquat = false;
            isAvoid = false;
            isStartRoll = false;
            at.isAttack = false;
            dashTime = 0.0f;
            jumpTime = 0.0f;
            rushTime = 0.0f;
            fallTime = 0.0f;

            SetLayer(11);

            return;
        }


        if (!isDown && !isAvoid && !at.isAttack && !isStop)
        {
            isEnAt = true;


            if (isGround && !isJump)
            {
                ySpeed = 0.0f;


                fallTime = 0.0f;
                if (horizontalkey > 0)
                {
                    #region //  横移動と向き変更詰め合わせ
                    transform.localScale = new Vector3(1, 1, 1);

                    isRight = true;
                    if (isDash && GManager.instance.isEnable)
                    {
                        sAni.Play("Dash");
                        xSpeed = dashSpeed;
                        rushTime += Time.fixedDeltaTime;
                        rushSt += Time.fixedDeltaTime;

                        if (rushSt >= 0.1f)
                        {

                            GManager.instance.currentSt -= 2.0f;
                            rushSt = 0.0f;
                        }

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

                    if (isDash && GManager.instance.isEnable)
                    {
                        sAni.Play("Dash");
                        xSpeed = -dashSpeed;
                        rushTime += Time.fixedDeltaTime;
                        rushSt += Time.fixedDeltaTime;

                        if (rushSt >= 0.1f)
                        {

                            GManager.instance.currentSt -= 2.0f;
                            rushSt = 0.0f;
                        }

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
                #endregion

                #region//空中動作としゃがみ

                if (verticalkey > 0 && GManager.instance.isEnable && !isJump)
                {
                    GManager.instance.currentSt -= 15.0f;
                    isJump = true;


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
                    SetLayer(11);
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

                Debug.Log("ごめん");

                jumpTime += Time.fixedDeltaTime;
                SetLayer(11);
                if (jumpTime <= jumpRes && verticalkey > 0)
                {
                    ySpeed = jumpSpeed;
                    xSpeed *= jumpMCurve.Evaluate(jumpTime);
                    ySpeed *= jumpCurve.Evaluate(jumpTime);
                    sAni.Play("Jump");


                }
                else
                {
                    isJump = false;
                    jumpTime = 0.0f;
                }


            }
            else if (!isJump && !isGround)
            {

                fallTime += Time.fixedDeltaTime;
                sAni.Play("Fall");
                ySpeed = -gravity;
                ySpeed *= fallCurve.Evaluate(fallTime);
                xSpeed *= fallMCurve.Evaluate(fallTime);



            }
            #endregion

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

            isStartRoll = true;

        }
        else if(!isGround && isAvoid)
        {
            rb.velocity = new Vector2(xSpeed, ySpeed);
        }

        #region//回避
        if (isStartRoll)
        {
            isEnAt = false;
            avoidTime += Time.fixedDeltaTime;


            if (isRight)
            {

                //プレイヤーが右側、かつクールタイム消化。
                if (avoidTime <= avoidRes)
                {
                    sAni.Play("Roll");

                    SetLayer(10);
                    rb.velocity = new Vector2(avoidSpeed, -gravity);

                }
                else
                {

                    rb.velocity = new Vector2(0, -gravity);

                    SetLayer(11);

                    Invoke("AChange", 0.3f);

                }


            }

            else if (!isRight)
            { //プレイヤーが左側、かつクールタイム消化。


                if (avoidTime <= avoidRes)
                {
                    sAni.Play("Roll");
                    SetLayer(10);
                    //突進の制限時間三秒である以内は方向転換を禁じて進み続ける。
                    rb.velocity = new Vector2(-avoidSpeed, -gravity);

                }
                else
                {

                    rb.velocity = new Vector2(0, -gravity);

                    SetLayer(11);

                    Invoke("AChange", 0.3f);
                }

            }

        }

        #endregion



    }

    void AChange()
    {
        
        isAvoid = false;
        avoidTime = 0.0f;
        isStartRoll = false;
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


    #region//接地判定と動く床
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

    #endregion
    public void SetLayer(int layerNumber)
    {

        this.gameObject.layer = layerNumber;

    }

    // public bool isEnableAt()
    // {
    //   if(!isAvoid && isGround)

    // }

}






