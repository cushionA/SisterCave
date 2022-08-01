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
    /// シスターさんのワープ
    /// isWarpでワープ開始してアニメーションイベントで遷移、終了
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/WarpAbility")]
    public class WarpAbillity : MyAbillityBase
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
        private LayerMask layerMask;

        bool isWarp;//ワープ中

        BrainAbility _sister;

        Vector2 warpPoint;

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
        }





        //ワープ専用の地面探知
        public float RayGroundCheck(Vector2 position)
        {

            //真下にレイを飛ばしましょう
            RaycastHit2D onHitRay = Physics2D.Raycast(position, Vector2.down, Mathf.Infinity, layerMask.value);

            //  ////Debug.log($"{onHitRay.transform.gameObject}");
            ////Debug.DrawRay(i_fromPosition,i_toTargetDir * SerchRadius);

            //Debug.Log($"当たったもの{onHitRay.transform.gameObject.name}");
            return onHitRay.point.y;
        }

        public void WarpEffect()
        {
            Transform gofire = _sister.firePosition;

            //Transform rotate = SManager.instance.useMagic.castEffect.LoadAssetAsync<Transform>().Result as Transform;

            Addressables.InstantiateAsync("WarpCircle", gofire.position, gofire.rotation);//.Result;//発生位置をPlayer
            GManager.instance.PlaySound("Warp", transform.position);
        }

        /// <summary>
        /// 戦闘時に逃げるワープ
        /// </summary>
        public void WarpStart(Vector2 point)
        {
            if (_sister.mp >= 5)
            {
                _sister.mp -= 5;
            }
            _movement.ChangeState(CharacterStates.MovementStates.Warp);
            warpPoint.Set(point.x,point.y);
        }
        //アニメーションイベント
        public void WarpAct()
        {
            transform.position = warpPoint;
            WarpEffect();
        }

        //アニメーションイベント
        public void WarpEnd()
        {
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
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
