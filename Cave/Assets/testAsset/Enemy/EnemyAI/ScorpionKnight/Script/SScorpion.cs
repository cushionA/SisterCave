using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SScorpion : EnemyBase
{
    //ジャンプしてるとき回避させるな
    //フラグの重複に気を付けて
    //当たり判定はある程度太くないといけない

    //攻撃番号説明
    //0、通常攻撃,１、横縦連撃,３、高威力のしっぽ,４、距離詰める突き

    float direX;//モーションのX方向を決める
    float direY;//モーションのｙ方向を決める
    float attackChanceTime;
    bool rareAttack;
    [SerializeField]bool test;
    // Start is called before the first frame update
    [SerializeField] int tesnum;
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected override void Update()
    {

    }

    protected override void FixedUpdate()
    {
   //     Debug.Log($"ついてるか{isAnimeStart}");



        base.FixedUpdate();
        if (isAggressive)
        { 
            GroundGuardAct(60);
         //   isGuard = status.ground == EnemyStatus.MoveState.stay && !guardBreak && !isAttack ? true : false;
 /*                if(isDamage && !isDown && (Mathf.Sign(transform.localScale.x) != Mathf.Sign(GManager.instance.Player.transform.localScale.x)))
       {
                status.ground = EnemyStatus.MoveState.stay;
            }*/

            attackChanceTime += Time.fixedDeltaTime;
            if(test){
                Attack(true, tesnum);
            }
            else
            { 
            if (attackChanceTime >= 1.5)
            {
                if (Mathf.Abs(distance.x) >= 45 && Mathf.Abs(distance.x) <= 60)
                {
                    if (RandomValue(1, 100) <= 75)
                    {
                        Attack(true, 4);
                    }
                    attackChanceTime = 0.0f;
                }
                else if (Mathf.Abs(distance.x) - status.agrDistance[0].x < 25 && rareAttack)
                {
                    if (RandomValue(1, 100) <= 55)
                    {
                        Attack(true, 3);
                    }
                    rareAttack = false;
                    attackChanceTime = 0.0f;
                }
                else if (Mathf.Abs(distance.x) - status.agrDistance[0].x <= 10)
                {

                    if (RandomValue(1, 100) <= 55)
                    {
                        //SetAttackNumber(0);
                        //attackNumber = 0;
                        Attack(true, 0);
                    }
                    else if (RandomValue(1, 100) <= 60)
                    {
                        //   Debug.Log("連続攻撃");
                        Attack(true, 1);
                    }
                    else
                    {
                        rareAttack = true;
                    }
                    attackChanceTime = 0.0f;

                }
                else
                {
                    attackChanceTime = 0.0f;
                }
            }
        }
            ////Debug.log($"{status.ground}");
            if (!isAttack)
            {
                AgrMove();
            }
            //isGuard = true;
            // AgrFly();
            /*    if(RandomValue(0,50000) <= 4 && !nowJump && !isAvoid)
           {
               isJump = true;
               RandomDirection(100, 50);
           }
      if (GManager.instance.isAttack && !isAvoid && !nowJump)
           {
               isAvoid = true;
               RandomDirection(100, 50);
           }*/
            // ////Debug.log($"レイヤー{this.gameObject.layer}");
            TriggerJump();
        }
        else if (!isAggressive)
        {
            //  PatrolFly();
            PatrolMove();
        }

        //  AirJump(direX * status.combatSpeed.x / 2);
        GroundJump(transform.localScale.x * status.jumpMove, status.jumpPower * 1.2f);
        // JumpCancel();
       // Avoid(direX);

        if (RandomValue(0, 100) <= 1 && !nowJump && !isAvoid)
        {
            // ActionFire(0);
        }

    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
    protected override void OnTriggerStay2D(Collider2D collision)
    {
        base.OnTriggerStay2D(collision);
    }
    void RandomDirection(int upperRes, int separate)
    {
        if (separate >= upperRes)
        {
            separate = upperRes;
        }
        if (RandomValue(0, upperRes) <= separate)
        {
            direX = 1;
        }
        else
        {
            direX = -1;
        }
    }
}
