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
    /// コンビネーションのクラス
    /// コンビネーションにはタイプがいくつかある。
    /// 標的がいるタイプ、ボタン長押ししてる間いろいろ選べるやつ
    /// ボタン長押しが適応されるやつは長押しフラグをコンビネーション側につける
    /// するとFire側にコンビネーション不可能フラグが立つ
    /// こちらでは実行できないストッパーをかけてもう一回ボタンを押したら始動するようにする
    /// コンビネーションや攻撃を凍結するFire側の仕掛けが必要
    /// 最後にStopフラグを解除してFire側に処理を戻す
    /// Combinationのアニメパラメーターの数字によってアニメーションが遷移する
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/CombinationAbility")]
    public class CombinationAbility : MyAbillityBase
    {
        /// このメソッドは、ヘルプボックスのテキストを表示するためにのみ使用されます。
        /// 能力のインスペクタの冒頭にある
        public override string HelpBoxText() { return "コンビネーションに関する能力"; }

        //   [Header("武器データ")]
        /// declare your parameters here
        ///WeaponHandle参考にして 


        // Animation parameters
        protected const string _combiParameterName = "CombinationNow";
        protected int _combiAnimationParameter;

        protected RewiredCorgiEngineInputManager ReInput;


        /// <summary>
        /// コンビネーションの段階
        /// 0の時は何もしない、1でモーション開始、2で発動
        /// </summary>
        enum CombinationState
        {
            Idle = 0,
            Act = 1,
            End = 2
        }

        CombinationState _combState;

        /// <summary>
        /// チェイン回数記録。
        /// チェインの数で処理変わるものがあれば
        /// </summary>
        int _chainNumber;

        float castTime;

        BrainAbility sb;
        FireAbility sf;

        /// <summary>
        /// 真なら必要な入力がもたらされたということ
        /// つまりボタンが離されたってこと
        /// </summary>
        bool needInput;

        /// <summary>
        ///　ここで、パラメータを初期化する必要があります。
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            // randomBool = false;
            ReInput = (RewiredCorgiEngineInputManager)_inputManager;
            sb = GetComponent<BrainAbility>();
            sf = GetComponent<FireAbility>();
        }

        /// <summary>
        /// 1フレームごとに、しゃがんでいるかどうか、まだしゃがんでいるべきかをチェックします
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
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
            if (_combState == CombinationState.Act)
            {
                //ボタンから手が離れているなら真に
                needInput =  (ReInput.CombinationButton.State.CurrentState == MMInput.ButtonStates.Off);
                
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

            CombinationController();
        }

        /// <summary>
        /// これがEndによってトリガーされるメソッド
        /// Endになったら発射モーションに移ってその終わりのアニメイベントで呼ばれる。
        /// chainナンバーはここで活用する。
        /// あるいはアニメーションパラメーターにしてチェインで詠唱モーション変えるとかしてもいいかも
        /// </summary>
        public void CombinationDo()
        {
            //まず使用するコンビネーションがどれか確認

            //ワープ
            if (sf.status.equipCombination._sortNumber == 1)
            {
                Vector2 warpPosi = SManager.instance.target.transform.position;


                float exY = sb.RayGroundCheck(warpPosi);

                //地表から十五以下の距離なら設置
                warpPosi.y = warpPosi.y - exY <= 15 ? exY : warpPosi.y;

                if (SManager.instance.target.transform.localScale.x > 0)
                {
                    warpPosi.Set(warpPosi.x - 10, warpPosi.y);

                    GManager.instance.Player.transform.position = warpPosi;
                    if (GManager.instance.Player.transform.localScale.x < 0)
                    {
                        GManager.instance.pm.Flip();
                    }
                }
                else
                {
                    warpPosi.Set(warpPosi.x + 10, warpPosi.y);

                    GManager.instance.Player.transform.position = warpPosi;
                    GManager.instance.pm.Flip();
                    if (GManager.instance.Player.transform.localScale.x > 0)
                    {
                        GManager.instance.pm.Flip();
                    }
                }
                Transform gofire = GManager.instance.PlayerEffector.transform;
                //gofire.localScale *= 0.8f;
                gofire.localScale = GManager.instance.Player.transform.localScale;
                Addressables.InstantiateAsync("WarpCircle", gofire);//.Result;//発生位置をPlayer
                GManager.instance.PlaySound("Warp", gofire.position);
                //return 1;
            }
            // return 0;

            //最後に初期化
            _combState = CombinationState.Idle;
            needInput = false;
            sf.isStop = false;
        }

        public void CombinationStart(int chainNumber)
        {
            _combState = CombinationState.Act;
            //これでコンビネーションの詠唱モーションが始まる。
            //Endになるまで始動モーションに入らない
            _chainNumber = chainNumber;
            //Stopはモーション終わりの処理の後解除する
            sf.isStop = true;
        }


        void CombinationController()
        {
            //Act中にEndを導く処理
            if (_combState == CombinationState.Act)
            {
                //Endにする処理をいくつか入れる
                //タイプで分けて、詠唱時間の時間経過や即座に始動モーションに入るもの、あとはボタンを離したとき始動モーションに入ったり
                if (sf.status.equipCombination._combiType == SisterCombination.ActType.soon)
                {
                    _combState = CombinationState.End;
                }
                else if (sf.status.equipCombination._combiType == SisterCombination.ActType.cast)
                {
                    castTime += _controller.DeltaTime;
                    //音鳴らしたりも頼むわ
                    if(castTime >= sb.status.equipCombination.castTime)
                    {
                        _combState = CombinationState.End;
                        castTime = 0;
                    }
                }
                else if (sf.status.equipCombination._combiType == SisterCombination.ActType.longPress)
                {
                    //長押しでなくボタンが押されてない状態ならでやる？
                    if (needInput)
                    {
                        _combState = CombinationState.End;
                    }
                }
                else if (sf.status.equipCombination._combiType == SisterCombination.ActType.castAndPress)
                {
                    //詠唱が終わった時ボタンが押されていない状態なら
                    castTime += _controller.DeltaTime;
                    bool nowOk = false;
                    if (castTime >= sb.status.equipCombination.castTime)
                    {
                        nowOk = true;
                    }
                    if (nowOk && needInput)
                    {
                        _combState = CombinationState.End;
                        castTime = 0;
                    }
                }
            }
        }


        /// <summary>
        ///  必要なアニメーターパラメーターがあれば、アニメーターパラメーターリストに追加します。
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_combiParameterName, AnimatorControllerParameterType.Int, out _combiAnimationParameter);
        }

        /// <summary>
        /// これをオーバーライドすると、キャラクターのアニメーターにパラメータを送信することができます。
        /// これは、Characterクラスによって、Early、normal、Late process()の後に、1サイクルごとに1回呼び出される。
        /// </summary>
        public override void UpdateAnimator()
        {
            //クラウチングに気をつけろよ
            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _combiAnimationParameter, (int)_combState, _character._animatorParameters);
        }
    }
}
