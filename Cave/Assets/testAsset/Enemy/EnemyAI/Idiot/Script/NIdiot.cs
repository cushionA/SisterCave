using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    public class NIdiot : EnemyAIBase
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

        [SerializeField] SpriteRenderer mat;

        protected override void Initialization()
        {
            base.Initialization();
        }

        public override void ProcessAbility()
        {
           // Debug.Log($"アイイイイイイイ");
            base.ProcessAbility();

            //UnBaindとかでカメラに映るまで動かない奴はちゃんと作り直せ


            if (isAggressive)
            {
               // Debug.Log($"ああああ");

                if (Mathf.Abs(distance.x) - status.agrDistance[0].x <= 10)
                {
                    attackChanceTime += _controller.DeltaTime;
                    if (attackChanceTime >= 3)
                    {
                        if (RandomValue(1, 100) <= 55)
                        {
                            //SetAttackNumber(0);
                            //attackNumber = 0;
                       //     Attack(true);
                        }
                        else if (RandomValue(1, 100) <= 60)
                        {
                           // Debug.Log("連続攻撃");
                     //       Attack(true, 1);
                        }
                        attackChanceTime = 0.0f;

                    }

                }
                else
                {
                    attackChanceTime = 0.0f;
                }

                if (isMovable)
                {
                   // Debug.Log($"ｄｓｆｓｆｓｆ");
                   // AgrMove();
                }
                else
                {
                   // Debug.Log($"ddddddddd");
                }

            }
            else if (!isAggressive)
            {
                //  PatrolFly();
               // PatrolMove();
            }




        }


    }
}