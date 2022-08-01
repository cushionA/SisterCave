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
        protected const string _guardParameterName = "GuardNow";
        protected int _guardAnimationParameter;



        [HideInInspector]
        public bool guardHit;

        float hitTime;



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
                else if (_movement.CurrentState == CharacterStates.MovementStates.Guard)
                {
                    GuardEnd();
                }

            if (guardHit)
            {
                hitTime += _controller.DeltaTime;
                if (isPlayer)
                {
                        // もしプレイヤーにガードヒットしたならスタミナは回復しなくなる
                        //停止時間盾の受け値か種類で変えてもいいかも
                        //ガードヒット時動けないようにしないと
                    GManager.instance.isStUse = true;

                    //ガードでヒットした際は横移動とジャンプを封じる
                    //いやでも普通にガードヒットした状態にした方がよさそう
                    //スタンでいいか？
                    //スタンの参照検索してスタンにすればうごかないようにできるか調べる
                    _inputManager.SetHorizontalMovement(0);
                    _inputManager.JumpButton.State.ChangeState(MMInput.ButtonStates.Off);
                }
                if (hitTime >= 0.1)
                {
                    guardHit = false;
                    hitTime = 0;
                    GManager.instance.isStUse = false;
                }
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
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                // or if we're grounded
                // or if we're gripping
                || (_movement.CurrentState != CharacterStates.MovementStates.Gripping))
            {
                // we do nothing and exit
                return;
            }

            _movement.ChangeState(CharacterStates.MovementStates.Guard);
            if (isPlayer)
            {
                //ガード中に
                GManager.instance.isGuard = true;
            }

            //普通移動の速度でいいわ
            /*
            if (_characterHorizontalMovement != null)
            {
                _characterHorizontalMovement.MovementSpeed = RunSpeed;
            }*/
        }

        public void GuardEnd()
        {
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
            if (isPlayer)
            {
                //ガード終わらせる
                GManager.instance.isGuard = false;
            }
        } 

        /// <summary>
        ///  必要なアニメーターパラメーターがあれば、アニメーターパラメーターリストに追加します。
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
           // Debug.Log($"焼肉{_animator.runtimeAnimatorController.name}");
            RegisterAnimatorParameter(_guardParameterName, AnimatorControllerParameterType.Bool, out _guardAnimationParameter);
        }

        /// <summary>
        /// これをオーバーライドすると、キャラクターのアニメーターにパラメータを送信することができます。
        /// これは、Characterクラスによって、Early、normal、Late process()の後に、1サイクルごとに1回呼び出される。
        /// </summary>
        public override void UpdateAnimator()
        {
            //クラウチングに気をつけろよ
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _guardAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Guard), _character._animatorParameters);
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
