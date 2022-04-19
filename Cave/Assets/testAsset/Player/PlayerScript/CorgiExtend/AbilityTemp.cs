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
    [AddComponentMenu("Corgi Engine/Character/Abilities/TODO_REPLACE_WITH_ABILITY_NAME")]
    public class AbillityTemp : CharacterAbility
    {
        /// このメソッドは、ヘルプボックスのテキストを表示するためにのみ使用されます。
        /// 能力のインスペクタの冒頭にある
        public override string HelpBoxText() { return "TODO_HELPBOX_TEXT."; }

        //   [Header("武器データ")]
        /// declare your parameters here
        ///WeaponHandle参考にして 


        // Animation parameters
        protected const string _todoParameterName = "TODO";
        protected int _todoAnimationParameter;

        protected RewiredCorgiEngineInputManager ReInput;


        /// <summary>
        ///　ここで、パラメータを初期化する必要があります。
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            // randomBool = false;
            ReInput = (RewiredCorgiEngineInputManager)_inputManager;
        }

        /// <summary>
        /// 1フレームごとに、しゃがんでいるかどうか、まだしゃがんでいるべきかをチェックします
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
        }

        /// <summary>
        /// アビリティサイクルの開始時に呼び出され、ここで入力の有無を確認します。
        /// </summary>
        protected override void HandleInput()
        {
            //ここで何ボタンが押されているかによって引数渡すか

            // here as an example we check if we're pressing down
            // on our main stick/direction pad/keyboard
            if (ReInput.PrimaryMovement.y < -ReInput.Threshold.y)
            {
                DoSomething();
            }
        }

        /// <summary>
        /// 押し込んでいる場合は、いくつかの条件を満たしているかどうかをチェックして、アクションを実行できるかどうかを確認します。
        /// </summary>
        protected virtual void DoSomething()
        {
            // if the ability is not permitted
            if (!AbilityPermitted
                // or if we're not in our normal stance
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                // or if we're grounded
                || (!_controller.State.IsGrounded)
                // or if we're gripping
                || (_movement.CurrentState == CharacterStates.MovementStates.Gripping))
            {
                // we do nothing and exit
                return;
            }

            // if we're still here, we display a text log in the console
            MMDebug.DebugLogTime("We're doing something yay!");
        }

        /// <summary>
        ///  必要なアニメーターパラメーターがあれば、アニメーターパラメーターリストに追加します。
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_todoParameterName, AnimatorControllerParameterType.Bool, out _todoAnimationParameter);
        }

        /// <summary>
        /// これをオーバーライドすると、キャラクターのアニメーターにパラメータを送信することができます。
        /// これは、Characterクラスによって、Early、normal、Late process()の後に、1サイクルごとに1回呼び出される。
        /// </summary>
        public override void UpdateAnimator()
        {
            //クラウチングに気をつけろよ
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _todoAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Crouching), _character._animatorParameters);
        }
    }
}
