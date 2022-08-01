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
    public class EAttackCon : MyAbillityBase
    {
        /// このメソッドは、ヘルプボックスのテキストを表示するためにのみ使用されます。
        /// 能力のインスペクタの冒頭にある
        public override string HelpBoxText() { return "TODO_HELPBOX_TEXT."; }

        //   [Header("武器データ")]
        /// declare your parameters here
        ///WeaponHandle参考にして 


        // Animation parameters
        protected const string _todoParameterName = "AttackNow";
        protected int _todoAnimationParameter;
        protected const string _numParameterName = "AttackNumber";
        protected int _numAnimationParameter;



        int motionNum;


        /// <summary>
        ///　ここで、パラメータを初期化する必要があります。
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            // randomBool = false;
        }

        /// <summary>
        /// 1フレームごとに、しゃがんでいるかどうか、まだしゃがんでいるべきかをチェックします
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();

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
        protected virtual void AttackAct()
        {
            // if the ability is not permitted
            if (!AbilityPermitted
                // or if we're not in our normal stance
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                // or if we're grounded
                // or if we're gripping
                || (_movement.CurrentState != CharacterStates.MovementStates.Attack))
            {
                // we do nothing and exit
                return;
            }
            //終了判定マジでどうしよう
            //あとここパリィ判定差し込める?bool atCancelとかで
            //単純にアニメパラメーター変えればいいだけか。それで切り替わるよ
            //ステートで集合作ってAttackパラメーター真なら移動、内部ではfloatも考慮
            //statemachinebehaviorでやる


        }

        public void AttackTrigger(int num = 0)
        {
            //攻撃中じゃなけりゃあ
            if (_movement.CurrentState != CharacterStates.MovementStates.Attack)
            {
                motionNum = num;
                _movement.ChangeState(CharacterStates.MovementStates.Attack);
            }
        }


        /// <summary>
        ///  必要なアニメーターパラメーターがあれば、アニメーターパラメーターリストに追加します。
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_todoParameterName, AnimatorControllerParameterType.Bool, out _todoAnimationParameter);
            RegisterAnimatorParameter(_numParameterName, AnimatorControllerParameterType.Bool, out _numAnimationParameter);
        }

        /// <summary>
        /// アビリティのサイクルが終了した時点。
        /// 現在のしゃがむ、這うの状態をアニメーターに送る。
        /// </summary>
        public override void UpdateAnimator()
        {
            //numパラメーターでアニメ再生の対象が変わる。
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _todoAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Attack), _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _numAnimationParameter, (motionNum), _character._animatorParameters);

            //コンボ系の連続再生はAIの方で定義
            //攻撃のクールタイム…というかこのへんはかわらんやろ
        }

        bool CheckEnd(string Name)
        {

            if (!_character._animator.GetCurrentAnimatorStateInfo(0).IsName(Name))// || sAni.GetCurrentAnimatorStateInfo(0).IsName("OStand"))
            {   // ここに到達直後はnormalizedTimeが"Default"の経過時間を拾ってしまうので、Resultに遷移完了するまではreturnする。
                return false;
            }
            if (_character._animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {   // 待機時間を作りたいならば、ここの値を大きくする。
                return false;
            }
            //AnimatorClipInfo[] clipInfo = sAni.GetCurrentAnimatorClipInfo(0);

            ////Debug.Log($"アニメ終了");

            return true;

            // return !(sAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
            //  (_currentStateName);
        }

    }
}

