using DarkTonic.MasterAudio;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    //インスペクタで設定
    [HideInInspector]public float speed;
    [HideInInspector]public float dashSpeed;
    [SerializeField] float squatSpeed;
    public float gravity;//重力変わるギミックとか攻撃とか
    [SerializeField] float jumpSpeed;
    [SerializeField] float jumpRes;
    [HideInInspector]public float avoidRes;
    [SerializeField] float avoidWait;
    [SerializeField] float rJumpRes;
    public AnimationCurve dashCurve;
    public AnimationCurve rushCurve;
    public AnimationCurve jumpCurve;
    public AnimationCurve jumpMCurve;
    public AnimationCurve fallCurve;
    public AnimationCurve fallMCurve;
    [SerializeField] float avoidSpeed;
    public GameObject Serch;
    [SerializeField] Vector2 jumpForce;
    //ロープジャンプの速度

     [SerializeField]float airMSpeed;
    //空中移動

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
    [HideInInspector] public bool isRJump;

    [HideInInspector]public bool isStop;



    public PlayMakerFSM fsm;
    public GameObject Guard;


    //変数を宣言

    Animator anim;
    Rigidbody2D rb;

    string groundTag = "Ground";
    bool isGroundEnter, isGroundStay, isGroundExit;
    string moveFloorTag = "MoveFloor";
    [HideInInspector] public bool isJump = false;
    float dashTime, jumpTime, rushTime, fallTime;
    string enemyTag = "Enemy";
    float avoidJudge;
    float horizontalkey;
    float verticalkey;
    float avoidKey;
    [HideInInspector]public float xSpeed;
    [HideInInspector]public float ySpeed;
    float avoidTime;
    float rushSt;
    [HideInInspector] public bool isSloopDown;



    PlayMakerFSM pm;
    AttackM at;
    MoveObject moveObj;

    ///<summary>
    ///マスクしてやれば地面しか検出しない。
    ///</summary>
    [SerializeField] ContactFilter2D filter;
    [SerializeField] LayerMask rayFilter;
    [SerializeField] float rayDis;
    public float sloopForce;

    private void Awake()
    {
        Application.targetFrameRate = 30;
    }
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        pm = GetComponent<PlayMakerFSM>();
        at = this.gameObject.GetComponent<AttackM>();

    }

    // Update is called once per frame

    public void Update()
    {
        

        #region//入力系
        if (Time.timeScale != 0.0f)
        {

            horizontalkey = GManager.instance.InputR.GetAxisRaw(MainUI.instance.rewiredAction0);
            verticalkey = GManager.instance.InputR.GetAxisRaw(MainUI.instance.rewiredAction2);
            if (GManager.instance.pStatus.stamina >= 1 && !GManager.instance.isGBreak && !GManager.instance.isAttack && GManager.instance.onGimmick && !isStop)
            {
                if (GManager.instance.guardEnable && GManager.instance.InputR.GetButton(MainUI.instance.rewiredAction11))
                {
                    GManager.instance.isGuard = true;
                }
                else if (GManager.instance.guardEnable && GManager.instance.guardHit)
                {
                    GManager.instance.isGuard = true;
                }
            }
            else if(!GManager.instance.InputR.GetButton(MainUI.instance.rewiredAction11))
            {
                GManager.instance.isGuard = false;
            }

            if (!isAvoid && isGround)
            {
                avoidKey = GManager.instance.InputR.GetAxisRaw(MainUI.instance.rewiredAction4);
            }
            else if (isAvoid)
            {
                avoidKey = 0.0f;
                //入力不可に
            }
            if (avoidKey == 1 && GManager.instance.isEnable)
            {
                avoidJudge += Time.deltaTime;
                if (horizontalkey != 0 && !GManager.instance.isGuard && !isSquat)
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
                    GManager.instance.StaminaUse(18);
                    isDash = false;
                    avoidJudge = 0.0f;
                    isAvoid = true;
                    MasterAudio.PlaySound("FireS");
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
        //  Debug.Log($"Yは{ySpeed}です");

        #region//フラグ管理

        if (isAvoid || isJump || isDash || GManager.instance.isAttack || GManager.instance.isGuard || GManager.instance.isGBreak)
        {

            isStUse = true;


        }
        else
        {
            isStUse = false;

        }

        /*        if (isGroundEnter || isGroundStay)
                {
                    isGround = true;
                   // //Debug.log("接地");
                }
                else// if (isGroundExit)
                {
                    isGround = false;
                }

                isGroundEnter = false;
                isGroundStay = false;
                isGroundExit = false;
                */
        #endregion

        //isGround = rb.IsTouching(filter);
        ///<summary>
        ///坂道下るとき浮かないようにする処理
        /// </summary>
        #region
        //isGround = rb.IsTouching(filter);
        //Debug.DrawRay(transform.position, Vector3.down, Color.blue, rayDis, false);
        if (rb.IsTouching(filter))
        {
            isGround = true;

        }

        else
        {

            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayDis, rayFilter);
            if (hit && isGround)
            {

                
                 isGround = true;
                // rb.AddForce(new Vector2(0,-80),ForceMode2D.Force);

            }
            else
            {
                //  Debug.Log("ハヤシライス");
                isGround = false;
                //}
            }
        }
        
        #endregion
        if (GManager.instance.isGuard && GManager.instance.pStatus.stamina <= 0)
        {
            GManager.instance.isGBreak = true;
            GManager.instance.isGuard = false;
        }
        Guard.SetActive(GManager.instance.isGuard == true ? true:false);

        if (!isDown && !isAvoid && !GManager.instance.isAttack && !isStop && !GManager.instance.onGimmick && !GManager.instance.guardHit)
        {
            isEnAt = true;
            //攻撃できる

            //Debug.Log($"{rb.velocity.y}だう");
            if (isGround && !isJump)
            {
              //  ySpeed = 0.0f;


                fallTime = 0.0f;
                if (horizontalkey > 0)
                {
                    #region //  横移動と向き変更詰め合わせ
                    transform.localScale = new Vector3(1, 1, 1);

                    isRight = true;
                    if (isDash && GManager.instance.isEnable)
                    {
                        if (!GManager.instance.pStatus.equipWeapon.twinHand)
                        {
        
                            anim.Play("ODash");
                        }
                        else
                        {
                            anim.Play("TDash");
                        }

                        xSpeed = dashSpeed;
                        rushTime += Time.fixedDeltaTime;
                        rushSt += Time.fixedDeltaTime;

                        if (rushSt >= 0.1f)
                        {

                            GManager.instance.StaminaUse(2);
                            rushSt = 0.0f;
                        }

                    }
                    else if (isSquat)
                    {
                        if (!GManager.instance.pStatus.equipWeapon.twinHand)
                        {

                            anim.Play("OSquatMove");
                        }
                        else
                        {
                            anim.Play("TSquatMove");
                        }
                        xSpeed = squatSpeed;

                    }
                    else
                    {
                        if (!GManager.instance.pStatus.equipWeapon.twinHand)
                        {

                            anim.Play("OMove");
                        }
                        else
                        {
                            anim.Play("TMove");
                        }
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
                        if (!GManager.instance.pStatus.equipWeapon.twinHand)
                        {

                            anim.Play("ODash");
                        }
                        else
                        {
                            anim.Play("TDash");
                        }
                        xSpeed = -dashSpeed;
                        rushTime += Time.fixedDeltaTime;
                        rushSt += Time.fixedDeltaTime;

                        if (rushSt >= 0.1f)
                        {

                            GManager.instance.StaminaUse(2);
                            rushSt = 0.0f;
                        }

                    }
                    else if (isSquat)
                    {
                        if (!GManager.instance.pStatus.equipWeapon.twinHand)
                        {

                            anim.Play("OSquatMove");
                        }
                        else
                        {
                            anim.Play("TSquatMove");
                        }
                        xSpeed = -squatSpeed;

                    }
                    else
                    {
                        if (!GManager.instance.pStatus.equipWeapon.twinHand)
                        {

                            anim.Play("OMove");
                        }
                        else
                        {
                            anim.Play("TMove");
                        }
                        xSpeed = -speed;
                        dashTime += Time.fixedDeltaTime;
                    }
                }

                else if(!isSquat)
                {
                   
                    if (!GManager.instance.pStatus.equipWeapon.twinHand)
                    {

                        anim.Play("OStand");
                       // Debug.Log("台無し");
                    }
                    else
                    {
                        anim.Play("TStand");
                    }
                    ySpeed = 0.0f;
                    xSpeed = 0.0f;
                    dashTime = 0.0f;
                    rushTime = 0.0f;
                }
                #endregion

                #region//空中動作としゃがみ

                if (verticalkey > 0 && GManager.instance.isEnable && !isJump && !isSquat)
                {
                    GManager.instance.StaminaUse(15);
                    isJump = true;


                }
                else if (verticalkey < 0)
                {
                    if (horizontalkey == 0)
                    {
                        if (!GManager.instance.pStatus.equipWeapon.twinHand)
                        {

                            anim.Play("OSquat");
                        }
                        else
                        {
                            anim.Play("TSquat");
                        }
                        ySpeed = 0.0f;
                        xSpeed = 0.0f;
                        dashTime = 0.0f;
                        rushTime = 0.0f;
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
                jumpTime += Time.fixedDeltaTime;
                SetLayer(11);
                if (jumpTime <= jumpRes && verticalkey > 0)
                {
                  //  Debug.Log("ちくわぶ");
                    ySpeed = jumpSpeed;
                    xSpeed *= jumpMCurve.Evaluate(jumpTime);
                    ySpeed *= jumpCurve.Evaluate(jumpTime);
                    if (!GManager.instance.pStatus.equipWeapon.twinHand)
                    {

                        anim.Play("OJump");
                    }
                    else
                    {
                        anim.Play("TJump");
                    }


                }
                else
                {
                    isJump = false;
                    jumpTime = 0.0f;
                }


            }
            else if (!isJump && !isGround && !isRJump)
            {

                fallTime += Time.fixedDeltaTime;
                if (fallTime >= 0.2)
                {
                    if (!GManager.instance.pStatus.equipWeapon.twinHand)
                    {

                        anim.Play("OFall");
                    }
                    else
                    {
                        anim.Play("TFall");
                    }
                }
            //    Debug.Log("ちくわさらだ");
                ySpeed = -gravity;
                ySpeed *= fallCurve.Evaluate(fallTime);
                xSpeed *= fallMCurve.Evaluate(fallTime);
            }

            else if(isRJump)
            {
                #region ロープジャンプ
                jumpTime += Time.fixedDeltaTime;
                if (isRight)
                {
                    //プレイヤーが右側、かつクールタイム消化。
                    if (jumpTime <= rJumpRes)
                    {
                        xSpeed = jumpForce.x;
                        ySpeed = jumpForce.y;
                        xSpeed *= jumpMCurve.Evaluate(jumpTime);
                        ySpeed *= jumpCurve.Evaluate(jumpTime);
                        rb.velocity = new Vector2(xSpeed,ySpeed);
                    }
                    else
                    {
                        jumpTime = 0.0f;
                        isRJump = false;
                    }

                }
                else if (!isRight)
                { //プレイヤーが左側、かつクールタイム消化。

                    if (jumpTime <= rJumpRes)
                    {
                        xSpeed = -jumpForce.x;
                        ySpeed = jumpForce.y;
                        xSpeed *= jumpCurve.Evaluate(jumpTime);
                        ySpeed *= jumpMCurve.Evaluate(jumpTime);
                        rb.velocity = new Vector2(xSpeed, ySpeed);
                    }
                    else
                    {
                        isRJump = false;
                        jumpTime = 0.0f;
                    }


                }
                #endregion
            }
            #endregion

            //移動速度を設定
            Vector2 addVelocity = Vector2.zero;
                if (moveObj != null)
                {
                    addVelocity = moveObj.GetVelocity();
                }
            
            //Debug.Log($"Xは{xSpeed}だよ");

                rb.velocity = new Vector2(xSpeed, ySpeed) + addVelocity;

            }
        

         if (!isGround && !GManager.instance.isAttack) {
            //空中で地味に動くためのやつ

            Vector2 move;
           
            if (horizontalkey > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
                isRight = true;

                move = new Vector2(airMSpeed, 0.0f);
                rb.AddForce(move, ForceMode2D.Force);

            }

            else if (horizontalkey < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                isRight = false;
                move = new Vector2(-airMSpeed, 0.0f);
                rb.AddForce(move, ForceMode2D.Force);
            }
        }


 

        #region//回避
        if (isAvoid)
        {
            fallTime = 0.0f;

            isEnAt = false;
            avoidTime += Time.fixedDeltaTime;


            if (isRight)
            {

                //プレイヤーが右側、かつクールタイム消化。
                if (avoidTime <= avoidRes)
                {
                    if (!GManager.instance.pStatus.equipWeapon.twinHand)
                    {

                        anim.Play("OAvoid");
                    }
                    else
                    {
                        anim.Play("TAvoid");
                    }

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
                    if (!GManager.instance.pStatus.equipWeapon.twinHand)
                    {

                        anim.Play("OAvoid");
                    }
                    else
                    {
                        anim.Play("TAvoid");
                    }
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

       // //Debug.log($"攻撃中{GManager.instance.isAttack}");
       // //Debug.log($"空中攻撃{GManager.instance.airAttack}");
        if (GManager.instance.isAttack && !GManager.instance.airAttack)
        {
            //Debug.log("こたつ");
            //空中攻撃以外では重力を適用
            rb.velocity = new Vector2(rb.velocity.x, -gravity);
        }


        #endregion



    }

    void AChange()
    {

        isAvoid = false;
        avoidTime = 0.0f;

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
 /*   private void OnTriggerEnter2D(Collider2D collision)
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
 */
    #endregion
    public void SetLayer(int layerNumber)
    {

        this.gameObject.layer = layerNumber;

    }

    // public bool isEnableAt()
    // {
    //   if(!isAvoid && isGround)

    // }
    public void Flip()
    {
        // Switch the way the player is labelled as facing.
        isRight = !isRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void RopeJump()
    {
        isJump = false;
        jumpTime = 0.0f;
        this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        xSpeed = 0;
        ySpeed = 0;
        isRJump = true;
    }

   public void Stop()
    {
            rb.velocity = new Vector2(0, 0);

            isJump = false;
            isDash = false;
            isSquat = false;
            isAvoid = false;
            GManager.instance.isAttack = false;
            dashTime = 0.0f;
            jumpTime = 0.0f;
            rushTime = 0.0f;
            fallTime = 0.0f;
            isGround = false;

            SetLayer(11);

            return;
    }

    void GuardBreake()
    {
        GManager.instance.isGBreak = false;
    }

}


    







