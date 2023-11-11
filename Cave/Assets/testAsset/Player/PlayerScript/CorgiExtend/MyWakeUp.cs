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
        
        ///絶対に停止してほしくない
        float lastTime;

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



        float blowTime;
        [SerializeField]
        PlayerRoll pr;

        public enum StunnType
        {
            Falter = 1,
            Parried = 2,
            GuardBreake = 3,
            Down = 4,
            NDie = 6,//普通の死
            BlowDie = 7,//吹き飛び死
            notStunned = 0

        }


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
            //ここで起き上がりアニメーション、またはよろめきモーションが終了したか、そして終了したならスタンが解除される
            DoSomething();
        }

        /// <summary>
        /// アビリティサイクルの開始時に呼び出され、ここで入力の有無を確認します。
        /// </summary>
        protected override void HandleInput()
        {

            //ここで何ボタンが押されているかによって引数渡すか


            if (_inputManager.AvoidButton.State.CurrentState == MMInput.ButtonStates.ButtonDown && isPlayer)
            {
                if (nowType == 5)
                {
                    //   _inputManager.AvoidButton.State.ChangeState(MMInput.ButtonStates.Off);
                    if (Mathf.Sign(_horizontalInput) != Mathf.Sign(transform.root.localScale.x) && _horizontalInput != 0)
                    {
                        _character.Flip();
                    }
                       Recover(true);
                    
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
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Stunned))
            {
                // we do nothing and exit
                return;
            }

         //   transform.root.gameObject.layer = 15;
          //  _horizontalInput = 0;
            if (nowType == 1)
            {
                //アニメ終わったらスタン解除
                //それか4以外全部まとめてアニメ待つ処理でもいいな
                //今のタイプ入れるところでアニメの名前切り替えてもよかろ
                blowTime += _controller.DeltaTime;
                //0.1秒以上で地面についたら
                if (blowTime >= 0.15)
                {
                   _controller.SetHorizontalForce(0);
                    blowTime = 0;
                }

                    if (CheckEnd("Falter"))
                {
                    blowTime = 0;
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
            else if (nowType == 6)
            {

                if (CheckEnd("NDie"))
                {
                    Die();
                }
                else if (isPlayer)
                {
                    Die();
                }
            }
            else if (nowType == 4 || nowType == 7)
            {

                    blowTime += _controller.DeltaTime;
                //0.1秒以上で地面についたら
                if (blowTime >= 0.4 && _controller.State.IsGrounded)
                {
                    _health._blowNow = false;
                    blowTime = 0;
                    _controller.SetForce(Vector2.zero);
                    if (nowType == 4)
                    {
                        nowType = 5;
                        if (isPlayer)
                            transform.root.gameObject.layer = 7;
                    }
                    else
                    {

                        nowType = 8;

                    }
                }

            }
            else if (nowType == 5)
            {
                _controller.SetForce(Vector2.zero);
                if (CheckEnd("WakeUp"))
                    {
                        Recover();
                    }
                //怯みモーション出たら戻る
                else if (CheckEnd("Falter"))
                {
                    nowType = 4;
                }
            }
            else if (nowType == 8)
            {
                _controller.SetForce(Vector2.zero);
                if (CheckEnd("DDie"))
                {
                    Die();
                }
                //怯みモーション出たらやり直し
                else if (CheckEnd("Falter"))
                {
                    nowType = 7;
                }
                else if (isPlayer)
                {
                    Die();
                }
            }


        }

        /// <summary>
        /// 1がよろめき2がパリィ3がガードブレイク4が吹き飛び
        /// </summary>
        /// <param name="type"></param>
        public void StartStunn(StunnType type)
        {

                _movement.ChangeState(CharacterStates.MovementStates.Idle);
                _condition.ChangeState(CharacterStates.CharacterConditions.Stunned);
                _characterHorizontalMovement.SetHorizontalMove(0);
                
                _characterHorizontalMovement.ReadInput = false;
                if (type == StunnType.Falter)
                {
                    nowType = 1;
                   // _controller.SetForce(Vector2.zero);
                }
                else if (type == StunnType.Parried)
                {
                    nowType = 2;
                    _controller.SetForce(Vector2.zero);
                }
                else if (type == StunnType.GuardBreake)
                {
                    nowType = 3;
                    _controller.SetForce(Vector2.zero);
                }
                else if(type == StunnType.Down)
                {
                    _health._blowNow = true;
                    //現在吹き飛ばされてます
                    nowType = 4;
                       if(isPlayer)
                    transform.root.gameObject.layer = 15;
                }
                else if (type == StunnType.NDie)
                {

                _health._blowNow = true;
                _controller.SetForce(Vector2.zero);
                    nowType = 6;
                }
                else if (type == StunnType.BlowDie)
                {
                    //現在吹き飛ばされてます
                    nowType = 7;
                    transform.root.gameObject.layer = 15;
                }
            
        }




        public void Die()
        {
            _health.Die();
            if (isPlayer)
            {

                GManager.instance.HPReset();
                transform.root.gameObject.layer = 7;
                Recover();
            }
            else
            {
                Destroy(_character.gameObject);
                
            }

        }



        public int GetStunState()
        {
            return nowType;
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
            
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(Name))// || sAni.GetCurrentAnimatorStateInfo(0).IsName("OStand"))
            {   // ここに到達直後はnormalizedTimeが"Default"の経過時間を拾ってしまうので、Resultに遷移完了するまではreturnする。
                return false;
            }
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {   // 待機時間を作りたいならば、ここの値を大きくする。
                if (lastTime > 0 && lastTime == _animator.GetCurrentAnimatorStateInfo(0).normalizedTime)
                {
                    lastTime = 0;
                    return true;
                }
                else 
                {
                    lastTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                    return false;
                }
            }
            //AnimatorClipInfo[] clipInfo = sAni.GetCurrentAnimatorClipInfo(0);

            ////Debug.Log($"アニメ終了");
            lastTime = 0;
            return true;

            // return !(sAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
            //  (_currentStateName);
        }

        void Recover(bool cancel = false)
        {

            nowType = 0;
            
            //アーマーリセットしてな
            _characterHorizontalMovement.ReadInput = true;
            _health.ArmorReset();
            if (!cancel)
            { _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
             //  _characterHorizontalMovement.ReadInput = true;
            }
            else
            {

                _movement.ChangeState(CharacterStates.MovementStates.Nostate);
                _character.banIdle = true;
                _condition.ChangeState(CharacterStates.CharacterConditions.Moving);

                pr.ForceRoll();

                //Debug.Log($"tgr{pr == null}");

            }
            
        }

    }

}
