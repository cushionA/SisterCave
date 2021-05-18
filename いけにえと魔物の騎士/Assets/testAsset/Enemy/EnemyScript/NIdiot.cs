using UnityEngine;

public class NIdiot : EnemyBase
{
    //ジャンプしてるとき回避させるな
    //フラグの重複に気を付けて
    //当たり判定はある程度太くないといけない

    float direX;//モーションのX方向を決める
    float direY;//モーションのｙ方向を決める
    float attackChanceTime;

    // Start is called before the first frame update
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
        Die();

        base.FixedUpdate();
        if (isAggressive)
        {
         //   //Debug.Log($"おちんたいむ{attackChanceTime}");
            if (Mathf.Abs(distance.x) - status.agrDistance.x <= 10)
            {
                attackChanceTime += Time.fixedDeltaTime;
                if(attackChanceTime >= 1)
                {
                //    //Debug.Log("おちんko");
                    if (RandomValue(1,100) <= 40)
                    {
                        //SetAttackNumber(0);
                        attackNumber = 0;
                        Attack();
             //           //Debug.Log("おちんぽ");
                    }
                    attackChanceTime = 0.0f;
                    
                }

            }
            else
            {
                attackChanceTime = 0.0f;
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
        GroundJump(transform.localScale.x * status.jumpMove,status.jumpPower * 1.2f);
       // JumpCancel();
        Avoid(direX);

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
    void RandomDirection(int upperRes,int separate)
    {
        if(separate >= upperRes)
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
