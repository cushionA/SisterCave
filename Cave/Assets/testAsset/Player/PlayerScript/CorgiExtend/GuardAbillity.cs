using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using UnityEngine.AddressableAssets;
using Rewired.Integration.CorgiEngine;
using DarkTonic.MasterAudio;
using static DefenseData;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// TODO_DESCRIPTION
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/GuardAbillity")]
    public class GuardAbillity : MyAbillityBase
    {
        /// このメソッドは、ヘルプボックスのテキストを表示するためにのみ使用されます。
        /// 能力のインスペクタの冒頭にある
        public override string HelpBoxText() { return "ガード状態に切り替えるよ"; }

        //   [Header("武器データ")]
        /// declare your parameters here
        ///WeaponHandle参考にして 
        
        //ガード歩きとかの方法考えとけよ

        // Animation parameters
        protected const string _guardParameterName = "GuardState";
        protected int _guardAnimationParameter;



        [HideInInspector]
        public bool guardHit;

        float hitTime;
        //0ガードしてない、１ガード、２移動ガード
        int state;


        /// <summary>
        ///　ここで、パラメータを初期化する必要があります。
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            // randomBool = false;
           // _inputManager = (RewiredCorgiEngineInputManager)_inputManager;
        }

        /// <summary>
        /// 1フレームごとに、しゃがんでいるかどうか、まだしゃがんでいるべきかをチェックします
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            DetermineState();
            HitCheck();
        }

        /// <summary>
        /// アビリティサイクルの開始時に呼び出され、ここで入力の有無を確認します。
        /// </summary>
        protected override void HandleInput()
        {
            //ここで何ボタンが押されているかによって引数渡すか

            // here as an example we check if we're pressing down
            // on our main stick/direction pad/keyboard
            // Debug.Log($"guibu{_horizontalInput}");
            if (_controller.State.IsGrounded)
                {
                    if ((_inputManager.GuardButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed || guardHit) && GManager.instance.isEnable)
                    {
                        ActGuard();
                    }
                    //ボタンを離すかスタミナ使用不可なら
                    else if(_inputManager.GuardButton.State.CurrentState == MMInput.ButtonStates.ButtonUp || !GManager.instance.isEnable)
                    {
                        GuardEnd();
                    } 
                }
                else if(_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove)
                {
                    GuardEnd();
                }



        }

        /// <summary>
        /// 押し込んでいる場合は、いくつかの条件を満たしているかどうかをチェックして、アクションを実行できるかどうかを確認します。
        /// </summary>
        public virtual void ActGuard()
        {
            // if the ability is not permitted
            if (!AbilityPermitted
                // or if we're not in our normal stance
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
            {
                // we do nothing and exit
                GuardEnd();
                return;
            }

            _movement.ChangeState(CharacterStates.MovementStates.Guard);

            //ガード状態に
            _health.HealthStateChange(false, DefState.ガード中);

            //普通移動の速度でいいわ
            /*
            if (_characterHorizontalMovement != null)
            {
                _characterHorizontalMovement.MovementSpeed = RunSpeed;
            }*/
        }

        /// <summary>
        /// しゃがんでいる状態から這うように移動するか、その逆を行うかどうかを毎フレーム確認する。
        /// </summary>
        protected virtual void DetermineState()
        {
            float threshold = (_inputManager != null) ? _inputManager.Threshold.x : 0f;

               state = 0;
            if ((_movement.CurrentState == CharacterStates.MovementStates.Guard) || (_movement.CurrentState == CharacterStates.MovementStates.GuardMove))
            {
                
                if ((Mathf.Abs(_horizontalInput) > threshold) && !guardHit)
                {
                    _movement.ChangeState(CharacterStates.MovementStates.GuardMove);
                    state = 2;
                }
                else
                {
                    _movement.ChangeState(CharacterStates.MovementStates.Guard);
                    state = 1;
                }
            }

        }

        //ガードヒット時はチェック切れないように
        //ガードブレイクの処理まで入れるか
        void HitCheck()
        {
            if (guardHit && _condition.CurrentState != CharacterStates.CharacterConditions.Stunned)
            {
                _condition.ChangeState(CharacterStates.CharacterConditions.Moving);
                _movement.ChangeState(CharacterStates.MovementStates.Guard);


                hitTime += _controller.DeltaTime;
                if (isPlayer)
                {
                    // もしプレイヤーにガードヒットしたならスタミナは回復しなくなる
                    //停止時間盾の受け値か種類で変えてもいいかも
                    //ガードヒット時動けないようにしないと
                    GManager.instance.isStUse = true;
                    //ガード中に
                    GManager.instance.isGuard = true;
                    //ガードでヒットした際は横移動とジャンプを封じる
                    //いやでも普通にガードヒットした状態にした方がよさそう
                    //スタンでいいか？
                    //スタンの参照検索してスタンにすればうごかないようにできるか調べる
                    _characterHorizontalMovement.MovementForbidden = true;
                    _inputManager.JumpButton.State.ChangeState(MMInput.ButtonStates.Off);
                    _inputManager.AvoidButton.State.ChangeState(MMInput.ButtonStates.Off);
                   // _inputManager.JumpButton.State.ChangeState(MMInput.ButtonStates.Off);
                }
                else
                {
                    _characterHorizontalMovement.MovementForbidden = true;
                }
                //ヘルスの方で解除してもいいね
                if (hitTime >= 0.1)
                {
                    _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
                    _characterHorizontalMovement.MovementForbidden = false;
                    guardHit = false;
                    hitTime = 0;
                    GManager.instance.isStUse = false;
                }
            }
        }

         public void GuardEnd()
        {
            if (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove)
            {
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
                //ガード終了
                _health.HealthStateChange(true,DefState.ガード中);
            }
        } 

        /// <summary>
        ///  必要なアニメーターパラメーターがあれば、アニメーターパラメーターリストに追加します。
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
           // Debug.Log($"焼肉{_animator.runtimeAnimatorController.name}");
            RegisterAnimatorParameter(_guardParameterName, AnimatorControllerParameterType.Int, out _guardAnimationParameter);
        }

        /// <summary>
        /// これをオーバーライドすると、キャラクターのアニメーターにパラメータを送信することができます。
        /// これは、Characterクラスによって、Early、normal、Late process()の後に、1サイクルごとに1回呼び出される。
        /// </summary>
        public override void UpdateAnimator()
        {

            //クラウチングに気をつけろよ
            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _guardAnimationParameter, (state), _character._animatorParameters);
        }

        public void GuardHit()
        {
            if (!guardHit)
            {
                guardHit = true;
                
            }
            else
            {
                guardHit = true;
                hitTime = 0;
            }
        }

    }
}
