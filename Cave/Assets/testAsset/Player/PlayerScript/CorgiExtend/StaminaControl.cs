using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using UnityEngine.AddressableAssets;
using Rewired.Integration.CorgiEngine;
using DarkTonic.MasterAudio;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// スタミナ管理を行うコード
    /// 現在走っているかなどの状態をもとにスタミナを減少させる
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/StaminaControl")]
    public class StaminaControl : MyAbillityBase
    {
        /// このメソッドは、ヘルプボックスのテキストを表示するためにのみ使用されます。
        /// 能力のインスペクタの冒頭にある
        public override string HelpBoxText() { return "スタミナ管理に使うコード"; }

        //スタミナ継続使用中かどうか
        bool isUsing;

        /// <summary>
        /// スタミナ使用関連で時間を計測
        /// </summary>
       float stTime;

        CharacterStates.MovementStates lastState = CharacterStates.MovementStates.Nostate;

        /// <summary>
        ///　ここで、パラメータを初期化する必要があります。
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();

        }

        /// <summary>
        /// 
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
     //       StaminaController();
            StaminaJudge();
        }



        /// <summary>
        /// スタミナ管理をする
        /// </summary>
        protected virtual void StaminaController()
        {
            if (NoUseJudge())
            {            Debug.Log($"カルピス{_movement.CurrentState}");
                //スタミナを使用してない状態に
                GManager.instance.isStUse = false;

                stTime = 0f;

            }

            else
            {
                //スタミナ使用中にして
                GManager.instance.isStUse = true;

            }

        }


        /// <summary>
        /// スタミナを利用してない状態かどうかを判断
        /// </summary>
        /// <returns></returns>
        protected virtual bool NoUseJudge()
        {


            //ガードに関してはガード中かつスタミナ使用中、またはダウン中でなければ
            return (!GManager.instance.isStUse && _condition.CurrentState == CharacterStates.CharacterConditions.Normal);
        }

        /// <summary>
        /// スタミナ使用の詳細を詰める
        /// </summary>
        protected virtual void StaminaJudge()
        {

            if(lastState != _movement.CurrentState)
            {
                if (_movement.CurrentState == CharacterStates.MovementStates.Running)
                {

                    GManager.instance.StaminaUse((int)(GManager.instance.dashSt * GManager.instance.stRatio));
                    GManager.instance.isStUse = true;
                    isUsing = true;

                }
                else if (_movement.CurrentState == CharacterStates.MovementStates.Jumping || _movement.CurrentState == CharacterStates.MovementStates.DoubleJumping)
                {

                    GManager.instance.StaminaUse((int)(GManager.instance.jumpSt * GManager.instance.stRatio));
                    GManager.instance.isStUse = true;

                }
                else if (_movement.CurrentState == CharacterStates.MovementStates.Rolling)
                {

                    GManager.instance.StaminaUse((int)(GManager.instance.rollSt * GManager.instance.stRatio));
                    GManager.instance.isStUse = true;

                }
                else if (!isUsing)
                {
                    GManager.instance.isStUse = false;
                }
                lastState = _movement.CurrentState;
            }


            if (isUsing)
            {
                stTime += _controller.DeltaTime;

                if (_movement.CurrentState == CharacterStates.MovementStates.Running)
                {
                    //スタミナ未使用か、あるいは0.1秒ごとに
                    if (stTime >= 0.1)
                    {
                        GManager.instance.StaminaUse((int)(GManager.instance.dashSt * GManager.instance.stRatio));
                        GManager.instance.isStUse = true;
                        stTime = 0;
                    }
                }
                else
                {
                    isUsing = false;
                    stTime = 0;
                    GManager.instance.isStUse = false;
                }
            }

 /*

            if(_movement.CurrentState == CharacterStates.MovementStates.Running)
            {
                stTime += _controller.DeltaTime;

                //スタミナ未使用か、あるいは0.1秒ごとに
                if(!isUsing || stTime >= 0.1)
                {
                    GManager.instance.StaminaUse((int)(GManager.instance.dashSt * GManager.instance.stRatio));
                    isUsed = true;
                    stTime = 0;
                }
                
            }
            else if (_movement.CurrentState == CharacterStates.MovementStates.Jumping || _movement.CurrentState == CharacterStates.MovementStates.DoubleJumping)
            {
                if (!isUsed)
                {
                    GManager.instance.StaminaUse((int)(GManager.instance.jumpSt * GManager.instance.stRatio));
                    isUsed = true;
                }
            }
            else if (_movement.CurrentState == CharacterStates.MovementStates.Rolling)
            {
                if (!isUsed)
                {
                    GManager.instance.StaminaUse((int)(GManager.instance.rollSt * GManager.instance.stRatio));
                    isUsed = true;
                }
            }


            //武器関連はWeaponAbillityで消費させてたわ
            //多分ガードも同じ
           else if (_movement.CurrentState == CharacterStates.MovementStates.Attack)
            {
                if (!isUsed)
                {
                    //武器振ってもスタミナが消費されないときは多分こいつのせい
                    GManager.instance.StaminaUse((int)GManager.instance.useAtValue.useStamina);
                    isUsed = true;
                }
            }*/
            /*   else if (_movement.CurrentState == CharacterStates.MovementStates.)
               {
                   if (!isUsed)
                   {
                       GManager.instance.StaminaUse((int)(GManager.instance. * GManager.instance.stRatio));
                       isUsed = true;
                   }

               }*/

        }

    }
}
