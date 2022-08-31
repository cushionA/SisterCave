using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    public class SaberScorpion : EnemyAIBase
    {
        //ジャンプしてるとき回避させるな
        //フラグの重複に気を付けて
        //当たり判定はある程度太くないといけない

        //攻撃番号説明
        //0、通常攻撃,１、横縦連撃,３、高威力のしっぽ,４、距離詰める突き


        float attackChanceTime;
        bool rareAttack;
        [SerializeField] bool test;
        // Start is called before the first frame update
        [SerializeField] int tesnum;
        protected override void Initialization()
        {
            base.Initialization();
        }
 



        public override void ProcessAbility()
        {
            base.ProcessAbility();

            //     Debug.Log($"ついてるか{isAnimeStart}");



            base.ProcessAbility();
            if (isAggressive)
            {

                if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
                {
                    attackChanceTime += _controller.DeltaTime; 
                }
                if (test && !atV.isCombo && isAtEnable)
                {

                    //モーション中振り向けちゃう問題はこのテストモードだけ
                    Attack(true, tesnum);

                 //   test = false;
                }
                else
                {
                    if (attackChanceTime >= 1.5 && !atV.isCombo)
                    {
                        if (Mathf.Abs(distance.x) >= 45 && Mathf.Abs(distance.x) <= 60)
                        {
                            if (RandomValue(1, 100) <= 75)
                            {
                                Attack(true, 5);
                            }
                            attackChanceTime = 0.0f;
                        }
                        else if (Mathf.Abs(distance.x) - status.agrDistance[0].x < 25 && rareAttack)
                        {
                            if (RandomValue(1, 100) <= 55)
                            {
                                Attack(true, 4);
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
                                Attack(true, 1);
                            }
                            else if (RandomValue(1, 100) <= 60)
                            {
                                //   Debug.Log("連続攻撃");
                                Attack(true, 2);

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

                    
                if(isMovable)
                {
                    if (!guardJudge && (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove))
                    {
                     GroundGuardAct(60);
                    }
                    
                    
                    AgrMove();
                }

                
            }
            else if (!isAggressive)
            {
                //  PatrolFly();
                PatrolMove();
            }

            //  AirJump(direX * status.combatSpeed.x / 2);
            
            // JumpCancel();
            // Avoid(direX);


        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
        }
        protected override void OnTriggerStay2D(Collider2D collision)
        {
            base.OnTriggerStay2D(collision);
        }

    }
}