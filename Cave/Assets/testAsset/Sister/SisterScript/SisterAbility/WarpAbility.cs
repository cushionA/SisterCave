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
    /// シスターさんの自己移動ワープ
    /// isWarpでワープ開始してアニメーションイベントで遷移、終了
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/WarpAbility")]
    public class WarpAbility : MyAbillityBase
    {
        /// このメソッドは、ヘルプボックスのテキストを表示するためにのみ使用されます。
        /// 能力のインスペクタの冒頭にある
        public override string HelpBoxText() { return "warp_HELPBOX_TEXT."; }

        //   [Header("武器データ")]
        /// declare your parameters here
        ///WeaponHandle参考にして 


        // Animation parameters
        protected const string _warpParameterName = "_nowWarp";
        protected int _warpAnimationParameter;

        //----------------------------------------------------------------------------

        [SerializeField]
        BrainAbility _sister;

        //----------------------------------------------------------------------------

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
            WarpEnd();
        }





        /// <summary>
        /// 戦闘時に逃げるワープ
        /// </summary>
        public void WarpStart()
        {
            if (_sister.mp >= 5)
            {
                _sister.mp -= 5;
            }
            Debug.Log("ｓｓ");
            _movement.ChangeState(CharacterStates.MovementStates.Warp);
            _condition.ChangeState(CharacterStates.CharacterConditions.Moving);

        }

        //アニメーションイベント
        public void WarpEnd()
        {
            if (_movement.CurrentState != CharacterStates.MovementStates.Warp)
            {
                return;
            }

            if (CheckEnd("Warp"))
            {
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
            _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
            }

        }
        bool CheckEnd(string Name)
        {

            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(Name))// || sAni.GetCurrentAnimatorStateInfo(0).IsName("OStand"))
            {   // ここに到達直後はnormalizedTimeが"Default"の経過時間を拾ってしまうので、Resultに遷移完了するまではreturnする。
                return false;
            }
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {   // 待機時間を作りたいならば、ここの値を大きくする。
                return false;
            }
            //AnimatorClipInfo[] clipInfo = sAni.GetCurrentAnimatorClipInfo(0);

            ////Debug.Log($"アニメ終了");

            return true;

            // return !(sAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
            //  (_currentStateName);
        }
        /// <summary>
        ///  必要なアニメーターパラメーターがあれば、アニメーターパラメーターリストに追加します。
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_warpParameterName, AnimatorControllerParameterType.Bool, out _warpAnimationParameter);
        }

        /// <summary>
        /// これをオーバーライドすると、キャラクターのアニメーターにパラメータを送信することができます。
        /// これは、Characterクラスによって、Early、normal、Late process()の後に、1サイクルごとに1回呼び出される。
        /// </summary>
        public override void UpdateAnimator()
        {
            //クラウチングに気をつけろよ
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _warpAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Warp), _character._animatorParameters);
        }
    }
}
