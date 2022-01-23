using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Porter : EnemyBase
{

    float attackChanceTime;//UŒ‚ŠÔŠu
    //[SerializeField] Material myWing;
    bool isChange;
    // Start is called before the first frame update
    int take;
    int moveType;
   protected override void Start()
    {
        base.Start();
    }


    // Update is called once per frame
    protected override void FixedUpdate()
    {
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


                if (attackChanceTime >= 6)
                {
                    if (!isAttack)
                    {

                    if (isDown)
                    {
                        attackChanceTime = 0f;
                    }

                    if ((ground == EnemyStatus.MoveState.stay && air == EnemyStatus.MoveState.stay) && !isChange)
                    {
                        isChange = true;
                        ground = EnemyStatus.MoveState.wakeup;
                    }
                        AgrFly(2);
                    
                    }
                    if (ground == EnemyStatus.MoveState.stay && air == EnemyStatus.MoveState.stay && isChange)
                    {

                    isChange = false;
                    //SetAttackNumber(0);
                    //attackNumber = 0;
                          Attack(true, 0,true);

                        attackChanceTime = 3.0f;
                        take = 2;
                    }
               else if(attackChanceTime >= 10)
                    {
                        attackChanceTime = 0;
                        isChange = false;
                    }
                }
            else if(!isAttack)
            {
                if(attackChanceTime < 6)
                {
                    AgrFly(moveType);
                }
                if (attackChanceTime >= 1.5 * take && take < 4 && attackChanceTime < 6)
                {
                    int sai = RandomValue(1,100);
                    if(sai <= 50)
                    {
                        moveType = 0;
                    }
                    else
                    {
                        moveType = 1;
                    }
                    take++;
                    if (isDown)
                    {
                        //      ˆÚ“®ƒ^ƒCƒv‚ð•ÏX
                        moveType = moveType == 0 ? 1 : 1;
                    }
                }
                else if (take >= 4)
                {
                    take = 0;
                }


            }
            



           // TriggerJump();
        }
        else if (!isAggressive)
        {
              PatrolFly();
            //PatrolMove();
        }

        //  AirJump(direX * status.combatSpeed.x / 2);
        GroundJump(transform.localScale.x * status.jumpMove, status.jumpPower * 1.2f);
        // JumpCancel();
        // Avoid(direX);

    }
}
