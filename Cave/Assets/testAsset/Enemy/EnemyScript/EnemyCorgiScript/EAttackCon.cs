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
    /// TODO_DESCRIPTION
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/EAttackCon")]
    public class EAttackCon : MyAbillityBase
    {
        /// このメソッドは、ヘルプボックスのテキストを表示するためにのみ使用されます。
        /// 能力のインスペクタの冒頭にある
        public override string HelpBoxText() { return "TODO_HELPBOX_TEXT."; }

        //   [Header("武器データ")]
        /// declare your parameters here
        ///WeaponHandle参考にして 



        protected const string _numParameterName = "AttackNumber";
        protected int _numAnimationParameter;
        [SerializeField]
        protected EnemyAIBase _base; 


        int motionNum;
        [HideInInspector]
        public bool nowAttack;




        public  void AttackEnd()
        {
         //
            motionNum = 0;
            _controller.DefaultParameters.Gravity = -_base.status.firstGravity;
            nowAttack = false;
            if(_condition.CurrentState == CharacterStates.CharacterConditions.Moving)
            {
                _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
               // Debug.Log("あじぇｋ");
            }

        }

        //どうせ呼ばれないから消す
        /*
        /// <summary>
        /// アビリティサイクルの開始時に呼び出され、ここで入力の有無を確認します。
        /// </summary>
        protected override void HandleInput()
        {
            //ここで何ボタンが押されているかによって行動変えたりできるねぇ
            //InputReactionフラグとかいいかも

            // here as an example we check if we're pressing down
            // on our main stick/direction pad/keyboard
            if (ReIManager.PrimaryMovement.y < -ReIManager.Threshold.y)
            {
                
            }//DoSomething();
            
        }
        */
        /// <summary>
        /// AIからこいつ呼び出す
        /// </summary>
        public void AttackTrigger(int num = 0)
        {
            //攻撃中じゃなけりゃあ
 _condition.ChangeState(CharacterStates.CharacterConditions.Moving);
                 motionNum = num;
                _movement.ChangeState(CharacterStates.MovementStates.Attack);
                //空中なら重力消す
                if (!_controller.State.IsGrounded)
                {
                    _controller.DefaultParameters.Gravity = 0f;
                }
            nowAttack = true;
           
            //  Debug.Log($"aaaaa{motionNum}");
        }


        /// <summary>
        ///  必要なアニメーターパラメーターがあれば、アニメーターパラメーターリストに追加します。
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {

            RegisterAnimatorParameter(_numParameterName, AnimatorControllerParameterType.Int, out _numAnimationParameter);
        }

        /// <summary>
        /// アビリティのサイクルが終了した時点。
        /// 現在のしゃがむ、這うの状態をアニメーターに送る。
        /// </summary>
        public override void UpdateAnimator()
        {
            //numパラメーターでアニメ再生の対象が変わる。
            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _numAnimationParameter, motionNum, _character._animatorParameters);
       //   Debug.Log($"あいいい{motionNum}");
            //コンボ系の連続再生はAIの方で定義
            //攻撃のクールタイム…というかこのへんはかわらんやろ
        }


    }
}

