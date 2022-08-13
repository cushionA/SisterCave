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
    [AddComponentMenu("Corgi Engine/Character/Abilities/MyWakeUp")]
    public class MyWakeUp : MyAbillityBase
    {
        /// このメソッドは、ヘルプボックスのテキストを表示するためにのみ使用されます。
        /// 能力のインスペクタの冒頭にある
        public override string HelpBoxText() { return "ダウンやよろめきを扱うためのアクション。プレイヤーは回避でダウンをキャンセルできる"; }

        //   [Header("武器データ")]
        /// declare your parameters here
        ///WeaponHandle参考にして 


        // Animation parameters


        //ダウンが今どの状態か、起き上がり中とか
        //吹っ飛び、倒れた、起き上がり中の三段階
       // protected const string _downStateParameterName = "BlowNow";
      //  protected int _downStateAnimationParameter;


        //よろめき
        protected const string _stunTypeParameterName = "StunState";
        protected int _stunTypeAnimationParameter;


        /// <summary>
        /// 1よろめき2パリィ3ガードブレイク、4ダウン
        /// </summary>
        int nowType;

        /// <summary>
        /// 現在吹っ飛ばされてるかどうか
        /// </summary>
        bool blowNow;

        float blowTime;

        PlayerRoll pr;

        public enum StunnType
        {
            Falter = 1,
            Parried = 2,
            GuardBreake = 3,
            Down = 4,
            notStunned = 0

        }


        /// <summary>
        ///　ここで、パラメータを初期化する必要があります。
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            // randomBool = false;
            pr = _controller.gameObject.GetComponent<PlayerRoll>();
        }

        /// <summary>
        /// 1フレームごとに、しゃがんでいるかどうか、まだしゃがんでいるべきかをチェックします
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            //ここで起き上がりアニメーション、またはよろめきモーションが終了したか、そして終了したならスタンが解除される
            DoSomething();
        }

        /// <summary>
        /// アビリティサイクルの開始時に呼び出され、ここで入力の有無を確認します。
        /// </summary>
        protected override void HandleInput()
        {

            //ここで何ボタンが押されているかによって引数渡すか

            // here as an example we check if we're pressing down
            // on our main stick/direction pad/keyboard
            if (_inputManager.AvoidButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {
                if (nowType == 4 && !blowNow)
                {
                    Recover();
                    pr.actRoll = true;
                }
            }
        }




        /// <summary>
        /// 押し込んでいる場合は、いくつかの条件を満たしているかどうかをチェックして、アクションを実行できるかどうかを確認します。
        /// </summary>
        protected virtual void DoSomething()
        {
            // if the ability is not permitted
            if (!AbilityPermitted
                // スタンしてないなら帰れ
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Stunned)

                // or if we're grounded
                || (!_controller.State.IsGrounded))
            {
                // we do nothing and exit
                return;
            }

            if(nowType == 1)
            {
                //アニメ終わったらスタン解除
                //それか4以外全部まとめてアニメ松処理でもいいな
                //今のタイプ入れるところでアニメの名前切り替えてもよかろ

                if (CheckEnd("Falter"))
                {
                    Recover();
                }

            }
            else if (nowType == 2)
            {
                if (CheckEnd("Parried"))
                {
                    Recover();
                }
            }
            else if(nowType == 3)
            {
                if (CheckEnd("GBreake"))
                {
                    Recover();
                }
            }
            else if (nowType == 4)
            {
                if (blowNow)
                {
                    blowTime += _controller.DeltaTime;
                    //0.1秒以上で地面についたら
                    if (blowTime >= 0.1 && _controller.State.IsGrounded)
                    {
                        blowNow = false;
                        blowTime = 0;
                    }
                }
                else
                {
                    //こっから起き上がり処理ですね
                    //起き上がりアニメが終わったらスタン解除
                    //ローリングでスタン解除も可能に
                    //Wakeupパラメーターオンでダウン（たたきつけられ）アニメ、そして起き上がりに派生するようにする。

                    if (CheckEnd("Wakeup"))
                    {
                        Recover();
                    }

                }
            }


        }

        /// <summary>
        /// 1がよろめき2がパリィ3がガードブレイク4が吹き飛び
        /// </summary>
        /// <param name="type"></param>
        public void StartStunn(StunnType type)
        {
            if (_character.CharacterHealth.CurrentHealth > 0 && _condition.CurrentState != CharacterStates.CharacterConditions.Stunned)
            {
                _condition.ChangeState(CharacterStates.CharacterConditions.Stunned);
                _characterHorizontalMovement.MovementForbidden = true;
                if (type == StunnType.Falter)
                {
                    nowType = 1;
                }
                else if (type == StunnType.Parried)
                {
                    nowType = 2;
                }
                else if (type == StunnType.GuardBreake)
                {
                    nowType = 3;
                }
                else
                {
                    //現在吹き飛ばされてます
                    nowType = 4;
                    blowNow = true;
                }
            }
        }









        /// <summary>
        ///  必要なアニメーターパラメーターがあれば、アニメーターパラメーターリストに追加します。
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {

           // RegisterAnimatorParameter(_downStateParameterName, AnimatorControllerParameterType.Bool, out _downStateAnimationParameter);
            RegisterAnimatorParameter(_stunTypeParameterName, AnimatorControllerParameterType.Int, out _stunTypeAnimationParameter);
        }

        /// <summary>
        /// これをオーバーライドすると、キャラクターのアニメーターにパラメータを送信することができます。
        /// これは、Characterクラスによって、Early、normal、Late process()の後に、1サイクルごとに1回呼び出される。
        /// </summary>
        public override void UpdateAnimator()
        {

            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _stunTypeAnimationParameter, (nowType), _character._animatorParameters);
        //    if()

 //           MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _downStateAnimationParameter, (), _character._animatorParameters);

            //ダウンのアニメーター 
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

        void Recover()
        {
            _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
            nowType = 0;
            _characterHorizontalMovement.MovementForbidden = false;
        }

    }

}
