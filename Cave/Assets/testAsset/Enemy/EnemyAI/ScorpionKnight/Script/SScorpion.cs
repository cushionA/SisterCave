using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SScorpion : EnemyBase
{
    //�W�����v���Ă�Ƃ�����������
    //�t���O�̏d���ɋC��t����
    //�����蔻��͂�����x�����Ȃ��Ƃ����Ȃ�

    //�U���ԍ�����
    //0�A�ʏ�U��,�P�A���c�A��,�R�A���З͂̂�����,�S�A�����l�߂�˂�

    float direX;//���[�V������X���������߂�
    float direY;//���[�V�����̂����������߂�
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
   //     Debug.Log($"���Ă邩{isAnimeStart}");



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
                        //   Debug.Log("�A���U��");
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
            // ////Debug.log($"���C���[{this.gameObject.layer}");
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
