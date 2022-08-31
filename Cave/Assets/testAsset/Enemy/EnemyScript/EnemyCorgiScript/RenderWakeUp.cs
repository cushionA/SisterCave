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
    [AddComponentMenu("Corgi Engine/Character/Abilities/RenderWakeUp")]
    public class RenderWakeUp : MyAbillityBase
    {
        /// このメソッドは、ヘルプボックスのテキストを表示するためにのみ使用されます。
        /// 能力のインスペクタの冒頭にある
        public override string HelpBoxText() { return "RenderWakeUp"; }

        //   [Header("武器データ")]
        /// declare your parameters here
        ///WeaponHandle参考にして 

        [SerializeField]
        EnemyAIBase eb;
        [System.NonSerialized] public bool cameraEnabled = false;//一回カメラに映った
        [System.NonSerialized] public bool inActiveZone = false;
        // protected Rigidbody2D _controller;
        string avtiveTag = "ActiveZone";

        bool activeEnter;
        bool activeStay;
        //bool activeOut;
        bool isSleep;//休眠中フラグ
                     // Animator ani;




        /// <summary>
        ///　ここで、パラメータを初期化する必要があります。
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
           // eb = GetComponent<EnemyAIBase>();

        }

        void OnWillRenderObject()
        {
            //  if (Camera.current.tag == "MainCamera")レンダリングしてるカメラがメインカメラならってやつ。しかしおそらくURPのせいで動かない
            // {

            eb.cameraRendered = true;
            cameraEnabled = true;

            //  }
        }

        /// <summary>
        /// 1フレームごとに、しゃがんでいるかどうか、まだしゃがんでいるべきかをチェックします
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            ////Debug.log($"カメラないでーす{eb.cameraRendered}");
           Debug.Log($"カメラな{activeStay}");
            if (!cameraEnabled && !isSleep)
            {
                //カメラにうつってない
             //   eb.rb.Sleep();
                isSleep = true;
                //Debug.Log("あああｓで");
                _animator.enabled = false;
                eb.cameraRendered = false;
                return;
            }
            if (eb.cameraRendered && isSleep)
            {

               // _controller.WakeUp();
                //isSleep = false;
                isSleep = false;
                _animator.enabled = true;
            }

            if (cameraEnabled)
            {
                if (activeEnter || activeStay)
                {
                    //アクティブゾーンにいるなら
                    eb.cameraRendered = true;
                    //activeOut = false;
                }
                else
                {
                    //  eb.cameraRendered = false;
                    //  eb.rb.Sleep();
                    //  isSleep = true;
                    //  eb.sAni.enabled = false;
                    cameraEnabled = false;

                }
                activeStay = false;
                activeEnter = false;

            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == avtiveTag)
            {
                activeEnter = true;
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag == avtiveTag)
            {
                activeStay = true;
            }
        }


    }
}
