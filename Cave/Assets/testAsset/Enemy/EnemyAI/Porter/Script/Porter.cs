using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    public class Porter : EnemyAIBase
    {

        float attackChanceTime;//�U���Ԋu
                               //[SerializeField] Material myWing;
        bool isChange;
        // Start is called before the first frame update
        int take;
        int moveType;

        [SerializeField]
        Material wingMatt;

        protected override void Initialization()
        {
            base.Initialization();
        }



        // Update is called once per frame
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            
            if (isAggressive)
            {


                attackChanceTime += _controller.DeltaTime;


                if (attackChanceTime >= 6)
                {
                    if (_movement.CurrentState != CharacterStates.MovementStates.Attack)
                    {

                        if (_condition.CurrentState == CharacterStates.CharacterConditions.Stunned)
                        {
                            attackChanceTime = 0f;
                        }

                        //�p�^�[���ύX
                        if ((ground == EnemyStatus.MoveState.stay && air == EnemyStatus.MoveState.stay) && !isChange)
                        {
                            isChange = true;
                            ground = EnemyStatus.MoveState.wakeup;
                        }
                        AgrFly(2);

                    }
                    //�s���ύX��Ȃ�
                    if (ground == EnemyStatus.MoveState.stay && air == EnemyStatus.MoveState.stay && isChange )
                    {
                        //���ɂ��鎞��at2
                        isChange = false;
                        //SetAttackNumber(0);
                        //attackNumber = 0;

                        if (targetPosition.x > transform.position.x)
                        {
                            Attack(true, 2, true);
                        }
                        else
                        {
                            Attack(true, 1, true);
                        }
               //         Debug.Log("����");

                        attackChanceTime = 3.0f;
                        take = 2;
                    }
                    else if (attackChanceTime >= 10)
                    {
                        attackChanceTime = 0;
                        isChange = false;
                    }
                }
                else if (_movement.CurrentState != CharacterStates.MovementStates.Attack)
                {
                    if (attackChanceTime < 6)
                    {
                       // 
                        AgrFly(moveType);
                    }
                    if (attackChanceTime >= 1.5 * take && take < 4 && attackChanceTime < 6)
                    {
                        int sai = RandomValue(1, 100);
                        if (sai <= 50)
                        {
                            moveType = 0;
                        }
                        else
                        {
                            moveType = 1;
                        }
                        take++;
                        if (_condition.CurrentState == CharacterStates.CharacterConditions.Stunned)
                        {
                            //      �ړ��^�C�v��ύX
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


        }


    }
}